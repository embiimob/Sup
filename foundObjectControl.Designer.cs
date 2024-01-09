namespace SUP
{
    partial class FoundObjectControl
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
            this.ObjectImage = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ObjectPrice = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ObjectOffer = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ObjectDescription = new System.Windows.Forms.Label();
            this.ObjectQty = new System.Windows.Forms.Label();
            this.ObjectAddress = new System.Windows.Forms.Label();
            this.ObjectName = new System.Windows.Forms.Label();
            this.ObjectCreators = new System.Windows.Forms.LinkLabel();
            this.ObjectCreators2 = new System.Windows.Forms.LinkLabel();
            this.ObjectId = new System.Windows.Forms.TextBox();
            this.btnOfficial = new System.Windows.Forms.Button();
            this.txtOfficialURN = new System.Windows.Forms.TextBox();
            this.lblOfficial = new System.Windows.Forms.Label();
            this.lblTrash = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ObjectImage)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ObjectImage
            // 
            this.ObjectImage.BackColor = System.Drawing.Color.Gray;
            this.ObjectImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ObjectImage.ImageLocation = "";
            this.ObjectImage.InitialImage = null;
            this.ObjectImage.Location = new System.Drawing.Point(7, 5);
            this.ObjectImage.Margin = new System.Windows.Forms.Padding(0);
            this.ObjectImage.Name = "ObjectImage";
            this.ObjectImage.Size = new System.Drawing.Size(208, 208);
            this.ObjectImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ObjectImage.TabIndex = 0;
            this.ObjectImage.TabStop = false;
            this.ObjectImage.Click += new System.EventHandler(this.foundObjectControl_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ObjectPrice);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.ObjectOffer);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(6, 292);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(209, 40);
            this.panel1.TabIndex = 5;
            this.panel1.Click += new System.EventHandler(this.foundListings_Click);
            // 
            // ObjectPrice
            // 
            this.ObjectPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ObjectPrice.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ObjectPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectPrice.Location = new System.Drawing.Point(2, 19);
            this.ObjectPrice.Name = "ObjectPrice";
            this.ObjectPrice.Size = new System.Drawing.Size(92, 20);
            this.ObjectPrice.TabIndex = 9;
            this.ObjectPrice.Text = "-";
            this.ObjectPrice.Click += new System.EventHandler(this.foundListings_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(172, -1);
            this.label4.MaximumSize = new System.Drawing.Size(170, 0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label4.Size = new System.Drawing.Size(33, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "offer";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label4.UseMnemonic = false;
            this.label4.Click += new System.EventHandler(this.foundListings_Click);
            // 
            // ObjectOffer
            // 
            this.ObjectOffer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectOffer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectOffer.Location = new System.Drawing.Point(109, 19);
            this.ObjectOffer.Name = "ObjectOffer";
            this.ObjectOffer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ObjectOffer.Size = new System.Drawing.Size(95, 20);
            this.ObjectOffer.TabIndex = 8;
            this.ObjectOffer.Text = "-";
            this.ObjectOffer.Click += new System.EventHandler(this.foundListings_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, -2);
            this.label3.MaximumSize = new System.Drawing.Size(170, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label3.Size = new System.Drawing.Size(37, 19);
            this.label3.TabIndex = 6;
            this.label3.Text = "price";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label3.UseMnemonic = false;
            this.label3.Click += new System.EventHandler(this.foundListings_Click);
            // 
            // ObjectDescription
            // 
            this.ObjectDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectDescription.ForeColor = System.Drawing.Color.White;
            this.ObjectDescription.Location = new System.Drawing.Point(4, 280);
            this.ObjectDescription.Name = "ObjectDescription";
            this.ObjectDescription.Size = new System.Drawing.Size(211, 41);
            this.ObjectDescription.TabIndex = 6;
            this.ObjectDescription.Text = "a collection of items found on a UFO";
            this.ObjectDescription.Click += new System.EventHandler(this.foundObjectControl_Click);
            // 
            // ObjectQty
            // 
            this.ObjectQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectQty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectQty.ForeColor = System.Drawing.Color.White;
            this.ObjectQty.Location = new System.Drawing.Point(118, 218);
            this.ObjectQty.Margin = new System.Windows.Forms.Padding(0);
            this.ObjectQty.Name = "ObjectQty";
            this.ObjectQty.Size = new System.Drawing.Size(99, 33);
            this.ObjectQty.TabIndex = 7;
            this.ObjectQty.Text = "100X";
            this.ObjectQty.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.ObjectQty.TextChanged += new System.EventHandler(this.ObjectQty_TextChanged);
            this.ObjectQty.Click += new System.EventHandler(this.foundObjectControl_Click);
            // 
            // ObjectAddress
            // 
            this.ObjectAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.75F);
            this.ObjectAddress.ForeColor = System.Drawing.Color.White;
            this.ObjectAddress.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ObjectAddress.Location = new System.Drawing.Point(6, 315);
            this.ObjectAddress.Name = "ObjectAddress";
            this.ObjectAddress.Size = new System.Drawing.Size(209, 31);
            this.ObjectAddress.TabIndex = 8;
            this.ObjectAddress.Text = "@";
            this.ObjectAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ObjectAddress.Click += new System.EventHandler(this.ObjectAddress_Click);
            // 
            // ObjectName
            // 
            this.ObjectName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectName.ForeColor = System.Drawing.Color.White;
            this.ObjectName.Location = new System.Drawing.Point(5, 216);
            this.ObjectName.Margin = new System.Windows.Forms.Padding(0);
            this.ObjectName.Name = "ObjectName";
            this.ObjectName.Size = new System.Drawing.Size(117, 31);
            this.ObjectName.TabIndex = 9;
            this.ObjectName.Text = "Fake UFO";
            this.ObjectName.Click += new System.EventHandler(this.foundObjectControl_Click);
            // 
            // ObjectCreators
            // 
            this.ObjectCreators.AutoSize = true;
            this.ObjectCreators.BackColor = System.Drawing.Color.Transparent;
            this.ObjectCreators.LinkColor = System.Drawing.SystemColors.Highlight;
            this.ObjectCreators.Location = new System.Drawing.Point(7, 247);
            this.ObjectCreators.Name = "ObjectCreators";
            this.ObjectCreators.Size = new System.Drawing.Size(0, 13);
            this.ObjectCreators.TabIndex = 10;
            this.ObjectCreators.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ObjectCreators.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ObjectCreators_LinkClicked);
            // 
            // ObjectCreators2
            // 
            this.ObjectCreators2.AutoSize = true;
            this.ObjectCreators2.BackColor = System.Drawing.Color.Transparent;
            this.ObjectCreators2.LinkColor = System.Drawing.SystemColors.Highlight;
            this.ObjectCreators2.Location = new System.Drawing.Point(7, 262);
            this.ObjectCreators2.Name = "ObjectCreators2";
            this.ObjectCreators2.Size = new System.Drawing.Size(0, 13);
            this.ObjectCreators2.TabIndex = 11;
            this.ObjectCreators2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ObjectCreators2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ObjectCreators2_LinkClicked);
            // 
            // ObjectId
            // 
            this.ObjectId.Location = new System.Drawing.Point(45, 86);
            this.ObjectId.Name = "ObjectId";
            this.ObjectId.Size = new System.Drawing.Size(79, 20);
            this.ObjectId.TabIndex = 13;
            // 
            // btnOfficial
            // 
            this.btnOfficial.BackColor = System.Drawing.Color.Yellow;
            this.btnOfficial.Location = new System.Drawing.Point(67, 172);
            this.btnOfficial.Name = "btnOfficial";
            this.btnOfficial.Size = new System.Drawing.Size(134, 28);
            this.btnOfficial.TabIndex = 14;
            this.btnOfficial.Text = "SEE OFFICIAL URN";
            this.btnOfficial.UseVisualStyleBackColor = false;
            this.btnOfficial.Visible = false;
            this.btnOfficial.Click += new System.EventHandler(this.btnOfficial_Click);
            // 
            // txtOfficialURN
            // 
            this.txtOfficialURN.Location = new System.Drawing.Point(4, 182);
            this.txtOfficialURN.Name = "txtOfficialURN";
            this.txtOfficialURN.Size = new System.Drawing.Size(83, 20);
            this.txtOfficialURN.TabIndex = 15;
            this.txtOfficialURN.Visible = false;
            // 
            // lblOfficial
            // 
            this.lblOfficial.AutoSize = true;
            this.lblOfficial.BackColor = System.Drawing.SystemColors.HotTrack;
            this.lblOfficial.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOfficial.ForeColor = System.Drawing.Color.Yellow;
            this.lblOfficial.Location = new System.Drawing.Point(0, 0);
            this.lblOfficial.Margin = new System.Windows.Forms.Padding(0);
            this.lblOfficial.Name = "lblOfficial";
            this.lblOfficial.Padding = new System.Windows.Forms.Padding(1, 1, 1, 3);
            this.lblOfficial.Size = new System.Drawing.Size(29, 22);
            this.lblOfficial.TabIndex = 16;
            this.lblOfficial.Text = "👑";
            this.lblOfficial.Visible = false;
            // 
            // lblTrash
            // 
            this.lblTrash.AutoSize = true;
            this.lblTrash.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTrash.ForeColor = System.Drawing.Color.White;
            this.lblTrash.Location = new System.Drawing.Point(201, 321);
            this.lblTrash.Name = "lblTrash";
            this.lblTrash.Size = new System.Drawing.Size(19, 13);
            this.lblTrash.TabIndex = 17;
            this.lblTrash.Text = "🗑️";
            this.lblTrash.Click += new System.EventHandler(this.lblTrash_Click);
            // 
            // FoundObjectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.ObjectQty);
            this.Controls.Add(this.ObjectCreators);
            this.Controls.Add(this.lblTrash);
            this.Controls.Add(this.ObjectDescription);
            this.Controls.Add(this.ObjectCreators2);
            this.Controls.Add(this.btnOfficial);
            this.Controls.Add(this.lblOfficial);
            this.Controls.Add(this.ObjectImage);
            this.Controls.Add(this.ObjectAddress);
            this.Controls.Add(this.txtOfficialURN);
            this.Controls.Add(this.ObjectName);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ObjectId);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.MinimumSize = new System.Drawing.Size(221, 221);
            this.Name = "FoundObjectControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(221, 341);
            this.Click += new System.EventHandler(this.foundObjectControl_Click);
            ((System.ComponentModel.ISupportInitialize)(this.ObjectImage)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label ObjectPrice;
        public System.Windows.Forms.Label ObjectOffer;
        public System.Windows.Forms.Label ObjectDescription;
        public System.Windows.Forms.Label ObjectQty;
        public System.Windows.Forms.PictureBox ObjectImage;
        public System.Windows.Forms.Label ObjectAddress;
        public System.Windows.Forms.Label ObjectName;
        public System.Windows.Forms.LinkLabel ObjectCreators;
        public System.Windows.Forms.LinkLabel ObjectCreators2;
        public System.Windows.Forms.TextBox ObjectId;
        public System.Windows.Forms.Button btnOfficial;
        public System.Windows.Forms.TextBox txtOfficialURN;
        public System.Windows.Forms.Label lblOfficial;
        public System.Windows.Forms.Label lblTrash;
    }
}
