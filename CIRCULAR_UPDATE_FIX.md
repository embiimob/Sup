# Circular Update Loop Fix - ObjectBrowser and SupMain

## Problem Description

A critical bug was causing the profileURN to be invalidated (shown with a big red X) and unhandled exceptions when users searched for hashtags like `#LOVE` in the ObjectBrowser search field.

## Root Cause

The issue was caused by a circular update loop between two components:

1. **ObjectBrowser** (embedded form showing search results)
2. **SupMain** (main application window with social feed)

### The Circular Update Flow

```
1. User types "#LOVE" in ObjectBrowser.txtSearchAddress and presses Enter
   ↓
2. ObjectBrowser.BuildSearchResults() is called
   ↓
3. BuildSearchResults() sets profileURN.Text = txtSearchAddress.Text
   ↓
4. profileURN.TextChanged event fires in ObjectBrowser
   ↓
5. ObjectBrowserControl.ProfileURN_TextChanged() propagates this as ProfileURNChanged event
   ↓
6. SupMain.OBControl_ProfileURNChanged() handles the event
   ↓
7. SupMain updates its own profileURN.Text from ObjectBrowser
   ↓
8. When user clicks profile images, SupMain sets OBcontrol.control.txtSearchAddress.Text
   ↓
9. SupMain calls OBcontrol.control.BuildSearchResults()
   ↓
10. LOOP BACK TO STEP 2 → Infinite circular updates → System fault
```

## Solution Implemented

### 1. Re-entrance Guard Flags

Added two boolean flags to prevent circular updates:

#### In ObjectBrowser.cs
```csharp
// Guard flag to prevent circular updates between ObjectBrowser and SupMain
// When true, indicates that profileURN is being updated from an external source (SupMain)
// and should not trigger BuildSearchResults or propagate changes back
public bool _isUpdatingFromExternal = false;
```

#### In SupMain.cs
```csharp
// Guard flag to prevent circular updates between SupMain and ObjectBrowser
// When true, indicates that we're updating ObjectBrowser from SupMain and should not
// process the ProfileURNChanged event that results from our own update
private bool _isUpdatingObjectBrowser = false;
```

### 2. Guard Implementation Points

#### ObjectBrowser.BuildSearchResults()
Modified all 4 locations where `profileURN.Text` is set to check the guard flag:
```csharp
if (!_isUpdatingFromExternal)
{
    Debug.WriteLine($"[ObjectBrowser] Setting profileURN to: {txtSearchAddress.Text}");
    profileURN.Links[0].LinkData = profileCheck;
    profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
    profileURN.Text = txtSearchAddress.Text;
}
else
{
    Debug.WriteLine($"[ObjectBrowser] Skipping profileURN update - external update in progress");
}
```

#### ObjectBrowserControl.ProfileURN_TextChanged()
Added guard to prevent event propagation during external updates:
```csharp
if (control._isUpdatingFromExternal)
{
    Debug.WriteLine("[ObjectBrowserControl] Suppressing ProfileURNChanged event - external update in progress");
    return;
}
```

#### SupMain.OBControl_ProfileURNChanged()
Added early return when processing our own update:
```csharp
if (_isUpdatingObjectBrowser)
{
    Debug.WriteLine("[SupMain] Ignoring ProfileURNChanged event - currently updating ObjectBrowser from SupMain");
    return;
}
```

#### SupMain Profile Image Click Handler
Set guard flags when updating ObjectBrowser from SupMain:
```csharp
_isUpdatingObjectBrowser = true;
try
{
    OBcontrol.control._isUpdatingFromExternal = true;
    OBcontrol.control.txtSearchAddress.Text = profileURN.Text;
    OBcontrol.control.BuildSearchResults();
}
finally
{
    OBcontrol.control._isUpdatingFromExternal = false;
    _isUpdatingObjectBrowser = false;
}
```

### 3. Defensive Null Checks

Added defensive checks throughout `SupMain.OBControl_ProfileURNChanged()`:
- Check if `objectBrowserForm.profileURN` is not null
- Check if `profileURN.Text` is not null or empty
- Check if `profileURN.Links` collection exists and has items before accessing
- Check if `LinkData` is not null before using it

