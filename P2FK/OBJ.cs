using LevelDB;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SUP.P2FK
{
    public class OBJ
    {
        public string urn { get; set; }
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

        public static OBJState GetObjectByAddress(string objectaddress, string username, string password, string url, string versionByte = "111")
        {

            OBJState objectState = new OBJState();
            var OBJ = new Options { CreateIfMissing = true };
            string JSONOBJ;

            lock (levelDBLocker)
            {
                //try grabbing the object state from levelDB cache
                using (var db = new DB(OBJ, @"root\obj"))
                {
                    JSONOBJ = db.Get(objectaddress);
                }
            }

            try { objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ); }
            catch { }



            var intProcessHeight = objectState.ProcessHeight;
            Root[] objectTransactions;
            lock (levelDBLocker)
            {
                //return all roots found at address
                objectTransactions = Root.GetRootByAddress(objectaddress, username, password, url, versionByte, true, intProcessHeight);
            }
            string logstatus;


            foreach (Root transaction in objectTransactions)
            {
                intProcessHeight = transaction.Id;
                logstatus = "";

                //ignore any transaction that is not signed
                if (transaction.Signed)
                {

                    if (transaction.Cached)
                    {
                        var modifiedDictionary = new Dictionary<string, byte[]>();

                        //Add cached file data from disk back into the root object
                        foreach (var kvp in transaction.File)
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
                        transaction.File = modifiedDictionary;
                    }


                    switch (transaction.File.Last().Key.ToString().Substring(transaction.File.Last().Key.ToString().Length - 3))
                    {
                        case "OBJ":
                            OBJ objectinspector = null;
                            try
                            {
                                objectinspector = JsonConvert.DeserializeObject<OBJ>(Encoding.ASCII.GetString(transaction.File.Last().Value));
                            }
                            catch
                            {

                                logstatus = "txid:" + transaction.TransactionId + ",object,inspect,\"failed due to invalid transaction format\"";
                                break;
                            }

                            //if (objectinspector == null)
                            //{
                            //    logstatus = "txid:" + transaction.TransactionId + " object creation failed due to invalid transaction format";
                            //    break;
                            //}


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



                            //has proper authority to make OBJ changes
                            if (objectState.Creators.Contains(transaction.SignedBy))
                            {
                                if (objectState.LockedDate.Year < 1975)
                                {
                                    if (objectinspector.urn != null) { objectState.ChangeDate = transaction.BlockDate; objectState.URN = objectinspector.urn; }
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

                        case "GIV":

                            List<List<int>> givinspector = JsonConvert.DeserializeObject<List<List<int>>>(Encoding.ASCII.GetString(transaction.File.Last().Value));
                            if (givinspector == null)
                            {
                                logstatus = "txid:" + transaction.TransactionId + ",action,giv,\"failed due to invalid transaction format\"";
                                break;
                            }
                            int giveCount = 0;
                            foreach (var give in givinspector)
                            {
                                string giver = transaction.SignedBy;
                                string reciever = transaction.Keyword.Reverse().ElementAt(give[0]).Key;
                                int qtyToGive = give[1];
                                giveCount++;
                                if (qtyToGive < 1)
                                {
                                    logstatus = "txid:" + transaction.TransactionId + ",action,giv," + giveCount + ",\"" + qtyToGive + " from " + transaction.SignedBy + " to " + reciever + " failed due to a give qty of < 1\"";
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(OBJ, @"root\event"))
                                        {
                                            db.Put(objectaddress + "!" + intProcessHeight + "!" + giveCount, logstatus);
                                        }
                                    }

                                    break;
                                }

                                //probably something needs to be done more here
                                //probably
                                if (objectState.Owners == null) { break; }

                                if (!objectState.Owners.TryGetValue(transaction.SignedBy, out int qtyOwnedG))
                                {
                                    //try grant access to object's self Owned qtyOwned to any creator
                                    if (!objectState.Owners.TryGetValue(objectaddress, out int selfOwned))
                                    {
                                        //Add Invalid trade attempt status
                                        logstatus = "txid:" + transaction.TransactionId + ",action,giv," + giveCount + ",\"" + qtyToGive + " from " + transaction.SignedBy + " to " + reciever + " failed due to insufficent qty owned\"";
                                        lock (levelDBLocker)
                                        {
                                            using (var db = new DB(OBJ, @"root\event"))
                                            {
                                                db.Put(objectaddress + "!" + intProcessHeight + "!" + giveCount, logstatus);
                                            }
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        if (objectState.Creators.Contains(transaction.SignedBy) || transaction.SignedBy == objectaddress)
                                        {
                                            giver = objectaddress;
                                            qtyOwnedG = selfOwned;
                                        }
                                    }
                                }


                                if (qtyOwnedG >= qtyToGive)
                                {
                                    //update owners Dictionary with new values

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
                                    logstatus = "txid:" + transaction.TransactionId + ",action,giv," + giveCount + ",\"" + qtyToGive + " from " + transaction.SignedBy + " to " + reciever + " succeeded\"";
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(OBJ, @"root\event"))
                                        {
                                            db.Put(objectaddress + "!" + intProcessHeight + "!" + giveCount, logstatus);
                                        }
                                    }



                                    if (objectState.LockedDate.Year < 1975)
                                    {

                                        logstatus = "txid:" + transaction.TransactionId + ",object,lock,\"success\"";
                                        lock (levelDBLocker)
                                        {
                                            using (var db = new DB(OBJ, @"root\event"))
                                            {
                                                db.Put(objectaddress + "!LockedDate", logstatus);
                                            }
                                        }

                                        objectState.LockedDate = transaction.BlockDate;
                                    }
                                    objectState.ChangeDate = transaction.BlockDate;
                                    logstatus = "";
                                }
                                else
                                {
                                    //Add Invalid trade attempt status
                                    logstatus = "txid:" + transaction.TransactionId + ",action,giv," + giveCount + ",\"" + qtyToGive + " from " + transaction.SignedBy + " to " + reciever + " failed due to insufficent qty owned\"";
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(OBJ, @"root\event"))
                                        {
                                            db.Put(objectaddress + "!" + intProcessHeight + "!" + giveCount, logstatus);
                                        }
                                    }
                                    break;
                                }



                            }
                            break;

                        case "BRN":

                            string burner = transaction.SignedBy;
                            int qtyToBurn = JsonConvert.DeserializeObject<int>(Encoding.ASCII.GetString(transaction.File.Last().Value));


                            if (qtyToBurn < 1)
                            {
                                logstatus = "txid:" + transaction.TransactionId + " burn " + qtyToBurn + " failed due to a qty of < = 0";
                                break;
                            }

                            int qtyOwnedB;
                            if (!objectState.Owners.TryGetValue(transaction.SignedBy, out qtyOwnedB))
                            {
                                //try grant access to object's self Owned qtyOwned to any creator
                                if (!objectState.Owners.TryGetValue(objectaddress, out int selfOwned))
                                {
                                    //Add Invalid trade attempt status
                                    logstatus = "txid:" + transaction.TransactionId + " burn " + qtyToBurn + " failed due to insufficent qty";
                                    break;
                                }
                                if (objectState.Creators.Contains(transaction.SignedBy))
                                {
                                    //assume self owned address identity if on creator list
                                    burner = objectaddress;
                                    qtyOwnedB = selfOwned;
                                }


                            }

                            if (qtyOwnedB >= qtyToBurn)
                            {
                                //update owners Dictionary with new values

                                // New value to update with
                                int newValue = qtyOwnedB - qtyToBurn;


                                // Check if the new value is an integer
                                if (newValue > 0)
                                {
                                    // Update the value
                                    objectState.Owners[burner] = newValue;
                                }
                                else
                                {
                                    // remove the dictionary key
                                    objectState.Owners.Remove(burner);
                                }
                                logstatus = "txid:" + transaction.TransactionId + " burn " + qtyToBurn + " of " + burner + " succeded";

                                objectState.ChangeDate = transaction.BlockDate;
                                if (objectState.LockedDate == null) { objectState.LockedDate = transaction.BlockDate; }



                            }
                            else
                            {
                                //Add Invalid trade attempt status
                                logstatus = "txid:" + transaction.TransactionId + " burn " + qtyToBurn + " of " + burner + " failed due to insufficent qty of " + qtyOwnedB;
                                break;
                            }

                            break;

                        default:
                            // ignore


                            break;
                    }


                }
                else { logstatus = "txid:" + transaction.TransactionId + " transaction failed due to invalid signature"; }
                if (logstatus != "")
                {

                    lock (levelDBLocker)
                    {
                        using (var db = new DB(OBJ, @"root\event"))
                        {
                            db.Put(objectaddress + "!" + intProcessHeight, logstatus);
                        }
                    }

                }

            }

            //used to determine where to begin object State processing when retrieved from cache
            objectState.ProcessHeight = intProcessHeight;
            lock (levelDBLocker)
            {
                using (var db = new DB(OBJ, @"root\obj"))
                {
                    db.Put(objectaddress, JsonConvert.SerializeObject(objectState));
                }
            }

            return objectState;

        }
    }


}
