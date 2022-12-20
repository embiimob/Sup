namespace SUP
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.txtPutKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtGetKey = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDeleteKey = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPutValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtGetValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(45, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Put";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(45, 124);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "get";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(45, 190);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "delete";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // txtPutKey
            // 
            this.txtPutKey.Location = new System.Drawing.Point(264, 66);
            this.txtPutKey.Name = "txtPutKey";
            this.txtPutKey.Size = new System.Drawing.Size(100, 20);
            this.txtPutKey.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(264, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Key";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(264, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "key";
            // 
            // txtGetKey
            // 
            this.txtGetKey.Location = new System.Drawing.Point(264, 126);
            this.txtGetKey.Name = "txtGetKey";
            this.txtGetKey.Size = new System.Drawing.Size(100, 20);
            this.txtGetKey.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(264, 171);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "key";
            // 
            // txtDeleteKey
            // 
            this.txtDeleteKey.Location = new System.Drawing.Point(264, 190);
            this.txtDeleteKey.Name = "txtDeleteKey";
            this.txtDeleteKey.Size = new System.Drawing.Size(100, 20);
            this.txtDeleteKey.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(427, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Value";
            // 
            // txtPutValue
            // 
            this.txtPutValue.Location = new System.Drawing.Point(427, 67);
            this.txtPutValue.Name = "txtPutValue";
            this.txtPutValue.Size = new System.Drawing.Size(100, 20);
            this.txtPutValue.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(430, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "value";
            // 
            // txtGetValue
            // 
            this.txtGetValue.Location = new System.Drawing.Point(430, 127);
            this.txtGetValue.Name = "txtGetValue";
            this.txtGetValue.Size = new System.Drawing.Size(100, 20);
            this.txtGetValue.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtGetValue);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPutValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDeleteKey);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtGetKey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPutKey);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtPutKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtGetKey;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDeleteKey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPutValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtGetValue;
    }
}