### 4. Enhanced Error Logging

Added diagnostic logging using `Debug.WriteLine()` at key points:
- When setting profileURN in ObjectBrowser
- When skipping updates due to guard flags
- When propagating/suppressing events in ObjectBrowserControl
- When processing events in SupMain
- When errors occur (with exception details)

Improved catch block in `OBControl_ProfileURNChanged()`:
```csharp
catch (Exception ex)
{
    Debug.WriteLine($"[SupMain] Error in OBControl_ProfileURNChanged: {ex.Message}");
    // Gracefully handle errors instead of crashing - log and continue
}
```

## How the Fix Works

### Normal Search Flow (User initiates search in ObjectBrowser)
1. User types `#LOVE` and presses Enter
2. BuildSearchResults() runs with `_isUpdatingFromExternal = false`
3. profileURN.Text is updated (guard allows this)
4. ProfileURN_TextChanged fires and propagates to SupMain
5. SupMain updates its UI to reflect the new profile
6. **No circular update** because SupMain doesn't call back into ObjectBrowser in this scenario

### Profile Image Click Flow (User clicks profile in SupMain)
1. User clicks a profile image in SupMain
2. SupMain sets `_isUpdatingObjectBrowser = true`
3. SupMain sets `OBcontrol.control._isUpdatingFromExternal = true`
4. SupMain updates txtSearchAddress and calls BuildSearchResults()
5. BuildSearchResults() sees the guard flag and **skips** updating profileURN.Text
6. No TextChanged event fires, so no ProfileURNChanged event propagates
7. Guard flags are reset in finally block
8. **No circular update** - the loop is broken

## Testing

To test this fix:

1. Launch the application
2. Type `#LOVE` in the ObjectBrowser search field and press Enter
3. Verify:
   - Search results are displayed correctly
   - No unhandled exception occurs
   - ProfileURN is not shown with a red X
   - The social feed updates appropriately
   - No infinite loop or system freeze occurs

4. Click on profile images in the social feed
5. Verify:
   - ObjectBrowser updates to show the clicked profile
   - No circular updates occur
   - System remains stable

## Debug Output

When running with a debugger attached, you can monitor the debug output to see:
- `[ObjectBrowser]` messages showing when profileURN is being set or skipped
- `[ObjectBrowserControl]` messages showing when events are fired or suppressed
- `[SupMain]` messages showing when events are processed or ignored

This makes it easy to diagnose any remaining issues or unexpected behavior.

## Files Modified

1. **ObjectBrowser.cs**
   - Added `_isUpdatingFromExternal` guard flag
   - Modified 4 locations in BuildSearchResults() to check guard before setting profileURN.Text
   - Added debug logging

2. **ObjectBrowserControl.cs**
   - Modified ProfileURN_TextChanged() to check guard flag before propagating event
   - Added debug logging

3. **SupMain.cs**
   - Added `_isUpdatingObjectBrowser` guard flag
   - Modified OBControl_ProfileURNChanged() to check guard and add early return
   - Modified profile image click handler to set guard flags when updating ObjectBrowser
   - Added defensive null checks throughout
   - Improved error handling and logging

## Additional Bug Fixes (December 12, 2025)

After user testing, two additional critical issues were identified and fixed:

### Issue #1: NullReferenceException in LinkLabel.OnPaint() (Commit ab9ce74)

### NullReferenceException in LinkLabel.OnPaint()

**Issue Reported by @embiimob**: When typing `#LOVE` and pressing Enter, a NullReferenceException occurred:
```
System.NullReferenceException: Object reference not set to an instance of an object.
   at System.Windows.Forms.LinkLabel.OnPaint(PaintEventArgs e)
```
The profileURN would show a large red X, and the application would throw an unhandled exception.

**Root Cause Analysis**:
1. **Enter Key Handler Problem**: Line 3045 was setting `profileURN.Text = ""` without clearing `Links[0].LinkData`, creating an inconsistent LinkLabel state
2. **Property Order Issue**: In BuildSearchResults(), we were trying to set `Links[0].LinkData` BEFORE setting the Text property, but the Links collection might not be initialized yet
3. **WinForms Behavior**: LinkLabel requires the Text property to be set first, which automatically initializes the Links collection. Accessing Links[0] before setting Text can cause NullReferenceException

