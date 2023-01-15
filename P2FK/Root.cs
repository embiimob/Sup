using LevelDB;
using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static NBitcoin.Scripting.PubKeyProvider;


namespace SUP.P2FK
{
    public class Root
    {
        public int Id { get; set; }
        public string[] Message { get; set; }
        public Dictionary<string, byte[]> File { get; set; }
        public Dictionary<string, string> Keyword { get; set; }
        public string Hash { get; set; }
        public string SignedBy { get; set; }
        public string Signature { get; set; }
        public bool Signed { get; set; }
        public DateTime BlockDate { get; set; }
        public int TotalByteSize { get; set; }
        public string TransactionId { get; set; }
        public int Confirmations { get; set; }
        public DateTime BuildDate { get; set; }
        public bool Cached { get; set; }

        //ensures levelDB is thread safely
        private readonly static object levelDBLocker = new object();
        public static Root GetRootByTransactionId(string transactionid, string username, string password, string url, string versionbyte = "111", bool usecache = true, byte[] rootbytes = null, string signatureaddress = null)
        {
            Root P2FKRoot = new Root();
            string diskpath = "root\\" + transactionid + "\\";

            //get cached P2FK Object from Disk
            if (usecache)
            {
                string P2FKJSONString = null;

                try
                {
                    using (StreamReader r = new StreamReader(diskpath + "P2FK.json"))
                    {
                        P2FKJSONString = r.ReadToEnd();
                    }

                }
                //Throws exception if P2FK.json file cache does not exist
                catch (Exception)
                {
                    //Check levelDB for P2FK transaction ID cache status
                    lock (levelDBLocker)
                    {
                        var ROOT = new Options { CreateIfMissing = true };
                        var db = new DB(ROOT, @"root");
                        P2FKJSONString = db.Get(transactionid);
                        db.Close();
                    }

                }
                //if transactionID is found in LevelDB cache with invalid status return null
                if (P2FKJSONString == "invalid")
                {
                    return null;
                }

                //Found P2FK Object Cache
                if (P2FKJSONString != null)
                {
                    P2FKRoot = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                    var modifiedDictionary = new Dictionary<string, byte[]>();

                    //Add cached file data from disk back into the root object
                    foreach (var kvp in P2FKRoot.File)
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
                    //put updated File element back into the object
                    P2FKRoot.File = modifiedDictionary;
                    P2FKRoot.Cached = false;
                    return P2FKRoot;
                }
            }

            //P2FK Object Cache does not exist or useCache = false
            //build P2FK Object from Blockchain

            //used as P2FK Delimiters
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            Regex regexSpecialChars = new Regex(@"([\\/:*?""<>|])\d+");
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
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
                RPCClient rpcClient = new RPCClient(credentials, new Uri(url));

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
                confirmations = deserializedObject.confirmations;
                blockdate =
                    DateTimeOffset.FromUnixTimeSeconds(
                        Convert.ToInt32(deserializedObject.blocktime)
                    ).DateTime;


                // we are spinning through all the out addresses within each bitcoin transaction
                // we are base58 decdoing each address to obtain a 20 byte payload that is appended to a byte[]
                foreach (dynamic v_out in deserializedObject.vout)
                {

                    // checking for all known P2FK bitcoin testnet microtransaction values
                    if (
                        v_out.value == "5.46E-06"
                        || v_out.value == "5.48E-06"
                        || v_out.value == "5.48E-05"
                        || v_out.value == "5.5E-05"
                    )
                    {
                        byte[] results = Array.Empty<byte>();
                        P2FKSignatureAddress = v_out.scriptPubKey.addresses[0];

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

                    //Process Ledger files until reaching Root
                    while (regexTransactionId.IsMatch(fileName))
                    {

                        //supports older objects where the file ledger name was repeated inside filecontents
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
                        usecache,
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
                            fileBytes = P2FKRoot.File.Values.First();
                        }
                        else { fileName = ""; fileBytes = Array.Empty<byte>(); }
                        P2FKRoot.TotalByteSize += totalByteSize;
                        P2FKRoot.Confirmations = confirmations;
                        P2FKRoot.BlockDate = blockdate;

                    }
                    if (isledger)
                    {
                        //Cache Root if enabled
                        if (usecache) { CacheRoot(P2FKRoot); }
                        return P2FKRoot;
                    }

                    if (fileName == "SIG")
                    {
                        sigStartByte = sigEndByte;
                        try
                        {
                            signature = transactionASCII.Substring(headerSize, packetSize);
                        }
                        catch {
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

                            return null; }
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

            //removing null characters from end of last payload.
            while (transactionASCII.IndexOf('\0') >= 0)
            {
                transactionASCII = transactionASCII.Substring(transactionASCII.IndexOf('\0') + 1);
            }

            //assumes any remaining unprocessed characters are keywords
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


            if (sigStartByte > 0)
            {
                //used in signature verification
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
                RPCClient rpcClient = new RPCClient(credentials, new Uri(url));

                var result = rpcClient.SendCommand(
                    "verifymessage",
                    P2FKSignatureAddress,
                    signature,
                    P2FKRoot.Hash
                );
                P2FKRoot.Signed = Convert.ToBoolean(result.Result);
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

            //Cache Root if enabled
            if (usecache) { CacheRoot(P2FKRoot); P2FKRoot.Cached = true; } else { P2FKRoot.Cached = false; }

            return P2FKRoot;
        }
        public static Root[] GetRootByAddress(string address, string username, string password, string url, string versionByte = "111", bool useCache = true, int skip = 0, int qty = 500)
        {
            var rootList = new List<Root>();
            NetworkCredential credentials = new NetworkCredential(username, password);
            var rpcClient = new RPCClient(credentials, new Uri(url));
            var synchronousData = new Dictionary<int, Root>();
            dynamic deserializedObject = null;
            try
            {
                deserializedObject = JsonConvert.DeserializeObject(rpcClient.SendCommand("searchrawtransactions", address, 0, skip, qty).ResultString);
            }
            catch (Exception ex)
            {
                var root = new Root
                {
                    Message = new[] { ex.Message },
                    BuildDate = DateTime.UtcNow,
                    File = new Dictionary<string, byte[]> { },
                    Keyword = new Dictionary<string, string> { },
                    TransactionId = address
                };
                rootList.Add(root);
                return rootList.ToArray();
            }

            // Use a ConcurrentDictionary to store the transaction data
            // This will allow multiple threads to add data to it concurrently
            var concurrentData = new ConcurrentDictionary<int, Root>();
            var tasks = new List<Task>();
            int taskCount = 0;

            // Iterate through the transaction IDs
            for (int i = 0; i < deserializedObject.Count; i++)
            {
                int rootId = i;
                string hexId = GetTransactionIdByHexString(deserializedObject[i].ToString());

                // Launch a separate task to retrieve the transaction bytes for this match
                var task = Task.Run(() =>
                {
                    var root = Root.GetRootByTransactionId(hexId, username, password, url, versionByte, useCache);

                    if (root != null && root.TotalByteSize > 0)
                    {
                        root.Id = rootId + skip;
                        concurrentData[rootId] = root;
                    }
                });
                tasks.Add(task);
                taskCount++;

                // If there are 10 or more tasks, wait for some of them to complete before continuing
                if (taskCount >= 8)
                {
                    Task.WaitAny(tasks.ToArray());
                    tasks = tasks.Where(t => !t.IsCompleted).ToList();
                    taskCount = tasks.Count;
                }
            }

            // Wait for all tasks to complete
            Task.WaitAll(tasks.ToArray());

            // Add the transaction data to the list in the correct order
            foreach (var kvp in concurrentData.OrderBy(kvp => kvp.Key))
            {
                rootList.Add(kvp.Value);
            }

            return rootList.ToArray();
        }
        public static byte[] GetRootBytesByLedger(string ledger, string username, string password, string url)
        {
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            byte[] transactionBytes = Array.Empty<byte>();
            NetworkCredential credentials = new NetworkCredential(username, password);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(url));
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
            foreach (string fileName in fileNames)
            {
                byte[] fileNameBytes = Encoding.ASCII.GetBytes(Path.GetFileName(fileName));
                byte[] separator1 = new byte[] { separators[new Random().Next(0, separators.Length)] };
                byte[] separator2 = new byte[] { separators[new Random().Next(0, separators.Length)] };
                byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
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
        static string GetTransactionIdByHexString(string transactionHex)
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
            byte[] decrypted = encryption.Decrypt(privateKey, output);
            return decrypted;
        }
        public static byte[] EncryptRootBytes(string username, string password, string url, string address, byte[] rootbytes)
        {
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
            ECPoint publicKey = Secp256k1.G.Multiply(privateKey);
            ECEncryption encryption = new ECEncryption();
            byte[] encrypted = encryption.Encrypt(publicKey, rootbytes);
            byte[] separators = new byte[] { 92, 47, 58, 42, 63, 34, 60, 62, 124 };
            byte[] fileNameBytes = Encoding.ASCII.GetBytes("SEC");
            byte[] separator1 = new byte[] { separators[new Random().Next(0, separators.Length)] };
            byte[] separator2 = new byte[] { separators[new Random().Next(0, separators.Length)] };
            byte[] fileSizeBytes = Encoding.ASCII.GetBytes(encrypted.Length.ToString());
            byte[] joinedBytes = fileNameBytes.Concat(separator1).Concat(fileSizeBytes).Concat(separator2).Concat(encrypted).ToArray();
            return joinedBytes;
        }
        private static void CacheRoot(Root root)
        {
            //we have to take the file data out of the object before storing it into a LevelDB cache
            //replacing the bytes array with the filebyte count.
            var modifiedDictionary = new Dictionary<string, byte[]>();


            try
            {
                foreach (var kvp in root.File)
                {
                    // Modify the key by adding "root/" to the beginning
                    string modifiedKey = @"root/" + root.TransactionId + @"/" + kvp.Key;

                    // Replace the value with an empty byte array
                    byte[] modifiedValue = Encoding.ASCII.GetBytes(kvp.Value.Length.ToString());

                    // Add the modified key-value pair to the new dictionary
                    modifiedDictionary.Add(modifiedKey, modifiedValue);
                }
            }
            catch (Exception) { }


            root.File = modifiedDictionary;
            root.Cached = true;
            var rootSerialized = JsonConvert.SerializeObject(root);
            var rootByteArray = Encoding.ASCII.GetBytes(rootSerialized);
            lock (levelDBLocker)
            {
                var ROOT = new Options { CreateIfMissing = true };
                var db = new DB(ROOT, @"root");
                db.Put(root.TransactionId, rootSerialized);
                db.Close();
            }

            using (FileStream fs = new FileStream(@"root/" + root.TransactionId + @"/" + "P2FK.json", FileMode.Create))
            {
                fs.Write(rootByteArray, 0, rootByteArray.Length);
            }

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