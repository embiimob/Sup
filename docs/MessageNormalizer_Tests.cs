// This file contains example tests for MessageNormalizer
// These tests are provided as documentation and would need a test framework (e.g., NUnit, xUnit, MSTest)
// to be executed. The repository does not currently have test infrastructure.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SUP;
using SUP.P2FK;

namespace SUP.Tests
{
    /// <summary>
    /// Example tests for MessageNormalizer functionality.
    /// These demonstrate expected behavior and can be used as a basis for actual unit tests.
    /// </summary>
    public class MessageNormalizerTests
    {
        /// <summary>
        /// Test: Deduplication removes duplicate messages based on TransactionId
        /// </summary>
        public void Test_Normalize_RemovesDuplicates()
        {
            // Arrange
            var messages = new List<MessageObject>
            {
                new MessageObject 
                { 
                    TransactionId = "tx1", 
                    Message = "Hello", 
                    FromAddress = "addr1",
                    BlockDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture)
                },
                new MessageObject 
                { 
                    TransactionId = "tx1",  // Duplicate!
                    Message = "Hello", 
                    FromAddress = "addr1",
                    BlockDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture)
                },
                new MessageObject 
                { 
                    TransactionId = "tx2", 
                    Message = "World", 
                    FromAddress = "addr2",
                    BlockDate = DateTime.Parse("2024-01-02", CultureInfo.InvariantCulture)
                }
            };

            // Act
            var normalized = MessageNormalizer.Normalize(messages);

            // Assert
            // Should have 2 messages (tx1 appears once, tx2 appears once)
            if (normalized.Count != 2)
                throw new Exception($"Expected 2 messages, got {normalized.Count}");
            
