using LevelDB;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NBitcoin;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using NBitcoin.RPC;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SUP
{

    public partial class ObjectBrowser : Form
    {
        private readonly string _objectaddress;
        private List<String> SearchHistory = new List<String>();
        private int SearchId = 0;
        private int isBuildingCounter = 0;
        private HashSet<string> loadedObjects = new HashSet<string>();
        private readonly static object levelDBLocker = new object();
        private List<string> BTCMemPool = new List<string>();
        private List<string> BTCTMemPool = new List<string>();
        private List<string> MZCMemPool = new List<string>();
        private List<string> LTCMemPool = new List<string>();
        private List<string> DOGMemPool = new List<string>();
        public ObjectBrowser(string objectaddress)
        {
            InitializeComponent();
            if (objectaddress != null)
            {
                _objectaddress = objectaddress;
            }
            else
            { _objectaddress = ""; }

        }

        private void GetObjectsbyAddress(string address)
        {


            string profileCheck = address;
            PROState searchprofile = PROState.GetProfileByAddress(address, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (searchprofile.URN != null)
            {
                linkLabel1.Text = searchprofile.URN;
                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(address, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                if (searchprofile.URN != null)
                {
                    linkLabel1.Text = TruncateAddress(searchprofile.Creators.First());
                    linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
                    profileCheck = searchprofile.Creators.First();
                }
                else
                {
                    linkLabel1.Text = "anon";
                    linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;

                }
            }


            List<OBJState> createdObjects = new List<OBJState>();

            int loadQty = ((flowLayoutPanel1.Size.Width / 208) * (flowLayoutPanel1.Size.Height / 309) + ((flowLayoutPanel1.Size.Width / 208) * 2) + ((flowLayoutPanel1.Size.Height / 309) * 2));

            if (loadQty > 1)
            {

                txtQty.Text = (flowLayoutPanel1.Controls.Count + loadQty).ToString();

                flowLayoutPanel1.SuspendLayout();

                if (btnCreated.BackColor == Color.Yellow)
                {

                    createdObjects = OBJState.GetObjectsCreatedByAddress(profileCheck, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtLast.Text), int.Parse(txtQty.Text));

                }
                else if (btnOwned.BackColor == Color.Yellow)
                {
                    createdObjects = OBJState.GetObjectsOwnedByAddress(profileCheck, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtLast.Text), int.Parse(txtQty.Text));
                }
                else
                {
                    if (txtSearchAddress.Text == "")
                    {

                        createdObjects = OBJState.GetFoundObjects(txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtLast.Text), int.Parse(txtQty.Text));
                        if (btnLive.BackColor == Color.Blue) { createdObjects.Reverse(); }

                    }
                    else
                    {
                        createdObjects = OBJState.GetObjectsByAddress(profileCheck, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtLast.Text), int.Parse(txtQty.Text));

                    }
                }


                foreach (OBJState objstate in createdObjects)
                {

                    if (objstate.Owners != null)
                    {

                        FoundObjectControl foundObject = new FoundObjectControl();

                        switch (objstate.Image.ToUpper().Substring(0, 4))
                        {
                            case "BTC:":
                                string transid = objstate.Image.Substring(4, 64);
                                if (!System.IO.Directory.Exists("root/" + transid))
                                {
                                    Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                                }
                                foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                                break;
                            case "MZC:":
                                transid = objstate.Image.Substring(4, 64);
                                if (!System.IO.Directory.Exists("root/" + transid))
                                {
                                    Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                                }
                                foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("MZC:", @"root/");
                                break;
                            case "LTC:":
                                transid = objstate.Image.Substring(4, 64);
                                if (!System.IO.Directory.Exists("root/" + transid))
                                {
                                    Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:9332", "48");
                                }
                                foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("LTC:", @"root/");
                                break;
                            case "DOG:":
                                transid = objstate.Image.Substring(4, 64);
                                if (!System.IO.Directory.Exists("root/" + transid))
                                {
                                    Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:22555", "30");
                                }
                                foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("DOG:", @"root/");
                                break;
                            case "DTC:":
                                transid = objstate.Image.Substring(4, 64);
                                if (!System.IO.Directory.Exists("root/" + transid))
                                {
                                    Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:11777", "30");
                                }
                                foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("DTC:", @"root/");
                                break;
                            case "IPFS":
                                transid = objstate.Image.Substring(5, 46);

                                if (!System.IO.Directory.Exists("ipfs/" + transid))
                                {
                                    Process process2 = new Process();
                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                    process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                                    process2.StartInfo.UseShellExecute = false;
                                    process2.StartInfo.CreateNoWindow = true;
                                    process2.Start();
                                    process2.WaitForExit();
                                    string fileName;
                                    if (System.IO.File.Exists("ipfs/" + transid))
                                    {
                                        System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                        System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                        fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                        System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                                    }


                                    var SUP = new Options { CreateIfMissing = true };
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(SUP, @"ipfs"))
                                        {

                                            string ipfsdaemon = db.Get("ipfs-daemon");

                                            if (ipfsdaemon == "true")
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

                                    }

                                }
                                if (objstate.Image.Length == 51)
                                { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                                else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }

                                break;
                            case "HTTP":
                                foundObject.ObjectImage.ImageLocation = objstate.Image;
                                break;


                            default:
                                transid = objstate.Image.Substring(0, 64);
                                if (!System.IO.Directory.Exists("root/" + transid))
                                {
                                    Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                                }
                                foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                                break;
                        }

                        foundObject.ObjectName.Text = objstate.Name;
                        foundObject.ObjectDescription.Text = objstate.Description;
                        foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                        foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";

                        foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                        {

                            if (creator.Value.Year > 1)
                            {
                                PROState profile = PROState.GetProfileByAddress(creator.Key, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                                if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                                {


                                    foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                    foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                }
                                else
                                {


                                    if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                                    {
                                        foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                        foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                    }

                                }
                            }
                            else
                            {

                                if (foundObject.ObjectCreators.Text == "")
                                {


                                    foundObject.ObjectCreators.Text = TruncateAddress(creator.Key);
                                    foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                }
                                else
                                {


                                    if (foundObject.ObjectCreators2.Text == "")
                                    {
                                        foundObject.ObjectCreators2.Text = TruncateAddress(creator.Key);
                                        foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                    }

                                }

                            }

                        }
                        foundObject.ObjectId.Text = objstate.Id.ToString();


                        if (!loadedObjects.Contains(foundObject.ObjectAddress.Text))
                        {
                            txtLast.Text = objstate.Id.ToString();
                            OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                            if (isOfficial.URN != null)
                            {
                                if (isOfficial.Creators.First().Key == foundObject.ObjectAddress.Text)
                                {
                                    foundObject.lblOfficial.Visible = true;
                                    foundObject.lblOfficial.Text = TruncateAddress(isOfficial.URN);
                                }
                                else
                                {
                                    foundObject.txtOfficialURN.Text = isOfficial.Creators.First().Key;
                                    foundObject.btnOfficial.Visible = true;

                                }
                            }



                            flowLayoutPanel1.Controls.Add(foundObject);
                            if (btnLive.BackColor == Color.Blue) { flowLayoutPanel1.Controls.SetChildIndex(foundObject, 0); }


                        }
                        loadedObjects.Add(foundObject.ObjectAddress.Text);


                    }
                }

                flowLayoutPanel1.ResumeLayout();
            }
        }

        private void GetObjectsByKeyword(string keyword)
        {

            List<string> keywords = keyword.Split(',').ToList();
            List<OBJState> createdObjects = new List<OBJState>();

            flowLayoutPanel1.SuspendLayout();

            int loadQty = (flowLayoutPanel1.Size.Width / 100) * (flowLayoutPanel1.Size.Height / 200) + (flowLayoutPanel1.Size.Width / 100);

            loadQty -= flowLayoutPanel1.Controls.Count;

            txtQty.Text = loadQty.ToString();
            if (loadQty > 0)
            {
                createdObjects = OBJState.GetObjectsByKeyword(keywords, txtLogin.Text, txtPassword.Text, txtUrl.Text, txtVersionByte.Text, int.Parse(txtLast.Text), int.Parse(txtQty.Text));
            }

            foreach (OBJState objstate in createdObjects)
            {
                if (objstate.Owners != null)
                {

                    FoundObjectControl foundObject = new FoundObjectControl();


                    switch (objstate.Image.ToUpper().Substring(0, 4))
                    {
                        case "BTC:":
                            string transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                            break;
                        case "MZC:":
                            transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("MZC:", @"root/");
                            break;
                        case "LTC:":
                            transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:9332", "48");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("LTC:", @"root/");
                            break;
                        case "DOG:":
                            transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:22555", "30");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("DOG:", @"root/");
                            break;
                        case "DTC:":
                            transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:11777", "30");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("DTC:", @"root/");
                            break;
                        case "IPFS":
                            transid = objstate.Image.Substring(5, 46);

                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();
                                string fileName;
                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                                }


                                var SUP = new Options { CreateIfMissing = true };
                                lock (levelDBLocker)
                                {
                                    using (var db = new DB(SUP, @"ipfs"))
                                    {

                                        string ipfsdaemon = db.Get("ipfs-daemon");

                                        if (ipfsdaemon == "true")
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
                                }
                            }
                            if (objstate.Image.Length == 51)
                            { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                            else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }

                            break;
                        case "HTTP":
                            foundObject.ObjectImage.ImageLocation = objstate.Image;
                            break;


                        default:
                            transid = objstate.Image.Substring(0, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                            }
                            foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                            break;
                    }
                    foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;
                    foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";

                    foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                    {

                        if (creator.Value.Year > 1)
                        {
                            PROState profile = PROState.GetProfileByAddress(creator.Key, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                            if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                            {


                                foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                            }
                            else
                            {


                                if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                                {
                                    foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                    foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);

                                }

                            }
                        }
                        else
                        {

                            if (foundObject.ObjectCreators.Text == "")
                            {


                                foundObject.ObjectCreators.Text = TruncateAddress(creator.Key);

                                foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);

                            }
                            else
                            {


                                if (foundObject.ObjectCreators2.Text == "")
                                {
                                    foundObject.ObjectCreators2.Text = TruncateAddress(creator.Key);

                                    foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);

                                }

                            }

                        }

                    }
                    foundObject.ObjectId.Text = objstate.Id.ToString();
                    if (!loadedObjects.Contains(foundObject.ObjectAddress.Text))
                    {
                        txtLast.Text = objstate.Id.ToString();
                        OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                        if (isOfficial.URN != null)
                        {
                            if (isOfficial.Creators.First().Key == foundObject.ObjectAddress.Text)
                            {
                                foundObject.lblOfficial.Visible = true;
                                foundObject.lblOfficial.Text = TruncateAddress(isOfficial.URN);

                            }
                            else
                            {

                                foundObject.txtOfficialURN.Text = isOfficial.Owners.First().Key;
                                foundObject.btnOfficial.Visible = true;

                            }
                        }
                        loadedObjects.Add(foundObject.ObjectAddress.Text);
                        flowLayoutPanel1.Controls.Add(foundObject);


                    }

                }
            }
            flowLayoutPanel1.ResumeLayout();

        }

        private void GetObjectsByURN(string urn)
        {

            flowLayoutPanel1.Controls.Clear();

            int loadQty = (flowLayoutPanel1.Size.Width / 100) * (flowLayoutPanel1.Size.Height / 200) + 3;

            loadQty -= flowLayoutPanel1.Controls.Count;

            txtQty.Text = loadQty.ToString();


            OBJState objstate = OBJState.GetObjectByURN(urn, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (objstate.Owners != null)
            {

                FoundObjectControl foundObject = new FoundObjectControl();

                switch (objstate.Image.ToUpper().Substring(0, 4))
                {
                    case "BTC:":
                        string transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                        break;
                    case "MZC:":
                        transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("MZC:", @"root/");
                        break;
                    case "LTC:":
                        transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:9332", "48");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("LTC:", @"root/");
                        break;
                    case "DOG:":
                        transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:22555", "30");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("DOG:", @"root/");
                        break;
                    case "DTC:":
                        transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:11777", "30");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("DTC:", @"root/");
                        break;
                    case "IPFS":
                        transid = objstate.Image.Substring(5, 46);

                        if (!System.IO.Directory.Exists("ipfs/" + transid))
                        {
                            Process process2 = new Process();
                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                            process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                            process2.StartInfo.UseShellExecute = false;
                            process2.StartInfo.CreateNoWindow = true;
                            process2.Start();
                            process2.WaitForExit();

                            if (System.IO.File.Exists("ipfs/" + transid))
                            {
                                System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                string fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                            }


                            var SUP = new Options { CreateIfMissing = true };
                            lock (levelDBLocker)
                            {
                                using (var db = new DB(SUP, @"ipfs"))
                                {

                                    string ipfsdaemon = db.Get("ipfs-daemon");

                                    if (ipfsdaemon == "true")
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
                            }

                        }
                        if (objstate.Image.Length == 51)
                        { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                        else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }

                        break;
                    case "HTTP":
                        foundObject.ObjectImage.ImageLocation = objstate.Image;
                        break;


                    default:
                        transid = objstate.Image.Substring(0, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                        }
                        foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                        break;
                }
                foundObject.ObjectName.Text = objstate.Name;
                foundObject.ObjectDescription.Text = objstate.Description;
                foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                {
                    if (creator.Value.Year > 1)
                    {
                        PROState profile = PROState.GetProfileByAddress(creator.Key, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                        if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                        {


                            foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                            foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                        }
                        else
                        {


                            if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                            {
                                foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                            }

                        }
                    }
                    else
                    {

                        if (foundObject.ObjectCreators.Text == "")
                        {


                            foundObject.ObjectCreators.Text = TruncateAddress(creator.Key);
                            foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                        }
                        else
                        {


                            if (foundObject.ObjectCreators2.Text == "")
                            {
                                foundObject.ObjectCreators2.Text = TruncateAddress(creator.Key);
                                foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                            }

                        }

                    }


                }
                txtLast.Text = objstate.Id.ToString();
                foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");

                if (isOfficial.URN != null)
                {
                    if (isOfficial.Creators.ContainsKey(foundObject.ObjectAddress.Text))
                    {
                        foundObject.lblOfficial.Visible = true;
                        foundObject.lblOfficial.Text = TruncateAddress(isOfficial.URN);
                    }
                    else
                    {
                        foundObject.txtOfficialURN.Text = isOfficial.Owners.First().Key;
                        foundObject.btnOfficial.Visible = true;

                    }
                }


                flowLayoutPanel1.Controls.Add(foundObject);

            }
        }

        private void GetObjectByFile(string filePath)
        {

            flowLayoutPanel1.Controls.Clear();
            int loadQty = (flowLayoutPanel1.Size.Width / 100) * (flowLayoutPanel1.Size.Height / 200) + 3;

            loadQty -= flowLayoutPanel1.Controls.Count;

            txtQty.Text = loadQty.ToString();

            OBJState objstate = OBJState.GetObjectByFile(filePath, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (objstate.Owners != null)
            {

                FoundObjectControl foundObject = new FoundObjectControl();

                switch (objstate.Image.ToUpper().Substring(0, 4))
                {
                    case "BTC:":
                        string transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                        break;
                    case "MZC:":
                        transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("MZC:", @"root/");
                        break;
                    case "LTC:":
                        transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:9332", "48");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("LTC:", @"root/");
                        break;
                    case "DOG:":
                        transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:22555", "30");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("DOG:", @"root/");
                        break;
                    case "DTC:":
                        transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:11777", "30");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("DTC:", @"root/");
                        break;
                    case "IPFS":
                        transid = objstate.Image.Substring(5, 46);

                        if (!System.IO.Directory.Exists("ipfs/" + transid))
                        {
                            Process process2 = new Process();
                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                            process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                            process2.StartInfo.UseShellExecute = false;
                            process2.StartInfo.CreateNoWindow = true;
                            process2.Start();
                            process2.WaitForExit();

                            if (System.IO.File.Exists("ipfs/" + transid))
                            {
                                System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                string fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                            }



                            var SUP = new Options { CreateIfMissing = true };
                            lock (levelDBLocker)
                            {
                                using (var db = new DB(SUP, @"ipfs"))
                                {

                                    string ipfsdaemon = db.Get("ipfs-daemon");

                                    if (ipfsdaemon == "true")
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
                            }

                        }
                        if (objstate.Image.Length == 51)
                        { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                        else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }

                        break;
                    case "HTTP":
                        foundObject.ObjectImage.ImageLocation = objstate.Image;
                        break;


                    default:
                        transid = objstate.Image.Substring(0, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                        }
                        foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                        break;
                }
                foundObject.ObjectName.Text = objstate.Name;
                foundObject.ObjectDescription.Text = objstate.Description;
                foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                {


                    if (creator.Value.Year > 1)
                    {
                        PROState profile = PROState.GetProfileByAddress(creator.Key, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                        if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                        {


                            foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                            foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                        }
                        else
                        {


                            if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                            {
                                foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                            }

                        }
                    }
                    else
                    {

                        if (foundObject.ObjectCreators.Text == "")
                        {


                            foundObject.ObjectCreators.Text = TruncateAddress(creator.Key);
                            foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                        }
                        else
                        {


                            if (foundObject.ObjectCreators2.Text == "")
                            {
                                foundObject.ObjectCreators2.Text = TruncateAddress(creator.Key);
                                foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                            }

                        }

                    }


                }
                foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                if (isOfficial.URN != null)
                {
                    if (isOfficial.Creators.First().Key == foundObject.ObjectAddress.Text)
                    {
                        foundObject.lblOfficial.Visible = true;

                        foundObject.lblOfficial.Text = TruncateAddress(isOfficial.URN);
                    }
                    else
                    {
                        foundObject.txtOfficialURN.Text = isOfficial.Owners.First().Key;
                        foundObject.btnOfficial.Visible = true;
                    }
                }

                flowLayoutPanel1.Controls.Add(foundObject);
            }

        }

        private void ButtonGetOwnedClick(object sender, EventArgs e)
        {

            btnOwned.BackColor = Color.Yellow;
            btnCreated.BackColor = Color.White;

            BuildSearchResults();


        }

        private void ButtonGetCreatedClick(object sender, EventArgs e)
        {

            btnCreated.BackColor = Color.Yellow;
            btnOwned.BackColor = Color.White;

            BuildSearchResults();

        }

        private void MainUserNameClick(object sender, LinkLabelLinkClickedEventArgs e)
        {



            PROState searchprofile = PROState.GetProfileByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (searchprofile.URN != null)
            {
                txtSearchAddress.Text = searchprofile.URN;
                linkLabel1.Text = TruncateAddress(searchprofile.Creators.First());

                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                if (searchprofile.URN != null)
                {
                    txtSearchAddress.Text = searchprofile.Creators.First();
                    linkLabel1.Text = searchprofile.URN;
                    linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;

                }
                else
                {
                    linkLabel1.Text = "anon";
                    linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;

                }
            }




        }

        private void SearchAddressKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                btnOwned.BackColor = Color.White;
                btnCreated.BackColor = Color.White;

                BuildSearchResults();

            }

        }

        private async void BuildSearchResults()
        {


            string isBuilding;

            lock (levelDBLocker)
            {
                var MUTE = new Options { CreateIfMissing = true };
                using (var db = new DB(MUTE, @"root\sup"))
                {
                    isBuilding = db.Get("isBuilding");
                }
            }


            if (isBuilding != "true")
            {

                lock (levelDBLocker)
                {
                    var MUTE = new Options { CreateIfMissing = true };
                    using (var db = new DB(MUTE, @"root\sup"))
                    {
                        db.Put("isBuilding", "true");
                    }
                }
                flowLayoutPanel1.Controls.Clear();

                loadedObjects.Clear();

                txtLast.Text = "0";
                int loadQty = (flowLayoutPanel1.Size.Width / 216) * (flowLayoutPanel1.Size.Height / 333);
                loadQty -= flowLayoutPanel1.Controls.Count - ((flowLayoutPanel1.Size.Width / 216));
                txtQty.Text = loadQty.ToString();



                if (SearchId == SearchHistory.Count)
                {
                    SearchHistory.Add(txtSearchAddress.Text);
                    SearchId++;
                }
                else
                {

                    if (SearchId > SearchHistory.Count - 1) { SearchId = SearchHistory.Count - 1; }
                    SearchHistory[SearchId] = txtSearchAddress.Text;

                }



                if (txtSearchAddress.Text.ToLower().StartsWith("http"))
                {
                    flowLayoutPanel1.Controls.Clear();
                    flowLayoutPanel1.AutoScroll = false;
                    var webBrowser1 = new Microsoft.Web.WebView2.WinForms.WebView2();
                    webBrowser1.Size = flowLayoutPanel1.Size;
                    flowLayoutPanel1.Controls.Add(webBrowser1);

                    await webBrowser1.EnsureCoreWebView2Async();
                    webBrowser1.CoreWebView2.Navigate(txtSearchAddress.Text);
                }
                else
                {

                    flowLayoutPanel1.AutoScroll = true;


                    if (txtSearchAddress.Text.StartsWith("#"))
                    {

                        GetObjectsByKeyword(txtSearchAddress.Text.Replace("#", ""));


                    }
                    else
                    {

                        if (txtSearchAddress.Text.ToLower().StartsWith(@"ipfs:") && txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Length >= 51)
                        {
                            string ipfsHash = txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Substring(5, 46);

                            if (!System.IO.Directory.Exists("ipfs/" + ipfsHash))
                            {

                                var SUP = new Options { CreateIfMissing = true };
                                string isLoading;
                                lock (levelDBLocker)
                                {
                                    using (var db = new DB(SUP, @"ipfs"))
                                    {
                                        isLoading = db.Get(ipfsHash);

                                    }
                                }

                                if (isLoading != "loading")
                                {
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(SUP, @"ipfs"))
                                        {

                                            db.Put(ipfsHash, "loading");

                                        }
                                    }
                                    Task ipfsTask = Task.Run(() =>
                                {
                                    Process process2 = new Process();
                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                    process2.StartInfo.Arguments = "get " + ipfsHash + @" -o ipfs\" + ipfsHash;
                                    process2.StartInfo.UseShellExecute = false;
                                    process2.StartInfo.CreateNoWindow = true;
                                    process2.Start();
                                    process2.WaitForExit();

                                    if (System.IO.File.Exists("ipfs/" + ipfsHash))
                                    {
                                        System.IO.File.Move("ipfs/" + ipfsHash, "ipfs/" + ipfsHash + "_tmp");
                                        System.IO.Directory.CreateDirectory("ipfs/" + ipfsHash);
                                        string fileName = txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                        System.IO.File.Move("ipfs/" + ipfsHash + "_tmp", @"ipfs/" + ipfsHash + @"/" + fileName);

                                    }


                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(SUP, @"ipfs"))
                                        {

                                            string ipfsdaemon = db.Get("ipfs-daemon");

                                            if (ipfsdaemon == "true")
                                            {
                                                Process process3 = new Process
                                                {
                                                    StartInfo = new ProcessStartInfo
                                                    {
                                                        FileName = @"ipfs\ipfs.exe",
                                                        Arguments = "pin add " + ipfsHash,
                                                        UseShellExecute = false,
                                                        CreateNoWindow = true
                                                    }
                                                };
                                                process3.Start();
                                            }
                                        }
                                    }


                                    if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash))
                                    {
                                        Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash);
                                    }
                                    else { Label filenotFound = new Label(); filenotFound.AutoSize = true; filenotFound.Text = "IPFS: Search failed! Verify IPFS pinning is enbaled"; flowLayoutPanel1.Controls.Clear(); flowLayoutPanel1.Controls.Add(filenotFound); }
                                    lock (levelDBLocker)
                                    {
                                        using (var db = new DB(SUP, @"ipfs"))
                                        {
                                            db.Delete(ipfsHash);

                                        }
                                    }
                                });
                                }
                            }
                            else
                            {

                                Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash);
                            }


                        }
                        else
                        {
                            if (txtSearchAddress.Text.StartsWith(@"sup:"))
                            {
                                GetObjectsByURN(txtSearchAddress.Text.Replace("sup:", "").Replace(@"\\", "").Replace(@"//", ""));
                            }
                            else
                            {

                                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");

                                if (txtSearchAddress.Text.Count() > 64 && regexTransactionId.IsMatch(txtSearchAddress.Text) && txtSearchAddress.Text.Contains(".htm"))
                                {
                                    switch (txtSearchAddress.Text.Substring(0, 4))
                                    {
                                        case "MZC:":
                                            Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                                            break;
                                        case "BTC:":
                                            Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                                            break;
                                        case "LTC:":
                                            Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:9332", "48");
                                            break;
                                        case "DOG:":
                                            Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:22555", "30");
                                            break;
                                        case "DTC:":
                                            Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:11777", "30");
                                            break;
                                        default:
                                            Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(0, 64), txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                                            break;
                                    }
                                    Match match = regexTransactionId.Match(txtSearchAddress.Text);
                                    string browserPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + txtSearchAddress.Text.Replace("MZC:", "").Replace("BTC:", "");
                                    browserPath = @"file:///" + browserPath.Replace(@"\", @"/");
                                    flowLayoutPanel1.Controls.Clear();
                                    flowLayoutPanel1.AutoScroll = false;
                                    var webBrowser1 = new Microsoft.Web.WebView2.WinForms.WebView2();
                                    webBrowser1.Size = flowLayoutPanel1.Size;
                                    flowLayoutPanel1.Controls.Add(webBrowser1);

                                    await webBrowser1.EnsureCoreWebView2Async();
                                    webBrowser1.CoreWebView2.Navigate(browserPath.Replace(@"/", @"\"));
                                }
                                else
                                {

                                    GetObjectsbyAddress(txtSearchAddress.Text.Replace("@", ""));

                                }

                            }
                        }
                    }
                }
                lock (levelDBLocker)
                {
                    var MUTE = new Options { CreateIfMissing = true };
                    using (var db = new DB(MUTE, @"root\sup"))
                    {
                        db.Delete("isBuilding");
                    }
                }
            }

        }


        private void SearchAddressUpdate()
        {
            string isBuilding;

            lock (levelDBLocker)
            {
                var MUTE = new Options { CreateIfMissing = true };
                using (var db = new DB(MUTE, @"root\sup"))
                {
                    isBuilding = db.Get("isBuilding");
                }
            }

            if (isBuilding != "true")
            {
                lock (levelDBLocker)
                {
                    var MUTE = new Options { CreateIfMissing = true };
                    using (var db = new DB(MUTE, @"root\sup"))
                    {
                        db.Put("isBuilding", "true");
                    }
                }


                if (txtSearchAddress.Text.StartsWith("#"))
                {
                    GetObjectsByKeyword(txtSearchAddress.Text.Replace("#", ""));

                }
                else
                {

                    GetObjectsbyAddress(txtSearchAddress.Text.Replace("@", ""));

                }

                lock (levelDBLocker)
                {
                    var MUTE = new Options { CreateIfMissing = true };
                    using (var db = new DB(MUTE, @"root\sup"))
                    {
                        db.Delete("isBuilding");
                    }
                }

            }

        }

        private void ObjectBrowserLoad(object sender, EventArgs e)
        {

            Form parentForm = this.Owner;
            bool isBlue = false;

            // Check if the parent form has a button named "btnLive" with blue background color
            try
            {
                isBlue = parentForm.Controls.OfType<Button>().Any(b => b.Name == "btnLive" && b.BackColor == System.Drawing.Color.Blue);
            }
            catch { }

            if (isBlue)
            {
                // If there is a button with blue background color, show a message box
                DialogResult result = MessageBox.Show("disable Live monitoring to browse sup!? objects.\r\nignoring this warning may cause temporary data corruption that could require a full purge of the cache", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    // If the user clicks OK, close the form
                    this.Close();
                }
            }
            else
            {


                string isBuilding;

                lock (levelDBLocker)
                {
                    var MUTE = new Options { CreateIfMissing = true };
                    using (var db = new DB(MUTE, @"root\sup"))
                    {
                        isBuilding = db.Get("isBuilding");
                    }
                }


                if (isBuilding != "true")
                {
                    lock (levelDBLocker)
                    {
                        var MUTE = new Options { CreateIfMissing = true };
                        using (var db = new DB(MUTE, @"root\sup"))
                        {
                            db.Put("isBuilding", "true");
                        }
                    }
                    if (_objectaddress.Length > 0)
                    {
                        txtSearchAddress.Text = _objectaddress;

                        if (txtSearchAddress.Text.StartsWith("#"))
                        {
                            GetObjectsByKeyword(txtSearchAddress.Text.Replace("#", ""));

                        }
                        else
                            GetObjectsbyAddress(txtSearchAddress.Text.Replace("@", ""));
                    }
                    else
                    {
                        var SUP = new Options { CreateIfMissing = true };
                        lock (levelDBLocker)
                        {
                            using (var db = new DB(SUP, @"ipfs"))
                            {

                                string ipfsdaemon = db.Get("ipfs-daemon");

                                if (ipfsdaemon == "true")
                                {

                                    var process = new Process
                                    {
                                        StartInfo = new ProcessStartInfo
                                        {
                                            FileName = @"ipfs\ipfs.exe",
                                            Arguments = "daemon",
                                            UseShellExecute = false,
                                            CreateNoWindow = true
                                        }
                                    };
                                    process.Start();
                                }

                            }
                        }
                    }

                    lock (levelDBLocker)
                    {
                        var MUTE = new Options { CreateIfMissing = true };
                        using (var db = new DB(MUTE, @"root\sup"))
                        {
                            db.Delete("isBuilding");
                        }
                    }
                }
                else
                {
                    lock (levelDBLocker)
                    {
                        var MUTE = new Options { CreateIfMissing = true };
                        using (var db = new DB(MUTE, @"root\sup"))
                        {
                            db.Delete("isBuilding");
                        }
                    }
                }
            }
        }

        private void ButtonLoadWorkBench(object sender, EventArgs e)
        {
            new WorkBench().Show();
        }

        private void ButtonLoadConnections(object sender, EventArgs e)
        {
            new Connections().Show();
        }

        string TruncateAddress(string input)
        {
            if (input.Length <= 13)
            {
                return input;
            }
            else
            {
                return input.Substring(0, 5) + "..." + input.Substring(input.Length - 5);
            }
        }

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
           

        }


        private void btnHistoryBack_Click(object sender, EventArgs e)
        {

            if (SearchHistory.Count - 1 > 0)
            {
                SearchId--;
                if (SearchId < 0) { SearchId = 0; }

                txtSearchAddress.Text = SearchHistory[SearchId].ToString();
                txtSearchAddress.Focus();
                SendKeys.SendWait("{Enter}");

            }
        }

        private void btnHistoryForward_Click(object sender, EventArgs e)
        {
            if (SearchHistory.Count - 1 > SearchId)
            {
                SearchId++;
                txtSearchAddress.Text = SearchHistory[SearchId].ToString();
                txtSearchAddress.Focus();
                SendKeys.SendWait("{Enter}");
            }
        }

        private void btnMint_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Check if the data being dragged is a file
            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
            {
                // Allow the drop operation
                e.Effect = System.Windows.Forms.DragDropEffects.Copy;
            }
            else
            {
                // Prevent the drop operation
                e.Effect = System.Windows.Forms.DragDropEffects.None;
            }
        }

        private void flowLayoutPanel1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string isBuilding;

            lock (levelDBLocker)
            {
                var MUTE = new Options { CreateIfMissing = true };
                using (var db = new DB(MUTE, @"root\sup"))
                {
                    isBuilding = db.Get("isBuilding");
                }
            }
            if (isBuilding != "true")
            {
                lock (levelDBLocker)
                {
                    var MUTE = new Options { CreateIfMissing = true };
                    using (var db = new DB(MUTE, @"root\sup"))
                    {
                        db.Put("isBuilding", "true");
                    }
                }
                // Get the file paths of the files being dropped
                string[] filePaths = (string[])e.Data.GetData((System.Windows.Forms.DataFormats.FileDrop));
                string filePath = filePaths[0];

                GetObjectByFile(filePath);

                lock (levelDBLocker)
                {
                    var MUTE = new Options { CreateIfMissing = true };
                    using (var db = new DB(MUTE, @"root\sup"))
                    {
                        db.Delete("isBuilding");
                    }
                }
            }

        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {
            if (flowLayoutPanel1.VerticalScroll.Value + flowLayoutPanel1.VerticalScroll.LargeChange >= flowLayoutPanel1.VerticalScroll.Maximum)
            {

                if (btnLive.BackColor != Color.Blue)
                {
                    int loadQty = ((flowLayoutPanel1.Size.Width / 208) * (flowLayoutPanel1.Size.Height / 309) + ((flowLayoutPanel1.Size.Width / 208) * 2) + ((flowLayoutPanel1.Size.Height / 309) * 2));

                    if (loadQty > 1)
                    {
                        SearchAddressUpdate();
                    };
                }
            }

        }
        private void flowLayoutPanel1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Check if the scrollbar position is at the bottom
            if (flowLayoutPanel1.VerticalScroll.Value + flowLayoutPanel1.VerticalScroll.LargeChange >= flowLayoutPanel1.VerticalScroll.Maximum)
            {
                if (btnLive.BackColor != Color.Blue)
                {
                    int loadQty = ((flowLayoutPanel1.Size.Width / 208) * (flowLayoutPanel1.Size.Height / 309) + ((flowLayoutPanel1.Size.Width / 208) * 2) + ((flowLayoutPanel1.Size.Height / 309) * 2));
             
                    if (loadQty > 1)
                    {
                        SearchAddressUpdate();
                    }
                }
            }
        }

        private void btnLive_Click(object sender, EventArgs e)
        {
            if (btnLive.BackColor == Color.White)
            {
                btnLive.BackColor = Color.Blue;
                btnLive.ForeColor = Color.Yellow;
                btnCreated.BackColor = Color.White;
                btnOwned.BackColor = Color.White;
                btnOwned.Enabled = false;
                btnCreated.Enabled = false;
                btnConnections.Enabled = false;
                btnWorkBench.Enabled = false;
                btnHistoryBack.Enabled = false;
                btnHistoryForward.Enabled = false;
                btnMint.Enabled = false;
                txtSearchAddress.Enabled = false;
                BuildSearchResults();
                tmrSearchMemoryPool.Enabled = true;

            }
            else
            {
                btnLive.BackColor = Color.White;
                btnLive.ForeColor = Color.Black;
                btnOwned.Enabled = true;
                btnCreated.Enabled = true;
                txtSearchAddress.Enabled = true;
                btnWorkBench.Enabled = true;
                btnConnections.Enabled = true;
                btnHistoryBack.Enabled = true;
                btnHistoryForward.Enabled = true;
                btnMint.Enabled = true;
                tmrSearchMemoryPool.Enabled = false;
            }
        }

        private void tmrSearchMemoryPool_Tick(object sender, EventArgs e)
        {
            try
            {
                Task SearchMemoryTask = Task.Run(() =>
                {
                    string isBuilding;
                    lock (levelDBLocker)
                    {
                        var MUTE = new Options { CreateIfMissing = true };
                        using (var db = new DB(MUTE, @"root\monitor"))
                        {
                            isBuilding = db.Get("isBuilding");
                        }
                    }
                    if (isBuilding != "true" || isBuildingCounter > 11)
                    {
                        lock (levelDBLocker)
                        {
                            var MUTE = new Options { CreateIfMissing = true };
                            using (var db = new DB(MUTE, @"root\monitor"))
                            {
                                db.Put("isBuilding", "true");
                                isBuildingCounter++;
                            }
                        }
                        int foundCount = 0;
                        List<string> differenceQuery = new List<string>();
                        List<string> newtransactions = new List<string>();
                        string flattransactions;
                        OBJState isobject = new OBJState();
                        NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                        RPCClient rpcClient;
                        try
                        {
                            rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:18332"), Network.Main);
                            flattransactions = rpcClient.SendCommand("getrawmempool").ResultString;
                            flattransactions = flattransactions.Replace("\"", "").Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
                            newtransactions = flattransactions.Split(',').ToList();

                            if (BTCTMemPool.Count == 0)
                            {
                                BTCTMemPool = newtransactions;
                            }
                            else
                            {
                                differenceQuery =
                                (List<string>)newtransactions.Except(BTCTMemPool).ToList(); ;

                                BTCTMemPool = newtransactions;

                                foreach (var s in differenceQuery)
                                {
                                    try
                                    {
                                        isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:18332");
                                    }
                                    catch { }

                                    if (isobject.URN != null) {

                                        lock (levelDBLocker)
                                        {
                                            var FOUND = new Options { CreateIfMissing = true };
                                            using (var db = new DB(FOUND, @"root\found"))
                                            {
                                                db.Put("found!" + isobject.CreatedDate.ToString("yyyyMMddHHmmss") + "!" + isobject.Creators.First().Key, "1");
                                            }

                                        }

                                        foundCount++; }

                                }

                            }
                        }
                        catch { }
                        newtransactions = new List<string>();

                        try
                        {
                            rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:8332"), Network.Main);
                            flattransactions = rpcClient.SendCommand("getrawmempool").ResultString;
                            flattransactions = flattransactions.Replace("\"", "").Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
                            newtransactions = flattransactions.Split(',').ToList();

                            if (BTCMemPool.Count == 0)
                            {
                                BTCMemPool = newtransactions;
                            }
                            else
                            {
                                differenceQuery =
                                (List<string>)newtransactions.Except(BTCMemPool).ToList(); ;

                                BTCMemPool = newtransactions;

                                foreach (var s in differenceQuery)
                                {
                                    try
                                    {
                                        isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                    }
                                    catch { }
                                    if (isobject.URN != null)
                                    {

                                        lock (levelDBLocker)
                                        {
                                            var FOUND = new Options { CreateIfMissing = true };
                                            using (var db = new DB(FOUND, @"root\found"))
                                            {
                                                db.Put("found!" + isobject.CreatedDate.ToString("yyyyMMddHHmmss") + "!" + isobject.Creators.First().Key, "1");
                                            }

                                        }

                                        foundCount++;
                                    }

                                }

                            }
                        }
                        catch { }
                        newtransactions = new List<string>();

                        try
                        {
                            rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:12832"), Network.Main);
                            flattransactions = rpcClient.SendCommand("getrawmempool").ResultString;
                            flattransactions = flattransactions.Replace("\"", "").Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
                            newtransactions = flattransactions.Split(',').ToList();
                            if (MZCMemPool.Count == 0)
                            {
                                MZCMemPool = newtransactions;
                            }
                            else
                            {
                                differenceQuery =
                                (List<string>)newtransactions.Except(MZCMemPool).ToList(); ;

                                MZCMemPool = newtransactions;

                                foreach (var s in differenceQuery)
                                {
                                    try
                                    {
                                        isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                    }
                                    catch { }
                                    if (isobject.URN != null)
                                    {

                                        lock (levelDBLocker)
                                        {
                                            var FOUND = new Options { CreateIfMissing = true };
                                            using (var db = new DB(FOUND, @"root\found"))
                                            {
                                                db.Put("found!" + isobject.CreatedDate.ToString("yyyyMMddHHmmss") + "!" + isobject.Creators.First().Key, "1");
                                            }

                                        }

                                        foundCount++;
                                    }

                                }

                            }
                        }
                        catch { }
                        newtransactions = new List<string>();

                        try
                        {
                            rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:9332"), Network.Main);
                            flattransactions = rpcClient.SendCommand("getrawmempool").ResultString;
                            flattransactions = flattransactions.Replace("\"", "").Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
                            newtransactions = flattransactions.Split(',').ToList();
                            if (LTCMemPool.Count == 0)
                            {
                                LTCMemPool = newtransactions;
                            }
                            else
                            {
                                differenceQuery =
                                (List<string>)newtransactions.Except(LTCMemPool).ToList(); ;

                                LTCMemPool = newtransactions;

                                foreach (var s in differenceQuery)
                                {
                                    try
                                    {
                                        isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                    }
                                    catch { }
                                    if (isobject.URN != null)
                                    {

                                        lock (levelDBLocker)
                                        {
                                            var FOUND = new Options { CreateIfMissing = true };
                                            using (var db = new DB(FOUND, @"root\found"))
                                            {
                                                db.Put("found!" + isobject.CreatedDate.ToString("yyyyMMddHHmmss") + "!" + isobject.Creators.First().Key, "1");
                                            }

                                        }

                                        foundCount++;
                                    }

                                }

                            }
                        }
                        catch { }
                        newtransactions = new List<string>();

                        try
                        {
                            rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:22555"), Network.Main);
                            flattransactions = rpcClient.SendCommand("getrawmempool").ResultString;
                            flattransactions = flattransactions.Replace("\"", "").Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
                            newtransactions = flattransactions.Split(',').ToList();

                            if (DOGMemPool.Count == 0)
                            {
                                DOGMemPool = newtransactions;
                            }
                            else
                            {
                                differenceQuery =
                                (List<string>)newtransactions.Except(DOGMemPool).ToList(); ;

                                DOGMemPool = newtransactions;

                                foreach (var s in differenceQuery)
                                {
                                    try
                                    {
                                        isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                    }
                                    catch { }
                                    if (isobject.URN != null)
                                    {

                                        lock (levelDBLocker)
                                        {
                                            var FOUND = new Options { CreateIfMissing = true };
                                            using (var db = new DB(FOUND, @"root\found"))
                                            {
                                                db.Put("found!" + isobject.CreatedDate.ToString("yyyyMMddHHmmss") + "!" + isobject.Creators.First().Key, "1");
                                            }

                                        }

                                        foundCount++;
                                    }

                                }

                            }
                        }
                        catch { }
                        newtransactions = new List<string>();

                        if (foundCount > 0)
                        {

                            int totalQty = 0;

                            // Update the txtQty control using Invoke to run it on the UI thread.
                            flowLayoutPanel1.Invoke((MethodInvoker)delegate
                            {
                                totalQty = flowLayoutPanel1.Controls.Count + ((foundCount * 2) + 2);
                            });


                            // Update the txtQty control using Invoke to run it on the UI thread.
                            txtQty.Invoke((MethodInvoker)delegate
                            {
                                txtQty.Text = totalQty.ToString(); totalQty = flowLayoutPanel1.Controls.Count + ((foundCount * 2) + 2);
                                ;
                            });


                            // Update the txtQty control using Invoke to run it on the UI thread.
                            txtLast.Invoke((MethodInvoker)delegate
                            {
                                txtLast.Text = "0";
                            });

                            lock (levelDBLocker)
                            {
                                var MUTE = new Options { CreateIfMissing = true };
                                using (var db = new DB(MUTE, @"root\monitor"))
                                {
                                    db.Delete("isBuilding");

                                }
                            }

                            this.Invoke((MethodInvoker)delegate
                            {                                

                                SearchAddressUpdate();

                            });

                            isBuildingCounter = 0;

                        }
                        lock (levelDBLocker)
                        {
                            var MUTE = new Options { CreateIfMissing = true };
                            using (var db = new DB(MUTE, @"root\monitor"))
                            {
                                db.Delete("isBuilding");
                                isBuildingCounter = 0;
                            }
                        }



                    }
                    else { isBuildingCounter++; }
                });
            }
            catch {
                lock (levelDBLocker)
                {
                    var MUTE = new Options { CreateIfMissing = true };
                    using (var db = new DB(MUTE, @"root\monitor"))
                    {
                        db.Delete("isBuilding");

                    }
                }

            }
        }


        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            if (btnLive.BackColor != Color.Blue)
            {
                if (flowLayoutPanel1.Controls.Count > 0 && flowLayoutPanel1.Controls[0] is Microsoft.Web.WebView2.WinForms.WebView2)
                {
                    flowLayoutPanel1.Controls[0].Size = flowLayoutPanel1.Size;
                }

                int loadQty = ((flowLayoutPanel1.Size.Width / 208) * (flowLayoutPanel1.Size.Height / 309) + ((flowLayoutPanel1.Size.Width / 208)) + ((flowLayoutPanel1.Size.Height / 309))) - flowLayoutPanel1.Controls.Count;

                if (loadQty > 1)
                {
                    SearchAddressUpdate();
                };

            }
        }
    }
}
