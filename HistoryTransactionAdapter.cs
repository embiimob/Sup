using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SUP
{
    /// <summary>
    /// Adapter for displaying transaction history in a VirtualizedMessageList.
    /// Shows transactions in chronological order with visual classifier tags for owned/created objects.
    /// Matches the original CreateFeedRow functionality.
    /// </summary>
    public class HistoryTransactionAdapter : IMessageListAdapter
    {
        private List<HistoryTransactionViewModel> _transactions;
        private Dictionary<string, string> _friendList;
        private ObjectBrowser _parentForm; // Need reference to parent for click handlers

        public HistoryTransactionAdapter(ObjectBrowser parentForm = null)
        {
            _transactions = new List<HistoryTransactionViewModel>();
            _friendList = new Dictionary<string, string>();
            _parentForm = parentForm;
        }

        /// <summary>
        /// Sets the transaction list (already sorted chronologically)
        /// </summary>
        public void SetTransactions(List<HistoryTransactionViewModel> transactions)
        {
            _transactions = transactions ?? new List<HistoryTransactionViewModel>();
        }

        /// <summary>
        /// Adds transactions to the end of the list
        /// </summary>
        public void AddTransactions(List<HistoryTransactionViewModel> transactions)
        {
            if (transactions != null && transactions.Count > 0)
            {
                _transactions.AddRange(transactions);
            }
        }

        /// <summary>
        /// Updates the friend list for profile images
        /// </summary>
        public void SetFriendList(Dictionary<string, string> friendList)
        {
            _friendList = friendList ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets a transaction by position
        /// </summary>
        public HistoryTransactionViewModel GetTransaction(int position)
        {
            if (position >= 0 && position < _transactions.Count)
                return _transactions[position];
            return null;
        }

        public int GetItemCount()
        {
            return _transactions.Count;
        }

        public Control GetView(int position, Control convertView)
        {
            var transaction = GetTransaction(position);
            if (transaction == null) return null;

            // Create a TableLayoutPanel similar to the original CreateFeedRow
            TableLayoutPanel row = convertView as TableLayoutPanel ?? new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 5,
                AutoSize = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0, 2, 0, 2),
                Tag = transaction.TransactionId
            };

            // Clear existing controls if reusing
            if (convertView != null)
            {
                row.Controls.Clear();
                row.ColumnStyles.Clear();
            }

            // Set column styles (5 columns for: From Image, From Info, Message, Object/Middle, To Info)
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));  // From image
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));  // From label
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // Message
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));  // Middle object
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));  // To label

            // Add classifier tag if in filtered mode
            if (!string.IsNullOrEmpty(transaction.ClassifierTag))
            {
                Label classifierLabel = new Label
                {
                    Text = "▌ " + transaction.ClassifierTag,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                    ForeColor = transaction.ClassifierColor,
                    BackColor = Color.Black,
                    Margin = new Padding(0),
                    Padding = new Padding(3, 0, 0, 0)
                };
                row.Controls.Add(classifierLabel);
                row.SetColumnSpan(classifierLabel, 5);
            }

            // Column 0: From Image (profile or object)
            PictureBox fromPicture = new PictureBox
            {
                Size = new Size(80, 80),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(0)
            };

            if (!string.IsNullOrEmpty(transaction.FromImageLocation) && 
                (File.Exists(transaction.FromImageLocation) || transaction.FromImageLocation.ToUpper().StartsWith("HTTP")))
            {
                fromPicture.ImageLocation = transaction.FromImageLocation;
            }
            else
            {
                fromPicture.ImageLocation = "includes\\anon.png";
            }
            row.Controls.Add(fromPicture, 0, 0);

            // Column 1: From Label (URN or address) or From Object Image
            if (transaction.IsFromObject && !string.IsNullOrEmpty(transaction.FromImageLocation))
            {
                PictureBox fromObjectPicture = new PictureBox
                {
                    Size = new Size(80, 80),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageLocation = transaction.FromImageLocation,
                    Margin = new Padding(0),
                    Cursor = Cursors.Hand
                };
                if (_parentForm != null)
                {
                    fromObjectPicture.MouseClick += (sender, e) => { _parentForm.object_LinkClicked(transaction.FromAddress); };
                }
                row.Controls.Add(fromObjectPicture, 1, 0);
            }
            else
            {
                LinkLabel fromLabel = new LinkLabel
                {
                    Text = !string.IsNullOrEmpty(transaction.FromURN) ? transaction.FromURN : TruncateAddress(transaction.FromAddress),
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true,
                    Dock = DockStyle.Bottom,
                    Font = new Font("Microsoft Sans Serif", 7.7F, FontStyle.Regular),
                    Margin = new Padding(3)
                };
                if (_parentForm != null)
                {
                    fromLabel.LinkClicked += (sender, e) => { _parentForm.Owner_LinkClicked(transaction.FromAddress); };
                }
                row.Controls.Add(fromLabel, 1, 0);
            }

            // Column 2: Message/Transaction Type
            Label messageLabel = new Label
            {
                AutoSize = true,
                Text = transaction.Message,
                Font = new Font("Segoe UI", 7.77F, FontStyle.Regular),
                Margin = new Padding(0),
                Padding = new Padding(0),
                TextAlign = ContentAlignment.BottomLeft,
                MinimumSize = new Size(100, 46)
            };
            row.Controls.Add(messageLabel, 2, 0);

            // Column 3: Middle Object (for GIV, BUY, etc.)
            if (transaction.HasMiddleObject && !string.IsNullOrEmpty(transaction.ObjectImageLocation))
            {
                PictureBox objectPicture = new PictureBox
                {
                    Size = new Size(80, 80),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageLocation = transaction.ObjectImageLocation,
                    Margin = new Padding(0),
                    Cursor = Cursors.Hand
                };
                if (_parentForm != null)
                {
                    objectPicture.MouseClick += (sender, e) => { _parentForm.object_LinkClicked(transaction.ObjectAddress); };
                }
                row.Controls.Add(objectPicture, 3, 0);
            }

            // Column 4: To Label or To Object Image
            if (!string.IsNullOrEmpty(transaction.ToAddress))
            {
                if (transaction.IsToObject && !string.IsNullOrEmpty(transaction.ToImageLocation))
                {
                    PictureBox toPicture = new PictureBox
                    {
                        Size = new Size(80, 80),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        ImageLocation = transaction.ToImageLocation,
                        Margin = new Padding(0),
                        Cursor = Cursors.Hand
                    };
                    if (_parentForm != null)
                    {
                        toPicture.MouseClick += (sender, e) => { _parentForm.object_LinkClicked(transaction.ToAddress); };
                    }
                    row.Controls.Add(toPicture, 4, 0);
                }
                else
                {
                    LinkLabel toLabel = new LinkLabel
                    {
                        Text = !string.IsNullOrEmpty(transaction.ToURN) ? transaction.ToURN : TruncateAddress(transaction.ToAddress),
                        BackColor = Color.Black,
                        ForeColor = Color.White,
                        AutoSize = true,
                        Dock = DockStyle.Bottom,
                        Font = new Font("Microsoft Sans Serif", 7.7F, FontStyle.Regular),
                        Margin = new Padding(3)
                    };
                    if (_parentForm != null)
                    {
                        toLabel.LinkClicked += (sender, e) => { _parentForm.Owner_LinkClicked(transaction.ToAddress); };
                    }
                    row.Controls.Add(toLabel, 4, 0);
                }
            }

            transaction.IsRendered = true;
            return row;
        }

        private string TruncateAddress(string address)
        {
            if (string.IsNullOrEmpty(address)) return "";
            if (address.Length <= 12) return address;
            return address.Substring(0, 6) + "..." + address.Substring(address.Length - 6);
        }

        public void OnItemRecycled(int position, Control view)
        {
            var transaction = GetTransaction(position);
            if (transaction != null)
            {
                transaction.IsRendered = false;
            }

            // Clean up any loaded resources
            if (view != null)
            {
                foreach (Control control in view.Controls)
                {
                    if (control is PictureBox pb)
                    {
                        // Release image resources
                        pb.Image?.Dispose();
                        pb.Image = null;
                    }
                }
            }

            Debug.WriteLine($"[HistoryTransactionAdapter] Recycled transaction at position {position}");
        }
    }
}
