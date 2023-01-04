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
                case "PRO":
                    var PRO = new Options { CreateIfMissing = true };
                    using (var db = new DB(PRO, @"PRO"))
                    {
                        db.Put(txtPutKey.Text, txtPutValue.Text);
                    }
                    break;

                case "COL":
                    var COL = new Options { CreateIfMissing = true };
                    using (var db = new DB(COL, @"COL"))
                    {
                        db.Put(txtPutKey.Text, txtPutValue.Text);
                    }
                    break;

                case "OBJ":
                    var OBJ = new Options { CreateIfMissing = true };
                    using (var db = new DB(OBJ, @"OBJ"))
                    {
                        db.Put(txtPutKey.Text, txtPutValue.Text);
                    }
                    break;

                case "LOG":
                    var LOG = new Options { CreateIfMissing = true };
                    using (var db = new DB(LOG, @"LOG"))
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
                    using (var db = new DB(ROOT, @"ROOT"))
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
                    using (var db = new DB(PRO, @"PRO"))
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
                    using (var db = new DB(COL, @"COL"))
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
                    using (var db = new DB(OBJ, @"OBJ"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();
                        for (it.Seek(txtGetKey.Text); it.IsValid() && it.KeyAsString().StartsWith(txtGetKey.Text); it.Next())
                        {
                            txtGetValue.Text = txtGetValue.Text + it.ValueAsString() + Environment.NewLine;
                        }
                        it.Dispose();
                    }
                    break;

                case "LOG":
                    var LOG = new Options { CreateIfMissing = true };
                    txtGetValue.Text = "";
                    using (var db = new DB(LOG, @"LOG"))
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
                case "PRO":
                    var PRO = new Options { CreateIfMissing = true };
                    using (var db = new DB(PRO, @"PRO"))
                    {
                        db.Delete(txtDeleteKey.Text);

                    }
                    break;

                case "COL":
                    var COL = new Options { CreateIfMissing = true };
                    using (var db = new DB(COL, @"COL"))
                    {
                        db.Delete(txtDeleteKey.Text);
                    }
                    break;

                case "OBJ":
                    var OBJ = new Options { CreateIfMissing = true };
                    using (var db = new DB(OBJ, @"OBJ"))
                    {
                        db.Delete(txtDeleteKey.Text);
                    }
                    break;

                case "LOG":
                    var LOG = new Options { CreateIfMissing = true };
                    using (var db = new DB(LOG, @"LOG"))
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

            Root[] roots = Root.GetRootsByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            dgTransactions.Rows.Clear();


            for (int i = 0; i < roots.Length; i += 1)
            {

                var ROOT = new Options { CreateIfMissing = true };
                using (var db = new DB(ROOT, @"ROOT"))
                {
                    db.Put(roots[i].TransactionId, JsonConvert.SerializeObject(roots[i]));
                }
                //object[] rowData = new object[] { roots[i].TransactionId, roots[i].Signed, roots[i].Signature, roots[i].SignedBy, Encoding.ASCII.GetString(roots[i].RawBytes), roots[i].Confirmations, roots[i].BlockDate, roots[i].BuildDate };
                object[] rowData = new object[] { roots[i].TransactionId, roots[i].Signed, roots[i].Signature, roots[i].SignedBy, roots[i].File.Count(), roots[i].Message.Count(), roots[i].Keyword.Count(), roots[i].TotalByteSize, roots[i].Confirmations, roots[i].BlockDate, roots[i].BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt") };
                dgTransactions.Rows.Add(rowData);
            }



        }

        private void btnGetTransactionId_Click(object sender, EventArgs e)
        {
            Root root = Root.GetRootByTransactionId(txtTransactionId.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            dgTransactions.Rows.Clear();

            var ROOT = new Options { CreateIfMissing = true };
            using (var db = new DB(ROOT, @"ROOT"))
            {
                db.Put(root.TransactionId, JsonConvert.SerializeObject(root));
            }
            //object[] rowData = new object[] { roots[i].TransactionId, roots[i].Signed, roots[i].Signature, roots[i].SignedBy, Encoding.ASCII.GetString(roots[i].RawBytes), roots[i].Confirmations, roots[i].BlockDate, roots[i].BuildDate };
            object[] rowData = new object[] { root.TransactionId, root.Signed, root.Signature, root.SignedBy, root.File.Count(), root.Message.Count(), root.Keyword.Count(), root.TotalByteSize, root.Confirmations, root.BlockDate, root.BuildDate.ToString("MM/dd/yyyy hh:mm:ss.ffff tt") };
            dgTransactions.Rows.Add(rowData);
        }


    }
}

