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
            var SUP = new Options { CreateIfMissing = true };

            switch (lbTableName.SelectedItem.ToString().Trim())
            {
                case "sup":
                   
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
                                    txtGetValue.Text + "[ " + it.KeyAsString() + " | "+ it.ValueAsString() +" ]" + Environment.NewLine;
                            }
                            it.Dispose();
                        }
                    
                    break;

                case "sig":

                    txtGetValue.Text = "";

                    using (var db = new DB(SUP, @"root\sig"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                    txtGetValue.Text + "[ " + it.KeyAsString() + " | " + it.ValueAsString() + " ]" + Environment.NewLine;
                        }
                        it.Dispose();
                    }

                    break;

                case "sec":

                    txtGetValue.Text = "";

                    using (var db = new DB(SUP, @"root\sec"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                    txtGetValue.Text + "[ " + it.KeyAsString() + " | " + it.ValueAsString() + " ]" + Environment.NewLine;
                        }
                        it.Dispose();
                    }

                    break;

                case "pro":

                    txtGetValue.Text = "";

                    using (var db = new DB(SUP, @"root\pro"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                    txtGetValue.Text + "[ " + it.KeyAsString() + " | " + it.ValueAsString() + " ]" + Environment.NewLine;
                        }
                        it.Dispose();
                    }

                    break;

                case "oblock":

                    txtGetValue.Text = "";

                    using (var db = new DB(SUP, @"root\oblock"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                    txtGetValue.Text + "[ " + it.KeyAsString() + " | " + it.ValueAsString() + " ]" + Environment.NewLine;
                        }
                        it.Dispose();
                    }

                    break;

                case "obj":

                    txtGetValue.Text = "";

                    using (var db = new DB(SUP, @"root\obj"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                    txtGetValue.Text + "[ " + it.KeyAsString() + " | " + it.ValueAsString() + " ]" + Environment.NewLine;
                        }
                        it.Dispose();
                    }

                    break;
                case "mute":

                    txtGetValue.Text = "";

                    using (var db = new DB(SUP, @"root\mute"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                    txtGetValue.Text + "[ " + it.KeyAsString() + " | " + it.ValueAsString() + " ]" + Environment.NewLine;
                        }
                        it.Dispose();
                    }

                    break;
                case "found":

                    txtGetValue.Text = "";

                    using (var db = new DB(SUP, @"root\found"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                            it.Seek(txtlevelDBKey.Text);
                            it.IsValid() && it.KeyAsString().StartsWith(txtlevelDBKey.Text);
                            it.Next()
                        )
                        {
                            txtGetValue.Text =
                                    txtGetValue.Text + "[ " + it.KeyAsString() + " | " + it.ValueAsString() + " ]" + Environment.NewLine;
                        }
                        it.Dispose();
                    }

                    break;

                case "event":
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
                                    txtGetValue.Text + "[ " + it.KeyAsString() + " | " + it.ValueAsString() + " ]" + Environment.NewLine;
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
            var SUP = new Options { CreateIfMissing = true };
            switch (lbTableName.SelectedItem.ToString().Trim())
            {
                case "sup":
                    
                    using (var db = new DB(SUP, @"root\sup"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;
                case "sig":

                    using (var db = new DB(SUP, @"root\sig"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;

                case "sec":

                    using (var db = new DB(SUP, @"root\sec"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;

                case "pro":

                    using (var db = new DB(SUP, @"root\pro"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;

                case "oblock":

                    using (var db = new DB(SUP, @"root\sup"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;
                case "obj":

                    using (var db = new DB(SUP, @"root\obj"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;
                case "mute":

                    using (var db = new DB(SUP, @"root\mute"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;
                case "found":

                    using (var db = new DB(SUP, @"root\found"))
                    {
                        db.Delete(txtlevelDBKey.Text);
                    }
                    break;

                case "event":

                    using (var db = new DB(SUP, @"root\event"))
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
            RPCClient rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);

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
            OBJState Tester = OBJState.GetObjectByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, checkBox1.Checked);
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

        private void ButtonPurgeClick(object sender, EventArgs e)
        {
            string directoryPath = @"root";

            // Get a list of all the subdirectories in the directory
            string[] subdirectories = Directory.GetDirectories(directoryPath);

            // Loop through each subdirectory
            foreach (string subdirectory in subdirectories)
            {

                // Delete the subdirectory and all its contents
                Directory.Delete(subdirectory, true);

            }

            // Get a list of all the files in the directory
            string[] files = Directory.GetFiles(directoryPath);

            // Loop through each file
            foreach (string file in files)
            {
                // Delete the file
                File.Delete(file);
            }


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
            List<OBJState> createdObjects = OBJState.GetObjectsByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtSkip.Text), int.Parse(txtQty.Text));
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

                    DisplayRootJSON(new JObject[] { JObject.Parse(System.IO.File.ReadAllText(@"root\" + transactionID + @"\OBJ.json")) });
                }
                catch { }
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
            PROState Tester = PROState.GetProfileByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, checkBox1.Checked, int.Parse(txtSkip.Text));
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
            PROState createdObject = PROState.GetProfileByURN(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, checkBox1.Checked, int.Parse(txtSkip.Text));
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
           
            if (txtTransactionId.Text.Length > 42)
            {
                try { System.IO.Directory.Delete(@"root\" + txtTransactionId.Text, true); } catch { }
            }
        }

        private void ButtonBlockAddressClick(object sender, EventArgs e)
        {

            try
            {
                var WORK = new Options { CreateIfMissing = true };
                using (var db = new DB(WORK, @"root\oblock"))
                {
                    db.Put(txtSearchAddress.Text, "true");

                }
                var WORK2 = new Options { CreateIfMissing = true };
                using (var db = new DB(WORK2, @"root\oblock2"))
                {
                    db.Put(txtSearchAddress.Text, "true");

                }

                var SUP = new Options { CreateIfMissing = true };
                var keysToDelete = new HashSet<string>(); // Create a new HashSet to store the keys to delete

                using (var db = new DB(SUP, @"root\found"))
                {
                    LevelDB.Iterator it = db.CreateIterator();

                    for (
                        it.SeekToLast();
                        it.IsValid();
                        it.Prev()
                    )
                    {
                        string key = it.KeyAsString();
                        if (key.Contains(txtSearchAddress.Text))
                        {
                            keysToDelete.Add(key); // Add the key to the HashSet
                        }
                    }

                    it.Dispose();

                    var batch = new WriteBatch(); // Create a new WriteBatch to delete the keys
                    foreach (var key in keysToDelete)
                    {
                        batch.Delete(key); // Add a delete operation for each key in the HashSet
                    }
                    db.Write(batch); // Execute the batch to delete the keys from the database
                }


                Root[] root = Root.GetRootsByAddress(txtSearchAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");

                foreach (Root rootItem in root)
                {

                    using (var db = new DB(WORK, @"root\tblock"))
                    {
                        db.Put(rootItem.TransactionId, "true");

                    }
                    try
                    {
                        Directory.Delete(@"root\" + rootItem.TransactionId, true);
                    }
                    catch { }
                }
                try
                {

                    string diskpath = "root\\" + txtSearchAddress.Text + "\\";

                    // fetch current JSONOBJ from disk if it exists
                    try
                    {
                        string JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetRootByTransactionId.json");
                        OBJState objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
                        try
                        {
                            if (objectState.URN != null)
                            {
                                try { Directory.Delete(@"root\" + GetTransactionId(objectState.URN), true); } catch { }
                                try { Directory.Delete(@"ipfs\" + GetTransactionId(objectState.URN), true); } catch { }
                            }
                            if (objectState.Image != null)
                            {
                                try { Directory.Delete(@"root\" + GetTransactionId(objectState.Image), true); } catch { }
                                try { Directory.Delete(@"ipfs\" + GetTransactionId(objectState.Image), true); } catch { }
                            }
                        }
                        catch { }
                    }
                    catch { }

                    Directory.Delete(@"root\" + txtSearchAddress.Text, true);

                }
                catch { }
            }
            catch { }


            }

        private void ButtonUnBlockAddressClick(object sender, EventArgs e)
        {
            var WORK = new Options { CreateIfMissing = true };
            using (var db = new DB(WORK, @"root\oblock"))
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
            try { splitContainer1.SplitterDistance = 0; } catch { }
        }

        private void WorkBench_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
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

                payload[0] = Byte.Parse(txtVersionByte.Text);
                string objectaddress = Base58.EncodeWithCheckSum(payload);

                txtGetValue.Text = @"Use the following keyword to register this file --> @" + objectaddress;

            }
        }

        private void ButtonGetFoundObjectsClick(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            List<OBJState> ownedObjects = OBJState.GetFoundObjects(txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtSkip.Text), int.Parse(txtQty.Text));
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

        private void ButtonGetObjectByTransactionId_Click(object sender, EventArgs e)
        {

            DateTime tmbeginCall = DateTime.UtcNow;
            OBJState Tester = OBJState.GetObjectByTransactionId(txtTransactionId.Text,txtLogin.Text,txtPassword.Text,txtUrl.Text,txtVersionByte.Text);
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

        private void ButtonGetPublicMessages_Click(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            dynamic deserializedObject = OBJState.GetPublicMessagesByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtSkip.Text), int.Parse(txtQty.Text));
            DateTime tmendCall = DateTime.UtcNow;

            // Cast deserializedObject to a specific type
            var messages = ((List<object>)deserializedObject.Messages).Cast<dynamic>();

            // Get the count of messages
            int messageCount = messages.Count();



            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            lblTotal.Text = "total: " + messageCount.ToString();
            //var json = JsonConvert.SerializeObject(deserializedObject);
            DisplayRootJSON(new JObject[] { JObject.FromObject(deserializedObject) });
        }

        private void ButtonGetPrivateMessages_Click(object sender, EventArgs e)
        {


            DateTime tmbeginCall = DateTime.UtcNow;
            dynamic deserializedObject = OBJState.GetPrivateMessagesByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtSkip.Text), int.Parse(txtQty.Text));
            DateTime tmendCall = DateTime.UtcNow;

            // Cast deserializedObject to a specific type
            var messages = ((List<object>)deserializedObject.Messages).Cast<dynamic>();

            // Get the count of messages
            int messageCount = messages.Count();



            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";
            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);
            lblTotal.Text = "total: " + messageCount.ToString();
            //var json = JsonConvert.SerializeObject(deserializedObject);
            DisplayRootJSON(new JObject[] { JObject.FromObject(deserializedObject) });




        }

        private void ButtonGetPublicKeys_Click(object sender, EventArgs e)
        {
            DateTime tmbeginCall = DateTime.UtcNow;
            var root = Root.GetPublicKeysByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            var json = JsonConvert.SerializeObject(root);

            DateTime tmendCall = DateTime.UtcNow;

            lblTotalBytes.Text = "bytes: ";
            lblTotalTime.Text = "time: ";
            lblKbs.Text = "kb/s: ";
            lblTotal.Text = "total:";

            TimeSpan elapsedTime = tmendCall - tmbeginCall;
            double elapsedMilliseconds = elapsedTime.TotalMilliseconds;

            lblTotalTime.Text = "time: " + Math.Truncate(elapsedMilliseconds);

            txtGetValue.Text = json;
        }

        public string GetTransactionId(string input)
        {
            int startIndex = input.IndexOf(":") + 1;
            if (startIndex == 0)
            {
                // No colon found, return the original string
                startIndex = 0;
            }

            int endIndex = input.IndexOf("/");
            if (endIndex == -1)
            {
                // No slash found, return the substring starting from the start index
                return input.Substring(startIndex);
            }

            // Return the substring between the colon and the slash
            return input.Substring(startIndex, endIndex - startIndex);
        }

        private void btnEditProfile_Click(object sender, EventArgs e)
        {
          
        }
    }
}

