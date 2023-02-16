using LevelDB;
using NBitcoin.Protocol;
using NBitcoin.RPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Forms;

namespace SUP
{
    public partial class WorkBench : Form
    {
        public WorkBench()
        {
            InitializeComponent();
        }

        private void ButtonGetLevelDBClick(object sender, EventArgs e)
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

                case "SUP":
                    var SUP = new Options { CreateIfMissing = true };
                    txtGetValue.Text = "";
                    using (var db = new DB(SUP, @"root\sup"))
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

        private void ButtonDeleteLevelDBClick(object sender, EventArgs e)
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

        private void ButtonTestConnectionClick(object sender, EventArgs e)
        {
            string walletUrl = txtUrl.Text;
            string walletUsername = txtLogin.Text;
            string walletPassword = txtPassword.Text;
            NetworkCredential credentials = new NetworkCredential(walletUsername, walletPassword);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(walletUrl),NBitcoin.Network.Main);

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

        private void ButtonSearchClick(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            Root[] roots = Root.GetRootsByAddress(
                txtSearchAddress.Text,
                txtLogin.Text,
                txtPassword.Text,
                txtUrl.Text,0,300,
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

        private void ButtonGetTransactionIdClick(object sender, EventArgs e)
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

        private void ButtonGetKeywordClick(object sender, EventArgs e)
        {
            string publicAddress = Root.GetPublicAddressByKeyword(txtSearchAddress.Text, txtVersionByte.Text);
            DateTime tmbeginCall = DateTime.UtcNow;
            Root[] roots = Root.GetRootsByAddress(
                publicAddress,
                txtLogin.Text,
                txtPassword.Text,
                txtUrl.Text, 0, 300,
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

        private void ButtonEncryptTransactionIdClick(object sender, EventArgs e)
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

        private void ButtonDecryptTransactionIdClick(object sender, EventArgs e)
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

        private void ButtonGetObjectByAddressClick(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            OBJState Tester = OBJState.GetObjectByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text,checkBox1.Checked);
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            lblTotal.Text = "total: " + Tester.ProcessHeight.ToString();
                       
            DisplayRootJSON(new JObject[] { JObject.FromObject(Tester)});
        }

        private void ButtonPurgeClick(object sender, EventArgs e)
        {
            


        }

        private void ButtonGetOwnedClick(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            List<OBJState> ownedObjects = OBJState.GetObjectsOwnedByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtSkip.Text), int.Parse(txtQty.Text));
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            lblTotal.Text = "total: " + ownedObjects.Count();

            JObject[] ObjectArray = new JObject[ownedObjects.Count];
            int objectcount = 0;
            foreach (object obj in ownedObjects)
            {
                
                ObjectArray[objectcount++] = JObject.FromObject(obj);
            }
            DisplayRootJSON(ObjectArray);
        }

        private void ButtonGetCreatedClick(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            List<OBJState> createdObjects = OBJState.GetObjectsCreatedByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtSkip.Text), int.Parse(txtQty.Text));
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            lblTotal.Text = "total: " + createdObjects.Count();
            JObject[] ObjectArray = new JObject[createdObjects.Count];
            int objectcount = 0;
            foreach (object obj in createdObjects)
            {

                ObjectArray[objectcount++] = JObject.FromObject(obj);
            }
            DisplayRootJSON(ObjectArray);

        }

