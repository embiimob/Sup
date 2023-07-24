using LevelDB;
using NBitcoin.RPC;
using NBitcoin;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Globalization;
using AngleSharp.Common;
using NBitcoin.Protocol;
using Message = System.Windows.Forms.Message;
using System.Data.SqlTypes;
using AngleSharp.Text;
using AngleSharp.Html.Dom;
using System.Windows.Interop;
using Org.BouncyCastle.Utilities.Net;
using System.Threading;
using System.Drawing.Imaging;
using System.Windows.Media.TextFormatting;

namespace SUP
{
    public partial class SupMain : Form
    {
        private readonly static object SupLocker = new object();
        private List<string> BTCMemPool = new List<string>();
        private List<string> BTCTMemPool = new List<string>();
        private List<string> MZCMemPool = new List<string>();
        private List<string> LTCMemPool = new List<string>();
        private List<string> DOGMemPool = new List<string>();
        List<Microsoft.Web.WebView2.WinForms.WebView2> webviewers = new List<Microsoft.Web.WebView2.WinForms.WebView2>();

        private bool ipfsActive;
        private bool btctActive;
        private bool btcActive;
        private bool mzcActive;
        private bool ltcActive;
        private bool dogActive;
        private RichTextBox richTextBox1;
        ObjectBrowserControl OBcontrol = new ObjectBrowserControl();
        private int numMessagesDisplayed;
        private int numPrivateMessagesDisplayed;
        private int numFriendFeedsDisplayed;
        FlowLayoutPanel supPrivateFlow = new FlowLayoutPanel();

        public SupMain()
        {
            InitializeComponent();
        }

        private void SupMaincs_Load(object sender, EventArgs e)
        {
            if (Directory.Exists("root")) { lblAdultsOnly.Visible = false; }


            //remove any partialy built IPFS files
            try
            {
                string[] subfolderNames = Directory.GetDirectories("ipfs");

                foreach (string subfolder in subfolderNames)
                {
                    if (subfolder.EndsWith("-build"))
                    {
                        Directory.Delete(subfolder, true);
                    }

                }

            }
            catch { }


            OBcontrol.Dock = DockStyle.Fill;
            OBcontrol.ProfileURNChanged += OBControl_ProfileURNChanged;
            splitContainer1.Panel2.Controls.Add(OBcontrol);

            // Read the JSON data from the file
            string filePath = @"root\MyFriendList.Json";
            try
            {
                string json = File.ReadAllText(filePath);

                // Deserialize the JSON into a Dictionary<string, string> object
                Dictionary<string, string> friendDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                // Create PictureBox controls for each friend in the dictionary
                foreach (var friend in friendDict)
                {
                    // Create a new PictureBox control
                    PictureBox pictureBox = new PictureBox();

                    // Set the PictureBox properties
                    pictureBox.Tag = friend.Key;
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Width = 50;
                    pictureBox.Height = 50;
                    pictureBox.ImageLocation = friend.Value;

                    // Add event handlers to the PictureBox
                    pictureBox.Click += new EventHandler(Friend_Click);
                    pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(Friend_MouseUp);

                    // Add the PictureBox to the FlowLayoutPanel
                    flowFollow.Controls.Add(pictureBox);
                }
            }
            catch { }

        }

        private void OBControl_ProfileURNChanged(object sender, EventArgs e)
        {
            if (sender is ObjectBrowserControl objectBrowserControl)
            {
                var objectBrowserForm = objectBrowserControl.Controls[0].Controls[0] as ObjectBrowser;
                if (objectBrowserForm != null)
                {

                    profileURN.Links[0].LinkData = objectBrowserForm.profileURN.Links[0].LinkData;
                    profileURN.Text = objectBrowserForm.profileURN.Text;
                    profileURN.Enabled = true;
                    btnBlock.Enabled = true;
                    btnFollow.Enabled = true;
                    btnHome.Enabled = true;
                    btnMute.Enabled = true;
                    btnPrivateMessage.Enabled = true;
                    btnPublicMessage.Enabled = true;
                    btnDisco.Enabled = true;

                    numMessagesDisplayed = 0;
                    numPrivateMessagesDisplayed = 0;
                    numFriendFeedsDisplayed = 0;
                    supFlow.Controls.Clear();

                    if (profileURN.Text.StartsWith("#")) {
                        profileBIO.Text = "Click the follow button to add this search to your community feed."; profileCreatedDate.Text = ""; profileIMG.ImageLocation = ""; lblProcessHeight.Text = "";

                        GenerateImage(profileURN.Text);

                        profileIMG.ImageLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\keywords\" + profileURN.Text + ".png";

                        RefreshSupMessages();
                    }
                    else
                    {
                        MakeActiveProfile(objectBrowserForm.profileURN.Links[0].LinkData.ToString());

                        List<string> islocal = Root.GetPublicKeysByAddress(objectBrowserForm.profileURN.Links[0].LinkData.ToString(), "good-user", "better-password", @"http://127.0.0.1:18332");
                        if
                             (islocal.Count == 2)
                        {
                            profileOwner.ImageLocation = profileIMG.ImageLocation;
                            profileOwner.Tag = objectBrowserForm.profileURN.Links[0].LinkData.ToString();
                        }
                    }

                }
            }
        }

