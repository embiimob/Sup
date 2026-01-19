using SQLite;

namespace MauiPlayer.Models;

/// <summary>
/// Application configuration settings
/// </summary>
[Table("AppSettings")]
public class AppSettings
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Bitcoin RPC URL
    /// </summary>
    public string RpcUrl { get; set; } = "http://127.0.0.1:18332";

    /// <summary>
    /// Bitcoin RPC username
    /// </summary>
    public string RpcUsername { get; set; } = string.Empty;

    /// <summary>
    /// Bitcoin RPC password (encrypted)
    /// </summary>
    public string RpcPassword { get; set; } = string.Empty;

    /// <summary>
    /// Version byte for testnet3
    /// </summary>
    public string VersionByte { get; set; } = "111";

    /// <summary>
    /// IPFS gateway URL
    /// </summary>
    public string IpfsGateway { get; set; } = "https://ipfs.io/ipfs/";

    /// <summary>
    /// Enable IPFS auto-download
    /// </summary>
    public bool AutoDownloadIpfs { get; set; } = true;

    /// <summary>
    /// Maximum file size to auto-download (in MB)
    /// </summary>
    public int MaxAutoDownloadSizeMb { get; set; } = 10;

    /// <summary>
    /// Monitor specific addresses (comma-separated)
    /// </summary>
    public string? MonitoredAddresses { get; set; }

    /// <summary>
    /// Monitor specific handles (comma-separated)
    /// </summary>
    public string? MonitoredHandles { get; set; }

    /// <summary>
    /// Enable transaction monitoring
    /// </summary>
    public bool MonitoringEnabled { get; set; } = true;

    /// <summary>
    /// Monitoring interval in seconds
    /// </summary>
    public int MonitoringIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Last processed block height
    /// </summary>
    public int LastProcessedBlockHeight { get; set; }
}
