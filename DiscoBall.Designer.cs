namespace SUP
{
    partial class DiscoBall
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.supMessage = new System.Windows.Forms.TextBox();
            this.btnAttach = new System.Windows.Forms.Button();
            this.txtFromAddress = new System.Windows.Forms.TextBox();
            this.flowAttachments = new System.Windows.Forms.FlowLayoutPanel();
            this.btnEncryptionStatus = new System.Windows.Forms.Button();
            this.txtToAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAttach = new System.Windows.Forms.TextBox();
            this.lblObjectStatus = new System.Windows.Forms.Label();
            this.lblTransactionId = new System.Windows.Forms.Label();
            this.btnEMOJI = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRecord = new System.Windows.Forms.Button();
            this.btnGIF = new System.Windows.Forms.Button();
            this.emojiPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnInquiry = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.fromImage = new System.Windows.Forms.PictureBox();
            this.toImage = new System.Windows.Forms.PictureBox();
            this.txtINQJson = new System.Windows.Forms.TextBox();
            this.txtINQAddress = new System.Windows.Forms.TextBox();
            this.btnFromSelector = new System.Windows.Forms.Button();
            this.btnToSelector = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toImage)).BeginInit();
            this.SuspendLayout();
            // 
            // supMessage
            // 
            this.supMessage.BackColor = System.Drawing.Color.Black;
            this.supMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.supMessage.ForeColor = System.Drawing.Color.White;
            this.supMessage.Location = new System.Drawing.Point(19, 19);
            this.supMessage.MaxLength = 420;
            this.supMessage.Multiline = true;
            this.supMessage.Name = "supMessage";
            this.supMessage.Size = new System.Drawing.Size(433, 202);
            this.supMessage.TabIndex = 0;
            this.supMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.supMessage_KeyDown);
            // 
            // btnAttach
            // 
            this.btnAttach.Font = new System.Drawing.Font("Segoe UI Emoji", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAttach.ForeColor = System.Drawing.Color.Black;
            this.btnAttach.Location = new System.Drawing.Point(402, 321);
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(54, 50);
            this.btnAttach.TabIndex = 87;
            this.btnAttach.Text = "📎";
            this.btnAttach.UseVisualStyleBackColor = true;
            this.btnAttach.Click += new System.EventHandler(this.btnAttach_Click);
            // 
            // txtFromAddress
            // 
            this.txtFromAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFromAddress.Location = new System.Drawing.Point(66, 434);
            this.txtFromAddress.Multiline = true;
            this.txtFromAddress.Name = "txtFromAddress";
            this.txtFromAddress.Size = new System.Drawing.Size(330, 46);
            this.txtFromAddress.TabIndex = 88;
            // 
            // flowAttachments
            // 
            this.flowAttachments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowAttachments.Location = new System.Drawing.Point(19, 244);
            this.flowAttachments.Name = "flowAttachments";
            this.flowAttachments.Size = new System.Drawing.Size(314, 50);
            this.flowAttachments.TabIndex = 89;
            // 
            // btnEncryptionStatus
            // 
            this.btnEncryptionStatus.BackColor = System.Drawing.Color.White;
            this.btnEncryptionStatus.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEncryptionStatus.ForeColor = System.Drawing.Color.Black;
            this.btnEncryptionStatus.Location = new System.Drawing.Point(20, 606);
            this.btnEncryptionStatus.Name = "btnEncryptionStatus";
            this.btnEncryptionStatus.Size = new System.Drawing.Size(140, 46);
            this.btnEncryptionStatus.TabIndex = 356;
            this.btnEncryptionStatus.Text = "PUBLIC 😍";
            this.btnEncryptionStatus.UseVisualStyleBackColor = false;
            // 
            // txtToAddress
            // 
            this.txtToAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtToAddress.Location = new System.Drawing.Point(66, 499);
            this.txtToAddress.Multiline = true;
            this.txtToAddress.Name = "txtToAddress";
            this.txtToAddress.Size = new System.Drawing.Size(330, 86);
            this.txtToAddress.TabIndex = 357;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 416);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 358;
            this.label1.Text = "from:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 483);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 359;
            this.label2.Text = "to:";
            // 
            // txtAttach
            // 
            this.txtAttach.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAttach.Location = new System.Drawing.Point(20, 321);
            this.txtAttach.Multiline = true;
            this.txtAttach.Name = "txtAttach";
            this.txtAttach.Size = new System.Drawing.Size(376, 83);
            this.txtAttach.TabIndex = 360;
            this.txtAttach.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAttach_KeyDown);
            // 
            // lblObjectStatus
            // 
            this.lblObjectStatus.AutoSize = true;
            this.lblObjectStatus.Location = new System.Drawing.Point(24, 590);
            this.lblObjectStatus.Name = "lblObjectStatus";
            this.lblObjectStatus.Size = new System.Drawing.Size(0, 13);
            this.lblObjectStatus.TabIndex = 361;
            // 
            // lblTransactionId
            // 
            this.lblTransactionId.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransactionId.Location = new System.Drawing.Point(2, 4);
            this.lblTransactionId.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTransactionId.Name = "lblTransactionId";
            this.lblTransactionId.Size = new System.Drawing.Size(356, 11);
            this.lblTransactionId.TabIndex = 362;
            this.lblTransactionId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnEMOJI
            // 
            this.btnEMOJI.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEMOJI.ForeColor = System.Drawing.Color.Black;
            this.btnEMOJI.Location = new System.Drawing.Point(342, 244);
            this.btnEMOJI.Name = "btnEMOJI";
            this.btnEMOJI.Size = new System.Drawing.Size(54, 50);
            this.btnEMOJI.TabIndex = 363;
            this.btnEMOJI.Text = "🙂";
            this.btnEMOJI.UseVisualStyleBackColor = true;
            this.btnEMOJI.Click += new System.EventHandler(this.btnEMOJI_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Location = new System.Drawing.Point(402, 606);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(54, 46);
            this.btnRefresh.TabIndex = 86;
            this.btnRefresh.Text = "📣";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.discoButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 303);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(324, 13);
            this.label3.TabIndex = 364;
            this.label3.Text = "ENTER URL HERE:  ( HTTP:// ,IPFS:  ,BTC: ,MZC: ,LTC: ,DOG: )";
            // 
            // btnRecord
            // 
            this.btnRecord.Font = new System.Drawing.Font("Segoe UI Emoji", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecord.ForeColor = System.Drawing.Color.Black;
            this.btnRecord.Location = new System.Drawing.Point(166, 606);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnRecord.Size = new System.Drawing.Size(50, 46);
            this.btnRecord.TabIndex = 365;
            this.btnRecord.Text = "⏺️";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnRecord_MouseDown);
            this.btnRecord.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnRecord_MouseUp);
            // 
            // btnGIF
            // 
            this.btnGIF.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGIF.ForeColor = System.Drawing.Color.Black;
            this.btnGIF.Location = new System.Drawing.Point(402, 244);
            this.btnGIF.Name = "btnGIF";
            this.btnGIF.Size = new System.Drawing.Size(54, 50);
            this.btnGIF.TabIndex = 366;
            this.btnGIF.Text = "GIF";
            this.btnGIF.UseVisualStyleBackColor = true;
            this.btnGIF.Click += new System.EventHandler(this.btnGIF_Click);
            // 
            // emojiPanel
            // 
            this.emojiPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emojiPanel.AutoScroll = true;
            this.emojiPanel.Location = new System.Drawing.Point(19, 244);
            this.emojiPanel.Name = "emojiPanel";
            this.emojiPanel.Size = new System.Drawing.Size(437, 408);
            this.emojiPanel.TabIndex = 367;
            this.emojiPanel.Visible = false;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnPrint.ForeColor = System.Drawing.Color.Black;
            this.btnPrint.Location = new System.Drawing.Point(342, 606);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(54, 46);
            this.btnPrint.TabIndex = 368;
            this.btnPrint.Text = "💌";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click_1);
            // 
            // btnInquiry
            // 
            this.btnInquiry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInquiry.Font = new System.Drawing.Font("Segoe UI Emoji", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInquiry.ForeColor = System.Drawing.Color.Black;
            this.btnInquiry.Location = new System.Drawing.Point(286, 606);
            this.btnInquiry.Name = "btnInquiry";
            this.btnInquiry.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnInquiry.Size = new System.Drawing.Size(50, 46);
            this.btnInquiry.TabIndex = 369;
            this.btnInquiry.Text = "⁉️";
            this.btnInquiry.UseVisualStyleBackColor = true;
            this.btnInquiry.Click += new System.EventHandler(this.btnInquiry_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(19, 241);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(437, 411);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // fromImage
            // 
            this.fromImage.BackColor = System.Drawing.Color.Black;
            this.fromImage.Location = new System.Drawing.Point(19, 434);
            this.fromImage.Name = "fromImage";
            this.fromImage.Size = new System.Drawing.Size(40, 40);
            this.fromImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.fromImage.TabIndex = 355;
            this.fromImage.TabStop = false;
            // 
            // toImage
            // 
            this.toImage.BackColor = System.Drawing.Color.Black;
            this.toImage.Location = new System.Drawing.Point(19, 499);
            this.toImage.Name = "toImage";
            this.toImage.Size = new System.Drawing.Size(40, 40);
            this.toImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.toImage.TabIndex = 354;
            this.toImage.TabStop = false;
            // 
            // txtINQJson
            // 
            this.txtINQJson.Location = new System.Drawing.Point(53, 37);
            this.txtINQJson.Multiline = true;
            this.txtINQJson.Name = "txtINQJson";
            this.txtINQJson.Size = new System.Drawing.Size(128, 173);
            this.txtINQJson.TabIndex = 370;
            this.txtINQJson.Visible = false;
            this.txtINQJson.TextChanged += new System.EventHandler(this.txtINQJson_TextChanged);
            // 
            // txtINQAddress
            // 
            this.txtINQAddress.Location = new System.Drawing.Point(187, 190);
            this.txtINQAddress.Name = "txtINQAddress";
            this.txtINQAddress.Size = new System.Drawing.Size(100, 20);
            this.txtINQAddress.TabIndex = 371;
            this.txtINQAddress.Visible = false;
            // 
            // btnFromSelector
            // 
            this.btnFromSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFromSelector.Font = new System.Drawing.Font("Segoe UI Emoji", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFromSelector.ForeColor = System.Drawing.Color.Black;
            this.btnFromSelector.Location = new System.Drawing.Point(402, 434);
            this.btnFromSelector.Name = "btnFromSelector";
            this.btnFromSelector.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnFromSelector.Size = new System.Drawing.Size(50, 46);
            this.btnFromSelector.TabIndex = 372;
            this.btnFromSelector.Text = "👤";
            this.btnFromSelector.UseVisualStyleBackColor = true;
            this.btnFromSelector.Click += new System.EventHandler(this.btnFromSelector_Click);
            // 
            // btnToSelector
            // 
            this.btnToSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToSelector.Font = new System.Drawing.Font("Segoe UI Emoji", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToSelector.ForeColor = System.Drawing.Color.Black;
            this.btnToSelector.Location = new System.Drawing.Point(402, 499);
            this.btnToSelector.Name = "btnToSelector";
            this.btnToSelector.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnToSelector.Size = new System.Drawing.Size(50, 46);
            this.btnToSelector.TabIndex = 373;
            this.btnToSelector.Text = "👤";
            this.btnToSelector.UseVisualStyleBackColor = true;
            this.btnToSelector.Click += new System.EventHandler(this.btnToSelector_Click);
            // 
            // DiscoBall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(474, 669);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.emojiPanel);
            this.Controls.Add(this.btnGIF);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnEMOJI);
            this.Controls.Add(this.lblTransactionId);
            this.Controls.Add(this.lblObjectStatus);
            this.Controls.Add(this.txtAttach);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtToAddress);
            this.Controls.Add(this.btnEncryptionStatus);
            this.Controls.Add(this.fromImage);
            this.Controls.Add(this.toImage);
            this.Controls.Add(this.flowAttachments);
            this.Controls.Add(this.txtFromAddress);
            this.Controls.Add(this.btnAttach);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.supMessage);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnInquiry);
            this.Controls.Add(this.txtINQJson);
            this.Controls.Add(this.txtINQAddress);
            this.Controls.Add(this.btnFromSelector);
            this.Controls.Add(this.btnToSelector);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximumSize = new System.Drawing.Size(490, 708);
            this.MinimumSize = new System.Drawing.Size(490, 708);
            this.Name = "DiscoBall";
            this.Text = "Disco";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.DiscoBall_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox supMessage;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnAttach;
        private System.Windows.Forms.TextBox txtFromAddress;
        private System.Windows.Forms.FlowLayoutPanel flowAttachments;
        private System.Windows.Forms.PictureBox toImage;
        private System.Windows.Forms.PictureBox fromImage;
        private System.Windows.Forms.Button btnEncryptionStatus;
        private System.Windows.Forms.TextBox txtToAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblObjectStatus;
        private System.Windows.Forms.Label lblTransactionId;
        private System.Windows.Forms.Button btnEMOJI;
        public System.Windows.Forms.TextBox txtAttach;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnINQ;
        private System.Windows.Forms.Button btnGIF;
        private System.Windows.Forms.FlowLayoutPanel emojiPanel;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnInquiry;
        public System.Windows.Forms.TextBox txtINQJson;
        public System.Windows.Forms.TextBox txtINQAddress;
        private System.Windows.Forms.Button btnFromSelector;
        private System.Windows.Forms.Button btnToSelector;
    }
}
