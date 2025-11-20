# Sup!? Dual-Mode Architecture Guide

## Overview

Sup!? Modern supports two distinct operating modes, allowing users to choose between full decentralization with complete control or convenient API access without running full blockchain nodes.

## Operating Modes

### Mode 1: Fully Decentralized (RPC)

**What It Is:**
- Direct RPC communication with local blockchain nodes
- Internal state engine and contract execution
- Complete data sovereignty
- No reliance on third-party services

**Requirements:**
- Running blockchain full node (Bitcoin, Litecoin, Dogecoin, etc.)
- RPC access configured
- Local IPFS daemon
- Sufficient disk space for blockchain data

**Benefits:**
- ✅ Maximum privacy and security
- ✅ Complete control over your data
- ✅ No third-party dependencies
- ✅ Censorship-resistant
- ✅ Direct blockchain verification

**Best For:**
- Power users
- Privacy-conscious individuals
- Node operators
- Development and testing
- Maximum decentralization

### Mode 2: API Mode (p2fk.io)

**What It Is:**
- Cloud-based API access to blockchain functions
- Managed wallet with signing capabilities
- No local node required
- Instant access without synchronization

**Requirements:**
- Internet connection
- API key (optional, for rate limits)
- API wallet credentials

**Benefits:**
- ✅ No blockchain synchronization needed
- ✅ Lower resource requirements
- ✅ Faster onboarding
- ✅ Works on any device
- ✅ Automatic updates

**Best For:**
- New users
- Mobile/lightweight devices
- Quick access
- Testing without infrastructure
- Casual usage

## Configuration

### Setting the Mode

Edit `appsettings.json`:

```json
{
  "Application": {
    "Mode": "Decentralized",  // or "API"
    "ApiBaseUrl": "https://api.p2fk.io",
    "ApiKey": "",
    "UseApiWallet": false
  }
}
```

**Mode Options:**
- `"Decentralized"` - Uses local RPC nodes
- `"API"` - Uses p2fk.io API

### Decentralized Mode Setup

1. **Install blockchain node:**
   ```bash
   # Example for Bitcoin testnet
   bitcoind -testnet -daemon
   ```

2. **Configure RPC access** in `appsettings.json`:
   ```json
   {
     "Blockchains": {
       "BitcoinTestnet": {
         "RpcUrl": "http://127.0.0.1:18332",
         "Username": "bitcoin-rpc",
         "Password": "your-rpc-password",
         "Enabled": true
       }
     }
   }
   ```

3. **Start IPFS daemon:**
   ```bash
   ipfs daemon
   ```

4. **Launch Sup!?:**
   ```bash
   cd Sup.Modern/Sup.Desktop
   dotnet run
   ```

### API Mode Setup

1. **Set API mode** in `appsettings.json`:
   ```json
   {
     "Application": {
       "Mode": "API",
       "ApiBaseUrl": "https://api.p2fk.io",
       "ApiKey": "your-api-key-here",  // Optional
       "UseApiWallet": true
     }
   }
   ```

2. **Create API wallet** (first time only):
   - Navigate to Settings
   - Click "Create API Wallet"
   - Save your wallet token securely
   - Use token for all operations

3. **Launch Sup!?:**
   ```bash
   cd Sup.Modern/Sup.Desktop
   dotnet run
   ```

No blockchain sync required - instant access!

## Feature Comparison

| Feature | Decentralized Mode | API Mode |
|---------|-------------------|----------|
| **Blockchain Sync** | Required (hours/days) | Not required |
| **Disk Space** | 50-900 GB per blockchain | Minimal |
| **Privacy** | Maximum | Standard |
| **Speed** | Depends on local node | Fast API calls |
| **Resource Usage** | High (CPU, RAM, Disk) | Low |
| **Internet Required** | Only for P2P sync | Yes, for all operations |
| **Cost** | Hardware + electricity | Potentially API fees |
| **Setup Time** | Hours (sync) | Minutes |
| **Censorship Resistance** | Maximum | Depends on API |
| **Transaction Signing** | Local | API wallet |

## API Operations Supported

When running in API mode, the following operations are available through p2fk.io:

### Object Operations
- ✅ Get object by address
- ✅ Search objects by keyword
- ✅ Mint new objects
- ✅ Get objects by creator

### Profile Operations
- ✅ Get profile by URN
- ✅ Register new profile
- ✅ Update profile (via new registration)

### Message Operations
- ✅ Get messages for address
- ✅ Send public messages
- ✅ Send encrypted messages
- ✅ Message history

