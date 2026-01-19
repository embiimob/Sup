using MauiPlayer.Models;
using SQLite;
using System.Security.Cryptography;
using System.Text;

namespace MauiPlayer.Services;

/// <summary>
/// Service for managing local encrypted data storage
/// </summary>
public class DataStorageService
{
    private readonly string _databasePath;
    private SQLiteAsyncConnection? _database;

    public DataStorageService()
    {
        _databasePath = Path.Combine(FileSystem.AppDataDirectory, "p2fk_player.db");
    }

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_database == null)
        {
            _database = new SQLiteAsyncConnection(_databasePath);
            await _database.CreateTableAsync<IndexedMessage>();
            await _database.CreateTableAsync<IndexedFile>();
            await _database.CreateTableAsync<BlockedAddress>();
            await _database.CreateTableAsync<AppSettings>();
            await _database.CreateTableAsync<Playlist>();
            await _database.CreateTableAsync<PlaylistItem>();
        }
        return _database;
    }

    // App Settings Methods
    public async Task<AppSettings> GetSettingsAsync()
    {
        var db = await GetDatabaseAsync();
        var settings = await db.Table<AppSettings>().FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new AppSettings();
            await db.InsertAsync(settings);
        }
        return settings;
    }

    public async Task<int> SaveSettingsAsync(AppSettings settings)
    {
        var db = await GetDatabaseAsync();
        if (settings.Id == 0)
            return await db.InsertAsync(settings);
        else
            return await db.UpdateAsync(settings);
    }

    // Message Methods
    public async Task<List<IndexedMessage>> GetRecentMessagesAsync(int limit = 50)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<IndexedMessage>()
            .Where(m => !m.IsBlocked)
            .OrderByDescending(m => m.BlockDate)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<IndexedMessage>> SearchMessagesByAddressAsync(string address)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<IndexedMessage>()
            .Where(m => (m.FromAddress == address || m.ToAddress == address) && !m.IsBlocked)
            .OrderByDescending(m => m.BlockDate)
            .ToListAsync();
    }

    public async Task<List<IndexedMessage>> SearchMessagesByHandleAsync(string handle)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<IndexedMessage>()
            .Where(m => m.Handle == handle && !m.IsBlocked)
            .OrderByDescending(m => m.BlockDate)
            .ToListAsync();
    }

    public async Task<int> SaveMessageAsync(IndexedMessage message)
    {
        var db = await GetDatabaseAsync();
        
        // Check if message already exists
        var existing = await db.Table<IndexedMessage>()
            .Where(m => m.TransactionId == message.TransactionId)
            .FirstOrDefaultAsync();

        if (existing != null)
            return 0; // Already exists

        message.IndexedDate = DateTime.UtcNow;
        return await db.InsertAsync(message);
    }

    public async Task<int> DeleteMessageAsync(string transactionId)
    {
        var db = await GetDatabaseAsync();
        var message = await db.Table<IndexedMessage>()
            .Where(m => m.TransactionId == transactionId)
            .FirstOrDefaultAsync();

        if (message != null)
        {
            // Delete associated file if exists
            if (!string.IsNullOrEmpty(message.LocalFilePath) && File.Exists(message.LocalFilePath))
            {
                try { File.Delete(message.LocalFilePath); } catch { }
            }
            return await db.DeleteAsync(message);
        }
        return 0;
    }

    // File Methods
    public async Task<List<IndexedFile>> GetRecentFilesAsync(int limit = 50)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<IndexedFile>()
            .Where(f => !f.IsBlocked)
            .OrderByDescending(f => f.DownloadedDate)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IndexedFile?> GetFileByHashAsync(string ipfsHash)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<IndexedFile>()
            .Where(f => f.IpfsHash == ipfsHash)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveFileAsync(IndexedFile file)
    {
        var db = await GetDatabaseAsync();
        
        var existing = await db.Table<IndexedFile>()
            .Where(f => f.IpfsHash == file.IpfsHash)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            file.Id = existing.Id;
            return await db.UpdateAsync(file);
        }

        file.DownloadedDate = DateTime.UtcNow;
        return await db.InsertAsync(file);
    }

    public async Task<int> DeleteFileAsync(string ipfsHash)
    {
        var db = await GetDatabaseAsync();
        var file = await db.Table<IndexedFile>()
            .Where(f => f.IpfsHash == ipfsHash)
            .FirstOrDefaultAsync();

        if (file != null)
        {
            // Delete local file
            if (File.Exists(file.LocalPath))
            {
                try { File.Delete(file.LocalPath); } catch { }
            }
            return await db.DeleteAsync(file);
        }
        return 0;
    }

    // Blocked Address Methods
    public async Task<List<BlockedAddress>> GetBlockedAddressesAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<BlockedAddress>().ToListAsync();
    }

    public async Task<bool> IsAddressBlockedAsync(string address)
    {
        var db = await GetDatabaseAsync();
        var blocked = await db.Table<BlockedAddress>()
            .Where(b => b.Address == address)
            .FirstOrDefaultAsync();
        return blocked != null;
    }

    public async Task<int> BlockAddressAsync(string address, string? handle = null, string? reason = null)
    {
        var db = await GetDatabaseAsync();
        
        var existing = await db.Table<BlockedAddress>()
            .Where(b => b.Address == address)
            .FirstOrDefaultAsync();

        if (existing != null)
            return 0; // Already blocked

        var blocked = new BlockedAddress
        {
            Address = address,
            Handle = handle,
            Reason = reason,
            BlockedDate = DateTime.UtcNow
        };

        // Mark existing messages and files as blocked
        await db.ExecuteAsync("UPDATE Messages SET IsBlocked = 1 WHERE FromAddress = ?", address);
        await db.ExecuteAsync("UPDATE IndexedFiles SET IsBlocked = 1 WHERE FromAddress = ?", address);

        return await db.InsertAsync(blocked);
    }

    public async Task<int> UnblockAddressAsync(string address)
    {
        var db = await GetDatabaseAsync();
        var blocked = await db.Table<BlockedAddress>()
            .Where(b => b.Address == address)
            .FirstOrDefaultAsync();

        if (blocked != null)
        {
            // Unmark messages and files
            await db.ExecuteAsync("UPDATE Messages SET IsBlocked = 0 WHERE FromAddress = ?", address);
            await db.ExecuteAsync("UPDATE IndexedFiles SET IsBlocked = 0 WHERE FromAddress = ?", address);
            
            return await db.DeleteAsync(blocked);
        }
        return 0;
    }

    // Clear all data
    public async Task ClearAllDataAsync()
    {
        var db = await GetDatabaseAsync();
        
        // Delete all local files
        var files = await db.Table<IndexedFile>().ToListAsync();
        foreach (var file in files)
        {
            if (File.Exists(file.LocalPath))
            {
                try { File.Delete(file.LocalPath); } catch { }
            }
        }

        var messages = await db.Table<IndexedMessage>().ToListAsync();
        foreach (var message in messages)
        {
            if (!string.IsNullOrEmpty(message.LocalFilePath) && File.Exists(message.LocalFilePath))
            {
                try { File.Delete(message.LocalFilePath); } catch { }
            }
        }

        // Clear database tables
        await db.DeleteAllAsync<IndexedMessage>();
        await db.DeleteAllAsync<IndexedFile>();
        await db.DeleteAllAsync<BlockedAddress>();
        await db.DeleteAllAsync<PlaylistItem>();
        await db.DeleteAllAsync<Playlist>();
    }

    // Playlist Methods
    public async Task<List<Playlist>> GetPlaylistsAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Playlist>()
            .OrderByDescending(p => p.ModifiedDate)
            .ToListAsync();
    }

    public async Task<Playlist?> GetPlaylistByIdAsync(int id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Playlist>()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreatePlaylistAsync(string name, string type = "mixed", string? description = null)
    {
        var db = await GetDatabaseAsync();
        var playlist = new Playlist
        {
            Name = name,
            Type = type,
            Description = description,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            ItemCount = 0
        };
        return await db.InsertAsync(playlist);
    }

    public async Task<int> UpdatePlaylistAsync(Playlist playlist)
    {
        var db = await GetDatabaseAsync();
        playlist.ModifiedDate = DateTime.UtcNow;
        return await db.UpdateAsync(playlist);
    }

    public async Task<int> DeletePlaylistAsync(int playlistId)
    {
        var db = await GetDatabaseAsync();
        
        // Delete all playlist items first
        await db.ExecuteAsync("DELETE FROM PlaylistItems WHERE PlaylistId = ?", playlistId);
        
        // Delete the playlist
        var playlist = await GetPlaylistByIdAsync(playlistId);
        if (playlist != null)
        {
            return await db.DeleteAsync(playlist);
        }
        return 0;
    }

    // Playlist Item Methods
    public async Task<List<PlaylistItem>> GetPlaylistItemsAsync(int playlistId)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<PlaylistItem>()
            .Where(i => i.PlaylistId == playlistId)
            .OrderBy(i => i.OrderIndex)
            .ToListAsync();
    }

    public async Task<int> AddToPlaylistAsync(int playlistId, IndexedFile file)
    {
        var db = await GetDatabaseAsync();
        
        // Get current max order index
        var maxOrder = await db.Table<PlaylistItem>()
            .Where(i => i.PlaylistId == playlistId)
            .OrderByDescending(i => i.OrderIndex)
            .Select(i => i.OrderIndex)
            .FirstOrDefaultAsync();

        var item = new PlaylistItem
        {
            PlaylistId = playlistId,
            IpfsHash = file.IpfsHash,
            FileName = file.FileName,
            LocalPath = file.LocalPath,
            MediaType = file.FileType,
            OrderIndex = maxOrder + 1,
            TransactionId = file.TransactionId,
            AddedDate = DateTime.UtcNow
        };

        var result = await db.InsertAsync(item);

        // Update playlist item count
        var playlist = await GetPlaylistByIdAsync(playlistId);
        if (playlist != null)
        {
            playlist.ItemCount++;
            await UpdatePlaylistAsync(playlist);
        }

        return result;
    }

    public async Task<int> RemoveFromPlaylistAsync(int playlistItemId)
    {
        var db = await GetDatabaseAsync();
        var item = await db.Table<PlaylistItem>()
            .Where(i => i.Id == playlistItemId)
            .FirstOrDefaultAsync();

        if (item != null)
        {
            var result = await db.DeleteAsync(item);

            // Update playlist item count
            var playlist = await GetPlaylistByIdAsync(item.PlaylistId);
            if (playlist != null && playlist.ItemCount > 0)
            {
                playlist.ItemCount--;
                await UpdatePlaylistAsync(playlist);
            }

            // Reorder remaining items
            var items = await GetPlaylistItemsAsync(item.PlaylistId);
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OrderIndex = i;
                await db.UpdateAsync(items[i]);
            }

            return result;
        }
        return 0;
    }

    public async Task<int> ReorderPlaylistItemAsync(int playlistItemId, int newIndex)
    {
        var db = await GetDatabaseAsync();
        var item = await db.Table<PlaylistItem>()
            .Where(i => i.Id == playlistItemId)
            .FirstOrDefaultAsync();

        if (item != null)
        {
            var oldIndex = item.OrderIndex;
            var items = await GetPlaylistItemsAsync(item.PlaylistId);

            // Shift items
            if (newIndex < oldIndex)
            {
                // Moving up
                foreach (var i in items.Where(x => x.OrderIndex >= newIndex && x.OrderIndex < oldIndex))
                {
                    i.OrderIndex++;
                    await db.UpdateAsync(i);
                }
            }
            else if (newIndex > oldIndex)
            {
                // Moving down
                foreach (var i in items.Where(x => x.OrderIndex > oldIndex && x.OrderIndex <= newIndex))
                {
                    i.OrderIndex--;
                    await db.UpdateAsync(i);
                }
            }

            item.OrderIndex = newIndex;
            return await db.UpdateAsync(item);
        }
        return 0;
    }

    // Encryption helpers for sensitive data
    public static string EncryptString(string plainText, string key)
    {
        using var aes = Aes.Create();
        var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        aes.Key = keyBytes;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var msEncrypt = new MemoryStream();
        msEncrypt.Write(aes.IV, 0, aes.IV.Length);
        
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public static string DecryptString(string cipherText, string key)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        aes.Key = keyBytes;

        var iv = new byte[aes.IV.Length];
        var cipher = new byte[fullCipher.Length - iv.Length];

        Array.Copy(fullCipher, iv, iv.Length);
        Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var msDecrypt = new MemoryStream(cipher);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
}
