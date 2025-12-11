# Messaging Architecture Documentation

## Overview

The Sup application supports three types of messaging:
1. **Private Messages** - Encrypted direct messages between users
2. **Public Messages** - Public posts from a specific profile
3. **Community Feed** - Aggregated public messages from all followed profiles

All three messaging types now use a shared normalization layer to ensure consistent, stable, and duplicate-free message display.

## Message Data Model

### Canonical Message Shape

All messages in the system conform to the `MessageObject` class defined in `P2FK/contracts/OBJ.cs`:

```csharp
public class MessageObject
{
    public string Message { get; set; }           // Message text content
    public string FromAddress { get; set; }       // Sender's address
    public string ToAddress { get; set; }         // Recipient's address (if applicable)
    public DateTime BlockDate { get; set; }       // Blockchain timestamp
    public string TransactionId { get; set; }     // Unique transaction identifier
}
```

### Key Fields

- **TransactionId**: The unique identifier for each message. This is the primary key used for deduplication.
- **BlockDate**: The timestamp when the message was recorded on the blockchain. Primary sort key.
- **FromAddress**: The address of the message sender.
- **ToAddress**: The address of the message recipient (for private and targeted public messages).
- **Message**: The actual message content (may be encrypted for private messages).

## Message Normalization Layer

### MessageNormalizer Class

Located in `MessageNormalizer.cs`, this static class provides centralized message processing:

#### Key Methods

**`Normalize(IEnumerable<MessageObject> messages)`**
- Converts MessageObjects to NormalizedMessage format
- Deduplicates by TransactionId
- Sorts by BlockDate DESC, then TransactionId DESC
- Returns stable, deduplicated message list

**`Merge(IEnumerable<NormalizedMessage> existing, IEnumerable<MessageObject> newMessages)`**
- Combines existing displayed messages with new messages
- Deduplicates across both sets
- Maintains sort order
- Used for pagination and real-time updates

**`Paginate(IEnumerable<NormalizedMessage> messages, int skip, int take)`**
- Extracts a specific page of messages
- Maintains sort order
- Used for infinite scroll / load more functionality

### Sorting Strategy

Messages are sorted with a two-level key:

1. **Primary**: `BlockDate` descending (newest messages first)
2. **Secondary**: `TransactionId` descending (stable tiebreaker for same timestamp)

