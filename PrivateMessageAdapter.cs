using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace SUP
{
    /// <summary>
    /// Adapter for displaying private messages in a VirtualizedMessageList.
    /// Manages a list of PrivateMessageViewModel objects and creates UI controls for them.
    /// </summary>
    public class PrivateMessageAdapter : IMessageListAdapter
    {
        private List<PrivateMessageViewModel> _messages;
        private SupMain _parentForm;
        private string _mainnetLogin;
        private string _mainnetPassword;
        private string _mainnetURL;
        private string _mainnetVersionByte;
        private bool _testnet;

        public PrivateMessageAdapter(
            SupMain parentForm,
            string mainnetLogin,
            string mainnetPassword,
            string mainnetURL,
            string mainnetVersionByte,
            bool testnet)
        {
            _messages = new List<PrivateMessageViewModel>();
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
        public void SetMessages(List<PrivateMessageViewModel> messages)
        {
            _messages = messages ?? new List<PrivateMessageViewModel>();
        }

        /// <summary>
        /// Adds messages to the end of the list
        /// </summary>
        public void AddMessages(List<PrivateMessageViewModel> messages)
        {
            if (messages != null && messages.Count > 0)
            {
                _messages.AddRange(messages);
            }
        }

        /// <summary>
        /// Gets a message by position
        /// </summary>
        public PrivateMessageViewModel GetMessage(int position)
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

            // Create message header (profile pic + name + timestamp)
            Panel headerPanel = CreateMessageHeader(message);
            messagePanel.Controls.Add(headerPanel);

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
                Dock = DockStyle.Bottom
            };
            messagePanel.Controls.Add(separator);

            // Mark as rendered
            message.IsRendered = true;

            return messagePanel;
        }

        private Panel CreateMessageHeader(PrivateMessageViewModel message)
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
            if (!string.IsNullOrEmpty(message.FromImageLocation))
            {
                try
                {
                    profilePic.ImageLocation = message.FromImageLocation;
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

            // Name label
            Label nameLabel = new Label
            {
                Text = message.FromName,
                Location = new Point(60, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(nameLabel);

            // Timestamp label
            Label timestampLabel = new Label
            {
                Text = message.Timestamp.ToString("MM/dd/yyyy hh:mm:ss tt"),
                Location = new Point(60, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(timestampLabel);

            return headerPanel;
        }

        private Control CreateAttachmentPlaceholder(MessageAttachment attachment, PrivateMessageViewModel message)
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
                case AttachmentType.SEC: return "🔒";
                case AttachmentType.Link: return "🔗";
                default: return "📄";
            }
        }

        private void LoadAttachment(MessageAttachment attachment, PrivateMessageViewModel message, Panel attachmentPanel)
        {
            Debug.WriteLine($"[PrivateMessageAdapter] Loading attachment: {attachment.Type} - {attachment.Content}");
            
            // Update UI to show loading
            if (attachmentPanel.Controls.Count > 0 && attachmentPanel.Controls[0] is Label label)
            {
                label.Text = $"⏳ Loading {attachment.Type}...";
            }

            // TODO: Implement actual lazy loading based on attachment type
            // For now, just log and notify parent form
            Debug.WriteLine($"[PrivateMessageAdapter] Attachment load requested but not yet implemented");
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

            Debug.WriteLine($"[PrivateMessageAdapter] Recycled message at position {position}");
        }
    }
}
