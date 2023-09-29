namespace SUP
{
    partial class FoundINQControl
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
            this.txtQUE = new System.Windows.Forms.Label();
            this.txtTransactionId = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.Label();
            this.txtFoundAddress = new System.Windows.Forms.TextBox();
            this.lblTrash = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnValueTotal = new System.Windows.Forms.Button();
            this.btnShowAllOrGated = new System.Windows.Forms.Button();
            this.lblVotesOrValue = new System.Windows.Forms.Label();
            this.txtCreatedBy = new System.Windows.Forms.Label();
            this.txtCreatedDate = new System.Windows.Forms.Label();
            this.txtObjectAddress = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtQUE
            // 
            this.txtQUE.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQUE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtQUE.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQUE.ForeColor = System.Drawing.Color.White;
            this.txtQUE.Location = new System.Drawing.Point(0, 0);
            this.txtQUE.Name = "txtQUE";
            this.txtQUE.Size = new System.Drawing.Size(489, 90);
            this.txtQUE.TabIndex = 6;
            this.txtQUE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtTransactionId
            // 
            this.txtTransactionId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTransactionId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtTransactionId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTransactionId.ForeColor = System.Drawing.Color.White;
            this.txtTransactionId.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.txtTransactionId.Location = new System.Drawing.Point(13, 438);
            this.txtTransactionId.Name = "txtTransactionId";
            this.txtTransactionId.Size = new System.Drawing.Size(481, 21);
            this.txtTransactionId.TabIndex = 8;
            this.txtTransactionId.Text = "@";
            this.txtTransactionId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtTransactionId.Click += new System.EventHandler(this.ObjectAddress_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStatus.ForeColor = System.Drawing.Color.White;
            this.txtStatus.Location = new System.Drawing.Point(193, 313);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(0);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(283, 18);
            this.txtStatus.TabIndex = 9;
            this.txtStatus.Text = "status: ";
            // 
            // txtFoundAddress
            // 
            this.txtFoundAddress.Location = new System.Drawing.Point(385, 45);
            this.txtFoundAddress.Name = "txtFoundAddress";
            this.txtFoundAddress.Size = new System.Drawing.Size(83, 20);
            this.txtFoundAddress.TabIndex = 15;
            this.txtFoundAddress.Visible = false;
            // 
            // lblTrash
            // 
            this.lblTrash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTrash.AutoSize = true;
            this.lblTrash.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTrash.ForeColor = System.Drawing.Color.White;
            this.lblTrash.Location = new System.Drawing.Point(463, 442);
            this.lblTrash.Name = "lblTrash";
            this.lblTrash.Size = new System.Drawing.Size(19, 13);
            this.lblTrash.TabIndex = 17;
            this.lblTrash.Text = "🗑️";
            this.lblTrash.Click += new System.EventHandler(this.lblTrash_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(13, 93);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(463, 199);
            this.flowLayoutPanel1.TabIndex = 18;
            // 
            // btnCalculate
            // 
            this.btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCalculate.BackColor = System.Drawing.Color.White;
            this.btnCalculate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalculate.ForeColor = System.Drawing.Color.Black;
            this.btnCalculate.Location = new System.Drawing.Point(13, 345);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(174, 30);
            this.btnCalculate.TabIndex = 19;
            this.btnCalculate.Text = "calculate results";
            this.btnCalculate.UseVisualStyleBackColor = false;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // btnValueTotal
            // 
            this.btnValueTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnValueTotal.BackColor = System.Drawing.Color.White;
            this.btnValueTotal.ForeColor = System.Drawing.Color.Black;
            this.btnValueTotal.Location = new System.Drawing.Point(13, 310);
            this.btnValueTotal.Name = "btnValueTotal";
            this.btnValueTotal.Size = new System.Drawing.Size(75, 23);
            this.btnValueTotal.TabIndex = 20;
            this.btnValueTotal.Text = "show values";
            this.btnValueTotal.UseVisualStyleBackColor = false;
            this.btnValueTotal.Click += new System.EventHandler(this.btnValueTotal_Click);
            // 
            // btnShowAllOrGated
            // 
            this.btnShowAllOrGated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowAllOrGated.BackColor = System.Drawing.Color.White;
            this.btnShowAllOrGated.ForeColor = System.Drawing.Color.Black;
            this.btnShowAllOrGated.Location = new System.Drawing.Point(112, 310);
            this.btnShowAllOrGated.Name = "btnShowAllOrGated";
            this.btnShowAllOrGated.Size = new System.Drawing.Size(75, 23);
            this.btnShowAllOrGated.TabIndex = 21;
            this.btnShowAllOrGated.Text = "show gated";
            this.btnShowAllOrGated.UseVisualStyleBackColor = false;
            this.btnShowAllOrGated.Visible = false;
            this.btnShowAllOrGated.Click += new System.EventHandler(this.btnShowAllOrGated_Click);
            // 
            // lblVotesOrValue
            // 
            this.lblVotesOrValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVotesOrValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVotesOrValue.ForeColor = System.Drawing.Color.White;
            this.lblVotesOrValue.Location = new System.Drawing.Point(208, 349);
            this.lblVotesOrValue.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblVotesOrValue.Name = "lblVotesOrValue";
            this.lblVotesOrValue.Size = new System.Drawing.Size(242, 26);
            this.lblVotesOrValue.TabIndex = 22;
            this.lblVotesOrValue.Text = "Total Votes: 0";
            this.lblVotesOrValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCreatedBy
            // 
            this.txtCreatedBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtCreatedBy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtCreatedBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCreatedBy.ForeColor = System.Drawing.Color.White;
            this.txtCreatedBy.Location = new System.Drawing.Point(10, 386);
            this.txtCreatedBy.Margin = new System.Windows.Forms.Padding(0);
            this.txtCreatedBy.Name = "txtCreatedBy";
            this.txtCreatedBy.Size = new System.Drawing.Size(480, 18);
            this.txtCreatedBy.TabIndex = 24;
            this.txtCreatedBy.Text = "created by:";
            // 
            // txtCreatedDate
            // 
            this.txtCreatedDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtCreatedDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtCreatedDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCreatedDate.ForeColor = System.Drawing.Color.White;
            this.txtCreatedDate.Location = new System.Drawing.Point(10, 408);
            this.txtCreatedDate.Margin = new System.Windows.Forms.Padding(0);
            this.txtCreatedDate.Name = "txtCreatedDate";
            this.txtCreatedDate.Size = new System.Drawing.Size(458, 18);
            this.txtCreatedDate.TabIndex = 25;
            this.txtCreatedDate.Text = "created date:";
            // 
            // txtObjectAddress
            // 
            this.txtObjectAddress.BackColor = System.Drawing.Color.White;
            this.txtObjectAddress.ForeColor = System.Drawing.Color.Black;
            this.txtObjectAddress.Location = new System.Drawing.Point(368, 385);
            this.txtObjectAddress.Name = "txtObjectAddress";
            this.txtObjectAddress.Size = new System.Drawing.Size(100, 20);
            this.txtObjectAddress.TabIndex = 26;
            this.txtObjectAddress.Visible = false;
            // 
            // FoundINQControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.txtObjectAddress);
            this.Controls.Add(this.txtCreatedDate);
            this.Controls.Add(this.txtCreatedBy);
            this.Controls.Add(this.lblVotesOrValue);
            this.Controls.Add(this.btnShowAllOrGated);
            this.Controls.Add(this.btnValueTotal);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.lblTrash);
            this.Controls.Add(this.txtTransactionId);
            this.Controls.Add(this.txtQUE);
            this.Controls.Add(this.txtFoundAddress);
            this.Controls.Add(this.txtStatus);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(5, 7, 8, 7);
            this.MinimumSize = new System.Drawing.Size(221, 221);
            this.Name = "FoundINQControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(489, 469);
            this.Load += new System.EventHandler(this.FoundINQControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label txtQUE;
        public System.Windows.Forms.Label txtTransactionId;
        public System.Windows.Forms.Label txtStatus;
        public System.Windows.Forms.TextBox txtFoundAddress;
        public System.Windows.Forms.Label lblTrash;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnValueTotal;
        private System.Windows.Forms.Button btnShowAllOrGated;
        private System.Windows.Forms.Label lblVotesOrValue;
        public System.Windows.Forms.Label txtCreatedBy;
        public System.Windows.Forms.Label txtCreatedDate;
        private System.Windows.Forms.TextBox txtObjectAddress;
    }
}
