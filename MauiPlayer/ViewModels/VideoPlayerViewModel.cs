using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPlayer.Models;
using MauiPlayer.Services;
using System.Collections.ObjectModel;

namespace MauiPlayer.ViewModels;

/// <summary>
/// ViewModel for Video Player - optimized for small screens
/// </summary>
public partial class VideoPlayerViewModel : ObservableObject
{
    private readonly DataStorageService _storage;

    [ObservableProperty]
    private ObservableCollection<IndexedFile> _videoFiles = new();

    [ObservableProperty]
    private IndexedFile? _currentVideo;

    [ObservableProperty]
    private int _currentIndex = -1;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private string _statusText = "No video loaded";

    [ObservableProperty]
    private bool _isLoading;

    public VideoPlayerViewModel(DataStorageService storage)
    {
        _storage = storage;
        _ = LoadVideoFilesAsync();
    }

    private async Task LoadVideoFilesAsync()
    {
        try
        {
            IsLoading = true;
            var files = await _storage.GetRecentFilesAsync(100);
            
            VideoFiles.Clear();
            foreach (var file in files.Where(f => f.FileType == "video"))
            {
                VideoFiles.Add(file);
            }

            StatusText = $"{VideoFiles.Count} video files";
            
            if (VideoFiles.Count > 0 && CurrentVideo == null)
            {
                CurrentIndex = 0;
                CurrentVideo = VideoFiles[0];
                StatusText = CurrentVideo.FileName;
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
    private void PlayPause()
    {
        IsPlaying = !IsPlaying;
        StatusText = IsPlaying ? "Playing..." : "Paused";
    }

    [RelayCommand]
    private void Next()
    {
        if (VideoFiles.Count == 0) return;

        CurrentIndex++;
        if (CurrentIndex >= VideoFiles.Count)
            CurrentIndex = 0;

        CurrentVideo = VideoFiles[CurrentIndex];
        StatusText = CurrentVideo.FileName;
        IsPlaying = true;
    }

    [RelayCommand]
    private void Previous()
    {
        if (VideoFiles.Count == 0) return;

        CurrentIndex--;
        if (CurrentIndex < 0)
            CurrentIndex = VideoFiles.Count - 1;

        CurrentVideo = VideoFiles[CurrentIndex];
        StatusText = CurrentVideo.FileName;
        IsPlaying = true;
    }

    [RelayCommand]
    private async Task SelectVideoAsync(IndexedFile video)
    {
        var index = VideoFiles.IndexOf(video);
        if (index >= 0)
        {
            CurrentIndex = index;
            CurrentVideo = video;
            StatusText = video.FileName;
            IsPlaying = true;
        }
    }

    [RelayCommand]
    private async Task AddToPlaylistAsync(IndexedFile file)
    {
        try
        {
            // Get or create default video playlist
            var playlists = await _storage.GetPlaylistsAsync();
            var videoPlaylist = playlists.FirstOrDefault(p => p.Type == "video");
            
            if (videoPlaylist == null)
            {
                var playlistId = await _storage.CreatePlaylistAsync("My Videos", "video", "Default video playlist");
                videoPlaylist = await _storage.GetPlaylistByIdAsync(playlistId);
            }

            if (videoPlaylist != null)
            {
                await _storage.AddToPlaylistAsync(videoPlaylist.Id, file);
                StatusText = $"Added to {videoPlaylist.Name}";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadVideoFilesAsync();
    }
}
