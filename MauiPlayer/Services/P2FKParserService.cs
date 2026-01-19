using MauiPlayer.Models;
using SUP.P2FK;
using System.Diagnostics;

namespace MauiPlayer.Services;

/// <summary>
/// Service for parsing P2FK messages from transactions
/// </summary>
public class P2FKParserService
{
    private readonly DataStorageService _storage;
    private readonly IpfsService _ipfsService;
    private readonly BitcoinService _bitcoinService;

    public P2FKParserService(DataStorageService storage, IpfsService ipfsService, BitcoinService bitcoinService)
    {
        _storage = storage;
        _ipfsService = ipfsService;
        _bitcoinService = bitcoinService;
    }

    /// <summary>
    /// Parse and index P2FK message from transaction
    /// </summary>
    public async Task<IndexedMessage?> ParseAndIndexTransactionAsync(string transactionId)
    {
        try
        {
            var (username, password, url, versionByte) = await _bitcoinService.GetRpcSettingsAsync();

            // Get Root object from transaction
            var root = Root.GetRootByTransactionId(transactionId, username, password, url, versionByte);
            
            if (root == null || root.Message == null || root.Message.Length == 0)
            {
                return null;
            }

            // Check if sender is blocked
            if (!string.IsNullOrEmpty(root.SignedBy) && await _storage.IsAddressBlockedAsync(root.SignedBy))
            {
                Debug.WriteLine($"Skipping message from blocked address: {root.SignedBy}");
                return null;
            }

            // Extract recipient address from keywords
            var toAddress = ExtractRecipientAddress(root);
            
            // Combine message parts
            var messageText = string.Join(" ", root.Message);

            // Determine message type and IPFS content
            var (messageType, ipfsHash) = DetermineMessageType(root, messageText);

            var indexedMessage = new IndexedMessage
            {
                TransactionId = transactionId,
                FromAddress = root.SignedBy ?? string.Empty,
                ToAddress = toAddress ?? string.Empty,
                Message = messageText,
                MessageType = messageType,
                IpfsHash = ipfsHash,
                BlockDate = root.BlockDate,
                BlockHeight = root.BlockHeight,
                Confirmations = root.Confirmations,
                IsBlocked = false
            };

            // Download IPFS content if present
            if (!string.IsNullOrEmpty(ipfsHash))
            {
                var settings = await _storage.GetSettingsAsync();
                
                // Check auto-download settings
                if (settings.AutoDownloadIpfs)
                {
                    try
                    {
                        var file = await _ipfsService.DownloadFileAsync(ipfsHash, transactionId, root.SignedBy);
                        if (file != null)
                        {
                            indexedMessage.LocalFilePath = file.LocalPath;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"IPFS download failed for {ipfsHash}: {ex.Message}");
                    }
                }
            }

            // Save to database
            await _storage.SaveMessageAsync(indexedMessage);

            return indexedMessage;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Parse transaction error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Parse multiple transactions in batch
    /// </summary>
    public async Task<List<IndexedMessage>> ParseTransactionBatchAsync(List<string> transactionIds)
    {
        var messages = new List<IndexedMessage>();
        
        foreach (var txid in transactionIds)
        {
            var message = await ParseAndIndexTransactionAsync(txid);
            if (message != null)
            {
                messages.Add(message);
            }
        }

        return messages;
    }

    /// <summary>
    /// Parse messages by address
    /// </summary>
    public async Task<List<IndexedMessage>> ParseMessagesByAddressAsync(string address, int skip = 0, int qty = 10)
    {
        try
        {
            var (username, password, url, versionByte) = await _bitcoinService.GetRpcSettingsAsync();

            // Use OBJ.GetPublicMessagesByAddress to get messages
            var messageObjects = OBJ.GetPublicMessagesByAddress(address, username, password, url, versionByte, skip, qty);
            
            var indexedMessages = new List<IndexedMessage>();

            foreach (var msgObj in messageObjects)
            {
                // Check if already indexed
                var existing = await _storage.SearchMessagesByAddressAsync(msgObj.FromAddress);
                if (existing.Any(e => e.TransactionId == msgObj.TransactionId))
                {
                    continue;
                }

                // Check if sender is blocked
                if (await _storage.IsAddressBlockedAsync(msgObj.FromAddress))
                {
                    continue;
                }

                var indexedMessage = new IndexedMessage
                {
                    TransactionId = msgObj.TransactionId,
                    FromAddress = msgObj.FromAddress,
                    ToAddress = msgObj.ToAddress,
                    Message = msgObj.Message,
                    MessageType = "Text", // Will be updated if IPFS content found
                    BlockDate = msgObj.BlockDate,
                    IsBlocked = false
                };

                // Check for IPFS content in message
                var ipfsHash = IpfsService.ParseIpfsHash(msgObj.Message);
                if (!string.IsNullOrEmpty(ipfsHash))
                {
                    indexedMessage.IpfsHash = ipfsHash;
                    
                    var settings = await _storage.GetSettingsAsync();
                    if (settings.AutoDownloadIpfs)
                    {
                        try
                        {
                            var file = await _ipfsService.DownloadFileAsync(ipfsHash, msgObj.TransactionId, msgObj.FromAddress);
                            if (file != null)
                            {
                                indexedMessage.LocalFilePath = file.LocalPath;
                                indexedMessage.MessageType = file.FileType;
                            }
                        }
                        catch { }
                    }
                }

                await _storage.SaveMessageAsync(indexedMessage);
                indexedMessages.Add(indexedMessage);
            }

            return indexedMessages;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Parse messages by address error: {ex.Message}");
            return new List<IndexedMessage>();
        }
    }

    /// <summary>
    /// Extract recipient address from Root keywords
    /// </summary>
    private string? ExtractRecipientAddress(Root root)
    {
        if (root.Keyword == null || root.Keyword.Count == 0)
            return null;

        // Get second-to-last or last keyword as recipient
        var keywords = root.Keyword.Values.ToList();
        if (keywords.Count >= 2)
        {
            return keywords[keywords.Count - 2];
        }
        else if (keywords.Count >= 1)
        {
            return keywords[keywords.Count - 1];
        }

        return null;
    }

    /// <summary>
    /// Determine message type and extract IPFS hash
    /// </summary>
    private (string messageType, string? ipfsHash) DetermineMessageType(Root root, string messageText)
    {
        // Check for IPFS content in message
        var ipfsHash = IpfsService.ParseIpfsHash(messageText);
        if (!string.IsNullOrEmpty(ipfsHash))
        {
            return ("ipfs", ipfsHash);
        }

        // Check for file attachments in Root.File
        if (root.File != null && root.File.Count > 0)
        {
            var firstFile = root.File.Keys.FirstOrDefault();
            if (!string.IsNullOrEmpty(firstFile))
            {
                // Check if it's an IPFS reference
                var fileIpfsHash = IpfsService.ParseIpfsHash(firstFile);
                if (!string.IsNullOrEmpty(fileIpfsHash))
                {
                    return ("ipfs", fileIpfsHash);
                }

                // Determine type by extension
                var ext = Path.GetExtension(firstFile).ToLowerInvariant();
                var type = ext switch
                {
                    ".jpg" or ".jpeg" or ".png" or ".gif" => "image",
                    ".mp3" or ".wav" or ".ogg" => "audio",
                    ".mp4" or ".avi" or ".mkv" => "video",
                    _ => "file"
                };
                return (type, null);
            }
        }

        return ("text", null);
    }
}
