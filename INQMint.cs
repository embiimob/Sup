using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NBitcoin;
using SUP.P2FK;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;


namespace SUP
{
    public partial class INQMint : Form
    {
        private bool ismint = false;
        Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        private DiscoBall parentForm;
        private Random random = new Random();

        public INQMint(DiscoBall parentForm, bool testnet = true)
        {
            InitializeComponent();
            this.parentForm = parentForm;
            if (!testnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";

            }
        }

        private string GetRandomDelimiter()
        {
            string[] delimiters = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };

            return delimiters[random.Next(delimiters.Length)];
        }

        private void UpdateRemainingChars()
        {


            INQ INQJson = new INQ();

            string newAddress = "";

            if (ismint)
            {
                NetworkCredential credentials = new NetworkCredential(mainnetLogin,mainnetPassword);
                NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);

                string P2FKASCII = "";
                char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
                int attempt = 0;
                while (true)
                {
                    try
                    {
                        newAddress = rpcClient.SendCommand("getnewaddress", txtQUE.Text + "!" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "!" + attempt.ToString()).ResultString;
                        P2FKASCII = Root.GetKeywordByPublicAddress(newAddress, "ASCII");
                        string pattern = "[" + Regex.Escape(new string(specialChars)) + "][0-9]";
                        if (!Regex.IsMatch(P2FKASCII, pattern))
                        {

                            break;
                        }
                        attempt++;
                    }
                    catch
                    {

                    }
                }
                parentForm.txtINQAddress.Text = newAddress;
            }
            INQJson.que = new Dictionary<string, string> { { newAddress, txtQUE.Text } };


