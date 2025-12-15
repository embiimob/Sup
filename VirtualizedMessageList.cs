using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SUP
{
    /// <summary>
    /// A virtualized message list control that only renders visible messages.
    /// Implements true UI virtualization to support thousands of messages while maintaining
    /// bounded memory usage (max ~20 messages rendered at any time).
    /// </summary>
    public partial class VirtualizedMessageList : UserControl
    {
        private VScrollBar _vScrollBar;
        private Panel _viewport;
        private int _itemHeight = 100; // Estimated average message height (can be dynamically adjusted)
        private int _totalItemCount = 0;
        private int _firstVisibleIndex = 0;
        private int _visibleItemCount = 0;
        private Dictionary<int, Control> _renderedControls = new Dictionary<int, Control>();
        private IMessageListAdapter _adapter;

        /// <summary>
        /// Maximum number of messages to keep rendered at once
        /// </summary>
        public const int MaxRenderedMessages = 20;
        
        /// <summary>
        /// Buffer items to render beyond visible area to reduce flicker during scrolling
        /// </summary>
        private const int BufferItemCount = 2;

        /// <summary>
        /// Sets the estimated average item height for scroll calculations.
        /// Can be dynamically updated based on actual rendered message heights.
        /// </summary>
        public void SetEstimatedItemHeight(int height)
        {
            if (height > 0)
            {
                _itemHeight = height;
                UpdateScrollBar();
            }
        }

        /// <summary>
        /// Automatically calculates average item height from currently rendered controls.
        /// Call this periodically to improve scroll accuracy with variable-height messages.
        /// </summary>
        public void RecalculateItemHeight()
        {
            if (_renderedControls.Count == 0) return;

            int totalHeight = 0;
            foreach (var control in _renderedControls.Values)
            {
                totalHeight += control.Height;
            }

            int avgHeight = totalHeight / _renderedControls.Count;
            if (avgHeight > 0)
            {
                _itemHeight = avgHeight;
                Debug.WriteLine($"[VirtualizedMessageList] Recalculated average item height: {_itemHeight}px");
            }
        }

        public VirtualizedMessageList()
        {
            InitializeComponent();
            SetupControls();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = Color.Black;
            this.Name = "VirtualizedMessageList";
            this.Size = new System.Drawing.Size(600, 400);
            this.ResumeLayout(false);
        }

        private void SetupControls()
        {
            // Create viewport panel (displays visible messages)
            _viewport = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                BackColor = Color.Black
            };
            _viewport.Paint += Viewport_Paint;

            // Create vertical scrollbar
            _vScrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                Minimum = 0,
                Maximum = 0,
                LargeChange = 1,
                SmallChange = 1
            };
            _vScrollBar.Scroll += VScrollBar_Scroll;

            // Add controls
            this.Controls.Add(_viewport);
            this.Controls.Add(_vScrollBar);

            // Handle resize
            this.Resize += VirtualizedMessageList_Resize;
        }

        /// <summary>
        /// Sets the adapter that provides message data and rendering
        /// </summary>
        public void SetAdapter(IMessageListAdapter adapter)
        {
            _adapter = adapter;
            _totalItemCount = adapter?.GetItemCount() ?? 0;
            UpdateScrollBar();
            InvalidateVisibleItems();
        }

        /// <summary>
        /// Notifies the list that data has changed and a refresh is needed
        /// </summary>
        public void NotifyDataSetChanged()
        {
            if (_adapter == null) return;

            _totalItemCount = _adapter.GetItemCount();
            UpdateScrollBar();
            InvalidateVisibleItems();
        }

        /// <summary>
        /// Scrolls to show a specific message index
        /// </summary>
        public void ScrollToItem(int index)
        {
            if (index < 0 || index >= _totalItemCount) return;

            _firstVisibleIndex = index;
            _vScrollBar.Value = Math.Min(index, _vScrollBar.Maximum);
            InvalidateVisibleItems();
        }

        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _firstVisibleIndex = e.NewValue;
            InvalidateVisibleItems();
        }

        private void VirtualizedMessageList_Resize(object sender, EventArgs e)
        {
            UpdateScrollBar();
            InvalidateVisibleItems();
        }

        private void UpdateScrollBar()
        {
            if (_totalItemCount == 0)
            {
                _vScrollBar.Enabled = false;
                _vScrollBar.Maximum = 0;
                return;
            }

            // Calculate how many items can fit in viewport
            _visibleItemCount = Math.Max(1, _viewport.Height / _itemHeight) + BufferItemCount;

            // Update scrollbar range
            int maxScroll = Math.Max(0, _totalItemCount - 1);
            _vScrollBar.Maximum = maxScroll + _visibleItemCount - 1; // Adjust for LargeChange
            _vScrollBar.LargeChange = _visibleItemCount;
            _vScrollBar.Enabled = _totalItemCount > _visibleItemCount;

            // Ensure first visible index is valid
            _firstVisibleIndex = Math.Min(_firstVisibleIndex, Math.Max(0, _totalItemCount - _visibleItemCount));
        }

        private void InvalidateVisibleItems()
        {
            if (_adapter == null) return;

            _viewport.SuspendLayout();

            try
            {
                // Calculate which items should be visible
                int lastVisibleIndex = Math.Min(_firstVisibleIndex + _visibleItemCount, _totalItemCount);
                HashSet<int> visibleIndices = new HashSet<int>();

                for (int i = _firstVisibleIndex; i < lastVisibleIndex; i++)
                {
                    visibleIndices.Add(i);
                }

                // Remove controls that are no longer visible
                var indicesToRemove = _renderedControls.Keys.Where(k => !visibleIndices.Contains(k)).ToList();
                foreach (int index in indicesToRemove)
                {
                    Control control = _renderedControls[index];
                    _viewport.Controls.Remove(control);
                    _renderedControls.Remove(index);

                    // Dispose control and notify adapter
                    try
                    {
                        _adapter.OnItemRecycled(index, control);
                        control.Dispose();
                    }
                    catch { }
                }

                // Add/update visible controls
                int yOffset = 0;
                for (int i = _firstVisibleIndex; i < lastVisibleIndex; i++)
                {
                    Control itemControl;

                    if (_renderedControls.ContainsKey(i))
                    {
                        // Reuse existing control
                        itemControl = _renderedControls[i];
                    }
                    else
                    {
                        // Create new control
                        itemControl = _adapter.GetView(i, null);
                        if (itemControl != null)
                        {
                            _renderedControls[i] = itemControl;
                            _viewport.Controls.Add(itemControl);
                        }
                        else
                        {
                            continue; // Skip if adapter returns null
                        }
                    }

                    // Position the control
                    itemControl.Location = new Point(0, yOffset);
                    itemControl.Width = _viewport.Width - (_vScrollBar.Visible ? _vScrollBar.Width : 0);

                    yOffset += itemControl.Height;
                }

                // Update memory diagnostics
                MemoryDiagnostics.LogMemoryUsage($"Rendered {_renderedControls.Count} message controls");
            }
            finally
            {
                _viewport.ResumeLayout();
            }
        }

        private void Viewport_Paint(object sender, PaintEventArgs e)
        {
            // Optional: Draw background or placeholder
        }

        /// <summary>
        /// Cleans up all rendered controls
        /// </summary>
        public void Clear()
        {
            _viewport.SuspendLayout();

            foreach (var kvp in _renderedControls)
            {
                try
                {
                    _adapter?.OnItemRecycled(kvp.Key, kvp.Value);
                    kvp.Value.Dispose();
                }
                catch { }
            }

            _renderedControls.Clear();
            _viewport.Controls.Clear();
            _totalItemCount = 0;
            _firstVisibleIndex = 0;
            UpdateScrollBar();

            _viewport.ResumeLayout();

            MemoryDiagnostics.LogMemoryUsage("Cleared virtualized message list");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
                _viewport?.Dispose();
                _vScrollBar?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Interface for providing data and views to the VirtualizedMessageList
    /// </summary>
    public interface IMessageListAdapter
    {
        /// <summary>
        /// Returns the total number of items in the dataset
        /// </summary>
        int GetItemCount();

        /// <summary>
        /// Creates or updates a view for the specified position.
        /// If convertView is not null, it should be reused.
        /// </summary>
        /// <param name="position">Position in the dataset</param>
        /// <param name="convertView">Optional existing view to reuse</param>
        /// <returns>View representing the item at this position</returns>
        Control GetView(int position, Control convertView);

        /// <summary>
        /// Called when an item view is being recycled (removed from display).
        /// Use this to clean up resources like loaded images.
        /// </summary>
        /// <param name="position">Position that was recycled</param>
        /// <param name="view">View that was recycled</param>
        void OnItemRecycled(int position, Control view);
    }
}
