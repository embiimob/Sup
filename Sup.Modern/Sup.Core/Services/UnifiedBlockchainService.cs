using Sup.Core.Models;

namespace Sup.Core.Services;

/// <summary>
/// Unified blockchain service that supports both Decentralized (RPC) and API modes
/// </summary>
public class UnifiedBlockchainService : IBlockchainService
{
    private readonly BlockchainService _rpcService;
    private readonly P2fkApiService _apiService;
    private ApplicationMode _currentMode;
    private string? _apiWalletToken;

    public UnifiedBlockchainService(
        BlockchainService rpcService,
        P2fkApiService apiService,
        ModeConfiguration? config = null)
    {
        _rpcService = rpcService;
        _apiService = apiService;
        _currentMode = config?.Mode ?? ApplicationMode.Decentralized;
    }

    /// <summary>
    /// Switch between Decentralized and API mode
    /// </summary>
    public void SetMode(ApplicationMode mode)
    {
        _currentMode = mode;
    }

    /// <summary>
    /// Get current application mode
    /// </summary>
    public ApplicationMode GetMode() => _currentMode;

    /// <summary>
    /// Set API wallet token for signing transactions in API mode
    /// </summary>
    public void SetApiWalletToken(string token)
    {
        _apiWalletToken = token;
    }

    #region IBlockchainService Implementation

