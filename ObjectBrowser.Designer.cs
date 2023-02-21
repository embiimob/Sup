using System.Timers;

namespace SUP
{
    partial class ObjectBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCreated = new System.Windows.Forms.Button();
            this.btnOwned = new System.Windows.Forms.Button();
            this.txtSearchAddress = new System.Windows.Forms.TextBox();
            this.btnWorkBench = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.txtVersionByte = new System.Windows.Forms.TextBox();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnHistoryBack = new System.Windows.Forms.Button();
            this.btnHistoryForward = new System.Windows.Forms.Button();
            this.btnMint = new System.Windows.Forms.Button();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.txtLast = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCreated
            // 
            this.btnCreated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreated.BackColor = System.Drawing.SystemColors.Control;
            this.btnCreated.Location = new System.Drawing.Point(429, 4);
            this.btnCreated.Name = "btnCreated";
            this.btnCreated.Size = new System.Drawing.Size(55, 20);
            this.btnCreated.TabIndex = 62;
            this.btnCreated.Text = "created";
            this.btnCreated.UseVisualStyleBackColor = false;
            this.btnCreated.Click += new System.EventHandler(this.ButtonGetCreatedClick);
            // 
            // btnOwned
            // 
            this.btnOwned.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOwned.BackColor = System.Drawing.SystemColors.Control;
            this.btnOwned.Location = new System.Drawing.Point(488, 4);
            this.btnOwned.Name = "btnOwned";
            this.btnOwned.Size = new System.Drawing.Size(49, 20);
            this.btnOwned.TabIndex = 61;
            this.btnOwned.Text = "owned";
            this.btnOwned.UseVisualStyleBackColor = false;
            this.btnOwned.Click += new System.EventHandler(this.ButtonGetOwnedClick);
            // 
            // txtSearchAddress
            // 
            this.txtSearchAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchAddress.Location = new System.Drawing.Point(42, 4);
            this.txtSearchAddress.Name = "txtSearchAddress";
            this.txtSearchAddress.Size = new System.Drawing.Size(227, 20);
            this.txtSearchAddress.TabIndex = 58;
            this.txtSearchAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchAddressKeyDown);
            // 
            // btnWorkBench
            // 
            this.btnWorkBench.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWorkBench.BackColor = System.Drawing.SystemColors.Control;
            this.btnWorkBench.Location = new System.Drawing.Point(620, 4);
            this.btnWorkBench.Name = "btnWorkBench";
            this.btnWorkBench.Size = new System.Drawing.Size(30, 20);
            this.btnWorkBench.TabIndex = 68;
            this.btnWorkBench.Text = "⚙️";
            this.btnWorkBench.UseVisualStyleBackColor = false;
            this.btnWorkBench.Click += new System.EventHandler(this.ButtonLoadWorkBench);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AllowDrop = true;
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 28);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(652, 595);
            this.flowLayoutPanel1.TabIndex = 69;
            this.flowLayoutPanel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.flowLayoutPanel1_Scroll);
            this.flowLayoutPanel1.SizeChanged += new System.EventHandler(this.flowLayoutPanel1_SizeChanged);
            this.flowLayoutPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.flowLayoutPanel1_DragDrop);
            this.flowLayoutPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.flowLayoutPanel1_DragEnter);
            this.flowLayoutPanel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.flowLayoutPanel1_MouseWheel);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.linkLabel1.Location = new System.Drawing.Point(278, 4);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(98, 21);
            this.linkLabel1.TabIndex = 71;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "anon";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MainUserNameClick);
            // 
            // txtVersionByte
            // 
            this.txtVersionByte.Enabled = false;
            this.txtVersionByte.Location = new System.Drawing.Point(369, 53);
            this.txtVersionByte.Name = "txtVersionByte";
            this.txtVersionByte.Size = new System.Drawing.Size(43, 20);
            this.txtVersionByte.TabIndex = 49;
            this.txtVersionByte.Text = "111";
            // 
            // txtUrl
            // 
            this.txtUrl.Enabled = false;
            this.txtUrl.Location = new System.Drawing.Point(178, 53);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(100, 20);
            this.txtUrl.TabIndex = 45;
            this.txtUrl.Text = "http://127.0.0.1:18332";
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(83, 53);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(89, 20);
            this.txtPassword.TabIndex = 43;
            this.txtPassword.Text = "better-password";
            // 
            // txtLogin
            // 
            this.txtLogin.Enabled = false;
            this.txtLogin.Location = new System.Drawing.Point(12, 53);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(65, 20);
            this.txtLogin.TabIndex = 41;
            this.txtLogin.Text = "good-user";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(586, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 20);
            this.button1.TabIndex = 72;
            this.button1.Text = "🔑";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.ButtonLoadConnections);
            // 
            // btnHistoryBack
            // 
            this.btnHistoryBack.BackColor = System.Drawing.SystemColors.Control;
            this.btnHistoryBack.Location = new System.Drawing.Point(2, 4);
            this.btnHistoryBack.Name = "btnHistoryBack";
            this.btnHistoryBack.Size = new System.Drawing.Size(18, 20);
            this.btnHistoryBack.TabIndex = 74;
            this.btnHistoryBack.Text = "<";
            this.btnHistoryBack.UseVisualStyleBackColor = false;
            this.btnHistoryBack.Click += new System.EventHandler(this.btnHistoryBack_Click);
            // 
            // btnHistoryForward
            // 
            this.btnHistoryForward.BackColor = System.Drawing.SystemColors.Control;
            this.btnHistoryForward.Location = new System.Drawing.Point(21, 4);
            this.btnHistoryForward.Name = "btnHistoryForward";
            this.btnHistoryForward.Size = new System.Drawing.Size(18, 20);
            this.btnHistoryForward.TabIndex = 75;
            this.btnHistoryForward.Text = ">";
            this.btnHistoryForward.UseVisualStyleBackColor = false;
            this.btnHistoryForward.Click += new System.EventHandler(this.btnHistoryForward_Click);
            // 
            // btnMint
            // 
            this.btnMint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMint.BackColor = System.Drawing.SystemColors.Control;
            this.btnMint.Location = new System.Drawing.Point(541, 4);
            this.btnMint.Name = "btnMint";
            this.btnMint.Size = new System.Drawing.Size(39, 20);
            this.btnMint.TabIndex = 76;
            this.btnMint.Text = "mint";
            this.btnMint.UseVisualStyleBackColor = false;
            this.btnMint.Click += new System.EventHandler(this.btnMint_Click);
            // 
            // txtQty
            // 
            this.txtQty.Enabled = false;
            this.txtQty.Location = new System.Drawing.Point(457, 53);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(43, 20);
            this.txtQty.TabIndex = 77;
            this.txtQty.Text = "6";
            // 
            // txtLast
            // 
            this.txtLast.Enabled = false;
            this.txtLast.Location = new System.Drawing.Point(506, 53);
            this.txtLast.Name = "txtLast";
            this.txtLast.Size = new System.Drawing.Size(43, 20);
            this.txtLast.TabIndex = 78;
            this.txtLast.Text = "0";
            // 
            // ObjectBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 623);
            this.Controls.Add(this.btnMint);
            this.Controls.Add(this.btnHistoryForward);
            this.Controls.Add(this.btnHistoryBack);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.btnWorkBench);
            this.Controls.Add(this.btnCreated);
            this.Controls.Add(this.btnOwned);
            this.Controls.Add(this.txtSearchAddress);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.txtVersionByte);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.txtLast);
            this.Controls.Add(this.txtQty);
            this.MinimumSize = new System.Drawing.Size(668, 401);
            this.Name = "ObjectBrowser";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Sup!? Object Browser";
            this.Load += new System.EventHandler(this.ObjectBrowserLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCreated;
        private System.Windows.Forms.Button btnOwned;
        private System.Windows.Forms.TextBox txtSearchAddress;
        private System.Windows.Forms.Button btnWorkBench;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox txtVersionByte;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnHistoryBack;
        private System.Windows.Forms.Button btnHistoryForward;
        private System.Windows.Forms.Button btnMint;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.TextBox txtLast;



    }
}