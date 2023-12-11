using System.Windows.Forms;

namespace SUP
{
    partial class WorkBench
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
            this.BlockHeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buildtime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGetTransactionId = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtTransactionId = new System.Windows.Forms.TextBox();
            this.btnGetKeyword = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.txtVersionByte = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.lblKbs = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnGetInquiryByTransactionId = new System.Windows.Forms.Button();
            this.btnGetInquiries = new System.Windows.Forms.Button();
            this.btnInquiriesCreated = new System.Windows.Forms.Button();
            this.btnGetInquiriesByKeyword = new System.Windows.Forms.Button();
            this.btnGetInquiry = new System.Windows.Forms.Button();
            this.btnCollections = new System.Windows.Forms.Button();
            this.btnGetLocalProfiles = new System.Windows.Forms.Button();
            this.ButtonGetPublicKeys = new System.Windows.Forms.Button();
            this.ButtonGetPrivateMessages = new System.Windows.Forms.Button();
            this.ButtonGetPublicMessages = new System.Windows.Forms.Button();
            this.ButtonGetObjectByTransactionId = new System.Windows.Forms.Button();
            this.btnGetFoundObjects = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSkip = new System.Windows.Forms.TextBox();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.btnUnBlockAddress = new System.Windows.Forms.Button();
            this.btnUnMuteAddress = new System.Windows.Forms.Button();
            this.btnMuteAddress = new System.Windows.Forms.Button();
            this.btnBlockTransaction = new System.Windows.Forms.Button();
            this.btnBlockAddress = new System.Windows.Forms.Button();
            this.btnProfileURN = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnObjectURN = new System.Windows.Forms.Button();
            this.txtGetValue = new System.Windows.Forms.RichTextBox();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.btnGetObjectsByKeyword = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnGetCreated = new System.Windows.Forms.Button();
            this.btnGetOwned = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnGetObject = new System.Windows.Forms.Button();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.btnEncrypt = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(13, 34);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(170, 20);
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
            this.label8.Location = new System.Drawing.Point(192, 14);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(190, 31);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(159, 20);
            this.txtPassword.TabIndex = 17;
            this.txtPassword.Text = "better-password";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(18, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "url";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(13, 83);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(170, 20);
            this.txtUrl.TabIndex = 19;
            this.txtUrl.Text = "http://127.0.0.1:18332";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(368, 29);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(86, 23);
            this.btnTestConnection.TabIndex = 21;
            this.btnTestConnection.Text = "Test Connect";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.ButtonTestConnectionClick);
            // 
            // lblTotalBytes
            // 
            this.lblTotalBytes.AutoSize = true;
            this.lblTotalBytes.Location = new System.Drawing.Point(200, 375);
            this.lblTotalBytes.Name = "lblTotalBytes";
            this.lblTotalBytes.Size = new System.Drawing.Size(35, 13);
            this.lblTotalBytes.TabIndex = 23;
            this.lblTotalBytes.Text = "bytes:";
            // 
            // txtbalance
            // 
            this.txtbalance.Location = new System.Drawing.Point(468, 34);
            this.txtbalance.Name = "txtbalance";
            this.txtbalance.Size = new System.Drawing.Size(127, 20);
            this.txtbalance.TabIndex = 22;
            // 
            // txtSearchAddress
            // 
            this.txtSearchAddress.Location = new System.Drawing.Point(9, 227);
            this.txtSearchAddress.Name = "txtSearchAddress";
            this.txtSearchAddress.Size = new System.Drawing.Size(419, 20);
            this.txtSearchAddress.TabIndex = 25;
            this.txtSearchAddress.Text = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 203);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "address";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(439, 227);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(76, 23);
            this.btnSearch.TabIndex = 27;
            this.btnSearch.Text = "get address";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.ButtonSearchClick);
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
            this.BlockHeight,
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
            this.dgTransactions.Size = new System.Drawing.Size(1154, 322);
            this.dgTransactions.TabIndex = 30;
            this.dgTransactions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewClick);
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
            // BlockHeight
            // 
            this.BlockHeight.HeaderText = "BlockHeight";
            this.BlockHeight.Name = "BlockHeight";
            // 
            // buildtime
            // 
            this.buildtime.HeaderText = "Build Time";
            this.buildtime.Name = "buildtime";
            // 
            // btnGetTransactionId
            // 
            this.btnGetTransactionId.Location = new System.Drawing.Point(560, 153);
            this.btnGetTransactionId.Name = "btnGetTransactionId";
            this.btnGetTransactionId.Size = new System.Drawing.Size(38, 23);
            this.btnGetTransactionId.TabIndex = 33;
            this.btnGetTransactionId.Text = "get";
            this.btnGetTransactionId.UseVisualStyleBackColor = true;
            this.btnGetTransactionId.Click += new System.EventHandler(this.ButtonGetTransactionIdClick);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 139);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 13);
            this.label12.TabIndex = 32;
            this.label12.Text = "txid";
            // 
            // txtTransactionId
            // 
            this.txtTransactionId.Location = new System.Drawing.Point(9, 155);
            this.txtTransactionId.Name = "txtTransactionId";
            this.txtTransactionId.Size = new System.Drawing.Size(545, 20);
            this.txtTransactionId.TabIndex = 31;
            this.txtTransactionId.Text = "7223070fbc3706856e90701b62a8bfb3f5a618dc13d64d8f029260cba187ca26";
            // 
            // btnGetKeyword
            // 
            this.btnGetKeyword.Location = new System.Drawing.Point(525, 227);
            this.btnGetKeyword.Name = "btnGetKeyword";
            this.btnGetKeyword.Size = new System.Drawing.Size(73, 23);
            this.btnGetKeyword.TabIndex = 36;
            this.btnGetKeyword.Text = "by keyword";
            this.btnGetKeyword.UseVisualStyleBackColor = true;
            this.btnGetKeyword.Click += new System.EventHandler(this.ButtonGetKeywordClick);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(187, 67);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(27, 13);
            this.label13.TabIndex = 38;
            this.label13.Text = "byte";
            // 
            // txtVersionByte
            // 
            this.txtVersionByte.Location = new System.Drawing.Point(190, 83);
            this.txtVersionByte.Name = "txtVersionByte";
            this.txtVersionByte.Size = new System.Drawing.Size(61, 20);
            this.txtVersionByte.TabIndex = 37;
            this.txtVersionByte.Text = "111";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(465, 18);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(45, 13);
            this.label14.TabIndex = 39;
            this.label14.Text = "balance";
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(25, 375);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(29, 13);
            this.lblTotalTime.TabIndex = 40;
            this.lblTotalTime.Text = "time:";
            // 
            // lblKbs
            // 
            this.lblKbs.AutoSize = true;
            this.lblKbs.Location = new System.Drawing.Point(314, 375);
            this.lblKbs.Name = "lblKbs";
            this.lblKbs.Size = new System.Drawing.Size(32, 13);
            this.lblKbs.TabIndex = 41;
            this.lblKbs.Text = "kb/s:";
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
            this.splitContainer1.Panel1.Controls.Add(this.btnGetInquiryByTransactionId);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetInquiries);
            this.splitContainer1.Panel1.Controls.Add(this.btnInquiriesCreated);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetInquiriesByKeyword);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetInquiry);
            this.splitContainer1.Panel1.Controls.Add(this.btnCollections);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetLocalProfiles);
            this.splitContainer1.Panel1.Controls.Add(this.ButtonGetPublicKeys);
            this.splitContainer1.Panel1.Controls.Add(this.ButtonGetPrivateMessages);
            this.splitContainer1.Panel1.Controls.Add(this.ButtonGetPublicMessages);
            this.splitContainer1.Panel1.Controls.Add(this.ButtonGetObjectByTransactionId);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetFoundObjects);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.txtSkip);
            this.splitContainer1.Panel1.Controls.Add(this.txtQty);
            this.splitContainer1.Panel1.Controls.Add(this.btnUnBlockAddress);
            this.splitContainer1.Panel1.Controls.Add(this.btnUnMuteAddress);
            this.splitContainer1.Panel1.Controls.Add(this.btnMuteAddress);
            this.splitContainer1.Panel1.Controls.Add(this.btnBlockTransaction);
            this.splitContainer1.Panel1.Controls.Add(this.btnBlockAddress);
            this.splitContainer1.Panel1.Controls.Add(this.btnProfileURN);
            this.splitContainer1.Panel1.Controls.Add(this.button4);
            this.splitContainer1.Panel1.Controls.Add(this.btnObjectURN);
            this.splitContainer1.Panel1.Controls.Add(this.txtGetValue);
            this.splitContainer1.Panel1.Controls.Add(this.chkVerbose);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetObjectsByKeyword);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetCreated);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetOwned);
            this.splitContainer1.Panel1.Controls.Add(this.lblTotal);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetObject);
            this.splitContainer1.Panel1.Controls.Add(this.btnDecrypt);
            this.splitContainer1.Panel1.Controls.Add(this.btnEncrypt);
            this.splitContainer1.Panel1.Controls.Add(this.lblKbs);
            this.splitContainer1.Panel1.Controls.Add(this.lblTotalTime);
            this.splitContainer1.Panel1.Controls.Add(this.label14);
            this.splitContainer1.Panel1.Controls.Add(this.label13);
            this.splitContainer1.Panel1.Controls.Add(this.txtVersionByte);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetKeyword);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetTransactionId);
            this.splitContainer1.Panel1.Controls.Add(this.label12);
            this.splitContainer1.Panel1.Controls.Add(this.txtTransactionId);
            this.splitContainer1.Panel1.Controls.Add(this.btnSearch);
            this.splitContainer1.Panel1.Controls.Add(this.label11);
            this.splitContainer1.Panel1.Controls.Add(this.txtSearchAddress);
            this.splitContainer1.Panel1.Controls.Add(this.lblTotalBytes);
            this.splitContainer1.Panel1.Controls.Add(this.txtLogin);
            this.splitContainer1.Panel1.Controls.Add(this.txtbalance);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.btnTestConnection);
            this.splitContainer1.Panel1.Controls.Add(this.txtPassword);
            this.splitContainer1.Panel1.Controls.Add(this.label9);
            this.splitContainer1.Panel1.Controls.Add(this.label8);
            this.splitContainer1.Panel1.Controls.Add(this.txtUrl);
            this.splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgTransactions);
            this.splitContainer1.Size = new System.Drawing.Size(1154, 732);
            this.splitContainer1.SplitterDistance = 400;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 42;
            this.splitContainer1.Resize += new System.EventHandler(this.OnContainerResize);
            // 
            // btnGetInquiryByTransactionId
            // 
            this.btnGetInquiryByTransactionId.Location = new System.Drawing.Point(95, 333);
            this.btnGetInquiryByTransactionId.Name = "btnGetInquiryByTransactionId";
            this.btnGetInquiryByTransactionId.Size = new System.Drawing.Size(79, 23);
            this.btnGetInquiryByTransactionId.TabIndex = 84;
            this.btnGetInquiryByTransactionId.Text = "by txid";
            this.btnGetInquiryByTransactionId.UseVisualStyleBackColor = true;
            this.btnGetInquiryByTransactionId.Click += new System.EventHandler(this.btnGetInquiryByTransactionId_Click);
            // 
            // btnGetInquiries
            // 
            this.btnGetInquiries.Location = new System.Drawing.Point(182, 333);
            this.btnGetInquiries.Name = "btnGetInquiries";
            this.btnGetInquiries.Size = new System.Drawing.Size(81, 23);
            this.btnGetInquiries.TabIndex = 83;
            this.btnGetInquiries.Text = "get inquiries";
            this.btnGetInquiries.UseVisualStyleBackColor = true;
            this.btnGetInquiries.Click += new System.EventHandler(this.btnGetInquiries_Click);
            // 
            // btnInquiriesCreated
            // 
            this.btnInquiriesCreated.Location = new System.Drawing.Point(354, 334);
            this.btnInquiriesCreated.Name = "btnInquiriesCreated";
            this.btnInquiriesCreated.Size = new System.Drawing.Size(79, 23);
            this.btnInquiriesCreated.TabIndex = 82;
            this.btnInquiriesCreated.Text = "inq created";
            this.btnInquiriesCreated.UseVisualStyleBackColor = true;
            this.btnInquiriesCreated.Click += new System.EventHandler(this.btnInquiriesCreated_Click);
            // 
            // btnGetInquiriesByKeyword
            // 
            this.btnGetInquiriesByKeyword.Location = new System.Drawing.Point(269, 333);
            this.btnGetInquiriesByKeyword.Name = "btnGetInquiriesByKeyword";
            this.btnGetInquiriesByKeyword.Size = new System.Drawing.Size(79, 23);
            this.btnGetInquiriesByKeyword.TabIndex = 81;
            this.btnGetInquiriesByKeyword.Text = "by keyword";
            this.btnGetInquiriesByKeyword.UseVisualStyleBackColor = true;
            this.btnGetInquiriesByKeyword.Click += new System.EventHandler(this.btnGetInquiriesByKeyword_Click);
            // 
            // btnGetInquiry
            // 
            this.btnGetInquiry.Location = new System.Drawing.Point(13, 333);
            this.btnGetInquiry.Name = "btnGetInquiry";
            this.btnGetInquiry.Size = new System.Drawing.Size(78, 23);
            this.btnGetInquiry.TabIndex = 80;
            this.btnGetInquiry.Text = "get inquiry";
            this.btnGetInquiry.UseVisualStyleBackColor = true;
            this.btnGetInquiry.Click += new System.EventHandler(this.btnGetInquiry_Click);
            // 
            // btnCollections
            // 
            this.btnCollections.Location = new System.Drawing.Point(354, 274);
            this.btnCollections.Name = "btnCollections";
            this.btnCollections.Size = new System.Drawing.Size(79, 23);
            this.btnCollections.TabIndex = 79;
            this.btnCollections.Text = "collections";
            this.btnCollections.UseVisualStyleBackColor = true;
            this.btnCollections.Click += new System.EventHandler(this.btnCollections_Click);
            // 
            // btnGetLocalProfiles
            // 
            this.btnGetLocalProfiles.Location = new System.Drawing.Point(437, 274);
            this.btnGetLocalProfiles.Name = "btnGetLocalProfiles";
            this.btnGetLocalProfiles.Size = new System.Drawing.Size(81, 23);
            this.btnGetLocalProfiles.TabIndex = 76;
            this.btnGetLocalProfiles.Text = "local profiles";
            this.btnGetLocalProfiles.UseVisualStyleBackColor = true;
            this.btnGetLocalProfiles.Click += new System.EventHandler(this.btnGetLocalProfiles_Click);
            // 
            // ButtonGetPublicKeys
            // 
            this.ButtonGetPublicKeys.Location = new System.Drawing.Point(300, 198);
            this.ButtonGetPublicKeys.Name = "ButtonGetPublicKeys";
            this.ButtonGetPublicKeys.Size = new System.Drawing.Size(72, 23);
            this.ButtonGetPublicKeys.TabIndex = 75;
            this.ButtonGetPublicKeys.Text = "pubkey";
            this.ButtonGetPublicKeys.UseVisualStyleBackColor = true;
            this.ButtonGetPublicKeys.Click += new System.EventHandler(this.ButtonGetPublicKeys_Click);
            // 
            // ButtonGetPrivateMessages
            // 
            this.ButtonGetPrivateMessages.Location = new System.Drawing.Point(269, 274);
            this.ButtonGetPrivateMessages.Name = "ButtonGetPrivateMessages";
            this.ButtonGetPrivateMessages.Size = new System.Drawing.Size(79, 23);
            this.ButtonGetPrivateMessages.TabIndex = 74;
            this.ButtonGetPrivateMessages.Text = "private  msgs";
            this.ButtonGetPrivateMessages.UseVisualStyleBackColor = true;
            this.ButtonGetPrivateMessages.Click += new System.EventHandler(this.ButtonGetPrivateMessages_Click);
            // 
            // ButtonGetPublicMessages
            // 
            this.ButtonGetPublicMessages.Location = new System.Drawing.Point(182, 274);
            this.ButtonGetPublicMessages.Name = "ButtonGetPublicMessages";
            this.ButtonGetPublicMessages.Size = new System.Drawing.Size(81, 23);
            this.ButtonGetPublicMessages.TabIndex = 73;
            this.ButtonGetPublicMessages.Text = "public msgs";
            this.ButtonGetPublicMessages.UseVisualStyleBackColor = true;
            this.ButtonGetPublicMessages.Click += new System.EventHandler(this.ButtonGetPublicMessages_Click);
            // 
            // ButtonGetObjectByTransactionId
            // 
            this.ButtonGetObjectByTransactionId.Location = new System.Drawing.Point(97, 303);
            this.ButtonGetObjectByTransactionId.Name = "ButtonGetObjectByTransactionId";
            this.ButtonGetObjectByTransactionId.Size = new System.Drawing.Size(79, 23);
            this.ButtonGetObjectByTransactionId.TabIndex = 72;
            this.ButtonGetObjectByTransactionId.Text = "by txid";
            this.ButtonGetObjectByTransactionId.UseVisualStyleBackColor = true;
            this.ButtonGetObjectByTransactionId.Click += new System.EventHandler(this.ButtonGetObjectByTransactionId_Click);
            // 
            // btnGetFoundObjects
            // 
            this.btnGetFoundObjects.Location = new System.Drawing.Point(521, 274);
            this.btnGetFoundObjects.Name = "btnGetFoundObjects";
            this.btnGetFoundObjects.Size = new System.Drawing.Size(77, 23);
            this.btnGetFoundObjects.TabIndex = 71;
            this.btnGetFoundObjects.Text = "obj found";
            this.btnGetFoundObjects.UseVisualStyleBackColor = true;
            this.btnGetFoundObjects.Click += new System.EventHandler(this.ButtonGetFoundObjectsClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(494, 370);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 69;
            this.label2.Text = "skip";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(492, 338);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 68;
            this.label1.Text = "qty";
            // 
            // txtSkip
            // 
            this.txtSkip.Location = new System.Drawing.Point(521, 368);
            this.txtSkip.Name = "txtSkip";
            this.txtSkip.Size = new System.Drawing.Size(74, 20);
            this.txtSkip.TabIndex = 67;
            this.txtSkip.Text = "0";
            // 
            // txtQty
            // 
            this.txtQty.Location = new System.Drawing.Point(522, 338);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(73, 20);
            this.txtQty.TabIndex = 66;
            this.txtQty.Text = "-1";
            // 
            // btnUnBlockAddress
            // 
            this.btnUnBlockAddress.Location = new System.Drawing.Point(238, 198);
            this.btnUnBlockAddress.Name = "btnUnBlockAddress";
            this.btnUnBlockAddress.Size = new System.Drawing.Size(56, 23);
            this.btnUnBlockAddress.TabIndex = 64;
            this.btnUnBlockAddress.Text = "unblock";
            this.btnUnBlockAddress.UseVisualStyleBackColor = true;
            this.btnUnBlockAddress.Click += new System.EventHandler(this.ButtonUnBlockAddressClick);
            // 
            // btnUnMuteAddress
            // 
            this.btnUnMuteAddress.Location = new System.Drawing.Point(173, 198);
            this.btnUnMuteAddress.Name = "btnUnMuteAddress";
            this.btnUnMuteAddress.Size = new System.Drawing.Size(59, 23);
            this.btnUnMuteAddress.TabIndex = 63;
            this.btnUnMuteAddress.Text = "unmute";
            this.btnUnMuteAddress.UseVisualStyleBackColor = true;
            this.btnUnMuteAddress.Click += new System.EventHandler(this.ButtonUnMuteAddressClick);
            // 
            // btnMuteAddress
            // 
            this.btnMuteAddress.Location = new System.Drawing.Point(112, 198);
            this.btnMuteAddress.Name = "btnMuteAddress";
            this.btnMuteAddress.Size = new System.Drawing.Size(58, 23);
            this.btnMuteAddress.TabIndex = 62;
            this.btnMuteAddress.Text = "mute";
            this.btnMuteAddress.UseVisualStyleBackColor = true;
            this.btnMuteAddress.Click += new System.EventHandler(this.ButtonMuteAddressClick);
            // 
            // btnBlockTransaction
            // 
            this.btnBlockTransaction.Location = new System.Drawing.Point(56, 126);
            this.btnBlockTransaction.Name = "btnBlockTransaction";
            this.btnBlockTransaction.Size = new System.Drawing.Size(102, 23);
            this.btnBlockTransaction.TabIndex = 61;
            this.btnBlockTransaction.Text = "delete transaction";
            this.btnBlockTransaction.UseVisualStyleBackColor = true;
            this.btnBlockTransaction.Click += new System.EventHandler(this.ButtonBlockTransactionIdClick);
            // 
            // btnBlockAddress
            // 
            this.btnBlockAddress.Location = new System.Drawing.Point(56, 198);
            this.btnBlockAddress.Name = "btnBlockAddress";
            this.btnBlockAddress.Size = new System.Drawing.Size(53, 23);
            this.btnBlockAddress.TabIndex = 60;
            this.btnBlockAddress.Text = "block address";
            this.btnBlockAddress.UseVisualStyleBackColor = true;
            this.btnBlockAddress.Click += new System.EventHandler(this.ButtonBlockAddressClick);
            // 
            // btnProfileURN
            // 
            this.btnProfileURN.Location = new System.Drawing.Point(97, 274);
            this.btnProfileURN.Name = "btnProfileURN";
            this.btnProfileURN.Size = new System.Drawing.Size(77, 23);
            this.btnProfileURN.TabIndex = 59;
            this.btnProfileURN.Text = "profile urn";
            this.btnProfileURN.UseVisualStyleBackColor = true;
            this.btnProfileURN.Click += new System.EventHandler(this.ButtonProfileURNClick);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(13, 274);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(79, 23);
            this.button4.TabIndex = 58;
            this.button4.Text = "get profile";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.ButtonGetProfileByAddressClick);
            // 
            // btnObjectURN
            // 
            this.btnObjectURN.Location = new System.Drawing.Point(182, 305);
            this.btnObjectURN.Name = "btnObjectURN";
            this.btnObjectURN.Size = new System.Drawing.Size(81, 23);
            this.btnObjectURN.TabIndex = 57;
            this.btnObjectURN.Text = "object urn";
            this.btnObjectURN.UseVisualStyleBackColor = true;
            this.btnObjectURN.Click += new System.EventHandler(this.ButtonGetObjectByURNClick);
            // 
            // txtGetValue
            // 
            this.txtGetValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGetValue.BackColor = System.Drawing.SystemColors.Control;
            this.txtGetValue.Location = new System.Drawing.Point(612, 12);
            this.txtGetValue.Name = "txtGetValue";
            this.txtGetValue.Size = new System.Drawing.Size(530, 383);
            this.txtGetValue.TabIndex = 56;
            this.txtGetValue.Text = "";
            // 
            // chkVerbose
            // 
            this.chkVerbose.AutoSize = true;
            this.chkVerbose.Location = new System.Drawing.Point(534, 198);
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.Size = new System.Drawing.Size(64, 17);
            this.chkVerbose.TabIndex = 55;
            this.chkVerbose.Text = "verbose";
            this.chkVerbose.UseVisualStyleBackColor = true;
            // 
            // btnGetObjectsByKeyword
            // 
            this.btnGetObjectsByKeyword.Location = new System.Drawing.Point(354, 303);
            this.btnGetObjectsByKeyword.Name = "btnGetObjectsByKeyword";
            this.btnGetObjectsByKeyword.Size = new System.Drawing.Size(79, 23);
            this.btnGetObjectsByKeyword.TabIndex = 53;
            this.btnGetObjectsByKeyword.Text = "by keyword";
            this.btnGetObjectsByKeyword.UseVisualStyleBackColor = true;
            this.btnGetObjectsByKeyword.Click += new System.EventHandler(this.ButtonGetObjectsByKeywordClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(269, 303);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 23);
            this.button1.TabIndex = 52;
            this.button1.Text = "get objects";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ButtonGetObjectsByAddressClick);
            // 
            // btnGetCreated
            // 
            this.btnGetCreated.Location = new System.Drawing.Point(521, 304);
            this.btnGetCreated.Name = "btnGetCreated";
            this.btnGetCreated.Size = new System.Drawing.Size(77, 23);
            this.btnGetCreated.TabIndex = 51;
            this.btnGetCreated.Text = "obj created";
            this.btnGetCreated.UseVisualStyleBackColor = true;
            this.btnGetCreated.Click += new System.EventHandler(this.ButtonGetCreatedClick);
            // 
            // btnGetOwned
            // 
            this.btnGetOwned.Location = new System.Drawing.Point(439, 303);
            this.btnGetOwned.Name = "btnGetOwned";
            this.btnGetOwned.Size = new System.Drawing.Size(76, 23);
            this.btnGetOwned.TabIndex = 50;
            this.btnGetOwned.Text = "obj owned";
            this.btnGetOwned.UseVisualStyleBackColor = true;
            this.btnGetOwned.Click += new System.EventHandler(this.ButtonGetOwnedClick);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(107, 375);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(30, 13);
            this.lblTotal.TabIndex = 49;
            this.lblTotal.Text = "total:";
            // 
            // btnGetObject
            // 
            this.btnGetObject.Location = new System.Drawing.Point(12, 303);
            this.btnGetObject.Name = "btnGetObject";
            this.btnGetObject.Size = new System.Drawing.Size(79, 23);
            this.btnGetObject.TabIndex = 44;
            this.btnGetObject.Text = "get object";
            this.btnGetObject.UseVisualStyleBackColor = true;
            this.btnGetObject.Click += new System.EventHandler(this.ButtonGetObjectByAddressClick);
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(240, 126);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(59, 23);
            this.btnDecrypt.TabIndex = 43;
            this.btnDecrypt.Text = "decrypt";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.ButtonDecryptTransactionIdClick);
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.Location = new System.Drawing.Point(176, 126);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(56, 23);
            this.btnEncrypt.TabIndex = 42;
            this.btnEncrypt.Text = "encrypt";
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.ButtonEncryptTransactionIdClick);
            // 
            // WorkBench
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 732);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(1072, 687);
            this.Name = "WorkBench";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Workbench";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.WorkBench_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.WorkBench_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.dgTransactions)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
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
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtVersionByte;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.Label lblKbs;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Button btnEncrypt;
        private Button btnDecrypt;
        private Button btnGetObject;
        private Label lblTotal;
        private Button btnGetCreated;
        private Button btnGetOwned;
        private Button button1;
        private Button btnGetObjectsByKeyword;
        private CheckBox chkVerbose;
        private RichTextBox txtGetValue;
        private Button btnObjectURN;
        private Button button4;
        private Button btnProfileURN;
        private Button btnMuteAddress;
        private Button btnBlockTransaction;
        private Button btnBlockAddress;
        private Button btnUnBlockAddress;
        private Button btnUnMuteAddress;
        private Label label2;
        private Label label1;
        private TextBox txtSkip;
        private TextBox txtQty;
        private Button btnGetFoundObjects;
        private Button ButtonGetObjectByTransactionId;
        private Button ButtonGetPrivateMessages;
        private Button ButtonGetPublicMessages;
        private Button ButtonGetPublicKeys;
        private Button btnGetLocalProfiles;
        private Button btnCollections;
        private Button btnGetInquiry;
        private Button btnGetInquiries;
        private Button btnInquiriesCreated;
        private Button btnGetInquiriesByKeyword;
        private Button btnGetInquiryByTransactionId;
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
        private DataGridViewTextBoxColumn BlockHeight;
        private DataGridViewTextBoxColumn buildtime;
    }
}

