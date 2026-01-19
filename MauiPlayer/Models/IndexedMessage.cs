using SQLite;

namespace MauiPlayer.Models;

/// <summary>
/// Represents a P2FK message indexed from Bitcoin testnet3
/// </summary>
[Table("Messages")]
public class IndexedMessage
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Transaction ID on blockchain
    /// </summary>
    [Indexed]
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Sender address (SignedBy from P2FK)
    /// </summary>
    [Indexed]
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Recipient address
    /// </summary>
    [Indexed]
    public string ToAddress { get; set; } = string.Empty;

    /// <summary>
    /// P2FK handle (URN) if available
    /// </summary>
    public string? Handle { get; set; }

    /// <summary>
    /// Message content
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Message type: Text, Image, Audio, Video
    /// </summary>
    public string MessageType { get; set; } = "Text";

    /// <summary>
    /// IPFS hash if message contains file
    /// </summary>
    public string? IpfsHash { get; set; }

    /// <summary>
    /// Local file path if downloaded
    /// </summary>
    public string? LocalFilePath { get; set; }

    /// <summary>
    /// Block date from blockchain
    /// </summary>
    public DateTime BlockDate { get; set; }

    /// <summary>
    /// Block height
    /// </summary>
    public int BlockHeight { get; set; }

    /// <summary>
    /// Number of confirmations
    /// </summary>
    public int Confirmations { get; set; }

    /// <summary>
    /// When this message was indexed locally
    /// </summary>
    public DateTime IndexedDate { get; set; }

    /// <summary>
    /// Is this message from a blocked address
    /// </summary>
    public bool IsBlocked { get; set; }
}
