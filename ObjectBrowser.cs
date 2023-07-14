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
        private bool _isUserControl;
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
        private bool ipfsActive;
        private bool btctActive;
        private bool btcActive;
        private bool mzcActive;
        private bool ltcActive;
        private bool dogActive;
        private readonly System.Windows.Forms.Timer _doubleClickTimer = new System.Windows.Forms.Timer();
        public ObjectBrowser(string objectaddress, bool iscontrol = false)
        {
            InitializeComponent();
            if (objectaddress != null)
            {
                _objectaddress = objectaddress;
            }
            else
            { _objectaddress = ""; }
            _isUserControl = iscontrol;

        }

        private void GetObjectsByAddress(string address)
        {

            string profileCheck = address;
            PROState searchprofile = new PROState();
            List<OBJState> createdObjects = new List<OBJState>();
            int skip = int.Parse(txtLast.Text);
            int qty = int.Parse(txtQty.Text);

            if (address.ToUpper().StartsWith(@"SUP:") || address.ToUpper().StartsWith(@"MZC:") || address.ToUpper().StartsWith(@"BTC:") || address.ToUpper().StartsWith(@"LTC:") || address.ToUpper().StartsWith(@"DOG:"))
            {
                string urnsearch = address;
                if (address.ToUpper().StartsWith(@"SUP:")) { urnsearch = address.Substring(6); } else { urnsearch = address.Substring(0, 20); }

                createdObjects = new List<OBJState> { OBJState.GetObjectByURN(urnsearch, "good-user", "better-password", @"http://127.0.0.1:18332") };

            }
            else
            {



                if (txtSearchAddress.Text.StartsWith("#"))
                {

                    this.Invoke((Action)(() =>
                    {

                        profileURN.Links[0].LinkData = Root.GetPublicAddressByKeyword(txtSearchAddress.Text.Substring(1), "111");
                        profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
                        profileURN.Text = txtSearchAddress.Text;
                        
                    }));

                }
                else
                {





                    try { searchprofile = PROState.GetProfileByAddress(address, "good-user", "better-password", @"http://127.0.0.1:18332"); } catch { }


                    try
                    {
                        if (searchprofile.URN == null)
                        {
                            searchprofile = PROState.GetProfileByURN(address, "good-user", "better-password", @"http://127.0.0.1:18332");
                        }

                        if (searchprofile.URN != null)
                        {

                            this.Invoke((Action)(() =>
                            {

                                profileURN.Links[0].LinkData = searchprofile.Creators.First();
                                profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
                                profileURN.Text = TruncateAddress(searchprofile.URN);
                                profileCheck = searchprofile.Creators.First();
                            }));

                        }
                        else
                        {
                            this.Invoke((Action)(() =>
                            {
                                profileURN.Links[0].LinkData = address;
                                profileURN.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                profileURN.Text = "anon";
                            }));
                        }
                    }
                    catch
                    {

                        this.Invoke((Action)(() =>
                        {
                            profileURN.Links[0].LinkData = address;
                            profileURN.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                            profileURN.Text = "anon";
                        }));
                    }
                }

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

            }


            this.Invoke((Action)(() =>
            {
                flowLayoutPanel1.SuspendLayout();
                pages.Maximum = createdObjects.Count - 1;
                txtTotal.Text = (createdObjects.Count).ToString();
                pages.Visible = true;

                createdObjects.Reverse();

                foreach (OBJState objstate in createdObjects.Skip(skip).Take(qty))
                {
                    try
                    {

                        if (objstate.Owners != null)
                        {
                            string transid = "";
                            FoundObjectControl foundObject = new FoundObjectControl();
                            foundObject.ObjectName.Text = objstate.Name;
                            foundObject.ObjectDescription.Text = objstate.Description;
                            foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                            foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                            foundObject.ObjectId.Text = objstate.Id.ToString();
 
                            //GPT3 reformed
                            if (objstate.Offers != null && objstate.Offers.Count > 0)
                            {
                                foundObject.Height = 389;
                                decimal highestValue = objstate.Offers.Max(offer => offer.Value);
                                foundObject.ObjectOffer.Text = highestValue.ToString();
                            }
                            //GPT3 reformed
                            if (objstate.Listings != null && objstate.Listings.Count > 0)
                            {
                                foundObject.Height = 389;
                                decimal lowestValue = objstate.Listings.Values.Min(listing => listing.Value);
                                foundObject.ObjectPrice.Text = lowestValue.ToString();
                            }



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

                            string imgurn = "";


                            if (objstate.Image != null)
                            {
                                imgurn = objstate.Image;

                                if (!objstate.Image.ToLower().StartsWith("http"))
                                {
                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                                    if (objstate.Image.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); if (objstate.Image.Length == 51) { imgurn += @"\artifact"; } }
                                }

                                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                                Match imgurnmatch = regexTransactionId.Match(imgurn);
                                transid = imgurnmatch.Value;
                            }
                            if (File.Exists(imgurn))
                            {

                                this.Invoke((Action)(() =>
                                {
                                    foundObject.ObjectImage.ImageLocation = imgurn;
                                }));
                            }
                            else
                            {

                                if (objstate.Image.LastIndexOf('.') > 0 && File.Exists("ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'))))
                                {
                                    this.Invoke((Action)(() =>
                                    {
                                        foundObject.ObjectImage.ImageLocation = "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));
                                    }));
                                }
                                else
                                {

                                    this.Invoke((Action)(() =>
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
                                    }));


                                    switch (objstate.Image.ToUpper().Substring(0, 4))
                                    {
                                        case "BTC:":
                                            if (btcActive)
                                            {
                                                Task.Run(() =>
                                                {
                                                    if (!System.IO.Directory.Exists("root/" + transid))
                                                    {
                                                        Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                                        if (File.Exists(imgurn))
                                                        {

                                                            this.Invoke((Action)(() =>
                                                            {
                                                                foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                                foundObject.ObjectImage.ImageLocation = imgurn;
                                                            }));
                                                        }
                                                    }
                                                });

                                            }
                                            break;
                                        case "MZC:":
                                            if (mzcActive)
                                            {
                                                Task.Run(() =>
                                                {
                                                    if (!System.IO.Directory.Exists("root/" + transid))
                                                    {
                                                        Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                                        if (File.Exists(imgurn))
                                                        {

                                                            this.Invoke((Action)(() =>
                                                            {
                                                                foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                                foundObject.ObjectImage.ImageLocation = imgurn;
                                                            }));
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                        case "LTC:":
                                            if (ltcActive)
                                            {
                                                Task.Run(() =>
                                                {
                                                    if (!System.IO.Directory.Exists("root/" + transid))
                                                    {
                                                        Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                                        if (File.Exists(imgurn))
                                                        {

                                                            this.Invoke((Action)(() =>
                                                            {
                                                                foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                                foundObject.ObjectImage.ImageLocation = imgurn;
                                                            }));
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                        case "DOG:":
                                            if (dogActive)
                                            {
                                                Task.Run(() =>
                                                {
                                                    if (!System.IO.Directory.Exists("root/" + transid))
                                                    {
                                                        Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                                        if (File.Exists(imgurn))
                                                        {

                                                            this.Invoke((Action)(() =>
                                                            {
                                                                foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                                foundObject.ObjectImage.ImageLocation = imgurn;
                                                            }));
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                        case "IPFS":
                                            transid = "empty";
                                            try { transid = objstate.Image.Substring(5, 46); } catch { }

                                            if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                            {


                                                Task.Run(() =>
                                                {
                                                    try { Directory.Delete("ipfs/" + transid, true); } catch { }
                                                    try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
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

                                                        try { System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName); }
                                                        catch (System.ArgumentException ex)
                                                        {

                                                            System.IO.File.Move("ipfs/" + transid + "_tmp", "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                            imgurn = "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));

                                                        }


                                                    }

                                                    if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                    {
                                                        fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                        try { System.IO.File.Move("ipfs/" + transid + "/" + transid, @"ipfs/" + transid + @"/" + fileName); }
                                                        catch (System.ArgumentException ex)
                                                        {

                                                            System.IO.File.Move("ipfs/" + transid + "/" + transid, "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                            imgurn = "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));

                                                        }
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

                                                    try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                                    this.Invoke((Action)(() =>
                                                    {
                                                        foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                        foundObject.ObjectImage.ImageLocation = imgurn;
                                                    }));

                                                });

                                            }

                                            break;
                                        case "HTTP":
                                            Task.Run(() =>
                                            {
                                                foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                foundObject.ObjectImage.ImageLocation = objstate.Image;
                                            });
                                            break;


                                        default:
                                            if (btctActive)
                                            {
                                                Task.Run(() =>
                                                {
                                                    if (!System.IO.Directory.Exists("root/" + transid))
                                                    {
                                                        Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332");

                                                        if (File.Exists(imgurn))
                                                        {

                                                            this.Invoke((Action)(() =>
                                                            {
                                                                foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                                foundObject.ObjectImage.ImageLocation = imgurn;
                                                            }));
                                                        }
                                                    }
                                                });

                                            }
                                            break;
                                    }
                                }

                            }

                            if (_viewMode == 0)
                            {
                                foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                                {

                                    if (creator.Value.Year > 1)
                                    {
                                        PROState profile = PROState.GetProfileByAddress(creator.Key, "good-user", "better-password", @"http://127.0.0.1:18332");
                                        PROState IsRegistered = PROState.GetProfileByURN(profile.URN, "good-user", "better-password", @"http://127.0.0.1:18332");


                                        try
                                        {


                                            if (profile.URN != null && foundObject.ObjectCreators.Text == "" && IsRegistered.Creators.Contains(creator.Key))
                                            {


                                                foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                                foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                                System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                                myTooltip.SetToolTip(foundObject.ObjectCreators, profile.URN);
                                            }
                                            else
                                            {


                                                if (profile.URN != null && foundObject.ObjectCreators2.Text == "" && IsRegistered.Creators.Contains(creator.Key))
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
                                        catch { }


                                    }


                                }
                            }



                            if (_viewMode == 1)
                            {
                                foundObject.Height = 221;
                                if (_isUserControl) { foundObject.Margin = new System.Windows.Forms.Padding(3, 3, 2, 3); }
                                flowLayoutPanel1.Controls.Add(foundObject);


                            }
                            if (_viewMode == 0)
                            {

                                flowLayoutPanel1.Controls.Add(foundObject);

                            }                          


                        }


                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                    }
                }
                flowLayoutPanel1.ResumeLayout();
            }));
        }


        private void GetObjectByFile(string filePath)
        {
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                control.Dispose();
            }

            var SUP = new Options { CreateIfMissing = true };
            using (var db = new DB(SUP, @"ipfs"))
            {

                string ipfsdaemon = db.Get("ipfs-daemon");

                if (ipfsdaemon == "true")
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        ipfsActive = true;
                    });
                }
            }

            flowLayoutPanel1.Controls.Clear();
            int loadQty = (flowLayoutPanel1.Size.Width / 100) * (flowLayoutPanel1.Size.Height / 200) + 3;

            loadQty -= flowLayoutPanel1.Controls.Count;

            txtQty.Text = loadQty.ToString();

            OBJState objstate = OBJState.GetObjectByFile(filePath, "good-user", "better-password", @"http://127.0.0.1:18332");

            if (objstate.Owners != null)
            {
                try
                {

                    if (objstate.Owners != null)
                    {
                        string transid = "";
                        FoundObjectControl foundObject = new FoundObjectControl();
                        foundObject.ObjectName.Text = objstate.Name;
                        foundObject.ObjectDescription.Text = objstate.Description;
                        foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                        foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                        foundObject.ObjectId.Text = objstate.Id.ToString();

                        //GPT3 reformed
                        if (objstate.Offers != null && objstate.Offers.Count > 0)
                        {
                            foundObject.Height = 389;
                            decimal highestValue = objstate.Offers.Max(offer => offer.Value);
                            foundObject.ObjectOffer.Text = highestValue.ToString();
                        }
                        //GPT3 reformed
                        if (objstate.Listings != null && objstate.Listings.Count > 0)
                        {
                            foundObject.Height = 389;
                            decimal lowestValue = objstate.Listings.Values.Min(listing => listing.Value);
                            foundObject.ObjectPrice.Text = lowestValue.ToString();
                        }

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

                        string imgurn = "";


                        if (objstate.Image != null)
                        {
                            imgurn = objstate.Image;

                            if (!objstate.Image.ToLower().StartsWith("http"))
                            {
                                imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");
                                if (objstate.Image.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); if (objstate.Image.Length == 51) { imgurn += @"\artifact"; } }
                            }

                            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                            Match imgurnmatch = regexTransactionId.Match(imgurn);
                            transid = imgurnmatch.Value;
                        }
                        if (File.Exists(imgurn))
                        {

                            this.Invoke((Action)(() =>
                            {
                                foundObject.ObjectImage.ImageLocation = imgurn;
                            }));
                        }
                        else
                        {

                            if (File.Exists("ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'))))
                            {
                                this.Invoke((Action)(() =>
                                {
                                    foundObject.ObjectImage.ImageLocation = "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));
                                }));
                            }
                            else
                            {

                                this.Invoke((Action)(() =>
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
                                }));

                                switch (objstate.Image.ToUpper().Substring(0, 4))
                                {
                                    case "BTC:":
                                        if (btcActive)
                                        {
                                            Task.Run(() =>
                                            {
                                                if (!System.IO.Directory.Exists("root/" + transid))
                                                {
                                                    Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                                    if (File.Exists(imgurn))
                                                    {

                                                        this.Invoke((Action)(() =>
                                                        {
                                                            foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                            foundObject.ObjectImage.ImageLocation = imgurn;
                                                        }));
                                                    }
                                                }
                                            });

                                        }
                                        break;
                                    case "MZC:":
                                        if (mzcActive)
                                        {
                                            Task.Run(() =>
                                            {
                                                if (!System.IO.Directory.Exists("root/" + transid))
                                                {
                                                    Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                                    if (File.Exists(imgurn))
                                                    {

                                                        this.Invoke((Action)(() =>
                                                        {
                                                            foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                            foundObject.ObjectImage.ImageLocation = imgurn;
                                                        }));
                                                    }
                                                }
                                            });
                                        }
                                        break;
                                    case "LTC:":
                                        if (ltcActive)
                                        {
                                            Task.Run(() =>
                                            {
                                                if (!System.IO.Directory.Exists("root/" + transid))
                                                {
                                                    Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                                    if (File.Exists(imgurn))
                                                    {

                                                        this.Invoke((Action)(() =>
                                                        {
                                                            foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                            foundObject.ObjectImage.ImageLocation = imgurn;
                                                        }));
                                                    }
                                                }
                                            });
                                        }
                                        break;
                                    case "DOG:":
                                        if (dogActive)
                                        {
                                            Task.Run(() =>
                                            {
                                                if (!System.IO.Directory.Exists("root/" + transid))
                                                {
                                                    Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                                    if (File.Exists(imgurn))
                                                    {

                                                        this.Invoke((Action)(() =>
                                                        {
                                                            foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                            foundObject.ObjectImage.ImageLocation = imgurn;
                                                        }));
                                                    }
                                                }
                                            });
                                        }
                                        break;
                                    case "IPFS":
                                        transid = "empty";
                                        try { transid = objstate.Image.Substring(5, 46); } catch { }

                                        if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                        {


                                            Task.Run(() =>
                                            {
                                                try { Directory.Delete("ipfs/" + transid, true); } catch { }
                                                try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
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

                                                    try { System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName); }
                                                    catch (System.ArgumentException ex)
                                                    {

                                                        System.IO.File.Move("ipfs/" + transid + "_tmp", "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                        imgurn = "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));

                                                    }


                                                }

                                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                {
                                                    fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                    try { System.IO.File.Move("ipfs/" + transid + "/" + transid, @"ipfs/" + transid + @"/" + fileName); }
                                                    catch (System.ArgumentException ex)
                                                    {

                                                        System.IO.File.Move("ipfs/" + transid + "/" + transid, "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                        imgurn = "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));

                                                    }
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

                                                try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                                this.Invoke((Action)(() =>
                                                {
                                                    foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                    foundObject.ObjectImage.ImageLocation = imgurn;
                                                }));

                                            });

                                        }

                                        break;
                                    case "HTTP":
                                        Task.Run(() =>
                                        {
                                            foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                            foundObject.ObjectImage.ImageLocation = objstate.Image;
                                        });
                                        break;


                                    default:
                                        if (btctActive)
                                        {
                                            Task.Run(() =>
                                            {
                                                if (!System.IO.Directory.Exists("root/" + transid))
                                                {
                                                    Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332");

                                                    if (File.Exists(imgurn))
                                                    {

                                                        this.Invoke((Action)(() =>
                                                        {
                                                            foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                            foundObject.ObjectImage.ImageLocation = imgurn;
                                                        }));
                                                    }
                                                }
                                            });

                                        }
                                        break;
                                }
                            }

                        }

                        if (_viewMode == 0)
                        {
                            foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                            {

                                if (creator.Value.Year > 1)
                                {
                                    PROState profile = PROState.GetProfileByAddress(creator.Key, "good-user", "better-password", @"http://127.0.0.1:18332");
                                    PROState IsRegistered = PROState.GetProfileByURN(profile.URN, "good-user", "better-password", @"http://127.0.0.1:18332");


                                    try
                                    {


                                        if (profile.URN != null && foundObject.ObjectCreators.Text == "" && IsRegistered.Creators.Contains(creator.Key))
                                        {


                                            foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                            foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                            myTooltip.SetToolTip(foundObject.ObjectCreators, profile.URN);
                                        }
                                        else
                                        {


                                            if (profile.URN != null && foundObject.ObjectCreators2.Text == "" && IsRegistered.Creators.Contains(creator.Key))
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
                                    catch { }


                                }


                            }
                        }



                        if (_viewMode == 1)
                        {
                            foundObject.Height = 221;
                            if (_isUserControl) { foundObject.Margin = new System.Windows.Forms.Padding(3, 3, 2, 3); }
                            flowLayoutPanel1.Controls.Add(foundObject);


                        }
                        if (_viewMode == 0)
                        {
                            
                            flowLayoutPanel1.Controls.Add(foundObject);

                        }

                       


                    }


                }
                catch (Exception ex)
                {
                    string error = ex.Message;
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

            txtSearchAddress.Text = profileURN.Links[0].LinkData.ToString();

        }

        private async void SearchAddressKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                if (txtSearchAddress.Text == "" || txtSearchAddress.Text.StartsWith("#") || txtSearchAddress.Text.ToUpper().StartsWith("SUP:") || txtSearchAddress.Text.ToUpper().StartsWith("HTTP") || txtSearchAddress.Text.ToUpper().StartsWith("BTC:") || txtSearchAddress.Text.ToUpper().StartsWith("MZC:") || txtSearchAddress.Text.ToUpper().StartsWith("LTC:") || txtSearchAddress.Text.ToUpper().StartsWith("DOG:") || txtSearchAddress.Text.ToUpper().StartsWith("IPFS:")) { btnCreated.BackColor = Color.White; btnOwned.BackColor = Color.White; }
                e.Handled = true;
                e.SuppressKeyPress = true;
                DisableSupInput();
                pages.Maximum = 0; pages.Value = 0;
                txtTotal.Text = "0";
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
                        flowLayoutPanel1.SuspendLayout();
                        foreach (Control control in flowLayoutPanel1.Controls)
                        {
                            foreach (Control _control in control.Controls)
                            { _control.Dispose(); }
                            control.Dispose();
                        }
                        flowLayoutPanel1.Controls.Clear();
                        flowLayoutPanel1.ResumeLayout();

                    }));



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

                        flowLayoutPanel1.SizeChanged += (sender, e) =>
                        {
                            webBrowser1.Size = flowLayoutPanel1.Size;
                        };

                        this.Invoke((Action)(async () =>
                        {
                            await webBrowser1.EnsureCoreWebView2Async();
                            webBrowser1.CoreWebView2.Navigate(txtSearchAddress.Text);                            
                            flowLayoutPanel1.Controls.Add(webBrowser1);
                            pages.Visible = true;
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

                                        string imgurl = "";
                                        if (!System.IO.Directory.Exists("ipfs/" + ipfsHash + "-build"))
                                        {
                                            try { Directory.Delete("ipfs/" + ipfsHash, true); } catch { }
                                            try { Directory.CreateDirectory("ipfs/" + ipfsHash); } catch { };
                                            Directory.CreateDirectory("ipfs/" + ipfsHash + "-build");
                                            Process process2 = new Process();
                                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                            process2.StartInfo.Arguments = "get " + ipfsHash + @" -o ipfs\" + ipfsHash;
                                            process2.StartInfo.UseShellExecute = false;
                                            process2.StartInfo.CreateNoWindow = true;
                                            process2.Start();
                                            process2.WaitForExit();
                                            string fileName;
                                            if (System.IO.File.Exists("ipfs/" + ipfsHash))
                                            {
                                                System.IO.File.Move("ipfs/" + ipfsHash, "ipfs/" + ipfsHash + "_tmp");
                                                System.IO.Directory.CreateDirectory("ipfs/" + ipfsHash);
                                                fileName = txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                Directory.CreateDirectory("ipfs/" + ipfsHash);
                                                System.IO.File.Move("ipfs/" + ipfsHash + "_tmp", @"ipfs/" + ipfsHash + @"/" + fileName);

                                                try { System.IO.File.Move("ipfs/" + ipfsHash + "_tmp", @"ipfs/" + ipfsHash + @"/" + fileName); }
                                                catch (System.ArgumentException ex)
                                                {

                                                    System.IO.File.Move("ipfs/" + ipfsHash + "_tmp", "ipfs/" + ipfsHash + "/artifact" + txtSearchAddress.Text.Substring(txtSearchAddress.Text.LastIndexOf('.')));
                                                    imgurl = "ipfs/" + ipfsHash + "/artifact" + txtSearchAddress.Text.Substring(txtSearchAddress.Text.LastIndexOf('.'));

                                                }


                                            }

                                            if (System.IO.File.Exists("ipfs/" + ipfsHash + "/" + ipfsHash))
                                            {
                                                fileName = txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                try { System.IO.File.Move("ipfs/" + ipfsHash + "/" + ipfsHash, @"ipfs/" + ipfsHash + @"/" + fileName); }
                                                catch (System.ArgumentException ex)
                                                {

                                                    System.IO.File.Move("ipfs/" + ipfsHash + "/" + ipfsHash, "ipfs/" + ipfsHash + "/artifact" + txtSearchAddress.Text.Substring(txtSearchAddress.Text.LastIndexOf('.')));
                                                    imgurl = "ipfs/" + ipfsHash + "/artifact" + txtSearchAddress.Text.Substring(txtSearchAddress.Text.LastIndexOf('.'));

                                                }
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

                                            try { Directory.Delete("ipfs/" + ipfsHash + "-build", true); } catch { }
                                        }

                                        if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash))
                                        {
                                            Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash);
                                        }
                                        else { System.Windows.Forms.Label filenotFound = new System.Windows.Forms.Label(); filenotFound.AutoSize = true; filenotFound.Text = "IPFS: Search failed! Verify IPFS pinning is enbaled"; flowLayoutPanel1.Controls.Clear(); flowLayoutPanel1.Controls.Add(filenotFound); }

                                        using (var db = new DB(SUP, @"ipfs"))
                                        {
                                            db.Delete(ipfsHash);

                                        }


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
                                    GetObjectsByAddress(txtSearchAddress.Text);

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
                                       
                                        flowLayoutPanel1.SizeChanged += (sender, e) =>
                                        {
                                            webBrowser1.Size = flowLayoutPanel1.Size;
                                        };

                                        this.Invoke((Action)(async () =>
                                        {
                                            
                                            flowLayoutPanel1.Controls.Add(webBrowser1);
                                            await webBrowser1.EnsureCoreWebView2Async();
                                            webBrowser1.CoreWebView2.Navigate(browserPath.Replace(@"/", @"\"));
                                            pages.Visible = true;
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
                            if (objstate.Offers != null & objstate.Offers.Count > 0) { foundObject.Height = 383; BID lowestOffer = objstate.Offers.OrderBy(offer => offer.Value).FirstOrDefault(); foundObject.ObjectOffer.Text = lowestOffer.Value.ToString(); }
                            if (objstate.Listings != null & objstate.Listings.Count > 0) { foundObject.Height = 383; BID lowestListing = objstate.Listings.Values.OrderBy(listing => listing.Value).FirstOrDefault(); foundObject.ObjectPrice.Text = lowestListing.Value.ToString(); }

                            try
                            {
                                switch (objstate.Image.ToUpper().Substring(0, 4))
                                {
                                    case "BTC:":
                                        if (btcActive)
                                        {
                                            if (!System.IO.Directory.Exists("root/" + transid))
                                            {
                                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                            }
                                        }
                                        break;
                                    case "MZC:":
                                        if (mzcActive)
                                        {
                                            if (!System.IO.Directory.Exists("root/" + transid))
                                            {
                                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                            }
                                        }
                                        break;
                                    case "LTC:":
                                        if (ltcActive)
                                        {
                                            if (!System.IO.Directory.Exists("root/" + transid))
                                            {
                                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                            }
                                        }
                                        break;
                                    case "DOG:":
                                        if (dogActive)
                                        {
                                            if (!System.IO.Directory.Exists("root/" + transid))
                                            {
                                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                            }
                                        }
                                        break;
                                    case "IPFS":
                                        if (ipfsActive)
                                        {
                                            string imgurl = "";
                                            if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                            {
                                                try { Directory.Delete("ipfs/" + transid, true); } catch { }
                                                try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
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

                                                    try { System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName); }
                                                    catch (System.ArgumentException ex)
                                                    {

                                                        System.IO.File.Move("ipfs/" + transid + "_tmp", "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                        imgurl = "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));

                                                    }


                                                }

                                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                {
                                                    fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                    try { System.IO.File.Move("ipfs/" + transid + "/" + transid, @"ipfs/" + transid + @"/" + fileName); }
                                                    catch (System.ArgumentException ex)
                                                    {

                                                        System.IO.File.Move("ipfs/" + transid + "/" + transid, "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                        imgurl = "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));

                                                    }
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

                                                try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }


                                            }

                                            if (objstate.Image.Length == 51)
                                            { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                                            else { if (imgurl == "") { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); } else foundObject.ObjectImage.ImageLocation = imgurl; }

                                        }
                                        break;
                                    case "HTTP":
                                        foundObject.ObjectImage.ImageLocation = objstate.Image;
                                        break;


                                    default:
                                        if (btctActive)
                                        {
                                            transid = objstate.Image.Substring(0, 64);
                                            if (!System.IO.Directory.Exists("root/" + transid))
                                            {
                                                Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                            }
                                            foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;
                                        }
                                        break;
                                }
                            }
                            catch { }

                            foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                            {

                                if (creator.Value.Year > 1)
                                {
                                    PROState profile = PROState.GetProfileByAddress(creator.Key, "good-user", "better-password", @"http://127.0.0.1:18332");
                                    PROState IsRegistered = PROState.GetProfileByURN(profile.URN, "good-user", "better-password", @"http://127.0.0.1:18332");

                                    if (profile.URN != null && foundObject.ObjectCreators.Text == "" && IsRegistered.Creators.Contains(creator.Key))
                                    {


                                        foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                        foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                        myTooltip.SetToolTip(foundObject.ObjectCreators, profile.URN);
                                    }
                                    else
                                    {


                                        if (profile.URN != null && foundObject.ObjectCreators2.Text == "" && IsRegistered.Creators.Contains(creator.Key))
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


                            if (_viewMode == 1)
                            {
                                foundObject.Height = 221;
                               
                            }

                            flowLayoutPanel1.Controls.Add(foundObject);
                            this.MinimumSize = new System.Drawing.Size(709, 558);
                            flowLayoutPanel1.Controls.SetChildIndex(foundObject, 0);

                        }
                        flowLayoutPanel1.ResumeLayout();
                    });
                }
                catch { }
            }


        }

        private async void ObjectBrowserLoad(object sender, EventArgs e)
        {
            if (_isUserControl) { this.Text = String.Empty; this.flowLayoutPanel1.Padding = new Padding(3, 80, 0, 0); this.Size = this.MinimumSize; }

            Form parentForm = this.Owner;
            bool isBlue = false;

            var SUP = new Options { CreateIfMissing = true };
            using (var db = new DB(SUP, @"ipfs"))
            {

                string ipfsdaemon = db.Get("ipfs-daemon");

                if (ipfsdaemon == "true")
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        ipfsActive = true;
                    });
                }
            }

            // Check if the parent form has a button named "btnLive" with blue background color
            try
            {
                if (parentForm != null) { isBlue = parentForm.Controls.OfType<System.Windows.Forms.Button>().Any(b => b.Name == "btnLive" && b.BackColor == System.Drawing.Color.Blue); }
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
                        case 1:
                            txtQty.Text = "9";
                            break;
                        case 0:
                            txtQty.Text = "6";
                            break;                      
                        default:
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



                    string walletUsername = "good-user";
                    string walletPassword = "better-password";
                    string walletUrl = @"http://127.0.0.1:18332";
                    NetworkCredential credentials = new NetworkCredential(walletUsername, walletPassword);
                    RPCClient rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                    Task.Run(() =>
                    {
                        try
                        {
                            string isActive = rpcClient.GetBalance().ToString();
                            this.Invoke((MethodInvoker)delegate
                            {
                                btctActive = true;
                            });

                        }
                        catch { }
                    });


                    Task.Run(() =>
                    {
                        try
                        {
                            walletUrl = @"http://127.0.0.1:8332";
                            rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                            string isActive = rpcClient.GetBalance().ToString();
                            this.Invoke((MethodInvoker)delegate
                            {
                                btcActive = true;
                            });

                        }
                        catch { }
                    });



                    Task.Run(() =>
                    {
                        try
                        {
                            walletUrl = @"http://127.0.0.1:9332";
                            rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                            string isActive = rpcClient.GetBalance().ToString();
                            if (decimal.TryParse(isActive, out decimal _))
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    ltcActive = true;
                                });

                            }
                        }
                        catch { }
                    });


                    Task.Run(() =>
                    {
                        try
                        {
                            walletUrl = @"http://127.0.0.1:12832";
                            rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                            string isActive = rpcClient.GetBalance().ToString();
                            if (decimal.TryParse(isActive, out decimal _))
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    mzcActive = true;
                                });

                            }
                        }
                        catch { }
                    });

                    Task.Run(() =>
                    {
                        try
                        {
                            walletUrl = @"http://127.0.0.1:22555";
                            rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                            string isActive = rpcClient.GetBalance().ToString();
                            if (decimal.TryParse(isActive, out decimal _))
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    dogActive = true;
                                });

                            }
                        }
                        catch { }
                    });


                    if (ipfsActive == true)
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

        private void flowLayoutPanel1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {

            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
            {

                e.Effect = System.Windows.Forms.DragDropEffects.Copy;
            }
            else
            {

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

            txtSearchAddress.Enabled = false;
            pages.Enabled = false;
            txtLast.Enabled = false;
        }

        private void EnableSupInput()
        {
            btnOwned.Enabled = true;
            btnCreated.Enabled = true;
            txtSearchAddress.Enabled = true;

            pages.Enabled = true;

            txtLast.Enabled = true;
        }

        private void tmrSearchMemoryPool_Tick(object sender, EventArgs e)
        {
            lock (SupLocker)
            {
                tmrSearchMemoryPool.Stop();

                try
                {
                    Task SearchMemoryTask = Task.Run(() =>
                     {
                         var SUP = new Options { CreateIfMissing = true };
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
                         if (btctActive)
                         {
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
                             catch
                             {

                             }
                         }

                         if (btcActive)
                         {
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
                         }

                         if (mzcActive)
                         {
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
                         }

                         if (ltcActive)
                         {
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
                         }

                         if (dogActive)
                         {
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
                         }




                         if (foundobjects.Count > 0)
                         {

                             this.Invoke((MethodInvoker)delegate
                             {
                                 AddToSearchResults(foundobjects);
                             });

                         }


                         this.Invoke((MethodInvoker)delegate
                         {
                             tmrSearchMemoryPool.Start();
                         });

                     });



                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    this.Invoke((MethodInvoker)delegate
                    {
                        tmrSearchMemoryPool.Start();
                    });

                }



            }
        }

        private async void ObjectBrowser_Resize(object sender, EventArgs e)
        {

            switch (_viewMode)
            {
                case 1:
                    if (pages.LargeChange != ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 200)) + (flowLayoutPanel1.Width / 200))
                    {
                        pages.LargeChange = ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 200)) + (flowLayoutPanel1.Width / 200);

                        txtQty.Text = pages.LargeChange.ToString();



                        if (this.WindowState == FormWindowState.Maximized)
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
                    break;

               
                case 0:
                    if (pages.LargeChange != ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 360)) + (flowLayoutPanel1.Width / 200));
                    {
                        pages.LargeChange = ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 360)) + (flowLayoutPanel1.Width / 200);


                        txtQty.Text = pages.LargeChange.ToString();

                        if (this.WindowState == FormWindowState.Maximized)
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
                    break;

                default:
                    break;
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

                switch (_viewMode)
                {
                    case 1:
                        if (pages.LargeChange != ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 200)) + (flowLayoutPanel1.Width / 200))
                        {
                            pages.LargeChange = ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 200)) + (flowLayoutPanel1.Width / 200);

                            txtQty.Text = pages.LargeChange.ToString();

                        }
                        break;
                  
                     
                    case 0:
                        if (pages.LargeChange != ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 360)) + (flowLayoutPanel1.Width / 200));
                        {
                            pages.LargeChange = ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 360)) + (flowLayoutPanel1.Width / 200);

                            txtQty.Text = pages.LargeChange.ToString();



                        }
                        break;
                    default:

                        break;
                }

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
            if (_viewMode == 2) { _viewMode = 0; }

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

        private void txtLast_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLast.Text, out int lastint) || lastint < 0) { txtLast.Text = "0"; }
        }


        private void flowLayoutPanel2_SizeChanged(object sender, EventArgs e)
        {
            flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(3, flowLayoutPanel2.Height, 0, 0);
        }
    }
}
