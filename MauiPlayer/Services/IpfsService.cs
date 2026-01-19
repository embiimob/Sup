using MauiPlayer.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MauiPlayer.Services;

/// <summary>
/// Service for IPFS integration with retry mechanism
/// </summary>
public class IpfsService
{
    private readonly DataStorageService _storage;
    private readonly string _ipfsExecutable;
    private readonly string _cacheDirectory;

    public IpfsService(DataStorageService storage)
    {
        _storage = storage;
        _ipfsExecutable = Path.Combine(AppContext.BaseDirectory, "ipfs", "ipfs.exe");
        _cacheDirectory = Path.Combine(FileSystem.AppDataDirectory, "ipfs_cache");
        
        // Ensure cache directory exists
        if (!Directory.Exists(_cacheDirectory))
        {
            Directory.CreateDirectory(_cacheDirectory);
        }
    }

    /// <summary>
    /// Download file from IPFS with retry mechanism
    /// </summary>
    public async Task<IndexedFile?> DownloadFileAsync(string ipfsHash, string? transactionId = null, string? fromAddress = null, int maxRetries = 3)
    {
        // Check if already downloaded
        var existing = await _storage.GetFileByHashAsync(ipfsHash);
        if (existing != null && File.Exists(existing.LocalPath))
        {
            return existing;
        }

        var settings = await _storage.GetSettingsAsync();
        
        for (int retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                // Try local IPFS daemon first
                var localSuccess = await DownloadViaLocalDaemonAsync(ipfsHash);
                if (localSuccess)
                {
                    return await IndexDownloadedFileAsync(ipfsHash, transactionId, fromAddress);
                }

                // Fallback to gateway
                if (!string.IsNullOrEmpty(settings.IpfsGateway))
                {
                    var gatewaySuccess = await DownloadViaGatewayAsync(ipfsHash, settings.IpfsGateway);
                    if (gatewaySuccess)
                    {
                        return await IndexDownloadedFileAsync(ipfsHash, transactionId, fromAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"IPFS download attempt {retry + 1} failed: {ex.Message}");
                if (retry < maxRetries - 1)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retry))); // Exponential backoff
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Download via local IPFS daemon
    /// </summary>
    private async Task<bool> DownloadViaLocalDaemonAsync(string ipfsHash)
    {
        if (!File.Exists(_ipfsExecutable))
        {
            return false;
        }

        try
        {
            var outputPath = Path.Combine(_cacheDirectory, ipfsHash);
            
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ipfsExecutable,
                    Arguments = $"get {ipfsHash} -o \"{outputPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            await process.WaitForExitAsync(cts.Token);

            return process.ExitCode == 0 && (File.Exists(outputPath) || Directory.Exists(outputPath));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Local IPFS daemon error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Download via IPFS gateway
    /// </summary>
    private async Task<bool> DownloadViaGatewayAsync(string ipfsHash, string gateway)
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);

            var url = $"{gateway.TrimEnd('/')}/{ipfsHash}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var outputPath = Path.Combine(_cacheDirectory, ipfsHash);
                var content = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(outputPath, content);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Gateway download error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Index downloaded file
    /// </summary>
    private async Task<IndexedFile> IndexDownloadedFileAsync(string ipfsHash, string? transactionId, string? fromAddress)
    {
        var localPath = Path.Combine(_cacheDirectory, ipfsHash);
        var fileInfo = new FileInfo(localPath);

        var fileType = DetermineFileType(localPath);
        var fileName = Path.GetFileName(localPath);

        var indexedFile = new IndexedFile
        {
            IpfsHash = ipfsHash,
            FileName = fileName,
            LocalPath = localPath,
            FileType = fileType,
            FileSize = fileInfo.Exists ? fileInfo.Length : 0,
            TransactionId = transactionId,
            FromAddress = fromAddress,
            IsBlocked = false
        };

        // Check if address is blocked
        if (!string.IsNullOrEmpty(fromAddress))
        {
            indexedFile.IsBlocked = await _storage.IsAddressBlockedAsync(fromAddress);
        }

        await _storage.SaveFileAsync(indexedFile);
        return indexedFile;
    }

    /// <summary>
    /// Determine file type from extension or content
    /// </summary>
    private string DetermineFileType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        
        return extension switch
        {
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" => "image",
            ".mp3" or ".wav" or ".ogg" or ".flac" or ".m4a" => "audio",
            ".mp4" or ".avi" or ".mkv" or ".mov" or ".webm" => "video",
            _ => "other"
        };
    }

    /// <summary>
    /// Parse IPFS hash from URN or URL
    /// </summary>
    public static string? ParseIpfsHash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        // Handle various IPFS formats
        // ipfs://QmHash
        // IPFS:QmHash
        // QmHash
        // https://ipfs.io/ipfs/QmHash

        var patterns = new[]
        {
            @"ipfs://([A-Za-z0-9]+)",
            @"IPFS:([A-Za-z0-9]+)",
            @"/ipfs/([A-Za-z0-9]+)",
            @"^(Qm[A-Za-z0-9]+)",
            @"^(bafybei[A-Za-z0-9]+)"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }
        }

        return null;
    }

    /// <summary>
    /// Delete file from cache
    /// </summary>
    public async Task<bool> DeleteFileAsync(string ipfsHash)
    {
        try
        {
            var localPath = Path.Combine(_cacheDirectory, ipfsHash);
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }
            else if (Directory.Exists(localPath))
            {
                Directory.Delete(localPath, true);
            }

            await _storage.DeleteFileAsync(ipfsHash);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Delete file error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Clear all cached files
    /// </summary>
    public async Task<bool> ClearCacheAsync()
    {
        try
        {
            if (Directory.Exists(_cacheDirectory))
            {
                Directory.Delete(_cacheDirectory, true);
                Directory.CreateDirectory(_cacheDirectory);
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Clear cache error: {ex.Message}");
            return false;
        }
    }
}
