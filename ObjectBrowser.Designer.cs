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
            this.components = new System.ComponentModel.Container();
            this.btnCreated = new System.Windows.Forms.Button();
            this.btnOwned = new System.Windows.Forms.Button();
            this.txtSearchAddress = new System.Windows.Forms.TextBox();
            this.btnWorkBench = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btnConnections = new System.Windows.Forms.Button();
            this.btnHistoryBack = new System.Windows.Forms.Button();
            this.btnHistoryForward = new System.Windows.Forms.Button();
            this.btnMint = new System.Windows.Forms.Button();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.txtLast = new System.Windows.Forms.TextBox();
            this.btnLive = new System.Windows.Forms.Button();
            this.tmrSearchMemoryPool = new System.Windows.Forms.Timer(this.components);
            this.imgLoading = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pages = new System.Windows.Forms.TrackBar();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.doubleClickTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.imgLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pages)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCreated
            // 
            this.btnCreated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreated.BackColor = System.Drawing.Color.White;
            this.btnCreated.ForeColor = System.Drawing.Color.Black;
            this.btnCreated.Location = new System.Drawing.Point(470, 4);
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
            this.btnOwned.BackColor = System.Drawing.Color.White;
            this.btnOwned.ForeColor = System.Drawing.Color.Black;
            this.btnOwned.Location = new System.Drawing.Point(529, 4);
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
            this.txtSearchAddress.CausesValidation = false;
            this.txtSearchAddress.Location = new System.Drawing.Point(42, 4);
            this.txtSearchAddress.Name = "txtSearchAddress";
            this.txtSearchAddress.Size = new System.Drawing.Size(175, 20);
            this.txtSearchAddress.TabIndex = 58;
            this.txtSearchAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchAddressKeyDown);
            // 
            // btnWorkBench
            // 
            this.btnWorkBench.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWorkBench.BackColor = System.Drawing.Color.White;
            this.btnWorkBench.ForeColor = System.Drawing.Color.Black;
            this.btnWorkBench.Location = new System.Drawing.Point(661, 4);
            this.btnWorkBench.Name = "btnWorkBench";
            this.btnWorkBench.Size = new System.Drawing.Size(30, 20);
            this.btnWorkBench.TabIndex = 68;
            this.btnWorkBench.Text = "⚙️";
            this.btnWorkBench.UseVisualStyleBackColor = false;
            this.btnWorkBench.Click += new System.EventHandler(this.ButtonLoadWorkBench);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.linkLabel1.Location = new System.Drawing.Point(321, 4);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(98, 21);
            this.linkLabel1.TabIndex = 71;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "anon";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MainUserNameClick);
            // 
            // btnConnections
            // 
            this.btnConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnections.BackColor = System.Drawing.Color.White;
            this.btnConnections.ForeColor = System.Drawing.Color.Black;
            this.btnConnections.Location = new System.Drawing.Point(627, 4);
            this.btnConnections.Name = "btnConnections";
            this.btnConnections.Size = new System.Drawing.Size(30, 20);
            this.btnConnections.TabIndex = 72;
            this.btnConnections.Text = "🔑";
            this.btnConnections.UseVisualStyleBackColor = false;
            this.btnConnections.Click += new System.EventHandler(this.ButtonLoadConnections);
            // 
            // btnHistoryBack
            // 
            this.btnHistoryBack.BackColor = System.Drawing.Color.White;
            this.btnHistoryBack.ForeColor = System.Drawing.Color.Black;
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
            this.btnHistoryForward.BackColor = System.Drawing.Color.White;
            this.btnHistoryForward.ForeColor = System.Drawing.Color.Black;
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
            this.btnMint.BackColor = System.Drawing.Color.White;
            this.btnMint.ForeColor = System.Drawing.Color.Black;
            this.btnMint.Location = new System.Drawing.Point(582, 4);
            this.btnMint.Name = "btnMint";
            this.btnMint.Size = new System.Drawing.Size(39, 20);
            this.btnMint.TabIndex = 76;
            this.btnMint.Text = "mint";
            this.btnMint.UseVisualStyleBackColor = false;
            this.btnMint.Click += new System.EventHandler(this.btnMint_Click);
            // 
            // txtQty
            // 
            this.txtQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQty.Location = new System.Drawing.Point(570, 95);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(43, 20);
            this.txtQty.TabIndex = 77;
            this.txtQty.Text = "12";
            this.txtQty.Visible = false;
            // 
            // txtLast
            // 
            this.txtLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLast.Location = new System.Drawing.Point(222, 4);
            this.txtLast.Name = "txtLast";
            this.txtLast.Size = new System.Drawing.Size(43, 20);
            this.txtLast.TabIndex = 78;
            this.txtLast.Text = "0";
            this.txtLast.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLast_KeyDown);
            // 
            // btnLive
            // 
            this.btnLive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLive.BackColor = System.Drawing.Color.White;
            this.btnLive.ForeColor = System.Drawing.Color.Black;
            this.btnLive.Location = new System.Drawing.Point(425, 4);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(39, 20);
            this.btnLive.TabIndex = 79;
            this.btnLive.Text = "live";
            this.btnLive.UseVisualStyleBackColor = false;
            this.btnLive.Click += new System.EventHandler(this.btnLive_Click);
            // 
            // tmrSearchMemoryPool
            // 
            this.tmrSearchMemoryPool.Interval = 5000;
            this.tmrSearchMemoryPool.Tick += new System.EventHandler(this.tmrSearchMemoryPool_Tick);
            // 
            // imgLoading
            // 
            this.imgLoading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imgLoading.ImageLocation = "";
            this.imgLoading.Location = new System.Drawing.Point(0, 28);
            this.imgLoading.Name = "imgLoading";
            this.imgLoading.Size = new System.Drawing.Size(693, 493);
            this.imgLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgLoading.TabIndex = 81;
            this.imgLoading.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AllowDrop = true;
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 28);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(693, 448);
            this.flowLayoutPanel1.TabIndex = 82;
            this.flowLayoutPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.flowLayoutPanel1_DragDrop);
            this.flowLayoutPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.flowLayoutPanel1_DragEnter);
            // 
            // pages
            // 
            this.pages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pages.AutoSize = false;
            this.pages.LargeChange = 12;
            this.pages.Location = new System.Drawing.Point(0, 476);
            this.pages.Name = "pages";
            this.pages.Size = new System.Drawing.Size(692, 45);
            this.pages.TabIndex = 84;
            this.pages.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.pages.Scroll += new System.EventHandler(this.pages_Scroll);
            this.pages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pages_MouseDown);
            this.pages.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pages_MouseUp);
            // 
            // txtTotal
            // 
            this.txtTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotal.Enabled = false;
            this.txtTotal.Location = new System.Drawing.Point(271, 4);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(43, 20);
            this.txtTotal.TabIndex = 85;
            this.txtTotal.Text = "0";
            // 
            // doubleClickTimer
            // 
            this.doubleClickTimer.Tick += new System.EventHandler(this.doubleClickTimer_Tick);
            // 
            // ObjectBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 519);
            this.Controls.Add(this.pages);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtLast);
            this.Controls.Add(this.txtQty);
            this.Controls.Add(this.btnLive);
            this.Controls.Add(this.btnMint);
            this.Controls.Add(this.btnHistoryForward);
            this.Controls.Add(this.btnHistoryBack);
            this.Controls.Add(this.btnConnections);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.btnWorkBench);
            this.Controls.Add(this.btnCreated);
            this.Controls.Add(this.btnOwned);
            this.Controls.Add(this.txtSearchAddress);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.imgLoading);
            this.MinimumSize = new System.Drawing.Size(709, 558);
            this.Name = "ObjectBrowser";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Sup!? Object Browser";
            this.Load += new System.EventHandler(this.ObjectBrowserLoad);
            this.Resize += new System.EventHandler(this.ObjectBrowser_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.imgLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCreated;
        private System.Windows.Forms.Button btnOwned;
        private System.Windows.Forms.TextBox txtSearchAddress;
        private System.Windows.Forms.Button btnWorkBench;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button btnConnections;
        private System.Windows.Forms.Button btnHistoryBack;
        private System.Windows.Forms.Button btnHistoryForward;
        private System.Windows.Forms.Button btnMint;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.TextBox txtLast;
        private System.Windows.Forms.Button btnLive;
        private System.Windows.Forms.Timer tmrSearchMemoryPool;
        private System.Windows.Forms.PictureBox imgLoading;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TrackBar pages;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Timer doubleClickTimer;
    }
}