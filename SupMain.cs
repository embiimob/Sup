//
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
using Message = System.Windows.Forms.Message;
using AngleSharp.Text;
using System.Drawing.Imaging;
using NAudio.Wave;
using System.Text;
using NReco.VideoConverter;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;

namespace SUP
{
    public partial class SupMain : Form
    {
        private readonly static object SupLocker = new object();
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        private bool testnet = true;
        private bool friendClicked = false;
        private List<string> BTCMemPool = new List<string>();
        private List<string> BTCTMemPool = new List<string>();
        private List<string> MZCMemPool = new List<string>();
        private List<string> LTCMemPool = new List<string>();
        private List<string> DOGMemPool = new List<string>();


        private bool btcActive;
        private bool mzcActive;
        private bool ltcActive;
        private bool dogActive;
        ObjectBrowserControl OBcontrol = new ObjectBrowserControl();
        private int numMessagesDisplayed;
        private int numPrivateMessagesDisplayed;
        private int numFriendFeedsDisplayed;
        
        // Track displayed messages to prevent duplicates
        private HashSet<string> displayedPrivateMessageIds = new HashSet<string>();
        private HashSet<string> displayedPublicMessageIds = new HashSet<string>();
        private HashSet<string> displayedCommunityMessageIds = new HashSet<string>();
        
        FlowLayoutPanel supPrivateFlow = new FlowLayoutPanel();
        AudioPlayer audioPlayer = new AudioPlayer();

        public SupMain()
        {
            InitializeComponent();
            supFlow.MouseWheel += supFlow_MouseWheel;
            supPrivateFlow.MouseWheel += supPrivateFlow_MouseWheel;

            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
            myTooltip.SetToolTip(btnLive, "click 🧿 to enable or disable realtime monitoring.");
            myTooltip.SetToolTip(btnMint, "click  💎 to open the sup profile and object minting menu.\nif 🔍 click to reload the object search panel.");
            myTooltip.SetToolTip(btnJukeBox, "click 🎵 to open the jukebox audio searching tool.\nthe active profile will be searched by default.");
            myTooltip.SetToolTip(btnBlock, "click to attempt to remove and block all transactions signed by the active profile.\n\nnote: signature blocks must be removed from the workbench.");
            myTooltip.SetToolTip(btnConnections, "click 🗝 to open the sup connections panel.\n\nfrom the connection panel you can:\nlaunch all 5 blockchains included with sup\nenable or disable various user preferences\nenable the IPFS dameon and perform related functions\npurge and clear cached data, blocks and mutes.");
            myTooltip.SetToolTip(btnDisco, "click 📣 to open the sup direct messaging panel.\nthe to: field is prepopulated with the active profile address.\nclick on 🤐 before clicking 📣 to send the active profile a private message\n\nnote: search for your own local profile first to prepopulate the from: field.");
            myTooltip.SetToolTip(btnFollow, "click to add the currently active profile or #search term to your follow list.");
            myTooltip.SetToolTip(btnHome, "click to view the active profile's link list if any have been published.");
            myTooltip.SetToolTip(btnInquirySearch, "click ⁉️ to open the sup poll searching tool.\nthe active profile will be searched by default.");
            myTooltip.SetToolTip(btnMute, "click to mute or unmute the active profile.\nmuting prevents any further sup messages signed by their address from being displayed.");
            myTooltip.SetToolTip(btnPrivateMessage, "click 🤐 to view the active profile's private messages if it's private key exists in your local wallet.\nclick 🤐 before clicking 📣 to send the active profile a direct private message.\n\nnote: your direct private messages are not viewable after sending.");
            myTooltip.SetToolTip(btnPublicMessage, "click 😍 to view the active profile's public messages.\nclick 😍 before clicking 📣 to send the active profile a public message.");
            myTooltip.SetToolTip(btnSkipAudio, "click ⏩ skip to skip the currently playing audio file while in live monitoring mode.");
            myTooltip.SetToolTip(btnVideoSearch, "click 🎬 to open the supflix video searching tool.\nthe active profile will be searched by default.");
            myTooltip.SetToolTip(btnWorkBench, "click ⚙️ to open the sup workbench panel.\n the workbench is full of tools and metrics to help you understand most sup function calls.");
            myTooltip.SetToolTip(lblProcessHeight, "the total amount of transactions associated with the active profile.\nincludes social media posts as well as object mints and trades.");
            myTooltip.SetToolTip(btnCommunityFeed, "click 🌆 to see the top 50 posts from all of the profiles you are following in the order that they were posted.");
            myTooltip.SetToolTip(btnMainnetSwitch, "click to toggle all sup functions between bitcoin testnet (green) and bitcoin mainnet (orange)");

        }

