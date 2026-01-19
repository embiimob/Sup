using SQLite;

namespace MauiPlayer.Models;

/// <summary>
/// Represents a blocked address
/// </summary>
[Table("BlockedAddresses")]
public class BlockedAddress
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Bitcoin address to block
    /// </summary>
    [Indexed, Unique]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Optional P2FK handle
    /// </summary>
    public string? Handle { get; set; }

    /// <summary>
    /// Reason for blocking
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// When address was blocked
    /// </summary>
    public DateTime BlockedDate { get; set; }
}
