using LevelDB;
using NBitcoin.RPC;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Newtonsoft.Json;
using SUP.SEC;

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
            string walletUrl = txtUrl.Text;
            string walletUsername = txtLogin.Text;
            string walletPassword = txtPassword.Text;
            NetworkCredential credentials = new NetworkCredential(walletUsername, walletPassword);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(walletUrl));
            lstTransactions.Items.Clear();

            dynamic deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("searchrawtransactions", txtSearchAddress.Text, 1, 0, 500).ResultString);

            foreach (dynamic transID in deserializedObject)
            {
                string transaction = "";

                foreach (dynamic v_out in transID.vout)
                {

                    if (v_out.value == "5.46E-06" ||  v_out.value == "5.48E-06" || v_out.value == "5.48E-05" )
                    {
                        byte[] results = new byte[0];
                        string strPublicAddress = v_out.scriptPubKey.addresses[0];
                        Base58.DecodeWithCheckSum(strPublicAddress, out results);
                        transaction = transaction + Encoding.UTF8.GetString(results).Remove(0, 1);
                    }
                }

                lstTransactions.Items.Add(transaction);

            }

        }
    }
}
