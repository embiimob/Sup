﻿using AngleSharp.Common;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SUP.P2FK
{
    public class INQ
    {
        public Dictionary<string, string> que { get; set; }
        public Dictionary<string, string> ans { get; set; }
        public string[] own { get; set; }
        public string[] cre { get; set; }
        public int end { get; set; } = 0;
        public int any { get; set; } = 0;


    }

    public class AnswerData
    {
        public string Address { get; set; }
        public string Answer { get; set; }
        public int TotalGatedVotes { get; set; }
        public decimal TotalGatedValue { get; set; }
        public int TotalVotes { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class INQState
    {
        public int Id { get; set; } = 0;
        public string TransactionId { get; set; }
        public string URN { get; set; }
        public string Question { get; set; }
        public List<AnswerData> AnswerData { get; set; }
        public string[] OwnsObjectGate { get; set; }
        public string[] OwnsCreatedByGate { get; set; }
        public List<String> AuthorizedByGate { get; set; } = new List<string>();
        public int TotalGatedVotes { get; set; } = 0;
        public decimal TotalGatedValue { get; set; } = 0;
        public int TotalVotes { get; set; } = 0;
        public decimal TotalValue { get; set; } = 0;
        public string status { get; set; } = "unknown";
        public int MaxBlockHeight { get; set; } = 0;
        public bool RequireSignature { get; set; } = true;
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ChangedDate { get; set; }



        private readonly static object SupLocker = new object();
        public static INQState GetInquiryByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", bool calculate = false)
        {
            using (FileStream fs = File.Create(@"GET_INQUIRY_BY_ADDRESS"))
            {

            }

            lock (SupLocker)
            {

                INQState objectState = new INQState();
                char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
                try
                {
                    bool fetched = false;

                    if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK"))
                    {
                        try { File.Delete(@"GET_INQUIRY_BY_ADDRESS"); } catch { }
                        return objectState;
                    }

                    string JSONOBJ;
                    string logstatus;
                    string diskpath = "root\\" + objectaddress + "\\";


                    // fetch current JSONOBJ from disk if it exists
                    try
                    {
                        JSONOBJ = System.IO.File.ReadAllText(diskpath + "INQ.json");
                        objectState = JsonConvert.DeserializeObject<INQState>(JSONOBJ);
                        fetched = true;

                    }
                    catch { }

                    if (fetched && objectState.URN == null && !calculate)
                    {
                        try { File.Delete(@"GET_INQUIRY_BY_ADDRESS"); } catch { }

                        return objectState;
                    }

                    if (fetched && objectState.URN != null && !calculate) { return objectState; }


                    Root[] objectTransactions = new Root[0];

                    //calculate blockheight will be needed
                    objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, 0, -1, versionByte, true);

                    if (calculate) { objectState = new INQState(); }

                    foreach (Root transaction in objectTransactions)
                    {


                        if (transaction.Signed && transaction.File.ContainsKey("INQ") )
                        {

                            string sigSeen = null;

                            // Calculate SHA-256 hash of the signature
                            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
                            {
                                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(transaction.Signature));
                                string hashedSignature = BitConverter.ToString(hashBytes).Replace("-", "");

                                string filePath = @"root\" + objectaddress + @"\sig\" + hashedSignature;

                                if (!System.IO.File.Exists(filePath))
                                {
                                    if (!System.IO.Directory.Exists(@"root\" + objectaddress + @"\sig")) { Directory.CreateDirectory(@"root\" + objectaddress + @"\sig"); }
                                    // If the file does not exist, create it and write the text string to it
                                    System.IO.File.WriteAllText(filePath, transaction.TransactionId);
                                }
                                else
                                {
                                    // If the file exists, read its content
                                    sigSeen = System.IO.File.ReadAllText(filePath);
                                }
                            }


                            if (sigSeen == null || (calculate && sigSeen == transaction.TransactionId))
                            {

                                INQ objectinspector = null;

                                //is this even the right object!?  no!?  goodbye!
                                if (!transaction.Keyword.ContainsKey(objectaddress))
                                {
                                    break;
                                }

                                try
                                {
                                    objectinspector = JsonConvert.DeserializeObject<INQ>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\INQ"));

                                }
                                catch (Exception e)
                                {

                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"inspect\",\"\",\"\",\"failed due to invalid format\"]";
                                    break;
                                }



                                //inquiry object is created
                                objectState.TransactionId = transaction.TransactionId;
                                objectState.CreatedDate = transaction.BlockDate;
                                objectState.CreatedBy = transaction.SignedBy;

                                if (objectinspector.any != 0) { objectState.RequireSignature = false; }

                                if (objectinspector.que != null) { objectState.ChangedDate = transaction.BlockDate; objectState.URN = objectinspector.que.First().Key; objectState.Question = objectinspector.que.First().Value; }

                                if (objectinspector.ans != null)
                                {
                                    objectState.ChangedDate = transaction.BlockDate;
                                    List<AnswerData> answerDataList = new List<AnswerData>();

                                    foreach (var entry in objectinspector.ans)
                                    {
                                        AnswerData answerData = new AnswerData
                                        {
                                            Address = entry.Key,   // You can use the key from the ans dictionary as Address
                                            Answer = entry.Value,  // Use the value from the ans dictionary as the Answer
                                            TotalGatedVotes = 0,   // Initialize total counts to 0
                                            TotalGatedValue = 0,   // Initialize total counts to 0
                                            TotalVotes = 0,       // Initialize total counts to 0
                                            TotalValue = 0        // Initialize total counts to 0
                                        };

                                        answerDataList.Add(answerData);
                                    }
                                    objectState.AnswerData = answerDataList;

                                }


                                if (objectinspector.own != null)
                                {
                                    objectState.ChangedDate = transaction.BlockDate; objectState.OwnsObjectGate = objectinspector.own;

                                }

                                if (objectinspector.cre != null)
                                {
                                    objectState.ChangedDate = transaction.BlockDate; objectState.OwnsCreatedByGate = objectinspector.cre;

                                }


                                if (objectinspector.end != 0)
                                {

                                    objectState.ChangedDate = transaction.BlockDate; objectState.MaxBlockHeight = transaction.BlockHeight + objectinspector.end;

                                }



                            }


                            //only inspect the FIRST INQ no updates on this one
                            break;
                        }



                    }

                    if (objectTransactions.Count() > 0 && calculate)
                    {
                        if (objectState.OwnsObjectGate != null)
                        {
                            foreach (string address in objectState.OwnsObjectGate)
                            {

                                OBJState objecto = new OBJState();
                                objecto = OBJState.GetObjectByAddress(address, username, password, url, versionByte);


                                foreach (string owner in objecto.Owners.Keys)
                                {

                                    if (!objectState.AuthorizedByGate.Contains(owner))
                                    {
                                        objectState.AuthorizedByGate.Add(owner);
                                    }

                                }



                            }
                        }


                        if (objectState.OwnsCreatedByGate != null)
                        {
                            foreach (string address in objectState.OwnsCreatedByGate)
                            {

                                List<OBJState> objects = new List<OBJState>();
                                objects = OBJState.GetObjectsCreatedByAddress(address, username, password, url, versionByte);

                                foreach (OBJState obj in objects)
                                {
                                    foreach (string owner in obj.Owners.Keys)
                                    {
                                        if (!objectState.AuthorizedByGate.Contains(owner))
                                        {
                                            objectState.AuthorizedByGate.Add(owner);
                                        }

                                    }


                                }



                            }
                        }

                        HashSet<string> hasVoted = new HashSet<string>();



                        foreach (AnswerData answer in objectState.AnswerData)
                        {
                            Root[] roots = new Root[0];

                            roots = Root.GetRootsByAddress(answer.Address, username, password, url, 0, -1, versionByte, true);

                            roots = roots.Where(obj => !hasVoted.Contains(obj.SignedBy)).ToArray();

                            foreach (Root root in roots)
                            {
                                if (root.Signed)
                                {
                                    try { hasVoted.Add(root.SignedBy); } catch { }
                                }
                            }


                            if (objectState.RequireSignature)
                            {
                                roots = roots.Where(obj => obj.Signed).ToArray();
                            }

                            if (objectState.MaxBlockHeight > 0)
                            {
                                roots = roots.Where(obj => obj.BlockHeight <= objectState.MaxBlockHeight).ToArray();
                            }

                            if (roots.Length > 0)
                            {
                                DateTime newestBlockDate = roots.Max(root => root.BlockDate);

                                //This will update the Last ChangedDate with the latest votes
                                if (newestBlockDate > objectState.ChangedDate)
                                {
                                    objectState.ChangedDate = newestBlockDate;
                                }

                                // total votes without gate checking
                                answer.TotalVotes = roots.Count();
                           
                                decimal totalsum = roots
                                    .Select(obj =>
                                    {
                                        if (obj.Output.TryGetValue(answer.Address, out string value))
                                        {

                                            if (decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out decimal parsedValue))
                                            {
                                                return parsedValue;
                                            }
                                        }
                                        return 0; // Default to 0 if address not found or parsing fails.
                                    })
                                    .Sum();

                                // total sent to answer address without gate checking
                                answer.TotalValue = totalsum;

                                var gatedObjects = roots
                                    .Where(obj => obj.Output.TryGetValue(answer.Address, out string value) &&
                                                  decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out decimal parsedValue) &&
                                                  objectState.AuthorizedByGate.Contains(obj.SignedBy))
                                    .GroupBy(obj => obj.SignedBy)
                                    .Select(group => group.First());

                                if (objectState.MaxBlockHeight > 0)
                                {
                                    gatedObjects = gatedObjects.Where(obj => obj.BlockHeight <= objectState.MaxBlockHeight);
                                }

                                decimal gatedSum = gatedObjects
                                    .Select(obj => decimal.TryParse(obj.Output[answer.Address], System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out decimal parsedValue) ? parsedValue : 0)
                                    .Sum();

                                int totalGatedCount = gatedObjects.Count();

                                answer.TotalGatedValue = gatedSum;
                                answer.TotalGatedVotes = totalGatedCount;
                            }
                        }


                        foreach (AnswerData answerData in objectState.AnswerData)
                        {
                            objectState.TotalVotes += answerData.TotalVotes;
                            objectState.TotalGatedVotes += answerData.TotalGatedVotes;
                            objectState.TotalValue += answerData.TotalValue;
                            objectState.TotalGatedValue += answerData.TotalGatedValue;
                        }



                    }

                    if (objectState.MaxBlockHeight > 0)
                    {
                        NetworkCredential credentials = new NetworkCredential(username, password);
                        NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(url), Network.Main);
                        dynamic blockCountResult = rpcClient.SendCommand("getblockcount");
                        int currentBlockHeight = int.Parse(blockCountResult.ResultString, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));

                        if (currentBlockHeight > objectState.MaxBlockHeight)
                        {
                            objectState.status = "closed";
                        }
                        else
                        { objectState.status = (objectState.MaxBlockHeight - currentBlockHeight).ToString(); }

                    }
                    else
                    {
                        if (objectState.CreatedDate.Year == 1) { objectState.status = "pending"; }
                        else
                        {
                            objectState.status = "open";
                        }
                    }

                    var objectSerialized = JsonConvert.SerializeObject(objectState);


                    if (!Directory.Exists(@"root\" + objectaddress))
                    {
                        Directory.CreateDirectory(@"root\" + objectaddress);
                    }
                    System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "INQ.json", objectSerialized);

                    try { File.Delete(@"GET_INQUIRY_BY_ADDRESS"); } catch { }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                finally { try { File.Delete(@"GET_INQUIRY_BY_ADDRESS"); } catch { } }
                return objectState;

            }
        }

        public static INQState GetInquiryByTransactionId(string transactionid, string username, string password, string url, string versionByte = "111", bool calculate = false)
        {

            INQState objectState = new INQState();
            INQ objectinspector = new INQ();
            char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
           
            var intProcessHeight = 0;
            Root transaction = Root.GetRootByTransactionId(transactionid, username, password, url, versionByte,null,null, calculate);

            string JSONOBJ;

            string diskpath = "root\\" + transactionid + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "INQ");
                objectinspector = JsonConvert.DeserializeObject<INQ>(JSONOBJ);

            }
            catch (Exception ex) { return objectState; }

            // fetch current JSONOBJ from disk if it exists
            try
            {
                objectState = INQState.GetInquiryByAddress(objectinspector.que.First().Key, username, password, url, versionByte, calculate);
            }
            catch { }

           
            return objectState;

        }

        public static List<INQState> GetInquiriesByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1, bool calculate = false)
        {
            using (FileStream fs = File.Create(@"GET_INQUIRIES_BY_ADDRESS"))
            {

            }

            lock (SupLocker)
            {
                List<INQState> objectStates = new List<INQState> { };
                try
                {
                    bool fetched = false;

                    if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK"))
                    {
                        try { File.Delete(@"GET_INQUIRIES_BY_ADDRESS"); } catch { }

                        return objectStates;
                    }


                    string JSONOBJ;
                    string diskpath = "root\\" + objectaddress + "\\";


                    // fetch current JSONOBJ from disk if it exists
                    try
                    {
                        JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetInquiriesByAddress.json");
                        objectStates = JsonConvert.DeserializeObject<List<INQState>>(JSONOBJ);
                        fetched = true;

                    }
                    catch { }
                    if (fetched && objectStates.Count < 1)
                    {
                        try { File.Delete(@"GET_INQUIRIES_BY_ADDRESS"); } catch { }

                        return objectStates;
                    }

                    int intProcessHeight = 0;

                    if (calculate) { intProcessHeight = 0; objectStates = new List<INQState> { }; }
                    else
                    {
                        try { intProcessHeight = objectStates.Max(max => max.Id); } catch { }

                    }

                    Root[] objectTransactions;
                    int maxID = 0;

                    //return all roots found at address
                    objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, 0, -1, versionByte);
                
                    try { maxID = objectTransactions.Max(max => max.Id); } catch { }
                    
                    if (intProcessHeight != 0 && intProcessHeight == maxID)
                    {
                        try { File.Delete(@"GET_INQUIRIES_BY_ADDRESS"); } catch { }

                        if (skip != 0)
                        {
                            //GPT3 SUGGESTED
                            var skippedList = objectStates.Where(state => state.Id >= skip);


                            if (qty == -1) { return skippedList.ToList(); }
                            else { return skippedList.Take(qty).ToList(); }
                        }
                        else
                        {
                            if (qty == -1) { return objectStates.ToList(); }
                            else { return objectStates.Take(qty).ToList(); }

                        }

                    }


                    List<string> addedValues = new List<string>();

                    //Do not process container address as object.
                    addedValues.Add(objectaddress);

                    int rowcount = 0;
                    foreach (Root transaction in objectTransactions)
                    {

                        rowcount++;
                        //ignore any transaction that is not signed
                        if (transaction.Signed && rowcount > intProcessHeight)
                        {

                            // string findId;

                            if (transaction.File.ContainsKey("INQ"))
                            {

                                foreach (string key in transaction.Keyword.Keys.Reverse().Take(2).Skip(1))
                                {

                                    if (!addedValues.Contains(key))
                                    {
                                        addedValues.Add(key);


                                        INQState existingObjectState = objectStates.FirstOrDefault(os => os.URN == key);
                                        if (existingObjectState != null)
                                        {
                                            INQState isObject = GetInquiryByAddress(key, username, password, url, versionByte, calculate);
                                            if (isObject.URN != null)
                                            {
                                                //isObject.Id = transaction.Id;
                                                objectStates[objectStates.IndexOf(existingObjectState)] = isObject;
                                            }
                                        }
                                        else
                                        {
                                            INQState newObject = GetInquiryByAddress(key, username, password, url, versionByte, calculate);
                                            if (newObject.URN != null)
                                            {
                                                //newObject.Id = transaction.Id;
                                                objectStates.Add(newObject);
                                            }
                                        }
                                    }
                                }



                            }
                        }


                    }
                    objectStates.Last().Id = objectTransactions.Count();

                    var objectSerialized = JsonConvert.SerializeObject(objectStates);

                    if (!Directory.Exists(@"root\" + objectaddress))
                    {
                        Directory.CreateDirectory(@"root\" + objectaddress);
                    }
                    System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetInquiriesByAddress.json", objectSerialized);

                    try { File.Delete(@"GET_INQUIRIES_BY_ADDRESS"); } catch { }
                }
                catch { }
                finally { try { File.Delete(@"GET_INQUIRIES_BY_ADDRESS"); } catch { } }
                
                if (skip != 0)
                {
                    //GPT3 SUGGESTED
                    var skippedList = objectStates.Where(state => state.Id >= skip);


                    if (qty == -1) { return skippedList.ToList(); }
                    else { return skippedList.Take(qty).ToList(); }
                }
                else
                {
                    if (qty == -1) { return objectStates.ToList(); }
                    else { return objectStates.Take(qty).ToList(); }

                }

            }
        }

        public static List<INQState> GetInquiriesCreatedByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1, bool calculate = false)
        {

            lock (SupLocker)
            {
                List<INQState> objectStates = new List<INQState> { };
                bool fetched = false;

                if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK")) { return objectStates; }

                string JSONOBJ;
                string diskpath = "root\\" + objectaddress + "\\";


                // fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetInquiriesCreatedByAddress.json");
                    objectStates = JsonConvert.DeserializeObject<List<INQState>>(JSONOBJ);
                    fetched = true;

                }
                catch { }
                if (fetched && objectStates.Count < 1) { return objectStates; }

                List<INQState> cachedObjectStates = INQState.GetInquiriesByAddress(objectaddress, username, password, url, versionByte, 0, -1, calculate);
                if (fetched && objectStates.Last().Id == cachedObjectStates.Last().Id)
                {

                    if (skip != 0)
                    {
                        //GPT3 SUGGESTED
                        var skippedList = objectStates.Where(state => state.Id >= skip);


                        if (qty == -1) { return skippedList.ToList(); }
                        else { return skippedList.Take(qty).ToList(); }
                    }
                    else
                    {
                        if (qty == -1) { return objectStates.ToList(); }
                        else { return objectStates.Take(qty).ToList(); }

                    }

                }

                objectStates = new List<INQState>();

                if (cachedObjectStates.Count() > 0)
                {
                    foreach (INQState objectstate in cachedObjectStates)
                    {
                        if (objectstate.URN != null && objectstate.CreatedBy == objectaddress)
                        {

                            objectStates.Add(objectstate);

                        }

                    }


                    if (objectStates.Count() > 0)
                    {
                        objectStates.Last().Id = cachedObjectStates.Last().Id;
                    }
                }

                var objectSerialized = JsonConvert.SerializeObject(objectStates);

                if (!Directory.Exists(@"root\" + objectaddress))
                {
                    Directory.CreateDirectory(@"root\" + objectaddress);
                }
                System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetInquiriesCreatedByAddress.json", objectSerialized);

                return objectStates;

            }

        }

        public static List<INQState> GetInquiriesByKeyword(List<string> searchstrings, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1, bool calculate = false)
        {
            lock (SupLocker)
            {


                List<INQState> totalSearch = new List<INQState>();

                foreach (string search in searchstrings)
                {

                    string objectaddress = Root.GetPublicAddressByKeyword(search, versionByte);


                    if (!System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK"))
                    {

                        List<INQState> keySearch = GetInquiriesByAddress(objectaddress, username, password, url, versionByte, 0, -1, calculate);

                        totalSearch = totalSearch.Concat(keySearch).ToList();

                    }
                }

                if (qty == -1) { return totalSearch.Skip(skip).ToList(); }
                else
                {
                    return totalSearch.Skip(skip).Take(qty).ToList();
                }
            }

        }
    }

}




