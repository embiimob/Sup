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
using System.IO;
using NBitcoin.RPC;
using System.Net;
using BitcoinNET.RPCClient;

namespace SUP
{

    public partial class ObjectBrowser : Form
    {
        private readonly string _objectaddress;
        private bool _isUserControl;
        private readonly static object SupLocker = new object();
        private const int DoubleClickInterval = 250;
        private bool _mouseLock;
        private bool _mouseClicked;
        private int _viewMode = 0;
        private bool ipfsActive;
        private string _activeProfile = null;

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

        private void GetObjectsByAddress(string address, bool calculate = false)
        {

            string profileCheck = address;
            PROState searchprofile = new PROState();
            List<OBJState> createdObjects = new List<OBJState>();
            int skip = 0; 
            try { skip = int.Parse(txtLast.Text); } catch { }
            int qty = -1;
            try { qty = int.Parse(txtQty.Text); } catch { };

            if (address.ToUpper().StartsWith(@"SUP:") || address.ToUpper().StartsWith(@"MZC:") || address.ToUpper().StartsWith(@"BTC:") || address.ToUpper().StartsWith(@"LTC:") || address.ToUpper().StartsWith(@"DOG:"))
            {
                string urnsearch = address;
                if (address.ToUpper().StartsWith(@"SUP:")) { urnsearch = address.Substring(6); } else { urnsearch = address.Substring(0, 20); }

                createdObjects = new List<OBJState> { OBJState.GetObjectByURN(urnsearch, "good-user", "better-password", @"http://127.0.0.1:18332") };

            }
            else
            {


                if (btnCreated.BackColor == Color.Yellow && txtSearchAddress.Text != "")
                {
                    try { searchprofile = PROState.GetProfileByAddress(address, "good-user", "better-password", @"http://127.0.0.1:18332", "111", calculate); } catch { }

                    if (searchprofile.URN == null)
                    {
                        searchprofile = PROState.GetProfileByURN(address, "good-user", "better-password", @"http://127.0.0.1:18332", "111", calculate);
                    }

                    if (searchprofile.URN != null)
                    {
                        profileCheck = searchprofile.Creators.First();
                    }
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
                    try { searchprofile = PROState.GetProfileByAddress(address, "good-user", "better-password", @"http://127.0.0.1:18332", "111", calculate); } catch { }

                    if (searchprofile.URN == null)
                    {
                        searchprofile = PROState.GetProfileByURN(address, "good-user", "better-password", @"http://127.0.0.1:18332", "111", calculate);
                    }

                    if (searchprofile.URN != null)
                    {
                        profileCheck = searchprofile.Creators.First();
                    }

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


                        if (!txtSearchAddress.Text.StartsWith("#"))
                        {

                            try { searchprofile = PROState.GetProfileByAddress(address, "good-user", "better-password", @"http://127.0.0.1:18332", "111", calculate); } catch { }
                            
                            if (searchprofile.URN == null)
                            {
                                searchprofile = PROState.GetProfileByURN(address, "good-user", "better-password", @"http://127.0.0.1:18332", "111", calculate);
                            }
                            if (searchprofile.URN != null)
                            {
                                profileCheck = searchprofile.Creators.First();
                            }

                        }
                        else
                        {


                            profileCheck = Root.GetPublicAddressByKeyword(txtSearchAddress.Text.Substring(1), "111");

                        }

                        if (!System.IO.File.Exists("root\\" + profileCheck + "\\GetObjectsByAddress.json"))
                        {
                            this.Invoke((Action)(() =>
                            {
                                flowLayoutPanel1.Visible = false;
                                pages.Visible = false;
                            }));

                        }

                        createdObjects = OBJState.GetObjectsByAddress(profileCheck, "good-user", "better-password", @"http://127.0.0.1:18332", "111", 0, -1,calculate);

                        this.Invoke((Action)(() =>
                        {   if (searchprofile.URN != null || createdObjects.Count > 0)
                            {
                                profileURN.Links[0].LinkData = profileCheck;
                                profileURN.LinkColor = System.Drawing.SystemColors.Highlight;
                                profileURN.Text = txtSearchAddress.Text;
                            }
                        }));

                    }
                }

            }

