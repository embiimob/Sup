using System.Text;
using System.Text.Json;
using Sup.Core.Models;

namespace Sup.Core.Services;

/// <summary>
/// Service for interacting with p2fk.io API
/// Provides an alternative to direct RPC for users who don't run full nodes
/// </summary>
public class P2fkApiService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;
    private string? _apiKey;

    public P2fkApiService(string apiBaseUrl = "https://api.p2fk.io", string? apiKey = null)
    {
        _apiBaseUrl = apiBaseUrl;
        _apiKey = apiKey;
        
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_apiBaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        }
    }

    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient.DefaultRequestHeaders.Remove("X-API-Key");
        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        }
    }

    #region Object Operations

    /// <summary>
    /// Get object by address from API
    /// </summary>
    public async Task<BlockchainObject?> GetObjectAsync(string address, string blockchain = "bitcoin-testnet")
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/objects/{blockchain}/{address}");
            
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<BlockchainObject>>(json);
            
            return apiResponse?.Data;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get object from API: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Search objects by keyword
    /// </summary>
    public async Task<List<BlockchainObject>> SearchObjectsAsync(string keyword, string blockchain = "bitcoin-testnet", int limit = 50)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/objects/{blockchain}/search?keyword={Uri.EscapeDataString(keyword)}&limit={limit}");
            
            if (!response.IsSuccessStatusCode)
                return new List<BlockchainObject>();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<BlockchainObject>>>(json);
            
            return apiResponse?.Data ?? new List<BlockchainObject>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to search objects: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mint object via API
    /// </summary>
    public async Task<string> MintObjectAsync(BlockchainObject obj, string blockchain = "bitcoin-testnet", string? walletToken = null)
    {
        try
        {
            var requestData = new
            {
                blockchain,
                obj,
                walletToken
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/v1/objects/mint", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<ApiTransactionResponse>>(responseJson);

            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                return apiResponse.Data.TransactionId;
            }

            throw new InvalidOperationException(apiResponse?.Error ?? "Unknown error minting object");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to mint object via API: {ex.Message}", ex);
        }
    }

    #endregion

    #region Profile Operations

    /// <summary>
    /// Get profile by URN from API
    /// </summary>
    public async Task<Profile?> GetProfileAsync(string urn, string blockchain = "bitcoin-testnet")
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/profiles/{blockchain}/{Uri.EscapeDataString(urn)}");
            
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Profile>>(json);
            
            return apiResponse?.Data;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get profile from API: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Register/mint profile via API
    /// </summary>
    public async Task<string> MintProfileAsync(Profile profile, string blockchain = "bitcoin-testnet", string? walletToken = null)
    {
        try
        {
            var requestData = new
            {
                blockchain,
                profile,
                walletToken
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/v1/profiles/mint", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<ApiTransactionResponse>>(responseJson);

            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                return apiResponse.Data.TransactionId;
            }

            throw new InvalidOperationException(apiResponse?.Error ?? "Unknown error minting profile");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to mint profile via API: {ex.Message}", ex);
        }
    }

    #endregion

    #region Message Operations

    /// <summary>
    /// Get messages for an address
    /// </summary>
    public async Task<List<Message>> GetMessagesAsync(string address, string blockchain = "bitcoin-testnet", bool includePrivate = false, int skip = 0, int take = 50)
    {
        try
        {
            var url = $"/api/v1/messages/{blockchain}/{address}?skip={skip}&take={take}&includePrivate={includePrivate}";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new List<Message>();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<Message>>>(json);
            
            return apiResponse?.Data ?? new List<Message>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get messages from API: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Send message via API
    /// </summary>
    public async Task<string> SendMessageAsync(string fromAddress, string toAddress, string content, string blockchain = "bitcoin-testnet", bool encrypt = false, string? walletToken = null)
    {
        try
        {
            var requestData = new
            {
                blockchain,
                fromAddress,
                toAddress,
                content,
                encrypt,
                walletToken
            };

            var json = JsonSerializer.Serialize(requestData);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/v1/messages/send", httpContent);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<ApiTransactionResponse>>(responseJson);

            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                return apiResponse.Data.TransactionId;
            }

            throw new InvalidOperationException(apiResponse?.Error ?? "Unknown error sending message");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to send message via API: {ex.Message}", ex);
        }
    }

    #endregion

    #region Wallet Operations

    /// <summary>
    /// Create wallet via API (returns wallet token for signing)
    /// </summary>
    public async Task<ApiWalletConfig> CreateWalletAsync(string blockchain = "bitcoin-testnet")
    {
        try
        {
            var requestData = new { blockchain };
            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/v1/wallet/create", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<ApiWalletConfig>>(responseJson);

            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                return apiResponse.Data;
            }

            throw new InvalidOperationException(apiResponse?.Error ?? "Unknown error creating wallet");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create wallet via API: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Get wallet balance via API
    /// </summary>
    public async Task<decimal> GetBalanceAsync(string address, string blockchain = "bitcoin-testnet")
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/wallet/{blockchain}/{address}/balance");
            
            if (!response.IsSuccessStatusCode)
                return 0;

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<decimal>>(json);
            
            return apiResponse?.Data ?? 0;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get balance from API: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Create and sign sendmany transaction via API wallet
    /// </summary>
    public async Task<string> SendManyAsync(string fromAddress, Dictionary<string, decimal> outputs, string blockchain = "bitcoin-testnet", string? walletToken = null, string? opReturnData = null)
    {
        try
        {
            var requestData = new ApiTransactionRequest
            {
                FromAddress = fromAddress,
                Outputs = outputs,
                OpReturnData = opReturnData,
                Blockchain = blockchain
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            if (!string.IsNullOrEmpty(walletToken))
            {
                content.Headers.Add("X-Wallet-Token", walletToken);
            }

            var response = await _httpClient.PostAsync("/api/v1/wallet/sendmany", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<ApiTransactionResponse>>(responseJson);

            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                return apiResponse.Data.TransactionId;
            }

            throw new InvalidOperationException(apiResponse?.Error ?? "Unknown error sending transaction");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to send transaction via API: {ex.Message}", ex);
        }
    }

    #endregion

    #region Blockchain Info

    /// <summary>
    /// Get blockchain info and status
    /// </summary>
    public async Task<bool> IsHealthyAsync(string blockchain = "bitcoin-testnet")
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/blockchain/{blockchain}/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get current block height
    /// </summary>
    public async Task<int> GetBlockHeightAsync(string blockchain = "bitcoin-testnet")
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/blockchain/{blockchain}/height");
            
            if (!response.IsSuccessStatusCode)
                return 0;

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<int>>(json);
            
            return apiResponse?.Data ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    #endregion

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
