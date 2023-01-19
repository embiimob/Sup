using LevelDB;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUP.P2FK
{
    public class OBJ
    {
        public string urn { get; set; }
        public string uri { get; set; }
        public string img { get; set; }
        public string nme { get; set; }
        public string dsc { get; set; }
        public string lic { get; set; }
        public int[] cre { get; set; }
        public Dictionary<int, int> own { get; set; }

    }
    public class OBJState
    {
        public string URN { get; set; }
        public string URI { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string License { get; set; }
        public List<string> Creators { get; set; }
        public Dictionary<string, int> Owners { get; set; }
        public DateTime LockedDate { get; set; }
        public int ProcessHeight { get; set; }
        public DateTime ChangeDate { get; set; }
        //ensures levelDB is thread safely
        private readonly static object levelDBLocker = new object();

        public static OBJState GetObjectByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", bool verbose = false)
        {

            OBJState objectState = new OBJState();
            var OBJ = new Options { CreateIfMissing = true };
            string JSONOBJ;
            string logstatus;
            string diskpath = "root\\" + objectaddress + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "P2FK.json");
                objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
            }
            catch { }

            var intProcessHeight = objectState.ProcessHeight;
            Root[] objectTransactions;

            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, versionByte, intProcessHeight);

            if (intProcessHeight > 0 && objectTransactions.Count() == 1) { return objectState; }

            foreach (Root transaction in objectTransactions)
            {

                intProcessHeight = transaction.Id;
                string sortableProcessHeight = intProcessHeight.ToString("X").PadLeft(9, '0');
                logstatus = "";



                //ignore any transaction that is not signed
                if (transaction.Signed)
                {

                    string sigSeen;

                    using (var db = new DB(OBJ, @"root\obj"))
                    {
                        sigSeen = db.Get(transaction.Signature);
                    }

                    if (sigSeen == null || sigSeen == transaction.TransactionId)
                    {


                        using (var db = new DB(OBJ, @"root\obj"))
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
                                catch
                                {

                                    logstatus = "txid:" + transaction.TransactionId + ",object,inspect,\"failed due to invalid transaction format\"";
                                    break;
                                }


                                if (objectinspector.cre != null && objectState.Creators == null)
                                {

                                    objectState.Creators = new List<string> { };
                                    foreach (int keywordId in objectinspector.cre)
                                    {

                                        string creator = transaction.Keyword.Reverse().ElementAt(keywordId).Key;

                                        if (!objectState.Creators.Contains(creator))
                                        {
                                            objectState.Creators.Add(creator);
                                        }

                                    }

                                    objectState.ChangeDate = transaction.BlockDate;
                                    objectinspector.cre = null;
                                }


                                try
                                {
                                    //has proper authority to make OBJ changes
                                    if (objectState.Creators.Contains(transaction.SignedBy))
                                    {
                                        if (objectState.LockedDate.Year == 1)
                                        {
                                            if (objectinspector.urn != null) { objectState.ChangeDate = transaction.BlockDate; objectState.URN = objectinspector.urn; }
                                            if (objectinspector.uri != null) { objectState.ChangeDate = transaction.BlockDate; objectState.URI = objectinspector.uri; }
                                            if (objectinspector.img != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Image = objectinspector.img; }
                                            if (objectinspector.nme != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Name = objectinspector.nme; }
                                            if (objectinspector.dsc != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Description = objectinspector.dsc; }
                                            if (objectinspector.lic != null) { objectState.ChangeDate = transaction.BlockDate; objectState.License = objectinspector.lic; }
                                            if (objectinspector.cre != null)
                                            {
                                                objectState.Creators.Clear();
                                                foreach (int keywordId in objectinspector.cre)
                                                {

                                                    try
                                                    {

                                                        string creator = transaction.Keyword.Reverse().ElementAt(keywordId).Key;

                                                        if (!objectState.Creators.Contains(creator))
                                                        {
                                                            objectState.Creators.Add(creator);
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        logstatus = "txid:" + transaction.TransactionId + ",object,create,\"failed due to invalid transaction format\"";
                                                        break;
                                                    }

                                                }

                                                objectState.ChangeDate = transaction.BlockDate;

                                            }
                                            if (objectState.ChangeDate == transaction.BlockDate)
                                            {
                                                logstatus = "txid:" + transaction.TransactionId + ",object,update,\"success\"";
                                            }
                                            if (objectinspector.own != null)
                                            {
                                                if (objectState.Owners == null)
                                                {
                                                    objectState.Owners = new Dictionary<string, int>();
                                                    logstatus = "txid:" + transaction.TransactionId + ",object,create,\"success\"";
                                                }

                                                objectState.ChangeDate = transaction.BlockDate;
                                                foreach (var ownerId in objectinspector.own)
                                                {
                                                    string owner = transaction.Keyword.Reverse().ElementAt(ownerId.Key).Key;
                                                    if (!objectState.Owners.ContainsKey(owner))
                                                    {
                                                        objectState.Owners.Add(owner, ownerId.Value);
                                                    }
                                                }
                                            }


                                        }
                                        else
                                        {
                                            logstatus = "txid:" + transaction.TransactionId + ",object,update,\"failed due to object lock\"";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        logstatus = "txid:" + transaction.TransactionId + ",object,update,\"failed due to insufficent privlidges\"";
                                    }
                                    break;



                                }
                                catch
                                {
                                    logstatus = "txid:" + transaction.TransactionId + ",object,create,\"failed due to invalid transaction format\"";

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
                                    logstatus = "txid:" + transaction.TransactionId + ",object,inspect,\"failed due to invalid transaction format\"";
                                    break;
                                }

                                if (givinspector == null)
                                {
                                    logstatus = "txid:" + transaction.TransactionId + ",action,giv,\"failed due to invalid transaction format\"";
                                    break;
                                }
                                int giveCount = 0;
                                foreach (var give in givinspector)
                                {
                                    string giver = transaction.SignedBy;

                                    string reciever;

                                    try
                                    {
                                        reciever = transaction.Keyword.Reverse().ElementAt(give[0]).Key;

                                    }
                                    catch
                                    {
                                        logstatus = "txid:" + transaction.TransactionId + ",action,giv,\"failed due to invalid keyword count\"";
                                        break;
                                    }

                                    int qtyToGive = give[1];
                                    giveCount++;
                                    string sortableGiveCount = giveCount.ToString("X").PadLeft(4, '0');



                                    // cannot give less then 1
                                    if (qtyToGive < 1)
                                    {
                                        if (verbose)
                                        {
                                            logstatus = "txid:" + transaction.TransactionId + ",action,giv," + sortableGiveCount + ",\"" + qtyToGive + " from " + transaction.SignedBy + " to " + reciever + " failed due to a give qty of < 1\"";
                                            lock (levelDBLocker)
                                            {
                                                using (var db = new DB(OBJ, @"root\event"))
                                                {
                                                    db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                }
                                            }
                                            logstatus = "";
                                        }
                                        break;
                                    }

                                    // no sense checking any further
                                    if (objectState.Owners == null) { break; }


                                    //is transaction signer not on the Owners list
                                    if (!objectState.Owners.TryGetValue(transaction.SignedBy, out int qtyOwnedG))
                                    {
                                        //is the object container empty
                                        if (!objectState.Owners.TryGetValue(objectaddress, out int selfOwned))
                                        {
                                            if (verbose)
                                            {
                                                //Add Invalid trade attempt status
                                                logstatus = "txid:" + transaction.TransactionId + ",action,giv," + sortableGiveCount + ",\"" + qtyToGive + " from " + transaction.SignedBy + " to " + reciever + " failed due to insufficent qty owned\"";
                                                lock (levelDBLocker)
                                                {
                                                    using (var db = new DB(OBJ, @"root\event"))
                                                    {
                                                        db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                    }
                                                }
                                                logstatus = "";
                                            }
                                            break;
                                        }
                                        else
                                        {    //if the transaction is signed by a creator who doesn't own any objects emulate container
                                            if (objectState.Creators.Contains(transaction.SignedBy) || transaction.SignedBy == objectaddress)
                                            {
                                                giver = objectaddress;
                                                qtyOwnedG = selfOwned;
                                            }
                                        }
                                    }


                                    if (qtyOwnedG >= qtyToGive)
                                    {


                                        // New value to update with
                                        int newValue = qtyOwnedG - qtyToGive;


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
                                        if (!objectState.Owners.TryGetValue(reciever, out int recieverOwned))
                                        {
                                            objectState.Owners.Add(reciever, qtyToGive);

                                        }
                                        else { objectState.Owners[reciever] = recieverOwned + qtyToGive; }

                                        if (verbose)
                                        {
                                            logstatus = "txid:" + transaction.TransactionId + ",action,giv," + sortableGiveCount + ",\"" + qtyToGive + " from " + transaction.SignedBy + " to " + reciever + " succeeded\"";
                                            lock (levelDBLocker)
                                            {
                                                using (var db = new DB(OBJ, @"root\event"))
                                                {
                                                    db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                }
                                            }
                                            logstatus = "";
                                        }

                                        if (objectState.LockedDate.Year == 1)
                                        {
                                            giveCount++;
                                            if (verbose)
                                            {
                                                logstatus = "txid:" + transaction.TransactionId + ",object,lock,\"success\"";
                                                lock (levelDBLocker)
                                                {
                                                    using (var db = new DB(OBJ, @"root\event"))
                                                    {
                                                        db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                    }
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
                                            logstatus = "txid:" + transaction.TransactionId + ",action,giv," + sortableGiveCount + ",\"" + qtyToGive + " from " + transaction.SignedBy + " to " + reciever + " failed due to insufficent qty owned\"";
                                            lock (levelDBLocker)
                                            {
                                                using (var db = new DB(OBJ, @"root\event"))
                                                {
                                                    db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableGiveCount, logstatus);
                                                }
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
                                    logstatus = "txid:" + transaction.TransactionId + ",object,inspect,\"failed due to invalid transaction format\"";
                                    break;
                                }
                                if (brninspector == null)
                                {
                                    logstatus = "txid:" + transaction.TransactionId + ",action,brn,\"failed due to invalid transaction format\"";
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
                                            logstatus = "txid:" + transaction.TransactionId + ",action,brn," + sortableBurnCount + ",\"" + qtyToBurn + " from " + transaction.SignedBy + " failed due to a burn qty of < 1\"";
                                            lock (levelDBLocker)
                                            {
                                                using (var db = new DB(OBJ, @"root\event"))
                                                {
                                                    db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                }
                                            }
                                            logstatus = "";
                                        }
                                        break;
                                    }


                                    if (objectState.Owners == null) { break; }

                                    if (!objectState.Owners.TryGetValue(transaction.SignedBy, out int qtyOwnedG))
                                    {
                                        //try grant access to object's self Owned qtyOwned to any creator
                                        if (!objectState.Owners.TryGetValue(objectaddress, out int selfOwned))
                                        {
                                            if (verbose)
                                            {
                                                //Add Invalid trade attempt status
                                                logstatus = "txid:" + transaction.TransactionId + ",action,brn," + sortableBurnCount + ",\"" + qtyToBurn + " from " + transaction.SignedBy + " failed due to insufficent qty owned\"";
                                                lock (levelDBLocker)
                                                {
                                                    using (var db = new DB(OBJ, @"root\event"))
                                                    {
                                                        db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                    }
                                                }
                                                logstatus = "";
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            if (objectState.Creators.Contains(transaction.SignedBy) || transaction.SignedBy == objectaddress)
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
                                        int newValue = qtyOwnedG - qtyToBurn;


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
                                            logstatus = "txid:" + transaction.TransactionId + ",action,brn," + sortableBurnCount + ",\"" + qtyToBurn + " from " + transaction.SignedBy + " succeeded\"";
                                            lock (levelDBLocker)
                                            {
                                                using (var db = new DB(OBJ, @"root\event"))
                                                {
                                                    db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                }
                                            }
                                            logstatus = "";
                                        }
                                        if (objectState.LockedDate.Year == 1)
                                        {
                                            burnCount++;
                                            if (verbose)
                                            {
                                                logstatus = "txid:" + transaction.TransactionId + ",object,lock,\"success\"";
                                                lock (levelDBLocker)
                                                {
                                                    using (var db = new DB(OBJ, @"root\event"))
                                                    {


                                                        db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                    }
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
                                            //Invalid trade attempt
                                            logstatus = "txid:" + transaction.TransactionId + ",action,brn," + sortableBurnCount + ",\"" + qtyToBurn + " from " + transaction.SignedBy + " failed due to insufficent qty owned\"";
                                            lock (levelDBLocker)
                                            {
                                                using (var db = new DB(OBJ, @"root\event"))
                                                {
                                                    db.Put(objectaddress + "!" + sortableProcessHeight + "!" + sortableBurnCount, logstatus);
                                                }
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
                    else { logstatus = "txid:" + transaction.TransactionId + " transaction failed due to duplicate signature"; }

                }
                else { logstatus = "txid:" + transaction.TransactionId + " transaction failed due to invalid signature"; }

                if (verbose)
                {
                    if (logstatus != "")
                    {

                        lock (levelDBLocker)
                        {
                            using (var db = new DB(OBJ, @"root\event"))
                            {
                                db.Put(objectaddress + "!" + sortableProcessHeight + "!" + "0", logstatus);
                            }
                        }

                    }
                }

            }

            //used to determine where to begin object State processing when retrieved from cache
            objectState.ProcessHeight = intProcessHeight;
            var objectSerialized = JsonConvert.SerializeObject(objectState);

            try
            {
                System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "P2FK.json", objectSerialized);
            }
            catch
            {
                if (!Directory.Exists(@"root\" + objectaddress))
                {
                    Directory.CreateDirectory(@"root\" + objectaddress);
                }
                System.IO.File.WriteAllText(@"root\" + objectaddress + @"\" + "P2FK.json", objectSerialized);
            }

            return objectState;

        }

        public static List<OBJState> GetObjectsByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0)
        {
            List<OBJState> objectStates = new List<OBJState> { };

            Root[] objectTransactions;

            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, versionByte, skip);
            HashSet<string> addedValues = new HashSet<string>();
            foreach (Root transaction in objectTransactions)
            {


                //ignore any transaction that is not signed
                if (transaction.Signed)
                {
                    string findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count -2).Key;


                    OBJState isObject = GetObjectByAddress(findObject, username, password, url, versionByte);

                    if (isObject.URN != null)


                        if (isObject.Creators.ElementAt(0) == findObject)
                        {

                            if (!addedValues.Contains(findObject))
                            {

                                objectStates.Add(isObject);
                                addedValues.Add(findObject);

                            }

                        }


                }


            }


            return objectStates;

        }

        public static List<OBJState> GetObjectsOwnedByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0)
        {

            List<OBJState> objectStates = new List<OBJState> { };

            Root[] objectTransactions;

            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, versionByte, skip);
            HashSet<string> addedValues = new HashSet<string>();
            foreach (Root transaction in objectTransactions)
            {


                //ignore any transaction that is not signed
                if (transaction.Signed)
                {
                    string findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 2).Key;

                    OBJState isOwnedObject = GetObjectByAddress(findObject, username, password, url, versionByte);

                    if (isOwnedObject.URN != null)
                    {



                        if (isOwnedObject.Creators.ElementAt(0) == findObject)
                        {
                            var isValidObject = (isOwnedObject.Owners.ContainsKey(objectaddress) ||
                                            (isOwnedObject.Creators.Contains(objectaddress) &&
                                             isOwnedObject.Owners.TryGetValue(transaction.Keyword.Last().Key, out int qty)));

                            if (isValidObject && !addedValues.Contains(findObject))
                            {
                                objectStates.Add(isOwnedObject);
                                addedValues.Add(findObject);
                            }
                        }
                    }



                }


            }


            return objectStates;

        }

        public static List<OBJState> GetObjectsCreatedByAddress(string objectaddress, string username, string password, string url, string versionByte = "111", int skip = 0)
        {

            List<OBJState> objectStates = new List<OBJState> { };

            Root[] objectTransactions;

            //return all roots found at address
            objectTransactions = Root.GetRootsByAddress(objectaddress, username, password, url, versionByte, skip);
            HashSet<string> addedValues = new HashSet<string>();
            foreach (Root transaction in objectTransactions)
            {


                //ignore any transaction that is not signed
                if (transaction.Signed)
                {
                    string findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 1).Key;

                    OBJState isCreatedObject = GetObjectByAddress(findObject, username, password, url, versionByte);


                    if (isCreatedObject.URN != null)
                    {
                        if (isCreatedObject.Creators.ElementAt(0) == findObject)
                        {
                            if (isCreatedObject.Creators.Contains(objectaddress) && !addedValues.Contains(findObject))
                            {
                                objectStates.Add(isCreatedObject);
                                addedValues.Add(transaction.Keyword.ElementAt(1).Key);


                            }
                        }

                    }

                }

            }

            return objectStates;

        }

        public static List<OBJState> GetObjectsByKeyword(List<string> searchstrings, string username, string password, string url, string versionByte = "111", int skip = 0)
        {
            List<OBJState> objectStates = new List<OBJState> { };

            Root[] objectTransactions;

            foreach (string keyword in searchstrings)
            {
                //return all roots found at address
                objectTransactions = Root.GetRootsByAddress(Root.GetPublicAddressByKeyword(keyword, versionByte), username, password, url, versionByte, skip);
                HashSet<string> addedValues = new HashSet<string>();
                foreach (Root transaction in objectTransactions)
                {


                    //ignore any transaction that is not signed
                    if (transaction.Signed)
                    {
                        string findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 2).Key;


                        OBJState isObject = GetObjectByAddress(findObject, username, password, url, versionByte);
                        if (isObject.URN != null)
                        {
                            if (isObject.Creators.ElementAt(0) == findObject)
                            {

                                if (!addedValues.Contains(findObject))
                                {

                                    objectStates.Add(isObject);
                                    addedValues.Add(findObject);

                                }
                            }


                        }
                    }


                }
            }


            return objectStates;

        }

        public static List<OBJState> GetURNObjects(List<string> searchstrings, string username, string password, string url, string versionByte = "111", int skip = 0)
        {
            List<OBJState> objectStates = new List<OBJState> { };

            Root[] objectTransactions;

            foreach (string keyword in searchstrings)
            {
                //return all roots found at address
                objectTransactions = Root.GetRootsByAddress(Root.GetPublicAddressByKeyword(keyword, versionByte), username, password, url, versionByte, skip);
                HashSet<string> addedValues = new HashSet<string>();
                foreach (Root transaction in objectTransactions)
                {

                    DateTime threeYearsAgo = DateTime.UtcNow.AddYears(-3);
                    int result = threeYearsAgo.CompareTo(transaction.BlockDate);

                    //ignore any transaction that is not signed
                    if (transaction.Signed && result < 0)
                    {
                        string findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 1).Key;


                        OBJState isOwnedObject = GetObjectByAddress(findObject, username, password, url, versionByte);
                        if (isOwnedObject.URN.TrimEnd('#') == keyword)
                        {


                            if (isOwnedObject.Creators.ElementAt(0) == findObject)
                            {
                                var isValidObject = (isOwnedObject.Owners.ContainsKey(findObject) ||
                                                (isOwnedObject.Creators.Contains(findObject) &&
                                                 isOwnedObject.Owners.TryGetValue(transaction.Keyword.Last().Key, out int qty)));

                                if (isValidObject )
                                {
                                    objectStates.Add(isOwnedObject);
                                    return objectStates;
                                }
                            }


                        }
                    }


                }
            }


            return objectStates;

        }
        static bool IsKeywordValid(string s)
        {
            Regex pattern  = new Regex("^.{0,19}#{1,104}$");

            if (s != null)
            {
                return pattern.IsMatch(s);

            }
            else { return false; }
               
        }
    }
}



