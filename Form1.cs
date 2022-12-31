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
using System.Text.RegularExpressions;
using System.Security.Cryptography;

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
            dgTransactions.Rows.Clear();
            dynamic deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("searchrawtransactions", txtSearchAddress.Text, 1, 0, 500).ResultString);

            foreach (dynamic transID in deserializedObject)
            {
                string transaction = "";
                string transactionID = transID.txid;
                string confirmations = transID.confirmations;
                string blocktime = transID.blocktime;
                byte[] transactionBytes = new byte[0];
                string strPublicAddress = "";
                int sigStartByte = 0;
                int sigEndByte = 0;
                string signature = "";
                Boolean sigCheck = false;

                foreach (dynamic v_out in transID.vout)
                {

                    if (v_out.value == "5.46E-06" || v_out.value == "5.48E-06" || v_out.value == "5.48E-05")
                    {
                        byte[] results = new byte[0];
                        strPublicAddress = v_out.scriptPubKey.addresses[0];
                        Base58.DecodeWithCheckSum(strPublicAddress, out results);
                        transactionBytes = AddBytes(transactionBytes, RemoveFirstByte(results));

                    }
                }

                transaction = Encoding.UTF8.GetString(transactionBytes);
                if (transaction.StartsWith("SIG"))
                    {

                    char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
                    string input = transaction;

                    // Use a regular expression to match numbers in the input string
                    Regex regex = new Regex(@"\d+");


                    // Perform the loop until the regular expression fails to match any more numbers
                    while (input.IndexOfAny(specialChars) != -1 && regex.IsMatch(input))
                    {
                         Match match = regex.Match(input);

                        if (input.IndexOfAny(specialChars) == match.Index -1)
                        {
                            sigEndByte += Int32.Parse(match.Value) + match.Index + match.Length;
                        }
                        
                        if (sigStartByte == 0) { sigStartByte = sigEndByte; signature = input.Substring(match.Index + match.Length+1, Int32.Parse(match.Value));
                        }
                       
                        try { input = input.Remove(0, (Int32.Parse(match.Value) + match.Index + match.Length )); }
                        catch(Exception ex) {
                            
                            break; }


                    }

                    System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                    byte[] hashValue = mySHA256.ComputeHash(transactionBytes.Skip(sigStartByte + 1).Take(sigEndByte - sigStartByte).ToArray());
                    var result = rpcClient.SendCommand("verifymessage", strPublicAddress, signature, BitConverter.ToString(hashValue).Replace("-", String.Empty));
                    object[] rowData = { transactionID, result.Result, signature, strPublicAddress,sigStartByte,sigEndByte,transaction, confirmations, blocktime };
                    dgTransactions.Rows.Add(rowData);
                }





     

            }

        }
        public static byte[] AddBytes(byte[] existingArray, byte[] newArray)
        {
            // Create a new array with a capacity large enough to hold both arrays
            byte[] combinedArray = new byte[existingArray.Length + newArray.Length];

            // Copy the elements from the existing array into the new array
            Array.Copy(existingArray, combinedArray, existingArray.Length);

            // Copy the elements from the new array into the new array
            Array.Copy(newArray, 0, combinedArray, existingArray.Length, newArray.Length);

            return combinedArray;
        }

        public static byte[] RemoveFirstByte(byte[] array)
        {
            // Check if the array is empty
            if (array.Length == 0)
            {
                return array;
            }

            // Create a new array with a capacity one less than the original array
            byte[] newArray = new byte[array.Length - 1];

            // Copy the elements from the original array into the new array, starting at the second element
            Array.Copy(array, 1, newArray, 0, newArray.Length);

            return newArray;
        }
        private void lstTransactions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
