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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectDetails));
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.webviewer = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.button1 = new System.Windows.Forms.Button();
            this.lblImageFullPath = new System.Windows.Forms.Label();
            this.lblURNFullPath = new System.Windows.Forms.Label();
            this.OwnersPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CreatorsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTotalOwned = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.imgPicture = new System.Windows.Forms.PictureBox();
            this.txtdesc = new System.Windows.Forms.TextBox();
            this.lblurn = new System.Windows.Forms.TextBox();
            this.lblimg = new System.Windows.Forms.TextBox();
            this.lbluri = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.supPanel = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.Label();
            this.supFlow = new System.Windows.Forms.FlowLayoutPanel();
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
            this.flowPanel.Location = new System.Drawing.Point(314, 11);
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
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click_1);
            // 
            // webviewer
            // 
            this.webviewer.AllowExternalDrop = true;
            this.webviewer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.webviewer.CreationProperties = null;
            this.webviewer.DefaultBackgroundColor = System.Drawing.SystemColors.ControlDark;
            this.webviewer.Location = new System.Drawing.Point(314, 11);
            this.webviewer.Name = "webviewer";
            this.webviewer.Size = new System.Drawing.Size(600, 600);
            this.webviewer.TabIndex = 1;
            this.webviewer.ZoomFactor = 1D;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(866, 566);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(34, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "[  ]";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblImageFullPath
            // 
            this.lblImageFullPath.AutoSize = true;
            this.lblImageFullPath.Location = new System.Drawing.Point(341, 617);
            this.lblImageFullPath.Name = "lblImageFullPath";
            this.lblImageFullPath.Size = new System.Drawing.Size(59, 13);
            this.lblImageFullPath.TabIndex = 3;
            this.lblImageFullPath.Text = "image path";
            this.lblImageFullPath.Visible = false;
            // 
            // lblURNFullPath
            // 
            this.lblURNFullPath.AutoSize = true;
            this.lblURNFullPath.Location = new System.Drawing.Point(406, 617);
            this.lblURNFullPath.Name = "lblURNFullPath";
            this.lblURNFullPath.Size = new System.Drawing.Size(46, 13);
            this.lblURNFullPath.TabIndex = 4;
            this.lblURNFullPath.Text = "urn path";
            this.lblURNFullPath.Visible = false;
            // 
            // OwnersPanel
            // 
            this.OwnersPanel.AutoScroll = true;
            this.OwnersPanel.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.OwnersPanel.Location = new System.Drawing.Point(927, 28);
            this.OwnersPanel.Name = "OwnersPanel";
            this.OwnersPanel.Size = new System.Drawing.Size(272, 317);
            this.OwnersPanel.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(924, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "owners";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(923, 391);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "creators";
            // 
            // CreatorsPanel
            // 
            this.CreatorsPanel.AutoScroll = true;
            this.CreatorsPanel.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.CreatorsPanel.Location = new System.Drawing.Point(927, 407);
            this.CreatorsPanel.Name = "CreatorsPanel";
            this.CreatorsPanel.Size = new System.Drawing.Size(157, 150);
            this.CreatorsPanel.TabIndex = 7;
            // 
            // lblTotalOwned
            // 
            this.lblTotalOwned.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalOwned.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTotalOwned.Location = new System.Drawing.Point(930, 358);
            this.lblTotalOwned.Name = "lblTotalOwned";
            this.lblTotalOwned.Padding = new System.Windows.Forms.Padding(3);
            this.lblTotalOwned.Size = new System.Drawing.Size(269, 23);
            this.lblTotalOwned.TabIndex = 9;
            this.lblTotalOwned.Text = "total: 100";
            this.lblTotalOwned.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(1090, 407);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(103, 33);
            this.button2.TabIndex = 10;
            this.button2.Text = "give";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(1090, 524);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(103, 33);
            this.button4.TabIndex = 12;
            this.button4.Text = "burn";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // imgPicture
            // 
            this.imgPicture.Location = new System.Drawing.Point(6, 11);
            this.imgPicture.Name = "imgPicture";
            this.imgPicture.Size = new System.Drawing.Size(70, 70);
            this.imgPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgPicture.TabIndex = 15;
            this.imgPicture.TabStop = false;
            // 
            // txtdesc
            // 
            this.txtdesc.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtdesc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtdesc.Location = new System.Drawing.Point(6, 100);
            this.txtdesc.Multiline = true;
            this.txtdesc.Name = "txtdesc";
            this.txtdesc.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtdesc.Size = new System.Drawing.Size(298, 205);
            this.txtdesc.TabIndex = 22;
            this.txtdesc.Text = "Description";
            // 
            // lblurn
            // 
            this.lblurn.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblurn.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblurn.Location = new System.Drawing.Point(6, 380);
            this.lblurn.Multiline = true;
            this.lblurn.Name = "lblurn";
            this.lblurn.Size = new System.Drawing.Size(298, 34);
            this.lblurn.TabIndex = 24;
            // 
            // lblimg
            // 
            this.lblimg.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblimg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblimg.Location = new System.Drawing.Point(6, 417);
            this.lblimg.Multiline = true;
            this.lblimg.Name = "lblimg";
            this.lblimg.Size = new System.Drawing.Size(298, 33);
            this.lblimg.TabIndex = 26;
            // 
            // lbluri
            // 
            this.lbluri.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lbluri.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbluri.Location = new System.Drawing.Point(6, 456);
            this.lbluri.Multiline = true;
            this.lbluri.Name = "lbluri";
            this.lbluri.Size = new System.Drawing.Size(298, 33);
            this.lbluri.TabIndex = 27;
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(1090, 446);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(103, 33);
            this.button5.TabIndex = 28;
            this.button5.Text = "list";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(1090, 485);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(103, 33);
            this.button6.TabIndex = 29;
            this.button6.Text = "buy";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(1090, 572);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(103, 42);
            this.button3.TabIndex = 30;
            this.button3.Text = "📣";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // supPanel
            // 
            this.supPanel.Controls.Add(this.supFlow);
            this.supPanel.Controls.Add(this.button7);
            this.supPanel.Controls.Add(this.textBox1);
            this.supPanel.Location = new System.Drawing.Point(917, 6);
            this.supPanel.Name = "supPanel";
            this.supPanel.Size = new System.Drawing.Size(282, 624);
            this.supPanel.TabIndex = 31;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox1.Enabled = false;
            this.textBox1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox1.Location = new System.Drawing.Point(6, 381);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.textBox1.Size = new System.Drawing.Size(269, 181);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(173, 566);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(103, 42);
            this.button7.TabIndex = 2;
            this.button7.Text = "📣";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(934, 571);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 45);
            this.label3.TabIndex = 32;
            this.label3.Text = "👑";
            this.label3.Click += new System.EventHandler(this.btnShowObjectDetails_Click);
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(82, 14);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(222, 67);
            this.txtName.TabIndex = 33;
            this.txtName.Text = "Title";
            this.txtName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // supFlow
            // 
            this.supFlow.AutoScroll = true;
            this.supFlow.Location = new System.Drawing.Point(6, 5);
            this.supFlow.Name = "supFlow";
            this.supFlow.Size = new System.Drawing.Size(269, 370);
            this.supFlow.TabIndex = 3;
            // 
            // ObjectDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(1202, 623);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.supPanel);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.lbluri);
            this.Controls.Add(this.lblimg);
            this.Controls.Add(this.lblurn);
            this.Controls.Add(this.txtdesc);
            this.Controls.Add(this.imgPicture);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lblTotalOwned);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CreatorsPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.OwnersPanel);
            this.Controls.Add(this.lblURNFullPath);
            this.Controls.Add(this.lblImageFullPath);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.webviewer);
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
        private System.Windows.Forms.Label lblImageFullPath;
        private System.Windows.Forms.Label lblURNFullPath;
        private System.Windows.Forms.FlowLayoutPanel OwnersPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel CreatorsPanel;
        private System.Windows.Forms.Label lblTotalOwned;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.PictureBox imgPicture;
        private System.Windows.Forms.TextBox txtdesc;
        private System.Windows.Forms.TextBox lblurn;
        private System.Windows.Forms.TextBox lblimg;
        private System.Windows.Forms.TextBox lbluri;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel supPanel;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label txtName;
        private System.Windows.Forms.FlowLayoutPanel supFlow;
    }
}