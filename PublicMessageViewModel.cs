using System;
using System.Collections.Generic;

namespace SUP
{
    /// <summary>
    /// View model for public/community messages in the feed.
    /// Provides a stable, sortable representation of message data.
    /// Implements IDisposable to clean up resources when message scrolls out of view.
    /// </summary>
    public class PublicMessageViewModel : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Stable unique identifier for the message (transaction ID)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Address of the message sender
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Display name for the sender (URN or truncated address)
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Profile image location for the sender
        /// </summary>
        public string FromImageLocation { get; set; }

        /// <summary>
        /// Address of the message recipient
        /// </summary>
        public string ToAddress { get; set; }

        /// <summary>
        /// Display name for the recipient (URN or truncated address)
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// Profile image location for the recipient
        /// </summary>
        public string ToImageLocation { get; set; }

        /// <summary>
        /// Message text content
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Raw message with attachment tags (e.g., &lt;&lt;hash/file.jpg&gt;&gt;)
        /// </summary>
        public string RawMessage { get; set; }

        /// <summary>
        /// Timestamp when the message was sent (from blockchain)
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Attachment information extracted from message tags
        /// </summary>
        public List<MessageAttachment> Attachments { get; set; }

        /// <summary>
        /// Tracks whether this message is currently rendered in the UI
        /// </summary>
        public bool IsRendered { get; set; }

        /// <summary>
        /// For community feed: source profile/keyword this message came from
        /// </summary>
        public string SourceProfile { get; set; }

        public PublicMessageViewModel()
        {
            Attachments = new List<MessageAttachment>();
            IsRendered = false;
        }

        /// <summary>
        /// Disposes resources associated with this message.
        /// Called when the message scrolls out of view and is no longer needed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose managed resources
                // Note: We don't dispose attachments themselves as they're just metadata
                // The actual UI controls will be disposed by the form
                Attachments?.Clear();
            }

            _disposed = true;
        }
    }
}
