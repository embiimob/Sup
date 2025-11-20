using Sup.Web.Components;
using Sup.Core.Services;
using Sup.Core.Models;
using Sup.Core.P2FK.Classes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register application services
builder.Services.AddSingleton<IIpfsService>(sp => 
{
    var ipfsService = new IpfsService();
    return ipfsService;
});

builder.Services.AddSingleton<IBlockchainService>(sp =>
{
    var blockchainService = new BlockchainService();
    
    // Load blockchain configurations from appsettings
    var configuration = sp.GetRequiredService<IConfiguration>();
    
    // Add Bitcoin Testnet configuration
    blockchainService.AddBlockchain("BitcoinTestnet", new BlockchainConfig
    {
        Name = "Bitcoin Testnet",
        ShortName = "BTC",
        RpcUrl = configuration["Blockchains:BitcoinTestnet:RpcUrl"] ?? "http://127.0.0.1:18332",
        Username = configuration["Blockchains:BitcoinTestnet:Username"] ?? "bitcoin-rpc",
        Password = configuration["Blockchains:BitcoinTestnet:Password"] ?? "",
        VersionByte = 111, // Testnet version byte
        Enabled = true
    });
    
    return blockchainService;
});

builder.Services.AddScoped<IWalletService>(sp =>
{
    // WalletService needs RPC clients, which we'll inject later
    var rpcClients = new Dictionary<string, BlockchainRpcService>();
    return new WalletService(rpcClients);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
