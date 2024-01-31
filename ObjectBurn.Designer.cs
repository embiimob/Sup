namespace SUP
{
    partial class ObjectBurn
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.qtyTextBox = new System.Windows.Forms.TextBox();
            this.addressQtyDataGridView = new System.Windows.Forms.DataGridView();
            this.address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnBurn = new System.Windows.Forms.Button();
            this.txtOBJJSON = new System.Windows.Forms.TextBox();
            this.txtOBJP2FK = new System.Windows.Forms.TextBox();
            this.txtAddressListJSON = new System.Windows.Forms.TextBox();
            this.txtSignatureAddress = new System.Windows.Forms.TextBox();
            this.lblObjectStatus = new System.Windows.Forms.Label();
            this.lblCost = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.addressQtyDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // addressTextBox
            // 
            this.addressTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.addressTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressTextBox.ForeColor = System.Drawing.Color.White;
            this.addressTextBox.Location = new System.Drawing.Point(12, 19);
            this.addressTextBox.Multiline = true;
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(337, 33);
            this.addressTextBox.TabIndex = 0;
            // 
            // qtyTextBox
            // 
            this.qtyTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.qtyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qtyTextBox.ForeColor = System.Drawing.Color.White;
            this.qtyTextBox.Location = new System.Drawing.Point(362, 19);
            this.qtyTextBox.Multiline = true;
            this.qtyTextBox.Name = "qtyTextBox";
            this.qtyTextBox.Size = new System.Drawing.Size(84, 33);
            this.qtyTextBox.TabIndex = 1;
            // 
            // addressQtyDataGridView
            // 
            this.addressQtyDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.addressQtyDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.addressQtyDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.address,
            this.qty});
            this.addressQtyDataGridView.Location = new System.Drawing.Point(12, 84);
            this.addressQtyDataGridView.Name = "addressQtyDataGridView";
            this.addressQtyDataGridView.ReadOnly = true;
            this.addressQtyDataGridView.RowHeadersWidth = 51;
            this.addressQtyDataGridView.Size = new System.Drawing.Size(489, 405);
            this.addressQtyDataGridView.TabIndex = 2;
            // 
            // address
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.address.DefaultCellStyle = dataGridViewCellStyle1;
            this.address.HeaderText = "object address to burn";
            this.address.MaxInputLength = 46;
            this.address.MinimumWidth = 6;
            this.address.Name = "address";
            this.address.ReadOnly = true;
            this.address.Width = 325;
            // 
            // qty
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qty.DefaultCellStyle = dataGridViewCellStyle2;
            this.qty.HeaderText = "qty to burn";
            this.qty.MinimumWidth = 6;
            this.qty.Name = "qty";
            this.qty.ReadOnly = true;
            this.qty.Width = 111;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnSave.Location = new System.Drawing.Point(12, 581);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(109, 53);
            this.btnSave.TabIndex = 273;
            this.btnSave.Text = "💾";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Location = new System.Drawing.Point(452, 19);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(49, 33);
            this.btnAdd.TabIndex = 274;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.addButton_Click);
            // 
            // btnBurn
            // 
            this.btnBurn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBurn.Enabled = false;
            this.btnBurn.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnBurn.Location = new System.Drawing.Point(392, 580);
            this.btnBurn.Name = "btnBurn";
            this.btnBurn.Size = new System.Drawing.Size(109, 53);
            this.btnBurn.TabIndex = 275;
            this.btnBurn.Text = "🔥";
            this.btnBurn.UseVisualStyleBackColor = true;
            this.btnBurn.Click += new System.EventHandler(this.burnButton_Click);
            // 
            // txtOBJJSON
            // 
            this.txtOBJJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOBJJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOBJJSON.ForeColor = System.Drawing.Color.Black;
            this.txtOBJJSON.Location = new System.Drawing.Point(18, 140);
            this.txtOBJJSON.Multiline = true;
            this.txtOBJJSON.Name = "txtOBJJSON";
            this.txtOBJJSON.Size = new System.Drawing.Size(141, 304);
            this.txtOBJJSON.TabIndex = 359;
            this.txtOBJJSON.Visible = false;
            // 
            // txtOBJP2FK
            // 
            this.txtOBJP2FK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOBJP2FK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOBJP2FK.ForeColor = System.Drawing.Color.Black;
            this.txtOBJP2FK.Location = new System.Drawing.Point(165, 140);
            this.txtOBJP2FK.Multiline = true;
            this.txtOBJP2FK.Name = "txtOBJP2FK";
            this.txtOBJP2FK.Size = new System.Drawing.Size(156, 304);
            this.txtOBJP2FK.TabIndex = 360;
            this.txtOBJP2FK.Visible = false;
            // 
            // txtAddressListJSON
            // 
            this.txtAddressListJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtAddressListJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddressListJSON.ForeColor = System.Drawing.Color.Black;
            this.txtAddressListJSON.Location = new System.Drawing.Point(327, 140);
            this.txtAddressListJSON.Multiline = true;
            this.txtAddressListJSON.Name = "txtAddressListJSON";
            this.txtAddressListJSON.Size = new System.Drawing.Size(160, 304);
            this.txtAddressListJSON.TabIndex = 358;
            this.txtAddressListJSON.Visible = false;
            // 
            // txtSignatureAddress
            // 
            this.txtSignatureAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSignatureAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtSignatureAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.txtSignatureAddress.ForeColor = System.Drawing.Color.White;
            this.txtSignatureAddress.Location = new System.Drawing.Point(12, 521);
            this.txtSignatureAddress.Multiline = true;
            this.txtSignatureAddress.Name = "txtSignatureAddress";
            this.txtSignatureAddress.Size = new System.Drawing.Size(489, 53);
            this.txtSignatureAddress.TabIndex = 361;
            // 
            // lblObjectStatus
            // 
            this.lblObjectStatus.AutoSize = true;
            this.lblObjectStatus.ForeColor = System.Drawing.Color.White;
            this.lblObjectStatus.Location = new System.Drawing.Point(15, 61);
            this.lblObjectStatus.Name = "lblObjectStatus";
            this.lblObjectStatus.Size = new System.Drawing.Size(0, 13);
            this.lblObjectStatus.TabIndex = 363;
            // 
            // lblCost
            // 
            this.lblCost.AutoSize = true;
            this.lblCost.ForeColor = System.Drawing.Color.White;
            this.lblCost.Location = new System.Drawing.Point(215, 611);
            this.lblCost.Name = "lblCost";
            this.lblCost.Size = new System.Drawing.Size(0, 13);
            this.lblCost.TabIndex = 364;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(15, 505);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 370;
            this.label4.Text = "enter signature address";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 371;
            this.label1.Text = "enter object address to burn";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(359, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 372;
            this.label2.Text = "enter qty to burn and hit   +";
            // 
            // ObjectBurn
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(516, 646);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblCost);
            this.Controls.Add(this.lblObjectStatus);
            this.Controls.Add(this.txtSignatureAddress);
            this.Controls.Add(this.txtOBJJSON);
            this.Controls.Add(this.txtOBJP2FK);
            this.Controls.Add(this.txtAddressListJSON);
            this.Controls.Add(this.btnBurn);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.addressQtyDataGridView);
            this.Controls.Add(this.qtyTextBox);
            this.Controls.Add(this.addressTextBox);
            this.MaximumSize = new System.Drawing.Size(532, 685);
            this.MinimumSize = new System.Drawing.Size(532, 685);
            this.Name = "ObjectBurn";
            this.Text = "ObjectBurn";
            this.Load += new System.EventHandler(this.ObjectBurn_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.addressQtyDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.TextBox qtyTextBox;
        private System.Windows.Forms.DataGridView addressQtyDataGridView;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn address;
        private System.Windows.Forms.DataGridViewTextBoxColumn qty;
        private System.Windows.Forms.Button btnBurn;
        private System.Windows.Forms.TextBox txtOBJJSON;
        private System.Windows.Forms.TextBox txtOBJP2FK;
        private System.Windows.Forms.TextBox txtAddressListJSON;
        private System.Windows.Forms.TextBox txtSignatureAddress;
        private System.Windows.Forms.Label lblObjectStatus;
        private System.Windows.Forms.Label lblCost;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}