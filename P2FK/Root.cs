using LevelDB;
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
        public bool Cached { get; set; }

        private readonly static object levelDBLocker = new object();
        public static Root GetRootByTransactionId(
            string transactionid,
            string username,
            string password,
            string url,
            string versionbyte = "111",
            bool usecache = true,
            byte[] rootbytes = null,
            string publicaddress = ""
        )
        {
            Root newRoot = new Root();
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
            Dictionary<string, string> keywords = new Dictionary<string, string>();
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");

            //checking LevelDB returning Cached Root if found
            if (usecache)
            {
                try
                {
                    string keyValue = null;

                    lock (levelDBLocker)
                    {
                        var ROOT = new Options { CreateIfMissing = true };
                        var db = new DB(ROOT, @"root");
                        keyValue = db.Get(transactionid);
                        db.Close();
                    }
                    if (keyValue == "invalid")
                    {
                        return null;
                    }
                    if (keyValue != null)
                    {
                        newRoot = JsonConvert.DeserializeObject<Root>(keyValue);

                        var modifiedDictionary = new Dictionary<string, byte[]>();

                        //we have to add the actual file data from disk back into the object
                        foreach (var kvp in newRoot.File)
                        {
                            byte[] fileBytes;
                            using (
                                FileStream fs = new FileStream(
                                    kvp.Key,
                                    FileMode.Open,
                                    FileAccess.Read
                                )
                            )
                            {
                                fileBytes = new byte[fs.Length];
                                fs.Read(fileBytes, 0, fileBytes.Length);
                            }

                            // Modify the key to be fileName
                            string modifiedKey = Path.GetFileName(kvp.Key);

                            // Replace the value with actual file Bytes
                            byte[] modifiedValue = fileBytes;

                            // Add the modified key-value pair to the new dictionary
                            modifiedDictionary.Add(modifiedKey, modifiedValue);
                        }
                        newRoot.File = modifiedDictionary;
                        return newRoot;
                    }
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
            }

            //used as P2FK Delimiters
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            Regex regexSpecialChars = new Regex(@"([\\/:*?""<>|])\d+");

            dynamic deserializedObject;
            byte VersionByte = byte.Parse(versionbyte);
            byte[] VersionByteArray = new byte[] { VersionByte };
            //defining items to include in the returned object

            List<String> MessageList = new List<String>();
            byte[] transactionBytes = Array.Empty<byte>();
            string transactionASCII;
            string strPublicAddress = publicaddress;
            string signature = "";
            DateTime blockdate = new DateTime();
            int confirmations = 0;
            byte[] KeywordArray = new byte[21];
            int sigStartByte = 0;
            int sigEndByte = 0;
            int totalByteSize;
            //create a new root object

            Root LedgerRoot = new Root();
            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));
            if (rootbytes == null)
            {
                try
                {
                    deserializedObject = JsonConvert.DeserializeObject(
                        rpcClient.SendCommand("getrawtransaction", transactionid, 1).ResultString
                    );
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
                blockdate =
                    DateTimeOffset.FromUnixTimeSeconds(
                        Convert.ToInt32(deserializedObject.blocktime)
                    ).DateTime;
                confirmations = deserializedObject.confirmations;
                // we are spinning through all the out addresses within each bitcoin transaction
                // we are base58 decdoing each address to obtain a 20 byte payload that is appended to a byte[]
                foreach (dynamic v_out in deserializedObject.vout)
                {
                    // checking for all known P2FK bitcoin testnet microtransaction values
                    if (
                        v_out.value == "5.46E-06"
                        || v_out.value == "5.48E-06"
                        || v_out.value == "5.48E-05"
                    )
                    {
                        byte[] results = Array.Empty<byte>();
                        strPublicAddress = v_out.scriptPubKey.addresses[0];

                        //retreiving payload data from each address
                        Base58.DecodeWithCheckSum(strPublicAddress, out results);

                        //append to a byte[] of all P2FK data
                        //transactionBytes = transactionBytes.Concat(results.Skip(1)).ToArray();


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
                if (transactionASCII.IndexOfAny(specialChars) != match.Index)
                {
                    break;
                }

                sigEndByte += packetSize + headerSize;

                string fileName = transactionASCII.Substring(0, match.Index);
                byte[] fileBytes = transactionBytes
                    .Skip(headerSize + (transactionBytesSize - transactionASCII.Length))
                    .Take(packetSize)
                    .ToArray();

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
                    while (regexTransactionId.IsMatch(fileName))
                    {
                        using (FileStream fs = new FileStream(diskpath + fileName, FileMode.Create))
                        {
                            fs.Write(fileBytes, 0, fileBytes.Length);
                        }

                        LedgerRoot = GetRootByTransactionId(
                            transactionid,
                            username,
                            password,
                            url,
                            versionbyte,
                            usecache,
                            GetLedgerBytes(
                                fileName
                                    + Environment.NewLine
                                    + Encoding.ASCII.GetString(fileBytes).Replace(fileName, ""),
                                username,
                                password,
                                url
                            ),
                            strPublicAddress
                        );
                        fileName = LedgerRoot.File.Keys.First();
                        fileBytes = LedgerRoot.File.Values.First();
                        totalByteSize += LedgerRoot.TotalByteSize;
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
                    else
                    {
                        break;
                    }
                }

                try
                {
                    transactionASCII = transactionASCII.Remove(0, (packetSize + headerSize));
                }
                catch (Exception)
                {
                    break;
                }
            }

            if (files.Count() + MessageList.Count() == 0)
            {
                if (usecache)
                {
                    lock (levelDBLocker)
                    {
                        var ROOT = new Options { CreateIfMissing = true };
                        var db = new DB(ROOT, @"root");
                        db.Put(transactionid, "invalid");
                        db.Close();
                    }
                }

                return null;
            }

            //removing null characters
            while (transactionASCII.IndexOf('\0') >= 0)
            {
                transactionASCII = transactionASCII.Substring(transactionASCII.IndexOf('\0') + 1);
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
                catch (Exception) { }
            }

            if (sigStartByte > 0 && LedgerRoot.Signature == null)
            {
                //used in signature verification
                System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                newRoot.Hash = BitConverter
                    .ToString(
                        mySHA256.ComputeHash(
                            transactionBytes
                                .Skip(sigStartByte)
                                .Take(sigEndByte - sigStartByte)
                                .ToArray()
                        )
                    )
                    .Replace("-", String.Empty);
                var result = rpcClient.SendCommand(
                    "verifymessage",
                    strPublicAddress,
                    signature,
                    newRoot.Hash
                );
                newRoot.Signed = Convert.ToBoolean(result.Result);
            }
            else
            {
                if (sigStartByte == 0)
                {
                    strPublicAddress = "";
                }
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

            //levelDBCache Root if not already there
            if (usecache)
            {
                //we have to take the file data out of the object before storing it into a LevelDB cache
                //replacing the bytes array with the filebyte count.
                var modifiedDictionary = new Dictionary<string, byte[]>();
                foreach (var kvp in newRoot.File)
                {
                    // Modify the key by adding "root/" to the beginning
                    string modifiedKey = @"root/" + newRoot.TransactionId + @"/" + kvp.Key;

                    // Replace the value with an empty byte array
                    byte[] modifiedValue = Encoding.ASCII.GetBytes(kvp.Value.Length.ToString());

                    // Add the modified key-value pair to the new dictionary
                    modifiedDictionary.Add(modifiedKey, modifiedValue);
                }
                newRoot.File = modifiedDictionary;
                newRoot.Cached = true;
                lock (levelDBLocker)
                {
                    var ROOT = new Options { CreateIfMissing = true };
                    var db = new DB(ROOT, @"root");
                    db.Put(newRoot.TransactionId, JsonConvert.SerializeObject(newRoot));
                    db.Close();
                }
            }

            newRoot.File = files;
            return newRoot;
        }
        public static Root[] GetRootByAddress(
            string address,
            string username,
            string password,
            string url,
            int skip = 0,
            int qty = 500,
            string versionbyte = "111",
            bool useCache = true
        )
        {
            List<Root> RootList = new List<Root>();

            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));
            Dictionary<string, Root> synchronousData = new Dictionary<string, Root>();
            dynamic deserializedObject = null;
            try
            {
                deserializedObject = JsonConvert.DeserializeObject(
                    rpcClient.SendCommand(
                        "searchrawtransactions",
                        address,
                        1,
                        skip,
                        qty
                    ).ResultString
                );
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
                Thread thread = new Thread(
                    () =>
                    {
                        string transactionid = transID.txid;
                        Root root = Root.GetRootByTransactionId(
                            transactionid,
                            username,
                            password,
                            url,
                            versionbyte,
                            useCache
                        );

                        if (root != null)
                        {
                            try
                            {
                                synchronousData.Add(transactionid, root);
                            }
                            catch (Exception) { }
                        }
                        countdownEvent.Signal();
                    }
                );

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
                catch (Exception) { }
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

            return Base58.EncodeWithCheckSum(
                new byte[] { byte.Parse(versionbyte) }
                    .Concat(System.Text.Encoding.ASCII.GetBytes(keyword))
                    .ToArray()
            );
        }
        public static string GetKeywordByPublicAddress(string public_address)
        {
            

            Base58.DecodeWithCheckSum(public_address, out byte[] payloadBytes);

            return Encoding.ASCII.GetString(payloadBytes).Replace("#", "").Substring(1);
        }
        private static byte[] GetLedgerBytes(
            string ledger,
            string username,
            string password,
            string url
        )
        {
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            byte[] transactionBytes = Array.Empty<byte>();
            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));
            int length1;
            int length2;
            byte[] result;
            byte[] results;
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
                    )
                    {
                        string strPublicAddress = v_out.scriptPubKey.addresses[0];

                        //retreiving payload data from each address
                        Base58.DecodeWithCheckSum(strPublicAddress, out results);

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
    }
}




