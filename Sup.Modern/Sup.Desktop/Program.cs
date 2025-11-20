using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sup.Web.Components;

namespace Sup.Desktop;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════╗");
        Console.WriteLine("║         Sup!? Desktop Launcher         ║");
        Console.WriteLine("║  Decentralized Social Network          ║");
        Console.WriteLine("╚═══════════════════════════════════════╝");
        Console.WriteLine();
        
        string url = "http://localhost:5555";
        
        // Set environment variable for URL
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", url);
        
        Console.WriteLine($"Starting Sup!? Web Server on {url}...");
        
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        Console.WriteLine("✓ Server started successfully!");
        Console.WriteLine($"✓ Opening browser at {url}");
        Console.WriteLine();
        Console.WriteLine("Press Ctrl+C to shutdown...");
        Console.WriteLine();
        
        // Open the browser
        OpenBrowser(url);
        
        await app.RunAsync();
    }

    static void OpenBrowser(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Could not open browser automatically: {ex.Message}");
            Console.WriteLine($"Please open your browser manually and navigate to: {url}");
        }
    }
}