            try
            {
                this.Invoke((Action)(() =>
                {


                    string selectedValue = selectSort.SelectedItem.ToString();


                    //THANKS GPT3
                    switch (selectedValue)
                    {
                        case "oldest":
                            createdObjects = createdObjects.OrderBy(obj => obj.CreatedDate).ToList();
                            break;

                        case "newest":
                            createdObjects = createdObjects.OrderByDescending(obj => obj.CreatedDate).ToList();
                            break;

                        case "price: highest":
                            createdObjects = createdObjects
                                .Where(obj => obj.Listings != null && obj.Listings.Any())
                                .OrderByDescending(obj => obj.Listings.Values.Select(listing => listing.Value).Min())
                                .ToList();
                            break;

                        case "price: lowest":
                            createdObjects = createdObjects
                                .Where(obj => obj.Listings != null && obj.Listings.Any())
                                .OrderBy(obj => obj.Listings.Values.Select(listing => listing.Value).Min())
                                .ToList();
                            break;

                        case "offer: highest":
                            createdObjects = createdObjects
                                .Where(obj => obj.Offers != null && obj.Offers.Any())
                                .OrderByDescending(obj => obj.Offers.Max(offer => offer.Value))
                                .ToList();
                            break;

                        case "offer: lowest":
                            createdObjects = createdObjects
                                .Where(obj => obj.Offers != null && obj.Offers.Any())
                                .OrderByDescending(obj => obj.Offers.Min(offer => offer.Value))
                                .ToList();
                            break;

                        case "activity: low":
                            createdObjects = createdObjects.OrderBy(obj => obj.ProcessHeight).ToList();
                            break;

                        case "activity: high":
                            createdObjects = createdObjects.OrderByDescending(obj => obj.ProcessHeight).ToList();
                            break;

                        default:
                            // Handle the case when none of the known values match.
                            break;
                    }


                    flowLayoutPanel1.SuspendLayout();
                    pages.Maximum = createdObjects.Count - 1;
                    txtTotal.Text = (createdObjects.Count).ToString();
                    pages.Visible = true;



                    foreach (OBJState objstate in createdObjects.Skip(skip).Take(qty))
                    {
                        try
                        {

                            if (objstate.Owners != null)
                            {
                                string transid = "";
                                FoundObjectControl foundObject = new FoundObjectControl(_activeProfile);
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

                                                Task.Run(() =>
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

                                                });


                                                break;
                                            case "MZC:":

                                                Task.Run(() =>
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

                                                });

                                                break;
                                            case "LTC:":

                                                Task.Run(() =>
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

                                                });

                                                break;
                                            case "DOG:":


                                                Task.Run(() =>
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

                                                });

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

                                                        if (process2.WaitForExit(30000))
                                                        {
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


                                                        }
                                                        else { process2.Kill(); }




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

                                                Task.Run(() =>
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

                                                });


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


                                            try
                                            {


                                                if (profile.URN != null && foundObject.ObjectCreators.Text == "" && objstate.Creators.TryGetValue(creator.Key, out DateTime dateacknowledged) && dateacknowledged.Year > 1975)
                                                {


                                                    foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                                    foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                                                    System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                                    myTooltip.SetToolTip(foundObject.ObjectCreators, profile.URN);
                                                }
                                                else
                                                {


                                                    if (profile.URN != null && foundObject.ObjectCreators2.Text == "" && objstate.Creators.TryGetValue(creator.Key, out DateTime dateacknowledged2) && dateacknowledged2.Year > 1975)
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
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private void GetCollectionsByAddress(string address, bool calculate = false)
        {

            string profileCheck = address;
            PROState searchprofile = new PROState();
            List<COLState> createdObjects = new List<COLState>();
            int skip = int.Parse(txtLast.Text);
            int qty = int.Parse(txtQty.Text);

            try
            {

                if (!address.StartsWith("#"))
                {
                    searchprofile = PROState.GetProfileByURN(address, "good-user", "better-password", @"http://127.0.0.1:18332");


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
                else
                {
                    this.Invoke((Action)(() =>
                    {
                        profileURN.Links[0].LinkData = address;
                        profileURN.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                        profileURN.Text = address;
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




            if (!System.IO.File.Exists("root\\" + profileCheck + "\\GetCollectionsByAddress.json"))
            {
                this.Invoke((Action)(() =>
                {
                    flowLayoutPanel1.Visible = false;
                    pages.Visible = false;
                }));

            }
            createdObjects = OBJState.GetObjectCollectionsByAddress(profileCheck, "good-user", "better-password", @"http://127.0.0.1:18332", "111", 0, -1);




            try
            {
                this.Invoke((Action)(() =>
                {
                    pages.Maximum = createdObjects.Count - 1;
                    txtTotal.Text = (createdObjects.Count).ToString();
                    pages.Visible = true;

                    createdObjects.Reverse();

                    foreach (COLState objstate in createdObjects.Skip(skip).Take(qty))
                    {
                        try
                        {


                            string transid = "";
                            FoundCollectionControl foundObject = new FoundCollectionControl();
                            foundObject.ObjectName.Text = objstate.Name;
                            foundObject.ObjectDescription.Text = objstate.Description;
                            foundObject.ObjectAddress.Text = objstate.URN;

                            string imgurn = "";
                            imgurn = objstate.Image;

                            if (!objstate.Image.ToLower().StartsWith("http"))
                            {
                                imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                                if (objstate.Image.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); if (objstate.Image.Length == 51) { imgurn += @"\artifact"; } }
                            }

                            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                            Match imgurnmatch = regexTransactionId.Match(imgurn);
                            transid = imgurnmatch.Value;

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

                                            Task.Run(() =>
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

                                            });


                                            break;
                                        case "MZC:":

                                            Task.Run(() =>
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

                                            });

                                            break;
                                        case "LTC:":

                                            Task.Run(() =>
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

                                            });

                                            break;
                                        case "DOG:":


                                            Task.Run(() =>
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

                                            });

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

                                                    if (process2.WaitForExit(30000))
                                                    {
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


                                                    }
                                                    else { process2.Kill(); }




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

                                            Task.Run(() =>
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

                                            });


                                            break;
                                    }
                                }

                            }


                            this.Invoke((Action)(() =>
                            {
                                flowLayoutPanel1.Controls.Add(foundObject);
                            }));


                        }
                        catch (Exception ex)
                        {
                            string error = ex.Message;
                        }
                    }
                }));
            }
            catch { }
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
                        FoundObjectControl foundObject = new FoundObjectControl(_activeProfile);
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
                                        Task.Run(() =>
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

                                          });


                                        break;
                                    case "MZC:":

                                        Task.Run(() =>
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

                                        });

