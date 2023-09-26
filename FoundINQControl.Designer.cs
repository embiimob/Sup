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
            this.ObjectDescription = new System.Windows.Forms.Label();
            this.ObjectAddress = new System.Windows.Forms.Label();
            this.ObjectName = new System.Windows.Forms.Label();
            this.txtFoundAddress = new System.Windows.Forms.TextBox();
            this.lblTrash = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.lblAnswer = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblAnswerTotal = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBar3 = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ObjectDescription
            // 
            this.ObjectDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectDescription.ForeColor = System.Drawing.Color.White;
            this.ObjectDescription.Location = new System.Drawing.Point(0, 0);
            this.ObjectDescription.Name = "ObjectDescription";
            this.ObjectDescription.Size = new System.Drawing.Size(497, 90);
            this.ObjectDescription.TabIndex = 6;
            this.ObjectDescription.Text = "What is your Favourite Color?";
            this.ObjectDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ObjectDescription.Click += new System.EventHandler(this.foundObjectControl_Click);
            // 
            // ObjectAddress
            // 
            this.ObjectAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.75F);
            this.ObjectAddress.ForeColor = System.Drawing.Color.White;
            this.ObjectAddress.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ObjectAddress.Location = new System.Drawing.Point(5, 403);
            this.ObjectAddress.Name = "ObjectAddress";
            this.ObjectAddress.Size = new System.Drawing.Size(229, 21);
            this.ObjectAddress.TabIndex = 8;
            this.ObjectAddress.Text = "@";
            this.ObjectAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ObjectAddress.Click += new System.EventHandler(this.ObjectAddress_Click);
            // 
            // ObjectName
            // 
            this.ObjectName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ObjectName.ForeColor = System.Drawing.Color.White;
            this.ObjectName.Location = new System.Drawing.Point(11, 343);
            this.ObjectName.Margin = new System.Windows.Forms.Padding(0);
            this.ObjectName.Name = "ObjectName";
            this.ObjectName.Size = new System.Drawing.Size(237, 18);
            this.ObjectName.TabIndex = 9;
            this.ObjectName.Text = "status: closes in 24 hr 59 min";
            this.ObjectName.Click += new System.EventHandler(this.foundObjectControl_Click);
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
            this.lblTrash.Location = new System.Drawing.Point(478, 402);
            this.lblTrash.Name = "lblTrash";
            this.lblTrash.Size = new System.Drawing.Size(19, 13);
            this.lblTrash.TabIndex = 17;
            this.lblTrash.Text = "🗑️";
            this.lblTrash.Click += new System.EventHandler(this.lblTrash_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.lblAnswer);
            this.flowLayoutPanel1.Controls.Add(this.progressBar1);
            this.flowLayoutPanel1.Controls.Add(this.lblAnswerTotal);
            this.flowLayoutPanel1.Controls.Add(this.button6);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.progressBar2);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.button7);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.progressBar3);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.button8);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 93);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(500, 200);
            this.flowLayoutPanel1.TabIndex = 18;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(331, 343);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(156, 23);
            this.button1.TabIndex = 19;
            this.button1.Text = "calculate results";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lblAnswer
            // 
            this.lblAnswer.AutoSize = true;
            this.lblAnswer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAnswer.ForeColor = System.Drawing.Color.White;
            this.lblAnswer.Location = new System.Drawing.Point(3, 10);
            this.lblAnswer.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblAnswer.MaximumSize = new System.Drawing.Size(440, 420);
            this.lblAnswer.MinimumSize = new System.Drawing.Size(440, 25);
            this.lblAnswer.Name = "lblAnswer";
            this.lblAnswer.Size = new System.Drawing.Size(440, 25);
            this.lblAnswer.TabIndex = 0;
            this.lblAnswer.Text = "Red";
            this.lblAnswer.Click += new System.EventHandler(this.lblAnswer_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(3, 38);
            this.progressBar1.Maximum = 3;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(314, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 1;
            this.progressBar1.Value = 1;
            // 
            // lblAnswerTotal
            // 
            this.lblAnswerTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAnswerTotal.ForeColor = System.Drawing.Color.White;
            this.lblAnswerTotal.Location = new System.Drawing.Point(323, 38);
            this.lblAnswerTotal.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblAnswerTotal.Name = "lblAnswerTotal";
            this.lblAnswerTotal.Size = new System.Drawing.Size(106, 26);
            this.lblAnswerTotal.TabIndex = 2;
            this.lblAnswerTotal.Text = "1";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 74);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(449, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "Blue";
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(3, 100);
            this.progressBar2.Maximum = 3;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(314, 23);
            this.progressBar2.Step = 1;
            this.progressBar2.TabIndex = 4;
            this.progressBar2.Value = 1;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(323, 100);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 26);
            this.label2.TabIndex = 5;
            this.label2.Text = "1";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(3, 136);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(449, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "Green";
            // 
            // progressBar3
            // 
            this.progressBar3.Location = new System.Drawing.Point(3, 162);
            this.progressBar3.Maximum = 3;
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.Size = new System.Drawing.Size(314, 23);
            this.progressBar3.Step = 1;
            this.progressBar3.TabIndex = 7;
            this.progressBar3.Value = 1;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(323, 162);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 26);
            this.label4.TabIndex = 8;
            this.label4.Text = "1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(85, 310);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 20;
            this.button2.Text = "show values";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(4, 310);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 21;
            this.button3.Text = "show all";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(291, 310);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(196, 26);
            this.label5.TabIndex = 22;
            this.label5.Text = "Total Votes: 3";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(173, 310);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 23;
            this.button4.Text = "show gates";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(435, 38);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(51, 23);
            this.button6.TabIndex = 25;
            this.button6.Text = "vote";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(435, 100);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(51, 23);
            this.button7.TabIndex = 26;
            this.button7.Text = "vote";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(435, 162);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(51, 23);
            this.button8.TabIndex = 27;
            this.button8.Text = "vote";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(7, 367);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(153, 18);
            this.label6.TabIndex = 24;
            this.label6.Text = "created by: embii4u";
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(8, 385);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(297, 18);
            this.label7.TabIndex = 25;
            this.label7.Text = "created date: 09/25/2023: 07:33 PM";
            // 
            // FoundINQControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.lblTrash);
            this.Controls.Add(this.ObjectAddress);
            this.Controls.Add(this.ObjectDescription);
            this.Controls.Add(this.txtFoundAddress);
            this.Controls.Add(this.ObjectName);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.MinimumSize = new System.Drawing.Size(221, 221);
            this.Name = "FoundINQControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(500, 424);
            this.Click += new System.EventHandler(this.foundObjectControl_Click);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label ObjectDescription;
        public System.Windows.Forms.Label ObjectAddress;
        public System.Windows.Forms.Label ObjectName;
        public System.Windows.Forms.TextBox txtFoundAddress;
        public System.Windows.Forms.Label lblTrash;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblAnswer;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblAnswerTotal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBar3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label7;
    }
}
