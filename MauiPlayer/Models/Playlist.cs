using SQLite;

namespace MauiPlayer.Models;

/// <summary>
/// Represents a playlist for organizing media files
/// </summary>
[Table("Playlists")]
public class Playlist
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Playlist name
    /// </summary>
    [Indexed]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Playlist type: audio, video, or mixed
    /// </summary>
    public string Type { get; set; } = "mixed";

    /// <summary>
    /// Optional description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// When playlist was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// When playlist was last modified
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Number of items in playlist
    /// </summary>
    public int ItemCount { get; set; }
}

/// <summary>
/// Represents an item in a playlist
/// </summary>
[Table("PlaylistItems")]
public class PlaylistItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Reference to playlist
    /// </summary>
    [Indexed]
    public int PlaylistId { get; set; }

    /// <summary>
    /// IPFS hash of the file
    /// </summary>
    public string IpfsHash { get; set; } = string.Empty;

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Local file path
    /// </summary>
    public string LocalPath { get; set; } = string.Empty;

    /// <summary>
    /// Media type: audio or video
    /// </summary>
    public string MediaType { get; set; } = string.Empty;

    /// <summary>
    /// Order in playlist
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Associated transaction ID
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// When item was added to playlist
    /// </summary>
    public DateTime AddedDate { get; set; }
}
