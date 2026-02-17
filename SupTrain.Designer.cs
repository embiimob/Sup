namespace SUP
{
    partial class SupTrain
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDiscover = new System.Windows.Forms.TabPage();
            this.flowJobList = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRefreshJobs = new System.Windows.Forms.Button();
            this.btnUseLatestCheckpoint = new System.Windows.Forms.Button();
            this.txtJobKeyword = new System.Windows.Forms.TextBox();
            this.lblJobKeyword = new System.Windows.Forms.Label();
            this.tabConfigure = new System.Windows.Forms.TabPage();
            this.btnValidate = new System.Windows.Forms.Button();
            this.cmbPrecision = new System.Windows.Forms.ComboBox();
            this.lblPrecision = new System.Windows.Forms.Label();
            this.txtBatchSize = new System.Windows.Forms.TextBox();
            this.lblBatchSize = new System.Windows.Forms.Label();
            this.txtLearningRate = new System.Windows.Forms.TextBox();
            this.lblLearningRate = new System.Windows.Forms.Label();
            this.txtEpochs = new System.Windows.Forms.TextBox();
            this.lblEpochs = new System.Windows.Forms.Label();
            this.lstDataPaths = new System.Windows.Forms.ListBox();
            this.btnRemoveData = new System.Windows.Forms.Button();
            this.btnAddFiles = new System.Windows.Forms.Button();
            this.btnAddFolder = new System.Windows.Forms.Button();
            this.lblDataSelection = new System.Windows.Forms.Label();
            this.txtActiveJob = new System.Windows.Forms.TextBox();
            this.lblActiveJob = new System.Windows.Forms.Label();
            this.tabRun = new System.Windows.Forms.TabPage();
            this.txtConsoleOutput = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnStopTraining = new System.Windows.Forms.Button();
            this.btnStartTraining = new System.Windows.Forms.Button();
            this.tabPublish = new System.Windows.Forms.TabPage();
            this.btnPublishUpdate = new System.Windows.Forms.Button();
            this.txtMetricsCID = new System.Windows.Forms.TextBox();
            this.lblMetricsCID = new System.Windows.Forms.Label();
            this.txtDeltaCID = new System.Windows.Forms.TextBox();
            this.lblDeltaCID = new System.Windows.Forms.Label();
            this.tabMonitor = new System.Windows.Forms.TabPage();
            this.flowMonitorList = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPinCheckpoint = new System.Windows.Forms.Button();
            this.btnFollowJob = new System.Windows.Forms.Button();
            this.btnRefreshMonitor = new System.Windows.Forms.Button();
            this.txtStatusLog = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabDiscover.SuspendLayout();
            this.tabConfigure.SuspendLayout();
            this.tabRun.SuspendLayout();
            this.tabPublish.SuspendLayout();
            this.tabMonitor.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtStatusLog);
            this.splitContainer1.Size = new System.Drawing.Size(1024, 768);
            this.splitContainer1.SplitterDistance = 600;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDiscover);
            this.tabControl1.Controls.Add(this.tabConfigure);
            this.tabControl1.Controls.Add(this.tabRun);
            this.tabControl1.Controls.Add(this.tabPublish);
            this.tabControl1.Controls.Add(this.tabMonitor);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1024, 600);
            this.tabControl1.TabIndex = 0;
            // 
            // tabDiscover
            // 
            this.tabDiscover.BackColor = System.Drawing.Color.Black;
            this.tabDiscover.Controls.Add(this.flowJobList);
            this.tabDiscover.Controls.Add(this.btnRefreshJobs);
            this.tabDiscover.Controls.Add(this.btnUseLatestCheckpoint);
            this.tabDiscover.Controls.Add(this.txtJobKeyword);
            this.tabDiscover.Controls.Add(this.lblJobKeyword);
            this.tabDiscover.ForeColor = System.Drawing.Color.White;
            this.tabDiscover.Location = new System.Drawing.Point(4, 22);
            this.tabDiscover.Name = "tabDiscover";
            this.tabDiscover.Padding = new System.Windows.Forms.Padding(3);
            this.tabDiscover.Size = new System.Drawing.Size(1016, 574);
            this.tabDiscover.TabIndex = 0;
            this.tabDiscover.Text = "Discover";
            // 
            // flowJobList
            // 
            this.flowJobList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowJobList.AutoScroll = true;
            this.flowJobList.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowJobList.Location = new System.Drawing.Point(6, 70);
            this.flowJobList.Name = "flowJobList";
            this.flowJobList.Size = new System.Drawing.Size(1004, 498);
            this.flowJobList.TabIndex = 4;
            this.flowJobList.WrapContents = false;
            // 
            // btnRefreshJobs
            // 
            this.btnRefreshJobs.ForeColor = System.Drawing.Color.Black;
            this.btnRefreshJobs.Location = new System.Drawing.Point(399, 30);
            this.btnRefreshJobs.Name = "btnRefreshJobs";
            this.btnRefreshJobs.Size = new System.Drawing.Size(120, 30);
            this.btnRefreshJobs.TabIndex = 3;
            this.btnRefreshJobs.Text = "Search Jobs";
            this.btnRefreshJobs.UseVisualStyleBackColor = true;
            this.btnRefreshJobs.Click += new System.EventHandler(this.btnRefreshJobs_Click);
            // 
            // btnUseLatestCheckpoint
            // 
            this.btnUseLatestCheckpoint.ForeColor = System.Drawing.Color.Black;
            this.btnUseLatestCheckpoint.Location = new System.Drawing.Point(525, 30);
            this.btnUseLatestCheckpoint.Name = "btnUseLatestCheckpoint";
            this.btnUseLatestCheckpoint.Size = new System.Drawing.Size(150, 30);
            this.btnUseLatestCheckpoint.TabIndex = 2;
            this.btnUseLatestCheckpoint.Text = "Use Latest Checkpoint";
            this.btnUseLatestCheckpoint.UseVisualStyleBackColor = true;
            this.btnUseLatestCheckpoint.Click += new System.EventHandler(this.btnUseLatestCheckpoint_Click);
            // 
            // txtJobKeyword
            // 
            this.txtJobKeyword.BackColor = System.Drawing.Color.Black;
            this.txtJobKeyword.ForeColor = System.Drawing.Color.White;
            this.txtJobKeyword.Location = new System.Drawing.Point(6, 35);
            this.txtJobKeyword.Name = "txtJobKeyword";
            this.txtJobKeyword.Size = new System.Drawing.Size(387, 20);
            this.txtJobKeyword.TabIndex = 1;
            // 
            // lblJobKeyword
            // 
            this.lblJobKeyword.AutoSize = true;
            this.lblJobKeyword.Location = new System.Drawing.Point(6, 12);
            this.lblJobKeyword.Name = "lblJobKeyword";
            this.lblJobKeyword.Size = new System.Drawing.Size(232, 13);
            this.lblJobKeyword.TabIndex = 0;
            this.lblJobKeyword.Text = "Job Keyword (e.g., #suptrain, #model:suplm):";
            // 
            // tabConfigure
            // 
            this.tabConfigure.BackColor = System.Drawing.Color.Black;
            this.tabConfigure.Controls.Add(this.btnValidate);
            this.tabConfigure.Controls.Add(this.cmbPrecision);
            this.tabConfigure.Controls.Add(this.lblPrecision);
            this.tabConfigure.Controls.Add(this.txtBatchSize);
            this.tabConfigure.Controls.Add(this.lblBatchSize);
            this.tabConfigure.Controls.Add(this.txtLearningRate);
            this.tabConfigure.Controls.Add(this.lblLearningRate);
            this.tabConfigure.Controls.Add(this.txtEpochs);
            this.tabConfigure.Controls.Add(this.lblEpochs);
            this.tabConfigure.Controls.Add(this.lstDataPaths);
            this.tabConfigure.Controls.Add(this.btnRemoveData);
            this.tabConfigure.Controls.Add(this.btnAddFiles);
            this.tabConfigure.Controls.Add(this.btnAddFolder);
            this.tabConfigure.Controls.Add(this.lblDataSelection);
            this.tabConfigure.Controls.Add(this.txtActiveJob);
            this.tabConfigure.Controls.Add(this.lblActiveJob);
            this.tabConfigure.ForeColor = System.Drawing.Color.White;
            this.tabConfigure.Location = new System.Drawing.Point(4, 22);
            this.tabConfigure.Name = "tabConfigure";
            this.tabConfigure.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigure.Size = new System.Drawing.Size(1016, 574);
            this.tabConfigure.TabIndex = 1;
            this.tabConfigure.Text = "Configure";
            // 
            // btnValidate
            // 
            this.btnValidate.ForeColor = System.Drawing.Color.Black;
            this.btnValidate.Location = new System.Drawing.Point(6, 530);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(150, 30);
            this.btnValidate.TabIndex = 15;
            this.btnValidate.Text = "Dry Run / Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // cmbPrecision
            // 
            this.cmbPrecision.BackColor = System.Drawing.Color.Black;
            this.cmbPrecision.ForeColor = System.Drawing.Color.White;
            this.cmbPrecision.FormattingEnabled = true;
            this.cmbPrecision.Items.AddRange(new object[] {
            "fp16",
            "bf16",
            "fp32"});
            this.cmbPrecision.Location = new System.Drawing.Point(6, 490);
            this.cmbPrecision.Name = "cmbPrecision";
            this.cmbPrecision.Size = new System.Drawing.Size(200, 21);
            this.cmbPrecision.TabIndex = 14;
            // 
            // lblPrecision
            // 
            this.lblPrecision.AutoSize = true;
            this.lblPrecision.Location = new System.Drawing.Point(6, 474);
            this.lblPrecision.Name = "lblPrecision";
            this.lblPrecision.Size = new System.Drawing.Size(56, 13);
            this.lblPrecision.TabIndex = 13;
            this.lblPrecision.Text = "Precision:";
            // 
            // txtBatchSize
            // 
            this.txtBatchSize.BackColor = System.Drawing.Color.Black;
            this.txtBatchSize.ForeColor = System.Drawing.Color.White;
            this.txtBatchSize.Location = new System.Drawing.Point(6, 445);
            this.txtBatchSize.Name = "txtBatchSize";
            this.txtBatchSize.Size = new System.Drawing.Size(200, 20);
            this.txtBatchSize.TabIndex = 12;
            this.txtBatchSize.Text = "1";
            // 
            // lblBatchSize
            // 
            this.lblBatchSize.AutoSize = true;
            this.lblBatchSize.Location = new System.Drawing.Point(6, 429);
            this.lblBatchSize.Name = "lblBatchSize";
            this.lblBatchSize.Size = new System.Drawing.Size(61, 13);
            this.lblBatchSize.TabIndex = 11;
            this.lblBatchSize.Text = "Batch Size:";
            // 
            // txtLearningRate
            // 
            this.txtLearningRate.BackColor = System.Drawing.Color.Black;
            this.txtLearningRate.ForeColor = System.Drawing.Color.White;
            this.txtLearningRate.Location = new System.Drawing.Point(6, 400);
            this.txtLearningRate.Name = "txtLearningRate";
            this.txtLearningRate.Size = new System.Drawing.Size(200, 20);
            this.txtLearningRate.TabIndex = 10;
            this.txtLearningRate.Text = "0.0001";
            // 
            // lblLearningRate
            // 
            this.lblLearningRate.AutoSize = true;
            this.lblLearningRate.Location = new System.Drawing.Point(6, 384);
            this.lblLearningRate.Name = "lblLearningRate";
            this.lblLearningRate.Size = new System.Drawing.Size(77, 13);
            this.lblLearningRate.TabIndex = 9;
            this.lblLearningRate.Text = "Learning Rate:";
            // 
            // txtEpochs
            // 
            this.txtEpochs.BackColor = System.Drawing.Color.Black;
            this.txtEpochs.ForeColor = System.Drawing.Color.White;
            this.txtEpochs.Location = new System.Drawing.Point(6, 355);
            this.txtEpochs.Name = "txtEpochs";
            this.txtEpochs.Size = new System.Drawing.Size(200, 20);
            this.txtEpochs.TabIndex = 8;
            this.txtEpochs.Text = "1";
            // 
            // lblEpochs
            // 
            this.lblEpochs.AutoSize = true;
            this.lblEpochs.Location = new System.Drawing.Point(6, 339);
            this.lblEpochs.Name = "lblEpochs";
            this.lblEpochs.Size = new System.Drawing.Size(72, 13);
            this.lblEpochs.TabIndex = 7;
            this.lblEpochs.Text = "Epochs/Steps:";
            // 
            // lstDataPaths
            // 
            this.lstDataPaths.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDataPaths.BackColor = System.Drawing.Color.Black;
            this.lstDataPaths.ForeColor = System.Drawing.Color.White;
            this.lstDataPaths.FormattingEnabled = true;
            this.lstDataPaths.Location = new System.Drawing.Point(6, 221);
            this.lstDataPaths.Name = "lstDataPaths";
            this.lstDataPaths.Size = new System.Drawing.Size(1004, 95);
            this.lstDataPaths.TabIndex = 6;
            // 
            // btnRemoveData
            // 
            this.btnRemoveData.ForeColor = System.Drawing.Color.Black;
            this.btnRemoveData.Location = new System.Drawing.Point(218, 185);
            this.btnRemoveData.Name = "btnRemoveData";
            this.btnRemoveData.Size = new System.Drawing.Size(100, 30);
            this.btnRemoveData.TabIndex = 5;
            this.btnRemoveData.Text = "Remove";
            this.btnRemoveData.UseVisualStyleBackColor = true;
            this.btnRemoveData.Click += new System.EventHandler(this.btnRemoveData_Click);
            // 
            // btnAddFiles
            // 
            this.btnAddFiles.ForeColor = System.Drawing.Color.Black;
            this.btnAddFiles.Location = new System.Drawing.Point(112, 185);
            this.btnAddFiles.Name = "btnAddFiles";
            this.btnAddFiles.Size = new System.Drawing.Size(100, 30);
            this.btnAddFiles.TabIndex = 4;
            this.btnAddFiles.Text = "Add Files";
            this.btnAddFiles.UseVisualStyleBackColor = true;
            this.btnAddFiles.Click += new System.EventHandler(this.btnAddFiles_Click);
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.ForeColor = System.Drawing.Color.Black;
            this.btnAddFolder.Location = new System.Drawing.Point(6, 185);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(100, 30);
            this.btnAddFolder.TabIndex = 3;
            this.btnAddFolder.Text = "Add Folder";
            this.btnAddFolder.UseVisualStyleBackColor = true;
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // lblDataSelection
            // 
            this.lblDataSelection.AutoSize = true;
            this.lblDataSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDataSelection.Location = new System.Drawing.Point(6, 160);
            this.lblDataSelection.Name = "lblDataSelection";
            this.lblDataSelection.Size = new System.Drawing.Size(153, 16);
            this.lblDataSelection.TabIndex = 2;
            this.lblDataSelection.Text = "Local Data Selection:";
            // 
            // txtActiveJob
            // 
            this.txtActiveJob.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtActiveJob.BackColor = System.Drawing.Color.Black;
            this.txtActiveJob.ForeColor = System.Drawing.Color.White;
            this.txtActiveJob.Location = new System.Drawing.Point(6, 30);
            this.txtActiveJob.Multiline = true;
            this.txtActiveJob.Name = "txtActiveJob";
            this.txtActiveJob.ReadOnly = true;
            this.txtActiveJob.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtActiveJob.Size = new System.Drawing.Size(1004, 120);
            this.txtActiveJob.TabIndex = 1;
            this.txtActiveJob.Text = "No active job selected. Go to Discover tab to select a job.";
            // 
            // lblActiveJob
            // 
            this.lblActiveJob.AutoSize = true;
            this.lblActiveJob.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveJob.Location = new System.Drawing.Point(6, 11);
            this.lblActiveJob.Name = "lblActiveJob";
            this.lblActiveJob.Size = new System.Drawing.Size(127, 16);
            this.lblActiveJob.TabIndex = 0;
            this.lblActiveJob.Text = "Active Job Details:";
            // 
            // tabRun
            // 
            this.tabRun.BackColor = System.Drawing.Color.Black;
            this.tabRun.Controls.Add(this.txtConsoleOutput);
            this.tabRun.Controls.Add(this.progressBar1);
            this.tabRun.Controls.Add(this.btnStopTraining);
            this.tabRun.Controls.Add(this.btnStartTraining);
            this.tabRun.ForeColor = System.Drawing.Color.White;
            this.tabRun.Location = new System.Drawing.Point(4, 22);
            this.tabRun.Name = "tabRun";
            this.tabRun.Size = new System.Drawing.Size(1016, 574);
            this.tabRun.TabIndex = 2;
            this.tabRun.Text = "Run";
            // 
            // txtConsoleOutput
            // 
            this.txtConsoleOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsoleOutput.BackColor = System.Drawing.Color.Black;
            this.txtConsoleOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsoleOutput.ForeColor = System.Drawing.Color.Lime;
            this.txtConsoleOutput.Location = new System.Drawing.Point(6, 80);
            this.txtConsoleOutput.Multiline = true;
            this.txtConsoleOutput.Name = "txtConsoleOutput";
            this.txtConsoleOutput.ReadOnly = true;
            this.txtConsoleOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConsoleOutput.Size = new System.Drawing.Size(1004, 488);
            this.txtConsoleOutput.TabIndex = 3;
            this.txtConsoleOutput.WordWrap = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 48);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(400, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // btnStopTraining
            // 
            this.btnStopTraining.Enabled = false;
            this.btnStopTraining.ForeColor = System.Drawing.Color.Black;
            this.btnStopTraining.Location = new System.Drawing.Point(162, 12);
            this.btnStopTraining.Name = "btnStopTraining";
            this.btnStopTraining.Size = new System.Drawing.Size(150, 30);
            this.btnStopTraining.TabIndex = 1;
            this.btnStopTraining.Text = "Stop";
            this.btnStopTraining.UseVisualStyleBackColor = true;
            this.btnStopTraining.Click += new System.EventHandler(this.btnStopTraining_Click);
            // 
            // btnStartTraining
            // 
            this.btnStartTraining.ForeColor = System.Drawing.Color.Black;
            this.btnStartTraining.Location = new System.Drawing.Point(6, 12);
            this.btnStartTraining.Name = "btnStartTraining";
            this.btnStartTraining.Size = new System.Drawing.Size(150, 30);
            this.btnStartTraining.TabIndex = 0;
            this.btnStartTraining.Text = "Start Training";
            this.btnStartTraining.UseVisualStyleBackColor = true;
            this.btnStartTraining.Click += new System.EventHandler(this.btnStartTraining_Click);
            // 
            // tabPublish
            // 
            this.tabPublish.BackColor = System.Drawing.Color.Black;
            this.tabPublish.Controls.Add(this.btnPublishUpdate);
            this.tabPublish.Controls.Add(this.txtMetricsCID);
            this.tabPublish.Controls.Add(this.lblMetricsCID);
            this.tabPublish.Controls.Add(this.txtDeltaCID);
            this.tabPublish.Controls.Add(this.lblDeltaCID);
            this.tabPublish.ForeColor = System.Drawing.Color.White;
            this.tabPublish.Location = new System.Drawing.Point(4, 22);
            this.tabPublish.Name = "tabPublish";
            this.tabPublish.Size = new System.Drawing.Size(1016, 574);
            this.tabPublish.TabIndex = 3;
            this.tabPublish.Text = "Publish";
            // 
            // btnPublishUpdate
            // 
            this.btnPublishUpdate.ForeColor = System.Drawing.Color.Black;
            this.btnPublishUpdate.Location = new System.Drawing.Point(6, 100);
            this.btnPublishUpdate.Name = "btnPublishUpdate";
            this.btnPublishUpdate.Size = new System.Drawing.Size(150, 30);
            this.btnPublishUpdate.TabIndex = 4;
            this.btnPublishUpdate.Text = "Publish Update";
            this.btnPublishUpdate.UseVisualStyleBackColor = true;
            this.btnPublishUpdate.Click += new System.EventHandler(this.btnPublishUpdate_Click);
            // 
            // txtMetricsCID
            // 
            this.txtMetricsCID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMetricsCID.BackColor = System.Drawing.Color.Black;
            this.txtMetricsCID.ForeColor = System.Drawing.Color.White;
            this.txtMetricsCID.Location = new System.Drawing.Point(6, 70);
            this.txtMetricsCID.Name = "txtMetricsCID";
            this.txtMetricsCID.ReadOnly = true;
            this.txtMetricsCID.Size = new System.Drawing.Size(1004, 20);
            this.txtMetricsCID.TabIndex = 3;
            // 
            // lblMetricsCID
            // 
            this.lblMetricsCID.AutoSize = true;
            this.lblMetricsCID.Location = new System.Drawing.Point(6, 54);
            this.lblMetricsCID.Name = "lblMetricsCID";
            this.lblMetricsCID.Size = new System.Drawing.Size(68, 13);
            this.lblMetricsCID.TabIndex = 2;
            this.lblMetricsCID.Text = "Metrics CID:";
            // 
            // txtDeltaCID
            // 
            this.txtDeltaCID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeltaCID.BackColor = System.Drawing.Color.Black;
            this.txtDeltaCID.ForeColor = System.Drawing.Color.White;
            this.txtDeltaCID.Location = new System.Drawing.Point(6, 25);
            this.txtDeltaCID.Name = "txtDeltaCID";
            this.txtDeltaCID.ReadOnly = true;
            this.txtDeltaCID.Size = new System.Drawing.Size(1004, 20);
            this.txtDeltaCID.TabIndex = 1;
            // 
            // lblDeltaCID
            // 
            this.lblDeltaCID.AutoSize = true;
            this.lblDeltaCID.Location = new System.Drawing.Point(6, 9);
            this.lblDeltaCID.Name = "lblDeltaCID";
            this.lblDeltaCID.Size = new System.Drawing.Size(57, 13);
            this.lblDeltaCID.TabIndex = 0;
            this.lblDeltaCID.Text = "Delta CID:";
            // 
            // tabMonitor
            // 
            this.tabMonitor.BackColor = System.Drawing.Color.Black;
            this.tabMonitor.Controls.Add(this.flowMonitorList);
            this.tabMonitor.Controls.Add(this.btnPinCheckpoint);
            this.tabMonitor.Controls.Add(this.btnFollowJob);
            this.tabMonitor.Controls.Add(this.btnRefreshMonitor);
            this.tabMonitor.ForeColor = System.Drawing.Color.White;
            this.tabMonitor.Location = new System.Drawing.Point(4, 22);
            this.tabMonitor.Name = "tabMonitor";
            this.tabMonitor.Size = new System.Drawing.Size(1016, 574);
            this.tabMonitor.TabIndex = 4;
            this.tabMonitor.Text = "Monitor";
            // 
            // flowMonitorList
            // 
            this.flowMonitorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowMonitorList.AutoScroll = true;
            this.flowMonitorList.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowMonitorList.Location = new System.Drawing.Point(6, 48);
            this.flowMonitorList.Name = "flowMonitorList";
            this.flowMonitorList.Size = new System.Drawing.Size(1004, 520);
            this.flowMonitorList.TabIndex = 3;
            this.flowMonitorList.WrapContents = false;
            // 
            // btnPinCheckpoint
            // 
            this.btnPinCheckpoint.ForeColor = System.Drawing.Color.Black;
            this.btnPinCheckpoint.Location = new System.Drawing.Point(288, 12);
            this.btnPinCheckpoint.Name = "btnPinCheckpoint";
            this.btnPinCheckpoint.Size = new System.Drawing.Size(150, 30);
            this.btnPinCheckpoint.TabIndex = 2;
            this.btnPinCheckpoint.Text = "Pin Latest Checkpoint";
            this.btnPinCheckpoint.UseVisualStyleBackColor = true;
            this.btnPinCheckpoint.Click += new System.EventHandler(this.btnPinCheckpoint_Click);
            // 
            // btnFollowJob
            // 
            this.btnFollowJob.ForeColor = System.Drawing.Color.Black;
            this.btnFollowJob.Location = new System.Drawing.Point(132, 12);
            this.btnFollowJob.Name = "btnFollowJob";
            this.btnFollowJob.Size = new System.Drawing.Size(150, 30);
            this.btnFollowJob.TabIndex = 1;
            this.btnFollowJob.Text = "Follow Job Keywords";
            this.btnFollowJob.UseVisualStyleBackColor = true;
            this.btnFollowJob.Click += new System.EventHandler(this.btnFollowJob_Click);
            // 
            // btnRefreshMonitor
            // 
            this.btnRefreshMonitor.ForeColor = System.Drawing.Color.Black;
            this.btnRefreshMonitor.Location = new System.Drawing.Point(6, 12);
            this.btnRefreshMonitor.Name = "btnRefreshMonitor";
            this.btnRefreshMonitor.Size = new System.Drawing.Size(120, 30);
            this.btnRefreshMonitor.TabIndex = 0;
            this.btnRefreshMonitor.Text = "Refresh";
            this.btnRefreshMonitor.UseVisualStyleBackColor = true;
            this.btnRefreshMonitor.Click += new System.EventHandler(this.btnRefreshMonitor_Click);
            // 
            // txtStatusLog
            // 
            this.txtStatusLog.BackColor = System.Drawing.Color.Black;
            this.txtStatusLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStatusLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStatusLog.ForeColor = System.Drawing.Color.Yellow;
            this.txtStatusLog.Location = new System.Drawing.Point(0, 0);
            this.txtStatusLog.Multiline = true;
            this.txtStatusLog.Name = "txtStatusLog";
            this.txtStatusLog.ReadOnly = true;
            this.txtStatusLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStatusLog.Size = new System.Drawing.Size(1024, 164);
            this.txtStatusLog.TabIndex = 0;
            this.txtStatusLog.WordWrap = false;
            // 
            // SupTrain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SupTrain";
            this.Text = "SupTrain - Decentralized AI Training";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SupTrain_FormClosing);
            this.Load += new System.EventHandler(this.SupTrain_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabDiscover.ResumeLayout(false);
            this.tabDiscover.PerformLayout();
            this.tabConfigure.ResumeLayout(false);
            this.tabConfigure.PerformLayout();
            this.tabRun.ResumeLayout(false);
            this.tabRun.PerformLayout();
            this.tabPublish.ResumeLayout(false);
            this.tabPublish.PerformLayout();
            this.tabMonitor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDiscover;
        private System.Windows.Forms.TabPage tabConfigure;
        private System.Windows.Forms.TabPage tabRun;
        private System.Windows.Forms.TabPage tabPublish;
        private System.Windows.Forms.TabPage tabMonitor;
        private System.Windows.Forms.TextBox txtStatusLog;
        private System.Windows.Forms.TextBox txtJobKeyword;
        private System.Windows.Forms.Label lblJobKeyword;
        private System.Windows.Forms.Button btnUseLatestCheckpoint;
        private System.Windows.Forms.Button btnRefreshJobs;
        private System.Windows.Forms.FlowLayoutPanel flowJobList;
        private System.Windows.Forms.Label lblActiveJob;
        private System.Windows.Forms.TextBox txtActiveJob;
        private System.Windows.Forms.Label lblDataSelection;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnAddFiles;
        private System.Windows.Forms.Button btnRemoveData;
        private System.Windows.Forms.ListBox lstDataPaths;
        private System.Windows.Forms.Label lblEpochs;
        private System.Windows.Forms.TextBox txtEpochs;
        private System.Windows.Forms.Label lblLearningRate;
        private System.Windows.Forms.TextBox txtLearningRate;
        private System.Windows.Forms.Label lblBatchSize;
        private System.Windows.Forms.TextBox txtBatchSize;
        private System.Windows.Forms.Label lblPrecision;
        private System.Windows.Forms.ComboBox cmbPrecision;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Button btnStartTraining;
        private System.Windows.Forms.Button btnStopTraining;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtConsoleOutput;
        private System.Windows.Forms.Label lblDeltaCID;
        private System.Windows.Forms.TextBox txtDeltaCID;
        private System.Windows.Forms.Label lblMetricsCID;
        private System.Windows.Forms.TextBox txtMetricsCID;
        private System.Windows.Forms.Button btnPublishUpdate;
        private System.Windows.Forms.Button btnRefreshMonitor;
        private System.Windows.Forms.Button btnFollowJob;
        private System.Windows.Forms.Button btnPinCheckpoint;
        private System.Windows.Forms.FlowLayoutPanel flowMonitorList;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