                                        break;
                                    case "LTC:":

                                        Task.Run(() =>
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

                                        });

                                        break;
                                    case "DOG:":

                                        Task.Run(() =>
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

                                        });

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
                                        Task.Run(() =>
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

                                            });


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
                btnCollections.BackColor = Color.White;
            }
            txtLast.Text = "0";
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
                flowLayoutPanel1.Visible = false;
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
                btnCollections.BackColor = Color.White;
            }
            txtLast.Text = "0";
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
                flowLayoutPanel1.Visible = false;
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
                EnableSupInput();

            }
        }

        private async void btnCollections_Click(object sender, EventArgs e)
        {
            if (btnCollections.BackColor == Color.Yellow) { btnCollections.BackColor = Color.White; }
            else
            {
                btnCreated.BackColor = Color.White;
                btnOwned.BackColor = Color.White;
                btnCollections.BackColor = Color.Yellow;
            }
            txtLast.Text = "0";
            if (txtSearchAddress.Text != "" && !txtSearchAddress.Text.ToUpper().StartsWith("BTC:") && !txtSearchAddress.Text.ToUpper().StartsWith("MZC:") && !txtSearchAddress.Text.ToUpper().StartsWith("LTC:") && !txtSearchAddress.Text.ToUpper().StartsWith("IPFS:") && !txtSearchAddress.Text.ToUpper().StartsWith("HTTP") && !txtSearchAddress.Text.ToUpper().StartsWith("SUP:"))
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
                flowLayoutPanel1.Visible = false;
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
                EnableSupInput();

            }
        }

        private void MainUserNameClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (profileURN.Links[0].LinkData != null)
            {
                txtSearchAddress.Text = profileURN.Links[0].LinkData.ToString();
            }

        }

        private async void SearchAddressKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                if (txtSearchAddress.Text == "" || txtSearchAddress.Text.StartsWith("#") || txtSearchAddress.Text.ToUpper().StartsWith("SUP:") || txtSearchAddress.Text.ToUpper().StartsWith("HTTP") || txtSearchAddress.Text.ToUpper().StartsWith("BTC:") || txtSearchAddress.Text.ToUpper().StartsWith("MZC:") || txtSearchAddress.Text.ToUpper().StartsWith("LTC:") || txtSearchAddress.Text.ToUpper().StartsWith("DOG:") || txtSearchAddress.Text.ToUpper().StartsWith("IPFS:")) { btnCreated.BackColor = Color.White; btnOwned.BackColor = Color.White; }
                profileURN.Text = "";
                e.Handled = true;
                e.SuppressKeyPress = true;
                DisableSupInput();
                pages.Maximum = 0; pages.Value = 0;
                txtTotal.Text = "0";
                txtLast.Text = "0";
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
                flowLayoutPanel1.Visible = false;
                if (Control.ModifierKeys == Keys.Control)
                {
                    await Task.Run(() => BuildSearchResults(true)); // Pass true when Ctrl is pressed
                }
                else
                {
                    await Task.Run(() => BuildSearchResults());
                }
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
                EnableSupInput();
            }


        }

        private async void BuildSearchResults(bool calculate = false)
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
                            pages.Visible = false;
                        }));

                    }
                    else
                    {
                        this.Invoke((Action)(() =>
                        {
                            flowLayoutPanel1.AutoScroll = true;
                            pages.Visible = true;
                        }));

                        if (txtSearchAddress.Text.StartsWith("#"))
                        {
                            if (btnCollections.BackColor == System.Drawing.Color.Yellow)
                            {

                                GetCollectionsByAddress(txtSearchAddress.Text);


                            }
                            else
                            {
                                GetObjectsByAddress(Root.GetPublicAddressByKeyword(txtSearchAddress.Text.Substring(1), "111"), calculate);
                            }

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
                                    GetObjectsByAddress(txtSearchAddress.Text,calculate);

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

                                        if (btnCollections.BackColor == System.Drawing.Color.Yellow)
                                        {
                                            if (!string.IsNullOrEmpty(txtSearchAddress.Text))
                                            {
                                                GetCollectionsByAddress(txtSearchAddress.Text.Replace("@", ""),calculate);
                                            }
                                            else
                                            {
                                                btnCollections.BackColor = System.Drawing.Color.White;
                                                GetObjectsByAddress("");
                                            }



                                        }
                                        else
                                        {
                                            GetObjectsByAddress(txtSearchAddress.Text.Replace("@", ""),calculate);
                                        }
                                    }

                                }
                            }
                        }
                    }




                }
                catch { }


            }
        }

        private async void ObjectBrowserLoad(object sender, EventArgs e)
        {
            if (_isUserControl) { this.Text = String.Empty; this.flowLayoutPanel1.Padding = new Padding(3, 80, 0, 0); this.Size = this.MinimumSize; btnJukeBox.Visible = false; btnVideoSearch.Visible = false; btnInquiry.Visible = false; }

            Form parentForm = this.Owner;
            bool isBlue = false;
            selectSort.SelectedIndex = 0;

            try
            {

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
            }
            catch { }

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


                if (_objectaddress.Length > 0)
                {
                    txtSearchAddress.Text = _objectaddress;
                    txtLast.Text = "0";

                    switch (_viewMode)
                    {
                        case 1:
                            txtQty.Text = "12";
                            break;
                        case 0:
                            txtQty.Text = "9";
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
                    flowLayoutPanel1.Visible = false;
                    await Task.Run(() => BuildSearchResults());
                    flowLayoutPanel1.Visible = true;
                    pages.Visible = true;
                    EnableSupInput();

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
            btnCollections.Enabled = false;
            selectSort.Enabled = false;
            txtSearchAddress.Enabled = false;
            pages.Enabled = false;
            txtLast.Enabled = false;
        }

        private void EnableSupInput()
        {
            btnOwned.Enabled = true;
            btnCreated.Enabled = true;
            btnCollections.Enabled = true;
            selectSort.Enabled = true;
            txtSearchAddress.Enabled = true;
            pages.Enabled = true;
            txtLast.Enabled = true;
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
                            flowLayoutPanel1.Visible = false;
                            await Task.Run(() => BuildSearchResults());
                            flowLayoutPanel1.Visible = true;
                            pages.Visible = true;
                        }
                    }
                    break;


                case 0:
                    if (pages.LargeChange != ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 360)) + (flowLayoutPanel1.Width / 200)) ;
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
                            flowLayoutPanel1.Visible = false;
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
                        if (pages.LargeChange != ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 360)) + (flowLayoutPanel1.Width / 200)) ;
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
                flowLayoutPanel1.Visible = false;
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;

            }
        }

        private async void txtLast_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
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
                        if (pages.LargeChange != ((flowLayoutPanel1.Width / 200) * (flowLayoutPanel1.Height / 360)) + (flowLayoutPanel1.Width / 200)) ;
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
                try { pages.Value = int.Parse(txtLast.Text); } catch { }
                flowLayoutPanel1.Visible = false;
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
                txtLast.Text = pages.Value.ToString();
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
            flowLayoutPanel1.Visible = false;
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

        //GPT3 <3
        private void txtLast_TextChanged(object sender, EventArgs e)
        {
            string previousText = txtLast.Text;
            if (!int.TryParse(txtLast.Text, out int lastint) || lastint < 0)
            {
                txtLast.Text = previousText;
            }
        }

        private void flowLayoutPanel2_SizeChanged(object sender, EventArgs e)
        {
            flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(3, flowLayoutPanel2.Height, 0, 0);
        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtLast.Text = "0";
            if (txtSearchAddress.Text != "" && !txtSearchAddress.Text.ToUpper().StartsWith("BTC:") && !txtSearchAddress.Text.ToUpper().StartsWith("MZC:") && !txtSearchAddress.Text.ToUpper().StartsWith("LTC:") && !txtSearchAddress.Text.ToUpper().StartsWith("IPFS:") && !txtSearchAddress.Text.ToUpper().StartsWith("HTTP") && !txtSearchAddress.Text.ToUpper().StartsWith("SUP:"))
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
                flowLayoutPanel1.Visible = false;
                await Task.Run(() => BuildSearchResults());
                flowLayoutPanel1.Visible = true;
                pages.Visible = true;
                EnableSupInput();

            }
        }

        private void btnVideoSearch_Click(object sender, EventArgs e)
        {
            btnVideoSearch.Enabled = false;

            if (txtSearchAddress.Text != null)
            {
                SupFlix supFlixForm = new SupFlix(txtSearchAddress.Text);
                supFlixForm.Show();
            }
            else
            {
                SupFlix supFlixForm = new SupFlix();
                supFlixForm.Show();
            }

            btnVideoSearch.Enabled = true;
        }

        private void btnJukeBox_Click(object sender, EventArgs e)
        {

            btnJukeBox.Enabled = false;

            if (txtSearchAddress.Text != null)
            {
                JukeBox jukeBoxForm = new JukeBox(txtSearchAddress.Text);
                jukeBoxForm.Show();
            }
            else
            {
                JukeBox jukeBoxForm = new JukeBox();
                jukeBoxForm.Show();
            }
            btnJukeBox.Enabled = true;

        }

        private void btnInquiry_Click(object sender, EventArgs e)
        {

            btnInquiry.Enabled = false;

            if (txtSearchAddress.Text != null)
            {
                INQSearch INQSearchForm = new INQSearch(txtSearchAddress.Text);
                INQSearchForm.Show();
            }
            else
            {
                INQSearch INQSearchForm = new INQSearch();
                INQSearchForm.Show();
            }
            btnInquiry.Enabled = true;
        }

        //should keep object browser active profile synched with social
        private void profileURN_TextChanged(object sender, EventArgs e)
        {

            if (profileURN.Links[0].LinkData != null)
            {

                try
                {
                    List<string> islocal = Root.GetPublicKeysByAddress(profileURN.Links[0].LinkData.ToString(), "good-user", "better-password", @"http://127.0.0.1:18332");
                    if
                         (islocal.Count == 2)
                    {
                        _activeProfile = profileURN.Links[0].LinkData.ToString();
                    }
                }
                catch { }
            }
        }
    }



}
