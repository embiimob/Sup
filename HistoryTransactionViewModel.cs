using System;
using System.Collections.Generic;
using System.Drawing;

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
        public string FromURN { get; set; } // Resolved profile URN or object URN
        public string ToAddress { get; set; }
        public string ToURN { get; set; } // Resolved profile URN or object URN
        public string ObjectAddress { get; set; }
        public string ObjectURN { get; set; } // For the middle object in transactions
        public string Message { get; set; }
        public DateTime BlockDate { get; set; }
        public string FromImageLocation { get; set; }
        public string ToImageLocation { get; set; }
        public string ObjectImageLocation { get; set; }
        public string ClassifierTag { get; set; } // Tag to show which owned/created object this belongs to (null for History-only)
        public Color ClassifierColor { get; set; } // Color for the classifier tag
        public bool IsRendered { get; set; }
        public bool IsFromObject { get; set; } // True if from is an object
        public bool IsToObject { get; set; } // True if to is an object
        public bool HasMiddleObject { get; set; } // True if there's a middle object (like in GIV)

        public void Dispose()
        {
            // Clean up any resources if needed
        }
    }
}
