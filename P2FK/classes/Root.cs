using AngleSharp.Common;
using LevelDB;
using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace SUP.P2FK
{
    public class Root
    {
        public int Id { get; set; }
        public string[] Message { get; set; }
        public Dictionary<string, BigInteger> File { get; set; }
        public Dictionary<string, string> Keyword { get; set; }
        public Dictionary<string, string> Output { get; set; }

        public string Hash { get; set; }
        public string SignedBy { get; set; }
        public string Signature { get; set; }
        public bool Signed { get; set; }
        public string TransactionId { get; set; }
        public DateTime BlockDate { get; set; }
        public int TotalByteSize { get; set; }
        public int Confirmations { get; set; }
        public DateTime BuildDate { get; set; }
        public bool Cached { get; set; }



        //ensures levelDB is thread safely
        private readonly static object SupLocker = new object();

        public static Root GetRootByTransactionId(string transactionid, string username, string password, string url, string versionbyte = "111", byte[] rootbytes = null, string signatureaddress = null)
        {
            Root P2FKRoot = new Root();
            string diskpath = "root\\" + transactionid + "\\";
            string P2FKJSONString = null;
            bool isMuted = false;

            if (rootbytes == null)
            {
                try
                {

                    P2FKJSONString = System.IO.File.ReadAllText(diskpath + "ROOT.json");
                    P2FKRoot = JsonConvert.DeserializeObject<Root>(P2FKJSONString);

                    //refresh LevelDB Sup Messaging.
                    return P2FKRoot;

                }
                catch { }

            }

            //P2FK Object Cache does not exist
            //build P2FK Object from Blockchain

            //used as P2FK Delimiters
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            Regex regexSpecialChars = new Regex(@"([\\/:*?""<>|])\d+");
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            Dictionary<string, BigInteger> files = new Dictionary<string, BigInteger>();
            Dictionary<string, string> keywords = new Dictionary<string, string>();
            DateTime blockdate = new DateTime();
            int confirmations = -1;
            bool isledger = false;
            dynamic deserializedObject;
            byte VersionByte = byte.Parse(versionbyte);
            byte[] VersionByteArray = new byte[] { VersionByte };
            byte[] transactionBytes = Array.Empty<byte>();
            //defining items to include in the returned object
            List<String> MessageList = new List<String>();
            string transactionASCII;
            string P2FKSignatureAddress = signatureaddress;
            string signature = "";
            byte[] KeywordArray = new byte[21];
            int sigStartByte = 0;
            int sigEndByte = 0;
            int totalByteSize;
            //create a new root object


            if (rootbytes == null)
            {
                NetworkCredential credentials = new NetworkCredential(username, password);
                RPCClient rpcClient = new RPCClient(credentials, new Uri(url), Network.Main);

                try
                {
                    deserializedObject = JsonConvert.DeserializeObject(
                        rpcClient.SendCommand("getrawtransaction", transactionid, 1).ResultString
                    );
                }
                catch (Exception ex)
                {
                    P2FKRoot.Message = new string[] { ex.Message };
                    P2FKRoot.BuildDate = DateTime.UtcNow;
                    P2FKRoot.File = files;
                    P2FKRoot.Keyword = keywords;
                    P2FKRoot.TransactionId = transactionid;
                    return P2FKRoot;
                }

                totalByteSize = deserializedObject.size;
                try { confirmations = deserializedObject.confirmations; } catch { confirmations = 0; }
                blockdate =
                 DateTimeOffset.FromUnixTimeSeconds(
                     Convert.ToInt32(deserializedObject.blocktime)
                 ).DateTime;


                P2FKRoot.Output = new Dictionary<string, string>();
                // we are spinning through all the out addresses within each bitcoin transaction
                // we are base58 decdoing each address to obtain a 20 byte payload that is appended to a byte[]
                foreach (dynamic v_out in deserializedObject.vout)
                {

                    string address = v_out.scriptPubKey.addresses[0].ToString();
                    string value = v_out.value.ToString();
                    P2FKRoot.Output.Add(address, value);

                    // checking for all known P2FK bitcoin testnet microtransaction values
                    if (
                       value == "5.46E-06"
                       || value == "5.48E-06"
                       || value == "5.48E-05"
                       || value == "5.5E-05"
                       || value == "5.5E-06"
                       || value == "1E-05"
                       || value == "1E-08"
                       || value == "1.3016426"
                       || value == "0.01"
                       || value == "1"
                    )
                    {
                        byte[] results = Array.Empty<byte>();
                        try
                        {
                            P2FKSignatureAddress = address;
                        }
                        catch { return P2FKRoot; }



                        //retreiving payload data from each address
                        Base58.DecodeWithCheckSum(P2FKSignatureAddress, out results);


                        int length1 = transactionBytes.Length;
                        int length2 = results.Length - 1;

                        byte[] result = new byte[length1 + length2];

                        System.Buffer.BlockCopy(transactionBytes, 0, result, 0, length1);
                        System.Buffer.BlockCopy(results, 1, result, length1, length2);

                        transactionBytes = result;

                    }
                }
            }
            else
            {
                transactionBytes = rootbytes;
                totalByteSize = transactionBytes.Count();

            }


            int transactionBytesSize = transactionBytes.Count();
            transactionASCII = Encoding.ASCII.GetString(transactionBytes);
           

            if (rootbytes == null && P2FKSignatureAddress == null) { return P2FKRoot; }

            // Perform the loop until no additional numbers are found and the regular expression fails to match
            while (regexSpecialChars.IsMatch(transactionASCII))
            {


                Match match = regexSpecialChars.Match(transactionASCII);
                int packetSize;
                int headerSize;
                try
                {
                    packetSize = Int32.Parse(match.Value.ToString().Remove(0, 1));
                    headerSize = match.Index + match.Length + 1;
                }
                catch { break; }

                //invalid if a special character is not found before a number
                if (transactionASCII.IndexOfAny(specialChars) != match.Index)
                {
                    break;
                }



                string fileName = transactionASCII.Substring(0, match.Index);
                byte[] fileBytes = transactionBytes
                    .Skip(headerSize + (transactionBytesSize - transactionASCII.Length))
                    .Take(packetSize)
                    .ToArray();

                bool isValid = true;

                if (!Directory.Exists(diskpath))
                {
                    Directory.CreateDirectory(diskpath);
                }
                if (fileName.Length > 1 & fileName.Replace("#","") != "BTC" & fileName.Replace("#", "") != "MZC" & fileName.Replace("#", "") != "LTC" & fileName.Replace("#", "") != "DOG" & fileName.Replace("#", "") != "IPFS")
                {
                    try
                    {
                        // This will throw an exception if the file name is not valid
                        System.IO.File.Create(diskpath + fileName).Dispose();
                        sigEndByte += packetSize + headerSize;
                    }
                    catch (Exception)
                    {
                        isValid = false;

                    }
                }
                else
                {
                    isValid = false;


                }

                if (isValid)
                {

                    lock (SupLocker)
                    {
                        //Process Ledger files until reaching Root
                        while (regexTransactionId.IsMatch(fileName))
                        {

                            // supports older objects where the file ledger name was repeated inside filecontents
                            byte[] removeStringBytes = Encoding.ASCII.GetBytes(fileName);
                            if (fileBytes.Take(removeStringBytes.Length).SequenceEqual(removeStringBytes))
                            {
                                byte[] truncatedBytes = new byte[fileBytes.Length - removeStringBytes.Length];
                                Buffer.BlockCopy(fileBytes, removeStringBytes.Length, truncatedBytes, 0, truncatedBytes.Length);
                                fileBytes = truncatedBytes;
                            }

                            isledger = true;

                            P2FKRoot = GetRootByTransactionId(
                            transactionid,
                            username,
                            password,
                            url,
                            versionbyte,
                            GetRootBytesByLedger(
                                fileName
                                    + Environment.NewLine
                                    + Encoding.ASCII.GetString(fileBytes),
                                username,
                                password,
                                url
                            ), P2FKSignatureAddress
                        );

                            if (P2FKRoot.File.Count > 0)
                            {
                                fileName = P2FKRoot.File.Keys.First();
                                fileBytes = System.IO.File.ReadAllBytes(@"root\" + P2FKRoot.TransactionId + @"\" + fileName); ;
                            }
                            else { fileName = ""; fileBytes = Array.Empty<byte>(); }
                            P2FKRoot.TotalByteSize += totalByteSize;
                            P2FKRoot.Confirmations = confirmations;
                            P2FKRoot.BlockDate = blockdate;

                        }

                    }

                    if (isledger)
                    {
                        var rootLedger = JsonConvert.SerializeObject(P2FKRoot);

                        System.IO.File.WriteAllText(@"root\" + P2FKRoot.TransactionId + @"\" + "ROOT.json", rootLedger);
                        return P2FKRoot;
                    }

                    if (fileName == "SIG")
                    {
                        sigStartByte = sigEndByte;
                        try
                        {
                            signature = transactionASCII.Substring(headerSize, packetSize);
                        }
                        catch
                        {

                            return null;
                        }
                    }

                    using (FileStream fs = new FileStream(diskpath + fileName, FileMode.Create))
                    {
                        fs.Write(fileBytes, 0, fileBytes.Length);
                    }

                    files.AddOrReplace(fileName, fileBytes.Length);
                }
                else
                {
                    if (fileName == "" && fileBytes.Length > 4)
                    {
                        sigEndByte += packetSize + headerSize;
                        MessageList.Add(Encoding.UTF8.GetString(fileBytes));
                        using (FileStream fs = new FileStream(diskpath + "MSG", FileMode.Create))
                        {
                            fs.Write(fileBytes, 0, fileBytes.Length);

                        }
                    }
                    else
                    {

                        break;
                    }
                }

                try
                {   //Removed processed header and payload bytes
                    transactionASCII = transactionASCII.Remove(0, (packetSize + headerSize));
                }
                catch (Exception)
                {
                    transactionASCII = "";
                    break;
                }
            }
            //if no P2FK files or messages were found return Invalid object
            if (files.Count() + MessageList.Count() == 0)
            {
                return P2FKRoot;
            }


            int charactersPerDivision = 20;

            if (transactionASCII.Length > charactersPerDivision)
            {
                int remainder = transactionASCII.Length % charactersPerDivision;
                transactionASCII = transactionASCII.Substring(remainder);
            }


            for (int i = 0; i < transactionASCII.Length; i += 20)
            {
                try
                {
                    System.Buffer.BlockCopy(VersionByteArray, 0, KeywordArray, 0, 1);
                    System.Buffer.BlockCopy(
                        transactionBytes,
                        i + (transactionBytesSize - transactionASCII.Length),
                        KeywordArray,
                        1,
                        20
                    );
                    keywords.Add(
                        Base58.EncodeWithCheckSum(KeywordArray),
                        Encoding.ASCII.GetString(KeywordArray)
                    );
                }
                catch { }

            }


            if (sigStartByte > 0)
            {

                System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                P2FKRoot.Hash = BitConverter
                    .ToString(
                        mySHA256.ComputeHash(
                            transactionBytes
                                .Skip(sigStartByte)
                                .Take(sigEndByte - sigStartByte)
                                .ToArray()
                        )
                    )
                    .Replace("-", String.Empty);

                NetworkCredential credentials = new NetworkCredential(username, password);
                RPCClient rpcClient = new RPCClient(credentials, new Uri(url), Network.Main);
                try
                {
                    var result = rpcClient.SendCommand(
                        "verifymessage",
                        P2FKSignatureAddress,
                        signature,
                        P2FKRoot.Hash
                    );
                    P2FKRoot.Signed = Convert.ToBoolean(result.Result);
                }
                catch { } // default to false if any issues with signature
            }
            else
            {
                //Object not signed stop tracking Signature address
                P2FKSignatureAddress = "";
            }

            //Populate P2FK object with all values
            P2FKRoot.Id = -1;
            P2FKRoot.TransactionId = transactionid;
            P2FKRoot.Signature = signature;
            P2FKRoot.SignedBy = P2FKSignatureAddress;
            P2FKRoot.BlockDate = blockdate;
            P2FKRoot.Confirmations = confirmations;
            P2FKRoot.File = files;
            P2FKRoot.Message = MessageList.ToArray();
            P2FKRoot.Keyword = keywords;
            P2FKRoot.TotalByteSize = totalByteSize;
            P2FKRoot.BuildDate = DateTime.UtcNow;
            P2FKRoot.Cached = true;

            //Cache Root to disk to speed up future crawls

            var rootSerialized = JsonConvert.SerializeObject(P2FKRoot);
            System.IO.File.WriteAllText(@"root\" + P2FKRoot.TransactionId + @"\" + "ROOT.json", rootSerialized);


            if (P2FKRoot.BlockDate.Year > 1975)
            {
                isMuted = System.IO.File.Exists(@"root\" + P2FKRoot.SignedBy + @"\MUTE");


                if (P2FKRoot.Message.Count() > 0 && !isMuted)
                {

                    foreach (KeyValuePair<string, string> keyword in P2FKRoot.Keyword)
                    {
                        string toAddress = "";
                        if (P2FKRoot.Keyword.Count() > 1) { toAddress = P2FKRoot.Keyword.Keys.GetItemByIndex(P2FKRoot.Keyword.Count() - 2); } else { toAddress = P2FKRoot.Keyword.Keys.Last(); }
                        string msg = "[\"" + P2FKRoot.SignedBy + "\",\"" + P2FKRoot.TransactionId + "\",\"" + toAddress + "\"]";
                        var ROOT = new Options { CreateIfMissing = true };
                        byte[] hashBytes = SHA256.Hash(Encoding.UTF8.GetBytes(msg));
                        string hashString = BitConverter.ToString(hashBytes).Replace("-", "");

                        try
                        {
                            lock (SupLocker)
                            {
                                var db = new DB(ROOT, @"root\"+ keyword.Key + @"\sup");
                                var db2 = new DB(ROOT, @"root\" + keyword.Key + @"\sup2");

                               

                                db.Put(keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss") + "!" + hashString, msg);
                                db.Close();
                                db.Dispose();

                                db2.Put(keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss") + "!" + hashString, msg);
                                db2.Close();
                                db2.Dispose();
                            }
                        }
                        catch
                        {
                            lock (SupLocker)
                            {
                                try { Directory.Delete(@"root\" + keyword.Key + @"\sup", true); } catch { System.Threading.Thread.Sleep(500); try { Directory.Delete(@"root\" + keyword.Key + @"\sup", true); } catch { } }

                                try { Directory.Move(@"root\" + keyword.Key + @"\sup2", @"root\" + keyword.Key + @"\sup"); } catch { }
                            }
                            try
                            {
                                lock (SupLocker)
                                {
                                    var db = new DB(ROOT, @"root\" + keyword.Key + @"\sup");
                                    var db2 = new DB(ROOT, @"root\" + keyword.Key + @"\sup2");
                                    db.Put(keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss") + "!" + hashString, msg);
                                    db.Close();
                                    db.Dispose();


                                    db2.Put(keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss") + "!" + hashString, msg);
                                    db2.Close();
                                    db2.Dispose();
                                }

                            }
                            catch
                            {

                                Directory.Delete(@"root\" + keyword.Key + @"\sup", true); Directory.Delete(@"root\" + keyword.Key + @"\sup2", true);
                            }
                        }


                    }

                }

                if (P2FKRoot.File.ContainsKey("SEC") && !isMuted)
                {

                    foreach (KeyValuePair<string, string> keyword in P2FKRoot.Keyword)
                    {
                        string msg;
                        if (!P2FKRoot.Signed)
                        {
                            msg = "[\"" + P2FKRoot.Keyword.Keys.Last() + "\",\"" + P2FKRoot.TransactionId + "\"]";
                        }
                        else { msg = "[\"" + P2FKRoot.SignedBy + "\",\"" + P2FKRoot.TransactionId + "\"]"; }
                        var ROOT = new Options { CreateIfMissing = true };
                        byte[] hashBytes = SHA256.Hash(Encoding.UTF8.GetBytes(msg));
                        string hashString = BitConverter.ToString(hashBytes).Replace("-", "");




                        try
                        {
                            lock (SupLocker)
                            {
                                var db = new DB(ROOT, @"root\"+ keyword.Key + @"\sec");
                                var db2 = new DB(ROOT, @"root\" + keyword.Key + @"\sec2");
                                db.Put(keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss") + "!" + hashString, msg);
                                db.Close();
                                db.Dispose();

                                db2.Put(keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss") + "!" + hashString, msg);
                                db2.Close();
                                db2.Dispose();
                            }
                        }
                        catch
                        {
                            lock (SupLocker)
                            {
                                try { Directory.Delete(@"root\" + keyword.Key + @"\sec", true); } catch { System.Threading.Thread.Sleep(500); try { Directory.Delete(@"root\" + keyword.Key + @"\sec", true); } catch { } }

                                Directory.Move(@"root\" + keyword.Key + @"\sec2", @"root\" + keyword.Key + @"\sec");
                            }
                            try
                            {
                                lock (SupLocker)
                                {
                                    var db = new DB(ROOT, @"root\" + keyword.Key + @"\sec");
                                    var db2 = new DB(ROOT, @"root\" + keyword.Key + @"\sec2");
                                    db.Put(keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss"), msg);
                                    db.Put("lastkey!" + keyword.Key, keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss"));
                                    db.Close();
                                    db.Dispose();


                                    db2.Put(keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss"), msg);
                                    db2.Put("lastkey!" + keyword.Key, keyword.Key + "!" + P2FKRoot.BlockDate.ToString("yyyyMMddHHmmss"));
                                    db2.Close();
                                    db2.Dispose();
                                }

                            }
                            catch
                            {

                                Directory.Delete(@"root\" + keyword.Key + @"\sec", true); Directory.Delete(@"root\" + keyword.Key + @"\sec2", true);
                            }
                        }


                    }

                }
            }

            return P2FKRoot;
        }
        public static Root[] GetRootsByAddress(string address, string username, string password, string url, int skip = 0, int qty = -1, string versionByte = "111")
        {
            var rootList = new List<Root>();

            if (address.Length < 34) { return rootList.ToArray(); }

            var credentials = new NetworkCredential(username, password);
            var rpcClient = new RPCClient(credentials, new Uri(url));
            bool fetched = false;
            address = address.Replace("<", "").Replace(">", "");
            try
            {
                string diskpath = "root\\" + address + "\\";
                string P2FKJSONString = System.IO.File.ReadAllText(diskpath + "ROOTS.json");
                rootList = JsonConvert.DeserializeObject<List<Root>>(P2FKJSONString);
                fetched = true;

            }
            catch { }

            if (fetched && rootList.Count == 0) { return rootList.ToArray(); }

            int intProcessHeight = 0;

            dynamic deserializedObject = null;

            try { intProcessHeight = rootList.Max(state => state.Id); } catch { }
   ;


            if (intProcessHeight != 0)
            {
                try
                {
                    deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("searchrawtransactions", address, 0, intProcessHeight, 1).ResultString);
                }
                catch { return rootList.ToArray(); }

                if (deserializedObject.Count == 0)
                {
                    if (qty == -1) { return rootList.Skip(skip).ToArray(); }
                    else { return rootList.Skip(skip).Take(qty).ToArray(); }
                }

            }
            //rootList = new List<Root>();
            int innerskip = intProcessHeight;
            //intProcessHeight = 0;
            while (true)
            {
                try
                {
                    deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("searchrawtransactions", address, 0, innerskip, 300).ResultString);
                }
                catch { break; }

                if (deserializedObject.Count == 0)
                    break;


                for (int i = 0; i < deserializedObject.Count; i++)
                {
                    int rootId = i;
                    intProcessHeight++;
                    string hexId = GetTransactionIdByHexString(deserializedObject[i].ToString());

                    var root = Root.GetRootByTransactionId(hexId, username, password, url, versionByte);

                    if (root != null && root.TotalByteSize > 0 && root.Output != null && root.Output.ContainsKey(address))
                    {
                        root.Id = intProcessHeight;
                        
                        rootList.Add(root);
                    }

                }
                innerskip += 300;

            }
            try { rootList.Last().Id = intProcessHeight; } catch { }
            try { Directory.CreateDirectory(@"root\" + address); } catch { }
            var rootSerialized = JsonConvert.SerializeObject(rootList);
            System.IO.File.WriteAllText(@"root\" + address + @"\" + "ROOTS.json", rootSerialized);

            if (qty == -1) { return rootList.Skip(skip).ToArray(); } else { return rootList.Skip(skip).Take(qty).ToArray(); }

        }
        public static byte[] GetRootBytesByLedger(string ledger, string username, string password, string url)
        {
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            byte[] transactionBytes = Array.Empty<byte>();
            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url), Network.Main);
            int length1;
            int length2;
            byte[] result;
            var matches = regexTransactionId.Matches(ledger);
            foreach (Match match in matches)
            {
                byte[] transactionBytesBatch = new byte[0];
                dynamic deserializedObject = JsonConvert.DeserializeObject(
                    rpcClient.SendCommand("getrawtransaction", match.Value, 1).ResultString
                );

                foreach (dynamic v_out in deserializedObject.vout)
                {
                    // checking for all known P2FK bitcoin testnet microtransaction values
                    if (
                        v_out.value == "5.46E-06"
                        || v_out.value == "5.48E-06"
                        || v_out.value == "5.48E-05"
                        || v_out.value == "5.5E-05"
                        || v_out.value == "5.5E-06"
                        || v_out.value == "1E-05"
                        || v_out.value == "1E-08"
                        || v_out.value == "1.3016426"
                        || v_out.value == "0.01"
                        || v_out.value == "1"
                    )
                    {
                        string P2FKSignatureAddress = v_out.scriptPubKey.addresses[0];

                        //retreiving payload data from each address
                        Base58.DecodeWithCheckSum(P2FKSignatureAddress, out byte[] results);

                        //append to a byte[] of all P2FK data

                        length1 = transactionBytesBatch.Length;
                        length2 = results.Length - 1;

                        result = new byte[length1 + length2];

                        System.Buffer.BlockCopy(transactionBytesBatch, 0, result, 0, length1);
                        System.Buffer.BlockCopy(results, 1, result, length1, length2);

                        transactionBytesBatch = result;
                    }
                }

                length1 = transactionBytes.Length;
                length2 = transactionBytesBatch.Length;

                result = new byte[length1 + length2];

                System.Buffer.BlockCopy(transactionBytes, 0, result, 0, length1);
                System.Buffer.BlockCopy(transactionBytesBatch, 0, result, length1, length2);

                transactionBytes = result;
            }
            return transactionBytes;
        }
        public static byte[] GetRootBytesByFile(string[] fileNames)
        {
            byte[] separators = new byte[] { 92, 47, 58, 42, 63, 34, 60, 62, 124 };
            byte[] joinedFileBytes = new byte[] { };
            byte[] fileBytes = new byte[] { };
            foreach (string fileName in fileNames)
            {
                byte[] fileNameBytes = Encoding.ASCII.GetBytes(Path.GetFileName(fileName));
                byte[] separator1 = new byte[] { separators[new Random().Next(0, separators.Length)] };
                byte[] separator2 = new byte[] { separators[new Random().Next(0, separators.Length)] };
                try {fileBytes = System.IO.File.ReadAllBytes(fileName); }
                catch { return joinedFileBytes; }
                byte[] fileSizeBytes = Encoding.ASCII.GetBytes(fileBytes.Length.ToString());
                byte[] joinedBytes = joinedFileBytes.Concat(fileNameBytes).Concat(separator1).Concat(fileSizeBytes).Concat(separator2).Concat(fileBytes).ToArray();
                joinedFileBytes = joinedBytes;

            }
            return joinedFileBytes;
        }
        public static byte[] GetRootBytesByMessage(string[] messages)
        {
            byte[] separators = new byte[] { 92, 47, 58, 42, 63, 34, 60, 62, 124 };
            byte[] joinedMessageBytes = new byte[] { };
            foreach (string message in messages)
            {
                byte[] separator1 = new byte[] { separators[new Random().Next(0, separators.Length)] };
                byte[] separator2 = new byte[] { separators[new Random().Next(0, separators.Length)] };
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                byte[] messageSizeBytes = Encoding.ASCII.GetBytes(messageBytes.Length.ToString());

                byte[] joinedBytes = joinedMessageBytes.Concat(separator1).Concat(messageSizeBytes).Concat(separator2).Concat(messageBytes).ToArray();
                joinedMessageBytes = joinedBytes;
            }
            return joinedMessageBytes;
        }
        public static List<String> GetPublicKeysByAddress(string address, string username, string password, string url)
        {
            List<String> Keys = new List<String>();
            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url), Network.Main);
            string privkey;

            try { privkey = rpcClient.SendCommand("dumpprivkey", address).ResultString; } catch { return Keys; }


            var privKeyHex = BitConverter.ToString(Base58.Decode(privkey)).Replace("-", "");
            privKeyHex = privKeyHex.Substring(2, 64);
            BigInteger privateKey = Hex.HexToBigInteger(privKeyHex);
            ECPoint publicKey = Secp256k1.G.Multiply(privateKey);

            Keys.Add(publicKey.X.ToHex());
            Keys.Add(publicKey.Y.ToHex());

            return Keys;
        }
        public static string GetPublicAddressByKeyword(string keyword, string versionbyte = "111")
        {
            if (keyword == null) { return null; }
            // Cut the string at 20 characters
            if (keyword.Length > 20)
            {
                keyword = keyword.Substring(0, 20);
            }
            // Right pad the string with '#' characters
            keyword = keyword.PadRight(20, '#');

            return Base58.EncodeWithCheckSum(
                new byte[] { byte.Parse(versionbyte) }
                    .Concat(System.Text.Encoding.ASCII.GetBytes(keyword))
                    .ToArray());
        }
        public static string GetKeywordByPublicAddress(string public_address)
        {


            Base58.DecodeWithCheckSum(public_address, out byte[] payloadBytes);

            return Encoding.ASCII.GetString(payloadBytes).Replace("#", "").Substring(1);
        }
        public static string GetTransactionIdByHexString(string transactionHex)
        {
            // Decode the hex string into a byte array
            byte[] transactionBytes = HexStringToByteArray(transactionHex);

            // Calculate the hash of the transaction
            byte[] hash = SHA256.Hash(SHA256.Hash(transactionBytes));

            // Reverse the hash to get the transaction id
            Array.Reverse(hash);

            // Convert the hash to a hex string and return it
            return ByteArrayToHexString(hash);
        }
        public static byte[] DecryptRootBytes(string username, string password, string url, string address, byte[] rootbytes)
        {
            byte[] separators = new byte[] { 92, 47, 58, 42, 63, 34, 60, 62, 124 };
            int secondIndex = -1;
            int count = 0;
            for (int i = 0; i < rootbytes.Length; i++)
            {
                if (separators.Contains(rootbytes[i]))
                {
                    count++;
                    if (count == 2)
                    {
                        secondIndex = i;
                        break;
                    }
                }
            }
            byte[] output = new byte[rootbytes.Length - (secondIndex + 1)];
            Array.Copy(rootbytes, secondIndex + 1, output, 0, output.Length);
            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));
            string privKeyHex;
            try
            {
                privKeyHex = BitConverter.ToString(Base58.Decode(rpcClient.SendCommand("dumpprivkey", parameters: address).ResultString)).Replace("-", "");
            }
            catch (Exception ex)
            {
                return Encoding.ASCII.GetBytes("?" + ex.Message.Length + "?" + ex.Message.ToString());
            }
            privKeyHex = privKeyHex.Substring(2, 64);
            BigInteger privateKey = Hex.HexToBigInteger(privKeyHex);
            ECEncryption encryption = new ECEncryption();
            byte[] decrypted = new byte[0];
            try { decrypted = encryption.Decrypt(privateKey, output); } catch { }
            return decrypted;
        }
        public static byte[] EncryptRootBytes(string username, string password, string url, string address, byte[] rootbytes, string pkx = "", string pky = "", bool returnfile = false)
        {
            ECPoint publicKey;
            if (pkx == "")
            {
                // generate public key from private key
                NetworkCredential credentials = new NetworkCredential(username, password);
                RPCClient rpcClient = new RPCClient(credentials, new Uri(url));
                string privKeyHex;
                try
                {
                    privKeyHex = BitConverter.ToString(Base58.Decode(rpcClient.SendCommand("dumpprivkey", parameters: address).ResultString)).Replace("-", "");
                }
                catch (Exception ex)
                {
                    return Encoding.ASCII.GetBytes("?" + ex.Message.Length + "?" + ex.Message.ToString());
                }
                privKeyHex = privKeyHex.Substring(2, 64);
                BigInteger privateKey = Hex.HexToBigInteger(privKeyHex);
                publicKey = Secp256k1.G.Multiply(privateKey);
            }
            else
            {
                // create ECPoint directly from pkx and pky values
                BigInteger x = Hex.HexToBigInteger(pkx);
                BigInteger y = Hex.HexToBigInteger(pky);
                publicKey = new ECPoint(x, y);
            }

            ECEncryption encryption = new ECEncryption();
            byte[] encrypted = encryption.Encrypt(publicKey, rootbytes);
            byte[] separators = new byte[] { 92, 47, 58, 42, 63, 34, 60, 62, 124 };
            byte[] fileNameBytes = Encoding.ASCII.GetBytes("SEC");
            byte[] separator1 = new byte[] { separators[new Random().Next(0, separators.Length)] };
            byte[] separator2 = new byte[] { separators[new Random().Next(0, separators.Length)] };
            byte[] fileSizeBytes = Encoding.ASCII.GetBytes(encrypted.Length.ToString());
            byte[] joinedBytes = fileNameBytes.Concat(separator1).Concat(fileSizeBytes).Concat(separator2).Concat(encrypted).ToArray();
            if (!returnfile) { return joinedBytes; } else { return encrypted;}
            
        }
        static byte[] HexStringToByteArray(string hex)
        {
            // Check for an even number of characters
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException("Hex string must have an even number of characters.");
            }

            // Allocate a new byte array
            byte[] bytes = new byte[hex.Length / 2];

            // Convert the hex string to a byte array
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
        static string ByteArrayToHexString(byte[] bytes)
        {
            // Allocate a new string builder
            StringBuilder sb = new StringBuilder(bytes.Length * 2);

            // Convert the byte array to a hex string
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }

    }
}