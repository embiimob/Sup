# Async RPC Refactoring Implementation Plan

## Problem Summary
The application experiences sporadic `NullReferenceException` crashes in `LinkLabel.OnPaint` during hashtag searches (e.g., "#LOVE"). The root cause is synchronous RPC calls blocking the UI thread, causing Windows to attempt painting controls in inconsistent states.

## Implementation Status

### Phase 1: RPC Infrastructure ✅ COMPLETED
- **BlockchainRPC.cs**: Added async HTTP call methods with CancellationToken support
- **BlockchainRPC.Methods.cs**: Added async versions of all RPC methods
- **Key changes**:
  - `HttpCallAsync()`: Async HTTP request method
  - `RpcCallAsync<T>()`: Async RPC wrapper with cancellation support
  - All synchronous methods have async counterparts (e.g., `SearchRawDataTransactionAsync`)

### Phase 2: ObjectBrowser.cs Critical Fixes ✅ PARTIAL
- **profileURN_TextChanged**: Converted to async to prevent UI thread blocking during signature verification
- **Remaining**: Other event handlers need similar treatment

### Phase 3: Required Changes (HIGH PRIORITY)

#### A. ObjectBrowser.cs Event Handlers
The following event handlers currently run synchronous RPC calls and must be updated:

1. **SearchAddressKeyDown** (Line ~2998)
   - Calls `BuildSearchResults()` via `Task.Run()` 
   - ✅ Already offloads to background thread
   - ⚠️ Inner methods still use sync RPC calls

2. **Button Click Handlers** (Lines ~2875, ~2914, ~2952, ~3727)
   - btnGetOwnedClick, ButtonGetCreatedClick, btnCollections_Click, btnActivity_Click
   - All call `BuildSearchResults()` via `Task.Run()`
   - ✅ Already offloads to background thread
   - ⚠️ Inner methods still use sync RPC calls

#### B. ObjectBrowser.cs Core Methods (CRITICAL)

1. **GetObjectsByAddress** (Line 216)
   - Makes multiple synchronous calls to:
     - `PROState.GetProfileByAddress()`
     - `PROState.GetProfileByURN()`
     - `OBJState.GetObjectsCreatedByAddress()`
     - `OBJState.GetObjectsOwnedByAddress()`
     - `OBJState.GetFoundObjects()`
     - `OBJState.GetObjectsByAddress()`
     - `OBJState.GetObjectByURN()`
   - **Action**: Convert to async and propagate through call chain

2. **GetHistoryByAddress** (Line 1462)
   - Calls:
     - `PROState.GetProfileByURN()`
     - `Root.GetRootsByAddress()`
     - `PROState.GetProfileByAddress()`
   - **Action**: Convert to async

3. **GetCollectionsByAddress** (Not shown but referenced)
   - **Action**: Convert to async

#### C. OBJ.cs Methods (CRITICAL - 3000+ lines)

These are the core methods that perform RPC calls:

1. **GetObjectByAddress** (Line 94)
   - Uses `lock (SupLocker)` - must be handled carefully with async
   - Calls `Root.GetRootsByAddress()` (sync)
   - Makes multiple RPC calls through Root methods
   - **Action**: 
     - Replace lock with `SemaphoreSlim` for async compatibility
     - Convert to async
     - Update all RPC calls to use async NBitcoin methods

2. **GetObjectsCreatedByAddress** (Line 2656)
   - Calls `GetObjectsByAddress()` (sync)
   - **Action**: Convert to async, chain async calls

3. **GetObjectsOwnedByAddress** (Line 2594)
   - Calls `GetObjectsByAddress()` (sync)
   - **Action**: Convert to async, chain async calls

4. **GetFoundObjects** (Line 2896)
   - Calls `GetObjectByAddress()` (sync)
   - **Action**: Convert to async, chain async calls

5. **GetObjectsByAddress (OBJ)** (Line 2394)
   - Calls `Root.GetRootsByAddress()` (sync)
   - Calls `GetObjectByAddress()` (sync)
   - **Action**: Convert to async

#### D. Root.cs Methods (CRITICAL)

1. **GetRootsByAddress** (Location TBD)
   - Makes synchronous RPC calls to blockchain
   - Uses `CoinRPC.SearchRawDataTransaction()`
   - **Action**: 
     - Convert to async
     - Use `SearchRawDataTransactionAsync()` instead
     - Add CancellationToken parameter

2. **GetRootByTransactionId** (Location TBD)
   - Makes synchronous RPC calls
   - **Action**: Convert to async

#### E. PRO.cs Methods

1. **GetProfileByAddress** (Line 377)
   - Uses `rpcClient.SendCommand("listreceivedbyaddress").ResultString`
   - **Action**: Convert to async using `SendCommandAsync()`

2. **GetProfileByURN** (Location TBD)
   - **Action**: Convert to async

### Phase 4: NBitcoin RPC Usage Pattern

#### Current Pattern (BLOCKING):
```csharp
NetworkCredential credentials = new NetworkCredential(username, password);
NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(url), Network.Main);
var result = rpcClient.SendCommand("getblock", hash).Result;  // ❌ BLOCKS UI THREAD
string data = rpcClient.SendCommand("getrawtransaction", txid).ResultString;  // ❌ BLOCKS UI THREAD
```

