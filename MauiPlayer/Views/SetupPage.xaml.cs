using MauiPlayer.ViewModels;

namespace MauiPlayer.Views;

public partial class SetupPage : ContentPage
{
    public SetupPage(SetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
