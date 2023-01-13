using LevelDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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


        public static OBJState GetObjectByAddress(string address, string username, string password, string url, string versionByte = "111", bool useCache = true)
        {

            OBJState objectState = new OBJState();
            var OBJ = new Options { CreateIfMissing = true };

            //try grabbing the object state from levelDB cache
            using (var db = new DB(OBJ, @"root\obj"))
            {
                objectState = JsonConvert.DeserializeObject<OBJState>(db.Get(address));
            }
            var intProcessHeight = objectState.ProcessHeight;

            //return all roots found at address
            Root[] objectTransactions = Root.GetRootByAddress(address, username, password, url, versionByte, useCache, intProcessHeight);
            string logstatus;

            foreach (Root transaction in objectTransactions)
            {
                intProcessHeight = transaction.Id;
                logstatus = "";

                //ignore any transaction that is not signed
                if (transaction.Signed)
                {


                    switch (transaction.File.First().Key.ToString())
                    {
                        case "OBJ":
                            OBJ objectinpector = JsonConvert.DeserializeObject<OBJ>(Encoding.ASCII.GetString(transaction.File.First().Value));
                            List<String> creators = new List<string> { address };
                            if (objectinpector == null)
                            {
                                logstatus = "txid:" + transaction.TransactionId + " object creation failed due to invalid transaction format";
                                break;
                            }

                            foreach (int keywordId in objectinpector.cre)
                            {
                                {
                                    string creator = transaction.Keyword.ElementAt(keywordId).Key;
                                    creators.Add(creator);
                                }
                            }
                            //has proper authority to make OBJ changes
                            if (creators.Contains(transaction.SignedBy))
                            {
                                if (objectState.LockedDate == null)
                                {
                                    if (objectinpector.urn != null) { objectState.ChangeDate = transaction.BlockDate; objectState.URN = objectinpector.urn; }
                                    if (objectinpector.img != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Image = objectinpector.img; }
                                    if (objectinpector.nme != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Name = objectinpector.nme; }
                                    if (objectinpector.dsc != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Description = objectinpector.dsc; }
                                    if (objectinpector.lic != null) { objectState.ChangeDate = transaction.BlockDate; objectState.License = objectinpector.lic; }
                                    if (objectinpector.cre != null) { objectState.ChangeDate = transaction.BlockDate; objectState.Creators = creators; }
                                    if (objectinpector.own != null)
                                    {
                                        objectState.ChangeDate = transaction.BlockDate;
                                        foreach (var ownerId in objectinpector.own)
                                        {
                                            {
                                                string owner = transaction.Keyword.ElementAt(ownerId.Key).Key;
                                                objectState.Owners.Add(owner, ownerId.Value);
                                            }
                                        }
                                    }

                                    logstatus = "txid:" + transaction.TransactionId + " object creation failed due to invalid privlidges";

                                }
                            }
                            else
                            {
                                logstatus = "txid:" + transaction.TransactionId + " object creation failed due to invalid privlidges";
                            }
                            break;

                        case "GIV":
                            List<List<int>> givinspector = JsonConvert.DeserializeObject<List<List<int>>>(Encoding.ASCII.GetString(transaction.File.First().Value));
                            if (givinspector == null)
                            {
                                logstatus = "txid:" + transaction.TransactionId + " give failed due to invalid transaction format";
                                break;
                            }
                            int giveCount = 0;
                            foreach (var give in givinspector)
                            {
                                string giver = transaction.SignedBy;
                                string reciever = transaction.Keyword.ElementAt(give[0]).Key;
                                int qtyToGive = give[1];

                                if (qtyToGive < 1)
                                {
                                    logstatus = "txid:" + transaction.TransactionId + " give " + qtyToGive + "to" + reciever + "failed due to a qty of < = 0";
                                    break;
                                }

                                int qtyOwnedG=0;
                                if (!objectState.Owners.TryGetValue(transaction.SignedBy, out qtyOwnedG))
                                {
                                    //try grant access to object's self Owned qtyOwned to any creator
                                    int selfOwned;
                                    if (!objectState.Owners.TryGetValue(address, out selfOwned))
                                    {
                                        //Add Invalid trade attempt status
                                        logstatus = "txid:" + transaction.TransactionId + " give " + qtyToGive + " to " + reciever + " failed due to insufficent qty";
                                        break;
                                    }
                                    if (objectState.Creators.Contains(transaction.SignedBy)) {
                                        giver = address;
                                        qtyOwnedG = selfOwned; }

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
                                    int recieverOwned;
                                    if (!objectState.Owners.TryGetValue(reciever, out recieverOwned))
                                    {
                                        objectState.Owners.Add(reciever, qtyToGive);

                                    }
                                    else { objectState.Owners[reciever] = recieverOwned + qtyToGive; }
                                    logstatus = "txid:" + transaction.TransactionId + ":" + giveCount++ + " give " + qtyToGive + " to " + reciever + " succeded";
                                    using (var db = new DB(OBJ, @"root\log"))
                                    {
                                        db.Put(address + "!" + intProcessHeight + "!" + giveCount, logstatus);
                                    }
                                    objectState.ChangeDate = transaction.BlockDate;
                                    logstatus = "";
                                }
                                else
                                {
                                    //Add Invalid trade attempt status
                                    logstatus = "txid:" + transaction.TransactionId + " give " + qtyToGive + "to" + reciever + "failed due to insufficent qty of " + qtyOwnedG;
                                    break;
                                }



                            }
                            break;

                        case "BRN":

                            string burner = transaction.SignedBy;
                            int qtyToBurn = JsonConvert.DeserializeObject<int>(Encoding.ASCII.GetString(transaction.File.First().Value));


                            if (qtyToBurn < 1)
                            {
                                logstatus = "txid:" + transaction.TransactionId + " burn " + qtyToBurn + " failed due to a qty of < = 0";
                                break;
                            }

                            int qtyOwnedB;
                            if (!objectState.Owners.TryGetValue(transaction.SignedBy, out qtyOwnedB))
                            {
                                //try grant access to object's self Owned qtyOwned to any creator
                                int selfOwned;
                                if (!objectState.Owners.TryGetValue(address, out selfOwned))
                                {
                                    //Add Invalid trade attempt status
                                    logstatus = "txid:" + transaction.TransactionId + " burn " + qtyToBurn + " failed due to insufficent qty";
                                    break;
                                }
                                if (objectState.Creators.Contains(transaction.SignedBy))
                                {
                                    //assume self owned address identity if on creator list
                                    burner = address;
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
                                objectState.ChangeDate = transaction.BlockDate;
                                logstatus = "txid:" + transaction.TransactionId + " burn " + qtyToBurn + " of " + burner + " succeded";
                                using (var db = new DB(OBJ, @"root\log"))
                                {
                                    db.Put(address + "!" + intProcessHeight , logstatus);
                                }
                                logstatus = "";

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
                    using (var db = new DB(OBJ, @"root\log"))
                    {
                        db.Put(address + "!" + intProcessHeight, logstatus);
                    }
                }


            }

            //used to determine where to begin object State processing when retrieved from cache
            objectState.ProcessHeight = intProcessHeight;
            
            using (var db = new DB(OBJ, @"root\obj"))
            {
                db.Put(address, JsonConvert.SerializeObject(objectState));
            }

            return objectState;

        }
    }


}
