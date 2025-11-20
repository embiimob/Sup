using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Sup.Core.P2FK.Classes;

/// <summary>
/// Modern async blockchain RPC service for .NET 8
/// Replaces the legacy synchronous HttpWebRequest-based implementation
/// </summary>
public class BlockchainRpcService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _rpcUrl;
    private readonly NetworkCredential _credentials;
    private int _requestId = 0;

    public BlockchainRpcService(string rpcUrl, string username, string password)
    {
        _rpcUrl = rpcUrl;
        _credentials = new NetworkCredential(username, password);
        
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(rpcUrl)
        };
        
        // Set up authentication
        var authBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
        var authHeader = Convert.ToBase64String(authBytes);
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Basic", authHeader);
        
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Execute an RPC call asynchronously
    /// </summary>
    public async Task<T> CallAsync<T>(string method, params object[] parameters)
    {
        var request = new RpcRequest
        {
            Id = Interlocked.Increment(ref _requestId),
            Method = method,
            Params = parameters
        };

        var jsonRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var rpcResponse = JsonSerializer.Deserialize<RpcResponse<T>>(jsonResponse, 
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            if (rpcResponse?.Error != null)
            {
                throw new BlockchainRpcException(
                    rpcResponse.Error.Code, 
                    rpcResponse.Error.Message);
            }

            return rpcResponse!.Result!;
        }
        catch (HttpRequestException ex)
        {
            throw new BlockchainRpcException(-1, $"HTTP request failed: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    private class RpcRequest
    {
        public string JsonRpc { get; set; } = "1.0";
        public int Id { get; set; }
        public string Method { get; set; } = "";
        public object[] Params { get; set; } = Array.Empty<object>();
    }

    private class RpcResponse<T>
    {
        public T? Result { get; set; }
        public RpcError? Error { get; set; }
        public int Id { get; set; }
    }

    private class RpcError
    {
        public int Code { get; set; }
        public string Message { get; set; } = "";
    }
}

public class BlockchainRpcException : Exception
{
    public int ErrorCode { get; }

    public BlockchainRpcException(int errorCode, string message) 
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public BlockchainRpcException(int errorCode, string message, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
