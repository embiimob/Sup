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
            this.SuspendLayout();
            // 
            // btnCreated
            // 
            this.btnCreated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreated.BackColor = System.Drawing.SystemColors.Control;
            this.btnCreated.Location = new System.Drawing.Point(854, 3);
            this.btnCreated.Name = "btnCreated";
            this.btnCreated.Size = new System.Drawing.Size(55, 20);
            this.btnCreated.TabIndex = 62;
            this.btnCreated.Text = "created";
            this.btnCreated.UseVisualStyleBackColor = false;
            this.btnCreated.Click += new System.EventHandler(this.btnGetCreated_Click);
            // 
            // btnOwned
            // 
            this.btnOwned.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOwned.BackColor = System.Drawing.SystemColors.Control;
            this.btnOwned.Location = new System.Drawing.Point(915, 3);
            this.btnOwned.Name = "btnOwned";
            this.btnOwned.Size = new System.Drawing.Size(55, 20);
            this.btnOwned.TabIndex = 61;
            this.btnOwned.Text = "owned";
            this.btnOwned.UseVisualStyleBackColor = false;
            this.btnOwned.Click += new System.EventHandler(this.btnGetOwned_Click);
            // 
            // txtSearchAddress
            // 
            this.txtSearchAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchAddress.Location = new System.Drawing.Point(12, 5);
            this.txtSearchAddress.Name = "txtSearchAddress";
            this.txtSearchAddress.Size = new System.Drawing.Size(713, 20);
            this.txtSearchAddress.TabIndex = 58;
            this.txtSearchAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchAddress_KeyDown);
            // 
            // btnWorkBench
            // 
            this.btnWorkBench.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWorkBench.BackColor = System.Drawing.SystemColors.Control;
            this.btnWorkBench.Location = new System.Drawing.Point(1037, 4);
            this.btnWorkBench.Name = "btnWorkBench";
            this.btnWorkBench.Size = new System.Drawing.Size(30, 20);
            this.btnWorkBench.TabIndex = 68;
            this.btnWorkBench.Text = "⚙️";
            this.btnWorkBench.UseVisualStyleBackColor = false;
            this.btnWorkBench.Click += new System.EventHandler(this.button4_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 31);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1078, 745);
            this.flowLayoutPanel1.TabIndex = 69;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.linkLabel1.Location = new System.Drawing.Point(742, 3);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(106, 21);
            this.linkLabel1.TabIndex = 71;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "anon";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
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
            this.button1.Location = new System.Drawing.Point(976, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 20);
            this.button1.TabIndex = 72;
            this.button1.Text = "🔑";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // ObjectBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1078, 776);
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
            this.MinimumSize = new System.Drawing.Size(669, 442);
            this.Name = "ObjectBrowser";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Sup!? Object Browser";
            this.Load += new System.EventHandler(this.ObjectBrowser_Load);
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
    }
}