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
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.webviewer = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.button1 = new System.Windows.Forms.Button();
            this.OwnersPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CreatorsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTotalOwnedMain = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.imgPicture = new System.Windows.Forms.PictureBox();
            this.txtdesc = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.supPanel = new System.Windows.Forms.Panel();
            this.supFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.txtSupMessage = new System.Windows.Forms.TextBox();
            this.btnRefreshSup = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.Label();
            this.lblTotalOwnedDetail = new System.Windows.Forms.Label();
            this.chkRunTrustedObject = new System.Windows.Forms.CheckBox();
            this.btnRefreshOwners = new System.Windows.Forms.Button();
            this.btnReloadObject = new System.Windows.Forms.Button();
            this.btnRefreshTransactions = new System.Windows.Forms.Button();
            this.lblURNBlockDate = new System.Windows.Forms.Label();
            this.txtURN = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIMG = new System.Windows.Forms.TextBox();
            this.lblIMGBlockDate = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtURI = new System.Windows.Forms.TextBox();
            this.lblURIBlockDate = new System.Windows.Forms.Label();
            this.lblImageFullPath = new System.Windows.Forms.Label();
            this.lblObjectCreatedDate = new System.Windows.Forms.Label();
            this.lblphtitle = new System.Windows.Forms.Label();
            this.lbllcdtitle = new System.Windows.Forms.Label();
            this.lblProcessHeight = new System.Windows.Forms.Label();
            this.lblLastChangedDate = new System.Windows.Forms.Label();
            this.lblURNFullPath = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblLicense = new System.Windows.Forms.Label();
            this.transFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.flowPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webviewer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPicture)).BeginInit();
            this.supPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowPanel
            // 
            this.flowPanel.Controls.Add(this.pictureBox1);
            this.flowPanel.Location = new System.Drawing.Point(295, 21);
            this.flowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(600, 600);
            this.flowPanel.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.MinimumSize = new System.Drawing.Size(600, 600);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(600, 600);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.ShowFullScreenModeClick);
            // 
            // webviewer
            // 
            this.webviewer.AllowExternalDrop = true;
            this.webviewer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.webviewer.CreationProperties = null;
            this.webviewer.DefaultBackgroundColor = System.Drawing.SystemColors.ControlDark;
            this.webviewer.Location = new System.Drawing.Point(295, 21);
            this.webviewer.Name = "webviewer";
            this.webviewer.Size = new System.Drawing.Size(600, 600);
            this.webviewer.TabIndex = 1;
            this.webviewer.ZoomFactor = 1D;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(847, 576);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(34, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "[  ]";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.LaunchURN);
            // 
            // OwnersPanel
            // 
            this.OwnersPanel.AutoScroll = true;
            this.OwnersPanel.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.OwnersPanel.Location = new System.Drawing.Point(919, 41);
            this.OwnersPanel.Name = "OwnersPanel";
            this.OwnersPanel.Size = new System.Drawing.Size(272, 317);
            this.OwnersPanel.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(916, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "owners";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(915, 404);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "creators";
            // 
            // CreatorsPanel
            // 
            this.CreatorsPanel.AutoScroll = true;
            this.CreatorsPanel.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.CreatorsPanel.Location = new System.Drawing.Point(911, 420);
            this.CreatorsPanel.Name = "CreatorsPanel";
            this.CreatorsPanel.Size = new System.Drawing.Size(157, 150);
            this.CreatorsPanel.TabIndex = 7;
            // 
            // lblTotalOwnedMain
            // 
            this.lblTotalOwnedMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalOwnedMain.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTotalOwnedMain.Location = new System.Drawing.Point(86, 61);
            this.lblTotalOwnedMain.Name = "lblTotalOwnedMain";
            this.lblTotalOwnedMain.Padding = new System.Windows.Forms.Padding(3);
            this.lblTotalOwnedMain.Size = new System.Drawing.Size(204, 23);
            this.lblTotalOwnedMain.TabIndex = 9;
            this.lblTotalOwnedMain.Text = "x";
            this.lblTotalOwnedMain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTotalOwnedMain.Click += new System.EventHandler(this.CopyAddressByTotalOwnedClick);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(1069, 420);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(103, 33);
            this.button2.TabIndex = 10;
            this.button2.Text = "give";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(1069, 537);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(103, 33);
            this.button4.TabIndex = 12;
            this.button4.Text = "burn";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // imgPicture
            // 
            this.imgPicture.Location = new System.Drawing.Point(19, 24);
            this.imgPicture.Name = "imgPicture";
            this.imgPicture.Size = new System.Drawing.Size(70, 70);
            this.imgPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgPicture.TabIndex = 15;
            this.imgPicture.TabStop = false;
            this.imgPicture.Click += new System.EventHandler(this.CopyAddressByImageClick);
            // 
            // txtdesc
            // 
            this.txtdesc.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtdesc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtdesc.Location = new System.Drawing.Point(19, 110);
            this.txtdesc.Multiline = true;
            this.txtdesc.Name = "txtdesc";
            this.txtdesc.ReadOnly = true;
            this.txtdesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtdesc.Size = new System.Drawing.Size(256, 206);
            this.txtdesc.TabIndex = 22;
            this.txtdesc.Text = "description";
            this.txtdesc.Click += new System.EventHandler(this.CopyDescriptionByDescriptionClick);
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(1069, 457);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(103, 33);
            this.button5.TabIndex = 28;
            this.button5.Text = "list";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(1069, 498);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(103, 33);
            this.button6.TabIndex = 29;
            this.button6.Text = "buy";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // supPanel
            // 
            this.supPanel.Controls.Add(this.supFlow);
            this.supPanel.Controls.Add(this.txtSupMessage);
            this.supPanel.Location = new System.Drawing.Point(909, 18);
            this.supPanel.Name = "supPanel";
            this.supPanel.Size = new System.Drawing.Size(282, 559);
            this.supPanel.TabIndex = 31;
            // 
            // supFlow
            // 
            this.supFlow.AutoScroll = true;
            this.supFlow.Location = new System.Drawing.Point(5, 6);
            this.supFlow.Name = "supFlow";
            this.supFlow.Size = new System.Drawing.Size(270, 478);
            this.supFlow.TabIndex = 3;
            // 
            // txtSupMessage
            // 
            this.txtSupMessage.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.txtSupMessage.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtSupMessage.Location = new System.Drawing.Point(8, 490);
            this.txtSupMessage.Multiline = true;
            this.txtSupMessage.Name = "txtSupMessage";
            this.txtSupMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSupMessage.Size = new System.Drawing.Size(252, 62);
            this.txtSupMessage.TabIndex = 1;
            this.txtSupMessage.Text = "This experimental Sup!? browser is read only\r\n\r\nUse the apertus.io @address featu" +
    "re to direct your messages to this object\'s address\r\n\r\n* Only signed P2FK messag" +
    "es are inspected.";
            // 
            // btnRefreshSup
            // 
            this.btnRefreshSup.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshSup.Location = new System.Drawing.Point(1069, 579);
            this.btnRefreshSup.Name = "btnRefreshSup";
            this.btnRefreshSup.Size = new System.Drawing.Size(100, 42);
            this.btnRefreshSup.TabIndex = 2;
            this.btnRefreshSup.Text = "📣";
            this.btnRefreshSup.UseVisualStyleBackColor = true;
            this.btnRefreshSup.Click += new System.EventHandler(this.RefreshSupMessages);
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(86, 25);
            this.txtName.Margin = new System.Windows.Forms.Padding(0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(209, 38);
            this.txtName.TabIndex = 33;
            this.txtName.Text = "Title";
            this.txtName.Click += new System.EventHandler(this.CopyAddressByNameClick);
            // 
            // lblTotalOwnedDetail
            // 
            this.lblTotalOwnedDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalOwnedDetail.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTotalOwnedDetail.Location = new System.Drawing.Point(948, 371);
            this.lblTotalOwnedDetail.Name = "lblTotalOwnedDetail";
            this.lblTotalOwnedDetail.Padding = new System.Windows.Forms.Padding(3);
            this.lblTotalOwnedDetail.Size = new System.Drawing.Size(221, 23);
            this.lblTotalOwnedDetail.TabIndex = 34;
            this.lblTotalOwnedDetail.Text = "total";
            this.lblTotalOwnedDetail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkRunTrustedObject
            // 
            this.chkRunTrustedObject.AutoSize = true;
            this.chkRunTrustedObject.Location = new System.Drawing.Point(229, 347);
            this.chkRunTrustedObject.Name = "chkRunTrustedObject";
            this.chkRunTrustedObject.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkRunTrustedObject.Size = new System.Drawing.Size(46, 17);
            this.chkRunTrustedObject.TabIndex = 35;
            this.chkRunTrustedObject.Text = "trust";
            this.chkRunTrustedObject.UseVisualStyleBackColor = true;
            this.chkRunTrustedObject.Visible = false;
            // 
            // btnRefreshOwners
            // 
            this.btnRefreshOwners.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshOwners.Location = new System.Drawing.Point(917, 580);
            this.btnRefreshOwners.Margin = new System.Windows.Forms.Padding(0);
            this.btnRefreshOwners.Name = "btnRefreshOwners";
            this.btnRefreshOwners.Size = new System.Drawing.Size(100, 42);
            this.btnRefreshOwners.TabIndex = 37;
            this.btnRefreshOwners.Text = "👑";
            this.btnRefreshOwners.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRefreshOwners.UseVisualStyleBackColor = true;
            this.btnRefreshOwners.Click += new System.EventHandler(this.ButtonShowObjectDetailsClick);
            // 
            // btnReloadObject
            // 
            this.btnReloadObject.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReloadObject.Location = new System.Drawing.Point(177, 579);
            this.btnReloadObject.Name = "btnReloadObject";
            this.btnReloadObject.Size = new System.Drawing.Size(100, 42);
            this.btnReloadObject.TabIndex = 4;
            this.btnReloadObject.Text = "♻️";
            this.btnReloadObject.UseVisualStyleBackColor = true;
            this.btnReloadObject.Click += new System.EventHandler(this.MainRefreshClick);
            // 
            // btnRefreshTransactions
            // 
            this.btnRefreshTransactions.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshTransactions.Location = new System.Drawing.Point(19, 579);
            this.btnRefreshTransactions.Name = "btnRefreshTransactions";
            this.btnRefreshTransactions.Size = new System.Drawing.Size(100, 42);
            this.btnRefreshTransactions.TabIndex = 38;
            this.btnRefreshTransactions.Text = "🔍";
            this.btnRefreshTransactions.UseVisualStyleBackColor = true;
            this.btnRefreshTransactions.Click += new System.EventHandler(this.ButtonRefreshTransactionsClick);
            // 
            // lblURNBlockDate
            // 
            this.lblURNBlockDate.AutoSize = true;
            this.lblURNBlockDate.Location = new System.Drawing.Point(60, 351);
            this.lblURNBlockDate.Name = "lblURNBlockDate";
            this.lblURNBlockDate.Size = new System.Drawing.Size(103, 13);
            this.lblURNBlockDate.TabIndex = 39;
            this.lblURNBlockDate.Text = "   [ is not immutable ]";
            this.lblURNBlockDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtURN
            // 
            this.txtURN.Location = new System.Drawing.Point(19, 367);
            this.txtURN.Multiline = true;
            this.txtURN.Name = "txtURN";
            this.txtURN.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtURN.Size = new System.Drawing.Size(256, 34);
            this.txtURN.TabIndex = 40;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(19, 349);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 15);
            this.label4.TabIndex = 41;
            this.label4.Text = "URN:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 409);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 44;
            this.label3.Text = "IMG:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtIMG
            // 
            this.txtIMG.Location = new System.Drawing.Point(19, 427);
            this.txtIMG.Multiline = true;
            this.txtIMG.Name = "txtIMG";
            this.txtIMG.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtIMG.Size = new System.Drawing.Size(256, 33);
            this.txtIMG.TabIndex = 43;
            // 
            // lblIMGBlockDate
            // 
            this.lblIMGBlockDate.AutoSize = true;
            this.lblIMGBlockDate.Location = new System.Drawing.Point(60, 411);
            this.lblIMGBlockDate.Name = "lblIMGBlockDate";
            this.lblIMGBlockDate.Size = new System.Drawing.Size(103, 13);
            this.lblIMGBlockDate.TabIndex = 42;
            this.lblIMGBlockDate.Text = "   [ is not immutable ]";
            this.lblIMGBlockDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(19, 465);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 15);
            this.label6.TabIndex = 47;
            this.label6.Text = "URI:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtURI
            // 
            this.txtURI.Location = new System.Drawing.Point(19, 483);
            this.txtURI.Multiline = true;
            this.txtURI.Name = "txtURI";
            this.txtURI.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtURI.Size = new System.Drawing.Size(256, 35);
            this.txtURI.TabIndex = 46;
            // 
            // lblURIBlockDate
            // 
            this.lblURIBlockDate.AutoSize = true;
            this.lblURIBlockDate.Location = new System.Drawing.Point(60, 467);
            this.lblURIBlockDate.Name = "lblURIBlockDate";
            this.lblURIBlockDate.Size = new System.Drawing.Size(103, 13);
            this.lblURIBlockDate.TabIndex = 45;
            this.lblURIBlockDate.Text = "   [ is not immutable ]";
            this.lblURIBlockDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblImageFullPath
            // 
            this.lblImageFullPath.AutoSize = true;
            this.lblImageFullPath.Location = new System.Drawing.Point(295, 621);
            this.lblImageFullPath.Name = "lblImageFullPath";
            this.lblImageFullPath.Size = new System.Drawing.Size(71, 13);
            this.lblImageFullPath.TabIndex = 50;
            this.lblImageFullPath.Text = "IMG Full Path";
            this.lblImageFullPath.Visible = false;
            // 
            // lblObjectCreatedDate
            // 
            this.lblObjectCreatedDate.AutoSize = true;
            this.lblObjectCreatedDate.Location = new System.Drawing.Point(92, 81);
            this.lblObjectCreatedDate.Margin = new System.Windows.Forms.Padding(0);
            this.lblObjectCreatedDate.Name = "lblObjectCreatedDate";
            this.lblObjectCreatedDate.Size = new System.Drawing.Size(94, 13);
            this.lblObjectCreatedDate.TabIndex = 51;
            this.lblObjectCreatedDate.Text = "[ is not immutable ]";
            this.lblObjectCreatedDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblObjectCreatedDate.Click += new System.EventHandler(this.CopyAddressByCreatedDateClick);
            // 
            // lblphtitle
            // 
            this.lblphtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblphtitle.Location = new System.Drawing.Point(15, 526);
            this.lblphtitle.Name = "lblphtitle";
            this.lblphtitle.Size = new System.Drawing.Size(102, 15);
            this.lblphtitle.TabIndex = 52;
            this.lblphtitle.Text = "process height";
            this.lblphtitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbllcdtitle
            // 
            this.lbllcdtitle.AutoSize = true;
            this.lbllcdtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbllcdtitle.Location = new System.Drawing.Point(23, 546);
            this.lbllcdtitle.Name = "lbllcdtitle";
            this.lbllcdtitle.Size = new System.Drawing.Size(94, 15);
            this.lbllcdtitle.TabIndex = 53;
            this.lbllcdtitle.Text = "changed date";
            this.lbllcdtitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblProcessHeight
            // 
            this.lblProcessHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessHeight.Location = new System.Drawing.Point(120, 526);
            this.lblProcessHeight.Name = "lblProcessHeight";
            this.lblProcessHeight.Size = new System.Drawing.Size(168, 15);
            this.lblProcessHeight.TabIndex = 54;
            this.lblProcessHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLastChangedDate
            // 
            this.lblLastChangedDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastChangedDate.Location = new System.Drawing.Point(120, 546);
            this.lblLastChangedDate.Name = "lblLastChangedDate";
            this.lblLastChangedDate.Size = new System.Drawing.Size(168, 15);
            this.lblLastChangedDate.TabIndex = 55;
            this.lblLastChangedDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblURNFullPath
            // 
            this.lblURNFullPath.AutoSize = true;
            this.lblURNFullPath.Location = new System.Drawing.Point(295, 634);
            this.lblURNFullPath.Name = "lblURNFullPath";
            this.lblURNFullPath.Size = new System.Drawing.Size(75, 13);
            this.lblURNFullPath.TabIndex = 48;
            this.lblURNFullPath.Text = "URN Full Path";
            this.lblURNFullPath.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(19, 319);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 15);
            this.label5.TabIndex = 56;
            this.label5.Text = "copyrights";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLicense
            // 
            this.lblLicense.AutoSize = true;
            this.lblLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicense.Location = new System.Drawing.Point(90, 322);
            this.lblLicense.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(88, 12);
            this.lblLicense.TabIndex = 57;
            this.lblLicense.Text = "All Rights Reserved";
            this.lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // transFlow
            // 
            this.transFlow.AutoScroll = true;
            this.transFlow.Location = new System.Drawing.Point(19, 110);
            this.transFlow.Margin = new System.Windows.Forms.Padding(0);
            this.transFlow.Name = "transFlow";
            this.transFlow.Size = new System.Drawing.Size(270, 408);
            this.transFlow.TabIndex = 58;
            // 
            // ObjectDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(1192, 642);
            this.Controls.Add(this.imgPicture);
            this.Controls.Add(this.transFlow);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblImageFullPath);
            this.Controls.Add(this.lblURNFullPath);
            this.Controls.Add(this.lblLastChangedDate);
            this.Controls.Add(this.lblProcessHeight);
            this.Controls.Add(this.lbllcdtitle);
            this.Controls.Add(this.lblphtitle);
            this.Controls.Add(this.lblObjectCreatedDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtURI);
            this.Controls.Add(this.lblURIBlockDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtIMG);
            this.Controls.Add(this.lblIMGBlockDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtURN);
            this.Controls.Add(this.lblURNBlockDate);
            this.Controls.Add(this.btnRefreshOwners);
            this.Controls.Add(this.btnRefreshSup);
            this.Controls.Add(this.btnRefreshTransactions);
            this.Controls.Add(this.btnReloadObject);
            this.Controls.Add(this.chkRunTrustedObject);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.supPanel);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.txtdesc);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lblTotalOwnedMain);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CreatorsPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.OwnersPanel);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.webviewer);
            this.Controls.Add(this.lblTotalOwnedDetail);
            this.MaximumSize = new System.Drawing.Size(1208, 681);
            this.MinimumSize = new System.Drawing.Size(1208, 681);
            this.Name = "ObjectDetails";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "ObjectDetails";
            this.Load += new System.EventHandler(this.ObjectDetails_Load);
            this.flowPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webviewer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPicture)).EndInit();
            this.supPanel.ResumeLayout(false);
            this.supPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Webviewer_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webviewer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FlowLayoutPanel OwnersPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel CreatorsPanel;
        private System.Windows.Forms.Label lblTotalOwnedMain;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.PictureBox imgPicture;
        private System.Windows.Forms.TextBox txtdesc;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Panel supPanel;
        private System.Windows.Forms.Button btnRefreshSup;
        private System.Windows.Forms.TextBox txtSupMessage;
        private System.Windows.Forms.FlowLayoutPanel supFlow;
        private System.Windows.Forms.Label lblTotalOwnedDetail;
        private System.Windows.Forms.CheckBox chkRunTrustedObject;
        private System.Windows.Forms.Button btnRefreshOwners;
        private System.Windows.Forms.Button btnReloadObject;
        private System.Windows.Forms.Button btnRefreshTransactions;
        private System.Windows.Forms.Label lblURNBlockDate;
        private System.Windows.Forms.TextBox txtURN;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIMG;
        private System.Windows.Forms.Label lblIMGBlockDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtURI;
        private System.Windows.Forms.Label lblURIBlockDate;
        private System.Windows.Forms.Label lblImageFullPath;
        private System.Windows.Forms.Label lblObjectCreatedDate;
        private System.Windows.Forms.Label lblphtitle;
        private System.Windows.Forms.Label lbllcdtitle;
        private System.Windows.Forms.Label lblProcessHeight;
        private System.Windows.Forms.Label lblLastChangedDate;
        private System.Windows.Forms.Label lblURNFullPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.FlowLayoutPanel transFlow;
        public System.Windows.Forms.Label txtName;
    }
}