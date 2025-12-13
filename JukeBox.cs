using NAudio.Wave;
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
    public partial class JukeBox : Form
    {
        private Timer delayTimer;
        private List<string> soundFiles;
        private int numPictureBoxesToAdd = 30;
        private int currentTrackIndex = 0;
        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFileReader;
        private int currentSoundIndex = -1; // Initialize with -1 to indicate no current playing sound
        private bool playNext = true;
        private bool playOldestFirst = false;
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";


        public JukeBox(string searchaddress = "mp3", bool testnet = true)
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

        private void JukeBox_Load(object sender, EventArgs e)
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
            FindSounds(txtSearch.Text);



        }

        private void FindSounds(string searchstring)
        {
            playNext = false;
            StopPlayback();
            currentSoundIndex = -1;

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
                    flowLayoutPanel1.Controls.Clear();
                });

                soundFiles = new List<string>();
                HashSet<string> seenObjectAddresses = new HashSet<string>();
                string searchAddress = Root.GetPublicAddressByKeyword(searchstring.Replace("#", ""), mainnetVersionByte);

                if (searchstring.Length > 20) { searchAddress = searchstring; }
                else
                {
                    if (!searchstring.StartsWith("#"))
                    {
                        PROState searchprofile = PROState.GetProfileByURN(searchstring, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                        if (searchprofile.Creators != null)
                        {
                            searchAddress = searchprofile.Creators[0];
                        }
                    }
                }

                List<OBJState> objects = new List<OBJState>();

                if (btnOwned.BackColor == Color.White)
                {

                    Root[] TRACKS = Root.GetRootsByAddress(searchAddress, mainnetLogin, mainnetPassword, mainnetURL, 0, -1, mainnetVersionByte);


                    foreach (Root TRACK in TRACKS)
                    {
                        foreach (string message in TRACK.Message)
                        {
                            // Find all occurrences of strings surrounded by << >> that end in .mp3 or .wav
                            foreach (Match match in Regex.Matches(message, @"<<([^>]*?(\s*\.mp3\s*(?=>>|$)|\.mp3\s*>>|\s*\.wav\s*(?=>>|$)|\.wav\s*>>))"))
                            {
                                string audioFrom = TRACK.SignedBy;
                                string audioPacket = getLocalPath(match.Groups[1].Value) + "," + TRACK.BlockDate + "," + audioFrom + "," + match.Groups[1].Value;

                                if (!soundFiles.Contains(audioPacket))
                                {
                                    soundFiles.Add(audioPacket);
                                }

                            }
                        }

                        foreach (string attachment in TRACK.File.Keys)
                        {
                            // Check if the attachment ends in .GIF (case-insensitive)
                            if (attachment.EndsWith(".MP3", StringComparison.OrdinalIgnoreCase) || attachment.EndsWith(".WAV", StringComparison.OrdinalIgnoreCase))
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

                    objects = OBJState.GetObjectsByKeyword(new List<string> { searchstring.Replace("#", "") }, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                    foreach (OBJState obj in objects)
                    {

                        if (obj.URN.ToLower().EndsWith(".mp3") || obj.URN.ToLower().EndsWith(".wav"))
                        {
                            // Use object creator address as unique identifier to prevent duplicates
                            if (obj.Creators != null && obj.Creators.Count > 0)
                            {
                                string objectAddress = obj.Creators.First().Key;
                                if (seenObjectAddresses.Add(objectAddress))
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
                                    string audioPacket = getLocalPath(obj.URN) + "," + obj.CreatedDate + "," + creatorKey + "," + obj.URN + "," + obj.Creators.First().Key;
                                    soundFiles.Add(audioPacket);
                                }
                            }
                        }
                    }


                    objects = OBJState.GetObjectsCreatedByAddress(searchAddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                    foreach (OBJState obj in objects)
                    {

                        if (obj.URN.ToLower().EndsWith(".mp3") || obj.URN.ToLower().EndsWith(".wav"))
                        {
                            // Use object creator address as unique identifier to prevent duplicates
                            if (obj.Creators != null && obj.Creators.Count > 0)
                            {
                                string objectAddress = obj.Creators.First().Key;
                                if (seenObjectAddresses.Add(objectAddress))
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

                                    string audioPacket = getLocalPath(obj.URN) + "," + obj.CreatedDate + "," + creatorKey + "," + obj.URN + "," + obj.Creators.First().Key;
                                    soundFiles.Add(audioPacket);
                                }
                            }
                        }
                    }
                }else
                {
                    objects = OBJState.GetObjectsOwnedByAddress(searchAddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                    foreach (OBJState obj in objects)
                    {

                        if (obj.URN.ToLower().EndsWith(".mp3") || obj.URN.ToLower().EndsWith(".wav"))
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

                            string audioPacket = getLocalPath(obj.URN) + "," + obj.CreatedDate + "," + creatorKey + "," + obj.URN + "," + obj.Creators.First().Key;
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
                    playNext = true;
                    InitializeAudioPlayer();

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
                string description = "";

                // should prevent all files from trying to build at once will be limited to 30 at a time.
                buildSoundFiles(soundInfo[0], soundURN);
                if (soundInfo.Length == 5)
                {

                    OBJState getObject = OBJState.GetObjectByAddress(soundInfo[4], mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                    if (getObject != null) { description = getObject.Description; }

                }

                LinkLabel linkLabel = new LinkLabel
                {

                    Text = currentTrackIndex + ": " + soundDate + " from: " + soundFrom + " - " + soundFile,
                    Tag = currentTrackIndex, // Store the index in the Tag property for reference
                    AutoSize = true,
                    Font = new Font("Arial", 12, FontStyle.Regular)
                };
                linkLabel.Click += LinkLabel_Click;
                toolTip1.SetToolTip(linkLabel, soundURN + "\n" + description);
                flowLayoutPanel1.Controls.Add(linkLabel);
                currentTrackIndex++;
            }


        }

        private void buildSoundFiles(string filelocation, string filepath)
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

        private void InitializeAudioPlayer()
        {
            waveOutDevice = new WaveOutEvent();
            waveOutDevice.PlaybackStopped += WaveOutDevice_PlaybackStopped;

            PlayNextSound();
        }

        private async void PlayNextSound()
        {
            if (currentSoundIndex >= 0)
            {
                if (waveOutDevice != null)
                {
                    waveOutDevice.Stop();
                }

                if (audioFileReader != null)
                {
                    audioFileReader.Dispose();
                }
            }


            int nextIndex = currentSoundIndex + 1;
            if (nextIndex < soundFiles.Count)
            {
                currentSoundIndex = nextIndex;

                this.Invoke((MethodInvoker)delegate
                {
                    HighlightLinkLabel(currentSoundIndex);
                });

                try
                {
                    audioFileReader = new AudioFileReader(soundFiles[nextIndex].Split(',')[0]);
                    waveOutDevice.Init(audioFileReader);
                    waveOutDevice.Play();

                }
                catch
                {
                    await Task.Delay(1000); // Pause for 1 second (adjust as needed)

                    object sender = new object(); // You can replace this with the actual sender object
                    StoppedEventArgs e = new StoppedEventArgs(); // Create a new StoppedEventArgs instance without an exception

                    // Simulate raising the event
                    WaveOutDevice_PlaybackStopped(sender, e);


                }

;

            }
            else
            {
                currentSoundIndex = -1;
                HighlightLinkLabel(currentSoundIndex);
            }
        }

        private void WaveOutDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {

            if (playNext)
            {
                PlayNextSound();
            }
        }

        private void HighlightLinkLabel(int index)
        {
            // Reset highlighting for all LinkLabels
            foreach (LinkLabel label in flowLayoutPanel1.Controls)
            {
                label.Font = new Font(label.Font, FontStyle.Regular);
            }

            // Highlight the LinkLabel at the given index
            if (index >= 0 && index < flowLayoutPanel1.Controls.Count)
            {
                LinkLabel highlightedLabel = (LinkLabel)flowLayoutPanel1.Controls[index];
                highlightedLabel.Font = new Font(highlightedLabel.Font, FontStyle.Bold);
            }
        }

        private void LinkLabel_Click(object sender, EventArgs e)
        {
            // playNext = false;

            LinkLabel clickedLabel = (LinkLabel)sender;
            int clickedIndex = (int)clickedLabel.Tag;
            currentSoundIndex = clickedIndex - 1; // Adjust index to play from the clicked position
            StopPlayback();


        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            playNext = false;

            StopPlayback();

        }

        private void StopPlayback()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();

            }

            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            StopPlayback();

            playNext = true;
            currentSoundIndex = currentSoundIndex - 1;

            object sender2 = new object(); // You can replace this with the actual sender object
            StoppedEventArgs e2 = new StoppedEventArgs(); // Create a new StoppedEventArgs instance without an exception

            // Simulate raising the event
            WaveOutDevice_PlaybackStopped(sender2, e2);

        }

        private void JukeBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            playNext = false;

            StopPlayback();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (playOldestFirst) { currentTrackIndex = 0; playOldestFirst = false; FindSounds(txtSearch.Text); } else { currentTrackIndex = 0; playOldestFirst = true; FindSounds(txtSearch.Text); }
        }

        private void btnOwned_Click(object sender, EventArgs e)
        {
            if (btnOwned.BackColor == Color.White)
            {
                btnOwned.BackColor = Color.Yellow;
            }
            else
            { btnOwned.BackColor = Color.White; }

            currentTrackIndex = 0;
            FindSounds(txtSearch.Text);
        }
    }
}
