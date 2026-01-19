using Microsoft.Extensions.Logging;
using MauiPlayer.Services;
using MauiPlayer.ViewModels;
using MauiPlayer.Views;

namespace MauiPlayer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services
        builder.Services.AddSingleton<BitcoinService>();
        builder.Services.AddSingleton<IpfsService>();
        builder.Services.AddSingleton<DataStorageService>();
        builder.Services.AddSingleton<TransactionMonitorService>();
        builder.Services.AddSingleton<P2FKParserService>();

        // Register ViewModels
        builder.Services.AddTransient<StatusViewModel>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<SetupViewModel>();
        builder.Services.AddTransient<AudioPlayerViewModel>();
        builder.Services.AddTransient<VideoPlayerViewModel>();
        builder.Services.AddTransient<PlaylistViewModel>();

        // Register Views
        builder.Services.AddTransient<StatusPage>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<SetupPage>();
        builder.Services.AddTransient<AudioPlayerPage>();
        builder.Services.AddTransient<VideoPlayerPage>();
        builder.Services.AddTransient<PlaylistPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

