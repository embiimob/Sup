using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPlayer.Models;
using MauiPlayer.Services;
using System.Collections.ObjectModel;

namespace MauiPlayer.ViewModels;

/// <summary>
/// ViewModel for Audio Player - optimized for small screens
/// </summary>
public partial class AudioPlayerViewModel : ObservableObject
{
    private readonly DataStorageService _storage;

    [ObservableProperty]
    private ObservableCollection<IndexedFile> _audioFiles = new();

    [ObservableProperty]
    private IndexedFile? _currentTrack;

    [ObservableProperty]
    private int _currentIndex = -1;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private string _statusText = "No audio loaded";

    [ObservableProperty]
    private string _currentTime = "0:00";

    [ObservableProperty]
    private string _totalTime = "0:00";

    [ObservableProperty]
    private bool _isLoading;

    public AudioPlayerViewModel(DataStorageService storage)
    {
        _storage = storage;
        _ = LoadAudioFilesAsync();
    }

    private async Task LoadAudioFilesAsync()
    {
        try
        {
            IsLoading = true;
            var files = await _storage.GetRecentFilesAsync(100);
            
            AudioFiles.Clear();
            foreach (var file in files.Where(f => f.FileType == "audio"))
            {
                AudioFiles.Add(file);
            }

            StatusText = $"{AudioFiles.Count} audio files";
            
            if (AudioFiles.Count > 0 && CurrentTrack == null)
            {
                CurrentIndex = 0;
                CurrentTrack = AudioFiles[0];
                StatusText = CurrentTrack.FileName;
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
        if (AudioFiles.Count == 0) return;

        CurrentIndex++;
        if (CurrentIndex >= AudioFiles.Count)
            CurrentIndex = 0;

        CurrentTrack = AudioFiles[CurrentIndex];
        StatusText = CurrentTrack.FileName;
        IsPlaying = true;
    }

    [RelayCommand]
    private void Previous()
    {
        if (AudioFiles.Count == 0) return;

        CurrentIndex--;
        if (CurrentIndex < 0)
            CurrentIndex = AudioFiles.Count - 1;

        CurrentTrack = AudioFiles[CurrentIndex];
        StatusText = CurrentTrack.FileName;
        IsPlaying = true;
    }

    [RelayCommand]
    private async Task SelectTrackAsync(IndexedFile track)
    {
        var index = AudioFiles.IndexOf(track);
        if (index >= 0)
        {
            CurrentIndex = index;
            CurrentTrack = track;
            StatusText = track.FileName;
            IsPlaying = true;
        }
    }

    [RelayCommand]
    private async Task AddToPlaylistAsync(IndexedFile file)
    {
        try
        {
            // Get or create default audio playlist
            var playlists = await _storage.GetPlaylistsAsync();
            var audioPlaylist = playlists.FirstOrDefault(p => p.Type == "audio");
            
            if (audioPlaylist == null)
            {
                var playlistId = await _storage.CreatePlaylistAsync("My Audio", "audio", "Default audio playlist");
                audioPlaylist = await _storage.GetPlaylistByIdAsync(playlistId);
            }

            if (audioPlaylist != null)
            {
                await _storage.AddToPlaylistAsync(audioPlaylist.Id, file);
                StatusText = $"Added to {audioPlaylist.Name}";
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
        await LoadAudioFilesAsync();
    }
}
