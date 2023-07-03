namespace SUP
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
            this.txtObjectAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblObjectStatus = new System.Windows.Forms.Label();
            this.txtAddressSearch = new System.Windows.Forms.TextBox();
            this.btnSearchMemory = new System.Windows.Forms.Button();
            this.tmrSearchMemoryPool = new System.Windows.Forms.Timer(this.components);
            this.flowOffers = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.flowListings = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowInMemoryResults = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ObjectImage = new System.Windows.Forms.PictureBox();
            this.txtdesc = new System.Windows.Forms.TextBox();
            this.lblObjectCreatedDate = new System.Windows.Forms.Label();
            this.lblLicense = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.Label();
            this.flowInMemoryResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ObjectImage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGive
            // 
            this.btnGive.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnGive.Location = new System.Drawing.Point(1071, 596);
            this.btnGive.Name = "btnGive";
            this.btnGive.Size = new System.Drawing.Size(109, 53);
            this.btnGive.TabIndex = 275;
            this.btnGive.Text = "💞";
            this.btnGive.UseVisualStyleBackColor = true;
            this.btnGive.Click += new System.EventHandler(this.giveButton_Click);
            // 
            // txtOBJJSON
            // 
            this.txtOBJJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOBJJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOBJJSON.ForeColor = System.Drawing.Color.Black;
            this.txtOBJJSON.Location = new System.Drawing.Point(45, 531);
            this.txtOBJJSON.Multiline = true;
            this.txtOBJJSON.Name = "txtOBJJSON";
            this.txtOBJJSON.Size = new System.Drawing.Size(160, 61);
            this.txtOBJJSON.TabIndex = 359;
            this.txtOBJJSON.Visible = false;
            // 
            // txtOBJP2FK
            // 
            this.txtOBJP2FK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOBJP2FK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOBJP2FK.ForeColor = System.Drawing.Color.Black;
            this.txtOBJP2FK.Location = new System.Drawing.Point(45, 563);
            this.txtOBJP2FK.Multiline = true;
            this.txtOBJP2FK.Name = "txtOBJP2FK";
            this.txtOBJP2FK.Size = new System.Drawing.Size(160, 61);
            this.txtOBJP2FK.TabIndex = 360;
            this.txtOBJP2FK.Visible = false;
            // 
            // txtAddressListJSON
            // 
            this.txtAddressListJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtAddressListJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddressListJSON.ForeColor = System.Drawing.Color.Black;
            this.txtAddressListJSON.Location = new System.Drawing.Point(45, 623);
            this.txtAddressListJSON.Multiline = true;
            this.txtAddressListJSON.Name = "txtAddressListJSON";
            this.txtAddressListJSON.Size = new System.Drawing.Size(160, 61);
            this.txtAddressListJSON.TabIndex = 358;
            this.txtAddressListJSON.Visible = false;
            // 
            // txtSignatureAddress
            // 
            this.txtSignatureAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtSignatureAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSignatureAddress.ForeColor = System.Drawing.Color.White;
            this.txtSignatureAddress.Location = new System.Drawing.Point(798, 695);
            this.txtSignatureAddress.Multiline = true;
            this.txtSignatureAddress.Name = "txtSignatureAddress";
            this.txtSignatureAddress.Size = new System.Drawing.Size(382, 36);
            this.txtSignatureAddress.TabIndex = 361;
            // 
            // lblCost
            // 
            this.lblCost.AutoSize = true;
            this.lblCost.ForeColor = System.Drawing.Color.White;
            this.lblCost.Location = new System.Drawing.Point(915, 618);
            this.lblCost.Name = "lblCost";
            this.lblCost.Size = new System.Drawing.Size(0, 13);
            this.lblCost.TabIndex = 364;
            // 
            // txtObjectAddress
            // 
            this.txtObjectAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtObjectAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtObjectAddress.ForeColor = System.Drawing.Color.White;
            this.txtObjectAddress.Location = new System.Drawing.Point(798, 535);
            this.txtObjectAddress.Multiline = true;
            this.txtObjectAddress.Name = "txtObjectAddress";
            this.txtObjectAddress.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtObjectAddress.Size = new System.Drawing.Size(385, 32);
            this.txtObjectAddress.TabIndex = 365;
            this.txtObjectAddress.Text = "mvVa2Ug1V3U8kZhELeUhE7kjpsPMpBKjht";
            this.txtObjectAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(795, 509);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 13);
            this.label3.TabIndex = 368;
            this.label3.Text = "enter owner address to buy from here";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(795, 671);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 369;
            this.label4.Text = "enter signature address";
            // 
            // lblObjectStatus
            // 
            this.lblObjectStatus.AutoSize = true;
            this.lblObjectStatus.ForeColor = System.Drawing.Color.White;
            this.lblObjectStatus.Location = new System.Drawing.Point(14, 66);
            this.lblObjectStatus.Name = "lblObjectStatus";
            this.lblObjectStatus.Size = new System.Drawing.Size(0, 13);
            this.lblObjectStatus.TabIndex = 363;
            // 
            // txtAddressSearch
            // 
            this.txtAddressSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtAddressSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddressSearch.ForeColor = System.Drawing.Color.White;
            this.txtAddressSearch.Location = new System.Drawing.Point(441, 502);
            this.txtAddressSearch.Multiline = true;
            this.txtAddressSearch.Name = "txtAddressSearch";
            this.txtAddressSearch.Size = new System.Drawing.Size(337, 33);
            this.txtAddressSearch.TabIndex = 371;
            this.txtAddressSearch.Text = "mvVa2Ug1V3U8kZhELeUhE7kjpsPMpBKjht";
            // 
            // btnSearchMemory
            // 
            this.btnSearchMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSearchMemory.BackColor = System.Drawing.Color.White;
            this.btnSearchMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchMemory.ForeColor = System.Drawing.Color.Black;
            this.btnSearchMemory.Location = new System.Drawing.Point(675, 537);
            this.btnSearchMemory.Name = "btnSearchMemory";
            this.btnSearchMemory.Size = new System.Drawing.Size(92, 33);
            this.btnSearchMemory.TabIndex = 373;
            this.btnSearchMemory.Text = "search";
            this.btnSearchMemory.UseVisualStyleBackColor = false;
            this.btnSearchMemory.Click += new System.EventHandler(this.btnSearchMemory_Click);
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
            this.flowOffers.Location = new System.Drawing.Point(286, 31);
            this.flowOffers.Name = "flowOffers";
            this.flowOffers.Size = new System.Drawing.Size(506, 459);
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
            this.label2.Location = new System.Drawing.Point(283, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 376;
            this.label2.Text = "non refundable offers";
            // 
            // flowListings
            // 
            this.flowListings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowListings.AutoScroll = true;
            this.flowListings.Location = new System.Drawing.Point(798, 31);
            this.flowListings.Name = "flowListings";
            this.flowListings.Size = new System.Drawing.Size(385, 459);
            this.flowListings.TabIndex = 377;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoScroll = true;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(248, 0);
            this.flowLayoutPanel2.TabIndex = 372;
            // 
            // flowInMemoryResults
            // 
            this.flowInMemoryResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowInMemoryResults.AutoScroll = true;
            this.flowInMemoryResults.Controls.Add(this.flowLayoutPanel2);
            this.flowInMemoryResults.Location = new System.Drawing.Point(16, 31);
            this.flowInMemoryResults.Name = "flowInMemoryResults";
            this.flowInMemoryResults.Size = new System.Drawing.Size(264, 459);
            this.flowInMemoryResults.TabIndex = 370;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(795, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 378;
            this.label5.Text = "current listings";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(798, 611);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(106, 36);
            this.textBox1.TabIndex = 379;
            this.textBox1.Text = "5";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(924, 611);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(133, 36);
            this.textBox2.TabIndex = 380;
            this.textBox2.Text = ".000000456";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(795, 584);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 381;
            this.label6.Text = "qty";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(921, 584);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 382;
            this.label7.Text = "each cost";
            // 
            // ObjectImage
            // 
            this.ObjectImage.BackColor = System.Drawing.Color.Gray;
            this.ObjectImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ObjectImage.ImageLocation = "";
            this.ObjectImage.InitialImage = null;
            this.ObjectImage.Location = new System.Drawing.Point(28, 523);
            this.ObjectImage.Margin = new System.Windows.Forms.Padding(0);
            this.ObjectImage.Name = "ObjectImage";
            this.ObjectImage.Size = new System.Drawing.Size(208, 208);
            this.ObjectImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ObjectImage.TabIndex = 383;
            this.ObjectImage.TabStop = false;
            // 
            // txtdesc
            // 
            this.txtdesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtdesc.BackColor = System.Drawing.Color.Black;
            this.txtdesc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtdesc.ForeColor = System.Drawing.Color.White;
            this.txtdesc.Location = new System.Drawing.Point(262, 618);
            this.txtdesc.Margin = new System.Windows.Forms.Padding(0);
            this.txtdesc.Multiline = true;
            this.txtdesc.Name = "txtdesc";
            this.txtdesc.ReadOnly = true;
            this.txtdesc.Size = new System.Drawing.Size(516, 113);
            this.txtdesc.TabIndex = 384;
            this.txtdesc.Text = "description";
            // 
            // lblObjectCreatedDate
            // 
            this.lblObjectCreatedDate.AutoSize = true;
            this.lblObjectCreatedDate.ForeColor = System.Drawing.Color.White;
            this.lblObjectCreatedDate.Location = new System.Drawing.Point(259, 568);
            this.lblObjectCreatedDate.Margin = new System.Windows.Forms.Padding(0);
            this.lblObjectCreatedDate.Name = "lblObjectCreatedDate";
            this.lblObjectCreatedDate.Size = new System.Drawing.Size(94, 13);
            this.lblObjectCreatedDate.TabIndex = 386;
            this.lblObjectCreatedDate.Text = "[ is not immutable ]";
            this.lblObjectCreatedDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLicense
            // 
            this.lblLicense.AutoSize = true;
            this.lblLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicense.ForeColor = System.Drawing.Color.White;
            this.lblLicense.Location = new System.Drawing.Point(261, 588);
            this.lblLicense.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(88, 12);
            this.lblLicense.TabIndex = 387;
            this.lblLicense.Text = "All Rights Reserved";
            this.lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(258, 532);
            this.txtName.Margin = new System.Windows.Forms.Padding(0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(534, 38);
            this.txtName.TabIndex = 385;
            this.txtName.Text = "Title";
            // 
            // ObjectBuy
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1203, 754);
            this.Controls.Add(this.txtAddressSearch);
            this.Controls.Add(this.txtAddressListJSON);
            this.Controls.Add(this.txtdesc);
            this.Controls.Add(this.txtOBJP2FK);
            this.Controls.Add(this.btnSearchMemory);
            this.Controls.Add(this.txtOBJJSON);
            this.Controls.Add(this.lblObjectCreatedDate);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.ObjectImage);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.flowListings);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flowOffers);
            this.Controls.Add(this.flowInMemoryResults);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtObjectAddress);
            this.Controls.Add(this.lblCost);
            this.Controls.Add(this.lblObjectStatus);
            this.Controls.Add(this.txtSignatureAddress);
            this.Controls.Add(this.btnGive);
            this.MinimumSize = new System.Drawing.Size(533, 699);
            this.Name = "ObjectBuy";
            this.Text = "ObjectBuy";
            this.Load += new System.EventHandler(this.ObjectGive_Load);
            this.flowInMemoryResults.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtObjectAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblObjectStatus;
        private System.Windows.Forms.TextBox txtAddressSearch;
        private System.Windows.Forms.Button btnSearchMemory;
        private System.Windows.Forms.Timer tmrSearchMemoryPool;
        private System.Windows.Forms.FlowLayoutPanel flowOffers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowListings;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowInMemoryResults;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.PictureBox ObjectImage;
        private System.Windows.Forms.TextBox txtdesc;
        private System.Windows.Forms.Label lblObjectCreatedDate;
        private System.Windows.Forms.Label lblLicense;
        public System.Windows.Forms.Label txtName;
    }
}