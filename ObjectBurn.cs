using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using BitcoinNET.RPCClient;
using NBitcoin.RPC;
using NBitcoin;
using Newtonsoft.Json;
using SUP.P2FK;


namespace SUP
{
    public partial class ObjectBurn : Form
    {
        //GPT3 ROCKS
        private const int MaxRows = 2000;
        private readonly List<(string address, int qty)> _addressQtyList = new List<(string address, int qty)>();
        bool mint = false;
        private readonly string brnaddress = "";
        public ObjectBurn(string _address="")
        {
            InitializeComponent();
            brnaddress = _address;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (_addressQtyList.Count >= MaxRows)
            {
                MessageBox.Show($"You cannot add more than {MaxRows} rows.");
                return;
            }

            var address = addressTextBox.Text;
            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Address cannot be empty.");
                return;
            }

            if (!int.TryParse(qtyTextBox.Text, out var qty) || qty < 1)
            {
                MessageBox.Show("Quantity must be a positive integer.");
                return;
            }

            _addressQtyList.Add((address, qty));
            addressQtyDataGridView.Rows.Add(address, qty);

            addressTextBox.Clear();
            qtyTextBox.Clear();
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (addressQtyDataGridView.Rows.Count == 0)
            {
                MessageBox.Show("No data to save.");
                return;
            }

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.WriteLine("Address,Qty");

                        foreach (var (address, qty) in _addressQtyList)
                        {
                            writer.WriteLine($"{address},{qty}");
                        }
                    }

                    MessageBox.Show("Data saved successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving data: {ex.Message}");
                }
            }
        }



        private void burnButton_Click(object sender, EventArgs e)
        {
            var dictionary = new Dictionary<string, int>();
            var newdictionary = new List<List<int>>();
            List<string> encodedList = new List<string>();
            int brnOrder = 1;
            foreach (var (address, qty) in _addressQtyList)
            {
                if (!dictionary.ContainsKey(address))
                {
                    dictionary[address] = qty;
                    if (address == txtSignatureAddress.Text)
                    {
                        newdictionary.Clear();
                        newdictionary.Add(new List<int> { 0, qty });
                        dictionary.Clear();
                        dictionary.Add(address, qty);
                        break;
                    }
                    newdictionary.Add(new List<int> { brnOrder, qty });
                    brnOrder++;
                }
            }
            int salt;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[4];
                rng.GetBytes(saltBytes);
                salt = -Math.Abs(BitConverter.ToInt32(saltBytes, 0) % 100000);
            }
            newdictionary.Add(new List<int> { 0, salt });



            var json = JsonConvert.SerializeObject(newdictionary);
            txtOBJJSON.Text = json;

            txtOBJP2FK.Text = "BRN" + "*" + txtOBJJSON.Text.Length + "*" + txtOBJJSON.Text;

            if (btnBurn.Enabled)
            {
                NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                RPCClient rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:18332"), Network.Main);
                System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                byte[] hashValue = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(txtOBJP2FK.Text));
                string signatureAddress;

                signatureAddress = txtSignatureAddress.Text;
                string signature = "";
                try { signature = rpcClient.SendCommand("signmessage", signatureAddress, BitConverter.ToString(hashValue).Replace("-", String.Empty)).ResultString; }
                catch (Exception ex)
                {
                    lblObjectStatus.Text = ex.Message;
                    btnBurn.BackColor = System.Drawing.Color.White;
                    btnBurn.ForeColor = System.Drawing.Color.Black;
                    mint = false;
                    return;
                }

                txtOBJP2FK.Text = "SIG" + ":" + "88" + ">" + signature + txtOBJP2FK.Text;

                
                for (int i = 0; i < txtOBJP2FK.Text.Length; i += 20)
                {
                    string chunk = txtOBJP2FK.Text.Substring(i, Math.Min(20, txtOBJP2FK.Text.Length - i));
                    if (chunk.Any())
                    {
                        encodedList.Add(Root.GetPublicAddressByKeyword(chunk));
                    }
                }

                foreach (string address in dictionary.Keys)
                {
                    encodedList.Add(address);
                }
             
                encodedList.Add(signatureAddress);
                txtAddressListJSON.Text = JsonConvert.SerializeObject(encodedList.Distinct());

                lblCost.Text = "cost: " + (0.00000546 * encodedList.Count).ToString("0.00000000") + "  + miner fee";

                if (mint)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to burn this?", "Confirmation", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        
                        var recipients = new Dictionary<string, decimal>();
                        foreach (var encodedAddress in encodedList)
                        {
                            try { recipients.Add(encodedAddress, 0.00000546m); } catch { }
                        }

                        CoinRPC a = new CoinRPC(new Uri("http://127.0.0.1:18332"), new NetworkCredential("good-user", "better-password"));

                        try
                        {
                            string accountsString = "";
                            try { accountsString = rpcClient.SendCommand("listaccounts").ResultString; } catch { }
                            var accounts = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(accountsString);
                            var keyWithLargestValue = accounts.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                            var results = a.SendMany(keyWithLargestValue, recipients);
                            lblObjectStatus.Text = results;
                        }
                        catch (Exception ex) { lblObjectStatus.Text = ex.Message; }
                        btnBurn.BackColor = System.Drawing.Color.White;
                        btnBurn.ForeColor = System.Drawing.Color.Black;
                        mint = false;

                    }
                    btnBurn.BackColor = System.Drawing.Color.White;
                    btnBurn.ForeColor = System.Drawing.Color.Black;
                    mint = false;
                }

                btnBurn.BackColor = System.Drawing.Color.Blue;
                btnBurn.ForeColor = System.Drawing.Color.Yellow;
                mint = true;

            }




        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length != 1)
            {
                MessageBox.Show("You can only drop one file at a time.");
                return;
            }

            var file = files[0];
            if (!File.Exists(file))
            {
                MessageBox.Show("File does not exist.");
                return;
            }

            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length != 2) continue;

                var address = parts[0].Trim();
                if (string.IsNullOrWhiteSpace(address)) continue;

                if (!int.TryParse(parts[1].Trim(), out var qty) || qty < 1) continue;

                if (_addressQtyList.Count >= MaxRows) break;

                _addressQtyList.Add((address, qty));
                addressQtyDataGridView.Rows.Add(address, qty);
            }
        }

        private void ObjectBurn_Load(object sender, EventArgs e)
        {
            addressTextBox.Text = brnaddress;
        }
    }
}