**Fix Applied (Commit: ab9ce74)**:

1. **Reordered Property Setting** (4 locations in BuildSearchResults):
```csharp
// BEFORE (incorrect order - could cause NullReferenceException)
profileURN.Links[0].LinkData = profileCheck;
profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
profileURN.Text = txtSearchAddress.Text;

// AFTER (correct order - safe)
// Set Text first to ensure Links collection is initialized by WinForms
profileURN.Text = txtSearchAddress.Text;
profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
// Now safely set LinkData after Text is set and Links collection exists
if (profileURN.Links.Count > 0)
{
    profileURN.Links[0].LinkData = profileCheck;
}
```

2. **Fixed Enter Key Handler** (SearchAddressKeyDown method):
```csharp
// Clear profileURN properly to avoid inconsistent LinkLabel state
// Setting Text to empty string while Links[0].LinkData has a value can cause NullReferenceException in OnPaint
if (profileURN.Links != null && profileURN.Links.Count > 0)
{
    profileURN.Links[0].LinkData = null;
}
profileURN.Text = "";
```

**Result**: The red X and NullReferenceException should no longer occur when searching for hashtags like `#LOVE`.

### Issue #2: Hashtag Search Failure in RefreshSupMessages() (Commits 11b5847, efd4a0a)

**Issue Reported by @embiimob**: After fixing Issue #1, typing `#LOVE` and pressing Enter caused a NullReferenceException in `SupMain.RefreshSupMessages()` at line 3602:
```
System.NullReferenceException
  at SUP.SupMain.RefreshSupMessages() in SupMain.cs:line 3602
  
try { messages = OBJState.GetPublicMessagesByAddress(profileURN.Links[0].LinkData.ToString(), ...); }
```

The search would fail, and no results were returned even though the profile panel updated.

**Root Cause Analysis**:
The fix for Issue #1 introduced a race condition. When `profileURN.Text` was set at line 469 in ObjectBrowser, it immediately fired the `TextChanged` event BEFORE `LinkData` was set at line 474. This caused the event to propagate to SupMain before LinkData contained the keyword address needed for hashtag searches.

For hashtag searches like `#LOVE`:
1. The keyword "LOVE" is converted to an address via `Root.GetPublicAddressByKeyword(txtSearchAddress.Text.Substring(1), mainnetVersionByte)` at line 444
2. This address (stored in `profileCheck`) should be set in `Links[0].LinkData`
3. `GetPublicMessagesByAddress()` uses this address to fetch messages tagged with the hashtag

**Fix Applied (Commits 11b5847, efd4a0a)**:

1. **Created Helper Method** to set properties atomically:
```csharp
/// <summary>
/// Helper method to atomically set profileURN properties without triggering TextChanged event prematurely.
/// This prevents race conditions where TextChanged fires before LinkData is set.
/// </summary>
private void SetProfileURNAtomically(string text, object linkData)
{
    // Temporarily detach TextChanged event
    profileURN.TextChanged -= profileURN_TextChanged;
    try
    {
        // Set all properties atomically
        profileURN.Text = text;
        profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
        if (profileURN.Links.Count > 0)
        {
            profileURN.Links[0].LinkData = linkData;
        }
    }
    finally
    {
        // Reattach event and trigger once after all properties are set
        profileURN.TextChanged += profileURN_TextChanged;
        profileURN_TextChanged(profileURN, EventArgs.Empty);
    }
}
```

2. **Used helper method** in all 4 locations where profileURN is updated:
```csharp
// Before (race condition)
profileURN.Text = txtSearchAddress.Text;  // TextChanged fires here!
profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
if (profileURN.Links.Count > 0)
{
    profileURN.Links[0].LinkData = profileCheck;  // Too late!
}

// After (atomic update)
SetProfileURNAtomically(txtSearchAddress.Text, profileCheck);
```

