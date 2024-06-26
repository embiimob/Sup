﻿using SUP.P2FK;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace SUP
{
    public partial class FoundINQControl : UserControl
    {
        private string _address;
        private string _activeprofile;
        private bool calculate = false;
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        private bool _testnet;
        public FoundINQControl(string address, string activeprofile = "", bool testnet = true)
        {
            SuspendLayout();
            InitializeComponent();
            _address = address;
            _activeprofile = activeprofile;
            if (!testnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";
            }
            _testnet = testnet;
        }


        private void ObjectAddress_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtTransactionId.Text);
        }

        private void lblTrash_Click(object sender, EventArgs e)
        {



            // Prompt the user to confirm whether they want to delete the file
            DialogResult result = MessageBox.Show($"Are you sure you want to delete this inquiry?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {


                try
                {

                    try { Directory.Delete(@"root\" + txtTransactionId.Text, true); } catch { }
                    try { Directory.CreateDirectory(@"root\" + txtTransactionId.Text); } catch { }

                    using (FileStream fs = File.Create(@"root\" + txtTransactionId.Text + @"\BLOCK"))
                    {

                    }

                    if (txtObjectAddress.Text != null)
                    {
                        try { Directory.Delete(@"root\" + txtObjectAddress.Text, true); } catch { }
                        try { Directory.CreateDirectory(@"root\" + txtObjectAddress.Text); } catch { }

                        using (FileStream fs = File.Create(@"root\" + txtObjectAddress.Text + @"\BLOCK"))
                        {

                        }
                    }


                }
                catch { }

                // Remove the user control from its parent flow panel
                this.Parent.Controls.Remove(this);
            }

        }

        private void btnVote_Click(object sender, EventArgs e)
        {

            Button voteButton = (Button)sender;

            string INQToKey = "";
            if (_testnet) {
                INQToKey = Root.GetPublicAddressByKeyword(txtTransactionId.Text);
            }
            else
            {
                INQToKey = Root.GetPublicAddressByKeyword(txtTransactionId.Text,"0");
            }
            string voteDust = INQToKey + "," + voteButton.Tag.ToString();
            DiscoBall disco = new DiscoBall(_activeprofile, "", voteDust, "", false,_testnet);
            disco.StartPosition = FormStartPosition.CenterScreen;
            disco.Show(this);
            disco.Focus();


        }

        private void btnValueTotal_Click(object sender, EventArgs e)
        {
            if (btnValueTotal.Text == "show values") { btnValueTotal.Text = "show votes"; } else { btnValueTotal.Text = "show values"; };
            this.Invoke((MethodInvoker)delegate
            {
                RefreshTotals();

            });
        }

        private void RefreshTotals()
        {

            flowLayoutPanel1.Controls.Clear();

            INQState iNQState = INQState.GetInquiryByTransactionId(_address, mainnetLogin,mainnetPassword,mainnetURL,mainnetVersionByte, calculate);

            if (iNQState.AnswerData != null)
            {
                txtObjectAddress.Text = iNQState.URN.ToString();
                txtQUE.Text = iNQState.Question;
                txtCreatedBy.Text = "created by: " + iNQState.CreatedBy.ToString();
                txtCreatedDate.Text = "created date: " + iNQState.CreatedDate.ToString();
                txtTransactionId.Text = iNQState.TransactionId;
                if (iNQState.OwnsObjectGate != null || iNQState.OwnsCreatedByGate != null) { btnShowAllOrGated.Visible = true; }
                if (int.TryParse(iNQState.status, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int status))
                {
                    DateTime currentTime = DateTime.Now;
                    DateTime futureTime = currentTime.AddMinutes(status * 10); // Each unit represents 10 minutes

                    // Calculate the time difference between futureTime and currentTime
                    TimeSpan timeRemaining = futureTime - currentTime;

                    // Format the time remaining
                    string timeRemainingString;
                    if (timeRemaining.TotalDays >= 1)
                    {
                        timeRemainingString = $"{(int)timeRemaining.TotalDays} day {(int)timeRemaining.Hours} hrs";
                    }
                    else
                    {
                        timeRemainingString = $"{(int)timeRemaining.TotalHours} hrs {timeRemaining.Minutes} minutes";
                    }
                    // Create the final status string
                    string statusString = $"status: {timeRemainingString}";

                    txtStatus.Text = statusString;
                }
                else
                {
                    txtStatus.Text = "status: " + iNQState.status;
                }

                if (btnValueTotal.Text == "show values")
                {
                    if (btnShowAllOrGated.Text == "show all")
                    {
                        lblVotesOrValue.Text = "Total Votes: " + iNQState.TotalGatedVotes.ToString();

                    }
                    else
                    {
                        lblVotesOrValue.Text = "Total Votes: " + iNQState.TotalVotes.ToString();

                    }

                }
                else
                {
                    if (btnShowAllOrGated.Text == "show all")
                    {
                        lblVotesOrValue.Text = "Total Value: " + iNQState.TotalGatedValue.ToString();

                    }
                    else
                    {
                        lblVotesOrValue.Text = "Total Value: " + iNQState.TotalValue.ToString();
                    };
                }


                foreach (AnswerData answer in iNQState.AnswerData)
                {
                    System.Windows.Forms.Label lblANS = new System.Windows.Forms.Label();
                    lblANS.AutoSize = true;
                    lblANS.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lblANS.ForeColor = System.Drawing.Color.White;
                    lblANS.Location = new System.Drawing.Point(3, 10);
                    lblANS.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
                    lblANS.MaximumSize = new System.Drawing.Size(400, 420);
                    lblANS.MinimumSize = new System.Drawing.Size(400, 25);
                    lblANS.Name = "lblANS";
                    lblANS.Size = new System.Drawing.Size(400, 25);
                    lblANS.TabIndex = 0;
                    lblANS.Text = answer.Answer;
                    flowLayoutPanel1.Controls.Add(lblANS);

                    System.Windows.Forms.ProgressBar progressANS = new System.Windows.Forms.ProgressBar();
                    progressANS.Location = new System.Drawing.Point(3, 38);
                    progressANS.Name = "progressANS";
                    progressANS.Size = new System.Drawing.Size(254, 23);


                    if (btnValueTotal.Text == "show values")
                    {
                        if (btnShowAllOrGated.Text == "show all")
                        {
                            progressANS.Maximum = iNQState.TotalGatedVotes;
                            progressANS.Step = 1;
                            progressANS.Value = answer.TotalGatedVotes;
                        }
                        else
                        {
                            progressANS.Maximum = iNQState.TotalVotes;
                            progressANS.Step = 1;
                            progressANS.Value = answer.TotalVotes;
                        }
                    }
                    else
                    {
                        if (btnShowAllOrGated.Text == "show all")
                        {
                            decimal totalValue = iNQState.TotalGatedValue;
                            decimal currentValue = answer.TotalGatedValue;
                            int percentage = 0;
                            try { percentage = (int)((currentValue / totalValue) * 100); } catch { }

                            progressANS.Maximum = 100; // Maximum value for percentage
                            progressANS.Step = 1;
                            progressANS.Value = percentage;
                        }
                        else
                        {
                            decimal totalValue = iNQState.TotalValue;
                            decimal currentValue = answer.TotalValue;
                            int percentage = 0;
                            try { percentage = (int)((currentValue / totalValue) * 100); } catch { }

                            progressANS.Maximum = 100; // Maximum value for percentage
                            progressANS.Step = 1;
                            progressANS.Value = percentage;
                        }
                    }



                    flowLayoutPanel1.Controls.Add(progressANS);

                    System.Windows.Forms.Label lblANSTotal = new System.Windows.Forms.Label();
                    lblANSTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lblANSTotal.ForeColor = System.Drawing.Color.White;
                    lblANSTotal.Location = new System.Drawing.Point(323, 38);
                    lblANSTotal.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
                    lblANSTotal.Name = "lblANSTotal";
                    lblANSTotal.Size = new System.Drawing.Size(106, 26);
                    lblANSTotal.TabIndex = 2;

                    if (btnValueTotal.Text == "show values")
                    {
                        if (btnShowAllOrGated.Text == "show all")
                        {
                            lblANSTotal.Text = answer.TotalGatedVotes.ToString();
                        }
                        else
                        {
                            lblANSTotal.Text = answer.TotalVotes.ToString();
                        }
                    }
                    else
                    {

                        if (btnShowAllOrGated.Text == "show all")
                        {
                            lblANSTotal.Text = answer.TotalGatedValue.ToString();
                        }
                        else
                        {
                            lblANSTotal.Text = answer.TotalValue.ToString();
                        }
                    }
                    flowLayoutPanel1.Controls.Add(lblANSTotal);

                    System.Windows.Forms.Button btnVote = new System.Windows.Forms.Button();
                    if ((_activeprofile == "" && iNQState.RequireSignature) || !int.TryParse(iNQState.status, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int dummy)) { btnVote.Enabled = false; }
                    btnVote.Location = new System.Drawing.Point(435, 38);
                    btnVote.Name = "btnVote";
                    btnVote.Size = new System.Drawing.Size(51, 23);
                    btnVote.TabIndex = 25;
                    btnVote.Text = "vote";
                    btnVote.Tag = answer.Address;
                    btnVote.UseVisualStyleBackColor = true;
                    btnVote.Click += new System.EventHandler(btnVote_Click);
                    flowLayoutPanel1.Controls.Add(btnVote);
                }
            }

        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            calculate = true;
            this.Invoke((MethodInvoker)delegate
            {
                RefreshTotals();

            });
        }

        private void FoundINQControl_Load(object sender, EventArgs e)
        {

            RefreshTotals();

        }

        private void btnShowAllOrGated_Click(object sender, EventArgs e)
        {
            if (btnShowAllOrGated.Text == "show all") { btnShowAllOrGated.Text = "show gated"; } else { btnShowAllOrGated.Text = "show all"; };
            this.Invoke((MethodInvoker)delegate
            {
                RefreshTotals();

            });
        }
    }
}
