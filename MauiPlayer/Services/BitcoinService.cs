using SUP.RPCClient;
using System.Net;

namespace MauiPlayer.Services;

/// <summary>
/// Service for connecting to Bitcoin testnet3 network
/// </summary>
public class BitcoinService
{
    private CoinRPC? _rpcClient;
    private readonly DataStorageService _storage;

    public bool IsConnected { get; private set; }
    public string? LastError { get; private set; }

    public BitcoinService(DataStorageService storage)
    {
        _storage = storage;
    }

    /// <summary>
    /// Initialize connection to Bitcoin testnet3
    /// </summary>
    public async Task<bool> ConnectAsync()
    {
        try
        {
            var settings = await _storage.GetSettingsAsync();
            
            if (string.IsNullOrEmpty(settings.RpcUsername) || string.IsNullOrEmpty(settings.RpcPassword))
            {
                LastError = "RPC credentials not configured";
                IsConnected = false;
                return false;
            }

            // Decrypt password
            var password = DataStorageService.DecryptString(settings.RpcPassword, GetDeviceId());

            var credentials = new NetworkCredential(settings.RpcUsername, password);
            _rpcClient = new CoinRPC(new Uri(settings.RpcUrl), credentials);

            // Test connection
            var blockCount = await GetBlockCountAsync();
            IsConnected = blockCount > 0;
            LastError = null;

            return IsConnected;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            IsConnected = false;
            return false;
        }
    }

    /// <summary>
    /// Get current block count
    /// </summary>
    public async Task<int> GetBlockCountAsync()
    {
        if (_rpcClient == null)
            throw new InvalidOperationException("Not connected to Bitcoin RPC");

        try
        {
            var result = await Task.Run(() => _rpcClient.SendCommand("getblockcount"));
            if (result != null && int.TryParse(result.ToString(), out var count))
            {
                return count;
            }
            return 0;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return 0;
        }
    }

    /// <summary>
    /// Get raw mempool (pending transactions)
    /// </summary>
    public async Task<List<string>> GetRawMempoolAsync()
    {
        if (_rpcClient == null)
            throw new InvalidOperationException("Not connected to Bitcoin RPC");

        try
        {
            var result = await Task.Run(() => _rpcClient.SendCommand("getrawmempool"));
            if (result is Newtonsoft.Json.Linq.JArray array)
            {
                return array.Select(t => t.ToString()).ToList();
            }
            return new List<string>();
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return new List<string>();
        }
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    public async Task<dynamic?> GetTransactionAsync(string txid)
    {
        if (_rpcClient == null)
            throw new InvalidOperationException("Not connected to Bitcoin RPC");

        try
        {
            return await Task.Run(() => _rpcClient.GetRawDataTransaction(txid, true));
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return null;
        }
    }

    /// <summary>
    /// Search transactions for address
    /// </summary>
    public async Task<List<dynamic>> SearchTransactionsByAddressAsync(string address, int skip = 0, int count = 10)
    {
        if (_rpcClient == null)
            throw new InvalidOperationException("Not connected to Bitcoin RPC");

        try
        {
            var result = await Task.Run(() => _rpcClient.SearchRawDataTransaction(address, true, skip, count));
            if (result is Newtonsoft.Json.Linq.JArray array)
            {
                return array.Select(t => (dynamic)t).ToList();
            }
            return new List<dynamic>();
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return new List<dynamic>();
        }
    }

    /// <summary>
    /// Get RPC client for advanced operations
    /// </summary>
    public CoinRPC? GetRpcClient() => _rpcClient;

    /// <summary>
    /// Get settings for RPC operations
    /// </summary>
    public async Task<(string username, string password, string url, string versionByte)> GetRpcSettingsAsync()
    {
        var settings = await _storage.GetSettingsAsync();
        var password = DataStorageService.DecryptString(settings.RpcPassword, GetDeviceId());
        return (settings.RpcUsername, password, settings.RpcUrl, settings.VersionByte);
    }

    /// <summary>
    /// Get device-specific encryption key
    /// </summary>
    private string GetDeviceId()
    {
        // Use a combination of device-specific identifiers
        return $"{DeviceInfo.Name}_{DeviceInfo.Platform}_{DeviceInfo.Idiom}";
    }
}
