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
using System.Net;
using NBitcoin.RPC;
using System.IO;

namespace SUP
{

    public partial class ObjectBrowser : Form
    {
        private readonly string _objectaddress;
        private List<String> SearchHistory = new List<String>();
        private int SearchId = 0;
        private HashSet<string> loadedObjects = new HashSet<string>();
        private readonly static object SupLocker = new object();
        private List<string> BTCMemPool = new List<string>();
        private List<string> BTCTMemPool = new List<string>();
        private List<string> MZCMemPool = new List<string>();
        private List<string> LTCMemPool = new List<string>();
        private List<string> DOGMemPool = new List<string>();
        private const int DoubleClickInterval = 250;
        private bool _mouseLock;
        private bool _mouseClicked;
        private int _viewMode = 0;
        private readonly System.Windows.Forms.Timer _doubleClickTimer = new System.Windows.Forms.Timer();
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

        private void GetObjectsByAddress(string address)
        {

            string profileCheck = address;
            PROState searchprofile = PROState.GetProfileByAddress(address, "good-user", "better-password", @"http://127.0.0.1:18332");

            if (searchprofile.URN != null)
            {
                this.Invoke((Action)(() =>
                {
                    linkLabel1.Text = searchprofile.URN;
                    linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
                }));
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(address, "good-user", "better-password", @"http://127.0.0.1:18332");

                if (searchprofile.URN != null)
                {

                    this.Invoke((Action)(() =>
                    {

                        linkLabel1.Text = TruncateAddress(searchprofile.Creators.First());
                        linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
                        profileCheck = searchprofile.Creators.First();
                    }));

                }
                else
                {

                    this.Invoke((Action)(() =>
                    {

                        linkLabel1.Text = "anon";
                        linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                    }));
                }
            }


            List<OBJState> createdObjects = new List<OBJState>();

            int skip = int.Parse(txtLast.Text);
            int qty = int.Parse(txtQty.Text);


            if (btnCreated.BackColor == Color.Yellow && txtSearchAddress.Text != "")
            {
                if (!System.IO.File.Exists("root\\" + profileCheck + "\\GetObjectsCreatedByAddress.json"))
                {
                    this.Invoke((Action)(() =>
                    {
                        flowLayoutPanel1.Visible = false;
                        pages.Visible = false;
                    }));

                }
                createdObjects = OBJState.GetObjectsCreatedByAddress(profileCheck, "good-user", "better-password", @"http://127.0.0.1:18332", "111", 0, -1);

            }
            else if (btnOwned.BackColor == Color.Yellow && txtSearchAddress.Text != "")
            {

                if (!System.IO.File.Exists("root\\" + profileCheck + "\\GetObjectsOwnedByAddress.json"))
                {
                    this.Invoke((Action)(() =>
                    {
                        flowLayoutPanel1.Visible = false;
                        pages.Visible = false;
                    }));

                }
                createdObjects = OBJState.GetObjectsOwnedByAddress(profileCheck, "good-user", "better-password", @"http://127.0.0.1:18332", "111", 0, -1);

            }
            else
            {
                if (txtSearchAddress.Text == "")
                {
                    if (!System.IO.File.Exists("root\\found\\GetFoundObjects.json"))
                    {
                        this.Invoke((Action)(() =>
                        {
                            flowLayoutPanel1.Visible = false;
                            pages.Visible = false;
                        }));

                    }
                    createdObjects = OBJState.GetFoundObjects("good-user", "better-password", @"http://127.0.0.1:18332", "111", 0, -1);


                }
                else
                {


                    if (!System.IO.File.Exists("root\\" + profileCheck + "\\GetObjectsByAddress.json"))
                    {
                        this.Invoke((Action)(() =>
                        {
                            flowLayoutPanel1.Visible = false;
                            pages.Visible = false;
                        }));

                    }
                    createdObjects = OBJState.GetObjectsByAddress(profileCheck, "good-user", "better-password", @"http://127.0.0.1:18332", "111", 0, -1);
                }
            }




            this.Invoke((Action)(() =>
            {
                pages.Maximum = createdObjects.Count - 1;
                txtTotal.Text = (createdObjects.Count).ToString();
                pages.Visible = true;

            }));

            createdObjects.Reverse();
            foreach (OBJState objstate in createdObjects.Skip(skip).Take(qty))
            {
                try
                {
                    flowLayoutPanel1.Invoke((MethodInvoker)delegate
                    {
                        flowLayoutPanel1.SuspendLayout();


                        if (objstate.Owners != null)
                        {
                            string transid = "";
                            string imgurl = "";
                            FoundObjectControl foundObject = new FoundObjectControl();

                            if (objstate.Image == null)
                            {


                                Random rnd = new Random();
                                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                                if (gifFiles.Length > 0)
                                {
                                    int randomIndex = rnd.Next(gifFiles.Length);
                                    objstate.Image = gifFiles[randomIndex];

                                }
                                else
                                {
                                    try
                                    {

                                        objstate.Image = @"includes\HugPuddle.jpg";
                                    }
                                    catch { }
                                }


                            }

                            try { transid = objstate.Image.Substring(4, 64).Replace(":", ""); } catch { try { transid = objstate.Image.Substring(5, 46); } catch { } }
                            imgurl = objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"\", @"/");
                            if (objstate.Image.ToUpper().StartsWith("IPFS:")) { imgurl = @"ipfs/" + imgurl; } else { imgurl = @"root/" + imgurl; }

                            if (File.Exists(imgurl))
                            {

                                foundObject.ObjectImage.ImageLocation = imgurl;
                            }
                            else
                            {
                                Random rnd = new Random();
                                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                                if (gifFiles.Length > 0)
                                {
                                    int randomIndex = rnd.Next(gifFiles.Length);
                                    string randomGifFile = gifFiles[randomIndex];
                                    foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.StretchImage;
                                    foundObject.ObjectImage.ImageLocation = randomGifFile;

                                }
                                else
                                {
                                    try
                                    {

                                        foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.StretchImage;
                                        foundObject.ObjectImage.ImageLocation = @"includes\HugPuddle.jpg";
                                    }
                                    catch { }
                                }


                            }
                            foundObject.ObjectName.Text = objstate.Name;
                            foundObject.ObjectDescription.Text = objstate.Description;
                            foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                            foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";


                            switch (objstate.Image.ToUpper().Substring(0, 4))
                            {
                                case "BTC:":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                            if (File.Exists(imgurl))
                                            {

                                                foundObject.ObjectImage.ImageLocation = imgurl;
                                            }
                                        }
                                    });
                                    break;
                                case "MZC:":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                            if (File.Exists(imgurl))
                                            {

                                                foundObject.ObjectImage.ImageLocation = imgurl;
                                            }
                                        }
                                    });
                                    break;
                                case "LTC:":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                            if (File.Exists(imgurl))
                                            {

                                                foundObject.ObjectImage.ImageLocation = imgurl;
                                            }
                                        }
                                    });
                                    break;
                                case "DOG:":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                            if (File.Exists(imgurl))
                                            {

                                                foundObject.ObjectImage.ImageLocation = imgurl;
                                            }
                                        }
                                    });
                                    break;
                                case "IPFS":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("ipfs/" + transid) && !System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                        {
                                            Directory.CreateDirectory("ipfs/" + transid + "-build");
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
                                                Directory.CreateDirectory("ipfs/" + transid);
                                                System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                                            }


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



                                            Directory.Delete("ipfs/" + transid + "-build", true);


                                        }
                                        if (objstate.Image.Length == 51)
                                        { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                                        else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }
                                    });
                                    break;
                                case "HTTP":
                                    Task.Run(() =>
                                    {
                                        foundObject.ObjectImage.ImageLocation = objstate.Image;
                                    });
                                    break;


                                default:
                                    try { transid = objstate.Image.Substring(0, 64); } catch { }
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                        }
                                        if (File.Exists(imgurl))
                                        {

                                            foundObject.ObjectImage.ImageLocation = imgurl;
                                        }
                                    });
                                    break;
                            }

                            foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                            {

                                if (creator.Value.Year > 1)
                                {
                                    PROState profile = PROState.GetProfileByAddress(creator.Key, "good-user", "better-password", @"http://127.0.0.1:18332");

                                    if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                                    {


                                        foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                        foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                        myTooltip.SetToolTip(foundObject.ObjectCreators, profile.URN);
                                    }
                                    else
                                    {


                                        if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                                        {
                                            foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                            foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                            myTooltip.SetToolTip(foundObject.ObjectCreators2, profile.URN);
                                        }

                                        if (foundObject.ObjectCreators.Text == "")
                                        {


                                            foundObject.ObjectCreators.Text = TruncateAddress(creator.Key);
                                            foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                            myTooltip.SetToolTip(foundObject.ObjectCreators, creator.Key);
                                        }
                                        else
                                        {


                                            if (foundObject.ObjectCreators2.Text == "")
                                            {
                                                foundObject.ObjectCreators2.Text = TruncateAddress(creator.Key);
                                                foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                                System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                                myTooltip.SetToolTip(foundObject.ObjectCreators2, creator.Key);
                                            }

                                        }

                                    }
                                }


                            }
                            foundObject.ObjectId.Text = objstate.Id.ToString();
                            if (!loadedObjects.Contains(foundObject.ObjectAddress.Text))
                            {

                                OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, "good-user", "better-password", @"http://127.0.0.1:18332");
                                if (isOfficial.URN != null)
                                {
                                    if (isOfficial.Creators.First().Key == foundObject.ObjectAddress.Text)
                                    {
                                        foundObject.lblOfficial.Visible = true;
                                        foundObject.lblOfficial.Text = TruncateAddress(isOfficial.URN);
                                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                        myTooltip.SetToolTip(foundObject.lblOfficial, isOfficial.URN);
                                    }
                                    else
                                    {
                                        foundObject.txtOfficialURN.Text = isOfficial.Creators.First().Key;
                                        foundObject.btnOfficial.Visible = true;

                                    }
                                }

                                if (_viewMode == 0)
                                {
                                    foundObject.Height = 221;
                                    flowLayoutPanel1.Controls.Add(foundObject);
                                    this.MinimumSize = new System.Drawing.Size(709, 558);

                                }
                                if (_viewMode == 1)
                                {
                                    flowLayoutPanel1.Controls.Add(foundObject);
                                    this.MinimumSize = new System.Drawing.Size(709, 558);
                                }

                                if (_viewMode == 2)
                                {

                                    ObjectDetailsControl control = new ObjectDetailsControl(foundObject.ObjectAddress.Text);
                                    flowLayoutPanel1.Controls.Add(control);
                                    this.MinimumSize = new System.Drawing.Size(1101, 521);
                                    this.Size = new System.Drawing.Size(1101, 521);
                                }


                            }
                            loadedObjects.Add(foundObject.ObjectAddress.Text);


                        }
                        flowLayoutPanel1.ResumeLayout();
                    });
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }

        }

        private void GetObjectByURN(string searchstring)
        {

            List<OBJState> createdObjects = new List<OBJState>();


            if (!System.IO.File.Exists("root\\" + Root.GetPublicAddressByKeyword(searchstring, "111") + "\\GetObjectByURN.json"))
            {
                this.Invoke((Action)(() =>
                {
                    flowLayoutPanel1.Visible = false;
                    pages.Visible = false;
                }));

            }

            createdObjects = new List<OBJState> { OBJState.GetObjectByURN(searchstring, "good-user", "better-password", @"http://127.0.0.1:18332", "111") };

            List<OBJState> reviewedObjects = new List<OBJState>();
            foreach (OBJState objstate in createdObjects)
            {
                string isBlocked = "";
                var OBJ = new Options { CreateIfMissing = true };
                try
                {
                    using (var db = new DB(OBJ, @"root\oblock"))
                    {
                        isBlocked = db.Get(objstate.Creators.First().Key);
                        db.Close();
                    }
                }
                catch
                {
                    try
                    {
                        using (var db = new DB(OBJ, @"root\oblock2"))
                        {
                            isBlocked = db.Get(objstate.Creators.First().Key);
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
                    reviewedObjects.Add(objstate);
                }

            }
            createdObjects = reviewedObjects;

            foreach (OBJState objstate in createdObjects)
            {
                try
                {
                    flowLayoutPanel1.Invoke((MethodInvoker)delegate
                    {
                        flowLayoutPanel1.SuspendLayout();
                        if (objstate.Owners != null)
                        {
                            string transid = "";
                            FoundObjectControl foundObject = new FoundObjectControl();
                            foundObject.SuspendLayout();



                            if (objstate.Image == null)
                            {

                                Random rnd = new Random();
                                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                                if (gifFiles.Length > 0)
                                {
                                    int randomIndex = rnd.Next(gifFiles.Length);
                                    objstate.Image = gifFiles[randomIndex];

                                }
                                else
                                {
                                    try
                                    {

                                        objstate.Image = @"includes\HugPuddle.jpg";
                                    }
                                    catch { }
                                }


                            }


                            try { transid = objstate.Image.Substring(4, 64).Replace(":", ""); } catch { try { transid = objstate.Image.Substring(5, 46); } catch { } }
                            string imgurl = objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("IPFS:", "");
                            if (objstate.Image.ToUpper().StartsWith("IPFS:")) { imgurl = @"ipfs/" + imgurl; } else { imgurl = @"root/" + imgurl; }

                            foundObject.ObjectImage.ImageLocation = imgurl;
                            foundObject.ObjectName.Text = objstate.Name;
                            foundObject.ObjectDescription.Text = objstate.Description;
                            foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                            foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";

                            switch (objstate.Image.ToUpper().Substring(0, 4))
                            {
                                case "BTC:":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                            if (File.Exists(imgurl))
                                            {

                                                foundObject.ObjectImage.ImageLocation = imgurl;
                                            }
                                        }
                                    });
                                    break;
                                case "MZC:":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                            if (File.Exists(imgurl))
                                            {

                                                foundObject.ObjectImage.ImageLocation = imgurl;
                                            }
                                        }
                                    });
                                    break;
                                case "LTC:":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                            if (File.Exists(imgurl))
                                            {

                                                foundObject.ObjectImage.ImageLocation = imgurl;
                                            }
                                        }
                                    });
                                    break;
                                case "DOG:":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                            if (File.Exists(imgurl))
                                            {

                                                foundObject.ObjectImage.ImageLocation = imgurl;
                                            }
                                        }
                                    });
                                    break;
                                case "IPFS":
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("ipfs/" + transid))
                                        {
                                            Directory.CreateDirectory("ipfs/" + transid);
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
                                        if (objstate.Image.Length == 51)
                                        { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                                        else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }
                                    });
                                    break;
                                case "HTTP":
                                    Task.Run(() =>
                                    {
                                        foundObject.ObjectImage.ImageLocation = objstate.Image;
                                    });
                                    break;


                                default:
                                    try { transid = objstate.Image.Substring(0, 64); } catch { foundObject.ObjectImage.ImageLocation = transid; }
                                    Task.Run(() =>
                                    {
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                        }
                                        if (File.Exists(imgurl))
                                        {

                                            foundObject.ObjectImage.ImageLocation = imgurl;
                                        }
                                    });
                                    break;
                            }


                            foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                            {

                                if (creator.Value.Year > 1)
                                {
                                    PROState profile = PROState.GetProfileByAddress(creator.Key, "good-user", "better-password", @"http://127.0.0.1:18332");

                                    if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                                    {


                                        foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                        foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                        myTooltip.SetToolTip(foundObject.ObjectCreators, profile.URN);
                                    }
                                    else
                                    {


                                        if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                                        {
                                            foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                            foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                            myTooltip.SetToolTip(foundObject.ObjectCreators2, profile.URN);
                                        }

                                    }
                                }
                                else
                                {

                                    if (foundObject.ObjectCreators.Text == "")
                                    {


                                        foundObject.ObjectCreators.Text = TruncateAddress(creator.Key);
                                        foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                        myTooltip.SetToolTip(foundObject.ObjectCreators, creator.Key);
                                    }
                                    else
                                    {


                                        if (foundObject.ObjectCreators2.Text == "")
                                        {
                                            foundObject.ObjectCreators2.Text = TruncateAddress(creator.Key);
                                            foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                            myTooltip.SetToolTip(foundObject.ObjectCreators2, creator.Key);
                                        }

                                    }

                                }

                            }
                            foundObject.ObjectId.Text = objstate.Id.ToString();


                            if (!loadedObjects.Contains(foundObject.ObjectAddress.Text))
                            {

                                OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, "good-user", "better-password", @"http://127.0.0.1:18332");
                                if (isOfficial.URN != null)
                                {
                                    if (isOfficial.Creators.First().Key == foundObject.ObjectAddress.Text)
                                    {
                                        foundObject.lblOfficial.Visible = true;
                                        foundObject.lblOfficial.Text = TruncateAddress(isOfficial.URN);
                                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                        myTooltip.SetToolTip(foundObject.lblOfficial, isOfficial.URN);
                                    }
                                    else
                                    {
                                        foundObject.txtOfficialURN.Text = isOfficial.Creators.First().Key;
                                        foundObject.btnOfficial.Visible = true;

                                    }
                                }
                                foundObject.ResumeLayout();
                                if (_viewMode == 0)
                                {
                                    foundObject.Height = 221;
                                    flowLayoutPanel1.Controls.Add(foundObject);
                                    this.MinimumSize = new System.Drawing.Size(709, 558);

                                }
                                if (_viewMode == 1)
                                {
                                    flowLayoutPanel1.Controls.Add(foundObject);
                                    this.MinimumSize = new System.Drawing.Size(709, 558);
                                }

                                if (_viewMode == 2)
                                {

                                    ObjectDetailsControl control = new ObjectDetailsControl(foundObject.ObjectAddress.Text);
                                    flowLayoutPanel1.Controls.Add(control);
                                    this.MinimumSize = new System.Drawing.Size(1101, 521);
                                    this.Size = new System.Drawing.Size(1101, 521);
                                }



                            }
                            loadedObjects.Add(foundObject.ObjectAddress.Text);


                        }
                        flowLayoutPanel1.ResumeLayout();
                    });
                }
                catch { }
            }

        }

        private void GetObjectByFile(string filePath)
        {

            flowLayoutPanel1.Controls.Clear();
            int loadQty = (flowLayoutPanel1.Size.Width / 100) * (flowLayoutPanel1.Size.Height / 200) + 3;

            loadQty -= flowLayoutPanel1.Controls.Count;

            txtQty.Text = loadQty.ToString();

            OBJState objstate = OBJState.GetObjectByFile(filePath, "good-user", "better-password", @"http://127.0.0.1:18332");

            if (objstate.Owners != null)
            {

                FoundObjectControl foundObject = new FoundObjectControl();
                string transid = "";

                if (objstate.Image == null)
                {

                    Random rnd = new Random();
                    string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                    if (gifFiles.Length > 0)
                    {
                        int randomIndex = rnd.Next(gifFiles.Length);
                        objstate.Image = gifFiles[randomIndex];

                    }
                    else
                    {
                        try
                        {

                            objstate.Image = @"includes\HugPuddle.jpg";
                        }
                        catch { }
                    }


                }

                try { transid = objstate.Image.Substring(4, 64).Replace(":", ""); } catch { try { transid = objstate.Image.Substring(5, 46); } catch { } }
                string imgurl = objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("IPFS:", "");
                if (objstate.Image.ToUpper().StartsWith("IPFS:")) { imgurl = @"ipfs/" + imgurl; } else { imgurl = @"root/" + imgurl; }

                foundObject.ObjectImage.ImageLocation = imgurl;
                foundObject.ObjectName.Text = objstate.Name;
                foundObject.ObjectDescription.Text = objstate.Description;
                foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";

                switch (objstate.Image.ToUpper().Substring(0, 4))
                {
                    case "BTC:":
                        Task.Run(() =>
                        {
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                if (File.Exists(imgurl))
                                {

                                    foundObject.ObjectImage.ImageLocation = imgurl;
                                }
                            }
                        });
                        break;
                    case "MZC:":
                        Task.Run(() =>
                        {
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                if (File.Exists(imgurl))
                                {

                                    foundObject.ObjectImage.ImageLocation = imgurl;
                                }
                            }
                        });
                        break;
                    case "LTC:":
                        Task.Run(() =>
                        {
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                if (File.Exists(imgurl))
                                {

                                    foundObject.ObjectImage.ImageLocation = imgurl;
                                }
                            }
                        });
                        break;
                    case "DOG:":
                        Task.Run(() =>
                        {
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                if (File.Exists(imgurl))
                                {

                                    foundObject.ObjectImage.ImageLocation = imgurl;
                                }
                            }
                        });
                        break;
                    case "IPFS":
                        Task.Run(() =>
                        {
                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
                                Directory.CreateDirectory("ipfs/" + transid);
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

                            if (objstate.Image.Length == 51)
                            { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                            else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }
                        });
                        break;
                    case "HTTP":
                        Task.Run(() =>
                        {
                            foundObject.ObjectImage.ImageLocation = objstate.Image;
                        });
                        break;


                    default:
                        transid = objstate.Image.Substring(0, 64);
                        Task.Run(() =>
                        {
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332");
                            }
                            if (File.Exists(imgurl))
                            {

                                foundObject.ObjectImage.ImageLocation = imgurl;
                            }
                        });
                        break;
                }


                foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                {


                    if (creator.Value.Year > 1)
                    {
                        PROState profile = PROState.GetProfileByAddress(creator.Key, "good-user", "better-password", @"http://127.0.0.1:18332");

                        if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                        {


                            foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                            foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                            myTooltip.SetToolTip(foundObject.ObjectCreators, profile.URN);
                        }
                        else
                        {


                            if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                            {
                                foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                myTooltip.SetToolTip(foundObject.ObjectCreators2, profile.URN);
                            }

                        }
                    }
                    else
                    {

                        if (foundObject.ObjectCreators.Text == "")
                        {


                            foundObject.ObjectCreators.Text = TruncateAddress(creator.Key);
                            foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                            myTooltip.SetToolTip(foundObject.ObjectCreators, creator.Key);

                        }
                        else
                        {


                            if (foundObject.ObjectCreators2.Text == "")
                            {
                                foundObject.ObjectCreators2.Text = TruncateAddress(creator.Key);
                                foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                myTooltip.SetToolTip(foundObject.ObjectCreators2, creator.Key);
                            }

                        }

                    }


                }
                foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, "good-user", "better-password", @"http://127.0.0.1:18332");
                if (isOfficial.URN != null)
                {
                    if (isOfficial.Creators.First().Key == foundObject.ObjectAddress.Text)
                    {
                        foundObject.lblOfficial.Visible = true;

                        foundObject.lblOfficial.Text = TruncateAddress(isOfficial.URN);

                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                        myTooltip.SetToolTip(foundObject.lblOfficial, isOfficial.URN);

                    }
                    else
                    {
                        foundObject.txtOfficialURN.Text = isOfficial.Owners.First().Key;
                        foundObject.btnOfficial.Visible = true;
                    }
                }

                if (_viewMode == 0)
                {
                    foundObject.Height = 221;
                    flowLayoutPanel1.Controls.Add(foundObject);
                    this.MinimumSize = new System.Drawing.Size(709, 558);

                }
                if (_viewMode == 1)
                {
                    flowLayoutPanel1.Controls.Add(foundObject);
                    this.MinimumSize = new System.Drawing.Size(709, 558);
                }

                if (_viewMode == 2)
                {

                    ObjectDetailsControl control = new ObjectDetailsControl(foundObject.ObjectAddress.Text);
                    flowLayoutPanel1.Controls.Add(control);
                    this.MinimumSize = new System.Drawing.Size(1101, 521);
                    this.Size = new System.Drawing.Size(1101, 521);
                }
            }

        }

        private async void ButtonGetOwnedClick(object sender, EventArgs e)
        {
            if (btnOwned.BackColor == Color.Yellow) { btnOwned.BackColor = Color.White; }
            else
            {
                btnOwned.BackColor = Color.Yellow;
                btnCreated.BackColor = Color.White;
            }
            if (txtSearchAddress.Text != "" && !txtSearchAddress.Text.StartsWith("#") && !txtSearchAddress.Text.ToUpper().StartsWith("BTC:") && !txtSearchAddress.Text.ToUpper().StartsWith("MZC:") && !txtSearchAddress.Text.ToUpper().StartsWith("LTC:") && !txtSearchAddress.Text.ToUpper().StartsWith("IPFS:") && !txtSearchAddress.Text.ToUpper().StartsWith("HTTP") && !txtSearchAddress.Text.ToUpper().StartsWith("SUP:"))
            {
                DisableSupInput();

                Random rnd = new Random();
                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                if (gifFiles.Length > 0)
                {
                    int randomIndex = rnd.Next(gifFiles.Length);
                    string randomGifFile = gifFiles[randomIndex];
                    imgLoading.ImageLocation = randomGifFile;
                }
                else
                {
                    imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
                }
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
                EnableSupInput();

            }


        }

        private async void ButtonGetCreatedClick(object sender, EventArgs e)
        {

            if (btnCreated.BackColor == Color.Yellow) { btnCreated.BackColor = Color.White; }
            else
            {
                btnCreated.BackColor = Color.Yellow;
                btnOwned.BackColor = Color.White;
            }
            if (txtSearchAddress.Text != "" && !txtSearchAddress.Text.StartsWith("#") && !txtSearchAddress.Text.ToUpper().StartsWith("BTC:") && !txtSearchAddress.Text.ToUpper().StartsWith("MZC:") && !txtSearchAddress.Text.ToUpper().StartsWith("LTC:") && !txtSearchAddress.Text.ToUpper().StartsWith("IPFS:") && !txtSearchAddress.Text.ToUpper().StartsWith("HTTP") && !txtSearchAddress.Text.ToUpper().StartsWith("SUP:"))
            {
                DisableSupInput();
                pages.Maximum = 0;
                pages.Value = 0;
                Random rnd = new Random();
                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                if (gifFiles.Length > 0)
                {
                    int randomIndex = rnd.Next(gifFiles.Length);
                    string randomGifFile = gifFiles[randomIndex];
                    imgLoading.ImageLocation = randomGifFile;
                }
                else
                {
                    imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
                }
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
                EnableSupInput();

            }
        }

        private void MainUserNameClick(object sender, LinkLabelLinkClickedEventArgs e)
        {

            PROState searchprofile = PROState.GetProfileByAddress(txtSearchAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");

            if (searchprofile.URN != null)
            {
                txtSearchAddress.Text = searchprofile.URN;
                linkLabel1.Text = TruncateAddress(searchprofile.Creators.First());

                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(txtSearchAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");

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

        private async void SearchAddressKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                if (txtSearchAddress.Text == "" || txtSearchAddress.Text.StartsWith("#") || txtSearchAddress.Text.ToUpper().StartsWith("SUP:") || txtSearchAddress.Text.ToUpper().StartsWith("HTTP") || txtSearchAddress.Text.ToUpper().StartsWith("BTC:") || txtSearchAddress.Text.ToUpper().StartsWith("MZC:") || txtSearchAddress.Text.ToUpper().StartsWith("LTC:") || txtSearchAddress.Text.ToUpper().StartsWith("DOG:") || txtSearchAddress.Text.ToUpper().StartsWith("IPFS:")) { btnCreated.BackColor = Color.White; btnOwned.BackColor = Color.White; }
                e.Handled = true;
                e.SuppressKeyPress = true;
                DisableSupInput();
                if (pages.Value == -1) { pages.Maximum = 0; }
                pages.Value = int.Parse(txtLast.Text);
                Random rnd = new Random();
                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                if (gifFiles.Length > 0)
                {
                    int randomIndex = rnd.Next(gifFiles.Length);
                    string randomGifFile = gifFiles[randomIndex];
                    imgLoading.ImageLocation = randomGifFile;
                }
                else
                {
                    imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
                }
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
                EnableSupInput();

            }

        }

        private async void BuildSearchResults()
        {
            lock (SupLocker)
            {
                var SUP = new Options { CreateIfMissing = true };
                try
                {
                    string isBuilding;
                    DateTime timestamp = DateTime.UtcNow;


                    using (var db = new DB(SUP, @"root\sup"))
                    {
                        isBuilding = db.Get("isBuilding");
                    }


                    DateTime lastTimestamp;
                    if (DateTime.TryParse(isBuilding, out lastTimestamp) && (timestamp - lastTimestamp).TotalSeconds <= 60)
                    {

                        return;
                    }


                    using (var db = new DB(SUP, @"root\sup"))
                    {
                        db.Put("isBuilding", timestamp.ToString("o"));
                    }

                }
                catch { try { Directory.Delete(@"root\sup"); } catch { } }

                try
                {
                    this.Invoke((Action)(() =>
                    {
                        flowLayoutPanel1.Controls.Clear();

                    }));

                    loadedObjects.Clear();


                    int loadQty = (flowLayoutPanel1.Size.Width / 213) * (flowLayoutPanel1.Size.Height / 336);
                    loadQty -= flowLayoutPanel1.Controls.Count;


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

                        this.Invoke((Action)(async () =>
                        {
                            flowLayoutPanel1.Controls.Add(webBrowser1);

                            await webBrowser1.EnsureCoreWebView2Async();
                            webBrowser1.CoreWebView2.Navigate(txtSearchAddress.Text);
                        }));

                    }
                    else
                    {
                        this.Invoke((Action)(() =>
                        {
                            flowLayoutPanel1.AutoScroll = true;
                        }));

                        if (txtSearchAddress.Text.StartsWith("#"))
                        {

                            GetObjectsByAddress(Root.GetPublicAddressByKeyword(txtSearchAddress.Text.Substring(1), "111"));

                        }
                        else
                        {

                            if (txtSearchAddress.Text.ToLower().StartsWith(@"ipfs:") && txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Length >= 51)
                            {
                                string ipfsHash = txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Substring(5, 46);

                                if (!System.IO.Directory.Exists("ipfs/" + ipfsHash))
                                {

                                    string isLoading;

                                    using (var db = new DB(SUP, @"ipfs"))
                                    {
                                        isLoading = db.Get(ipfsHash);

                                    }


                                    if (isLoading != "loading")
                                    {

                                        using (var db = new DB(SUP, @"ipfs"))
                                        {

                                            db.Put(ipfsHash, "loading");

                                        }

                                        Task ipfsTask = Task.Run(() =>
                                        {
                                            Process process2 = new Process();
                                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                            process2.StartInfo.Arguments = "get " + ipfsHash + @" -o ipfs\" + ipfsHash;
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




                                            if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash))
                                            {
                                                Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash);
                                            }
                                            else { System.Windows.Forms.Label filenotFound = new System.Windows.Forms.Label(); filenotFound.AutoSize = true; filenotFound.Text = "IPFS: Search failed! Verify IPFS pinning is enbaled"; flowLayoutPanel1.Controls.Clear(); flowLayoutPanel1.Controls.Add(filenotFound); }

                                            using (var db = new DB(SUP, @"ipfs"))
                                            {
                                                db.Delete(ipfsHash);

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
                                if (txtSearchAddress.Text.ToUpper().StartsWith(@"SUP:"))
                                {

                                    GetObjectByURN(txtSearchAddress.Text.Replace(@"sup://", "").Replace(@"sup:\\", "").Replace("sup:", "").Replace(@"SUP://", "").Replace(@"SUP:\\", "").Replace("SUP:", ""));
                                }
                                else
                                {

                                    Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");

                                    if (txtSearchAddress.Text.Length > 64 && regexTransactionId.IsMatch(txtSearchAddress.Text) && txtSearchAddress.Text.Contains(".htm"))
                                    {
                                        switch (txtSearchAddress.Text.Substring(0, 4))
                                        {
                                            case "MZC:":
                                                Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                                break;
                                            case "BTC:":
                                                Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                                break;
                                            case "LTC:":
                                                Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                                break;
                                            case "DOG:":
                                                Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                                break;
                                            default:
                                                Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(0, 64), "good-user", "better-password", @"http://127.0.0.1:18332");
                                                break;
                                        }
                                        Match match = regexTransactionId.Match(txtSearchAddress.Text);
                                        string browserPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + txtSearchAddress.Text.Replace("MZC:", "").Replace("BTC:", "").Replace("LTC:", "").Replace("DOG:", "");
                                        browserPath = @"file:///" + browserPath.Replace(@"\", @"/");
                                        flowLayoutPanel1.Controls.Clear();
                                        flowLayoutPanel1.AutoScroll = false;
                                        var webBrowser1 = new Microsoft.Web.WebView2.WinForms.WebView2();
                                        webBrowser1.Size = flowLayoutPanel1.Size;
                                        this.Invoke((Action)(async () =>
                                        {
                                            flowLayoutPanel1.Controls.Add(webBrowser1);

                                            await webBrowser1.EnsureCoreWebView2Async();
                                            webBrowser1.CoreWebView2.Navigate(browserPath.Replace(@"/", @"\"));
                                        }));

                                    }
                                    else
                                    {
                                        GetObjectsByAddress(txtSearchAddress.Text.Replace("@", ""));

                                    }

                                }
                            }
                        }
                    }


                    using (var db = new DB(SUP, @"root\sup"))
                    {
                        db.Delete("isBuilding");
                    }

                }
                catch { }
                finally
                {
                    try
                    {

                        using (var db = new DB(SUP, @"root\sup"))
                        {
                            db.Delete("isBuilding");
                        }

                    }
                    catch { try { Directory.Delete(@"root\sup", true); } catch { } }
                }


            }
        }

        private void AddToSearchResults(List<OBJState> objects)
        {

            foreach (OBJState objstate in objects)
            {
                try
                {
                    flowLayoutPanel1.Invoke((MethodInvoker)delegate
                    {
                        flowLayoutPanel1.SuspendLayout();
                        if (objstate.Owners != null)
                        {
                            string transid = "";
                            FoundObjectControl foundObject = new FoundObjectControl();
                            foundObject.SuspendLayout();
                            try { transid = objstate.Image.Substring(4, 64).Replace(":", ""); } catch { try { transid = objstate.Image.Substring(5, 46); } catch { } }
                            try { foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", ""); } catch { }
                            foundObject.ObjectName.Text = objstate.Name;
                            foundObject.ObjectDescription.Text = objstate.Description;
                            foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                            foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                            try
                            {
                                switch (objstate.Image.ToUpper().Substring(0, 4))
                                {
                                    case "BTC:":

                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                        }
                                        break;
                                    case "MZC:":

                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                        }

                                        break;
                                    case "LTC:":

                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                        }

                                        break;
                                    case "DOG:":

                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                        }
                                        break;
                                    case "IPFS":

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
                                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                        }
                                        foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                                        break;
                                }
                            }
                            catch { }

                            foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                            {

                                if (creator.Value.Year > 1)
                                {
                                    PROState profile = PROState.GetProfileByAddress(creator.Key, "good-user", "better-password", @"http://127.0.0.1:18332");

                                    if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                                    {


                                        foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                        foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                        myTooltip.SetToolTip(foundObject.ObjectCreators, profile.URN);
                                    }
                                    else
                                    {


                                        if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                                        {
                                            foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                            foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                            myTooltip.SetToolTip(foundObject.ObjectCreators2, profile.URN);
                                        }

                                    }
                                }
                                else
                                {

                                    if (foundObject.ObjectCreators.Text == "")
                                    {


                                        foundObject.ObjectCreators.Text = TruncateAddress(creator.Key);
                                        foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                        myTooltip.SetToolTip(foundObject.ObjectCreators, creator.Key);
                                    }
                                    else
                                    {


                                        if (foundObject.ObjectCreators2.Text == "")
                                        {
                                            foundObject.ObjectCreators2.Text = TruncateAddress(creator.Key);
                                            foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                            myTooltip.SetToolTip(foundObject.ObjectCreators2, creator.Key);
                                        }

                                    }

                                }

                            }
                            foundObject.ObjectId.Text = objstate.Id.ToString();

                            OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, "good-user", "better-password", @"http://127.0.0.1:18332");
                            if (isOfficial.URN != null)
                            {
                                if (isOfficial.Creators.First().Key == foundObject.ObjectAddress.Text)
                                {
                                    foundObject.lblOfficial.Visible = true;
                                    foundObject.lblOfficial.Text = TruncateAddress(isOfficial.URN);
                                    System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                    myTooltip.SetToolTip(foundObject.lblOfficial, isOfficial.URN);
                                }
                                else
                                {
                                    foundObject.txtOfficialURN.Text = isOfficial.Creators.First().Key;
                                    foundObject.btnOfficial.Visible = true;

                                }
                            }
                            foundObject.ResumeLayout();


                            if (_viewMode == 0)
                            {
                                foundObject.Height = 221;
                                flowLayoutPanel1.Controls.Add(foundObject);
                                this.MinimumSize = new System.Drawing.Size(709, 558);
                                flowLayoutPanel1.Controls.SetChildIndex(foundObject, 0);

                            }
                            if (_viewMode == 1)
                            {
                                flowLayoutPanel1.Controls.Add(foundObject);
                                this.MinimumSize = new System.Drawing.Size(709, 558);
                                flowLayoutPanel1.Controls.SetChildIndex(foundObject, 0);
                            }

                            if (_viewMode == 2)
                            {

                                ObjectDetailsControl control = new ObjectDetailsControl(foundObject.ObjectAddress.Text);
                                flowLayoutPanel1.Controls.Add(control);
                                this.MinimumSize = new System.Drawing.Size(1101, 521);
                                this.Size = new System.Drawing.Size(1101, 521);
                                flowLayoutPanel1.Controls.SetChildIndex(control, 0);
                            }








                        }
                        flowLayoutPanel1.ResumeLayout();
                    });
                }
                catch { }
            }


        }

        private async void ObjectBrowserLoad(object sender, EventArgs e)
        {

            Form parentForm = this.Owner;
            bool isBlue = false;

            // Check if the parent form has a button named "btnLive" with blue background color
            try
            {
                isBlue = parentForm.Controls.OfType<System.Windows.Forms.Button>().Any(b => b.Name == "btnLive" && b.BackColor == System.Drawing.Color.Blue);
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



                if (_objectaddress.Length > 0)
                {
                    txtSearchAddress.Text = _objectaddress;
                    txtLast.Text = "0";

                    switch (_viewMode)
                    {
                        case 0:
                            txtQty.Text = "9";
                            break;
                        case 1:
                            txtQty.Text = "6";
                            break;
                        case 2:
                            txtQty.Text = "3";
                            break;
                        default:
                            // Handle any other cases here
                            break;
                    }

                    DisableSupInput();
                    Random rnd = new Random();
                    string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                    if (gifFiles.Length > 0)
                    {
                        int randomIndex = rnd.Next(gifFiles.Length);
                        string randomGifFile = gifFiles[randomIndex];
                        imgLoading.ImageLocation = randomGifFile;
                    }
                    else
                    {
                        imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
                    }
                    await Task.Run(() => BuildSearchResults());
                    flowLayoutPanel1.Visible = true;
                    pages.Visible = true;
                    EnableSupInput();

                }
                else
                {
                    var SUP = new Options { CreateIfMissing = true };
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
            ObjectMint mintform = new ObjectMint();
            mintform.StartPosition = FormStartPosition.CenterScreen;
            mintform.Show();
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
            DisableSupInput();
            pages.Minimum = 0;
            pages.Value = 0;
            string[] filePaths = (string[])e.Data.GetData((System.Windows.Forms.DataFormats.FileDrop));
            string filePath = filePaths[0];
            GetObjectByFile(filePath);
            flowLayoutPanel1.Visible = true;
            EnableSupInput();

        }

        private void DisableSupInput()
        {
            btnOwned.Enabled = false;
            btnCreated.Enabled = false;
            btnConnections.Enabled = false;
            btnWorkBench.Enabled = false;
            btnHistoryBack.Enabled = false;
            btnHistoryForward.Enabled = false;
            btnMint.Enabled = false;
            txtSearchAddress.Enabled = false;
            pages.Enabled = false;
            btnLive.Enabled = false;
        }

        private void EnableSupInput()
        {
            btnOwned.Enabled = true;
            btnCreated.Enabled = true;
            txtSearchAddress.Enabled = true;
            btnWorkBench.Enabled = true;
            btnConnections.Enabled = true;
            btnHistoryBack.Enabled = true;
            btnHistoryForward.Enabled = true;
            btnMint.Enabled = true;
            pages.Enabled = true;
            btnLive.Enabled = true;
        }

        private async void btnLive_Click(object sender, EventArgs e)
        {


            if (btnLive.BackColor == Color.White)
            {
                btnLive.BackColor = Color.Blue;
                btnLive.ForeColor = Color.Yellow;
                DisableSupInput();
                tmrSearchMemoryPool.Enabled = true;
                Random rnd = new Random();
                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                if (gifFiles.Length > 0)
                {
                    int randomIndex = rnd.Next(gifFiles.Length);
                    string randomGifFile = gifFiles[randomIndex];
                    imgLoading.ImageLocation = randomGifFile;
                }
                else
                {
                    imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
                }
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
                btnLive.Enabled = true;
            }
            else
            {
                btnLive.BackColor = Color.White;
                btnLive.ForeColor = Color.Black;
                tmrSearchMemoryPool.Enabled = false;
                EnableSupInput();

            }
        }

        private void tmrSearchMemoryPool_Tick(object sender, EventArgs e)
        {
            lock (SupLocker)
            {
                try
                {
                    Task SearchMemoryTask = Task.Run(() =>
                    {
                        var SUP = new Options { CreateIfMissing = true };
                        try
                        {
                            string isBuilding;
                            DateTime timestamp = DateTime.UtcNow;


                            using (var db = new DB(SUP, @"root\sup"))
                            {
                                isBuilding = db.Get("isBuilding");
                            }


                            DateTime lastTimestamp;
                            if (DateTime.TryParse(isBuilding, out lastTimestamp) && (timestamp - lastTimestamp).TotalSeconds <= 60)
                            {

                                return;
                            }

                            using (var db = new DB(SUP, @"root\sup"))
                            {
                                db.Put("isBuilding", timestamp.ToString("o"));
                            }

                        }
                        catch { try { Directory.Delete(@"root\sup"); } catch { } }

                        List<string> differenceQuery = new List<string>();
                        List<string> newtransactions = new List<string>();
                        string flattransactions;
                        OBJState isobject = new OBJState();
                        List<OBJState> foundobjects = new List<OBJState>();

                        NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                        RPCClient rpcClient;

                        string filter = "";


                        txtSearchAddress.Invoke((MethodInvoker)delegate
                        {
                            filter = txtSearchAddress.Text;


                        });

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

                                        Root root = Root.GetRootByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:18332");
                                        if (root.Signed == true)
                                        {
                                            string isBlocked = "";
                                            var OBJ = new Options { CreateIfMissing = true };
                                            try
                                            {
                                                using (var db = new DB(OBJ, @"root\oblock"))
                                                {
                                                    isBlocked = db.Get(root.Signature);
                                                    db.Close();
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    using (var db = new DB(OBJ, @"root\oblock2"))
                                                    {
                                                        isBlocked = db.Get(root.Signature);
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
                                                bool find = false;

                                                if (filter != "")
                                                {

                                                    if (filter.StartsWith("#"))
                                                    {
                                                        find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1)));
                                                    }
                                                    else
                                                    {

                                                        find = root.Keyword.ContainsKey(filter);


                                                    }
                                                }
                                                else { find = true; }

                                                isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:18332");
                                                if (isobject.URN != null && find == true)
                                                {
                                                    foundobjects.Add(isobject);

                                                    using (var db = new DB(SUP, @"root\found"))
                                                    {
                                                        db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                    }




                                                }


                                            }
                                            else { try { System.IO.Directory.Delete(@"root\" + s, true); } catch { } }

                                        }
                                        else
                                        {

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        string error = ex.Message;
                                    }
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

                                        Root root = Root.GetRootByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                        if (root.Signed == true)
                                        {
                                            string isBlocked = "";
                                            var OBJ = new Options { CreateIfMissing = true };
                                            try
                                            {
                                                using (var db = new DB(OBJ, @"root\oblock"))
                                                {
                                                    isBlocked = db.Get(root.Signature);
                                                    db.Close();
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    using (var db = new DB(OBJ, @"root\oblock2"))
                                                    {
                                                        isBlocked = db.Get(root.Signature);
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
                                                bool find = false;

                                                if (filter != "")
                                                {

                                                    if (filter.StartsWith("#"))
                                                    {
                                                        find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1)));
                                                    }
                                                    else
                                                    {

                                                        find = root.Keyword.ContainsKey(filter);


                                                    }
                                                }
                                                else { find = true; }

                                                isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                                if (isobject.URN != null && find == true)
                                                {
                                                    foundobjects.Add(isobject);


                                                    using (var db = new DB(SUP, @"root\found"))
                                                    {
                                                        db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                    }


                                                }


                                            }
                                            else { try { System.IO.Directory.Delete(@"root\" + s, true); } catch { } }

                                        }
                                        else { }

                                    }
                                    catch { }

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

                                        Root root = Root.GetRootByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                        if (root.Signed == true)
                                        {
                                            string isBlocked = "";
                                            var OBJ = new Options { CreateIfMissing = true };
                                            try
                                            {
                                                using (var db = new DB(OBJ, @"root\oblock"))
                                                {
                                                    isBlocked = db.Get(root.Signature);
                                                    db.Close();
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    using (var db = new DB(OBJ, @"root\oblock2"))
                                                    {
                                                        isBlocked = db.Get(root.Signature);
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
                                                bool find = false;

                                                if (filter != "")
                                                {

                                                    if (filter.StartsWith("#"))
                                                    {
                                                        find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1)));
                                                    }
                                                    else
                                                    {

                                                        find = root.Keyword.ContainsKey(filter);


                                                    }
                                                }
                                                else { find = true; }

                                                isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                                if (isobject.URN != null && find == true)
                                                {


                                                    foundobjects.Add(isobject);
                                                    using (var db = new DB(SUP, @"root\found"))
                                                    {
                                                        db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                    }



                                                }


                                            }
                                            else { try { System.IO.Directory.Delete(@"root\" + s, true); } catch { } }

                                        }
                                        else { }

                                    }
                                    catch { }

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

                                        Root root = Root.GetRootByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                        if (root.Signed == true)
                                        {
                                            string isBlocked = "";
                                            var OBJ = new Options { CreateIfMissing = true };
                                            try
                                            {
                                                using (var db = new DB(OBJ, @"root\oblock"))
                                                {
                                                    isBlocked = db.Get(root.Signature);
                                                    db.Close();
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    using (var db = new DB(OBJ, @"root\oblock2"))
                                                    {
                                                        isBlocked = db.Get(root.Signature);
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
                                                bool find = false;

                                                if (filter != "")
                                                {

                                                    if (filter.StartsWith("#"))
                                                    {
                                                        find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1)));
                                                    }
                                                    else
                                                    {

                                                        find = root.Keyword.ContainsKey(filter);


                                                    }
                                                }
                                                else { find = true; }

                                                isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                                if (isobject.URN != null && find == true)
                                                {
                                                    foundobjects.Add(isobject);


                                                    using (var db = new DB(SUP, @"root\found"))
                                                    {
                                                        db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                    }


                                                }


                                            }
                                            else { try { System.IO.Directory.Delete(@"root\" + s, true); } catch { } }

                                        }
                                        else { }

                                    }
                                    catch { }

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

                                        Root root = Root.GetRootByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                        if (root.Signed == true)
                                        {

                                            string isBlocked = "";
                                            var OBJ = new Options { CreateIfMissing = true };
                                            try
                                            {
                                                using (var db = new DB(OBJ, @"root\oblock"))
                                                {
                                                    isBlocked = db.Get(root.Signature);
                                                    db.Close();
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    using (var db = new DB(OBJ, @"root\oblock2"))
                                                    {
                                                        isBlocked = db.Get(root.Signature);
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
                                                bool find = false;

                                                if (filter.Length > 0)
                                                {

                                                    if (filter.StartsWith("#"))
                                                    {
                                                        find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1)));
                                                    }
                                                    else
                                                    {

                                                        find = root.Keyword.ContainsKey(filter);


                                                    }
                                                }
                                                else { find = true; }

                                                isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                                if (isobject.URN != null && find == true)
                                                {
                                                    foundobjects.Add(isobject);

                                                    using (var db = new DB(SUP, @"root\found"))
                                                    {
                                                        db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                    }



                                                }


                                            }
                                            else { try { System.IO.Directory.Delete(@"root\" + s, true); } catch { } }

                                        }
                                        else { }

                                    }
                                    catch { }

                                }

                            }
                        }
                        catch { }
                        newtransactions = new List<string>();

                        if (foundobjects.Count > 0)
                        {


                            this.Invoke((MethodInvoker)delegate
                            {

                                AddToSearchResults(foundobjects);

                            });



                        }

                        using (var db = new DB(SUP, @"root\sup"))
                        {
                            db.Delete("isBuilding");

                        }




                    });
                }
                catch
                {

                    var MUTE = new Options { CreateIfMissing = true };
                    using (var db = new DB(MUTE, @"root\sup"))
                    {
                        db.Delete("isBuilding");
                    }

                }

            }
        }

        private async void ObjectBrowser_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {

                switch (_viewMode)
                {
                    case 0:
                        if (pages.LargeChange != ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 200)) + (flowLayoutPanel1.Width / 200))
                        {
                            pages.LargeChange = ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 200)) + (flowLayoutPanel1.Width / 200);

                            txtQty.Text = pages.LargeChange.ToString();

                            Random rnd = new Random();
                            string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                            if (gifFiles.Length > 0)
                            {
                                int randomIndex = rnd.Next(gifFiles.Length);
                                string randomGifFile = gifFiles[randomIndex];
                                imgLoading.ImageLocation = randomGifFile;
                            }
                            else
                            {
                                imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
                            }

                            await Task.Run(() => BuildSearchResults());
                            flowLayoutPanel1.Visible = true;
                            pages.Visible = true;
                        }
                        break;

                    case 2:
                        if (pages.LargeChange != ((flowLayoutPanel1.Width / 1059) * (flowLayoutPanel1.Height / 411)) + (flowLayoutPanel1.Width / 1059))
                        {
                            pages.LargeChange = ((flowLayoutPanel1.Width / 1059) * (flowLayoutPanel1.Height / 411)) + (flowLayoutPanel1.Width / 1059);

                            txtQty.Text = pages.LargeChange.ToString();

                            Random rnd = new Random();
                            string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                            if (gifFiles.Length > 0)
                            {
                                int randomIndex = rnd.Next(gifFiles.Length);
                                string randomGifFile = gifFiles[randomIndex];
                                imgLoading.ImageLocation = randomGifFile;
                            }
                            else
                            {
                                imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
                            }

                            await Task.Run(() => BuildSearchResults());
                            flowLayoutPanel1.Visible = true;
                            pages.Visible = true;
                        }
                        break;
                    case 1:
                        if (pages.LargeChange != ((flowLayoutPanel1.Width / 221) * (flowLayoutPanel1.Height / 336)))
                        {
                            pages.LargeChange = ((flowLayoutPanel1.Width / 221) * (flowLayoutPanel1.Height / 336)) + (flowLayoutPanel1.Width / 221);

                            txtQty.Text = pages.LargeChange.ToString();

                            Random rnd = new Random();
                            string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                            if (gifFiles.Length > 0)
                            {
                                int randomIndex = rnd.Next(gifFiles.Length);
                                string randomGifFile = gifFiles[randomIndex];
                                imgLoading.ImageLocation = randomGifFile;
                            }
                            else
                            {
                                imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
                            }

                            await Task.Run(() => BuildSearchResults());
                            flowLayoutPanel1.Visible = true;
                            pages.Visible = true;
                        }
                        break;
                    default:
                        // Handle any other cases here
                        break;
                }



            }
        }

        private void pages_Scroll(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {

                txtLast.Text = (pages.Value).ToString();
            }
        }

        private async void pages_MouseUp(object sender, MouseEventArgs e)
        {
            if (_mouseLock == false)
            {

                Random rnd = new Random();
                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                if (gifFiles.Length > 0)
                {
                    int randomIndex = rnd.Next(gifFiles.Length);
                    string randomGifFile = gifFiles[randomIndex];
                    imgLoading.ImageLocation = randomGifFile;
                }
                else
                {
                    imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
                }
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
            }
        }

        private void txtLast_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtSearchAddress.Focus();
                SendKeys.SendWait("{Enter}");
            }
        }

        private void pages_MouseDown(object sender, MouseEventArgs e)
        {

            if (_mouseClicked)
            {
                doubleClickTimer.Stop();
                DoubleClickEventHandler();
                _mouseClicked = false;
                _mouseLock = true;
            }
            else
            {
                _mouseClicked = true;
                _mouseLock = true;
                doubleClickTimer.Interval = DoubleClickInterval;
                doubleClickTimer.Start();

            }

        }

        private async void DoubleClickEventHandler()
        {
            ++_viewMode;
            if (_viewMode == 3) { _viewMode = 0; }

            Random rnd = new Random();
            string[] gifFiles = Directory.GetFiles("includes", "*.gif");
            if (gifFiles.Length > 0)
            {
                int randomIndex = rnd.Next(gifFiles.Length);
                string randomGifFile = gifFiles[randomIndex];
                imgLoading.ImageLocation = randomGifFile;
            }
            else
            {
                imgLoading.ImageLocation = @"includes\HugPuddle.jpg";
            }

            await Task.Run(() => BuildSearchResults());
            flowLayoutPanel1.Visible = true;
            pages.Visible = true;
        }


        private void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            doubleClickTimer.Stop();
            _mouseClicked = false;
            _mouseLock = false;
        }
    }
}
