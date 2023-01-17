using LevelDB;
using NBitcoin.RPC;
using Newtonsoft.Json;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
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

        private void BtnGet_Click(object sender, EventArgs e)
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
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
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
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
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
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
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
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
                        }
                        it.Dispose();
                    }
                    break;

                case "EVENT":
                    var LOG = new Options { CreateIfMissing = true };
                    txtGetValue.Text = "";
                    using (var db = new DB(LOG, @"root\event"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
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

        private void BtnDelete_Click(object sender, EventArgs e)
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
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;
                case "PRO":
                    var PRO = new Options { CreateIfMissing = true };
                    using (var db = new DB(PRO, @"root\pro"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;

                case "COL":
                    var COL = new Options { CreateIfMissing = true };
                    using (var db = new DB(COL, @"root\col"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;

                case "OBJ":
                    var OBJ = new Options { CreateIfMissing = true };
                    using (var db = new DB(OBJ, @"root\obj"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;

                case "EVENT":
                    var LOG = new Options { CreateIfMissing = true };
                    using (var db = new DB(LOG, @"root\event"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;

                default:
                    MessageBox.Show("something went wrong");
                    break;
            }
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
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

                Root newRoot = new Root
                {
                    Message = new string[] { ex.Message },
                    BuildDate = DateTime.UtcNow,
                    File = new Dictionary<string, BigInteger> { },
                    Keyword = new Dictionary<string, string> { },
                    TransactionId = ""
                };

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

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            Root[] roots = Root.GetRootsByAddress(
                txtSearchAddress.Text,
                txtLogin.Text,
                txtPassword.Text,
                txtUrl.Text,
                txtVersionByte.Text
            );
            DateTime tmendCall = DateTime.UtcNow;

            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            dgTransactions.Rows.Clear();
            int totalbytes = 0;
            BigInteger totalfilebytes = 0;
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            for (int i = 0; i < roots.Length; i += 1)
            {
                if (roots[i] != null)
                {
                    string strmessage = "";

                    foreach (var rfile in roots[i].Message)
                    {
                        strmessage += rfile;
                    }
                    foreach (var rfile in roots[i].File)
                    {
                        totalfilebytes += rfile.Value;

                    }

                    object[] rowData = new object[]
                    {
                    roots[i].Id,
                    strmessage,
                    roots[i].Signed,
                    roots[i].SignedBy,
                    roots[i].File.Count(),
                    totalfilebytes,
                    roots[i].Keyword.Count(),
                    roots[i].TotalByteSize,
                    roots[i].BlockDate,
                    roots[i].TransactionId,
                    roots[i].Signature,
                    roots[i].Confirmations,
                    roots[i].BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt")
                        };
                    dgTransactions.Rows.Add(rowData);
                    totalbytes += roots[i].TotalByteSize;
                }
            }
            dgTransactions.AutoResizeRows();
            dgTransactions.AutoResizeColumns();
            lblTotalBytes.Text = "bytes: " + totalbytes.ToString();
            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            double secondsExpired = elapsedMilliseconds / 1000.0;
            double kilobytes = totalbytes / 1024.0;
            double kbs = kilobytes / secondsExpired;
            lblKbs.Text = "kb/s: " + Math.Truncate(kbs);
            lblTotal.Text = "total:" + roots.Length.ToString();
        }

        private void BtnGetTransactionId_Click(object sender, EventArgs e)
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
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            dgTransactions.Rows.Clear();
            int totalbytes;

            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            if (root != null)
            {
                string strmessage = "";
                BigInteger totalfilebytes = 0;
                foreach (var rfile in root.Message)
                {
                    strmessage += rfile;

                }

                foreach (var rfile in root.File)
                {
                    totalfilebytes += rfile.Value;


                }

                object[] rowData = new object[]
                {
                    root.Id,
                    strmessage,
                    root.Signed,
                    root.SignedBy,
                    root.File.Count(),
                    totalfilebytes,
                    root.Keyword.Count(),
                    root.TotalByteSize,
                    root.BlockDate,
                    root.TransactionId,
                    root.Signature,
                    root.Confirmations,
                    root.BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt")
                };
                dgTransactions.Rows.Add(rowData);
                dgTransactions.AutoResizeRows();
                dgTransactions.AutoResizeColumns();

                totalbytes = root.TotalByteSize;
                lblTotalBytes.Text = "bytes: " + totalbytes.ToString();
                lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
                double secondsExpired = elapsedMilliseconds / 1000.0;
                double kilobytes = totalbytes / 1024.0;
                double kbs = kilobytes / secondsExpired;
                lblKbs.Text = "kb/s: " + Math.Truncate(kbs);
                if (root != null) { lblTotal.Text = "total: 1"; }

            }
        }

        private void BtnGetKeyword_Click(object sender, EventArgs e)
        {
            string publicAddress = Root.GetPublicAddressByKeyword(txtSearchAddress.Text, txtVersionByte.Text);
            DateTime tmbeginCall = DateTime.UtcNow;
            Root[] roots = Root.GetRootsByAddress(
                publicAddress,
                txtLogin.Text,
                txtPassword.Text,
                txtUrl.Text,
                txtVersionByte.Text
            );
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            dgTransactions.Rows.Clear();
            int totalbytes = 0;

            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            for (int i = 0; i < roots.Length; i += 1)
            {
                string strmessage = "";
                BigInteger totalfilebytes = 0;
                foreach (var rfile in roots[i].Message)
                {
                    strmessage += rfile;
                }
                foreach (var rfile in roots[i].File)
                {
                    totalfilebytes += rfile.Value;

                }

                object[] rowData = new object[]
                {
                    roots[i].Id,
                    strmessage,
                    roots[i].Signed,
                    roots[i].SignedBy,
                    roots[i].File.Count(),
                    totalfilebytes,
                    roots[i].Keyword.Count(),
                    roots[i].TotalByteSize,
                    roots[i].BlockDate,
                    roots[i].TransactionId,
                    roots[i].Signature,
                    roots[i].Confirmations,
                    roots[i].BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt")
   };
                dgTransactions.Rows.Add(rowData);
                totalbytes += roots[i].TotalByteSize;
            }
            dgTransactions.AutoResizeRows();
            dgTransactions.AutoResizeColumns();

            lblTotalBytes.Text = "bytes: " + totalbytes.ToString();
            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            double secondsExpired = elapsedMilliseconds / 1000.0;
            double kilobytes = totalbytes / 1024.0;
            double kbs = kilobytes / secondsExpired;
            lblKbs.Text = "kb/s: " + Math.Truncate(kbs);
            lblTotal.Text = "total:" + roots.Length.ToString();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true
            };
            byte[] result = new byte[0];
            DateTime tmbeginCall = DateTime.UtcNow;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                result = Root.GetRootBytesByFile(openFileDialog.FileNames);
                result = Root.EncryptRootBytes(txtLogin.Text, txtPassword.Text, txtUrl.Text, txtSearchAddress.Text, result);

            }
            DateTime tmendCall = DateTime.UtcNow;

            string jobId = Guid.NewGuid().ToString();
            Root root = Root.GetRootByTransactionId(jobId, null, null, null, txtVersionByte.Text, result);
            txtTransactionId.Text = jobId;
            
            dgTransactions.Rows.Clear();
            int totalbytes;

            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            if (root != null)
            {
                string strmessage = "";

                foreach (var rfile in root.Message)
                {
                    strmessage += rfile;

                }
                BigInteger totalfilebytes = 0;
                foreach (var rfile in root.File)
                {
                    totalfilebytes += rfile.Value;
                }

                object[] rowData = new object[]
                {
                    root.Id,
                    strmessage,
                    root.Signed,
                    root.SignedBy,
                    root.File.Count(),
                    totalfilebytes,
                    root.Keyword.Count(),
                    root.TotalByteSize,
                    root.BlockDate,
                    root.TransactionId,
                    root.Signature,
                    root.Confirmations,
                    root.BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt")
                };
                dgTransactions.Rows.Add(rowData);
                dgTransactions.AutoResizeRows();
                dgTransactions.AutoResizeColumns();

                totalbytes = root.TotalByteSize;
                lblTotalBytes.Text = "Total Bytes: " + totalbytes.ToString();
                lblTotalTime.Text = "Total Time: " + elapsedMilliseconds;
                double secondsExpired = elapsedMilliseconds / 1000.0;
                double kilobytes = totalbytes / 1024.0;
                double kbs = kilobytes / secondsExpired;
                lblKbs.Text = "Kb/s " + kbs;
            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            byte[] result = Root.GetRootBytesByFile(new string[] { @"root/" + txtTransactionId.Text + @"/SEC" });
            result = Root.DecryptRootBytes(txtLogin.Text, txtPassword.Text, txtUrl.Text, txtSearchAddress.Text, result);

            Root root = Root.GetRootByTransactionId(txtTransactionId.Text, null, null, null, txtVersionByte.Text, result);
            DateTime tmendCall = DateTime.UtcNow;
            dgTransactions.Rows.Clear();
            int totalbytes;

            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            if (root != null)
            {
                string strmessage = "";

                foreach (var rfile in root.Message)
                {
                    strmessage += rfile;

                }
                BigInteger totalfilebytes = 0;
                foreach (var rfile in root.File)
                {
                    totalfilebytes += rfile.Value;
                }

                object[] rowData = new object[]
                {
                    root.Id,
                    strmessage,
                    root.Signed,
                    root.SignedBy,
                    root.File.Count(),
                    totalfilebytes,
                    root.Keyword.Count(),
                    root.TotalByteSize,
                    root.BlockDate,
                    root.TransactionId,
                    root.Signature,
                    root.Confirmations,
                    root.BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt")
                };
                dgTransactions.Rows.Add(rowData);
                dgTransactions.AutoResizeRows();
                dgTransactions.AutoResizeColumns();

                totalbytes = root.TotalByteSize;
                lblTotalBytes.Text = "Total Bytes: " + totalbytes.ToString();
                lblTotalTime.Text = "Total Time: " + elapsedMilliseconds;
                double secondsExpired = elapsedMilliseconds / 1000.0;
                double kilobytes = totalbytes / 1024.0;
                double kbs = kilobytes / secondsExpired;
                lblKbs.Text = "Kb/s " + kbs;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            OBJState Tester = OBJState.GetObjectByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text);
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            lblTotal.Text = "total: " + Tester.ProcessHeight.ToString();
            txtGetValue.Text = JsonConvert.SerializeObject(Tester);
        }

        private void btnPurge_Click(object sender, EventArgs e)
        {
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            if (Directory.Exists("root")) { Directory.Delete("root", true); };
            txtGetValue.Clear();
        }

        private void btnGetOwned_Click(object sender, EventArgs e)
        {

            List<OBJState> ownedObjects = OBJState.GetObjectsOwnedByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtGetValue.Text = JsonConvert.SerializeObject(ownedObjects);
        }

        private void btnGetCreated_Click(object sender, EventArgs e)
        {

            List<OBJState> createdObjects = OBJState.GetObjectsCreatedByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtGetValue.Text = JsonConvert.SerializeObject(createdObjects);

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            List<OBJState> createdObjects = OBJState.GetObjectsByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtGetValue.Text = JsonConvert.SerializeObject(createdObjects);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            List<string> searchList = new List<string> { txtSearchAddress.Text };
            List<OBJState> createdObjects = OBJState.GetObjectsByKeyword(searchList, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtGetValue.Text = JsonConvert.SerializeObject(createdObjects);


        }
    }
}

