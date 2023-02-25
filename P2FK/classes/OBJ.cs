using LevelDB;
using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

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
        public int[] cre { get; set; }
        public Dictionary<int, long> own { get; set; }

    }
    public class OBJState
    {
        public int Id { get; set; }
        public string URN { get; set; }
        public string URI { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string License { get; set; }
        public Dictionary<string, DateTime> Creators { get; set; }
        public Dictionary<string, long> Owners { get; set; }
        public DateTime LockedDate { get; set; }
        public int ProcessHeight { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ChangeDate { get; set; }
        public bool Verbose { get; set; }
        //ensures levelDB is thread safely
        private readonly static object levelDBLocker = new object();
        public static OBJState GetObjectByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", bool verbose = false)
        {

            OBJState objectState = new OBJState();
            var OBJ = new Options { CreateIfMissing = true };
            string isBlocked;
            lock (levelDBLocker)
            {
                using (var db = new DB(OBJ, @"root\block"))
                {
                    isBlocked = db.Get(objectaddress);
                    db.Close();
                }
                if (isBlocked == "true") { return objectState; }

            }
            string JSONOBJ;
            string logstatus;
            string diskpath = "root\\" + objectaddress + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "P2FK.json");
                objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
                verbose = objectState.Verbose;
            }
            catch { }

            var intProcessHeight = objectState.ProcessHeight;
            Root[] objectTransactions;

            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, 300, versionByte);

            if (intProcessHeight > 0 && objectTransactions.Count() == 1) { return objectState; }


            foreach (Root transaction in objectTransactions)
            {

                intProcessHeight = transaction.Id;
                string sortableProcessHeight = intProcessHeight.ToString("X").PadLeft(9, '0');
                logstatus = "";



                //ignore any transaction that is not signed
                if (transaction.Signed && (transaction.File.ContainsKey("OBJ") || transaction.File.ContainsKey("GIV") || transaction.File.ContainsKey("BRN")))
                {

                    string sigSeen;
                    lock (levelDBLocker)
                    {
                        using (var db = new DB(OBJ, @"root\obj"))
                        {
                            sigSeen = db.Get(transaction.Signature);
                        }
                    }

                    if (sigSeen == null || sigSeen == transaction.TransactionId)
                    {

                        lock (levelDBLocker)
                        {
                            using (var db = new DB(OBJ, @"root\obj"))
                            {
                                db.Put(transaction.Signature, transaction.TransactionId);
                            }
                        }

                        switch (transaction.File.Last().Key.ToString().Substring(transaction.File.Last().Key.ToString().Length - 3))
                        {
                            case "OBJ":
                                OBJ objectinspector = null;
                                try
                                {
                                    objectinspector = JsonConvert.DeserializeObject<OBJ>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\OBJ"));

                                }
                                catch (Exception e)
                                {

                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"inspect\",\"\",\"\",\"failed due to invalid format\"]";
                                    break;
                                }


                                foreach (string key in transaction.Keyword.Keys)
                                // Check each byte to see if it's in the ASCII range
                                {
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(OBJ, @"root\obj"))
                                        {
                                            db.Put(objectaddress + "!" + key, transaction.TransactionId);
                                        }
                                    }

                                }



                                if (objectinspector.cre != null && objectState.Creators == null && transaction.SignedBy == objectaddress)
                                {

                                    objectState.Creators = new Dictionary<string, DateTime> { };
                                    try
                                    {
                                        foreach (int keywordId in objectinspector.cre)
                                        {

                                            string creator = transaction.Keyword.Reverse().ElementAt(keywordId).Key;

                                            if (!objectState.Creators.ContainsKey(creator))
                                            {
                                                objectState.Creators.Add(creator, new DateTime());
                                            }

                                        }
                                    }
                                    catch
                                    {
                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"create\",\"\",\"\",\"failed due to invalid transaction format\"]";
                                        break;
                                    }
                                    objectState.ChangeDate = transaction.BlockDate;
                                    objectinspector.cre = null;
                                }


                                try
                                {
                                    //has proper authority to make OBJ changes
                                    if (objectState.Creators.ContainsKey(transaction.SignedBy))
                                    {

                                        if (objectinspector.cre != null && objectState.Creators.TryGet(transaction.SignedBy).Year == 1)
                                        {
                                            objectState.Creators[transaction.SignedBy] = transaction.BlockDate;
                                            objectState.ChangeDate = transaction.BlockDate;
                                            if (verbose)
                                            {

                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"grant\",\"\",\"\",\"success\"]";

                                                lock (levelDBLocker)
                                                {
                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!", logstatus);
                                                    db.Close();
                                                }


                                            }

                                        }



                                        if (objectState.LockedDate.Year == 1)
                                        {
                                            if (objectinspector.urn != null) { objectState.ChangeDate = transaction.BlockDate; objectState.URN = objectinspector.urn.Replace('“', '"').Replace('”', '"'); }
                                            if (objectinspector.uri != null) { objectState.ChangeDate = transaction.BlockDate; objectState.URI = objectinspector.uri.Replace('“', '"').Replace('”', '"'); ; }
                                            if (objectinspector.img != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Image = objectinspector.img.Replace('“', '"').Replace('”', '"'); ; }
                                            if (objectinspector.nme != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Name = objectinspector.nme.Replace('“', '"').Replace('”', '"'); ; }
                                            if (objectinspector.dsc != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Description = objectinspector.dsc.Replace('“', '"').Replace('”', '"'); ; }
                                            if (objectinspector.atr != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Attributes = objectinspector.atr; }
                                            if (objectinspector.lic != null) { objectState.ChangeDate = transaction.BlockDate; objectState.License = objectinspector.lic.Replace('“', '"').Replace('”', '"'); ; }
                                            if (intProcessHeight > 0 && objectState.ChangeDate == transaction.BlockDate)
                                            {
                                                if (!logstatus.Contains("grant"))
                                                {

                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"update\",\"\",\"\",\"success\"]";
                                                }
                                                else { logstatus = ""; }
                                            }
                                            if (objectinspector.own != null)
                                            {
                                                if (objectState.Owners == null)
                                                {
                                                    objectState.CreatedDate = transaction.BlockDate;
                                                    objectState.Owners = new Dictionary<string, long>();
                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"create\",\"" + objectinspector.own.Values.Sum() + "\",\"\",\"success\"]";


                                                    lock (levelDBLocker)
                                                    {
                                                        string found;
                                                        
                                                        using (var db = new DB(OBJ, @"root\found"))
                                                        {
                                                            found = db.Get(objectaddress);
                                                        }

                                                        if (found == null)
                                                        {
                                                            using (var db = new DB(OBJ, @"root\found"))
                                                            {
                                                                db.Put(objectaddress, "1");
                                                            }
                                                        }

                                                    }
                                                }


                                                foreach (var ownerId in objectinspector.own)
                                                {
                                                    string owner = transaction.Keyword.Reverse().ElementAt(ownerId.Key).Key;
                                                    if (!objectState.Owners.ContainsKey(owner))
                                                    {
                                                        objectState.Owners.Add(owner, ownerId.Value);
                                                    }
                                                    else
                                                    {
                                                        objectState.Owners[owner] = ownerId.Value;

                                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"update\",\"" + ownerId.Value + "\",\"\",\"success\"]";
                                                    }
                                                }
                                            }


                                        }
                                        else
                                        {
                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"update\",\"\",\"\",\"failed due to object lock\"]";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"inspect\",\"\",\"\",\"failed due to insufficient privileges\"]";
                                    }
                                    break;



                                }
                                catch
                                {
                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"create\",\"\",\"\",\"failed due to invalid transaction format\"]";

                                    break;
                                }



                            case "GIV":

                                List<List<int>> givinspector = new List<List<int>> { };
                                try
                                {
                                    givinspector = JsonConvert.DeserializeObject<List<List<int>>>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\GIV"));
                                }
                                catch
                                {
                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"inspect\",\"\",\"\",\"failed due to invalid transaction format\"]";

                                    break;
                                }

                                if (givinspector == null)
                                {
                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"give\",\"\",\"\",\"failed due to no data\"]";
                                    break;
                                }
                                int giveCount = 0;
                                foreach (var give in givinspector)
                                {
                                    int qtyToGive = 0;
                                    string giver = transaction.SignedBy;
                                    string reciever;

                                    try
                                    {
                                        reciever = transaction.Keyword.Reverse().ElementAt(give[0]).Key;

                                    }
                                    catch
                                    {
                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"give\",\"\",\"\",\"failed due to invalid keyword count\"]";
                                        break;
                                    }
                                    try
                                    {
                                        qtyToGive = give[1];
                                    }
                                    catch
                                    {
                                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"give\",\"\",\"\",\"failed due to invalid keyword count\"]";
                                        break;
                                    }

                                    giveCount++;
                                    string sortableGiveCount = giveCount.ToString("X").PadLeft(4, '0');



                                    // cannot give less then 1
                                    if (qtyToGive < 1)
                                    {
                                        if (verbose)
                                        {
                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"failed due to a give qty of < 1\"]";

                                            lock (levelDBLocker)
                                            {
                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                db.Close();
                                            }
                                            logstatus = "";
                                        }
                                        break;
                                    }

                                    // no sense checking any further
                                    if (objectState.Owners == null) { break; }


                                    //is transaction signer not on the Owners list
                                    if (!objectState.Owners.TryGetValue(transaction.SignedBy, out long qtyOwnedG))
                                    {
                                        //is the object container empty
                                        if (!objectState.Owners.TryGetValue(objectaddress, out long selfOwned))
                                        {
                                            if (verbose)
                                            {
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"failed due to insufficent qty owned\"]";

                                                lock (levelDBLocker)
                                                {
                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                    db.Close();
                                                }
                                                logstatus = "";
                                            }
                                            break;
                                        }
                                        else
                                        {    //if the transaction is signed by a creator who doesn't own any objects emulate container
                                            if (objectState.Creators.ContainsKey(transaction.SignedBy) || transaction.SignedBy == objectaddress)
                                            {
                                                giver = objectaddress;
                                                qtyOwnedG = selfOwned;
                                            }
                                        }
                                    }


                                    if (qtyOwnedG >= qtyToGive)
                                    {


                                        // New value to update with
                                        long newValue = qtyOwnedG - qtyToGive;


                                        // Check if the new value is an integer
                                        if (newValue > 0)
                                        {
                                            // Update the value
                                            objectState.Owners[giver] = newValue;
                                        }
                                        else
                                        {
                                            // remove the dictionary key
                                            objectState.Owners.Remove(giver);
                                        }


                                        //check if Reciever already has a qty if not add a  new owner if yes increment
                                        if (!objectState.Owners.TryGetValue(reciever, out long recieverOwned))
                                        {
                                            objectState.Owners.Add(reciever, qtyToGive);

                                        }
                                        else { objectState.Owners[reciever] = recieverOwned + qtyToGive; }

                                        if (verbose)
                                        {

                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"success\"]";

                                            lock (levelDBLocker)
                                            {
                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                db.Close();
                                            }
                                            logstatus = "";
                                        }

                                        if (objectState.LockedDate.Year == 1)
                                        {

                                            if (verbose)
                                            {
                                                giveCount++;
                                                sortableGiveCount = giveCount.ToString("X").PadLeft(4, '0');
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"lock\",\"\",\"\",\"success\"]";

                                                lock (levelDBLocker)
                                                {
                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                    db.Close();
                                                }
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
                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"failed due to insufficent qty owned\"]";

                                            lock (levelDBLocker)
                                            {
                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                db.Close();
                                            }
                                            logstatus = "";
                                        }
                                        break;
                                    }



                                }
                                break;

                            case "BRN":

                                List<List<int>> brninspector = new List<List<int>> { };

                                try
                                {
                                    brninspector = JsonConvert.DeserializeObject<List<List<int>>>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\BRN"));
                                }
                                catch
                                {
                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"inspect\",\"\",\"\",\"failed due to invalid transaction format\"]";

                                    break;
                                }
                                if (brninspector == null)
                                {
                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"burn\",\"\",\"\",\"failed due to no data\"]";
                                    break;
                                }
                                int burnCount = 0;
                                foreach (var burn in brninspector)
                                {
                                    string burnr = transaction.SignedBy;
                                    int qtyToBurn = burn[1];
                                    burnCount++;
                                    string sortableBurnCount = burnCount.ToString("X").PadLeft(4, '0');

                                    if (qtyToBurn < 1)
                                    {
                                        if (verbose)
                                        {
                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"burn\",\"" + qtyToBurn + "\",\"\",\"failed due to a burn qty of < 1\"]";

                                            lock (levelDBLocker)
                                            {
                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                db.Close();
                                            }
                                            logstatus = "";
                                        }
                                        break;
                                    }


                                    if (objectState.Owners == null) { break; }

                                    if (!objectState.Owners.TryGetValue(transaction.SignedBy, out long qtyOwnedG))
                                    {
                                        //try grant access to object's self Owned qtyOwned to any creator
                                        if (!objectState.Owners.TryGetValue(objectaddress, out long selfOwned))
                                        {
                                            if (verbose)
                                            {
                                                //Add Invalid trade attempt status
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"burn\",\"" + qtyToBurn + "\",\"\",\"failed due to a insufficent qty owned\"]";

                                                lock (levelDBLocker)
                                                {
                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                    db.Close();
                                                }
                                                logstatus = "";
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            if (objectState.Creators.ContainsKey(transaction.SignedBy))
                                            {
                                                burnr = objectaddress;
                                                qtyOwnedG = selfOwned;
                                            }
                                        }
                                    }


                                    if (qtyOwnedG >= qtyToBurn)
                                    {
                                        //update owners Dictionary with new values

                                        // New value to update with
                                        long newValue = qtyOwnedG - qtyToBurn;


                                        // Check if the new value is an integer
                                        if (newValue > 0)
                                        {
                                            // Update the value
                                            objectState.Owners[burnr] = newValue;
                                        }
                                        else
                                        {
                                            // remove the dictionary key
                                            objectState.Owners.Remove(burnr);
                                        }
                                        if (verbose)
                                        {
                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"burn\",\"" + qtyToBurn + "\",\"\",\"success\"]";

                                            lock (levelDBLocker)
                                            {
                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                db.Close();
                                            }
                                            logstatus = "";
                                        }
                                        if (objectState.LockedDate.Year == 1)
                                        {

                                            if (verbose)
                                            {
                                                burnCount++;
                                                sortableBurnCount = burnCount.ToString("X").PadLeft(4, '0');
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"lock\",\"\",\"\",\"success\"]";

                                                lock (levelDBLocker)
                                                {
                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                    db.Close();
                                                }
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
                                            logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"burn\",\"" + qtyToBurn + "\",\"\",\"failed due to a insufficent qty owned\"]";

                                            lock (levelDBLocker)
                                            {
                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                db.Close();
                                            }
                                            logstatus = "";
                                        }
                                        break;
                                    }


                                }
                                break;

                            default:
                                // ignore

                                break;
                        }
                    }
                    else
                    {
                        logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"burn\",\"\",\"\",\"failed due to duplicate signature\"]";
                    }



                }
                else { logstatus = ""; }

                if (verbose)
                {
                    if (logstatus != "")
                    {


                        lock (levelDBLocker)
                        {
                            var ROOT = new Options { CreateIfMissing = true };
                            var db = new DB(ROOT, @"root\event");
                            db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight, logstatus);
                            db.Close();
                        }


                    }
                }

            }

            //used to determine where to begin object State processing when retrieved from cache
            objectState.ProcessHeight = intProcessHeight;
            objectState.Verbose = verbose;
            var objectSerialized = JsonConvert.SerializeObject(objectState);


            if (!Directory.Exists(@"root\" + objectaddress))
            {
                Directory.CreateDirectory(@"root\" + objectaddress);
            }
            System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "P2FK.json", objectSerialized);


            return objectState;

        }
        //required for in-memory object discovery and creation. get object by address information is not available until after confirmation... added to workbench for testing
        public static OBJState GetObjectByTransactionId(string transactionid, string username, string password, string url, string versionByte = "111", bool verbose = false)
        {

            OBJState objectState = new OBJState();
            var OBJ = new Options { CreateIfMissing = true };
            string isBlocked;
            lock (levelDBLocker)
            {
                using (var db = new DB(OBJ, @"root\block"))
                {
                    isBlocked = db.Get(transactionid);
                }
                if (isBlocked == "true") { return objectState; }
            }
            string JSONOBJ;
            string logstatus;
            string diskpath = "root\\" + transactionid + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "OBJ");
                objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
                verbose = objectState.Verbose;
            }
            catch { }

            var intProcessHeight = 0;
            Root objectTransaction;

            //return all roots found at address
            objectTransaction = Root.GetRootByTransactionId(transactionid, username, password, url, versionByte);




            string sortableProcessHeight = intProcessHeight.ToString("X").PadLeft(9, '0');
            logstatus = "";



            //ignore any transaction that is not signed
            if (objectTransaction.Signed && (objectTransaction.File.ContainsKey("OBJ")))
            {

                string sigSeen;
                lock (levelDBLocker)
                {
                    using (var db = new DB(OBJ, @"root\obj"))
                    {
                        sigSeen = db.Get(objectTransaction.Signature);
                    }
                }

                if (sigSeen == null || sigSeen == objectTransaction.TransactionId)
                {

                    lock (levelDBLocker)
                    {
                        using (var db = new DB(OBJ, @"root\obj"))
                        {
                            db.Put(objectTransaction.Signature, objectTransaction.TransactionId);
                        }
                    }

                    switch (objectTransaction.File.Last().Key.ToString().Substring(objectTransaction.File.Last().Key.ToString().Length - 3))
                    {
                        case "OBJ":
                            OBJ objectinspector = null;
                            try
                            {
                                objectinspector = JsonConvert.DeserializeObject<OBJ>(File.ReadAllText(@"root\" + objectTransaction.TransactionId + @"\OBJ"));

                            }
                            catch (Exception e)
                            {

                                logstatus = "[\"" + objectTransaction.SignedBy + "\",\"" + objectTransaction.SignedBy + "\",\"inspect\",\"\",\"\",\"failed due to invalid format\"]";
                                break;
                            }


                            foreach (string key in objectTransaction.Keyword.Keys)

                            {
                                lock (levelDBLocker)
                                {
                                    using (var db = new DB(OBJ, @"root\obj"))
                                    {
                                        db.Put(objectTransaction.SignedBy + "!" + key, objectTransaction.TransactionId);
                                    }
                                }

                            }



                            if (objectinspector.cre != null && objectState.Creators == null)
                            {

                                objectState.Creators = new Dictionary<string, DateTime> { };
                                foreach (int keywordId in objectinspector.cre)
                                {

                                    string creator = objectTransaction.Keyword.Reverse().ElementAt(keywordId).Key;

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
                                        if (verbose)
                                        {

                                            logstatus = "[\"" + objectTransaction.SignedBy + "\",\"" + objectTransaction.SignedBy + "\",\"grant\",\"\",\"\",\"success\"]";

                                            lock (levelDBLocker)
                                            {
                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectTransaction.SignedBy + "!" + objectTransaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!", logstatus);
                                                db.Close();
                                            }


                                        }

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
                                        if (intProcessHeight > 0 && objectState.ChangeDate == objectTransaction.BlockDate)
                                        {
                                            if (!logstatus.Contains("grant"))
                                            {

                                                logstatus = "[\"" + objectTransaction.SignedBy + "\",\"" + objectTransaction.SignedBy + "\",\"update\",\"\",\"\",\"success\"]";
                                            }
                                            else { logstatus = ""; }
                                        }
                                        if (objectinspector.own != null)
                                        {
                                            if (objectState.Owners == null)
                                            {
                                                objectState.CreatedDate = objectTransaction.BlockDate;
                                                objectState.Owners = new Dictionary<string, long>();
                                                logstatus = "[\"" + objectTransaction.SignedBy + "\",\"" + objectTransaction.SignedBy + "\",\"create\",\"" + objectinspector.own.Values.Sum() + "\",\"\",\"success\"]";
                                                lock (levelDBLocker)
                                                {
                                                    using (var db = new DB(OBJ, @"root\found"))
                                                    {
                                                        db.Put(objectTransaction.SignedBy, "1");
                                                    }
                                                }

                                            }


                                            foreach (var ownerId in objectinspector.own)
                                            {
                                                string owner = objectTransaction.Keyword.Reverse().ElementAt(ownerId.Key).Key;
                                                if (!objectState.Owners.ContainsKey(owner))
                                                {
                                                    objectState.Owners.Add(owner, ownerId.Value);
                                                }
                                                else
                                                {
                                                    objectState.Owners[owner] = ownerId.Value;

                                                    logstatus = "[\"" + objectTransaction.SignedBy + "\",\"" + objectTransaction.SignedBy + "\",\"update\",\"" + ownerId.Value + "\",\"\",\"success\"]";
                                                }
                                            }
                                        }


                                    }
                                    else
                                    {
                                        logstatus = "[\"" + objectTransaction.SignedBy + "\",\"" + objectTransaction.SignedBy + "\",\"update\",\"\",\"\",\"failed due to object lock\"]";
                                        break;
                                    }
                                }
                                else
                                {
                                    logstatus = "[\"" + objectTransaction.SignedBy + "\",\"" + objectTransaction.SignedBy + "\",\"inspect\",\"\",\"\",\"failed due to insufficient privileges\"]";
                                }
                                break;



                            }
                            catch
                            {
                                logstatus = "[\"" + objectTransaction.SignedBy + "\",\"" + objectTransaction.SignedBy + "\",\"create\",\"\",\"\",\"failed due to invalid transaction format\"]";

                                break;
                            }


                        default:
                            // ignore

                            break;
                    }
                }
                else
                {
                    logstatus = "[\"" + objectTransaction.SignedBy + "\",\"" + objectTransaction.SignedBy + "\",\"burn\",\"\",\"\",\"failed due to duplicate signature\"]";
                }



            }
            else { logstatus = ""; }

            if (verbose)
            {
                if (logstatus != "")
                {


                    lock (levelDBLocker)
                    {
                        var ROOT = new Options { CreateIfMissing = true };
                        var db = new DB(ROOT, @"root\event");
                        db.Put(objectTransaction.SignedBy + "!" + objectTransaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight, logstatus);
                        db.Close();
                    }


                }
            }



            //used to determine where to begin object State processing when retrieved from cache
            objectState.ProcessHeight = intProcessHeight;
            objectState.Verbose = verbose;
            var objectSerialized = JsonConvert.SerializeObject(objectState);

            if (objectTransaction.SignedBy != null)
            {
                if (!Directory.Exists(@"root\" + objectTransaction.SignedBy))
                {
                    Directory.CreateDirectory(@"root\" + objectTransaction.SignedBy);
                }
                System.IO.File.WriteAllText(@"root\" + objectTransaction.SignedBy + @"\" + "P2FK.json", objectSerialized);
            }

            return objectState;

        }
        public static OBJState GetObjectByURN(string searchstring, string username, string password, string url, string versionByte = "111", int skip = 0)
        {
            OBJState objectState = new OBJState { };
            Root[] objectTransactions;
            string objectaddress = Root.GetPublicAddressByKeyword(searchstring, versionByte);
            var OBJ = new Options { CreateIfMissing = true };

            string isBlocked;
            lock (levelDBLocker)
            {

                using (var db = new DB(OBJ, @"root\block"))
                {
                    isBlocked = db.Get(objectaddress);
                    db.Close();
                }
                if (isBlocked == "true") { return objectState; }

            }
            int depth = skip;
            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, skip, 300, versionByte);
            foreach (Root transaction in objectTransactions)
            {


                //ignore any transaction that is not signed
                if (transaction.Signed && transaction.File.ContainsKey("OBJ"))
                {
                    string findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 1).Key;
                    OBJState isObject = GetObjectByAddress(findObject, username, password, url, versionByte);

                    if (isObject.URN != null && isObject.URN == searchstring && isObject.Owners != null && isObject.ChangeDate > DateTime.Now.AddYears(-3))
                    {
                        if (isObject.Creators.ElementAt(0).Key == findObject)
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
        public static OBJState GetObjectByFile(string filepath, string username, string password, string url, string versionByte = "111", int skip = 0)
        {
            OBJState objectState = new OBJState { };
            Root[] objectTransactions;

            byte[] payload = new byte[21];

            using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
            {
                fileStream.Read(payload, 1, 20);
            }


            payload[0] = Byte.Parse("111");
            string objectaddress = Base58.EncodeWithCheckSum(payload);

            var OBJ = new Options { CreateIfMissing = true };

            string isBlocked;
            lock (levelDBLocker)
            {

                using (var db = new DB(OBJ, @"root\block"))
                {
                    isBlocked = db.Get(objectaddress);
                    db.Close();
                }
                if (isBlocked == "true") { return objectState; }

            }
            int depth = skip;
            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, skip, 300, versionByte);
            foreach (Root transaction in objectTransactions)
            {


                //ignore any transaction that is not signed
                if (transaction.Signed && transaction.File.ContainsKey("OBJ"))
                {
                    byte[] hash1 = new byte[0];
                    byte[] hash2 = new byte[0];
                    string file1 = filepath;
                    string file2 = null;
                    string findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 1).Key;
                    OBJState isObject = GetObjectByAddress(findObject, username, password, url, versionByte);
                    if (isObject.URN != null)
                    {
                        if (isObject.URN.Contains("IPFS:"))
                        {
                            file2 = @"ipfs\" + isObject.URN.Replace("IPFS:", "");

                            string transid = isObject.URN.Substring(5, 46);
                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {

                                var SUP = new Options { CreateIfMissing = true };
                                string isLoading;
                                lock (levelDBLocker)
                                {
                                    using (var db = new DB(SUP, @"ipfs"))
                                    {
                                        isLoading = db.Get(transid);

                                    }
                                }

                                if (isLoading != "loading")
                                {
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(SUP, @"ipfs"))
                                        {

                                            db.Put(transid, "loading");

                                        }
                                    }
                                    Task ipfsTask = Task.Run(() =>
                                    {
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                        process2.StartInfo.UseShellExecute = false;
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
                                        lock (levelDBLocker)
                                        {
                                            using (var db = new DB(SUP, @"ipfs"))
                                            {
                                                db.Delete(transid);

                                            }
                                        }
                                    });
                                }


                            }

                        }
                        else
                        {
                            Root.GetRootByTransactionId(isObject.URN.Replace("MZC:", "").Replace("BTC:", "").Substring(0, 64), username, password, url, versionByte);
                            file2 = @"root\" + isObject.URN.Replace("MZC:", "").Replace("BTC:", "").Replace(@"/", @"\");
                        }




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


                    if (isObject.URN != null && StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2) && isObject.Owners != null && isObject.ChangeDate > DateTime.Now.AddYears(-3))
                    {
                        if (isObject.Creators.ElementAt(0).Key == findObject)
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
        public static List<OBJState> GetObjectsByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {

            List<OBJState> objectStates = new List<OBJState> { };

            var OBJ = new Options { CreateIfMissing = true };
            string isBlocked;
            lock (levelDBLocker)
            {

                using (var db = new DB(OBJ, @"root\block"))
                {
                    isBlocked = db.Get(objectaddress);
                    db.Close();
                }
                if (isBlocked == "true") { return objectStates; }

            }
            Root[] objectTransactions;

            int _qty = 0;
            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, skip, 300, versionByte);
            HashSet<string> addedValues = new HashSet<string>();
            foreach (Root transaction in objectTransactions)
            {


                //ignore any transaction that is not signed
                if (transaction.Signed)
                {

                    string findId;

                    if (transaction.File.ContainsKey("OBJ") || transaction.File.ContainsKey("GIV") || transaction.File.ContainsKey("MSG"))
                    {



                        if (transaction.File.ContainsKey("GIV"))
                        {


                            foreach (string key in transaction.Keyword.Keys)
                            {

                                if (!addedValues.Contains(key))
                                {
                                    addedValues.Add(key);

                                    OBJState isObject = GetObjectByAddress(key, username, password, url, versionByte);

                                    if (isObject.URN != null && key != objectaddress)
                                    {
                                        lock (levelDBLocker)
                                        {
                                            using (var db = new DB(OBJ, @"root\obj"))
                                            {
                                                findId = db.Get(objectaddress + "!" + key);
                                            }
                                        }

                                        if (findId == transaction.Id.ToString() || findId == null)
                                        {
                                            if (findId == null)
                                            {
                                                lock (levelDBLocker)
                                                {
                                                    using (var db = new DB(OBJ, @"root\obj"))
                                                    {
                                                        db.Put(objectaddress + "!" + key, transaction.Id.ToString());
                                                    }
                                                }
                                            }
                                        }

                                        isObject.Id = transaction.Id;
                                        objectStates.Add(isObject);
                                        _qty++;
                                        if (_qty == qty)
                                        {
                                            return objectStates;
                                        }
                                    }
                                }
                            }
                        }
                        else if (transaction.File.ContainsKey("OBJ") || transaction.File.ContainsKey("MSG"))
                        {
                            string key;
                            key = transaction.Keyword.Last().Key;

                            if (!addedValues.Contains(key))
                            {
                                addedValues.Add(key);

                                OBJState isObject = GetObjectByAddress(key, username, password, url, versionByte);

                                if (isObject.URN != null && key != objectaddress)
                                {
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(OBJ, @"root\obj"))
                                        {
                                            findId = db.Get(objectaddress + "!" + key);
                                        }
                                    }

                                    if (findId == transaction.Id.ToString() || findId == null)
                                    {
                                        if (findId == null)
                                        {
                                            lock (levelDBLocker)
                                            {
                                                using (var db = new DB(OBJ, @"root\obj"))
                                                {
                                                    db.Put(objectaddress + "!" + key, transaction.Id.ToString());
                                                }
                                            }
                                        }
                                    }

                                    isObject.Id = transaction.Id;
                                    objectStates.Add(isObject);
                                    _qty++;
                                    if (_qty == qty)
                                    {
                                        return objectStates;
                                    }
                                }


                            }
                        }

                    }
                }


            }

            return objectStates;


        }
        public static List<OBJState> GetObjectsOwnedByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {
            List<OBJState> objectStates = new List<OBJState> { };

            var OBJ = new Options { CreateIfMissing = true };
            string isBlocked;
            lock (levelDBLocker)
            {

                using (var db = new DB(OBJ, @"root\block"))
                {
                    isBlocked = db.Get(objectaddress);
                    db.Close();
                }
                if (isBlocked == "true") { return objectStates; }

            }
            Root[] objectTransactions;

            int _qty = 0;
            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, skip, 300, versionByte);
            HashSet<string> addedValues = new HashSet<string>();
            foreach (Root transaction in objectTransactions)
            {


                //ignore any transaction that is not signed
                if (transaction.Signed)
                {

                    string findId;

                    if (transaction.File.ContainsKey("OBJ") || transaction.File.ContainsKey("GIV"))
                    {



                        if (transaction.File.ContainsKey("GIV"))
                        {


                            foreach (string key in transaction.Keyword.Keys)
                            {

                                if (!addedValues.Contains(key))
                                {
                                    addedValues.Add(key);

                                    OBJState isObject = GetObjectByAddress(key, username, password, url, versionByte);

                                    //if (isObject.URN != null && key != objectaddress)
                                    if (isObject.URN != null && key != objectaddress && isObject.Owners.ContainsKey(objectaddress) || (isObject.URN != null && key != objectaddress && isObject.Owners.ContainsKey(key) && isObject.Creators.ContainsKey(objectaddress)))
                                    {
                                        lock (levelDBLocker)
                                        {
                                            using (var db = new DB(OBJ, @"root\obj"))
                                            {
                                                findId = db.Get(objectaddress + "!" + key);
                                            }
                                        }

                                        if (findId == transaction.Id.ToString() || findId == null)
                                        {
                                            if (findId == null)
                                            {
                                                lock (levelDBLocker)
                                                {
                                                    using (var db = new DB(OBJ, @"root\obj"))
                                                    {
                                                        db.Put(objectaddress + "!" + key, transaction.Id.ToString());
                                                    }
                                                }
                                            }
                                        }

                                        isObject.Id = transaction.Id;
                                        objectStates.Add(isObject);
                                        _qty++;
                                        if (_qty == qty)
                                        {
                                            return objectStates;
                                        }
                                    }
                                }
                            }
                        }
                        else if (transaction.File.ContainsKey("OBJ"))
                        {
                            string key = transaction.Keyword.Last().Key;

                            if (!addedValues.Contains(key))
                            {
                                addedValues.Add(key);

                                OBJState isObject = GetObjectByAddress(key, username, password, url, versionByte);

                                if (isObject.URN != null && key != objectaddress && isObject.Owners.ContainsKey(objectaddress) || (isObject.URN != null && key != objectaddress && isObject.Owners.ContainsKey(key) && isObject.Creators.ContainsKey(objectaddress)))
                                {
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(OBJ, @"root\obj"))
                                        {
                                            findId = db.Get(objectaddress + "!" + key);
                                        }
                                    }

                                    if (findId == transaction.Id.ToString() || findId == null)
                                    {
                                        if (findId == null)
                                        {
                                            lock (levelDBLocker)
                                            {
                                                using (var db = new DB(OBJ, @"root\obj"))
                                                {
                                                    db.Put(objectaddress + "!" + key, transaction.Id.ToString());
                                                }
                                            }
                                        }
                                    }

                                    isObject.Id = transaction.Id;
                                    objectStates.Add(isObject);
                                    _qty++;
                                    if (_qty == qty)
                                    {
                                        return objectStates;
                                    }
                                }


                            }
                        }

                    }
                }


            }

            return objectStates;


        }
        public static List<OBJState> GetObjectsCreatedByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {

            List<OBJState> objectStates = new List<OBJState> { };

            var OBJ = new Options { CreateIfMissing = true };
            string isBlocked;
            lock (levelDBLocker)
            {

                using (var db = new DB(OBJ, @"root\block"))
                {
                    isBlocked = db.Get(objectaddress);
                    db.Close();
                }
                if (isBlocked == "true") { return objectStates; }

            }
            Root[] objectTransactions;

            int _qty = 0;
            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, skip, 300, versionByte);
            HashSet<string> addedValues = new HashSet<string>();
            foreach (Root transaction in objectTransactions)
            {


                //ignore any transaction that is not signed
                if (transaction.Signed)
                {
                    string findObject;
                    string findId;

                    if (transaction.File.ContainsKey("OBJ"))
                    {
                        findObject = transaction.Keyword.Last().Key;

                        if (!addedValues.Contains(findObject))
                        {
                            addedValues.Add(findObject);


                            OBJState isObject = GetObjectByAddress(findObject, username, password, url, versionByte);

                            if (isObject.URN != null && findObject != objectaddress && isObject.Creators.TryGet(objectaddress).Year > 1)
                            {
                                lock (levelDBLocker)
                                {
                                    using (var db = new DB(OBJ, @"root\obj"))
                                    {
                                        findId = db.Get(objectaddress + "!" + findObject);
                                    }
                                }

                                if (findId == transaction.Id.ToString() || findId == null)
                                {
                                    if (findId == null)
                                    {
                                        lock (levelDBLocker)
                                        {
                                            using (var db = new DB(OBJ, @"root\obj"))
                                            {
                                                db.Put(objectaddress + "!" + findObject, transaction.Id.ToString());
                                            }
                                        }
                                    }
                                }

                                isObject.Id = transaction.Id;
                                objectStates.Add(isObject);
                                _qty++;
                                if (_qty == qty)
                                {
                                    return objectStates;
                                }
                            }

                        }
                    }
                }


            }

            return objectStates;



        }
        public static List<OBJState> GetObjectsByKeyword(List<string> searchstrings, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {
            List<OBJState> totalSearch = new List<OBJState>();

            foreach (string search in searchstrings)
            {
                string objectaddress = Root.GetPublicAddressByKeyword(search, versionByte);
                List<OBJState> keySearch = GetObjectsByAddress(objectaddress, username, password, url, versionByte, skip, qty);
                totalSearch = totalSearch.Concat(keySearch).ToList();
            }
            return totalSearch;

        }
        public static List<OBJState> GetFoundObjects(string username, string password, string url, string versionByte = "111", int skip = 0, int qty = 100)
        {
            List<OBJState> totalSearch = new List<OBJState>();
            HashSet<string> addedValues = new HashSet<string>();
            int rownum = 0;


            lock (levelDBLocker)
            {
                var SUP = new Options { CreateIfMissing = true };

                using (var db = new DB(SUP, @"root\found"))
                {
                    LevelDB.Iterator it = db.CreateIterator();
                    for (
                       it.SeekToLast();
                       it.IsValid() && rownum < skip + qty; // Only display next 10 messages
                        it.Prev()
                     )
                    {
                        // Display only if rownum > numMessagesDisplayed to skip already displayed messages
                        if (rownum >= skip)
                        {

                            if (!addedValues.Contains(it.KeyAsString()))
                            {

                                addedValues.Add(it.KeyAsString());
                                OBJState isObject = GetObjectByAddress(it.KeyAsString(), username, password, url, versionByte);
                                totalSearch.Add(isObject);

                            }

                        }
                        rownum++;
                    }
                    it.Dispose();


                }

            }


            return totalSearch;

        }
        public static List<string> GetKeywordsByAddress(string objectaddress, string username, string password, string url, string versionByte = "111")
        {

            GetObjectByAddress(objectaddress, username, password, url, versionByte);

            List<string> keywords = new List<string>();

            var KEY = new Options { CreateIfMissing = true };
            lock (levelDBLocker)
            {
                using (var db = new DB(KEY, @"root\obj"))
                {
                    LevelDB.Iterator it = db.CreateIterator();
                    for (
                       it.Seek(objectaddress);
                       it.IsValid() && it.KeyAsString().StartsWith(objectaddress + "!");  // && rownum <= numMessagesDisplayed + 10; // Only display next 10 messages
                        it.Next()
                     )
                    {
                        string keyaddress = it.KeyAsString().Substring(it.KeyAsString().IndexOf('!') + 1);
                        Base58.DecodeWithCheckSum(keyaddress, out byte[] payloadBytes);
                        keyaddress = IsAsciiText(payloadBytes);

                        if (keyaddress != null)
                        {

                            keywords.Add(keyaddress);

                        }

                    }
                    it.Dispose();
                }
            }

            return keywords;
        }
        public static object GetPublicMessagesByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = 10)
        {
            GetObjectByAddress(objectaddress, username, password, url, versionByte);

            List<object> messages = new List<object>();

            int rownum = 1;
            var SUP = new Options { CreateIfMissing = true };
            lock (levelDBLocker)
            {
                using (var db = new DB(SUP, @"root\sup"))
                {
                    LevelDB.Iterator it = db.CreateIterator();
                    for (
                       it.Seek(objectaddress);
                       it.IsValid() && it.KeyAsString().StartsWith(objectaddress) && rownum <= skip + qty; // Only display next 10 messages
                        it.Next()
                     )
                    {
                        // Display only if rownum > numMessagesDisplayed to skip already displayed messages
                        if (rownum > skip)
                        {
                            string process = it.ValueAsString();

                            List<string> supMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);

                            string message = System.IO.File.ReadAllText(@"root/" + supMessagePacket[1] + @"/MSG").Replace("@" + objectaddress, "").Replace('“', '"').Replace('”', '"');

                            string fromAddress = supMessagePacket[0];

                            string tstamp = it.KeyAsString().Split('!')[1];

                            // Add the message data to the messages list
                            messages.Add(new
                            {
                                Message = message,
                                FromAddress = fromAddress,
                                BlockDate = tstamp
                            });
                        }
                        rownum++;
                    }
                    it.Dispose();
                }
            }

            return new { Messages = messages };
        }
        public static object GetPrivateMessagesByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = 10)
        {
            GetObjectByAddress(objectaddress, username, password, url, versionByte);

            List<object> messages = new List<object>();

            int rownum = 1;
            var SUP = new Options { CreateIfMissing = true };
            lock (levelDBLocker)
            {
                using (var db = new DB(SUP, @"root\sec"))
                {
                    LevelDB.Iterator it = db.CreateIterator();
                    for (
                       it.Seek(objectaddress);
                       it.IsValid() && it.KeyAsString().StartsWith(objectaddress) && rownum <= skip + qty; // Only display next 10 messages
                        it.Next()
                     )
                    {
                        // Display only if rownum > numMessagesDisplayed to skip already displayed messages
                        if (rownum > skip)
                        {
                            string process = it.ValueAsString();

                            List<string> supMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);
                            Root root = Root.GetRootByTransactionId(supMessagePacket[1],username,password,url, versionByte);
                            byte[] result = Root.GetRootBytesByFile(new string[] { @"root/" + supMessagePacket[1] + @"/SEC" });
                            result = Root.DecryptRootBytes(username, password, url, objectaddress, result);

                            root = Root.GetRootByTransactionId(supMessagePacket[1],null,null,null, versionByte, result);
                           
                                foreach (string message in root.Message)
                                {

                                    string fromAddress = supMessagePacket[0];

                                    string tstamp = it.KeyAsString().Split('!')[1];

                                    // Add the message data to the messages list
                                    messages.Add(new
                                    {
                                        Message = message,
                                        FromAddress = fromAddress,
                                        BlockDate = tstamp
                                    });
                                }
                            


                        }
                        rownum++;
                    }
                    it.Dispose();
                }
            }
            return new { Messages = messages };
        }
        private static string IsAsciiText(byte[] data)
        {
            // Check each byte to see if it's in the ASCII range
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < 0x20 || data[i] > 0x7E)
                {
                    return null;
                }
            }

            return Encoding.ASCII.GetString(data).Replace("#", "").Substring(1);
        }

    }

}




