using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;

namespace SUP
{
    /// <summary>
    /// Adapter for displaying transaction history in a VirtualizedMessageList.
    /// Shows transactions in chronological order with visual classifier tags for owned/created objects.
    /// </summary>
    public class HistoryTransactionAdapter : IMessageListAdapter
    {
        private List<HistoryTransactionViewModel> _transactions;
        private Dictionary<string, string> _friendList;

        public HistoryTransactionAdapter()
        {
            _transactions = new List<HistoryTransactionViewModel>();
            _friendList = new Dictionary<string, string>();
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

            // Create a transaction row view
            return CreateTransactionView(transaction, convertView);
        }

        private Control CreateTransactionView(HistoryTransactionViewModel transaction, Control convertView)
        {
            Panel transactionPanel = convertView as Panel ?? new Panel
            {
                Height = 90, // Increased height to accommodate classifier tag
                BackColor = Color.Black,
                Padding = new Padding(5),
                Margin = new Padding(0, 2, 0, 2)
            };

            // Clear existing controls if reusing
            if (convertView != null)
            {
                transactionPanel.Controls.Clear();
            }

            // Classifier tag (if in filtered mode)
            int leftOffset = 5;
            if (!string.IsNullOrEmpty(transaction.ClassifierTag))
            {
                Panel classifierPanel = new Panel
                {
                    Size = new Size(8, 80), // Vertical colored bar
                    Location = new Point(5, 5),
                    BackColor = transaction.ClassifierColor
                };
                transactionPanel.Controls.Add(classifierPanel);

                Label classifierLabel = new Label
                {
                    Text = transaction.ClassifierTag,
                    Location = new Point(18, 5),
                    AutoSize = true,
                    MaximumSize = new Size(150, 20),
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    ForeColor = transaction.ClassifierColor,
                    BackColor = Color.Transparent
                };
                transactionPanel.Controls.Add(classifierLabel);
                leftOffset = 18;
            }

            // Profile picture
            PictureBox profilePic = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(leftOffset, 20),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            // Set image location
            string imageLocation = transaction.ImageLocation;
            if (!string.IsNullOrEmpty(imageLocation))
            {
                try
                {
                    // Try friend list override
                    if (_friendList.ContainsKey(transaction.FromAddress))
                    {
                        imageLocation = _friendList[transaction.FromAddress];
                    }
                    profilePic.ImageLocation = imageLocation;
                }
                catch
                {
                    profilePic.ImageLocation = "includes\\anon.png";
                }
            }
            else
            {
                profilePic.ImageLocation = "includes\\anon.png";
            }

            transactionPanel.Controls.Add(profilePic);

            int textLeftOffset = leftOffset + 70;

            // From address (clickable)
            LinkLabel fromLabel = new LinkLabel
            {
                Text = TruncateAddress(transaction.FromAddress),
                Location = new Point(textLeftOffset, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                LinkColor = Color.LightBlue,
                ActiveLinkColor = Color.White,
                VisitedLinkColor = Color.LightBlue,
                BackColor = Color.Transparent
            };
            fromLabel.Links.Add(0, fromLabel.Text.Length, transaction.FromAddress);
            transactionPanel.Controls.Add(fromLabel);

            // Message label
            Label messageLabel = new Label
            {
                Text = transaction.Message,
                Location = new Point(textLeftOffset, 40),
                AutoSize = true,
                MaximumSize = new Size(transactionPanel.Width - textLeftOffset - 200, 30),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            transactionPanel.Controls.Add(messageLabel);

            // To address (if present)
            if (!string.IsNullOrEmpty(transaction.ToAddress))
            {
                Label toLabel = new Label
                {
                    Text = "→ " + TruncateAddress(transaction.ToAddress),
                    Location = new Point(textLeftOffset, 60),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    BackColor = Color.Transparent
                };
                transactionPanel.Controls.Add(toLabel);
            }

            // Block date label
            Label dateLabel = new Label
            {
                Text = transaction.BlockDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                Location = new Point(transactionPanel.Width - 180, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            transactionPanel.Controls.Add(dateLabel);

            // Transaction ID label (clickable)
            LinkLabel txIdLabel = new LinkLabel
            {
                Text = TruncateAddress(transaction.TransactionId),
                Location = new Point(transactionPanel.Width - 180, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                LinkColor = Color.DarkGray,
                ActiveLinkColor = Color.White,
                VisitedLinkColor = Color.DarkGray,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            txIdLabel.Links.Add(0, txIdLabel.Text.Length, transaction.TransactionId);
            transactionPanel.Controls.Add(txIdLabel);

            // Separator line
            Panel separator = new Panel
            {
                Height = 1,
                BackColor = Color.FromArgb(40, 40, 40),
                Dock = DockStyle.Bottom
            };
            transactionPanel.Controls.Add(separator);

            transaction.IsRendered = true;
            return transactionPanel;
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