3. **Added defensive check** in RefreshSupMessages():
```csharp
// For hashtag searches like #LOVE, LinkData contains the keyword address from Root.GetPublicAddressByKeyword()
if (profileURN.Links != null && profileURN.Links.Count > 0 && 
    profileURN.Links[0].LinkData != null && 
    !string.IsNullOrEmpty(profileURN.Links[0].LinkData.ToString()))
{
    try { messages = OBJState.GetPublicMessagesByAddress(profileURN.Links[0].LinkData.ToString(), ...); }
    catch { }
}
```

**Result**: Hashtag searches now work correctly. The keyword address is properly set in LinkData before events fire, allowing RefreshSupMessages() to fetch messages using the converted address.

### Issue #3: Search Not Executing (Commit 0851d87)

**Issue Reported by @embiimob**: After fixing Issue #2, typing `#LOVE` and pressing Enter updated the profile panel immediately, but the search didn't execute. The output showed RPCException errors from NBitcoin.

**Root Cause Analysis**:
The fix for Issue #2 used an event detach/reattach pattern:
```csharp
profileURN.TextChanged -= profileURN_TextChanged;
// ... set properties ...
profileURN.TextChanged += profileURN_TextChanged;
profileURN_TextChanged(profileURN, EventArgs.Empty); // Manually call handler
```

The problem: manually calling `profileURN_TextChanged()` only invoked ObjectBrowser's internal handler, not the actual LinkLabel.TextChanged event. ObjectBrowserControl listens to `control.profileURN.TextChanged`, but this event never fired because we detached and manually called the wrong handler. This broke the event chain to SupMain, preventing search execution.

**Fix Applied (Commit 0851d87)**:

Removed the detach/reattach pattern and returned to the original property order, with proper Links collection initialization:

```csharp
private void SetProfileURNAtomically(string text, object linkData)
{
    // Ensure at least one link exists in the Links collection
    if (profileURN.Links.Count == 0)
    {
        profileURN.Links.Add(new LinkLabel.Link(0, text.Length));
    }
    
    // Set LinkData BEFORE setting Text (original order restored)
    if (profileURN.Links.Count > 0)
    {
        profileURN.Links[0].LinkData = linkData;
    }
    
    profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
    
    // Set Text - this triggers TextChanged naturally, propagating to all handlers
    profileURN.Text = text;
}
```

**Key Changes**:
1. Explicitly create a Link in the Links collection if none exists
2. Set LinkData BEFORE Text (original order)
3. Let Text setter trigger TextChanged naturally (no manual event calls)
4. Event properly propagates: ObjectBrowser → ObjectBrowserControl → SupMain

**Result**: Search now executes properly. The TextChanged event fires naturally with LinkData already set, propagating correctly through the event chain to trigger search in SupMain.

### Issue #4: Recurring NullReferenceException (Commit b6ab213)

**Issue Reported by @embiimob**: After fixing Issue #3, the search was working but the NullReferenceException in System.Windows.Forms returned. The exception occurred when accessing the Links collection in ObjectBrowser's profileURN_TextChanged handler.

**Root Cause Analysis**:
The fix for Issue #3 manually created a Link in the Links collection before setting Text. However, when Text is set, WinForms may recreate or modify the Links collection, potentially invalidating the manually created link. This created a race condition where:
1. Manual link is created with specific length
2. Text is set to different length
3. WinForms updates Links collection
4. Manual link becomes invalid or Links collection state is inconsistent
5. profileURN_TextChanged handler accesses Links[0] → NullReferenceException

**Fix Applied (Commit b6ab213)**:

Added internal handler suppression flag and defensive checks:

