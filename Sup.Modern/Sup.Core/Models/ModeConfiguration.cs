namespace Sup.Core.Models;

/// <summary>
/// Application mode configuration
/// </summary>
public enum ApplicationMode
{
    /// <summary>
    /// Fully decentralized - uses direct RPC to blockchain nodes
    /// </summary>
    Decentralized,
    
    /// <summary>
    /// API mode - uses p2fk.io API for most operations
    /// </summary>
    API
}

/// <summary>
/// Configuration for application mode
/// </summary>
public class ModeConfiguration
{
    public ApplicationMode Mode { get; set; } = ApplicationMode.Decentralized;
    public string ApiBaseUrl { get; set; } = "https://api.p2fk.io";
    public string? ApiKey { get; set; }
    public bool UseApiWallet { get; set; } = false;
}

/// <summary>
/// API wallet configuration for signing transactions
/// </summary>
public class ApiWalletConfig
{
    public string? WalletAddress { get; set; }
    public string? ApiToken { get; set; }
    public bool RememberCredentials { get; set; } = false;
}

/// <summary>
/// API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// API transaction request
/// </summary>
public class ApiTransactionRequest
{
    public string FromAddress { get; set; } = "";
    public Dictionary<string, decimal> Outputs { get; set; } = new();
    public string? OpReturnData { get; set; }
    public string Blockchain { get; set; } = "bitcoin-testnet";
}

/// <summary>
/// API transaction response
/// </summary>
public class ApiTransactionResponse
{
    public string TransactionId { get; set; } = "";
    public string RawTransaction { get; set; } = "";
    public bool RequiresSigning { get; set; }
    public string? SigningUrl { get; set; }
}
