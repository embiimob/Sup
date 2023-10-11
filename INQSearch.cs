using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUP
{
    public partial class INQSearch : Form
    {
        private Timer delayTimer;
        private List<string> soundFiles;
        private int numPictureBoxesToAdd = 10;
        private int currentTrackIndex = 0;

        private bool playOldestFirst = false;
        List<Microsoft.Web.WebView2.WinForms.WebView2> webviewers = new List<Microsoft.Web.WebView2.WinForms.WebView2>();

        public INQSearch(string searchaddress = "inq")
        {
            InitializeComponent();
            InitializeDelayTimer();
            soundFiles = new List<string>();
            txtSearch.Text = searchaddress;


        }

        private void INQSearch_Load(object sender, EventArgs e)
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
                string searchAddress = Root.GetPublicAddressByKeyword(searchstring.Replace("#", ""));

                if (searchstring.Length > 20) { searchAddress = searchstring; }
                else
                {
                    if (!searchstring.StartsWith("#"))
                    {
                        PROState searchprofile = PROState.GetProfileByURN(searchstring, "good-user", "better-password", @"http://127.0.0.1:18332");
                        if (searchprofile.Creators != null)
                        {
                            searchAddress = searchprofile.Creators[0];
                        }
                    }
                }



                Root[] INQUIRYS = Root.GetRootsByAddress(searchAddress, "good-user", "better-password", @"http://127.0.0.1:18332");

                if (INQUIRYS.Count() > 0)
                {

                    foreach (Root INQUIRY in INQUIRYS)
                    {
                      
                                               

                        if (INQUIRY.File.Keys.Count() > 0)
                        {
                            foreach (string attachment in INQUIRY.File.Keys)
                            {
                                // Check if the attachment ends in .GIF (case-insensitive)
                                if (attachment.EndsWith("INQ", StringComparison.OrdinalIgnoreCase))
                                {
                                    string audioFrom = INQUIRY.SignedBy;
                                    string audioPacket = getLocalPath(INQUIRY.TransactionId + @"\" + attachment) + "," + INQUIRY.BlockDate + "," + audioFrom + "," + INQUIRY.TransactionId + @"\" + attachment;

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


                objects = OBJState.GetObjectsByKeyword(new List<string> { searchstring.Replace("#", "") }, "good-user", "better-password", @"http://127.0.0.1:18332");

                if (objects.Count > 0)
                {
                    foreach (OBJState obj in objects)
                    {

                        if (obj.URN.ToLower().EndsWith("inq"))
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

                objects = OBJState.GetObjectsCreatedByAddress(searchAddress, "good-user", "better-password", @"http://127.0.0.1:18332");

                if (objects.Count() > 0)
                {
                    foreach (OBJState obj in objects)
                    {

                        if (obj.URN.ToLower().EndsWith("inq"))
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


            if (!filepath.ToLower().StartsWith("http") || !filepath.ToLower().StartsWith("ipfs"))
            {
                filelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + filepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace(@"/", @"\");
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
                PROState searchprofile = PROState.GetProfileByAddress(soundFrom, "good-user", "better-password", @"http://127.0.0.1:18332");
                if (searchprofile.URN != null)
                {
                    soundFrom = searchprofile.URN;
                }

                string soundFile = Path.GetFileName(soundInfo[0]);
                string soundDate = soundInfo[1];
                string soundURN = soundInfo[3];

                // should prevent all files from trying to build at once will be limited to 30 at a time.

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


              if (!videopath.ToLower().StartsWith("http") && !videopath.ToLower().StartsWith("ipfs"))
                {
                    videolocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + videopath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace(@"/", @"\");

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
                                    Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50", null, null, true);

                                    break;
                                case "BTC:":

                                    Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0", null, null, true);

                                    break;
                                case "LTC:":

                                    Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48", null, null, true);


                                    break;
                                case "DOG:":
                                    Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30", null, null, true);

                                    break;
                                                                   

                                default:
                                    if (!videopath.ToUpper().StartsWith("HTTP") && transactionid != "")
                                    {
                                        Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332","111", null, null, true);

                                    }
                                    break;
                            }

                            if (File.Exists(videolocation))
                            {

                                string pattern = @"[0-9a-fA-F]{64}";

                       
                                Match match = Regex.Match(videolocation, pattern);

                              
                                if (match.Success)
                                {
                                   
                                    string transactionId = match.Value;

                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        FoundINQControl foundObject = new FoundINQControl(match.Value);
                                        msg.Controls.Add(foundObject);
                                    });

                                }

                              


                            }
                          

                        });




                    }
                    else
                    {
                        string pattern = @"[0-9a-fA-F]{64}";


                        Match match = Regex.Match(videolocation, pattern);


                        if (match.Success)
                        {

                            string transactionId = match.Value;

                            this.Invoke((MethodInvoker)delegate
                            {
                                FoundINQControl foundObject = new FoundINQControl(match.Value);
                                msg.Controls.Add(foundObject);
                            });

                        }

                    }


                }
              


            }


        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (playOldestFirst) { currentTrackIndex = 0; playOldestFirst = false; FindVideos(txtSearch.Text); } else { currentTrackIndex = 0; playOldestFirst = true; FindVideos(txtSearch.Text); }
        }

    }
}
