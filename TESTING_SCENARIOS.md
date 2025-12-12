# Testing Scenarios for Circular Update Loop Fix

## Prerequisites
- Build the application in Windows environment with .NET Framework 4.7.2
- Have at least one blockchain daemon running (Bitcoin testnet recommended)
- Enable debug output in Visual Studio to see diagnostic messages

## Test Scenario 1: Hashtag Search in ObjectBrowser

**Purpose**: Verify that searching for hashtags doesn't cause circular updates or crashes

**Steps**:
1. Launch the SUP application
2. Navigate to or ensure the ObjectBrowser is visible
3. In the ObjectBrowser search field, type `#LOVE`
4. Press Enter or click the search button

**Expected Results**:
- ✅ Search results are displayed in the ObjectBrowser panel
- ✅ The profileURN in the main SupMain window is updated to show `#LOVE`
- ✅ Social feed in SupMain displays posts tagged with #LOVE
- ✅ NO red X appears on the profileURN
- ✅ NO unhandled exceptions occur
- ✅ NO infinite loop or system freeze
- ✅ Debug output shows:
  - `[ObjectBrowser] Setting profileURN to: #LOVE`
  - `[ObjectBrowserControl] Firing ProfileURNChanged event for: #LOVE`
  - `[SupMain] Processing ProfileURNChanged - new profileURN: #LOVE`

**Failure Indicators**:
- ❌ Red X appears on profileURN
- ❌ Application crashes or throws unhandled exception
- ❌ Application freezes or becomes unresponsive
- ❌ Debug output shows repeated messages indicating a loop

## Test Scenario 2: Profile Image Click in SupMain Feed

**Purpose**: Verify that clicking profile images in the social feed doesn't cause circular updates

**Steps**:
1. Launch the SUP application
2. Ensure the social feed is populated with posts (search for a popular hashtag first if needed)
3. Click on a profile image in the social feed

**Expected Results**:
- ✅ The ObjectBrowser panel updates to show the clicked profile's objects
- ✅ The main profileURN updates to show the selected profile address
- ✅ NO circular updates occur
- ✅ NO red X appears on profileURN
- ✅ NO unhandled exceptions
- ✅ Debug output shows:
  - `[SupMain] Updating ObjectBrowser search to: <profile-address>`
  - `[ObjectBrowser] Skipping profileURN update - external update in progress`
  - `[ObjectBrowserControl] Suppressing ProfileURNChanged event - external update in progress`
  - NO repeated messages indicating a loop

**Failure Indicators**:
- ❌ Multiple rapid updates visible in the UI
- ❌ Debug output shows repeated update messages
- ❌ Application becomes slow or unresponsive

## Test Scenario 3: Rapid Sequential Searches

**Purpose**: Verify that rapid sequential searches don't cause race conditions or circular updates

**Steps**:
1. Launch the SUP application
2. Type `#LOVE` and press Enter
3. Immediately type `#MUSIC` and press Enter
4. Immediately type `#ART` and press Enter
5. Click on a profile image in the feed
6. Immediately search for `#LOVE` again

**Expected Results**:
- ✅ Each search completes successfully
- ✅ UI updates reflect the most recent search
- ✅ NO red X appears
- ✅ NO crashes or exceptions
- ✅ Guard flags are properly reset between operations
- ✅ Debug output shows proper guard flag management

**Failure Indicators**:
- ❌ Searches fail or get stuck
- ❌ UI shows mixed state from multiple searches
- ❌ Guard flags remain stuck in 'true' state

## Test Scenario 4: Empty and Invalid Searches

**Purpose**: Verify that edge cases are handled gracefully

**Steps**:
1. Launch the SUP application
2. Search for an empty string (just press Enter with empty field)
3. Search for a non-existent hashtag like `#THISTAGDOESNOTEXIST123456`
4. Search for a malformed address
5. Search for special characters: `#!@#$%^&*()`

