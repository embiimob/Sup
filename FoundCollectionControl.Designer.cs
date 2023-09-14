namespace SUP
{
    partial class FoundCollectionControl
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
            this.ObjectDescription = new System.Windows.Forms.Label();
            this.ObjectQty = new System.Windows.Forms.Label();
            this.ObjectAddress = new System.Windows.Forms.Label();
            this.ObjectName = new System.Windows.Forms.Label();
            this.txtOfficialURN = new System.Windows.Forms.TextBox();
            this.lblTrash = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ObjectImage)).BeginInit();
            this.SuspendLayout();
            // 
            // ObjectImage
            // 
            this.ObjectImage.BackColor = System.Drawing.Color.Gray;
            this.ObjectImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ObjectImage.ImageLocation = "";
            this.ObjectImage.InitialImage = null;
            this.ObjectImage.Location = new System.Drawing.Point(4, 5);
            this.ObjectImage.Margin = new System.Windows.Forms.Padding(0);
            this.ObjectImage.Name = "ObjectImage";
            this.ObjectImage.Size = new System.Drawing.Size(260, 260);
            this.ObjectImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ObjectImage.TabIndex = 0;
            this.ObjectImage.TabStop = false;
            this.ObjectImage.Click += new System.EventHandler(this.foundObjectControl_Click);
            // 
            // ObjectDescription
            // 
            this.ObjectDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectDescription.ForeColor = System.Drawing.Color.White;
            this.ObjectDescription.Location = new System.Drawing.Point(8, 305);
            this.ObjectDescription.Name = "ObjectDescription";
            this.ObjectDescription.Size = new System.Drawing.Size(256, 95);
            this.ObjectDescription.TabIndex = 6;
            this.ObjectDescription.Text = "🦁🐺💃🐭🧚🦬🦉🐍🧞‍♀️☄️🐝🕷🐟🐋🐴🦤\r\n🐒🧚‍♀️🧞🦹‍♂️🦈🦃🐿🐲🐅🐠🍄🐚🧚‍♂️🌵🐊🌱\r\n😈" +
    "🍀🌎🧞🐢🪐🦦✨☀️🕳️🌊❄️ 🔥🤖👽🦐\r\n🐙 🧚🧞‍♂️🐜 🦝👼💃👻🔴💙💃🌚👶🕺🏽🌸⚡";
            this.ObjectDescription.Click += new System.EventHandler(this.foundObjectControl_Click);
            // 
            // ObjectQty
            // 
            this.ObjectQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectQty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectQty.ForeColor = System.Drawing.Color.White;
            this.ObjectQty.Location = new System.Drawing.Point(179, 272);
            this.ObjectQty.Name = "ObjectQty";
            this.ObjectQty.Size = new System.Drawing.Size(78, 20);
            this.ObjectQty.TabIndex = 7;
            this.ObjectQty.Text = "10000x";
            this.ObjectQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ObjectQty.Click += new System.EventHandler(this.foundObjectControl_Click);
            // 
            // ObjectAddress
            // 
            this.ObjectAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.75F);
            this.ObjectAddress.ForeColor = System.Drawing.Color.White;
            this.ObjectAddress.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ObjectAddress.Location = new System.Drawing.Point(5, 415);
            this.ObjectAddress.Name = "ObjectAddress";
            this.ObjectAddress.Size = new System.Drawing.Size(258, 21);
            this.ObjectAddress.TabIndex = 8;
            this.ObjectAddress.Text = "@";
            this.ObjectAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ObjectAddress.Click += new System.EventHandler(this.ObjectAddress_Click);
            // 
            // ObjectName
            // 
            this.ObjectName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectName.ForeColor = System.Drawing.Color.White;
            this.ObjectName.Location = new System.Drawing.Point(8, 275);
            this.ObjectName.Margin = new System.Windows.Forms.Padding(0);
            this.ObjectName.Name = "ObjectName";
            this.ObjectName.Size = new System.Drawing.Size(122, 18);
            this.ObjectName.TabIndex = 9;
            this.ObjectName.Text = "FakeUFO Keywords this hoy sdgfd";
            this.ObjectName.Click += new System.EventHandler(this.foundObjectControl_Click);
            // 
            // txtOfficialURN
            // 
            this.txtOfficialURN.Location = new System.Drawing.Point(4, 182);
            this.txtOfficialURN.Name = "txtOfficialURN";
            this.txtOfficialURN.Size = new System.Drawing.Size(83, 20);
            this.txtOfficialURN.TabIndex = 15;
            this.txtOfficialURN.Visible = false;
            // 
            // lblTrash
            // 
            this.lblTrash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTrash.AutoSize = true;
            this.lblTrash.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTrash.ForeColor = System.Drawing.Color.White;
            this.lblTrash.Location = new System.Drawing.Point(248, 414);
            this.lblTrash.Name = "lblTrash";
            this.lblTrash.Size = new System.Drawing.Size(19, 13);
            this.lblTrash.TabIndex = 17;
            this.lblTrash.Text = "🗑️";
            this.lblTrash.Click += new System.EventHandler(this.lblTrash_Click);
            // 
            // FoundCollectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.lblTrash);
            this.Controls.Add(this.ObjectImage);
            this.Controls.Add(this.ObjectAddress);
            this.Controls.Add(this.ObjectDescription);
            this.Controls.Add(this.txtOfficialURN);
            this.Controls.Add(this.ObjectName);
            this.Controls.Add(this.ObjectQty);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.MinimumSize = new System.Drawing.Size(221, 221);
            this.Name = "FoundCollectionControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(270, 436);
            this.Click += new System.EventHandler(this.foundObjectControl_Click);
            ((System.ComponentModel.ISupportInitialize)(this.ObjectImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label ObjectDescription;
        public System.Windows.Forms.Label ObjectQty;
        public System.Windows.Forms.PictureBox ObjectImage;
        public System.Windows.Forms.Label ObjectAddress;
        public System.Windows.Forms.Label ObjectName;
        public System.Windows.Forms.TextBox txtOfficialURN;
        public System.Windows.Forms.Label lblTrash;
    }
}