        private void ButtonGetObjectsByAddressClick(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            List<OBJState> createdObjects = OBJState.GetObjectsByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text,txtVersionByte.Text, int.Parse(txtSkip.Text), int.Parse(txtQty.Text));
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            lblTotal.Text = "total: " + createdObjects.Count();
            JObject[] ObjectArray = new JObject[createdObjects.Count];
            int objectcount = 0;

     
            foreach (object obj in createdObjects)
            {

                ObjectArray[objectcount++] = JObject.FromObject(obj);
            }
            DisplayRootJSON(ObjectArray);
        }

        private void ButtonGetObjectsByKeywordClick(object sender, EventArgs e)
        {

            List<string> searchList = new List<string> { txtSearchAddress.Text };
            DateTime tmbeginCall = DateTime.UtcNow;
            List<OBJState> createdObjects = OBJState.GetObjectsByKeyword(searchList, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtSkip.Text), int.Parse(txtQty.Text));
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            lblTotal.Text = "total: " + createdObjects.Count();
            JObject[] ObjectArray = new JObject[createdObjects.Count];
            int objectcount = 0;
            foreach (object obj in createdObjects)
            {

                ObjectArray[objectcount++] = JObject.FromObject(obj);
            }
            DisplayRootJSON(ObjectArray);


        }

        private void DataGridViewClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {

                try
                {

                    string transactionID = dgTransactions.Rows[e.RowIndex].Cells[9].Value.ToString();
                    
                    DisplayRootJSON(new JObject[] { JObject.Parse(System.IO.File.ReadAllText(@"root\" + transactionID + @"\P2Fk.json")) });
                                   }
                catch{ }
            }
        }

        private void ButtonGetObjectByURNClick(object sender, EventArgs e)
        {
                    
            DateTime tmbeginCall = DateTime.UtcNow;
            OBJState createdObject = OBJState.GetObjectByURN(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
           
            JObject[] ObjectArray = new JObject[1];
                  
           ObjectArray[0] = JObject.FromObject(createdObject);
           
            DisplayRootJSON(ObjectArray);

        }

        private void ButtonGetProfileByAddressClick(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            PROState Tester = PROState.GetProfileByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, checkBox1.Checked,int.Parse(txtSkip.Text));
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            lblTotal.Text = "total: " + Tester.ProcessHeight.ToString();

            DisplayRootJSON(new JObject[] { JObject.FromObject(Tester) });
        }

        private void ButtonProfileURNClick(object sender, EventArgs e)
        {

            DateTime tmbeginCall = DateTime.UtcNow;
            PROState createdObject = PROState.GetProfileByURN(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text,int.Parse(txtSkip.Text));
            DateTime tmendCall = DateTime.UtcNow;
            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);

            JObject[] ObjectArray = new JObject[1];

            ObjectArray[0] = JObject.FromObject(createdObject);

            DisplayRootJSON(ObjectArray);
        }

        private void ButtonBlockTransactionIdClick(object sender, EventArgs e)
        {
            var WORK = new Options { CreateIfMissing = true };
            using (var db = new DB(WORK, @"root\block"))
            {
                db.Put(txtTransactionId.Text, "true");
            }
        }

        private void ButtonUnblockTransactionIdClick(object sender, EventArgs e)
        {

            var WORK = new Options { CreateIfMissing = true };
            using (var db = new DB(WORK, @"root\block"))
            {
                db.Delete(txtTransactionId.Text);
            }

        }

        private void ButtonBlockAddressClick(object sender, EventArgs e)
        {
            
                var WORK = new Options { CreateIfMissing = true };
                using (var db = new DB(WORK, @"root\block"))
                {
                    db.Put(txtSearchAddress.Text,"true");
                }

            
        }

        private void ButtonUnBlockAddressClick(object sender, EventArgs e)
        {
            var WORK = new Options { CreateIfMissing = true };
            using (var db = new DB(WORK, @"root\block"))
            {
                db.Delete(txtSearchAddress.Text);
            }
        }

        private void ButtonMuteAddressClick(object sender, EventArgs e)
        {
            var WORK = new Options { CreateIfMissing = true };
            using (var db = new DB(WORK, @"root\mute"))
            {
                db.Put(txtSearchAddress.Text, "true");
            }
        }

        private void ButtonUnMuteAddressClick(object sender, EventArgs e)
        {
            var WORK = new Options { CreateIfMissing = true };
            using (var db = new DB(WORK, @"root\mute"))
            {
                db.Delete(txtSearchAddress.Text);
            }
        }

        private void DisplayRootJSON(JObject[] jsonObject)
        {
            // parse the JSON string
            JObject[] json = jsonObject;

            // use JsonConvert to format the JSON
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                NullValueHandling = NullValueHandling.Ignore
            };
            string formattedJson = JsonConvert.SerializeObject(json, settings);

            // clear the RichTextBox
            txtGetValue.Clear();

            // set the color for the different elements
            txtGetValue.SelectionColor = Color.Blue; // for key
            txtGetValue.SelectionBackColor = System.Drawing.SystemColors.Control;
            txtGetValue.SelectionFont = new Font(txtGetValue.SelectionFont, FontStyle.Bold);

            string[] lines = formattedJson.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("\""))
                {
                    txtGetValue.AppendText(line + Environment.NewLine);
                    try { txtGetValue.SelectionStart = txtGetValue.TextLength - line.Length - 1; } catch { }
                    txtGetValue.SelectionLength = line.IndexOf(':') + 1;
                    txtGetValue.SelectionColor = Color.Green; // for key
                    try { txtGetValue.SelectionFont = new Font(txtGetValue.SelectionFont, FontStyle.Bold); } catch { }
                }
                else if (line.Contains(":"))
                {
                    txtGetValue.AppendText(line + Environment.NewLine);
                    try
                    {
                        txtGetValue.SelectionStart = txtGetValue.TextLength - line.Length - 1;
                    }
                    catch { }
                    txtGetValue.SelectionLength = line.Length;
                    txtGetValue.SelectionColor = Color.Black; // for value
                }
                else
                {
                    txtGetValue.AppendText(line + Environment.NewLine);
                    try
                    {
                        txtGetValue.SelectionStart = txtGetValue.TextLength - line.Length - 1;
                    }
                    catch { }
                    txtGetValue.SelectionLength = line.Length;
                    txtGetValue.SelectionColor = Color.Red; // for separator
                }
            }
        }

        private void OnContainerResize(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = 0;
        }

        private void WorkBench_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
    {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void WorkBench_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string[] filePaths = (string[])e.Data.GetData((System.Windows.Forms.DataFormats.FileDrop));
                string filepath = filePaths[0];
                byte[] payload = new byte[21];

                using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
                {
                    fileStream.Read(payload, 1, 20);
                }

                payload[0] = Byte.Parse("111");
                string objectaddress = Base58.EncodeWithCheckSum(payload);

                txtGetValue.Text = @"Use the following keyword to register this file --> @"+objectaddress;

            }
        }
    }
}

