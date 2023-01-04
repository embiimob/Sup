using NBitcoin.RPC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SUP.P2FK
{
    public class Root
    {
        public string TransactionId { get; set; }
        public string Hash { get; set; }
        public string Signature { get; set; }
        public bool Signed { get; set; }
        public string SignedBy { get; set; }
        public Dictionary<string, byte[]> File { get; set; }
        public string[] Message { get; set; }
        public Dictionary<string, string> Keyword { get; set; }
        public DateTime BlockDate { get; set; }
        public int TotalByteSize { get; set; }
        public int Confirmations { get; set; }
        public DateTime BuildDate { get; set; }


        public static Root[] GetRootsByAddress(string address, string username, string password, string url, int skip = 0, int qty = 100)
        {

            //used as P2FK Delimiters 
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            Regex regex = new Regex(@"([\\/:*?""<>|])\d+");

            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));

            dynamic deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("searchrawtransactions", address, 1, skip, qty).ResultString);


            //used in signature verification
            System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();


            // calls the bitcoin testnet wallet's searchrawtransaction command and returns it as a dynamic deserialized object.


            //create a list to hold the Root objects
            List<Root> RootList = new List<Root>();

            //itterating through JSON search results
            foreach (dynamic transID in deserializedObject)
            {
                //defining items to include in the returned object

                Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
                Dictionary<string, string> keywords = new Dictionary<string, string>();
                List<String> MessageList = new List<String>();
                byte[] transactionBytes = Array.Empty<byte>();
                string transactionASCII;
                string strPublicAddress = "";
                string signature = "";
                int sigStartByte = 0;
                int sigEndByte = 0;
                int totalByteSize = transID.size;

                //create a new root object
                Root newRoot = new Root();

                // we are spinning through all the out addresses within each bitcoin transaction
                // we are base58 decdoing each address to obtain a 20 byte payload that is appended to a byte[]
                foreach (dynamic v_out in transID.vout)
                {

                    // checking for all known P2FK bitcoin testnet microtransaction values
                    if (v_out.value == "5.46E-06" || v_out.value == "5.48E-06" || v_out.value == "5.48E-05")
                    {



                        byte[] results = Array.Empty<byte>();
                        strPublicAddress = v_out.scriptPubKey.addresses[0];

                        //retreiving payload data from each address
                        Base58.DecodeWithCheckSum(strPublicAddress, out results);

                        //append to a byte[] of all P2FK data
                        transactionBytes = AddBytes(transactionBytes, RemoveFirstByte(results));

                    }
                }

                // newRoot.RawBytes = transactionBytes;
                //ASCII Header Encoding is working still troubleshooting why some signed objects are not being recogrnized as signed
                transactionASCII = Encoding.ASCII.GetString(transactionBytes);


                // Perform the loop until no additional numbers are found and the regular expression fails to match 
                while (regex.IsMatch(transactionASCII))
                {
                    Match match = regex.Match(transactionASCII);
                    int mValue = Int32.Parse(match.Value.ToString().Remove(0, 1));




                    //invalid if a special character is not found before the number
                    if (transactionASCII.IndexOfAny(specialChars) == match.Index)
                    {

                        sigEndByte += mValue + match.Index + match.Length + 1;

                        string fileName = transactionASCII.Substring(0, transactionASCII.IndexOfAny(specialChars));


                        if (fileName == "SIG")
                        {

                            sigStartByte = sigEndByte;

                            signature = transactionASCII.Substring(match.Index + match.Length + 1, mValue);
                        }


                        byte[] filebytes = transactionBytes.Skip(match.Index + match.Length + 1 + (transactionBytes.Count() - transactionASCII.Length)).Take(mValue).ToArray();

                        if (fileName != "")
                        {

                            files.Add(fileName, filebytes);

                        }
                        else { MessageList.Add(Encoding.UTF8.GetString(filebytes)); }



                        transactionASCII = transactionASCII.Remove(0, (mValue + match.Index + match.Length + 1));

                    }
                    else
                    {


                        break;
                    }

                }


                for (int i = 0; i < transactionASCII.Length; i += 20)
                {
                    keywords.Add(Base58.EncodeWithCheckSum(AddBytes(new byte[] { byte.Parse("111") }, transactionBytes.Skip(i + (transactionBytes.Count() - transactionASCII.Length)).Take(20).ToArray())), Encoding.ASCII.GetString(transactionBytes.Skip(i + (transactionBytes.Count() - transactionASCII.Length)).Take(20).ToArray()));

                }

                if (sigStartByte > 0)
                {
                    //verify a hash of the byte data was signed by the last address found on the transaction 


                    newRoot.Hash = BitConverter.ToString(mySHA256.ComputeHash(transactionBytes.Skip(sigStartByte).Take(sigEndByte - sigStartByte).ToArray())).Replace("-", String.Empty);
                    var result = rpcClient.SendCommand("verifymessage", strPublicAddress, signature, newRoot.Hash);
                    newRoot.Signed = Convert.ToBoolean(result.Result);
                    keywords.Remove(strPublicAddress);
                }
                else { strPublicAddress = ""; }

                newRoot.TransactionId = transID.txid;
                newRoot.Signature = signature;
                newRoot.SignedBy = strPublicAddress;
                newRoot.BlockDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(transID.blocktime)).DateTime;
                newRoot.File = files;
                newRoot.Message = MessageList.ToArray();
                newRoot.Keyword = keywords;
                newRoot.TotalByteSize = totalByteSize;
                newRoot.Confirmations = transID.confirmations;
                newRoot.BuildDate = DateTime.UtcNow;
                RootList.Add(newRoot);

            }
            return RootList.ToArray();
        }


        public static Root GetRootByTransactionId(string transactionid, string username, string password, string url)
        {

            //used as P2FK Delimiters 
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            Regex regex = new Regex(@"([\\/:*?""<>|])\d+");

            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));

            dynamic deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("getrawtransaction", transactionid, 1).ResultString);


            //used in signature verification
            System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();

       
            //defining items to include in the returned object

            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
            Dictionary<string, string> keywords = new Dictionary<string, string>();
            List<String> MessageList = new List<String>();
            byte[] transactionBytes = Array.Empty<byte>();
            string transactionASCII;
            string strPublicAddress = "";
            string signature = "";
            int sigStartByte = 0;
            int sigEndByte = 0;
            int totalByteSize = deserializedObject.size;

            //create a new root object
            Root newRoot = new Root();

            // we are spinning through all the out addresses within each bitcoin transaction
            // we are base58 decdoing each address to obtain a 20 byte payload that is appended to a byte[]
            foreach (dynamic v_out in deserializedObject.vout)
            {

                // checking for all known P2FK bitcoin testnet microtransaction values
                if (v_out.value == "5.46E-06" || v_out.value == "5.48E-06" || v_out.value == "5.48E-05")
                {



                    byte[] results = Array.Empty<byte>();
                    strPublicAddress = v_out.scriptPubKey.addresses[0];

                    //retreiving payload data from each address
                    Base58.DecodeWithCheckSum(strPublicAddress, out results);

                    //append to a byte[] of all P2FK data
                    transactionBytes = AddBytes(transactionBytes, RemoveFirstByte(results));

                }
            }

            // newRoot.RawBytes = transactionBytes;
            //ASCII Header Encoding is working still troubleshooting why some signed objects are not being recogrnized as signed
            transactionASCII = Encoding.ASCII.GetString(transactionBytes);


            // Perform the loop until no additional numbers are found and the regular expression fails to match 
            while (regex.IsMatch(transactionASCII))
            {
                Match match = regex.Match(transactionASCII);
                int mValue = Int32.Parse(match.Value.ToString().Remove(0, 1));




                //invalid if a special character is not found before the number
                if (transactionASCII.IndexOfAny(specialChars) == match.Index)
                {

                    sigEndByte += mValue + match.Index + match.Length + 1;

                    string fileName = transactionASCII.Substring(0, transactionASCII.IndexOfAny(specialChars));


                    if (fileName == "SIG")
                    {

                        sigStartByte = sigEndByte;

                        signature = transactionASCII.Substring(match.Index + match.Length + 1, mValue);
                    }


                    byte[] filebytes = transactionBytes.Skip(match.Index + match.Length + 1 + (transactionBytes.Count() - transactionASCII.Length)).Take(mValue).ToArray();

                    if (fileName != "")
                    {

                        files.Add(fileName, filebytes);

                    }
                    else { MessageList.Add(Encoding.UTF8.GetString(filebytes)); }



                    transactionASCII = transactionASCII.Remove(0, (mValue + match.Index + match.Length + 1));

                }
                else
                {


                    break;
                }

            }


            for (int i = 0; i < transactionASCII.Length; i += 20)
            {
                keywords.Add(Base58.EncodeWithCheckSum(AddBytes(new byte[] { byte.Parse("111") }, transactionBytes.Skip(i + (transactionBytes.Count() - transactionASCII.Length)).Take(20).ToArray())), Encoding.ASCII.GetString(transactionBytes.Skip(i + (transactionBytes.Count() - transactionASCII.Length)).Take(20).ToArray()));

            }

            if (sigStartByte > 0)
            {
                //verify a hash of the byte data was signed by the last address found on the transaction 


                newRoot.Hash = BitConverter.ToString(mySHA256.ComputeHash(transactionBytes.Skip(sigStartByte).Take(sigEndByte - sigStartByte).ToArray())).Replace("-", String.Empty);
                var result = rpcClient.SendCommand("verifymessage", strPublicAddress, signature, newRoot.Hash);
                newRoot.Signed = Convert.ToBoolean(result.Result);
                keywords.Remove(strPublicAddress);
            }
            else { strPublicAddress = ""; }

            newRoot.TransactionId = deserializedObject.txid;
            newRoot.Signature = signature;
            newRoot.SignedBy = strPublicAddress;
            newRoot.BlockDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(deserializedObject.blocktime)).DateTime;
            newRoot.File = files;
            newRoot.Message = MessageList.ToArray();
            newRoot.Keyword = keywords;
            newRoot.TotalByteSize = totalByteSize;
            newRoot.Confirmations = deserializedObject.confirmations;
            newRoot.BuildDate = DateTime.UtcNow;
            return newRoot;
        }


        private static byte[] AddBytes(byte[] existingArray, byte[] newArray)
        {
            // Create a new array with a capacity large enough to hold both arrays
            byte[] combinedArray = new byte[existingArray.Length + newArray.Length];

            // Copy the elements from the existing array into the new array
            Array.Copy(existingArray, combinedArray, existingArray.Length);

            // Copy the elements from the new array into the new array
            Array.Copy(newArray, 0, combinedArray, existingArray.Length, newArray.Length);

            return combinedArray;
        }

        private static byte[] RemoveFirstByte(byte[] array)
        {

            // Create a new array with a capacity one less than the original array
            byte[] newArray = new byte[array.Length - 1];

            // Copy the elements from the original array into the new array, starting at the second element
            Array.Copy(array, 1, newArray, 0, newArray.Length);

            return newArray;
        }


    }
}




