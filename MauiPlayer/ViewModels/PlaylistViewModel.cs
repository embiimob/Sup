using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPlayer.Models;
using MauiPlayer.Services;
using System.Collections.ObjectModel;

namespace MauiPlayer.ViewModels;

/// <summary>
/// ViewModel for Playlist Management
/// </summary>
public partial class PlaylistViewModel : ObservableObject
{
    private readonly DataStorageService _storage;

    [ObservableProperty]
    private ObservableCollection<Playlist> _playlists = new();

    [ObservableProperty]
    private Playlist? _selectedPlaylist;

    [ObservableProperty]
    private ObservableCollection<PlaylistItem> _playlistItems = new();

    [ObservableProperty]
    private PlaylistItem? _currentItem;

    [ObservableProperty]
    private int _currentIndex = -1;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private string _statusText = "No playlists";

    [ObservableProperty]
    private bool _isLoading;

    public PlaylistViewModel(DataStorageService storage)
    {
        _storage = storage;
        _ = LoadPlaylistsAsync();
    }

    private async Task LoadPlaylistsAsync()
    {
        try
        {
            IsLoading = true;
            var lists = await _storage.GetPlaylistsAsync();
            
            Playlists.Clear();
            foreach (var list in lists)
            {
                Playlists.Add(list);
            }

            StatusText = $"{Playlists.Count} playlists";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CreatePlaylistAsync()
    {
        try
        {
            var name = await Application.Current!.MainPage!.DisplayPromptAsync(
                "New Playlist",
                "Enter playlist name:",
                placeholder: "My Playlist");

            if (!string.IsNullOrWhiteSpace(name))
            {
                var type = await Application.Current!.MainPage!.DisplayActionSheet(
                    "Playlist Type",
                    "Cancel",
                    null,
                    "Audio", "Video", "Mixed");

                if (type != "Cancel" && !string.IsNullOrEmpty(type))
                {
                    var playlistId = await _storage.CreatePlaylistAsync(
                        name,
                        type.ToLower(),
                        $"{type} playlist");

                    await LoadPlaylistsAsync();
                    StatusText = $"Created: {name}";
                }
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SelectPlaylistAsync(Playlist playlist)
    {
        try
        {
            SelectedPlaylist = playlist;
            IsLoading = true;

            var items = await _storage.GetPlaylistItemsAsync(playlist.Id);
            
            PlaylistItems.Clear();
            foreach (var item in items)
            {
                PlaylistItems.Add(item);
            }

            StatusText = $"{playlist.Name}: {items.Count} items";

            if (PlaylistItems.Count > 0)
            {
                CurrentIndex = 0;
                CurrentItem = PlaylistItems[0];
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeletePlaylistAsync(Playlist playlist)
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Delete Playlist",
                $"Delete '{playlist.Name}'?",
                "Delete",
                "Cancel");

            if (confirm)
            {
                await _storage.DeletePlaylistAsync(playlist.Id);
                Playlists.Remove(playlist);
                
                if (SelectedPlaylist?.Id == playlist.Id)
                {
                    SelectedPlaylist = null;
                    PlaylistItems.Clear();
                }

                StatusText = $"Deleted: {playlist.Name}";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task RemoveItemAsync(PlaylistItem item)
    {
        try
        {
            await _storage.RemoveFromPlaylistAsync(item.Id);
            PlaylistItems.Remove(item);

            if (SelectedPlaylist != null)
            {
                SelectedPlaylist.ItemCount--;
                StatusText = $"{SelectedPlaylist.Name}: {PlaylistItems.Count} items";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void PlayPause()
    {
        IsPlaying = !IsPlaying;
        StatusText = IsPlaying ? "Playing..." : "Paused";
    }

    [RelayCommand]
    private void Next()
    {
        if (PlaylistItems.Count == 0) return;

        CurrentIndex++;
        if (CurrentIndex >= PlaylistItems.Count)
            CurrentIndex = 0;

        CurrentItem = PlaylistItems[CurrentIndex];
        IsPlaying = true;
        StatusText = $"Playing: {CurrentItem.FileName}";
    }

    [RelayCommand]
    private void Previous()
    {
        if (PlaylistItems.Count == 0) return;

        CurrentIndex--;
        if (CurrentIndex < 0)
            CurrentIndex = PlaylistItems.Count - 1;

        CurrentItem = PlaylistItems[CurrentIndex];
        IsPlaying = true;
        StatusText = $"Playing: {CurrentItem.FileName}";
    }

    [RelayCommand]
    private async Task PlayItemAsync(PlaylistItem item)
    {
        var index = PlaylistItems.IndexOf(item);
        if (index >= 0)
        {
            CurrentIndex = index;
            CurrentItem = item;
            IsPlaying = true;
            StatusText = $"Playing: {item.FileName}";
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadPlaylistsAsync();
        if (SelectedPlaylist != null)
        {
            await SelectPlaylistAsync(SelectedPlaylist);
        }
    }
}
