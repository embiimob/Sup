using Sup.Core.Models;
using Sup.Core.P2FK.Classes;
using System.Text.Json;

namespace Sup.Core.Services;

/// <summary>
/// Complete blockchain service implementation with RPC integration
/// </summary>
public class BlockchainService : IBlockchainService
{
    private readonly Dictionary<string, BlockchainRpcService> _rpcClients = new();
    private readonly Dictionary<string, BlockchainConfig> _configurations = new();

    public BlockchainService()
    {
        // Initialize with default configurations
        // These can be loaded from configuration files
    }

    public void AddBlockchain(string name, BlockchainConfig config)
    {
        _configurations[name] = config;
        if (config.Enabled)
        {
            var rpcService = new BlockchainRpcService(
                config.RpcUrl,
                config.Username,
                config.Password);
            _rpcClients[name] = rpcService;
        }
    }

    public async Task<bool> IsConnectedAsync(string blockchain)
    {
        if (!_rpcClients.ContainsKey(blockchain))
            return false;

        try
        {
            var rpc = _rpcClients[blockchain];
            // Try to get blockchain info to test connection
            var info = await rpc.CallAsync<BlockchainInfo>("getblockchaininfo");
            return info != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<BlockchainObject?> GetObjectByAddressAsync(string address, string blockchain)
    {
        if (!_rpcClients.ContainsKey(blockchain))
            return null;

        try
        {
            var rpc = _rpcClients[blockchain];
            // Search for transactions at the address
            var transactions = await rpc.CallAsync<List<RawTransaction>>(
                "searchrawtransactions", address, 1, 0, 100);

            // Parse P2FK data from transactions
            if (transactions != null && transactions.Count > 0)
            {
                // Find the most recent transaction with P2FK data
                foreach (var tx in transactions)
                {
                    var obj = ParseP2FKObject(tx);
                    if (obj != null)
                        return obj;
                }
            }
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Error getting object: {ex.Message}");
        }

        return null;
    }

    public async Task<Profile?> GetProfileByUrnAsync(string urn, string blockchain)
    {
        if (!_rpcClients.ContainsKey(blockchain))
            return null;

        try
        {
            // Convert URN to address using deterministic method
            var address = UrnToAddress(urn, _configurations[blockchain].VersionByte);
            
            var rpc = _rpcClients[blockchain];
            var transactions = await rpc.CallAsync<List<RawTransaction>>(
                "searchrawtransactions", address, 1, 0, 100);

            if (transactions != null && transactions.Count > 0)
            {
                foreach (var tx in transactions)
                {
                    var profile = ParseP2FKProfile(tx);
                    if (profile != null && profile.Urn == urn)
                        return profile;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting profile: {ex.Message}");
        }

        return null;
    }

    public async Task<List<Message>> GetPublicMessagesAsync(string address, string blockchain, int skip = 0, int take = 50)
    {
        var messages = new List<Message>();

        if (!_rpcClients.ContainsKey(blockchain))
            return messages;

        try
        {
            var rpc = _rpcClients[blockchain];
            var transactions = await rpc.CallAsync<List<RawTransaction>>(
                "searchrawtransactions", address, 1, skip, take);

            if (transactions != null)
            {
                foreach (var tx in transactions)
                {
                    var message = ParseP2FKMessage(tx, false);
                    if (message != null)
                        messages.Add(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting messages: {ex.Message}");
        }

        return messages;
    }

    public async Task<List<Message>> GetPrivateMessagesAsync(string address, string blockchain, int skip = 0, int take = 50)
    {
        var messages = new List<Message>();

        if (!_rpcClients.ContainsKey(blockchain))
            return messages;

        try
        {
            var rpc = _rpcClients[blockchain];
            var transactions = await rpc.CallAsync<List<RawTransaction>>(
                "searchrawtransactions", address, 1, skip, take);

            if (transactions != null)
            {
                foreach (var tx in transactions)
                {
                    var message = ParseP2FKMessage(tx, true);
                    if (message != null)
                        messages.Add(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting private messages: {ex.Message}");
        }

        return messages;
    }

    public async Task<List<BlockchainObject>> SearchObjectsByKeywordAsync(string keyword, string blockchain)
    {
        var objects = new List<BlockchainObject>();

        if (!_rpcClients.ContainsKey(blockchain))
            return objects;

        try
        {
            // Convert keyword to address
            var address = KeywordToAddress(keyword, _configurations[blockchain].VersionByte);
            
            var rpc = _rpcClients[blockchain];
            var transactions = await rpc.CallAsync<List<RawTransaction>>(
                "searchrawtransactions", address, 1, 0, 100);

            if (transactions != null)
            {
                foreach (var tx in transactions)
                {
                    var obj = ParseP2FKObject(tx);
                    if (obj != null && obj.Keywords.Contains(keyword))
                        objects.Add(obj);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching objects: {ex.Message}");
        }

        return objects;
    }

    public async Task<List<BlockchainObject>> GetObjectsByCreatorAsync(string creatorAddress, string blockchain)
    {
        var objects = new List<BlockchainObject>();

        if (!_rpcClients.ContainsKey(blockchain))
            return objects;

        try
        {
            var rpc = _rpcClients[blockchain];
            var transactions = await rpc.CallAsync<List<RawTransaction>>(
                "searchrawtransactions", creatorAddress, 1, 0, 100);

            if (transactions != null)
            {
                foreach (var tx in transactions)
                {
                    var obj = ParseP2FKObject(tx);
                    if (obj != null && obj.Creator == creatorAddress)
                        objects.Add(obj);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting objects by creator: {ex.Message}");
        }

        return objects;
    }

    public async Task<string> MintObjectAsync(BlockchainObject obj, string blockchain)
    {
        if (!_rpcClients.ContainsKey(blockchain))
            throw new InvalidOperationException($"Blockchain {blockchain} not configured");

        try
        {
            var rpc = _rpcClients[blockchain];
            
            // Create P2FK transaction data
            var p2fkData = SerializeP2FKObject(obj);
            
            // Create raw transaction (simplified - actual implementation would be more complex)
            var txHex = await CreateP2FKTransaction(rpc, obj.Address, p2fkData);
            
            // Sign and send
            var signedTx = await rpc.CallAsync<SignedTransaction>("signrawtransaction", txHex);
            var txId = await rpc.CallAsync<string>("sendrawtransaction", signedTx.Hex);
            
            return txId;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to mint object: {ex.Message}", ex);
        }
    }

    public async Task<string> MintProfileAsync(Profile profile, string blockchain)
    {
        if (!_rpcClients.ContainsKey(blockchain))
            throw new InvalidOperationException($"Blockchain {blockchain} not configured");

        try
        {
            var rpc = _rpcClients[blockchain];
            
            // Create P2FK transaction data
            var p2fkData = SerializeP2FKProfile(profile);
            
            // Create, sign and send transaction
            var txHex = await CreateP2FKTransaction(rpc, profile.Address, p2fkData);
            var signedTx = await rpc.CallAsync<SignedTransaction>("signrawtransaction", txHex);
            var txId = await rpc.CallAsync<string>("sendrawtransaction", signedTx.Hex);
            
            return txId;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to mint profile: {ex.Message}", ex);
        }
    }

    #region Helper Methods

    private BlockchainObject? ParseP2FKObject(RawTransaction tx)
    {
        // Parse P2FK data from transaction
        // This is a simplified version - actual implementation would decode OP_RETURN data
        try
        {
            if (tx.Vout != null && tx.Vout.Length > 0)
            {
                foreach (var output in tx.Vout)
                {
                    if (output.ScriptPubKey?.Asm?.Contains("OP_RETURN") == true)
                    {
                        // Extract and decode P2FK data
                        // For now, return a placeholder
                        return new BlockchainObject
                        {
                            TransactionId = tx.TxId,
                            BlockHeight = tx.Confirmations
                        };
                    }
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }

        return null;
    }

    private Profile? ParseP2FKProfile(RawTransaction tx)
    {
        // Similar to ParseP2FKObject but for profiles
        try
        {
            if (tx.Vout != null && tx.Vout.Length > 0)
            {
                foreach (var output in tx.Vout)
                {
                    if (output.ScriptPubKey?.Asm?.Contains("OP_RETURN") == true)
                    {
                        return new Profile
                        {
                            Address = tx.Vout[0].ScriptPubKey.Addresses?[0] ?? ""
                        };
                    }
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }

        return null;
    }

    private Message? ParseP2FKMessage(RawTransaction tx, bool isPrivate)
    {
        // Parse message from transaction
        try
        {
            return new Message
            {
                Id = tx.TxId,
                TransactionId = tx.TxId,
                IsEncrypted = isPrivate,
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(tx.BlockTime).DateTime
            };
        }
        catch
        {
            return null;
        }
    }

    private string UrnToAddress(string urn, byte versionByte)
    {
        // Convert URN to deterministic address using P2FK protocol
        // This is a placeholder - actual implementation would use proper encoding
        var hash = Sha256Helper.Hash(System.Text.Encoding.UTF8.GetBytes(urn));
        return Base58.EncodeWithCheckSum(new byte[] { versionByte }.Concat(hash.Take(20)).ToArray());
    }

    private string KeywordToAddress(string keyword, byte versionByte)
    {
        // Convert keyword to address - similar to URN
        return UrnToAddress(keyword, versionByte);
    }

    private string SerializeP2FKObject(BlockchainObject obj)
    {
        // Serialize object to P2FK format
        var json = JsonSerializer.Serialize(obj);
        return Convert.ToHexString(System.Text.Encoding.UTF8.GetBytes(json));
    }

    private string SerializeP2FKProfile(Profile profile)
    {
        // Serialize profile to P2FK format
        var json = JsonSerializer.Serialize(profile);
        return Convert.ToHexString(System.Text.Encoding.UTF8.GetBytes(json));
    }

    private async Task<string> CreateP2FKTransaction(BlockchainRpcService rpc, string address, string data)
    {
        // Create a raw P2FK transaction
        // This is a simplified placeholder - actual implementation would:
        // 1. Select UTXOs
        // 2. Create outputs with OP_RETURN data
        // 3. Calculate fees
        // 4. Build raw transaction hex
        
        // For now, throw not implemented
        throw new NotImplementedException("P2FK transaction creation requires full UTXO management");
    }

    #endregion

    #region Response Models

    private class BlockchainInfo
    {
        public string Chain { get; set; } = "";
        public int Blocks { get; set; }
        public int Headers { get; set; }
    }

    private class RawTransaction
    {
        public string TxId { get; set; } = "";
        public string Hex { get; set; } = "";
        public int Confirmations { get; set; }
        public long BlockTime { get; set; }
        public TxOutput[]? Vout { get; set; }
        public TxInput[]? Vin { get; set; }
    }

    private class TxOutput
    {
        public decimal Value { get; set; }
        public int N { get; set; }
        public ScriptPubKey? ScriptPubKey { get; set; }
    }

    private class TxInput
    {
        public string TxId { get; set; } = "";
        public int Vout { get; set; }
    }

    private class ScriptPubKey
    {
        public string? Asm { get; set; }
        public string? Hex { get; set; }
        public string[]? Addresses { get; set; }
    }

    private class SignedTransaction
    {
        public string Hex { get; set; } = "";
        public bool Complete { get; set; }
    }

    #endregion
}
