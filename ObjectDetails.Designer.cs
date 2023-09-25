namespace SUP
{
    partial class ObjectDetails
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
            this.lblImageFullPath = new System.Windows.Forms.Label();
            this.lblURNFullPath = new System.Windows.Forms.Label();
            this.txtOfficialURN = new System.Windows.Forms.TextBox();
            this.btnOfficial = new System.Windows.Forms.Button();
            this.transFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.btnLaunchURN = new System.Windows.Forms.Button();
            this.flowPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.KeysFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.lblLicense = new System.Windows.Forms.Label();
            this.lblLastChangedDate = new System.Windows.Forms.Label();
            this.lblProcessHeight = new System.Windows.Forms.Label();
            this.lbllcdtitle = new System.Windows.Forms.Label();
            this.lblphtitle = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtURI = new System.Windows.Forms.TextBox();
            this.lblURIBlockDate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIMG = new System.Windows.Forms.TextBox();
            this.lblIMGBlockDate = new System.Windows.Forms.Label();
            this.txtURN = new System.Windows.Forms.TextBox();
            this.lblURNBlockDate = new System.Windows.Forms.Label();
            this.btnRefreshOwners = new System.Windows.Forms.Button();
            this.btnRefreshSup = new System.Windows.Forms.Button();
            this.btnRefreshTransactions = new System.Windows.Forms.Button();
            this.btnReloadObject = new System.Windows.Forms.Button();
            this.chkRunTrustedObject = new System.Windows.Forms.CheckBox();
            this.txtName = new System.Windows.Forms.Label();
            this.supPanel = new System.Windows.Forms.Panel();
            this.supFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.txtdesc = new System.Windows.Forms.TextBox();
            this.btnBurn = new System.Windows.Forms.Button();
            this.btnGive = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.CreatorsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.OwnersPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.webviewer = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.lblTotalOwnedDetail = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblObjectCreatedDate = new System.Windows.Forms.Label();
            this.registrationPanel = new System.Windows.Forms.Panel();
            this.lblLaunchURI = new System.Windows.Forms.Label();
            this.lblOfficial = new System.Windows.Forms.Label();
            this.lblPleaseStandBy = new System.Windows.Forms.Label();
            this.btnDisco = new System.Windows.Forms.Button();
            this.RoyaltiesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTotalRoyaltiesDetail = new System.Windows.Forms.Label();
            this.btnBuy = new System.Windows.Forms.Button();
            this.imgPicture = new System.Windows.Forms.PictureBox();
            this.btnJukeBox = new System.Windows.Forms.Button();
            this.btnSupFlix = new System.Windows.Forms.Button();
            this.flowPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.supPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webviewer)).BeginInit();
            this.registrationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // lblImageFullPath
            // 
            this.lblImageFullPath.AutoSize = true;
            this.lblImageFullPath.Location = new System.Drawing.Point(292, 810);
            this.lblImageFullPath.Name = "lblImageFullPath";
            this.lblImageFullPath.Size = new System.Drawing.Size(71, 13);
            this.lblImageFullPath.TabIndex = 50;
            this.lblImageFullPath.Text = "IMG Full Path";
            this.lblImageFullPath.Visible = false;
            // 
            // lblURNFullPath
            // 
            this.lblURNFullPath.AutoSize = true;
            this.lblURNFullPath.Location = new System.Drawing.Point(590, 810);
            this.lblURNFullPath.Name = "lblURNFullPath";
            this.lblURNFullPath.Size = new System.Drawing.Size(75, 13);
            this.lblURNFullPath.TabIndex = 48;
            this.lblURNFullPath.Text = "URN Full Path";
            this.lblURNFullPath.Visible = false;
            // 
            // txtOfficialURN
            // 
            this.txtOfficialURN.Location = new System.Drawing.Point(834, 810);
            this.txtOfficialURN.Name = "txtOfficialURN";
            this.txtOfficialURN.Size = new System.Drawing.Size(100, 20);
            this.txtOfficialURN.TabIndex = 60;
            this.txtOfficialURN.Visible = false;
            // 
            // btnOfficial
            // 
            this.btnOfficial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOfficial.BackColor = System.Drawing.Color.Yellow;
            this.btnOfficial.Location = new System.Drawing.Point(738, 487);
            this.btnOfficial.Name = "btnOfficial";
            this.btnOfficial.Size = new System.Drawing.Size(87, 40);
            this.btnOfficial.TabIndex = 105;
            this.btnOfficial.Text = "SEE OFFICIAL";
            this.btnOfficial.UseVisualStyleBackColor = false;
            this.btnOfficial.Visible = false;
            this.btnOfficial.Click += new System.EventHandler(this.btnOfficial_Click);
            // 
            // transFlow
            // 
            this.transFlow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.transFlow.AutoScroll = true;
            this.transFlow.Location = new System.Drawing.Point(9, 84);
            this.transFlow.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.transFlow.Name = "transFlow";
            this.transFlow.Size = new System.Drawing.Size(293, 298);
            this.transFlow.TabIndex = 102;
            // 
            // btnLaunchURN
            // 
            this.btnLaunchURN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLaunchURN.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnLaunchURN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLaunchURN.Location = new System.Drawing.Point(791, 493);
            this.btnLaunchURN.Name = "btnLaunchURN";
            this.btnLaunchURN.Size = new System.Drawing.Size(34, 31);
            this.btnLaunchURN.TabIndex = 66;
            this.btnLaunchURN.Text = "[  ]";
            this.btnLaunchURN.UseVisualStyleBackColor = false;
            this.btnLaunchURN.Click += new System.EventHandler(this.LaunchURN);
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Controls.Add(this.pictureBox1);
            this.flowPanel.Location = new System.Drawing.Point(312, 12);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(531, 527);
            this.flowPanel.TabIndex = 104;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(531, 527);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.ShowFullScreenModeClick);
            // 
            // lblWarning
            // 
            this.lblWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblWarning.Location = new System.Drawing.Point(332, 29);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(222, 40);
            this.lblWarning.TabIndex = 62;
            this.lblWarning.Text = "WARNING FILES EXECUTE LOCALY";
            this.lblWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // KeysFlow
            // 
            this.KeysFlow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.KeysFlow.AutoScroll = true;
            this.KeysFlow.Location = new System.Drawing.Point(9, 385);
            this.KeysFlow.Name = "KeysFlow";
            this.KeysFlow.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.KeysFlow.Size = new System.Drawing.Size(293, 108);
            this.KeysFlow.TabIndex = 64;
            this.KeysFlow.Visible = false;
            // 
            // lblLicense
            // 
            this.lblLicense.AutoSize = true;
            this.lblLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicense.ForeColor = System.Drawing.Color.White;
            this.lblLicense.Location = new System.Drawing.Point(85, 62);
            this.lblLicense.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(88, 12);
            this.lblLicense.TabIndex = 101;
            this.lblLicense.Text = "All Rights Reserved";
            this.lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLastChangedDate
            // 
            this.lblLastChangedDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLastChangedDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastChangedDate.ForeColor = System.Drawing.Color.White;
            this.lblLastChangedDate.Location = new System.Drawing.Point(114, 209);
            this.lblLastChangedDate.Name = "lblLastChangedDate";
            this.lblLastChangedDate.Size = new System.Drawing.Size(168, 15);
            this.lblLastChangedDate.TabIndex = 99;
            this.lblLastChangedDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblProcessHeight
            // 
            this.lblProcessHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProcessHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessHeight.ForeColor = System.Drawing.Color.White;
            this.lblProcessHeight.Location = new System.Drawing.Point(114, 189);
            this.lblProcessHeight.Name = "lblProcessHeight";
            this.lblProcessHeight.Size = new System.Drawing.Size(168, 15);
            this.lblProcessHeight.TabIndex = 98;
            this.lblProcessHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbllcdtitle
            // 
            this.lbllcdtitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbllcdtitle.AutoSize = true;
            this.lbllcdtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbllcdtitle.ForeColor = System.Drawing.Color.White;
            this.lbllcdtitle.Location = new System.Drawing.Point(17, 209);
            this.lbllcdtitle.Name = "lbllcdtitle";
            this.lbllcdtitle.Size = new System.Drawing.Size(94, 15);
            this.lbllcdtitle.TabIndex = 97;
            this.lbllcdtitle.Text = "changed date";
            this.lbllcdtitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblphtitle
            // 
            this.lblphtitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblphtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblphtitle.ForeColor = System.Drawing.Color.White;
            this.lblphtitle.Location = new System.Drawing.Point(9, 189);
            this.lblphtitle.Name = "lblphtitle";
            this.lblphtitle.Size = new System.Drawing.Size(102, 15);
            this.lblphtitle.TabIndex = 96;
            this.lblphtitle.Text = "process height";
            this.lblphtitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(16, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 15);
            this.label6.TabIndex = 94;
            this.label6.Text = "URI:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtURI
            // 
            this.txtURI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtURI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtURI.ForeColor = System.Drawing.Color.White;
            this.txtURI.Location = new System.Drawing.Point(13, 146);
            this.txtURI.Multiline = true;
            this.txtURI.Name = "txtURI";
            this.txtURI.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtURI.Size = new System.Drawing.Size(232, 35);
            this.txtURI.TabIndex = 93;
            // 
            // lblURIBlockDate
            // 
            this.lblURIBlockDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblURIBlockDate.AutoSize = true;
            this.lblURIBlockDate.ForeColor = System.Drawing.Color.White;
            this.lblURIBlockDate.Location = new System.Drawing.Point(54, 130);
            this.lblURIBlockDate.Name = "lblURIBlockDate";
            this.lblURIBlockDate.Size = new System.Drawing.Size(103, 13);
            this.lblURIBlockDate.TabIndex = 92;
            this.lblURIBlockDate.Text = "   [ is not immutable ]";
            this.lblURIBlockDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(15, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 91;
            this.label3.Text = "IMG:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtIMG
            // 
            this.txtIMG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtIMG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtIMG.ForeColor = System.Drawing.Color.White;
            this.txtIMG.Location = new System.Drawing.Point(13, 90);
            this.txtIMG.Multiline = true;
            this.txtIMG.Name = "txtIMG";
            this.txtIMG.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtIMG.Size = new System.Drawing.Size(279, 33);
            this.txtIMG.TabIndex = 90;
            // 
            // lblIMGBlockDate
            // 
            this.lblIMGBlockDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIMGBlockDate.AutoSize = true;
            this.lblIMGBlockDate.ForeColor = System.Drawing.Color.White;
            this.lblIMGBlockDate.Location = new System.Drawing.Point(54, 74);
            this.lblIMGBlockDate.Name = "lblIMGBlockDate";
            this.lblIMGBlockDate.Size = new System.Drawing.Size(103, 13);
            this.lblIMGBlockDate.TabIndex = 89;
            this.lblIMGBlockDate.Text = "   [ is not immutable ]";
            this.lblIMGBlockDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtURN
            // 
            this.txtURN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtURN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtURN.ForeColor = System.Drawing.Color.White;
            this.txtURN.Location = new System.Drawing.Point(13, 30);
            this.txtURN.Multiline = true;
            this.txtURN.Name = "txtURN";
            this.txtURN.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtURN.Size = new System.Drawing.Size(279, 34);
            this.txtURN.TabIndex = 87;
            // 
            // lblURNBlockDate
            // 
            this.lblURNBlockDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblURNBlockDate.AutoSize = true;
            this.lblURNBlockDate.ForeColor = System.Drawing.Color.White;
            this.lblURNBlockDate.Location = new System.Drawing.Point(54, 14);
            this.lblURNBlockDate.Name = "lblURNBlockDate";
            this.lblURNBlockDate.Size = new System.Drawing.Size(103, 13);
            this.lblURNBlockDate.TabIndex = 86;
            this.lblURNBlockDate.Text = "   [ is not immutable ]";
            this.lblURNBlockDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRefreshOwners
            // 
            this.btnRefreshOwners.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshOwners.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnRefreshOwners.Location = new System.Drawing.Point(856, 499);
            this.btnRefreshOwners.Margin = new System.Windows.Forms.Padding(0);
            this.btnRefreshOwners.Name = "btnRefreshOwners";
            this.btnRefreshOwners.Size = new System.Drawing.Size(60, 42);
            this.btnRefreshOwners.TabIndex = 84;
            this.btnRefreshOwners.Text = "👑";
            this.btnRefreshOwners.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRefreshOwners.UseVisualStyleBackColor = true;
            this.btnRefreshOwners.Click += new System.EventHandler(this.ButtonShowObjectDetailsClick);
            // 
            // btnRefreshSup
            // 
            this.btnRefreshSup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshSup.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnRefreshSup.Location = new System.Drawing.Point(1257, 499);
            this.btnRefreshSup.Name = "btnRefreshSup";
            this.btnRefreshSup.Size = new System.Drawing.Size(60, 42);
            this.btnRefreshSup.TabIndex = 67;
            this.btnRefreshSup.Text = "😍";
            this.btnRefreshSup.UseVisualStyleBackColor = true;
            this.btnRefreshSup.Click += new System.EventHandler(this.ShowSupPanel);
            // 
            // btnRefreshTransactions
            // 
            this.btnRefreshTransactions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefreshTransactions.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnRefreshTransactions.Location = new System.Drawing.Point(13, 500);
            this.btnRefreshTransactions.Name = "btnRefreshTransactions";
            this.btnRefreshTransactions.Size = new System.Drawing.Size(60, 42);
            this.btnRefreshTransactions.TabIndex = 85;
            this.btnRefreshTransactions.Text = "🔍";
            this.btnRefreshTransactions.UseVisualStyleBackColor = true;
            this.btnRefreshTransactions.Click += new System.EventHandler(this.ButtonRefreshTransactionsClick);
            // 
            // btnReloadObject
            // 
            this.btnReloadObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReloadObject.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnReloadObject.Location = new System.Drawing.Point(222, 500);
            this.btnReloadObject.Name = "btnReloadObject";
            this.btnReloadObject.Size = new System.Drawing.Size(60, 42);
            this.btnReloadObject.TabIndex = 68;
            this.btnReloadObject.Text = "♻️";
            this.btnReloadObject.UseVisualStyleBackColor = true;
            this.btnReloadObject.Click += new System.EventHandler(this.MainRefreshClick);
            // 
            // chkRunTrustedObject
            // 
            this.chkRunTrustedObject.AutoSize = true;
            this.chkRunTrustedObject.ForeColor = System.Drawing.Color.White;
            this.chkRunTrustedObject.Location = new System.Drawing.Point(252, 62);
            this.chkRunTrustedObject.Name = "chkRunTrustedObject";
            this.chkRunTrustedObject.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkRunTrustedObject.Size = new System.Drawing.Size(46, 17);
            this.chkRunTrustedObject.TabIndex = 83;
            this.chkRunTrustedObject.Text = "trust";
            this.chkRunTrustedObject.UseVisualStyleBackColor = true;
            this.chkRunTrustedObject.Visible = false;
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(82, 6);
            this.txtName.Margin = new System.Windows.Forms.Padding(0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(220, 38);
            this.txtName.TabIndex = 81;
            this.txtName.Text = "Title";
            this.txtName.Click += new System.EventHandler(this.txtName_Click);
            // 
            // supPanel
            // 
            this.supPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.supPanel.Controls.Add(this.supFlow);
            this.supPanel.Location = new System.Drawing.Point(856, 7);
            this.supPanel.Name = "supPanel";
            this.supPanel.Size = new System.Drawing.Size(467, 476);
            this.supPanel.TabIndex = 80;
            this.supPanel.Visible = false;
            // 
            // supFlow
            // 
            this.supFlow.AutoScroll = true;
            this.supFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.supFlow.Location = new System.Drawing.Point(0, 0);
            this.supFlow.Name = "supFlow";
            this.supFlow.Size = new System.Drawing.Size(467, 476);
            this.supFlow.TabIndex = 3;
            // 
            // txtdesc
            // 
            this.txtdesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtdesc.BackColor = System.Drawing.Color.Black;
            this.txtdesc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtdesc.ForeColor = System.Drawing.Color.White;
            this.txtdesc.Location = new System.Drawing.Point(9, 92);
            this.txtdesc.Margin = new System.Windows.Forms.Padding(0);
            this.txtdesc.Multiline = true;
            this.txtdesc.Name = "txtdesc";
            this.txtdesc.ReadOnly = true;
            this.txtdesc.Size = new System.Drawing.Size(283, 405);
            this.txtdesc.TabIndex = 77;
            this.txtdesc.Text = "description";
            // 
            // btnBurn
            // 
            this.btnBurn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBurn.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBurn.Location = new System.Drawing.Point(1257, 393);
            this.btnBurn.Name = "btnBurn";
            this.btnBurn.Size = new System.Drawing.Size(60, 42);
            this.btnBurn.TabIndex = 75;
            this.btnBurn.Text = "🔥";
            this.btnBurn.UseVisualStyleBackColor = true;
            this.btnBurn.Click += new System.EventHandler(this.btnBurn_Click);
            // 
            // btnGive
            // 
            this.btnGive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGive.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGive.Location = new System.Drawing.Point(1257, 441);
            this.btnGive.Name = "btnGive";
            this.btnGive.Size = new System.Drawing.Size(60, 42);
            this.btnGive.TabIndex = 74;
            this.btnGive.Text = "💞";
            this.btnGive.UseVisualStyleBackColor = true;
            this.btnGive.Click += new System.EventHandler(this.btnGive_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(853, 377);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 72;
            this.label2.Text = "creators";
            // 
            // CreatorsPanel
            // 
            this.CreatorsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CreatorsPanel.AutoScroll = true;
            this.CreatorsPanel.BackColor = System.Drawing.Color.Black;
            this.CreatorsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CreatorsPanel.Location = new System.Drawing.Point(856, 393);
            this.CreatorsPanel.Name = "CreatorsPanel";
            this.CreatorsPanel.Size = new System.Drawing.Size(395, 90);
            this.CreatorsPanel.TabIndex = 71;
            // 
            // OwnersPanel
            // 
            this.OwnersPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OwnersPanel.AutoScroll = true;
            this.OwnersPanel.BackColor = System.Drawing.Color.Black;
            this.OwnersPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OwnersPanel.Location = new System.Drawing.Point(856, 26);
            this.OwnersPanel.Name = "OwnersPanel";
            this.OwnersPanel.Size = new System.Drawing.Size(453, 219);
            this.OwnersPanel.TabIndex = 69;
            // 
            // webviewer
            // 
            this.webviewer.AllowExternalDrop = true;
            this.webviewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webviewer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.webviewer.CreationProperties = null;
            this.webviewer.DefaultBackgroundColor = System.Drawing.SystemColors.ControlDark;
            this.webviewer.Location = new System.Drawing.Point(312, 12);
            this.webviewer.Name = "webviewer";
            this.webviewer.Size = new System.Drawing.Size(531, 527);
            this.webviewer.TabIndex = 65;
            this.webviewer.ZoomFactor = 1D;
            // 
            // lblTotalOwnedDetail
            // 
            this.lblTotalOwnedDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalOwnedDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalOwnedDetail.ForeColor = System.Drawing.Color.White;
            this.lblTotalOwnedDetail.Location = new System.Drawing.Point(919, 499);
            this.lblTotalOwnedDetail.Name = "lblTotalOwnedDetail";
            this.lblTotalOwnedDetail.Padding = new System.Windows.Forms.Padding(3);
            this.lblTotalOwnedDetail.Size = new System.Drawing.Size(163, 23);
            this.lblTotalOwnedDetail.TabIndex = 82;
            this.lblTotalOwnedDetail.Text = "total";
            this.lblTotalOwnedDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(15, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 15);
            this.label4.TabIndex = 88;
            this.label4.Text = "URN:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblObjectCreatedDate
            // 
            this.lblObjectCreatedDate.AutoSize = true;
            this.lblObjectCreatedDate.ForeColor = System.Drawing.Color.White;
            this.lblObjectCreatedDate.Location = new System.Drawing.Point(83, 42);
            this.lblObjectCreatedDate.Margin = new System.Windows.Forms.Padding(0);
            this.lblObjectCreatedDate.Name = "lblObjectCreatedDate";
            this.lblObjectCreatedDate.Size = new System.Drawing.Size(94, 13);
            this.lblObjectCreatedDate.TabIndex = 95;
            this.lblObjectCreatedDate.Text = "[ is not immutable ]";
            this.lblObjectCreatedDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // registrationPanel
            // 
            this.registrationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.registrationPanel.Controls.Add(this.lblLaunchURI);
            this.registrationPanel.Controls.Add(this.lblURNBlockDate);
            this.registrationPanel.Controls.Add(this.txtURN);
            this.registrationPanel.Controls.Add(this.lblIMGBlockDate);
            this.registrationPanel.Controls.Add(this.txtIMG);
            this.registrationPanel.Controls.Add(this.label3);
            this.registrationPanel.Controls.Add(this.lblURIBlockDate);
            this.registrationPanel.Controls.Add(this.txtURI);
            this.registrationPanel.Controls.Add(this.label6);
            this.registrationPanel.Controls.Add(this.lblphtitle);
            this.registrationPanel.Controls.Add(this.lbllcdtitle);
            this.registrationPanel.Controls.Add(this.lblProcessHeight);
            this.registrationPanel.Controls.Add(this.lblLastChangedDate);
            this.registrationPanel.Controls.Add(this.label4);
            this.registrationPanel.Location = new System.Drawing.Point(0, 259);
            this.registrationPanel.Name = "registrationPanel";
            this.registrationPanel.Size = new System.Drawing.Size(294, 237);
            this.registrationPanel.TabIndex = 106;
            this.registrationPanel.Visible = false;
            // 
            // lblLaunchURI
            // 
            this.lblLaunchURI.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblLaunchURI.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lblLaunchURI.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLaunchURI.Location = new System.Drawing.Point(251, 146);
            this.lblLaunchURI.Name = "lblLaunchURI";
            this.lblLaunchURI.Padding = new System.Windows.Forms.Padding(2, 0, 0, 4);
            this.lblLaunchURI.Size = new System.Drawing.Size(37, 35);
            this.lblLaunchURI.TabIndex = 109;
            this.lblLaunchURI.Text = "[  ]";
            this.lblLaunchURI.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLaunchURI.Click += new System.EventHandler(this.lblLaunchURI_Click);
            // 
            // lblOfficial
            // 
            this.lblOfficial.AutoSize = true;
            this.lblOfficial.BackColor = System.Drawing.SystemColors.HotTrack;
            this.lblOfficial.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOfficial.ForeColor = System.Drawing.Color.Yellow;
            this.lblOfficial.Location = new System.Drawing.Point(39, 62);
            this.lblOfficial.Margin = new System.Windows.Forms.Padding(0);
            this.lblOfficial.Name = "lblOfficial";
            this.lblOfficial.Padding = new System.Windows.Forms.Padding(4, 1, 1, 3);
            this.lblOfficial.Size = new System.Drawing.Size(32, 22);
            this.lblOfficial.TabIndex = 107;
            this.lblOfficial.Text = "👑";
            this.lblOfficial.Visible = false;
            // 
            // lblPleaseStandBy
            // 
            this.lblPleaseStandBy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPleaseStandBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPleaseStandBy.ForeColor = System.Drawing.Color.White;
            this.lblPleaseStandBy.Location = new System.Drawing.Point(312, 12);
            this.lblPleaseStandBy.Name = "lblPleaseStandBy";
            this.lblPleaseStandBy.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblPleaseStandBy.Size = new System.Drawing.Size(528, 527);
            this.lblPleaseStandBy.TabIndex = 108;
            this.lblPleaseStandBy.Text = "please stand by... locked for loading";
            this.lblPleaseStandBy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDisco
            // 
            this.btnDisco.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisco.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisco.Location = new System.Drawing.Point(1191, 499);
            this.btnDisco.Name = "btnDisco";
            this.btnDisco.Size = new System.Drawing.Size(60, 42);
            this.btnDisco.TabIndex = 79;
            this.btnDisco.Text = "📣";
            this.btnDisco.UseVisualStyleBackColor = true;
            this.btnDisco.Click += new System.EventHandler(this.btnDisco_Click);
            // 
            // RoyaltiesPanel
            // 
            this.RoyaltiesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RoyaltiesPanel.AutoScroll = true;
            this.RoyaltiesPanel.BackColor = System.Drawing.Color.Black;
            this.RoyaltiesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RoyaltiesPanel.Location = new System.Drawing.Point(856, 274);
            this.RoyaltiesPanel.Name = "RoyaltiesPanel";
            this.RoyaltiesPanel.Size = new System.Drawing.Size(453, 90);
            this.RoyaltiesPanel.TabIndex = 70;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(853, 258);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 109;
            this.label1.Text = "royalties";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(853, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 110;
            this.label5.Text = "owners";
            // 
            // lblTotalRoyaltiesDetail
            // 
            this.lblTotalRoyaltiesDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalRoyaltiesDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalRoyaltiesDetail.ForeColor = System.Drawing.Color.White;
            this.lblTotalRoyaltiesDetail.Location = new System.Drawing.Point(919, 519);
            this.lblTotalRoyaltiesDetail.Name = "lblTotalRoyaltiesDetail";
            this.lblTotalRoyaltiesDetail.Padding = new System.Windows.Forms.Padding(3);
            this.lblTotalRoyaltiesDetail.Size = new System.Drawing.Size(160, 23);
            this.lblTotalRoyaltiesDetail.TabIndex = 111;
            this.lblTotalRoyaltiesDetail.Text = "royalties";
            this.lblTotalRoyaltiesDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnBuy
            // 
            this.btnBuy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBuy.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnBuy.Location = new System.Drawing.Point(113, 499);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(60, 42);
            this.btnBuy.TabIndex = 112;
            this.btnBuy.Text = "⚡️";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // imgPicture
            // 
            this.imgPicture.Location = new System.Drawing.Point(9, 7);
            this.imgPicture.Name = "imgPicture";
            this.imgPicture.Size = new System.Drawing.Size(70, 70);
            this.imgPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgPicture.TabIndex = 76;
            this.imgPicture.TabStop = false;
            this.imgPicture.Click += new System.EventHandler(this.imgPicture_Click);
            this.imgPicture.Validated += new System.EventHandler(this.imgPicture_Validated);
            // 
            // btnJukeBox
            // 
            this.btnJukeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJukeBox.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJukeBox.Location = new System.Drawing.Point(1125, 500);
            this.btnJukeBox.Name = "btnJukeBox";
            this.btnJukeBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnJukeBox.Size = new System.Drawing.Size(60, 42);
            this.btnJukeBox.TabIndex = 113;
            this.btnJukeBox.Text = "🎵";
            this.btnJukeBox.UseVisualStyleBackColor = true;
            this.btnJukeBox.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSupFlix
            // 
            this.btnSupFlix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSupFlix.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSupFlix.Location = new System.Drawing.Point(1059, 499);
            this.btnSupFlix.Name = "btnSupFlix";
            this.btnSupFlix.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnSupFlix.Size = new System.Drawing.Size(60, 42);
            this.btnSupFlix.TabIndex = 114;
            this.btnSupFlix.Text = "🎬";
            this.btnSupFlix.UseVisualStyleBackColor = true;
            this.btnSupFlix.Click += new System.EventHandler(this.btnSupFlix_Click);
            // 
            // ObjectDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1329, 549);
            this.Controls.Add(this.btnSupFlix);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.btnJukeBox);
            this.Controls.Add(this.btnOfficial);
            this.Controls.Add(this.btnLaunchURN);
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.registrationPanel);
            this.Controls.Add(this.lblOfficial);
            this.Controls.Add(this.txtdesc);
            this.Controls.Add(this.supPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RoyaltiesPanel);
            this.Controls.Add(this.lblPleaseStandBy);
            this.Controls.Add(this.lblObjectCreatedDate);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.OwnersPanel);
            this.Controls.Add(this.chkRunTrustedObject);
            this.Controls.Add(this.transFlow);
            this.Controls.Add(this.KeysFlow);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.imgPicture);
            this.Controls.Add(this.btnRefreshOwners);
            this.Controls.Add(this.btnRefreshSup);
            this.Controls.Add(this.btnRefreshTransactions);
            this.Controls.Add(this.btnReloadObject);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.btnDisco);
            this.Controls.Add(this.btnBurn);
            this.Controls.Add(this.btnGive);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CreatorsPanel);
            this.Controls.Add(this.webviewer);
            this.Controls.Add(this.lblTotalOwnedDetail);
            this.Controls.Add(this.txtOfficialURN);
            this.Controls.Add(this.lblImageFullPath);
            this.Controls.Add(this.lblURNFullPath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblTotalRoyaltiesDetail);
            this.MinimumSize = new System.Drawing.Size(1069, 421);
            this.Name = "ObjectDetails";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Object Details";
            this.Load += new System.EventHandler(this.ObjectDetails_Load);
            this.flowPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.supPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webviewer)).EndInit();
            this.registrationPanel.ResumeLayout(false);
            this.registrationPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Webviewer_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion
        private System.Windows.Forms.Label lblImageFullPath;
        private System.Windows.Forms.Label lblURNFullPath;
        private System.Windows.Forms.TextBox txtOfficialURN;
        public System.Windows.Forms.Button btnOfficial;
        private System.Windows.Forms.FlowLayoutPanel transFlow;
        private System.Windows.Forms.Button btnLaunchURN;
        private System.Windows.Forms.Panel flowPanel;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel KeysFlow;
        private System.Windows.Forms.PictureBox imgPicture;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.Label lblLastChangedDate;
        private System.Windows.Forms.Label lblProcessHeight;
        private System.Windows.Forms.Label lbllcdtitle;
        private System.Windows.Forms.Label lblphtitle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtURI;
        private System.Windows.Forms.Label lblURIBlockDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIMG;
        private System.Windows.Forms.Label lblIMGBlockDate;
        private System.Windows.Forms.TextBox txtURN;
        private System.Windows.Forms.Label lblURNBlockDate;
        private System.Windows.Forms.Button btnRefreshOwners;
        private System.Windows.Forms.Button btnRefreshSup;
        private System.Windows.Forms.Button btnRefreshTransactions;
        private System.Windows.Forms.Button btnReloadObject;
        private System.Windows.Forms.CheckBox chkRunTrustedObject;
        public System.Windows.Forms.Label txtName;
        private System.Windows.Forms.Panel supPanel;
        private System.Windows.Forms.TextBox txtdesc;
        private System.Windows.Forms.Button btnBurn;
        private System.Windows.Forms.Button btnGive;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel CreatorsPanel;
        private System.Windows.Forms.FlowLayoutPanel OwnersPanel;
        private Microsoft.Web.WebView2.WinForms.WebView2 webviewer;
        private System.Windows.Forms.Label lblTotalOwnedDetail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblObjectCreatedDate;
        private System.Windows.Forms.Panel registrationPanel;
        public System.Windows.Forms.Label lblOfficial;
        private System.Windows.Forms.Label lblPleaseStandBy;
        private System.Windows.Forms.Label lblLaunchURI;
        private System.Windows.Forms.FlowLayoutPanel supFlow;
        private System.Windows.Forms.Button btnDisco;
        private System.Windows.Forms.FlowLayoutPanel RoyaltiesPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblTotalRoyaltiesDetail;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.Button btnJukeBox;
        private System.Windows.Forms.Button btnSupFlix;
    }
}