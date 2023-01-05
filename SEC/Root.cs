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
        public static Root[] GetRootByAddress(string address, string username, string password, string url, int skip = 0, int qty = 100)
        {

            //used as P2FK Delimiters 
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            Regex regexSpecialChars = new Regex(@"([\\/:*?""<>|])\d+");
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            byte bitcoinTestnetVersionByte = byte.Parse("111");

            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));

            // calls the bitcoin testnet wallet's searchrawtransaction command and returns it as a dynamic deserialized object.
            // implements the address search feature found here -->  https://github.com/btcdrak/bitcoin/tree/addrindex-0.14 
            dynamic deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("searchrawtransactions", address, 1, skip, qty).ResultString);

            //create a list to hold the Root objects
            List<Root> RootList = new List<Root>();

            //itterating through JSON search results
            foreach (dynamic transID in deserializedObject)
            {
                //defining items to include in the returned objects

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
                Root LedgerRoot = new Root();
                // we are spinning through all the out addresses within each bitcoin transaction
                // we are base58 decoding each address to obtain a 20 byte payload that is appended to a byte[]
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
                int transactionBytesSize = transactionBytes.Count();

                // Perform the loop until no additional numbers are found and the regular expression fails to match 
                while (regexSpecialChars.IsMatch(transactionASCII))
                {
                    Match match = regexSpecialChars.Match(transactionASCII);
                    int packetSize = Int32.Parse(match.Value.ToString().Remove(0, 1));
                    int headerSize = match.Index + match.Length + 1;

                    //invalid if a special character is not found before a number
                    if (transactionASCII.IndexOfAny(specialChars) == match.Index)
                    {
                        //increment a counter used to determind total size of signed content
                        sigEndByte += packetSize + headerSize;

                        string fileName = transactionASCII.Substring(0, transactionASCII.IndexOfAny(specialChars));
                        byte[] fileBytes = transactionBytes.Skip(headerSize + (transactionBytesSize - transactionASCII.Length)).Take(packetSize).ToArray();

                        if (fileName != "")
                        {

                            while (regexTransactionId.IsMatch(fileName))
                            {
                                string strTransactionID = transID.txid;
                                LedgerRoot = GetRootByByteArray(GetLedgerBytes(fileName + Environment.NewLine + Encoding.ASCII.GetString(fileBytes).Replace(fileName, ""), username, password, url), username, password, url, strPublicAddress, strTransactionID);
                                fileName = LedgerRoot.File.Keys.First();
                                fileBytes = LedgerRoot.File.Values.First();
                                newRoot = LedgerRoot;
                                newRoot.TransactionId = transID.txid;
                                newRoot.Confirmations = transID.confirmations;
                                newRoot.BlockDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(transID.blocktime)).DateTime;
                            }

                            if (fileName == "SIG")
                            {
                                sigStartByte = sigEndByte;
                                signature = transactionASCII.Substring(headerSize, packetSize);
                            }



                            files.Add(fileName, fileBytes);

                            try
                            {
                                string diskpath = "root\\" + transID.txid + "\\";
                                if (!Directory.Exists(diskpath))
                                {
                                    Directory.CreateDirectory(diskpath);
                                }

                                using (FileStream fs = new FileStream(diskpath + fileName, FileMode.Create))
                                {
                                    fs.Write(fileBytes, 0, fileBytes.Length);
                                }
                            }
                            catch (Exception ex) { }

                        }
                        else
                        {
                            MessageList.Add(Encoding.UTF8.GetString(fileBytes));

                            try
                            {
                                string diskpath = "root\\" + transID.txid + "\\";
                                if (!Directory.Exists(diskpath))
                                {
                                    Directory.CreateDirectory(diskpath);
                                }

                                using (FileStream fs = new FileStream(diskpath + "MSG", FileMode.Create))
                                {
                                    fs.Write(fileBytes, 0, fileBytes.Length);
                                }
                            }
                            catch (Exception ex) { }

                        }

                        transactionASCII = transactionASCII.Remove(0, (packetSize + headerSize));

                    }
                    else
                    {
                        break;
                    }

                }

                //removing a null character prevents some keyewords from being out of alignment.....still researching why
                int index = transactionASCII.IndexOf('\0');

                if (index >= 0)
                {
                    transactionASCII = transactionASCII.Substring(index + 1);
                }

                for (int i = 0; i < transactionASCII.Length; i += 20)
                {
                    try
                    {
                        keywords.Add(Base58.EncodeWithCheckSum(AddBytes(new byte[] { byte.Parse("111") }, transactionBytes.Skip(i + (transactionBytesSize - transactionASCII.Length)).Take(20).ToArray())), Encoding.ASCII.GetString(transactionBytes.Skip(i + (transactionBytesSize - transactionASCII.Length)).Take(20).ToArray()));

                    }
                    catch (Exception ex) { }
                }

                if (sigStartByte > 0 && LedgerRoot.Signature == null)
                {
                    //create a SHA256 hash of the transaction byte data.  pass it along with a wallet address to a bitcoin testnet wallet's verifymessage command.
                    //true: the transaction file data has been signed using the address provided
                    //false: the transaction file data has not been signed using the address provided
                    System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                    newRoot.Hash = BitConverter.ToString(mySHA256.ComputeHash(transactionBytes.Skip(sigStartByte).Take(sigEndByte - sigStartByte).ToArray())).Replace("-", String.Empty);
                    var result = rpcClient.SendCommand("verifymessage", strPublicAddress, signature, newRoot.Hash);
                    newRoot.Signed = Convert.ToBoolean(result.Result);

                }
                else { if (sigStartByte == 0) { strPublicAddress = ""; } }

                if (LedgerRoot.TransactionId != null)
                {
                    LedgerRoot.Keyword = keywords;
                    LedgerRoot.Confirmations = transID.confirmations;
                    LedgerRoot.BuildDate = DateTime.UtcNow;
                    if (LedgerRoot.File.Count() + LedgerRoot.Message.Count() > 0) { RootList.Add(LedgerRoot); }

                }
                else
                {


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
                    if (newRoot.File.Count() + newRoot.Message.Count() > 0) { RootList.Add(newRoot); }
                }
            }
            return RootList.ToArray();
        }
        public static Root GetRootByTransactionId(string transactionid, string username, string password, string url)
        {

            //used as P2FK Delimiters 
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            Regex regexSpecialChars = new Regex(@"([\\/:*?""<>|])\d+");
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");

            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));

            dynamic deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("getrawtransaction", transactionid, 1).ResultString);

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
            Root LedgerRoot = new Root();
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
            int transactionBytesSize = transactionBytes.Count();

            // Perform the loop until no additional numbers are found and the regular expression fails to match 
            while (regexSpecialChars.IsMatch(transactionASCII))
            {
                Match match = regexSpecialChars.Match(transactionASCII);
                int packetSize = Int32.Parse(match.Value.ToString().Remove(0, 1));
                int headerSize = match.Index + match.Length + 1;




                //invalid if a special character is not found before a number
                if (transactionASCII.IndexOfAny(specialChars) == match.Index)
                {

                    sigEndByte += packetSize + headerSize;

                    string fileName = transactionASCII.Substring(0, transactionASCII.IndexOfAny(specialChars));
                    byte[] fileBytes = transactionBytes.Skip(headerSize + (transactionBytesSize - transactionASCII.Length)).Take(packetSize).ToArray();

                    if (fileName != "")
                    {


                        while (regexTransactionId.IsMatch(fileName))
                        {
                            LedgerRoot = GetRootByByteArray(GetLedgerBytes(fileName + Environment.NewLine + Encoding.ASCII.GetString(fileBytes).Replace(fileName, ""), username, password, url), username, password, url, strPublicAddress, transactionid);
                            fileName = LedgerRoot.File.Keys.First();
                            fileBytes = LedgerRoot.File.Values.First();
                            newRoot = LedgerRoot;
                            newRoot.TransactionId = transactionid;
                            newRoot.Confirmations = deserializedObject.confirmations;
                            newRoot.BlockDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(deserializedObject.blocktime)).DateTime;
                        }



                        if (fileName == "SIG")
                        {
                            sigStartByte = sigEndByte;
                            signature = transactionASCII.Substring(headerSize, packetSize);
                        }

                        files.Add(fileName, fileBytes);
                        try
                        {
                            string diskpath = "root\\" + transactionid + "\\";
                            if (!Directory.Exists(diskpath))
                            {
                                Directory.CreateDirectory(diskpath);
                            }

                            using (FileStream fs = new FileStream(diskpath + fileName, FileMode.Create))
                            {
                                fs.Write(fileBytes, 0, fileBytes.Length);
                            }
                        }
                        catch (Exception ex) { }
                    }
                    else
                    {

                        MessageList.Add(Encoding.UTF8.GetString(fileBytes));
                        try
                        {
                            string diskpath = "root\\" + transactionid + "\\";
                            if (!Directory.Exists(diskpath))
                            {
                                Directory.CreateDirectory(diskpath);
                            }

                            using (FileStream fs = new FileStream(diskpath + "MSG", FileMode.Create))
                            {
                                fs.Write(fileBytes, 0, fileBytes.Length);
                            }
                        }
                        catch (Exception ex) { }
                    }

                    transactionASCII = transactionASCII.Remove(0, (packetSize + headerSize));

                }
                else
                {


                    break;
                }

            }

            int index = transactionASCII.IndexOf('\0');

            if (index >= 0)
            {
                transactionASCII = transactionASCII.Substring(index + 1);
            }

            for (int i = 0; i < transactionASCII.Length; i += 20)
            {
                try
                {

                    keywords.Add(Base58.EncodeWithCheckSum(AddBytes(new byte[] { byte.Parse("111") }, transactionBytes.Skip(i + (transactionBytesSize - transactionASCII.Length)).Take(20).ToArray())), Encoding.ASCII.GetString(transactionBytes.Skip(i + (transactionBytesSize - transactionASCII.Length)).Take(20).ToArray()));
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
                LedgerRoot.Keyword = keywords;
                LedgerRoot.Confirmations = deserializedObject.confirmations;
                LedgerRoot.BuildDate = DateTime.UtcNow;
                return LedgerRoot;
            }


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
        public static Root GetRootByByteArray(byte[] input, string username, string password, string url, string publicAddress, string transactionid)
        {
            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));
            byte[] transactionBytes = input;
            int transactionBytesSize = transactionBytes.Count();
            string transactionASCII = Encoding.ASCII.GetString(transactionBytes);
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            Regex regexSpecialChars = new Regex(@"([\\/:*?""<>|])\d+");
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            byte bitcoinTestnetVersionByte = byte.Parse("111");
            Root newRoot = new Root();
            System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
            Dictionary<string, string> keywords = new Dictionary<string, string>();
            List<String> MessageList = new List<String>();
            string strPublicAddress = publicAddress;
            string signature = "";
            int sigStartByte = 0;
            int sigEndByte = 0;
            int totalByteSize = input.Count();


            // Perform the loop until no additional numbers are found and the regular expression fails to match 
            while (regexSpecialChars.IsMatch(transactionASCII))
            {
                Match match = regexSpecialChars.Match(transactionASCII);
                int packetSize = Int32.Parse(match.Value.ToString().Remove(0, 1));
                int headerSize = match.Index + match.Length + 1;

                //invalid if a special character is not found before a number
                if (transactionASCII.IndexOfAny(specialChars) == match.Index)
                {

                    sigEndByte += packetSize + headerSize;

                    string fileName = transactionASCII.Substring(0, transactionASCII.IndexOfAny(specialChars));
                    byte[] fileBytes = transactionBytes.Skip(headerSize + (transactionBytesSize - transactionASCII.Length)).Take(packetSize).ToArray();

                    if (fileName != "")
                    {

                        if (fileName == "SIG")
                        {
                            sigStartByte = sigEndByte;
                            signature = transactionASCII.Substring(headerSize, packetSize);
                        }

                        files.Add(fileName, fileBytes);
                        try
                        {
                            string diskpath = "root\\" + transactionid + "\\";
                            if (!Directory.Exists(diskpath))
                            {
                                Directory.CreateDirectory(diskpath);
                            }

                            using (FileStream fs = new FileStream(diskpath + fileName, FileMode.Create))
                            {
                                fs.Write(fileBytes, 0, fileBytes.Length);
                            }
                        }
                        catch (Exception ex) { }
                    }
                    else
                    {
                        MessageList.Add(Encoding.UTF8.GetString(fileBytes));
                        try
                        {
                            string diskpath = "root\\" + transactionid + "\\";
                            if (!Directory.Exists(diskpath))
                            {
                                Directory.CreateDirectory(diskpath);
                            }

                            using (FileStream fs = new FileStream(diskpath + "MSG", FileMode.Create))
                            {
                                fs.Write(fileBytes, 0, fileBytes.Length);
                            }
                        }
                        catch (Exception ex) { }
                    }


                    //remove what has been processed from the string
                    transactionASCII = transactionASCII.Remove(0, (packetSize + headerSize));

                }
                else
                {


                    break;
                }

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


            newRoot.Signature = signature;
            newRoot.SignedBy = strPublicAddress;
            newRoot.File = files;
            newRoot.Message = MessageList.ToArray();
            newRoot.Keyword = keywords;
            newRoot.TotalByteSize = totalByteSize;
            newRoot.BuildDate = DateTime.UtcNow;
            return newRoot;


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


            return Base58.EncodeWithCheckSum(AddBytes(new byte[] { byte.Parse(versionbyte) }, System.Text.Encoding.ASCII.GetBytes(keyword)));


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
                byte[] transactionBytesBatch = Array.Empty<byte>();
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
                        transactionBytesBatch = AddBytes(transactionBytesBatch, RemoveFirstByte(results));

                    }
                }
                transactionBytes = AddBytes(transactionBytes, transactionBytesBatch);



            }
            return transactionBytes;

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




