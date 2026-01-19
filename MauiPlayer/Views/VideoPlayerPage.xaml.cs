using MauiPlayer.ViewModels;

namespace MauiPlayer.Views;

public partial class VideoPlayerPage : ContentPage
{
    public VideoPlayerPage(VideoPlayerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
