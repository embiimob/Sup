using LevelDB;
using NBitcoin.RPC;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SUP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnPut_Click(object sender, EventArgs e)
        {
            if (lbTableName.SelectedItem == null)
            {
                lbTableName.SelectedIndex = 0;
            }

            switch (lbTableName.SelectedItem.ToString().Trim())
            {
                case "ROOT":
                    var ROOT = new Options { CreateIfMissing = true };
                    using (var db = new DB(ROOT, @"root"))
                    {
                        db.Put(txtPutKey.Text, txtPutValue.Text);
                    }
                    break;

                case "PRO":
                    var PRO = new Options { CreateIfMissing = true };
                    using (var db = new DB(PRO, @"root\pro"))
                    {
                        db.Put(txtPutKey.Text, txtPutValue.Text);
                    }
                    break;

                case "COL":
                    var COL = new Options { CreateIfMissing = true };
                    using (var db = new DB(COL, @"root\col"))
                    {
                        db.Put(txtPutKey.Text, txtPutValue.Text);
                    }
                    break;

                case "OBJ":
                    var OBJ = new Options { CreateIfMissing = true };
                    using (var db = new DB(OBJ, @"root\obj"))
                    {
                        db.Put(txtPutKey.Text, txtPutValue.Text);
                    }
                    break;

                case "LOGS":
                    var LOGS = new Options { CreateIfMissing = true };
                    using (var db = new DB(LOGS, @"root\logs"))
                    {
                        db.Put(txtPutKey.Text, txtPutValue.Text);
                    }
                    break;

                default:
                    MessageBox.Show("something went wrong");
                    break;
            }
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            if (lbTableName.SelectedItem == null)
            {
                lbTableName.SelectedIndex = 0;
            }

            switch (lbTableName.SelectedItem.ToString().Trim())
            {
                case "ROOT":
                    var ROOT = new Options { CreateIfMissing = true };
                    txtGetValue.Text = "";
                    using (var db = new DB(ROOT, @"root"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtGetKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
                        }
                        it.Dispose();
                    }
                    break;

                case "PRO":
                    var PRO = new Options { CreateIfMissing = true };
                    txtGetValue.Text = "";
                    using (var db = new DB(PRO, @"root\pro"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtGetKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
                        }
                        it.Dispose();
                    }
                    break;

                case "COL":
                    var COL = new Options { CreateIfMissing = true };
                    txtGetValue.Text = "";
                    using (var db = new DB(COL, @"root\col"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtGetKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
                        }
                        it.Dispose();
                    }
                    break;

                case "OBJ":
                    var OBJ = new Options { CreateIfMissing = true };
                    txtGetValue.Text = "";
                    using (var db = new DB(OBJ, @"root\obj"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtGetKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
                        }
                        it.Dispose();
                    }
                    break;

                case "LOGS":
                    var LOG = new Options { CreateIfMissing = true };
                    txtGetValue.Text = "";
                    using (var db = new DB(LOG, @"root\log"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtGetKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
                        }
                        it.Dispose();
                    }
                    break;

                default:
                    MessageBox.Show("something went wrong");
                    break;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lbTableName.SelectedItem == null)
            {
                lbTableName.SelectedIndex = 0;
            }

            switch (lbTableName.SelectedItem.ToString().Trim())
            {
                case "ROOT":
                    var ROOT = new Options { CreateIfMissing = true };
                    using (var db = new DB(ROOT, @"root"))
                    {
                        db.Delete(txtDeleteKey.Text);
                    }
                    break;
                case "PRO":
                    var PRO = new Options { CreateIfMissing = true };
                    using (var db = new DB(PRO, @"root\pro"))
                    {
                        db.Delete(txtDeleteKey.Text);
                    }
                    break;

                case "COL":
                    var COL = new Options { CreateIfMissing = true };
                    using (var db = new DB(COL, @"root\col"))
                    {
                        db.Delete(txtDeleteKey.Text);
                    }
                    break;

                case "OBJ":
                    var OBJ = new Options { CreateIfMissing = true };
                    using (var db = new DB(OBJ, @"root\obj"))
                    {
                        db.Delete(txtDeleteKey.Text);
                    }
                    break;

                case "LOGS":
                    var LOG = new Options { CreateIfMissing = true };
                    using (var db = new DB(LOG, @"root\logs"))
                    {
                        db.Delete(txtDeleteKey.Text);
                    }
                    break;

                default:
                    MessageBox.Show("something went wrong");
                    break;
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            string walletUrl = txtUrl.Text;
            string walletUsername = txtLogin.Text;
            string walletPassword = txtPassword.Text;
            NetworkCredential credentials = new NetworkCredential(walletUsername, walletPassword);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(walletUrl));

            try
            {
                txtbalance.Text = rpcClient.GetBalance().ToString();
            }
            catch (Exception ex)
            {
                dgTransactions.Rows.Clear();

                Root newRoot = new Root();
                newRoot.Message = new string[] { ex.Message };
                newRoot.BuildDate = DateTime.UtcNow;
                newRoot.File = new Dictionary<string, byte[]> { };
                newRoot.Keyword = new Dictionary<string, string> { };
                newRoot.TransactionId = "";

                object[] rowData = new object[]
                {
                    newRoot.TransactionId,
                    newRoot.Signed,
                    newRoot.SignedBy,
                    newRoot.Signature,
                    newRoot.File.Count(),
                    0,
                    ex.Message,
                    newRoot.Keyword.Count(),
                    newRoot.TotalByteSize,
                    newRoot.BlockDate,
                    newRoot.Confirmations,
                    newRoot.BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt")
                };
                dgTransactions.Rows.Add(rowData);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            Root[] roots = Root.GetRootByAddress(
                txtSearchAddress.Text,
                txtLogin.Text,
                txtPassword.Text,
                txtUrl.Text,
                txtVersionByte.Text
            );
            DateTime tmendCall = DateTime.UtcNow;
            dgTransactions.Rows.Clear();
            int totalbytes = 0;
            int totalfilebytes = 0;
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            for (int i = 0; i < roots.Length; i += 1)
            {
                if (roots[i] != null)
                {
                    string strmessage = "";

                    foreach (var rfile in roots[i].Message)
                    {
                        strmessage = strmessage + rfile;
                    }
                    foreach (var rfile in roots[i].File)
                    {
                        totalfilebytes += rfile.Value.Length;

                    }

                    object[] rowData = new object[]
                    {
                    roots[i].Id,
                    roots[i].TransactionId,
                    roots[i].Signed,
                    roots[i].SignedBy,
                    roots[i].Signature,
                    roots[i].File.Count(),
                    totalfilebytes,
                    strmessage,
                    roots[i].Keyword.Count(),
                    roots[i].TotalByteSize,
                    roots[i].BlockDate,
                    roots[i].Confirmations,
                    roots[i].BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt")
                    };
                    dgTransactions.Rows.Add(rowData);
                    totalbytes += roots[i].TotalByteSize;
                }
            }
            dgTransactions.AutoResizeRows();
            lblTotalBytes.Text = "Total Bytes: " + totalbytes.ToString();
            lblTotalTime.Text = "Total Time: " + elapsedMilliseconds;
            double secondsExpired = elapsedMilliseconds / 1000.0;
            double kilobytes = totalbytes / 1024.0;
            double kbs = kilobytes / secondsExpired;
            lblKbs.Text = "Kb/s " + kbs;
        }

        private void btnGetTransactionId_Click(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            Root root = Root.GetRootByTransactionId(
                txtTransactionId.Text,
                txtLogin.Text,
                txtPassword.Text,
                txtUrl.Text,
                txtVersionByte.Text
            );
            DateTime tmendCall = DateTime.UtcNow;
            dgTransactions.Rows.Clear();
            int totalbytes = 0;
            
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            if (root != null)
            {
                string strmessage = "";
                int totalfilebytes = 0;
                foreach (var rfile in root.Message)
                {
                    strmessage = strmessage + rfile;
                    
                }
                foreach (var rfile in root.File)
                {
                    totalfilebytes+= rfile.Value.Length;

                }

                object[] rowData = new object[]
                {
                    root.Id,
                    root.TransactionId,
                    root.Signed,
                    root.SignedBy,
                    root.Signature,
                    root.File.Count(),
                    totalfilebytes,
                    strmessage,
                    root.Keyword.Count(),
                    root.TotalByteSize,
                    root.BlockDate,
                    root.Confirmations,
                    root.BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt")
                };
                dgTransactions.Rows.Add(rowData);
                dgTransactions.AutoResizeRows();

                totalbytes = root.TotalByteSize;
                lblTotalBytes.Text = "Total Bytes: " + totalbytes.ToString();
                lblTotalTime.Text = "Total Time: " + elapsedMilliseconds;
                double secondsExpired = elapsedMilliseconds / 1000.0;
                double kilobytes = totalbytes / 1024.0;
                double kbs = kilobytes / secondsExpired;
                lblKbs.Text = "Kb/s " + kbs;
            }
        }

        private void btnGetKeyword_Click(object sender, EventArgs e)
        {
            string publicAddress = Root.GetPublicAddressByKeyword(txtGetKeyword.Text,txtVersionByte.Text);
            DateTime tmbeginCall = DateTime.UtcNow;
            Root[] roots = Root.GetRootByAddress(
                publicAddress,
                txtLogin.Text,
                txtPassword.Text,
                txtUrl.Text,
                txtVersionByte.Text
            );
            DateTime tmendCall = DateTime.UtcNow;
            dgTransactions.Rows.Clear();
            int totalbytes = 0;
           
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            for (int i = 0; i < roots.Length; i += 1)
            {
                string strmessage = "";
                int totalfilebytes = 0;
                foreach (var rfile in roots[i].Message)
                {
                    strmessage = strmessage + rfile;
                }
                foreach (var rfile in roots[i].File)
                {
                    totalfilebytes += rfile.Value.Length;

                }

                object[] rowData = new object[]
                {
                    roots[i].Id,
                    roots[i].TransactionId,
                    roots[i].Signed,
                    roots[i].SignedBy,
                    roots[i].Signature,
                    roots[i].File.Count(),
                    totalfilebytes,
                    strmessage,
                    roots[i].Keyword.Count(),
                    roots[i].TotalByteSize,
                    roots[i].BlockDate,
                    roots[i].Confirmations,
                    roots[i].BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt")
                };
                dgTransactions.Rows.Add(rowData);
                totalbytes += roots[i].TotalByteSize;
            }
            dgTransactions.AutoResizeRows();
            lblTotalBytes.Text = "Total Bytes: " + totalbytes.ToString();
            lblTotalTime.Text = "Total Time: " + elapsedMilliseconds;
            double secondsExpired = elapsedMilliseconds / 1000.0;
            double kilobytes = totalbytes / 1024.0;
            double kbs = kilobytes / secondsExpired;
            lblKbs.Text = "Kb/s " + kbs;
        }
    }
}

