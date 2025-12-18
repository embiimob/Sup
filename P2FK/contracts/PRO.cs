
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
    public class PRO
    {
        public string urn { get; set; }
        public string dnm { get; set; }
        public string fnm { get; set; }
        public string mnm { get; set; }
        public string lnm { get; set; }
        public string sfx { get; set; }
        public string bio { get; set; }
        public string img { get; set; }
        public Dictionary<string, string> url { get; set; }
        public Dictionary<string, string> loc { get; set; }
        public string pkx { get; set; }
        public string pky { get; set; }
        public string[] cre { get; set; }


    }
    public class PROState
    {
        public int Id { get; set; }
        public string URN { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public Dictionary<string, string> URL { get; set; }
        public Dictionary<string, string> Location { get; set; }
        public string PKX { get; set; }
        public string PKY { get; set; }
        public List<string> Creators { get; set; }
        public int ProcessHeight { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ChangeDate { get; set; }
        private static char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

        public static PROState GetProfileByAddress(string profileaddress, string username, string password, string url, string versionByte = "111", bool verbose = false)
        {

            PROState profileState = new PROState();
           
            if (System.IO.File.Exists(@"root\" + profileaddress + @"\BLOCK")) { return new PROState { }; }
               
            string JSONOBJ;
            string logstatus;
            string diskpath = "root\\" + profileaddress + "\\";


            // fetch current JSONOBJ from disk if it exists
            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetProfileByAddress.json");
                profileState = JsonConvert.DeserializeObject<PROState>(JSONOBJ);
            }
            catch { }

            int intProcessHeight = 0;
            bool calculated = false;
            try { intProcessHeight = profileState.Id; } catch { }

            Root[] objectTransactions;

            if (verbose == true) { intProcessHeight = 0; profileState = new PROState(); }

            objectTransactions = Root.GetRootsByAddress(profileaddress, username, password, url, intProcessHeight, -1, versionByte, verbose);

            if (intProcessHeight != 0 && objectTransactions.Count() == 0)
            {
                try { File.Delete(@"GET_OBJECT_BY_ADDRESS"); } catch { }

                return profileState;
            }

            foreach (Root transaction in objectTransactions)
            {
                if (transaction.Id > intProcessHeight)
                {
                    calculated = true;
                    //ignore any transaction that is not signed
                    if (transaction.Signed && (transaction.File.ContainsKey("PRO")))
                    {
                        
                        intProcessHeight = transaction.Id;
                        profileState.ProcessHeight = intProcessHeight;


                        string sigSeen = null;

                        // Calculate SHA-256 hash of the signature
                        using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
                        {
                            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(transaction.Signature));
                            string hashedSignature = BitConverter.ToString(hashBytes).Replace("-", "");

                            string filePath = @"root\" + profileaddress + @"\sig\" + hashedSignature;

                            if (!System.IO.File.Exists(filePath))
                            {
                                if (!System.IO.Directory.Exists(@"root\" + profileaddress + @"\sig")) { Directory.CreateDirectory(@"root\" + profileaddress + @"\sig"); }
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


                            PRO profileinspector = null;
                            try
                            {
                                profileinspector = JsonConvert.DeserializeObject<PRO>(File.ReadAllText(@"root\" + transaction.TransactionId + @"\PRO"));
                            }
                            catch{}



                            if (profileinspector != null && profileState.Creators == null && transaction.SignedBy == profileaddress)
                            {

                                profileState.Creators = new List<string> { };

                                if (profileinspector.cre != null)
                                {
                                    foreach (string keywordId in profileinspector.cre)
                                    {
                                        if (int.TryParse(keywordId, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int intkey))
                                        {
                                            string creator = transaction.Keyword.Reverse().ElementAt(intkey).Key;

                                            if (!profileState.Creators.Contains(creator))
                                            {
                                                profileState.Creators.Add(creator);
                                            }
                                        }
                                        else
                                        {
                                            if (!profileState.Creators.Contains(keywordId))
                                            {
                                                profileState.Creators.Add(keywordId);
                                            }
                                        }

                                    }

                                }
                                else { profileState.Creators.Add(profileaddress); }
                                profileState.CreatedDate = transaction.BlockDate;
                                profileState.ChangeDate = transaction.BlockDate;
                                profileinspector.cre = null;
                            }


                            //has proper authority to make OBJ changes
                            if (profileinspector != null && profileState.Creators != null && profileState.Creators.Contains(transaction.SignedBy))
                            {
                                if (profileinspector.cre != null && profileinspector.cre.Contains(transaction.SignedBy))
                                {
                                    profileState.Creators.Clear();

                                    foreach (string keywordId in profileinspector.cre)
                                    {
                                        if (int.TryParse(keywordId, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int intkey))
                                        {
                                            string creator = transaction.Keyword.Reverse().ElementAt(intkey).Key;

                                            if (!profileState.Creators.Contains(creator))
                                            {
                                                profileState.Creators.Add(creator);
                                            }
                                        }
                                        else
                                        {
                                            if (!profileState.Creators.Contains(keywordId))
                                            {
                                                profileState.Creators.Add(keywordId);
                                            }
                                        }

                                    }


                                    profileState.ChangeDate = transaction.BlockDate;

                                }
                                if (profileinspector.urn != null) { profileState.ChangeDate = transaction.BlockDate; profileState.URN = profileinspector.urn; }
                                if (profileinspector.dnm != null) { profileState.ChangeDate = transaction.BlockDate; profileState.DisplayName = profileinspector.dnm; }
                                if (profileinspector.fnm != null) { profileState.ChangeDate = transaction.BlockDate; profileState.FirstName = profileinspector.fnm; }
                                if (profileinspector.mnm != null) { profileState.ChangeDate = transaction.BlockDate; profileState.MiddleName = profileinspector.mnm; }
                                if (profileinspector.lnm != null) { profileState.ChangeDate = transaction.BlockDate; profileState.LastName = profileinspector.lnm; }
                                if (profileinspector.sfx != null) { profileState.ChangeDate = transaction.BlockDate; profileState.Suffix = profileinspector.sfx; }
                                if (profileinspector.bio != null) { profileState.ChangeDate = transaction.BlockDate; profileState.Bio = profileinspector.bio; }
                                if (profileinspector.img != null) { profileState.ChangeDate = transaction.BlockDate; profileState.Image = profileinspector.img; }
                                if (profileinspector.url != null) { profileState.ChangeDate = transaction.BlockDate; profileState.URL = profileinspector.url; }
                                if (profileinspector.loc != null) { profileState.ChangeDate = transaction.BlockDate; profileState.Location = profileinspector.loc; }
                                if (profileinspector.pkx != null) { profileState.ChangeDate = transaction.BlockDate; profileState.PKX = profileinspector.pkx; }
                                if (profileinspector.pky != null) { profileState.ChangeDate = transaction.BlockDate; profileState.PKY = profileinspector.pky; }
                               

                               

                            }
                            

                        }
                        

                        
                    }

                }
                
            }

            if (calculated)
            {
                if (objectTransactions.Count() > 0)
                {
                    profileState.Id = objectTransactions.Max(max => max.Id);
                }

                var profileSerialized = JsonConvert.SerializeObject(profileState);

                try
                {
                    System.IO.File.WriteAllText(@"root\" + profileaddress + @"\" + "GetProfileByAddress.json", profileSerialized);
                }
                catch
                {

                    try
                    {
                        if (!Directory.Exists(@"root\" + profileaddress))

                        {
                            Directory.CreateDirectory(@"root\" + profileaddress);
                        }
                        System.IO.File.WriteAllText(@"root\" + profileaddress + @"\" + "GetProfileByAddress.json", profileSerialized);
                    }
                    catch { };
                }
            }

            return profileState;

        }
        
        public static PROState GetProfileByURN(string searchstring, string username, string password, string url, string versionByte = "111", bool verbose = false)
        {
            PROState profileState = new PROState { };
            string JSONOBJ;
            string profileaddress = Root.GetPublicAddressByKeyword(searchstring, versionByte);


            string diskpath = "root\\" + profileaddress + "\\";

            try
            {
                JSONOBJ = System.IO.File.ReadAllText(diskpath + "GetProfileByURN.json");
                profileState = JsonConvert.DeserializeObject<PROState>(JSONOBJ);
            }
            catch { }

            var intProcessHeight = profileState.Id;
            Root[] profileTransactions;

            if (verbose == true) { intProcessHeight = 0; profileState = new PROState(); }

            profileTransactions = Root.GetRootsByAddress(profileaddress, username, password, url, intProcessHeight, -1, versionByte, verbose);

            if (intProcessHeight > 0 && profileTransactions.Count() == 0) { return profileState; }


            HashSet<string> addedValues = new HashSet<string>();

            foreach (Root transaction in profileTransactions)
            {
                if (transaction.Id > intProcessHeight)
                {
                    intProcessHeight = transaction.Id;
                    //ignore any transaction that is not signed
                    if (transaction.Signed && transaction.File.ContainsKey("PRO") && ((profileState.Creators != null && profileState.Creators.Contains(transaction.SignedBy)) || profileState.Creators == null))
                    {
                        string findObject = transaction.Keyword.ElementAt(transaction.Keyword.Count - 1).Key;

                        if (!System.IO.File.Exists(@"root\" + findObject + @"\BLOCK"))
                        {


                            PROState isObject = GetProfileByAddress(findObject, username, password, url, versionByte);

                            if (isObject.URN != null && isObject.URN == searchstring && isObject.ChangeDate > DateTime.Now.AddYears(-10))
                            {
                                if (isObject.Creators.Contains(findObject))
                                {

                                    isObject.Id = profileTransactions.Max(max => max.Id);
                                    isObject.ProcessHeight = intProcessHeight;

                                    var profileSerialized = JsonConvert.SerializeObject(isObject);
                                    try
                                    {
                                        System.IO.File.WriteAllText(@"root\" + profileaddress + @"\" + "GetProfileByURN.json", profileSerialized);
                                    }
                                    catch
                                    {

                                        try
                                        {
                                            if (!Directory.Exists(@"root\" + profileaddress))

                                            {
                                                Directory.CreateDirectory(@"root\" + profileaddress);
                                            }
                                            System.IO.File.WriteAllText(@"root\" + profileaddress + @"\" + "GetProfileByURN.json", profileSerialized);
                                        }
                                        catch { };
                                    }


                                    return isObject;

                                }


                            }

                        }
                    }
                }

            }

            return profileState;

        }

        public static List<PROState> GetLocalProfiles(string username, string password, string url, string versionByte = "111", bool verbose = false)
        {
            List<PROState> profileStates = new List<PROState> { };
            string JSONOBJ;

            if (!verbose)
            {
                // fetch current JSONOBJ from disk if it exists
                try
                {
                    JSONOBJ = System.IO.File.ReadAllText(@"root\GetLocalProfiles.json");
                    profileStates = JsonConvert.DeserializeObject<List<PROState>>(JSONOBJ);
                    return profileStates;
                }
                catch { }
            }
            
            // Check if API mode is enabled
            if (BitcoinBackendFactory.IsApiModeEnabled())
            {
                // GetLocalProfiles requires wallet access (listreceivedbyaddress) which is not available in API mode
                return profileStates;
            }
            
            NetworkCredential credentials = new NetworkCredential(username, password);
            NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(url), Network.Main);

            string addressesString = "";
            try
            {
                addressesString = rpcClient.SendCommand("listreceivedbyaddress").ResultString;
            }
            catch
            {
                return profileStates;
            }

            var addresses = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(addressesString);

            List<string> bitcoinAddresses = new List<string>();

            foreach (var addressInfo in addresses)
            {
                string address = addressInfo["address"].ToString();
                bitcoinAddresses.Add(address);
            }


            foreach (string address in bitcoinAddresses)
            {
                PROState isObject = GetProfileByAddress(address, username, password, url, versionByte);

                if (isObject.URN != null && isObject.Creators != null)
                {
                    PROState activeprofile = PROState.GetProfileByURN(isObject.URN, username, password, url, versionByte);
                    
                    if (string.Concat(activeprofile.Creators) == string.Concat(isObject.Creators))
                    {
                        profileStates.Add(activeprofile);
                    }
                }
            }

            var profileSerialized = JsonConvert.SerializeObject(profileStates);
            try
            {
                System.IO.File.WriteAllText(@"root\GetLocalProfiles.json", profileSerialized);
            }
            catch
            {

                try
                {
                    if (!Directory.Exists(@"root"))

                    {
                        Directory.CreateDirectory(@"root");
                    }
                    System.IO.File.WriteAllText(@"root\GetLocalProfiles.json", profileSerialized);
                }
                catch { };
            }
            var sortedprofileStatese = profileStates.OrderBy(urn => urn.URN);
            return sortedprofileStatese.ToList();

        }
       

    }
}



