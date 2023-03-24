namespace SUP
{
    partial class SupMain
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnMint = new System.Windows.Forms.Button();
            this.btnWorkBench = new System.Windows.Forms.Button();
            this.btnConnections = new System.Windows.Forms.Button();
            this.btnPublicMessage = new System.Windows.Forms.Button();
            this.btnLive = new System.Windows.Forms.Button();
            this.btnPrivateMessage = new System.Windows.Forms.Button();
            this.tmrSearchMemoryPool = new System.Windows.Forms.Timer(this.components);
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnFollow = new System.Windows.Forms.Button();
            this.btnBlock = new System.Windows.Forms.Button();
            this.btnMute = new System.Windows.Forms.Button();
            this.profileCreatedDate = new System.Windows.Forms.Label();
            this.profileBIO = new System.Windows.Forms.Label();
            this.profileURN = new System.Windows.Forms.Label();
            this.profileIMG = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.activeIMG = new System.Windows.Forms.PictureBox();
            this.flowFollow = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.profileIMG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.activeIMG)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1MinSize = 359;
            this.splitContainer1.Panel2MinSize = 281;
            this.splitContainer1.Size = new System.Drawing.Size(650, 627);
            this.splitContainer1.SplitterDistance = 359;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.DoubleClick += new System.EventHandler(this.splitContainer1_DoubleClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.flowFollow);
            this.splitContainer2.Panel1.Controls.Add(this.btnLive);
            this.splitContainer2.Panel1.Controls.Add(this.activeIMG);
            this.splitContainer2.Panel1.Controls.Add(this.btnConnections);
            this.splitContainer2.Panel1.Controls.Add(this.btnWorkBench);
            this.splitContainer2.Panel1MinSize = 70;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Panel2.Controls.Add(this.btnRefresh);
            this.splitContainer2.Panel2.Controls.Add(this.btnPrivateMessage);
            this.splitContainer2.Panel2.Controls.Add(this.btnPublicMessage);
            this.splitContainer2.Panel2.Controls.Add(this.btnMint);
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer2.Size = new System.Drawing.Size(357, 625);
            this.splitContainer2.SplitterDistance = 70;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.TabStop = false;
            // 
            // btnMint
            // 
            this.btnMint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMint.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F);
            this.btnMint.Location = new System.Drawing.Point(7, 578);
            this.btnMint.Name = "btnMint";
            this.btnMint.Size = new System.Drawing.Size(50, 40);
            this.btnMint.TabIndex = 80;
            this.btnMint.Text = "💎";
            this.btnMint.UseVisualStyleBackColor = true;
            this.btnMint.Click += new System.EventHandler(this.btnMint_Click);
            // 
            // btnWorkBench
            // 
            this.btnWorkBench.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnWorkBench.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F);
            this.btnWorkBench.Location = new System.Drawing.Point(9, 530);
            this.btnWorkBench.Name = "btnWorkBench";
            this.btnWorkBench.Size = new System.Drawing.Size(50, 42);
            this.btnWorkBench.TabIndex = 81;
            this.btnWorkBench.Text = "⚙️";
            this.btnWorkBench.UseVisualStyleBackColor = true;
            this.btnWorkBench.Click += new System.EventHandler(this.ButtonLoadWorkBench);
            // 
            // btnConnections
            // 
            this.btnConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConnections.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F);
            this.btnConnections.Location = new System.Drawing.Point(9, 580);
            this.btnConnections.Name = "btnConnections";
            this.btnConnections.Size = new System.Drawing.Size(50, 40);
            this.btnConnections.TabIndex = 82;
            this.btnConnections.Text = "🗝️";
            this.btnConnections.UseVisualStyleBackColor = true;
            this.btnConnections.Click += new System.EventHandler(this.ButtonLoadConnections);
            // 
            // btnPublicMessage
            // 
            this.btnPublicMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPublicMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F);
            this.btnPublicMessage.Location = new System.Drawing.Point(160, 578);
            this.btnPublicMessage.Name = "btnPublicMessage";
            this.btnPublicMessage.Size = new System.Drawing.Size(50, 40);
            this.btnPublicMessage.TabIndex = 83;
            this.btnPublicMessage.Text = "📣";
            this.btnPublicMessage.UseVisualStyleBackColor = true;
            this.btnPublicMessage.Click += new System.EventHandler(this.btnPublicMessage_Click);
            // 
            // btnLive
            // 
            this.btnLive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLive.BackColor = System.Drawing.Color.White;
            this.btnLive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLive.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F);
            this.btnLive.ForeColor = System.Drawing.Color.Black;
            this.btnLive.Location = new System.Drawing.Point(9, 482);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(50, 42);
            this.btnLive.TabIndex = 83;
            this.btnLive.Text = "🧿";
            this.btnLive.UseVisualStyleBackColor = false;
            this.btnLive.Click += new System.EventHandler(this.btnLive_Click);
            // 
            // btnPrivateMessage
            // 
            this.btnPrivateMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrivateMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F);
            this.btnPrivateMessage.Location = new System.Drawing.Point(77, 578);
            this.btnPrivateMessage.Name = "btnPrivateMessage";
            this.btnPrivateMessage.Size = new System.Drawing.Size(50, 40);
            this.btnPrivateMessage.TabIndex = 84;
            this.btnPrivateMessage.Text = "🤐";
            this.btnPrivateMessage.UseVisualStyleBackColor = true;
            // 
            // tmrSearchMemoryPool
            // 
            this.tmrSearchMemoryPool.Interval = 5000;
            this.tmrSearchMemoryPool.Tick += new System.EventHandler(this.tmrSearchMemoryPool_Tick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F);
            this.btnRefresh.Location = new System.Drawing.Point(229, 580);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 40);
            this.btnRefresh.TabIndex = 85;
            this.btnRefresh.Text = "♻️";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnFollow);
            this.panel1.Controls.Add(this.btnBlock);
            this.panel1.Controls.Add(this.btnMute);
            this.panel1.Controls.Add(this.profileCreatedDate);
            this.panel1.Controls.Add(this.profileBIO);
            this.panel1.Controls.Add(this.profileURN);
            this.panel1.Controls.Add(this.profileIMG);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(286, 172);
            this.panel1.TabIndex = 0;
            // 
            // btnFollow
            // 
            this.btnFollow.Location = new System.Drawing.Point(10, 148);
            this.btnFollow.Name = "btnFollow";
            this.btnFollow.Size = new System.Drawing.Size(67, 23);
            this.btnFollow.TabIndex = 10;
            this.btnFollow.Text = "follow";
            this.btnFollow.UseVisualStyleBackColor = true;
            // 
            // btnBlock
            // 
            this.btnBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBlock.Location = new System.Drawing.Point(212, 148);
            this.btnBlock.Name = "btnBlock";
            this.btnBlock.Size = new System.Drawing.Size(67, 23);
            this.btnBlock.TabIndex = 9;
            this.btnBlock.Text = "block";
            this.btnBlock.UseVisualStyleBackColor = true;
            // 
            // btnMute
            // 
            this.btnMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMute.Location = new System.Drawing.Point(139, 148);
            this.btnMute.Name = "btnMute";
            this.btnMute.Size = new System.Drawing.Size(67, 23);
            this.btnMute.TabIndex = 8;
            this.btnMute.Text = "mute";
            this.btnMute.UseVisualStyleBackColor = true;
            // 
            // profileCreatedDate
            // 
            this.profileCreatedDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.profileCreatedDate.ForeColor = System.Drawing.Color.White;
            this.profileCreatedDate.Location = new System.Drawing.Point(171, 122);
            this.profileCreatedDate.Name = "profileCreatedDate";
            this.profileCreatedDate.Size = new System.Drawing.Size(112, 16);
            this.profileCreatedDate.TabIndex = 3;
            this.profileCreatedDate.Click += new System.EventHandler(this.profileCreatedDate_Click);
            // 
            // profileBIO
            // 
            this.profileBIO.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.profileBIO.ForeColor = System.Drawing.Color.White;
            this.profileBIO.Location = new System.Drawing.Point(110, 9);
            this.profileBIO.Name = "profileBIO";
            this.profileBIO.Size = new System.Drawing.Size(176, 100);
            this.profileBIO.TabIndex = 2;
            // 
            // profileURN
            // 
            this.profileURN.AutoSize = true;
            this.profileURN.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.profileURN.ForeColor = System.Drawing.Color.White;
            this.profileURN.Location = new System.Drawing.Point(3, 117);
            this.profileURN.Name = "profileURN";
            this.profileURN.Size = new System.Drawing.Size(45, 20);
            this.profileURN.TabIndex = 1;
            this.profileURN.Text = "anon";
            // 
            // profileIMG
            // 
            this.profileIMG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.profileIMG.Location = new System.Drawing.Point(4, 9);
            this.profileIMG.Name = "profileIMG";
            this.profileIMG.Size = new System.Drawing.Size(100, 100);
            this.profileIMG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.profileIMG.TabIndex = 0;
            this.profileIMG.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Black;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 178);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(283, 394);
            this.flowLayoutPanel1.TabIndex = 99;
            this.flowLayoutPanel1.WrapContents = false;
            this.flowLayoutPanel1.SizeChanged += new System.EventHandler(this.flowLayoutPanel1_SizeChanged);
            this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // activeIMG
            // 
            this.activeIMG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.activeIMG.Location = new System.Drawing.Point(9, 9);
            this.activeIMG.Name = "activeIMG";
            this.activeIMG.Size = new System.Drawing.Size(50, 50);
            this.activeIMG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.activeIMG.TabIndex = 94;
            this.activeIMG.TabStop = false;
            // 
            // flowFollow
            // 
            this.flowFollow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowFollow.AutoScroll = true;
            this.flowFollow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowFollow.Location = new System.Drawing.Point(5, 65);
            this.flowFollow.Name = "flowFollow";
            this.flowFollow.Size = new System.Drawing.Size(59, 410);
            this.flowFollow.TabIndex = 94;
            // 
            // SupMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 627);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(666, 666);
            this.Name = "SupMain";
            this.ShowIcon = false;
            this.Text = "Sup!?";
            this.Load += new System.EventHandler(this.SupMaincs_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.profileIMG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.activeIMG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnWorkBench;
        private System.Windows.Forms.Button btnConnections;
        private System.Windows.Forms.Button btnPublicMessage;
        private System.Windows.Forms.Button btnMint;
        private System.Windows.Forms.Button btnLive;
        private System.Windows.Forms.Button btnPrivateMessage;
        private System.Windows.Forms.Timer tmrSearchMemoryPool;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnFollow;
        private System.Windows.Forms.Button btnBlock;
        private System.Windows.Forms.Button btnMute;
        private System.Windows.Forms.Label profileCreatedDate;
        private System.Windows.Forms.Label profileBIO;
        private System.Windows.Forms.Label profileURN;
        private System.Windows.Forms.PictureBox profileIMG;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox activeIMG;
        private System.Windows.Forms.FlowLayoutPanel flowFollow;
    }
}