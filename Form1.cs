using LevelDB;
using NBitcoin.RPC;
using Newtonsoft.Json;
using SUP.P2FK;
using System;
using System.Linq;
using System.Net;
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

            if (lbTableName.SelectedItem == null) { lbTableName.SelectedIndex = 0; }


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


            if (lbTableName.SelectedItem == null) { lbTableName.SelectedIndex = 0; }


            switch (lbTableName.SelectedItem.ToString().Trim())
            {


                case "ROOT":
                    var ROOT = new Options { CreateIfMissing = true };
                    txtGetValue.Text = "";
                    using (var db = new DB(ROOT, @"root"))
                    {

                        LevelDB.Iterator it = db.CreateIterator();
                        for (it.Seek(txtGetKey.Text); it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text); it.Next())
                        {
                            txtGetValue.Text = txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
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
                        for (it.Seek(txtGetKey.Text); it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text); it.Next())
                        {
                            txtGetValue.Text = txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
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
                        for (it.Seek(txtGetKey.Text); it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text); it.Next())
                        {
                            txtGetValue.Text = txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
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
                        for (it.Seek(txtGetKey.Text); it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text); it.Next())
                        {
                            txtGetValue.Text = txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
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
                        for (it.Seek(txtGetKey.Text); it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text); it.Next())
                        {
                            txtGetValue.Text = txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
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

            if (lbTableName.SelectedItem == null) { lbTableName.SelectedIndex = 0; }


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
            txtbalance.Text = rpcClient.GetBalance().ToString();

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            Root[] roots = Root.GetRootByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            dgTransactions.Rows.Clear();
            int totalbytes = 0;

            for (int i = 0; i < roots.Length; i += 1)
            {
                if (roots[i].TransactionId != null)
                {
                    var ROOT = new Options { CreateIfMissing = true };
                    using (var db = new DB(ROOT, @"root"))
                    {
                        db.Put(roots[i].TransactionId, JsonConvert.SerializeObject(roots[i]));
                    }
                }
                string strmessage = "";
                
                foreach (var rfile in roots[i].Message)
                { strmessage = strmessage + rfile; }

                object[] rowData = new object[] { roots[i].TransactionId, roots[i].Signed, roots[i].SignedBy, roots[i].Signature, roots[i].File.Count(), strmessage, roots[i].Keyword.Count(), roots[i].TotalByteSize, roots[i].BlockDate, roots[i].Confirmations, roots[i].BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt") };
                dgTransactions.Rows.Add(rowData);
                totalbytes += roots[i].TotalByteSize;
            }
            lblTotalBytes.Text = "Total Bytes: " + totalbytes.ToString();



        }

        private void btnGetTransactionId_Click(object sender, EventArgs e)
        {
            Root root = Root.GetRootByTransactionId(txtTransactionId.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            dgTransactions.Rows.Clear();

            if (root != null)
            {
                var ROOT = new Options { CreateIfMissing = true };
                using (var db = new DB(ROOT, @"root"))
                {
                    db.Put(root.TransactionId, JsonConvert.SerializeObject(root));

                }
                string strmessage = "";

                foreach (var rfile in root.Message)
                { strmessage = strmessage + rfile; }
                object[] rowData = new object[] { root.TransactionId, root.Signed, root.SignedBy, root.Signature, root.File.Count(), strmessage, root.Keyword.Count(), root.TotalByteSize, root.BlockDate, root.Confirmations, root.BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt") };
                dgTransactions.Rows.Add(rowData);

                lblTotalBytes.Text = "Total Bytes: " + root.TotalByteSize.ToString();
            }

        }

        private void btnGetKeyword_Click(object sender, EventArgs e)
        {
            string publicAddress = Root.GetPublicAddressByKeyword(txtGetKeyword.Text);

            Root[] roots = Root.GetRootByAddress(publicAddress, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            dgTransactions.Rows.Clear();
            int totalbytes = 0;

            for (int i = 0; i < roots.Length; i += 1)
            {

                var ROOT = new Options { CreateIfMissing = true };
                using (var db = new DB(ROOT, @"root"))
                {
                    db.Put(roots[i].TransactionId, JsonConvert.SerializeObject(roots[i]));
                }

                string strmessage = "";

                foreach (var rfile in roots[i].Message)
                { strmessage = strmessage + rfile; }

                object[] rowData = new object[] { roots[i].TransactionId, roots[i].Signed, roots[i].SignedBy, roots[i].Signature, roots[i].File.Count(), strmessage, roots[i].Keyword.Count(), roots[i].TotalByteSize, roots[i].BlockDate, roots[i].Confirmations, roots[i].BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt") };
                dgTransactions.Rows.Add(rowData);
                totalbytes += roots[i].TotalByteSize;
            }
            lblTotalBytes.Text = "Total Bytes: " + totalbytes.ToString();
        }
    }
}

