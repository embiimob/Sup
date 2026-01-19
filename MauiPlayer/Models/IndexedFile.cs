using SQLite;

namespace MauiPlayer.Models;

/// <summary>
/// Represents a file indexed from IPFS
/// </summary>
[Table("IndexedFiles")]
public class IndexedFile
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// IPFS hash
    /// </summary>
    [Indexed, Unique]
    public string IpfsHash { get; set; } = string.Empty;

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Local storage path
    /// </summary>
    public string LocalPath { get; set; } = string.Empty;

    /// <summary>
    /// File type: image, audio, video, other
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Associated transaction ID
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// Associated sender address
    /// </summary>
    public string? FromAddress { get; set; }

    /// <summary>
    /// When file was downloaded
    /// </summary>
    public DateTime DownloadedDate { get; set; }

    /// <summary>
    /// Is file from blocked address
    /// </summary>
    public bool IsBlocked { get; set; }
}
