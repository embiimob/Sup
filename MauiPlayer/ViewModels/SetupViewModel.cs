using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPlayer.Models;
using MauiPlayer.Services;
using System.Collections.ObjectModel;

namespace MauiPlayer.ViewModels;

/// <summary>
/// ViewModel for Setup Page - configuration and settings
/// </summary>
public partial class SetupViewModel : ObservableObject
{
    private readonly DataStorageService _storage;
    private readonly BitcoinService _bitcoin;
    private readonly TransactionMonitorService _monitor;

    [ObservableProperty]
    private string _rpcUrl = "http://127.0.0.1:18332";

    [ObservableProperty]
    private string _rpcUsername = string.Empty;

    [ObservableProperty]
    private string _rpcPassword = string.Empty;

    [ObservableProperty]
    private string _versionByte = "111";

    [ObservableProperty]
    private string _ipfsGateway = "https://ipfs.io/ipfs/";

    [ObservableProperty]
    private bool _autoDownloadIpfs = true;

    [ObservableProperty]
    private int _maxAutoDownloadSizeMb = 10;

    [ObservableProperty]
    private string _monitoredAddresses = string.Empty;

    [ObservableProperty]
    private string _monitoredHandles = string.Empty;

    [ObservableProperty]
    private bool _monitoringEnabled = true;

    [ObservableProperty]
    private int _monitoringIntervalSeconds = 30;

    [ObservableProperty]
    private int _lastProcessedBlockHeight;

    [ObservableProperty]
    private ObservableCollection<BlockedAddress> _blockedAddresses = new();

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private bool _isSaving;

    public SetupViewModel(
        DataStorageService storage,
        BitcoinService bitcoin,
        TransactionMonitorService monitor)
    {
        _storage = storage;
        _bitcoin = bitcoin;
        _monitor = monitor;

        _ = LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            var settings = await _storage.GetSettingsAsync();

            RpcUrl = settings.RpcUrl;
            RpcUsername = settings.RpcUsername;
            // Don't load encrypted password for display
            VersionByte = settings.VersionByte;
            IpfsGateway = settings.IpfsGateway;
            AutoDownloadIpfs = settings.AutoDownloadIpfs;
            MaxAutoDownloadSizeMb = settings.MaxAutoDownloadSizeMb;
            MonitoredAddresses = settings.MonitoredAddresses ?? string.Empty;
            MonitoredHandles = settings.MonitoredHandles ?? string.Empty;
            MonitoringEnabled = settings.MonitoringEnabled;
            MonitoringIntervalSeconds = settings.MonitoringIntervalSeconds;
            LastProcessedBlockHeight = settings.LastProcessedBlockHeight;

            // Load blocked addresses
            var blocked = await _storage.GetBlockedAddressesAsync();
            BlockedAddresses.Clear();
            foreach (var addr in blocked)
            {
                BlockedAddresses.Add(addr);
            }

            StatusText = "Settings loaded";
        }
        catch (Exception ex)
        {
            StatusText = $"Error loading settings: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        try
        {
            IsSaving = true;
            StatusText = "Saving...";

            var settings = await _storage.GetSettingsAsync();

            settings.RpcUrl = RpcUrl;
            settings.RpcUsername = RpcUsername;
            
            // Encrypt password if changed
            if (!string.IsNullOrEmpty(RpcPassword))
            {
                var deviceId = $"{DeviceInfo.Name}_{DeviceInfo.Platform}_{DeviceInfo.Idiom}";
                settings.RpcPassword = DataStorageService.EncryptString(RpcPassword, deviceId);
            }

            settings.VersionByte = VersionByte;
            settings.IpfsGateway = IpfsGateway;
            settings.AutoDownloadIpfs = AutoDownloadIpfs;
            settings.MaxAutoDownloadSizeMb = MaxAutoDownloadSizeMb;
            settings.MonitoredAddresses = MonitoredAddresses;
            settings.MonitoredHandles = MonitoredHandles;
            settings.MonitoringEnabled = MonitoringEnabled;
            settings.MonitoringIntervalSeconds = MonitoringIntervalSeconds;

            await _storage.SaveSettingsAsync(settings);

            StatusText = "Settings saved successfully";

            // Restart monitoring if it was running
            if (_monitor.IsMonitoring)
            {
                await _monitor.StopMonitoringAsync();
                await _monitor.StartMonitoringAsync();
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error saving settings: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        try
        {
            StatusText = "Testing connection...";

            // Temporarily save settings for connection test
            await SaveSettingsAsync();

            var connected = await _bitcoin.ConnectAsync();
            if (connected)
            {
                var blockCount = await _bitcoin.GetBlockCountAsync();
                StatusText = $"Connected! Current block: {blockCount}";
            }
            else
            {
                StatusText = $"Connection failed: {_bitcoin.LastError}";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Connection error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task UnblockAddressAsync(BlockedAddress blockedAddress)
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Unblock Address",
                $"Unblock {blockedAddress.Address}?",
                "Unblock",
                "Cancel");

            if (confirm)
            {
                await _storage.UnblockAddressAsync(blockedAddress.Address);
                BlockedAddresses.Remove(blockedAddress);
                StatusText = "Address unblocked";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ClearAllDataAsync()
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Clear All Data",
                "This will delete all indexed messages, files, and cached data. This action cannot be undone. Continue?",
                "Clear All",
                "Cancel");

            if (confirm)
            {
                var doubleConfirm = await Application.Current!.MainPage!.DisplayAlert(
                    "Are You Sure?",
                    "Really delete all data?",
                    "Yes, Delete All",
                    "Cancel");

                if (doubleConfirm)
                {
                    await _storage.ClearAllDataAsync();
                    StatusText = "All data cleared";
                    
                    // Reload settings to reset UI
                    await LoadSettingsAsync();
                }
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task RescanBlockchainAsync()
    {
        try
        {
            StatusText = "Starting blockchain rescan...";
            await _monitor.RescanBlockchainAsync();
            StatusText = "Rescan complete";
        }
        catch (Exception ex)
        {
            StatusText = $"Rescan error: {ex.Message}";
        }
    }
}
