using AngleSharp.Text;
using Ganss.Xss;
using NAudio.Wave;
using NBitcoin;
using Newtonsoft.Json;
using NReco.VideoConverter;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.NBitcoin;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace SUP
{
    public partial class ObjectDetails : Form
    {
        private readonly string _objectaddress;
        private bool isVerbose = false;
        private int numMessagesDisplayed = 0;
        private int numChangesDisplayed = 0;
        private bool isUserControl = false;
        private string _activeprofile;
        private string _genid;
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        private bool _testnet = true;
        private bool isDiscoBallOpen = false;

        List<Microsoft.Web.WebView2.WinForms.WebView2> webviewers = new List<Microsoft.Web.WebView2.WinForms.WebView2>();

        public ObjectDetails(string objectaddress, string activeprofile, bool isusercontrol = false, bool testnet = true)
        {
            InitializeComponent();
            _objectaddress = objectaddress;
            isUserControl = isusercontrol;
            _activeprofile = activeprofile;
            supFlow.MouseWheel += supFlow_MouseWheel;

            if (!testnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";
            }
            _testnet = testnet;

            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
            myTooltip.SetToolTip(btnJukeBox, "click 🎵 to open the jukebox audio searching tool.\nthe active object's public messages will be searched by default.");
            myTooltip.SetToolTip(btnRefreshSup, "click 📣 to to view the active object's public messages and open the sup direct messaging panel.\nthe to: field is prepopulated with the active object's address.\nnote: search for your own local profile first to prepopulate the from: field.");
            myTooltip.SetToolTip(btnInquiry, "click ⁉️ to open the sup poll searching tool.\nthe active object will be searched by default.");
            myTooltip.SetToolTip(btnSupFlix, "click 🎬 to open the supflix video searching tool.\nthe active object's public messages will be searched by default.");
            myTooltip.SetToolTip(lblProcessHeight, "the total amount of transactions associated with the object.\nincludes comments as well as object listings, gives, burns and buys.");
            myTooltip.SetToolTip(btnReloadObject, "click ♻️ to refresh the object's details.\n");
            myTooltip.SetToolTip(btnBuy, "click ⚡️ to open the object's marketplace.\n this is where you can list and buy the object.");
            myTooltip.SetToolTip(btnBurn, "click 🔥 to burn the object.\nburn all units of the object and it will no longer be discoverable.");
            myTooltip.SetToolTip(btnGive, "click 💞 to give the object.\n");
            myTooltip.SetToolTip(btnRefreshTransactions, "click 🔍 to show the object's most recent provenance.\nprovenance is a historical record of all object state changes\n click 🔍 again to load additional provenance records.");
            myTooltip.SetToolTip(lblLaunchURI, "click to launch the associated URI\nnote: this is potentially dangerous. verify the URI before clicking.\nthis button could be used to launch a local script.");
            myTooltip.SetToolTip(btnLaunchURN, "click to view the active URN using your systems default viewer.\nthis allows fullscreen mode for video and html based content.\nnote: this is potentially dangerous. verify the URN before clicking.\nthis button could be used to launch a local script.");
            myTooltip.SetToolTip(chkRunTrustedObject, "check this box to allow the object to execute scripts.\nclick ♻️ to reload the object URN with script execution activated.\nnote: this is potentially dangerous. verify the object's publisher and URN before clicking.\nexecuting local scripts could be harmful to your computer.");

        }

        private void ObjectDetails_Load(object sender, EventArgs e)
        {



            this.Text = String.Empty;

            if (!isUserControl)
            {
                this.Text = "[ " + _objectaddress + " ]";
                registrationPanel.Visible = true;
            }


            btnReloadObject.PerformClick();
            lblPleaseStandBy.Visible = false;

        }

        private void supFlow_MouseWheel(object sender, MouseEventArgs e)
        {
            if (supFlow.VerticalScroll.Value + supFlow.ClientSize.Height >= supFlow.VerticalScroll.Maximum)
            {
                // Add more PictureBoxes if available              

                if (btnRefreshSup.BackColor == System.Drawing.Color.Blue)
                { RefreshSupMessages(); }
                else { btnRefreshSup.PerformClick(); }

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
                if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id))
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

        private string TruncateAddress(string input)
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

        private void ShowFullScreenModeClick(object sender, EventArgs e)
        {
            new FullScreenView(pictureBox1.ImageLocation).Show();
        }

        private void LaunchURN(object sender, EventArgs e)
        {
            string src = lblURNFullPath.Text;

            // Show a confirmation dialog
            DialogResult result = MessageBox.Show("Are you sure!? Launching files executes locally!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Check if src ends with "index.zip"
                    if (src.EndsWith("index.zip", StringComparison.OrdinalIgnoreCase))
                    {
                        // Change the file extension to ".html"
                        src = src.Substring(0, src.Length - "index.zip".Length) + "index.html";
                    }

                    System.Diagnostics.Process.Start(src);
                }
                catch
                {
                    System.Media.SystemSounds.Exclamation.Play();
                }
            }


        }

        private void LinkClicked(object sender, MouseEventArgs e)
        {
            var linkLabel = (LinkLabel)sender;
            var linkData = linkLabel.Links[0].LinkData;

            if (e.Button == MouseButtons.Left)
            {
                // Perform left-click action
                new ObjectBrowser((string)linkData, false, _testnet).Show();
            }
            else if (e.Button == MouseButtons.Right)
            {
                _activeprofile = (string)linkData;
                btnReloadObject.PerformClick();

            }
        }


        private void RefreshOwners()
        {
            CreatorsPanel.SuspendLayout();
            OwnersPanel.SuspendLayout();
            CreatorsPanel.Controls.Clear();
            OwnersPanel.Controls.Clear();
            RoyaltiesPanel.Controls.Clear();
            supPanel.Visible = false;
            btnRefreshSup.BackColor = Color.White;
            btnRefreshSup.ForeColor = Color.Black;

            numMessagesDisplayed = 0;


            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
            Dictionary<string, string> profileAddress = new Dictionary<string, string> { };


            if (objstate.Owners != null)
            {


                OwnersPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                OwnersPanel.AutoScroll = true;

                int row = 0;
                foreach (var kvp in objstate.Owners)
                {
                    TableLayoutPanel rowPanel = new TableLayoutPanel
                    {
                        RowCount = 1,
                        ColumnCount = 2,
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Padding = new System.Windows.Forms.Padding(3)
                    };

                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 310));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

                    LinkLabel keyLabel = new LinkLabel();
                    string searchkey = kvp.Key;

                    if (!profileAddress.ContainsKey(searchkey))
                    {
                        PROState profile = PROState.GetProfileByAddress(searchkey, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                        if (profile.URN != null)
                        {
                            PROState URNHolder = PROState.GetProfileByURN(profile.URN, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                            if (URNHolder.Creators[0] == searchkey) { keyLabel.Text = profile.URN; }else { keyLabel.Text = kvp.Key; }
                        }
                        else
                        {
                            keyLabel.Text = kvp.Key;
                        }
                        profileAddress.Add(searchkey, keyLabel.Text);
                    }
                    else
                    {
                        profileAddress.TryGetValue(searchkey, out string ShortName);
                        keyLabel.Text = ShortName;
                    }

                    keyLabel.Links[0].LinkData = kvp.Key;
                    keyLabel.AutoSize = true;
                    keyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    keyLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                    keyLabel.LinkColor = System.Drawing.Color.Black;
                    keyLabel.ActiveLinkColor = System.Drawing.Color.Black;
                    keyLabel.VisitedLinkColor = System.Drawing.Color.Black;
                    keyLabel.MouseClick += new MouseEventHandler(LinkClicked);
                    keyLabel.Dock = DockStyle.Left;

                    Label valueLabel = new Label
                    {
                        Text = kvp.Value.Item1.ToString("N0"),
                        AutoSize = true,
                        Dock = DockStyle.Right
                    };

                    rowPanel.Controls.Add(keyLabel, 0, 0);
                    rowPanel.Controls.Add(valueLabel, 1, 0);

                    if (row % 2 == 0)
                    {
                        rowPanel.BackColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        rowPanel.BackColor = System.Drawing.Color.LightGray;
                    }

                    OwnersPanel.Controls.Add(rowPanel);
                    row++;
                }


                long totalQty = objstate.Owners.Values.Sum(tuple => tuple.Item1);

                lblTotalOwnedDetail.Text = "total: " + totalQty.ToString("N0");


                ///royalties 

                if (objstate.Royalties != null)
                {


                    RoyaltiesPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                    RoyaltiesPanel.AutoScroll = true;

                    row = 0;
                    foreach (KeyValuePair<string, decimal> item in objstate.Royalties)
                    {


                        TableLayoutPanel rowPanel = new TableLayoutPanel
                        {
                            RowCount = 1,
                            ColumnCount = 2,
                            Dock = DockStyle.Top,
                            AutoSize = true,
                            Padding = new System.Windows.Forms.Padding(3)
                        };

                        rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 310));
                        rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));


                        LinkLabel keyLabel = new LinkLabel();


                        string searchkey = item.Key;


                        if (!profileAddress.ContainsKey(searchkey))
                        {

                            PROState profile = PROState.GetProfileByAddress(searchkey, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                            if (profile.URN != null)
                            {
                                keyLabel.Text = profile.URN;

                            }
                            else
                            {
                                keyLabel.Text = item.Key;
                            }
                            profileAddress.Add(searchkey, keyLabel.Text);
                        }
                        else
                        {
                            profileAddress.TryGetValue(searchkey, out string ShortName);
                            keyLabel.Text = ShortName;
                        }

                        keyLabel.Links[0].LinkData = item.Key;
                        keyLabel.AutoSize = true;
                        keyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        keyLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                        keyLabel.LinkColor = System.Drawing.Color.Black;
                        keyLabel.ActiveLinkColor = System.Drawing.Color.Black;
                        keyLabel.VisitedLinkColor = System.Drawing.Color.Black;
                        keyLabel.MouseClick += new MouseEventHandler(LinkClicked);
                        keyLabel.Dock = DockStyle.Left;


                        Label valueLabel = new Label
                        {
                            Text = item.Value.ToString(),
                            AutoSize = true,
                            Dock = DockStyle.Right
                        };


                        rowPanel.Controls.Add(keyLabel, 0, 0);
                        rowPanel.Controls.Add(valueLabel, 1, 0);


                        if (row % 2 == 0)
                        {
                            rowPanel.BackColor = System.Drawing.Color.White;
                        }
                        else
                        {
                            rowPanel.BackColor = System.Drawing.Color.LightGray;
                        }


                        RoyaltiesPanel.Controls.Add(rowPanel);
                        row++;



                    }

                    decimal totalRoytalties = objstate.Royalties.Values.Sum();

                    lblTotalRoyaltiesDetail.Text = "royalties: " + totalRoytalties.ToString();
                }

                ///


                foreach (KeyValuePair<string, DateTime> item in objstate.Creators)
                {

                    if (item.Value.Year > 1)

                    {


                        TableLayoutPanel rowPanel = new TableLayoutPanel
                        {
                            RowCount = 1,
                            ColumnCount = 2,
                            Dock = DockStyle.Top,
                            AutoSize = true,
                            Padding = new System.Windows.Forms.Padding(3)
                        };


                        rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 370));
                        rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 0));



                        LinkLabel keyLabel = new LinkLabel();

                        string searchkey = item.Key;
                        PROState profile = PROState.GetProfileByAddress(searchkey, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                        if (profile.URN != null)
                        {
                            keyLabel.Text = profile.URN;
                        }
                        else
                        {


                            keyLabel.Text = item.Key;
                        }
                        keyLabel.Links[0].LinkData = item.Key;
                        keyLabel.AutoSize = true;
                        keyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        keyLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                        keyLabel.LinkColor = System.Drawing.Color.Black;
                        keyLabel.ActiveLinkColor = System.Drawing.Color.Black;
                        keyLabel.VisitedLinkColor = System.Drawing.Color.Black;
                        keyLabel.MouseClick += new MouseEventHandler(LinkClicked);
                        keyLabel.Dock = DockStyle.Left; // set dock property for key label 
                        rowPanel.Controls.Add(keyLabel, 0, 0);



                        if (row % 2 == 0)
                        {
                            rowPanel.BackColor = System.Drawing.Color.White;
                        }
                        else
                        {
                            rowPanel.BackColor = System.Drawing.Color.LightGray;
                        }


                        CreatorsPanel.Controls.Add(rowPanel);
                        row++;
                    }
                }

            }
            CreatorsPanel.ResumeLayout();
            OwnersPanel.ResumeLayout();
            supPanel.Visible = false;
            btnRefreshSup.BackColor = Color.White;
            btnRefreshSup.ForeColor = Color.Black;
            OwnersPanel.Visible = true;

        }

        private void ShowSupPanel(object sender, EventArgs e)
        {
            if (!isDiscoBallOpen)
            {
                // Set the flag to true since the window is being opened
                isDiscoBallOpen = true;

                // Open the window
                DiscoBall disco = new DiscoBall(_activeprofile, "", _objectaddress, imgPicture.ImageLocation, false, _testnet);
                disco.StartPosition = FormStartPosition.CenterScreen;
                disco.FormClosed += (s, d) => { isDiscoBallOpen = false; }; // Reset the flag when the window is closed
                disco.Show(this);
                disco.Focus();

                // Other actions you want to perform
                supPanel.Visible = true;
                btnRefreshSup.BackColor = Color.Blue;
                btnRefreshSup.ForeColor = Color.Yellow;
                RefreshSupMessages();
            }
        }

        private void RefreshSupMessages()
        {

            // sorry cannot run two searches at a time
            if (btnRefreshSup.Enabled == false) { return; }

            this.Invoke((MethodInvoker)delegate
            {
                btnRefreshSup.Enabled = false;
            });

            supFlow.SuspendLayout();

            // Clear controls if no messages have been displayed yet
            if (numMessagesDisplayed == 0)
            {
                Task.Run(() =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        foreach (var viewer in webviewers)
                        {

                            try { viewer.Dispose(); } catch { }

                        }
                    });

                });


                supFlow.Controls.Clear();

            }

            List<MessageObject> messages = new List<MessageObject>();

            try { messages = OBJState.GetPublicMessagesByAddress(_objectaddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, numMessagesDisplayed, 10); }
            catch (Exception ex) { string error = ex.Message; return; }


            Dictionary<string, string[]> profileAddress = new Dictionary<string, string[]> { };


            foreach (MessageObject messagePacket in messages)
            {
                numMessagesDisplayed++;

                string message = "";
                string tid = messagePacket.TransactionId.ToString();
                try
                {


                    message = messagePacket.Message.ToString();

                    string relativeFolderPath = @"root\" + tid;
                    string folderPath = Path.Combine(Environment.CurrentDirectory, relativeFolderPath);

                    string[] files = Directory.GetFiles(folderPath);

                    foreach (string file in files)
                    {
                        string extension = Path.GetExtension(file);

                        if ((!string.IsNullOrEmpty(extension) && !file.Contains("ROOT.json")) || Path.GetFileName(file) == "INQ")
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


                        DateTime tstamp = messagePacket.BlockDate;
                        System.Drawing.Color bgcolor = System.Drawing.Color.White;


                        this.Invoke((MethodInvoker)delegate
                        {
                            CreateRow(fromImage, fromAddress, messagePacket.FromAddress, tstamp, message, tid, false, supFlow);
                        });


                        bool containsFileWithINQ = files.Any(file =>
                            file.EndsWith("INQ", StringComparison.OrdinalIgnoreCase) &&
                            !file.EndsWith("BLOCK", StringComparison.OrdinalIgnoreCase));

                        if (containsFileWithINQ)
                        {
                            //ADD INQ IF IT EXISTS AND IS NOT BLOCKED
                            this.Invoke((MethodInvoker)delegate
                            {
                                string profileowner = "";

                                if (_activeprofile != null) { profileowner = _activeprofile; }
                                FoundINQControl foundObject = new FoundINQControl(messagePacket.TransactionId, profileowner);

                                foundObject.Margin = new Padding(2, 7, 8, 7);
                                supFlow.Controls.Add(foundObject);
                            });
                        }

                        string pattern = "<<.*?>>";
                        List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4", ".mov", ".avi", ".wav", ".mp3" };

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
                                            this.Invoke((MethodInvoker)delegate
                                            {
                                                try { AddVideo(content); } catch { }
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
                            Margin = new System.Windows.Forms.Padding(0, 10, 0, 10),
                            Padding = new System.Windows.Forms.Padding(0)

                        };

                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 50));
                        this.Invoke((MethodInvoker)delegate
                        {
                            supFlow.Controls.Add(padding);
                        });
                    }
                }
                catch (Exception ex)
                {
                    string help = ex.Message;
                }//deleted file



            }



            this.Invoke((MethodInvoker)delegate
            {
                btnRefreshSup.Enabled = true;
            });

            this.Invoke((MethodInvoker)delegate
            {
                supFlow.ResumeLayout();
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

                msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420));






                if (addtoTop)
                {
                    supFlow.Controls.Add(msg);
                    supFlow.Controls.SetChildIndex(msg, 2);
                }
                else
                {
                    supFlow.Controls.Add(msg);
                }



                PictureBox pictureBox = new PictureBox();

                // Set the PictureBox properties

                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Width = 400;
                pictureBox.Height = 400;
                pictureBox.BackColor = Color.Black;
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
                                        else
                                        {
                                            process2.Kill();

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
                        pictureBox.ImageLocation = imagelocation;
                        pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };

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

        async void AddVideo(string videopath, bool isprivate = false, bool addtoTop = false, bool autoPlay = false)
        {
            string videolocation = "";
            if (videopath != null)
            {
                videolocation = videopath;

                //build web viewer with default loading page
                Microsoft.Web.WebView2.WinForms.WebView2 webviewer = new Microsoft.Web.WebView2.WinForms.WebView2();
                webviewer.AllowExternalDrop = true;
                webviewer.BackColor = System.Drawing.Color.Black;
                webviewer.CreationProperties = null;
                webviewer.DefaultBackgroundColor = System.Drawing.Color.Black;
                webviewer.Name = "webviewer";
                webviewer.Size = new System.Drawing.Size(400, 300);
                webviewer.ZoomFactor = 1D;
                webviewers.Add(webviewer);

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



                if (addtoTop)
                {

                    supFlow.Controls.Add(msg);
                    supFlow.Controls.SetChildIndex(msg, 2);


                }
                else
                {

                    supFlow.Controls.Add(msg);


                }



                msg.Controls.Add(webviewer);
                await webviewer.EnsureCoreWebView2Async();
                // immediately load Progress content into the WebView2 control
                webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\includes\loading.html");


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
                                        else { process2.Kill(); }



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



                                string encodedPath = "file:///" + Uri.EscapeUriString(videolocation.Replace('\\', '/'));
                                string viewerPath = Path.GetDirectoryName(videolocation) + @"\urnviewer.html";
                                string htmlstring = "<html><body><embed src=\"" + encodedPath + "\" width=100% height=100% ></body></html>";
                                System.IO.File.WriteAllText(viewerPath, htmlstring);

                                this.Invoke((Action)(() =>
                                {
                                    webviewer.CoreWebView2.Navigate(viewerPath);


                                    try
                                    {


                                        // If it's a .wav file and autoplay is enabled, trigger the audio playback
                                        if (videolocation.ToLower().EndsWith(".wav") && autoPlay)

                                        {
                                            WaveOut waveOut = new WaveOut();
                                            WaveFileReader reader = new WaveFileReader(videolocation);
                                            waveOut.Init(reader);
                                            waveOut.Play();


                                        }

                                        // If it's a .mp3 file and autoplay is enabled, trigger the audio playback
                                        if (videolocation.ToLower().EndsWith(".mp3") && autoPlay)

                                        {
                                            WaveOut waveOut = new WaveOut();
                                            Mp3FileReader reader = new Mp3FileReader(videolocation);
                                            waveOut.Init(reader);
                                            waveOut.Play();

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        string error = ex.Message;// Handle exceptions here
                                    }
                                }));


                            }
                            else
                            {
                                webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\includes\notfound.html");

                            }



                        });




                    }
                    else
                    {

                        if (videolocation.ToLower().EndsWith(".mov"))
                        {
                            string inputFilePath = videolocation;
                            string outputFilePath = System.IO.Path.ChangeExtension(inputFilePath, ".mp4");
                            videolocation = outputFilePath;
                        }

                        string encodedPath = "file:///" + Uri.EscapeUriString(videolocation.Replace('\\', '/'));
                        string viewerPath = Path.GetDirectoryName(videolocation) + @"\urnviewer.html";
                        string htmlstring = "<html><body><embed src=\"" + encodedPath + "\" width=100% height=100% ></body></html>";
                        System.IO.File.WriteAllText(viewerPath, htmlstring);
                        webviewer.CoreWebView2.Navigate(viewerPath);
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

                    webviewer.CoreWebView2.Navigate(videolocation);
                }



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

        void CreateTransRow(string fromName, string fromId, string toName, string toId, string action, string qty, string amount, DateTime timestamp, string status, System.Drawing.Color bgcolor, FlowLayoutPanel layoutPanel)
        {

            this.Invoke((Action)(() =>
            {

                // Create a table layout panel for each row
                TableLayoutPanel row = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 3,
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    Padding = new System.Windows.Forms.Padding(0),
                    BackColor = bgcolor,
                    Margin = new System.Windows.Forms.Padding(0)
                };
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 107));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 62));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 107));
                layoutPanel.Controls.Add(row);


                LinkLabel fromname = new LinkLabel
                {
                    Text = fromName,
                    AutoSize = true
                };
                fromname.LinkClicked += (sender, e) => { Owner_LinkClicked(fromId); };
                fromname.Dock = DockStyle.Left;
                fromname.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                fromname.Margin = new System.Windows.Forms.Padding(0);
                row.Controls.Add(fromname, 0, 0);

                Label laction = new Label
                {
                    Text = action,
                    AutoSize = true,
                    Dock = DockStyle.Left,
                    TextAlign = System.Drawing.ContentAlignment.TopLeft,
                    Margin = new System.Windows.Forms.Padding(0)
                };
                row.Controls.Add(laction, 1, 0);


                LinkLabel toname = new LinkLabel
                {
                    Text = toName,
                    AutoSize = true
                };
                toname.LinkClicked += (sender, e) => { Owner_LinkClicked(toId); };
                toname.Dock = DockStyle.Right;
                toname.TextAlign = System.Drawing.ContentAlignment.TopLeft;
                toname.Margin = new System.Windows.Forms.Padding(0);
                row.Controls.Add(toname, 2, 0);


                if (qty.Length + amount.Length > 0)
                {


                    TableLayoutPanel stats = new TableLayoutPanel
                    {
                        RowCount = 1,
                        ColumnCount = 2,
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        BackColor = bgcolor,
                        Padding = new System.Windows.Forms.Padding(0),
                        Margin = new System.Windows.Forms.Padding(0)
                    };
                    // Add the width of the first column to fixed value and second to fill remaining space
                    stats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 138));
                    stats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 138));
                    layoutPanel.Controls.Add(stats);


                    Label lqty = new Label
                    {
                        Text = qty,
                        AutoSize = true,
                        TextAlign = System.Drawing.ContentAlignment.TopLeft,
                        Margin = new System.Windows.Forms.Padding(0)
                    };
                    stats.Controls.Add(lqty, 0, 0);


                    Label lamount = new Label
                    {
                        Text = amount,
                        AutoSize = true,
                        TextAlign = System.Drawing.ContentAlignment.TopLeft,
                        Margin = new System.Windows.Forms.Padding(0)
                    };
                    stats.Controls.Add(lamount, 1, 0);
                }


                TableLayoutPanel msg = new TableLayoutPanel
                {
                    RowCount = 2,
                    ColumnCount = 1,
                    Dock = DockStyle.Bottom,
                    AutoSize = true,
                    BackColor = bgcolor,
                    Padding = new System.Windows.Forms.Padding(0),
                    Margin = new System.Windows.Forms.Padding(0)
                };
                // Add the width of the first column to fixed value and second to fill remaining space
                msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 276));


                layoutPanel.Controls.Add(msg);

                Label lstatus = new Label
                {
                    Text = status,
                    AutoSize = true,
                    TextAlign = System.Drawing.ContentAlignment.TopLeft,
                    Margin = new System.Windows.Forms.Padding(0)
                };
                msg.Controls.Add(lstatus, 0, 0);

                // Create a LinkLabel with the owner name
                Label tstamp = new Label
                {
                    AutoSize = true,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                    TextAlign = System.Drawing.ContentAlignment.TopLeft,
                    Margin = new System.Windows.Forms.Padding(0)
                };
                msg.Controls.Add(tstamp, 0, 1);
            }));
        }

        void Owner_LinkClicked(string ownerId)
        {

            new ObjectBrowser(ownerId, false, _testnet).Show();
        }

        private async void MainRefreshClick(object sender, EventArgs e)
        {
            transFlow.Visible = false;
            KeysFlow.Visible = false;
            txtdesc.Visible = true;
            registrationPanel.Visible = true;
            KeysFlow.Controls.Clear();
            string transactionid;
            string ipfsurn = null;
            string ipfsimg = null;
            string ipfsuri = null;
            bool isWWW = false;
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");

            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            if (objstate.Owners != null)
            {

                string urn = "";
                if (objstate.URN != null)
                {
                    urn = objstate.URN;
                    if (objstate.Image == null)
                    {
                        // Check to see if objstate.URN has an image extension
                        string[] validImageExtensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".ico", ".tiff", ".wmf", ".emf" }; // Add more if needed

                        bool hasValidImageExtension = validImageExtensions.Any(ext =>
                            objstate.URN.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

                        if (hasValidImageExtension)
                        {
                            objstate.Image = objstate.URN;
                        }
                    }

                    if (!objstate.URN.ToLower().StartsWith("http"))
                    {
                        urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URN.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                        if (objstate.URN.ToLower().StartsWith("ipfs:")) { urn = urn.Replace(@"\root\", @"\ipfs\"); if (objstate.URN.Length == 51) { urn += @"\artifact"; } }


                    }
                    else
                    {
                        webviewer.Visible = true;
                        await webviewer.EnsureCoreWebView2Async();
                        webviewer.CoreWebView2.Navigate(objstate.URN);
                        lblURNBlockDate.Text = "http get: " + DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                        isWWW = true;
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
                }


                string uriurn = "";
                if (objstate.URI != null)
                {
                    uriurn = objstate.URI;

                    if (!objstate.URI.ToLower().StartsWith("http"))
                    {
                        uriurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URI.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                        if (objstate.URI.ToLower().StartsWith("ipfs:")) { uriurn = uriurn.Replace(@"\root\", @"\ipfs\"); if (objstate.URI.Length == 51) { uriurn += @"\artifact"; } }

                    }
                }


                DateTime urnblockdate = new DateTime();
                DateTime imgblockdate = new DateTime();
                DateTime uriblockdate = new DateTime();
                lblObjectCreatedDate.Text = objstate.CreatedDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                try
                {

                    Match imgurnmatch = regexTransactionId.Match(imgurn);
                    transactionid = imgurnmatch.Value;
                    Root root = new Root();
                    if (imgurn != "")
                    {
                        switch (objstate.Image.ToUpper().Substring(0, 4))
                        {
                            // MZC case
                            case "MZC:":
                                Task verificationTask = Task.Run(() =>
                                {
                                    try
                                    {
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                        if (root.BlockDate.Year > 1975)
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "mazacoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                            }));
                                        }
                                        else
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "mazacoin could not be verified: ";
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblIMGBlockDate.Invoke(new Action(() =>
                                        {
                                            lblIMGBlockDate.Text = "mazacoin could not be verified: ";
                                        }));
                                    }
                                });

                                break;

                            // BTC case
                            case "BTC:":
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                        if (root.BlockDate.Year > 1975)
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                            }));
                                        }
                                        else
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "bitcoin could not be verified: ";
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblIMGBlockDate.Invoke(new Action(() =>
                                        {
                                            lblIMGBlockDate.Text = "bitcoin could not be verified: ";
                                        }));
                                    }
                                });

                                break;

                            // LTC case
                            case "LTC:":
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                        if (root.BlockDate.Year > 1975)
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "litecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                            }));
                                        }
                                        else
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "litecoin could not be verified: ";
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblIMGBlockDate.Invoke(new Action(() =>
                                        {
                                            lblIMGBlockDate.Text = "litecoin could not be verified: ";
                                        }));
                                    }
                                });

                                break;

                            // DOG case
                            case "DOG:":
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                        if (root.BlockDate.Year > 1975)
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "dogecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                            }));
                                        }
                                        else
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "dogecoin could not be verified: ";
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblIMGBlockDate.Invoke(new Action(() =>
                                        {
                                            lblIMGBlockDate.Text = "dogecoin could not be verified: ";
                                        }));
                                    }
                                });

                                break;

                            case "IPFS":
                                Task.Run(() =>
                                {
                                    string transid = "empty";
                                    try { transid = objstate.Image.Substring(5, 46); } catch { }

                                    if (!File.Exists(imgurn) && !File.Exists("ipfs/" + transid + "/artifact") && !File.Exists("ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'))))
                                    {
                                        if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                        {
                                            try { Directory.Delete("ipfs/" + transid, true); } catch { }
                                            try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                            Directory.CreateDirectory("ipfs/" + transid + "-build");
                                            Process process2 = new Process();
                                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                            process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
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

                                                try { System.IO.File.Move("ipfs/" + transid + "_tmp", imgurn); }
                                                catch (System.ArgumentException ex)
                                                {
                                                    System.IO.File.Move("ipfs/" + transid + "_tmp", "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + transid + @"\artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));
                                                }
                                            }

                                            if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                            {
                                                fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);

                                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                try { System.IO.File.Move("ipfs/" + transid + "/" + transid, imgurn); }
                                                catch (System.ArgumentException ex)
                                                {
                                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + transid + @"\artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));
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
                                    }
                                    else
                                    {
                                        if (File.Exists("ipfs/" + transid + "/artifact"))
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "IPFS verified: " + DateTime.Now.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                            }));
                                        }
                                        else
                                        {
                                            if (File.Exists("ipfs/" + objstate.Image.Substring(5, 46) + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'))))
                                            {
                                                lblIMGBlockDate.Invoke(new Action(() =>
                                                {
                                                    lblIMGBlockDate.Text = "IPFS could not be verified: ";
                                                }));
                                            }
                                        }
                                    }
                                });

                                break;

                            default:
                                if (!txtIMG.Text.ToUpper().StartsWith("HTTP") && transactionid != "")
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                            if (root.BlockDate.Year > 1975)
                                            {
                                                lblIMGBlockDate.Invoke(new Action(() =>
                                                {
                                                    if (mainnetVersionByte == "111")
                                                    {
                                                        lblIMGBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                                    }
                                                    else
                                                    {
                                                        lblIMGBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                                    }
                                                }));
                                            }
                                            else
                                            {
                                                lblIMGBlockDate.Invoke(new Action(() =>
                                                {
                                                    lblIMGBlockDate.Text = "could not be verified: ";
                                                }));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            lblIMGBlockDate.Invoke(new Action(() =>
                                            {
                                                lblIMGBlockDate.Text = "could not be verified: ";
                                            }));
                                        }
                                    });
                                }

                                break;

                        }
                    }

                }
                catch { }



                try
                {
                    transactionid = "";
                    Match urnmatch = regexTransactionId.Match(urn);
                    transactionid = urnmatch.Value;
                    Root root = new Root();


                    switch (objstate.URN.Substring(0, 4))
                    {
                        // MZC case
                        case "MZC:":
                            Task.Run(() =>
                            {
                                try
                                {
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                    if (root.BlockDate.Year > 1975)
                                    {
                                        lblURNBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURNBlockDate.Text = "mazacoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                        }));
                                    }
                                    else
                                    {
                                        lblURNBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURNBlockDate.Text = "mazacoin could not be verified: ";
                                        }));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    lblURNBlockDate.Invoke(new Action(() =>
                                    {
                                        lblURNBlockDate.Text = "mazacoin could not be verified: ";
                                    }));
                                }
                            });

                            break;

                        // BTC case
                        case "BTC:":
                            Task.Run(() =>
                            {
                                try
                                {
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                    if (root.BlockDate.Year > 1975)
                                    {
                                        lblURNBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURNBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                        }));
                                    }
                                    else
                                    {
                                        lblURNBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURNBlockDate.Text = "bitcoin could not be verified: ";
                                        }));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    lblURNBlockDate.Invoke(new Action(() =>
                                    {
                                        lblURNBlockDate.Text = "bitcoin could not be verified: ";
                                    }));
                                }
                            });

                            break;

                        // LTC case
                        case "LTC:":
                            Task.Run(() =>
                            {
                                try
                                {
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                    if (root.BlockDate.Year > 1975)
                                    {
                                        lblURNBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURNBlockDate.Text = "litecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                        }));
                                    }
                                    else
                                    {
                                        lblURNBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURNBlockDate.Text = "litecoin could not be verified: ";
                                        }));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    lblURNBlockDate.Invoke(new Action(() =>
                                    {
                                        lblURNBlockDate.Text = "litecoin could not be verified: ";
                                    }));
                                }
                            });

                            break;

                        // DOG case
                        case "DOG:":
                            Task.Run(() =>
                            {
                                try
                                {
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                    if (root.BlockDate.Year > 1975)
                                    {
                                        lblURNBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURNBlockDate.Text = "dogecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                        }));
                                    }
                                    else
                                    {
                                        lblURNBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURNBlockDate.Text = "dogecoin could not be verified: ";
                                        }));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    lblURNBlockDate.Invoke(new Action(() =>
                                    {
                                        lblURNBlockDate.Text = "dogecoin could not be verified: ";
                                    }));
                                }
                            });

                            break;

                        case "IPFS":
                            Task.Run(() =>
                            {
                                string transid = "empty";

                                try
                                {
                                    transid = objstate.URN.Substring(5, 46);
                                }
                                catch { }

                                if (!File.Exists(urn) && !File.Exists("ipfs/" + transid + "/artifact") && !File.Exists("ipfs/" + transid + "/artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.'))))
                                {
                                    if (!System.IO.Directory.Exists(@"ipfs/" + transid + "-build"))
                                    {
                                        try
                                        {
                                            Directory.Delete("ipfs/" + transid, true);
                                        }
                                        catch { }
                                        try
                                        {
                                            Directory.CreateDirectory("ipfs/" + transid);
                                        }
                                        catch { };
                                        Directory.CreateDirectory(@"ipfs/" + transid + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = "cmd.exe";
                                        process2.StartInfo.Arguments = "/c ipfs\\ipfs.exe get " + transid + @" -o ipfs\" + transid;
                                        process2.StartInfo.UseShellExecute = true;
                                        process2.Start();
                                        process2.WaitForExit();

                                        string fileName;

                                        if (System.IO.File.Exists("ipfs/" + transid))
                                        {
                                            System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                            System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                            fileName = objstate.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                            Directory.CreateDirectory("ipfs/" + transid);

                                            try
                                            {
                                                System.IO.File.Move("ipfs/" + transid + "_tmp", urn);
                                            }
                                            catch (System.ArgumentException ex)
                                            {
                                                System.IO.File.Move("ipfs/" + transid + "_tmp", "ipfs/" + transid + "/artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.')));
                                                urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + transid + @"\artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.'));
                                            }
                                        }

                                        if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                        {
                                            fileName = objstate.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                            try
                                            {
                                                System.IO.File.Move("ipfs/" + transid + "/" + transid, urn);
                                            }
                                            catch (System.ArgumentException ex)
                                            {
                                                System.IO.File.Move("ipfs/" + transid + "/" + transid, "ipfs/" + transid + "/artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.')));
                                                urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + transid + @"\artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.'));
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

                                        try
                                        {
                                            Directory.Delete(@"ipfs/" + transid + "-build");
                                        }
                                        catch { }



                                    }

                                    if (File.Exists(urn))
                                    {
                                        if (urn.ToLower().EndsWith(".mov"))
                                        {
                                            string inputFilePath = urn;
                                            string outputFilePath = System.IO.Path.ChangeExtension(inputFilePath, ".mp4");
                                            if (!File.Exists(outputFilePath))
                                            {
                                                try
                                                {
                                                    var ffMpeg = new FFMpegConverter();
                                                    ffMpeg.ConvertMedia(inputFilePath, outputFilePath, Format.mp4);
                                                   
                                                }
                                                catch { }
                                            }
                                           
                                        }

                                        this.Invoke(new Action(() =>
                                        {
                                            btnReloadObject.PerformClick();
                                        }));
                                    }
                                }

                            });

                            break;

                        default:
                            if (!txtURN.Text.ToUpper().StartsWith("HTTP") && transactionid != "")
                            {
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                        if (root.BlockDate.Year > 1975)
                                        {
                                            lblURNBlockDate.Invoke(new Action(() =>
                                            {
                                                if (mainnetVersionByte == "111")
                                                {
                                                    lblURNBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                                }
                                                else
                                                {
                                                    lblURNBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                                }
                                            }));
                                        }
                                        else
                                        {
                                            lblURNBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURNBlockDate.Text = "could not be verified: ";
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblURNBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURNBlockDate.Text = "could not be verified: ";
                                        }));
                                    }
                                });
                            }

                            break;

                    }



                }
                catch { urn = imgurn; }


                try
                {
                    transactionid = "";
                    Root root = new Root();
                    Match urimatch = regexTransactionId.Match(uriurn);
                    transactionid = urimatch.Value;

                    if (uriurn != "")
                    {
                        switch (objstate.URI.Substring(0, 4))
                        {
                            // MZC case
                            case "MZC:":
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                        if (root.BlockDate.Year > 1975)
                                        {
                                            lblURIBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURIBlockDate.Text = "mazacoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                            }));
                                        }
                                        else
                                        {
                                            lblURIBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURIBlockDate.Text = "mazacoin could not be verified: ";
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblURIBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURIBlockDate.Text = "mazacoin could not be verified: ";
                                        }));
                                    }
                                });

                                break;

                            // BTC case
                            case "BTC:":
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                        if (root.BlockDate.Year > 1975)
                                        {
                                            lblURIBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURIBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                            }));
                                        }
                                        else
                                        {
                                            lblURIBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURIBlockDate.Text = "bitcoin could not be verified: ";
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblURIBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURIBlockDate.Text = "bitcoin could not be verified: ";
                                        }));
                                    }
                                });

                                break;

                            // LTC case
                            case "LTC:":
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                        if (root.BlockDate.Year > 1975)
                                        {
                                            lblURIBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURIBlockDate.Text = "litecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                            }));
                                        }
                                        else
                                        {
                                            lblURIBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURIBlockDate.Text = "litecoin could not be verified: ";
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblURIBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURIBlockDate.Text = "litecoin could not be verified: ";
                                        }));
                                    }
                                });

                                break;

                            // DOG case
                            case "DOG:":
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                        if (root.BlockDate.Year > 1975)
                                        {
                                            lblURIBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURIBlockDate.Text = "dogecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                            }));
                                        }
                                        else
                                        {
                                            lblURIBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURIBlockDate.Text = "dogecoin could not be verified: ";
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblURIBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURIBlockDate.Text = "dogecoin could not be verified: ";
                                        }));
                                    }
                                });

                                break;

                            case "IPFS":
                                Task.Run(() =>
                                {
                                    string transid = "empty";

                                    try
                                    {
                                        transid = objstate.URI.Substring(5, 46);
                                    }
                                    catch { }

                                    if (!System.IO.Directory.Exists(@"ipfs/" + transid + "-build") && !System.IO.Directory.Exists(@"ipfs/" + transid))
                                    {
                                        try
                                        {
                                            Directory.Delete("ipfs/" + transid, true);
                                        }
                                        catch { }
                                        try
                                        {
                                            Directory.CreateDirectory("ipfs/" + transid);
                                        }
                                        catch { };
                                        Directory.CreateDirectory(@"ipfs/" + transid + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                        process2.Start();
                                        process2.WaitForExit();

                                        string fileName;
                                        if (System.IO.File.Exists("ipfs/" + transid))
                                        {
                                            System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                            System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                            fileName = objstate.URI.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                            Directory.CreateDirectory("ipfs/" + transid);
                                            System.IO.File.Move("ipfs/" + transid + "_tmp", uriurn);

                                            try
                                            {
                                                System.IO.File.Move("ipfs/" + transid + "_tmp", uriurn);
                                            }
                                            catch (System.ArgumentException ex)
                                            {
                                                System.IO.File.Move("ipfs/" + transid + "_tmp", "ipfs/" + transid + "/artifact" + objstate.URI.Substring(objstate.URI.LastIndexOf('.')));
                                                uriurn = "ipfs/" + transid + "/artifact" + objstate.URI.Substring(objstate.URI.LastIndexOf('.'));
                                            }
                                        }

                                        if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                        {
                                            fileName = objstate.URI.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                            try
                                            {
                                                System.IO.File.Move("ipfs/" + transid + "/" + transid, uriurn);
                                            }
                                            catch (System.ArgumentException ex)
                                            {
                                                System.IO.File.Move("ipfs/" + transid + "/" + transid, "ipfs/" + transid + "/artifact" + objstate.URI.Substring(objstate.URI.LastIndexOf('.')));
                                                uriurn = "ipfs/" + transid + "/artifact" + objstate.URI.Substring(objstate.URI.LastIndexOf('.'));
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

                                        Directory.Delete(@"ipfs/" + transid + "-build");
                                    }
                                    else
                                    {
                                        lblURIBlockDate.Invoke(new Action(() =>
                                        {
                                            lblURIBlockDate.Text = "ipfs verified: " + DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                        }));
                                    }
                                });

                                break;

                            default:
                                if (!txtURI.Text.ToUpper().StartsWith("HTTP") && transactionid != "")
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                            if (root.BlockDate.Year > 1975)
                                            {
                                                lblURIBlockDate.Invoke(new Action(() =>
                                                {
                                                    if (mainnetVersionByte == "111")
                                                    {
                                                        lblURIBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                                    }
                                                    else
                                                    {
                                                        lblURIBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                                    }
                                                }));
                                            }
                                            else
                                            {
                                                lblURIBlockDate.Invoke(new Action(() =>
                                                {
                                                    lblURIBlockDate.Text = "could not be verified: ";
                                                }));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            lblURIBlockDate.Invoke(new Action(() =>
                                            {
                                                lblURIBlockDate.Text = "could not be verified: ";
                                            }));
                                        }
                                    });
                                }
                                break;

                        }
                    }

                }
                catch { }



                // Get the file extension
                string extension = Path.GetExtension(urn).ToLower();
                Match match = regexTransactionId.Match(urn);
                transactionid = match.Value;
                string filePath = urn;
                filePath = filePath.Replace(@"\root\root\", @"\root\");
                lblURNFullPath.Text = filePath;
                txtURN.Text = objstate.URN;
                txtIMG.Text = objstate.Image;
                txtURI.Text = objstate.URI;
                lblLicense.Text = objstate.License;

                List<string> keywords = new List<string>();

                keywords = OBJState.GetKeywordsByAddress(_objectaddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                foreach (string keyword in keywords)
                {


                    LinkLabel keywordLabel = new LinkLabel
                    {
                        Text = keyword,
                        AutoSize = true
                    };

                    keywordLabel.LinkClicked += (Ksender, b) => { Owner_LinkClicked("#" + keyword); };
                    keywordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    keywordLabel.Margin = new System.Windows.Forms.Padding(0);
                    keywordLabel.Dock = DockStyle.Bottom;
                    KeysFlow.Controls.Add(keywordLabel);


                }

                lblProcessHeight.Text = objstate.ProcessHeight.ToString();
                lblLastChangedDate.Text = objstate.ChangeDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                if (urnblockdate.Year > 1)
                {
                    lblURNBlockDate.Text = urnblockdate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                }
                if (imgblockdate.Year > 1)
                {
                    lblIMGBlockDate.Text = imgblockdate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                }
                if (uriblockdate.Year > 1)
                {
                    lblURIBlockDate.Text = uriblockdate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                }

                txtdesc.Text = objstate.Description;
                if (!objstate.URN.ToUpper().StartsWith("HTTP") && !objstate.URN.ToUpper().StartsWith("IPFS"))
                {
                    try
                    {
                        string urnmsgpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URN.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Substring(0, 64) + @"\MSG";

                        // Check if the file exists at urnmsgpath
                        if (File.Exists(urnmsgpath))
                        {
                            // Read the text from the file
                            string fileText = File.ReadAllText(urnmsgpath);

                            // Append the text to foundObject.ObjectDescription.Text
                            if (string.IsNullOrEmpty(txtdesc.Text))
                            {
                                txtdesc.Text = fileText;
                            }
                            else
                            {
                                txtdesc.Text += Environment.NewLine + fileText;
                            }
                        }
                    }
                    catch { }
                }

                txtName.Text = objstate.Name;
                long totalQty = objstate.Owners.Values.Sum(tuple => tuple.Item1);


                if (OwnersPanel.Visible)
                {

                    RefreshOwners();

                }
                else
                {
                    btnRefreshSup.PerformClick();

                }

                if (!isUserControl) { registrationPanel.Visible = true; }


                OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                if (isOfficial.URN != null)
                {
                    if (isOfficial.Creators.First().Key != this._objectaddress)
                    {
                        txtOfficialURN.Text = isOfficial.Creators.First().Key;
                        btnLaunchURN.Visible = false;
                        btnOfficial.Visible = true;
                        lblLaunchURI.Visible = false;
                        lblWarning.Visible = false;

                    }
                    else
                    {

                        lblOfficial.Visible = true;
                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                        myTooltip.SetToolTip(lblOfficial, isOfficial.URN);

                    }
                }


                switch (extension.ToLower())
                {
                    case "":

                        if (File.Exists(urn))
                        {
                            pictureBox1.ImageLocation = urn;
                        }
                        else
                        {
                            lblPleaseStandBy.Text = txtURN.Text;
                            lblPleaseStandBy.Visible = true;
                        }
                        break;
                    case ".exe":
                    case ".dll":
                    case ".bat":
                    case ".cmd":
                    case ".com":
                    case ".msi":
                    case ".scr":
                    case ".vbs":
                    case ".wsf":
                    case ".ps1":
                    case ".psm1":
                    case ".psd1":
                    case ".reg":
                    case ".hta":
                    case ".jar":
                    case ".jse":
                    case ".lnk":
                    case ".mht":
                    case ".mhtml":
                    case ".msc":
                    case ".msp":
                    case ".mst":
                    case ".pif":
                    case ".py":
                    case ".pyc":
                    case ".pyo":
                    case ".pyw":
                    case ".pyz":
                    case ".pyzw":
                    case ".sct":
                    case ".shb":
                    case ".u3p":
                    case ".vb":
                    case ".vbe":
                    case ".vbscript":
                    case ".ws":
                    case ".xla":
                    case ".xlam":
                    case ".xls":
                    case ".xlsb":
                    case ".xlsm":
                    case ".xlsx":
                    case ".xltm":
                    case ".xltx":
                    case ".xml":
                    case ".xsl":
                    case ".xslt":
                        pictureBox1.SuspendLayout();
                        if (File.Exists(imgurn))
                        {

                            pictureBox1.ImageLocation = imgurn;
                        }
                        else
                        {
                            Random rnd = new Random();
                            string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                            if (gifFiles.Length > 0)
                            {
                                int randomIndex = rnd.Next(gifFiles.Length);
                                string randomGifFile = gifFiles[randomIndex];

                                pictureBox1.ImageLocation = randomGifFile;

                            }
                            else
                            {
                                try
                                {
                                    pictureBox1.ImageLocation = @"includes\HugPuddle.jpg";
                                }
                                catch { }
                            }


                        }
                        pictureBox1.ResumeLayout();


                        if (btnOfficial.Visible == false)
                        {
                            btnLaunchURN.Visible = true;
                            lblWarning.Visible = true;
                        }
                        break;

                    case ".glb":
                        //Show image in main box and show open file button
                        pictureBox1.SuspendLayout();
                        if (File.Exists(imgurn))
                        {

                            pictureBox1.ImageLocation = imgurn;
                        }
                        else
                        {
                            Random rnd = new Random();
                            string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                            if (gifFiles.Length > 0)
                            {
                                int randomIndex = rnd.Next(gifFiles.Length);
                                string randomGifFile = gifFiles[randomIndex];

                                pictureBox1.ImageLocation = randomGifFile;

                            }
                            else
                            {
                                try
                                {
                                    pictureBox1.ImageLocation = @"includes\HugPuddle.jpg";
                                }
                                catch { }
                            }


                        }
                        pictureBox1.ResumeLayout();
                        if (btnOfficial.Visible == false) { btnLaunchURN.Visible = true; }
                        break;
                    case ".bmp":
                    case ".gif":
                    case ".ico":
                    case ".jpeg":
                    case ".jpg":
                    case ".png":
                    case ".tif":
                    case ".tiff":
                        // Create a new PictureBox
                        pictureBox1.SuspendLayout();
                        if (File.Exists(urn))
                        {

                            pictureBox1.ImageLocation = urn;
                        }
                        else
                        {
                            Random rnd = new Random();
                            string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                            if (gifFiles.Length > 0)
                            {
                                int randomIndex = rnd.Next(gifFiles.Length);
                                string randomGifFile = gifFiles[randomIndex];

                                pictureBox1.ImageLocation = randomGifFile;

                            }
                            else
                            {
                                try
                                {
                                    pictureBox1.ImageLocation = @"includes\HugPuddle.jpg";
                                }
                                catch { }
                            }


                        }
                        pictureBox1.ResumeLayout();


                        if (btnOfficial.Visible == false) { btnLaunchURN.Visible = true; }
                        break;
                    case ".mp4":
                    case ".mov":
                    case ".avi":
                    case ".mp3":
                    case ".wav":
                    case ".pdf":

                        string outputFilePath = urn;
                        if (extension.ToLower() == ".mov")
                        {
                            string inputFilePath = urn;
                            outputFilePath = System.IO.Path.ChangeExtension(inputFilePath, ".mp4");
                           
                        }

                        flowPanel.Visible = false;
                        string viewerPath = Path.GetDirectoryName(outputFilePath) + @"\urnviewer.html";
                        flowPanel.Controls.Clear();
                        string encodedPath = "file:///" + Uri.EscapeUriString(outputFilePath.Replace('\\', '/'));
                        string htmlstring = "<html><body><embed src=\"" + encodedPath + "\" width=100% height=100%></body></html>";

                        try
                        {
                            System.IO.File.WriteAllText(Path.GetDirectoryName(outputFilePath) + @"\urnviewer.html", htmlstring);
                            if (btnOfficial.Visible == false) { btnLaunchURN.Visible = true; }
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




                        break;
                    case ".htm":
                    case ".html":
                    case ".zip":

                        if (extension.ToLower() == ".htm" || extension.ToLower() == ".html" || Path.GetFileName(urn).ToLower() == "index.zip")
                        {
                            if (isWWW == false)
                            {
                                chkRunTrustedObject.Visible = true;
                                flowPanel.Visible = false;
                                flowPanel.Controls.Clear();

                                string htmlembed = "<html><body><embed src=\"" + urn + "\" width=100% height=100%></body></html>";
                                string potentialyUnsafeHtml = "";

                                try
                                {
                                    potentialyUnsafeHtml = System.IO.File.ReadAllText(urn);

                                }
                                catch { }


                                if (chkRunTrustedObject.Checked)
                                {
                                    lblWarning.Visible = false;
                                    try
                                    {
                                        if (Path.GetFileName(urn).ToLower() != "index.zip")
                                        {
                                            try { System.IO.Directory.Delete(Path.GetDirectoryName(urn), true); } catch { }
                                        }

                                        switch (objstate.URN.Substring(0, 4))
                                        {
                                            case "MZC:":
                                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                                {
                                                    Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                                }
                                                break;
                                            case "BTC:":
                                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                                {
                                                    Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                                }
                                                break;
                                            case "LTC:":
                                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                                {
                                                    Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                                }
                                                break;
                                            case "DOG:":
                                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                                {
                                                    Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                                }
                                                break;
                                            case "IPFS":
                                                ipfsurn = urn;

                                                if (Path.GetFileName(urn).ToLower() != "index.zip")
                                                {
                                                    if (!System.IO.Directory.Exists(@"ipfs/" + objstate.URN.Substring(5, 46) + "-build"))
                                                    {

                                                        Directory.CreateDirectory(@"ipfs/" + objstate.URN.Substring(5, 46) + "-build");
                                                        Process process2 = new Process();
                                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                        process2.StartInfo.Arguments = "get " + objstate.URN.Substring(5, 46) + @" -o ipfs\" + objstate.URN.Substring(5, 46);
                                                        process2.StartInfo.CreateNoWindow = true;
                                                        process2.Start();
                                                        process2.WaitForExit();

                                                        if (System.IO.File.Exists("ipfs/" + objstate.URN.Substring(5, 46)))
                                                        {
                                                            System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46), "ipfs/" + objstate.URN.Substring(5, 46) + "_tmp");

                                                            string fileName = objstate.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                            if (fileName == "")
                                                            {
                                                                fileName = "artifact";
                                                            }
                                                            else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                            try { Directory.CreateDirectory(@"ipfs/" + objstate.URN.Substring(5, 46)); } catch { }
                                                            try { System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46) + "_tmp", ipfsurn); } catch { }
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
                                                                        Arguments = "pin add " + objstate.URN.Substring(5, 46),
                                                                        UseShellExecute = false,
                                                                        CreateNoWindow = true
                                                                    }
                                                                };
                                                                process3.Start();
                                                            }
                                                        }
                                                        catch { }

                                                        Directory.Delete(@"ipfs/" + objstate.URN.Substring(5, 46) + "-build");


                                                    }
                                                }

                                                break;
                                            default:
                                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                                {
                                                    Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                                                }
                                                break;
                                        }

                                        if (Path.GetFileName(urn).ToLower() == "index.zip")
                                        {
                                            string destinationFolder = Path.GetDirectoryName(urn);

                                            // Check if index.html is not present
                                            string indexPath = Path.Combine(destinationFolder, "index.html");
                                            if (!File.Exists(indexPath))
                                            {
                                                // Unzip the contents of index.zip to the same folder using System.IO.Compression.ZipFile
                                                try
                                                {
                                                    ZipFile.ExtractToDirectory(urn, destinationFolder);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show("Error extracting zip file: " + ex.Message);
                                                    return;
                                                }
                                            }

                                            // Update urn to reference index.html
                                            urn = indexPath;
                                        }

                                        try
                                        {
                                            if (Uri.TryCreate(urn, UriKind.Absolute, out Uri uri) && uri.Scheme == Uri.UriSchemeHttp)
                                            {
                                                using (var client = new WebClient())
                                                {
                                                    potentialyUnsafeHtml = client.DownloadString(uri);
                                                    System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                                                    byte[] hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(potentialyUnsafeHtml));
                                                    urn = @"root\" + BitConverter.ToString(hashValue).Replace("-", String.Empty) + @"\index.html";
                                                }

                                            }
                                            else
                                            {
                                                potentialyUnsafeHtml = System.IO.File.ReadAllText(urn);
                                            }
                                        }
                                        catch { }
                                       
                                            var matches = regexTransactionId.Matches(potentialyUnsafeHtml);
                                       
                                            foreach (Match transactionID in matches)
                                            {

                                                switch (objstate.URN.Substring(0, 4))
                                                {
                                                    case "MZC:":
                                                        if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                                        {

                                                            Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                                        }
                                                        break;
                                                    case "BTC:":
                                                        if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                                        {

                                                            Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                                        }
                                                        break;
                                                    case "LTC:":
                                                        if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                                        {

                                                            Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                                        }
                                                        break;
                                                    case "DOG:":
                                                        if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                                        {

                                                            Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                                        }
                                                        break;
                                                    default:
                                                        if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                                        {

                                                            Root.GetRootByTransactionId(transactionID.Value, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                                        }
                                                        break;
                                                }

                                            }
                                        
string _address = _objectaddress;
string _transactionid = objstate.TransactionId;
_genid = objstate.TransactionId;
string _viewer = _activeprofile;
string _viewername = null;

if (!string.IsNullOrEmpty(_activeprofile))
{
    if (objstate.Owners.TryGetValue(_activeprofile, out var tupl))
    { 
        _genid = tupl.Item2; 
    }

    PROState profile = PROState.GetProfileByAddress(
        _activeprofile, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte
    );
    if (profile.URN != null)
    {
        _viewername = HttpUtility.UrlEncode(profile.URN);
    }
}

string _creator = objstate.Creators.Last().Key;
string _owner = objstate.Owners.Last().Key;
string _ownername = null;

if (_owner != "")
{
    PROState profile = PROState.GetProfileByAddress(
        _owner, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte
    );
    if (profile.URN != null)
    {
        _ownername = HttpUtility.UrlEncode(profile.URN);
    }
}

string _urn = HttpUtility.UrlEncode(objstate.URN);
string _uri = HttpUtility.UrlEncode(objstate.URI);
string _img = HttpUtility.UrlEncode(objstate.Image);

// 🔹 NEW: determine paths based on current executing program
string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

// path to "...\<exe-folder>\ipfs"
string ipfsPath = Path.Combine(basePath, "ipfs");

// path to "...\<exe-folder>\root"
string rootPath = Path.Combine(basePath, "root");

// URL-encode for safe querystring usage
string _ipfsPath = HttpUtility.UrlEncode(ipfsPath);
string _rootPath = HttpUtility.UrlEncode(rootPath);

string querystring =
    "?address=" + _address +
    "&viewer=" + _viewer +
    "&viewername=" + _viewername +
    "&creator=" + _creator +
    "&owner=" + _owner +
    "&ownername=" + _ownername +
    "&urn=" + _urn +
    "&uri=" + _uri +
    "&img=" + _img +
    "&transactionid=" + _transactionid +
    "&genid=" + _genid +
    "&ipfs-path=" + _ipfsPath +
    "&root-path=" + _rootPath;

htmlembed =
    "<html><body><embed src=\"" +
    urn + querystring +
    "\" width=100% height=100%></body></html>";



                                    }
                                    catch { }



                                }
                                else
                                {
                                    lblWarning.Visible = true;

                                    if (Path.GetFileName(urn).ToLower() != "index.zip")
                                    {
                                        var sanitizer = new HtmlSanitizer();
                                        var sanitized = sanitizer.Sanitize(potentialyUnsafeHtml);
                                        try { System.IO.File.WriteAllText(urn, sanitized, System.Text.Encoding.UTF8); } catch { }
                                    }
                                }

                                try
                                {
                                    System.IO.File.WriteAllText(Path.GetDirectoryName(urn) + @"\urnviewer.html", htmlembed);
                                    if (btnOfficial.Visible == false) { btnLaunchURN.Visible = true; }

                                    await webviewer.EnsureCoreWebView2Async();
                                    webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(urn) + @"\urnviewer.html");
                                }
                                catch
                                {
                                    Thread.Sleep(500);
                                    try
                                    {
                                        await webviewer.EnsureCoreWebView2Async();
                                        webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(urn) + @"\urnviewer.html");
                                    }
                                    catch { }
                                }

                            }
                        }
                        else
                        {
                            pictureBox1.Invoke(new Action(() => pictureBox1.ImageLocation = imgurn));

                            if (btnOfficial.Visible == false)
                            {
                                btnLaunchURN.Visible = true;
                                lblWarning.Visible = true;
                            }

                        }

                        break;

                    default:

                        pictureBox1.Invoke(new Action(() => pictureBox1.ImageLocation = imgurn));

                        if (btnOfficial.Visible == false)
                        {
                            btnLaunchURN.Visible = true;
                            lblWarning.Visible = true;
                        }

                        break;
                }


                imgPicture.SuspendLayout();
                if (File.Exists(imgurn))
                {

                    imgPicture.ImageLocation = imgurn;
                }
                else
                {
                    if (extension == "")
                    {

                        Match urnmatch = regexTransactionId.Match(urn);
                        transactionid = urnmatch.Value;

                        if (File.Exists(@"root/" + transactionid + @"/MSG"))
                        {
                            string text = File.ReadAllText(@"root/" + transactionid + @"/MSG");
                            GenerateImage(text);
                            string hashedString = "";
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
                            filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\keywords\" + hashedString + ".png";
                            pictureBox1.ImageLocation = filePath;
                            if (txtIMG.Text == "") { imgPicture.ImageLocation = filePath; }

                        }


                    }
                    else
                    {
                        Random rnd = new Random();
                        string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                        if (gifFiles.Length > 0)
                        {
                            int randomIndex = rnd.Next(gifFiles.Length);
                            string randomGifFile = gifFiles[randomIndex];

                            pictureBox1.ImageLocation = randomGifFile;

                        }
                        else
                        {
                            try
                            {
                                pictureBox1.ImageLocation = @"includes\HugPuddle.jpg";
                            }
                            catch { }
                        }
                    }

                }

                imgPicture.ResumeLayout();
                if (lblOfficial.Visible) { lblOfficial.Refresh(); }


            }
        }

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


        private void CopyAddressByCreatedDateClick(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void CopyDescriptionByDescriptionClick(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(txtdesc.Text);
        }

        private void ButtonRefreshTransactionsClick(object sender, EventArgs e)
        {

            this.Invoke((Action)(() =>
            {
                btnRefreshTransactions.Enabled = false;
            }));

            transFlow.SuspendLayout();
            txtdesc.Visible = false;
            registrationPanel.Visible = false;
            // Clear controls if no messages have been displayed yet
            if (numChangesDisplayed == 0)
            {
                transFlow.Controls.Clear();
            }

            int rownum = 1;
            bool isverbose;

            // fetch current JSONOBJ from disk if it exists
            try
            {
                string JSONOBJ = System.IO.File.ReadAllText(@"root\" + _objectaddress + @"\OBJ.json");
                OBJState objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
                if (objectState.Verbose) { isverbose = true; }
                else
                {
                    try { System.IO.Directory.Delete(@"root/" + _objectaddress, true); isVerbose = true; } catch { }
                }
            }
            catch { }

            OBJState OBJEvents = OBJState.GetObjectByAddress(_objectaddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, true);
            OBJEvents.ChangeLog.Reverse();
            foreach (string eventMessage in OBJEvents.ChangeLog.Skip(numChangesDisplayed).Take(10))
            {

                string process = eventMessage;
                List<string> transMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);
                string fromAddress = TruncateAddress(transMessagePacket[0]);
                string toAddress = TruncateAddress(transMessagePacket[1]);
                string action = transMessagePacket[2];
                string qty = transMessagePacket[3];
                string amount = transMessagePacket[4];
                string status = transMessagePacket[5];
                DateTime tstamp = DateTime.Parse(transMessagePacket[6]);

                System.Drawing.Color bgcolor;
                if (rownum % 2 == 0)
                {
                    bgcolor = System.Drawing.Color.White;
                }
                else
                {
                    bgcolor = System.Drawing.Color.LightGray;
                }

                CreateTransRow(fromAddress, transMessagePacket[0], toAddress, transMessagePacket[1], action, qty, amount, tstamp, status, bgcolor, transFlow);


                rownum++;
                numChangesDisplayed++;
            }
            this.Invoke((Action)(() =>
            {
            transFlow.ResumeLayout();
            transFlow.Visible = true;
            KeysFlow.Visible = true;

          
                btnRefreshTransactions.Enabled = true;
            }));

        }

        void Attachment_Clicked(string path)
        {
            if (path.ToUpper().StartsWith("IPFS:") || path.ToUpper().StartsWith("BTC:") || path.ToUpper().StartsWith("MZC:") || path.ToUpper().StartsWith("LTC:") || path.ToUpper().StartsWith("DOG:"))
            {
                new ObjectBrowser(path, false, _testnet).Show();
            }
            else
            {
                try
                { System.Diagnostics.Process.Start(path); }
                catch { System.Media.SystemSounds.Exclamation.Play(); }
            }
        }

        private void btnOfficial_Click(object sender, EventArgs e)
        {
            new ObjectDetails(txtOfficialURN.Text, _activeprofile, _testnet).Show();
        }

        private void imgPicture_Validated(object sender, EventArgs e)
        {
            lblOfficial.Refresh();
        }

        private void imgPicture_Click(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void txtName_Click(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void lblLaunchURI_Click(object sender, EventArgs e)
        {
            string src = txtURI.Text;

            // Show a confirmation dialog
            DialogResult result = MessageBox.Show("Are you sure!? Launching files executes locally!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (!string.IsNullOrEmpty(src))
                    {
                        if (!src.StartsWith("http://") && !src.StartsWith("https://"))
                        {
                            // Check if it starts with IPFS
                            if (src.StartsWith("IPFS:"))
                            {
                                // Extract the path after "IPFS:"
                                string ipfsPath = src.Substring(5).Trim();

                                // Assuming the application folder contains the IPFS folder
                                string appFolder = Path.GetDirectoryName(Application.ExecutablePath);
                                string ipfsFolderPath = Path.Combine(appFolder, "ipfs", ipfsPath);

                                // Launch the process
                                System.Diagnostics.Process.Start(ipfsFolderPath);
                            }
                            else
                            {
                                // Assuming the application folder contains the root folder
                                string appFolder = Path.GetDirectoryName(Application.ExecutablePath);

                                // Combine the application folder and the provided relative path
                                string fullPath = Path.Combine(appFolder, "root", src);

                                // Launch the process
                                System.Diagnostics.Process.Start(fullPath);
                            }
                        }
                        else
                        {
                            // If it's a web URL, launch it directly
                            System.Diagnostics.Process.Start(src);
                        }
                    }
                }
                catch
                {
                    System.Media.SystemSounds.Exclamation.Play();
                }
            }

        }

        private void btnBurn_Click(object sender, EventArgs e)
        {
            new ObjectBurn(_objectaddress, _activeprofile, _testnet).Show();
        }

        private void btnGive_Click(object sender, EventArgs e)
        {
            new ObjectGive(_objectaddress, _activeprofile, _testnet).Show();
        }


        private void btnBuy_Click(object sender, EventArgs e)
        {
            new ObjectBuy(_objectaddress, _activeprofile, _testnet).Show();
        }

        private void btnJukeBox_Click(object sender, EventArgs e)
        {

            JukeBox jukeBoxForm = new JukeBox(_objectaddress, _testnet);
            jukeBoxForm.Show();// Pass the reference to the current form as the parent form

        }

        private void btnSupFlix_Click(object sender, EventArgs e)
        {
            SupFlix supflixForm = new SupFlix(_objectaddress, _testnet);
            supflixForm.Show();
        }

        private void btnInquiry_Click(object sender, EventArgs e)
        {
            btnInquiry.Enabled = false;


            INQSearch INQSearchForm = new INQSearch(_objectaddress, _testnet);
            INQSearchForm.Show();

            btnInquiry.Enabled = true;
        }

    }
}

