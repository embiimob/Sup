using System.Windows.Forms;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnPut = new System.Windows.Forms.Button();
            this.btnGet = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
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
            this.lbTableName = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.lblTotalBytes = new System.Windows.Forms.Label();
            this.txtbalance = new System.Windows.Forms.TextBox();
            this.txtSearchAddress = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dgTransactions = new System.Windows.Forms.DataGridView();
            this.rootid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messagecount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.is_signed = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.signed_by = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filecount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalfilebytes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.keywordcount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_bytes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.block_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transaction_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.signature = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.confirmations = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildtime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGetTransactionId = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtTransactionId = new System.Windows.Forms.TextBox();
            this.btnGetKeyword = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtGetKeyword = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtVersionByte = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.lblKbs = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPut
            // 
            this.btnPut.Location = new System.Drawing.Point(972, 50);
            this.btnPut.Name = "btnPut";
            this.btnPut.Size = new System.Drawing.Size(75, 23);
            this.btnPut.TabIndex = 0;
            this.btnPut.Text = "Put";
            this.btnPut.UseVisualStyleBackColor = true;
            this.btnPut.Click += new System.EventHandler(this.BtnPut_Click);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(972, 123);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(75, 23);
            this.btnGet.TabIndex = 1;
            this.btnGet.Text = "Get";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.BtnGet_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(972, 179);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // txtPutKey
            // 
            this.txtPutKey.Location = new System.Drawing.Point(369, 32);
            this.txtPutKey.Multiline = true;
            this.txtPutKey.Name = "txtPutKey";
            this.txtPutKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPutKey.Size = new System.Drawing.Size(242, 41);
            this.txtPutKey.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(366, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "levelDB Key";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(366, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "levelDB Key";
            // 
            // txtGetKey
            // 
            this.txtGetKey.Location = new System.Drawing.Point(369, 103);
            this.txtGetKey.Multiline = true;
            this.txtGetKey.Name = "txtGetKey";
            this.txtGetKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGetKey.Size = new System.Drawing.Size(242, 43);
            this.txtGetKey.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(614, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "levelDB Key";
            // 
            // txtDeleteKey
            // 
            this.txtDeleteKey.Location = new System.Drawing.Point(617, 182);
            this.txtDeleteKey.Name = "txtDeleteKey";
            this.txtDeleteKey.Size = new System.Drawing.Size(350, 20);
            this.txtDeleteKey.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(614, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Value";
            // 
            // txtPutValue
            // 
            this.txtPutValue.Location = new System.Drawing.Point(617, 32);
            this.txtPutValue.Multiline = true;
            this.txtPutValue.Name = "txtPutValue";
            this.txtPutValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPutValue.Size = new System.Drawing.Size(350, 41);
            this.txtPutValue.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(614, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Value";
            // 
            // txtGetValue
            // 
            this.txtGetValue.Location = new System.Drawing.Point(617, 103);
            this.txtGetValue.Multiline = true;
            this.txtGetValue.Name = "txtGetValue";
            this.txtGetValue.ReadOnly = true;
            this.txtGetValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGetValue.Size = new System.Drawing.Size(350, 43);
            this.txtGetValue.TabIndex = 11;
            // 
            // lbTableName
            // 
            this.lbTableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lbTableName.FormattingEnabled = true;
            this.lbTableName.Items.AddRange(new object[] {
            "ROOT",
            "PRO",
            "COL",
            "OBJ",
            "LOGS"});
            this.lbTableName.Location = new System.Drawing.Point(525, 172);
            this.lbTableName.Name = "lbTableName";
            this.lbTableName.ScrollAlwaysVisible = true;
            this.lbTableName.Size = new System.Drawing.Size(83, 30);
            this.lbTableName.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(522, 156);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "levelDB Table";
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(13, 34);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(111, 20);
            this.txtLogin.TabIndex = 15;
            this.txtLogin.Text = "good-user";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "login";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(132, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(130, 34);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(121, 20);
            this.txtPassword.TabIndex = 17;
            this.txtPassword.Text = "better-password";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(18, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "url";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(14, 75);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(170, 20);
            this.txtUrl.TabIndex = 19;
            this.txtUrl.Text = "http://127.0.0.1:18332";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(257, 32);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(97, 23);
            this.btnTestConnection.TabIndex = 21;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.BtnTestConnection_Click);
            // 
            // lblTotalBytes
            // 
            this.lblTotalBytes.AutoSize = true;
            this.lblTotalBytes.Location = new System.Drawing.Point(759, 213);
            this.lblTotalBytes.Name = "lblTotalBytes";
            this.lblTotalBytes.Size = new System.Drawing.Size(60, 13);
            this.lblTotalBytes.TabIndex = 23;
            this.lblTotalBytes.Text = "Total Bytes";
            // 
            // txtbalance
            // 
            this.txtbalance.Location = new System.Drawing.Point(257, 75);
            this.txtbalance.Name = "txtbalance";
            this.txtbalance.Size = new System.Drawing.Size(97, 20);
            this.txtbalance.TabIndex = 22;
            // 
            // txtSearchAddress
            // 
            this.txtSearchAddress.Location = new System.Drawing.Point(10, 166);
            this.txtSearchAddress.Name = "txtSearchAddress";
            this.txtSearchAddress.Size = new System.Drawing.Size(238, 20);
            this.txtSearchAddress.TabIndex = 25;
            this.txtSearchAddress.Text = "mmkLUNKvLdLBHycaTRmRv1tZpDxsAKXBNn";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 151);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Address";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(254, 164);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(58, 23);
            this.btnSearch.TabIndex = 27;
            this.btnSearch.Text = "Get";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // dgTransactions
            // 
            this.dgTransactions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgTransactions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgTransactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTransactions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rootid,
            this.messagecount,
            this.is_signed,
            this.signed_by,
            this.filecount,
            this.totalfilebytes,
            this.keywordcount,
            this.total_bytes,
            this.block_time,
            this.transaction_id,
            this.signature,
            this.confirmations,
            this.buildtime});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgTransactions.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgTransactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgTransactions.Location = new System.Drawing.Point(0, 0);
            this.dgTransactions.Name = "dgTransactions";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgTransactions.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgTransactions.Size = new System.Drawing.Size(1056, 87);
            this.dgTransactions.TabIndex = 30;
            // 
            // rootid
            // 
            this.rootid.HeaderText = "Object ID";
            this.rootid.MinimumWidth = 60;
            this.rootid.Name = "rootid";
            // 
            // messagecount
            // 
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.messagecount.DefaultCellStyle = dataGridViewCellStyle2;
            this.messagecount.HeaderText = "Messages";
            this.messagecount.MaxInputLength = 100000000;
            this.messagecount.Name = "messagecount";
            // 
            // is_signed
            // 
            this.is_signed.HeaderText = "Signed";
            this.is_signed.MinimumWidth = 50;
            this.is_signed.Name = "is_signed";
            this.is_signed.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.is_signed.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // signed_by
            // 
            this.signed_by.HeaderText = "Signed By";
            this.signed_by.Name = "signed_by";
            // 
            // filecount
            // 
            this.filecount.HeaderText = "Files";
            this.filecount.Name = "filecount";
            // 
            // totalfilebytes
            // 
            this.totalfilebytes.HeaderText = "File Bytes";
            this.totalfilebytes.Name = "totalfilebytes";
            this.totalfilebytes.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // keywordcount
            // 
            this.keywordcount.HeaderText = "Keys";
            this.keywordcount.Name = "keywordcount";
            // 
            // total_bytes
            // 
            this.total_bytes.HeaderText = "Total Bytes";
            this.total_bytes.Name = "total_bytes";
            // 
            // block_time
            // 
            this.block_time.HeaderText = "Block Time";
            this.block_time.Name = "block_time";
            // 
            // transaction_id
            // 
            this.transaction_id.HeaderText = "TID";
            this.transaction_id.Name = "transaction_id";
            // 
            // signature
            // 
            this.signature.HeaderText = "SIG";
            this.signature.Name = "signature";
            // 
            // confirmations
            // 
            this.confirmations.HeaderText = "Confirms";
            this.confirmations.Name = "confirmations";
            // 
            // buildtime
            // 
            this.buildtime.HeaderText = "Build Time";
            this.buildtime.Name = "buildtime";
            // 
            // btnGetTransactionId
            // 
            this.btnGetTransactionId.Location = new System.Drawing.Point(439, 203);
            this.btnGetTransactionId.Name = "btnGetTransactionId";
            this.btnGetTransactionId.Size = new System.Drawing.Size(58, 23);
            this.btnGetTransactionId.TabIndex = 33;
            this.btnGetTransactionId.Text = "Get";
            this.btnGetTransactionId.UseVisualStyleBackColor = true;
            this.btnGetTransactionId.Click += new System.EventHandler(this.BtnGetTransactionId_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 189);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 13);
            this.label12.TabIndex = 32;
            this.label12.Text = "Transaction Id";
            // 
            // txtTransactionId
            // 
            this.txtTransactionId.Location = new System.Drawing.Point(10, 205);
            this.txtTransactionId.Name = "txtTransactionId";
            this.txtTransactionId.Size = new System.Drawing.Size(423, 20);
            this.txtTransactionId.TabIndex = 31;
            this.txtTransactionId.Text = "7223070fbc3706856e90701b62a8bfb3f5a618dc13d64d8f029260cba187ca26";
            // 
            // btnGetKeyword
            // 
            this.btnGetKeyword.Location = new System.Drawing.Point(254, 126);
            this.btnGetKeyword.Name = "btnGetKeyword";
            this.btnGetKeyword.Size = new System.Drawing.Size(58, 23);
            this.btnGetKeyword.TabIndex = 36;
            this.btnGetKeyword.Text = "Get";
            this.btnGetKeyword.UseVisualStyleBackColor = true;
            this.btnGetKeyword.Click += new System.EventHandler(this.BtnGetKeyword_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 113);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Keyword";
            // 
            // txtGetKeyword
            // 
            this.txtGetKeyword.Location = new System.Drawing.Point(10, 128);
            this.txtGetKeyword.Name = "txtGetKeyword";
            this.txtGetKeyword.Size = new System.Drawing.Size(238, 20);
            this.txtGetKeyword.TabIndex = 34;
            this.txtGetKeyword.Text = "HugPuddle";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(187, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(27, 13);
            this.label13.TabIndex = 38;
            this.label13.Text = "byte";
            // 
            // txtVersionByte
            // 
            this.txtVersionByte.Location = new System.Drawing.Point(190, 75);
            this.txtVersionByte.Name = "txtVersionByte";
            this.txtVersionByte.Size = new System.Drawing.Size(61, 20);
            this.txtVersionByte.TabIndex = 37;
            this.txtVersionByte.Text = "111";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(254, 59);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(45, 13);
            this.label14.TabIndex = 39;
            this.label14.Text = "balance";
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(555, 213);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(57, 13);
            this.lblTotalTime.TabIndex = 40;
            this.lblTotalTime.Text = "Total Time";
            // 
            // lblKbs
            // 
            this.lblKbs.AutoSize = true;
            this.lblKbs.Location = new System.Drawing.Point(928, 213);
            this.lblKbs.Name = "lblKbs";
            this.lblKbs.Size = new System.Drawing.Size(30, 13);
            this.lblKbs.TabIndex = 41;
            this.lblKbs.Text = "Kb/s";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.button2);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.txtPutValue);
            this.splitContainer1.Panel1.Controls.Add(this.lblKbs);
            this.splitContainer1.Panel1.Controls.Add(this.btnPut);
            this.splitContainer1.Panel1.Controls.Add(this.lblTotalTime);
            this.splitContainer1.Panel1.Controls.Add(this.btnGet);
            this.splitContainer1.Panel1.Controls.Add(this.label14);
            this.splitContainer1.Panel1.Controls.Add(this.btnDelete);
            this.splitContainer1.Panel1.Controls.Add(this.label13);
            this.splitContainer1.Panel1.Controls.Add(this.txtPutKey);
            this.splitContainer1.Panel1.Controls.Add(this.txtVersionByte);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetKeyword);
            this.splitContainer1.Panel1.Controls.Add(this.txtGetKey);
            this.splitContainer1.Panel1.Controls.Add(this.label10);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.txtGetKeyword);
            this.splitContainer1.Panel1.Controls.Add(this.txtDeleteKey);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetTransactionId);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label12);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.txtTransactionId);
            this.splitContainer1.Panel1.Controls.Add(this.txtGetValue);
            this.splitContainer1.Panel1.Controls.Add(this.btnSearch);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.label11);
            this.splitContainer1.Panel1.Controls.Add(this.lbTableName);
            this.splitContainer1.Panel1.Controls.Add(this.txtSearchAddress);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.lblTotalBytes);
            this.splitContainer1.Panel1.Controls.Add(this.txtLogin);
            this.splitContainer1.Panel1.Controls.Add(this.txtbalance);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.btnTestConnection);
            this.splitContainer1.Panel1.Controls.Add(this.txtPassword);
            this.splitContainer1.Panel1.Controls.Add(this.label9);
            this.splitContainer1.Panel1.Controls.Add(this.label8);
            this.splitContainer1.Panel1.Controls.Add(this.txtUrl);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgTransactions);
            this.splitContainer1.Size = new System.Drawing.Size(1056, 335);
            this.splitContainer1.SplitterDistance = 228;
            this.splitContainer1.SplitterWidth = 20;
            this.splitContainer1.TabIndex = 42;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(385, 162);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(60, 23);
            this.button2.TabIndex = 43;
            this.button2.Text = "decrypt";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(319, 162);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 23);
            this.button1.TabIndex = 42;
            this.button1.Text = "encrypt";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 335);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(1072, 374);
            this.Name = "Form1";
            this.Text = "Sup!? Just testing ";
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPut;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.Button btnDelete;
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
        private System.Windows.Forms.ListBox lbTableName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblTotalBytes;
        private System.Windows.Forms.TextBox txtbalance;
        private System.Windows.Forms.TextBox txtSearchAddress;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgTransactions;
        private System.Windows.Forms.Button btnGetTransactionId;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtTransactionId;
        private System.Windows.Forms.Button btnGetKeyword;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtGetKeyword;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtVersionByte;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.Label lblKbs;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private DataGridViewTextBoxColumn rootid;
        private DataGridViewTextBoxColumn messagecount;
        private DataGridViewCheckBoxColumn is_signed;
        private DataGridViewTextBoxColumn signed_by;
        private DataGridViewTextBoxColumn filecount;
        private DataGridViewTextBoxColumn totalfilebytes;
        private DataGridViewTextBoxColumn keywordcount;
        private DataGridViewTextBoxColumn total_bytes;
        private DataGridViewTextBoxColumn block_time;
        private DataGridViewTextBoxColumn transaction_id;
        private DataGridViewTextBoxColumn signature;
        private DataGridViewTextBoxColumn confirmations;
        private DataGridViewTextBoxColumn buildtime;
        private Button button1;
        private Button button2;
    }
}

