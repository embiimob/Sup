# P2FK Mini Player - .NET MAUI Application

A minimalist, cross-platform media player application for viewing and playing P2FK messages, audio, and video files from the Bitcoin testnet3 blockchain and IPFS network.

## Features

### Core Functionality
- **Direct Bitcoin testnet3 integration**: Monitor blockchain transactions without third-party services
- **P2FK message parsing**: Extract and index messages from Bitcoin transactions
- **IPFS file downloads**: Download media files with automatic retry mechanism
- **Offline-first storage**: Encrypted local database with SQLite
- **Blockchain filtering**: Block unwanted addresses and content
- **Real-time monitoring**: Continuous monitoring of mempool and confirmed transactions

### Media Players
- **🎵 Audio Player**: Minimalist audio player with playlist support
- **🎬 Video Player**: Lightweight video player with sequential playback
- **📃 Playlists**: Create and manage custom playlists for audio and video
- **Large Touch Targets**: Optimized for small screens (Apple Watch compatible)

### User Interface
- **Status Screen**: View latest indexed files and messages
- **Search Screen**: Search by address, handle, or transaction ID
- **Setup Screen**: Configure Bitcoin RPC, IPFS, and monitoring settings
- **Swipe Gestures**: Quick actions with swipe-to-delete and swipe-to-add

## Supported Platforms

- Windows 10/11
- Android 5.0+ (API 21)
- iOS 14.2+
- macOS 14.0+ (Catalyst)

## Prerequisites

1. **.NET 10.0 SDK** or later
2. **Bitcoin Core** (testnet3) running with RPC enabled
3. **IPFS daemon** (optional, for local IPFS support)

## Quick Start

### 1. Configure Bitcoin RPC

Edit your `bitcoin.conf` file:
```ini
testnet=1
server=1
rpcuser=your_username
rpcpassword=your_password
rpcport=18332
txindex=1
addressindex=1
```

### 2. Build and Run

```bash
cd MauiPlayer
dotnet build
dotnet run
```

Or use Visual Studio 2022+ to open `MauiPlayer.csproj`.

### 3. Initial Setup

1. Navigate to **Setup** page
2. Enter Bitcoin RPC credentials:
   - URL: `http://127.0.0.1:18332`
   - Username: Your RPC username
   - Password: Your RPC password
3. Click **Test Connection**
4. Enable **Transaction Monitoring**
5. Click **Save Settings**

### 4. Start Using

1. Go to **Status** page
2. Click **Connect** to connect to Bitcoin testnet3
3. Click **Start Monitor** to begin indexing transactions
4. Switch to **Audio Player** or **Video Player** to view media
5. Create playlists in the **Playlists** page

## Architecture

### Services
- **BitcoinService**: Manages Bitcoin RPC connection and queries
- **IpfsService**: Handles IPFS file downloads with retry logic
- **DataStorageService**: SQLite database with encrypted password storage
- **TransactionMonitorService**: Monitors mempool and confirmed transactions
- **P2FKParserService**: Parses P2FK messages from transactions

### Models
- **IndexedMessage**: Represents a P2FK message with metadata
- **IndexedFile**: Represents a downloaded IPFS file
- **Playlist**: Playlist container for media files
- **PlaylistItem**: Individual media item in a playlist
- **BlockedAddress**: Blocked Bitcoin address
- **AppSettings**: Application configuration

### Views (Optimized for Small Screens)
- **StatusPage**: Dashboard and monitoring status
- **AudioPlayerPage**: Audio playback interface
- **VideoPlayerPage**: Video playback interface
- **PlaylistPage**: Playlist management
- **SearchPage**: Search and filter messages
- **SetupPage**: Configuration and settings

## Security Features

- **Encrypted password storage**: AES encryption for RPC credentials
- **Input sanitization**: Protection against injection attacks
- **Address blocking**: Prevent indexing from specific addresses
- **Local-only data**: No cloud services or third-party APIs

## Data Management

### Clear Data
- Delete individual messages or files with swipe gestures
- Clear all data from Setup page

### Block Addresses
- Block specific Bitcoin addresses to prevent indexing
- View and manage blocked addresses in Setup page

### Playlists
- Create audio, video, or mixed playlists
- Add files with swipe-to-add gesture
- Reorder playlist items
- Sequential playback

## Troubleshooting

### Connection Issues
- Verify Bitcoin Core is running
- Check RPC credentials in `bitcoin.conf`
- Ensure testnet3 is fully synced
- Verify firewall allows port 18332

### IPFS Issues
- Ensure IPFS daemon is running
- Check IPFS gateway URL in settings
- Verify internet connection for gateway access

### Performance
- Reduce monitoring interval for slower devices
- Limit auto-download file size
- Clear old data periodically

## Development

### Project Structure
```
MauiPlayer/
├── Models/              # Data models
├── Services/            # Business logic services
├── ViewModels/          # MVVM view models
├── Views/               # XAML pages
├── Converters/          # Value converters
├── Resources/           # Assets and styles
└── Platforms/           # Platform-specific code
```

### Adding Features
1. Create models in `Models/`
2. Implement service in `Services/`
3. Create ViewModel in `ViewModels/`
4. Design View in `Views/`
5. Register in `MauiProgram.cs`

### Testing
- Test on different screen sizes
- Verify touch targets are large enough (minimum 44x44 pts)
- Test with limited network connectivity
- Validate Bitcoin RPC connection handling

## License

See parent repository LICENSE.txt

## Credits

Built on the Sup!? P2FK protocol by embii.
Uses NBitcoin, SQLite, and .NET MAUI frameworks.
