﻿using AngleSharp.Common;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SUP.P2FK
{
    public class OBJ
    {
        public string urn { get; set; }
        public string uri { get; set; }
        public string img { get; set; }
        public string nme { get; set; }
        public string dsc { get; set; }
        public Dictionary<string, string> atr { get; set; }
        public string lic { get; set; }
        public long max { get; set; }
        public string[] cre { get; set; }
        public Dictionary<string, long> own { get; set; }
        public Dictionary<string, decimal> roy { get; set; }

    }

    public class BID
    {
        public string Requestor { get; set; }
        public string Owner { get; set; }
        public long Qty { get; set; }
        public decimal Value { get; set; }
        public DateTime BlockDate { get; set; }
    }

    public class MessageObject
    {
        public string Message { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public DateTime BlockDate { get; set; }
        public string TransactionId { get; set; }
    }


    public class COLState
    {
        public int Id { get; set; }
        public string URN { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, DateTime> Creators { get; set; }
        public Dictionary<string, string> URL { get; set; }
        public Dictionary<string, string> Location { get; set; }
        public DateTime CreatedDate { get; set; }

    }


    public class OBJState
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string URN { get; set; }
        public string URI { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string License { get; set; }
        public long Maximum { get; set; }
        public Dictionary<string, DateTime> Creators { get; set; }
        public Dictionary<string, (long, string)> Owners { get; set; }
        public Dictionary<string, decimal> Royalties { get; set; }
        public List<BID> Offers { get; set; }
        public Dictionary<string, BID> Listings { get; set; }
        public DateTime LockedDate { get; set; }
        public int ProcessHeight { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ChangeDate { get; set; }
        public List<string> ChangeLog { get; set; }
        public bool Verbose { get; set; }


        private readonly static object SupLocker = new object();
        public static OBJState GetObjectByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", bool verbose = false)
        {

            OBJState objectState = new OBJState();
            objectState.ChangeLog = new List<string>();

            try
            {
                bool fetched = false;

                if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK"))
                {

                    return objectState;
                }

                string JSONOBJ;
                string logstatus;
                string diskpath = "root\\" + objectaddress + "\\";


                // fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(diskpath + "OBJ.json");
                    objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
                    fetched = true;

                }
                catch { }
                if (fetched && objectState.URN == null && objectState.ProcessHeight == 0)
                {

                    return objectState;
                }


                if (objectState.URN != null && objectState.ChangeDate.Year.ToString() == "1970")
                {
                    Root unconfimredobj = new Root();
                    try
                    {
                        JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetRootByTransctionId.json");
                        unconfimredobj = JsonConvert.DeserializeObject<Root>(JSONOBJ);

                        return OBJState.GetObjectByTransactionId(unconfimredobj.TransactionId, username, password, url, versionByte);


                    }
                    catch
                    {
                        objectState = new OBJState();
                        objectState.ChangeLog = new List<string>();
                    }

                }

                int intProcessHeight = 0;
                try { intProcessHeight = objectState.Id; } catch { }

                Root[] objectTransactions;

                if (verbose == true) { intProcessHeight = 0; objectState = new OBJState(); objectState.ChangeLog = new List<string>(); }

                lock (SupLocker)
                {
                    objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, -1, versionByte, verbose);


                    if (intProcessHeight != 0 && objectTransactions.Count() == 0)
                    {
                        return objectState;
                    }
                    string[] requiredKeys = { "OBJ", "GIV", "BRN", "BUY", "LST" };

                    foreach (Root transaction in objectTransactions)
                    {
                        if (transaction.Id > intProcessHeight)
                        {
                            intProcessHeight = transaction.Id;


                            if (requiredKeys.Any(key => transaction.File.ContainsKey(key)) && transaction.Signed)
                            {
                                //alright you met minnimum criteria lets inspect this further
                                logstatus = "";
                                objectState.ProcessHeight = intProcessHeight;

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


                                if (sigSeen == null || (verbose && sigSeen == transaction.TransactionId))
                                {

                                    switch (transaction.File.ElementAtOrDefault(1).Key?.ToString())
                                    {
                                        case "OBJ":
                                            OBJ objectinspector = null;
                                            //is this even the right object!?  no!?  goodbye!
                                            if (!transaction.Keyword.ContainsKey(objectaddress))
                                            {
                                                break;
                                            }
                                            try
                                            {
                                                objectinspector = JsonConvert.DeserializeObject<OBJ>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\OBJ"));

                                            }
                                            catch (Exception e)
                                            {
                                                if (verbose)
                                                {
                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"inspect\",\"\",\"\",\"failed due to invalid format\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                    objectState.ChangeLog.Add(logstatus);
                                                    logstatus = "";
                                                }
                                                break;
                                            }



                                            try
                                            {
                                                //builds the creator element
                                                if (objectinspector.urn != null && objectinspector.cre != null && objectState.Creators == null && objectinspector.cre[0] == objectaddress || (objectinspector.urn != null && objectinspector.cre != null && objectState.Creators == null && int.TryParse(objectinspector.cre[0], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int intID) && objectaddress == transaction.Keyword.Reverse().ElementAt(intID).Key))

                                                {


                                                    objectState.Creators = new Dictionary<string, DateTime> { };


                                                    try
                                                    {


                                                        foreach (string keywordId in objectinspector.cre)
                                                        {
                                                            string creator = "";
                                                            if (int.TryParse(keywordId, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int intId))
                                                            {
                                                                creator = transaction.Keyword.Reverse().ElementAt(intId).Key;
                                                            }
                                                            else
                                                            {
                                                                creator = keywordId;


                                                            }

                                                            if (!objectState.Creators.ContainsKey(creator))
                                                            {
                                                                objectState.Creators.Add(creator, new DateTime());
                                                            }

                                                        }




                                                    }
                                                    catch
                                                    {
                                                        break;
                                                    }
                                                    objectState.ChangeDate = transaction.BlockDate;
                                                    objectinspector.cre = null;
                                                }
                                            }
                                            catch { }///allows ack signature confirmation

                                            try
                                            {

                                                //creator grant authorization timestamp if currently null
                                                if (objectState.Creators != null && objectState.Creators.ContainsKey(transaction.SignedBy))
                                                {


                                                    try
                                                    {
                                                        if (objectinspector.cre != null)
                                                        {
                                                            objectState.Creators = objectState.Creators.Take(1).ToDictionary(pair => pair.Key, pair => pair.Value);

                                                            foreach (string keywordId in objectinspector.cre)
                                                            {
                                                                string creator = "";
                                                                if (int.TryParse(keywordId, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int intId))
                                                                {
                                                                    creator = transaction.Keyword.Reverse().ElementAt(intId).Key;
                                                                }
                                                                else
                                                                {
                                                                    creator = keywordId;

                                                                }


                                                                if (!objectState.Creators.ContainsKey(creator))
                                                                {
                                                                    objectState.Creators.Add(creator, new DateTime());
                                                                }



                                                            }



                                                        }
                                                    }
                                                    catch
                                                    {
                                                        break;
                                                    }



                                                    objectState.ChangeDate = transaction.BlockDate;
                                                    objectinspector.cre = null;

                                                    if (objectState.LockedDate.Year == 1)
                                                    {
                                                        if (objectinspector.urn != null)
                                                        {

                                                            //prevents someone from trying to claim a previously sigend etching using a different signature
                                                            if (versionByte != "111" && !objectinspector.urn.ToUpper().StartsWith("IPFS:"))
                                                            {
                                                                var _url = url;
                                                                var _versionByte = versionByte;

                                                                if (objectinspector.urn.ToUpper().StartsWith("MZC:"))
                                                                {
                                                                    _url = @"http://127.0.0.1:12832";
                                                                    _versionByte = "50";

                                                                }
                                                                else if (objectinspector.urn.ToUpper().StartsWith("LTC:"))
                                                                {
                                                                    _url = @"http://127.0.0.1:9332";
                                                                    _versionByte = "48";

                                                                }
                                                                else if (objectinspector.urn.ToUpper().StartsWith("DOG:"))
                                                                {
                                                                    _url = @"http://127.0.0.1:22555";
                                                                    _versionByte = "30";

                                                                }


                                                                Root signedRoot = Root.GetRootByTransactionId(objectinspector.urn.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Substring(0, 64), username, password, _url, _versionByte);

                                                                if (signedRoot.Signed)
                                                                {
                                                                    if (signedRoot.SignedBy != transaction.SignedBy)
                                                                    {
                                                                        if (verbose)
                                                                        {
                                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"create\",\"\",\"\",\"failed due to previous claim\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                            objectState.ChangeLog.Add(logstatus);
                                                                            logstatus = "";
                                                                        }
                                                                        break;
                                                                    }

                                                                }

                                                                //obtain URN creation date if it exists
                                                                if (signedRoot.BlockDate.Year > 1975)
                                                                {
                                                                    objectState.CreatedDate = signedRoot.BlockDate;
                                                                    if (verbose)
                                                                    {
                                                                        logstatus = "[\"" + signedRoot.SignedBy + "\",\"" + objectaddress + "\",\"create\",\"\",\"\",\"success\",\"" + signedRoot.BlockDate.ToString() + "\"]";
                                                                        objectState.ChangeLog.Add(logstatus);

                                                                    }
                                                                }
                                                            }

                                                            objectState.ChangeDate = transaction.BlockDate; objectState.URN = objectinspector.urn.Replace('“', '"').Replace('”', '"');

                                                        }
                                                        if (objectinspector.uri != null) { objectState.ChangeDate = transaction.BlockDate; objectState.URI = objectinspector.uri.Replace('“', '"').Replace('”', '"'); }
                                                        if (objectinspector.img != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Image = objectinspector.img.Replace('“', '"').Replace('”', '"'); }
                                                        if (objectinspector.nme != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Name = objectinspector.nme.Replace('“', '"').Replace('”', '"'); }
                                                        if (objectinspector.dsc != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Description = objectinspector.dsc.Replace('“', '"').Replace('”', '"'); }
                                                        if (objectinspector.atr != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Attributes = objectinspector.atr; }
                                                        if (objectinspector.lic != null) { objectState.ChangeDate = transaction.BlockDate; objectState.License = objectinspector.lic.Replace('“', '"').Replace('”', '"'); }
                                                        if (objectinspector.max != objectState.Maximum) { objectState.ChangeDate = transaction.BlockDate; objectState.Maximum = objectinspector.max; }
                                                        if (objectinspector.own != null)
                                                        {
                                                            if (objectState.Owners == null)
                                                            {
                                                                //if URN creation date cannot be be determined use date claimed
                                                                if (objectState.CreatedDate.Year < 1975)
                                                                {
                                                                    objectState.CreatedDate = transaction.BlockDate;
                                                                }

                                                                objectState.TransactionId = transaction.TransactionId;
                                                                objectState.Owners = new Dictionary<string, (long, string)>();
                                                                objectState.Royalties = new Dictionary<string, decimal>();
                                                                if (verbose)
                                                                {
                                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"claim\",\"" + objectinspector.own.Values.Sum() + "\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                    objectState.ChangeLog.Add(logstatus);
                                                                    logstatus = "";
                                                                }


                                                            }
                                                            else { objectState.Owners = new Dictionary<string, (long, string)>(); }


                                                            foreach (var ownerId in objectinspector.own)
                                                            {
                                                                string owner = "";
                                                                if (int.TryParse(ownerId.Key, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int intId))
                                                                {
                                                                    owner = transaction.Keyword.Reverse().ElementAt(intId).Key;
                                                                }
                                                                else { owner = ownerId.Key; }

                                                                if (!objectState.Owners.ContainsKey(owner))
                                                                {
                                                                    // If the key doesn't exist, add it with the new tuple value
                                                                    objectState.Owners.Add(owner, (ownerId.Value, null)); // Replace "someStringValue" with an appropriate string value.
                                                                }
                                                                else
                                                                {
                                                                    // If the key exists, update the tuple with the new ownerId.Value
                                                                    objectState.Owners[owner] = (ownerId.Value, objectState.Owners[owner].Item2);
                                                                    if (verbose)
                                                                    {
                                                                        logstatus = $"[\"{transaction.SignedBy}\",\"{objectaddress}\",\"update\",\"{ownerId.Value}\",\"\",\"success\",\"{transaction.BlockDate.ToString()}\"]";
                                                                        objectState.ChangeLog.Add(logstatus);
                                                                        logstatus = "";
                                                                    }
                                                                }


                                                            }
                                                        }
                                                        if (objectinspector.roy != null)
                                                        {
                                                            objectState.Royalties = new Dictionary<string, decimal>();

                                                            foreach (var royaltyId in objectinspector.roy)
                                                            {
                                                                string royalty = "";
                                                                if (int.TryParse(royaltyId.Key, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int intId))
                                                                {
                                                                    royalty = transaction.Keyword.Reverse().ElementAt(intId).Key;
                                                                }
                                                                else { royalty = royaltyId.Key; }

                                                                if (!objectState.Royalties.ContainsKey(royalty))
                                                                {
                                                                    objectState.Royalties.Add(royalty, royaltyId.Value);
                                                                }
                                                                else
                                                                {
                                                                    objectState.Royalties[royalty] = royaltyId.Value;
                                                                    if (verbose)
                                                                    {
                                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"update\",\"" + royaltyId.Value + "\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                        objectState.ChangeLog.Add(logstatus);
                                                                        logstatus = "";
                                                                    }
                                                                }

                                                            }
                                                        }

                                                        if (objectState.Creators.TryGet(transaction.SignedBy).Year == 1)
                                                        {
                                                            objectState.Creators[transaction.SignedBy] = transaction.BlockDate;
                                                            objectState.ChangeDate = transaction.BlockDate;
                                                            if (verbose)
                                                            {

                                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"grant\",\"\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                objectState.ChangeLog.Add(logstatus);

                                                            }

                                                        }

                                                        if (intProcessHeight > 0 && objectState.ChangeDate == transaction.BlockDate)
                                                        {
                                                            if (!logstatus.Contains("success"))
                                                            {
                                                                if (verbose)
                                                                {
                                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"update\",\"\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                    objectState.ChangeLog.Add(logstatus);
                                                                    logstatus = "";
                                                                }
                                                            }
                                                            else { logstatus = ""; }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (verbose)
                                                        {
                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"update\",\"\",\"\",\"failed due to object lock\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                            objectState.ChangeLog.Add(logstatus);
                                                            logstatus = "";
                                                        }
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (verbose)
                                                    {
                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"inspect\",\"\",\"\",\"failed due to insufficient privileges\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                        objectState.ChangeLog.Add(logstatus);
                                                        logstatus = "";
                                                    }
                                                }
                                                break;


                                            }
                                            catch
                                            {
                                                if (verbose)
                                                {
                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"create\",\"\",\"\",\"failed due to invalid transaction format\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                    objectState.ChangeLog.Add(logstatus);
                                                    logstatus = "";
                                                }
                                                break;
                                            }



                                        case "GIV":

                                            //is this even the right object!?  no!?  goodbye!
                                            if (!transaction.Keyword.ContainsKey(objectaddress)) { break; }
                                            // no sense checking any further
                                            if (objectState.Owners == null) { break; }
                                            List<List<long>> givinspector = new List<List<long>> { };
                                            try
                                            {
                                                givinspector = JsonConvert.DeserializeObject<List<List<long>>>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\GIV"));
                                            }
                                            catch { break; }

                                            if (givinspector == null) { break; }

                                            foreach (var give in givinspector)
                                            {
                                                long qtyToGive = 0;
                                                string giver = transaction.SignedBy;
                                                string reciever;

                                                try
                                                {
                                                    reciever = transaction.Keyword.Reverse().ElementAt((int)give[0]).Key;

                                                }
                                                catch
                                                {
                                                    if (verbose)
                                                    {
                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"give\",\"\",\"\",\"failed due to invalid keyword count\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                        objectState.ChangeLog.Add(logstatus);
                                                        logstatus = "";
                                                    }
                                                    break;
                                                }


                                                try
                                                {
                                                    qtyToGive = give[1];
                                                }
                                                catch
                                                {
                                                    if (verbose)
                                                    {
                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"give\",\"\",\"\",\"failed due to invalid qty format\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                        objectState.ChangeLog.Add(logstatus);
                                                        logstatus = "";
                                                    }
                                                    break;
                                                }


                                                // cannot give less then 1
                                                if (qtyToGive < 0)
                                                {
                                                    //salt
                                                    break;
                                                }

                                                // GIV Transaction with 0 qty closes all pending offers
                                                if (qtyToGive == 0)
                                                {
                                                    objectState.Offers.RemoveAll(offer => offer.Requestor == reciever && offer.Owner == transaction.SignedBy);

                                                    if (verbose)
                                                    {
                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"close all offers" + qtyToGive.ToString() + "\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                        objectState.ChangeLog.Add(logstatus);

                                                        logstatus = "";
                                                    }
                                                    break;
                                                }


                                                if (objectState.Maximum > 0)
                                                {
                                                    if (qtyToGive > objectState.Maximum)
                                                    {
                                                        qtyToGive = objectState.Maximum;
                                                    }

                                                    if (objectState.Owners.TryGetValue(reciever, out var tuple) && tuple.Item1 + qtyToGive > objectState.Maximum)
                                                    {
                                                        if (verbose)
                                                        {
                                                            logstatus = $"[\"{transaction.SignedBy}\",\"{reciever}\",\"give\",\"{qtyToGive}\",\"\",\"failed due to over maximum qty\",\"{transaction.BlockDate.ToString()}\"]";
                                                            objectState.ChangeLog.Add(logstatus);
                                                            logstatus = "";
                                                        }
                                                        break;
                                                    }

                                                }


                                                // Check if the transaction signer is not on the Owners list
                                                if (!objectState.Owners.TryGetValue(transaction.SignedBy, out var qtyOwnedG))
                                                {
                                                    // Check if the object container is empty
                                                    if (!objectState.Owners.TryGetValue(objectaddress, out var tuple) || tuple.Item1 <= 0)
                                                    {
                                                        if (verbose)
                                                        {
                                                            logstatus = $"[\"{transaction.SignedBy}\",\"{reciever}\",\"give\",\"{qtyToGive}\",\"\",\"failed due to insufficient qty owned\",\"{transaction.BlockDate.ToString()}\"]";

                                                            objectState.ChangeLog.Add(logstatus);

                                                            logstatus = "";
                                                        }
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        // If the transaction is signed by a creator who doesn't own any objects, emulate container
                                                        if (objectState.Creators.ContainsKey(transaction.SignedBy))
                                                        {
                                                            giver = objectaddress;
                                                            qtyOwnedG = tuple; // Assuming the first item in the tuple represents the quantity
                                                        }
                                                    }
                                                }


                                                long qtyListed = 0;
                                                try { if (objectState.Listings != null) { qtyListed = qtyListed + objectState.Listings[giver].Qty; } } catch { }

                                                if (qtyOwnedG.Item1 - qtyListed >= qtyToGive)
                                                {



                                                    //check if Reciever already has a qty if not add a new owner if yes increment
                                                    if (!objectState.Owners.TryGetValue(reciever, out var recieverOwned))
                                                    {

                                                        string genid = "";

                                                        if (giver == objectaddress)
                                                        {
                                                            genid = transaction.TransactionId;
                                                        }
                                                        else
                                                        {
                                                            genid = objectState.Owners[giver].Item2;
                                                        }
                                                        // If the key doesn't exist, add it with the new tuple value
                                                        objectState.Owners.Add(reciever, (qtyToGive, genid));
                                                    }
                                                    else
                                                    {
                                                        string genid = "";

                                                        if (giver == objectaddress)
                                                        {
                                                            genid = transaction.TransactionId;
                                                        }
                                                        else
                                                        {
                                                            genid = objectState.Owners[giver].Item2;
                                                        }
                                                        // If the key exists, update the tuple with the new quantity
                                                        objectState.Owners[reciever] = (recieverOwned.Item1 + qtyToGive, genid);
                                                    }

                                                    // New value to update with
                                                    long newValue = qtyOwnedG.Item1 - qtyToGive;


                                                    // Check if the new value is greater then 0
                                                    if (newValue > 0)
                                                    {
                                                        string genid = "";

                                                        if (giver == objectaddress)
                                                        {
                                                            genid = null;
                                                        }
                                                        else
                                                        {
                                                            genid = objectState.Owners[giver].Item2;
                                                        }
                                                        // Update the value
                                                        objectState.Owners[giver] = (newValue, genid);
                                                    }
                                                    else
                                                    {
                                                        // Remove the dictionary key
                                                        objectState.Owners.Remove(giver);
                                                    }

                                                    //close all currently open offers from reciever
                                                    try { if (objectState.Offers != null) { objectState.Offers.RemoveAll(offer => offer.Requestor == reciever && offer.Owner == giver); } } catch { }

                                                    if (verbose)
                                                    {
                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                        objectState.ChangeLog.Add(logstatus);
                                                        logstatus = "";
                                                    }

                                                    if (objectState.Creators != null && objectState.Creators.ContainsKey(transaction.SignedBy))
                                                    {
                                                        if (objectState.Creators.TryGet(transaction.SignedBy).Year == 1)
                                                        {
                                                            objectState.Creators[transaction.SignedBy] = transaction.BlockDate;
                                                            objectState.ChangeDate = transaction.BlockDate;
                                                            if (verbose)
                                                            {
                                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"grant\",\"\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                objectState.ChangeLog.Add(logstatus);
                                                                logstatus = "";
                                                            }

                                                        }
                                                    }

                                                    if (objectState.LockedDate.Year == 1)
                                                    {

                                                        if (verbose)
                                                        {
                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"lock\",\"\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                            objectState.ChangeLog.Add(logstatus);

                                                            logstatus = "";
                                                        }
                                                        objectState.LockedDate = transaction.BlockDate;
                                                    }
                                                    objectState.ChangeDate = transaction.BlockDate;

                                                }
                                                else
                                                {
                                                    if (verbose)
                                                    { //Invalid trade attempt
                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"failed due to insufficent available qty owned\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                        objectState.ChangeLog.Add(logstatus);
                                                        logstatus = "";
                                                    }
                                                    break;
                                                }



                                            }
                                            break;

                                        case "BRN":
                                            //does this even contain the right object!?  no!?  goodbye!
                                            if (!transaction.Keyword.ContainsKey(objectaddress)) { break; }

                                            if (objectState.Owners == null) { break; }

                                            List<List<long>> brninspector = new List<List<long>> { };

                                            try
                                            {
                                                brninspector = JsonConvert.DeserializeObject<List<List<long>>>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\BRN"));
                                            }
                                            catch { break; }

                                            if (brninspector == null) { break; }

                                            foreach (var burn in brninspector)
                                            {
                                                //is this the right object to burn?
                                                if (transaction.Keyword.Reverse().GetItemByIndex((int)burn[0]).Key != objectaddress) { break; }

                                                string burnr = transaction.SignedBy;
                                                long qtyToBurn = burn[1];

                                                if (qtyToBurn < 1)
                                                {
                                                    //salt
                                                    break;
                                                }


                                                if (objectState.Owners == null) { break; }

                                                // Check if the transaction signer is not on the Owners list
                                                if (!objectState.Owners.TryGetValue(transaction.SignedBy, out var qtyOwnedG))
                                                {
                                                    // Try granting access to the object's self-owned qtyOwned to any creator
                                                    if (!objectState.Owners.TryGetValue(objectaddress, out var tuple) || tuple.Item1 <= 0)
                                                    {
                                                        if (verbose)
                                                        {
                                                            // Add Invalid trade attempt status
                                                            logstatus = $"[\"{transaction.SignedBy}\",\"{objectaddress}\",\"burn\",\"{qtyToBurn}\",\"\",\"failed due to insufficient qty owned\",\"{transaction.BlockDate.ToString()}\"]";

                                                            objectState.ChangeLog.Add(logstatus);

                                                            logstatus = "";
                                                        }
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        // If the transaction signer is a creator, emulate container
                                                        if (objectState.Creators.ContainsKey(transaction.SignedBy))
                                                        {
                                                            burnr = objectaddress;
                                                            qtyOwnedG = tuple; // Assuming the first item in the tuple represents the quantity
                                                        }
                                                    }
                                                }



                                                if (qtyOwnedG.Item1 >= qtyToBurn)
                                                {
                                                    //update owners Dictionary with new values

                                                    // New value to update with
                                                    long newValue = qtyOwnedG.Item1 - qtyToBurn;


                                                    // Check if the new value is an integer
                                                    if (newValue > 0)
                                                    {
                                                        // Update the value
                                                        objectState.Owners[burnr] = (newValue, objectState.Owners[burnr].Item2);
                                                        try
                                                        {
                                                            if (objectState.Listings != null && objectState.Listings.ContainsKey(burnr))
                                                            {
                                                                if (objectState.Listings[burnr].Qty > newValue)
                                                                {
                                                                    objectState.Listings[burnr].Qty = newValue;
                                                                }
                                                            }


                                                        }
                                                        catch { }
                                                    }
                                                    else
                                                    {
                                                        // remove the dictionary key
                                                        objectState.Owners.Remove(burnr);
                                                        try { objectState.Listings.Remove(burnr); } catch { }

                                                        if (objectState.Owners.Count() < 1)
                                                        {
                                                            try
                                                            {
                                                                using (FileStream fs = File.Create(@"root\" + objectaddress + @"\BLOCK"))
                                                                {

                                                                }

                                                                string urnCLAIM = Root.GetPublicAddressByKeyword(objectState.URN, versionByte);


                                                                ///trying various things to remove the urn registratiion
                                                                try { Directory.Delete(@"root/" + urnCLAIM, true); } catch { }
                                                                try { Directory.Delete(@"root/" + burnr, true); } catch { }
                                                                try { Directory.CreateDirectory(@"root\" + objectState.TransactionId); } catch { }

                                                                var rootSerialized = JsonConvert.SerializeObject(new Root()); ;
                                                                System.IO.File.WriteAllText(@"root\" + objectState.TransactionId + @"\" + "ROOTS.json", rootSerialized);

                                                                objectState = new OBJState();
                                                                break;
                                                            }
                                                            catch { }
                                                        }
                                                    }
                                                    if (verbose)
                                                    {
                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"burn\",\"" + qtyToBurn + "\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                        objectState.ChangeLog.Add(logstatus);
                                                        logstatus = "";
                                                    }
                                                    if (objectState.LockedDate.Year == 1)
                                                    {

                                                        if (verbose)
                                                        {
                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"lock\",\"\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                            objectState.ChangeLog.Add(logstatus);

                                                            logstatus = "";
                                                        }
                                                        objectState.LockedDate = transaction.BlockDate;
                                                    }
                                                    objectState.ChangeDate = transaction.BlockDate;

                                                }
                                                else
                                                {
                                                    if (verbose)
                                                    {
                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"burn\",\"" + qtyToBurn + "\",\"\",\"failed due to a insufficent qty owned\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                        objectState.ChangeLog.Add(logstatus);

                                                        logstatus = "";
                                                    }
                                                    break;
                                                }


                                            }
                                            break;

                                        case "BUY":
                                            //BUY command can only be directed at one object at a time for now.
                                            //cannot garuntee the placement of outputs. can occur in possibly two locations.
                                            if (objectaddress != transaction.Output.ElementAt(transaction.Output.Count - 2).Key && objectaddress != transaction.Output.ElementAt(transaction.Output.Count - 3).Key)
                                            {
                                                break;
                                            }

                                            if (objectState.Owners == null) { break; }

                                            if (transaction.SignedBy == objectaddress) { break; }

                                            List<List<string>> buyinspector = new List<List<string>> { };

                                            try
                                            {
                                                buyinspector = JsonConvert.DeserializeObject<List<List<string>>>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\BUY"));
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                            if (buyinspector == null)
                                            {
                                                break;
                                            }
                                            decimal royaltiesPaid = 0;

                                            foreach (var buy in buyinspector)
                                            {


                                                if (long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) < 1)
                                                {
                                                    //salt

                                                    break;
                                                }

                                                if (objectState.Maximum > 0)
                                                {
                                                    if (long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) > objectState.Maximum)
                                                    {
                                                        if (verbose)
                                                        {
                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + buy[0] + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed due to over maximum qty\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                            objectState.ChangeLog.Add(logstatus);
                                                            logstatus = "";
                                                        }
                                                        break;
                                                    }

                                                    if (objectState.Owners.TryGetValue(transaction.SignedBy, out var tuple) && tuple.Item1 + long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) > objectState.Maximum)
                                                    {
                                                        if (verbose)
                                                        {
                                                            logstatus = $"[\"{transaction.SignedBy}\",\"{buy[0]}\",\"buy\",\"{buy[1]}\",\"\",\"failed due to over maximum qty held\",\"{transaction.BlockDate.ToString()}\"]";
                                                            objectState.ChangeLog.Add(logstatus);
                                                            logstatus = "";
                                                        }
                                                        break;
                                                    }
                                                }

                                                // Are their enough listed to buy?
                                                if (objectState.Listings != null && objectState.Listings.TryGetValue(buy[0], out BID qtyListed) && qtyListed.Qty >= long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")))
                                                {

                                                    foreach (KeyValuePair<string, decimal> pair in objectState.Royalties)
                                                    {

                                                        if (pair.Key != transaction.SignedBy)
                                                        {
                                                            string outputSent;
                                                            transaction.Output.TryGetValue(pair.Key, out outputSent);
                                                            decimal logSent = decimal.Parse(outputSent, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"));

                                                            //have required royalties been paid or greator?
                                                            if (logSent >= ((qtyListed.Value * long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"))) * (pair.Value / 100))) { royaltiesPaid = royaltiesPaid + ((qtyListed.Value * long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"))) * (pair.Value / 100)); }
                                                            else
                                                            {
                                                                if (verbose)
                                                                {
                                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + buy[0] + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed " + pair.Key + " " + logSent + " insuficent royalties paid\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                    objectState.ChangeLog.Add(logstatus);

                                                                }
                                                                logstatus = "break";

                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (logstatus == "break") { logstatus = ""; break; }


                                                    //was the current owner paid what was listed with required royalties removed or greator?
                                                    decimal ownerPaid = 0;
                                                    string ownerValue;
                                                    transaction.Output.TryGetValue(buy[0], out ownerValue);
                                                    ownerPaid = decimal.Parse(ownerValue, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"));

                                                    if (ownerPaid >= (qtyListed.Value * long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"))) - royaltiesPaid)
                                                    {

                                                        //remove from listing
                                                        objectState.Listings[buy[0]].Qty = objectState.Listings[buy[0]].Qty - long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));

                                                        //remove from owners
                                                        long quantityToRemove = long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
                                                        string genid = "";

                                                        if (objectState.Owners.TryGetValue(buy[0], out var tuple))
                                                        {

                                                            if (buy[0] == objectaddress)
                                                            {
                                                                genid = transaction.TransactionId;
                                                            }
                                                            else
                                                            {
                                                                genid = tuple.Item2;
                                                            }


                                                            // Subtract the quantityToRemove from the existing value
                                                            var updatedTuple = (tuple.Item1 - quantityToRemove, tuple.Item2);

                                                            // Remove previous owner from the list if the quantity is less than 1

                                                            if (updatedTuple.Item1 < 1)
                                                            {
                                                                objectState.Owners.Remove(buy[0]);
                                                            }
                                                            else
                                                            {
                                                                // Update the value in the dictionary
                                                                objectState.Owners[buy[0]] = updatedTuple;
                                                            }
                                                        }

                                                        // Increment the new owner if already owned
                                                        if (objectState.Owners.ContainsKey(transaction.SignedBy))
                                                        {

                                                            var currentOwnerTuple = objectState.Owners[transaction.SignedBy];

                                                            if (buy[0] == objectaddress)
                                                            {
                                                                genid = transaction.TransactionId;
                                                            }
                                                            else
                                                            {
                                                                genid = tuple.Item2;
                                                            }

                                                            objectState.Owners[transaction.SignedBy] = (currentOwnerTuple.Item1 + long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")), genid);
                                                        }
                                                        // Add the new owner to the list if not currently listed
                                                        else
                                                        {
                                                            if (buy[0] == objectaddress)
                                                            {
                                                                genid = transaction.TransactionId;
                                                            }
                                                            else
                                                            {
                                                                genid = tuple.Item2;
                                                            }
                                                            objectState.Owners.Add(transaction.SignedBy, (long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")), genid));
                                                        }

                                                        //remove previous owner from list if now 0
                                                        if (objectState.Listings[buy[0]].Qty < 1) { objectState.Listings.Remove(buy[0]); }


                                                        if (verbose)
                                                        {
                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + buy[0] + "\",\"buy\",\"" + buy[1] + "\",\"\",\"success " + ownerPaid.ToString() + "\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                            objectState.ChangeLog.Add(logstatus);

                                                            logstatus = "";
                                                        }

                                                        break;



                                                    }
                                                    //conditons were not met log failed event.
                                                    else
                                                    {

                                                        if (verbose)
                                                        {
                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + buy[0] + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed " + ownerPaid.ToString() + " insuficent owner paid\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                            objectState.ChangeLog.Add(logstatus);

                                                            logstatus = "";
                                                        }

                                                        break;
                                                    }




                                                }

                                                //not enough listed check is still valid offer?
                                                else
                                                {

                                                    //attempt buy out if any listings remain
                                                    if (objectState.Listings != null && objectState.Listings.TryGetValue(buy[0], out BID qtyListed2) && long.TryParse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out long buyQty) && buyQty > qtyListed2.Qty)
                                                    {

                                                        buy[1] = qtyListed2.Qty.ToString();


                                                        foreach (KeyValuePair<string, decimal> pair in objectState.Royalties)
                                                        {

                                                            if (pair.Key != transaction.SignedBy)
                                                            {
                                                                string outputSent;
                                                                transaction.Output.TryGetValue(pair.Key, out outputSent);
                                                                decimal logSent = decimal.Parse(outputSent, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"));

                                                                //have required royalties been paid or greator?
                                                                if (logSent >= ((qtyListed2.Value * long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"))) * (pair.Value / 100))) { royaltiesPaid = royaltiesPaid + ((qtyListed2.Value * long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"))) * (pair.Value / 100)); }
                                                                else
                                                                {
                                                                    if (verbose)
                                                                    {
                                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + buy[0] + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed " + pair.Key + " " + logSent + " insuficent royalties paid\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                        objectState.ChangeLog.Add(logstatus);

                                                                    }
                                                                    logstatus = "break";

                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        if (logstatus == "break") { logstatus = ""; break; }


                                                        //was the current owner paid what was listed with required royalties removed or greator?
                                                        decimal ownerPaid = 0;
                                                        string ownerValue;
                                                        transaction.Output.TryGetValue(buy[0], out ownerValue);
                                                        ownerPaid = decimal.Parse(ownerValue, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"));

                                                        if (ownerPaid >= (qtyListed2.Value * long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"))) - royaltiesPaid)
                                                        {

                                                            //remove from listing
                                                            objectState.Listings[buy[0]].Qty = objectState.Listings[buy[0]].Qty - long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));

                                                            //remove from owners
                                                            long quantityToRemove = long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
                                                            string genid = "";

                                                            if (objectState.Owners.TryGetValue(buy[0], out var tuple))
                                                            {

                                                                if (buy[0] == objectaddress)
                                                                {
                                                                    genid = transaction.TransactionId;
                                                                }
                                                                else
                                                                {
                                                                    genid = tuple.Item2;
                                                                }


                                                                // Subtract the quantityToRemove from the existing value
                                                                var updatedTuple = (tuple.Item1 - quantityToRemove, tuple.Item2);

                                                                // Remove previous owner from the list if the quantity is less than 1

                                                                if (updatedTuple.Item1 < 1)
                                                                {
                                                                    objectState.Owners.Remove(buy[0]);
                                                                }
                                                                else
                                                                {
                                                                    // Update the value in the dictionary
                                                                    objectState.Owners[buy[0]] = updatedTuple;
                                                                }
                                                            }

                                                            // Increment the new owner if already owned
                                                            if (objectState.Owners.ContainsKey(transaction.SignedBy))
                                                            {

                                                                var currentOwnerTuple = objectState.Owners[transaction.SignedBy];

                                                                if (buy[0] == objectaddress)
                                                                {
                                                                    genid = transaction.TransactionId;
                                                                }
                                                                else
                                                                {
                                                                    genid = tuple.Item2;
                                                                }

                                                                objectState.Owners[transaction.SignedBy] = (currentOwnerTuple.Item1 + long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")), genid);
                                                            }
                                                            // Add the new owner to the list if not currently listed
                                                            else
                                                            {
                                                                if (buy[0] == objectaddress)
                                                                {
                                                                    genid = transaction.TransactionId;
                                                                }
                                                                else
                                                                {
                                                                    genid = tuple.Item2;
                                                                }
                                                                objectState.Owners.Add(transaction.SignedBy, (long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")), genid));
                                                            }

                                                            //remove previous owner from list if now 0
                                                            if (objectState.Listings[buy[0]].Qty < 1) { objectState.Listings.Remove(buy[0]); }


                                                            if (verbose)
                                                            {
                                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + buy[0] + "\",\"buy\",\"" + buy[1] + "\",\"\",\"success " + ownerPaid.ToString() + "\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                                objectState.ChangeLog.Add(logstatus);

                                                                logstatus = "";
                                                            }

                                                            break;



                                                        }
                                                        //conditons were not met log failed event.
                                                        else
                                                        {

                                                            if (verbose)
                                                            {
                                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + buy[0] + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed " + ownerPaid.ToString() + " insuficent owner paid\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                objectState.ChangeLog.Add(logstatus);

                                                                logstatus = "";
                                                            }

                                                            break;
                                                        }

                                                    }

                                                    else
                                                    {

                                                        // is qty owned enough to fill the Offer?
                                                        if (objectState.Owners.TryGetValue(buy[0], out var tuple) && tuple.Item1 >= long.Parse(buy[1]))
                                                        {


                                                            decimal totalPaid;
                                                            decimal totalRoyaltiesPercent = 0;

                                                            //determine all required royalties have been paid
                                                            foreach (KeyValuePair<string, decimal> pair in objectState.Royalties)
                                                            {
                                                                if (transaction.Output.TryGetValue(pair.Key, out string output) && decimal.TryParse(output, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal sentValue))
                                                                {

                                                                    //royalties not necessary if seller is defined as a royalties recipient
                                                                    if (pair.Key != buy[0])
                                                                    {
                                                                        royaltiesPaid = royaltiesPaid + sentValue;
                                                                        totalRoyaltiesPercent = totalRoyaltiesPercent + pair.Value;
                                                                    }
                                                                }

                                                                //transaction failed due to insufficent royalties paid
                                                                else
                                                                {
                                                                    if (verbose)
                                                                    {
                                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed " + pair.Key + " insuficent royalties paid\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                        objectState.ChangeLog.Add(logstatus);

                                                                    }
                                                                    logstatus = "break";
                                                                    break;
                                                                }
                                                            }

                                                            if (logstatus == "break") { logstatus = ""; break; }


                                                            //has owner been paid some amount?
                                                            if (transaction.Output.TryGetValue(buy[0], out string ownerValue) && decimal.TryParse(ownerValue, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal ownerPaid))
                                                            {
                                                                totalPaid = ownerPaid + royaltiesPaid;


                                                                if (totalPaid * (totalRoyaltiesPercent / 100) == royaltiesPaid)
                                                                {

                                                                    //finalroyalties check
                                                                    foreach (KeyValuePair<string, decimal> pair in objectState.Royalties)
                                                                    {

                                                                        if (pair.Key != buy[0])
                                                                        {
                                                                            //have required royalties been paid or greator?
                                                                            if (transaction.Output.TryGetValue(pair.Key, out string output) && decimal.TryParse(output, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal sentValue) && sentValue >= ((totalPaid * long.Parse(buy[1])) * (pair.Value / 100)))
                                                                            { }

                                                                            //transaction failed insufficent royalties paid - reject
                                                                            else
                                                                            {
                                                                                if (verbose)
                                                                                {
                                                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed due to insuficent royalties paid\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                                                    objectState.ChangeLog.Add(logstatus);

                                                                                }
                                                                                logstatus = "break";


                                                                                break;
                                                                            }
                                                                        }
                                                                    }

                                                                    if (logstatus == "break") { logstatus = ""; break; }


                                                                    // success add valid Offer
                                                                    BID offer = new BID();
                                                                    offer.Requestor = transaction.SignedBy;
                                                                    offer.Owner = buy[0];
                                                                    offer.Value = totalPaid / long.Parse(buy[1]);
                                                                    offer.Qty = long.Parse(buy[1]);
                                                                    offer.BlockDate = transaction.BlockDate;

                                                                    if (objectState.Offers == null)
                                                                    {
                                                                        objectState.Offers = new List<BID>();
                                                                    }

                                                                    objectState.Offers.Add(offer);

                                                                    if (verbose)
                                                                    {
                                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + buy[0] + "\",\"offer\",\"" + buy[1] + "\",\"\",\"success - " + totalPaid / long.Parse(buy[1]) + "\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                                        objectState.ChangeLog.Add(logstatus);

                                                                        logstatus = "";
                                                                    }
                                                                    break;

                                                                }
                                                                //transaction failed insufficent royalties paid - reject
                                                                else
                                                                {

                                                                    if (verbose)
                                                                    {
                                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed due to insuficent royalties paid\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                                        objectState.ChangeLog.Add(logstatus);

                                                                        logstatus = "";
                                                                    }
                                                                    break;

                                                                }
                                                            }
                                                            //transaction failed insuficent owner payment - reject
                                                            else
                                                            {
                                                                if (verbose)
                                                                {
                                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed due to insuficent owner payment\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                                    objectState.ChangeLog.Add(logstatus);

                                                                    logstatus = "";
                                                                }
                                                                break;



                                                            }

                                                        }
                                                        //not enough owned to fill a buy request - reject
                                                        else
                                                        {

                                                            if (verbose)
                                                            {
                                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"buy\",\"" + buy[1] + "\",\"\",\"failed due to insuficent Qty owned\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                                objectState.ChangeLog.Add(logstatus);

                                                                logstatus = "";
                                                            }
                                                            break;
                                                        }

                                                    }



                                                }


                                            }





                                            break;

                                        case "LST":
                                            //is this even the right object!?  no!?  goodbye!
                                            if (!transaction.Keyword.ContainsKey(objectaddress)) { break; }

                                            // no sense checking any further
                                            if (objectState.Owners == null) { break; }

                                            List<List<string>> lstinspector = new List<List<string>> { };
                                            try
                                            {
                                                lstinspector = JsonConvert.DeserializeObject<List<List<string>>>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\LST"));
                                            }
                                            catch
                                            {
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"inspect\",\"\",\"\",\"failed due to invalid transaction format\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                break;
                                            }

                                            if (lstinspector == null)
                                            {
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"list\",\"\",\"\",\"failed due to no data\",\"" + transaction.BlockDate.ToString() + "\"]";
                                                break;
                                            }
                                            foreach (var List in lstinspector)
                                            {
                                                long qtyToList = 0;
                                                string Listr = transaction.SignedBy;
                                                string objectToList;
                                                decimal eachCost = 0;

                                                try
                                                {
                                                    objectToList = List[0];

                                                }
                                                catch
                                                {
                                                    break;
                                                }

                                                if (objectToList == objectaddress)
                                                {

                                                    try
                                                    {
                                                        qtyToList = long.Parse(List[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
                                                    }
                                                    catch
                                                    {
                                                        break;
                                                    }

                                                    try
                                                    {
                                                        eachCost = decimal.Parse(List[2], System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"));
                                                    }
                                                    catch
                                                    {
                                                        break;
                                                    }


                                                    // salt
                                                    if (qtyToList < 0)
                                                    {

                                                        break;
                                                    }

                                                    //listing is an opportunity to acknowledge being a creator
                                                    if (objectState.Creators != null && objectState.Creators.ContainsKey(transaction.SignedBy))
                                                    {
                                                        // update grant date if null and signed by a creator
                                                        if (objectState.Creators.TryGet(transaction.SignedBy).Year == 1)
                                                        {
                                                            objectState.Creators[transaction.SignedBy] = transaction.BlockDate;
                                                            objectState.ChangeDate = transaction.BlockDate;
                                                            if (verbose)
                                                            {

                                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"grant\",\"\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                                objectState.ChangeLog.Add(logstatus);


                                                            }

                                                        }
                                                    }


                                                    // LST Transaction with 0 qty closes all listings
                                                    if (qtyToList == 0)
                                                    {
                                                        try
                                                        {
                                                            if (objectState.Listings != null)
                                                            {
                                                                if (objectState.Listings.ContainsKey(Listr))
                                                                {
                                                                    objectState.Listings.Remove(Listr);
                                                                }
                                                                else
                                                                {
                                                                    if (objectState.Creators.ContainsKey(Listr)) { try { objectState.Listings.Remove(objectaddress); } catch { } }
                                                                }
                                                            }
                                                        }
                                                        catch { }

                                                        if (verbose)
                                                        {
                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectToList + "\",\"List\",\"" + qtyToList.ToString() + "\",\"\",\"close all listings\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                            objectState.ChangeLog.Add(logstatus);

                                                            logstatus = "";
                                                        }
                                                        break;
                                                    }




                                                    long qtyOwnedG = 0;
                                                    // Check if the transaction signer is not on the Owners list
                                                    if (!objectState.Owners.TryGetValue(transaction.SignedBy, out var tuple))
                                                    {
                                                        // Check if the object container is empty
                                                        if (!objectState.Owners.TryGetValue(objectaddress, out var selfOwned))
                                                        {
                                                            if (verbose)
                                                            {
                                                                // Add Invalid trade attempt status
                                                                logstatus = $"[\"{transaction.SignedBy}\",\"{objectToList}\",\"List\",\"{qtyToList}\",\"\",\"failed due to insufficient qty owned\",\"{transaction.BlockDate.ToString()}\"]";

                                                                objectState.ChangeLog.Add(logstatus);

                                                                logstatus = "";
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            // If the transaction is signed by a creator who doesn't own any objects, emulate container
                                                            if (objectState.Creators.ContainsKey(transaction.SignedBy) || transaction.SignedBy == objectaddress)
                                                            {
                                                                Listr = objectaddress;
                                                                qtyOwnedG = selfOwned.Item1; // Assuming the first item in the tuple represents the quantity
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        qtyOwnedG = tuple.Item1;
                                                    }




                                                    if (qtyOwnedG >= qtyToList)
                                                    {
                                                        if (objectState.Listings == null) { objectState.Listings = new Dictionary<string, BID>(); }

                                                        try { objectState.Listings.Remove(Listr); } catch { }

                                                        BID listing = new BID();

                                                        listing.Owner = Listr;
                                                        listing.Requestor = transaction.SignedBy;
                                                        listing.Qty = qtyToList;
                                                        listing.Value = eachCost;
                                                        listing.BlockDate = transaction.BlockDate;
                                                        objectState.Listings.Add(Listr, listing);

                                                        //Lock Object upon successfull Listing
                                                        if (objectState.LockedDate.Year == 1)
                                                        {

                                                            if (verbose)
                                                            {
                                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"lock\",\"\",\"\",\"success\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                                objectState.ChangeLog.Add(logstatus);

                                                                logstatus = "";
                                                            }
                                                            objectState.LockedDate = transaction.BlockDate;
                                                        }



                                                        //force all assoicated collections to update by purging the cache file when listed on secondary
                                                        foreach (string creatorAddress in objectState.Creators.Keys)
                                                        {
                                                            try { System.IO.File.Delete(@"root\" + creatorAddress + @"\" + "GetObjectsByAddress.json"); } catch { }
                                                            try { System.IO.File.Delete(@"root\" + creatorAddress + @"\" + "GetObjectsCreatedByAddress.json"); } catch { }

                                                        }
                                                        if (verbose)
                                                        {
                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectToList + "\",\"List\",\"" + qtyToList + "\",\"" + eachCost + "\",\"Success\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                            objectState.ChangeLog.Add(logstatus);
                                                            logstatus = "";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (verbose)
                                                        { //Invalid list attempt
                                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectToList + "\",\"List\",\"" + qtyToList + "\",\"\",\"failed due to insufficent available qty owned\",\"" + transaction.BlockDate.ToString() + "\"]";

                                                            objectState.ChangeLog.Add(logstatus);
                                                            logstatus = "";
                                                        }
                                                        break;
                                                    }
                                                }


                                            }
                                            break;


                                        default:
                                            // ignore

                                            break;
                                    }

                                }


                            }

                        }
                    }

                    //used to determine where to begin object State processing when retrieved from cache

                    objectState.Id = objectTransactions.Max(state => state.Id);
                    objectState.Verbose = verbose;


                    var objectSerialized = JsonConvert.SerializeObject(objectState);

                    if (!Directory.Exists(@"root\" + objectaddress))
                    {
                        Directory.CreateDirectory(@"root\" + objectaddress);
                    }
                    System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "OBJ.json", objectSerialized);
                }

            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            return objectState;


        }

        public static OBJState GetObjectByTransactionId(string transactionid, string username, string password, string url, string versionByte = "111")
        {

            OBJState objectState = new OBJState();
            OBJ objectinspector = new OBJ();

            var intProcessHeight = 0;
            Root objectTransaction = Root.GetRootByTransactionId(transactionid, username, password, url, versionByte);

            string JSONOBJ;

            string diskpath = "root\\" + transactionid + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "OBJ");
                objectinspector = JsonConvert.DeserializeObject<OBJ>(JSONOBJ);

            }
            catch (Exception ex) { return objectState; }


            if (objectinspector.cre != null && objectState.Creators == null)
            {

                objectState.Creators = new Dictionary<string, DateTime> { };
                foreach (string keywordId in objectinspector.cre)
                {
                    string creator = "";
                    if (int.TryParse(keywordId, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int intId))
                    {
                        creator = objectTransaction.Keyword.Reverse().ElementAt(intId).Key;
                    }
                    else { creator = keywordId; }

                    if (!objectState.Creators.ContainsKey(creator))
                    {
                        objectState.Creators.Add(creator, new DateTime());
                    }

                }

                objectState.ChangeDate = objectTransaction.BlockDate;
                objectinspector.cre = null;
            }


            try
            {
                //has proper authority to make OBJ changes
                if (objectState.Creators.ContainsKey(objectTransaction.SignedBy))
                {

                    if (objectinspector.cre != null && objectState.Creators.TryGet(objectTransaction.SignedBy).Year == 1)
                    {
                        objectState.Creators[objectTransaction.SignedBy] = objectTransaction.BlockDate;
                        objectState.ChangeDate = objectTransaction.BlockDate;


                    }


                    if (objectState.LockedDate.Year == 1)
                    {
                        if (objectinspector.urn != null) { objectState.ChangeDate = objectTransaction.BlockDate; objectState.URN = objectinspector.urn.Replace('“', '"').Replace('”', '"'); }
                        if (objectinspector.uri != null) { objectState.ChangeDate = objectTransaction.BlockDate; objectState.URI = objectinspector.uri.Replace('“', '"').Replace('”', '"'); ; }
                        if (objectinspector.img != null) { objectState.ChangeDate = objectTransaction.BlockDate; objectState.Image = objectinspector.img.Replace('“', '"').Replace('”', '"'); ; }
                        if (objectinspector.nme != null) { objectState.ChangeDate = objectTransaction.BlockDate; objectState.Name = objectinspector.nme.Replace('“', '"').Replace('”', '"'); ; }
                        if (objectinspector.dsc != null) { objectState.ChangeDate = objectTransaction.BlockDate; objectState.Description = objectinspector.dsc.Replace('“', '"').Replace('”', '"'); ; }
                        if (objectinspector.atr != null) { objectState.ChangeDate = objectTransaction.BlockDate; objectState.Attributes = objectinspector.atr; }
                        if (objectinspector.lic != null) { objectState.ChangeDate = objectTransaction.BlockDate; objectState.License = objectinspector.lic.Replace('“', '"').Replace('”', '"'); ; }

                        if (objectinspector.own != null)
                        {
                            if (objectState.Owners == null)
                            {
                                objectState.CreatedDate = objectTransaction.BlockDate;
                                objectState.Owners = new Dictionary<string, (long, string)>();


                            }


                            foreach (var ownerId in objectinspector.own)
                            {
                                string owner = "";

                                if (int.TryParse(ownerId.Key, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int intId))
                                {
                                    owner = objectTransaction.Keyword.Reverse().ElementAt(intId).Key;
                                }
                                else { owner = ownerId.Key; }

                                if (!objectState.Owners.ContainsKey(owner))
                                {
                                    // If the key doesn't exist, add it with the new tuple value
                                    objectState.Owners.Add(owner, (ownerId.Value, null));
                                }
                                else
                                {
                                    // If the key exists, update the tuple with the new ownerId.Value
                                    objectState.Owners[owner] = (ownerId.Value, objectState.Owners[owner].Item2);
                                }

                            }
                        }


                    }

                }




            }
            catch
            {
                return objectState;
            }




            var objectSerialized = JsonConvert.SerializeObject(objectState);


            if (!Directory.Exists(@"root\" + objectTransaction.SignedBy))
            {
                Directory.CreateDirectory(@"root\" + objectTransaction.SignedBy);
            }
            System.IO.File.WriteAllText(@"root\" + objectTransaction.SignedBy + @"\" + "OBJ.json", objectSerialized);


            objectSerialized = JsonConvert.SerializeObject(objectTransaction);

            if (!Directory.Exists(@"root\" + objectTransaction.SignedBy))
            {
                Directory.CreateDirectory(@"root\" + objectTransaction.SignedBy);
            }
            System.IO.File.WriteAllText(@"root\" + objectTransaction.SignedBy + @"\" + "GetRootByTransactionId.json", objectSerialized);

            //used to determine where to begin object State processing when retrieved from cache
            objectState.ProcessHeight = intProcessHeight;
            return objectState;

        }

        public static OBJState GetObjectByURN(string searchstring, string username, string password, string url, string versionByte = "111")
        {

            OBJState objectState = new OBJState();
            if (searchstring == null) { return objectState; }
            string objectaddress = Root.GetPublicAddressByKeyword(searchstring, versionByte);
            if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK")) { return objectState; }

            string JSONOBJ;
            string diskpath = "root\\" + objectaddress + "\\";
            string filepath = "";
            // Generate SHA256 hash of searchstring
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(searchstring.ToLower()));
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                // Use the hash as part of the filename
                string filename = "GetObjectByURN_" + hashString + ".json";
                filepath = Path.Combine(diskpath, filename);

                // Fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(filepath);
                    objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
                }
                catch { }
            }


            if (objectState.URN != null && objectState.ChangeDate.Year.ToString() == "1970") { objectState = new OBJState(); }

            var intProcessHeight = objectState.Id;
            Root[] objectTransactions;


            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, -1, versionByte);
            foreach (Root transaction in objectTransactions)
            {
                if (transaction.Id > intProcessHeight)
                {
                    intProcessHeight = transaction.Id;

                    //ignore any transaction that is not signed
                    if (transaction.Signed && transaction.File.ContainsKey("OBJ") && ((objectState.Creators != null && objectState.Creators.ContainsKey(transaction.SignedBy)) || objectState.Creators == null))
                    {
                        objectState.ProcessHeight = intProcessHeight;
                        string findObject = "";
                        if (transaction.Keyword.Count > 1)
                        {
                            findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 2).Key;
                        }
                        else
                        {
                            findObject = transaction.Keyword.ElementAt(0).Key;
                        }
                        OBJState isObject = GetObjectByAddress(findObject, username, password, url, versionByte);

                        if (isObject.URN != null && isObject.URN == searchstring && isObject.Owners != null && isObject.ChangeDate > DateTime.Now.AddYears(-3))
                        {
                            if (isObject.Creators.ElementAt(0).Key == findObject)
                            {
                                isObject.Id = objectTransactions.Max(max => max.Id);

                                var profileSerialized1 = JsonConvert.SerializeObject(isObject);
                                try
                                {
                                    System.IO.File.WriteAllText(filepath, profileSerialized1);
                                }
                                catch
                                {

                                    try
                                    {
                                        if (!Directory.Exists(@"root\" + objectaddress))

                                        {
                                            Directory.CreateDirectory(@"root\" + objectaddress);
                                        }
                                        System.IO.File.WriteAllText(filepath, profileSerialized1);
                                    }
                                    catch { };
                                }

                                return isObject;

                            }

                        }
                        else
                        {
                            findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 1).Key;
                            isObject = GetObjectByAddress(findObject, username, password, url, versionByte);

                            if (isObject.URN != null && isObject.URN == searchstring && isObject.Owners != null && isObject.ChangeDate > DateTime.Now.AddYears(-3) && isObject.Creators.ElementAt(0).Key == findObject)
                            {
                                isObject.Id = objectTransactions.Max(max => max.Id);

                                var profileSerialized2 = JsonConvert.SerializeObject(isObject);
                                try
                                {
                                    System.IO.File.WriteAllText(filepath, profileSerialized2);
                                }
                                catch
                                {

                                    try
                                    {
                                        if (!Directory.Exists(@"root\" + objectaddress))

                                        {
                                            Directory.CreateDirectory(@"root\" + objectaddress);
                                        }
                                        System.IO.File.WriteAllText(filepath, profileSerialized2);
                                    }
                                    catch { };
                                }

                                return isObject;

                            }

                        }

                    }

                }
                else
                {
                    // return objectState;
                }
            }
            try { objectState.Id = objectTransactions.Max(max => max.Id); } catch { }
            var profileSerialized3 = JsonConvert.SerializeObject(objectState);
            try
            {
                System.IO.File.WriteAllText(filepath, profileSerialized3);
            }
            catch
            {

                try
                {
                    if (!Directory.Exists(@"root\" + objectaddress))

                    {
                        Directory.CreateDirectory(@"root\" + objectaddress);
                    }
                    System.IO.File.WriteAllText(filepath, profileSerialized3);
                }
                catch { };
            }

            return objectState;


        }

        public static OBJState GetObjectByFile(string filepath, string username, string password, string url, string versionByte = "111", int skip = 0)
        {
            OBJState objectState = new OBJState { };
            Root[] objectTransactions;

            // Create payload byte array
            byte[] payload = new byte[21];

            using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
            {
                using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(fileStream);
                    Array.Copy(hash, payload, 20);
                }
            }

            if (versionByte == "111")
            {
                payload[0] = 0x6F; // 0x6F is the hexadecimal representation of 111
            }
            else
            {
                payload[0] = 0x00; // Hexadecimal representation of 0
            }
            string objectaddress = Base58.EncodeWithCheckSum(payload);


            if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK")) { return objectState; }



            int depth = skip;
            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, skip, -1, versionByte);
            foreach (Root transaction in objectTransactions)
            {

                //ignore any transaction that is not signed
                if (transaction.Signed && transaction.File.ContainsKey("OBJ"))
                {
                    byte[] hash1 = new byte[0];
                    byte[] hash2 = new byte[0];
                    string file1 = filepath;
                    string file2 = null;


                    //self signed search first.
                    string findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 1).Key;
                    OBJState isObject = GetObjectByAddress(findObject, username, password, url, versionByte);
                    if (isObject.URN != null && !isObject.URN.ToUpper().StartsWith("HTTP"))
                    {
                        if (isObject.URN.ToUpper().Contains("IPFS:"))
                        {
                            file2 = @"ipfs\" + isObject.URN.Replace("IPFS:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                            string transid = isObject.URN.Substring(5, 46);
                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                process2.Start();
                                process2.WaitForExit();

                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    string fileName = isObject.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "")
                                    {
                                        fileName = "artifact";
                                        file2 += @"\artifact";
                                    }
                                    else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                                }


                                try
                                {
                                    if (File.Exists("IPFS_PINNING_ENABLED"))
                                    {
                                        Process process3 = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = @"ipfs\ipfs.exe",
                                                Arguments = "pin add " + transid,
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                    }
                                }
                                catch { }


                            }

                        }
                        else
                        {

                            string transid = isObject.URN.Replace("MZC:", "").Replace("BTC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("mzc:", "").Replace("btc:", "").Replace("ltc:", "").Replace("dog:", "").Substring(0, 64);
                            switch (isObject.URN.ToUpper().Substring(0, 4))
                            {
                                case "BTC:":
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:8332", "0");
                                    }
                                    break;
                                case "MZC:":
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:12832", "50");
                                    }
                                    break;
                                case "LTC:":
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:9332", "48");
                                    }
                                    break;
                                case "DOG:":
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:22555", "30");
                                    }
                                    break;
                                default:
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root root = Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:18332");
                                    }
                                    break;
                            }

                            file2 = @"root\" + isObject.URN.Replace("MZC:", "").Replace("BTC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("mzc:", "").Replace("btc:", "").Replace("ltc:", "").Replace("dog:", "").Replace(@"/", @"\");
                        }



                        try
                        {
                            using (MD5 md5 = MD5.Create())
                            {
                                using (var stream1 = File.OpenRead(file1))
                                using (var stream2 = File.OpenRead(file2))
                                {
                                    hash1 = md5.ComputeHash(stream1);
                                    hash2 = md5.ComputeHash(stream2);
                                }
                            }
                        }
                        catch { }

                    }


                    if (isObject.URN != null && hash1.Length > 0 && StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2) && isObject.Owners != null && isObject.ChangeDate > DateTime.Now.AddYears(-3))
                    {
                        if (isObject.Creators.ContainsKey(findObject))
                        {
                            isObject.Id = depth;
                            return isObject;
                        }

                    }



                    //creator signed search second.
                    findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 2).Key;
                    isObject = GetObjectByAddress(findObject, username, password, url, versionByte);
                    if (isObject.URN != null && !isObject.URN.ToUpper().StartsWith("HTTP"))
                    {
                        if (isObject.URN.ToUpper().Contains("IPFS:"))
                        {
                            file2 = @"ipfs\" + isObject.URN.Replace("IPFS:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                            string transid = isObject.URN.Substring(5, 46);
                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {

                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                process2.Start();
                                process2.WaitForExit();

                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    string fileName = isObject.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "")
                                    {
                                        fileName = "artifact";
                                        file2 += @"\artifact";
                                    }
                                    else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                                }


                                //attempt to pin fails silently if daemon is not running
                                try
                                {
                                    if (File.Exists("IPFS_PINNING_ENABLED"))
                                    {
                                        Process process3 = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = @"ipfs\ipfs.exe",
                                                Arguments = "pin add " + transid,
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                    }
                                }
                                catch { }



                            }

                        }
                        else
                        {

                            string transid = isObject.URN.Replace("MZC:", "").Replace("BTC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("mzc:", "").Replace("btc:", "").Replace("ltc:", "").Replace("dog:", "").Substring(0, 64);
                            switch (isObject.URN.ToUpper().Substring(0, 4))
                            {
                                case "BTC:":
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:8332", "0");
                                    }
                                    break;
                                case "MZC:":
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:12832", "50");
                                    }
                                    break;
                                case "LTC:":
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:9332", "48");
                                    }
                                    break;
                                case "DOG:":
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:22555", "30");
                                    }
                                    break;
                                default:
                                    if (!System.IO.Directory.Exists("root/" + transid))
                                    {
                                        Root root = Root.GetRootByTransactionId(transid, username, password, @"http://127.0.0.1:18332");
                                    }
                                    break;
                            }

                            file2 = @"root\" + isObject.URN.Replace("MZC:", "").Replace("BTC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("mzc:", "").Replace("btc:", "").Replace("ltc:", "").Replace("dog:", "").Replace(@"/", @"\");
                        }




                        try
                        {
                            using (MD5 md5 = MD5.Create())
                            {
                                using (var stream1 = File.OpenRead(file1))
                                using (var stream2 = File.OpenRead(file2))
                                {
                                    hash1 = md5.ComputeHash(stream1);
                                    hash2 = md5.ComputeHash(stream2);
                                }
                            }
                        }
                        catch { }

                    }


                    if (isObject.URN != null && hash1.Length > 0 && StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2) && isObject.Owners != null && isObject.ChangeDate > DateTime.Now.AddYears(-3))
                    {
                        if (isObject.Creators.ContainsKey(findObject))
                        {
                            isObject.Id = depth;
                            return isObject;
                        }

                    }


                }

                depth++;
            }
            return objectState;

        }

        public static List<OBJState> GetObjectsByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1, bool calculate = false)
        {

            List<OBJState> objectStates = new List<OBJState> { };

            if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK"))
            {

                return objectStates;
            }


            string JSONOBJ;
            string diskpath = "root\\" + objectaddress + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetObjectsByAddress.json");
                objectStates = JsonConvert.DeserializeObject<List<OBJState>>(JSONOBJ);

            }
            catch { }


            int intProcessHeight = 0;

            // this one is a bit different... it cannot use max id as the object will have their own Id.   so it stores the cache height at the last id.
            try { intProcessHeight = objectStates.Last().Id; ; } catch { objectStates = new List<OBJState> { }; }

            if (calculate) { intProcessHeight = 0; }

            Root[] objectTransactions;

            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, -1, versionByte, calculate);


            if (intProcessHeight != 0 && objectTransactions.Count() == 0)
            {


                if (skip != 0)
                {
                    //GPT3 SUGGESTED
                    var skippedList = objectStates.Where(state => state.Id >= skip); ;


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
            HashSet<string> requiredKeys = new HashSet<string> { "OBJ", "GIV", "BRN", "BUY", "LST" };
            HashSet<string> searchallKeys = new HashSet<string> { "GIV", "BRN", "LST" };



            foreach (Root transaction in objectTransactions)
            {
                if (transaction.Id > intProcessHeight)
                {
                    intProcessHeight = transaction.Id;

                    //ignore any transaction that is not signed
                    if (transaction.Signed)
                    {



                        if (requiredKeys.Overlaps(transaction.File.Keys))
                        {

                            //perform a more in depth analysis as all keywords could be objects
                            if (searchallKeys.Overlaps(transaction.File.Keys))
                            {
                                foreach (string key in transaction.Keyword.Keys)
                                {
                                    if (!addedValues.Contains(key))
                                    {
                                        addedValues.Add(key);

                                        OBJState existingObjectState = null;
                                        try { existingObjectState = objectStates.FirstOrDefault(os => os.Creators.First().Key == key); } catch { }

                                        if (existingObjectState != null)
                                        {
                                            OBJState isObject = GetObjectByAddress(key, username, password, url, versionByte, calculate);
                                            if (isObject.URN != null)
                                            {
                                                objectStates[objectStates.IndexOf(existingObjectState)] = isObject;
                                            }
                                        }
                                        else
                                        {
                                            OBJState newObject = GetObjectByAddress(key, username, password, url, versionByte, calculate);
                                            if (newObject.URN != null)
                                            {
                                                objectStates.Add(newObject);
                                            }
                                        }
                                    }
                                }


                            }
                            else
                            {
                                // an object address should appear in the first 3 characters
                                foreach (string key in transaction.Output.Keys.Reverse().Take(3))
                                {

                                    if (!addedValues.Contains(key))
                                    {
                                        addedValues.Add(key);


                                        OBJState existingObjectState = null;

                                        try { existingObjectState = objectStates.FirstOrDefault(os => os.Creators.First().Key == key); } catch { } // MOVE ON

                                        if (existingObjectState != null)
                                        {
                                            OBJState isObject = GetObjectByAddress(key, username, password, url, versionByte, calculate);
                                            if (isObject.URN != null)
                                            {
                                                objectStates[objectStates.IndexOf(existingObjectState)] = isObject;
                                            }
                                        }
                                        else
                                        {
                                            OBJState newObject = GetObjectByAddress(key, username, password, url, versionByte, calculate);
                                            if (newObject.URN != null)
                                            {
                                                objectStates.Add(newObject);
                                            }
                                        }
                                    }
                                }

                            }

                        }
                    }

                }


            }




            try { objectStates.Last().Id = objectTransactions.Max(max => max.Id); } catch { }


            Task.Run(() =>
            {
                if (objectStates.Count > 0)
                {
                    var objectSerialized = JsonConvert.SerializeObject(objectStates);

                    if (!Directory.Exists(@"root\" + objectaddress))
                    {
                        Directory.CreateDirectory(@"root\" + objectaddress);
                    }
                    System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectsByAddress.json", objectSerialized);
                }

            });

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

        public static List<OBJState> GetObjectsOwnedByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {
            List<OBJState> objectStates = new List<OBJState> { };

            if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK")) { return objectStates; }

            string JSONOBJ;
            string diskpath = "root\\" + objectaddress + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetObjectsOwnedByAddress.json");
                objectStates = JsonConvert.DeserializeObject<List<OBJState>>(JSONOBJ);
            }
            catch { }

            List<OBJState> cachedObjectStates = OBJState.GetObjectsByAddress(objectaddress, username, password, url, versionByte, 0, -1);
            try
            {
                if (objectStates.Last().Id == cachedObjectStates.Last().Id)
                {

                    if (qty == -1) { return objectStates.Skip(skip).ToList(); }
                    else { return objectStates.Skip(skip).Take(qty).ToList(); }

                }
            }
            catch { }

            objectStates = new List<OBJState>();
            //return all roots found at address
            foreach (OBJState objectstate in cachedObjectStates)
            {
                if (objectstate.URN != null && objectstate.Owners.ContainsKey(objectaddress))
                {

                    objectStates.Add(objectstate);

                }

            }

            if (cachedObjectStates.Count() > 0)
            {
                try { objectStates.Last().Id = cachedObjectStates.Last().Id; } catch { }
            }

            var objectSerialized = JsonConvert.SerializeObject(objectStates);

            if (!Directory.Exists(@"root\" + objectaddress))
            {
                Directory.CreateDirectory(@"root\" + objectaddress);
            }
            System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectsOwnedByAddress.json", objectSerialized);

            return objectStates;


        }

        public static List<OBJState> GetObjectsCreatedByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {


            List<OBJState> objectStates = new List<OBJState> { };

            if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK")) { return objectStates; }

            string JSONOBJ;
            string diskpath = "root\\" + objectaddress + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetObjectsCreatedByAddress.json");
                objectStates = JsonConvert.DeserializeObject<List<OBJState>>(JSONOBJ);

            }
            catch { }

            List<OBJState> cachedObjectStates = OBJState.GetObjectsByAddress(objectaddress, username, password, url, versionByte, 0, -1);
            try
            {
                if (objectStates.Last().Id == cachedObjectStates.Last().Id)
                {

                    if (qty == -1) { return objectStates.Skip(skip).ToList(); }
                    else { return objectStates.Skip(skip).Take(qty).ToList(); }

                }
            }
            catch { }

            objectStates = new List<OBJState>();

            if (cachedObjectStates.Count() > 0)
            {
                foreach (OBJState objectstate in cachedObjectStates)
                {
                    if (objectstate.URN != null && objectstate.Creators.ContainsKey(objectaddress) && objectstate.Creators[objectaddress] != null && objectstate.Creators[objectaddress].Year > 1975)
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
            System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectsCreatedByAddress.json", objectSerialized);

            return objectStates;



        }

        public static List<COLState> GetObjectCollectionsByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {


            List<COLState> objectStates = new List<COLState> { };
            bool isKeywordSearch = false;

            if (objectaddress.StartsWith("#")) { objectaddress = Root.GetPublicAddressByKeyword(objectaddress.Substring(1), versionByte); isKeywordSearch = true; }

            if (System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK")) { return objectStates; }

            string JSONOBJ;
            string diskpath = "root\\" + objectaddress + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetObjectsCollectionsByAddress.json");
                objectStates = JsonConvert.DeserializeObject<List<COLState>>(JSONOBJ);

            }
            catch { }

            List<OBJState> cachedObjectStates = OBJState.GetObjectsByAddress(objectaddress, username, password, url, versionByte, 0, -1);
            try
            {
                if (objectStates.Last().Id == cachedObjectStates.Last().Id)
                {

                    if (qty == -1) { return objectStates.Skip(skip).ToList(); }
                    else { return objectStates.Skip(skip).Take(qty).ToList(); }

                }
            }
            catch { }

            objectStates = new List<COLState>();
            List<string> addedValues = new List<string>();

            if (cachedObjectStates.Count() > 0)
            {
                foreach (OBJState objectstate in cachedObjectStates)
                {
                    if (isKeywordSearch)
                    {
                        if (objectstate.URN != null && objectstate.Creators.Count() > 1 && objectstate.Creators.ElementAt(1).Value.Year > 1975)
                        {

                            if (!addedValues.Contains(objectstate.Creators.ElementAt(1).Key) && !System.IO.File.Exists(@"root\" + objectstate.Creators.ElementAt(1).Key + @"\BLOCK") && !System.IO.File.Exists(@"root\" + objectstate.Creators.ElementAt(0).Key + @"\BLOCK"))
                            {
                                addedValues.Add(objectstate.Creators.ElementAt(1).Key);
                                COLState colstate = new COLState();
                                colstate.URN = objectstate.Creators.ElementAt(1).Key;

                                PROState activeProfile = PROState.GetProfileByAddress(objectstate.Creators.ElementAt(1).Key, username, password, url, versionByte);

                                if (activeProfile.URN != null)
                                {

                                    colstate.Name = activeProfile.URN;
                                    colstate.Description = activeProfile.Bio;
                                    colstate.Image = activeProfile.Image;
                                    colstate.URL = activeProfile.URL;
                                    colstate.Location = activeProfile.Location;
                                    colstate.CreatedDate = activeProfile.CreatedDate;

                                    if (activeProfile.DisplayName != null)
                                    {
                                        colstate.Name = activeProfile.DisplayName;
                                    }

                                    objectStates.Add(colstate);
                                }


                            }
                        }
                    }
                    else
                    {
                        if (objectstate.URN != null && objectstate.Creators != null && objectstate.Creators.Count > 1 && objectstate.Creators.ContainsKey(objectaddress) && objectstate.Creators[objectaddress].Year > 1975)
                        {

                            if (!addedValues.Contains(objectstate.Creators.ElementAt(1).Key) && !System.IO.File.Exists(@"root\" + objectstate.Creators.ElementAt(1).Key + @"\BLOCK"))
                            {
                                addedValues.Add(objectstate.Creators.ElementAt(1).Key);
                                COLState colstate = new COLState();
                                colstate.URN = objectstate.Creators.ElementAt(1).Key;

                                PROState activeProfile = PROState.GetProfileByAddress(objectstate.Creators.ElementAt(1).Key, username, password, url, versionByte);

                                if (activeProfile.URN != null)
                                {

                                    colstate.Name = activeProfile.URN;
                                    colstate.Description = activeProfile.Bio;
                                    colstate.Image = activeProfile.Image;
                                    colstate.URL = activeProfile.URL;
                                    colstate.Location = activeProfile.Location;
                                    colstate.CreatedDate = activeProfile.CreatedDate;

                                    if (activeProfile.DisplayName != null)
                                    {
                                        colstate.Name = activeProfile.DisplayName;
                                    }

                                    objectStates.Add(colstate);
                                }


                            }
                        }

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
            System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectsCollectionsByAddress.json", objectSerialized);

            return objectStates;



        }

        public static List<OBJState> GetObjectsByKeyword(List<string> searchstrings, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {



            List<OBJState> totalSearch = new List<OBJState>();

            foreach (string search in searchstrings)
            {

                string objectaddress = Root.GetPublicAddressByKeyword(search, versionByte);


                if (!System.IO.File.Exists(@"root\" + objectaddress + @"\BLOCK"))
                {

                    List<OBJState> keySearch = GetObjectsByAddress(objectaddress, username, password, url, versionByte, 0, -1);

                    totalSearch = totalSearch.Concat(keySearch).ToList();

                }
            }

            if (qty == -1) { return totalSearch.Skip(skip).ToList(); }
            else
            {
                return totalSearch.Skip(skip).Take(qty).ToList();
            }


        }

        public static List<OBJState> GetFoundObjects(string username, string password, string url, string versionByte = "111", bool calculate = false)
        {

            List<OBJState> objectStates = new List<OBJState> { };
            string JSONOBJ;
            string diskpath = "root\\found\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetFoundObjects.json");
                objectStates = JsonConvert.DeserializeObject<List<OBJState>>(JSONOBJ);

            }
            catch { }


            if (objectStates.Count() > 0 && !calculate) { return objectStates; }

            objectStates.Clear();

            string[] subDirectories = Directory.GetDirectories(@"root\");

            foreach (string subDirectory in subDirectories)
            {
                string directoryName = new DirectoryInfo(subDirectory).Name;

                if (Regex.IsMatch(directoryName, "^[1-9A-HJ-NP-Za-km-z]{34}$"))
                {
                    OBJState isOBject = OBJState.GetObjectByAddress(directoryName, username, password, url, versionByte, calculate);
                    if (isOBject.URN != null) { objectStates.Add(isOBject); }
                }

            }

            var objectSerialized = JsonConvert.SerializeObject(objectStates);

            if (!Directory.Exists(@"root\found"))
            {
                Directory.CreateDirectory(@"root\found");
            }
            System.IO.File.WriteAllText(@"root\found\GetFoundObjects.json", objectSerialized);

            return objectStates;

        }

        public static List<string> GetKeywordsByAddress(string objectaddress, string username, string password, string url, string versionByte = "111")
        {
            Root[] roots = Root.GetRootsByAddress(objectaddress, username, password, url, 0, -1, versionByte);


            List<string> keywords = new List<string>();

            foreach (Root root in roots)
            {
                foreach (string keyword in root.Keyword.Values)
                {
                    string formattedKeyword = keyword.Replace("#", "").Substring(1);

                    // Check if the formattedKeyword contains only valid UTF-8 characters


                    if (!keywords.Contains(formattedKeyword) && IsStandardASCII(formattedKeyword))
                    {
                        keywords.Add(formattedKeyword);
                    }

                }
            }

            return keywords;

        }

        public static List<MessageObject> GetPublicMessagesByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = 10)
        {
            Root[] AllRoots = Root.GetRootsByAddress(objectaddress, username, password, url, 0, -1, versionByte);

            List<MessageObject> messages = new List<MessageObject>();


            var filteredObjects = AllRoots
                .Where(obj => obj.Message != null && obj.Message.Length > 0)
                .OrderByDescending(obj => obj.Id)
                .Skip(skip)
                .Take(qty);

            // Now, filteredObjects contains objects with at least one message or one file
            foreach (var obj in filteredObjects)
            {


                string fromAddress = obj.SignedBy.ToString();
                string toAddress = "";

                if (obj.Keyword.Keys.Count >= 2)
                {
                    int index = obj.Keyword.Keys.Count - 2;
                    toAddress = obj.Keyword.Keys.ElementAt(index);
                }
                else if (obj.Keyword.Keys.Count == 1)
                {
                    toAddress = obj.Keyword.Keys.First();
                }

                string message = string.Join(" ", obj.Message);
                DateTime tstamp = obj.BlockDate;
                string transactionID = obj.TransactionId.ToString();
                MessageObject messageob = new MessageObject();
                // Add the message data to the messages list

                messageob.FromAddress = fromAddress;
                messageob.ToAddress = toAddress;
                messageob.Message = message;
                messageob.BlockDate = tstamp;
                messageob.TransactionId = transactionID;
                messages.Add(messageob);

            }

            return messages;

        }

        public static List<MessageObject> GetPrivateMessagesByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = 10)
        {
            Root[] AllRoots = Root.GetRootsByAddress(objectaddress, username, password, url, 0, -1, versionByte);

            List<MessageObject> messages = new List<MessageObject>();

            var filteredObjects = AllRoots
                           .Where(obj => obj.File != null && obj.File.Keys.Contains("SEC"))
                           .OrderByDescending(obj => obj.Id)
                           .Skip(skip);
            // Now, filteredObjects contains objects with at least one message or one file
            foreach (var obj in filteredObjects)
            {

                try
                {

                    Root root = Root.GetRootByTransactionId(obj.TransactionId, username, password, url, versionByte);
                    byte[] result = Root.GetRootBytesByFile(new string[] { @"root/" + obj.TransactionId + @"/SEC" });
                    result = Root.DecryptRootBytes(username, password, url, objectaddress, result);

                    root = Root.GetRootByTransactionId(obj.TransactionId, username, password, url, versionByte, result, obj.Keyword.Keys.Last());

                    if (root == null || root.Message == null)
                    {

                        MessageObject messageObject = new MessageObject();
                        messageObject.Message = "<UNABLE TO DECRYPT>";
                        messageObject.FromAddress = obj.Keyword.Keys.Last();
                        messageObject.BlockDate = obj.BlockDate;
                        messageObject.TransactionId = obj.TransactionId;
                        messages.Add(messageObject);

                        if (messages.Count() == qty) { break; }

                    }
                    else
                    {
                        if (root.Signed)
                        {
                            string allMessages = "";
                            foreach (string message in root.Message)
                            {

                                allMessages = allMessages + message;

                            }

                            MessageObject messageObject = new MessageObject();
                            messageObject.Message = allMessages;
                            messageObject.FromAddress = obj.Keyword.Keys.Last();
                            messageObject.BlockDate = obj.BlockDate;
                            messageObject.TransactionId = obj.TransactionId;
                            messages.Add(messageObject);
                            if (messages.Count() == qty) { break; }
                        }
                        else
                        {
                            string allMessages = "";
                            foreach (string message in root.Message)
                            {

                                allMessages = allMessages + message;

                            }

                            MessageObject messageObject = new MessageObject();
                            messageObject.Message = allMessages;
                            messageObject.FromAddress = "";
                            messageObject.BlockDate = obj.BlockDate;
                            messageObject.TransactionId = obj.TransactionId;
                            messages.Add(messageObject);
                            if (messages.Count() == qty) { break; }
                        }
                    }
                }
                catch { }
                if (messages.Count() == qty) { break; }
            }


            return messages;

        }

        private static bool IsStandardASCII(string input)
        {
            foreach (char c in input)
            {
                if (c < 32 || c > 127)
                {
                    return false;
                }
            }
            return true;
        }

    }

}




