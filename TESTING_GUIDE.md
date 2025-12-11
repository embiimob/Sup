# Testing Guide for Private Messaging Fixes

## Prerequisites

### Environment Setup
1. **Windows OS** (Windows 10 or later recommended)
2. **.NET Framework 4.7.2** or later
3. **Visual Studio 2019+** or MSBuild
4. **IPFS daemon** running locally
5. **Bitcoin testnet node** synced (for full testing)

### Test Data Setup
1. Create at least two test profiles with private keys in wallet
2. Send private messages between profiles with:
   - Plain text messages
   - Messages with SEC IPFS attachments (encrypted images)
   - Messages with SEC IPFS attachments (encrypted videos/audio)
   - Messages with multiple attachments

## Building the Application

### Visual Studio
1. Open `SUP.sln` in Visual Studio
2. Select **Debug** or **Release** configuration
3. Build → Build Solution (Ctrl+Shift+B)
4. Check Output window for any errors

### MSBuild Command Line
```bash
cd /path/to/Sup
msbuild SUP.sln /p:Configuration=Release
```

### Expected Build Output
- `bin/Debug/SUP.exe` or `bin/Release/SUP.exe`
- All dependencies should be copied to output directory
- No build warnings related to IpfsHelper.cs or SupMain.cs

## Manual Testing Checklist

### Test 1: Basic Message Loading (No Attachments)
**Objective**: Verify basic message loading still works

**Steps**:
1. Launch SUP.exe
2. Search for your test profile
3. Click the 🤐 (private message) button
4. Observe message loading

**Expected Results**:
- ✅ Messages load quickly (< 2 seconds)
- ✅ UI remains responsive throughout
- ✅ All text messages are displayed correctly
- ✅ Timestamps are correct
- ✅ Profile images appear (or default avatar if missing)

**Known Issues to Ignore**:
- ⚠️ Profile image IPFS loading is still blocking (documented, low priority)

---

### Test 2: Messages with SEC Attachments (Happy Path)
**Objective**: Verify SEC attachments load asynchronously without blocking

**Setup**:
- Ensure IPFS daemon is running (`ipfs daemon`)
- Have messages with SEC IPFS attachments available

**Steps**:
1. Open private messages with SEC attachments
2. Observe loading behavior

**Expected Results**:
- ✅ Text messages appear immediately (< 1 second)
- ✅ "Loading..." or no attachment initially
- ✅ Attachments appear within 60 seconds
- ✅ UI remains scrollable while attachments load
- ✅ Can interact with other messages while waiting
- ✅ Multiple attachments load in parallel
- ✅ Decrypted images/videos display correctly

**Metrics to Record**:
- Time for text to appear: _____ seconds
- Time for first attachment: _____ seconds
- Time for all attachments: _____ seconds
- UI froze: Yes / No
- Any errors: ___________________

---

### Test 3: IPFS Timeout Scenario
**Objective**: Verify graceful handling when IPFS is slow

**Setup**:
- Slow down network (optional: use traffic shaping)
- OR use an IPFS hash that doesn't exist
- OR temporarily pause IPFS daemon after starting message load

**Steps**:
1. Open private messages with SEC attachments
2. If using pause method: quickly pause ipfs.exe in Task Manager
3. Wait for timeout (up to 60 seconds)

**Expected Results**:
- ✅ Text messages appear immediately
- ✅ UI remains responsive
- ✅ After timeout, error panel appears:
  - Red background
  - ⚠️ icon
  - Error message with hash
- ✅ Other messages continue to load
- ✅ No application freeze
- ✅ No crash

**Failure Indicators**:
- ❌ UI freezes
- ❌ Blank screen
- ❌ Application crashes
- ❌ Other messages don't load

---

### Test 4: IPFS Daemon Not Running
**Objective**: Verify behavior when IPFS is completely unavailable

**Setup**:
- Stop IPFS daemon (`kill` ipfs.exe or `ipfs shutdown`)

**Steps**:
1. Open private messages with SEC attachments
2. Observe behavior

**Expected Results**:
- ✅ Text messages appear immediately
- ✅ Error panels appear for attachments
- ✅ No application freeze
- ✅ Can still scroll and read text messages

---

### Test 5: Mixed Content
**Objective**: Verify handling of conversations with mixed content

**Test Data**:
- Messages with only text
- Messages with only SEC attachments
- Messages with text + multiple SEC attachments
- Messages with non-SEC attachments (regular IPFS)

**Steps**:
1. Open such a conversation
2. Observe loading behavior

**Expected Results**:
- ✅ All text appears immediately
- ✅ Non-SEC attachments load normally
- ✅ SEC attachments load asynchronously
- ✅ Failed SEC attachments show errors
- ✅ No message types block others

---

### Test 6: Rapid Navigation
**Objective**: Verify no resource leaks or accumulation

**Steps**:
1. Open private messages (with attachments)
2. Quickly navigate away
3. Repeat 10 times rapidly
4. Check Task Manager for SUP.exe

