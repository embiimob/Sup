using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


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
        public static Root GetRootByTransactionId(string transactionid, string username, string password, string url, string versionbyte = "111", byte[] rootBytes = null, string publicaddress = "")
        {

            //used as P2FK Delimiters 
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            Regex regexSpecialChars = new Regex(@"([\\/:*?""<>|])\d+");
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            dynamic deserializedObject = null;
            byte VersionByte = byte.Parse(versionbyte);
            //defining items to include in the returned object
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
            Dictionary<string, string> keywords = new Dictionary<string, string>();
            List<String> MessageList = new List<String>();
            byte[] transactionBytes = Array.Empty<byte>();
            string transactionASCII;
            string strPublicAddress = publicaddress;
            string signature = "";
            DateTime blockdate = new DateTime();
            int confirmations = 0;


            int sigStartByte = 0;
            int sigEndByte = 0;
            int totalByteSize = 0;
            //create a new root object
            Root newRoot = new Root();
            Root LedgerRoot = new Root();
            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));
            if (rootBytes == null)
            {


                try
                {
                    deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("getrawtransaction", transactionid, 1).ResultString);

                }
                catch (Exception ex)
                {
                    newRoot.Message = new string[] { ex.Message };
                    newRoot.BuildDate = DateTime.UtcNow;
                    newRoot.File = files;
                    newRoot.Keyword = keywords;
                    newRoot.TransactionId = transactionid;
                    return newRoot;
                }

                totalByteSize = deserializedObject.size;
                blockdate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(deserializedObject.blocktime)).DateTime;
                confirmations = deserializedObject.confirmations;
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
                        transactionBytes = transactionBytes.Concat(results.Skip(1)).ToArray();

                        //transactionBytes = AddBytes(transactionBytes, results.Skip(1).ToArray());

                    }
                }
            }
            else { transactionBytes = rootBytes; totalByteSize = transactionBytes.Count(); }
            // newRoot.RawBytes = transactionBytes;
            //ASCII Header Encoding is working still troubleshooting why some signed objects are not being recogrnized as signed

            int transactionBytesSize = transactionBytes.Count();
            transactionASCII = Encoding.ASCII.GetString(transactionBytes);

            // Perform the loop until no additional numbers are found and the regular expression fails to match 
            while (regexSpecialChars.IsMatch(transactionASCII))
            {
                Match match = regexSpecialChars.Match(transactionASCII);
                int packetSize = Int32.Parse(match.Value.ToString().Remove(0, 1));
                int headerSize = match.Index + match.Length + 1;


                //invalid if a special character is not found before a number
                if (transactionASCII.IndexOfAny(specialChars) != match.Index) { break; }

                sigEndByte += packetSize + headerSize;

                string fileName = transactionASCII.Substring(0, match.Index);
                byte[] fileBytes = transactionBytes.Skip(headerSize + (transactionBytesSize - transactionASCII.Length)).Take(packetSize).ToArray();

                bool isValid = true;
                string diskpath = "root\\" + transactionid + "\\";
                if (!Directory.Exists(diskpath))
                {
                    Directory.CreateDirectory(diskpath);
                }
                if (fileName != "")
                {

                    try
                    {
                        // This will throw an exception if the file name is not valid
                        System.IO.File.Create(diskpath + fileName).Dispose();
                    }
                    catch (Exception ex) { isValid = false; }
                }
                else { isValid = false; }

                if (isValid)
                {

                    while (regexTransactionId.IsMatch(fileName))
                    {
                        using (FileStream fs = new FileStream(diskpath + fileName, FileMode.Create))
                        {
                            fs.Write(fileBytes, 0, fileBytes.Length);
                        }

                        LedgerRoot = GetRootByTransactionId(transactionid, username, password, url, versionbyte, GetLedgerBytes(fileName + Environment.NewLine + Encoding.ASCII.GetString(fileBytes).Replace(fileName, ""), username, password, url), strPublicAddress);
                        fileName = LedgerRoot.File.Keys.First();
                        fileBytes = LedgerRoot.File.Values.First();
                        newRoot = LedgerRoot;
                        newRoot.TransactionId = transactionid;
                        newRoot.Confirmations = confirmations;
                        newRoot.BlockDate = blockdate;
                    }

                    if (fileName == "SIG")
                    {
                        sigStartByte = sigEndByte;
                        signature = transactionASCII.Substring(headerSize, packetSize);
                    }


                    using (FileStream fs = new FileStream(diskpath + fileName, FileMode.Create))
                    {
                        fs.Write(fileBytes, 0, fileBytes.Length);
                    }

                    files.AddOrReplace(fileName, fileBytes);
                }
                else
                {
                    if (fileName == "")
                    {
                        MessageList.Add(Encoding.UTF8.GetString(fileBytes));


                        using (FileStream fs = new FileStream(diskpath + "MSG", FileMode.Create))
                        {
                            fs.Write(fileBytes, 0, fileBytes.Length);
                        }
                    }
                    else { break; }

                }

                try
                {
                    transactionASCII = transactionASCII.Remove(0, (packetSize + headerSize));
                }
                catch (Exception ex) { break; }
            }


            if (files.Count() + MessageList.Count() == 0) { return null; }

            //removing null characters
            while (transactionASCII.IndexOf('\0') >= 0)
            {
                transactionASCII = transactionASCII.Substring(transactionASCII.IndexOf('\0') + 1);
            }

            for (int i = 0; i < transactionASCII.Length; i += 20)
            {
                try
                {

                    keywords.Add(Base58.EncodeWithCheckSum(new byte[] { VersionByte }.Concat(transactionBytes.Skip(i + (transactionBytesSize - transactionASCII.Length)).Take(20)).ToArray()), Encoding.ASCII.GetString(transactionBytes.Skip(i + (transactionBytesSize - transactionASCII.Length)).Take(20).ToArray()));

                }
                catch (Exception ex) { }
            }


            if (sigStartByte > 0 && LedgerRoot.Signature == null)
            {

                //used in signature verification
                System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                newRoot.Hash = BitConverter.ToString(mySHA256.ComputeHash(transactionBytes.Skip(sigStartByte).Take(sigEndByte - sigStartByte).ToArray())).Replace("-", String.Empty);
                var result = rpcClient.SendCommand("verifymessage", strPublicAddress, signature, newRoot.Hash);
                newRoot.Signed = Convert.ToBoolean(result.Result);

            }
            else { if (sigStartByte == 0) { strPublicAddress = ""; } }

            if (LedgerRoot.TransactionId != null)
            {
                LedgerRoot.Message = MessageList.ToArray();
                LedgerRoot.Keyword = keywords;
                LedgerRoot.Confirmations = confirmations;
                LedgerRoot.BuildDate = DateTime.UtcNow;
                return LedgerRoot;
            }


            newRoot.TransactionId = transactionid;
            newRoot.Signature = signature;
            newRoot.SignedBy = strPublicAddress;
            newRoot.BlockDate = blockdate;
            newRoot.Confirmations = confirmations;
            newRoot.File = files;
            newRoot.Message = MessageList.ToArray();
            newRoot.Keyword = keywords;
            newRoot.TotalByteSize = totalByteSize;
            newRoot.BuildDate = DateTime.UtcNow;
            return newRoot;
        }
        public static Root[] GetRootByAddress(string address, string username, string password, string url, int skip = 0, int qty = 500, string versionbyte = "111", byte[] rootBytes = null, string publicaddress = "")
        {
            List<Root> RootList = new List<Root>();

            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));
            Dictionary<string, Root> synchronousData = new Dictionary<string, Root>();
            dynamic deserializedObject = null;
            try
            {
                deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("searchrawtransactions", address, 1, skip, qty).ResultString);
            }
            catch (Exception ex)
            {
                Root newRoot = new Root();
                newRoot.Message = new string[] { ex.Message };
                newRoot.BuildDate = DateTime.UtcNow;
                newRoot.File = new Dictionary<string, byte[]> { };
                newRoot.Keyword = new Dictionary<string, string> { };
                newRoot.TransactionId = address;
                RootList.Add(newRoot);
                return RootList.ToArray();
            }

            CountdownEvent countdownEvent = new CountdownEvent(deserializedObject.Count);
            //itterating through JSON search results
            foreach (dynamic transID in deserializedObject)
            {
                // Launch a separate thread to retrieve the transaction bytes for this match
                Thread thread = new Thread(() =>
                {
                    string transactionid = transID.txid;
                    Root root = Root.GetRootByTransactionId(transactionid, username, password, url);

                    if (root != null)
                    {
                        
                        try
                        {
                            synchronousData.Add(transactionid, root);
                        }
                        catch (Exception ex) { }
                    }
                    countdownEvent.Signal();


                });

                thread.Start();

            }
            countdownEvent.Wait();


            foreach (dynamic transID in deserializedObject)
            {
                string transactionid = transID.txid;
                try
                {
                    RootList.Add(synchronousData[transactionid]);
                }
                catch (Exception ex) { }
            }

            return RootList.ToArray();
        }
        public static string GetPublicAddressByKeyword(string keyword, string versionbyte = "111")
        {


            // Cut the string at 20 characters
            if (keyword.Length > 20)
            {
                keyword = keyword.Substring(0, 20);
            }
            // Right pad the string with '#' characters
            keyword = keyword.PadRight(20, '#');


            return Base58.EncodeWithCheckSum(new byte[] { byte.Parse(versionbyte) }.Concat(System.Text.Encoding.ASCII.GetBytes(keyword)).ToArray());

        }
        public static string GetKeywordByPublicAddress(string public_address)
        {


            byte[] payloadBytes = new byte[0];

            Base58.DecodeWithCheckSum(public_address, out payloadBytes);

            return Encoding.ASCII.GetString(payloadBytes).Replace("#", "").Substring(1);


        }
        private static byte[] GetLedgerBytes(string ledger, string username, string password, string url)
        {
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            byte[] transactionBytes = Array.Empty<byte>();
            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));


            var matches = regexTransactionId.Matches(ledger);
            foreach (Match match in matches)
            {
                byte[] transactionBytesBatch = new byte[0];
                dynamic deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("getrawtransaction", match.Value, 1).ResultString);

                foreach (dynamic v_out in deserializedObject.vout)
                {

                    // checking for all known P2FK bitcoin testnet microtransaction values
                    if (v_out.value == "5.46E-06" || v_out.value == "5.48E-06" || v_out.value == "5.48E-05")
                    {

                        byte[] results = Array.Empty<byte>();
                        string strPublicAddress = v_out.scriptPubKey.addresses[0];
                        //retreiving payload data from each address
                        Base58.DecodeWithCheckSum(strPublicAddress, out results);

                        //append to a byte[] of all P2FK data

                        transactionBytesBatch = transactionBytesBatch.Concat(results.Skip(1)).ToArray();
                    }
                }

                transactionBytes = transactionBytes.Concat(transactionBytesBatch).ToArray();

            }
            return transactionBytes;

        }

    }
}




