using System;
using System.Collections.Generic;
using System.Linq;
using SUP.P2FK;

namespace SUP
{
    /// <summary>
    /// Provides centralized message normalization, deduplication, and sorting
    /// for private, public, and community messaging features.
    /// </summary>
    public static class MessageNormalizer
    {
        /// <summary>
        /// Represents a normalized message with a unique identifier and stable sort keys.
        /// </summary>
        public class NormalizedMessage
        {
            public string TransactionId { get; set; }
            public string Message { get; set; }
            public string FromAddress { get; set; }
            public string ToAddress { get; set; }
            public DateTime BlockDate { get; set; }
            
            // For stable sorting and deduplication
            public string UniqueKey => TransactionId;
            
            // Primary sort: BlockDate descending, Secondary: TransactionId descending
            public class Comparer : IComparer<NormalizedMessage>
            {
                public int Compare(NormalizedMessage x, NormalizedMessage y)
                {
                    if (x == null && y == null) return 0;
                    if (x == null) return 1;
                    if (y == null) return -1;
                    
                    // Primary: BlockDate descending (newer first)
                    int dateCompare = y.BlockDate.CompareTo(x.BlockDate);
                    if (dateCompare != 0) return dateCompare;
                    
                    // Secondary: TransactionId descending (stable tiebreaker)
                    return string.Compare(y.TransactionId, x.TransactionId, StringComparison.Ordinal);
                }
            }
        }
        
        /// <summary>
        /// Converts a MessageObject to a NormalizedMessage.
        /// </summary>
        public static NormalizedMessage ToNormalized(MessageObject msg)
        {
            if (msg == null) return null;
            
            return new NormalizedMessage
            {
                TransactionId = msg.TransactionId ?? string.Empty,
                Message = msg.Message ?? string.Empty,
                FromAddress = msg.FromAddress ?? string.Empty,
                ToAddress = msg.ToAddress ?? string.Empty,
                BlockDate = msg.BlockDate
            };
        }
        
        /// <summary>
        /// Normalizes, deduplicates, and sorts a collection of messages.
        /// </summary>
        /// <param name="messages">Input messages (can contain duplicates)</param>
        /// <returns>Deduplicated and sorted messages</returns>
        public static List<NormalizedMessage> Normalize(IEnumerable<MessageObject> messages)
        {
            if (messages == null) return new List<NormalizedMessage>();
            
            // Convert to normalized messages
            var normalized = messages
                .Where(m => m != null && !string.IsNullOrEmpty(m.TransactionId))
                .Select(ToNormalized)
                .ToList();
            
            // Deduplicate by TransactionId
            var deduplicated = normalized
                .GroupBy(m => m.UniqueKey)
                .Select(g => g.First()) // Take first occurrence of each unique TransactionId
                .ToList();
            
            // Sort stably: BlockDate DESC, then TransactionId DESC
            deduplicated.Sort(new NormalizedMessage.Comparer());
            
            return deduplicated;
        }
        
        /// <summary>
        /// Merges new messages with existing messages, deduplicating and maintaining sort order.
        /// Useful for pagination and real-time updates.
        /// </summary>
        /// <param name="existing">Already displayed messages</param>
        /// <param name="newMessages">New messages to merge in</param>
        /// <returns>Merged, deduplicated, and sorted messages</returns>
        public static List<NormalizedMessage> Merge(
            IEnumerable<NormalizedMessage> existing,
            IEnumerable<MessageObject> newMessages)
        {
            if (existing == null) existing = new List<NormalizedMessage>();
            if (newMessages == null) newMessages = new List<MessageObject>();
            
            // Combine existing and new messages
            var allMessages = existing
                .Concat(newMessages.Select(ToNormalized))
                .Where(m => m != null && !string.IsNullOrEmpty(m.UniqueKey))
                .ToList();
            
            // Deduplicate by TransactionId
            var deduplicated = allMessages
                .GroupBy(m => m.UniqueKey)
                .Select(g => g.First())
                .ToList();
            
            // Sort stably
            deduplicated.Sort(new NormalizedMessage.Comparer());
            
            return deduplicated;
        }
        
        /// <summary>
        /// Gets a page of messages from a normalized collection.
        /// </summary>
        /// <param name="messages">Normalized message collection</param>
        /// <param name="skip">Number of messages to skip</param>
        /// <param name="take">Number of messages to take</param>
        /// <returns>Paginated messages</returns>
        public static List<NormalizedMessage> Paginate(
            IEnumerable<NormalizedMessage> messages,
            int skip,
            int take)
        {
            if (messages == null) return new List<NormalizedMessage>();
            
            return messages
                .Skip(skip)
                .Take(take)
                .ToList();
        }
        
        /// <summary>
        /// Converts normalized messages back to MessageObjects.
        /// </summary>
        public static List<MessageObject> ToMessageObjects(IEnumerable<NormalizedMessage> normalized)
        {
            if (normalized == null) return new List<MessageObject>();
            
            return normalized.Select(n => new MessageObject
            {
                TransactionId = n.TransactionId,
                Message = n.Message,
                FromAddress = n.FromAddress,
                ToAddress = n.ToAddress,
                BlockDate = n.BlockDate
            }).ToList();
        }
    }
}
