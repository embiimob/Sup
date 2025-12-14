using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace SUP
{
    /// <summary>
    /// Adapter for displaying public/community messages in a VirtualizedMessageList.
    /// Manages a list of PublicMessageViewModel objects and creates UI controls for them.
    /// </summary>
    public class PublicMessageAdapter : IMessageListAdapter
    {
        private List<PublicMessageViewModel> _messages;
        private SupMain _parentForm;
        private string _mainnetLogin;
        private string _mainnetPassword;
        private string _mainnetURL;
        private string _mainnetVersionByte;
        private bool _testnet;

        public PublicMessageAdapter(
            SupMain parentForm,
            string mainnetLogin,
            string mainnetPassword,
            string mainnetURL,
            string mainnetVersionByte,
            bool testnet)
        {
            _messages = new List<PublicMessageViewModel>();
            _parentForm = parentForm;
            _mainnetLogin = mainnetLogin;
            _mainnetPassword = mainnetPassword;
            _mainnetURL = mainnetURL;
            _mainnetVersionByte = mainnetVersionByte;
            _testnet = testnet;
        }

        /// <summary>
        /// Updates the message list
        /// </summary>
        public void SetMessages(List<PublicMessageViewModel> messages)
        {
            _messages = messages ?? new List<PublicMessageViewModel>();
        }

        /// <summary>
        /// Adds messages to the end of the list
        /// </summary>
        public void AddMessages(List<PublicMessageViewModel> messages)
        {
            if (messages != null && messages.Count > 0)
            {
                _messages.AddRange(messages);
            }
        }

        /// <summary>
        /// Gets a message by position
        /// </summary>
        public PublicMessageViewModel GetMessage(int position)
        {
            if (position >= 0 && position < _messages.Count)
                return _messages[position];
            return null;
        }

        public int GetItemCount()
        {
            return _messages.Count;
        }

        public Control GetView(int position, Control convertView)
        {
            var message = GetMessage(position);
            if (message == null) return null;

            // Create a container panel for this message
            Panel messagePanel = convertView as Panel ?? new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Black,
                Padding = new Padding(5),
                Margin = new Padding(0, 5, 0, 5)
            };

            // Clear existing controls if reusing
            if (convertView != null)
            {
                messagePanel.Controls.Clear();
            }

            // Create FROM header (sender)
            Panel fromHeader = CreateMessageHeader(
                message.FromName,
                message.FromImageLocation,
                message.FromAddress,
                message.Timestamp,
                true);
            messagePanel.Controls.Add(fromHeader);

            // Create message body (text content)
            if (!string.IsNullOrWhiteSpace(message.MessageText))
            {
                Label messageLabel = new Label
                {
                    Text = message.MessageText,
                    AutoSize = true,
                    MaximumSize = new Size(messagePanel.Width - 60, 0),
                    Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    Padding = new Padding(50, 10, 10, 10)
                };
                messagePanel.Controls.Add(messageLabel);
            }

            // Create TO header (recipient)
            Panel toHeader = CreateMessageHeader(
                message.ToName,
                message.ToImageLocation,
                message.ToAddress,
                DateTime.MinValue, // Don't show timestamp for recipient
                false);
            messagePanel.Controls.Add(toHeader);

            // Create attachment placeholders (lazy loaded)
            foreach (var attachment in message.Attachments)
            {
                Control attachmentControl = CreateAttachmentPlaceholder(attachment, message);
                if (attachmentControl != null)
                {
                    messagePanel.Controls.Add(attachmentControl);
                }
            }

            // Add separator
            Panel separator = new Panel
            {
                Height = 2,
                BackColor = Color.FromArgb(40, 40, 40),
                Dock = DockStyle.Bottom,
                Margin = new Padding(0, 10, 0, 0)
            };
            messagePanel.Controls.Add(separator);

            // Mark as rendered
            message.IsRendered = true;

            return messagePanel;
        }

        private Panel CreateMessageHeader(string name, string imageLocation, string address, DateTime timestamp, bool showTimestamp)
        {
            Panel headerPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.Black
            };

            // Profile picture
            PictureBox profilePic = new PictureBox
            {
                Size = new Size(50, 50),
                Location = new Point(5, 5),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            // Set image location (lazy load)
            if (!string.IsNullOrEmpty(imageLocation))
            {
                try
                {
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

            headerPanel.Controls.Add(profilePic);

            // Name label (clickable)
            LinkLabel nameLabel = new LinkLabel
            {
                Text = name,
                Location = new Point(60, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                LinkColor = Color.White,
                ActiveLinkColor = Color.LightBlue,
                VisitedLinkColor = Color.White,
                BackColor = Color.Transparent
            };
            nameLabel.Links.Add(0, name.Length, address);
            headerPanel.Controls.Add(nameLabel);

            // Timestamp label (only for sender)
            if (showTimestamp && timestamp.Year > 1975)
            {
                Label timestampLabel = new Label
                {
                    Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    Location = new Point(60, 35),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    BackColor = Color.Transparent
                };
                headerPanel.Controls.Add(timestampLabel);
            }

            return headerPanel;
        }

        private Control CreateAttachmentPlaceholder(MessageAttachment attachment, PublicMessageViewModel message)
        {
            Panel attachmentPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(20, 20, 20),
                Padding = new Padding(50, 5, 5, 5),
                Cursor = Cursors.Hand
            };

            string attachmentText = $"📎 {GetAttachmentTypeIcon(attachment.Type)} {attachment.Content}";
            if (attachmentText.Length > 80)
            {
                attachmentText = attachmentText.Substring(0, 77) + "...";
            }

            Label attachmentLabel = new Label
            {
                Text = attachmentText,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.LightBlue,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            attachmentPanel.Controls.Add(attachmentLabel);

            // Click handler to load attachment
            attachmentPanel.Click += (s, e) => LoadAttachment(attachment, message, attachmentPanel);
            attachmentLabel.Click += (s, e) => LoadAttachment(attachment, message, attachmentPanel);

            return attachmentPanel;
        }

        private string GetAttachmentTypeIcon(AttachmentType type)
        {
            switch (type)
            {
                case AttachmentType.Image: return "🖼️";
                case AttachmentType.Video: return "🎥";
                case AttachmentType.Audio: return "🎵";
                case AttachmentType.Link: return "🔗";
                default: return "📄";
            }
        }

        private void LoadAttachment(MessageAttachment attachment, PublicMessageViewModel message, Panel attachmentPanel)
        {
            Debug.WriteLine($"[PublicMessageAdapter] Loading attachment: {attachment.Type} - {attachment.Content}");
            
            // Update UI to show loading
            if (attachmentPanel.Controls.Count > 0 && attachmentPanel.Controls[0] is Label label)
            {
                label.Text = $"⏳ Loading {attachment.Type}...";
            }

            // TODO: Implement actual lazy loading based on attachment type
            // For now, just log
            Debug.WriteLine($"[PublicMessageAdapter] Attachment load requested but not yet implemented");
        }

        public void OnItemRecycled(int position, Control view)
        {
            var message = GetMessage(position);
            if (message != null)
            {
                message.IsRendered = false;
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

            Debug.WriteLine($"[PublicMessageAdapter] Recycled message at position {position}");
        }
    }
}