            // Should have tx1 and tx2
            if (!normalized.Any(m => m.TransactionId == "tx1"))
                throw new Exception("Missing tx1");
            if (!normalized.Any(m => m.TransactionId == "tx2"))
                throw new Exception("Missing tx2");
        }

        /// <summary>
        /// Test: Messages are sorted by BlockDate descending, then TransactionId descending
        /// </summary>
        public void Test_Normalize_SortsCorrectly()
        {
            // Arrange
            var messages = new List<MessageObject>
            {
                new MessageObject 
                { 
                    TransactionId = "tx1", 
                    Message = "Old", 
                    FromAddress = "addr1",
                    BlockDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture)
                },
                new MessageObject 
                { 
                    TransactionId = "tx3", 
                    Message = "Newest", 
                    FromAddress = "addr3",
                    BlockDate = DateTime.Parse("2024-01-03", CultureInfo.InvariantCulture)
                },
                new MessageObject 
                { 
                    TransactionId = "tx2", 
                    Message = "Middle", 
                    FromAddress = "addr2",
                    BlockDate = DateTime.Parse("2024-01-02", CultureInfo.InvariantCulture)
                }
            };

            // Act
            var normalized = MessageNormalizer.Normalize(messages);

            // Assert
            // Should be sorted newest first: tx3, tx2, tx1
            if (normalized[0].TransactionId != "tx3")
                throw new Exception($"Expected tx3 first, got {normalized[0].TransactionId}");
            if (normalized[1].TransactionId != "tx2")
                throw new Exception($"Expected tx2 second, got {normalized[1].TransactionId}");
            if (normalized[2].TransactionId != "tx1")
                throw new Exception($"Expected tx1 third, got {normalized[2].TransactionId}");
        }

        /// <summary>
        /// Test: When BlockDates are the same, TransactionId is used as tiebreaker
        /// </summary>
        public void Test_Normalize_UsesTransactionIdAsTiebreaker()
        {
            // Arrange
            var sameDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture);
            var messages = new List<MessageObject>
            {
                new MessageObject 
                { 
                    TransactionId = "tx_aaa", 
                    Message = "A", 
                    FromAddress = "addr1",
                    BlockDate = sameDate
                },
                new MessageObject 
                { 
                    TransactionId = "tx_zzz", 
                    Message = "Z", 
                    FromAddress = "addr2",
                    BlockDate = sameDate
                },
                new MessageObject 
                { 
                    TransactionId = "tx_mmm", 
                    Message = "M", 
                    FromAddress = "addr3",
                    BlockDate = sameDate
                }
            };

            // Act
            var normalized = MessageNormalizer.Normalize(messages);

            // Assert
            // Should be sorted by TransactionId descending: tx_zzz, tx_mmm, tx_aaa
            if (normalized[0].TransactionId != "tx_zzz")
                throw new Exception($"Expected tx_zzz first, got {normalized[0].TransactionId}");
            if (normalized[1].TransactionId != "tx_mmm")
                throw new Exception($"Expected tx_mmm second, got {normalized[1].TransactionId}");
            if (normalized[2].TransactionId != "tx_aaa")
                throw new Exception($"Expected tx_aaa third, got {normalized[2].TransactionId}");
        }

        /// <summary>
        /// Test: Merge combines existing and new messages without duplicates
        /// </summary>
        public void Test_Merge_CombinesWithoutDuplicates()
        {
            // Arrange
            var existing = new List<MessageNormalizer.NormalizedMessage>
            {
                new MessageNormalizer.NormalizedMessage
                {
                    TransactionId = "tx1",
                    Message = "Existing",
                    FromAddress = "addr1",
                    BlockDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture)
                }
            };

            var newMessages = new List<MessageObject>
            {
                new MessageObject 
                { 
                    TransactionId = "tx1",  // Duplicate of existing
                    Message = "Existing", 
                    FromAddress = "addr1",
                    BlockDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture)
                },
                new MessageObject 
                { 
                    TransactionId = "tx2",  // New message
                    Message = "New", 
                    FromAddress = "addr2",
                    BlockDate = DateTime.Parse("2024-01-02", CultureInfo.InvariantCulture)
                }
            };

            // Act
            var merged = MessageNormalizer.Merge(existing, newMessages);

            // Assert
            // Should have 2 messages (tx1 and tx2)
            if (merged.Count != 2)
                throw new Exception($"Expected 2 messages, got {merged.Count}");
            
            // Should have one tx1 and one tx2
            if (merged.Count(m => m.TransactionId == "tx1") != 1)
                throw new Exception("tx1 should appear exactly once");
            if (merged.Count(m => m.TransactionId == "tx2") != 1)
                throw new Exception("tx2 should appear exactly once");
        }

        /// <summary>
        /// Test: Paginate returns correct subset
        /// </summary>
        public void Test_Paginate_ReturnsCorrectSubset()
        {
            // Arrange
            var messages = new List<MessageNormalizer.NormalizedMessage>();
            for (int i = 0; i < 50; i++)
            {
                messages.Add(new MessageNormalizer.NormalizedMessage
                {
                    TransactionId = $"tx{i:D3}",
                    Message = $"Message {i}",
                    FromAddress = "addr",
                    BlockDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture).AddSeconds(i)
                });
            }

            // Act - Get second page (skip 10, take 10)
            var page = MessageNormalizer.Paginate(messages, 10, 10);

            // Assert
            if (page.Count != 10)
                throw new Exception($"Expected 10 messages, got {page.Count}");
            
            // First message should be at index 10 from original list
            if (page[0].TransactionId != messages[10].TransactionId)
                throw new Exception($"Expected {messages[10].TransactionId}, got {page[0].TransactionId}");
        }

        /// <summary>
        /// Test: Null and empty TransactionIds are filtered out
        /// </summary>
        public void Test_Normalize_FiltersInvalidMessages()
        {
            // Arrange
            var messages = new List<MessageObject>
            {
                new MessageObject 
                { 
                    TransactionId = "tx1", 
                    Message = "Valid", 
                    FromAddress = "addr1",
                    BlockDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture)
                },
                new MessageObject 
                { 
                    TransactionId = null,  // Invalid
                    Message = "Invalid", 
                    FromAddress = "addr2",
                    BlockDate = DateTime.Parse("2024-01-02", CultureInfo.InvariantCulture)
                },
                new MessageObject 
                { 
                    TransactionId = "",  // Invalid
                    Message = "Also Invalid", 
                    FromAddress = "addr3",
                    BlockDate = DateTime.Parse("2024-01-03", CultureInfo.InvariantCulture)
                },
                new MessageObject 
                { 
                    TransactionId = "tx2", 
                    Message = "Also Valid", 
                    FromAddress = "addr4",
                    BlockDate = DateTime.Parse("2024-01-04", CultureInfo.InvariantCulture)
                }
            };

            // Act
            var normalized = MessageNormalizer.Normalize(messages);

            // Assert
            // Should only have 2 valid messages
            if (normalized.Count != 2)
                throw new Exception($"Expected 2 messages, got {normalized.Count}");
            
            // Should have tx1 and tx2
            if (!normalized.Any(m => m.TransactionId == "tx1"))
                throw new Exception("Missing tx1");
            if (!normalized.Any(m => m.TransactionId == "tx2"))
                throw new Exception("Missing tx2");
        }

        /// <summary>
        /// Test: ToMessageObjects converts back correctly
        /// </summary>
        public void Test_ToMessageObjects_ConvertsBack()
        {
            // Arrange
            var normalized = new List<MessageNormalizer.NormalizedMessage>
            {
                new MessageNormalizer.NormalizedMessage
                {
                    TransactionId = "tx1",
                    Message = "Test",
                    FromAddress = "addr1",
                    ToAddress = "addr2",
                    BlockDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture)
                }
            };

            // Act
            var messageObjects = MessageNormalizer.ToMessageObjects(normalized);

            // Assert
            if (messageObjects.Count != 1)
                throw new Exception($"Expected 1 message, got {messageObjects.Count}");
            
            var msg = messageObjects[0];
            if (msg.TransactionId != "tx1")
                throw new Exception($"Expected tx1, got {msg.TransactionId}");
            if (msg.Message != "Test")
                throw new Exception($"Expected 'Test', got {msg.Message}");
            if (msg.FromAddress != "addr1")
                throw new Exception($"Expected addr1, got {msg.FromAddress}");
            if (msg.ToAddress != "addr2")
                throw new Exception($"Expected addr2, got {msg.ToAddress}");
        }
    }
}