    public async Task<bool> IsConnectedAsync(string blockchain)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            return await _rpcService.IsConnectedAsync(blockchain);
        }
        else
        {
            return await _apiService.IsHealthyAsync(MapBlockchainName(blockchain));
        }
    }

    public async Task<BlockchainObject?> GetObjectByAddressAsync(string address, string blockchain)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            return await _rpcService.GetObjectByAddressAsync(address, blockchain);
        }
        else
        {
            return await _apiService.GetObjectAsync(address, MapBlockchainName(blockchain));
        }
    }

    public async Task<Profile?> GetProfileByUrnAsync(string urn, string blockchain)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            return await _rpcService.GetProfileByUrnAsync(urn, blockchain);
        }
        else
        {
            return await _apiService.GetProfileAsync(urn, MapBlockchainName(blockchain));
        }
    }

    public async Task<List<Message>> GetPublicMessagesAsync(string address, string blockchain, int skip = 0, int take = 50)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            return await _rpcService.GetPublicMessagesAsync(address, blockchain, skip, take);
        }
        else
        {
            return await _apiService.GetMessagesAsync(address, MapBlockchainName(blockchain), false, skip, take);
        }
    }

    public async Task<List<Message>> GetPrivateMessagesAsync(string address, string blockchain, int skip = 0, int take = 50)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            return await _rpcService.GetPrivateMessagesAsync(address, blockchain, skip, take);
        }
        else
        {
            return await _apiService.GetMessagesAsync(address, MapBlockchainName(blockchain), true, skip, take);
        }
    }

    public async Task<List<BlockchainObject>> SearchObjectsByKeywordAsync(string keyword, string blockchain)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            return await _rpcService.SearchObjectsByKeywordAsync(keyword, blockchain);
        }
        else
        {
            return await _apiService.SearchObjectsAsync(keyword, MapBlockchainName(blockchain));
        }
    }

    public async Task<List<BlockchainObject>> GetObjectsByCreatorAsync(string creatorAddress, string blockchain)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            return await _rpcService.GetObjectsByCreatorAsync(creatorAddress, blockchain);
        }
        else
        {
            // API mode - use search endpoint with creator filter
            var allObjects = await _apiService.SearchObjectsAsync("", MapBlockchainName(blockchain), 100);
            return allObjects.Where(o => o.Creator == creatorAddress).ToList();
        }
    }

    public async Task<string> MintObjectAsync(BlockchainObject obj, string blockchain)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            return await _rpcService.MintObjectAsync(obj, blockchain);
        }
        else
        {
            if (string.IsNullOrEmpty(_apiWalletToken))
            {
                throw new InvalidOperationException("API wallet token required for minting in API mode. Please set wallet credentials.");
            }
            return await _apiService.MintObjectAsync(obj, MapBlockchainName(blockchain), _apiWalletToken);
        }
    }

    public async Task<string> MintProfileAsync(Profile profile, string blockchain)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            return await _rpcService.MintProfileAsync(profile, blockchain);
        }
        else
        {
            if (string.IsNullOrEmpty(_apiWalletToken))
            {
                throw new InvalidOperationException("API wallet token required for minting in API mode. Please set wallet credentials.");
            }
            return await _apiService.MintProfileAsync(profile, MapBlockchainName(blockchain), _apiWalletToken);
        }
    }

    #endregion

    #region Additional API-specific Methods

    /// <summary>
    /// Send message (available in both modes)
    /// </summary>
    public async Task<string> SendMessageAsync(string fromAddress, string toAddress, string content, string blockchain, bool encrypt = false)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            // Use RPC to send message
            throw new NotImplementedException("RPC message sending not yet implemented");
        }
        else
        {
            if (string.IsNullOrEmpty(_apiWalletToken))
            {
                throw new InvalidOperationException("API wallet token required for sending messages in API mode.");
            }
            return await _apiService.SendMessageAsync(fromAddress, toAddress, content, MapBlockchainName(blockchain), encrypt, _apiWalletToken);
        }
    }

    /// <summary>
    /// Create API wallet (only in API mode)
    /// </summary>
    public async Task<ApiWalletConfig> CreateApiWalletAsync(string blockchain)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            throw new InvalidOperationException("API wallet creation is only available in API mode");
        }
        
        var walletConfig = await _apiService.CreateWalletAsync(MapBlockchainName(blockchain));
        _apiWalletToken = walletConfig.ApiToken;
        return walletConfig;
    }

    /// <summary>
    /// Get balance (works in both modes)
    /// </summary>
    public async Task<decimal> GetBalanceAsync(string address, string blockchain)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            // Would use wallet service
            throw new NotImplementedException("RPC balance check requires WalletService integration");
        }
        else
        {
            return await _apiService.GetBalanceAsync(address, MapBlockchainName(blockchain));
        }
    }

    /// <summary>
    /// Send many outputs (sendmany transaction)
    /// </summary>
    public async Task<string> SendManyAsync(string fromAddress, Dictionary<string, decimal> outputs, string blockchain, string? opReturnData = null)
    {
        if (_currentMode == ApplicationMode.Decentralized)
        {
            // Use RPC sendmany
            throw new NotImplementedException("RPC sendmany not yet implemented");
        }
        else
        {
            if (string.IsNullOrEmpty(_apiWalletToken))
            {
                throw new InvalidOperationException("API wallet token required for transactions in API mode.");
            }
            return await _apiService.SendManyAsync(fromAddress, outputs, MapBlockchainName(blockchain), _apiWalletToken, opReturnData);
        }
    }

    /// <summary>
    /// Get current block height
    /// </summary>
    public async Task<int> GetBlockHeightAsync(string blockchain)
    {
        if (_currentMode == ApplicationMode.API)
        {
            return await _apiService.GetBlockHeightAsync(MapBlockchainName(blockchain));
        }
        else
        {
            // Would use RPC getblockcount
            throw new NotImplementedException("RPC block height not yet implemented");
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Map internal blockchain names to API format
    /// </summary>
    private string MapBlockchainName(string blockchain)
    {
        return blockchain.ToLower() switch
        {
            "bitcointestnet" => "bitcoin-testnet",
            "bitcoinmainnet" => "bitcoin-mainnet",
            "litecoinmainnet" => "litecoin-mainnet",
            "dogecoinmainnet" => "dogecoin-mainnet",
            "mazacoinmainnet" => "mazacoin-mainnet",
            _ => blockchain.ToLower()
        };
    }

    #endregion
}
