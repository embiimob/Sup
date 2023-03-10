namespace SUP
{
    partial class ObjectMint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectMint));
            this.lblRemainingChars = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.flowOwners = new System.Windows.Forms.FlowLayoutPanel();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnObjectCreators = new System.Windows.Forms.Button();
            this.flowCreators = new System.Windows.Forms.FlowLayoutPanel();
            this.btnObjectOwners = new System.Windows.Forms.Button();
            this.btnObjectAttributes = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowPanel = new System.Windows.Forms.Panel();
            this.webviewer = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.btnObjectAddress = new System.Windows.Forms.Button();
            this.txtObjectAddress = new System.Windows.Forms.TextBox();
            this.btnMint = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnObjectDescription = new System.Windows.Forms.Button();
            this.btnObjectName = new System.Windows.Forms.Button();
            this.flowAttribute = new System.Windows.Forms.FlowLayoutPanel();
            this.btnObjectURI = new System.Windows.Forms.Button();
            this.btnObjectURN = new System.Windows.Forms.Button();
            this.btnObjectImage = new System.Windows.Forms.Button();
            this.txtURI = new System.Windows.Forms.TextBox();
            this.txtURN = new System.Windows.Forms.TextBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtIMG = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtAddressListJSON = new System.Windows.Forms.TextBox();
            this.btnMaximum = new System.Windows.Forms.Button();
            this.txtMaximum = new System.Windows.Forms.TextBox();
            this.lblIMGBlockDate = new System.Windows.Forms.Label();
            this.lblURNBlockDate = new System.Windows.Forms.Label();
            this.lblASCIIURN = new System.Windows.Forms.Label();
            this.flowKeywords = new System.Windows.Forms.FlowLayoutPanel();
            this.btnObjectKeywords = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webviewer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRemainingChars
            // 
            this.lblRemainingChars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRemainingChars.AutoSize = true;
            this.lblRemainingChars.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemainingChars.Location = new System.Drawing.Point(474, 572);
            this.lblRemainingChars.Name = "lblRemainingChars";
            this.lblRemainingChars.Size = new System.Drawing.Size(52, 29);
            this.lblRemainingChars.TabIndex = 180;
            this.lblRemainingChars.Text = "880";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(602, 590);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(550, 27);
            this.textBox1.TabIndex = 179;
            this.textBox1.Tag = "";
            // 
            // flowOwners
            // 
            this.flowOwners.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowOwners.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowOwners.Location = new System.Drawing.Point(602, 698);
            this.flowOwners.Name = "flowOwners";
            this.flowOwners.Size = new System.Drawing.Size(429, 53);
            this.flowOwners.TabIndex = 175;
            this.flowOwners.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.flowOwners_ControlAdded);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Enabled = false;
            this.btnEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnEdit.Location = new System.Drawing.Point(600, 769);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(109, 53);
            this.btnEdit.TabIndex = 15;
            this.btnEdit.Text = "EDIT";
            this.btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnObjectCreators
            // 
            this.btnObjectCreators.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectCreators.Enabled = false;
            this.btnObjectCreators.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnObjectCreators.Location = new System.Drawing.Point(1043, 624);
            this.btnObjectCreators.Name = "btnObjectCreators";
            this.btnObjectCreators.Size = new System.Drawing.Size(109, 53);
            this.btnObjectCreators.TabIndex = 10;
            this.btnObjectCreators.Text = "✅ CRE";
            this.btnObjectCreators.UseVisualStyleBackColor = true;
            this.btnObjectCreators.Click += new System.EventHandler(this.btnObjectCreators_Click);
            // 
            // flowCreators
            // 
            this.flowCreators.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowCreators.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowCreators.Location = new System.Drawing.Point(602, 628);
            this.flowCreators.Name = "flowCreators";
            this.flowCreators.Size = new System.Drawing.Size(429, 49);
            this.flowCreators.TabIndex = 173;
            this.flowCreators.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.flowCreators_ControlAdded);
            // 
            // btnObjectOwners
            // 
            this.btnObjectOwners.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectOwners.Enabled = false;
            this.btnObjectOwners.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnObjectOwners.Location = new System.Drawing.Point(1043, 698);
            this.btnObjectOwners.Name = "btnObjectOwners";
            this.btnObjectOwners.Size = new System.Drawing.Size(109, 53);
            this.btnObjectOwners.TabIndex = 11;
            this.btnObjectOwners.Text = "✅ OWN";
            this.btnObjectOwners.UseVisualStyleBackColor = true;
            this.btnObjectOwners.Click += new System.EventHandler(this.btnObjectOwners_Click);
            // 
            // btnObjectAttributes
            // 
            this.btnObjectAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectAttributes.Enabled = false;
            this.btnObjectAttributes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnObjectAttributes.Location = new System.Drawing.Point(469, 698);
            this.btnObjectAttributes.Name = "btnObjectAttributes";
            this.btnObjectAttributes.Size = new System.Drawing.Size(109, 53);
            this.btnObjectAttributes.TabIndex = 8;
            this.btnObjectAttributes.Text = "✅ ATR";
            this.btnObjectAttributes.UseVisualStyleBackColor = true;
            this.btnObjectAttributes.Click += new System.EventHandler(this.btnObjectAttributes_Click);
            // 
            // btnScan
            // 
            this.btnScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScan.Location = new System.Drawing.Point(736, 769);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(109, 53);
            this.btnScan.TabIndex = 14;
            this.btnScan.Text = "🔍";
            this.btnScan.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(602, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(550, 550);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 167;
            this.pictureBox1.TabStop = false;
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Location = new System.Drawing.Point(602, 44);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(550, 550);
            this.flowPanel.TabIndex = 169;
            // 
            // webviewer
            // 
            this.webviewer.AllowExternalDrop = true;
            this.webviewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.webviewer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.webviewer.CreationProperties = null;
            this.webviewer.DefaultBackgroundColor = System.Drawing.SystemColors.ControlDark;
            this.webviewer.Location = new System.Drawing.Point(602, 44);
            this.webviewer.Name = "webviewer";
            this.webviewer.Size = new System.Drawing.Size(550, 550);
            this.webviewer.TabIndex = 168;
            this.webviewer.ZoomFactor = 1D;
            // 
            // btnObjectAddress
            // 
            this.btnObjectAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectAddress.Enabled = false;
            this.btnObjectAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnObjectAddress.Location = new System.Drawing.Point(467, 142);
            this.btnObjectAddress.Name = "btnObjectAddress";
            this.btnObjectAddress.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.btnObjectAddress.Size = new System.Drawing.Size(109, 50);
            this.btnObjectAddress.TabIndex = 166;
            this.btnObjectAddress.Text = "💎";
            this.btnObjectAddress.UseVisualStyleBackColor = true;
            this.btnObjectAddress.Click += new System.EventHandler(this.btnObjectAddress_Click);
            // 
            // txtObjectAddress
            // 
            this.txtObjectAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtObjectAddress.Enabled = false;
            this.txtObjectAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtObjectAddress.Location = new System.Drawing.Point(220, 142);
            this.txtObjectAddress.Multiline = true;
            this.txtObjectAddress.Name = "txtObjectAddress";
            this.txtObjectAddress.Size = new System.Drawing.Size(238, 50);
            this.txtObjectAddress.TabIndex = 2;
            this.txtObjectAddress.TextChanged += new System.EventHandler(this.txtObjectAddress_TextChanged);
            // 
            // btnMint
            // 
            this.btnMint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMint.Enabled = false;
            this.btnMint.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnMint.Location = new System.Drawing.Point(910, 769);
            this.btnMint.Name = "btnMint";
            this.btnMint.Size = new System.Drawing.Size(109, 53);
            this.btnMint.TabIndex = 13;
            this.btnMint.Text = "MINT";
            this.btnMint.UseVisualStyleBackColor = true;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnPrint.Location = new System.Drawing.Point(1043, 769);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(109, 53);
            this.btnPrint.TabIndex = 12;
            this.btnPrint.Text = "PRINT";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.button12_Click);
            // 
            // btnObjectDescription
            // 
            this.btnObjectDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectDescription.Enabled = false;
            this.btnObjectDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnObjectDescription.Location = new System.Drawing.Point(467, 460);
            this.btnObjectDescription.Name = "btnObjectDescription";
            this.btnObjectDescription.Size = new System.Drawing.Size(109, 53);
            this.btnObjectDescription.TabIndex = 162;
            this.btnObjectDescription.Text = "✅ DSC";
            this.btnObjectDescription.UseVisualStyleBackColor = true;
            this.btnObjectDescription.Click += new System.EventHandler(this.btnObjectDescription_Click);
            // 
            // btnObjectName
            // 
            this.btnObjectName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnObjectName.Location = new System.Drawing.Point(466, 66);
            this.btnObjectName.Name = "btnObjectName";
            this.btnObjectName.Size = new System.Drawing.Size(109, 50);
            this.btnObjectName.TabIndex = 161;
            this.btnObjectName.Text = "✅ NME";
            this.btnObjectName.UseVisualStyleBackColor = true;
            this.btnObjectName.Click += new System.EventHandler(this.btnObjectName_Click);
            // 
            // flowAttribute
            // 
            this.flowAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowAttribute.Cursor = System.Windows.Forms.Cursors.Default;
            this.flowAttribute.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowAttribute.Location = new System.Drawing.Point(48, 698);
            this.flowAttribute.Name = "flowAttribute";
            this.flowAttribute.Size = new System.Drawing.Size(410, 53);
            this.flowAttribute.TabIndex = 160;
            this.flowAttribute.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.flowAttribute_ControlAdded);
            this.flowAttribute.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.flowAttribute_ControlRemoved);
            // 
            // btnObjectURI
            // 
            this.btnObjectURI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectURI.Enabled = false;
            this.btnObjectURI.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnObjectURI.Location = new System.Drawing.Point(467, 382);
            this.btnObjectURI.Name = "btnObjectURI";
            this.btnObjectURI.Size = new System.Drawing.Size(109, 58);
            this.btnObjectURI.TabIndex = 159;
            this.btnObjectURI.Text = "✅ URI";
            this.btnObjectURI.UseVisualStyleBackColor = true;
            this.btnObjectURI.Click += new System.EventHandler(this.btnObjectURI_Click);
            // 
            // btnObjectURN
            // 
            this.btnObjectURN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectURN.Enabled = false;
            this.btnObjectURN.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnObjectURN.Location = new System.Drawing.Point(467, 300);
            this.btnObjectURN.Name = "btnObjectURN";
            this.btnObjectURN.Size = new System.Drawing.Size(109, 57);
            this.btnObjectURN.TabIndex = 158;
            this.btnObjectURN.Text = "✅ URN";
            this.btnObjectURN.UseVisualStyleBackColor = true;
            this.btnObjectURN.Click += new System.EventHandler(this.btnObjectURN_Click);
            // 
            // btnObjectImage
            // 
            this.btnObjectImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectImage.Enabled = false;
            this.btnObjectImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnObjectImage.Location = new System.Drawing.Point(467, 217);
            this.btnObjectImage.Name = "btnObjectImage";
            this.btnObjectImage.Size = new System.Drawing.Size(109, 57);
            this.btnObjectImage.TabIndex = 157;
            this.btnObjectImage.Text = "✅ IMG";
            this.btnObjectImage.UseVisualStyleBackColor = true;
            this.btnObjectImage.Click += new System.EventHandler(this.btnObjectImage_Click);
            // 
            // txtURI
            // 
            this.txtURI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtURI.Enabled = false;
            this.txtURI.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtURI.Location = new System.Drawing.Point(48, 383);
            this.txtURI.Multiline = true;
            this.txtURI.Name = "txtURI";
            this.txtURI.Size = new System.Drawing.Size(410, 57);
            this.txtURI.TabIndex = 5;
            this.txtURI.TextChanged += new System.EventHandler(this.txtURI_TextChanged);
            // 
            // txtURN
            // 
            this.txtURN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtURN.Enabled = false;
            this.txtURN.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtURN.Location = new System.Drawing.Point(48, 300);
            this.txtURN.Multiline = true;
            this.txtURN.Name = "txtURN";
            this.txtURN.Size = new System.Drawing.Size(410, 57);
            this.txtURN.TabIndex = 4;
            this.txtURN.TextChanged += new System.EventHandler(this.txtURN_TextChanged);
            // 
            // txtTitle
            // 
            this.txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(222, 66);
            this.txtTitle.Multiline = true;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(238, 50);
            this.txtTitle.TabIndex = 1;
            this.txtTitle.TextChanged += new System.EventHandler(this.txtTitle_TextChanged);
            // 
            // txtIMG
            // 
            this.txtIMG.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIMG.Enabled = false;
            this.txtIMG.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIMG.Location = new System.Drawing.Point(45, 217);
            this.txtIMG.Multiline = true;
            this.txtIMG.Name = "txtIMG";
            this.txtIMG.Size = new System.Drawing.Size(413, 57);
            this.txtIMG.TabIndex = 3;
            this.txtIMG.TextChanged += new System.EventHandler(this.txtIMG_TextChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(48, 42);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(150, 150);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 152;
            this.pictureBox2.TabStop = false;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Enabled = false;
            this.txtDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescription.Location = new System.Drawing.Point(48, 460);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(410, 141);
            this.txtDescription.TabIndex = 6;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // txtAddressListJSON
            // 
            this.txtAddressListJSON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAddressListJSON.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddressListJSON.Location = new System.Drawing.Point(688, 381);
            this.txtAddressListJSON.Multiline = true;
            this.txtAddressListJSON.Name = "txtAddressListJSON";
            this.txtAddressListJSON.Size = new System.Drawing.Size(344, 153);
            this.txtAddressListJSON.TabIndex = 181;
            this.txtAddressListJSON.Text = resources.GetString("txtAddressListJSON.Text");
            // 
            // btnMaximum
            // 
            this.btnMaximum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMaximum.Enabled = false;
            this.btnMaximum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnMaximum.Location = new System.Drawing.Point(469, 621);
            this.btnMaximum.Name = "btnMaximum";
            this.btnMaximum.Size = new System.Drawing.Size(109, 58);
            this.btnMaximum.TabIndex = 183;
            this.btnMaximum.Text = "✅ MAX";
            this.btnMaximum.UseVisualStyleBackColor = true;
            this.btnMaximum.Click += new System.EventHandler(this.btnMaximum_Click);
            // 
            // txtMaximum
            // 
            this.txtMaximum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaximum.Enabled = false;
            this.txtMaximum.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaximum.Location = new System.Drawing.Point(48, 622);
            this.txtMaximum.Multiline = true;
            this.txtMaximum.Name = "txtMaximum";
            this.txtMaximum.Size = new System.Drawing.Size(410, 57);
            this.txtMaximum.TabIndex = 7;
            this.txtMaximum.Tag = "";
            this.txtMaximum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMaximum.WordWrap = false;
            // 
            // lblIMGBlockDate
            // 
            this.lblIMGBlockDate.AutoSize = true;
            this.lblIMGBlockDate.Location = new System.Drawing.Point(45, 201);
            this.lblIMGBlockDate.Name = "lblIMGBlockDate";
            this.lblIMGBlockDate.Size = new System.Drawing.Size(0, 13);
            this.lblIMGBlockDate.TabIndex = 184;
            this.lblIMGBlockDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblURNBlockDate
            // 
            this.lblURNBlockDate.AutoSize = true;
            this.lblURNBlockDate.Location = new System.Drawing.Point(45, 284);
            this.lblURNBlockDate.Name = "lblURNBlockDate";
            this.lblURNBlockDate.Size = new System.Drawing.Size(0, 13);
            this.lblURNBlockDate.TabIndex = 185;
            this.lblURNBlockDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblASCIIURN
            // 
            this.lblASCIIURN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblASCIIURN.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblASCIIURN.Location = new System.Drawing.Point(602, 44);
            this.lblASCIIURN.Name = "lblASCIIURN";
            this.lblASCIIURN.Size = new System.Drawing.Size(550, 550);
            this.lblASCIIURN.TabIndex = 186;
            this.lblASCIIURN.Text = "enter object name to begin";
            this.lblASCIIURN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowKeywords
            // 
            this.flowKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowKeywords.Cursor = System.Windows.Forms.Cursors.Default;
            this.flowKeywords.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowKeywords.Location = new System.Drawing.Point(48, 771);
            this.flowKeywords.Name = "flowKeywords";
            this.flowKeywords.Size = new System.Drawing.Size(410, 53);
            this.flowKeywords.TabIndex = 161;
            this.flowKeywords.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.flowKeyword_ControlAdded);
            this.flowKeywords.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.flowKeyword_ControlRemoved);
            // 
            // btnObjectKeywords
            // 
            this.btnObjectKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnObjectKeywords.Enabled = false;
            this.btnObjectKeywords.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnObjectKeywords.Location = new System.Drawing.Point(469, 774);
            this.btnObjectKeywords.Name = "btnObjectKeywords";
            this.btnObjectKeywords.Size = new System.Drawing.Size(109, 53);
            this.btnObjectKeywords.TabIndex = 9;
            this.btnObjectKeywords.Text = "✅ KEY";
            this.btnObjectKeywords.UseVisualStyleBackColor = true;
            this.btnObjectKeywords.Click += new System.EventHandler(this.btnObjectKeywords_Click);
            // 
            // ObjectMint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1198, 864);
            this.Controls.Add(this.btnObjectKeywords);
            this.Controls.Add(this.flowKeywords);
            this.Controls.Add(this.lblASCIIURN);
            this.Controls.Add(this.webviewer);
            this.Controls.Add(this.lblURNBlockDate);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblIMGBlockDate);
            this.Controls.Add(this.btnMaximum);
            this.Controls.Add(this.txtMaximum);
            this.Controls.Add(this.lblRemainingChars);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.flowOwners);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnObjectCreators);
            this.Controls.Add(this.flowCreators);
            this.Controls.Add(this.btnObjectOwners);
            this.Controls.Add(this.btnObjectAttributes);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.btnObjectAddress);
            this.Controls.Add(this.txtObjectAddress);
            this.Controls.Add(this.btnMint);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnObjectDescription);
            this.Controls.Add(this.btnObjectName);
            this.Controls.Add(this.flowAttribute);
            this.Controls.Add(this.btnObjectURI);
            this.Controls.Add(this.btnObjectURN);
            this.Controls.Add(this.btnObjectImage);
            this.Controls.Add(this.txtURI);
            this.Controls.Add(this.txtURN);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.txtIMG);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.txtAddressListJSON);
            this.MinimumSize = new System.Drawing.Size(1214, 903);
            this.Name = "ObjectMint";
            this.Text = "Sup!? Object Mint";
            this.Load += new System.EventHandler(this.ObjectMint_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webviewer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRemainingChars;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.FlowLayoutPanel flowOwners;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnObjectCreators;
        private System.Windows.Forms.FlowLayoutPanel flowCreators;
        private System.Windows.Forms.Button btnObjectOwners;
        private System.Windows.Forms.Button btnObjectAttributes;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel flowPanel;
        private Microsoft.Web.WebView2.WinForms.WebView2 webviewer;
        private System.Windows.Forms.Button btnObjectAddress;
        private System.Windows.Forms.TextBox txtObjectAddress;
        private System.Windows.Forms.Button btnMint;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnObjectDescription;
        private System.Windows.Forms.Button btnObjectName;
        private System.Windows.Forms.FlowLayoutPanel flowAttribute;
        private System.Windows.Forms.Button btnObjectURI;
        private System.Windows.Forms.Button btnObjectURN;
        private System.Windows.Forms.Button btnObjectImage;
        private System.Windows.Forms.TextBox txtURI;
        private System.Windows.Forms.TextBox txtURN;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtIMG;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtAddressListJSON;
        private System.Windows.Forms.Button btnMaximum;
        private System.Windows.Forms.TextBox txtMaximum;
        private System.Windows.Forms.Label lblIMGBlockDate;
        private System.Windows.Forms.Label lblURNBlockDate;
        private System.Windows.Forms.Label lblASCIIURN;
        private System.Windows.Forms.FlowLayoutPanel flowKeywords;
        private System.Windows.Forms.Button btnObjectKeywords;
    }
}