# Sup!? Modern - Cross-Platform Decentralized Social Network

## Overview

**Sup!? Modern** is a complete redesign and modernization of the Sup application, migrated from .NET Framework 4.7.2 Windows Forms to **.NET 8 Blazor Hybrid**, providing a modern web-like experience while running natively on Windows, Linux, and macOS.

## Architecture

The application is built using a modern, modular architecture:

### Project Structure

```
Sup.Modern/
├── Sup.Core/              # Cross-platform business logic
│   ├── P2FK/             # P2FK blockchain protocol implementation
│   │   ├── Classes/      # Core cryptographic and RPC classes
│   │   └── Contracts/    # Smart contract interfaces
│   ├── Models/           # Data models
│   └── Services/         # Service interfaces
│
├── Sup.Web/              # Blazor web application
│   ├── Components/       # Blazor components
│   │   ├── Layout/      # Application layouts
│   │   └── Pages/       # Application pages
│   └── wwwroot/         # Static assets
│
└── Sup.Desktop/          # Desktop launcher
    └── Program.cs       # Entry point that hosts the web app
```

### Technology Stack

- **.NET 8.0** - Latest LTS version with modern C# features
- **Blazor Server** - Interactive web UI with server-side rendering
- **ASP.NET Core** - High-performance web framework
- **Modern Cryptography** - Using .NET 8's native crypto APIs
- **Async/Await** - All I/O operations are async for better performance

## Key Features

### Modern UI/UX

- **Completely Redesigned Interface** - Modern, intuitive web-like experience
- **Responsive Design** - Works on all screen sizes
- **Real-time Updates** - Powered by Blazor Server's SignalR connection
- **Modern Animations** - Smooth transitions and interactions

### Core Functionality

- **Home Feed** - Social media-style feed with posts, likes, comments
- **Explore** - Browse objects, profiles, and collections
- **Messages** - Public and private messaging with encryption
- **Profile Management** - Create and manage blockchain profiles
- **Object Minting** - Create NFT-like objects on the blockchain
- **Media Player** - Audio and video playback (JukeBox, SupFlix)
- **Live Feed** - Real-time blockchain activity monitoring
- **Settings** - Configure blockchain connections and preferences

### Cross-Platform Support

- **Windows** - Native exe launcher
- **Linux** - Native executable
- **macOS** - Native app for Intel and Apple Silicon

## Building and Running

### Prerequisites

- .NET 8.0 SDK or later
- A blockchain node (Bitcoin, Litecoin, Dogecoin, or Mazacoin)
- IPFS Kubo daemon (optional, for distributed storage)

### Build

```bash
cd Sup.Modern
dotnet build
```

### Run

```bash
cd Sup.Modern/Sup.Desktop
dotnet run
```

The application will:
1. Start a local web server on http://localhost:5555
2. Automatically open your default browser
3. Display the modern Sup!? interface

### Publish for Distribution

#### Windows
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

#### Linux
```bash
dotnet publish -c Release -r linux-x64 --self-contained
```

#### macOS (Intel)
```bash
dotnet publish -c Release -r osx-x64 --self-contained
```

#### macOS (Apple Silicon)
```bash
dotnet publish -c Release -r osx-arm64 --self-contained
```

## Migration from Legacy Version

### What's New

1. **Framework**: .NET Framework 4.7.2 → .NET 8.0
2. **UI**: Windows Forms → Blazor Server
3. **Architecture**: Monolithic → Modular (Core + Web + Desktop)
4. **Async**: Synchronous I/O → Async/await throughout
5. **Cryptography**: Custom implementations → Modern .NET crypto APIs
6. **HTTP**: HttpWebRequest → HttpClient
7. **Design**: Windows 98 style → Modern web UI

### Backend Compatibility

The P2FK blockchain protocol remains fully compatible with the original version:
- Same RPC interface
- Same cryptographic algorithms
- Same blockchain data format
- Same IPFS integration

### Configuration Migration

Legacy blockchain connection settings can be migrated to the new Settings page:
- Navigate to Settings → Blockchain Connections
- Enter your RPC credentials
- Test the connection
- Save settings

## Development

### Project Layout

- **Sup.Core** - Platform-independent business logic, can be shared with mobile apps
- **Sup.Web** - Blazor components and pages, reusable across platforms
- **Sup.Desktop** - Lightweight launcher that hosts the web app

### Adding New Features

1. **New Page**: Add to `Sup.Web/Components/Pages/`
2. **New Service**: Define interface in `Sup.Core/Services/`
3. **New Model**: Add to `Sup.Core/Models/`
4. **Navigation**: Update `SupLayout.razor`

### Styling

Each Blazor component can have its own CSS file:
- `ComponentName.razor` - Component markup and logic
- `ComponentName.razor.css` - Scoped component styles

Global styles are in `wwwroot/app.css`.

## Configuration

Configuration is stored in `appsettings.json` and can be overridden with environment variables or user settings.

### Blockchain Configuration

Edit `Settings` page or modify `appsettings.json`:

```json
{
  "Blockchains": {
    "BitcoinTestnet": {
      "RpcUrl": "http://127.0.0.1:18332",
      "Username": "your-username",
      "Password": "your-password",
      "Enabled": true
    }
  }
}
```

### IPFS Configuration

```json
{
  "IPFS": {
    "ApiUrl": "http://127.0.0.1:5001",
    "GatewayUrl": "http://127.0.0.1:8080",
    "AutoStart": true,
    "AutoPin": true
  }
}
```

## Roadmap

### Phase 1: Core Migration ✓
- [x] Project structure and build system
- [x] Modern UI layout and navigation
- [x] Home page with feed
- [x] Explore page
- [x] Settings page
- [x] Desktop launcher

### Phase 2: Backend Integration (In Progress)
- [ ] Blockchain RPC service implementation
- [ ] IPFS service implementation
- [ ] Wallet service implementation
- [ ] Object minting functionality
- [ ] Profile management
- [ ] Message system

### Phase 3: Advanced Features
- [ ] Live feed monitoring
- [ ] Media player (audio/video)
- [ ] Poll/inquiry system
- [ ] Search functionality
- [ ] Trading marketplace

### Phase 4: Polish & Distribution
- [ ] Error handling and logging
- [ ] Performance optimization
- [ ] Installer packages
- [ ] Documentation
- [ ] User guides

## Contributing

Contributions are welcome! This modernization effort is ongoing.

## License

Same as original Sup!? project.

## Credits

- Original Sup!? by embii
- Modernization and redesign: .NET 8 Blazor migration
- P2FK Protocol: embii (2013)
- Based on concepts from Satoshi Uploader

---

**Note**: This is a modern reimplementation. While maintaining protocol compatibility with the original version, it provides a completely new user experience optimized for modern operating systems and development practices.
