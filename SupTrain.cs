using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUP
{
    public partial class SupTrain : Form
    {
        private string searchKey;
        private bool testnet;
        private string mainnetURL;
        private string mainnetLogin;
        private string mainnetPassword;
        private string mainnetVersionByte;
        private Process trainingProcess;

        // Active job state
        private string activeJobId;
        private string activeModelSlug;
        private int activeRound;
        private string activeBaseCkptCID;
        private string activeManifestCID;

        public SupTrain(string searchKey = "#suptrain", bool testnet = true)
        {
            InitializeComponent();
            this.searchKey = searchKey;
            this.testnet = testnet;

            if (!testnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";
            }
            else
            {
                mainnetURL = @"http://127.0.0.1:18332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "111";
            }
        }

        private void SupTrain_Load(object sender, EventArgs e)
        {
            LogStatus("SupTrain module loaded.");
            LogStatus($"Search key: {searchKey}");
            LogStatus($"Network: {(testnet ? "Testnet" : "Mainnet")}");

            txtJobKeyword.Text = searchKey;
            cmbPrecision.SelectedIndex = 0; // Default to fp16

            // Add mouse wheel support for flow panels
            flowJobList.MouseWheel += (s, ev) => { };
            flowMonitorList.MouseWheel += (s, ev) => { };
        }

        private void SupTrain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Clean up training process if running
            if (trainingProcess != null && !trainingProcess.HasExited)
            {
                try
                {
                    trainingProcess.Kill();
                }
                catch { }
            }
        }

        #region Discover Tab

        private void btnRefreshJobs_Click(object sender, EventArgs e)
        {
            LogStatus($"Searching for jobs with keyword: {txtJobKeyword.Text}");
            flowJobList.Controls.Clear();

            Task.Run(() =>
            {
                // TODO: Implement job discovery using SupTrainService
                // For now, show placeholder
                this.Invoke((MethodInvoker)delegate
                {
                    var panel = CreateJobCard(
                        "Example Job", 
                        "suplm", 
                        "job123", 
                        1, 
                        "QmExampleBaseCID...", 
                        "QmExampleManifestCID...",
                        DateTime.Now.ToString()
                    );
                    flowJobList.Controls.Add(panel);
                    LogStatus("Job search complete. Found 1 example job.");
                });
            });
        }

        private Panel CreateJobCard(string jobName, string modelSlug, string jobId, int round, 
            string baseCkptCID, string manifestCID, string createdDate)
        {
            var panel = new Panel
            {
                Width = flowJobList.Width - 30,
                Height = 120,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(30, 30, 30),
                Cursor = Cursors.Hand
            };

            var lblName = new Label
            {
                Text = $"Job: {jobName}",
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var lblModel = new Label
            {
                Text = $"Model: {modelSlug} | Round: {round}",
                ForeColor = Color.LightGray,
                Location = new Point(10, 35),
                AutoSize = true
            };

            var lblCheckpoint = new Label
            {
                Text = $"Base Checkpoint: {TruncateCID(baseCkptCID, 30)}...",
                ForeColor = Color.LightGray,
                Location = new Point(10, 55),
                AutoSize = true
            };

            var lblDate = new Label
            {
                Text = $"Created: {createdDate}",
                ForeColor = Color.Gray,
                Location = new Point(10, 75),
                AutoSize = true
            };

            panel.Controls.AddRange(new Control[] { lblName, lblModel, lblCheckpoint, lblDate });

            panel.Click += (s, e) =>
            {
                SelectJob(jobId, modelSlug, round, baseCkptCID, manifestCID);
            };

            return panel;
        }

        private void SelectJob(string jobId, string modelSlug, int round, string baseCkptCID, string manifestCID)
        {
            activeJobId = jobId;
            activeModelSlug = modelSlug;
            activeRound = round;
            activeBaseCkptCID = baseCkptCID;
            activeManifestCID = manifestCID;

            txtActiveJob.Text = $"Job ID: {jobId}\r\n" +
                                $"Model: {modelSlug}\r\n" +
                                $"Round: {round}\r\n" +
                                $"Base Checkpoint CID: {baseCkptCID}\r\n" +
                                $"Manifest CID: {manifestCID}\r\n";

            LogStatus($"Selected job: {jobId}");
            tabControl1.SelectedTab = tabConfigure;
        }

        private void btnUseLatestCheckpoint_Click(object sender, EventArgs e)
        {
            LogStatus("Searching for latest checkpoint...");
            // TODO: Implement latest checkpoint discovery
            MessageBox.Show("Feature coming soon: Will search for latest #checkpoint announcement", 
                "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Configure Tab

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select training data folder";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!lstDataPaths.Items.Contains(folderDialog.SelectedPath))
                    {
                        lstDataPaths.Items.Add(folderDialog.SelectedPath);
                        LogStatus($"Added folder: {folderDialog.SelectedPath}");
                    }
                }
            }
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Multiselect = true;
                fileDialog.Title = "Select training data files";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in fileDialog.FileNames)
                    {
                        if (!lstDataPaths.Items.Contains(file))
                        {
                            lstDataPaths.Items.Add(file);
                            LogStatus($"Added file: {file}");
                        }
                    }
                }
            }
        }

        private void btnRemoveData_Click(object sender, EventArgs e)
        {
            if (lstDataPaths.SelectedIndex >= 0)
            {
                var item = lstDataPaths.SelectedItem.ToString();
                lstDataPaths.Items.RemoveAt(lstDataPaths.SelectedIndex);
                LogStatus($"Removed: {item}");
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            LogStatus("Validating configuration...");

            if (string.IsNullOrEmpty(activeJobId))
            {
                MessageBox.Show("Please select a job from the Discover tab first.", 
                    "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lstDataPaths.Items.Count == 0)
            {
                MessageBox.Show("Please add at least one training data source.", 
                    "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate numeric inputs
            if (!int.TryParse(txtEpochs.Text, out int epochs) || epochs < 1)
            {
                MessageBox.Show("Epochs must be a positive integer.", 
                    "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtLearningRate.Text, out double lr) || lr <= 0)
            {
                MessageBox.Show("Learning rate must be a positive number.", 
                    "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtBatchSize.Text, out int batchSize) || batchSize < 1)
            {
                MessageBox.Show("Batch size must be a positive integer.", 
                    "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbPrecision.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a precision option.", 
                    "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LogStatus("✓ Configuration validated successfully!");
            MessageBox.Show("Configuration is valid. Ready to start training.", 
                "Validation Passed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Run Tab

        private void btnStartTraining_Click(object sender, EventArgs e)
        {
            LogStatus("Starting training...");

            // Validate first
            if (string.IsNullOrEmpty(activeJobId))
            {
                MessageBox.Show("Please select and configure a job first.", 
                    "Cannot Start", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lstDataPaths.Items.Count == 0)
            {
                MessageBox.Show("Please add training data first.", 
                    "Cannot Start", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnStartTraining.Enabled = false;
            btnStopTraining.Enabled = true;
            progressBar1.Value = 0;
            txtConsoleOutput.Text = "";

            Task.Run(() =>
            {
                try
                {
                    RunTrainingWorker();
                }
                catch (Exception ex)
                {
                    LogConsole($"Error: {ex.Message}");
                    LogStatus($"Training error: {ex.Message}");
                }
                finally
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        btnStartTraining.Enabled = true;
                        btnStopTraining.Enabled = false;
                    });
                }
            });
        }

        private void RunTrainingWorker()
        {
            // TODO: Implement actual Python worker execution
            // For now, simulate training
            LogConsole("=== SupTrain Worker ===");
            LogConsole($"Job ID: {activeJobId}");
            LogConsole($"Model: {activeModelSlug}");
            LogConsole($"Round: {activeRound}");
            LogConsole($"Base Checkpoint: {activeBaseCkptCID}");
            LogConsole("");
            LogConsole("Phase 1: Downloading base checkpoint from IPFS...");
            System.Threading.Thread.Sleep(1000);
            LogConsole("✓ Checkpoint downloaded");
            LogConsole("");
            LogConsole("Phase 2: Loading model and tokenizer...");
            System.Threading.Thread.Sleep(1000);
            LogConsole("✓ Model loaded");
            LogConsole("");
            LogConsole("Phase 3: Preparing training data...");
            
            int dataCount = 0;
            this.Invoke((MethodInvoker)delegate
            {
                dataCount = lstDataPaths.Items.Count;
            });
            
            for (int i = 0; i < dataCount; i++)
            {
                string path = "";
                this.Invoke((MethodInvoker)delegate
                {
                    path = lstDataPaths.Items[i].ToString();
                });
                LogConsole($"  - {path}");
            }
            LogConsole("✓ Data prepared");
            LogConsole("");

            // Simulate training epochs
            int epochs = 1;
            this.Invoke((MethodInvoker)delegate
            {
                int.TryParse(txtEpochs.Text, out epochs);
            });

            for (int epoch = 1; epoch <= epochs; epoch++)
            {
                LogConsole($"Epoch {epoch}/{epochs}:");
                for (int step = 1; step <= 5; step++)
                {
                    double loss = 2.5 - (epoch * 0.3) - (step * 0.1);
                    LogConsole($"  Step {step}/5 - loss: {loss:F4}");
                    System.Threading.Thread.Sleep(500);
                    
                    int progress = (int)(((epoch - 1) * 5 + step) / (double)(epochs * 5) * 100);
                    this.Invoke((MethodInvoker)delegate
                    {
                        progressBar1.Value = Math.Min(100, progress);
                    });
                }
            }

            LogConsole("");
            LogConsole("Phase 4: Saving LoRA adapter (delta)...");
            System.Threading.Thread.Sleep(500);
            // Note: These are simulated CIDs for testing. In production, actual IPFS CIDs will be returned
            string deltaCID = "QmExampleDelta" + Guid.NewGuid().ToString().Substring(0, 8);
            LogConsole($"✓ Delta uploaded to IPFS: {deltaCID}");
            LogConsole("");
            
            LogConsole("Phase 5: Saving metrics...");
            System.Threading.Thread.Sleep(500);
            // Note: These are simulated CIDs for testing. In production, actual IPFS CIDs will be returned
            string metricsCID = "QmExampleMetrics" + Guid.NewGuid().ToString().Substring(0, 8);
            LogConsole($"✓ Metrics uploaded to IPFS: {metricsCID}");
            LogConsole("");
            
            LogConsole("=== Training Complete ===");
            LogStatus("Training completed successfully!");

            // Update publish tab
            this.Invoke((MethodInvoker)delegate
            {
                txtDeltaCID.Text = deltaCID;
                txtMetricsCID.Text = metricsCID;
                progressBar1.Value = 100;
                tabControl1.SelectedTab = tabPublish;
            });
        }

        private void btnStopTraining_Click(object sender, EventArgs e)
        {
            LogStatus("Stopping training...");
            if (trainingProcess != null && !trainingProcess.HasExited)
            {
                try
                {
                    trainingProcess.Kill();
                    LogStatus("Training stopped.");
                }
                catch (Exception ex)
                {
                    LogStatus($"Error stopping training: {ex.Message}");
                }
            }
            
            btnStartTraining.Enabled = true;
            btnStopTraining.Enabled = false;
        }

        #endregion

        #region Publish Tab

        private void btnPublishUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDeltaCID.Text) || string.IsNullOrEmpty(txtMetricsCID.Text))
            {
                MessageBox.Show("No training results to publish. Please run training first.", 
                    "Cannot Publish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LogStatus("Publishing update to Sup!? network...");

            Task.Run(() =>
            {
                try
                {
                    // TODO: Implement actual publishing via SupTrainService
                    System.Threading.Thread.Sleep(1000);

                    string message = $"#suptrain #update #job:{activeJobId} #round:{activeRound} " +
                                   $"#model:{activeModelSlug} #base:{activeBaseCkptCID} " +
                                   $"#delta:{txtDeltaCID.Text} #metrics:{txtMetricsCID.Text}";

                    LogStatus($"Update message: {message}");
                    LogStatus("✓ Update published successfully!");

                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Training update published to the network!", 
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });
                }
                catch (Exception ex)
                {
                    LogStatus($"Error publishing: {ex.Message}");
                }
            });
        }

        #endregion

        #region Monitor Tab

        private void btnRefreshMonitor_Click(object sender, EventArgs e)
        {
            LogStatus("Refreshing monitor feed...");
            flowMonitorList.Controls.Clear();

            Task.Run(() =>
            {
                // TODO: Implement actual monitoring using SupTrainService
                System.Threading.Thread.Sleep(500);

                this.Invoke((MethodInvoker)delegate
                {
                    // Add example entries
                    AddMonitorEntry("Checkpoint", "Round 1 checkpoint published", "QmExample1...", "2 hours ago");
                    AddMonitorEntry("Update", "Worker update received", "QmExample2...", "1 hour ago");
                    AddMonitorEntry("Aggregate", "New aggregated checkpoint", "QmExample3...", "30 mins ago");
                    LogStatus("Monitor feed refreshed.");
                });
            });
        }

        private void AddMonitorEntry(string type, string description, string cid, string time)
        {
            var panel = new Panel
            {
                Width = flowMonitorList.Width - 30,
                Height = 80,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(20, 20, 20)
            };

            var lblType = new Label
            {
                Text = $"[{type}]",
                ForeColor = Color.Yellow,
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = description,
                ForeColor = Color.White,
                Location = new Point(10, 30),
                AutoSize = true
            };

            var lblCid = new Label
            {
                Text = $"CID: {cid}",
                ForeColor = Color.Gray,
                Location = new Point(10, 50),
                AutoSize = true
            };

            var lblTime = new Label
            {
                Text = time,
                ForeColor = Color.Gray,
                Location = new Point(panel.Width - 150, 10),
                AutoSize = true
            };

            panel.Controls.AddRange(new Control[] { lblType, lblDesc, lblCid, lblTime });
            flowMonitorList.Controls.Add(panel);
        }

        private void btnFollowJob_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(activeJobId))
            {
                MessageBox.Show("Please select a job first.", "No Job Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LogStatus($"Following job keywords: #job:{activeJobId}");
            MessageBox.Show($"Now following updates for job: {activeJobId}", 
                "Following Job", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPinCheckpoint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(activeBaseCkptCID))
            {
                MessageBox.Show("No checkpoint CID available.", "Cannot Pin", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LogStatus($"Pinning checkpoint: {activeBaseCkptCID}");
            Task.Run(async () =>
            {
                await IpfsHelper.PinAsync(activeBaseCkptCID);
                LogStatus($"✓ Checkpoint pinned: {activeBaseCkptCID}");
            });
        }

        #endregion

        #region Logging Helpers

        private void LogStatus(string message)
        {
            if (txtStatusLog.InvokeRequired)
            {
                txtStatusLog.Invoke((MethodInvoker)delegate
                {
                    LogStatus(message);
                });
            }
            else
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                txtStatusLog.AppendText($"[{timestamp}] {message}\r\n");
                txtStatusLog.SelectionStart = txtStatusLog.Text.Length;
                txtStatusLog.ScrollToCaret();
            }
        }

        private void LogConsole(string message)
        {
            if (txtConsoleOutput.InvokeRequired)
            {
                txtConsoleOutput.Invoke((MethodInvoker)delegate
                {
                    LogConsole(message);
                });
            }
            else
            {
                txtConsoleOutput.AppendText(message + "\r\n");
                txtConsoleOutput.SelectionStart = txtConsoleOutput.Text.Length;
                txtConsoleOutput.ScrollToCaret();
            }
        }

        #endregion

        #region Helper Methods

        private string TruncateCID(string cid, int maxLength = 30)
        {
            if (string.IsNullOrEmpty(cid))
                return "";
            
            if (cid.Length <= maxLength)
                return cid;
            
            return cid.Substring(0, maxLength);
        }

        #endregion
    }
}
