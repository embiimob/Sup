# P2FK Mini Player - Quick Start Guide

## What is P2FK Mini Player?

A minimalist, cross-platform application for browsing P2FK messages and playing audio/video files from the Bitcoin testnet3 blockchain and IPFS. Designed to work on devices as small as an Apple Watch.

## Key Features

### 📱 Screens
- **Status**: View indexed messages and files, monitor blockchain connection
- **Audio Player**: Play audio files with playlist support
- **Video Player**: Play video files with playlist support  
- **Playlists**: Create and manage custom playlists
- **Search**: Find messages by address, handle, or transaction
- **Setup**: Configure Bitcoin RPC and IPFS settings

### 🎯 Optimized for Small Screens
- Large touch targets (minimum 50pt)
- Minimal text, maximum functionality
- Swipe gestures for quick actions
- Sequential navigation

## Prerequisites

1. **Bitcoin Core** running testnet3 with RPC enabled
2. **IPFS daemon** (optional, for direct IPFS support)
3. **.NET 10** runtime (for Windows/Mac desktop)
4. **Android 5.0+** or **iOS 14.2+** (for mobile)

## Quick Setup

### Step 1: Configure Bitcoin Core

Edit `bitcoin.conf`:
```ini
testnet=1
server=1
rpcuser=myusername
rpcpassword=mypassword
rpcport=18332
txindex=1
addressindex=1
```

Restart Bitcoin Core and wait for testnet3 sync.

### Step 2: Launch Mini Player

**Desktop (Windows/Mac):**
```bash
cd MauiPlayer
dotnet run
```

**Android/iOS:**
- Build and deploy via Visual Studio 2022+
- Or use `dotnet build -t:Run -f net10.0-android`

### Step 3: Configure App

1. Open **Setup** page
2. Enter RPC credentials:
   - URL: `http://127.0.0.1:18332`
   - Username: `myusername`
   - Password: `mypassword`
3. Click **Test Connection**
4. Enable **Transaction Monitoring**
5. (Optional) Add addresses to monitor
6. Click **Save Settings**

### Step 4: Start Monitoring

1. Go to **Status** page
2. Click **Connect**
3. Click **Start Monitor**
4. Wait for messages to be indexed

## Basic Usage

### Playing Audio

1. Navigate to **🎵 Audio Player**
2. Tap any audio file to play
3. Use ⏮ ⏸ ⏭ controls
4. Swipe right on a file and tap **+** to add to playlist

### Playing Video

1. Navigate to **🎬 Video Player**
2. Tap any video file to play
3. Use ⏮ ⏸ ⏭ controls
4. Swipe right on a file and tap **+** to add to playlist

### Creating Playlists

1. Navigate to **📃 Playlists**
2. Tap **+ New**
3. Enter playlist name
4. Select type (Audio, Video, or Mixed)
5. Add files from audio/video players using swipe gesture
6. Tap playlist to view and play items

### Searching Messages

1. Navigate to **🔍 Search**
2. Select search type:
   - **Address**: Bitcoin address (e.g., `muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs`)
   - **Handle**: P2FK handle (e.g., `@embii4u`)
   - **Transaction**: Transaction ID
3. Enter query
4. Tap **Search**
5. Swipe left on any result to:
   - **Block**: Block sender's address
   - **Delete**: Remove from database

### Blocking Unwanted Content

1. In **Search** or **Status**, swipe left on any message
2. Tap **Block** (orange button)
3. Confirm blocking
4. All content from that address will be hidden
5. Manage blocked addresses in **Setup** page

### Managing Data

**Delete individual items:**
- Swipe left → Tap **Delete**

**Clear all data:**
1. Go to **Setup** page
2. Scroll to **Advanced Actions**
3. Tap **Clear All Data**
4. Confirm (requires double confirmation)

## Tips for Small Screens

### Apple Watch Optimization
- Use swipe gestures instead of buttons when possible
- Navigation via Shell flyout (swipe from left edge)
- Large playback controls: ⏮ ⏸ ⏭
- Minimal text displays

### Touch Targets
- All interactive elements are 44pt or larger
- Swipe gestures have generous hit areas
- Double-tap prevention (200ms delay)

### Battery Saving
1. Reduce **Monitoring Interval** (Setup page)
2. Disable **Auto-download IPFS** 
3. Limit **Max auto-download size**
4. Stop monitoring when not actively using

## Troubleshooting

### "Connection failed"
- Verify Bitcoin Core is running
- Check RPC credentials in bitcoin.conf
- Ensure testnet3 is synced (use `bitcoin-cli -testnet getblockcount`)
- Check firewall allows port 18332

### "No messages found"
- Wait for blockchain sync to complete
- Ensure addresses you're searching actually have P2FK messages
- Try monitoring specific addresses in Setup page
- Check Bitcoin RPC connection in Status page

### "IPFS download failed"
- Verify IPFS daemon is running (`ipfs daemon`)
- Check IPFS gateway URL in Setup page
- Try different IPFS gateways (e.g., `https://cloudflare-ipfs.com/ipfs/`)
- Check internet connection

### App runs slowly
- Reduce number of monitored addresses
- Increase monitoring interval
- Clear old data periodically
- Close other apps on small devices

## Advanced Features

### Monitoring Specific Addresses

Setup page → **Monitored Addresses**:
```
muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs, mwJDUTXksGKUmU3z9nKeMvnjNnWjEXj5rW
```

### Monitoring Specific Handles

Setup page → **Monitored Handles**:
```
embii4u, example_handle
```

### Blockchain Rescan

If you missed transactions:
1. Go to **Setup** page
2. Tap **Rescan Blockchain**
3. Wait for rescan to complete

### Export/Backup

Data is stored in:
- **Android**: `/data/data/com.sup.mauiplayer/files/p2fk_player.db`
- **iOS**: App Documents folder
- **Windows**: `%LOCALAPPDATA%\Packages\[AppPackage]\LocalState\p2fk_player.db`

Use SQLite tools to export/backup database.

## Security Best Practices

1. **Use strong RPC password** in bitcoin.conf
2. **Don't expose RPC port** to the internet
3. **Regularly clear old data** to minimize storage
4. **Block suspicious addresses** immediately
5. **Keep app updated** for security patches

## Getting Help

- Issues: https://github.com/embiimob/Sup/issues
- P2FK Protocol: See main README.md
- Bitcoin testnet3: https://developer.bitcoin.org/examples/testing.html

## Next Steps

- Explore existing P2FK content by searching `embii4u`
- Create your own P2FK profile (see main Sup!? documentation)
- Share your playlists with friends via address sharing
- Experiment with different IPFS gateways for better performance
