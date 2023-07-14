﻿namespace SUP
{
    partial class ObjectBuy
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
            this.btnGive = new System.Windows.Forms.Button();
            this.txtOBJJSON = new System.Windows.Forms.TextBox();
            this.txtOBJP2FK = new System.Windows.Forms.TextBox();
            this.txtAddressListJSON = new System.Windows.Forms.TextBox();
            this.txtSignatureAddress = new System.Windows.Forms.TextBox();
            this.lblCost = new System.Windows.Forms.Label();
            this.txtCurrentOwnerAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblObjectStatus = new System.Windows.Forms.Label();
            this.txtAddressSearch = new System.Windows.Forms.TextBox();
            this.tmrSearchMemoryPool = new System.Windows.Forms.Timer(this.components);
            this.flowOffers = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.flowListings = new System.Windows.Forms.FlowLayoutPanel();
            this.flowInMemoryResults = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtListQty = new System.Windows.Forms.TextBox();
            this.txtEachValue = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ObjectImage = new System.Windows.Forms.PictureBox();
            this.lblObjectCreatedDate = new System.Windows.Forms.Label();
            this.lblLicense = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.Label();
            this.lblTotalOwnedDetail = new System.Windows.Forms.Label();
            this.lblTotalRoyaltiesDetail = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBuyEachCost = new System.Windows.Forms.TextBox();
            this.txtBuyQty = new System.Windows.Forms.TextBox();
            this.btnBuy = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblBuyCost = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ObjectImage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGive
            // 
            this.btnGive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGive.Font = new System.Drawing.Font("Segoe UI Emoji", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGive.Location = new System.Drawing.Point(671, 438);
            this.btnGive.Name = "btnGive";
            this.btnGive.Size = new System.Drawing.Size(112, 32);
            this.btnGive.TabIndex = 275;
            this.btnGive.Text = "LIST";
            this.btnGive.UseVisualStyleBackColor = true;
            this.btnGive.Click += new System.EventHandler(this.giveButton_Click);
            // 
            // txtOBJJSON
            // 
            this.txtOBJJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOBJJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOBJJSON.ForeColor = System.Drawing.Color.Black;
            this.txtOBJJSON.Location = new System.Drawing.Point(17, 426);
            this.txtOBJJSON.Multiline = true;
            this.txtOBJJSON.Name = "txtOBJJSON";
            this.txtOBJJSON.Size = new System.Drawing.Size(123, 42);
            this.txtOBJJSON.TabIndex = 359;
            this.txtOBJJSON.Visible = false;
            // 
            // txtOBJP2FK
            // 
            this.txtOBJP2FK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOBJP2FK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOBJP2FK.ForeColor = System.Drawing.Color.Black;
            this.txtOBJP2FK.Location = new System.Drawing.Point(17, 474);
            this.txtOBJP2FK.Multiline = true;
            this.txtOBJP2FK.Name = "txtOBJP2FK";
            this.txtOBJP2FK.Size = new System.Drawing.Size(123, 35);
            this.txtOBJP2FK.TabIndex = 360;
            this.txtOBJP2FK.Visible = false;
            // 
            // txtAddressListJSON
            // 
            this.txtAddressListJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtAddressListJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddressListJSON.ForeColor = System.Drawing.Color.Black;
            this.txtAddressListJSON.Location = new System.Drawing.Point(17, 515);
            this.txtAddressListJSON.Multiline = true;
            this.txtAddressListJSON.Name = "txtAddressListJSON";
            this.txtAddressListJSON.Size = new System.Drawing.Size(123, 36);
            this.txtAddressListJSON.TabIndex = 358;
            this.txtAddressListJSON.Visible = false;
            // 
            // txtSignatureAddress
            // 
            this.txtSignatureAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSignatureAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtSignatureAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSignatureAddress.ForeColor = System.Drawing.Color.White;
            this.txtSignatureAddress.Location = new System.Drawing.Point(401, 513);
            this.txtSignatureAddress.Multiline = true;
            this.txtSignatureAddress.Name = "txtSignatureAddress";
            this.txtSignatureAddress.Size = new System.Drawing.Size(382, 36);
            this.txtSignatureAddress.TabIndex = 361;
            // 
            // lblCost
            // 
            this.lblCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCost.AutoSize = true;
            this.lblCost.ForeColor = System.Drawing.Color.White;
            this.lblCost.Location = new System.Drawing.Point(628, 412);
            this.lblCost.Name = "lblCost";
            this.lblCost.Size = new System.Drawing.Size(0, 13);
            this.lblCost.TabIndex = 364;
            // 
            // txtCurrentOwnerAddress
            // 
            this.txtCurrentOwnerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtCurrentOwnerAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtCurrentOwnerAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurrentOwnerAddress.ForeColor = System.Drawing.Color.White;
            this.txtCurrentOwnerAddress.Location = new System.Drawing.Point(806, 436);
            this.txtCurrentOwnerAddress.Multiline = true;
            this.txtCurrentOwnerAddress.Name = "txtCurrentOwnerAddress";
            this.txtCurrentOwnerAddress.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtCurrentOwnerAddress.Size = new System.Drawing.Size(385, 32);
            this.txtCurrentOwnerAddress.TabIndex = 365;
            this.txtCurrentOwnerAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(803, 410);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 13);
            this.label3.TabIndex = 368;
            this.label3.Text = "enter owner address to buy from here";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label4.ForeColor = System.Drawing.Color.Yellow;
            this.label4.Location = new System.Drawing.Point(400, 489);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 369;
            this.label4.Text = "signature address required";
            // 
            // lblObjectStatus
            // 
            this.lblObjectStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblObjectStatus.AutoSize = true;
            this.lblObjectStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectStatus.ForeColor = System.Drawing.Color.White;
            this.lblObjectStatus.Location = new System.Drawing.Point(534, 490);
            this.lblObjectStatus.Name = "lblObjectStatus";
            this.lblObjectStatus.Size = new System.Drawing.Size(0, 13);
            this.lblObjectStatus.TabIndex = 363;
            // 
            // txtAddressSearch
            // 
            this.txtAddressSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtAddressSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtAddressSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddressSearch.ForeColor = System.Drawing.Color.White;
            this.txtAddressSearch.Location = new System.Drawing.Point(297, 568);
            this.txtAddressSearch.Multiline = true;
            this.txtAddressSearch.Name = "txtAddressSearch";
            this.txtAddressSearch.Size = new System.Drawing.Size(308, 33);
            this.txtAddressSearch.TabIndex = 371;
            this.txtAddressSearch.Text = "mvVa2Ug1V3U8kZhELeUhE7kjpsPMpBKjht";
            this.txtAddressSearch.Visible = false;
            // 
            // tmrSearchMemoryPool
            // 
            this.tmrSearchMemoryPool.Interval = 5000;
            this.tmrSearchMemoryPool.Tick += new System.EventHandler(this.tmrSearchMemoryPool_Tick);
            // 
            // flowOffers
            // 
            this.flowOffers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowOffers.AutoScroll = true;
            this.flowOffers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowOffers.Location = new System.Drawing.Point(413, 31);
            this.flowOffers.Name = "flowOffers";
            this.flowOffers.Size = new System.Drawing.Size(460, 364);
            this.flowOffers.TabIndex = 374;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(13, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 375;
            this.label1.Text = "in memory pending transactions";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(410, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 376;
            this.label2.Text = "non refundable offers";
            this.label2.DoubleClick += new System.EventHandler(this.label2_DoubleClick);
            // 
            // flowListings
            // 
            this.flowListings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowListings.AutoScroll = true;
            this.flowListings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowListings.Location = new System.Drawing.Point(879, 31);
            this.flowListings.Name = "flowListings";
            this.flowListings.Size = new System.Drawing.Size(312, 364);
            this.flowListings.TabIndex = 377;
            // 
            // flowInMemoryResults
            // 
            this.flowInMemoryResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowInMemoryResults.AutoScroll = true;
            this.flowInMemoryResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowInMemoryResults.Location = new System.Drawing.Point(12, 31);
            this.flowInMemoryResults.Name = "flowInMemoryResults";
            this.flowInMemoryResults.Size = new System.Drawing.Size(395, 364);
            this.flowInMemoryResults.TabIndex = 370;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(876, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 378;
            this.label5.Text = "current listings";
            this.label5.DoubleClick += new System.EventHandler(this.label5_DoubleClick);
            // 
            // txtListQty
            // 
            this.txtListQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtListQty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtListQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtListQty.ForeColor = System.Drawing.Color.White;
            this.txtListQty.Location = new System.Drawing.Point(403, 438);
            this.txtListQty.Multiline = true;
            this.txtListQty.Name = "txtListQty";
            this.txtListQty.Size = new System.Drawing.Size(106, 32);
            this.txtListQty.TabIndex = 379;
            this.txtListQty.Text = "0";
            // 
            // txtEachValue
            // 
            this.txtEachValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtEachValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtEachValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEachValue.ForeColor = System.Drawing.Color.White;
            this.txtEachValue.Location = new System.Drawing.Point(521, 439);
            this.txtEachValue.Multiline = true;
            this.txtEachValue.Name = "txtEachValue";
            this.txtEachValue.Size = new System.Drawing.Size(133, 31);
            this.txtEachValue.TabIndex = 380;
            this.txtEachValue.Text = "0";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(400, 412);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 381;
            this.label6.Text = "qty";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(518, 412);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 382;
            this.label7.Text = "each cost";
            // 
            // ObjectImage
            // 
            this.ObjectImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ObjectImage.BackColor = System.Drawing.Color.Gray;
            this.ObjectImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ObjectImage.ImageLocation = "";
            this.ObjectImage.InitialImage = null;
            this.ObjectImage.Location = new System.Drawing.Point(15, 423);
            this.ObjectImage.Margin = new System.Windows.Forms.Padding(0);
            this.ObjectImage.Name = "ObjectImage";
            this.ObjectImage.Size = new System.Drawing.Size(130, 130);
            this.ObjectImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ObjectImage.TabIndex = 383;
            this.ObjectImage.TabStop = false;
            // 
            // lblObjectCreatedDate
            // 
            this.lblObjectCreatedDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblObjectCreatedDate.AutoSize = true;
            this.lblObjectCreatedDate.ForeColor = System.Drawing.Color.White;
            this.lblObjectCreatedDate.Location = new System.Drawing.Point(151, 463);
            this.lblObjectCreatedDate.Margin = new System.Windows.Forms.Padding(0);
            this.lblObjectCreatedDate.Name = "lblObjectCreatedDate";
            this.lblObjectCreatedDate.Size = new System.Drawing.Size(136, 13);
            this.lblObjectCreatedDate.TabIndex = 386;
            this.lblObjectCreatedDate.Text = "Tue, 27 Jun 2023 03:15:30";
            this.lblObjectCreatedDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLicense
            // 
            this.lblLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLicense.AutoSize = true;
            this.lblLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicense.ForeColor = System.Drawing.Color.White;
            this.lblLicense.Location = new System.Drawing.Point(154, 483);
            this.lblLicense.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(24, 12);
            this.lblLicense.TabIndex = 387;
            this.lblLicense.Text = "CC0";
            this.lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(153, 423);
            this.txtName.Margin = new System.Windows.Forms.Padding(0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(236, 38);
            this.txtName.TabIndex = 385;
            this.txtName.Text = "Royalty Check";
            // 
            // lblTotalOwnedDetail
            // 
            this.lblTotalOwnedDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalOwnedDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalOwnedDetail.ForeColor = System.Drawing.Color.White;
            this.lblTotalOwnedDetail.Location = new System.Drawing.Point(154, 505);
            this.lblTotalOwnedDetail.Name = "lblTotalOwnedDetail";
            this.lblTotalOwnedDetail.Padding = new System.Windows.Forms.Padding(3);
            this.lblTotalOwnedDetail.Size = new System.Drawing.Size(161, 23);
            this.lblTotalOwnedDetail.TabIndex = 388;
            this.lblTotalOwnedDetail.Text = "total: 100";
            this.lblTotalOwnedDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalRoyaltiesDetail
            // 
            this.lblTotalRoyaltiesDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalRoyaltiesDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalRoyaltiesDetail.ForeColor = System.Drawing.Color.White;
            this.lblTotalRoyaltiesDetail.Location = new System.Drawing.Point(154, 528);
            this.lblTotalRoyaltiesDetail.Name = "lblTotalRoyaltiesDetail";
            this.lblTotalRoyaltiesDetail.Padding = new System.Windows.Forms.Padding(3);
            this.lblTotalRoyaltiesDetail.Size = new System.Drawing.Size(161, 23);
            this.lblTotalRoyaltiesDetail.TabIndex = 389;
            this.lblTotalRoyaltiesDetail.Text = "royalties: 9.0 %";
            this.lblTotalRoyaltiesDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(931, 487);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 396;
            this.label8.Text = "each cost";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(803, 487);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 13);
            this.label9.TabIndex = 395;
            this.label9.Text = "qty";
            // 
            // txtBuyEachCost
            // 
            this.txtBuyEachCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtBuyEachCost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtBuyEachCost.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuyEachCost.ForeColor = System.Drawing.Color.White;
            this.txtBuyEachCost.Location = new System.Drawing.Point(932, 514);
            this.txtBuyEachCost.Multiline = true;
            this.txtBuyEachCost.Name = "txtBuyEachCost";
            this.txtBuyEachCost.Size = new System.Drawing.Size(133, 36);
            this.txtBuyEachCost.TabIndex = 394;
            this.txtBuyEachCost.Text = "0";
            // 
            // txtBuyQty
            // 
            this.txtBuyQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtBuyQty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtBuyQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuyQty.ForeColor = System.Drawing.Color.White;
            this.txtBuyQty.Location = new System.Drawing.Point(806, 514);
            this.txtBuyQty.Multiline = true;
            this.txtBuyQty.Name = "txtBuyQty";
            this.txtBuyQty.Size = new System.Drawing.Size(106, 36);
            this.txtBuyQty.TabIndex = 393;
            this.txtBuyQty.Text = "0";
            // 
            // btnBuy
            // 
            this.btnBuy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBuy.Font = new System.Drawing.Font("Segoe UI Emoji", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuy.Location = new System.Drawing.Point(1079, 515);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(112, 34);
            this.btnBuy.TabIndex = 390;
            this.btnBuy.Text = "BUY";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar1.Location = new System.Drawing.Point(12, 568);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1179, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 397;
            // 
            // lblBuyCost
            // 
            this.lblBuyCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBuyCost.AutoSize = true;
            this.lblBuyCost.ForeColor = System.Drawing.Color.White;
            this.lblBuyCost.Location = new System.Drawing.Point(1027, 489);
            this.lblBuyCost.Name = "lblBuyCost";
            this.lblBuyCost.Size = new System.Drawing.Size(0, 13);
            this.lblBuyCost.TabIndex = 398;
            // 
            // ObjectBuy
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1203, 607);
            this.Controls.Add(this.lblBuyCost);
            this.Controls.Add(this.lblObjectStatus);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtBuyEachCost);
            this.Controls.Add(this.txtBuyQty);
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.lblTotalOwnedDetail);
            this.Controls.Add(this.lblTotalRoyaltiesDetail);
            this.Controls.Add(this.txtAddressListJSON);
            this.Controls.Add(this.txtOBJP2FK);
            this.Controls.Add(this.txtOBJJSON);
            this.Controls.Add(this.lblObjectCreatedDate);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.ObjectImage);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtEachValue);
            this.Controls.Add(this.txtListQty);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.flowListings);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flowOffers);
            this.Controls.Add(this.flowInMemoryResults);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCurrentOwnerAddress);
            this.Controls.Add(this.lblCost);
            this.Controls.Add(this.txtSignatureAddress);
            this.Controls.Add(this.btnGive);
            this.Controls.Add(this.txtAddressSearch);
            this.MaximumSize = new System.Drawing.Size(1219, 1219);
            this.MinimumSize = new System.Drawing.Size(1219, 350);
            this.Name = "ObjectBuy";
            this.Text = "ObjectBuy";
            this.Load += new System.EventHandler(this.ObjectGive_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ObjectImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnGive;
        private System.Windows.Forms.TextBox txtOBJJSON;
        private System.Windows.Forms.TextBox txtOBJP2FK;
        private System.Windows.Forms.TextBox txtAddressListJSON;
        private System.Windows.Forms.TextBox txtSignatureAddress;
        private System.Windows.Forms.Label lblCost;
        private System.Windows.Forms.TextBox txtCurrentOwnerAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblObjectStatus;
        private System.Windows.Forms.TextBox txtAddressSearch;
        private System.Windows.Forms.Timer tmrSearchMemoryPool;
        private System.Windows.Forms.FlowLayoutPanel flowOffers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowListings;
        private System.Windows.Forms.FlowLayoutPanel flowInMemoryResults;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtListQty;
        private System.Windows.Forms.TextBox txtEachValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.PictureBox ObjectImage;
        private System.Windows.Forms.Label lblObjectCreatedDate;
        private System.Windows.Forms.Label lblLicense;
        public System.Windows.Forms.Label txtName;
        private System.Windows.Forms.Label lblTotalOwnedDetail;
        private System.Windows.Forms.Label lblTotalRoyaltiesDetail;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtBuyEachCost;
        private System.Windows.Forms.TextBox txtBuyQty;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblBuyCost;
    }
}