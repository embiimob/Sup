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
using System.Drawing.Imaging;
using AngleSharp.Common;
using Newtonsoft.Json;
using System.Globalization;

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
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        private bool _testnet = true;
        int historySeen = 0;

        List<FoundObjectControl> foundObjects = new List<FoundObjectControl>();
        List<PictureBox> pictureBoxes = new List<PictureBox>();

        private readonly System.Windows.Forms.Timer _doubleClickTimer = new System.Windows.Forms.Timer();
        public ObjectBrowser(string objectaddress, bool iscontrol = false, bool testnet = true)
        {
            InitializeComponent();
            if (objectaddress != null)
            {
                _objectaddress = objectaddress;
            }
            else
            { _objectaddress = ""; }
            _isUserControl = iscontrol;

            if (!testnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";
                _testnet = testnet;
            }
            flowLayoutPanel1.MouseWheel += history_MouseWheel;
        }

        private async void ObjectBrowserLoad(object sender, EventArgs e)
        {
            if (_isUserControl) { this.Text = String.Empty; this.flowLayoutPanel1.Padding = new Padding(3, 80, 0, 0); this.Size = this.MinimumSize; btnJukeBox.Visible = false; btnVideoSearch.Visible = false; btnInquiry.Visible = false; }

            Form parentForm = this.Owner;
            bool isBlue = false;
            selectSort.SelectedIndex = 0;


            if (_objectaddress.Length > 0)
            {
                txtSearchAddress.Text = _objectaddress;
                txtLast.Text = "0";
                //txtQty.Text = "20";


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

        private void history_MouseWheel(object sender, MouseEventArgs e)
        {
            if (flowLayoutPanel1.VerticalScroll.Value + flowLayoutPanel1.ClientSize.Height >= flowLayoutPanel1.VerticalScroll.Maximum)
            {
                // Add more PictureBoxes if available              
                if (btnActivity.BackColor == Color.Yellow) { GetHistoryByAddress(txtSearchAddress.Text); }

            }
        }
        static bool IsValidWebPageUrl(string uriString)
        {
            Uri uri;

            // Try to create a Uri instance
            if (Uri.TryCreate(uriString, UriKind.Absolute, out uri))
            {
                // Check if the scheme is http or https
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            }

            return false; // Invalid URI
        }

        private void GetObjectsByAddress(string address, bool calculate = false)
        {

            string profileCheck = address;
            PROState searchprofile = new PROState();
            List<OBJState> createdObjects = new List<OBJState>();
            int skip = 0;
            try { skip = int.Parse(txtLast.Text); } catch { }
            int qty = 32;

            if (address.ToUpper().StartsWith(@"SUP:") || address.ToUpper().StartsWith(@"MZC:") || address.ToUpper().StartsWith(@"BTC:") || address.ToUpper().StartsWith(@"LTC:") || address.ToUpper().StartsWith(@"DOG:"))
            {
                string urnsearch = address;
                if (address.ToUpper().StartsWith(@"SUP:")) { urnsearch = address.Substring(6); } else { urnsearch = address.Substring(0, 20); }

                createdObjects = new List<OBJState> { OBJState.GetObjectByURN(urnsearch, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte) };

                if (createdObjects.Count > 0 && createdObjects[0].URI != null && IsValidWebPageUrl(createdObjects[0].URI))
                {
                    Process.Start(createdObjects[0].URI);
                    return;

                }


            }
            else
            {


                if (btnCreated.BackColor == Color.Yellow && txtSearchAddress.Text != "")
                {
                    try { searchprofile = PROState.GetProfileByAddress(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, calculate); } catch { }

                    if (searchprofile.URN == null)
                    {
                        searchprofile = PROState.GetProfileByURN(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, calculate);
                    }

                    if (searchprofile.URN != null)
                    {
                        profileCheck = searchprofile.Creators.First();
                    }
                    else { profileCheck = address; }
                    if (!System.IO.File.Exists("root\\" + profileCheck + "\\GetObjectsCreatedByAddress.json"))
                    {
                        this.Invoke((Action)(() =>
                        {
                            flowLayoutPanel1.Visible = false;
                            pages.Visible = false;
                        }));

                    }
                    createdObjects = OBJState.GetObjectsCreatedByAddress(profileCheck, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, 0, -1);

                }
                else if (btnOwned.BackColor == Color.Yellow && txtSearchAddress.Text != "")
                {
                    try { searchprofile = PROState.GetProfileByAddress(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, calculate); } catch { }

                    if (searchprofile.URN == null)
                    {
                        searchprofile = PROState.GetProfileByURN(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, calculate);
                    }

                    if (searchprofile.URN != null)
                    {
                        profileCheck = searchprofile.Creators.First();
                    }
                    else { profileCheck = address; }

                    if (!System.IO.File.Exists("root\\" + profileCheck + "\\GetObjectsOwnedByAddress.json"))
                    {
                        this.Invoke((Action)(() =>
                        {
                            flowLayoutPanel1.Visible = false;
                            pages.Visible = false;
                        }));

                    }
                    createdObjects = OBJState.GetObjectsOwnedByAddress(profileCheck, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, 0, -1);

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
                        createdObjects = OBJState.GetFoundObjects(mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, calculate);


                    }
                    else
                    {


                        if (!txtSearchAddress.Text.StartsWith("#"))
                        {

                            try { searchprofile = PROState.GetProfileByAddress(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, calculate); } catch { }

                            if (searchprofile.URN == null)
                            {
                                searchprofile = PROState.GetProfileByURN(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, calculate);
                            }
                            if (searchprofile.URN != null)
                            {
                                profileCheck = searchprofile.Creators.First();
                            }
                            else { profileCheck = address; }


                        }
                        else
                        {


                            profileCheck = Root.GetPublicAddressByKeyword(txtSearchAddress.Text.Substring(1), mainnetVersionByte);

                        }

                        if (!System.IO.File.Exists("root\\" + profileCheck + "\\GetObjectsByAddress.json"))
                        {
                            this.Invoke((Action)(() =>
                            {
                                flowLayoutPanel1.Visible = false;
                                pages.Visible = false;
                            }));

                        }

                        createdObjects = OBJState.GetObjectsByAddress(profileCheck, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, 0, -1, calculate);

                        this.Invoke((Action)(() =>
                        {
                            if (searchprofile.URN != null || createdObjects.Count > 0 || txtSearchAddress.Text.StartsWith("#"))
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


                    if (createdObjects.Count == 1 && createdObjects[0].Owners == null) { return; }

                    flowLayoutPanel1.SuspendLayout();
                    pages.Maximum = createdObjects.Count - 1;
                    txtTotal.Text = (createdObjects.Count).ToString();
                    pages.Visible = true;

                    //disposeFoundObjects
                    foreach (var foundobject in foundObjects)
                    {

                        try { foundobject.Dispose(); } catch { }

                    }

                    foreach (OBJState objstate in createdObjects.Skip(skip).Take(qty))
                    {
                        try
                        {

                            if (objstate.Owners != null)
                            {
                                string transid = "";
                                FoundObjectControl foundObject = new FoundObjectControl(_activeProfile, _testnet);
                                foundObject.ObjectName.Text = objstate.Name;
                                foundObject.ObjectDescription.Text = objstate.Description;
                                if (!objstate.URN.ToUpper().StartsWith("IPFS") && !objstate.URN.ToUpper().StartsWith("HTTP") && !txtSearchAddress.Text.ToUpper().StartsWith("SUP"))
                                {
                                    try
                                    {
                                        string urnmsgpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URN.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Substring(0, 64) + @"\MSG";

                                        // Check if the file exists at urnmsgpath
                                        if (System.IO.File.Exists(urnmsgpath))
                                        {
                                            // Read the text from the file
                                            string fileText = System.IO.File.ReadAllText(urnmsgpath);

                                            // Append the text to foundObject.ObjectDescription.Text
                                            if (string.IsNullOrEmpty(foundObject.ObjectDescription.Text))
                                            {
                                                foundObject.ObjectDescription.Text = fileText;
                                            }
                                            else
                                            {
                                                foundObject.ObjectDescription.Text += Environment.NewLine + fileText;
                                            }
                                        }
                                    }
                                    catch { }
                                }
                                foundObject.ObjectAddress.Text = objstate.Creators.First().Key;

                                if (btnOwned.BackColor == Color.Yellow)
                                {
                                    if (objstate.Owners.TryGetValue(profileCheck, out var tuple))
                                    {
                                        long ownerQty = tuple.Item1;
                                        foundObject.ObjectQty.Text = $"{ownerQty.ToString("N0")} / {objstate.Owners.Values.Sum(t => t.Item1).ToString("N0")}x";
                                    }
                                    else
                                    {
                                        foundObject.ObjectQty.Text = $"{objstate.Owners.Values.Sum(t => t.Item1).ToString("N0")}x";
                                    }
                                }
                                else
                                {
                                    foundObject.ObjectQty.Text = $"{objstate.Owners.Values.Sum(t => t.Item1).ToString("N0")}x";
                                }


                                foundObject.ObjectId.Text = objstate.Id.ToString();

                                //GPT3 reformed
                                if (objstate.Offers != null && objstate.Offers.Count > 0)
                                {
                                    foundObject.Height = 395;
                                    decimal highestValue = objstate.Offers.Max(offer => offer.Value);
                                    foundObject.ObjectOffer.Text = highestValue.ToString();
                                }
                                //GPT3 reformed
                                if (objstate.Listings != null && objstate.Listings.Count > 0)
                                {
                                    foundObject.Height = 395;
                                    decimal lowestValue = objstate.Listings.Values.Min(listing => listing.Value);
                                    foundObject.ObjectPrice.Text = lowestValue.ToString();
                                }



                                OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
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
                                    // Check to see if objstate.URN has an image extension
                                    string[] validImageExtensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".ico", ".tiff", ".wmf", ".emf" }; // Add more if needed

                                    bool hasValidImageExtension = validImageExtensions.Any(extension =>
                                        objstate.URN.EndsWith(extension, StringComparison.OrdinalIgnoreCase));

                                    if (hasValidImageExtension)
                                    {
                                        objstate.Image = objstate.URN;
                                    }
                                    else
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
                                if (System.IO.File.Exists(imgurn))
                                {
                                    this.Invoke((Action)(() =>
                                    {
                                        string thumbnailPath = imgurn + "-thumbnail.jpg";

                                        // Check if a thumbnail exists
                                        if (System.IO.File.Exists(thumbnailPath))
                                        {
                                            foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                            foundObject.ObjectImage.ImageLocation = thumbnailPath;
                                        }
                                        else
                                        {
                                            // Load the original image from file
                                            Image originalImage = Image.FromFile(imgurn);

                                            // Check if the original image is a GIF
                                            if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                                            {
                                                // For GIF images, directly use the original image without creating a thumbnail
                                                foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                foundObject.ObjectImage.Image = originalImage;
                                            }
                                            else
                                            {
                                                // Resize the image if needed
                                                int maxWidth = foundObject.ObjectImage.Width;
                                                int maxHeight = foundObject.ObjectImage.Height;

                                                int newWidth, newHeight;
                                                if (originalImage.Width > originalImage.Height)
                                                {
                                                    newWidth = maxWidth;
                                                    newHeight = (int)((double)originalImage.Height / originalImage.Width * newWidth);
                                                }
                                                else
                                                {
                                                    newHeight = maxHeight;
                                                    newWidth = (int)((double)originalImage.Width / originalImage.Height * newHeight);
                                                }

                                                Image resizedImage = new Bitmap(originalImage, newWidth, newHeight);
                                                originalImage.Dispose();
                                                foundObject.ObjectImage.Image = resizedImage;

                                                // Save the resized image as a thumbnail
                                                resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                            }
                                        }
                                    }));
                                }
                                else
                                {

                                    if (objstate.Image.LastIndexOf('.') > 0 && System.IO.File.Exists("ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'))))
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

                                                    if (System.IO.File.Exists(imgurn))
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

                                                    if (System.IO.File.Exists(imgurn))
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

                                                    if (System.IO.File.Exists(imgurn))
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

                                                    if (System.IO.File.Exists(imgurn))
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
                                                else
                                                {
                                                    Task.Run(() =>
                                                    {
                                                        DateTime startTime = DateTime.Now;

                                                        while (DateTime.Now - startTime < TimeSpan.FromSeconds(10))
                                                        {
                                                            // Check if the file exists
                                                            if (System.IO.File.Exists(imgurn))
                                                            {
                                                                // File exists, so perform the desired action
                                                                this.Invoke((Action)(() =>
                                                                {
                                                                    foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                                    foundObject.ObjectImage.ImageLocation = imgurn;
                                                                }));
                                                                break; // Exit the loop since you've found the file
                                                            }

                                                            // Sleep for 100 milliseconds before the next check
                                                            System.Threading.Thread.Sleep(100);
                                                        }
                                                    });

                                                }

                                                break;
                                            case "HTTP":
                                                Task.Run(() =>
                                                {
                                                    this.Invoke((Action)(() =>
                                                    {
                                                        foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                                        foundObject.ObjectImage.ImageLocation = objstate.Image;

                                                    }));
                                                });
                                                break;


                                            default:

                                                Task.Run(() =>
                                                {

                                                    Root.GetRootByTransactionId(transid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                                    if (System.IO.File.Exists(imgurn))
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
                                            PROState profile = PROState.GetProfileByAddress(creator.Key, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);


                                            try
                                            {


                                                if (profile.URN != null && foundObject.ObjectCreators.Text == "" && objstate.Creators.TryGetValue(creator.Key, out DateTime dateacknowledged) && dateacknowledged.Year > 1975)
                                                {


                                                    foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                                    foundObject.ObjectCreators.Links.Add(0, profile.URN.Length, profile.URN);
                                                    System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                                                    myTooltip.SetToolTip(foundObject.ObjectCreators, profile.URN);
                                                }
                                                else
                                                {


                                                    if (profile.URN != null && foundObject.ObjectCreators2.Text == "" && objstate.Creators.TryGetValue(creator.Key, out DateTime dateacknowledged2) && dateacknowledged2.Year > 1975)
                                                    {
                                                        foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                                        foundObject.ObjectCreators2.Links.Add(0, profile.URN.Length, profile.URN);
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

                                foundObjects.Add(foundObject);
                                foundObject.Margin = new System.Windows.Forms.Padding(3, 3, 2, 3);

                                if (_viewMode == 1)
                                {
                                    foundObject.Height = 250;
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

                    this.Invoke((Action)(() =>
                    {
                        Label bottomSpacer = new Label();
                        bottomSpacer.Height = 500; // Adjust the height to create enough space.
                        flowLayoutPanel1.Controls.Add(bottomSpacer);
                        flowLayoutPanel1.ResumeLayout();
                    }));
                }));
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }


        void Owner_LinkClicked(string ownerId)
        {

            new ObjectBrowser(ownerId, false, _testnet).Show();
        }

        void object_LinkClicked(string objectAddress)
        {

            new ObjectDetails(objectAddress, _activeProfile, false, _testnet).Show();
        }

        void CreateFeedRow(string objectaddress, string imageLocation, string SentTo, string SentFrom, DateTime timestamp, string messageText, string transactionid, System.Drawing.Color bgcolor, FlowLayoutPanel layoutPanel, bool addtoTop = false)
        {
            OBJState isFromObject = new OBJState();
            OBJState isToObject = new OBJState();
            OBJState isGiveObject = new OBJState();
            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 4,
                AutoSize = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Tag = transactionid

            };

            int objectadjustment = 0;
            if (objectaddress != null) { objectadjustment = 40; }
            // Add the width of the first column to fixed value and second to fill remaining space
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
            if (flowLayoutPanel1.Width < 400)
            {
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, flowLayoutPanel1.Width - (240 + objectadjustment)));
            }
            else
            {
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, flowLayoutPanel1.Width - (400 + objectadjustment)));
            }


            if (addtoTop)
            {
                layoutPanel.Controls.Add(row);
                layoutPanel.Controls.SetChildIndex(row, 0);
            }
            else
            {
                layoutPanel.Controls.Add(row);
            }



            // Create a PictureBox with the specified image

            if (System.IO.File.Exists(imageLocation + "-thumbnail.jpg") || imageLocation.ToUpper().StartsWith("HTTP"))
            {
                PictureBox picture = new PictureBox
                {
                    Size = new System.Drawing.Size(40, 40),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    ImageLocation = imageLocation + "-thumbnail.jpg",
                    Margin = new System.Windows.Forms.Padding(0),

                };
                row.Controls.Add(picture, 0, 0);
            }
            else
            {
                Random rnd = new Random();
                string randomGifFile;
                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                if (gifFiles.Length > 0)
                {
                    int randomIndex = rnd.Next(gifFiles.Length);
                    randomGifFile = gifFiles[randomIndex];
                }
                else
                {
                    randomGifFile = @"includes\HugPuddle.jpg";
                }



                PictureBox picture = new PictureBox
                {
                    Size = new System.Drawing.Size(40, 40),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageLocation = randomGifFile,
                    Margin = new System.Windows.Forms.Padding(0),
                };
                row.Controls.Add(picture, 0, 0);
            }



            PROState profile = PROState.GetProfileByAddress(SentFrom, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            if (profile.URN != null)
            {
                SentFrom = profile.URN;
            }
            else
            {
                isFromObject = OBJState.GetObjectByAddress(SentTo, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            }

            profile = PROState.GetProfileByAddress(SentTo, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            if (profile.URN != null)
            {
                SentTo = profile.URN;
            }
            else
            {
                isToObject = OBJState.GetObjectByAddress(SentTo, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                messageText = isToObject.Name + "\n" + messageText;
            }

            if (objectaddress != null)
            {
                isGiveObject = OBJState.GetObjectByAddress(objectaddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
            }



            if (isFromObject.Creators != null)
            {

                string objectImagelocation = "";
                string imagepath = "";
                if (isFromObject.Image == null) { imagepath = isFromObject.URN; } else { imagepath = isFromObject.Image; }
                if (!imagepath.ToLower().StartsWith("http"))
                {
                    objectImagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (imagepath.ToLower().StartsWith("ipfs:")) { objectImagelocation = objectImagelocation.Replace(@"\root\", @"\ipfs\"); if (imagepath.Length == 51) { objectImagelocation += @"\artifact"; } }
                }


                PictureBox picture = new PictureBox
                {
                    Size = new System.Drawing.Size(40, 40),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageLocation = objectImagelocation + "-thumbnail.jpg",
                    Margin = new System.Windows.Forms.Padding(0),
                };


                picture.MouseClick += (sender, e) => { object_LinkClicked(SentFrom); };

                row.Controls.Add(picture, 1, 0);

            }
            else
            {
                // Create a LinkLabel with the owner name
                LinkLabel sentfrom = new LinkLabel
                {
                    Text = SentFrom,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true,
                    Dock = DockStyle.Bottom

                };
                if (SentTo != "primary")
                {
                    sentfrom.LinkClicked += (sender, e) => { Owner_LinkClicked(SentFrom); };
                }
                sentfrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                sentfrom.Margin = new System.Windows.Forms.Padding(3);
                row.Controls.Add(sentfrom, 1, 0);
            }


            if (isToObject.Creators != null)
            {

                string objectImagelocation = "";
                string imagepath = "";
                if (isToObject.Image == null) { if (isToObject.URN != null) { imagepath = isToObject.URN; } } else { imagepath = isToObject.Image; }
                if (!imagepath.ToLower().StartsWith("http"))
                {
                    objectImagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (imagepath.ToLower().StartsWith("ipfs:")) { objectImagelocation = objectImagelocation.Replace(@"\root\", @"\ipfs\"); if (imagepath.Length == 51) { objectImagelocation += @"\artifact"; } }
                }


                PictureBox picture = new PictureBox
                {
                    Size = new System.Drawing.Size(40, 40),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageLocation = objectImagelocation + "-thumbnail.jpg",
                    Margin = new System.Windows.Forms.Padding(0),
                };


                picture.MouseClick += (sender, e) => { object_LinkClicked(SentTo); };

                row.Controls.Add(picture, 2, 0);

            }
            else
            {
                // Create a LinkLabel with the owner name
                LinkLabel sentto = new LinkLabel
                {
                    Text = SentTo,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true,
                    Dock = DockStyle.Bottom

                };
                if (SentTo != "primary")
                {
                    sentto.LinkClicked += (sender, e) => { Owner_LinkClicked(SentTo); };
                }
                sentto.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                sentto.Margin = new System.Windows.Forms.Padding(3);
                row.Controls.Add(sentto, 2, 0);
            }


 
            Label message = new Label
            {
                AutoSize = true,
                Text = messageText,
                Font = new System.Drawing.Font("Segoe UI", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Margin = new System.Windows.Forms.Padding(0),
                Padding = new System.Windows.Forms.Padding(0),
                TextAlign = ContentAlignment.TopLeft

            };
            if (flowLayoutPanel1.Width < 400)
            {
                try { message.MinimumSize = new Size(flowLayoutPanel1.Width - (240 + objectadjustment), 46); } catch { }
            }
            else
            {
                try{ message.MinimumSize = new Size(flowLayoutPanel1.Width - (400 + objectadjustment), 46); }catch{ }
            }
            row.Controls.Add(message, 3, 0);

            if (isGiveObject.URN != null)
            {
                string objectImagelocation = "";
                string imagepath = "";
                if (isGiveObject.Image == null) { imagepath = isGiveObject.URN; } else { imagepath = isGiveObject.Image; }
                if (!imagepath.ToLower().StartsWith("http"))
                {
                    objectImagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (imagepath.ToLower().StartsWith("ipfs:")) { objectImagelocation = objectImagelocation.Replace(@"\root\", @"\ipfs\"); if (imagepath.Length == 51) { objectImagelocation += @"\artifact"; } }
                }

                PictureBox picture = new PictureBox
                {
                    Size = new System.Drawing.Size(40, 40),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageLocation = objectImagelocation + "-thumbnail.jpg",
                    Margin = new System.Windows.Forms.Padding(0),
                };


                picture.MouseClick += (sender, e) => { object_LinkClicked(objectaddress); };
                flowLayoutPanel1.Controls.Add(picture);
            }

            Label timeStamp = new Label
            {
                AutoSize = true,
                Text = timestamp.ToString(),
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = Color.White,
                MinimumSize = new Size(140, 46),
                Font = new System.Drawing.Font("Segoe UI", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Margin = new System.Windows.Forms.Padding(0),
                Padding = new System.Windows.Forms.Padding(0)

            };
            flowLayoutPanel1.Controls.Add(timeStamp);


        }

        private void GetHistoryByAddress(string address, bool calculate = false)
        {
            string profileCheck = address;
            PROState searchprofile = new PROState();
            if (historySeen == 0)
            {
                Task.Run(() =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        foreach (var picturebox in pictureBoxes)
                        {

                            try { picturebox.Dispose(); } catch { }

                        }
                    });

                });
                pictureBoxes.Clear();

            }

            try
            {

                if (!address.StartsWith("#"))
                {
                    searchprofile = PROState.GetProfileByURN(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);


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


            string myFriendsJson = "";
            string friendPath = @"root\MyFriendList.Json";
            Dictionary<string, string> myFriends = new Dictionary<string, string>();

            if (mainnetVersionByte == "0") { friendPath = @"root\MyProdFriendList.Json"; }

            if (System.IO.File.Exists(friendPath))
            {
                myFriendsJson = System.IO.File.ReadAllText(friendPath);
                myFriends = JsonConvert.DeserializeObject<Dictionary<string, string>>(myFriendsJson);
            }


            List<Root> combinedRoots = Root.GetRootsByAddress(profileCheck, mainnetLogin, mainnetPassword, mainnetURL, 0, -1, mainnetVersionByte, calculate).ToList();

            List<OBJState> objStates = OBJState.GetObjectsCreatedByAddress(profileCheck, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            try
            {
                foreach (OBJState objState in objStates)
                {
                    // Assuming Creators is a List<string>, change the type accordingly
                    string creator = objState.Creators.Count > 0 ? objState.Creators.First().Key : null;

                    if (creator != null)
                    {
                        Root[] roots = Root.GetRootsByAddress(creator, mainnetLogin, mainnetPassword, mainnetURL, 0, -1, mainnetVersionByte, calculate);
                        combinedRoots.AddRange(roots);
                    }
                }
            }
            catch { }

            try
            {
                objStates = OBJState.GetObjectsOwnedByAddress(profileCheck, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                foreach (OBJState objState in objStates)
                {
                    // Assuming Creators is a List<string>, change the type accordingly
                    string creator = objState.Creators.Count > 0 ? objState.Creators.First().Key : null;

                    if (creator != null)
                    {
                        Root[] roots = Root.GetRootsByAddress(creator, mainnetLogin, mainnetPassword, mainnetURL, 0, -1, mainnetVersionByte, calculate);
                        combinedRoots.AddRange(roots);
                    }
                }
            }
            catch { }
            // Filter out duplicates based on TransactionId
            combinedRoots = combinedRoots.GroupBy(root => root.TransactionId)
                                         .Select(group => group.First())
                                         .ToList();
            // Sort the combinedRoots by BlockDate
            combinedRoots.Sort((root1, root2) => root2.BlockDate.CompareTo(root1.BlockDate));

            // Now combinedRoots contains all roots sorted by BlockDate
            int found = 0;
            int lastSeen = historySeen;
            flowLayoutPanel1.SuspendLayout();
            for (int i = 0; found < 25 && i < combinedRoots.Skip(lastSeen).Count(); i++)
            {
                Root root = combinedRoots[historySeen];
                ++historySeen;

                try
                {
                    if (root.Signed == true)
                    {



                        if (!System.IO.File.Exists(@"root\" + root.SignedBy + @"\BLOCK"))
                        {



                            switch (root.File.Last().Key.ToString().Substring(root.File.Last().Key.ToString().Length - 3))
                            {
                                case "OBJ":


                                    OBJ objinspector = new OBJ();
                                    try
                                    {
                                        objinspector = JsonConvert.DeserializeObject<OBJ>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\OBJ"));
                                    }
                                    catch
                                    {

                                        break;
                                    }

                                    if (objinspector == null)
                                    {
                                        break;
                                    }
                                    else
                                    {

                                        string _from = root.SignedBy;
                                        string _to = "";
                                        string _message = "";

                                        try { _to = objinspector.cre.First(); } catch { }
                                        try { if (objinspector.own != null) { _message = "MINT 💎 " + objinspector.own.Values.Sum(); } } catch { }
                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                        string imglocation = objinspector.img;
                                        if (imglocation == null) { imglocation = objinspector.urn; }
                                        if (imglocation == null) { imglocation = ""; }


                                        this.Invoke((MethodInvoker)delegate
                                                {
                                                    try { imglocation = myFriends[_from]; } catch { }

                                                    CreateFeedRow(null, imglocation, _to, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowLayoutPanel1);
                                                    found++;
                                                });

                                    }


                                    break;



                                case "GIV":


                                    List<List<int>> givinspector = new List<List<int>> { };
                                    try
                                    {
                                        givinspector = JsonConvert.DeserializeObject<List<List<int>>>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\GIV"));
                                    }
                                    catch
                                    {

                                        break;
                                    }

                                    if (givinspector == null)
                                    {
                                        break;
                                    }
                                    List<int> firstElements = givinspector
            .Where(sublist => sublist.Count > 0 && sublist[0] > 0)
            .Select(sublist => sublist[0])
            .ToList();

                                        // Find the lowest integer among the selected first elements
                                        int lowestFirstElement = firstElements.Min();
                                        Console.WriteLine("The lowest integer among the first elements greater than or equal to -1 is: " + lowestFirstElement);
                                    

                                    foreach (var give in givinspector)
                                    {
                                        if (root.Keyword.Count > lowestFirstElement)
                                        {
                                            for (int g = 1; g < lowestFirstElement; g++)
                                            {
                                                string _from = root.SignedBy;
                                                string _to = "";
                                                string objectaddress = root.Keyword.Reverse().GetItemByIndex(g).Key;

                                                try { _to = root.Keyword.Reverse().GetItemByIndex(give[0]).Key; } catch { }
                                                string _message = "GIV 💕 ";
                                                try { _message = _message + give[1]; } catch { }
                                                string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                string imglocation = "";

                                                if (give[1] > 0)
                                                {


                                                    this.Invoke((MethodInvoker)delegate
                                                    {
                                                        try { imglocation = myFriends[_from]; } catch { }

                                                        CreateFeedRow(objectaddress, imglocation, _to, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowLayoutPanel1);
                                                        found++;
                                                    });
                                                }
                                            }
                                        }

                                    }
                                    break;

                                case "BUY":



                                    List<List<string>> buyinspector = new List<List<string>> { };
                                    try
                                    {
                                        buyinspector = JsonConvert.DeserializeObject<List<List<string>>>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\BUY"));
                                    }
                                    catch
                                    {

                                        break;
                                    }

                                    if (buyinspector == null)
                                    {
                                        break;
                                    }

                                    foreach (var buy in buyinspector)
                                    {
                                        string _from = root.SignedBy;
                                        string _to = buy[0];
                                        string _message = "BUY 💰 " + buy[1];
                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                        string imglocation = "";

                                        if (long.Parse(buy[1]) < 0)
                                        {
                                            break;
                                        }


                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            try { imglocation = myFriends[_from]; } catch { }

                                            CreateFeedRow(null, imglocation, _to, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowLayoutPanel1);
                                            found++;
                                        });




                                    }
                                    break;

                                case "LST":


                                    List<List<string>> lstinspector = new List<List<string>> { };
                                    try
                                    {
                                        lstinspector = JsonConvert.DeserializeObject<List<List<string>>>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\LST"));
                                    }
                                    catch
                                    {

                                        break;
                                    }

                                    if (lstinspector == null)
                                    {
                                        break;
                                    }

                                    foreach (var lst in lstinspector)
                                    {
                                        string _from = root.SignedBy;
                                        string _to = lst[0];


                                        string _message = "LST 📰 ";

                                        try { _message = _message + lst[1] + " at "; } catch { }
                                        try { _message = _message + lst[2] + " each"; } catch { }
                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                        string imglocation = "";

                                        if (long.Parse(lst[1]) < 0)
                                        {
                                            break;
                                        }


                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            try { imglocation = myFriends[_from]; } catch { }

                                            CreateFeedRow(null, imglocation, _to, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowLayoutPanel1);
                                            found++;
                                        });


                                    }
                                    break;

                                case "BRN":


                                    List<List<int>> brninspector = new List<List<int>> { };
                                    try
                                    {
                                        brninspector = JsonConvert.DeserializeObject<List<List<int>>>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\BRN"));
                                    }
                                    catch
                                    {

                                        break;
                                    }

                                    if (brninspector == null)
                                    {
                                        break;
                                    }
                                    

                                    foreach (var burn in brninspector)
                                    {
                                        if (root.Keyword.GetItemByIndex(burn[0]).Key != null)
                                        {
                                           
                                                string _from = root.SignedBy;
                                                string _to = "";
                                                string objectaddress = root.Keyword.GetItemByIndex(burn[0]).Key;

                                                try { _to = root.Keyword.Reverse().GetItemByIndex(burn[0]).Key; } catch { }
                                                string _message = "BURN 🔥 ";
                                                try { _message = _message + burn[1]; } catch { }
                                                string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                string imglocation = "";

                                                if (burn[1] > 0)
                                                {


                                                    this.Invoke((MethodInvoker)delegate
                                                    {
                                                        try { imglocation = myFriends[_from]; } catch { }

                                                        CreateFeedRow(objectaddress, imglocation, _to, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowLayoutPanel1);
                                                        found++;
                                                    });
                                                }
                                            
                                        }

                                    }
                                    break;

                                default:

                                    break;


                            }

                        }

                    }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }
            flowLayoutPanel1.ResumeLayout();
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
                    searchprofile = PROState.GetProfileByURN(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);


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
            createdObjects = OBJState.GetObjectCollectionsByAddress(profileCheck, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, 0, -1);




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
                            FoundCollectionControl foundObject = new FoundCollectionControl(_testnet);
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

                            if (System.IO.File.Exists(imgurn))
                            {
                                this.Invoke((Action)(() =>
                                {
                                    string thumbnailPath = imgurn + "-thumbnail.jpg";

                                    // Check if a thumbnail exists
                                    if (System.IO.File.Exists(thumbnailPath))
                                    {
                                        foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                        foundObject.ObjectImage.ImageLocation = thumbnailPath;
                                    }
                                    else
                                    {
                                        // Load the original image from file
                                        Image originalImage = Image.FromFile(imgurn);

                                        // Check if the original image is a GIF
                                        if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                                        {
                                            // For GIF images, directly use the original image without creating a thumbnail
                                            foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                            foundObject.ObjectImage.Image = originalImage;
                                        }
                                        else
                                        {
                                            // Resize the image if needed
                                            int maxWidth = foundObject.ObjectImage.Width;
                                            int maxHeight = foundObject.ObjectImage.Height;

                                            int newWidth, newHeight;
                                            if (originalImage.Width > originalImage.Height)
                                            {
                                                newWidth = maxWidth;
                                                newHeight = (int)((double)originalImage.Height / originalImage.Width * newWidth);
                                            }
                                            else
                                            {
                                                newHeight = maxHeight;
                                                newWidth = (int)((double)originalImage.Width / originalImage.Height * newHeight);
                                            }

                                            Image resizedImage = new Bitmap(originalImage, newWidth, newHeight);
                                            originalImage.Dispose();
                                            foundObject.ObjectImage.Image = resizedImage;

                                            // Save the resized image as a thumbnail
                                            resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                        }
                                    }
                                }));
                            }
                            else
                            {

                                if (objstate.Image.LastIndexOf('.') > 0 && System.IO.File.Exists("ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'))))
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

                                                if (System.IO.File.Exists(imgurn))
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

                                                if (System.IO.File.Exists(imgurn))
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

                                                if (System.IO.File.Exists(imgurn))
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

                                                if (System.IO.File.Exists(imgurn))
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

                                                Root.GetRootByTransactionId(transid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                                if (System.IO.File.Exists(imgurn))
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


            flowLayoutPanel1.Controls.Clear();


            OBJState objstate = OBJState.GetObjectByFile(filePath, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            if (objstate.Owners != null)
            {
                try
                {

                    if (objstate.Owners != null)
                    {
                        string transid = "";
                        FoundObjectControl foundObject = new FoundObjectControl(_activeProfile, _testnet);
                        foundObject.ObjectName.Text = objstate.Name;
                        foundObject.ObjectDescription.Text = objstate.Description;
                        if (!objstate.URN.ToUpper().StartsWith("IPFS"))
                        {
                            string urnmsgpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URN.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Substring(0, 64) + @"\MSG";

                            // Check if the file exists at urnmsgpath
                            if (System.IO.File.Exists(urnmsgpath))
                            {
                                // Read the text from the file
                                string fileText = System.IO.File.ReadAllText(urnmsgpath);

                                // Append the text to foundObject.ObjectDescription.Text
                                if (string.IsNullOrEmpty(foundObject.ObjectDescription.Text))
                                {
                                    foundObject.ObjectDescription.Text = fileText;
                                }
                                else
                                {
                                    foundObject.ObjectDescription.Text += Environment.NewLine + fileText;
                                }
                            }
                        }
                        foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                        foundObject.ObjectQty.Text = objstate.Owners.Values.Sum(tuple => tuple.Item1).ToString("N0") + "x";
                        foundObject.ObjectId.Text = objstate.Id.ToString();

                        //GPT3 reformed
                        if (objstate.Offers != null && objstate.Offers.Count > 0)
                        {
                            foundObject.Height = 395;
                            decimal highestValue = objstate.Offers.Max(offer => offer.Value);
                            foundObject.ObjectOffer.Text = highestValue.ToString();
                        }
                        //GPT3 reformed
                        if (objstate.Listings != null && objstate.Listings.Count > 0)
                        {
                            foundObject.Height = 395;
                            decimal lowestValue = objstate.Listings.Values.Min(listing => listing.Value);
                            foundObject.ObjectPrice.Text = lowestValue.ToString();
                        }

                        OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
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
                            // Check to see if objstate.URN has an image extension
                            string[] validImageExtensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".ico", ".tiff", ".wmf", ".emf" }; // Add more if needed

                            bool hasValidImageExtension = validImageExtensions.Any(extension =>
                                objstate.URN.EndsWith(extension, StringComparison.OrdinalIgnoreCase));

                            if (hasValidImageExtension)
                            {
                                objstate.Image = objstate.URN;
                            }
                            else
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
                        if (System.IO.File.Exists(imgurn))
                        {
                            this.Invoke((Action)(() =>
                            {
                                string thumbnailPath = imgurn + "-thumbnail.jpg";

                                // Check if a thumbnail exists
                                if (System.IO.File.Exists(thumbnailPath))
                                {
                                    foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                    foundObject.ObjectImage.ImageLocation = thumbnailPath;
                                }
                                else
                                {
                                    // Load the original image from file
                                    Image originalImage = Image.FromFile(imgurn);

                                    // Check if the original image is a GIF
                                    if (Path.GetExtension(imgurn).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // For GIF images, directly use the original image without creating a thumbnail
                                        foundObject.ObjectImage.SizeMode = PictureBoxSizeMode.Zoom;
                                        foundObject.ObjectImage.Image = originalImage;
                                    }
                                    else
                                    {
                                        // Resize the image if needed
                                        int maxWidth = foundObject.ObjectImage.Width;
                                        int maxHeight = foundObject.ObjectImage.Height;

                                        int newWidth, newHeight;
                                        if (originalImage.Width > originalImage.Height)
                                        {
                                            newWidth = maxWidth;
                                            newHeight = (int)((double)originalImage.Height / originalImage.Width * newWidth);
                                        }
                                        else
                                        {
                                            newHeight = maxHeight;
                                            newWidth = (int)((double)originalImage.Width / originalImage.Height * newHeight);
                                        }

                                        Image resizedImage = new Bitmap(originalImage, newWidth, newHeight);
                                        originalImage.Dispose();
                                        foundObject.ObjectImage.Image = resizedImage;

                                        // Save the resized image as a thumbnail
                                        resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                                    }
                                }
                            }));
                        }
                        else
                        {

                            if (System.IO.File.Exists("ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'))))
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

                                              if (System.IO.File.Exists(imgurn))
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

                                            if (System.IO.File.Exists(imgurn))
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

                                            if (System.IO.File.Exists(imgurn))
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

                                            if (System.IO.File.Exists(imgurn))
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

                                                Root.GetRootByTransactionId(transid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                                if (System.IO.File.Exists(imgurn))
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
                                    PROState profile = PROState.GetProfileByAddress(creator.Key, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                                    PROState IsRegistered = PROState.GetProfileByURN(profile.URN, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);


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

                        txtTotal.Text = "1";

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
                btnActivity.BackColor = Color.White;
            }
            txtLast.Text = "0";
            txtTotal.Text = "0";
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
                btnActivity.BackColor = Color.White;
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
                btnActivity.BackColor = Color.White;
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
                if (btnActivity.BackColor == Color.Yellow || btnCollections.BackColor == Color.Yellow) { pages.Visible = false; } else { pages.Visible = true; }
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
                if (btnActivity.BackColor == Color.Yellow || btnCollections.BackColor == Color.Yellow) { pages.Visible = false; } else { pages.Visible = true; }
                EnableSupInput();
            }


        }

        public async void BuildSearchResults(bool calculate = false)
        {
            lock (SupLocker)
            {

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



                    int loadQty = 32;// (flowLayoutPanel1.Size.Width / 213) * (flowLayoutPanel1.Size.Height / 336);
                    //loadQty -= flowLayoutPanel1.Controls.Count;


                    if (txtSearchAddress.Text.ToLower().StartsWith("http"))
                    {
                        flowLayoutPanel1.Controls.Clear();
                        flowLayoutPanel1.AutoScroll = false;

                        var webBrowser1 = new Microsoft.Web.WebView2.WinForms.WebView2();

                        // Set the initial size of the webBrowser1
                        webBrowser1.Size = new Size(flowLayoutPanel1.Width - 10, flowLayoutPanel1.Height - 40);

                        flowLayoutPanel1.SizeChanged += (sender, e) =>
                        {
                            // Adjust the size of webBrowser1 when the flowLayoutPanel's size changes
                            webBrowser1.Size = new Size(flowLayoutPanel1.Width - 10, flowLayoutPanel1.Height - 40);
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


                                GetObjectsByAddress(Root.GetPublicAddressByKeyword(txtSearchAddress.Text.Substring(1), mainnetVersionByte), calculate);
                            }

                        }
                        else
                        {

                            if (txtSearchAddress.Text.ToLower().StartsWith(@"ipfs:") && txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Length >= 51)
                            {
                                string ipfsHash = txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Substring(5, 46);

                                if (!System.IO.Directory.Exists("ipfs/" + ipfsHash))
                                {



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


                                        try
                                        {
                                            if (System.IO.File.Exists("IPFS_PINNING_ENABLED"))
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
                                        catch { }

                                        try { Directory.Delete("ipfs/" + ipfsHash + "-build", true); } catch { }
                                    }

                                    if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash))
                                    {
                                        Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash);
                                    }
                                    else { System.Windows.Forms.Label filenotFound = new System.Windows.Forms.Label(); filenotFound.AutoSize = true; filenotFound.Text = "IPFS: Search failed! Verify IPFS pinning is enbaled"; flowLayoutPanel1.Controls.Clear(); flowLayoutPanel1.Controls.Add(filenotFound); }




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
                                    GetObjectsByAddress(txtSearchAddress.Text, calculate);

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
                                                Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(0, 64), mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
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
                                                GetCollectionsByAddress(txtSearchAddress.Text.Replace("@", ""), calculate);
                                            }
                                            else
                                            {
                                                btnCollections.BackColor = System.Drawing.Color.White;
                                                GetObjectsByAddress("");
                                            }



                                        }
                                        else
                                        {

                                            if (btnActivity.BackColor == System.Drawing.Color.Yellow)
                                            {
                                                if (!string.IsNullOrEmpty(txtSearchAddress.Text))
                                                {
                                                    historySeen = 0;
                                                    GetHistoryByAddress(txtSearchAddress.Text.Replace("@", ""), calculate);

                                                }
                                                else
                                                {

                                                    btnActivity.BackColor = System.Drawing.Color.White;
                                                    GetObjectsByAddress("");
                                                }
                                            }
                                            else
                                            {


                                                GetObjectsByAddress(txtSearchAddress.Text.Replace("@", ""), calculate);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    this.Invoke((Action)(async () =>
                    {
                        flowLayoutPanel1.Visible = true;
                    }));

                }
                catch { }


            }
        }

        string TruncateAddress(string input)
        {
            if (input.Length <= 20)
            {
                return input;
            }
            else
            {
                return input.Substring(0, 7) + "..." + input.Substring(input.Length - 7);
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
            txtTotal.Text = "0";
            txtLast.Text = "0";
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
                    List<string> islocal = Root.GetPublicKeysByAddress(profileURN.Links[0].LinkData.ToString(), mainnetLogin, mainnetPassword, mainnetURL);
                    if
                         (islocal.Count == 2)
                    {
                        _activeProfile = profileURN.Links[0].LinkData.ToString();
                    }
                }
                catch { }
            }
        }

        private async void btnActivity_Click(object sender, EventArgs e)
        {
            if (btnActivity.BackColor == Color.Yellow) { btnActivity.BackColor = Color.White; }
            else
            {
                btnCreated.BackColor = Color.White;
                btnOwned.BackColor = Color.White;
                btnCollections.BackColor = Color.White;
                btnActivity.BackColor = Color.Yellow;
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
                if (btnActivity.BackColor == Color.Yellow || btnCollections.BackColor == Color.Yellow) { pages.Visible = false; } else { pages.Visible = true; }
                EnableSupInput();

            }
        }
    }



}
