# P2FK Mini Player - Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     .NET MAUI Application                    │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────────────────────────────────────────────┐  │
│  │                   Views (XAML)                        │  │
│  │  ┌──────┐ ┌──────┐ ┌──────┐ ┌────────┐ ┌────────┐  │  │
│  │  │Status│ │Audio │ │Video │ │Playlist│ │Search  │  │  │
│  │  │ Page │ │Player│ │Player│ │ Page   │ │ Page   │  │  │
│  │  └──────┘ └──────┘ └──────┘ └────────┘ └────────┘  │  │
│  └────────────────┬─────────────────────────────────────┘  │
│                   │ Data Binding                            │
│  ┌────────────────▼─────────────────────────────────────┐  │
│  │              ViewModels (MVVM)                        │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌────────┐ │  │
│  │  │StatusVM  │ │AudioVM   │ │VideoVM   │ │Setup VM│ │  │
│  │  ├──────────┤ ├──────────┤ ├──────────┤ ├────────┤ │  │
│  │  │PlaylistVM│ │SearchVM  │ │  ...     │ │  ...   │ │  │
│  │  └──────────┘ └──────────┘ └──────────┘ └────────┘ │  │
│  └────────────────┬─────────────────────────────────────┘  │
│                   │ Service Calls                           │
│  ┌────────────────▼─────────────────────────────────────┐  │
│  │                    Services                           │  │
│  │  ┌─────────────┐ ┌─────────────┐ ┌──────────────┐   │  │
│  │  │Bitcoin      │ │IPFS         │ │DataStorage   │   │  │
│  │  │Service      │ │Service      │ │Service       │   │  │
│  │  ├─────────────┤ ├─────────────┤ ├──────────────┤   │  │
│  │  │Transaction  │ │P2FK Parser  │ │   ...        │   │  │
│  │  │Monitor      │ │Service      │ │              │   │  │
│  │  └─────────────┘ └─────────────┘ └──────────────┘   │  │
│  └────────────────┬─────────────────────────────────────┘  │
│                   │                                         │
└───────────────────┼─────────────────────────────────────────┘
                    │
        ┌───────────┴───────────┐
        │                       │
    ┌───▼────────┐      ┌──────▼────┐
    │ Bitcoin    │      │   IPFS    │
    │ testnet3   │      │  Network  │
    │    RPC     │      │  Gateway  │
    └────────────┘      └───────────┘
```

## Layer Responsibilities

### Views (Presentation Layer)
- **XAML-based UI**: Declarative markup optimized for small screens
- **Touch-first design**: Large buttons (50pt), swipe gestures
- **Responsive layout**: Adapts to screen sizes from watch to tablet
- **Minimal chrome**: Focus on content, not decoration

**Key Views:**
- `StatusPage.xaml` - Dashboard and connection status
- `AudioPlayerPage.xaml` - Audio playback interface
- `VideoPlayerPage.xaml` - Video playback interface
- `PlaylistPage.xaml` - Playlist management
- `SearchPage.xaml` - Search interface
- `SetupPage.xaml` - Configuration screen

### ViewModels (Business Logic Layer)
- **MVVM pattern**: Separation of concerns
- **Observable properties**: Two-way data binding
- **Command pattern**: User actions as commands
- **No platform dependencies**: Pure .NET logic

**Key ViewModels:**
- `StatusViewModel` - Connection state, recent items
- `AudioPlayerViewModel` - Audio playback state
- `VideoPlayerViewModel` - Video playback state
- `PlaylistViewModel` - Playlist CRUD operations
- `SearchViewModel` - Search logic and results
- `SetupViewModel` - App configuration

### Services (Data Access Layer)
**BitcoinService**: 
- RPC connection management
- Transaction queries
- Mempool monitoring
- Block height tracking

**IpfsService**:
- File downloads from IPFS
- Retry logic with exponential backoff
- Gateway fallback
- Cache management

**DataStorageService**:
- SQLite database operations
- Encrypted password storage (AES-256)
- CRUD for all models
- Transaction support

**TransactionMonitorService**:
- Background monitoring thread
- Mempool scanning
- Address-specific monitoring
- Event-driven notifications

**P2FKParserService**:
- Transaction parsing
- P2FK message extraction
- IPFS hash detection
- Message indexing

### Models (Data Layer)
**Core Models:**
- `IndexedMessage` - P2FK message with metadata
- `IndexedFile` - Downloaded IPFS file
- `Playlist` - Playlist container
- `PlaylistItem` - Individual playlist entry
- `BlockedAddress` - Blocked sender
- `AppSettings` - Configuration

## Data Flow

### 1. Message Indexing Flow
```
Bitcoin testnet3
    │
    │ RPC Query
    ▼
BitcoinService
    │
    │ Transaction Data
    ▼
P2FKParserService
    │
    │ Parsed Message
    ▼
DataStorageService
    │
    │ SQLite
    ▼
StatusViewModel
    │
    │ Data Binding
    ▼
StatusPage (UI)
```

### 2. File Download Flow
```
IndexedMessage (with IPFS hash)
    │
    │ Parse Hash
    ▼
IpfsService
    │
    ├─→ Local Daemon (if available)
    │
    └─→ Gateway (fallback)
    │
    │ Downloaded File
    ▼
DataStorageService
    │
    │ File Metadata
    ▼
