namespace SUP
{
    partial class Form2
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
            this.txtVersionByte = new System.Windows.Forms.TextBox();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnGetCreated = new System.Windows.Forms.Button();
            this.btnGetOwned = new System.Windows.Forms.Button();
            this.btnGetObject = new System.Windows.Forms.Button();
            this.txtSearchAddress = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // txtVersionByte
            // 
            this.txtVersionByte.Location = new System.Drawing.Point(886, 7);
            this.txtVersionByte.Name = "txtVersionByte";
            this.txtVersionByte.Size = new System.Drawing.Size(36, 20);
            this.txtVersionByte.TabIndex = 49;
            this.txtVersionByte.Text = "111";
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(600, 7);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(57, 20);
            this.txtLogin.TabIndex = 41;
            this.txtLogin.Text = "good-user";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(663, 8);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(85, 20);
            this.txtPassword.TabIndex = 43;
            this.txtPassword.Text = "better-password";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(754, 7);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(126, 20);
            this.txtUrl.TabIndex = 45;
            this.txtUrl.Text = "http://127.0.0.1:18332";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.Control;
            this.button3.Location = new System.Drawing.Point(552, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(42, 20);
            this.button3.TabIndex = 66;
            this.button3.Text = "URN";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Control;
            this.button2.Location = new System.Drawing.Point(477, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(69, 20);
            this.button2.TabIndex = 64;
            this.button2.Text = "by keyword";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(302, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(53, 20);
            this.button1.TabIndex = 63;
            this.button1.Text = "objects";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnGetCreated
            // 
            this.btnGetCreated.BackColor = System.Drawing.SystemColors.Control;
            this.btnGetCreated.Location = new System.Drawing.Point(416, 5);
            this.btnGetCreated.Name = "btnGetCreated";
            this.btnGetCreated.Size = new System.Drawing.Size(55, 20);
            this.btnGetCreated.TabIndex = 62;
            this.btnGetCreated.Text = "created";
            this.btnGetCreated.UseVisualStyleBackColor = false;
            this.btnGetCreated.Click += new System.EventHandler(this.btnGetCreated_Click);
            // 
            // btnGetOwned
            // 
            this.btnGetOwned.BackColor = System.Drawing.SystemColors.Control;
            this.btnGetOwned.Location = new System.Drawing.Point(361, 5);
            this.btnGetOwned.Name = "btnGetOwned";
            this.btnGetOwned.Size = new System.Drawing.Size(49, 20);
            this.btnGetOwned.TabIndex = 61;
            this.btnGetOwned.Text = "owned";
            this.btnGetOwned.UseVisualStyleBackColor = false;
            this.btnGetOwned.Click += new System.EventHandler(this.btnGetOwned_Click);
            // 
            // btnGetObject
            // 
            this.btnGetObject.BackColor = System.Drawing.SystemColors.Control;
            this.btnGetObject.Location = new System.Drawing.Point(248, 5);
            this.btnGetObject.Name = "btnGetObject";
            this.btnGetObject.Size = new System.Drawing.Size(48, 20);
            this.btnGetObject.TabIndex = 60;
            this.btnGetObject.Text = "object";
            this.btnGetObject.UseVisualStyleBackColor = false;
            this.btnGetObject.Click += new System.EventHandler(this.btnGetObject_Click);
            // 
            // txtSearchAddress
            // 
            this.txtSearchAddress.Location = new System.Drawing.Point(2, 5);
            this.txtSearchAddress.Name = "txtSearchAddress";
            this.txtSearchAddress.Size = new System.Drawing.Size(240, 20);
            this.txtSearchAddress.TabIndex = 58;
            this.txtSearchAddress.Text = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs";
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.Control;
            this.button4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button4.Location = new System.Drawing.Point(0, 459);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(936, 22);
            this.button4.TabIndex = 67;
            this.button4.Text = "open workbench";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(936, 481);
            this.flowLayoutPanel1.TabIndex = 68;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 481);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGetCreated);
            this.Controls.Add(this.btnGetOwned);
            this.Controls.Add(this.btnGetObject);
            this.Controls.Add(this.txtSearchAddress);
            this.Controls.Add(this.txtVersionByte);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.flowLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(952, 520);
            this.Name = "Form2";
            this.Text = "Sup!? Object Browser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtVersionByte;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnGetCreated;
        private System.Windows.Forms.Button btnGetOwned;
        private System.Windows.Forms.Button btnGetObject;
        private System.Windows.Forms.TextBox txtSearchAddress;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}