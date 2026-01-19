using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPlayer.Models;
using MauiPlayer.Services;
using System.Collections.ObjectModel;

namespace MauiPlayer.ViewModels;

/// <summary>
/// ViewModel for Search Page - search messages and files
/// </summary>
public partial class SearchViewModel : ObservableObject
{
    private readonly DataStorageService _storage;
    private readonly P2FKParserService _parser;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string _searchType = "Address";

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private string _statusText = "Enter an address or handle to search";

    [ObservableProperty]
    private ObservableCollection<IndexedMessage> _searchResults = new();

    [ObservableProperty]
    private int _resultCount;

    public List<string> SearchTypes { get; } = new() { "Address", "Handle", "Transaction" };

    public SearchViewModel(DataStorageService storage, P2FKParserService parser)
    {
        _storage = storage;
        _parser = parser;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            StatusText = "Please enter a search query";
            return;
        }

        try
        {
            IsSearching = true;
            StatusText = "Searching...";
            SearchResults.Clear();

            List<IndexedMessage> results;

            switch (SearchType)
            {
                case "Address":
                    results = await SearchByAddressAsync(SearchQuery.Trim());
                    break;

                case "Handle":
                    results = await SearchByHandleAsync(SearchQuery.Trim());
                    break;

                case "Transaction":
                    results = await SearchByTransactionAsync(SearchQuery.Trim());
                    break;

                default:
                    results = new List<IndexedMessage>();
                    break;
            }

            foreach (var result in results)
            {
                SearchResults.Add(result);
            }

            ResultCount = SearchResults.Count;
            StatusText = $"Found {ResultCount} result(s)";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsSearching = false;
        }
    }

    private async Task<List<IndexedMessage>> SearchByAddressAsync(string address)
    {
        // First check local database
        var localResults = await _storage.SearchMessagesByAddressAsync(address);

        // If no local results, fetch from blockchain
        if (localResults.Count == 0)
        {
            StatusText = "Fetching from blockchain...";
            var blockchainResults = await _parser.ParseMessagesByAddressAsync(address, skip: 0, qty: 50);
            return blockchainResults;
        }

        return localResults;
    }

    private async Task<List<IndexedMessage>> SearchByHandleAsync(string handle)
    {
        // Remove @ if present
        handle = handle.TrimStart('@');

        var results = await _storage.SearchMessagesByHandleAsync(handle);
        
        // TODO: Could also fetch from blockchain using handle resolution
        
        return results;
    }

    private async Task<List<IndexedMessage>> SearchByTransactionAsync(string transactionId)
    {
        var result = await _parser.ParseAndIndexTransactionAsync(transactionId);
        
        if (result != null)
        {
            return new List<IndexedMessage> { result };
        }

        return new List<IndexedMessage>();
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
                SearchResults.Remove(message);
                ResultCount = SearchResults.Count;
                StatusText = $"Deleted. {ResultCount} result(s) remaining";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task BlockAddressAsync(IndexedMessage message)
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Block Address",
                $"Block all content from {message.FromAddress}?",
                "Block",
                "Cancel");

            if (confirm)
            {
                var reason = await Application.Current!.MainPage!.DisplayPromptAsync(
                    "Block Reason",
                    "Enter reason for blocking (optional):",
                    placeholder: "Reason");

                await _storage.BlockAddressAsync(message.FromAddress, message.Handle, reason);
                
                // Remove messages from this address
                var toRemove = SearchResults.Where(m => m.FromAddress == message.FromAddress).ToList();
                foreach (var msg in toRemove)
                {
                    SearchResults.Remove(msg);
                }

                ResultCount = SearchResults.Count;
                StatusText = $"Address blocked. {ResultCount} result(s) remaining";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClearResults()
    {
        SearchResults.Clear();
        ResultCount = 0;
        StatusText = "Results cleared";
    }
}