        private void MakeActiveProfile(string address)
        {
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            PROState activeProfile = PROState.GetProfileByAddress(address, "good-user", "better-password", @"http://127.0.0.1:18332");
            string ismuted = "";

            var WORK = new Options { CreateIfMissing = true };
            using (var db = new DB(WORK, @"root\mute"))
            {
                ismuted = db.Get(address);
            }

            if (ismuted == "true") { btnMute.Text = "unmute"; } else { btnMute.Text = "mute"; }



            if (activeProfile.URN == null) { profileURN.Text = "anon"; profileBIO.Text = ""; profileCreatedDate.Text = ""; profileIMG.ImageLocation = ""; activeProfile.Image = ""; lblProcessHeight.Text = ""; return; }

            profileBIO.Text = activeProfile.Bio;
            profileURN.Text = activeProfile.URN;
            profileURN.Links[0].LinkData = address;
            lblProcessHeight.Text = "ph: " + activeProfile.ProcessHeight.ToString();
            profileCreatedDate.Text = "since: " + activeProfile.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt");

            if (activeProfile.URL != null)
            {
                supFlow.Controls.Clear();
                foreach (string key in activeProfile.URL.Keys)
                {
                    Button button = new Button();
                    button.Text = key;
                    button.Font = new Font("Segoe UI", 12); // Set the button text font size
                    button.ForeColor = Color.White;
                    button.Height = 50;
                    button.Width = supFlow.Width - 40; // Subtract padding of 10 pixels on each side
                    button.Margin = new Padding(10, 3, 10, 4);
                    button.Click += new EventHandler((sender, e) => button_Click(sender, e, activeProfile.URL[key]));
                    supFlow.Controls.Add(button);
                }
            }



            string imgurn = "";


            if (activeProfile.Image != null)
            {
                imgurn = activeProfile.Image;


                if (!activeProfile.Image.ToLower().StartsWith("http"))
                {
                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + activeProfile.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");
                    if (activeProfile.Image.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); if (activeProfile.Image.Length == 51) { imgurn += @"\artifact"; } }
                }


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
                        profileIMG.SizeMode = PictureBoxSizeMode.StretchImage;
                        profileIMG.ImageLocation = randomGifFile;

                    }
                    else
                    {
                        try
                        {

                            profileIMG.SizeMode = PictureBoxSizeMode.StretchImage;
                            profileIMG.ImageLocation = @"includes\HugPuddle.jpg";
                        }
                        catch { }
                    }
                }));

                return;
            }


            if (File.Exists(imgurn))
            {

                this.Invoke((Action)(() =>
                {
                    profileIMG.ImageLocation = imgurn;
                }));
            }
            else
            {
                Match imgurnmatch = regexTransactionId.Match(imgurn);
                string transid = imgurnmatch.Value;

                if (activeProfile.Image.LastIndexOf('.') > 0 && File.Exists("ipfs/" + transid + "/artifact" + activeProfile.Image.Substring(activeProfile.Image.LastIndexOf('.'))))
                {
                    this.Invoke((Action)(() =>
                    {
                        profileIMG.ImageLocation = "ipfs/" + transid + "/artifact" + activeProfile.Image.Substring(activeProfile.Image.LastIndexOf('.'));
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
                            profileIMG.SizeMode = PictureBoxSizeMode.StretchImage;
                            profileIMG.ImageLocation = randomGifFile;

                        }
                        else
                        {
                            try
                            {

                                profileIMG.SizeMode = PictureBoxSizeMode.StretchImage;
                                profileIMG.ImageLocation = @"includes\HugPuddle.jpg";
                            }
                            catch { }
                        }
                    }));


                    switch (activeProfile.Image.ToUpper().Substring(0, 4))
                    {
                        case "BTC:":

                            Task.Run(() =>
                            {

                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                if (File.Exists(imgurn))
                                {

                                    this.Invoke((Action)(() =>
                                        {
                                            profileIMG.SizeMode = PictureBoxSizeMode.Zoom;
                                            profileIMG.ImageLocation = imgurn;
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
                                            profileIMG.SizeMode = PictureBoxSizeMode.Zoom;
                                            profileIMG.ImageLocation = imgurn;
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
                                            profileIMG.SizeMode = PictureBoxSizeMode.Zoom;
                                            profileIMG.ImageLocation = imgurn;
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
                                            profileIMG.SizeMode = PictureBoxSizeMode.Zoom;
                                            profileIMG.ImageLocation = imgurn;
                                        }));
                                }

                            });

                            break;
                        case "IPFS":
                            transid = "empty";
                            try { transid = activeProfile.Image.Substring(5, 46); } catch { }

                            if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                            {


                                Task.Run(() =>
                                {
                                    try { Directory.Delete("ipfs/" + transid, true); } catch { }
                                    try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                    Directory.CreateDirectory("ipfs/" + transid + "-build");
                                    Process process2 = new Process();
                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                    process2.StartInfo.Arguments = "get " + activeProfile.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                                    process2.StartInfo.UseShellExecute = false;
                                    process2.StartInfo.CreateNoWindow = true;
                                    process2.Start();
                                    process2.WaitForExit();
                                    string fileName;
                                    if (System.IO.File.Exists("ipfs/" + transid))
                                    {
                                        System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                        System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                        fileName = activeProfile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                        Directory.CreateDirectory("ipfs/" + transid);

                                        try { System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName); }
                                        catch (System.ArgumentException ex)
                                        {

                                            System.IO.File.Move("ipfs/" + transid + "_tmp", "ipfs/" + transid + "/artifact" + activeProfile.Image.Substring(activeProfile.Image.LastIndexOf('.')));
                                            imgurn = "ipfs/" + transid + "/artifact" + activeProfile.Image.Substring(activeProfile.Image.LastIndexOf('.'));

                                        }


                                    }

                                    if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                    {
                                        fileName = activeProfile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                        try { System.IO.File.Move("ipfs/" + transid + "/" + transid, @"ipfs/" + transid + @"/" + fileName); }
                                        catch (System.ArgumentException ex)
                                        {

                                            System.IO.File.Move("ipfs/" + transid + "/" + transid, "ipfs/" + transid + "/artifact" + activeProfile.Image.Substring(activeProfile.Image.LastIndexOf('.')));
                                            imgurn = "ipfs/" + transid + "/artifact" + activeProfile.Image.Substring(activeProfile.Image.LastIndexOf('.'));

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
                                        profileIMG.SizeMode = PictureBoxSizeMode.Zoom;
                                        profileIMG.ImageLocation = imgurn;
                                    }));

                                });

                            }

                            break;
                        case "HTTP":
                            Task.Run(() =>
                            {
                                profileIMG.SizeMode = PictureBoxSizeMode.Zoom;
                                profileIMG.ImageLocation = activeProfile.Image;
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
                                            profileIMG.SizeMode = PictureBoxSizeMode.Zoom;
                                            profileIMG.ImageLocation = imgurn;
                                        }));
                                }

                            });


                            break;
                    }
                }




            }



        }
        private void button_Click(object sender, EventArgs e, string value)
        {
            // Launch a new form and pass the value as the only parameter to the load function

            if (value.ToUpper().StartsWith("HTTP"))
            {
                System.Diagnostics.Process.Start(value);
            }
            else
            {

                ObjectBrowser browser = new ObjectBrowser(value);
                browser.Show();
            }
        }


        //GPT3
        private void btnMint_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel2Collapsed = false;
            }
            else
            {
                if (splitContainer1.Panel2.Controls.Contains(supPrivateFlow))
                {
                    splitContainer1.Panel2.Controls.Clear();
                    numPrivateMessagesDisplayed = 0;
                    splitContainer1.Panel2.Controls.Add(OBcontrol);
                }
                else
                {
                    // Create the form that will contain the buttons
                    CustomForm buttonForm = new CustomForm();
                    buttonForm.BackColor = Color.White;
                    buttonForm.Size = new Size(310, 200);


                    // Create the "Object Mint" button
                    Button objectMintButton = new Button();
                    objectMintButton.Text = @"Object Mint \ Update";
                    objectMintButton.Font = new Font("Arial", 16, FontStyle.Bold);
                    objectMintButton.Size = new Size(250, 50);
                    objectMintButton.Location = new Point(25, 25);
                    objectMintButton.Click += (s, ev) =>
                    {
                        // Close the button form
                        buttonForm.Close();

                        // Show the "ObjectMint" form and set focus to it
                        ObjectMint mintform = new ObjectMint();
                        mintform.StartPosition = FormStartPosition.CenterScreen;
                        mintform.Show(this);
                        mintform.Focus();
                    };
                    buttonForm.Controls.Add(objectMintButton);

                    // Create the "Mint Profile" button
                    Button mintProfileButton = new Button();
                    mintProfileButton.Text = @"Profile Mint \ Update";
                    mintProfileButton.Font = new Font("Arial", 16, FontStyle.Bold);
                    mintProfileButton.Size = new Size(250, 50);
                    mintProfileButton.Location = new Point(25, 85);
                    mintProfileButton.Click += (s, ev) =>
                    {
                        // Close the button form
                        buttonForm.Close();

                        // Show the "ProfileMint" form and set focus to it
                        ProfileMint mintprofile = new ProfileMint();
                        mintprofile.StartPosition = FormStartPosition.CenterScreen;
                        mintprofile.Show(this);
                        mintprofile.Focus();
                    };
                    buttonForm.Controls.Add(mintProfileButton);

                    // Show the button form centered on the launching program and set focus to it
                    buttonForm.StartPosition = FormStartPosition.CenterParent;
                    buttonForm.ShowDialog(this);
                    buttonForm.Focus();
                }
            }
        }

        //GPT3
        public class CustomForm : Form
        {
            protected override void OnDeactivate(EventArgs e)
            {
                // Close the form when it loses focus
                base.OnDeactivate(e);
                Close();
            }

            protected override void WndProc(ref Message m)
            {
                // Close the form when a click or key event occurs outside of its controls
                const int WM_NCLBUTTONDOWN = 0x00A1;
                const int WM_KEYDOWN = 0x0100;
                if (m.Msg == WM_NCLBUTTONDOWN || m.Msg == WM_KEYDOWN)
                {
                    Close();
                    return;
                }
                base.WndProc(ref m);
            }
        }


        private async void btnLive_Click(object sender, EventArgs e)
        {

            if (btnLive.BackColor == Color.White)
            {
                btnLive.BackColor = Color.Blue;
                btnLive.ForeColor = Color.Yellow;

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


                tmrSearchMemoryPool.Enabled = true;

            }
            else
            {
                btnLive.BackColor = Color.White;
                btnLive.ForeColor = Color.Black;
                tmrSearchMemoryPool.Enabled = false;

            }
        }


        private void AddToSearchResults(List<OBJState> objects)
        {

            foreach (OBJState objstate in objects)
            {
                try
                {
                    supFlow.Invoke((MethodInvoker)delegate
                    {
                        supFlow.SuspendLayout();
                        if (objstate.Owners != null)
                        {
                            string transid = "";
                            FoundObjectControl foundObject = new FoundObjectControl();
                            foundObject.SuspendLayout();
                            if (objstate.Image != null)
                            {
                                try { transid = objstate.Image.Substring(4, 64).Replace(":", ""); } catch { try { transid = objstate.Image.Substring(5, 46); } catch { } }
                                try { foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", ""); } catch { }
                            }
                            foundObject.ObjectName.Text = objstate.Name;
                            foundObject.ObjectDescription.Text = objstate.Description;
                            foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                            foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                            foundObject.ObjectId.Text = objstate.TransactionId;
                            try
                            {
                                if (objstate.Image != null)
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
                            }
                            catch { }

                            foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
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

                            foundObject.ResumeLayout();

                            supFlow.Controls.Add(foundObject);
                            supFlow.Controls.SetChildIndex(foundObject, 0);

                        }
                        supFlow.ResumeLayout();
                    });
                }
                catch { }
            }


        }
        private void ButtonLoadWorkBench(object sender, EventArgs e)
        {
            new WorkBench().Show();
        }

        private void ButtonLoadConnections(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel2Collapsed = false;
            }
            else
            {
                new Connections().Show();
            }


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
                        string myFriendsJson = "";
                        Dictionary<string, string> myFriends = new Dictionary<string, string>();

                        if (File.Exists(@"root\MyFriendList.Json"))
                        {
                            myFriendsJson = File.ReadAllText(@"root\MyFriendList.Json");
                            myFriends = JsonConvert.DeserializeObject<Dictionary<string, string>>(myFriendsJson);
                        }
                        string filter = "";

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

                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.First(); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = string.Join(" ", root.Message);
                                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateFeedRow(imglocation, _to, _to, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", Color.White, supFlow, true);
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateFeedRow(imglocation, _from, _from, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, supFlow, true);


                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, out int r) && !content.Trim().StartsWith("#"))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff" };

                                                                if (!imgExtensions.Contains(extension))
                                                                {


                                                                    try
                                                                    {
                                                                        // Create a WebClient object to fetch the webpage
                                                                        WebClient client = new WebClient();
                                                                        string html = client.DownloadString(content.StripLeadingTrailingSpaces());

                                                                        // Use regular expressions to extract the metadata from the HTML
                                                                        string title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                                                        string description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                                                        string imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                                                        if (description != "")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 30, 100);

                                                                                // Create a label for the title
                                                                                Label titleLabel = new Label();
                                                                                titleLabel.Text = title;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                                titleLabel.ForeColor = Color.White;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(titleLabel);

                                                                                // Create a label for the description
                                                                                Label descriptionLabel = new Label();
                                                                                descriptionLabel.Text = description;
                                                                                descriptionLabel.ForeColor = Color.White;
                                                                                descriptionLabel.Dock = DockStyle.Fill;
                                                                                descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                                                                descriptionLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(descriptionLabel);

                                                                                // Add an image to the panel if one is defined
                                                                                if (!String.IsNullOrEmpty(imageUrl))
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        // Create a MemoryStream object from the image data
                                                                                        byte[] imageData = client.DownloadData(imageUrl);
                                                                                        MemoryStream memoryStream = new MemoryStream(imageData);

                                                                                        // Create a new PictureBox control and add it to the panel
                                                                                        PictureBox pictureBox = new PictureBox();
                                                                                        pictureBox.Dock = DockStyle.Left;
                                                                                        pictureBox.Size = new Size(100, 100);
                                                                                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                                        pictureBox.Image = Image.FromStream(memoryStream);
                                                                                        pictureBox.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                        panel.Controls.Add(pictureBox);
                                                                                    }
                                                                                    catch
                                                                                    {
                                                                                    }
                                                                                }


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                        else
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {  // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 20, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                                panel.Controls.Add(titleLabel);


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                    }
                                                                    catch
                                                                    {

                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {  // Create a new panel to display the metadata
                                                                            Panel panel = new Panel();
                                                                            panel.BorderStyle = BorderStyle.FixedSingle;
                                                                            panel.Size = new Size(supFlow.Width - 20, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                            titleLabel.Padding = new Padding(5);
                                                                            titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                            panel.Controls.Add(titleLabel);


                                                                            this.supFlow.Controls.Add(panel);
                                                                            supFlow.Controls.SetChildIndex(panel, 0);
                                                                        });



                                                                    }
                                                                }
                                                                else
                                                                {


                                                                    if (!int.TryParse(content, out int id))
                                                                    {
                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {
                                                                            AddImage(content, false, true);
                                                                        });
                                                                    }

                                                                }
                                                            }
                                                        }

                                                        TableLayoutPanel padding = new TableLayoutPanel
                                                        {
                                                            RowCount = 1,
                                                            ColumnCount = 1,
                                                            Dock = DockStyle.Top,
                                                            BackColor = Color.Black,
                                                            ForeColor = Color.White,
                                                            AutoSize = true,
                                                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                                                            Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                                                            Padding = new System.Windows.Forms.Padding(0)

                                                        };

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 20));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            supFlow.Controls.Add(padding);
                                                        });


                                                    }


                                                    isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:18332");
                                                    if (isobject.URN != null && find == true)
                                                    {
                                                        isobject.TransactionId = s;
                                                        foundobjects.Add(isobject);
                                                        try { Directory.Delete(@"root\" + s, true); } catch { }

                                                        using (var db = new DB(SUP, @"root\found"))
                                                        {
                                                            db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                        }




                                                    }
                                                    try { System.IO.Directory.Delete(@"root\" + s, true); } catch { }

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

                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.GetItemByIndex(root.Keyword.Count() - 2); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = string.Join(" ", root.Message);
                                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateFeedRow(imglocation, _to, _to, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", Color.White, supFlow, true);
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateFeedRow(imglocation, _from, _from, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, supFlow, true);
                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, out int r) && !content.Trim().StartsWith("#"))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff" };

                                                                if (!imgExtensions.Contains(extension))
                                                                {


                                                                    try
                                                                    {
                                                                        // Create a WebClient object to fetch the webpage
                                                                        WebClient client = new WebClient();
                                                                        string html = client.DownloadString(content.StripLeadingTrailingSpaces());

                                                                        // Use regular expressions to extract the metadata from the HTML
                                                                        string title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                                                        string description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                                                        string imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                                                        if (description != "")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 30, 100);

                                                                                // Create a label for the title
                                                                                Label titleLabel = new Label();
                                                                                titleLabel.Text = title;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                                titleLabel.ForeColor = Color.White;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(titleLabel);

                                                                                // Create a label for the description
                                                                                Label descriptionLabel = new Label();
                                                                                descriptionLabel.Text = description;
                                                                                descriptionLabel.ForeColor = Color.White;
                                                                                descriptionLabel.Dock = DockStyle.Fill;
                                                                                descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                                                                descriptionLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(descriptionLabel);

                                                                                // Add an image to the panel if one is defined
                                                                                if (!String.IsNullOrEmpty(imageUrl))
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        // Create a MemoryStream object from the image data
                                                                                        byte[] imageData = client.DownloadData(imageUrl);
                                                                                        MemoryStream memoryStream = new MemoryStream(imageData);

                                                                                        // Create a new PictureBox control and add it to the panel
                                                                                        PictureBox pictureBox = new PictureBox();
                                                                                        pictureBox.Dock = DockStyle.Left;
                                                                                        pictureBox.Size = new Size(100, 100);
                                                                                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                                        pictureBox.Image = Image.FromStream(memoryStream);
                                                                                        pictureBox.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                        panel.Controls.Add(pictureBox);
                                                                                    }
                                                                                    catch
                                                                                    {
                                                                                    }
                                                                                }


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                        else
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {  // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 20, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                                panel.Controls.Add(titleLabel);


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                    }
                                                                    catch
                                                                    {

                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {  // Create a new panel to display the metadata
                                                                            Panel panel = new Panel();
                                                                            panel.BorderStyle = BorderStyle.FixedSingle;
                                                                            panel.Size = new Size(supFlow.Width - 20, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                            titleLabel.Padding = new Padding(5);
                                                                            titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                            panel.Controls.Add(titleLabel);


                                                                            this.supFlow.Controls.Add(panel);
                                                                            supFlow.Controls.SetChildIndex(panel, 0);
                                                                        });



                                                                    }
                                                                }
                                                                else
                                                                {


                                                                    if (!int.TryParse(content, out int id))
                                                                    {
                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {
                                                                            AddImage(content, false, true);
                                                                        });
                                                                    }

                                                                }
                                                            }
                                                        }

                                                        TableLayoutPanel padding = new TableLayoutPanel
                                                        {
                                                            RowCount = 1,
                                                            ColumnCount = 1,
                                                            Dock = DockStyle.Top,
                                                            BackColor = Color.Black,
                                                            ForeColor = Color.White,
                                                            AutoSize = true,
                                                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                                                            Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                                                            Padding = new System.Windows.Forms.Padding(0)

                                                        };

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 20));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            supFlow.Controls.Add(padding);
                                                        });


                                                    }


                                                    isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                                    if (isobject.URN != null && find == true)
                                                    {
                                                        isobject.TransactionId = s;
                                                        foundobjects.Add(isobject);
                                                        try { Directory.Delete(@"root\" + s, true); } catch { }


                                                        using (var db = new DB(SUP, @"root\found"))
                                                        {
                                                            db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                        }


                                                    }
                                                    try { System.IO.Directory.Delete(@"root\" + s, true); } catch { }

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
                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.GetItemByIndex(root.Keyword.Count() - 2); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = string.Join(" ", root.Message);
                                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateFeedRow(imglocation, _to, _to, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", Color.White, supFlow, true);
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateFeedRow(imglocation, _from, _from, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, supFlow, true);
                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, out int r) && !content.Trim().StartsWith("#"))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff" };

                                                                if (!imgExtensions.Contains(extension))
                                                                {


                                                                    try
                                                                    {
                                                                        // Create a WebClient object to fetch the webpage
                                                                        WebClient client = new WebClient();
                                                                        string html = client.DownloadString(content.StripLeadingTrailingSpaces());

                                                                        // Use regular expressions to extract the metadata from the HTML
                                                                        string title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                                                        string description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                                                        string imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                                                        if (description != "")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 30, 100);

                                                                                // Create a label for the title
                                                                                Label titleLabel = new Label();
                                                                                titleLabel.Text = title;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                                titleLabel.ForeColor = Color.White;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(titleLabel);

                                                                                // Create a label for the description
                                                                                Label descriptionLabel = new Label();
                                                                                descriptionLabel.Text = description;
                                                                                descriptionLabel.ForeColor = Color.White;
                                                                                descriptionLabel.Dock = DockStyle.Fill;
                                                                                descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                                                                descriptionLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(descriptionLabel);

                                                                                // Add an image to the panel if one is defined
                                                                                if (!String.IsNullOrEmpty(imageUrl))
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        // Create a MemoryStream object from the image data
                                                                                        byte[] imageData = client.DownloadData(imageUrl);
                                                                                        MemoryStream memoryStream = new MemoryStream(imageData);

                                                                                        // Create a new PictureBox control and add it to the panel
                                                                                        PictureBox pictureBox = new PictureBox();
                                                                                        pictureBox.Dock = DockStyle.Left;
                                                                                        pictureBox.Size = new Size(100, 100);
                                                                                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                                        pictureBox.Image = Image.FromStream(memoryStream);
                                                                                        pictureBox.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                        panel.Controls.Add(pictureBox);
                                                                                    }
                                                                                    catch
                                                                                    {
                                                                                    }
                                                                                }


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                        else
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {  // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 20, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                                panel.Controls.Add(titleLabel);


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                    }
                                                                    catch
                                                                    {

                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {  // Create a new panel to display the metadata
                                                                            Panel panel = new Panel();
                                                                            panel.BorderStyle = BorderStyle.FixedSingle;
                                                                            panel.Size = new Size(supFlow.Width - 20, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                            titleLabel.Padding = new Padding(5);
                                                                            titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                            panel.Controls.Add(titleLabel);


                                                                            this.supFlow.Controls.Add(panel);
                                                                            supFlow.Controls.SetChildIndex(panel, 0);
                                                                        });



                                                                    }
                                                                }
                                                                else
                                                                {


                                                                    if (!int.TryParse(content, out int id))
                                                                    {
                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {
                                                                            AddImage(content, false, true);
                                                                        });
                                                                    }

                                                                }
                                                            }
                                                        }

                                                        TableLayoutPanel padding = new TableLayoutPanel
                                                        {
                                                            RowCount = 1,
                                                            ColumnCount = 1,
                                                            Dock = DockStyle.Top,
                                                            BackColor = Color.Black,
                                                            ForeColor = Color.White,
                                                            AutoSize = true,
                                                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                                                            Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                                                            Padding = new System.Windows.Forms.Padding(0)

                                                        };

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 20));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            supFlow.Controls.Add(padding);
                                                        });


                                                    }

                                                    isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                                    if (isobject.URN != null && find == true)
                                                    {

                                                        isobject.TransactionId = s;
                                                        foundobjects.Add(isobject);
                                                        try { Directory.Delete(@"root\" + s, true); } catch { }
                                                        using (var db = new DB(SUP, @"root\found"))
                                                        {
                                                            db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                        }



                                                    }
                                                    try { System.IO.Directory.Delete(@"root\" + s, true); } catch { }

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
                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.GetItemByIndex(root.Keyword.Count() - 2); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = string.Join(" ", root.Message);
                                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateFeedRow(imglocation, _to, _to, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", Color.White, supFlow, true);
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateFeedRow(imglocation, _from, _from, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, supFlow, true);
                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, out int r) && !content.Trim().StartsWith("#"))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff" };

                                                                if (!imgExtensions.Contains(extension))
                                                                {


                                                                    try
                                                                    {
                                                                        // Create a WebClient object to fetch the webpage
                                                                        WebClient client = new WebClient();
                                                                        string html = client.DownloadString(content.StripLeadingTrailingSpaces());

                                                                        // Use regular expressions to extract the metadata from the HTML
                                                                        string title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                                                        string description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                                                        string imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                                                        if (description != "")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 30, 100);

                                                                                // Create a label for the title
                                                                                Label titleLabel = new Label();
                                                                                titleLabel.Text = title;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                                titleLabel.ForeColor = Color.White;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(titleLabel);

                                                                                // Create a label for the description
                                                                                Label descriptionLabel = new Label();
                                                                                descriptionLabel.Text = description;
                                                                                descriptionLabel.ForeColor = Color.White;
                                                                                descriptionLabel.Dock = DockStyle.Fill;
                                                                                descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                                                                descriptionLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(descriptionLabel);

                                                                                // Add an image to the panel if one is defined
                                                                                if (!String.IsNullOrEmpty(imageUrl))
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        // Create a MemoryStream object from the image data
                                                                                        byte[] imageData = client.DownloadData(imageUrl);
                                                                                        MemoryStream memoryStream = new MemoryStream(imageData);

                                                                                        // Create a new PictureBox control and add it to the panel
                                                                                        PictureBox pictureBox = new PictureBox();
                                                                                        pictureBox.Dock = DockStyle.Left;
                                                                                        pictureBox.Size = new Size(100, 100);
                                                                                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                                        pictureBox.Image = Image.FromStream(memoryStream);
                                                                                        pictureBox.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                        panel.Controls.Add(pictureBox);
                                                                                    }
                                                                                    catch
                                                                                    {
                                                                                    }
                                                                                }


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                        else
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {  // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 20, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                                panel.Controls.Add(titleLabel);


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                    }
                                                                    catch
                                                                    {

                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {  // Create a new panel to display the metadata
                                                                            Panel panel = new Panel();
                                                                            panel.BorderStyle = BorderStyle.FixedSingle;
                                                                            panel.Size = new Size(supFlow.Width - 20, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                            titleLabel.Padding = new Padding(5);
                                                                            titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                            panel.Controls.Add(titleLabel);


                                                                            this.supFlow.Controls.Add(panel);
                                                                            supFlow.Controls.SetChildIndex(panel, 0);
                                                                        });



                                                                    }
                                                                }
                                                                else
                                                                {


                                                                    if (!int.TryParse(content, out int id))
                                                                    {
                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {
                                                                            AddImage(content, false, true);
                                                                        });
                                                                    }

                                                                }
                                                            }
                                                        }

                                                        TableLayoutPanel padding = new TableLayoutPanel
                                                        {
                                                            RowCount = 1,
                                                            ColumnCount = 1,
                                                            Dock = DockStyle.Top,
                                                            BackColor = Color.Black,
                                                            ForeColor = Color.White,
                                                            AutoSize = true,
                                                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                                                            Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                                                            Padding = new System.Windows.Forms.Padding(0)

                                                        };

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 20));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            supFlow.Controls.Add(padding);
                                                        });


                                                    }

                                                    isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                                    if (isobject.URN != null && find == true)
                                                    {
                                                        isobject.TransactionId = s;
                                                        foundobjects.Add(isobject);
                                                        try { Directory.Delete(@"root\" + s, true); } catch { }

                                                        using (var db = new DB(SUP, @"root\found"))
                                                        {
                                                            db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                        }


                                                    }
                                                    try { System.IO.Directory.Delete(@"root\" + s, true); } catch { }

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
                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.GetItemByIndex(root.Keyword.Count() - 2); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = string.Join(" ", root.Message);
                                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateFeedRow(imglocation, _to, _to, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", Color.White, supFlow, true);
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateFeedRow(imglocation, _from, _from, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, supFlow, true);

                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, out int r) && !content.Trim().StartsWith("#"))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff" };

                                                                if (!imgExtensions.Contains(extension))
                                                                {


                                                                    try
                                                                    {
                                                                        // Create a WebClient object to fetch the webpage
                                                                        WebClient client = new WebClient();
                                                                        string html = client.DownloadString(content.StripLeadingTrailingSpaces());

                                                                        // Use regular expressions to extract the metadata from the HTML
                                                                        string title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                                                        string description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                                                        string imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                                                        if (description != "")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 30, 100);

                                                                                // Create a label for the title
                                                                                Label titleLabel = new Label();
                                                                                titleLabel.Text = title;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                                titleLabel.ForeColor = Color.White;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(titleLabel);

                                                                                // Create a label for the description
                                                                                Label descriptionLabel = new Label();
                                                                                descriptionLabel.Text = description;
                                                                                descriptionLabel.ForeColor = Color.White;
                                                                                descriptionLabel.Dock = DockStyle.Fill;
                                                                                descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                                                                descriptionLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(descriptionLabel);

                                                                                // Add an image to the panel if one is defined
                                                                                if (!String.IsNullOrEmpty(imageUrl))
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        // Create a MemoryStream object from the image data
                                                                                        byte[] imageData = client.DownloadData(imageUrl);
                                                                                        MemoryStream memoryStream = new MemoryStream(imageData);

                                                                                        // Create a new PictureBox control and add it to the panel
                                                                                        PictureBox pictureBox = new PictureBox();
                                                                                        pictureBox.Dock = DockStyle.Left;
                                                                                        pictureBox.Size = new Size(100, 100);
                                                                                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                                        pictureBox.Image = Image.FromStream(memoryStream);
                                                                                        pictureBox.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                        panel.Controls.Add(pictureBox);
                                                                                    }
                                                                                    catch
                                                                                    {
                                                                                    }
                                                                                }


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                        else
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {  // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(supFlow.Width - 20, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                                panel.Controls.Add(titleLabel);


                                                                                this.supFlow.Controls.Add(panel);
                                                                                supFlow.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                    }
                                                                    catch
                                                                    {

                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {  // Create a new panel to display the metadata
                                                                            Panel panel = new Panel();
                                                                            panel.BorderStyle = BorderStyle.FixedSingle;
                                                                            panel.Size = new Size(supFlow.Width - 20, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 120, 30);
                                                                            titleLabel.Padding = new Padding(5);
                                                                            titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                            panel.Controls.Add(titleLabel);


                                                                            this.supFlow.Controls.Add(panel);
                                                                            supFlow.Controls.SetChildIndex(panel, 0);
                                                                        });



                                                                    }
                                                                }
                                                                else
                                                                {


                                                                    if (!int.TryParse(content, out int id))
                                                                    {
                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {
                                                                            AddImage(content, false, true);
                                                                        });
                                                                    }

                                                                }
                                                            }
                                                        }

                                                        TableLayoutPanel padding = new TableLayoutPanel
                                                        {
                                                            RowCount = 1,
                                                            ColumnCount = 1,
                                                            Dock = DockStyle.Top,
                                                            BackColor = Color.Black,
                                                            ForeColor = Color.White,
                                                            AutoSize = true,
                                                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                                                            Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                                                            Padding = new System.Windows.Forms.Padding(0)

                                                        };

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 20));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            supFlow.Controls.Add(padding);
                                                        });


                                                    }

                                                    isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                                    if (isobject.URN != null && find == true)
                                                    {
                                                        isobject.TransactionId = s;
                                                        foundobjects.Add(isobject);
                                                        try { Directory.Delete(@"root\" + s, true); } catch { }

                                                        using (var db = new DB(SUP, @"root\found"))
                                                        {
                                                            db.Put("found!" + root.BlockDate.ToString("yyyyMMddHHmmss") + "!" + root.SignedBy, "1");
                                                        }



                                                    }
                                                    try { System.IO.Directory.Delete(@"root\" + s, true); } catch { }

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

        private void RefreshSupMessages()
        {
            // Clear controls if no messages have been displayed yet
            if (numMessagesDisplayed == 0)
            {

                foreach (var viewer in webviewers)
                {
                    viewer.Dispose();
                }
                supFlow.Controls.Clear();
            }
            if (profileURN.Links[0].LinkData != null)
            {

                Task BuildMessage = Task.Run(() =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        btnPublicMessage.Enabled = false;
                    });

                Dictionary<string, string[]> profileAddress = new Dictionary<string, string[]> { };
                OBJState objstate = OBJState.GetObjectByAddress(profileURN.Links[0].LinkData.ToString(), "good-user", "better-password", "http://127.0.0.1:18332");
                int rownum = 1;

                var SUP = new Options { CreateIfMissing = true };
                    try
                    {
                        using (var db = new DB(SUP, @"root\" + profileURN.Links[0].LinkData.ToString() + @"\sup"))
                        {

                            LevelDB.Iterator it = db.CreateIterator();
                            for (
                               it.SeekToLast();
                               it.IsValid() && rownum <= numMessagesDisplayed + 10; // Only display next 10 messages
                                it.Prev()
                             )
                            {
                                // Display only if rownum > numMessagesDisplayed to skip already displayed messages
                                if (rownum > numMessagesDisplayed)
                                {
                                    string process = it.ValueAsString();

                                    List<string> supMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);

                                    string message = "";
                                    try
                                    {
                                        message = System.IO.File.ReadAllText(@"root/" + supMessagePacket[1] + @"/MSG").Replace("@" + profileURN.Links[0].LinkData.ToString(), "");

                                        string relativeFolderPath = @"root\" + supMessagePacket[1];
                                        string folderPath = Path.Combine(Environment.CurrentDirectory, relativeFolderPath);

                                        string[] files = Directory.GetFiles(folderPath);

                                        foreach (string file in files)
                                        {
                                            string extension = Path.GetExtension(file);

                                            if (!string.IsNullOrEmpty(extension) && !file.Contains("ROOT.json"))
                                            {
                                                message = message + @"<<" + supMessagePacket[1] + @"/" + Path.GetFileName(file) + ">>";
                                            }
                                        }

                                        string fromAddress = supMessagePacket[0];
                                        string imagelocation = "";


                                        if (!profileAddress.ContainsKey(fromAddress))
                                        {

                                            PROState profile = PROState.GetProfileByAddress(fromAddress, "good-user", "better-password", "http://127.0.0.1:18332");

                                            if (profile.URN != null)
                                            {
                                                fromAddress = TruncateAddress(profile.URN);

                                                if (profile.Image != null)
                                                {
                                                    imagelocation = profile.Image;


                                                    if (!profile.Image.ToLower().StartsWith("http"))
                                                    {
                                                        imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + profile.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                                                        if (profile.Image.ToLower().StartsWith("ipfs:")) { imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\"); if (profile.Image.Length == 51) { imagelocation += @"\artifact"; } }
                                                    }
                                                    Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                                                    Match imgurnmatch = regexTransactionId.Match(imagelocation);
                                                    string transactionid = imgurnmatch.Value;
                                                    Root root = new Root();
                                                    if (!File.Exists(imagelocation))
                                                    {
                                                        switch (profile.Image.ToUpper().Substring(0, 4))
                                                        {
                                                            case "MZC:":
                                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                                                break;
                                                            case "BTC:":

                                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                                                break;
                                                            case "LTC:":

                                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");


                                                                break;
                                                            case "DOG:":
                                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                                                break;
                                                            case "IPFS":
                                                                string transid = "empty";
                                                                try { transid = profile.Image.Substring(5, 46); } catch { }

                                                                if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                                                {
                                                                    try { Directory.Delete("ipfs/" + transid, true); } catch { }
                                                                    try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                                                    Directory.CreateDirectory("ipfs/" + transid + "-build");
                                                                    Process process2 = new Process();
                                                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                                    process2.StartInfo.Arguments = "get " + profile.Image.Substring(5, 46) + @" -o ipfs\" + transid;
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
                                                                        System.IO.File.Move("ipfs/" + transid + "_tmp", imagelocation);
                                                                    }

                                                                    if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                                    {
                                                                        fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                                        System.IO.File.Move("ipfs/" + transid + "/" + transid, imagelocation);
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

                                                                break;
                                                            default:
                                                                if (!profile.Image.ToUpper().StartsWith("HTTP") && transactionid != "")
                                                                {
                                                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");

                                                                }
                                                                break;
                                                        }
                                                    }





                                                }




                                            }
                                            else
                                            { fromAddress = TruncateAddress(fromAddress); }

                                            string[] profilePacket = new string[2];

                                            profilePacket[0] = fromAddress;
                                            profilePacket[1] = imagelocation;
                                            profileAddress.Add(supMessagePacket[0], profilePacket);

                                        }
                                        else
                                        {
                                            string[] profilePacket = new string[] { };
                                            profileAddress.TryGetValue(fromAddress, out profilePacket);
                                            fromAddress = profilePacket[0];
                                            imagelocation = profilePacket[1];

                                        }


                                        string tstamp = it.KeyAsString().Split('!')[1];
                                        System.Drawing.Color bgcolor = System.Drawing.Color.White;
                                        string unfilteredmessage = message;
                                        message = Regex.Replace(message, "<<.*?>>", "");

                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            CreateRow(imagelocation, fromAddress, supMessagePacket[0], DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, supMessagePacket[1], false, supFlow);
                                        });

                                        string pattern = "<<.*?>>";
                                        List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4",".avi" };

                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                        foreach (Match match in matches)
                                        {


                                            string content = match.Value.Substring(2, match.Value.Length - 4);

                                            if (!int.TryParse(content, out int cnt) && !content.Trim().StartsWith("#"))
                                            {



                                                string imgurn = content;

                                                if (!content.ToLower().StartsWith("http"))
                                                {
                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                }

                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                if (!imgExtensions.Contains(extension) && !imgurn.Contains("youtube.com") && !imgurn.Contains("youtu.be"))
                                                {


                                                    try
                                                    {
                                                        // Create a WebClient object to fetch the webpage
                                                        WebClient client = new WebClient();
                                                        string html = client.DownloadString(content.StripLeadingTrailingSpaces());

                                                        // Use regular expressions to extract the metadata from the HTML
                                                        string title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                                        string description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                                        string imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                                        if (description != "")
                                                        {
                                                            // Create a new panel to display the metadata
                                                            Panel panel = new Panel();
                                                            panel.BorderStyle = BorderStyle.FixedSingle;
                                                            panel.Size = new Size(supFlow.Width - 30, 100);

                                                            // Create a label for the title
                                                            Label titleLabel = new Label();
                                                            titleLabel.Text = title;
                                                            titleLabel.Dock = DockStyle.Top;
                                                            titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                            titleLabel.ForeColor = Color.White;
                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 130, 30);
                                                            titleLabel.Padding = new Padding(5);
                                                            titleLabel.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                                            panel.Controls.Add(titleLabel);

                                                            // Create a label for the description
                                                            Label descriptionLabel = new Label();
                                                            descriptionLabel.Text = description;
                                                            descriptionLabel.ForeColor = Color.White;
                                                            descriptionLabel.Dock = DockStyle.Fill;
                                                            descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                                            descriptionLabel.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                                            panel.Controls.Add(descriptionLabel);

                                                            // Add an image to the panel if one is defined
                                                            if (!String.IsNullOrEmpty(imageUrl))
                                                            {
                                                                try
                                                                {
                                                                    // Create a MemoryStream object from the image data
                                                                    byte[] imageData = client.DownloadData(imageUrl);
                                                                    MemoryStream memoryStream = new MemoryStream(imageData);

                                                                    // Create a new PictureBox control and add it to the panel
                                                                    PictureBox pictureBox = new PictureBox();
                                                                    pictureBox.Dock = DockStyle.Left;
                                                                    pictureBox.Size = new Size(100, 100);
                                                                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                    pictureBox.Image = Image.FromStream(memoryStream);
                                                                    pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                                                    panel.Controls.Add(pictureBox);
                                                                }
                                                                catch
                                                                {
                                                                }
                                                            }

                                                            // Add the panel to the flow layout panel
                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                this.supFlow.Controls.Add(panel);
                                                            });
                                                        }
                                                        else
                                                        {
                                                            // Create a new panel to display the metadata
                                                            Panel panel = new Panel();
                                                            panel.BorderStyle = BorderStyle.FixedSingle;
                                                            panel.Size = new Size(supFlow.Width - 20, 30);

                                                            // Create a label for the title
                                                            LinkLabel titleLabel = new LinkLabel();
                                                            titleLabel.Text = content;
                                                            titleLabel.Links[0].LinkData = content;
                                                            titleLabel.Dock = DockStyle.Top;
                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 130, 30);
                                                            titleLabel.Padding = new Padding(5);
                                                            titleLabel.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                                            panel.Controls.Add(titleLabel);

                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                this.supFlow.Controls.Add(panel);
                                                            });

                                                        }
                                                    }
                                                    catch
                                                    {

                                                        // Create a new panel to display the metadata
                                                        Panel panel = new Panel();
                                                        panel.BorderStyle = BorderStyle.FixedSingle;
                                                        panel.Size = new Size(supFlow.Width - 30, 30);

                                                        // Create a label for the title
                                                        LinkLabel titleLabel = new LinkLabel();
                                                        titleLabel.Text = content;
                                                        titleLabel.Links[0].LinkData = content;
                                                        titleLabel.Dock = DockStyle.Top;
                                                        titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                        titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                        titleLabel.MinimumSize = new Size(supFlow.Width - 130, 30);
                                                        titleLabel.Padding = new Padding(5);
                                                        titleLabel.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                                        panel.Controls.Add(titleLabel);
                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            this.supFlow.Controls.Add(panel);
                                                        });


                                                    }
                                                }
                                                else
                                                {


                                                    if (!int.TryParse(content, out int id))
                                                    {

                                                        if (extension == ".mp4" || extension == ".avi" || content.Contains("youtube.com") || content.Contains("youtu.be"))
                                                        {

                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                AddVideo(content);
                                                            });

                                                        }
                                                        else
                                                        {

                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                AddImage(content);
                                                            });
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        TableLayoutPanel padding = new TableLayoutPanel
                                        {
                                            RowCount = 1,
                                            ColumnCount = 1,
                                            Dock = DockStyle.Top,
                                            BackColor = Color.Black,
                                            ForeColor = Color.White,
                                            AutoSize = true,
                                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                                            Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                                            Padding = new System.Windows.Forms.Padding(0)

                                        };

                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 20));
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            supFlow.Controls.Add(padding);
                                        });

                                    }
                                    catch { }//deleted file

                                }
                                rownum++;



                            }
                            it.Dispose();
                        }

                        // Update number of messages displayed
                        numMessagesDisplayed += 10;

                        supFlow.ResumeLayout();
                    }
                    catch { }

                    this.Invoke((MethodInvoker)delegate
                    {
                        btnPublicMessage.Enabled = true;
                    });
                });
            }
        }

        private void RefreshPrivateSupMessages()
        {

            // Clear controls if no messages have been displayed yet
            if (numPrivateMessagesDisplayed == 0)
            {
                splitContainer1.Panel2.Controls.Clear();


                foreach (var viewer in webviewers)
                {
                    viewer.Dispose();
                }
                supPrivateFlow.Controls.Clear();
                supPrivateFlow.Dock = DockStyle.Fill;
                supPrivateFlow.AutoScroll = true;
                splitContainer1.Panel2.Controls.Add(supPrivateFlow);
            }
            Task BuildMessage = Task.Run(() =>
            {

                this.Invoke((MethodInvoker)delegate
                {
                    btnPrivateMessage.Enabled = false;
                });

                Dictionary<string, string[]> profileAddress = new Dictionary<string, string[]> { };
                OBJState objstate = OBJState.GetObjectByAddress(profileURN.Links[0].LinkData.ToString(), "good-user", "better-password", "http://127.0.0.1:18332");
                int rownum = 1;

                var SUP = new Options { CreateIfMissing = true };
                try
                {
                    using (var db = new DB(SUP, @"root\" + profileURN.Links[0].LinkData.ToString() + @"\sec"))
                    {

                        LevelDB.Iterator it = db.CreateIterator();
                        for (
                           it.SeekToLast();
                           it.IsValid() && rownum <= numPrivateMessagesDisplayed + 10; // Only display next 10 messages
                            it.Prev()
                         )
                        {
                            if (rownum > numPrivateMessagesDisplayed)
                            {
                                string process = it.ValueAsString();

                                List<string> supMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);
                                Root root = Root.GetRootByTransactionId(supMessagePacket[1], "good-user", "better-password", "http://127.0.0.1:18332");
                                byte[] result = Root.GetRootBytesByFile(new string[] { @"root/" + supMessagePacket[1] + @"/SEC" });
                                result = Root.DecryptRootBytes("good-user", "better-password", "http://127.0.0.1:18332", profileURN.Links[0].LinkData.ToString(), result);
                                root = Root.GetRootByTransactionId(supMessagePacket[1], "good-user", "better-password", "http://127.0.0.1:18332", "111", result, supMessagePacket[0]);
                                if (root.Signed)
                                {
                                    string message = "";
                                    try { message = string.Join(" ", root.Message); } catch { };

                                    string fromAddress = supMessagePacket[0];
                                    string imagelocation = "";


                                    if (!profileAddress.ContainsKey(fromAddress))
                                    {

                                        PROState profile = PROState.GetProfileByAddress(fromAddress, "good-user", "better-password", "http://127.0.0.1:18332");

                                        if (profile.URN != null)
                                        {
                                            fromAddress = TruncateAddress(profile.URN);
                                            imagelocation = profile.Image;


                                            if (imagelocation.StartsWith("BTC:") || imagelocation.StartsWith("MZC:"))
                                            {
                                                if (imagelocation.Length > 64)
                                                {
                                                    string transid = imagelocation.Substring(4, 64);
                                                    imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagelocation.Replace("BTC:", "").Replace("MZC:", "").Replace(@"/", @"\");


                                                    if (!System.IO.Directory.Exists("root/" + transid))
                                                    {
                                                        if (profile.Image.StartsWith("BTC:"))
                                                        {
                                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                                                        }
                                                        else
                                                        {
                                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                                        }
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                if (imagelocation.Length > 64)
                                                {
                                                    string transid = imagelocation.Substring(0, 64);
                                                    imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagelocation.Replace(@" / ", @"\");
                                                    if (!System.IO.Directory.Exists("root/" + transid))
                                                    {
                                                        Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332");

                                                    }
                                                }


                                                if (imagelocation.StartsWith("IPFS:"))
                                                {

                                                    string transid = imagelocation.Substring(5, 46);
                                                    if (!System.IO.Directory.Exists("ipfs/" + transid))
                                                    {

                                                        string isLoading;
                                                        using (var db2 = new DB(SUP, @"ipfs"))
                                                        {
                                                            isLoading = db2.Get(transid);

                                                        }

                                                        if (isLoading != "loading")
                                                        {
                                                            using (var db2 = new DB(SUP, @"ipfs"))
                                                            {

                                                                db2.Put(transid, "loading");

                                                            }

                                                            Task ipfsTask = Task.Run(() =>
                                                            {
                                                                Process process2 = new Process();
                                                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                                process2.StartInfo.Arguments = "get " + transid + @"-p -o ipfs\" + transid;
                                                                process2.Start();
                                                                process2.WaitForExit();

                                                                if (System.IO.File.Exists("ipfs/" + transid))
                                                                {
                                                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                                    string fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "")
                                                                    {
                                                                        fileName = "artifact";

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


                                                                using (var db2 = new DB(SUP, @"ipfs"))
                                                                {
                                                                    db2.Delete(transid);

                                                                }
                                                            });
                                                        }

                                                    }

                                                }




                                            }


                                        }
                                        else
                                        { fromAddress = TruncateAddress(fromAddress); }

                                        string[] profilePacket = new string[2];

                                        profilePacket[0] = fromAddress;
                                        profilePacket[1] = imagelocation;
                                        profileAddress.Add(supMessagePacket[0], profilePacket);

                                    }
                                    else
                                    {
                                        string[] profilePacket = new string[] { };
                                        profileAddress.TryGetValue(fromAddress, out profilePacket);
                                        fromAddress = profilePacket[0];
                                        imagelocation = profilePacket[1];

                                    }


                                    string tstamp = it.KeyAsString().Split('!')[1];
                                    System.Drawing.Color bgcolor = System.Drawing.Color.White;

                                    string unfilteredmessage = message;
                                    message = Regex.Replace(message, "<<.*?>>", "");

                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        CreateRow(imagelocation, fromAddress, supMessagePacket[0], DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, supMessagePacket[1], true, supPrivateFlow);
                                    });

                                    string pattern = "<<.*?>>";
                                    MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                    foreach (Match match in matches)
                                    {
                                        string content = match.Value.Substring(2, match.Value.Length - 4);
                                        if (!int.TryParse(content, out int id) && !content.Trim().StartsWith("#"))
                                        {

                                            if (content.StartsWith("IPFS:") && content.EndsWith(@"\SEC"))
                                            {


                                                string transid = "empty";
                                                try { transid = content.Substring(5, 46); } catch { }

                                                if (!System.IO.Directory.Exists("root/" + transid + "-build"))
                                                {
                                                    try { Directory.CreateDirectory("root/" + transid); } catch { };
                                                    Directory.CreateDirectory("root/" + transid + "-build");
                                                    Process process2 = new Process();
                                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                    process2.StartInfo.Arguments = "get " + content.Substring(5, 46) + @" -o root\" + transid;
                                                    process2.StartInfo.UseShellExecute = false;
                                                    process2.StartInfo.CreateNoWindow = true;
                                                    process2.Start();
                                                    process2.WaitForExit();
                                                    string fileName;
                                                    if (System.IO.File.Exists("root/" + transid))
                                                    {
                                                        System.IO.File.Move("root/" + transid, "root/" + transid + "_tmp");
                                                        System.IO.Directory.CreateDirectory("root/" + transid);
                                                        fileName = content.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                        Directory.CreateDirectory("root/" + transid);
                                                        System.IO.File.Move("root/" + transid + "_tmp", @"root/" + transid + @"/SEC");
                                                    }

                                                    if (System.IO.File.Exists("root/" + transid + "/" + transid))
                                                    {
                                                        fileName = content.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                        try { System.IO.File.Move("root/" + transid + "/" + transid, @"root/" + transid + @"/SEC"); }
                                                        catch { }
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

                                                byte[] result2 = Root.GetRootBytesByFile(new string[] { @"root/" + transid + @"/SEC" });
                                                result = Root.DecryptRootBytes("good-user", "better-password", @"http://127.0.0.1:18332", profileURN.Links[0].LinkData.ToString(), result2);

                                                Root decryptedroot = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332", "111", result, profileURN.Links[0].LinkData.ToString());
                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff",".mp4",".avi" };

                                                foreach (string file in decryptedroot.File.Keys)
                                                {


                                                    string extension = Path.GetExtension(transid + @"\" + file).ToLower();
                                                    if (!imgExtensions.Contains(extension))
                                                    {
                                                        // Create a new panel to display the metadata
                                                        Panel panel = new Panel();
                                                        panel.BorderStyle = BorderStyle.FixedSingle;
                                                        panel.MinimumSize = new Size(supPrivateFlow.Width - 30, 30);
                                                        panel.AutoSize = true;
                                                        // Create a label for the title
                                                        LinkLabel titleLabel = new LinkLabel();
                                                        titleLabel.Text = transid + @"\" + file;
                                                        titleLabel.Links[0].LinkData = transid + @"\" + file;
                                                        titleLabel.AutoSize = true;
                                                        titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                        titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                        titleLabel.Padding = new Padding(5);
                                                        titleLabel.MouseClick += (sender, e) => { Attachment_Clicked(@"root\" + transid + @"\" + file); };
                                                        panel.Controls.Add(titleLabel);

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            this.supPrivateFlow.Controls.Add(panel);
                                                        });

                                                    }
                                                    else
                                                    {
                                                        if (extension == ".mp4" || extension == ".avi")
                                                        {

                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                AddVideo(transid + @"\" + file, true);
                                                            });

                                                        }
                                                        else
                                                        {

                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                AddImage(transid + @"\" + file, true);
                                                            });
                                                        }

                                                    }


                                                }


                                            }
                                            else
                                            {


                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff",".mp4",".avi" };

                                                string extension = Path.GetExtension(content).ToLower();
                                                if (!imgExtensions.Contains(extension) && !content.Contains("youtube.com") && !content.Contains("youtu.be"))
                                                {
                                                    WebClient client = new WebClient();
                                                    string html = "";
                                                    try { html = client.DownloadString(content.StripLeadingTrailingSpaces()); } catch { }


                                                    // Use regular expressions to extract the metadata from the HTML
                                                    string title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                                    string description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                                    string imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                                    if (description != "")
                                                    {
                                                        // Create a new panel to display the metadata
                                                        Panel panel = new Panel();
                                                        panel.BorderStyle = BorderStyle.FixedSingle;
                                                        panel.Size = new Size(supPrivateFlow.Width - 30, 100);


                                                        // Create a label for the title
                                                        Label titleLabel = new Label();
                                                        titleLabel.Text = title;
                                                        titleLabel.Dock = DockStyle.Top;
                                                        titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                        titleLabel.ForeColor = Color.White;
                                                        titleLabel.MinimumSize = new Size(supPrivateFlow.Width - 130, 30);
                                                        titleLabel.Padding = new Padding(5);
                                                        titleLabel.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                                        panel.Controls.Add(titleLabel);

                                                        // Create a label for the description
                                                        Label descriptionLabel = new Label();
                                                        descriptionLabel.Text = description;
                                                        descriptionLabel.ForeColor = Color.White;
                                                        descriptionLabel.Dock = DockStyle.Fill;
                                                        descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                                        descriptionLabel.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                                        panel.Controls.Add(descriptionLabel);

                                                        // Add an image to the panel if one is defined
                                                        if (!String.IsNullOrEmpty(imageUrl))
                                                        {
                                                            try
                                                            {
                                                                // Create a MemoryStream object from the image data
                                                                byte[] imageData = client.DownloadData(imageUrl);
                                                                MemoryStream memoryStream = new MemoryStream(imageData);

                                                                // Create a new PictureBox control and add it to the panel
                                                                PictureBox pictureBox = new PictureBox();
                                                                pictureBox.Dock = DockStyle.Left;
                                                                pictureBox.Size = new Size(100, 100);
                                                                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                pictureBox.Image = Image.FromStream(memoryStream);
                                                                pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                                                panel.Controls.Add(pictureBox);
                                                            }
                                                            catch
                                                            {
                                                            }
                                                        }
                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            // Add the panel to the flow layout panel
                                                            this.supPrivateFlow.Controls.Add(panel);
                                                        });
                                                    }
                                                    else
                                                    {
                                                        // Create a new panel to display the metadata
                                                        Panel panel = new Panel();
                                                        panel.BorderStyle = BorderStyle.FixedSingle;
                                                        panel.MinimumSize = new Size(supPrivateFlow.Width - 30, 30);
                                                        panel.AutoSize = true;
                                                        // Create a label for the title
                                                        LinkLabel titleLabel = new LinkLabel();
                                                        titleLabel.Text = content;
                                                        titleLabel.Links[0].LinkData = content;
                                                        titleLabel.AutoSize = true;
                                                        titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                        titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                        titleLabel.Padding = new Padding(5);
                                                        titleLabel.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                                        panel.Controls.Add(titleLabel);
                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            this.supPrivateFlow.Controls.Add(panel);
                                                        });

                                                    }

                                                }
                                                else
                                                {
                                                    if (extension == ".mp4" || extension == ".avi" || content.Contains("youtube.com") || content.Contains("youtu.be"))
                                                    {

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            AddVideo(content,true);
                                                        });

                                                    }
                                                    else
                                                    {

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            AddImage(content,true);
                                                        });
                                                    }

                                                }


                                            }
                                        }

                                    }

                                    TableLayoutPanel padding = new TableLayoutPanel
                                    {
                                        RowCount = 1,
                                        ColumnCount = 1,
                                        Dock = DockStyle.Top,
                                        BackColor = Color.Black,
                                        ForeColor = Color.White,
                                        AutoSize = true,
                                        CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                                        Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                                        Padding = new System.Windows.Forms.Padding(0)

                                    };

                                    padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supPrivateFlow.Width - 20));
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        supPrivateFlow.Controls.Add(padding);
                                    });


                                }
                            }
                            rownum++;
                        }
                        it.Dispose();
                    }

                    // Update number of messages displayed
                    numPrivateMessagesDisplayed += 10;
                    supPrivateFlow.ResumeLayout();
                }
                catch { }

                this.Invoke((MethodInvoker)delegate
                {
                    btnPrivateMessage.Enabled = true;
                });
            });

            }

        void AddImage(string imagepath, bool isprivate = false, bool addtoTop = false)
        {
            string imagelocation = "";
            if (imagepath != null)
            {
                imagelocation = imagepath;


                if (!imagepath.ToLower().StartsWith("http"))
                {
                    imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (imagepath.ToLower().StartsWith("ipfs:")) { imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\"); if (imagepath.Length == 51) { imagelocation += @"\artifact"; } }
                }
                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                Match imgurnmatch = regexTransactionId.Match(imagelocation);
                string transactionid = imgurnmatch.Value;
                Root root = new Root();
                if (!File.Exists(imagelocation))
                {
                    switch (imagepath.ToUpper().Substring(0, 4))
                    {
                        case "MZC:":
                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                            break;
                        case "BTC:":

                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                            break;
                        case "LTC:":

                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");


                            break;
                        case "DOG:":
                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                            break;
                        case "IPFS":
                            string transid = "empty";
                            try { transid = imagepath.Substring(5, 46); } catch { }

                            if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                            {
                                try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                Directory.CreateDirectory("ipfs/" + transid + "-build");
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + imagepath.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();
                                string fileName;
                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    fileName = imagepath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    Directory.CreateDirectory("ipfs/" + transid);
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", imagelocation);
                                }

                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                {
                                    fileName = imagepath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, imagelocation);
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

                            break;
                        default:
                            if (!imagepath.ToUpper().StartsWith("HTTP") && transactionid != "")
                            {
                                Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");

                            }
                            break;
                    }
                }



            }


            TableLayoutPanel msg = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 1,
                Dock = DockStyle.Top,
                BackColor = Color.Black,
                ForeColor = Color.White,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 0),
                Padding = new System.Windows.Forms.Padding(0)

            };

            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420));

            if (isprivate)
            {
                supPrivateFlow.Controls.Add(msg);
            }
            else
            {





                if (addtoTop)
                {
                    supFlow.Controls.Add(msg);
                    supFlow.Controls.SetChildIndex(msg, 2);
                }
                else
                {
                    supFlow.Controls.Add(msg);
                }


            }
            PictureBox pictureBox = new PictureBox();

            // Set the PictureBox properties

            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Width = 400;
            pictureBox.Height = 400;
            pictureBox.BackColor = Color.Black;
            pictureBox.ImageLocation = imagelocation;
            pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };
            msg.Controls.Add(pictureBox);


        }

        async void AddVideo(string videopath, bool isprivate = false, bool addtoTop = false)
        {
            string videolocation = "";
            if (videopath != null)
            {
                videolocation = videopath;


                if (!videopath.ToLower().StartsWith("http"))
                {
                    videolocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + videopath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (videopath.ToLower().StartsWith("ipfs:")) { videolocation = videolocation.Replace(@"\root\", @"\ipfs\"); if (videopath.Length == 51) { videolocation += @"\artifact"; } }
                }
                else
                {
                    string pattern = @"(?:youtu\.be/|youtube(?:-nocookie)?\.com/(?:[^/\n\s]*[/\n\s]*(?:v/|e(?:mbed)?/|.*[?&]v=))?)?([a-zA-Z0-9_-]{11})";

                    Match match = Regex.Match(videopath, pattern);
                    if (match.Success)
                    {
                        videolocation = @"https://www.youtube.com/embed/" + match.Groups[1].Value;
                    }

                }

                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                Match imgurnmatch = regexTransactionId.Match(videolocation);
                string transactionid = imgurnmatch.Value;
                Root root = new Root();
                if (!videolocation.Contains("www.youtube.com") && !File.Exists(videolocation))
                {
                    switch (videopath.ToUpper().Substring(0, 4))
                    {
                        case "MZC:":
                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                            break;
                        case "BTC:":

                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                            break;
                        case "LTC:":

                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");


                            break;
                        case "DOG:":
                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                            break;
                        case "IPFS":
                            string transid = "empty";
                            try { transid = videopath.Substring(5, 46); } catch { }

                            if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                            {
                                try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                Directory.CreateDirectory("ipfs/" + transid + "-build");
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + videopath.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();
                                string fileName;
                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    fileName = videopath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    Directory.CreateDirectory("ipfs/" + transid);
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", videolocation);
                                }

                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                {
                                    fileName = videopath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, videolocation);
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

                            break;
                        default:
                            if (!videopath.ToUpper().StartsWith("HTTP") && transactionid != "")
                            {
                                Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");

                            }
                            break;
                    }
                }



            }


            TableLayoutPanel msg = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 1,
                Dock = DockStyle.Top,
                BackColor = Color.Black,
                ForeColor = Color.White,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 0),
                Padding = new System.Windows.Forms.Padding(0)

            };

            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420));

            if (isprivate)
            {
                supPrivateFlow.Controls.Add(msg);
            }
            else
            {





                if (addtoTop)
                {
                    supFlow.Controls.Add(msg);
                    supFlow.Controls.SetChildIndex(msg, 2);
                }
                else
                {
                    supFlow.Controls.Add(msg);
                }


            }
            Microsoft.Web.WebView2.WinForms.WebView2 webviewer = new Microsoft.Web.WebView2.WinForms.WebView2();
            webviewer.AllowExternalDrop = true;
            webviewer.BackColor = System.Drawing.Color.Black;
            webviewer.CreationProperties = null;
            webviewer.DefaultBackgroundColor = System.Drawing.Color.Black;
           
            webviewer.Name = "webviewer";
            webviewer.Size = new System.Drawing.Size(400, 300);
            webviewer.ZoomFactor = 1D;

            string viewerPath = Path.GetDirectoryName(videolocation) + @"\urnviewer.html";

            if (videolocation.Contains("www.youtube.com")){
                try { Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + videolocation.Substring(29)); } catch { }
                viewerPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\"+ videolocation.Substring(29) + @"\urnviewer.html"; }

            string htmlstring = "<html><body><embed src=\"" + videolocation + "\" width=100% height=100% ></body></html>";
            webviewers.Add(webviewer);
            msg.Controls.Add(webviewer);
           

            try
            {
                System.IO.File.WriteAllText(viewerPath, htmlstring);
                await webviewer.EnsureCoreWebView2Async();
                webviewer.CoreWebView2.Navigate(viewerPath);
            }
            catch
            {
                Thread.Sleep(1000);
                try
                {
                    await webviewer.EnsureCoreWebView2Async();
                    webviewer.CoreWebView2.Navigate(viewerPath);
                }
                catch { }
            }
                      

        }

        void CreateRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp, string messageText, string transactionid, bool isprivate, FlowLayoutPanel layoutPanel)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 4,
                AutoSize = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            // Add the width of the first column to fixed value and second to fill remaining space
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));

            layoutPanel.Controls.Add(row);

            // Create a PictureBox with the specified image

            if (File.Exists(imageLocation) || imageLocation.ToUpper().StartsWith("HTTP"))
            {
                PictureBox picture = new PictureBox
                {
                    Size = new System.Drawing.Size(50, 50),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    ImageLocation = imageLocation,
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
                    Size = new System.Drawing.Size(50, 50),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    ImageLocation = randomGifFile,
                    Margin = new System.Windows.Forms.Padding(0),
                };
                row.Controls.Add(picture, 0, 0);
            }


            // Create a LinkLabel with the owner name
            LinkLabel owner = new LinkLabel
            {
                Text = ownerName,
                BackColor = Color.Black,
                ForeColor = Color.White,
                AutoSize = true

            };
            owner.LinkClicked += (sender, e) => { Owner_LinkClicked(ownerId); };
            owner.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            owner.Margin = new System.Windows.Forms.Padding(3);
            owner.Dock = DockStyle.Bottom;
            row.Controls.Add(owner, 1, 0);


            // Create a LinkLabel with the owner name
            Label tstamp = new Label
            {
                AutoSize = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                Margin = new System.Windows.Forms.Padding(0),
                Dock = DockStyle.Bottom
            };
            row.Controls.Add(tstamp, 2, 0);


            Label deleteme = new Label
            {
                AutoSize = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Text = "🗑",
                Margin = new System.Windows.Forms.Padding(0),
                Dock = DockStyle.Bottom
            };
            deleteme.Click += (sender, e) => { deleteme_LinkClicked(transactionid); };
            row.Controls.Add(deleteme, 3, 0);


            if (messageText != "")
            {
                TableLayoutPanel msg = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 1,
                    Dock = DockStyle.Top,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true,
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                    Margin = new System.Windows.Forms.Padding(0, 0, 0, 0),
                    Padding = new System.Windows.Forms.Padding(0)

                };
                msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, layoutPanel.Width - 20));


                layoutPanel.Controls.Add(msg);


                Label message = new Label
                {
                    AutoSize = true,
                    Text = messageText,
                    MinimumSize = new Size(200, 46),
                    Font = new System.Drawing.Font("Segoe UI", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Margin = new System.Windows.Forms.Padding(0),
                    Padding = new System.Windows.Forms.Padding(10, 20, 10, 20),
                    TextAlign = System.Drawing.ContentAlignment.TopLeft
                };
                msg.Controls.Add(message, 1, 0);
            }


        }

        void CreateFeedRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp, string messageText, string transactionid, System.Drawing.Color bgcolor, FlowLayoutPanel layoutPanel, bool addtoTop = false)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 4,
                AutoSize = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            // Add the width of the first column to fixed value and second to fill remaining space
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));

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

            if (File.Exists(imageLocation) || imageLocation.ToUpper().StartsWith("HTTP"))
            {
                PictureBox picture = new PictureBox
                {
                    Size = new System.Drawing.Size(50, 50),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    ImageLocation = imageLocation,
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
                    Size = new System.Drawing.Size(50, 50),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    ImageLocation = randomGifFile,
                    Margin = new System.Windows.Forms.Padding(0),
                };
                row.Controls.Add(picture, 0, 0);
            }


            // Create a LinkLabel with the owner name
            LinkLabel owner = new LinkLabel
            {
                Text = ownerName,
                BackColor = Color.Black,
                ForeColor = Color.White,
                AutoSize = true

            };
            owner.LinkClicked += (sender, e) => { Owner_LinkClicked(ownerId); };
            owner.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            owner.Margin = new System.Windows.Forms.Padding(3);
            owner.Dock = DockStyle.Bottom;
            row.Controls.Add(owner, 1, 0);


            if (timestamp.Year > 1975)
            {  // Create a LinkLabel with the owner name
                Label tstamp = new Label
                {
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                    Margin = new System.Windows.Forms.Padding(0),
                    Dock = DockStyle.Bottom
                };
                row.Controls.Add(tstamp, 2, 0);
            }

            Label deleteme = new Label
            {
                AutoSize = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Text = "🗑",
                Margin = new System.Windows.Forms.Padding(0),
                Dock = DockStyle.Bottom
            };
            deleteme.Click += (sender, e) => { deleteme_LinkClicked(transactionid); };
            row.Controls.Add(deleteme, 3, 0);

            if (messageText != "")
            {
                TableLayoutPanel msg = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 1,
                    Dock = DockStyle.Top,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true,
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                    Margin = new System.Windows.Forms.Padding(0, 0, 0, 0),
                    Padding = new System.Windows.Forms.Padding(0)

                };

                msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 20));

                if (addtoTop)
                {
                    layoutPanel.Controls.Add(msg);
                    layoutPanel.Controls.SetChildIndex(msg, 1);
                }
                else
                {
                    layoutPanel.Controls.Add(msg);
                }


                Label message = new Label
                {
                    AutoSize = true,
                    Text = messageText,
                    MinimumSize = new Size(280, 46),
                    Font = new System.Drawing.Font("Segoe UI", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Margin = new System.Windows.Forms.Padding(0),
                    Padding = new System.Windows.Forms.Padding(10, 20, 10, 20),
                    TextAlign = System.Drawing.ContentAlignment.TopLeft
                };
                msg.Controls.Add(message);
            }

        }

        void Owner_LinkClicked(string ownerId)
        {

            new ObjectBrowser(ownerId).Show();
        }

        void Attachment_Clicked(string path)
        {
            if (path.ToUpper().StartsWith("IPFS:") || path.ToUpper().StartsWith("BTC:") || path.ToUpper().StartsWith("MZC:") || path.ToUpper().StartsWith("LTC:") || path.ToUpper().StartsWith("DOG:"))
            {
                new ObjectBrowser(path).Show();
            }
            else
            {
                try
                { System.Diagnostics.Process.Start(path); }
                catch { System.Media.SystemSounds.Exclamation.Play(); }
            }
        }

        void deleteme_LinkClicked(string transactionid)
        {

            string unfilteredmessage = "";
            try { unfilteredmessage = System.IO.File.ReadAllText(@"root/" + transactionid + @"/MSG"); } catch { }


            string pattern = "<<.*?>>";
            MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
            foreach (Match match in matches)
            {
                string content = match.Value.Substring(2, match.Value.Length - 4);
                if (!int.TryParse(content, out int id) && !content.Trim().StartsWith("#"))
                {

                    string imagelocation = "";
                    if (content != null)
                    {
                        imagelocation = content;

                        if (!content.ToLower().StartsWith("http"))
                        {
                            imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");
                            if (content.ToLower().StartsWith("ipfs:"))
                            {
                                imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\");
                                if (content.Length == 51) { imagelocation += @"\artifact"; }
                            }

                            string parentDir = Path.GetDirectoryName(imagelocation);

                            if (Directory.Exists(parentDir))
                            {
                                Directory.Delete(parentDir, true);
                            }

                            parentDir = parentDir.Replace(@"\ipfs\", @"\root\");

                            if (Directory.Exists(parentDir))
                            {
                                Directory.Delete(parentDir, true);
                            }

                            if (Directory.Exists(parentDir + "-build"))
                            {
                                Directory.Delete(parentDir + "-build", true);
                            }

                        }
                    }


                }
            }

            try
            {
                Directory.Delete(@"root\" + transactionid, true);
                Directory.CreateDirectory(@"root\" + transactionid);
            }
            catch { }
            Root P2FKRoot = new Root();
            var rootSerialized = JsonConvert.SerializeObject(P2FKRoot);
            System.IO.File.WriteAllText(@"root\" + transactionid + @"\" + "ROOT.json", rootSerialized);
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


        private void btnPublicMessage_Click(object sender, EventArgs e)
        {
            refreshFriendFeed.BackColor = System.Drawing.Color.White;
            refreshFriendFeed.ForeColor = System.Drawing.Color.Black;
            RefreshSupMessages();
            if (btnPublicMessage.BackColor == Color.White)
            {
                btnPublicMessage.BackColor = Color.Blue; btnPublicMessage.ForeColor = Color.Yellow;

                btnPrivateMessage.BackColor = Color.White;
                btnPrivateMessage.ForeColor = Color.Black;
            }
            
        }


        private void splitContainer1_DoubleClick(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel2Collapsed = false;
            }
            else
            {

                splitContainer1.Panel2Collapsed = true;
            }
        }

        private void profileURN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ProfileMint(profileURN.Links[0].LinkData.ToString()).Show();
        }

        private void btnPrivateMessage_Click(object sender, EventArgs e)
        {
            refreshFriendFeed.BackColor = System.Drawing.Color.White;
            refreshFriendFeed.ForeColor = System.Drawing.Color.Black;

            RefreshPrivateSupMessages();

            if (btnPrivateMessage.BackColor == Color.White)
            {
                if (splitContainer1.Panel2Collapsed)
                {
                    splitContainer1.Panel2Collapsed = false;
                }

                btnPrivateMessage.BackColor = Color.Blue; btnPrivateMessage.ForeColor = Color.Yellow;

                btnPublicMessage.BackColor = Color.White;
                btnPublicMessage.ForeColor = Color.Black;
            }
          

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string profileowner = "";
            string toaddress = "";
            if (profileOwner.Tag != null) { profileowner = profileOwner.Tag.ToString(); }
            if (profileURN.Links[0].LinkData != null) { toaddress = profileURN.Links[0].LinkData.ToString(); }
            bool isprivate = false;

            if (btnPrivateMessage.BackColor == Color.Blue) { isprivate = true; }

            DiscoBall disco = new DiscoBall(profileowner, profileOwner.ImageLocation, toaddress, profileIMG.ImageLocation, isprivate);
            disco.StartPosition = FormStartPosition.CenterScreen;
            disco.Show(this);
            disco.Focus();


        }

        private void btnFollow_Click(object sender, EventArgs e)
        {

            Dictionary<string, string> friendDict = new Dictionary<string, string>();


            foreach (PictureBox pb in flowFollow.Controls)
            {

                try { friendDict.Add(pb.Tag.ToString(), pb.ImageLocation); } catch { }
            }

            // Create a new PictureBox
            PictureBox pictureBox = new PictureBox();

            // Set the PictureBox properties
            pictureBox.Tag = profileURN.Links[0].LinkData.ToString();
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Width = 50;
            pictureBox.Height = 50;
            pictureBox.ImageLocation = profileIMG.ImageLocation;

            // Add event handlers to the PictureBox
            pictureBox.Click += new EventHandler(Friend_Click);
            pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(Friend_MouseUp);

            try {
                friendDict.Add(profileURN.Links[0].LinkData.ToString(), profileIMG.ImageLocation);
                flowFollow.Controls.Add(pictureBox);
            } catch { }
            // Add the PictureBox to the FlowLayoutPanel
                     

            string json = JsonConvert.SerializeObject(friendDict);
            string filePath = @"root\MyFriendList.Json";
            File.WriteAllText(filePath, json);

        }


        private void Friend_Click(object sender, EventArgs e)
        {
            // Check if the user left-clicked on the PictureBox
            if (((System.Windows.Forms.MouseEventArgs)e).Button == MouseButtons.Left)
            {
                numMessagesDisplayed = 0;
                numFriendFeedsDisplayed = 0;
                numPrivateMessagesDisplayed = 0;
                refreshFriendFeed.BackColor = System.Drawing.Color.White;
                refreshFriendFeed.ForeColor = System.Drawing.Color.Black;
                btnPrivateMessage.BackColor = System.Drawing.Color.White;
                btnPrivateMessage.ForeColor = System.Drawing.Color.Black;
                btnPublicMessage.BackColor = Color.Blue;
                btnPublicMessage.ForeColor = Color.Yellow;
                supPrivateFlow.Controls.Clear();

                if (!((PictureBox)sender).ImageLocation.ToString().Contains(@"root\keywords") ){
                    // Get the tag text from the PictureBox
                    string address = ((PictureBox)sender).Tag.ToString();
                    MakeActiveProfile(address);
                        }
                else
                {

                    profileBIO.Text = ""; profileCreatedDate.Text = ""; profileIMG.ImageLocation = ""; lblProcessHeight.Text = "";
                    profileURN.Links[0].LinkData = ((PictureBox)sender).Tag.ToString();
                    profileURN.Text = Path.GetFileNameWithoutExtension(((PictureBox)sender).ImageLocation.ToString());
                    profileIMG.ImageLocation = ((PictureBox)sender).ImageLocation.ToString();
                    //string address = ((PictureBox)sender).Tag.ToString();
                   // MakeActiveProfile(address);

                }

                RefreshSupMessages();

            }
        }

        private void Friend_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Check if the user right-clicked on the PictureBox
            if (e.Button == MouseButtons.Right)
            {
                // Remove the PictureBox from the FlowLayoutPanel
                flowFollow.Controls.Remove((PictureBox)sender);

                Dictionary<string, string> friendDict = new Dictionary<string, string>();
                foreach (PictureBox pb in flowFollow.Controls)
                {

                    try { friendDict.Add(pb.Tag.ToString(), pb.ImageLocation); } catch { }
                }

                string json = JsonConvert.SerializeObject(friendDict);
                string filePath = @"root\MyFriendList.Json";
                File.WriteAllText(filePath, json);
                try { File.Delete(@"root\MyFriendFeed.Json"); } catch { }
                numFriendFeedsDisplayed = 0;
            }
        }

        private void refreshFriendFeed_Click(object sender, EventArgs e)
        {
            refreshFriendFeed.BackColor = System.Drawing.Color.Blue;
            refreshFriendFeed.ForeColor = System.Drawing.Color.Yellow;
            btnPublicMessage.BackColor = System.Drawing.Color.White;
            btnPrivateMessage.BackColor = System.Drawing.Color.White;
            btnPublicMessage.ForeColor = System.Drawing.Color.Black;
            btnPrivateMessage.ForeColor = System.Drawing.Color.Black;

            refreshFriendFeed.Enabled = false;
            numMessagesDisplayed = 0;
            List<string> friendFeed = new List<string>();

            if (numFriendFeedsDisplayed == 0) {

                foreach (var viewer in webviewers)
                {
                    viewer.Dispose();
                }

                supFlow.Controls.Clear();
                 }

            if (File.Exists(@"root\MyFriendList.Json"))
            {
                Task BuildMessage = Task.Run(() =>
                {

                    var myFriendsJson = File.ReadAllText(@"root\MyFriendList.Json");
                    var myFriends = JsonConvert.DeserializeObject<Dictionary<string, string>>(myFriendsJson);

                    // Iterate over each key in the dictionary, get public messages by address, and combine them into a list
                    var allMessages = new List<object>();
                    foreach (var key in myFriends.Keys)
                    {
                        var result = OBJState.GetPublicMessagesByAddress(key, "good-user", "better-password", "http://127.0.0.1:18332");
                        var messages = result.GetType().GetProperty("Messages").GetValue(result) as List<object>;

                        // Add the "to" element to each message object
                        foreach (var message in messages)
                        {

                            var fromProp = message.GetType().GetProperty("FromAddress");
                            var messageProp = message.GetType().GetProperty("Message");
                            var blockDateProp = message.GetType().GetProperty("BlockDate");
                            var toProp = message.GetType().GetProperty("ToAddress");
                            string _from = fromProp?.GetValue(message).ToString();
                            string _to = toProp?.GetValue(message).ToString();
                            string _message = messageProp?.GetValue(message).ToString();
                            string _blockdate = blockDateProp?.GetValue(message).ToString();

                            if (!friendFeed.Contains(_from + _message + _blockdate))
                            {
                                friendFeed.Add(_from + _message + _blockdate);

                                allMessages.Add(new
                                {
                                    Message = _message,
                                    FromAddress = _from,
                                    ToAddress = _to,
                                    BlockDate = _blockdate
                                });
                            }
                        }
                    }

                    // Sort the combined list by block date
                    allMessages.Sort((m1, m2) =>
                    {
                        var date1Prop = m1?.GetType().GetProperty("BlockDate");
                        var date2Prop = m2?.GetType().GetProperty("BlockDate");
                        if (date1Prop == null && date2Prop == null)
                        {
                            return 0;
                        }
                        else if (date1Prop == null)
                        {
                            return -1;
                        }
                        else if (date2Prop == null)
                        {
                            return 1;
                        }
                        else
                        {
                            var date1 = DateTime.ParseExact(date1Prop.GetValue(m1).ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            var date2 = DateTime.ParseExact(date2Prop.GetValue(m2).ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                            return date2.CompareTo(date1);
                        }
                    });

                    // Serialize the combined list to MyFriendsFeed.Json file
                    var myFriendsFeedJson = JsonConvert.SerializeObject(allMessages);
                    File.WriteAllText(@"root\MyFriendFeed.Json", myFriendsFeedJson);


                    foreach (var message in allMessages.Skip(numFriendFeedsDisplayed).Take(10))
                    {
                        var fromProp = message.GetType().GetProperty("FromAddress");
                        var toProp = message.GetType().GetProperty("ToAddress");
                        var messageProp = message.GetType().GetProperty("Message");
                        var blockDateProp = message.GetType().GetProperty("BlockDate");

                        string _from = fromProp?.GetValue(message).ToString();
                        string _to = toProp?.GetValue(message).ToString();
                        string _message = messageProp?.GetValue(message).ToString();
                        string _blockdate = blockDateProp?.GetValue(message).ToString();
                        string imglocation = "";

                        string unfilteredmessage = _message;
                        _message = Regex.Replace(_message, "<<.*?>>", "");
                        this.Invoke((MethodInvoker)delegate
                        {
                            try { imglocation = myFriends[_from]; } catch { }
                            CreateFeedRow(imglocation, _from, _from, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, "", Color.White, supFlow);

                            try { imglocation = myFriends[_to]; } catch { }
                            CreateFeedRow(imglocation, _to, _to, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", Color.White, supFlow);
                        });
                        string pattern = "<<.*?>>";
                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                        foreach (Match match in matches)
                        {


                            string content = match.Value.Substring(2, match.Value.Length - 4);
                            if (!int.TryParse(content, out int id) && !content.Trim().StartsWith("#"))
                            {

                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff" ,".mp4",".avi"};

                                string extension = Path.GetExtension(content).ToLower();
                                if (!imgExtensions.Contains(extension) && !content.Contains("youtube.com") && !content.Contains("youtu.be"))
                                {
                                    WebClient client = new WebClient();
                                    string html = "";
                                    try
                                    {
                                        html = client.DownloadString(content.StripLeadingTrailingSpaces());

                                    }
                                    catch { }

                                    // Use regular expressions to extract the metadata from the HTML
                                    string title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                    string description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                    string imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                    if (description != "")
                                    {
                                        // Create a new panel to display the metadata
                                        Panel panel = new Panel();
                                        panel.BorderStyle = BorderStyle.FixedSingle;
                                        panel.Size = new Size(supFlow.Width - 30, 100);


                                        // Create a label for the title
                                        Label titleLabel = new Label();
                                        titleLabel.Text = title;
                                        titleLabel.Dock = DockStyle.Top;
                                        titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                        titleLabel.ForeColor = Color.White;
                                        titleLabel.MinimumSize = new Size(supFlow.Width - 130, 30);
                                        titleLabel.Padding = new Padding(5);
                                        panel.Controls.Add(titleLabel);

                                        // Create a label for the description
                                        Label descriptionLabel = new Label();
                                        descriptionLabel.Text = description;
                                        descriptionLabel.ForeColor = Color.White;
                                        descriptionLabel.Dock = DockStyle.Fill;
                                        descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                        panel.Controls.Add(descriptionLabel);

                                        // Add an image to the panel if one is defined
                                        if (!String.IsNullOrEmpty(imageUrl))
                                        {
                                            try
                                            {
                                                // Create a MemoryStream object from the image data
                                                byte[] imageData = client.DownloadData(imageUrl);
                                                MemoryStream memoryStream = new MemoryStream(imageData);

                                                // Create a new PictureBox control and add it to the panel
                                                PictureBox pictureBox = new PictureBox();
                                                pictureBox.Dock = DockStyle.Left;
                                                pictureBox.Size = new Size(100, 100);
                                                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                pictureBox.Image = Image.FromStream(memoryStream);
                                                panel.Controls.Add(pictureBox);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            // Add the panel to the flow layout panel
                                            this.supFlow.Controls.Add(panel);
                                        });
                                    }
                                    else
                                    {
                                        // Create a new panel to display the metadata
                                        Panel panel = new Panel();
                                        panel.BorderStyle = BorderStyle.FixedSingle;
                                        panel.MinimumSize = new Size(supFlow.Width - 30, 30);
                                        panel.AutoSize = true;
                                        // Create a label for the title
                                        LinkLabel titleLabel = new LinkLabel();
                                        titleLabel.Text = content;
                                        titleLabel.Links[0].LinkData = content;
                                        titleLabel.AutoSize = true;
                                        titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                        titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                        titleLabel.Padding = new Padding(5);
                                        panel.Controls.Add(titleLabel);
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            this.supFlow.Controls.Add(panel);
                                        });

                                    }

                                }
                                else
                                {

                                        if (extension == ".mp4" || extension == ".avi" || content.Contains("youtube.com") || content.Contains("youtu.be"))
                                        {

                                            this.Invoke((MethodInvoker)delegate
                                            {
                                                AddVideo(content);
                                            });

                                        }
                                        else
                                        {

                                            this.Invoke((MethodInvoker)delegate
                                            {
                                                AddImage(content);
                                            });
                                        }
                                    

                                }



                            }


                        }

                        TableLayoutPanel padding = new TableLayoutPanel
                        {
                            RowCount = 1,
                            ColumnCount = 1,
                            Dock = DockStyle.Top,
                            BackColor = Color.Black,
                            ForeColor = Color.White,
                            AutoSize = true,
                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                            Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                            Padding = new System.Windows.Forms.Padding(0)

                        };

                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 20));

                        this.Invoke((MethodInvoker)delegate
                        {
                            supFlow.Controls.Add(padding);
                        });

                        numFriendFeedsDisplayed++;

                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        refreshFriendFeed.Enabled = true;
                    });

                });

            }
            

        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            if (btnMute.Text == "mute")
            {
                var WORK = new Options { CreateIfMissing = true };
                using (var db = new DB(WORK, @"root\mute"))
                {
                    db.Put(profileURN.Links[0].LinkData.ToString(), "true");
                }
                btnMute.Text = "unmute";
            }
            else
            {
                var WORK = new Options { CreateIfMissing = true };
                using (var db = new DB(WORK, @"root\mute"))
                {
                    db.Delete(profileURN.Links[0].LinkData.ToString());
                }
                btnMute.Text = "mute";
            }
        }

        private void btnBlock_Click(object sender, EventArgs e)
        {


            try
            {
                var WORK = new Options { CreateIfMissing = true };
                using (var db = new DB(WORK, @"root\oblock"))
                {
                    db.Put(profileURN.Links[0].LinkData.ToString(), "true");

                }
                var WORK2 = new Options { CreateIfMissing = true };
                using (var db = new DB(WORK2, @"root\oblock2"))
                {
                    db.Put(profileURN.Links[0].LinkData.ToString(), "true");

                }

                var SUP = new Options { CreateIfMissing = true };
                var keysToDelete = new HashSet<string>(); // Create a new HashSet to store the keys to delete

                using (var db = new DB(SUP, @"root\found"))
                {
                    LevelDB.Iterator it = db.CreateIterator();

                    for (
                        it.SeekToLast();
                        it.IsValid();
                        it.Prev()
                    )
                    {
                        string key = it.KeyAsString();
                        if (key.Contains(profileURN.Links[0].LinkData.ToString()))
                        {
                            keysToDelete.Add(key); // Add the key to the HashSet
                        }
                    }

                    it.Dispose();

                    var batch = new WriteBatch(); // Create a new WriteBatch to delete the keys
                    foreach (var key in keysToDelete)
                    {
                        batch.Delete(key); // Add a delete operation for each key in the HashSet
                    }
                    db.Write(batch); // Execute the batch to delete the keys from the database
                }


                Root[] root = Root.GetRootsByAddress(profileURN.Links[0].LinkData.ToString(), "good-user", "better-password", @"http://127.0.0.1:18332");

                foreach (Root rootItem in root)
                {

                    using (var db = new DB(WORK, @"root\tblock"))
                    {
                        db.Put(rootItem.TransactionId, "true");

                    }
                    try
                    {
                        Directory.Delete(@"root\" + rootItem.TransactionId, true);
                    }
                    catch { }
                }


                try { Directory.Delete(@"root\" + profileURN.Links[0].LinkData.ToString(), true); } catch { }
                try { Directory.Delete(@"root\" + Root.GetPublicAddressByKeyword(profileURN.Text), true); } catch { }

                foreach (Control control in flowFollow.Controls)
                {
                    if (control is PictureBox pictureBox && pictureBox.Tag.ToString() == profileURN.Links[0].LinkData.ToString())
                    {
                        flowFollow.Controls.Remove(pictureBox);
                    }
                }




            }
            catch { }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            MakeActiveProfile(profileURN.Links[0].LinkData.ToString());
            numMessagesDisplayed = 0;
            refreshFriendFeed.BackColor = System.Drawing.Color.White;
            refreshFriendFeed.ForeColor = System.Drawing.Color.Black;
        }

        //GPT3
        static void GenerateImage(string text)
        {
            // Set the image size
            int width = 1000;
            int height = 1000;

            if (!Directory.Exists(@"root\keywords")) { Directory.CreateDirectory(@"root\keywords"); }

            // Create a new bitmap image with the specified size
            using (Bitmap bmp = new Bitmap(width, height))
            {
                // Create a graphics object to draw on the image
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    // Clear the image with a random background color
                    Random random = new Random();
                    int red = random.Next(128, 256); // From 128 to 255 (avoiding very dark colors)
                    int green = random.Next(128, 256);
                    int blue = random.Next(128, 256);
                    
                    graphics.Clear(Color.FromArgb(red, green, blue));

                    // Set up the font and text formatting
                    float fontSize = 150;
                    FontFamily fontFamily = FontFamily.GenericSansSerif;
                    Font font = null;

                    // Declare textSize outside the loop
                    SizeF textSize;

                    // Calculate the font size dynamically based on the image size and text length
                    while (true)
                    {
                        font?.Dispose();
                        font = new Font(fontFamily, fontSize);
                        textSize = graphics.MeasureString(text, font);
                        if (textSize.Width < width && textSize.Height < height)
                            break;

                        fontSize -= 1;
                    }

                    StringFormat stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    // Calculate the center positions for the text
                    float x = 0;//(width - textSize.Width) / 2;
                    float y = 0; //(height - textSize.Height) / 2;

                    graphics.DrawString(text, font, Brushes.Black, new RectangleF(x, y, width, height), stringFormat);

                    // Save the image to the specified folder
                    string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\keywords\" + text + ".png";
                    bmp.Save(filePath, ImageFormat.Png);
                }
            }
        }






    }
}
