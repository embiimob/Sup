using AngleSharp.Common;
using LevelDB;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        public int max { get; set; }
        public string[] cre { get; set; }
        public Dictionary<string, long> own { get; set; }

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
        public int Maximum { get; set; }
        public Dictionary<string, DateTime> Creators { get; set; }
        public Dictionary<string, long> Owners { get; set; }
        public DateTime LockedDate { get; set; }
        public int ProcessHeight { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ChangeDate { get; set; }
        public bool Verbose { get; set; }

        private readonly static object SupLocker = new object();
        public static OBJState GetObjectByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", bool verbose = false)
        {
            lock (SupLocker)
            {

                OBJState objectState = new OBJState();
                var OBJ = new Options { CreateIfMissing = true };
                string isBlocked = "";
                bool fetched = false;

                try
                {
                    using (var db = new DB(OBJ, @"root\oblock"))
                    {
                        isBlocked = db.Get(objectaddress);
                        db.Close();
                    }
                }
                catch
                {
                    try
                    {
                        using (var db = new DB(OBJ, @"root\oblock2"))
                        {
                            isBlocked = db.Get(objectaddress);
                            db.Close();
                        }
                        Directory.Move(@"root\oblock2", @"root\oblock");
                    }
                    catch
                    {
                        try { Directory.Delete(@"root\oblock", true); }
                        catch { }
                    }

                }
                if (isBlocked == "true") { return objectState; }

                string JSONOBJ;
                string logstatus;
                string diskpath = "root\\" + objectaddress + "\\";


                // fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(diskpath + "OBJ.json");
                    objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
                    verbose = objectState.Verbose;
                    fetched = true;

                }
                catch { }
                if (fetched && objectState.URN == null && objectState.ProcessHeight == 0) { return objectState; }

                if (objectState.URN != null && objectState.ChangeDate.Year.ToString() == "1970")
                {
                    Root unconfimredobj = new Root();
                    try
                    {
                        JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetRootByTransctionId.json");
                        unconfimredobj = JsonConvert.DeserializeObject<Root>(JSONOBJ);
                        return OBJState.GetObjectByTransactionId(unconfimredobj.TransactionId, username, password, url, versionByte);

                    }
                    catch { return objectState; }

                }

                int intProcessHeight = 0;

                try { intProcessHeight = objectState.ProcessHeight; } catch { }

                Root[] objectTransactions;
                objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, 1, versionByte);

                if (intProcessHeight != 0 && objectTransactions.Count() == 0) { return objectState; }

                objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, -1, versionByte);

                foreach (Root transaction in objectTransactions)
                {

                    intProcessHeight = transaction.Id;
                    string sortableProcessHeight = intProcessHeight.ToString("X").PadLeft(9, '0');
                    logstatus = "";


                    if (transaction.Signed && (transaction.File.ContainsKey("OBJ") || transaction.File.ContainsKey("GIV") || transaction.File.ContainsKey("BRN")))
                    {

                        string sigSeen;

                        using (var db = new DB(OBJ, @"root\sig"))
                        {
                            sigSeen = db.Get(transaction.Signature);
                        }


                        if (sigSeen == null || sigSeen == transaction.TransactionId)
                        {


                            using (var db = new DB(OBJ, @"root\sig"))
                            {
                                db.Put(transaction.Signature, transaction.TransactionId);
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
                                    {

                                        using (var db = new DB(OBJ, @"root\obj"))
                                        {
                                            db.Put(objectaddress + "!" + key, transaction.TransactionId);
                                        }


                                    }

                                    try
                                    {
                                        if ((objectinspector.cre != null && objectState.Creators == null && int.TryParse(objectinspector.cre[0], out int intID) && objectaddress == transaction.Keyword.Reverse().ElementAt(intID).Key) || objectinspector.cre != null && objectState.Creators == null && objectinspector.cre[0] == objectaddress)

                                        {

                                            objectState.Creators = new Dictionary<string, DateTime> { };
                                            try
                                            {
                                                foreach (string keywordId in objectinspector.cre)
                                                {
                                                    string creator = "";
                                                    if (int.TryParse(keywordId, out int intId))
                                                    {
                                                        creator = transaction.Keyword.Reverse().ElementAt(intId).Key;
                                                    }
                                                    else
                                                    {
                                                        creator = keywordId;

                                                        objectaddress = objectinspector.cre.First();
                                                    }

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
                                    }
                                    catch { }///allows ack signature confirmation

                                    try
                                    {
                                        //has proper authority to make OBJ changes
                                        if (objectState.Creators != null && objectState.Creators.ContainsKey(transaction.SignedBy))
                                        {

                                            if (objectState.Creators.TryGet(transaction.SignedBy).Year == 1)
                                            {
                                                objectState.Creators[transaction.SignedBy] = transaction.BlockDate;
                                                objectState.ChangeDate = transaction.BlockDate;
                                                if (verbose)
                                                {

                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"grant\",\"\",\"\",\"success\"]";

                                                    lock (SupLocker)
                                                    {
                                                        var ROOT = new Options { CreateIfMissing = true };
                                                        var db = new DB(ROOT, @"root\event");
                                                        db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!", logstatus);
                                                        db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!");
                                                        db.Close();
                                                    }


                                                }

                                            }



                                            if (objectState.LockedDate.Year == 1)
                                            {
                                                if (objectinspector.urn != null) { objectState.ChangeDate = transaction.BlockDate; objectState.URN = objectinspector.urn.Replace('“', '"').Replace('”', '"'); }
                                                if (objectinspector.uri != null) { objectState.ChangeDate = transaction.BlockDate; objectState.URI = objectinspector.uri.Replace('“', '"').Replace('”', '"'); }
                                                if (objectinspector.img != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Image = objectinspector.img.Replace('“', '"').Replace('”', '"'); }
                                                if (objectinspector.nme != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Name = objectinspector.nme.Replace('“', '"').Replace('”', '"'); }
                                                if (objectinspector.dsc != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Description = objectinspector.dsc.Replace('“', '"').Replace('”', '"'); }
                                                if (objectinspector.atr != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Attributes = objectinspector.atr; }
                                                if (objectinspector.lic != null) { objectState.ChangeDate = transaction.BlockDate; objectState.License = objectinspector.lic.Replace('“', '"').Replace('”', '"'); }
                                                if (objectinspector.max != objectState.Maximum) { objectState.ChangeDate = transaction.BlockDate; objectState.Maximum = objectinspector.max; }
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


                                                        using (var db = new DB(OBJ, @"root\found"))
                                                        {
                                                            db.Put("found!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + transaction.SignedBy, "1");
                                                        }


                                                    }


                                                    foreach (var ownerId in objectinspector.own)
                                                    {
                                                        string owner = "";
                                                        if (int.TryParse(ownerId.Key, out int intId))
                                                        {
                                                            owner = transaction.Keyword.Reverse().ElementAt(intId).Key;
                                                        }
                                                        else { owner = ownerId.Key; }

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
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"salt" + qtyToGive.ToString() + "\"]";


                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount);

                                                db.Close();

                                                logstatus = "";
                                            }
                                            break;
                                        }

                                        // no sense checking any further
                                        if (objectState.Owners == null) { break; }
                                        if (objectState.Maximum > 0)
                                        {
                                            if (qtyToGive > objectState.Maximum)
                                            {
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"failed due to over maximum qty\"]";

                                                break;
                                            }

                                            if (objectState.Owners.TryGetValue(reciever, out long value) && value + qtyToGive >= objectState.Maximum)
                                            {
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"failed due to over maximum qty\"]";

                                                break;
                                            }
                                        }


                                        //is transaction signer not on the Owners list
                                        if (!objectState.Owners.TryGetValue(transaction.SignedBy, out long qtyOwnedG))
                                        {
                                            //is the object container empty
                                            if (!objectState.Owners.TryGetValue(objectaddress, out long selfOwned))
                                            {
                                                if (verbose)
                                                {
                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + reciever + "\",\"give\",\"" + qtyToGive + "\",\"\",\"failed due to insufficent qty owned\"]";


                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                    db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount);

                                                    db.Close();

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


                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount);

                                                db.Close();

                                                logstatus = "";
                                            }

                                            if (objectState.LockedDate.Year == 1)
                                            {

                                                if (verbose)
                                                {
                                                    giveCount++;
                                                    sortableGiveCount = giveCount.ToString("X").PadLeft(4, '0');
                                                    logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"lock\",\"\",\"\",\"success\"]";

                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                    db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount);
                                                    db.Close();

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

                                                lock (SupLocker)
                                                {
                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                    db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableGiveCount);
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
                                                logstatus = "[\"" + transaction.SignedBy + "\",\"" + objectaddress + "\",\"burn\",\"" + qtyToBurn + "\",\"\",\"salt" + qtyToBurn.ToString() + "\"]";

                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount);
                                                db.Close();

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


                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                    db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount);

                                                    db.Close();

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

                                                lock (SupLocker)
                                                {
                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                    db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount);
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


                                                    var ROOT = new Options { CreateIfMissing = true };
                                                    var db = new DB(ROOT, @"root\event");
                                                    db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                    db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount);
                                                    db.Close();

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

                                                var ROOT = new Options { CreateIfMissing = true };
                                                var db = new DB(ROOT, @"root\event");
                                                db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight + "!" + sortableBurnCount);

                                                db.Close();

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



                            var ROOT = new Options { CreateIfMissing = true };
                            var db = new DB(ROOT, @"root\event");
                            db.Put(objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight, logstatus);
                            db.Put("lastkey!" + objectaddress, objectaddress + "!" + transaction.BlockDate.ToString("yyyyMMddHHmmss") + "!" + sortableProcessHeight);
                            db.Close();

                        }
                    }

                }

                //used to determine where to begin object State processing when retrieved from cache
                objectState.ProcessHeight = objectTransactions.Count();
                objectState.Verbose = verbose;
                var objectSerialized = JsonConvert.SerializeObject(objectState);


                if (!Directory.Exists(@"root\" + objectaddress))
                {
                    Directory.CreateDirectory(@"root\" + objectaddress);
                }
                System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "OBJ.json", objectSerialized);


                return objectState;
            }
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
                    if (int.TryParse(keywordId, out int intId))
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
                                objectState.Owners = new Dictionary<string, long>();


                            }


                            foreach (var ownerId in objectinspector.own)
                            {
                                string owner = "";

                                if (int.TryParse(ownerId.Key, out int intId))
                                {
                                    owner = objectTransaction.Keyword.Reverse().ElementAt(intId).Key;
                                }
                                else { owner = ownerId.Key; }
                                if (!objectState.Owners.ContainsKey(owner))
                                {
                                    objectState.Owners.Add(owner, ownerId.Value);
                                }
                                else
                                {
                                    objectState.Owners[owner] = ownerId.Value;

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
        public static OBJState GetObjectByURN(string searchstring, string username, string password, string url, string versionByte = "111", int skip = 0)
        {
            lock (SupLocker)
            {
                OBJState objectState = new OBJState();
                string objectaddress = Root.GetPublicAddressByKeyword(searchstring, versionByte);
                var OBJ = new Options { CreateIfMissing = true };
                string isBlocked = "";
                bool fetched = false;
                try
                {
                    using (var db = new DB(OBJ, @"root\oblock"))
                    {
                        isBlocked = db.Get(objectaddress);
                        db.Close();
                    }
                }
                catch
                {
                    try
                    {
                        using (var db = new DB(OBJ, @"root\oblock2"))
                        {
                            isBlocked = db.Get(objectaddress);
                            db.Close();
                        }
                        Directory.Move(@"root\oblock2", @"root\oblock");
                    }
                    catch
                    {
                        try { Directory.Delete(@"root\oblock", true); }
                        catch { }
                    }

                }
                if (isBlocked == "true") { return objectState; }

                string JSONOBJ;
                string diskpath = "root\\" + objectaddress + "\\";


                // fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetObjectByURN.json");
                    objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
                    fetched = true;

                }
                catch { }

                if (fetched && objectState.URN == null) { return objectState; }

                if (objectState.URN != null && objectState.ChangeDate.Year.ToString() == "1970") { objectState = new OBJState(); }

                var intProcessHeight = objectState.Id;
                Root[] objectTransactions;

                //return all roots found at address
                objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, 1, versionByte);


                if (intProcessHeight > 0 && objectTransactions.Count() == 0)
                {

                    return objectState;

                }

                //return all roots found at address
                objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, 0, -1, versionByte);
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
                                isObject.Id = objectTransactions.Count();
                                var profileSerialized = JsonConvert.SerializeObject(isObject);
                                try
                                {
                                    System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectByURN.json", profileSerialized);
                                }
                                catch
                                {

                                    try
                                    {
                                        if (!Directory.Exists(@"root\" + objectaddress))

                                        {
                                            Directory.CreateDirectory(@"root\" + objectaddress);
                                        }
                                        System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectByURN.json", profileSerialized);
                                    }
                                    catch { };
                                }


                                return isObject;

                            }

                        }


                    }
                }
                objectState.Id = objectTransactions.Count();
                var profileSerialized2 = JsonConvert.SerializeObject(objectState);
                try
                {
                    System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectByURN.json", profileSerialized2);
                }
                catch
                {

                    try
                    {
                        if (!Directory.Exists(@"root\" + objectaddress))

                        {
                            Directory.CreateDirectory(@"root\" + objectaddress);
                        }
                        System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectByURN.json", profileSerialized2);
                    }
                    catch { };
                }
                return objectState;


            }
        }
        public static OBJState GetObjectByFile(string filepath, string username, string password, string url, string versionByte = "111", int skip = 0)
        {
            OBJState objectState = new OBJState { };
            Root[] objectTransactions;

            byte[] payload = new byte[21];

            try
            {
                using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
                {
                    fileStream.Read(payload, 1, 20);
                }
            }
            catch { }

            payload[0] = Byte.Parse(versionByte);
            string objectaddress = Base58.EncodeWithCheckSum(payload);

            var OBJ = new Options { CreateIfMissing = true };
            string isBlocked = "";

            try
            {
                using (var db = new DB(OBJ, @"root\oblock"))
                {
                    isBlocked = db.Get(objectaddress);
                    db.Close();
                }
            }
            catch
            {
                try
                {
                    using (var db = new DB(OBJ, @"root\oblock2"))
                    {
                        isBlocked = db.Get(objectaddress);
                        db.Close();
                    }
                    Directory.Move(@"root\oblock2", @"root\oblock");
                }
                catch
                {
                    try { Directory.Delete(@"root\oblock", true); }
                    catch { }
                }

            }
            if (isBlocked == "true") { return objectState; }


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

                                var SUP = new Options { CreateIfMissing = true };
                                string isLoading;
                                lock (SupLocker)
                                {
                                    using (var db = new DB(SUP, @"ipfs"))
                                    {
                                        isLoading = db.Get(transid);

                                    }
                                }

                                if (isLoading != "loading")
                                {
                                    lock (SupLocker)
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
                                        lock (SupLocker)
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

                            string transid = isObject.URN.Replace("MZC:", "").Replace("BTC:", "").Replace("LTC:", "").Replace("DOG:", "").Substring(0, 64);
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

                            file2 = @"root\" + isObject.URN.Replace("MZC:", "").Replace("BTC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace(@"/", @"\");
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
        public static List<OBJState> GetObjectsByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {
            lock (SupLocker)
            {
                List<OBJState> objectStates = new List<OBJState> { };
                var OBJ = new Options { CreateIfMissing = true };
                string isBlocked = "";
                bool fetched = false;

                try
                {
                    using (var db = new DB(OBJ, @"root\oblock"))
                    {
                        isBlocked = db.Get(objectaddress);
                        db.Close();
                    }
                }
                catch
                {
                    try
                    {
                        using (var db = new DB(OBJ, @"root\oblock2"))
                        {
                            isBlocked = db.Get(objectaddress);
                            db.Close();
                        }
                        Directory.Move(@"root\oblock2", @"root\oblock");
                    }
                    catch
                    {
                        try { Directory.Delete(@"root\oblock", true); }
                        catch { }
                    }

                }
                if (isBlocked == "true") { return objectStates; }

                string JSONOBJ;
                string diskpath = "root\\" + objectaddress + "\\";


                // fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetObjectsByAddress.json");
                    objectStates = JsonConvert.DeserializeObject<List<OBJState>>(JSONOBJ);
                    fetched = true;

                }
                catch { }
                if (fetched && objectStates.Count < 1) { return objectStates; }

                int intProcessHeight = 0;

                //try { intProcessHeight = objectStates.Max(state => state.Id); } catch { }
                try { intProcessHeight = objectStates.Max(state => state.ProcessHeight); } catch { }

                Root[] objectTransactions;

                //return all roots found at address
                objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, 1, versionByte);

                if (intProcessHeight != 0 && objectTransactions.Count() == 0)
                {

                    if (qty == -1) { return objectStates.ToList(); }
                    else { return objectStates.Skip(skip).Take(qty).ToList(); }

                }

                objectStates = new List<OBJState> { };
                objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, 0, -1, versionByte);

                //return all roots found at address

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

                                        if (isObject.URN != null)
                                        {

                                            using (var db = new DB(OBJ, @"root\obj"))
                                            {
                                                findId = db.Get(objectaddress + "!" + key);
                                            }


                                            if (findId == transaction.Id.ToString() || findId == null)
                                            {
                                                if (findId == null)
                                                {

                                                    using (var db = new DB(OBJ, @"root\obj"))
                                                    {
                                                        db.Put(objectaddress + "!" + key, transaction.Id.ToString());
                                                    }

                                                }
                                            }

                                            isObject.Id = transaction.Id;
                                            objectStates.Add(isObject);

                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (string key in transaction.Keyword.Keys.Reverse().Take(2))
                                {

                                    if (!addedValues.Contains(key))
                                    {
                                        addedValues.Add(key);

                                        OBJState isObject = GetObjectByAddress(key, username, password, url, versionByte);

                                        if (isObject.URN != null)
                                        {

                                            using (var db = new DB(OBJ, @"root\obj"))
                                            {
                                                findId = db.Get(objectaddress + "!" + key);
                                            }


                                            if (findId == transaction.Id.ToString() || findId == null)
                                            {
                                                if (findId == null)
                                                {

                                                    using (var db = new DB(OBJ, @"root\obj"))
                                                    {
                                                        db.Put(objectaddress + "!" + key, transaction.Id.ToString());
                                                    }

                                                }
                                            }

                                            isObject.Id = transaction.Id;
                                            objectStates.Add(isObject);
                                            break;

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
                System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectsByAddress.json", objectSerialized);

                if (qty == -1) { return objectStates.ToList(); }
                else { return objectStates.Skip(skip).Take(qty).ToList(); }

            }
        }
        public static List<OBJState> GetObjectsOwnedByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {

            lock (SupLocker)
            {
                List<OBJState> objectStates = new List<OBJState> { };
                var OBJ = new Options { CreateIfMissing = true };
                string isBlocked = "";
                bool fetched = false;
                try
                {
                    using (var db = new DB(OBJ, @"root\oblock"))
                    {
                        isBlocked = db.Get(objectaddress);
                        db.Close();
                    }
                }
                catch
                {
                    try
                    {
                        using (var db = new DB(OBJ, @"root\oblock2"))
                        {
                            isBlocked = db.Get(objectaddress);
                            db.Close();
                        }
                        Directory.Move(@"root\oblock2", @"root\oblock");
                    }
                    catch
                    {
                        try { Directory.Delete(@"root\oblock", true); }
                        catch { }
                    }

                }
                if (isBlocked == "true") { return objectStates; }

                string JSONOBJ;
                string diskpath = "root\\" + objectaddress + "\\";


                // fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetObjectsOwnedByAddress.json");
                    objectStates = JsonConvert.DeserializeObject<List<OBJState>>(JSONOBJ);
                    fetched = true;
                }
                catch { }
                if (fetched && objectStates.Count < 1) { return objectStates; }

                int intProcessHeight = 0;
                //try { intProcessHeight = objectStates.Max(state => state.Id); } catch { }
                try { intProcessHeight = objectStates.Max(state => state.ProcessHeight); } catch { }
                Root[] objectTransactions;

                //return all roots found at address
                objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, 1, versionByte);
                if (intProcessHeight > 0 && objectTransactions.Count() == 0)
                {

                    if (qty == -1) { return objectStates.ToList(); }
                    else { return objectStates.Skip(skip).Take(qty).ToList(); }

                }
                List<OBJState> cachedObjectStates = OBJState.GetObjectsByAddress(objectaddress, username, password, url, versionByte, 0, -1);
                objectStates = new List<OBJState>();
                //return all roots found at address
                foreach (OBJState objectstate in cachedObjectStates)
                {
                    if (objectstate.URN != null && objectstate.Owners.ContainsKey(objectaddress))
                    {

                        objectStates.Add(objectstate);

                    }

                }

                var objectSerialized = JsonConvert.SerializeObject(objectStates);

                if (!Directory.Exists(@"root\" + objectaddress))
                {
                    Directory.CreateDirectory(@"root\" + objectaddress);
                }
                System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "GetObjectsOwnedByAddress.json", objectSerialized);

                return objectStates;

            }
        }
        public static List<OBJState> GetObjectsCreatedByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {

            lock (SupLocker)
            {
                List<OBJState> objectStates = new List<OBJState> { };
                var OBJ = new Options { CreateIfMissing = true };
                string isBlocked = "";
                bool fetched = false;

                try
                {
                    using (var db = new DB(OBJ, @"root\oblock"))
                    {
                        isBlocked = db.Get(objectaddress);
                        db.Close();
                    }
                }
                catch
                {
                    try
                    {
                        using (var db = new DB(OBJ, @"root\oblock2"))
                        {
                            isBlocked = db.Get(objectaddress);
                            db.Close();
                        }
                        Directory.Move(@"root\oblock2", @"root\oblock");
                    }
                    catch
                    {
                        try { Directory.Delete(@"root\oblock", true); }
                        catch { }
                    }

                }
                if (isBlocked == "true") { return objectStates; }

                string JSONOBJ;
                string diskpath = "root\\" + objectaddress + "\\";


                // fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetObjectsCreatedByAddress.json");
                    objectStates = JsonConvert.DeserializeObject<List<OBJState>>(JSONOBJ);
                    fetched = true;

                }
                catch { }
                if (fetched && objectStates.Count < 1) { return objectStates; }

                int intProcessHeight = 0;
                //try { intProcessHeight = objectStates.Max(state => state.Id); } catch { }
                try { intProcessHeight = objectStates.Max(state => state.ProcessHeight); } catch { }
                Root[] objectTransactions;

                //return all roots found at address
                objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, intProcessHeight, 1, versionByte);
                if (intProcessHeight > 0 && objectTransactions.Count() == 0)
                {

                    if (qty == -1) { return objectStates.ToList(); }
                    else { return objectStates.Skip(skip).Take(qty).ToList(); }

                }

                List<OBJState> cachedObjectStates = OBJState.GetObjectsByAddress(objectaddress, username, password, url, versionByte, 0, -1);
                objectStates = new List<OBJState>();
                //return all roots found at address
                foreach (OBJState objectstate in cachedObjectStates)
                {
                    if (objectstate.URN != null && objectstate.Creators.ContainsKey(objectaddress) && objectstate.Creators[objectaddress] != null && objectstate.Creators[objectaddress].Year > 1975)
                    {

                        objectStates.Add(objectstate);

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

        }
        public static List<OBJState> GetObjectsByKeyword(List<string> searchstrings, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {
            lock (SupLocker)
            {


                List<OBJState> totalSearch = new List<OBJState>();

                foreach (string search in searchstrings)
                {

                    string objectaddress = Root.GetPublicAddressByKeyword(search, versionByte);
                    string isBlocked = "";
                    var OBJ = new Options { CreateIfMissing = true };
                    try
                    {
                        using (var db = new DB(OBJ, @"root\oblock"))
                        {
                            isBlocked = db.Get(search);
                            db.Close();
                        }
                    }
                    catch
                    {
                        try
                        {
                            using (var db = new DB(OBJ, @"root\oblock2"))
                            {
                                isBlocked = db.Get(search);
                                db.Close();
                            }
                            Directory.Move(@"root\oblock2", @"root\oblock");
                        }
                        catch
                        {
                            try { Directory.Delete(@"root\oblock", true); }
                            catch { }
                        }

                    }

                    if (isBlocked != "true")
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

        }
        public static List<OBJState> GetFoundObjects(string username, string password, string url, string versionByte = "111", int skip = 0, int qty = -1)
        {
            lock (SupLocker)
            {
                List<OBJState> objectStates = new List<OBJState> { };
                var OBJ = new Options { CreateIfMissing = true };
                string JSONOBJ;
                string diskpath = "root\\found\\";


                // fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetFoundObjects.json");
                    objectStates = JsonConvert.DeserializeObject<List<OBJState>>(JSONOBJ);

                }
                catch { }

                int foundCount = 0;
                var SUP = new Options { CreateIfMissing = true };

                using (var db = new DB(SUP, @"root\found"))
                {
                    LevelDB.Iterator it = db.CreateIterator();

                    for (
                           it.SeekToLast();
                          it.IsValid();
                            it.Prev()
                     ) { foundCount++; }

                    it.Dispose();
                }

                if (objectStates.Count() == foundCount) { return objectStates; }

                objectStates.Clear();
                HashSet<string> addedValues = new HashSet<string>();
                int rownum = 0;
                if (qty == -1) { qty = foundCount; }


                using (var db = new DB(SUP, @"root\found"))
                {
                    LevelDB.Iterator it = db.CreateIterator();

                    for (
                       it.SeekToLast();
                      it.IsValid() && it.KeyAsString().StartsWith("found!") && rownum < skip + qty;
                        it.Prev()
                 )

                    {
                        string whatis = it.KeyAsString().Split('!')[2];
                        // Display only if rownum > numMessagesDisplayed to skip already displayed messages
                        if (rownum >= skip && !addedValues.Contains(it.KeyAsString().Split('!')[2]))
                        {

                            addedValues.Add(it.KeyAsString().Split('!')[2]);
                            OBJState isObject = GetObjectByAddress(it.KeyAsString().Split('!')[2], username, password, url, versionByte);
                            objectStates.Add(isObject);


                        }
                        rownum++;
                    }
                    it.Dispose();


                }


                var objectSerialized = JsonConvert.SerializeObject(objectStates);

                if (!Directory.Exists(@"root\found"))
                {
                    Directory.CreateDirectory(@"root\found");
                }
                System.IO.File.WriteAllText(@"root\found\GetFoundObjects.json", objectSerialized);

                return objectStates;
            }
        }
        public static List<string> GetKeywordsByAddress(string objectaddress, string username, string password, string url, string versionByte = "111")
        {

            GetObjectByAddress(objectaddress, username, password, url, versionByte);

            lock (SupLocker)
            {
                List<string> keywords = new List<string>();

                var KEY = new Options { CreateIfMissing = true };

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
                        // Check each byte to see if it's in the ASCII range
                        for (int i = 0; i < payloadBytes.Length; i++)
                        {
                            if (payloadBytes[i] < 0x20 || payloadBytes[i] > 0x7E)
                            {
                                keyaddress = null;
                            }
                        }

                        if (keyaddress != null)
                        {
                            keyaddress = Encoding.ASCII.GetString(payloadBytes).Replace("#", "").Substring(1);

                            keywords.Add(keyaddress);

                        }

                    }
                    it.Dispose();
                }

                return keywords;
            }
        }
        public static object GetPublicMessagesByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = 20)
        {
            GetObjectByAddress(objectaddress, username, password, url, versionByte);
                                  
            
            lock (SupLocker)
            {
                List<object> messages = new List<object>();

                int rownum = 1;
                var SUP = new Options { CreateIfMissing = true };

                using (var db = new DB(SUP, @"root\"+ objectaddress + @"\sup"))
                {
                    LevelDB.Iterator it = db.CreateIterator();
                    for (
                       it.SeekToLast();
                       it.IsValid() && rownum <= skip + qty; // Only display next 20 messages
                        it.Prev()
                     )
                    {
                        // Display only if rownum > numMessagesDisplayed to skip already displayed messages
                        if (rownum > skip)
                        {
                            string process = it.ValueAsString();

                            List<string> supMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);

                            string message = "";

                            try { message = System.IO.File.ReadAllText(@"root/" + supMessagePacket[1] + @"/MSG").Replace("@" + objectaddress, "").Replace('“', '"').Replace('”', '"'); } catch { }

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


                return new { Messages = messages };
            }
        }
        public static object GetPrivateMessagesByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0, int qty = 10)
        {
            GetObjectByAddress(objectaddress, username, password, url, versionByte);
            lock (SupLocker)
            {
                List<object> messages = new List<object>();

                int rownum = 1;
                var SUP = new Options { CreateIfMissing = true };

                using (var db = new DB(SUP, @"root\"+ objectaddress +@"\sec"))
                {
                   
                    LevelDB.Iterator it = db.CreateIterator();
                    for (
                       it.SeekToLast();
                       it.IsValid() && it.KeyAsString().StartsWith(objectaddress) && rownum <= skip + qty; // Only display next 10 messages
                        it.Prev()
                     )
                    {
                        // Display only if rownum > numMessagesDisplayed to skip already displayed messages
                        if (rownum > skip)
                        {
                            string process = it.ValueAsString();

                            List<string> supMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);
                            Root root = Root.GetRootByTransactionId(supMessagePacket[1], username, password, url, versionByte);
                            byte[] result = Root.GetRootBytesByFile(new string[] { @"root/" + supMessagePacket[1] + @"/SEC" });
                            result = Root.DecryptRootBytes(username, password, url, objectaddress, result); 

                            root = Root.GetRootByTransactionId(supMessagePacket[1], "good-user", "better-password", "http://127.0.0.1:18332", versionByte, result, objectaddress);

                            try
                            {
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
                            catch { }


                        }
                        rownum++;
                    }
                    it.Dispose();
                }

                return new { Messages = messages };
            }
        }

    }

}




