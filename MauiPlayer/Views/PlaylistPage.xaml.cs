using MauiPlayer.ViewModels;

namespace MauiPlayer.Views;

public partial class PlaylistPage : ContentPage
{
    private readonly PlaylistViewModel _viewModel;

    public PlaylistPage(PlaylistViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private void OnBackToPlaylistsClicked(object sender, EventArgs e)
    {
        _viewModel.SelectedPlaylist = null;
        _viewModel.PlaylistItems.Clear();
        _viewModel.StatusText = $"{_viewModel.Playlists.Count} playlists";
    }
}