**Expected Results**:
- ✅ Application handles empty searches gracefully (shows "anon" or no results)
- ✅ Non-existent hashtags show no results or appropriate message
- ✅ Malformed addresses are handled without crashes
- ✅ Special characters don't cause exceptions
- ✅ Guard flags still work correctly
- ✅ Debug output shows appropriate handling of edge cases

**Failure Indicators**:
- ❌ Null reference exceptions
- ❌ Application crashes
- ❌ profileURN shows red X incorrectly

## Test Scenario 5: Profile URN Null/Invalid State Recovery

**Purpose**: Verify that defensive checks prevent issues with null or invalid profileURN

**Steps**:
1. Launch the SUP application
2. Perform a normal search to populate profileURN
3. Monitor behavior when switching between different views
4. Click on elements that might clear or invalidate profileURN

**Expected Results**:
- ✅ Null checks prevent null reference exceptions
- ✅ Empty profileURN.Text is handled gracefully
- ✅ Missing LinkData doesn't cause crashes
- ✅ Debug output shows defensive checks working: "Skipping ProfileURNChanged - profileURN text is null or empty"

**Failure Indicators**:
- ❌ NullReferenceException in logs
- ❌ Application crashes when profileURN is in invalid state

## Debug Output Guide

When running tests, monitor the debug output window for these key messages:

### Normal Operation Indicators:
```
[ObjectBrowser] Setting profileURN to: <value>
[ObjectBrowserControl] Firing ProfileURNChanged event for: <value>
[SupMain] Processing ProfileURNChanged - new profileURN: <value>
[SupMain] Updating ObjectBrowser search to: <value>
[ObjectBrowser] Skipping profileURN update - external update in progress
[ObjectBrowserControl] Suppressing ProfileURNChanged event - external update in progress
```

### Circular Update Prevented Indicators:
```
[SupMain] Ignoring ProfileURNChanged event - currently updating ObjectBrowser from SupMain
[ObjectBrowser] Skipping profileURN update - external update in progress
[ObjectBrowserControl] Suppressing ProfileURNChanged event - external update in progress
```

### Error Indicators (should investigate if seen):
```
[SupMain] Error in OBControl_ProfileURNChanged: <exception-type>: <message>
[SupMain] Stack trace: <stack-trace>
[SupMain] Skipping ProfileURNChanged - profileURN text is null or empty
```

### Warning Signs (circular loop still occurring):
```
[ObjectBrowser] Setting profileURN to: <value>
[ObjectBrowserControl] Firing ProfileURNChanged event for: <value>
[SupMain] Processing ProfileURNChanged - new profileURN: <value>
[SupMain] Updating ObjectBrowser search to: <value>
[ObjectBrowser] Setting profileURN to: <value>   <- REPEATED - BAD!
[ObjectBrowserControl] Firing ProfileURNChanged event for: <value>   <- REPEATED - BAD!
```

## Performance Considerations

While testing, also monitor:
- CPU usage should remain reasonable (no sustained 100% usage)
- Memory usage should be stable (no rapid increases indicating memory leaks)
- UI should remain responsive throughout operations

## Regression Testing

After verifying the fix works, also test these existing features to ensure nothing broke:
1. Minting new profiles
2. Creating objects
3. Following/unfollowing profiles
4. Blocking profiles
5. Sending public messages
6. Sending private messages
7. Opening other search tools (JukeBox, SupFlix, INQSearch)
8. Connecting to blockchain daemons
9. Loading profile links
10. Viewing profile details

## Known Limitations

This fix addresses the circular update loop but does not change:
- The overall architecture of event communication between components
- The way profileURN is stored and managed
- Any other existing bugs unrelated to circular updates

## Reporting Issues

If you encounter any issues during testing:
1. Note the exact steps taken
2. Copy the debug output showing the issue
3. Include any error messages or exceptions
4. Note the state of the UI (screenshots helpful)
5. Describe expected vs actual behavior