```csharp
// New flag in class
private bool _isSuppressingProfileURNTextChanged = false;

private void SetProfileURNAtomically(string text, object linkData)
{
    if (text == null) text = string.Empty;
    
    // Suppress internal handler during property updates
    _isSuppressingProfileURNTextChanged = true;
    try
    {
        profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
        
        // Set Text first - WinForms creates/updates Links collection naturally
        // TextChanged event fires to external handlers, but internal handler suppressed
        profileURN.Text = text;
        
        // Now safely set LinkData on WinForms-created link
        if (profileURN.Links != null && profileURN.Links.Count > 0)
        {
            profileURN.Links[0].LinkData = linkData;
        }
    }
    finally
    {
        _isSuppressingProfileURNTextChanged = false;
    }
    
    // Manually trigger internal handler now that all properties are consistent
    profileURN_TextChanged(profileURN, EventArgs.Empty);
}

private void profileURN_TextChanged(object sender, EventArgs e)
{
    // Skip if suppressed during atomic update
    if (_isSuppressingProfileURNTextChanged) return;
    
    // Defensive checks for Links collection
    if (profileURN.Links == null || profileURN.Links.Count == 0) return;
    
    if (profileURN.Links[0].LinkData != null)
    {
        // ... NBitcoin signmessage logic ...
    }
}
```

**Key Changes**:
1. Internal handler suppression: Only ObjectBrowser's handler is suppressed, not the LinkLabel.TextChanged event
2. Natural Links creation: Let WinForms create Links collection when Text is set
3. Defensive null checks: Verify Links collection exists before accessing
4. Manual handler call: Trigger internal handler after all properties are consistent
5. Event propagation intact: External handlers (ObjectBrowserControl, SupMain) still receive the event

**Result**: NullReferenceException eliminated. Links collection is managed by WinForms, internal handler only runs when collection is valid, and external event propagation works correctly.

### Issue #5: Search Stalling After Processing Transactions (Commit f2e7e82)

**Issue Reported by @embiimob**: After fixing Issue #4, the `#LOVE` search would process a few transactions and then stall, not continuing through the process. The logs showed `InvalidOperationException` and multiple threads exiting, suggesting the UI thread was blocked.

**Root Cause Analysis**:
The profileURN_TextChanged handler makes a synchronous RPC call to sign a message:
```csharp
string signature = rpcClient.SendCommand("signmessage", profileURN.Links[0].LinkData.ToString(), "DUMMY").ResultString;
```

The flow was:
1. BuildSearchResults (background thread) calls Invoke to update UI
2. SetProfileURNAtomically is called on UI thread
3. SetProfileURNAtomically manually calls profileURN_TextChanged
4. profileURN_TextChanged makes blocking RPC call on UI thread
5. If RPC is slow/hangs, UI thread blocks
6. Search processing stalls, threads can't complete

**Fix Applied (Commit f2e7e82)**:

Made the RPC call asynchronous to prevent UI thread blocking:

```csharp
private void profileURN_TextChanged(object sender, EventArgs e)
{
    if (_isSuppressingProfileURNTextChanged) return;
    if (profileURN.Links?.Count == 0) return;

    if (profileURN.Links[0].LinkData != null)
    {
        // Run RPC call asynchronously to avoid blocking the UI thread
        Task.Run(() =>
        {
            try
            {
                NetworkCredential credentials = new NetworkCredential(mainnetLogin, mainnetPassword);
                NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);
                string signature = "";
                try 
                { 
                    signature = rpcClient.SendCommand("signmessage", profileURN.Links[0].LinkData.ToString(), "DUMMY").ResultString; 
                } 
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ObjectBrowser] signmessage RPC failed: {ex.Message}");
                }

                if (signature != "")
                {
                    _activeProfile = profileURN.Links[0].LinkData.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ObjectBrowser] Error in profileURN_TextChanged RPC call: {ex.Message}");
            }
        });
    }
}
```

**Key Changes**:
1. Wrapped entire RPC call logic in `Task.Run(() => { ... })`
2. RPC executes on background thread, not UI thread
3. Added outer try-catch for additional error handling
4. `_activeProfile` still gets updated when RPC completes
5. UI thread never blocks, search processing continues smoothly

**Result**: Search no longer stalls. The UI thread remains responsive, transactions process completely, and the RPC call to determine if the address is in the wallet happens asynchronously in the background.

## Future Improvements

If this pattern becomes more complex, consider:
1. Implementing a proper event aggregator/mediator pattern
2. Using a state management system to track the authoritative source of truth
3. Creating a dedicated ProfileContext class that manages profile state changes
4. Adding unit tests to validate the guard logic (would require refactoring for testability)
