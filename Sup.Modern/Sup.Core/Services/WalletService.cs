using Sup.Core.P2FK.Classes;
using System.Security.Cryptography;

namespace Sup.Core.Services;

/// <summary>
/// Wallet service for managing cryptocurrency addresses and keys
/// </summary>
public class WalletService : IWalletService
{
    private readonly Dictionary<string, BlockchainRpcService> _rpcClients;

    public WalletService(Dictionary<string, BlockchainRpcService> rpcClients)
    {
        _rpcClients = rpcClients;
    }

    public async Task<decimal> GetBalanceAsync(string address, string blockchain)
    {
        if (!_rpcClients.ContainsKey(blockchain))
            throw new InvalidOperationException($"Blockchain {blockchain} not configured");

        try
        {
            var rpc = _rpcClients[blockchain];
            
            // Get unspent transactions for the address
            var unspent = await rpc.CallAsync<List<UnspentOutput>>(
                "listunspent", 0, 9999999, new[] { address });

            if (unspent == null || unspent.Count == 0)
                return 0;

            return unspent.Sum(u => u.Amount);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get balance: {ex.Message}", ex);
        }
    }

    public async Task<string> GetNewAddressAsync(string blockchain)
    {
        if (!_rpcClients.ContainsKey(blockchain))
            throw new InvalidOperationException($"Blockchain {blockchain} not configured");

        try
        {
            var rpc = _rpcClients[blockchain];
            var address = await rpc.CallAsync<string>("getnewaddress");
            return address;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to generate new address: {ex.Message}", ex);
        }
    }

    public async Task<List<string>> ListAddressesAsync(string blockchain)
    {
        if (!_rpcClients.ContainsKey(blockchain))
            throw new InvalidOperationException($"Blockchain {blockchain} not configured");

        try
        {
            var rpc = _rpcClients[blockchain];
            
            // Get all addresses with balances or transactions
            var addresses = await rpc.CallAsync<List<AddressInfo>>("listreceivedbyaddress", 0, true);
            
            if (addresses == null)
                return new List<string>();

            return addresses.Select(a => a.Address).ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to list addresses: {ex.Message}", ex);
        }
    }

    public async Task<string> SignMessageAsync(string address, string message, string blockchain)
    {
        if (!_rpcClients.ContainsKey(blockchain))
            throw new InvalidOperationException($"Blockchain {blockchain} not configured");

        try
        {
            var rpc = _rpcClients[blockchain];
            var signature = await rpc.CallAsync<string>("signmessage", address, message);
            return signature;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to sign message: {ex.Message}", ex);
        }
    }

    public async Task<bool> VerifyMessageAsync(string address, string signature, string message)
    {
        // Message verification can be done locally without RPC
        try
        {
            // This would require full ECDSA signature verification
            // For now, we'll use a simplified approach
            
            // In production, this would:
            // 1. Decode the signature from Base64
            // 2. Recover the public key from signature
            // 3. Verify the signature against the message
            // 4. Check if recovered address matches provided address
            
            // Placeholder implementation
            return !string.IsNullOrEmpty(signature) && !string.IsNullOrEmpty(address);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generate a new key pair (private/public key)
    /// </summary>
    public (byte[] privateKey, byte[] publicKey) GenerateKeyPair()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var parameters = ecdsa.ExportParameters(true);
        
        if (parameters.D == null || parameters.Q.X == null || parameters.Q.Y == null)
            throw new InvalidOperationException("Failed to generate key pair");

        var privateKey = parameters.D;
        var publicKey = new byte[parameters.Q.X.Length + parameters.Q.Y.Length];
        Buffer.BlockCopy(parameters.Q.X, 0, publicKey, 0, parameters.Q.X.Length);
        Buffer.BlockCopy(parameters.Q.Y, 0, publicKey, parameters.Q.X.Length, parameters.Q.Y.Length);

        return (privateKey, publicKey);
    }

    /// <summary>
    /// Derive address from public key
    /// </summary>
    public string PublicKeyToAddress(byte[] publicKey, byte versionByte)
    {
        // Hash the public key
        var sha256Hash = Sha256Helper.Hash(publicKey);
        var ripemd160 = new byte[20]; // Placeholder - would use RIPEMD160
        
        // Add version byte
        var versionedHash = new byte[] { versionByte }.Concat(ripemd160).ToArray();
        
        // Base58Check encode
        return Base58.EncodeWithCheckSum(versionedHash);
    }

    #region Response Models

    private class UnspentOutput
    {
        public string TxId { get; set; } = "";
        public int Vout { get; set; }
        public string Address { get; set; } = "";
        public decimal Amount { get; set; }
        public int Confirmations { get; set; }
    }

    private class AddressInfo
    {
        public string Address { get; set; } = "";
        public decimal Amount { get; set; }
        public int Confirmations { get; set; }
    }

    #endregion
}
