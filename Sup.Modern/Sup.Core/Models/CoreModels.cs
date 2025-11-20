namespace Sup.Core.Models;

/// <summary>
/// Represents a blockchain object in the P2FK protocol
/// </summary>
public class BlockchainObject
{
    public string Address { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string Urn { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Creator { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int BlockHeight { get; set; }
    public decimal Quantity { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Represents a user profile in the P2FK protocol
/// </summary>
public class Profile
{
    public string Address { get; set; } = string.Empty;
    public string Urn { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public List<string> Links { get; set; } = new();
    public List<string> PublicKeys { get; set; } = new();
    public DateTime RegisteredDate { get; set; }
    public DateTime LastModified { get; set; }
}

/// <summary>
/// Represents a message in the P2FK protocol
/// </summary>
public class Message
{
    public string Id { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Attachments { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public bool IsEncrypted { get; set; }
    public string TransactionId { get; set; } = string.Empty;
}

/// <summary>
/// Represents an inquiry/poll in the P2FK protocol
/// </summary>
public class Inquiry
{
    public string Address { get; set; } = string.Empty;
    public string Creator { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public List<InquiryOption> Options { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool RequiresSignature { get; set; }
    public string? TokenGate { get; set; }
}

public class InquiryOption
{
    public string Text { get; set; } = string.Empty;
    public int VoteCount { get; set; }
}

/// <summary>
/// Blockchain configuration
/// </summary>
public class BlockchainConfig
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string RpcUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public byte VersionByte { get; set; }
    public bool Enabled { get; set; }
    public string ExecutablePath { get; set; } = string.Empty;
}