#### Fixed Pattern (NON-BLOCKING):
```csharp
NetworkCredential credentials = new NetworkCredential(username, password);
NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(url), Network.Main);
var response = await rpcClient.SendCommandAsync("getblock", hash).ConfigureAwait(false);  // ✅ ASYNC
var result = response.Result;
var response2 = await rpcClient.SendCommandAsync("getrawtransaction", txid).ConfigureAwait(false);  // ✅ ASYNC
string data = response2.ResultString;
```

### Phase 5: Cancellation Support

#### ObjectBrowser.cs Additions Needed:
```csharp
private CancellationTokenSource _searchCancellationTokenSource;

// In search methods:
_searchCancellationTokenSource?.Cancel();
_searchCancellationTokenSource = new CancellationTokenSource();
var token = _searchCancellationTokenSource.Token;

// Pass token through all async calls
await GetObjectsByAddressAsync(address, token);

// In Form_Closing:
_searchCancellationTokenSource?.Cancel();
_searchCancellationTokenSource?.Dispose();
```

### Phase 6: UI Thread Marshaling Rules

1. **Use ConfigureAwait(false)** for all internal async methods to avoid capturing sync context
2. **Use ConfigureAwait(true) or Control.Invoke** only when updating UI controls
3. **Pattern**:
   ```csharp
   // In background work
   var data = await GetDataAsync().ConfigureAwait(false);
   
   // When updating UI
   this.Invoke((Action)(() =>
   {
       label.Text = data;
   }));
   ```

### Phase 7: Error Handling Pattern

```csharp
try
{
    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
    {
        var result = await rpcClient.SendCommandAsync("method", param, cts.Token).ConfigureAwait(false);
        return result.ResultString;
    }
}
catch (OperationCanceledException)
{
    // Log: Operation cancelled or timed out
    this.Invoke((Action)(() => {
        MessageBox.Show("Operation timed out. Please check your connection.");
    }));
}
catch (Exception ex)
{
    // Log exception
    this.Invoke((Action)(() => {
        MessageBox.Show($"Error: {ex.Message}");
    }));
}
finally
{
    this.Invoke((Action)(() => {
        EnableSupInput();  // Re-enable UI
    }));
}
```

### Phase 8: Testing Checklist

- [ ] Test hashtag search "#LOVE" - no crashes
- [ ] Test rapid successive searches - proper cancellation
- [ ] Test with slow/unresponsive RPC node - timeout handling
- [ ] Test form closing during search - graceful cancellation
- [ ] Verify UI remains responsive during all operations
- [ ] Verify no LinkLabel.OnPaint exceptions
- [ ] Test address-based searches (created, owned, found)
- [ ] Test profile validation and signature verification
- [ ] Monitor for deadlocks (no `.Wait()` or `.Result` calls on UI thread)

## Implementation Priority

### CRITICAL (Must Fix):
1. ✅ profileURN_TextChanged - DONE
2. All NBitcoin `SendCommand().Result` calls in UI-adjacent code
3. Root.GetRootsByAddress - Convert to async
4. OBJState.GetObjectsByAddress - Convert to async
5. OBJState.GetObjectByAddress - Convert to async with SemaphoreSlim

### HIGH:
1. PRO.GetProfileByAddress/GetProfileByURN - Convert to async
2. All OBJState Get* methods
3. Add cancellation support to ObjectBrowser

### MEDIUM:
1. Timeout handling
2. Comprehensive error messages
3. Loading indicators

## Known Challenges

1. **Lock to SemaphoreSlim Conversion**: `OBJState.GetObjectByAddress` uses `lock (SupLocker)` which is not compatible with async/await. Must be converted to `SemaphoreSlim`.

2. **Large Call Chains**: Methods have deep call chains of synchronous calls. Must convert entire chains to async to avoid blocking.

3. **UI Invoke Patterns**: Existing code already uses `this.Invoke((Action)(() => { }))`. Must audit to ensure it's used correctly with async methods.

4. **File I/O**: Many synchronous file operations (`File.ReadAllText`, `File.WriteAllText`). Consider converting to async for consistency, though less critical than RPC calls.

## Files Requiring Changes

### Critical Files:
- [x] P2FK/classes/BlockchainRPC.cs - Infrastructure done
- [x] P2FK/classes/BlockchainRPC.Methods.cs - Infrastructure done
- [ ] P2FK/contracts/Root.cs - RPC calls
- [ ] P2FK/contracts/OBJ.cs - Core business logic
- [ ] P2FK/contracts/PRO.cs - Profile operations
- [ ] ObjectBrowser.cs - UI layer
- [ ] ObjectBrowser.Designer.cs - May need event signature updates

### Supporting Files:
- [ ] ObjectMint.cs - Uses RPC for signing
- [ ] ProfileMint.cs - Uses RPC for signing
- [ ] ObjectBuy.cs - Uses RPC
- [ ] SupMain.cs - Uses RPC
- [ ] WorkBench.cs - Uses RPC

## Estimated Scope

- **Lines of code to modify**: ~5000+
- **Methods to convert**: ~50+
- **Files to modify**: ~10+
- **Critical path**: Root.cs → OBJ.cs → ObjectBrowser.cs

## Next Steps

1. Convert Root.GetRootsByAddress to async (highest impact)
2. Convert OBJState core methods to async
3. Update ObjectBrowser.GetObjectsByAddress to use async methods
4. Add cancellation token support throughout
5. Comprehensive testing

## Notes

- This is a large-scale refactoring that touches core application functionality
- Each change must be tested to ensure no regressions
- The async conversion must be done carefully to avoid introducing new bugs
- Consider doing this in multiple PRs for easier review
- Priority should be on preventing UI thread blocking over perfect async hygiene