AudioPlayerViewModel / VideoPlayerViewModel
    │
    │ Data Binding
    ▼
Audio/Video Player (UI)
```

### 3. Playlist Management Flow
```
User Action (Swipe + Add)
    │
    ▼
AudioPlayerViewModel.AddToPlaylistCommand
    │
    ▼
DataStorageService.AddToPlaylistAsync()
    │
    ├─→ Create Playlist (if not exists)
    │
    └─→ Add PlaylistItem
    │
    ▼
PlaylistViewModel (auto-refresh)
    │
    │ Data Binding
    ▼
PlaylistPage (UI)
```

## Database Schema

```sql
-- Messages indexed from blockchain
CREATE TABLE IndexedMessages (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TransactionId TEXT INDEXED,
    FromAddress TEXT INDEXED,
    ToAddress TEXT INDEXED,
    Handle TEXT,
    Message TEXT,
    MessageType TEXT,
    IpfsHash TEXT,
    LocalFilePath TEXT,
    BlockDate DATETIME,
    BlockHeight INTEGER,
    Confirmations INTEGER,
    IndexedDate DATETIME,
    IsBlocked BOOLEAN
);

-- Files downloaded from IPFS
CREATE TABLE IndexedFiles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    IpfsHash TEXT UNIQUE INDEXED,
    FileName TEXT,
    LocalPath TEXT,
    FileType TEXT,
    FileSize INTEGER,
    TransactionId TEXT,
    FromAddress TEXT,
    DownloadedDate DATETIME,
    IsBlocked BOOLEAN
);

-- Playlists
CREATE TABLE Playlists (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT INDEXED,
    Type TEXT,
    Description TEXT,
    CreatedDate DATETIME,
    ModifiedDate DATETIME,
    ItemCount INTEGER
);

-- Playlist items
CREATE TABLE PlaylistItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PlaylistId INTEGER INDEXED,
    IpfsHash TEXT,
    FileName TEXT,
    LocalPath TEXT,
    MediaType TEXT,
    OrderIndex INTEGER,
    TransactionId TEXT,
    AddedDate DATETIME
);

-- Blocked addresses
CREATE TABLE BlockedAddresses (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Address TEXT UNIQUE INDEXED,
    Handle TEXT,
    Reason TEXT,
    BlockedDate DATETIME
);

-- App settings
CREATE TABLE AppSettings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    RpcUrl TEXT,
    RpcUsername TEXT,
    RpcPassword TEXT,  -- AES encrypted
    VersionByte TEXT,
    IpfsGateway TEXT,
    AutoDownloadIpfs BOOLEAN,
    MaxAutoDownloadSizeMb INTEGER,
    MonitoredAddresses TEXT,
    MonitoredHandles TEXT,
    MonitoringEnabled BOOLEAN,
    MonitoringIntervalSeconds INTEGER,
    LastProcessedBlockHeight INTEGER
);
```

## Security Architecture

### Password Encryption
```csharp
// Encryption (on save)
var key = DeviceInfo.Name + DeviceInfo.Platform + DeviceInfo.Idiom;
var encrypted = AES256.Encrypt(password, key);
settings.RpcPassword = encrypted;

// Decryption (on load)
var decrypted = AES256.Decrypt(settings.RpcPassword, key);
```

### Input Sanitization
- All user inputs are validated before RPC calls
- SQL injection prevention via parameterized queries
- Path traversal protection in file operations
- XSS prevention in message display

### Address Blocking
- Addresses blocked at service layer
- Prevents indexing new content
- Marks existing content as blocked
- UI automatically filters blocked content

## Performance Optimizations

### Small Screen Optimization
- **Minimal DOM**: Simple layouts, few nested containers
- **Virtual scrolling**: Only render visible items
- **Image lazy loading**: Load thumbnails on demand
- **Touch debouncing**: 200ms delay to prevent accidental taps

### Battery Optimization
- **Configurable polling**: 30s default, adjustable
- **Sleep when inactive**: Pause monitoring in background
- **Efficient queries**: Indexed database columns
- **Batch operations**: Bulk insert/update

### Memory Management
- **Object pooling**: Reuse view models
- **Weak references**: Prevent memory leaks
- **Dispose pattern**: Clean up resources
- **Collection limits**: Max 100 items in lists

## Deployment Targets

### Windows
- Minimum: Windows 10 1809 (10.0.17763)
- Package: MSIX/AppX
- Storage: `%LOCALAPPDATA%\Packages\`

### Android
- Minimum: API 21 (Android 5.0 Lollipop)
- Package: APK/AAB
- Storage: `/data/data/com.sup.mauiplayer/files/`

### iOS
- Minimum: iOS 14.2
- Package: IPA
- Storage: App Sandbox Documents

### macOS
- Minimum: macOS 14.0 (Catalyst)
- Package: PKG
- Storage: `~/Library/Containers/`

## Future Enhancements

1. **Background sync**: Continue monitoring when app is backgrounded
2. **Push notifications**: Alert on new messages
3. **Offline mode**: Full functionality without internet
4. **P2P sync**: Direct blockchain sync without RPC
5. **Media streaming**: Stream large files without full download
6. **Advanced search**: Full-text search, filters
7. **Export/import**: Backup and restore playlists
8. **Themes**: Dark/light mode, custom colors
