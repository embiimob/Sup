using MauiPlayer.ViewModels;

namespace MauiPlayer.Views;

public partial class StatusPage : ContentPage
{
    public StatusPage(StatusViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