### Wallet Operations
- ✅ Create API wallet
- ✅ Get balance
- ✅ Generate addresses
- ✅ Sign transactions
- ✅ Sendmany transactions

### Blockchain Info
- ✅ Health checks
- ✅ Current block height
- ✅ Network status

## API Wallet System

### Creating an API Wallet

```csharp
var unifiedService = (UnifiedBlockchainService)BlockchainService;
var walletConfig = await unifiedService.CreateApiWalletAsync("bitcoin-testnet");

// Save these securely!
string walletAddress = walletConfig.WalletAddress;
string apiToken = walletConfig.ApiToken;
```

### Using the API Wallet

The wallet token is used automatically for all signing operations:

```csharp
// Set the wallet token once
unifiedService.SetApiWalletToken(apiToken);

// All subsequent operations use this wallet
var txId = await unifiedService.MintObjectAsync(obj, "BitcoinTestnet");
var msgTxId = await unifiedService.SendMessageAsync(from, to, content, "BitcoinTestnet");
```

### Security Considerations

**API Wallet:**
- Token is used for signing transactions
- Store token securely (encrypted storage)
- Never share your API token
- Consider it equivalent to a private key
- Can be revoked via API

**Decentralized Wallet:**
- Private keys never leave your machine
- Full control of funds
- Backup responsibility is yours
- No third-party can access

## Switching Modes at Runtime

You can switch modes programmatically:

```csharp
@inject IBlockchainService BlockchainService

var unifiedService = (UnifiedBlockchainService)BlockchainService;

// Switch to API mode
unifiedService.SetMode(ApplicationMode.API);
unifiedService.SetApiWalletToken(yourToken);

// Switch back to Decentralized
unifiedService.SetMode(ApplicationMode.Decentralized);
```

**Note:** Mode switching at runtime is available but not recommended during active operations. Restart the application after changing modes in configuration.

## Code Transparency

Both modes use the same interface (`IBlockchainService`), making your code mode-agnostic:

```csharp
// This code works in BOTH modes!
@inject IBlockchainService BlockchainService

// Get an object
var obj = await BlockchainService.GetObjectByAddressAsync(address, blockchain);

// Mint an object
var txId = await BlockchainService.MintObjectAsync(newObject, blockchain);

// Search objects
var results = await BlockchainService.SearchObjectsByKeywordAsync("art", blockchain);
```

The `UnifiedBlockchainService` automatically routes calls to either RPC or API based on current mode.

## Best Practices

### For Decentralized Mode:
1. **Keep your node synced** - Run `bitcoind` continuously
2. **Backup your wallet** - Use `dumpprivkey` for important addresses
3. **Monitor disk space** - Blockchain data grows over time
4. **Use testnet first** - Test with testnet before mainnet
5. **Firewall rules** - Allow P2P ports for blockchain sync

### For API Mode:
1. **Secure your API token** - Treat it like a password
2. **Monitor API usage** - Be aware of any rate limits
3. **Keep credentials backed up** - Save wallet info securely
4. **Use HTTPS** - API calls should always be encrypted
5. **Test with small amounts** - Start with test transactions

## Troubleshooting

### Decentralized Mode Issues

**Problem:** Can't connect to blockchain
**Solution:**
- Check if blockchain daemon is running
- Verify RPC credentials
- Check firewall settings
- Ensure RPC port is accessible

**Problem:** Slow performance
**Solution:**
- Wait for blockchain sync to complete
- Increase node's memory allocation
- Use SSD for blockchain data
- Consider pruning mode

### API Mode Issues

**Problem:** API calls failing
**Solution:**
- Check internet connection
- Verify API key is correct
- Check API service status
- Review rate limits

**Problem:** Wallet operations not working
**Solution:**
- Ensure API wallet token is set
- Verify token hasn't expired
- Check wallet has sufficient balance
- Re-create wallet if needed

## Future Enhancements

Planned improvements for the dual-mode system:

- [ ] Hybrid mode (local node with API fallback)
- [ ] Mode auto-detection based on node availability
- [ ] Real-time mode switching in Settings UI
- [ ] API usage statistics and monitoring
- [ ] Multi-wallet support in API mode
- [ ] Offline transaction signing
- [ ] API request caching
- [ ] Load balancing across multiple APIs

## Conclusion

The dual-mode architecture provides flexibility for all user types:

- **Decentralized Mode** offers maximum control and privacy for power users
- **API Mode** provides easy access for newcomers without infrastructure requirements
- **Single Codebase** means no duplication and easier maintenance
- **User Choice** empowers users to select the mode that fits their needs

This architecture makes Sup!? accessible to everyone while maintaining its core principle of decentralization for those who want it.