            Dictionary<string, string> mintANS = new Dictionary<string, string>();
            int answerCnt = 0;
            foreach (Button attributeControl in flowANS.Controls)
            {
                answerCnt++;
                string ANSAddress = answerCnt.ToString();
                if (ismint)
                {
                    NetworkCredential credentials = new NetworkCredential(mainnetLogin,mainnetPassword);
                    NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);
                    ANSAddress = rpcClient.SendCommand("getnewaddress", attributeControl.Text + "!" + DateTime.UtcNow.ToString("yyyyMMddHHmmss")).ResultString;

                }
                mintANS.Add(ANSAddress, attributeControl.Text);

            }
            INQJson.ans = mintANS;



            List<string> mintOWN = new List<string>();
            foreach (Button attributeControl in flowOWN.Controls)
            {


                mintOWN.Add(attributeControl.Text);

            }
            INQJson.own = mintOWN.ToArray();


            List<string> mintCRE = new List<string>();
            foreach (Button attributeControl in flowCRE.Controls)
            {


                mintCRE.Add(attributeControl.Text);

            }
            INQJson.cre = mintCRE.ToArray();

            if (!checkBox1.Checked) { INQJson.any = 1; }
            if (int.TryParse(txtEND.Text, out int intEND) && intEND > 0) { INQJson.end = intEND; }


            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            // Serialize the modified JObject back into a JSON string ",\"end\":0"
            var objectSerialized = JsonConvert.SerializeObject(INQJson, Formatting.None, settings);

            txtOBJJSON.Text = objectSerialized.Replace(",\"ans\":{}", "").Replace(",\"que\":{}", "").Replace(",\"own\":[]", "").Replace(",\"cre\":[]", "").Replace(",\"end\":0", "").Replace(",\"any\":0", "");
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(txtOBJJSON.Text);
            int lengthInBytes = utf8Bytes.Length;
            string objString = "INQ" + GetRandomDelimiter() + lengthInBytes + GetRandomDelimiter() + txtOBJJSON.Text;
            txtOBJP2FK.Text = objString;

            if (ismint)
            {
                parentForm.txtINQJson.Text = objString;
                this.Close();
            }

        }

        private void btnAttachClick(object sender, EventArgs e)
        {

            ismint = true;
            UpdateRemainingChars();
        }

        private void INQMint_Load(object sender, EventArgs e)
        {
            selectTimeGate.SelectedIndex = 3;
        }

        //GPT3
        private void selectTimeGate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedValue = comboBox.SelectedItem.ToString();
            int minutes;

            // Map selected items to their corresponding time periods in minutes
            switch (selectedValue)
            {
                case "1 hour":
                    minutes = 60;
                    break;
                case "3 hours":
                    minutes = 3 * 60;
                    break;
                case "12 hours":
                    minutes = 12 * 60;
                    break;
                case "1 day":
                    minutes = 24 * 60;
                    break;
                case "3 days":
                    minutes = 3 * 24 * 60;
                    break;
                case "7 days":
                    minutes = 7 * 24 * 60;
                    break;
                case "1 month":
                    minutes = 30 * 24 * 60; // Assuming a month has 30 days
                    break;
                case "6 months":
                    minutes = 6 * 30 * 24 * 60;
                    break;
                case "1 year":
                    minutes = 365 * 24 * 60; // Assuming a year has 365 days
                    break;
                case "3 years":
                    minutes = 3 * 365 * 24 * 60;
                    break;
                case "10 years":
                    minutes = 10 * 365 * 24 * 60;
                    break;
                case "100 years":
                    minutes = 100 * 365 * 24 * 60;
                    break;
                case "1000 years":
                    minutes = 1000 * 365 * 24 * 60;
                    break;
                default:
                    minutes = 0; // Handle unknown selections
                    break;
            }

            // Calculate the number of 10-minute blocks and display it in txtEND
            int numberOfBlocks = minutes / 10;
            txtEND.Text = numberOfBlocks.ToString();
        }

        private void btnANS_Click(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = String.Empty;
                dialog.AutoSize = true;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ControlBox = false;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new Size(400, 80);

                var tableLayout = new TableLayoutPanel();
                tableLayout.ColumnCount = 1;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                var keyLabel = new Label();
                keyLabel.Text = "Answer";
                keyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var keyTextBox = new TextBox();
                keyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                keyTextBox.Multiline = true;
                keyTextBox.Size = new Size(340, 70);
                tableLayout.Controls.Add(keyLabel, 0, 0);
                tableLayout.Controls.Add(keyTextBox, 0, 1);

                var addButton = new Button();
                addButton.Text = "Add";
                addButton.DialogResult = DialogResult.OK;
                addButton.Anchor = AnchorStyles.Right;

                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Anchor = AnchorStyles.Right;

                dialog.Controls.Add(tableLayout);
                dialog.Controls.Add(addButton);
                dialog.Controls.Add(cancelButton);

                tableLayout.Dock = DockStyle.Top;
                addButton.Dock = DockStyle.Right;
                cancelButton.Dock = DockStyle.Right;

                dialog.AcceptButton = addButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var key = keyTextBox.Text;

                    var button = new Button();
                    button.ForeColor = Color.White;
                    button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                    button.Text = $"{key}";
                    button.AutoSize = true;

                    // add event handler for left click
                    button.MouseClick += (s, ev) =>
                    {
                        // only proceed if left mouse button is clicked
                        if (ev.Button == MouseButtons.Left)
                        {
                            // disable the button to prevent double-clicks
                            button.Enabled = false;

                            // remove the button from the flow layout panel and dispose of it
                            flowANS.Controls.Remove(button);
                            button.Dispose();
                        }
                    };

                    // add event handler for right click
                    button.MouseDown += (s, ev) =>
                    {
                        // only proceed if right mouse button is clicked
                        if (ev.Button == MouseButtons.Right)
                        {
                            // create the object browser form and show it
                            string labelText = button.Text;
                            ObjectBrowser form = new ObjectBrowser(labelText);
                            form.Show();
                        }
                    };

                    flowANS.Controls.Add(button);
                }
            }
            UpdateRemainingChars();

        }

        private void btnOWN_Click(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = String.Empty;
                dialog.AutoSize = true;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ControlBox = false;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new Size(400, 80);

                var tableLayout = new TableLayoutPanel();
                tableLayout.ColumnCount = 1;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                var keyLabel = new Label();
                keyLabel.Text = "Object Address";
                keyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var keyTextBox = new TextBox();
                keyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                keyTextBox.Multiline = true;
                keyTextBox.Size = new Size(340, 70);
                tableLayout.Controls.Add(keyLabel, 0, 0);
                tableLayout.Controls.Add(keyTextBox, 0, 1);

                var addButton = new Button();
                addButton.Text = "Add";
                addButton.DialogResult = DialogResult.OK;
                addButton.Anchor = AnchorStyles.Right;

                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Anchor = AnchorStyles.Right;

                dialog.Controls.Add(tableLayout);
                dialog.Controls.Add(addButton);
                dialog.Controls.Add(cancelButton);

                tableLayout.Dock = DockStyle.Top;
                addButton.Dock = DockStyle.Right;
                cancelButton.Dock = DockStyle.Right;

                dialog.AcceptButton = addButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var key = keyTextBox.Text;

                    var button = new Button();
                    button.ForeColor = Color.White;
                    button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                    button.Text = $"{key}";
                    button.AutoSize = true;

                    // add event handler for left click
                    button.MouseClick += (s, ev) =>
                    {
                        // only proceed if left mouse button is clicked
                        if (ev.Button == MouseButtons.Left)
                        {
                            // disable the button to prevent double-clicks
                            button.Enabled = false;

                            // remove the button from the flow layout panel and dispose of it
                            flowOWN.Controls.Remove(button);
                            button.Dispose();
                        }
                    };

                    // add event handler for right click
                    button.MouseDown += (s, ev) =>
                    {
                        // only proceed if right mouse button is clicked
                        if (ev.Button == MouseButtons.Right)
                        {
                            // create the object browser form and show it
                            string labelText = button.Text;
                            ObjectBrowser form = new ObjectBrowser(labelText);
                            form.Show();
                        }
                    };

                    flowOWN.Controls.Add(button);
                }
            }
            UpdateRemainingChars();
        }

        private void txtQUE_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
        }

        private void btnCRE_Click(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = String.Empty;
                dialog.AutoSize = true;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ControlBox = false;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new Size(400, 80);

                var tableLayout = new TableLayoutPanel();
                tableLayout.ColumnCount = 1;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                var keyLabel = new Label();
                keyLabel.Text = "Creator Address";
                keyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var keyTextBox = new TextBox();
                keyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                keyTextBox.Multiline = true;
                keyTextBox.Size = new Size(340, 70);
                tableLayout.Controls.Add(keyLabel, 0, 0);
                tableLayout.Controls.Add(keyTextBox, 0, 1);

                var addButton = new Button();
                addButton.Text = "Add";
                addButton.DialogResult = DialogResult.OK;
                addButton.Anchor = AnchorStyles.Right;

                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Anchor = AnchorStyles.Right;

                dialog.Controls.Add(tableLayout);
                dialog.Controls.Add(addButton);
                dialog.Controls.Add(cancelButton);

                tableLayout.Dock = DockStyle.Top;
                addButton.Dock = DockStyle.Right;
                cancelButton.Dock = DockStyle.Right;

                dialog.AcceptButton = addButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var key = keyTextBox.Text;

                    var button = new Button();
                    button.ForeColor = Color.White;
                    button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                    button.Text = $"{key}";
                    button.AutoSize = true;

                    // add event handler for left click
                    button.MouseClick += (s, ev) =>
                    {
                        // only proceed if left mouse button is clicked
                        if (ev.Button == MouseButtons.Left)
                        {
                            // disable the button to prevent double-clicks
                            button.Enabled = false;

                            // remove the button from the flow layout panel and dispose of it
                            flowCRE.Controls.Remove(button);
                            button.Dispose();
                        }
                    };

                    // add event handler for right click
                    button.MouseDown += (s, ev) =>
                    {
                        // only proceed if right mouse button is clicked
                        if (ev.Button == MouseButtons.Right)
                        {
                            // create the object browser form and show it
                            string labelText = button.Text;
                            ObjectBrowser form = new ObjectBrowser(labelText);
                            form.Show();
                        }
                    };

                    flowCRE.Controls.Add(button);
                }
            }
            UpdateRemainingChars();
        }

        private void txtEND_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
        }
    }
}
