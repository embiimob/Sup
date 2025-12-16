using System;
using System.Collections.Generic;

namespace SUP
{
    /// <summary>
    /// View model for displaying transaction history entries.
    /// Represents a single transaction in the history view.
    /// </summary>
    public class HistoryTransactionViewModel : IDisposable
    {
        public string TransactionId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string ObjectAddress { get; set; }
        public string ObjectName { get; set; } // For display in classifier tag
        public string Message { get; set; }
        public DateTime BlockDate { get; set; }
        public string ImageLocation { get; set; }
        public string ClassifierTag { get; set; } // Tag to show which owned/created object this belongs to (null for History-only)
        public Color ClassifierColor { get; set; } // Color for the classifier tag
        public bool IsRendered { get; set; }

        public void Dispose()
        {
            // Clean up any resources if needed
        }
    }
}