        private void supFlow_MouseWheel(object sender, MouseEventArgs e)
        {

            if (btnCommunityFeed.Enabled == false || btnPublicMessage.Enabled == false || btnPrivateMessage.Enabled == false) { return; }

            if (btnPrivateMessage.BackColor == System.Drawing.Color.Blue) { btnPrivateMessage.BackColor = System.Drawing.Color.White; btnPrivateMessage.ForeColor = System.Drawing.Color.Black; btnPublicMessage.BackColor = System.Drawing.Color.Blue; btnPublicMessage.ForeColor = System.Drawing.Color.Yellow; }

            if (supFlow.VerticalScroll.Value != 0 && supFlow.VerticalScroll.Value + supFlow.ClientSize.Height >= supFlow.VerticalScroll.Maximum)
            {
                if (panel1.Visible)
                {
                    panel1.Visible = false;
                    supFlow.Location = new System.Drawing.Point(supFlow.Location.X, supFlow.Location.Y - 150); // Change the X and Y coordinates
                    supFlow.Size = new System.Drawing.Size(supFlow.Width, supFlow.Height + 150); // Change the width and height
                }

                if (btnPublicMessage.BackColor == System.Drawing.Color.Blue)
                {
                    int startNum = numMessagesDisplayed;
                    RefreshSupMessages();

                    if (numMessagesDisplayed == startNum + 10)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            supFlow.AutoScrollPosition = new Point(0, 10);
                        });
                    }

                }
                else
                {

                    if (btnCommunityFeed.BackColor == System.Drawing.Color.Blue)
                    {
                        int startNum = numFriendFeedsDisplayed;
                        RefreshCommunityMessages();

                        if (numFriendFeedsDisplayed == startNum + 10)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                supFlow.AutoScrollPosition = new Point(0, 10);
                            });
                        }
                    }
                }
            }
            else if (supFlow.VerticalScroll.Value == 0)
            {
                if (!panel1.Visible && btnCommunityFeed.BackColor != System.Drawing.Color.Blue)
                {
                    panel1.Visible = true;
                    supFlow.Location = new System.Drawing.Point(supFlow.Location.X, supFlow.Location.Y + 150); // Change the X and Y coordinates
                    supFlow.Size = new System.Drawing.Size(supFlow.Width, supFlow.Height - 150); // Change the width and height
                }






                if (btnPublicMessage.BackColor == System.Drawing.Color.Blue)
                {

                    if (numMessagesDisplayed > 10)
                    {
                        numMessagesDisplayed = numMessagesDisplayed - 20; if (numMessagesDisplayed < 0) { numMessagesDisplayed = 0; }
                        else
                        {
                            RefreshSupMessages();
                            this.Invoke((MethodInvoker)delegate
                        {
                            supFlow.AutoScrollPosition = new Point(0, supFlow.VerticalScroll.Maximum - 10);

                        });
                        }
                    }
                }
                else
                {

                    if (btnCommunityFeed.BackColor == System.Drawing.Color.Blue)
                    {
                        if (numFriendFeedsDisplayed > 10)
                        {
                            numFriendFeedsDisplayed = numFriendFeedsDisplayed - 20; if (numFriendFeedsDisplayed < 0) { numFriendFeedsDisplayed = 0; }
                            else
                            {
                                RefreshCommunityMessages();
                                this.Invoke((MethodInvoker)delegate
                                {
                                    supFlow.AutoScrollPosition = new Point(0, supFlow.VerticalScroll.Maximum - 10);

                                });
                            }
                        }
                    }
                }
            }

        }

        private void supPrivateFlow_MouseWheel(object sender, MouseEventArgs e)
        {
            if (btnCommunityFeed.Enabled == false || btnPublicMessage.Enabled == false || btnPrivateMessage.Enabled == false) { return; }

            if (btnPublicMessage.BackColor == System.Drawing.Color.Blue) { btnPublicMessage.BackColor = System.Drawing.Color.White; btnPublicMessage.ForeColor = System.Drawing.Color.Black; btnPrivateMessage.BackColor = System.Drawing.Color.Blue; btnPrivateMessage.ForeColor = System.Drawing.Color.Yellow; }


            if (supPrivateFlow.VerticalScroll.Value + supPrivateFlow.ClientSize.Height >= supPrivateFlow.VerticalScroll.Maximum)
            {
                int startNum = numPrivateMessagesDisplayed;
                RefreshPrivateSupMessages();

                if (numPrivateMessagesDisplayed == startNum + 10)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        supPrivateFlow.AutoScrollPosition = new Point(0, 10);
                    });
                }
            }
            else if (supPrivateFlow.VerticalScroll.Value == 0)
            {
                if (numPrivateMessagesDisplayed > 10)
                {
                    numPrivateMessagesDisplayed = numPrivateMessagesDisplayed - 20; if (numPrivateMessagesDisplayed < 0) { numPrivateMessagesDisplayed = 0; }
                    else
                    {
                        RefreshPrivateSupMessages();
                        this.Invoke((MethodInvoker)delegate
                        {
                            supPrivateFlow.AutoScrollPosition = new Point(0, supPrivateFlow.VerticalScroll.Maximum - 10);

                        });
                    }
                }
            }
        }

        private void SupMaincs_Load(object sender, EventArgs e)
        {
            if (Directory.Exists("root")) { lblAdultsOnly.Visible = false; }
            else
            {
                try { Directory.CreateDirectory("root\\sig"); } catch { }
            }


            if (File.Exists("LIVE_MONITOR_ENABLED")) { btnLive.PerformClick(); }


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

            try { File.Delete(@"ROOTS-PROCESSING"); } catch { }


            OBcontrol.Dock = DockStyle.Fill;
            OBcontrol.ProfileURNChanged += OBControl_ProfileURNChanged;
            splitContainer1.Panel2.Controls.Add(OBcontrol);
            // Combine the application's startup path with the relative path
            string anonImageUrl = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "includes\\anon.png");
            // Load the image
            profileIMG.ImageLocation = anonImageUrl;
            profileOwner.ImageLocation = anonImageUrl;
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
            try
            {
                if (sender is ObjectBrowserControl objectBrowserControl && !friendClicked)
                {
                    var objectBrowserForm = objectBrowserControl.Controls[0].Controls[0] as ObjectBrowser;
                    if (objectBrowserForm != null)
                    {
                        if (!panel1.Visible)
                        {
                            panel1.Visible = true;
                            supFlow.Location = new System.Drawing.Point(supFlow.Location.X, supFlow.Location.Y + 150); // Change the X and Y coordinates
                            supFlow.Size = new System.Drawing.Size(supFlow.Width, supFlow.Height - 150); // Change the width and height
                        }

                        profileURN.Links[0].LinkData = objectBrowserForm.profileURN.Links[0].LinkData;
                        profileURN.Text = objectBrowserForm.profileURN.Text;
                        numMessagesDisplayed = 0;
                        numPrivateMessagesDisplayed = 0;
                        numFriendFeedsDisplayed = 0;

                        if (File.Exists(@"root\" + profileURN.Links[0].LinkData.ToString() + @"\MUTE")) { btnMute.Text = "unmute"; }

                        if (profileURN.Text.StartsWith("#"))
                        {
                            lblOfficial.Visible = false;
                            profileBIO.Text = "Click the follow button to add this search to your community feed."; profileCreatedDate.Text = ""; profileIMG.ImageLocation = ""; lblProcessHeight.Text = "";

                            GenerateImage(profileURN.Text);

                            string hashedString = "";
                            // get a hash of the text for image storage.
                            byte[] bytes = Encoding.ASCII.GetBytes(profileURN.Text);

                            // Create a SHA256 hash object
                            using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
                            {
                                // Compute hash value from the input
                                byte[] hashBytes = sha256Hash.ComputeHash(bytes);

                                // Convert byte array to a string representation
                                StringBuilder stringBuilder = new StringBuilder();
                                for (int i = 0; i < hashBytes.Length; i++)
                                {
                                    stringBuilder.Append(hashBytes[i].ToString("x2"));
                                }
                                hashedString = stringBuilder.ToString();

                            }
                            profileIMG.ImageLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\keywords\" + hashedString + ".png";
                            btnPublicMessage.BackColor = Color.Blue;
                            btnPublicMessage.ForeColor = Color.Yellow;
                            this.Invoke((Action)(() =>
                            {
                                ClearMessages(supFlow);
                            }));

                            RefreshSupMessages();
                        }
                        else
                        {
                            MakeActiveProfile(objectBrowserForm.profileURN.Links[0].LinkData.ToString());
                            btnPublicMessage.BackColor = Color.White;
                            btnPublicMessage.ForeColor = Color.Black;


                            if (profileURN.Links[0].LinkData != null)
                            {

                                string profileURN = objectBrowserForm.profileURN.Links[0].LinkData.ToString();

                                if (profileURN != null)
                                {

                                    NetworkCredential credentials = new NetworkCredential(mainnetLogin, mainnetPassword);
                                    NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);
                                    string signature = "";
                                    try { signature = rpcClient.SendCommand("signmessage", profileURN, "DUMMY").ResultString; ; } catch { }


                                   if
                                        ( signature != "")
                                    {
                                        profileOwner.ImageLocation = profileIMG.ImageLocation;
                                        profileOwner.Tag = objectBrowserForm.profileURN.Links[0].LinkData.ToString();
                                    }

                                }
                            }
                        }

                    }
                }
                friendClicked = false;
            }
            catch { }
        }

        private void MakeActiveProfile(string address)
        {

            if (!btnPrivateMessage.Enabled || !btnPublicMessage.Enabled || !btnCommunityFeed.Enabled || System.IO.File.Exists("ROOTS-PROCESSING"))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            lblOfficial.Visible = false;

            this.Invoke((Action)(() =>
            {
                ClearMessages(supFlow);
                ClearMessages(supPrivateFlow);
            }));

            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            PROState activeProfile = PROState.GetProfileByAddress(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
            if (activeProfile.URN == null)
            {
                profileURN.Text = "anon"; profileBIO.Text = ""; profileCreatedDate.Text = "";
                string errorImageUrl = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "includes\\anon.png");

                profileIMG.ImageLocation = errorImageUrl;
                lblProcessHeight.Text = ""; return;

            }

            profileBIO.Text = activeProfile.Bio;
            profileURN.Text = activeProfile.URN;
            profileURN.Links[0].LinkData = address;
            profileIMG.Tag = address;

            Task.Run(() =>
            {
                OBJState ownedProfile = OBJState.GetObjectByURN(activeProfile.Image, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                if (ownedProfile.Owners != null && ownedProfile.Owners.TryGetValue(address, out (long, string) owned) && owned.Item1 > 0)
                {
                    this.Invoke((Action)(() =>
                    {

                        lblOfficial.Visible = true;
                    }));
                }
            });

            if (File.Exists(@"root\" + profileURN.Links[0].LinkData.ToString() + @"\MUTE")) { btnMute.Text = "unmute"; } else { btnMute.Text = "mute"; }

            if (!panel1.Visible)
            {
                panel1.Visible = true;
                supFlow.Location = new System.Drawing.Point(supFlow.Location.X, supFlow.Location.Y + 150); // Change the X and Y coordinates
                supFlow.Size = new System.Drawing.Size(supFlow.Width, supFlow.Height - 150); // Change the width and height
            }


            lblProcessHeight.Text = "ph: " + activeProfile.Id.ToString();
            profileCreatedDate.Text = "since: " + activeProfile.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt");

            if (activeProfile.URL != null)
            {


                if (!friendClicked)
                {

                    //supPrivateFlow.Controls.Clear();
                    foreach (string key in activeProfile.URL.Keys)
                    {
                        Panel panel = new Panel();
                        panel.BackColor = Color.Transparent;
                        panel.Width = supFlow.Width - 30; // Set the fixed width
                        panel.Height = 50;

                        Label label = new Label();
                        label.Text = key;
                        label.Font = new Font("Segoe UI", 18, FontStyle.Bold); // Adjust font size
                        label.ForeColor = Color.White;
                        label.BackColor = Color.Transparent;
                        label.TextAlign = ContentAlignment.MiddleCenter; // Center the text within the label
                        label.Dock = DockStyle.Fill; // Fill the entire space within the panel
                        label.Click += new EventHandler((sender, e) => button_Click(sender, e, activeProfile.URL[key]));
                        // Add a border to the panel
                        panel.BorderStyle = BorderStyle.FixedSingle;


                        // Handle mouse enter event
                        label.MouseEnter += (sender, e) =>
                        {
                            label.BackColor = Color.FromArgb(64, 64, 64);
                        };

                        // Handle mouse leave event
                        label.MouseLeave += (sender, e) =>
                        {
                            label.BackColor = Color.Transparent; // Change back to the original color when the mouse leaves
                        };


                        // Add the label to the panel
                        panel.Controls.Add(label);

                        // Add the panel to the container (supFlow)
                        supFlow.Controls.Add(panel);
                    }





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
                    string anonImageUrl = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "includes\\anon.png");
                    profileIMG.ImageLocation = anonImageUrl;
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

                        string anonImageUrl = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "includes\\anon.png");
                        profileIMG.ImageLocation = anonImageUrl;

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



                                    try
                                    {
                                        if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                    catch { }

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

                                Root.GetRootByTransactionId(transid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

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

            // panel1.ResumeLayout();

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

                if (value.ToUpper().StartsWith("@"))
                {

                    PROState profile = PROState.GetProfileByURN(value.Replace("@", ""), mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                    if (profile.Creators != null) { MakeActiveProfile(profile.Creators.First()); }

                }
                else
                {
                    if (value.StartsWith("#"))
                    {

                        string searchAddress = Root.GetPublicAddressByKeyword(value.Replace("#", ""), mainnetVersionByte);
                        MakeActiveProfile(searchAddress);
                        profileURN.Text = value;
                        profileURN.Links[0].LinkData = searchAddress;
                        profileBIO.Text = "Click the follow button to add this search to your community feed."; profileCreatedDate.Text = ""; profileIMG.ImageLocation = ""; lblProcessHeight.Text = "";

                        GenerateImage(value);

                        string hashedString = "";
                        // get a hash of the text for image storage.
                        byte[] bytes = Encoding.ASCII.GetBytes(value);

                        // Create a SHA256 hash object
                        using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
                        {
                            // Compute hash value from the input
                            byte[] hashBytes = sha256Hash.ComputeHash(bytes);

                            // Convert byte array to a string representation
                            StringBuilder stringBuilder = new StringBuilder();
                            for (int i = 0; i < hashBytes.Length; i++)
                            {
                                stringBuilder.Append(hashBytes[i].ToString("x2"));
                            }
                            hashedString = stringBuilder.ToString();

                        }

                        profileIMG.ImageLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\keywords\" + hashedString + ".png";
                        btnPublicMessage.BackColor = Color.Blue;
                        btnPublicMessage.ForeColor = Color.Yellow;

                        RefreshSupMessages();


                    }
                    else
                    {

                        ObjectBrowser browser = new ObjectBrowser(value);
                        browser.Show();
                    }
                }
            }
        }
        //GPT3
        private void btnMint_Click(object sender, EventArgs e)
        {

            numPrivateMessagesDisplayed = 0;
            btnPrivateMessage.BackColor = System.Drawing.Color.White;
            btnPrivateMessage.ForeColor = System.Drawing.Color.Black;
            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel2.Controls.Add(OBcontrol);


            if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel2Collapsed = false;

            }
            else
            {
                if (btnMint.Text == "💎")
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
                        ObjectMint mintform = new ObjectMint(testnet);
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
                        ProfileMint mintprofile = new ProfileMint("", testnet);
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
                else { btnMint.Text = "💎"; }

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

                try
                {
                    using (FileStream fs = File.Create(@"LIVE_MONITOR_ENABLED"))
                    {

                    }
                }
                catch { }

                string walletUsername = mainnetLogin;
                string walletPassword = mainnetPassword;
                string walletUrl = @"http://127.0.0.1:18332";
                NetworkCredential credentials = new NetworkCredential(walletUsername, walletPassword);
                NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);


                Task.Run(() =>
                {
                    try
                    {
                        walletUrl = @"http://127.0.0.1:8332";
                        rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
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
                        rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                        string isActive = rpcClient.GetBalance().ToString();
                        if (decimal.TryParse(isActive, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal _))
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
                        rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                        string isActive = rpcClient.GetBalance().ToString();
                        if (decimal.TryParse(isActive, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal _))
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
                        rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                        string isActive = rpcClient.GetBalance().ToString();
                        if (decimal.TryParse(isActive, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal _))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                dogActive = true;
                            });

                        }
                    }
                    catch { }
                });

                if (File.Exists("IPFS_PINNING_ENABLED"))
                {
                    string ipfsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    // Set the environment variable IPFS_PATH to the same directory
                    Environment.SetEnvironmentVariable("IPFS_PATH", ipfsDir + @"\ipfs");

                    // Create the process to initialize the repo
                    var init = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = @"ipfs\ipfs.exe",
                            Arguments = "init",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    init.Start();
                    init.WaitForExit();
                    // Set the cache location to the same directory
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = @"ipfs\ipfs.exe",
                            Arguments = $"daemon --repo-dir {ipfsDir + @"\ipfs"}",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                }

                tmrSearchMemoryPool.Enabled = true;

            }
            else
            {
                btnLive.BackColor = Color.White;
                btnLive.ForeColor = Color.Black;
                tmrSearchMemoryPool.Enabled = false;


                try { File.Delete(@"LIVE_MONITOR_ENABLED"); } catch { }


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
                            string profileowner = "";
                            if (profileOwner.Tag != null) { profileowner = profileOwner.Tag.ToString(); }
                            FoundObjectControl foundObject = new FoundObjectControl(profileowner, testnet);

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
                            }


                            foundObject.SuspendLayout();
                            foundObject.ObjectName.Text = objstate.Name;
                            foundObject.ObjectDescription.Text = objstate.Description;
                            foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                            foundObject.ObjectQty.Text = objstate.Owners.Values.Sum(tuple => tuple.Item1).ToString() + "x";
                            foundObject.ObjectId.Text = objstate.TransactionId;

                            string imgurn = "";


                            if (objstate.Image != null)
                            {
                                imgurn = objstate.Image;
                                if (!imgurn.Contains(@"root\keywords\"))
                                {
                                    if (!objstate.Image.ToLower().StartsWith("http"))
                                    {
                                        imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                                        if (objstate.Image.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); if (objstate.Image.Length == 51) { imgurn += @"\artifact"; } }
                                    }
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
                                        System.Drawing.Image originalImage = System.Drawing.Image.FromFile(imgurn);

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

                                            System.Drawing.Image resizedImage = new Bitmap(originalImage, newWidth, newHeight);
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
                        List<string> differenceQuery = new List<string>();
                        List<string> newtransactions = new List<string>();
                        string flattransactions;
                        OBJState isobject = new OBJState();

                        List<OBJState> foundobjects = new List<OBJState>();
                        NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                        NBitcoin.RPC.RPCClient rpcClient;
                        string myFriendsJson = "";
                        Dictionary<string, string> myFriends = new Dictionary<string, string>();

                        if (File.Exists(@"root\MyFriendList.Json"))
                        {
                            myFriendsJson = File.ReadAllText(@"root\MyFriendList.Json");
                            myFriends = JsonConvert.DeserializeObject<Dictionary<string, string>>(myFriendsJson);
                        }
                        string filter = "";


                        try
                        {
                            rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(@"http://127.0.0.1:18332"), Network.Main);
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

                                        Root root = Root.GetRootByTransactionId(s, mainnetLogin, mainnetPassword, @"http://127.0.0.1:18332", "111");
                                        if (root.Signed == true || (root.File != null && root.File.ContainsKey("SEC")))
                                        {

                                            if (!System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\BLOCK") && !System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\MUTE"))
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

                                                if (File.Exists(@"LIVE_FILTER_ENABLED")) { find = myFriends.ContainsKey(root.Keyword.Keys.Last()); }
                                                if (find && root.File.ContainsKey("SEC") && root.Keyword.ContainsKey(profileURN.Links[0].LinkData.ToString()))
                                                {
                                                    //enough delay so the in memory element data is populated
                                                    root = Root.GetRootByTransactionId(s, mainnetLogin, mainnetPassword, @"http://127.0.0.1:18332", "111");
                                                    Root[] addToRoot = Root.GetRootsByAddress(profileURN.Links[0].LinkData.ToString(), mainnetLogin, mainnetPassword, @"http://127.0.0.1:18332");//ADD SEC message to leveld DB with system date stamp and refresh supPrivate Screen if active provile.
                                                    root.Id = addToRoot.Max(x => x.Id) + 1;

                                                    // Convert addToRoot to a List
                                                    List<Root> addToRootList = new List<Root>(addToRoot);

                                                    // Add the root object to the list
                                                    addToRootList.Add(root);

                                                    // Convert the list back to an array if needed
                                                    addToRoot = addToRootList.ToArray();

                                                    try { Directory.CreateDirectory(@"root\" + profileURN.Links[0].LinkData.ToString()); } catch { }
                                                    var rootSerialized = JsonConvert.SerializeObject(addToRoot);
                                                    System.IO.File.WriteAllText(@"root\" + profileURN.Links[0].LinkData.ToString() + @"\" + "ROOTS.json", rootSerialized);

                                                    this.Invoke((MethodInvoker)delegate
                                                    {
                                                        numPrivateMessagesDisplayed = 0;

                                                        RefreshPrivateSupMessages();


                                                        if (splitContainer1.Panel2Collapsed)
                                                        {
                                                            splitContainer1.Panel2Collapsed = false;
                                                        }
                                                    });

                                                }

                                                if (find && root.Message.Count() > 0)
                                                {

                                                    string _from = root.SignedBy;
                                                    string _to = "";
                                                    if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.First(); } else { _to = root.Keyword.Keys.Last(); }

                                                    string _fromId = _from;

                                                    PROState Fromprofile = PROState.GetProfileByAddress(_fromId, mainnetLogin, mainnetPassword, @"http://127.0.0.1:18332", "111");

                                                    if (Fromprofile.URN != null)
                                                    { _fromId = Fromprofile.URN; }

                                                    string _toId = _to;

                                                    PROState Toprofile = PROState.GetProfileByAddress(_toId, mainnetLogin, mainnetPassword, @"http://127.0.0.1:18332", "111");

                                                    if (Toprofile.URN != null)
                                                    { _toId = Toprofile.URN; }

                                                    string _message = string.Join(" ", root.Message);
                                                    string imglocation = "";
                                                    string unfilteredmessage = _message;
                                                    _message = Regex.Replace(_message, "<<.*?>>", "");


                                                    this.Invoke((MethodInvoker)delegate
                                                    {
                                                        try { imglocation = myFriends[_to]; } catch { }
                                                        CreateRow(imglocation, _toId, _to, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", false, supFlow, true);
                                                        imglocation = "";
                                                        try { imglocation = myFriends[_from]; } catch { }
                                                        CreateRow(imglocation, _fromId, _from, DateTime.Now, _message, root.TransactionId, false, supFlow, true);

                                                    });

                                                    string pattern = "<<.*?>>";
                                                    MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                    foreach (Match match in matches)
                                                    {


                                                        string content = match.Value.Substring(2, match.Value.Length - 4);
                                                        if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int r) && !content.Trim().StartsWith("#"))
                                                        {

                                                            string imgurn = content;

                                                            if (!content.ToLower().StartsWith("http"))
                                                            {
                                                                imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                            }

                                                            string extension = Path.GetExtension(imgurn).ToLower();
                                                            List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4", ".mov", ".avi", ".wav", ".mp3" };


                                                            string vpattern = @"(?:youtu\.be/|youtube(?:-nocookie)?\.com/(?:[^/\n\s]*[/\n\s]*(?:v/|e(?:mbed)?/|.*[?&]v=))?)?([a-zA-Z0-9_-]{11})";
                                                            Match vmatch = Regex.Match(content, vpattern);


                                                            if (!imgExtensions.Contains(extension) && vmatch.Value.Length < 12)
                                                            {


                                                                try
                                                                {
                                                                    string html = "";
                                                                    WebClient client = new WebClient();
                                                                    // Create a WebClient object to fetch the webpage
                                                                    if (!content.ToLower().EndsWith(".zip"))
                                                                    {
                                                                        html = client.DownloadString(content.StripLeadingTrailingSpaces());
                                                                    }

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
                                                                            panel.Size = new Size(supFlow.Width - 50, 100);

                                                                            // Create a label for the title
                                                                            Label titleLabel = new Label();
                                                                            titleLabel.Text = title;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                            titleLabel.ForeColor = Color.White;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                                    pictureBox.Image = System.Drawing.Image.FromStream(memoryStream);
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
                                                                            panel.Size = new Size(supFlow.Width - 50, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                        panel.Size = new Size(supFlow.Width - 50, 30);

                                                                        // Create a label for the title
                                                                        LinkLabel titleLabel = new LinkLabel();
                                                                        titleLabel.Text = content;
                                                                        titleLabel.Links[0].LinkData = imgurn;
                                                                        titleLabel.Dock = DockStyle.Top;
                                                                        titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                        titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                        titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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


                                                                if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id))
                                                                {


                                                                    if ((vmatch.Success && !imgExtensions.Contains(extension)) || extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".wav" || extension == ".mp3")
                                                                    {
                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {
                                                                            AddMedia(content, false, true, true);
                                                                        });
                                                                    }
                                                                    else
                                                                    {


                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {
                                                                            AddImage(content, false, true);
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

                                                    padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 50));

                                                    this.Invoke((MethodInvoker)delegate
                                                    {
                                                        supFlow.Controls.Add(padding);
                                                    });


                                                }

                                                if (find && root.File.ContainsKey("PRO"))
                                                {
                                                    PRO profileinspector = null;
                                                    try
                                                    {
                                                        profileinspector = JsonConvert.DeserializeObject<PRO>(File.ReadAllText(@"root\" + root.TransactionId + @"\PRO"));

                                                        if (profileinspector != null)
                                                        {

                                                            // Create and add the welcome label
                                                            if (profileinspector.urn != null && profileinspector.cre != null && profileinspector.img != null)
                                                            {
                                                                this.Invoke((MethodInvoker)delegate
                                                                {
                                                                    AddProfile(profileinspector.img, profileinspector.urn, root.SignedBy);

                                                                });


                                                            }
                                                        }
                                                    }
                                                    catch {  }
                                                }


                                                if (find && root.File.ContainsKey("INQ"))
                                                {
                                                    INQState isINQ = new INQState();
                                                    isINQ = INQState.GetInquiryByTransactionId(s, mainnetLogin, mainnetPassword, @"http://127.0.0.1:18332", "111");

                                                    if (isINQ.TransactionId != null)
                                                    {

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            FoundINQControl foundObject = new FoundINQControl(s, "", true);
                                                            foundObject.Margin = new Padding(20, 7, 8, 7);
                                                            supFlow.Controls.Add(foundObject);
                                                            supFlow.Controls.SetChildIndex(foundObject, 2);
                                                        });
                                                    }

                                                }


                                                isobject = OBJState.GetObjectByTransactionId(s, mainnetLogin, mainnetPassword, @"http://127.0.0.1:18332", "111");
                                                if (isobject.URN != null && find == true)
                                                {
                                                    isobject.TransactionId = s;
                                                    foundobjects.Add(isobject);
                                                    try { Directory.Delete(@"root\" + s, true); } catch { }



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


                        if (btcActive)
                        {
                            newtransactions = new List<string>();
                            try
                            {
                                rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(@"http://127.0.0.1:8332"), Network.Main);
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

                                            Root root = Root.GetRootByTransactionId(s, mainnetLogin, mainnetPassword, @"http://127.0.0.1:8332", "0");
                                            if (root.Signed == true || (root.File != null && root.File.ContainsKey("SEC")))
                                            {

                                                if (!System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\BLOCK") && !System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\MUTE"))
                                                {
                                                    bool find = false;

                                                    if (filter != "")
                                                    {

                                                        if (filter.StartsWith("#"))
                                                        {
                                                            find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1), "0"));
                                                        }
                                                        else
                                                        {

                                                            find = root.Keyword.ContainsKey(filter);

                                                        }
                                                    }
                                                    else { find = true; }

                                                    if (File.Exists(@"LIVE_FILTER_ENABLED")) { find = myFriends.ContainsKey(root.Keyword.Keys.Last()); }
                                                    if (find && root.File.ContainsKey("SEC") && root.Keyword.ContainsKey(profileURN.Links[0].LinkData.ToString()))
                                                    {
                                                        //enough delay so the in memory element data is populated
                                                        root = Root.GetRootByTransactionId(s, mainnetLogin, mainnetPassword, @"http://127.0.0.1:8332", "0");
                                                        Root[] addToRoot = Root.GetRootsByAddress(profileURN.Links[0].LinkData.ToString(), mainnetLogin, mainnetPassword, @"http://127.0.0.1:8332", 0, -1, "0");//ADD SEC message to leveld DB with system date stamp and refresh supPrivate Screen if active provile.
                                                        root.Id = addToRoot.Max(x => x.Id) + 1;

                                                        // Convert addToRoot to a List
                                                        List<Root> addToRootList = new List<Root>(addToRoot);

                                                        // Add the root object to the list
                                                        addToRootList.Add(root);

                                                        // Convert the list back to an array if needed
                                                        addToRoot = addToRootList.ToArray();

                                                        try { Directory.CreateDirectory(@"root\" + profileURN.Links[0].LinkData.ToString()); } catch { }
                                                        var rootSerialized = JsonConvert.SerializeObject(addToRoot);
                                                        System.IO.File.WriteAllText(@"root\" + profileURN.Links[0].LinkData.ToString() + @"\" + "ROOTS.json", rootSerialized);

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            numPrivateMessagesDisplayed = 0;

                                                            RefreshPrivateSupMessages();


                                                            if (splitContainer1.Panel2Collapsed)
                                                            {
                                                                splitContainer1.Panel2Collapsed = false;
                                                            }
                                                        });

                                                    }

                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.First(); } else { _to = root.Keyword.Keys.Last(); }

                                                        string _fromId = _from;

                                                        PROState Fromprofile = PROState.GetProfileByAddress(_fromId, mainnetLogin, mainnetPassword, @"http://127.0.0.1:8332", "0");

                                                        if (Fromprofile.URN != null)
                                                        { _fromId = Fromprofile.URN; }

                                                        string _toId = _to;

                                                        PROState Toprofile = PROState.GetProfileByAddress(_toId, mainnetLogin, mainnetPassword, @"http://127.0.0.1:8332", "0");

                                                        if (Toprofile.URN != null)
                                                        { _toId = Toprofile.URN; }

                                                        string _message = string.Join(" ", root.Message);
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateRow(imglocation, _toId, _to, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", false, supFlow, true);
                                                            imglocation = "";
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateRow(imglocation, _fromId, _from, DateTime.Now, _message, root.TransactionId, false, supFlow, true);


                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int r) && !content.Trim().StartsWith("#"))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mov", ".avi", ".wav", ".mp3" };


                                                                string vpattern = @"(?:youtu\.be/|youtube(?:-nocookie)?\.com/(?:[^/\n\s]*[/\n\s]*(?:v/|e(?:mbed)?/|.*[?&]v=))?)?([a-zA-Z0-9_-]{11})";
                                                                Match vmatch = Regex.Match(content, vpattern);


                                                                if (!imgExtensions.Contains(extension) && vmatch.Value.Length < 12)
                                                                {


                                                                    try
                                                                    {
                                                                        string html = "";
                                                                        WebClient client = new WebClient();
                                                                        // Create a WebClient object to fetch the webpage
                                                                        if (!content.ToLower().EndsWith(".zip"))
                                                                        {
                                                                            html = client.DownloadString(content.StripLeadingTrailingSpaces());
                                                                        }

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
                                                                                panel.Size = new Size(supFlow.Width - 50, 100);

                                                                                // Create a label for the title
                                                                                Label titleLabel = new Label();
                                                                                titleLabel.Text = title;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                                titleLabel.ForeColor = Color.White;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                                        pictureBox.Image = System.Drawing.Image.FromStream(memoryStream);
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
                                                                                panel.Size = new Size(supFlow.Width - 50, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                            panel.Size = new Size(supFlow.Width - 50, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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


                                                                    if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id))
                                                                    {


                                                                        if ((vmatch.Success && !imgExtensions.Contains(extension)) || extension == ".mov" || extension == ".mp4" || extension == ".avi" || extension == ".wav" || extension == ".mp3")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                AddMedia(content, false, true, true);
                                                                            });
                                                                        }
                                                                        else
                                                                        {


                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                AddImage(content, false, true);
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

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 50));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            supFlow.Controls.Add(padding);
                                                        });


                                                    }

                                                    if (find && root.File.ContainsKey("PRO"))
                                                    {
                                                        PRO profileinspector = null;
                                                        try
                                                        {
                                                            profileinspector = JsonConvert.DeserializeObject<PRO>(File.ReadAllText(@"root\" + root.TransactionId + @"\PRO"));

                                                            if (profileinspector != null)
                                                            {

                                                                // Create and add the welcome label
                                                                if (profileinspector.urn != null && profileinspector.cre != null && profileinspector.img != null)
                                                                {
                                                                    this.Invoke((MethodInvoker)delegate
                                                                    {
                                                                        AddProfile(profileinspector.img, profileinspector.urn, root.SignedBy);

                                                                    });


                                                                }
                                                            }
                                                        }
                                                        catch { }
                                                    }


                                                    if (find && root.File.ContainsKey("INQ"))
                                                    {
                                                        INQState isINQ = new INQState();
                                                        isINQ = INQState.GetInquiryByTransactionId(s, mainnetLogin, mainnetPassword, @"http://127.0.0.1:8332", "0");

                                                        if (isINQ.TransactionId != null)
                                                        {

                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                FoundINQControl foundObject = new FoundINQControl(s, "", true);
                                                                foundObject.Margin = new Padding(20, 7, 8, 7);
                                                                supFlow.Controls.Add(foundObject);
                                                                supFlow.Controls.SetChildIndex(foundObject, 2);
                                                            });
                                                        }

                                                    }

                                                    isobject = OBJState.GetObjectByTransactionId(s, mainnetLogin, mainnetPassword, @"http://127.0.0.1:8332", "0");
                                                    if (isobject.URN != null && find == true)
                                                    {
                                                        isobject.TransactionId = s;
                                                        foundobjects.Add(isobject);
                                                        try { Directory.Delete(@"root\" + s, true); } catch { }



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

                        if (mzcActive)
                        {
                            newtransactions = new List<string>();

                            try
                            {
                                rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(@"http://127.0.0.1:12832"), Network.Main);
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
                                                if (!System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\BLOCK") && !System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\MUTE"))
                                                {
                                                    bool find = false;

                                                    if (filter != "")
                                                    {

                                                        if (filter.StartsWith("#"))
                                                        {
                                                            find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1), mainnetVersionByte));
                                                        }
                                                        else
                                                        {

                                                            find = root.Keyword.ContainsKey(filter);


                                                        }
                                                    }
                                                    else { find = true; }
                                                    if (File.Exists(@"LIVE_FILTER_ENABLED")) { find = myFriends.ContainsKey(root.SignedBy); }
                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.GetItemByIndex(root.Keyword.Count() - 2); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = string.Join(" ", root.Message);
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateRow(imglocation, _to, _to, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", false, supFlow, true);
                                                            imglocation = "";
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateRow(imglocation, _from, _from, DateTime.Now, _message, root.TransactionId, false, supFlow, true);
                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int r) && !content.Trim().StartsWith("#"))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4", ".mov", ".avi", ".wav", ".mp3" };
                                                                string vpattern = @"(?:youtu\.be/|youtube(?:-nocookie)?\.com/(?:[^/\n\s]*[/\n\s]*(?:v/|e(?:mbed)?/|.*[?&]v=))?)?([a-zA-Z0-9_-]{11})";
                                                                Match vmatch = Regex.Match(content, vpattern);
                                                                if (!imgExtensions.Contains(extension) && vmatch.Value.Length < 12)
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
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                                        pictureBox.Image = System.Drawing.Image.FromStream(memoryStream);
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
                                                                                panel.Size = new Size(supFlow.Width - 50, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                            panel.Size = new Size(supFlow.Width - 50, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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


                                                                    if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id))
                                                                    {
                                                                        if (vmatch.Success || extension == ".mov" || extension == ".mp4" || extension == ".avi" || extension == ".wav" || extension == ".mp3")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                AddMedia(content, false, true, true);
                                                                            });
                                                                        }
                                                                        else
                                                                        {


                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                AddImage(content, false, true);
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

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 50));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            supFlow.Controls.Add(padding);
                                                        });


                                                    }
                                                    if (find && root.File.ContainsKey("PRO"))
                                                    {
                                                        PRO profileinspector = null;
                                                        try
                                                        {
                                                            profileinspector = JsonConvert.DeserializeObject<PRO>(File.ReadAllText(@"root\" + root.TransactionId + @"\PRO"));

                                                            if (profileinspector != null)
                                                            {

                                                                // Create and add the welcome label
                                                                if (profileinspector.urn != null && profileinspector.cre != null && profileinspector.img != null)
                                                                {
                                                                    this.Invoke((MethodInvoker)delegate
                                                                    {
                                                                        AddProfile(profileinspector.img, profileinspector.urn, root.SignedBy);

                                                                    });


                                                                }
                                                            }
                                                        }
                                                        catch { }
                                                    }

                                                    if (find && root.File.ContainsKey("INQ"))
                                                    {
                                                        INQState isINQ = new INQState();
                                                        isINQ = INQState.GetInquiryByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                                        if (isINQ.TransactionId != null)
                                                        {

                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                FoundINQControl foundObject = new FoundINQControl(s, "", testnet);
                                                                foundObject.Margin = new Padding(20, 7, 8, 7);
                                                                supFlow.Controls.Add(foundObject);
                                                                supFlow.Controls.SetChildIndex(foundObject, 2);
                                                            });
                                                        }

                                                    }
                                                    isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                                    if (isobject.URN != null && find == true)
                                                    {

                                                        isobject.TransactionId = s;
                                                        foundobjects.Add(isobject);
                                                        try { Directory.Delete(@"root\" + s, true); } catch { }

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
                                rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(@"http://127.0.0.1:9332"), Network.Main);
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
                                                if (!System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\BLOCK") && !System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\MUTE"))
                                                {
                                                    bool find = false;

                                                    if (filter != "")
                                                    {

                                                        if (filter.StartsWith("#"))
                                                        {
                                                            find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1), mainnetVersionByte));
                                                        }
                                                        else
                                                        {

                                                            find = root.Keyword.ContainsKey(filter);


                                                        }
                                                    }
                                                    else { find = true; }
                                                    if (File.Exists(@"LIVE_FILTER_ENABLED")) { find = myFriends.ContainsKey(root.SignedBy); }
                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.GetItemByIndex(root.Keyword.Count() - 2); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = string.Join(" ", root.Message);
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateRow(imglocation, _to, _to, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", false, supFlow, true);
                                                            imglocation = "";
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateRow(imglocation, _from, _from, DateTime.Now, _message, root.TransactionId, false, supFlow, true);
                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int r) && !content.Trim().StartsWith("#"))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4", ".mov", ".avi", ".wav", ".mp3" };
                                                                string vpattern = @"(?:youtu\.be/|youtube(?:-nocookie)?\.com/(?:[^/\n\s]*[/\n\s]*(?:v/|e(?:mbed)?/|.*[?&]v=))?)?([a-zA-Z0-9_-]{11})";
                                                                Match vmatch = Regex.Match(content, vpattern);
                                                                if (!imgExtensions.Contains(extension) && vmatch.Value.Length < 12)
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
                                                                                panel.Size = new Size(supFlow.Width - 50, 100);

                                                                                // Create a label for the title
                                                                                Label titleLabel = new Label();
                                                                                titleLabel.Text = title;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                                titleLabel.ForeColor = Color.White;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                                        pictureBox.Image = System.Drawing.Image.FromStream(memoryStream);
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
                                                                                panel.Size = new Size(supFlow.Width - 50, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                            panel.Size = new Size(supFlow.Width - 50, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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


                                                                    if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id))
                                                                    {
                                                                        if (vmatch.Success || extension == ".mov" || extension == ".mp4" || extension == ".avi" || extension == ".wav" || extension == ".mp3")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                AddMedia(content, false, true, true);
                                                                            });
                                                                        }
                                                                        else
                                                                        {


                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                AddImage(content, false, true);
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

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 50));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            supFlow.Controls.Add(padding);
                                                        });


                                                    }

                                                    if (find && root.File.ContainsKey("PRO"))
                                                    {
                                                        PRO profileinspector = null;
                                                        try
                                                        {
                                                            profileinspector = JsonConvert.DeserializeObject<PRO>(File.ReadAllText(@"root\" + root.TransactionId + @"\PRO"));

                                                            if (profileinspector != null)
                                                            {

                                                                // Create and add the welcome label
                                                                if (profileinspector.urn != null && profileinspector.cre != null && profileinspector.img != null)
                                                                {
                                                                    this.Invoke((MethodInvoker)delegate
                                                                    {
                                                                        AddProfile(profileinspector.img, profileinspector.urn, root.SignedBy);

                                                                    });


                                                                }
                                                            }
                                                        }
                                                        catch { }
                                                    }


                                                    if (find && root.File.ContainsKey("INQ"))
                                                    {
                                                        INQState isINQ = new INQState();
                                                        isINQ = INQState.GetInquiryByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                                        if (isINQ.TransactionId != null)
                                                        {

                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                FoundINQControl foundObject = new FoundINQControl(s, "", testnet);
                                                                foundObject.Margin = new Padding(20, 7, 8, 7);
                                                                supFlow.Controls.Add(foundObject);
                                                                supFlow.Controls.SetChildIndex(foundObject, 2);
                                                            });
                                                        }

                                                    }
                                                    isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                                    if (isobject.URN != null && find == true)
                                                    {
                                                        isobject.TransactionId = s;
                                                        foundobjects.Add(isobject);
                                                        try { Directory.Delete(@"root\" + s, true); } catch { }


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
                                rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(@"http://127.0.0.1:22555"), Network.Main);
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

                                                if (!System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\BLOCK") && !System.IO.File.Exists(@"root\" + root.Keyword.Keys.Last() + @"\MUTE"))
                                                {
                                                    bool find = false;

                                                    if (filter.Length > 0)
                                                    {

                                                        if (filter.StartsWith("#"))
                                                        {
                                                            find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1), mainnetVersionByte));
                                                        }
                                                        else
                                                        {

                                                            find = root.Keyword.ContainsKey(filter);


                                                        }
                                                    }
                                                    else { find = true; }
                                                    if (File.Exists(@"LIVE_FILTER_ENABLED")) { find = myFriends.ContainsKey(root.SignedBy); }
                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.GetItemByIndex(root.Keyword.Count() - 2); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = string.Join(" ", root.Message);
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateRow(imglocation, _to, _to, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", false, supFlow, true);

                                                            imglocation = "";
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateRow(imglocation, _from, _from, DateTime.Now, _message, root.TransactionId, false, supFlow, true);
                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int r) && !content.Trim().StartsWith("#"))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4", ".mov", ".avi", ".wav", ".mp3" };
                                                                string vpattern = @"(?:youtu\.be/|youtube(?:-nocookie)?\.com/(?:[^/\n\s]*[/\n\s]*(?:v/|e(?:mbed)?/|.*[?&]v=))?)?([a-zA-Z0-9_-]{11})";
                                                                Match vmatch = Regex.Match(content, vpattern);
                                                                if (!imgExtensions.Contains(extension) && vmatch.Value.Length < 12)
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
                                                                                panel.Size = new Size(supFlow.Width - 50, 100);

                                                                                // Create a label for the title
                                                                                Label titleLabel = new Label();
                                                                                titleLabel.Text = title;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                                titleLabel.ForeColor = Color.White;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                                        pictureBox.Image = System.Drawing.Image.FromStream(memoryStream);
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
                                                                                panel.Size = new Size(supFlow.Width - 50, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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
                                                                            panel.Size = new Size(supFlow.Width - 50, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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


                                                                    if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id))
                                                                    {
                                                                        if (vmatch.Success || extension == ".mov" || extension == ".mp4" || extension == ".avi" || extension == ".wav" || extension == ".mp3")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                AddMedia(content, false, true, true);
                                                                            });
                                                                        }
                                                                        else
                                                                        {


                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                AddImage(content, false, true);
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

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 50));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            supFlow.Controls.Add(padding);
                                                        });


                                                    }


                                                    if (find && root.File.ContainsKey("PRO"))
                                                    {
                                                        PRO profileinspector = null;
                                                        try
                                                        {
                                                            profileinspector = JsonConvert.DeserializeObject<PRO>(File.ReadAllText(@"root\" + root.TransactionId + @"\PRO"));

                                                            if (profileinspector != null)
                                                            {

                                                                // Create and add the welcome label
                                                                if (profileinspector.urn != null && profileinspector.cre != null && profileinspector.img != null)
                                                                {
                                                                    this.Invoke((MethodInvoker)delegate
                                                                    {
                                                                        AddProfile(profileinspector.img, profileinspector.urn, root.SignedBy);

                                                                    });


                                                                }
                                                            }
                                                        }
                                                        catch { }
                                                    }


                                                    if (find && root.File.ContainsKey("INQ"))
                                                    {
                                                        INQState isINQ = new INQState();
                                                        isINQ = INQState.GetInquiryByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                                        if (isINQ.TransactionId != null)
                                                        {

                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                FoundINQControl foundObject = new FoundINQControl(s, "", testnet);
                                                                foundObject.Margin = new Padding(20, 7, 8, 7);
                                                                supFlow.Controls.Add(foundObject);
                                                                supFlow.Controls.SetChildIndex(foundObject, 2);
                                                            });
                                                        }

                                                    }
                                                    isobject = OBJState.GetObjectByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                                    if (isobject.URN != null && find == true)
                                                    {
                                                        isobject.TransactionId = s;
                                                        foundobjects.Add(isobject);
                                                        try { Directory.Delete(@"root\" + s, true); } catch { }


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

        private void RemoveOverFlowMessages(FlowLayoutPanel flowLayoutPanel)
        {

            // Iterate through the controls in the FlowLayoutPanel
            List<Control> controlsList = flowLayoutPanel.Controls.Cast<Control>().ToList();
            foreach (Control control in controlsList)
            {

                Task memoryPrune = Task.Run(() =>
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        flowLayoutPanel.Controls.Remove(control);
                        control.Dispose();
                    });
                });

            }
        }

        private void ClearMessages(FlowLayoutPanel flowLayoutPanel)
        {

            this.BeginInvoke((MethodInvoker)delegate
            {
                List<Control> controlsList = flowLayoutPanel.Controls.Cast<Control>().ToList();
                flowLayoutPanel.Controls.Clear();

                foreach (Control control in controlsList)
                {

                    control.Dispose();
                }
                
                // Reset tracking sets based on which panel is being cleared
                if (flowLayoutPanel == supPrivateFlow)
                {
                    displayedPrivateMessageIds.Clear();
                }
                else if (flowLayoutPanel == supFlow)
                {
                    // Both public and community messages share the same supFlow panel
                    // Clear both sets to ensure clean state regardless of which view was active
                    // This is intentionally broad - the cost is just re-rendering already-seen messages
                    // which is better than showing duplicates from stale tracking state
                    displayedPublicMessageIds.Clear();
                    displayedCommunityMessageIds.Clear();
                }

            });


        }

        private void RefreshSupMessages()
        {

            // sorry cannot run two searches at a time
            if (!btnPrivateMessage.Enabled || !btnPrivateMessage.Enabled || !btnCommunityFeed.Enabled || System.IO.File.Exists("ROOTS-PROCESSING"))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            // Check if profileURN has valid LinkData before proceeding
            if (profileURN.Links.Count == 0 || profileURN.Links[0].LinkData == null)
            {
                supFlow.ResumeLayout();
                btnPublicMessage.Enabled = true;
                return;
            }

            btnPublicMessage.Enabled = false;

            List<MessageObject> messages = new List<MessageObject>();

            try { messages = OBJState.GetPublicMessagesByAddress(profileURN.Links[0].LinkData.ToString(), mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, numMessagesDisplayed, 10); }
            catch { }

            // Normalize and deduplicate messages
            var normalizedMessages = MessageNormalizer.Normalize(messages);
            
            // Filter out messages we've already displayed
            var newMessages = normalizedMessages.Where(m => !displayedPublicMessageIds.Contains(m.TransactionId)).ToList();

            supFlow.SuspendLayout();

            // Trigger memory cleanup if we received a full page from server (indicates more to load)
            // Check original message count, not filtered count, since filtering may reduce the count
            if (messages.Count == 10)
            {
                Task memoryPrune = Task.Run(() =>
                {

                    RemoveOverFlowMessages(supFlow);

                });
            }

            numFriendFeedsDisplayed = 0;
            numPrivateMessagesDisplayed = 0;



            Dictionary<string, string[]> profileAddress = new Dictionary<string, string[]> { };

            foreach (var normalizedMessage in newMessages)
            {
                // Mark this message as displayed to prevent duplicates
                displayedPublicMessageIds.Add(normalizedMessage.TransactionId);
                numMessagesDisplayed++;
                
                // Convert back to MessageObject for rendering
                MessageObject messagePacket = new MessageObject
                {
                    TransactionId = normalizedMessage.TransactionId,
                    Message = normalizedMessage.Message,
                    FromAddress = normalizedMessage.FromAddress,
                    ToAddress = normalizedMessage.ToAddress,
                    BlockDate = normalizedMessage.BlockDate
                };

                string message = "";
                string tid = messagePacket.TransactionId.ToString();
                string tstamp = messagePacket.BlockDate.ToString("yyyyMMddHHmmss");
                try
                {


                    message = messagePacket.Message.ToString();

                    string relativeFolderPath = @"root\" + tid;
                    string folderPath = Path.Combine(Environment.CurrentDirectory, relativeFolderPath);

                    string[] files = Directory.GetFiles(folderPath);

                    foreach (string file in files)
                    {
                        string extension = Path.GetExtension(file);

                        if (Path.GetFileName(file) == "INQ" || (!string.IsNullOrEmpty(extension) && !file.Contains("ROOT.json") && !file.EndsWith("-thumbnail.jpg")))
                        {
                            message = message + @"<<" + tid + @"/" + Path.GetFileName(file) + ">>";
                        }

                    }

                    string fromAddress = messagePacket.FromAddress;
                    string toAddress = messagePacket.ToAddress;
                    string fromImage = "";
                    string toImage = "";


                    string unfilteredmessage = message;
                    string[] blocks = Regex.Matches(message, "<<[^<>]+>>")
                                             .Cast<Match>()
                                             .Select(m => m.Value.Trim(new char[] { '<', '>' }))
                                             .ToArray();
                    message = Regex.Replace(message, "<<.*?>>", "");


                    if (message != "" || blocks.Length > 1 || (blocks.Length == 1 && !int.TryParse(blocks[0], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out _)))
                    {

                        if (!profileAddress.ContainsKey(fromAddress))
                        {

                            PROState profile = PROState.GetProfileByAddress(fromAddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                            if (profile.URN != null)
                            {
                                fromAddress = TruncateAddress(profile.URN);

                                if (profile.Image != null)
                                {
                                    fromImage = profile.Image;


                                    if (!profile.Image.ToLower().StartsWith("http"))
                                    {
                                        fromImage = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + profile.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                                        if (profile.Image.ToLower().StartsWith("ipfs:")) { fromImage = fromImage.Replace(@"\root\", @"\ipfs\"); if (profile.Image.Length == 51) { fromImage += @"\artifact"; } }
                                    }
                                    Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                                    Match imgurnmatch = regexTransactionId.Match(fromImage);
                                    string transactionid = imgurnmatch.Value;
                                    Root root = new Root();
                                    if (!File.Exists(fromImage))
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
                                                    try
                                                    {
                                                        Directory.CreateDirectory("ipfs/" + transid);
                                                    }
                                                    catch { };

                                                    Directory.CreateDirectory("ipfs/" + transid + "-build");
                                                    Process process2 = new Process();
                                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                    process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                    process2.StartInfo.UseShellExecute = false;
                                                    process2.StartInfo.CreateNoWindow = true;
                                                    process2.Start();
                                                    if (process2.WaitForExit(5000))
                                                    {
                                                        string fileName;
                                                        if (System.IO.File.Exists("ipfs/" + transid))
                                                        {
                                                            System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                            System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                            fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                            Directory.CreateDirectory("ipfs/" + transid);
                                                            System.IO.File.Move("ipfs/" + transid + "_tmp", fromImage);
                                                        }

                                                        if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                        {
                                                            fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                            System.IO.File.Move("ipfs/" + transid + "/" + transid, fromImage);
                                                        }

                                                        try
                                                        {
                                                            if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                                        catch { }

                                                        try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                                    }
                                                    else
                                                    {
                                                        process2.Kill();

                                                        Task.Run(() =>
                                                        {
                                                            process2 = new Process();
                                                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                            process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                            process2.StartInfo.UseShellExecute = false;
                                                            process2.StartInfo.CreateNoWindow = true;
                                                            process2.Start();
                                                            if (process2.WaitForExit(550000))
                                                            {
                                                                string fileName;
                                                                if (System.IO.File.Exists("ipfs/" + transid))
                                                                {
                                                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                                    Directory.CreateDirectory("ipfs/" + transid);
                                                                    System.IO.File.Move("ipfs/" + transid + "_tmp", fromImage);
                                                                }

                                                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                                {
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, fromImage);
                                                                }

                                                                try
                                                                {
                                                                    if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                                                catch { }

                                                                try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }


                                                            }
                                                            else
                                                            {
                                                                process2.Kill();
                                                            }
                                                        });

                                                    }


                                                }

                                                break;
                                            default:
                                                if (!profile.Image.ToUpper().StartsWith("HTTP") && transactionid != "")
                                                {
                                                    root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

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
                            profilePacket[1] = fromImage;
                            profileAddress.Add(messagePacket.FromAddress, profilePacket);

                        }
                        else
                        {
                            string[] profilePacket = new string[] { };
                            profileAddress.TryGetValue(fromAddress, out profilePacket);
                            fromAddress = profilePacket[0];
                            fromImage = profilePacket[1];

                        }


                        if (!profileAddress.ContainsKey(toAddress))
                        {

                            PROState profile = PROState.GetProfileByAddress(toAddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                            if (profile.URN != null)
                            {
                                toAddress = TruncateAddress(profile.URN);

                                if (profile.Image != null)
                                {
                                    toImage = profile.Image;


                                    if (!profile.Image.ToLower().StartsWith("http"))
                                    {
                                        toImage = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + profile.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                                        if (profile.Image.ToLower().StartsWith("ipfs:")) { toImage = toImage.Replace(@"\root\", @"\ipfs\"); if (profile.Image.Length == 51) { toImage += @"\artifact"; } }
                                    }
                                    Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                                    Match imgurnmatch = regexTransactionId.Match(toImage);
                                    string transactionid = imgurnmatch.Value;
                                    Root root = new Root();
                                    if (!File.Exists(toImage))
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
                                                    try
                                                    {
                                                        Directory.CreateDirectory("ipfs/" + transid);
                                                    }
                                                    catch { };

                                                    Directory.CreateDirectory("ipfs/" + transid + "-build");
                                                    Process process2 = new Process();
                                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                    process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                    process2.StartInfo.UseShellExecute = false;
                                                    process2.StartInfo.CreateNoWindow = true;
                                                    process2.Start();
                                                    if (process2.WaitForExit(5000))
                                                    {
                                                        string fileName;
                                                        if (System.IO.File.Exists("ipfs/" + transid))
                                                        {
                                                            System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                            System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                            fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                            Directory.CreateDirectory("ipfs/" + transid);
                                                            System.IO.File.Move("ipfs/" + transid + "_tmp", toImage);
                                                        }

                                                        if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                        {
                                                            fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                            System.IO.File.Move("ipfs/" + transid + "/" + transid, toImage);
                                                        }

                                                        try
                                                        {
                                                            if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                                        catch { }

                                                        try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                                    }
                                                    else
                                                    {
                                                        process2.Kill();

                                                        Task.Run(() =>
                                                        {
                                                            process2 = new Process();
                                                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                            process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                            process2.StartInfo.UseShellExecute = false;
                                                            process2.StartInfo.CreateNoWindow = true;
                                                            process2.Start();
                                                            if (process2.WaitForExit(550000))
                                                            {
                                                                string fileName;
                                                                if (System.IO.File.Exists("ipfs/" + transid))
                                                                {
                                                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                                    Directory.CreateDirectory("ipfs/" + transid);
                                                                    System.IO.File.Move("ipfs/" + transid + "_tmp", toImage);
                                                                }

                                                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                                {
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, toImage);
                                                                }

                                                                try
                                                                {
                                                                    if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                                                catch { }

                                                                try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }


                                                            }
                                                            else
                                                            {
                                                                process2.Kill();
                                                            }
                                                        });

                                                    }


                                                }

                                                break;
                                            default:
                                                if (!profile.Image.ToUpper().StartsWith("HTTP") && transactionid != "")
                                                {
                                                    root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                                }
                                                break;
                                        }
                                    }



                                }


                            }
                            else
                            { toAddress = TruncateAddress(toAddress); }

                            string[] profilePacket = new string[2];

                            profilePacket[0] = toAddress;
                            profilePacket[1] = toImage;
                            profileAddress.Add(messagePacket.ToAddress, profilePacket);

                        }
                        else
                        {
                            string[] profilePacket = new string[] { };
                            profileAddress.TryGetValue(toAddress, out profilePacket);
                            toAddress = profilePacket[0];
                            toImage = profilePacket[1];

                        }



                        System.Drawing.Color bgcolor = System.Drawing.Color.White;



                        CreateRow(fromImage, fromAddress, messagePacket.FromAddress, DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, tid, false, supFlow);
                        CreateRow(toImage, toAddress, messagePacket.ToAddress, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), "", tid, false, supFlow);

                        bool containsFileWithINQ = files.Any(file =>
                        file.EndsWith("INQ", StringComparison.OrdinalIgnoreCase) &&
                        !file.EndsWith("BLOCK", StringComparison.OrdinalIgnoreCase));

                        if (containsFileWithINQ)
                        {
                            //ADD INQ IF IT EXISTS AND IS NOT BLOCKED

                            string profileowner = "";

                            if (profileOwner.Tag != null) { profileowner = profileOwner.Tag.ToString(); }
                            FoundINQControl foundObject = new FoundINQControl(messagePacket.TransactionId, profileowner, testnet);

                            foundObject.Margin = new Padding(20, 7, 8, 7);
                            supFlow.Controls.Add(foundObject);

                        }

                        string pattern = "<<.*?>>";
                        List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4",".mov" , ".avi", ".wav", ".mp3" };

                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                        foreach (Match match in matches)
                        {


                            string content = match.Value.Substring(2, match.Value.Length - 4);

                            if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int cnt) && !content.Trim().StartsWith("#") && !content.EndsWith(@"/INQ"))
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


                                    string title = content;
                                    string description = content;
                                    string imageUrl = @"includes\disco.png";

                                    // Create a new panel to display the metadata
                                    Panel panel = new Panel();
                                    panel.BorderStyle = BorderStyle.FixedSingle;
                                    panel.Size = new Size(supFlow.Width - 50, 100);

                                    // Create a label for the title
                                    Label titleLabel = new Label();
                                    titleLabel.Text = title;
                                    titleLabel.Dock = DockStyle.Top;
                                    titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                    titleLabel.ForeColor = Color.White;
                                    titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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

                               

                                        // Create a new PictureBox control and add it to the panel
                                        PictureBox pictureBox = new PictureBox();
                                        pictureBox.Dock = DockStyle.Left;
                                        pictureBox.Size = new Size(100, 100);
                                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                                        pictureBox.ImageLocation = imageUrl;
                                        pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                        panel.Controls.Add(pictureBox);
                                        //pictures.Add(pictureBox);
                                  
                                    this.supFlow.Controls.Add(panel);

                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            string html = "";
                                            WebClient client = new WebClient();
                                            // Create a WebClient object to fetch the webpage
                                            if (!content.ToLower().EndsWith(".zip"))
                                            {
                                                html = client.DownloadString(content.StripLeadingTrailingSpaces());
                                            }
                                            // Create a MemoryStream object from the image data

                                            // Use regular expressions to extract the metadata from the HTML
                                            title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                            description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                            imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                            byte[] imageData = client.DownloadData(imageUrl);
                                            MemoryStream memoryStream = new MemoryStream(imageData);

                                            this.Invoke((MethodInvoker)delegate
                                            {
                                                titleLabel.Text = title;
                                                descriptionLabel.Text = description;
                                                pictureBox.ImageLocation = null;
                                                pictureBox.Image = System.Drawing.Image.FromStream(memoryStream);
                                            });



                                        }
                                        catch { }
                                    });
                                   
                                }
                                else
                                {


                                    if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id))
                                    {

                                        if (extension == ".mp4" || extension == ".mov" || extension == ".avi" || content.Contains("youtube.com") || content.Contains("youtu.be") || extension == ".wav" || extension == ".mp3")
                                        {

                                            try { AddMedia(content); } catch { }


                                        }
                                        else
                                        {


                                            AddImage(content);

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
                            AutoSizeMode = AutoSizeMode.GrowAndShrink,
                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                            Margin = new System.Windows.Forms.Padding(0, 10, 0, 10),
                            Padding = new System.Windows.Forms.Padding(0),
                        };

                        // Add the TableLayoutPanel to the FlowLayoutPanel
                        supFlow.Controls.Add(padding);

                        // Set the initial column style
                        // Set the initial width of the first column
                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 50));

                        splitContainer2.Panel2.SizeChanged += (sender, e) =>
                        {

                            padding.ColumnStyles[0].Width = splitContainer2.Panel2.Width - 100;
                            padding.Width = splitContainer2.Panel2.Width - 100;


                        };




                    }
                }
                catch (Exception ex)
                {
                    string help = ex.Message;
                }//deleted file



            }


            this.BeginInvoke((MethodInvoker)delegate
            {

                supFlow.ResumeLayout();
                btnPublicMessage.Enabled = true;
            });





        }

        private void RefreshPrivateSupMessages()
        {
            // Wrap async call to avoid blocking
            Task.Run(async () => await RefreshPrivateSupMessagesAsync());
        }

        private async Task RefreshPrivateSupMessagesAsync()
        {
            // sorry cannot run two searches at a time
            if (!btnPrivateMessage.Enabled || !btnPublicMessage.Enabled || !btnCommunityFeed.Enabled || System.IO.File.Exists("ROOTS-PROCESSING"))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            
            this.Invoke((MethodInvoker)delegate
            {
                supPrivateFlow.SuspendLayout();
            });
            // Clear controls if no messages have been displayed yet
            if (numPrivateMessagesDisplayed == 0)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    splitContainer1.Panel2.Controls.Clear();
                    supPrivateFlow.Dock = DockStyle.Fill;
                    supPrivateFlow.AutoScroll = true;
                    supPrivateFlow.FlowDirection = FlowDirection.LeftToRight;
                    splitContainer1.Panel2.Controls.Add(supPrivateFlow);
                });
            }
            if (profileURN.Links.Count == 0 || profileURN.Links[0].LinkData == null)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    supPrivateFlow.ResumeLayout();
                    btnPrivateMessage.Enabled = true;
                });
                return;
            }
            List<MessageObject> messages = OBJState.GetPrivateMessagesByAddress(profileURN.Links[0].LinkData.ToString(), mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, numPrivateMessagesDisplayed, 10);

            // Normalize and deduplicate messages
            var normalizedMessages = MessageNormalizer.Normalize(messages);
            
            // Filter out messages we've already displayed
            var newMessages = normalizedMessages.Where(m => !displayedPrivateMessageIds.Contains(m.TransactionId)).ToList();

            // Trigger memory cleanup if we received a full page from server (indicates more to load)
            // Check original message count, not filtered count, since filtering may reduce the count
            if (messages.Count == 10)
            {
                Task memoryPrune = Task.Run(() =>
                {

                    RemoveOverFlowMessages(supPrivateFlow);

                });
            }


            this.Invoke((MethodInvoker)delegate
            {
                btnPrivateMessage.Enabled = false;
            });

            Dictionary<string, string[]> profileAddress = new Dictionary<string, string[]> { };


            try
            {


                foreach (var normalizedMessage in newMessages)
                {
                    // Mark this message as displayed to prevent duplicates
                    displayedPrivateMessageIds.Add(normalizedMessage.TransactionId);
                    numPrivateMessagesDisplayed++;
                    
                    // Convert back to MessageObject for rendering
                    MessageObject messagePacket = new MessageObject
                    {
                        TransactionId = normalizedMessage.TransactionId,
                        Message = normalizedMessage.Message,
                        FromAddress = normalizedMessage.FromAddress,
                        ToAddress = normalizedMessage.ToAddress,
                        BlockDate = normalizedMessage.BlockDate
                    };

                    byte[] result = Array.Empty<byte>();
                    string message = "";
                    try { message = messagePacket.Message; } catch { };


                    string fromAddress = messagePacket.FromAddress;
                    string imagelocation = @"includes\anon.png";


                    if (!profileAddress.ContainsKey(fromAddress))
                    {

                        PROState profile = PROState.GetProfileByAddress(fromAddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                        if (profile.URN != null)
                        {
                            fromAddress = TruncateAddress(profile.URN);
                            imagelocation = profile.Image;

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
                                Root broot = new Root();
                                if (!File.Exists(imagelocation))
                                {
                                    switch (profile.Image.ToUpper().Substring(0, 4))
                                    {
                                        case "MZC:":
                                            broot = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                            break;
                                        case "BTC:":

                                            broot = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                            break;
                                        case "LTC:":

                                            broot = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");


                                            break;
                                        case "DOG:":
                                            broot = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                            break;
                                        case "IPFS":
                                            string transid = "empty";
                                            try { transid = profile.Image.Substring(5, 46); } catch { }

                                            if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                            {
                                                try
                                                {
                                                    Directory.CreateDirectory("ipfs/" + transid);
                                                }
                                                catch { };

                                                Directory.CreateDirectory("ipfs/" + transid + "-build");
                                                Process process2 = new Process();
                                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                process2.StartInfo.UseShellExecute = false;
                                                process2.StartInfo.CreateNoWindow = true;
                                                process2.Start();
                                                if (process2.WaitForExit(5000))
                                                {
                                                    string fileName;
                                                    if (System.IO.File.Exists("ipfs/" + transid))
                                                    {
                                                        System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                        System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                        fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                        Directory.CreateDirectory("ipfs/" + transid);
                                                        System.IO.File.Move("ipfs/" + transid + "_tmp", imagelocation);
                                                    }

                                                    if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                    {
                                                        fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                        System.IO.File.Move("ipfs/" + transid + "/" + transid, imagelocation);
                                                    }

                                                    try
                                                    {
                                                        if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                                    catch { }

                                                    try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                                }
                                                else
                                                {
                                                    process2.Kill();

                                                    Task.Run(() =>
                                                    {
                                                        process2 = new Process();
                                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                        process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                        process2.StartInfo.UseShellExecute = false;
                                                        process2.StartInfo.CreateNoWindow = true;
                                                        process2.Start();
                                                        if (process2.WaitForExit(550000))
                                                        {
                                                            string fileName;
                                                            if (System.IO.File.Exists("ipfs/" + transid))
                                                            {
                                                                System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                                System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                                fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                                Directory.CreateDirectory("ipfs/" + transid);
                                                                System.IO.File.Move("ipfs/" + transid + "_tmp", imagelocation);
                                                            }

                                                            if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                            {
                                                                fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                                System.IO.File.Move("ipfs/" + transid + "/" + transid, imagelocation);
                                                            }

                                                            try
                                                            {
                                                                if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                                            catch { }

                                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }


                                                        }
                                                        else
                                                        {
                                                            process2.Kill();
                                                        }
                                                    });

                                                }


                                            }

                                            break;
                                        default:
                                            if (!profile.Image.ToUpper().StartsWith("HTTP") && transactionid != "")
                                            {
                                                broot = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

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
                        profileAddress.Add(messagePacket.FromAddress, profilePacket);

                    }
                    else
                    {
                        string[] profilePacket = new string[] { };
                        profileAddress.TryGetValue(fromAddress, out profilePacket);
                        fromAddress = profilePacket[0];
                        imagelocation = profilePacket[1];

                    }


                    string tstamp = messagePacket.BlockDate.ToString("yyyyMMddHHmmss");
                    System.Drawing.Color bgcolor = System.Drawing.Color.White;

                    string unfilteredmessage = message;
                    string[] blocks = Regex.Matches(message, "<<[^<>]+>>")
                                             .Cast<Match>()
                                             .Select(m => m.Value.Trim(new char[] { '<', '>' }))
                                             .ToArray();
                    message = Regex.Replace(message, "<<.*?>>", "");

                    if (message != "" || blocks.Length > 1 || (blocks.Length == 1 && !int.TryParse(blocks[0], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out _)))
                    {


                        this.Invoke((MethodInvoker)delegate
                        {
                            CreateRow(imagelocation, fromAddress, messagePacket.FromAddress, DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, messagePacket.TransactionId, true, supPrivateFlow);
                        });


                        string pattern = "<<.*?>>";
                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                        foreach (Match match in matches)
                        {
                            string content = match.Value.Substring(2, match.Value.Length - 4);
                            if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id) && !content.Trim().StartsWith("#"))
                            {

                                if (content.StartsWith("IPFS:") && content.EndsWith(@"\SEC"))
                                {
                                    // Load SEC attachment asynchronously without blocking message loading
                                    // This prevents the UI from freezing when IPFS is slow or fails
                                    // Use Task.Run to ensure the async operation completes instead of being abandoned
                                    // Task.Run properly handles async lambdas in both Debug and Release modes
                                    _ = Task.Run(async () => await LoadSecAttachmentAsync(content, messagePacket.TransactionId, profileURN.Links[0].LinkData.ToString()).ConfigureAwait(false));
                                }
                                else
                                {


                                    List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4", ".mov", ".avi", ".wav", ".mp3" };

                                    string extension = Path.GetExtension(content).ToLower();
                                    if (!imgExtensions.Contains(extension) && !content.Contains("youtube.com") && !content.Contains("youtu.be"))
                                    {
                                        string title = content;
                                        string description = content;
                                        string imageUrl = @"includes\disco.png";

                                        // Create a new panel to display the metadata
                                        Panel panel = new Panel();
                                        panel.BorderStyle = BorderStyle.FixedSingle;
                                        panel.Size = new Size(supFlow.Width - 50, 100);

                                        // Create a label for the title
                                        Label titleLabel = new Label();
                                        titleLabel.Text = title;
                                        titleLabel.Dock = DockStyle.Top;
                                        titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                        titleLabel.ForeColor = Color.White;
                                        titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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



                                        // Create a new PictureBox control and add it to the panel
                                        PictureBox pictureBox = new PictureBox();
                                        pictureBox.Dock = DockStyle.Left;
                                        pictureBox.Size = new Size(100, 100);
                                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                                        pictureBox.ImageLocation = imageUrl;
                                        pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                        panel.Controls.Add(pictureBox);
                                        //pictures.Add(pictureBox);

                                        this.supPrivateFlow.Controls.Add(panel);

                                        Task.Run(() =>
                                        {
                                            try
                                            {
                                                string html = "";
                                                WebClient client = new WebClient();
                                                // Create a WebClient object to fetch the webpage
                                                if (!content.ToLower().EndsWith(".zip"))
                                                {
                                                    html = client.DownloadString(content.StripLeadingTrailingSpaces());
                                                }
                                                // Create a MemoryStream object from the image data

                                                // Use regular expressions to extract the metadata from the HTML
                                                title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                                description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                                imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                                byte[] imageData = client.DownloadData(imageUrl);
                                                MemoryStream memoryStream = new MemoryStream(imageData);

                                                this.Invoke((MethodInvoker)delegate
                                                {
                                                    titleLabel.Text = title;
                                                    descriptionLabel.Text = description;
                                                    pictureBox.ImageLocation = null;
                                                    pictureBox.Image = System.Drawing.Image.FromStream(memoryStream);
                                                });



                                            }
                                            catch { }
                                        });


                                    }
                                    else
                                    {
                                        if (extension == ".mp4" || extension == ".mov" || extension == ".avi" || content.Contains("youtube.com") || content.Contains("youtu.be") || extension == ".wav" || extension == ".mp3")
                                        {

                                            this.Invoke((MethodInvoker)delegate
                                            {
                                                try { AddMedia(content, true); } catch { }
                                            });

                                        }
                                        else
                                        {

                                            this.Invoke((MethodInvoker)delegate
                                            {
                                                AddImage(content, true);
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
                            Margin = new System.Windows.Forms.Padding(0, 10, 0, 10),
                            Padding = new System.Windows.Forms.Padding(0)

                        };

                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supPrivateFlow.Width - 20));
                        this.Invoke((MethodInvoker)delegate
                        {
                            supPrivateFlow.Controls.Add(padding);
                        });

                    }

                }


            }
            catch (Exception ex)
            {
                string errormessage = ex.Message;
            }

            this.Invoke((MethodInvoker)delegate
            {
                supPrivateFlow.ResumeLayout();
                btnPrivateMessage.Enabled = true;
            });



        }

        /// <summary>
        /// Asynchronously loads and decrypts an SEC IPFS attachment for a private message.
        /// This method runs in the background and updates the UI when complete or if it fails.
        /// </summary>
        /// <param name="content">The IPFS content string (e.g., "IPFS:hash\SEC")</param>
        /// <param name="messageTransactionId">The transaction ID of the message (reserved for future use in error tracking)</param>
        /// <param name="recipientAddress">The recipient's address for decryption</param>
        private async Task LoadSecAttachmentAsync(string content, string messageTransactionId, string recipientAddress)
        {
            string transid = "empty";
            try { transid = content.Substring(5, 46); } catch { }

            try
            {
                Debug.WriteLine($"[LoadSecAttachmentAsync] Starting SEC attachment load for {transid}");

            // Check if already downloaded
            var secPath = @"root/" + transid + @"/SEC";
            if (System.IO.File.Exists(secPath))
            {
                Debug.WriteLine($"[LoadSecAttachmentAsync] SEC file already exists for {transid}, displaying");
                await DisplaySecAttachmentAsync(transid, recipientAddress).ConfigureAwait(false);
                return;
            }

            // Check if file was downloaded but not yet processed (from previous interrupted run)
            var hashPath = "root/" + transid;
            if (System.IO.File.Exists(hashPath) || System.IO.Directory.Exists(hashPath))
            {
                // File/directory exists but SEC doesn't - try to process it
                Debug.WriteLine($"[LoadSecAttachmentAsync] Found existing download for {transid}, attempting to process");
                
                bool processed = IpfsHelper.ProcessDownloadedFile(transid, "root", "SEC");
                if (processed)
                {
                    IpfsHelper.CleanupBuildDirectory(transid, "root");
                    await DisplaySecAttachmentAsync(transid, recipientAddress).ConfigureAwait(false);
                    return;
                }
                else
                {
                    // Failed to process existing download - clean up and re-download
                    Debug.WriteLine($"[LoadSecAttachmentAsync] Failed to process existing download for {transid}, will re-download");
                    CleanupEmptyDirectory(transid, "root");
                }
            }

            // Clean up any stale build directories (from previous crash/interruption)
            if (System.IO.Directory.Exists("root/" + transid + "-build"))
            {
                Debug.WriteLine($"[LoadSecAttachmentAsync] Cleaning up stale build directory for {transid}");
                IpfsHelper.CleanupBuildDirectory(transid, "root");
            }

            // Create build directory marker (not the target directory yet)
            // IPFS will create root/{hash} as file or directory
            try
            {
                Directory.CreateDirectory("root/" + transid + "-build");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoadSecAttachmentAsync] Error creating build directory: {ex.Message}");
                return;
            }

            // Download from IPFS using the async helper with 60 second timeout
            // Since we're already async, we can wait longer without blocking the UI
            // IPFS will download to root/{hash} - either as a file or directory
            Debug.WriteLine($"[LoadSecAttachmentAsync] About to call IPFS GetAsync for {transid}");
            bool downloadSuccess = await IpfsHelper.GetAsync(transid, "root\\" + transid, 60000).ConfigureAwait(false);
            Debug.WriteLine($"[LoadSecAttachmentAsync] IPFS GetAsync returned {downloadSuccess} for {transid}");

            if (downloadSuccess)
            {
                Debug.WriteLine($"[LoadSecAttachmentAsync] Download succeeded for {transid}, processing file");
                
                // Process the downloaded file
                bool processed = IpfsHelper.ProcessDownloadedFile(transid, "root", "SEC");

                if (processed)
                {
                    Debug.WriteLine($"[LoadSecAttachmentAsync] File processed successfully for {transid}, displaying");
                    
                    // Pin the file asynchronously (fire and forget)
                    _ = IpfsHelper.PinAsync(transid);

                    // Clean up build directory
                    IpfsHelper.CleanupBuildDirectory(transid, "root");

                    // Display the decrypted attachment
                    await DisplaySecAttachmentAsync(transid, recipientAddress).ConfigureAwait(false);
                    
                    Debug.WriteLine($"[LoadSecAttachmentAsync] Completed displaying {transid}");
                }
                else
                {
                    Debug.WriteLine($"[LoadSecAttachmentAsync] Failed to process downloaded file for {transid}");
                    // Clean up build directory and empty hash directory
                    IpfsHelper.CleanupBuildDirectory(transid, "root");
                    CleanupEmptyDirectory(transid, "root");
                    ShowAttachmentError(transid, "Failed to process attachment");
                }
            }
            else
            {
                // Download failed after 60 second timeout
                Debug.WriteLine($"[LoadSecAttachmentAsync] Download failed for {transid} after timeout");
                // Clean up build directory and empty hash directory
                IpfsHelper.CleanupBuildDirectory(transid, "root");
                CleanupEmptyDirectory(transid, "root");
                ShowAttachmentError(transid, "IPFS download timeout");
            }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoadSecAttachmentAsync] EXCEPTION in LoadSecAttachmentAsync for {transid}: {ex.Message}");
                Debug.WriteLine($"[LoadSecAttachmentAsync] Stack trace: {ex.StackTrace}");
                try
                {
                    IpfsHelper.CleanupBuildDirectory(transid, "root");
                    CleanupEmptyDirectory(transid, "root");
                    ShowAttachmentError(transid, $"Error: {ex.Message}");
                }
                catch { }
            }
        }

        /// <summary>
        /// Decrypts and displays an SEC attachment that has been downloaded from IPFS.
        /// </summary>
        private async Task DisplaySecAttachmentAsync(string transid, string recipientAddress)
        {
            try
            {
                Debug.WriteLine($"[DisplaySecAttachmentAsync] Decrypting SEC attachment for {transid}");

                // Read and decrypt the SEC file
                byte[] result2 = Root.GetRootBytesByFile(new string[] { @"root/" + transid + @"/SEC" });
                
                if (result2 == null || result2.Length == 0)
                {
                    Debug.WriteLine($"[DisplaySecAttachmentAsync] SEC file is empty for {transid}");
                    ShowAttachmentError(transid, "Attachment file is empty");
                    return;
                }

                byte[] result = Root.DecryptRootBytes(mainnetLogin, mainnetPassword, mainnetURL, recipientAddress, result2);

                if (result == null || result.Length == 0)
                {
                    Debug.WriteLine($"[DisplaySecAttachmentAsync] Decryption failed for {transid}");
                    ShowAttachmentError(transid, "Failed to decrypt attachment");
                    return;
                }

                Root decryptedroot = Root.GetRootByTransactionId(transid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, result, recipientAddress);
                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4", ".mov", ".avi", ".wav", ".mp3" };

                if (decryptedroot.File != null)
                {
                    foreach (string file in decryptedroot.File.Keys)
                    {
                        string extension = Path.GetExtension(transid + @"\" + file).ToLower();
                        
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (!imgExtensions.Contains(extension))
                            {
                                // Create a new panel to display non-media files
                                Panel panel = new Panel();
                                panel.BorderStyle = BorderStyle.FixedSingle;
                                panel.MinimumSize = new Size(supPrivateFlow.Width - 50, 30);
                                panel.MaximumSize = new Size(supPrivateFlow.Width - 20, 30);
                                panel.AutoSize = true;

                                LinkLabel titleLabel = new LinkLabel();
                                titleLabel.Text = transid + @"\" + file;
                                titleLabel.Links[0].LinkData = transid + @"\" + file;
                                titleLabel.AutoSize = true;
                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                titleLabel.Padding = new Padding(5);
                                titleLabel.MouseClick += (sender, e) => { Attachment_Clicked(@"root\" + transid + @"\" + file); };
                                panel.Controls.Add(titleLabel);

                                this.supPrivateFlow.Controls.Add(panel);
                            }
                            else if (extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".wav" || extension == ".mp3")
                            {
                                try { AddMedia(transid + @"\" + file, true); } catch { }
                            }
                            else
                            {
                                AddImage(transid + @"\" + file, true);
                            }
                        });
                    }

                    Debug.WriteLine($"[DisplaySecAttachmentAsync] Successfully displayed SEC attachment for {transid}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DisplaySecAttachmentAsync] Exception: {ex.Message}");
                ShowAttachmentError(transid, "Error displaying attachment");
            }
        }

        /// <summary>
        /// Shows an error message for a failed attachment load.
        /// </summary>
        private void ShowAttachmentError(string transid, string errorMessage)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    Panel panel = new Panel();
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    panel.MinimumSize = new Size(supPrivateFlow.Width - 50, 30);
                    panel.MaximumSize = new Size(supPrivateFlow.Width - 20, 30);
                    panel.AutoSize = true;
                    panel.BackColor = System.Drawing.Color.DarkRed;

                    Label errorLabel = new Label();
                    errorLabel.Text = $"⚠ {errorMessage}: {transid.Substring(0, Math.Min(12, transid.Length))}...";
                    errorLabel.AutoSize = true;
                    errorLabel.Font = new Font("Segoe UI", 8, FontStyle.Italic);
                    errorLabel.ForeColor = System.Drawing.Color.White;
                    errorLabel.Padding = new Padding(5);
                    panel.Controls.Add(errorLabel);

                    this.supPrivateFlow.Controls.Add(panel);
                });
            }
            catch { }
        }

        /// <summary>
        /// Cleans up empty directory if download failed.
        /// </summary>
        private void CleanupEmptyDirectory(string hash, string baseDir)
        {
            try
            {
                var hashDir = Path.Combine(baseDir, hash);
                if (Directory.Exists(hashDir))
                {
                    // Check if directory is empty or only contains empty subdirectories
                    var files = Directory.GetFiles(hashDir, "*", SearchOption.AllDirectories);
                    if (files.Length == 0)
                    {
                        Directory.Delete(hashDir, true);
                        Debug.WriteLine($"[CleanupEmptyDirectory] Removed empty directory: {hashDir}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CleanupEmptyDirectory] Error cleaning up {hash}: {ex.Message}");
            }
        }

        private void RefreshCommunityMessages()
        {
            // sorry cannot run two searches at a time
            if (!btnPrivateMessage.Enabled || !btnPrivateMessage.Enabled || !btnCommunityFeed.Enabled || System.IO.File.Exists("ROOTS-PROCESSING"))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            supFlow.SuspendLayout();



            this.Invoke((MethodInvoker)delegate
            {
                btnCommunityFeed.BackColor = System.Drawing.Color.Blue;
                btnCommunityFeed.ForeColor = System.Drawing.Color.Yellow;
                btnPublicMessage.BackColor = System.Drawing.Color.White;
                btnPrivateMessage.BackColor = System.Drawing.Color.White;
                btnPublicMessage.ForeColor = System.Drawing.Color.Black;
                btnPrivateMessage.ForeColor = System.Drawing.Color.Black;
                btnCommunityFeed.Enabled = false;

                if (panel1.Visible)
                {
                    panel1.Visible = false;
                    supFlow.Location = new System.Drawing.Point(supFlow.Location.X, supFlow.Location.Y - 150); // Change the X and Y coordinates
                    supFlow.Size = new System.Drawing.Size(supFlow.Width, supFlow.Height + 150); // Change the width and height
                }
            });


            numMessagesDisplayed = 0;
            numPrivateMessagesDisplayed = 0;


            string FriendsListPath = "";
            if (testnet) { FriendsListPath = @"root\MyFriendList.Json"; } else { FriendsListPath = @"root\MyProdFriendList.Json"; }


            if (File.Exists(FriendsListPath))
            {

                var myFriendsJson = File.ReadAllText(FriendsListPath);
                var myFriends = JsonConvert.DeserializeObject<Dictionary<string, string>>(myFriendsJson);

                // Collect all messages from followed profiles
                var allMessagesFromFriends = new List<MessageObject>();
                foreach (var key in myFriends.Keys)
                {
                    try
                    {
                        List<MessageObject> result = OBJState.GetPublicMessagesByAddress(key, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, 0, 50);
                        allMessagesFromFriends.AddRange(result);
                    }
                    catch { }
                }

                // Use MessageNormalizer to deduplicate and sort all messages
                var normalizedMessages = MessageNormalizer.Normalize(allMessagesFromFriends);
                
                // Filter out messages we've already displayed
                var newMessages = normalizedMessages.Where(m => !displayedCommunityMessageIds.Contains(m.TransactionId)).ToList();
                
                // Paginate the results
                var messagesToDisplay = MessageNormalizer.Paginate(newMessages, numFriendFeedsDisplayed, 10);

                // Trigger memory cleanup if we have more messages available to paginate
                // Check if there are at least 10 more messages after the current page
                if (newMessages.Count > numFriendFeedsDisplayed + 10)
                {
                    Task memoryPrune = Task.Run(() =>
                    {

                        RemoveOverFlowMessages(supFlow);

                    });
                }

                foreach (var normalizedMessage in messagesToDisplay)
                {
                    // Mark this message as displayed to prevent duplicates
                    displayedCommunityMessageIds.Add(normalizedMessage.TransactionId);
                    
                    string _from = normalizedMessage.FromAddress;
                    string _to = normalizedMessage.ToAddress;
                    string _transactionId = normalizedMessage.TransactionId;
                    string fromURN = normalizedMessage.FromAddress;
                    string _message = normalizedMessage.Message;
                    string _blockdate = normalizedMessage.BlockDate.ToString("yyyyMMddHHmmss");
                    string imglocation = "";
                    string unfilteredmessage = _message;
                    string[] blocks = Regex.Matches(_message, "<<[^<>]+>>")
                                             .Cast<Match>()
                                             .Select(m => m.Value.Trim(new char[] { '<', '>' }))
                                             .ToArray();
                    _message = Regex.Replace(_message, "<<.*?>>", "");




                    if (_message != "" || blocks.Length > 1 || (blocks.Length == 1 && !int.TryParse(blocks[0], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out _)) || File.Exists(@"root\" + _transactionId + @"\INQ"))
                    {

                        PROState fromProfile = PROState.GetProfileByAddress(fromURN, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                        if (fromProfile.URN != null)
                        {
                            fromURN = fromProfile.URN;

                        }


                        string toURN = toProp?.GetValue(message).ToString();
                        PROState toProfile = PROState.GetProfileByAddress(toURN, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                        if (toProfile.URN != null)
                        {
                            toURN = toProfile.URN;

                        }


                        this.Invoke((MethodInvoker)delegate
                        {
                            try
                            {
                                try { imglocation = myFriends[_from]; } catch { imglocation = @"includes\anon.png"; }
                                CreateRow(imglocation, fromURN, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, _transactionId, false, supFlow);
                                imglocation = "";
                                try { imglocation = myFriends[_to]; } catch { imglocation = @"includes\anon.png"; }
                                CreateRow(imglocation, toURN, _to, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), "", _transactionId, false, supFlow);
                            }
                            catch { }
                        });

                        try
                        {
                            string[] files = Directory.GetFiles(@"root\" + _transactionId);

                            bool containsFileWithINQ = files.Any(file =>
                                   file.EndsWith("INQ", StringComparison.OrdinalIgnoreCase) &&
                                   !file.EndsWith("BLOCK", StringComparison.OrdinalIgnoreCase));

                            if (containsFileWithINQ)
                            {
                                //ADD INQ IF IT EXISTS AND IS NOT BLOCKED
                                this.Invoke((MethodInvoker)delegate
                                {
                                    string profileowner = "";

                                    if (profileOwner.Tag != null) { profileowner = profileOwner.Tag.ToString(); }

                                    FoundINQControl foundObject = new FoundINQControl(_transactionId, profileowner, testnet);
                                    foundObject.Margin = new Padding(20, 7, 8, 7);
                                    supFlow.Controls.Add(foundObject);
                                });
                            }
                        }
                        catch { }

                        string pattern = "<<.*?>>";
                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                        foreach (Match match in matches)
                        {


                            string content = match.Value.Substring(2, match.Value.Length - 4);
                            if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id) && !content.Trim().StartsWith("#"))
                            {

                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4", ".mov", ".avi", ".wav", ".mp3" };

                                string extension = Path.GetExtension(content).ToLower();
                                if (!imgExtensions.Contains(extension) && !content.Contains("youtube.com") && !content.Contains("youtu.be"))
                                {
                                    string title = content;
                                    string description = content;
                                    string imageUrl = @"includes\disco.png";

                                    // Create a new panel to display the metadata
                                    Panel panel = new Panel();
                                    panel.BorderStyle = BorderStyle.FixedSingle;
                                    panel.Size = new Size(supFlow.Width - 50, 100);

                                    // Create a label for the title
                                    Label titleLabel = new Label();
                                    titleLabel.Text = title;
                                    titleLabel.Dock = DockStyle.Top;
                                    titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                    titleLabel.ForeColor = Color.White;
                                    titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
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



                                    // Create a new PictureBox control and add it to the panel
                                    PictureBox pictureBox = new PictureBox();
                                    pictureBox.Dock = DockStyle.Left;
                                    pictureBox.Size = new Size(100, 100);
                                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                                    try { pictureBox.ImageLocation = imageUrl; } catch { }
                                    pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                    panel.Controls.Add(pictureBox);
                                    //pictures.Add(pictureBox);

                                    this.supFlow.Controls.Add(panel);

                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            string html = "";
                                            WebClient client = new WebClient();
                                            // Create a WebClient object to fetch the webpage
                                            if (!content.ToLower().EndsWith(".zip"))
                                            {
                                                html = client.DownloadString(content.StripLeadingTrailingSpaces());
                                            }
                                            // Create a MemoryStream object from the image data

                                            // Use regular expressions to extract the metadata from the HTML
                                            title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                            description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                            imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                            byte[] imageData = client.DownloadData(imageUrl);
                                            MemoryStream memoryStream = new MemoryStream(imageData);

                                            this.Invoke((MethodInvoker)delegate
                                            {
                                                titleLabel.Text = title;
                                                descriptionLabel.Text = description;
                                                pictureBox.ImageLocation = null;
                                                pictureBox.Image = System.Drawing.Image.FromStream(memoryStream);
                                            });



                                        }
                                        catch { }
                                    });


                                }
                                else
                                {

                                    if (extension == ".mp4" || extension == ".mov" || extension == ".avi" || content.Contains("youtube.com") || content.Contains("youtu.be") || extension == ".wav" || extension == ".mp3")
                                    {

                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            try { AddMedia(content); } catch { }
                                        });

                                    }
                                    else
                                    {

                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            try { AddImage(content); } catch { }
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
                            Margin = new System.Windows.Forms.Padding(0, 10, 0, 10),
                            Padding = new System.Windows.Forms.Padding(0)

                        };

                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 20));

                        this.Invoke((MethodInvoker)delegate
                        {
                            supFlow.Controls.Add(padding);
                        });


                    }
                    numFriendFeedsDisplayed++;
                }



            }

            this.BeginInvoke((MethodInvoker)delegate
            {
                supFlow.ResumeLayout();
                btnCommunityFeed.Enabled = true;
            });




        }

        void AddImage(string imagepath, bool isprivate = false, bool addtoTop = false)
        {
            string imagelocation = "";
            if (imagepath != null)
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

                int calcwidth;

                if (isprivate)
                {
                    calcwidth = supPrivateFlow.Width - 50;
                    if (calcwidth > 480) { calcwidth = 480; }

                    msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, calcwidth));

                    supPrivateFlow.Controls.Add(msg);
                }
                else
                {
                    calcwidth = supFlow.Width - 50;
                    if (calcwidth > 480) { calcwidth = 480; }
                    msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, calcwidth));





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

                pictureBox.Width = calcwidth;
                pictureBox.Height = calcwidth;


                pictureBox.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
                pictureBox.ImageLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\includes\progress.gif";

                msg.Controls.Add(pictureBox);

                imagelocation = imagepath;

                if (!imagepath.ToLower().StartsWith("http"))
                {
                    imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (imagepath.ToLower().StartsWith("ipfs:")) { imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\"); if (imagepath.Length == 51) { imagelocation += @"\artifact"; } }

                    Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                    Match imgurnmatch = regexTransactionId.Match(imagelocation);
                    string transactionid = imgurnmatch.Value;
                    Root root = new Root();
                    if (!File.Exists(imagelocation))
                    {

                        Task.Run(() =>
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
                                        try
                                        {
                                            Directory.CreateDirectory("ipfs/" + transid);
                                        }
                                        catch { };

                                        Directory.CreateDirectory("ipfs/" + transid + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + imagepath.Substring(5, 46) + @" -o ipfs\" + transid;
                                        process2.StartInfo.UseShellExecute = false;
                                        process2.StartInfo.CreateNoWindow = true;
                                        process2.Start();
                                        if (process2.WaitForExit(550000))
                                        {
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

                                            try
                                            {
                                                if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                            catch { }

                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                        }
                                        else
                                        {
                                            process2.Kill();
                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }

                                            this.Invoke((Action)(() =>
                                            {

                                                Random rnd = new Random();
                                                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                                                if (gifFiles.Length > 0)
                                                {
                                                    int randomIndex = rnd.Next(gifFiles.Length);
                                                    pictureBox.ImageLocation = gifFiles[randomIndex];

                                                }
                                                else
                                                {
                                                    try
                                                    {

                                                        pictureBox.ImageLocation = @"includes\HugPuddle.jpg";
                                                    }
                                                    catch { }
                                                }
                                            }));
                                        }

                                    }

                                    break;
                                default:
                                    if (!imagepath.ToUpper().StartsWith("HTTP") && transactionid != "")
                                    {
                                        Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                    }
                                    break;

                            }
                            if (File.Exists(imagelocation))
                            {
                                this.Invoke((Action)(() =>
                                {
                                    pictureBox.ImageLocation = imagelocation;
                                    pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };

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
                                        pictureBox.ImageLocation = gifFiles[randomIndex];

                                    }
                                    else
                                    {
                                        try
                                        {

                                            pictureBox.ImageLocation = @"includes\HugPuddle.jpg";
                                        }
                                        catch { }
                                    }
                                }));
                            }


                        });



                    }
                    else
                    {



                        string thumbnailPath = imagelocation + "-thumbnail.jpg";

                        // Check if a thumbnail exists
                        if (System.IO.File.Exists(thumbnailPath))
                        {

                            pictureBox.ImageLocation = thumbnailPath;
                            pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };
                     

                        }
                        else
                        {
                            // Load the original image from file
                            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(imagelocation);

                            // Check if the original image is a GIF
                            if (Path.GetExtension(imagelocation).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                            {
                                // For GIF images, directly use the original image without creating a thumbnail
                                pictureBox.ImageLocation = imagelocation;
                                pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };
                                
                            }
                            else
                            {
                                // Resize the image if needed
                                int maxWidth = pictureBox.Width;
                                int maxHeight = pictureBox.Height;

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

                                System.Drawing.Image resizedImage = new Bitmap(originalImage, newWidth, newHeight);
                                originalImage.Dispose();
                                this.Invoke((Action)(() =>
                                {
                                    pictureBox.ImageLocation = null;
                                    pictureBox.Image = resizedImage;

                                }));

                                // Save the resized image as a thumbnail
                                resizedImage.Save(thumbnailPath, ImageFormat.Jpeg);
                            }
                        }


                    }
                }
                else
                {

                    Task.Run(() =>
                    {
                        this.Invoke((Action)(() =>
                        {

                            pictureBox.ImageLocation = imagelocation;


                            pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };
                        }));

                    });

                }


            }


        }

        void AddProfile(string imagepath, string profileurn, string profileaddress)
        {
            string imagelocation = "";
            if (imagepath != null)
            {
                TableLayoutPanel msg = new TableLayoutPanel
                {
                    RowCount = 2,
                    ColumnCount = 1,
                    Dock = DockStyle.Top,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true,
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                    Margin = new System.Windows.Forms.Padding(10), // Add margin for spacing
                    Padding = new System.Windows.Forms.Padding(10) // Add padding for spacing
                };

                int calcwidth = 200;
                msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, calcwidth));
                msg.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // First row for PictureBox
                msg.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Second row for Label

                Label welcomeLabel = new Label();
                welcomeLabel.Text = "Sup!? " + profileurn;
                welcomeLabel.ForeColor = Color.White; // Ensure the text is visible on the black background
                welcomeLabel.Font = new Font("Arial", 12, FontStyle.Bold); // Change font style and size
                welcomeLabel.TextAlign = ContentAlignment.MiddleCenter; // Center align text
                welcomeLabel.AutoSize = true; // Enable AutoSize for the label
                welcomeLabel.Dock = DockStyle.Fill; // Make the label fill the cell
                welcomeLabel.Padding = new Padding(0); // Remove padding if text is cut off
                welcomeLabel.Margin = new Padding(0); // Remove margin if text is cut off

                // Add click event handler
                welcomeLabel.Click += (sender2, e2) =>
                {
                    MakeActiveProfile(profileaddress);
                };

                // Add the label to the second row and first column
                msg.Controls.Add(welcomeLabel, 0, 1);

                PictureBox pictureBox = new PictureBox();

                // Set the PictureBox properties
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Width = calcwidth;
                pictureBox.Height = calcwidth;
                pictureBox.BorderStyle = BorderStyle.Fixed3D;
                pictureBox.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
                pictureBox.ImageLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\includes\progress.gif";

                // Add the PictureBox to the first row and first column
                msg.Controls.Add(pictureBox, 0, 0);

                supFlow.Controls.Add(msg);
                supFlow.Controls.SetChildIndex(msg, 0);



                imagelocation = imagepath;

                if (!imagepath.ToLower().StartsWith("http"))
                {
                    imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (imagepath.ToLower().StartsWith("ipfs:")) { imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\"); if (imagepath.Length == 51) { imagelocation += @"\artifact"; } }

                    Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                    Match imgurnmatch = regexTransactionId.Match(imagelocation);
                    string transactionid = imgurnmatch.Value;
                    Root root = new Root();
                    if (!File.Exists(imagelocation))
                    {

                        Task.Run(() =>
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
                                        try
                                        {
                                            Directory.CreateDirectory("ipfs/" + transid);
                                        }
                                        catch { };

                                        Directory.CreateDirectory("ipfs/" + transid + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + imagepath.Substring(5, 46) + @" -o ipfs\" + transid;
                                        process2.StartInfo.UseShellExecute = false;
                                        process2.StartInfo.CreateNoWindow = true;
                                        process2.Start();
                                        if (process2.WaitForExit(550000))
                                        {
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

                                            try
                                            {
                                                if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                            catch { }

                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                        }
                                        else
                                        {
                                            process2.Kill();
                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }

                                            this.Invoke((Action)(() =>
                                            {

                                                Random rnd = new Random();
                                                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                                                if (gifFiles.Length > 0)
                                                {
                                                    int randomIndex = rnd.Next(gifFiles.Length);
                                                    pictureBox.ImageLocation = gifFiles[randomIndex];

                                                }
                                                else
                                                {
                                                    try
                                                    {

                                                        pictureBox.ImageLocation = @"includes\HugPuddle.jpg";
                                                    }
                                                    catch { }
                                                }
                                            }));
                                        }

                                    }

                                    break;
                                default:
                                    if (!imagepath.ToUpper().StartsWith("HTTP") && transactionid != "")
                                    {
                                        Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                    }
                                    break;

                            }
                            if (File.Exists(imagelocation))
                            {
                                this.Invoke((Action)(() =>
                                {
                                    pictureBox.ImageLocation = imagelocation;
                                    pictureBox.MouseClick += (sender, e) => { MakeActiveProfile(profileaddress); };

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
                                        pictureBox.ImageLocation = gifFiles[randomIndex];

                                    }
                                    else
                                    {
                                        try
                                        {

                                            pictureBox.ImageLocation = @"includes\HugPuddle.jpg";
                                        }
                                        catch { }
                                    }
                                }));
                            }


                        });



                    }
                    else
                    {
                        pictureBox.ImageLocation = imagelocation;
                        pictureBox.MouseClick += (sender, e) => { MakeActiveProfile(profileaddress);};

                    }
                }
                else
                {

                    Task.Run(() =>
                    {
                        this.Invoke((Action)(() =>
                        {

                            pictureBox.ImageLocation = imagelocation;


                            pictureBox.MouseClick += (sender, e) => { MakeActiveProfile(profileaddress); };
                        }));

                    });

                }


            }


        }


        async void AddMedia(string videopath, bool isprivate = false, bool addtoTop = false, bool autoPlay = false)
        {
            string videolocation = "";
            if (videopath != null)
            {
                videolocation = videopath;

                //build web viewer with default loading page
                Microsoft.Web.WebView2.WinForms.WebView2 webviewer = new Microsoft.Web.WebView2.WinForms.WebView2();
                webviewer.AllowExternalDrop = false;
                webviewer.TabStop = false;
                webviewer.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
                webviewer.CreationProperties = null;
                webviewer.DefaultBackgroundColor = System.Drawing.Color.FromArgb(22, 22, 22);
                webviewer.Name = "webviewer";



                if (videolocation.ToLower().EndsWith(".wav") || videolocation.ToLower().EndsWith(".mp3"))
                {


                    if (isprivate)
                    {
                        int calcwidth = supPrivateFlow.Width - 50;
                        if (calcwidth > 480) { calcwidth = 480; }

                        webviewer.Size = new System.Drawing.Size(calcwidth, 150);
                    }
                    else
                    {
                        int calcwidth = supFlow.Width - 50;
                        if (calcwidth > 480) { calcwidth = 480; }
                        webviewer.Size = new System.Drawing.Size(calcwidth, 150);
                    }
                }
                else
                {
                    if (isprivate)
                    {
                        int calcwidth = supPrivateFlow.Width - 50;
                        if (calcwidth > 960) { calcwidth = 960; }
                        webviewer.Size = new System.Drawing.Size(calcwidth, calcwidth - 100);
                    }
                    else
                    {
                        int calcwidth = supFlow.Width - 50;
                        if (calcwidth > 960) { calcwidth = 960; }
                        webviewer.Size = new System.Drawing.Size(calcwidth, calcwidth - 100);

                        splitContainer2.Panel2.SizeChanged += (sender, e) =>
                        {
                            webviewer.Size = new System.Drawing.Size(splitContainer2.Panel2.Width - 100, splitContainer2.Panel2.Width - 200);
                        };
                    }
                }
                webviewer.ZoomFactor = 1D;




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


                if (isprivate)
                {
                    msg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, supPrivateFlow.Width - 50));
                    supPrivateFlow.Controls.Add(msg);


                }
                else
                {
                    msg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, supFlow.Width - 50));

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
                msg.Controls.Add(webviewer);

                try { await webviewer.EnsureCoreWebView2Async(); } catch { }
                // immediately load Progress content into the WebView2 control
                try { webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\includes\loading.html"); } catch { }


                if (!videopath.ToLower().StartsWith("http"))
                {
                    videolocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + videopath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (videopath.ToLower().StartsWith("ipfs:")) { videolocation = videolocation.Replace(@"\root\", @"\ipfs\"); if (videopath.Length == 51) { videolocation += @"\artifact"; } }


                    if (!File.Exists(videolocation))
                    {



                        Task.Run(() =>
                        {
                            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                            Match imgurnmatch = regexTransactionId.Match(videolocation);
                            string transactionid = imgurnmatch.Value;
                            Root root = new Root();

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
                                        try
                                        {
                                            Directory.CreateDirectory("ipfs/" + transid);
                                        }
                                        catch { };

                                        Directory.CreateDirectory("ipfs/" + transid + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + videopath.Substring(5, 46) + @" -o ipfs\" + transid;
                                        process2.StartInfo.UseShellExecute = false;
                                        process2.StartInfo.CreateNoWindow = true;
                                        process2.Start();
                                        if (process2.WaitForExit(550000))
                                        {
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

                                            try
                                            {
                                                if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                            catch { }
                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                        }
                                        else
                                        {
                                            process2.Kill();

                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                        }



                                    }

                                    break;

                                default:
                                    if (!videopath.ToUpper().StartsWith("HTTP") && transactionid != "")
                                    {
                                        Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                    }
                                    break;
                            }

                            if (File.Exists(videolocation))
                            {
                                if (videolocation.ToLower().EndsWith(".mov"))
                                {
                                    string inputFilePath = videolocation;
                                    string outputFilePath = System.IO.Path.ChangeExtension(inputFilePath, ".mp4");
                                    if (!File.Exists(outputFilePath))
                                    {
                                        try
                                        {
                                            var ffMpeg = new FFMpegConverter();
                                            ffMpeg.ConvertMedia(inputFilePath, outputFilePath, Format.mp4);
                                            videolocation = outputFilePath;
                                        }
                                        catch { }
                                    }
                                    else { videolocation = outputFilePath; }
                                }

                                this.Invoke((Action)(() =>
                                {
                                    // Check if videolocation is valid before using it
                                    if (!string.IsNullOrEmpty(videolocation))
                                    {
                                        if ((videolocation.ToLower().EndsWith(".wav") || videolocation.ToLower().EndsWith(".mp3")) && autoPlay)
                                        {
                                            audioPlayer.AddToPlaylist(videolocation);

                                        }
                                        string encodedPath = "file:///" + Uri.EscapeUriString(videolocation.Replace('\\', '/'));
                                        string viewerPath = Path.GetDirectoryName(videolocation) + @"\urnviewer.html";
                                        string htmlstring = $"<html><body><embed src=\"{encodedPath}\" width=100% height=100% ></body></html>";

                                        System.IO.File.WriteAllText(viewerPath, htmlstring);

                                        try { webviewer.CoreWebView2.Navigate(viewerPath); } catch { }
                                    }


                                }));


                            }
                            else
                            {
                                try { webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\includes\notfound.html"); } catch { }

                            }



                        });




                    }
                    else
                    {

                        if ((videolocation.ToLower().EndsWith(".wav") || videolocation.ToLower().EndsWith(".mp3")) && autoPlay)
                        {
                            audioPlayer.AddToPlaylist(videolocation);
                        }

                        if (videolocation.ToLower().EndsWith(".mov"))
                        {
                            string inputFilePath = videolocation;
                            string outputFilePath = System.IO.Path.ChangeExtension(inputFilePath, ".mp4");
                            videolocation = outputFilePath; 
                        }
                        string encodedPath = "file:///" + Uri.EscapeUriString(videolocation.Replace('\\', '/'));
                        string viewerPath = Path.GetDirectoryName(videolocation) + @"\urnviewer.html";
                        string htmlstring = $"<html><body><embed src=\"{encodedPath}\" width=100% height=100% ></body></html>";


                        System.IO.File.WriteAllText(viewerPath, htmlstring);
                        try { webviewer.CoreWebView2.Navigate(viewerPath); } catch { }
                    }


                }
                else
                {
                    string pattern = @"(?:youtu\.be/|youtube(?:-nocookie)?\.com/(?:[^/\n\s]*[/\n\s]*(?:v/|e(?:mbed)?/|.*[?&]v=))?)?([a-zA-Z0-9_-]{11})";

                    Match match = Regex.Match(videopath, pattern);
                    if (match.Success)
                    {
                        videolocation = @"https://www.youtube.com/embed/" + match.Groups[1].Value;
                    }

                    try { webviewer.CoreWebView2.Navigate(videolocation); } catch { }
                }



            }


        }


        void CreateRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp, string messageText, string transactionid, bool isprivate, FlowLayoutPanel layoutPanel, bool addtoTop = false)
        {


            if (!isprivate)
            {
                // Create a table layout panel for each row
                TableLayoutPanel row = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 5,
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };
                // Add the width of the first column to fixed value and second to fill remaining space
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));


                // Create a PictureBox with the specified image

                if (File.Exists(imageLocation) || imageLocation.ToUpper().StartsWith("HTTP"))
                {
                    PictureBox picture = new PictureBox
                    {
                        Size = new System.Drawing.Size(50, 50),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        ImageLocation = imageLocation,
                        Margin = new System.Windows.Forms.Padding(0)

                    };
                    picture.Click += (sender, e) => { profileImageClick(ownerId); };
                    row.Controls.Add(picture, 0, 0);
                    //pictures.Add(picture);
                }
                else
                {
                    string errorImageUrl = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "includes\\anon.png");


                    PictureBox picture = new PictureBox
                    {
                        Size = new System.Drawing.Size(50, 50),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        ImageLocation = errorImageUrl,
                        Margin = new System.Windows.Forms.Padding(0),
                    };
                    picture.Click += (sender, e) => { profileImageClick(ownerId); };
                    row.Controls.Add(picture, 0, 0);
                    //pictures.Add(picture);
                }


                // Create a LinkLabel with the owner name
                LinkLabel owner = new LinkLabel
                {
                    Text = ownerName,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true

                };
                owner.LinkClicked += (sender, e) => { Owner_LinkClicked(sender, e, ownerId, imageLocation); };
                owner.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                owner.Margin = new System.Windows.Forms.Padding(3);
                owner.Dock = DockStyle.Bottom;
                row.Controls.Add(owner, 1, 0);

                if (timestamp.Year > 1975)
                {
                    // Create a LinkLabel with the owner name
                    Label tstamp = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.Gray,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };
                    row.Controls.Add(tstamp, 2, 0);


                    Label loveme = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.Red,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = "🤍",
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };

                    loveme.Click += (sender, e) => { Owner_LinkClicked(sender, e, ownerId, imageLocation, transactionid);
                    if (profileOwner.Tag == null) { MessageBox.Show("Search for a local user to login.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning); } else { loveme.ForeColor = Color.Blue; } };


                    row.Controls.Add(loveme, 3, 0);
                    Task.Run(() =>
                    {

                        Root[] roots = Root.GetRootsByAddress(Root.GetPublicAddressByKeyword(transactionid, mainnetVersionByte), mainnetLogin, mainnetPassword, mainnetURL, 0, -1, mainnetVersionByte);
                        if (roots != null && roots.Length > 0)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                loveme.Text = "🖤 " + roots.Length.ToString();
                                loveme.ForeColor = Color.Red;
                                loveme.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            });
                        }

                    });


                    Label deleteme = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.White,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = "🗑",
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };
                    deleteme.Click += (sender, e) =>
                    {
                        deleteme_LinkClicked(transactionid);
                        deleteme.ForeColor = Color.Black;
                    };
                    row.Controls.Add(deleteme, 4, 0);
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
            }
            else
            {

                // Create a table layout panel for each row
                TableLayoutPanel row = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 2,
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };
                // Add the width of the first column to fixed value and second to fill remaining space
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));

                // Create a table layout panel for each row
                TableLayoutPanel row2 = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 2,
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };
                row2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
                row2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));


                // Create a PictureBox with the specified image

                if (File.Exists(imageLocation) || imageLocation.ToUpper().StartsWith("HTTP"))
                {
                    PictureBox picture = new PictureBox
                    {
                        Size = new System.Drawing.Size(50, 50),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        ImageLocation = imageLocation,
                        Margin = new System.Windows.Forms.Padding(0)

                    };
                    picture.Click += (sender, e) => { profileImageClick(ownerId); };
                    row.Controls.Add(picture, 0, 0);
                    //pictures.Add(picture);
                }
                else
                {
                    string errorImageUrl = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "includes\\anon.png");


                    PictureBox picture = new PictureBox
                    {
                        Size = new System.Drawing.Size(50, 50),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        ImageLocation = errorImageUrl,
                        Margin = new System.Windows.Forms.Padding(0),
                    };
                    picture.Click += (sender, e) => { profileImageClick(ownerId); };
                    row.Controls.Add(picture, 0, 0);
                    //pictures.Add(picture);
                }


                // Create a LinkLabel with the owner name
                LinkLabel owner = new LinkLabel
                {
                    Text = ownerName,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true

                };
                owner.LinkClicked += (sender, e) => { Owner_LinkClicked(sender, e, ownerId, imageLocation); };
                owner.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                owner.Margin = new System.Windows.Forms.Padding(3);
                owner.Dock = DockStyle.Bottom;
                row.Controls.Add(owner, 1, 0);

                if (timestamp.Year > 1975)
                {
                    // Create a LinkLabel with the owner name
                    Label tstamp = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.Gray,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };
                    row2.Controls.Add(tstamp, 0, 0);




                    Label deleteme = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.White,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = "🗑",
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };
                    deleteme.Click += (sender, e) =>
                    {
                        deleteme_LinkClicked(transactionid);
                        deleteme.ForeColor = Color.Black;
                    };
                    row2.Controls.Add(deleteme, 1, 0);
                }

                if (addtoTop)
                {

                    layoutPanel.Controls.Add(row);
                    layoutPanel.Controls.SetChildIndex(row, 0);
                    layoutPanel.Controls.Add(row2);
                    layoutPanel.Controls.SetChildIndex(row, 1);
                }
                else
                {
                    layoutPanel.Controls.Add(row);
                    layoutPanel.Controls.Add(row2);

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
                Margin = new System.Windows.Forms.Padding(0),
                Padding = new System.Windows.Forms.Padding(0, 0, 0, 0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None

            };
            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, layoutPanel.Width - 50));


            if (addtoTop)
            {
                layoutPanel.Controls.Add(msg);
                layoutPanel.Controls.SetChildIndex(msg, 1);
            }
            else
            {
                layoutPanel.Controls.Add(msg);
            }

            if (messageText != "")
            {
                Label message = new Label
                {
                    AutoSize = true,
                    Text = messageText,
                    MinimumSize = new Size(200, 46),
                    Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Margin = new System.Windows.Forms.Padding(0),
                    Padding = new System.Windows.Forms.Padding(10, 20, 10, 20),
                    TextAlign = System.Drawing.ContentAlignment.TopLeft
                };

                msg.Controls.Add(message, 1, 0);
            }
            else
            {

                Label message = new Label
                {
                    AutoSize = true,
                    Text = messageText,
                    Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Margin = new System.Windows.Forms.Padding(0),
                    Padding = new System.Windows.Forms.Padding(0, 0, 0, 0),
                    TextAlign = System.Drawing.ContentAlignment.TopLeft
                };

                msg.Controls.Add(message, 1, 0);

            }


        }

        void Owner_LinkClicked(object sender, EventArgs e, string ownerId, string imageLocation, string transactionId = "")
        {

            MouseEventArgs me = e as MouseEventArgs;
            bool isTestnet = true;

            if (mainnetVersionByte == "0") { isTestnet = false; }

            if (me != null)
            {
                if (me.Button == MouseButtons.Left)
                {



                    if (profileOwner.Tag != null)
                    {
                        if (transactionId != "")
                        {
                            ownerId = Root.GetPublicAddressByKeyword(transactionId, mainnetVersionByte);
                        }

                        DiscoBall disco = new DiscoBall(profileOwner.Tag.ToString(), profileOwner.ImageLocation, ownerId, "includes\\Hugpuddle.jpg", false, isTestnet);
                        disco.StartPosition = FormStartPosition.CenterScreen;
                        disco.Show(this);
                        disco.Focus();
                    }

                }
                // Check if the right mouse button was clicked
                else if (me.Button == MouseButtons.Right && transactionId != "")
                {
                    // Code to execute for right click
                    profileURN.Links[0].LinkData = Root.GetPublicAddressByKeyword(transactionId, mainnetVersionByte);
                    string activeUser = "";
                    string activeUserImage = "";

                    try
                    {
                        activeUser = profileOwner.Tag.ToString();
                        activeUserImage = profileOwner.ImageLocation;
                    }
                    catch { }


                    SupThread thread = new SupThread(Root.GetPublicAddressByKeyword(transactionId, mainnetVersionByte), activeUser, activeUserImage, isTestnet);
                    thread.StartPosition = FormStartPosition.CenterScreen;
                    thread.Show(this);
                    thread.Focus();


                }
            }
            else
            {
                bool isprivate = false;
                if (btnPrivateMessage.BackColor == Color.Blue) { isprivate = true; }


                if (profileOwner.Tag != null)
                {
                    DiscoBall disco = new DiscoBall(profileOwner.Tag.ToString(), profileOwner.ImageLocation, ownerId, imageLocation, isprivate, isTestnet);
                    disco.StartPosition = FormStartPosition.CenterScreen;
                    disco.Show(this);
                    disco.Focus();
                }
            }







        }

        void profileImageClick(string ownerId)
        {
            numMessagesDisplayed = 0;
            numPrivateMessagesDisplayed = 0;
            numFriendFeedsDisplayed = 0;
            if (btnCommunityFeed.BackColor == Color.Blue) { btnCommunityFeed.BackColor = Color.White; btnCommunityFeed.ForeColor = Color.Black; btnPublicMessage.BackColor = Color.Blue; btnPublicMessage.ForeColor = Color.Yellow; }
            friendClicked = true;
            MakeActiveProfile(ownerId);
            RefreshSupMessages();
        }

        void Attachment_Clicked(string path)
        {
            if (Regex.IsMatch(path, "^[1-9A-HJ-NP-Za-km-z]+$") || path.ToUpper().StartsWith("@") || path.ToUpper().StartsWith("SUP:") || path.ToUpper().StartsWith("IPFS:") || path.ToUpper().StartsWith("BTC:") || path.ToUpper().StartsWith("MZC:") || path.ToUpper().StartsWith("LTC:") || path.ToUpper().StartsWith("DOG:"))
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
            try
            {
                string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transactionid + @"/ROOT.json");
                Root DeleteRoot = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                try { unfilteredmessage = DeleteRoot.Message.FirstOrDefault().ToString(); } catch { }
                foreach (string keyword in DeleteRoot.Keyword.Keys) { try { File.Delete(@"root/" + keyword + @"/ROOTS.json"); } catch { } }
            }
            catch { }


            string pattern = "<<.*?>>";
            MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
            foreach (Match match in matches)
            {
                string content = match.Value.Substring(2, match.Value.Length - 4);
                if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id) && !content.Trim().StartsWith("#"))
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

            try { Directory.Delete(@"root\" + transactionid, true); } catch { }
            try { Directory.CreateDirectory(@"root\" + transactionid); } catch { }
            Root P2FKRoot = new Root();
            P2FKRoot.Confirmations = 1;
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

        private void splitContainer1_DoubleClick(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel2Collapsed = false;
                btnMint.Text = "💎";

            }
            else
            {

                splitContainer1.Panel2Collapsed = true;
                btnMint.Text = "🔍";
            }
        }

        private void profileURN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            string ActiveProfile = "";
            if (profileURN.Links[0].LinkData != null) { ActiveProfile = profileURN.Links[0].LinkData.ToString(); }
            try { new ProfileMint(ActiveProfile, testnet).Show(); } catch { }
        }

        private void btnPublicMessage_Click(object sender, EventArgs e)
        {
            if (btnPublicMessage.BackColor != Color.Blue) { ClearMessages(supFlow); }

            btnCommunityFeed.BackColor = System.Drawing.Color.White;
            btnCommunityFeed.ForeColor = System.Drawing.Color.Black;

            RefreshSupMessages();
            if (btnPublicMessage.BackColor == Color.White)
            {
                btnPublicMessage.BackColor = Color.Blue; btnPublicMessage.ForeColor = Color.Yellow;

                btnPrivateMessage.BackColor = Color.White;
                btnPrivateMessage.ForeColor = Color.Black;
            }

        }

        private void RefreshCommunityMessages_Click(object sender, EventArgs e)
        {
            if (btnCommunityFeed.BackColor != Color.Blue) { ClearMessages(supFlow); }
            btnCommunityFeed.BackColor = System.Drawing.Color.Blue; btnCommunityFeed.ForeColor = System.Drawing.Color.Yellow;
            RefreshCommunityMessages();

            btnPrivateMessage.BackColor = System.Drawing.Color.White;
            btnPrivateMessage.ForeColor = System.Drawing.Color.Black;
            btnPublicMessage.BackColor = System.Drawing.Color.White;
            btnPublicMessage.ForeColor = System.Drawing.Color.Black;

        }

        private void btnPrivateMessage_Click(object sender, EventArgs e)
        {

            if (btnPrivateMessage.BackColor != Color.Blue) { ClearMessages(supPrivateFlow); }

            btnPrivateMessage.BackColor = Color.Blue;
            btnPrivateMessage.ForeColor = Color.Yellow;
            btnMint.Text = "🔍";

            RefreshPrivateSupMessages();


            if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel2Collapsed = false;
            }

            btnPublicMessage.BackColor = Color.White;
            btnPublicMessage.ForeColor = Color.Black;
            btnCommunityFeed.BackColor = Color.White;
            btnCommunityFeed.ForeColor = Color.Black;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string profileowner = "";
            string toaddress = "";
            if (profileOwner.Tag != null) { profileowner = profileOwner.Tag.ToString(); }
            if (profileURN.Links[0].LinkData != null) { toaddress = profileURN.Links[0].LinkData.ToString(); }
            bool isprivate = false;

            if (btnPrivateMessage.BackColor == Color.Blue) { isprivate = true; }

            DiscoBall disco = new DiscoBall(profileowner, profileOwner.ImageLocation, toaddress, profileIMG.ImageLocation, isprivate,testnet);
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
            try { pictureBox.Tag = profileURN.Links[0].LinkData.ToString(); } catch { return; }
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Width = 50;
            pictureBox.Height = 50;
            pictureBox.ImageLocation = profileIMG.ImageLocation;

            // Add event handlers to the PictureBox
            pictureBox.Click += new EventHandler(Friend_Click);
            pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(Friend_MouseUp);

            try
            {
                friendDict.Add(profileURN.Links[0].LinkData.ToString(), profileIMG.ImageLocation);
                flowFollow.Controls.Add(pictureBox);
            }
            catch { }
            // Add the PictureBox to the FlowLayoutPanel


            string json = JsonConvert.SerializeObject(friendDict);
            string filePath = @"root\MyFriendList.Json";
            if (!testnet) { filePath = @"root\MyProdFriendList.Json"; }
            File.WriteAllText(filePath, json);

        }

        private void Friend_Click(object sender, EventArgs e)
        {
            friendClicked = true;
            lblOfficial.Visible = false;
            //if any current searches are loading you got to wait.  
            if (!btnPrivateMessage.Enabled || !btnPublicMessage.Enabled || !btnCommunityFeed.Enabled || ((PictureBox)sender).ImageLocation == null || System.IO.File.Exists("ROOTS-PROCESSING"))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            //make profile card visible
            if (!panel1.Visible)
            {
                panel1.Visible = true;
                supFlow.Location = new System.Drawing.Point(supFlow.Location.X, supFlow.Location.Y + 150); // Change the X and Y coordinates
                supFlow.Size = new System.Drawing.Size(supFlow.Width, supFlow.Height - 150); // Change the width and height
            }

            // Check if the user left-clicked on the PictureBox
            if (((System.Windows.Forms.MouseEventArgs)e).Button == MouseButtons.Left)
            {



                if (!((PictureBox)sender).ImageLocation.Contains("anon.png") || profileURN.Text != "anon")
                {
                    numMessagesDisplayed = 0;
                    numFriendFeedsDisplayed = 0;
                    numPrivateMessagesDisplayed = 0;
                    btnCommunityFeed.BackColor = System.Drawing.Color.White;
                    btnCommunityFeed.ForeColor = System.Drawing.Color.Black;
                    btnPrivateMessage.BackColor = System.Drawing.Color.White;
                    btnPrivateMessage.ForeColor = System.Drawing.Color.Black;
                    btnPublicMessage.BackColor = Color.Blue;
                    btnPublicMessage.ForeColor = Color.Yellow;




                    if (!((PictureBox)sender).ImageLocation.ToString().Contains(@"root\keywords"))
                    {
                        // Get the tag text from the PictureBox
                        string address = null;
                        try { address = ((PictureBox)sender).Tag.ToString(); } catch { }

                        if (address != null)
                        {

                            MakeActiveProfile(address);

                            Task obControlTask = Task.Run(() =>
                            {
                                try
                                {
                                    this.BeginInvoke((MethodInvoker)delegate
                                 {
                                     OBcontrol.control.txtSearchAddress.Text = profileURN.Text;
                                     OBcontrol.control.BuildSearchResults();
                                 });
                                }
                                catch { }
                            });


                        }
                    }
                    else
                    {

                        profileBIO.Text = ""; profileCreatedDate.Text = ""; profileIMG.ImageLocation = ""; lblProcessHeight.Text = "";
                        profileURN.Links[0].LinkData = ((PictureBox)sender).Tag.ToString();
                        profileURN.Text = Path.GetFileNameWithoutExtension(((PictureBox)sender).ImageLocation.ToString());
                        profileIMG.ImageLocation = ((PictureBox)sender).ImageLocation.ToString();

                    }

                    RefreshSupMessages();


                }



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
                if (!testnet) { filePath = @"root\MyProdFriendList.Json"; }
                File.WriteAllText(filePath, json);
                try { File.Delete(filePath); } catch { }
                numFriendFeedsDisplayed = 0;
            }
        }

        private void btnMute_Click(object sender, EventArgs e)
        {

            if (btnMute.Text == "mute")
            {

                try
                {
                    using (FileStream fs = File.Create(@"root\" + profileURN.Links[0].LinkData.ToString() + @"\MUTE"))
                    {

                    }
                }
                catch { }

                btnMute.Text = "unmute";
            }
            else
            {
                try { File.Delete(@"root\" + profileURN.Links[0].LinkData.ToString() + @"\MUTE"); } catch { }
                btnMute.Text = "mute";
            }
        }

        private void btnBlock_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Are you sure!? Blocking will attempt to remove all associated files!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {

                try
                {


                    var keysToDelete = new HashSet<string>(); // Create a new HashSet to store the keys to delete




                    Root[] root = Root.GetRootsByAddress(profileURN.Links[0].LinkData.ToString(), mainnetLogin, mainnetPassword, mainnetURL, 0, -1, mainnetVersionByte);

                    foreach (Root rootItem in root)
                    {

                        try
                        {
                            Directory.Delete(@"root\" + rootItem.TransactionId, true);
                        }
                        catch { }

                        foreach (string key in rootItem.Keyword.Keys)
                        {
                            try { Directory.Delete(@"root\" + key, true); } catch { }
                        }

                    }


                    try { Directory.Delete(@"root\" + profileURN.Text.Replace("#", ""), true); } catch { }

                    try { Directory.Delete(@"root\" + profileURN.Links[0].LinkData.ToString(), true); } catch { }
                    try { Directory.CreateDirectory(@"root\" + profileURN.Links[0].LinkData.ToString()); } catch { }
                    try
                    {
                        using (FileStream fs = File.Create(@"root\" + profileURN.Links[0].LinkData.ToString() + @"\BLOCK"))
                        {

                        }
                    }
                    catch { }

                    foreach (Control control in flowFollow.Controls)
                    {
                        if (control is PictureBox pictureBox && pictureBox.Tag.ToString() == profileURN.Links[0].LinkData.ToString())
                        {
                            flowFollow.Controls.Remove(pictureBox);
                        }
                    }

                    //remove from follow list
                    Dictionary<string, string> friendDict = new Dictionary<string, string>();
                    foreach (PictureBox pb in flowFollow.Controls)
                    {

                        try { friendDict.Add(pb.Tag.ToString(), pb.ImageLocation); } catch { }
                    }
                    string json = JsonConvert.SerializeObject(friendDict);
                    string filePath = @"root\MyFriendList.Json";
                    File.WriteAllText(filePath, json);

                    if (panel1.Visible)
                    {
                        panel1.Visible = false;
                        supFlow.Location = new System.Drawing.Point(supFlow.Location.X, supFlow.Location.Y - 150); // Change the X and Y coordinates
                        supFlow.Size = new System.Drawing.Size(supFlow.Width, supFlow.Height + 150); // Change the width and height
                    }
                    profileBIO.Text = ""; profileCreatedDate.Text = ""; profileIMG.ImageLocation = ""; lblProcessHeight.Text = "";
                    profileURN.Links[0].LinkData = null;
                    profileURN.Text = "anon";
                    ClearMessages(supFlow);

                }
                catch { }
            }


        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            friendClicked = false;
            try
            {

                MakeActiveProfile(profileURN.Links[0].LinkData.ToString());
                numMessagesDisplayed = 0;
                btnCommunityFeed.BackColor = System.Drawing.Color.White;
                btnCommunityFeed.ForeColor = System.Drawing.Color.Black;
            }
            catch { }
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
                    Font font = null;

                    // Calculate the font size dynamically based on the image size and text length
                    while (true)
                    {
                        font?.Dispose();
                        font = new Font("Segoe UI Emoji", fontSize);
                        SizeF textSize = graphics.MeasureString(text, font, width);

                        if (textSize.Width < width && textSize.Height < height)
                            break;

                        fontSize -= 1;
                    }

                    StringFormat stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    // Draw the text with word wrap
                    RectangleF rect = new RectangleF(0, 0, width, height);
                    graphics.DrawString(text, font, Brushes.Black, rect, stringFormat);
                    string hashedString = "";
                    // get a hash of the text for image storage.
                    byte[] bytes = Encoding.ASCII.GetBytes(text);

                    // Create a SHA256 hash object
                    using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
                    {
                        // Compute hash value from the input
                        byte[] hashBytes = sha256Hash.ComputeHash(bytes);

                        // Convert byte array to a string representation
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < hashBytes.Length; i++)
                        {
                            stringBuilder.Append(hashBytes[i].ToString("x2"));
                        }
                        hashedString = stringBuilder.ToString();

                    }
                    // Save the image to the specified folder
                    string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\keywords\" + hashedString + ".png";
                    bmp.Save(filePath, ImageFormat.Png);
                }
            }
        }



        private void btnJukeBox_Click(object sender, EventArgs e)
        {

            btnJukeBox.Enabled = false;

            if (profileURN.Links[0].LinkData != null)
            {
                JukeBox jukeBoxForm = new JukeBox(profileURN.Text, testnet);
                jukeBoxForm.Show();
            }
            else
            {
                JukeBox jukeBoxForm = new JukeBox(null, testnet);
                jukeBoxForm.Show();
            }
            btnJukeBox.Enabled = true;

        }

        private void btnSkipAudio_Click(object sender, EventArgs e)
        {
            audioPlayer.SkipCurrent();
        }

        private void btnVideoSearch_Click(object sender, EventArgs e)
        {

            btnVideoSearch.Enabled = false;

            if (profileURN.Links[0].LinkData != null)
            {
                SupFlix supFlixForm = new SupFlix(profileURN.Text);
                supFlixForm.Show();
            }
            else
            {
                SupFlix supFlixForm = new SupFlix();
                supFlixForm.Show();
            }

            btnVideoSearch.Enabled = true;
        }

        private void btnInquirySearch_Click(object sender, EventArgs e)
        {
            btnVideoSearch.Enabled = false;

            if (profileURN.Links[0].LinkData != null)
            {
                INQSearch INQSearchForm = new INQSearch(profileURN.Text);
                INQSearchForm.Show();
            }
            else
            {
                INQSearch INQSearchForm = new INQSearch();
                INQSearchForm.Show();
            }

            btnVideoSearch.Enabled = true;
        }

        private void SupMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"ipfs\ipfs.exe",
                    Arguments = "shutdown",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();

            try { System.IO.File.Delete("ROOTS-PROCESSING"); } catch { }

        }

        private void imgBTCSwitch_Click(object sender, EventArgs e)
        {
            profileURN.Text = "anon"; profileBIO.Text = ""; profileCreatedDate.Text = ""; profileIMG.ImageLocation = @"includes/anon.png"; lblProcessHeight.Text = ""; profileURN.Links[0].LinkData = null; profileURN.Links[0].Tag = ""; profileIMG.Tag = ""; profileOwner.ImageLocation = null; profileOwner.Tag = null;
            ClearMessages(supFlow);
            ClearMessages(supPrivateFlow);
            numMessagesDisplayed = 0;
            numPrivateMessagesDisplayed = 0;
            numFriendFeedsDisplayed = 0;
            lblOfficial.Visible = false;

            if (btnMainnetSwitch.ImageLocation == @"includes/BCT_Logo.png")
            {
                btnMainnetSwitch.ImageLocation = @"includes/BC_Logo.png";
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";
                testnet = false;
                OBcontrol = new ObjectBrowserControl("", true, false);
                OBcontrol.Dock = DockStyle.Fill;
                OBcontrol.ProfileURNChanged += OBControl_ProfileURNChanged;
                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Panel2.Controls.Add(OBcontrol);

                // Read the JSON data from the file
                string filePath = @"root\MyProdFriendList.Json";
                try
                {
                    flowFollow.Controls.Clear();
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
            else
            {
                btnMainnetSwitch.ImageLocation = @"includes/BCT_Logo.png";
                mainnetURL = @"http://127.0.0.1:18332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "111";
                testnet = true;
                OBcontrol = new ObjectBrowserControl("", true, true);
                OBcontrol.Dock = DockStyle.Fill;
                OBcontrol.ProfileURNChanged += OBControl_ProfileURNChanged;
                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Panel2.Controls.Add(OBcontrol);

                string filePath = @"root\MyFriendList.Json";
                try
                {
                    flowFollow.Controls.Clear();
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
        }

        private void profileIMG_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Combine the application's startup path with the relative path
                string errorImageUrl = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "includes\\anon.png");

                // Load the image
                profileIMG.ImageLocation = errorImageUrl;
            }
        }




    }
}
