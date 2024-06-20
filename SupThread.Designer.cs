namespace SUP
{
    partial class SupThread
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
            this.supFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // supFlow
            // 
            this.supFlow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.supFlow.AutoScroll = true;
            this.supFlow.BackColor = System.Drawing.Color.Black;
            this.supFlow.ForeColor = System.Drawing.Color.White;
            this.supFlow.Location = new System.Drawing.Point(12, 12);
            this.supFlow.Name = "supFlow";
            this.supFlow.Size = new System.Drawing.Size(573, 750);
            this.supFlow.TabIndex = 0;
            // 
            // SupThread
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(584, 761);
            this.Controls.Add(this.supFlow);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "SupThread";
            this.Text = "SupThread";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel supFlow;
    }
}