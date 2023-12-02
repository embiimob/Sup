using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUP
{
    public partial class SupFlix : Form
    {
        private Timer delayTimer;
        private List<string> soundFiles;
        private int numPictureBoxesToAdd = 10;
        private int currentTrackIndex = 0;
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";

        private bool playOldestFirst = false;
        List<Microsoft.Web.WebView2.WinForms.WebView2> webviewers = new List<Microsoft.Web.WebView2.WinForms.WebView2>();

        public SupFlix(string searchaddress = "mp4", bool testnet = true)
        {
            InitializeComponent();
            InitializeDelayTimer();
            soundFiles = new List<string>();
            txtSearch.Text = searchaddress;
            if (!testnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";

            }


        }

        private void SupFlix_Load(object sender, EventArgs e)
        {

            flowLayoutPanel1.MouseWheel += new MouseEventHandler(flowLayoutPanel1_Scroll);
        }

        private void InitializeDelayTimer()
        {
            delayTimer = new Timer();
            delayTimer.Interval = 2000; // 2 seconds delay
            delayTimer.Tick += DelayTimer_Tick;
            delayTimer.Stop();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            // Stop the existing timer if running
            delayTimer.Stop();

            // Restart the timer
            delayTimer.Start();
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            delayTimer.Stop();
            string typedText = txtSearch.Text;
            // Reset the currentPictureBoxIndex and add the first batch of PictureBoxes
            currentTrackIndex = 0;
            FindVideos(txtSearch.Text);



        }

        private void FindVideos(string searchstring)
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

            flowLayoutPanel1.Controls.Clear();


            Task BuildMessage = Task.Run(() =>
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

                this.Invoke((MethodInvoker)delegate
                {
                    pictureBox1.ImageLocation = randomGifFile;
                    pictureBox1.Visible = true;
                });

                soundFiles = new List<string>();
                string searchAddress = Root.GetPublicAddressByKeyword(searchstring.Replace("#", ""), mainnetVersionByte);

                if (searchstring.Length > 20) { searchAddress = searchstring; }
                else
                {
                    if (!searchstring.StartsWith("#"))
                    {
                        PROState searchprofile = PROState.GetProfileByURN(searchstring, mainnetLogin, mainnetPassword, mainnetURL,mainnetVersionByte);
                        if (searchprofile.Creators != null)
                        {
                            searchAddress = searchprofile.Creators[0];
                        }
                    }
                }



                Root[] TRACKS = Root.GetRootsByAddress(searchAddress, mainnetLogin, mainnetPassword, mainnetURL,0,-1,mainnetVersionByte);

                if (TRACKS.Count() > 0)
                {

                    foreach (Root TRACK in TRACKS)
                    {

                        if (TRACK.Message.Count() > 0)
                        {

                            foreach (string message in TRACK.Message)
                            {
                                // Find all occurrences of strings surrounded by << >> that end in .mp3 or .wav
                                foreach (Match match in Regex.Matches(message, @"<<([^>]*?(?i:\.mp4\s*(?=>>|$)|\.avi\s*(?=>>|$)|youtube\.com[^\s>]*|youtu\.be[^\s>]*))>>"))
                                {
                                    string audioFrom = TRACK.SignedBy;
                                    string audioPacket = getLocalPath(match.Groups[1].Value) + "," + TRACK.BlockDate + "," + audioFrom + "," + match.Groups[1].Value;

                                    if (!soundFiles.Contains(audioPacket))
                                    {
                                        soundFiles.Add(audioPacket);
                                    }

                                }
                            }
                        }

                        if (TRACK.File.Keys.Count() > 0)
                        {
                            foreach (string attachment in TRACK.File.Keys)
                            {
                                // Check if the attachment ends in .GIF (case-insensitive)
                                if (attachment.EndsWith(".MP4", StringComparison.OrdinalIgnoreCase) || attachment.EndsWith(".AVI", StringComparison.OrdinalIgnoreCase))
                                {
                                    string audioFrom = TRACK.SignedBy;
                                    string audioPacket = getLocalPath(TRACK.TransactionId + @"\" + attachment) + "," + TRACK.BlockDate + "," + audioFrom + "," + TRACK.TransactionId + @"\" + attachment;

                                    if (!soundFiles.Contains(audioPacket))
                                    {
                                        soundFiles.Add(audioPacket);
                                    }
                                }
                            }
                        }
                    }
                }
                List<OBJState> objects = new List<OBJState>();


                objects = OBJState.GetObjectsByKeyword(new List<string> { searchstring.Replace("#", "") }, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                if (objects.Count > 0)
                {
                    foreach (OBJState obj in objects)
                    {

                        if (obj.URN.ToLower().EndsWith(".mp4") || obj.URN.ToLower().EndsWith(".avi"))
                        {

                            string creatorKey = null;

                            if (obj.Creators.Keys.Count > 1)
                            {
                                creatorKey = obj.Creators.Keys.ElementAt(1);
                            }
                            else if (obj.Creators.Keys.Count == 1)
                            {
                                creatorKey = obj.Creators.Keys.First();
                            }
                            string audioPacket = getLocalPath(obj.URN) + "," + obj.CreatedDate + "," + creatorKey + "," + obj.URN;
                            if (!soundFiles.Contains(audioPacket))
                            {
                                soundFiles.Add(audioPacket);
                            }
                        }
                    }
                }

                objects = OBJState.GetObjectsCreatedByAddress(searchAddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                if (objects.Count() > 0)
                {
                    foreach (OBJState obj in objects)
                    {

                        if (obj.URN.ToLower().EndsWith(".mp4") || obj.URN.ToLower().EndsWith(".avi"))
                        {

                            string creatorKey = null;

                            if (obj.Creators.Keys.Count > 1)
                            {
                                creatorKey = obj.Creators.Keys.ElementAt(1);
                            }
                            else if (obj.Creators.Keys.Count == 1)
                            {
                                creatorKey = obj.Creators.Keys.First();
                            }

                            string audioPacket = getLocalPath(obj.URN) + "," + obj.CreatedDate + "," + creatorKey + "," + obj.URN;
                            if (!soundFiles.Contains(audioPacket))
                            {
                                soundFiles.Add(audioPacket);

                            }
                        }
                    }
                }
                this.Invoke((MethodInvoker)delegate
                {
                    pictureBox1.Visible = false;
                    if (!playOldestFirst) { soundFiles.Reverse(); }
                    AddTracksToFlowLayout();
                });
            });
        }

        private string getLocalPath(string filepath)
        {
            string filelocation = "";

            filelocation = filepath;


            if (!filepath.ToLower().StartsWith("http"))
            {
                filelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + filepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                if (filepath.ToLower().StartsWith("ipfs:")) { filelocation = filelocation.Replace(@"\root\", @"\ipfs\"); if (filepath.Length == 51) { filelocation += @"\artifact"; } }
            }

            return filelocation;
        }

        private void flowLayoutPanel1_Scroll(object sender, MouseEventArgs e)
        {
            // Check if the user has scrolled to the bottom
            if (flowLayoutPanel1.VerticalScroll.Value + flowLayoutPanel1.ClientSize.Height >= flowLayoutPanel1.VerticalScroll.Maximum)
            {
                // Add more PictureBoxes if available
                AddTracksToFlowLayout();
            }
        }

        private void AddTracksToFlowLayout()
        {
            // Determine the number of PictureBoxes to add in this batch
            int countToAdd = Math.Min(numPictureBoxesToAdd, soundFiles.Count - currentTrackIndex);

            // Add PictureBoxes to the FlowLayoutPanel
            for (int i = 0; i < countToAdd; i++)
            {
                string[] soundInfo = soundFiles[currentTrackIndex].Split(',');
                string soundFrom = soundInfo[2];
                PROState searchprofile = PROState.GetProfileByAddress(soundFrom, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                if (searchprofile.URN != null)
                {
                    soundFrom = searchprofile.URN;
                }

                string soundFile = Path.GetFileName(soundInfo[0]);
                string soundDate = soundInfo[1];
                string soundURN = soundInfo[3];

                // should prevent all files from trying to build at once will be limited to 30 at a time.
                buildVideoFiles(soundInfo[0], soundURN);
                AddVideo(soundURN);

                LinkLabel linkLabel = new LinkLabel
                {

                    Text = currentTrackIndex + ": " + soundDate + " from: " + soundFrom + " - " + soundFile,
                    Tag = soundInfo[0], // Store the index in the Tag property for reference
                    AutoSize = true,
                    Font = new Font("Arial", 12, FontStyle.Regular)
                };
                linkLabel.Click += LinkLabel_Click;
                toolTip1.SetToolTip(linkLabel, soundURN);

                flowLayoutPanel1.Controls.Add(linkLabel);

                TableLayoutPanel padding = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 1,
                    Dock = DockStyle.Top,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true,
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                    Margin = new System.Windows.Forms.Padding(0, 10, 0, 30),
                    Padding = new System.Windows.Forms.Padding(0)

                };

                padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, flowLayoutPanel1.Width - 20));
                this.Invoke((MethodInvoker)delegate
                {
                    flowLayoutPanel1.Controls.Add(padding);
                });

                currentTrackIndex++;
            }


        }

        private void buildVideoFiles(string filelocation, string filepath)
        {

            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
            Match imgurnmatch = regexTransactionId.Match(filelocation);
            string transactionid = imgurnmatch.Value;
            Root root = new Root();
            if (!File.Exists(filelocation))
            {
                Task BuildMessage = Task.Run(() =>
                {


                    switch (filepath.ToUpper().Substring(0, 4))
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
                            try { transid = filepath.Substring(5, 46); } catch { }

                            if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                            {
                                try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                Directory.CreateDirectory("ipfs/" + transid + "-build");
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + filepath.Substring(5, 46) + @" -o ipfs\" + transid;
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
                                        fileName = filepath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                        Directory.CreateDirectory("ipfs/" + transid);
                                        System.IO.File.Move("ipfs/" + transid + "_tmp", filelocation);
                                    }

                                    if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                    {
                                        fileName = filepath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                        if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                        System.IO.File.Move("ipfs/" + transid + "/" + transid, filelocation);
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
                            if (!filepath.ToUpper().StartsWith("HTTP") && transactionid != "")
                            {
                                Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                            }
                            break;

                    }
                });

            }


        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {
            // Check if the user has scrolled to the bottom
            if (flowLayoutPanel1.VerticalScroll.Value + flowLayoutPanel1.ClientSize.Height >= flowLayoutPanel1.VerticalScroll.Maximum)
            {
                // Add more PictureBoxes if available
                AddTracksToFlowLayout();
            }
        }

        private void LinkLabel_Click(object sender, EventArgs e)
        {
            // playNext = false;

            LinkLabel clickedLabel = (LinkLabel)sender;
            string videoPath = (string)clickedLabel.Tag;

            try
            { System.Diagnostics.Process.Start(videoPath); }
            catch { System.Media.SystemSounds.Exclamation.Play(); }

        }

        private void SupFlix_Closing(object sender, FormClosingEventArgs e)
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
                webviewers.Clear();

            });
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

                webviewer.Size = new System.Drawing.Size(600, 500);


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

                msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 620));



                if (addtoTop)
                {

                    flowLayoutPanel1.Controls.Add(msg);
                    flowLayoutPanel1.Controls.SetChildIndex(msg, 2);


                }
                else
                {

                    flowLayoutPanel1.Controls.Add(msg);


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

                                this.Invoke((Action)(() =>
                                {


                                    string viewerPath = Path.GetDirectoryName(videolocation) + @"\urnviewer.html";
                                    string htmlstring = "<html><body><embed src=\"" + videolocation + "\" width=100% height=100% ></body></html>";
                                    System.IO.File.WriteAllText(viewerPath, htmlstring);

                                    try { webviewer.CoreWebView2.Navigate(viewerPath); } catch { }


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


                        string viewerPath = Path.GetDirectoryName(videolocation) + @"\urnviewer.html";
                        string htmlstring = "<html><body><embed src=\"" + videolocation + "\" width=100% height=100% ></body></html>";
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

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (playOldestFirst) { currentTrackIndex = 0; playOldestFirst = false; FindVideos(txtSearch.Text); } else { currentTrackIndex = 0; playOldestFirst = true; FindVideos(txtSearch.Text); }
        }

    }
}