**Expected Results**:
- ✅ No memory leak (memory stays stable)
- ✅ No process leak (ipfs.exe count doesn't grow)
- ✅ No UI artifacts or errors
- ✅ Application remains stable

---

### Test 7: Concurrent Conversations
**Objective**: Verify multiple conversations work independently

**Steps**:
1. Open conversation A with SEC attachments
2. While A is loading, switch to conversation B
3. Switch back to A
4. Switch to conversation C

**Expected Results**:
- ✅ All conversations load independently
- ✅ No cross-contamination of attachments
- ✅ Switching doesn't cause errors
- ✅ Attachments continue loading in background

---

### Test 8: Encryption/Decryption Validation
**Objective**: Ensure security is maintained

**Steps**:
1. Send an encrypted message with SEC attachment
2. View the message as recipient
3. Check the decrypted content

**Expected Results**:
- ✅ SEC file is encrypted on disk (binary/gibberish)
- ✅ Decrypted content displays correctly
- ✅ Only recipient with private key can decrypt
- ✅ No plaintext leakage

**Security Checks**:
- [ ] Verify `root/{hash}/SEC` file is encrypted (open in text editor)
- [ ] Verify decryption uses recipient's private key
- [ ] Verify no plaintext appears in temp directories

---

### Test 9: Large Attachments
**Objective**: Verify handling of large SEC files

**Test Data**: SEC attachments > 10MB

**Steps**:
1. Open message with large SEC attachment
2. Monitor behavior during download

**Expected Results**:
- ✅ UI remains responsive
- ✅ Text appears immediately
- ✅ Progress is shown (or timeout message after 60s)
- ✅ Memory usage is reasonable
- ✅ Large files either load or show error gracefully

---

### Test 10: Profile Image IPFS Loading
**Objective**: Document profile image loading behavior (known limitation)

**Steps**:
1. View private messages from profile with IPFS-hosted image
2. Note if UI blocks during profile image load

**Expected Results**:
- ⚠️ Profile image loading may still block briefly
- ✅ This is a known limitation (documented)
- ✅ Does not prevent messages from loading
- ✅ Only affects profile picture, not message content

**Note**: This is intentionally not fixed in this PR as it's lower priority

---

## Debugging

### Enable Debug Logging
The application writes to Debug output. To view:

**Visual Studio**:
- Debug → Windows → Output
- Select "Debug" from dropdown

**DebugView**:
- Download Sysinternals DebugView
- Run as Administrator
- Capture → Capture Global Win32

### Key Log Messages to Look For

```
[IpfsHelper.GetAsync] Starting IPFS get for {hash}
[IpfsHelper.GetAsync] Successfully retrieved {hash}
[IpfsHelper.GetAsync] IPFS get timeout for {hash}

[LoadSecAttachmentAsync] Starting SEC attachment load for {hash}
[LoadSecAttachmentAsync] Already processing {hash}, skipping
[LoadSecAttachmentAsync] SEC file already exists for {hash}
[LoadSecAttachmentAsync] Failed to process downloaded file

[DisplaySecAttachmentAsync] Decrypting SEC attachment for {hash}
[DisplaySecAttachmentAsync] Successfully displayed SEC attachment
[DisplaySecAttachmentAsync] Decryption failed for {hash}

[IpfsHelper.PinAsync] Pin completed for {hash}
```

### Common Issues and Solutions

**Issue**: Build fails with "reference assemblies not found"
- **Solution**: Install .NET Framework 4.7.2 Developer Pack

**Issue**: Application won't start
- **Solution**: Check if all DLLs are in bin directory, rebuild

**Issue**: Messages don't load
- **Solution**: Verify Bitcoin testnet is synced and RPC is accessible

**Issue**: All attachments fail
- **Solution**: Check IPFS daemon is running: `ipfs version`

**Issue**: Specific attachment fails repeatedly
- **Solution**: Check if hash exists in IPFS: `ipfs get {hash}`

## Performance Benchmarks

Record these metrics during testing:

### Before Fix (Historical Reference)
- Message load time: 5-550 seconds per message with attachment
- UI freeze duration: 5-550 seconds
- Recovery from IPFS hang: Not possible (requires task kill)

### After Fix (Expected)
- Message text load time: < 1 second
- Attachment load time: Up to 60 seconds per attachment (background)
- UI freeze duration: 0 seconds (responsive throughout)
- Recovery from IPFS hang: Automatic (timeout + error display)

### Your Results
| Metric | Your Result |
|--------|-------------|
| Text load time | ___ seconds |
| First attachment time | ___ seconds |
| All attachments time | ___ seconds |
| UI remained responsive | Yes / No |
| Timeout worked correctly | Yes / No |
| Errors displayed gracefully | Yes / No |

## Regression Testing

Ensure these existing features still work:

- [ ] Public messages still load correctly
- [ ] Community feed still works
- [ ] Object minting still works
- [ ] Profile minting still works
- [ ] Non-SEC IPFS content loads correctly
- [ ] Other IPFS operations unaffected

## Test Report Template

```
Date: __________
Tester: __________
Version: __________
OS: Windows __ 
.NET Framework: __________

Test Results:
[ ] Test 1: Basic Messages - PASS / FAIL
[ ] Test 2: SEC Attachments - PASS / FAIL
[ ] Test 3: IPFS Timeout - PASS / FAIL
[ ] Test 4: IPFS Unavailable - PASS / FAIL
[ ] Test 5: Mixed Content - PASS / FAIL
[ ] Test 6: Rapid Navigation - PASS / FAIL
[ ] Test 7: Concurrent Conversations - PASS / FAIL
[ ] Test 8: Encryption - PASS / FAIL
[ ] Test 9: Large Attachments - PASS / FAIL
[ ] Test 10: Profile Images - DOCUMENTED

Critical Issues Found: ____________
Minor Issues Found: ____________
Performance Notes: ____________
Recommendations: ____________
```

## Security Test Checklist

- [ ] SEC files remain encrypted on disk
- [ ] Decryption requires correct private key
- [ ] No plaintext leakage in temp files
- [ ] IPFS timeout doesn't expose sensitive data
- [ ] Error messages don't leak private information
- [ ] Process isolation maintained (ipfs.exe separate)
- [ ] No new security warnings in build
- [ ] CodeQL scan passes (already confirmed)

## Sign-Off

After completing all tests:

```
Tested by: __________
Date: __________
Build Version: __________
All critical tests passed: YES / NO
Ready for production: YES / NO / WITH NOTES

Notes:
_________________________________
_________________________________
_________________________________
```
