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
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnAttach = new System.Windows.Forms.Button();
            this.txtFromAddress = new System.Windows.Forms.TextBox();
            this.flowAttachments = new System.Windows.Forms.FlowLayoutPanel();
            this.btnEncryptionStatus = new System.Windows.Forms.Button();
            this.txtToAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAttach = new System.Windows.Forms.TextBox();
            this.lblObjectStatus = new System.Windows.Forms.Label();
            this.fromImage = new System.Windows.Forms.PictureBox();
            this.toImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.fromImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toImage)).BeginInit();
            this.SuspendLayout();
            // 
            // supMessage
            // 
            this.supMessage.BackColor = System.Drawing.Color.Black;
            this.supMessage.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.supMessage.ForeColor = System.Drawing.Color.White;
            this.supMessage.Location = new System.Drawing.Point(19, 17);
            this.supMessage.MaxLength = 420;
            this.supMessage.Multiline = true;
            this.supMessage.Name = "supMessage";
            this.supMessage.Size = new System.Drawing.Size(274, 253);
            this.supMessage.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Image = global::SUP.Properties.Resources.disco;
            this.btnRefresh.Location = new System.Drawing.Point(242, 527);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 40);
            this.btnRefresh.TabIndex = 86;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.discoButton_Click);
            // 
            // btnAttach
            // 
            this.btnAttach.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnAttach.ForeColor = System.Drawing.Color.Black;
            this.btnAttach.Location = new System.Drawing.Point(264, 463);
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(28, 40);
            this.btnAttach.TabIndex = 87;
            this.btnAttach.Text = "📎";
            this.btnAttach.UseVisualStyleBackColor = true;
            this.btnAttach.Click += new System.EventHandler(this.btnAttach_Click);
            // 
            // txtFromAddress
            // 
            this.txtFromAddress.Location = new System.Drawing.Point(65, 299);
            this.txtFromAddress.Multiline = true;
            this.txtFromAddress.Name = "txtFromAddress";
            this.txtFromAddress.Size = new System.Drawing.Size(227, 40);
            this.txtFromAddress.TabIndex = 88;
            // 
            // flowAttachments
            // 
            this.flowAttachments.Location = new System.Drawing.Point(18, 407);
            this.flowAttachments.Name = "flowAttachments";
            this.flowAttachments.Size = new System.Drawing.Size(291, 50);
            this.flowAttachments.TabIndex = 89;
            // 
            // btnEncryptionStatus
            // 
            this.btnEncryptionStatus.BackColor = System.Drawing.Color.Blue;
            this.btnEncryptionStatus.Font = new System.Drawing.Font("Segoe UI Emoji", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEncryptionStatus.ForeColor = System.Drawing.Color.Yellow;
            this.btnEncryptionStatus.Location = new System.Drawing.Point(3, 518);
            this.btnEncryptionStatus.Name = "btnEncryptionStatus";
            this.btnEncryptionStatus.Size = new System.Drawing.Size(57, 58);
            this.btnEncryptionStatus.TabIndex = 356;
            this.btnEncryptionStatus.Text = "😍";
            this.btnEncryptionStatus.UseVisualStyleBackColor = false;
            this.btnEncryptionStatus.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtToAddress
            // 
            this.txtToAddress.Location = new System.Drawing.Point(65, 361);
            this.txtToAddress.Multiline = true;
            this.txtToAddress.Name = "txtToAddress";
            this.txtToAddress.Size = new System.Drawing.Size(227, 40);
            this.txtToAddress.TabIndex = 357;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 281);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 15);
            this.label1.TabIndex = 358;
            this.label1.Text = "from:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 345);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 15);
            this.label2.TabIndex = 359;
            this.label2.Text = "to:";
            // 
            // txtAttach
            // 
            this.txtAttach.Location = new System.Drawing.Point(19, 463);
            this.txtAttach.Multiline = true;
            this.txtAttach.Name = "txtAttach";
            this.txtAttach.Size = new System.Drawing.Size(236, 40);
            this.txtAttach.TabIndex = 360;
            // 
            // lblObjectStatus
            // 
            this.lblObjectStatus.AutoSize = true;
            this.lblObjectStatus.Location = new System.Drawing.Point(17, 506);
            this.lblObjectStatus.Name = "lblObjectStatus";
            this.lblObjectStatus.Size = new System.Drawing.Size(0, 15);
            this.lblObjectStatus.TabIndex = 361;
            // 
            // fromImage
            // 
            this.fromImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fromImage.Location = new System.Drawing.Point(18, 299);
            this.fromImage.Name = "fromImage";
            this.fromImage.Size = new System.Drawing.Size(40, 40);
            this.fromImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.fromImage.TabIndex = 355;
            this.fromImage.TabStop = false;
            // 
            // toImage
            // 
            this.toImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toImage.Location = new System.Drawing.Point(18, 361);
            this.toImage.Name = "toImage";
            this.toImage.Size = new System.Drawing.Size(40, 40);
            this.toImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.toImage.TabIndex = 354;
            this.toImage.TabStop = false;
            // 
            // DiscoBall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(311, 579);
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
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "DiscoBall";
            this.Load += new System.EventHandler(this.DiscoBall_Load);
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
        private System.Windows.Forms.TextBox txtAttach;
        private System.Windows.Forms.Label lblObjectStatus;
    }
}
