using System;
using System.Collections.Generic;
using System.Linq;

namespace SUP
{
    /// <summary>
    /// Manages a sliding window cache of messages to support efficient virtualized scrolling.
    /// Maintains a bounded set of messages (typically 20) in memory while allowing access
    /// to a much larger dataset via paging.
    /// </summary>
    /// <typeparam name="T">The message view model type</typeparam>
    public class MessageCache<T> where T : class
    {
        private readonly int _windowSize;
        private readonly List<T> _allMessages;
        private int _currentWindowStart;
        private int _currentWindowEnd;

        /// <summary>
        /// Creates a new message cache with the specified window size.
        /// </summary>
        /// <param name="windowSize">Maximum number of messages to keep in the visible window (default 20)</param>
        public MessageCache(int windowSize = 20)
        {
            _windowSize = windowSize;
            _allMessages = new List<T>();
            _currentWindowStart = 0;
            _currentWindowEnd = 0;
        }

        /// <summary>
        /// Total number of messages in the cache
        /// </summary>
        public int TotalCount => _allMessages.Count;

        /// <summary>
        /// Current start index of the visible window
        /// </summary>
        public int WindowStart => _currentWindowStart;

        /// <summary>
        /// Current end index of the visible window
        /// </summary>
        public int WindowEnd => _currentWindowEnd;

        /// <summary>
        /// Number of messages currently in the visible window
        /// </summary>
        public int WindowCount => _currentWindowEnd - _currentWindowStart;

        /// <summary>
        /// Adds a message to the end of the cache
        /// </summary>
        public void Add(T message)
        {
            _allMessages.Add(message);
            
            // Expand window if we haven't reached max size yet
            if (WindowCount < _windowSize)
            {
                _currentWindowEnd = _allMessages.Count;
            }
        }

        /// <summary>
        /// Adds multiple messages to the end of the cache
        /// </summary>
        public void AddRange(IEnumerable<T> messages)
        {
            foreach (var msg in messages)
            {
                Add(msg);
            }
        }

        /// <summary>
        /// Clears all messages from the cache
        /// </summary>
        public void Clear()
        {
            // Dispose any IDisposable messages before clearing
            foreach (var message in _allMessages)
            {
                if (message is IDisposable disposable)
                {
                    try { disposable.Dispose(); }
                    catch { /* Ignore disposal errors */ }
                }
            }
            
            _allMessages.Clear();
            _currentWindowStart = 0;
            _currentWindowEnd = 0;
        }

        /// <summary>
        /// Gets the messages currently in the visible window
        /// </summary>
        public List<T> GetVisibleMessages()
        {
            if (_currentWindowStart >= _allMessages.Count)
                return new List<T>();
                
            int actualEnd = Math.Min(_currentWindowEnd, _allMessages.Count);
            return _allMessages.Skip(_currentWindowStart).Take(actualEnd - _currentWindowStart).ToList();
        }

        /// <summary>
        /// Scrolls the window forward (toward newer messages) by the specified count
        /// </summary>
        /// <returns>The messages that just entered the window, or empty list if at end</returns>
        public List<T> ScrollForward(int count = 1)
        {
            if (_currentWindowEnd >= _allMessages.Count)
                return new List<T>(); // Already at end

            int oldEnd = _currentWindowEnd;
            _currentWindowEnd = Math.Min(_currentWindowEnd + count, _allMessages.Count);
            _currentWindowStart = Math.Max(0, _currentWindowEnd - _windowSize);

            // Return newly visible messages
            if (_currentWindowEnd > oldEnd)
            {
                return _allMessages.Skip(oldEnd).Take(_currentWindowEnd - oldEnd).ToList();
            }

            return new List<T>();
        }

        /// <summary>
        /// Scrolls the window backward (toward older messages) by the specified count
        /// </summary>
        /// <returns>The messages that just entered the window, or empty list if at start</returns>
        public List<T> ScrollBackward(int count = 1)
        {
            if (_currentWindowStart <= 0)
                return new List<T>(); // Already at start

            int oldStart = _currentWindowStart;
            _currentWindowStart = Math.Max(0, _currentWindowStart - count);
            _currentWindowEnd = Math.Min(_allMessages.Count, _currentWindowStart + _windowSize);

            // Return newly visible messages
            if (_currentWindowStart < oldStart)
            {
                return _allMessages.Skip(_currentWindowStart).Take(oldStart - _currentWindowStart).ToList();
            }

            return new List<T>();
        }

        /// <summary>
        /// Gets messages that should be removed from UI (no longer in visible window)
        /// </summary>
        /// <param name="previousStart">Previous window start index</param>
        /// <param name="previousEnd">Previous window end index</param>
        /// <returns>Messages that were visible before but aren't now</returns>
        public List<T> GetMessagesToRemove(int previousStart, int previousEnd)
        {
            var toRemove = new List<T>();

            // Messages before current window
            if (previousStart < _currentWindowStart)
            {
                int removeEnd = Math.Min(previousEnd, _currentWindowStart);
                toRemove.AddRange(_allMessages.Skip(previousStart).Take(removeEnd - previousStart));
            }

            // Messages after current window
            if (previousEnd > _currentWindowEnd)
            {
                int removeStart = Math.Max(previousStart, _currentWindowEnd);
                toRemove.AddRange(_allMessages.Skip(removeStart).Take(previousEnd - removeStart));
            }

            return toRemove;
        }

        /// <summary>
        /// Checks if a message is currently in the visible window
        /// </summary>
        public bool IsInVisibleWindow(T message)
        {
            int index = _allMessages.IndexOf(message);
            return index >= _currentWindowStart && index < _currentWindowEnd;
        }

        /// <summary>
        /// Gets the index of a message in the cache
        /// </summary>
        public int IndexOf(T message)
        {
            return _allMessages.IndexOf(message);
        }
    }
}
