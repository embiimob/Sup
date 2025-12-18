using AngleSharp.Common;
using SUP.RPCClient;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;


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
        public int BlockHeight { get; set; }
        public int TotalByteSize { get; set; }
        public int Confirmations { get; set; }
        public DateTime BuildDate { get; set; }
        public bool Cached { get; set; }


        public static Root GetRootByTransactionId(string transactionid, string username, string password, string url, string versionbyte = "111", byte[] rootbytes = null, string signatureaddress = null, bool calculate = false)
        {
            Root P2FKRoot = new Root();
            string diskpath = "root\\" + transactionid + "\\";
            string P2FKJSONString = null;
            bool isMuted = false;
            try
            {
                if (rootbytes == null && calculate == false)
                {
                    try
                    {

                        P2FKJSONString = System.IO.File.ReadAllText(diskpath + "ROOT.json");
                        P2FKRoot = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                       
                        if (P2FKRoot.Confirmations > 0)
                        {
                            return P2FKRoot;
                        }
                    }
                    catch { }

                }

    
                //used as P2FK Delimiters
                char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
                Regex regexSpecialChars = new Regex(@"([\\/:*?""<>|])\d+");
                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                Dictionary<string, BigInteger> files = new Dictionary<string, BigInteger>();
                Dictionary<string, string> keywords = new Dictionary<string, string>();
                DateTime blockdate = new DateTime();
                int confirmations = -1;
                int blockheight = 0;
                Dictionary<string, string> outputs;
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
                    var allowedValues = new HashSet<string>
{
    "0.00000001",
    "0.00000546",
    "0.00000548",
    "0.00005480",
    "0.00000550",
    "0.00005500",
    "0.00001000",
    "0.01000000",
    "0.02000000",
    "1"
   
};


                    try
                    {
                        // Use backend factory to get appropriate backend (RPC or API)
                        IBitcoinBackend backend = BitcoinBackendFactory.Create(url, username, password, versionbyte);
                        deserializedObject = backend.GetRawTransaction(transactionid, 1);

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

                    if (calculate)
                    {
                        try
                        {
                            // Use backend factory for block height lookup
                            IBitcoinBackend backend = BitcoinBackendFactory.Create(url, username, password, versionbyte);
                            string hash = deserializedObject.blockhash;
                            blockheight = backend.GetBlockHeight(hash);
                            P2FKRoot.BlockHeight = blockheight;
                        }
                        catch(Exception ex) {
                            string er = ex.Message;

                        }

                    }
                    P2FKRoot.Output = new Dictionary<string, string>();
                    // we are spinning through all the out addresses within each bitcoin transaction
                    // we are base58 decdoing each address to obtain a 20 byte payload that is appended to a byte[]
                    foreach (dynamic v_out in deserializedObject.vout)
                    {

                        string address = v_out.scriptPubKey.addresses[0].ToString();
                        string value = v_out.value.ToString();
                        P2FKRoot.Output.Add(address, value);

                        // checking for all known P2FK bitcoin testnet microtransaction values
                        if (allowedValues.Contains(value))
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
                outputs = P2FKRoot.Output;
                // Perform the loop until no additional numbers are found and the regular expression fails to match
                while (regexSpecialChars.IsMatch(transactionASCII))
                {


                    Match match = regexSpecialChars.Match(transactionASCII);
                    int packetSize;
                    int headerSize;
                    try
                    {
                        packetSize = Int32.Parse(match.Value.ToString().Remove(0, 1), NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
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
                    if ((fileName.Length > 2 & fileName.Contains(".")) || (fileName.Length == 3 && !"BTC,LTC,DOG,MZC,IPFS".Contains(fileName)) || (!fileName.Contains(".") & fileName.Length == 64))

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
                            if (P2FKRoot.File != null && P2FKRoot.File.Count > 0)
                            {
                                fileName = P2FKRoot.File.Keys.First();
                            }
                            else { fileName = ""; fileBytes = Array.Empty<byte>(); }

                            P2FKRoot.TotalByteSize += totalByteSize;
                            P2FKRoot.Confirmations = confirmations;
                            P2FKRoot.BlockDate = blockdate;
                            P2FKRoot.Output = outputs;
                            P2FKRoot.BlockHeight = blockheight;

                        }



                        if (isledger)
                        {
                            // Do not cache ledger/multifile root objects because their Keywords are assembled
                            // from child transactions which may not match the parent's outputs, causing
                            // issues when the same ledger is accessed for different object addresses
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
                        if (fileName == "" && fileBytes.Length > 1)
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
                            Encoding.UTF8.GetString(KeywordArray)
                        );
                    }
                    catch { }

                }


                if (sigStartByte > 0)
                {
                    // Convert the byte array to an ASCII-encoded string
                    //string asciiString = Encoding.ASCII.GetString(transactionBytes);

                    // Create SHA-256 hash
                    System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                    P2FKRoot.Hash = BitConverter
                        .ToString(
                            mySHA256.ComputeHash(transactionBytes
                                    .Skip(sigStartByte)
                                    .Take(sigEndByte - sigStartByte)
                                    .ToArray()
                            )
                        )
                        .Replace("-", String.Empty);

                    NetworkCredential credentials = new NetworkCredential(username, password);
                    NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(url), Network.Main);
                    try
                    {
                        string result = rpcClient.SendCommand(
                            "verifymessage",
                            P2FKSignatureAddress,
                            signature,
                            P2FKRoot.Hash
                        ).ResultString;
                        P2FKRoot.Signed = Convert.ToBoolean(result);
                    }
                    catch { } // default to false if any issues with signature
                }
                else
                {
                    // Object not signed, stop tracking Signature address
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
                P2FKRoot.BlockHeight = blockheight;
                P2FKRoot.Cached = true;

                //Cache Root to disk to speed up future crawls


                    if (!System.IO.File.Exists(@"root\" + P2FKRoot.SignedBy + @"\BLOCK"))
                    {
                        var rootSerialized = JsonConvert.SerializeObject(P2FKRoot);
                        System.IO.File.WriteAllText(@"root\" + P2FKRoot.TransactionId + @"\" + "ROOT.json", rootSerialized);
                    }
                                      

              

            }
            catch (Exception ex)  { 
                    string error = ex.Message; }
            return P2FKRoot;
        }

        public static Root[] GetRootsByAddress(string address, string username, string password, string url, int skip = 0, int qty = -1, string versionByte = "111", bool calculate = false)
        {

            try { using (System.IO.File.Create("ROOTS-PROCESSING")) { } }catch { }

            var rootList = new List<Root>();

            try
            {
                if (address.Length < 33)
                {

                    try { System.IO.File.Delete("ROOTS-PROCESSING"); } catch { }
                    return rootList.ToArray();
                }

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


                int intProcessHeight = 0;

                try { intProcessHeight = rootList.Max(max => max.Id); } catch { }


                if (calculate) { intProcessHeight = 0; rootList = new List<Root>(); }

                int innerskip = intProcessHeight;
                bool calculated = false;



                while (true)
                {
                        // Use backend factory to get appropriate backend (RPC or API)
                        IBitcoinBackend backend = BitcoinBackendFactory.Create(url, username, password, versionByte);

                        List<GetRawDataTransactionResponse> results = null;

                    try { results = backend.SearchRawTransactions(address, 0, innerskip, 300); } catch { break; }


                        if (results == null || results.Count == 0) { break; }

                        for (int i = 0; i < results.Count; i++)
                        {
                            calculated = true;
                            intProcessHeight++;
                            // Use txid directly from the response instead of calculating from hex
                            // This works for both RPC (which includes hex) and API mode (which provides txid)
                            string txId = results[i].txid;
                            
                            // Fallback: if txid is null but hex is available, calculate it from hex
                            if (string.IsNullOrEmpty(txId) && !string.IsNullOrEmpty(results[i].hex))
                            {
                                txId = GetTransactionIdByHexString(results[i].hex);
                            }
                            
                            if (string.IsNullOrEmpty(txId))
                            {
                                // Skip this transaction if we can't get the ID
                                intProcessHeight--;
                                continue;
                            }
                            
                            Root root = new Root();
                            root = Root.GetRootByTransactionId(txId, username, password, url, versionByte, null, null, calculate);

                            if (root != null && root.TotalByteSize > 0 && root.Output != null && !rootList.Any(ROOT => ROOT.TransactionId == root.TransactionId) && root.Output.ContainsKey(address) && root.BlockDate.Year > 1975)
                            {
                                root.Id = intProcessHeight;

                                rootList.Add(root);
                            }
                            else
                            {
                                if (root != null && root.Output != null && root.BlockDate.Year < 1975) { intProcessHeight--; }
                            }

                        }
                        innerskip += 300;
                   

                }




                if (calculated && rootList.Count() > 0)
                {
                    try { rootList.Last().Id = intProcessHeight; } catch { }

                    try { Directory.CreateDirectory(@"root\" + address); } catch { }
                    var rootSerialized = JsonConvert.SerializeObject(rootList);
                    System.IO.File.WriteAllText(@"root\" + address + @"\" + "ROOTS.json", rootSerialized);

                }

                try { System.IO.File.Delete("ROOTS-PROCESSING"); } catch { }


                if (skip != 0)
                {
                    //GPT3 SUGGESTED
                    var skippedList = rootList.Where(state => state.Id >= skip);


                    if (qty == -1) { return skippedList.ToArray(); }
                    else { return skippedList.Take(qty).ToArray(); }
                }
                else
                {
                    if (qty == -1) { return rootList.ToArray(); }
                    else { return rootList.Take(qty).ToArray(); }

                }
            }
            catch
            {

                try { System.IO.File.Delete("ROOTS-PROCESSING"); } catch { }

                return rootList.ToArray();
            }
        }

        public static byte[] GetRootBytesByLedger(string ledger, string username, string password, string url)
        {
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            byte[] transactionBytes = Array.Empty<byte>();


            int length1;
            int length2;
            byte[] result;
            var matches = regexTransactionId.Matches(ledger);
            var allowedValues = new HashSet<string>
{
    "0.00000001",
    "0.00000546",
    "0.00000548",
    "0.00005480",
    "0.00000550",
    "0.00005500",
    "0.00001000",
    "0.01000000",
    "0.02000000",
    "1"
};
            foreach (Match match in matches)
            {


                byte[] transactionBytesBatch = new byte[0];
                CoinRPC a = new CoinRPC(new Uri(url), new NetworkCredential(username, password));
                dynamic deserializedObject = a.GetRawDataTransaction(match.Value, 1);

                foreach (dynamic v_out in deserializedObject.vout)
                {
                    // checking for all known P2FK bitcoin testnet microtransaction values

                    string value = v_out.value.ToString();
                    if (allowedValues.Contains(value))
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
                try { fileBytes = System.IO.File.ReadAllBytes(fileName); }
                catch { return joinedFileBytes; }
                byte[] fileSizeBytes = Encoding.ASCII.GetBytes(fileBytes.Length.ToString());
                byte[] joinedBytes = joinedFileBytes.Concat(fileNameBytes).Concat(separator1).Concat(fileSizeBytes).Concat(separator2).Concat(fileBytes).ToArray();
                joinedFileBytes = joinedBytes;

            }
            return joinedFileBytes;
        }

        public static List<String> GetPublicKeysByAddress(string address, string username, string password, string url)
        {
            List<String> Keys = new List<String>();

            CoinRPC a = new CoinRPC(new Uri(url), new NetworkCredential(username, password));

            string privkey;

            try { privkey = a.DumpPrivKey(address); } catch { return Keys; }

            if (privkey == null) { return Keys; }

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

            // Convert the string to UTF-8 bytes
            byte[] keywordBytes = Encoding.UTF8.GetBytes(keyword);

            // Ensure the keywordBytes is exactly 20 bytes in length by right-padding with '#' characters
            if (keywordBytes.Length < 20)
            {
                byte[] paddedBytes = new byte[20];
                Array.Copy(keywordBytes, paddedBytes, keywordBytes.Length);
                for (int i = keywordBytes.Length; i < 20; i++)
                {
                    paddedBytes[i] = (byte)'#';
                }
                keywordBytes = paddedBytes;
            }
            else if (keywordBytes.Length > 20)
            {
                // If it's more than 20 bytes, truncate it to 20 bytes
                byte[] truncatedBytes = new byte[20];
                Array.Copy(keywordBytes, truncatedBytes, 20);
                keywordBytes = truncatedBytes;
            }

            return Base58.EncodeWithCheckSum(
                new byte[] { byte.Parse(versionbyte) }
                    .Concat(keywordBytes)
                    .ToArray());
        }

        public static string GetKeywordByPublicAddress(string public_address, string encoding = "UTF8")
        {


            Base58.DecodeWithCheckSum(public_address, out byte[] payloadBytes);
            if (encoding == "UTF8")
            {
                return Encoding.UTF8.GetString(payloadBytes).Replace("#", "").Substring(1);
            }
            else
            {
                return Encoding.ASCII.GetString(payloadBytes).Replace("#", "").Substring(1);
            }
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
            
            // Check if API mode is enabled
            if (BitcoinBackendFactory.IsApiModeEnabled())
            {
                return Encoding.ASCII.GetBytes("?ERROR: DecryptRootBytes requires wallet access which is not available in API mode. Please use local RPC mode for this operation.");
            }
            
            NetworkCredential credentials = new NetworkCredential(username, password);
            NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(url));
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
                // Check if API mode is enabled
                if (BitcoinBackendFactory.IsApiModeEnabled())
                {
                    return Encoding.ASCII.GetBytes("?ERROR: EncryptRootBytes requires wallet access which is not available in API mode. Please use local RPC mode for this operation.");
                }
                
                // generate public key from private key
                NetworkCredential credentials = new NetworkCredential(username, password);
                NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(url));
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
            if (!returnfile) { return joinedBytes; } else { return encrypted; }

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