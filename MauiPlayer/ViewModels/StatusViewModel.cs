using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPlayer.Models;
using MauiPlayer.Services;
using System.Collections.ObjectModel;

namespace MauiPlayer.ViewModels;

/// <summary>
/// ViewModel for Status Page - displays latest indexed files and messages
/// </summary>
public partial class StatusViewModel : ObservableObject
{
    private readonly DataStorageService _storage;
    private readonly TransactionMonitorService _monitor;
    private readonly BitcoinService _bitcoin;

    [ObservableProperty]
    private string _statusText = "Initializing...";

    [ObservableProperty]
    private bool _isMonitoring;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private int _messageCount;

    [ObservableProperty]
    private int _fileCount;

    [ObservableProperty]
    private int _blockHeight;

    [ObservableProperty]
    private ObservableCollection<IndexedMessage> _recentMessages = new();

    [ObservableProperty]
    private ObservableCollection<IndexedFile> _recentFiles = new();

    public StatusViewModel(
        DataStorageService storage,
        TransactionMonitorService monitor,
        BitcoinService bitcoin)
    {
        _storage = storage;
        _monitor = monitor;
        _bitcoin = bitcoin;

        // Subscribe to monitoring events
        _monitor.StatusChanged += OnMonitoringStatusChanged;
        _monitor.NewMessageIndexed += OnNewMessageIndexed;

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await RefreshDataAsync();
    }

    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        try
        {
            StatusText = "Loading...";

            // Load recent messages
            var messages = await _storage.GetRecentMessagesAsync(20);
            RecentMessages.Clear();
            foreach (var msg in messages)
            {
                RecentMessages.Add(msg);
            }
            MessageCount = RecentMessages.Count;

            // Load recent files
            var files = await _storage.GetRecentFilesAsync(20);
            RecentFiles.Clear();
            foreach (var file in files)
            {
                RecentFiles.Add(file);
            }
            FileCount = RecentFiles.Count;

            // Check Bitcoin connection
            IsConnected = _bitcoin.IsConnected;
            if (IsConnected)
            {
                BlockHeight = await _bitcoin.GetBlockCountAsync();
                StatusText = $"Connected to testnet3 at block {BlockHeight}";
            }
            else
            {
                StatusText = "Not connected to Bitcoin";
            }

            IsMonitoring = _monitor.IsMonitoring;
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ToggleMonitoringAsync()
    {
        try
        {
            if (_monitor.IsMonitoring)
            {
                await _monitor.StopMonitoringAsync();
            }
            else
            {
                await _monitor.StartMonitoringAsync();
            }

            IsMonitoring = _monitor.IsMonitoring;
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ConnectToBitcoinAsync()
    {
        try
        {
            StatusText = "Connecting to Bitcoin testnet3...";
            var connected = await _bitcoin.ConnectAsync();
            
            if (connected)
            {
                IsConnected = true;
                BlockHeight = await _bitcoin.GetBlockCountAsync();
                StatusText = $"Connected to testnet3 at block {BlockHeight}";
            }
            else
            {
                IsConnected = false;
                StatusText = $"Connection failed: {_bitcoin.LastError}";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
            IsConnected = false;
        }
    }

    [RelayCommand]
    private async Task DeleteMessageAsync(IndexedMessage message)
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Delete Message",
                "Are you sure you want to delete this message?",
                "Delete",
                "Cancel");

            if (confirm)
            {
                await _storage.DeleteMessageAsync(message.TransactionId);
                RecentMessages.Remove(message);
                MessageCount = RecentMessages.Count;
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteFileAsync(IndexedFile file)
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Delete File",
                "Are you sure you want to delete this file?",
                "Delete",
                "Cancel");

            if (confirm)
            {
                await _storage.DeleteFileAsync(file.IpfsHash);
                RecentFiles.Remove(file);
                FileCount = RecentFiles.Count;
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    private void OnMonitoringStatusChanged(object? sender, string status)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            StatusText = status;
        });
    }

    private void OnNewMessageIndexed(object? sender, IndexedMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            RecentMessages.Insert(0, message);
            MessageCount = RecentMessages.Count;
            
            // Keep only latest 20
            while (RecentMessages.Count > 20)
            {
                RecentMessages.RemoveAt(RecentMessages.Count - 1);
            }
        });
    }
}
