using Sup.Core.Models;

namespace Sup.Core.Services;

/// <summary>
/// Service for managing blockchain connections and operations
/// </summary>
public interface IBlockchainService
{
    Task<bool> IsConnectedAsync(string blockchain);
    Task<BlockchainObject?> GetObjectByAddressAsync(string address, string blockchain);
    Task<Profile?> GetProfileByUrnAsync(string urn, string blockchain);
    Task<List<Message>> GetPublicMessagesAsync(string address, string blockchain, int skip = 0, int take = 50);
    Task<List<Message>> GetPrivateMessagesAsync(string address, string blockchain, int skip = 0, int take = 50);
    Task<List<BlockchainObject>> SearchObjectsByKeywordAsync(string keyword, string blockchain);
    Task<List<BlockchainObject>> GetObjectsByCreatorAsync(string creatorAddress, string blockchain);
    Task<string> MintObjectAsync(BlockchainObject obj, string blockchain);
    Task<string> MintProfileAsync(Profile profile, string blockchain);
}

/// <summary>
/// Service for managing IPFS operations
/// </summary>
public interface IIpfsService
{
    Task<bool> IsRunningAsync();
    Task StartDaemonAsync();
    Task StopDaemonAsync();
    Task<string> AddFileAsync(string filePath);
    Task<string> AddDataAsync(byte[] data);
    Task<byte[]> GetDataAsync(string cid);
    Task PinAsync(string cid);
    Task UnpinAsync(string cid);
}

/// <summary>
/// Service for managing cryptocurrency wallets
/// </summary>
public interface IWalletService
{
    Task<decimal> GetBalanceAsync(string address, string blockchain);
    Task<string> GetNewAddressAsync(string blockchain);
    Task<List<string>> ListAddressesAsync(string blockchain);
    Task<string> SignMessageAsync(string address, string message, string blockchain);
    Task<bool> VerifyMessageAsync(string address, string signature, string message);
}