This ensures:
- Chronological ordering (newest first)
- Deterministic ordering (same input always produces same output)
- Stable pagination (messages don't jump around when loading more)

### Deduplication Strategy

Deduplication uses `TransactionId` as the unique key:

- Each message has exactly one TransactionId
- When duplicates are detected (same TransactionId), only the first occurrence is kept
- This prevents the same message from appearing multiple times due to:
  - Multiple data source queries
  - Pagination overlaps
  - Real-time updates vs. initial loads
  - Network retries

## Data Flow

### Private Messages

```
User clicks Private Messages button
    ↓
RefreshPrivateSupMessages() called
    ↓
RefreshPrivateSupMessagesAsync() executes asynchronously
    ↓
GetPrivateMessagesByAddress() fetches encrypted messages
    ↓
MessageNormalizer.Normalize() deduplicates and sorts
    ↓
Filter out already-displayed messages using displayedPrivateMessageIds
    ↓
For each new message:
    - Decrypt message content
    - Load attachments asynchronously
    - Render to UI
    - Add TransactionId to displayedPrivateMessageIds
```

### Public Messages

```
User clicks Public Messages button
    ↓
RefreshSupMessages() called
    ↓
GetPublicMessagesByAddress() fetches public messages
    ↓
MessageNormalizer.Normalize() deduplicates and sorts
    ↓
Filter out already-displayed messages using displayedPublicMessageIds
    ↓
For each new message:
    - Load profile images
    - Render message and attachments
    - Add TransactionId to displayedPublicMessageIds
```

### Community Feed

```
User clicks Community Feed button
    ↓
RefreshCommunityMessages() called
    ↓
For each followed profile:
    - GetPublicMessagesByAddress() fetches messages
    - Collect all messages into single list
    ↓
MessageNormalizer.Normalize() deduplicates and sorts all messages
    ↓
Filter out already-displayed messages using displayedCommunityMessageIds
    ↓
MessageNormalizer.Paginate() extracts current page
    ↓
For each message in page:
    - Render message and attachments
    - Add TransactionId to displayedCommunityMessageIds
```

## State Management

### Displayed Message Tracking

Three HashSets track which messages have been displayed:

```csharp
private HashSet<string> displayedPrivateMessageIds;
private HashSet<string> displayedPublicMessageIds;
private HashSet<string> displayedCommunityMessageIds;
```

These sets are:
- Populated when messages are rendered to the UI
- Cleared when the user switches conversations or clears the view
- Used to filter out messages that have already been displayed

### View Isolation

Each message type maintains its own state:
- Switching from public to private messages doesn't mix their displayed IDs
- Community feed has its own tracking separate from individual profile views
- Clearing one view doesn't affect the others (unless switching view types)

## Pagination

### Load More Pattern

1. User scrolls to bottom of message list
2. `numMessagesDisplayed` (or equivalent counter) is incremented
3. Next page of messages is fetched with `skip = numMessagesDisplayed`
4. New messages are normalized and filtered against `displayedXXXMessageIds`
5. Only truly new messages are rendered
6. Message IDs are added to tracking set

### Benefits

- No duplicate messages when loading more
- Stable ordering (messages don't re-order as new ones load)
- Efficient (only renders messages not already displayed)
- Works correctly even if server returns overlapping results

## Ordering Consistency

### Database Layer (OBJ.cs)

Both `GetPublicMessagesByAddress` and `GetPrivateMessagesByAddress` now use consistent ordering:

```csharp
.OrderByDescending(obj => obj.BlockDate)
.ThenByDescending(obj => obj.TransactionId)
```

### Normalization Layer (MessageNormalizer.cs)

The `NormalizedMessage.Comparer` implements the same ordering:

```csharp
public int Compare(NormalizedMessage x, NormalizedMessage y)
{
    // Primary: BlockDate descending (newer first)
    int dateCompare = y.BlockDate.CompareTo(x.BlockDate);
    if (dateCompare != 0) return dateCompare;
    
    // Secondary: TransactionId descending (stable tiebreaker)
    return string.Compare(y.TransactionId, x.TransactionId, StringComparison.Ordinal);
}
```

### UI Layer (SupMain.cs)

Messages are rendered in the order provided by MessageNormalizer, maintaining the sort order throughout the display pipeline.

## Error Handling

### Missing TransactionId

Messages without a TransactionId are filtered out during normalization:

```csharp
.Where(m => m != null && !string.IsNullOrEmpty(m.TransactionId))
```

### Null Messages

Null messages are filtered at multiple stages:
- During normalization
- During conversion to NormalizedMessage
- During deduplication

### Decryption Failures (Private Messages)

If a private message cannot be decrypted:
- The message is still added to the list with `Message = "<UNABLE TO DECRYPT>"`
- The message is rendered with error indication
- The TransactionId is still tracked to prevent duplicate error messages

## Future Enhancements

### Real-Time Updates

To add real-time message updates (via SignalR or polling):

1. New messages arrive via real-time channel
2. Convert to `MessageObject` format
3. Use `MessageNormalizer.Merge(existing, newMessages)`
4. Update UI with only the new messages
5. Add new TransactionIds to tracking set

### Optimistic Sending

To add optimistic message sending:

1. Generate temporary TransactionId (e.g., "temp-{guid}")
2. Display message immediately with temp ID
3. When server confirms, receive real TransactionId
4. Replace temp ID with real ID in tracking set
5. MessageNormalizer handles deduplication automatically

### Message Updates/Edits

If messages can be edited:

1. Server sends updated message with same TransactionId
2. Remove old version from tracking set
3. Add updated message
4. Re-normalize and re-render affected portion of UI

## Testing Recommendations

### Unit Tests

1. **Deduplication**: Feed same message multiple times, verify only one instance remains
2. **Ordering**: Feed messages out of order, verify correct sort
3. **Pagination**: Verify correct skip/take behavior
4. **Merge**: Verify combining existing and new messages works correctly
5. **Empty/Null**: Verify handling of edge cases

### Integration Tests

1. **Private Messages**: Load conversation, scroll to load more, verify no duplicates
2. **Public Messages**: Switch between profiles, verify messages don't leak
3. **Community Feed**: Follow multiple profiles, verify correct aggregation and ordering
4. **Clear/Switch**: Clear one view, verify tracking sets reset correctly

### Manual Testing

1. Load messages with pagination
2. Switch between message types
3. Clear and reload
4. Test with slow network (verify no duplicate renders during loading)
5. Test with identical timestamps (verify stable secondary sort)

## Troubleshooting

### Messages Appearing Multiple Times

- Check if TransactionId is properly set on all messages
- Verify tracking set is being populated
- Verify tracking set is being checked before rendering
- Check if ClearMessages is resetting the correct tracking set

### Messages Out of Order

- Verify GetPublicMessagesByAddress and GetPrivateMessagesByAddress are using correct OrderBy
- Verify MessageNormalizer.Comparer is being used
- Check if UI is rendering messages in a different order than provided

### Messages Disappearing When Loading More

- Verify skip parameter is being calculated correctly
- Ensure numMessagesDisplayed counter is being incremented properly
- Check if tracking set is preventing re-display of messages that should appear

### Community Feed Showing Stale Data

- Verify GetPublicMessagesByAddress is being called for all followed profiles
- Check if normalization is combining all messages correctly
- Verify pagination is extracting the correct range

## References

- `MessageNormalizer.cs` - Shared normalization layer
- `P2FK/contracts/OBJ.cs` - Message data retrieval methods
- `SupMain.cs` - UI integration (RefreshPrivateSupMessagesAsync, RefreshSupMessages, RefreshCommunityMessages)
- `PRIVATE_MESSAGING_FIXES.md` - Previous async/IPFS improvements
