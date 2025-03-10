﻿namespace SUP
{
    partial class ObjectGive
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.qtyTextBox = new System.Windows.Forms.TextBox();
            this.addressQtyDataGridView = new System.Windows.Forms.DataGridView();
            this.address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnGive = new System.Windows.Forms.Button();
            this.txtOBJJSON = new System.Windows.Forms.TextBox();
            this.txtOBJP2FK = new System.Windows.Forms.TextBox();
            this.txtAddressListJSON = new System.Windows.Forms.TextBox();
            this.txtSignatureAddress = new System.Windows.Forms.TextBox();
            this.lblObjectStatus = new System.Windows.Forms.Label();
            this.lblCost = new System.Windows.Forms.Label();
            this.txtObjectAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnFromSelector = new System.Windows.Forms.Button();
            this.btnToSelector = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.addressQtyDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // addressTextBox
            // 
            this.addressTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.addressTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.addressTextBox.ForeColor = System.Drawing.Color.White;
            this.addressTextBox.Location = new System.Drawing.Point(12, 22);
            this.addressTextBox.Multiline = true;
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(291, 33);
            this.addressTextBox.TabIndex = 0;
            // 
            // qtyTextBox
            // 
            this.qtyTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.qtyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qtyTextBox.ForeColor = System.Drawing.Color.White;
            this.qtyTextBox.Location = new System.Drawing.Point(364, 22);
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
            this.addressQtyDataGridView.Location = new System.Drawing.Point(12, 93);
            this.addressQtyDataGridView.Name = "addressQtyDataGridView";
            this.addressQtyDataGridView.ReadOnly = true;
            this.addressQtyDataGridView.RowHeadersWidth = 51;
            this.addressQtyDataGridView.Size = new System.Drawing.Size(491, 321);
            this.addressQtyDataGridView.TabIndex = 2;
            // 
            // address
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.address.DefaultCellStyle = dataGridViewCellStyle3;
            this.address.HeaderText = "recipient addres";
            this.address.MaxInputLength = 46;
            this.address.MinimumWidth = 6;
            this.address.Name = "address";
            this.address.ReadOnly = true;
            this.address.Width = 325;
            // 
            // qty
            // 
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qty.DefaultCellStyle = dataGridViewCellStyle4;
            this.qty.HeaderText = "qty to give";
            this.qty.MinimumWidth = 6;
            this.qty.Name = "qty";
            this.qty.ReadOnly = true;
            this.qty.Width = 111;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnSave.Location = new System.Drawing.Point(12, 593);
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
            this.btnAdd.Location = new System.Drawing.Point(454, 22);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(49, 33);
            this.btnAdd.TabIndex = 274;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.addButton_Click);
            // 
            // btnGive
            // 
            this.btnGive.Enabled = false;
            this.btnGive.Font = new System.Drawing.Font("Segoe UI Emoji", 13.8F);
            this.btnGive.Location = new System.Drawing.Point(394, 593);
            this.btnGive.Name = "btnGive";
            this.btnGive.Size = new System.Drawing.Size(109, 53);
            this.btnGive.TabIndex = 275;
            this.btnGive.Text = "💞";
            this.btnGive.UseVisualStyleBackColor = true;
            this.btnGive.Click += new System.EventHandler(this.giveButton_Click);
            // 
            // txtOBJJSON
            // 
            this.txtOBJJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOBJJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOBJJSON.ForeColor = System.Drawing.Color.Black;
            this.txtOBJJSON.Location = new System.Drawing.Point(18, 164);
            this.txtOBJJSON.Multiline = true;
            this.txtOBJJSON.Name = "txtOBJJSON";
            this.txtOBJJSON.Size = new System.Drawing.Size(141, 226);
            this.txtOBJJSON.TabIndex = 359;
            this.txtOBJJSON.Visible = false;
            // 
            // txtOBJP2FK
            // 
            this.txtOBJP2FK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOBJP2FK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOBJP2FK.ForeColor = System.Drawing.Color.Black;
            this.txtOBJP2FK.Location = new System.Drawing.Point(165, 164);
            this.txtOBJP2FK.Multiline = true;
            this.txtOBJP2FK.Name = "txtOBJP2FK";
            this.txtOBJP2FK.Size = new System.Drawing.Size(156, 226);
            this.txtOBJP2FK.TabIndex = 360;
            this.txtOBJP2FK.Visible = false;
            // 
            // txtAddressListJSON
            // 
            this.txtAddressListJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtAddressListJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddressListJSON.ForeColor = System.Drawing.Color.Black;
            this.txtAddressListJSON.Location = new System.Drawing.Point(327, 164);
            this.txtAddressListJSON.Multiline = true;
            this.txtAddressListJSON.Name = "txtAddressListJSON";
            this.txtAddressListJSON.Size = new System.Drawing.Size(160, 226);
            this.txtAddressListJSON.TabIndex = 358;
            this.txtAddressListJSON.Visible = false;
            // 
            // txtSignatureAddress
            // 
            this.txtSignatureAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtSignatureAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSignatureAddress.ForeColor = System.Drawing.Color.White;
            this.txtSignatureAddress.Location = new System.Drawing.Point(12, 525);
            this.txtSignatureAddress.Multiline = true;
            this.txtSignatureAddress.Name = "txtSignatureAddress";
            this.txtSignatureAddress.Size = new System.Drawing.Size(436, 53);
            this.txtSignatureAddress.TabIndex = 361;
            // 
            // lblObjectStatus
            // 
            this.lblObjectStatus.AutoSize = true;
            this.lblObjectStatus.ForeColor = System.Drawing.Color.White;
            this.lblObjectStatus.Location = new System.Drawing.Point(14, 66);
            this.lblObjectStatus.Name = "lblObjectStatus";
            this.lblObjectStatus.Size = new System.Drawing.Size(0, 13);
            this.lblObjectStatus.TabIndex = 363;
            // 
            // lblCost
            // 
            this.lblCost.AutoSize = true;
            this.lblCost.ForeColor = System.Drawing.Color.White;
            this.lblCost.Location = new System.Drawing.Point(222, 623);
            this.lblCost.Name = "lblCost";
            this.lblCost.Size = new System.Drawing.Size(0, 13);
            this.lblCost.TabIndex = 364;
            // 
            // txtObjectAddress
            // 
            this.txtObjectAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtObjectAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtObjectAddress.ForeColor = System.Drawing.Color.White;
            this.txtObjectAddress.Location = new System.Drawing.Point(12, 445);
            this.txtObjectAddress.Multiline = true;
            this.txtObjectAddress.Name = "txtObjectAddress";
            this.txtObjectAddress.Size = new System.Drawing.Size(491, 56);
            this.txtObjectAddress.TabIndex = 365;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 366;
            this.label1.Text = "enter recipient address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(367, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 13);
            this.label2.TabIndex = 367;
            this.label2.Text = "enter qty to give and hit   +";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(15, 427);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 13);
            this.label3.TabIndex = 368;
            this.label3.Text = "enter object address to give";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(15, 509);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 369;
            this.label4.Text = "enter signature address";
            // 
            // btnFromSelector
            // 
            this.btnFromSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFromSelector.Font = new System.Drawing.Font("Segoe UI Emoji", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFromSelector.ForeColor = System.Drawing.Color.Black;
            this.btnFromSelector.Location = new System.Drawing.Point(453, 525);
            this.btnFromSelector.Name = "btnFromSelector";
            this.btnFromSelector.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnFromSelector.Size = new System.Drawing.Size(50, 53);
            this.btnFromSelector.TabIndex = 373;
            this.btnFromSelector.Text = "👤";
            this.btnFromSelector.UseVisualStyleBackColor = true;
            this.btnFromSelector.Click += new System.EventHandler(this.btnFromSelector_Click);
            // 
            // btnToSelector
            // 
            this.btnToSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToSelector.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToSelector.ForeColor = System.Drawing.Color.Black;
            this.btnToSelector.Location = new System.Drawing.Point(309, 22);
            this.btnToSelector.Name = "btnToSelector";
            this.btnToSelector.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnToSelector.Size = new System.Drawing.Size(39, 33);
            this.btnToSelector.TabIndex = 374;
            this.btnToSelector.Text = "👤";
            this.btnToSelector.UseVisualStyleBackColor = true;
            this.btnToSelector.Click += new System.EventHandler(this.btnToSelector_Click);
            // 
            // ObjectGive
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(517, 660);
            this.Controls.Add(this.btnToSelector);
            this.Controls.Add(this.btnFromSelector);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtObjectAddress);
            this.Controls.Add(this.lblCost);
            this.Controls.Add(this.lblObjectStatus);
            this.Controls.Add(this.txtSignatureAddress);
            this.Controls.Add(this.txtOBJJSON);
            this.Controls.Add(this.txtOBJP2FK);
            this.Controls.Add(this.txtAddressListJSON);
            this.Controls.Add(this.btnGive);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.addressQtyDataGridView);
            this.Controls.Add(this.qtyTextBox);
            this.Controls.Add(this.addressTextBox);
            this.MaximumSize = new System.Drawing.Size(533, 699);
            this.MinimumSize = new System.Drawing.Size(533, 699);
            this.Name = "ObjectGive";
            this.Text = "ObjectGive";
            this.Load += new System.EventHandler(this.ObjectGive_Load);
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
        private System.Windows.Forms.Button btnGive;
        private System.Windows.Forms.TextBox txtOBJJSON;
        private System.Windows.Forms.TextBox txtOBJP2FK;
        private System.Windows.Forms.TextBox txtAddressListJSON;
        private System.Windows.Forms.TextBox txtSignatureAddress;
        private System.Windows.Forms.Label lblObjectStatus;
        private System.Windows.Forms.Label lblCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn address;
        private System.Windows.Forms.DataGridViewTextBoxColumn qty;
        private System.Windows.Forms.TextBox txtObjectAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnFromSelector;
        private System.Windows.Forms.Button btnToSelector;
    }
}