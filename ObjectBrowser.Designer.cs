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
            this.txtQty = new System.Windows.Forms.TextBox();
            this.txtLast = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pages = new System.Windows.Forms.TrackBar();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.doubleClickTimer = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.profileURN = new System.Windows.Forms.LinkLabel();
            this.btnCollections = new System.Windows.Forms.Button();
            this.selectSort = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.imgLoading = new System.Windows.Forms.PictureBox();
            this.btnVideoSearch = new System.Windows.Forms.Button();
            this.btnJukeBox = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pages)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCreated
            // 
            this.btnCreated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreated.BackColor = System.Drawing.Color.White;
            this.btnCreated.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreated.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreated.ForeColor = System.Drawing.Color.Black;
            this.btnCreated.Location = new System.Drawing.Point(466, 3);
            this.btnCreated.Name = "btnCreated";
            this.btnCreated.Size = new System.Drawing.Size(64, 26);
            this.btnCreated.TabIndex = 62;
            this.btnCreated.Text = "created";
            this.btnCreated.UseVisualStyleBackColor = false;
            this.btnCreated.Click += new System.EventHandler(this.ButtonGetCreatedClick);
            // 
            // btnOwned
            // 
            this.btnOwned.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOwned.BackColor = System.Drawing.Color.White;
            this.btnOwned.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOwned.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOwned.ForeColor = System.Drawing.Color.Black;
            this.btnOwned.Location = new System.Drawing.Point(396, 3);
            this.btnOwned.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.btnOwned.Name = "btnOwned";
            this.btnOwned.Size = new System.Drawing.Size(64, 26);
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
            this.txtSearchAddress.Location = new System.Drawing.Point(615, 3);
            this.txtSearchAddress.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.txtSearchAddress.Multiline = true;
            this.txtSearchAddress.Name = "txtSearchAddress";
            this.txtSearchAddress.Size = new System.Drawing.Size(179, 26);
            this.txtSearchAddress.TabIndex = 58;
            this.txtSearchAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchAddressKeyDown);
            // 
            // txtQty
            // 
            this.txtQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQty.Location = new System.Drawing.Point(868, 157);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(43, 20);
            this.txtQty.TabIndex = 77;
            this.txtQty.Text = "9";
            this.txtQty.Visible = false;
            // 
            // txtLast
            // 
            this.txtLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLast.Location = new System.Drawing.Point(297, 3);
            this.txtLast.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.txtLast.Multiline = true;
            this.txtLast.Name = "txtLast";
            this.txtLast.Size = new System.Drawing.Size(42, 26);
            this.txtLast.TabIndex = 78;
            this.txtLast.Text = "0";
            this.txtLast.TextChanged += new System.EventHandler(this.txtLast_TextChanged);
            this.txtLast.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLast_KeyDown);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AllowDrop = true;
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(3, 30, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(987, 545);
            this.flowLayoutPanel1.TabIndex = 82;
            this.flowLayoutPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.flowLayoutPanel1_DragDrop);
            this.flowLayoutPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.flowLayoutPanel1_DragEnter);
            // 
            // pages
            // 
            this.pages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pages.AutoSize = false;
            this.pages.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.pages.LargeChange = 12;
            this.pages.Location = new System.Drawing.Point(0, 500);
            this.pages.Name = "pages";
            this.pages.Size = new System.Drawing.Size(987, 45);
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
            this.txtTotal.Location = new System.Drawing.Point(345, 3);
            this.txtTotal.Multiline = true;
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(42, 26);
            this.txtTotal.TabIndex = 85;
            this.txtTotal.Text = "0";
            // 
            // doubleClickTimer
            // 
            this.doubleClickTimer.Tick += new System.EventHandler(this.doubleClickTimer_Tick);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.flowLayoutPanel2.Controls.Add(this.profileURN);
            this.flowLayoutPanel2.Controls.Add(this.txtLast);
            this.flowLayoutPanel2.Controls.Add(this.txtTotal);
            this.flowLayoutPanel2.Controls.Add(this.btnOwned);
            this.flowLayoutPanel2.Controls.Add(this.btnCreated);
            this.flowLayoutPanel2.Controls.Add(this.btnCollections);
            this.flowLayoutPanel2.Controls.Add(this.txtSearchAddress);
            this.flowLayoutPanel2.Controls.Add(this.selectSort);
            this.flowLayoutPanel2.Controls.Add(this.btnVideoSearch);
            this.flowLayoutPanel2.Controls.Add(this.btnJukeBox);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(987, 32);
            this.flowLayoutPanel2.TabIndex = 86;
            this.flowLayoutPanel2.SizeChanged += new System.EventHandler(this.flowLayoutPanel2_SizeChanged);
            // 
            // profileURN
            // 
            this.profileURN.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.profileURN.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.profileURN.Location = new System.Drawing.Point(3, 0);
            this.profileURN.Name = "profileURN";
            this.profileURN.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.profileURN.Size = new System.Drawing.Size(279, 30);
            this.profileURN.TabIndex = 71;
            this.profileURN.TabStop = true;
            this.profileURN.Text = "anon";
            this.profileURN.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.profileURN.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MainUserNameClick);
            // 
            // btnCollections
            // 
            this.btnCollections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollections.BackColor = System.Drawing.Color.White;
            this.btnCollections.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollections.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollections.ForeColor = System.Drawing.Color.Black;
            this.btnCollections.Location = new System.Drawing.Point(536, 3);
            this.btnCollections.Name = "btnCollections";
            this.btnCollections.Size = new System.Drawing.Size(64, 26);
            this.btnCollections.TabIndex = 86;
            this.btnCollections.Text = "collections";
            this.btnCollections.UseVisualStyleBackColor = false;
            this.btnCollections.Click += new System.EventHandler(this.btnCollections_Click);
            // 
            // selectSort
            // 
            this.selectSort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectSort.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectSort.FormattingEnabled = true;
            this.selectSort.Items.AddRange(new object[] {
            "newest",
            "price: highest",
            "price: lowest",
            "offer: highest",
            "offer: lowest",
            "activity: high",
            "activity: low",
            "oldest"});
            this.selectSort.Location = new System.Drawing.Point(800, 3);
            this.selectSort.Name = "selectSort";
            this.selectSort.Size = new System.Drawing.Size(117, 26);
            this.selectSort.TabIndex = 87;
            this.selectSort.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.imgLoading);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(987, 545);
            this.panel1.TabIndex = 88;
            // 
            // imgLoading
            // 
            this.imgLoading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imgLoading.ImageLocation = "";
            this.imgLoading.Location = new System.Drawing.Point(3, 3);
            this.imgLoading.Name = "imgLoading";
            this.imgLoading.Size = new System.Drawing.Size(984, 539);
            this.imgLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgLoading.TabIndex = 81;
            this.imgLoading.TabStop = false;
            // 
            // btnVideoSearch
            // 
            this.btnVideoSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnVideoSearch.BackColor = System.Drawing.Color.White;
            this.btnVideoSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVideoSearch.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVideoSearch.Location = new System.Drawing.Point(923, 3);
            this.btnVideoSearch.Name = "btnVideoSearch";
            this.btnVideoSearch.Size = new System.Drawing.Size(26, 26);
            this.btnVideoSearch.TabIndex = 102;
            this.btnVideoSearch.Text = "🎬";
            this.btnVideoSearch.UseVisualStyleBackColor = false;
            this.btnVideoSearch.Click += new System.EventHandler(this.btnVideoSearch_Click);
            // 
            // btnJukeBox
            // 
            this.btnJukeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnJukeBox.BackColor = System.Drawing.Color.White;
            this.btnJukeBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJukeBox.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJukeBox.Location = new System.Drawing.Point(955, 3);
            this.btnJukeBox.Name = "btnJukeBox";
            this.btnJukeBox.Size = new System.Drawing.Size(26, 26);
            this.btnJukeBox.TabIndex = 101;
            this.btnJukeBox.Text = "🎵";
            this.btnJukeBox.UseVisualStyleBackColor = false;
            this.btnJukeBox.Click += new System.EventHandler(this.btnJukeBox_Click);
            // 
            // ObjectBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(987, 545);
            this.Controls.Add(this.pages);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtQty);
            this.MinimumSize = new System.Drawing.Size(298, 584);
            this.Name = "ObjectBrowser";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Object Browser";
            this.Load += new System.EventHandler(this.ObjectBrowserLoad);
            this.Resize += new System.EventHandler(this.ObjectBrowser_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pages)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCreated;
        private System.Windows.Forms.Button btnOwned;
        private System.Windows.Forms.TextBox txtSearchAddress;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.TextBox txtLast;
        private System.Windows.Forms.PictureBox imgLoading;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TrackBar pages;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Timer doubleClickTimer;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.LinkLabel profileURN;
        private System.Windows.Forms.Button btnCollections;
        private System.Windows.Forms.ComboBox selectSort;
        private System.Windows.Forms.Button btnVideoSearch;
        private System.Windows.Forms.Button btnJukeBox;
    }
}