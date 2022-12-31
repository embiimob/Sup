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
using System.CodeDom.Compiler;

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
            
            //used as P2FK Delimiters 
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

           //find all occurences of numbers that start with 0
            Regex regex = new Regex(@"\d*0\d+");



            NetworkCredential credentials = new NetworkCredential(walletUsername, walletPassword);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(walletUrl));

            //Clearing the datagrid of any data from previous searches
            dgTransactions.Rows.Clear();

            // calls the bitcoin wallet searchrawtransaction command and returns it as a dynamic deserialized object.
            dynamic deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("searchrawtransactions", txtSearchAddress.Text, 1, 0, 500).ResultString);


            //itterating through JSON search results
            foreach (dynamic transID in deserializedObject)
            {
                //defining items to include in the search results
                string transactionID = transID.txid;
                string confirmations = transID.confirmations;
                DateTime blocktime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(transID.blocktime)).DateTime; 
                byte[] transactionBytes = new byte[0];
                string transactionASCII = "";
                string strPublicAddress = "";
                int sigStartByte = 0;
                int sigEndByte = 0;
                string signature = "";
                object[] rowData = new object[0];
                var sigResult = false;

                foreach (dynamic v_out in transID.vout)
                {
                 
                    // checking for all known microtransaction values
                    if (v_out.value == "5.46E-06" || v_out.value == "5.48E-06" || v_out.value == "5.48E-05")
                    {   
                        byte[] results = new byte[0];
                        strPublicAddress = v_out.scriptPubKey.addresses[0];

                        //retreiving payload data from each address
                        Base58.DecodeWithCheckSum(strPublicAddress, out results);

                        //append to a byte[] of all P2FK data
                        transactionBytes = AddBytes(transactionBytes, RemoveFirstByte(results));

                    }
                }


                //review necessary P2FK header encoding. 
                //ASCII Header Encoding is working still troubleshooting why some signed objects are not being recogrnized as signed
                transactionASCII = Encoding.ASCII.GetString(transactionBytes);


                //a lot of overhead only needed for objects that ar signed
                if (transactionASCII.StartsWith("SIG"))
                {

                    
                    string input = transactionASCII;


                    // Perform the loop until no additional numbers are found and the regular expression fails to match 
                    while (input.IndexOfAny(specialChars) != -1 && regex.IsMatch(input))
                    {
                        Match match = regex.Match(input);
                        sigEndByte++;
                        //invalid if a special character is not found before the number
                        if (input.IndexOfAny(specialChars) == match.Index - 1 && match.Length > 1)
                        {
                           
                            sigEndByte += Int32.Parse(match.Value) + match.Index + match.Length;

                            if (sigStartByte == 0)
                            {

                                sigStartByte = sigEndByte;
                                
                                signature = input.Substring(match.Index + match.Length + 1, Int32.Parse(match.Value));
                            }

                                input = input.Remove(0, (Int32.Parse(match.Value) + match.Index + match.Length + 1));
                           
                        }
                        else { break; }
                        
                    }

                    //verify a hash of the byte data was signed by the last address found on the transaction 
                    System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                    byte[] hashValue = mySHA256.ComputeHash(transactionBytes.Skip(sigStartByte).Take(sigEndByte - sigStartByte).ToArray());
                    var result = rpcClient.SendCommand("verifymessage", strPublicAddress, signature, BitConverter.ToString(hashValue).Replace("-", String.Empty));
                    sigResult = Convert.ToBoolean(result.Result);
                }else { strPublicAddress = ""; }
                rowData = new object[] { transactionID, sigResult, signature, strPublicAddress, sigStartByte, sigEndByte, transactionASCII, transactionBytes.Length, confirmations, blocktime };
                dgTransactions.Rows.Add(rowData);
        

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
