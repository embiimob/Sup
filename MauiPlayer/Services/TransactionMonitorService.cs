using MauiPlayer.Models;
using System.Diagnostics;

namespace MauiPlayer.Services;

/// <summary>
/// Service for monitoring Bitcoin testnet3 transactions
/// </summary>
public class TransactionMonitorService
{
    private readonly BitcoinService _bitcoinService;
    private readonly P2FKParserService _parserService;
    private readonly DataStorageService _storage;
    
    private CancellationTokenSource? _monitoringCts;
    private Task? _monitoringTask;
    private HashSet<string> _processedTxIds = new();

    public bool IsMonitoring { get; private set; }
    public event EventHandler<IndexedMessage>? NewMessageIndexed;
    public event EventHandler<string>? StatusChanged;

    public TransactionMonitorService(
        BitcoinService bitcoinService,
        P2FKParserService parserService,
        DataStorageService storage)
    {
        _bitcoinService = bitcoinService;
        _parserService = parserService;
        _storage = storage;
    }

    /// <summary>
    /// Start monitoring transactions
    /// </summary>
    public async Task StartMonitoringAsync()
    {
        if (IsMonitoring)
            return;

        var settings = await _storage.GetSettingsAsync();
        if (!settings.MonitoringEnabled)
        {
            StatusChanged?.Invoke(this, "Monitoring disabled in settings");
            return;
        }

        // Connect to Bitcoin
        if (!_bitcoinService.IsConnected)
        {
            var connected = await _bitcoinService.ConnectAsync();
            if (!connected)
            {
                StatusChanged?.Invoke(this, $"Failed to connect to Bitcoin: {_bitcoinService.LastError}");
                return;
            }
        }

        _monitoringCts = new CancellationTokenSource();
        IsMonitoring = true;

        _monitoringTask = Task.Run(async () =>
        {
            StatusChanged?.Invoke(this, "Monitoring started");
            await MonitorLoopAsync(_monitoringCts.Token);
        });
    }

    /// <summary>
    /// Stop monitoring transactions
    /// </summary>
    public async Task StopMonitoringAsync()
    {
        if (!IsMonitoring)
            return;

        _monitoringCts?.Cancel();
        
        if (_monitoringTask != null)
        {
            await _monitoringTask;
        }

        IsMonitoring = false;
        StatusChanged?.Invoke(this, "Monitoring stopped");
    }

    /// <summary>
    /// Main monitoring loop
    /// </summary>
    private async Task MonitorLoopAsync(CancellationToken cancellationToken)
    {
        var settings = await _storage.GetSettingsAsync();
        var interval = TimeSpan.FromSeconds(settings.MonitoringIntervalSeconds);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await MonitorNewTransactionsAsync(settings);
                await Task.Delay(interval, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Monitoring error: {ex.Message}");
                StatusChanged?.Invoke(this, $"Error: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }
    }

    /// <summary>
    /// Monitor new transactions from mempool and recent blocks
    /// </summary>
    private async Task MonitorNewTransactionsAsync(AppSettings settings)
    {
        // Get mempool transactions
        var mempoolTxIds = await _bitcoinService.GetRawMempoolAsync();
        
        // Get current block height
        var currentHeight = await _bitcoinService.GetBlockCountAsync();
        
        // Update last processed height if needed
        if (settings.LastProcessedBlockHeight == 0)
        {
            settings.LastProcessedBlockHeight = currentHeight;
            await _storage.SaveSettingsAsync(settings);
        }

        // Process new transactions from mempool
        foreach (var txid in mempoolTxIds)
        {
            if (_processedTxIds.Contains(txid))
                continue;

            await ProcessTransactionAsync(txid, settings);
        }

        // If monitoring specific addresses, fetch their recent transactions
        if (!string.IsNullOrEmpty(settings.MonitoredAddresses))
        {
            var addresses = settings.MonitoredAddresses.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var address in addresses)
            {
                await MonitorAddressAsync(address.Trim());
            }
        }

        // Clean up old processed transaction IDs (keep last 1000)
        if (_processedTxIds.Count > 1000)
        {
            var toRemove = _processedTxIds.Take(_processedTxIds.Count - 1000).ToList();
            foreach (var txid in toRemove)
            {
                _processedTxIds.Remove(txid);
            }
        }
    }

    /// <summary>
    /// Process a single transaction
    /// </summary>
    private async Task ProcessTransactionAsync(string txid, AppSettings settings)
    {
        try
        {
            _processedTxIds.Add(txid);

            var message = await _parserService.ParseAndIndexTransactionAsync(txid);
            if (message != null)
            {
                StatusChanged?.Invoke(this, $"New message indexed: {txid.Substring(0, 8)}...");
                NewMessageIndexed?.Invoke(this, message);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Process transaction error for {txid}: {ex.Message}");
        }
    }

    /// <summary>
    /// Monitor specific address for new transactions
    /// </summary>
    private async Task MonitorAddressAsync(string address)
    {
        try
        {
            // Get recent messages for this address
            var messages = await _parserService.ParseMessagesByAddressAsync(address, skip: 0, qty: 10);
            
            foreach (var message in messages)
            {
                if (!_processedTxIds.Contains(message.TransactionId))
                {
                    _processedTxIds.Add(message.TransactionId);
                    NewMessageIndexed?.Invoke(this, message);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Monitor address error for {address}: {ex.Message}");
        }
    }

    /// <summary>
    /// Perform full rescan of blockchain from last processed height
    /// </summary>
    public async Task RescanBlockchainAsync(int? fromHeight = null)
    {
        try
        {
            var settings = await _storage.GetSettingsAsync();
            var startHeight = fromHeight ?? settings.LastProcessedBlockHeight;
            var currentHeight = await _bitcoinService.GetBlockCountAsync();

            StatusChanged?.Invoke(this, $"Rescanning from block {startHeight} to {currentHeight}");

            // This is a simplified rescan - in production, you'd want to:
            // 1. Get block hash for each height
            // 2. Get all transactions in block
            // 3. Parse P2FK transactions
            // For now, we'll just scan monitored addresses

            if (!string.IsNullOrEmpty(settings.MonitoredAddresses))
            {
                var addresses = settings.MonitoredAddresses.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var address in addresses)
                {
                    await _parserService.ParseMessagesByAddressAsync(address.Trim(), skip: 0, qty: 100);
                }
            }

            settings.LastProcessedBlockHeight = currentHeight;
            await _storage.SaveSettingsAsync(settings);

            StatusChanged?.Invoke(this, $"Rescan complete");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Rescan error: {ex.Message}");
            StatusChanged?.Invoke(this, $"Rescan error: {ex.Message}");
        }
    }
}
