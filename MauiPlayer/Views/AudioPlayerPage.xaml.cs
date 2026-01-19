using MauiPlayer.ViewModels;

namespace MauiPlayer.Views;

public partial class AudioPlayerPage : ContentPage
{
    public AudioPlayerPage(AudioPlayerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
