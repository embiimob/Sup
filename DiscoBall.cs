using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using LevelDB;
using BitcoinNET.RPCClient;
using NBitcoin.RPC;
using NBitcoin;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Timers;

using NAudio.Wave;
using AngleSharp.Html.Dom;
using Microsoft.Win32;

namespace SUP
{
    public partial class DiscoBall : Form
    {
        private string _fromaddress;
        private string _toaddress;
        private string _fromimageurl;
        private string _toimageurl;

        // GPT3 AUDIO RECORDING MAGIC
        private WaveInEvent waveIn;
        private BufferedWaveProvider bufferedWaveProvider;
        private WaveFileWriter writer;
        private WaveOut waveOut;
        private WaveFileReader reader;

        private string wavFileName = @"sup.wav";
        private bool isRecording = false;
        private System.Timers.Timer recordTimer;

        public DiscoBall(string fromaddress = "", string fromimageurl = "", string toaddress = "", string toimageurl = "", bool isprivate = false)
        {
            InitializeComponent();
            _fromaddress = fromaddress;
            _toaddress = toaddress;
            _fromimageurl = fromimageurl;
            _toimageurl = toimageurl;

            if (isprivate) { btnEncryptionStatus.Text = "PRIVATE 🤐"; }

        }


        private void DiscoBall_Load(object sender, EventArgs e)
        {
            fromImage.ImageLocation = _fromimageurl;
            toImage.ImageLocation = _toimageurl;
            txtFromAddress.Text = _fromaddress;
            txtToAddress.Text = _toaddress;

            // GPT3 Initialize NAudio objects for recording and playback
            waveIn = new WaveInEvent();
            waveIn.BufferMilliseconds = 100; // Increase the buffer size (adjust as needed)
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.RecordingStopped += WaveIn_RecordingStopped;
            bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
            bufferedWaveProvider.BufferLength = waveIn.BufferMilliseconds * 2 * waveIn.WaveFormat.AverageBytesPerSecond;
            // Initialize the timer to prevent loss of last few seconds of recordings
            recordTimer = new System.Timers.Timer();
            recordTimer.Interval = 1000; // 2 seconds
            recordTimer.Elapsed += RecordTimer_Elapsed;
            recordTimer.AutoReset = false; // Only trigger once

        }

        public async void btnAttach_Click(object sender, EventArgs e)
        {
            if (flowAttachments.Controls.Count < 6)
            {
                string imgurn = "";
                List<string> imageExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", "" };


                if (txtAttach.Text != "")
                {
                    imgurn = txtAttach.Text;

                    if (!txtAttach.Text.ToLower().StartsWith("http"))
                    {
                        imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + txtAttach.Text.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                        if (txtAttach.Text.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                    }
                }
                else
                {
                    string ipfsHash = "";
                    // Open file dialog and get file path and name
                    System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog1.FileName;
                        string fileName = openFileDialog1.SafeFileName;



                        string proccessingFile = filePath;
                        string processingid = Guid.NewGuid().ToString();

                        if (btnEncryptionStatus.Text == "PRIVATE 🤐")
                        {

                            byte[] rootbytes = Root.GetRootBytesByFile(new string[] { filePath });
                            PROState toProfile = PROState.GetProfileByAddress(txtToAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");
                            rootbytes = Root.EncryptRootBytes("good-user", "better-password", @"http://127.0.0.1:18332", txtToAddress.Text, rootbytes, toProfile.PKX, toProfile.PKY, true);
                            string proccessingDirectory = @"root\" + processingid;
                            Directory.CreateDirectory(proccessingDirectory);
                            proccessingFile = proccessingDirectory + @"\SEC";
                            File.WriteAllBytes(proccessingFile, rootbytes);

                        }



                        // Add file to IPFS
                        Task<string> addTask = Task.Run(() =>
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = @"ipfs\ipfs.exe";
                            process.StartInfo.Arguments = "add \"" + proccessingFile + "\"";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.CreateNoWindow = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.Start();
                            string output = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();
                            string hash = output.Split(' ')[1];

                            PictureBox pictureBox = new PictureBox();
                            if (btnEncryptionStatus.Text == "PRIVATE 🤐")
                            { pictureBox.Tag = "IPFS:" + hash + @"\SEC"; }
                            else
                            {
                                // Set the PictureBox properties
                                pictureBox.Tag = "IPFS:" + hash + @"\" + fileName;
                            }
                            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                            pictureBox.Width = 50;
                            pictureBox.Height = 50;


                            string extension = Path.GetExtension(filePath).ToLower();
                            if (imageExtensions.Contains(extension))
                            {

                                pictureBox.ImageLocation = filePath;
                                pictureBox.MouseClick += PictureBox_MouseClick;

                            }
                            else {               
                                    pictureBox.ImageLocation = @"includes\HugPuddle.jpg";
                                    pictureBox.MouseClick += PictureBox_MouseClick;
                                  }
                            
                            this.Invoke((MethodInvoker)delegate
                            {
                                flowAttachments.Controls.Add(pictureBox);
                            });

                            return "IPFS:" + hash;
                        });
                        ipfsHash = await addTask;
                        try { Directory.Delete(@"root\" + processingid, true); } catch { }
                    }



                }


                string extension2 = Path.GetExtension(imgurn).ToLower();

                if (imageExtensions.Contains(extension2))
                {


                    try
                    {
                        Root root = new Root();
                        Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                        Match urimatch = regexTransactionId.Match(txtAttach.Text);
                        string transactionid = urimatch.Value;
                        switch (txtAttach.Text.Substring(0, 4).ToUpper())
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
                                if (txtAttach.Text.Length == 51) { imgurn += @"\artifact"; }
                                if (!System.IO.Directory.Exists(@"ipfs/" + txtAttach.Text.Substring(5, 46) + "-build") && !System.IO.File.Exists(@"ipfs/" + txtAttach.Text.Substring(5, 46)))
                                {


                                    Task ipfsTask = Task.Run(() =>
                                    {
                                        Directory.CreateDirectory(@"ipfs/" + txtAttach.Text.Substring(5, 46) + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + txtAttach.Text.Substring(5, 46) + @" -o ipfs\" + txtAttach.Text.Substring(5, 46);
                                        process2.Start();
                                        process2.WaitForExit();

                                        if (System.IO.File.Exists("ipfs/" + txtAttach.Text.Substring(5, 46)))
                                        {
                                            try { System.IO.File.Move("ipfs/" + txtAttach.Text.Substring(5, 46), "ipfs/" + txtAttach.Text.Substring(5, 46) + "_tmp"); }
                                            catch
                                            {

                                                System.IO.File.Delete("ipfs/" + txtAttach.Text.Substring(5, 46) + "_tmp");
                                                System.IO.File.Move("ipfs/" + txtAttach.Text.Substring(5, 46), "ipfs/" + txtAttach.Text.Substring(5, 46) + "_tmp");

                                            }

                                            string fileName = txtAttach.Text.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "")
                                            {
                                                fileName = "artifact";
                                            }
                                            else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                            Directory.CreateDirectory(@"ipfs/" + txtAttach.Text.Substring(5, 46));
                                            try { System.IO.File.Move("ipfs/" + txtAttach.Text.Substring(5, 46) + "_tmp", imgurn); } catch { }
                                        }

                                        var SUP = new Options { CreateIfMissing = true };

                                        using (var db = new DB(SUP, @"ipfs"))
                                        {

                                            string ipfsdaemon = db.Get("ipfs-daemon");

                                            if (ipfsdaemon == "true")
                                            {
                                                Process process3 = new Process
                                                {
                                                    StartInfo = new ProcessStartInfo
                                                    {
                                                        FileName = @"ipfs\ipfs.exe",
                                                        Arguments = "pin add " + txtAttach.Text.Substring(5, 46),
                                                        UseShellExecute = false,
                                                        CreateNoWindow = true
                                                    }
                                                };
                                                process3.Start();
                                            }
                                        }

                                        try { Directory.Delete(@"ipfs/" + txtAttach.Text.Substring(5, 46)); } catch { }
                                        try
                                        {
                                            Directory.Delete(@"ipfs/" + txtAttach.Text.Substring(5, 46) + "-build");
                                        }
                                        catch { }



                                    });

                                }
                                else
                                {

                                }
                                break;
                            default:

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");


                                break;
                        }


                    }
                    catch { }

                    if (File.Exists(imgurn) || imgurn.ToUpper().StartsWith("HTTP"))
                    {
                        PictureBox pictureBox = new PictureBox();

                        // Set the PictureBox properties
                        pictureBox.Tag = txtAttach.Text;
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox.Width = 50;
                        pictureBox.Height = 50;
                        pictureBox.ImageLocation = imgurn;
                        pictureBox.MouseClick += PictureBox_MouseClick;
                        // Add the PictureBox to the FlowLayoutPanel
                        this.Invoke((MethodInvoker)delegate
                        {
                            flowAttachments.Controls.Add(pictureBox);
                        });

                    }


                }
                else
                {
                    PictureBox pictureBox = new PictureBox();

                    // Set the PictureBox properties
                    pictureBox.Tag = txtAttach.Text;
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Width = 50;
                    pictureBox.Height = 50;
                    pictureBox.ImageLocation = @"includes\HugPuddle.jpg";
                    pictureBox.MouseClick += PictureBox_MouseClick;
                    // Add the PictureBox to the FlowLayoutPanel
                    this.Invoke((MethodInvoker)delegate
                    {
                        flowAttachments.Controls.Add(pictureBox);
                    });

                }




            }
            this.Invoke((MethodInvoker)delegate
            {

                txtAttach.Text = "";
            });
        }
        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pictureBox = (PictureBox)sender;
                flowAttachments.Controls.Remove(pictureBox);

            }
        }
        private void discoButton_Click(object sender, EventArgs e)
        {

            string transMessage = supMessage.Text;
            List<string> encodedList = new List<string>();
            foreach (Control attach in flowAttachments.Controls)
            {
                transMessage = transMessage + "<<" + attach.Tag.ToString() + ">>";
            }
            int salt;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[4];
                rng.GetBytes(saltBytes);
                salt = -Math.Abs(BitConverter.ToInt32(saltBytes, 0) % 100000);
            }
            transMessage = transMessage + "<<" + salt.ToString() + ">>";

            string OBJP2FK = ">" + transMessage.Length + ">" + transMessage;
            byte[] OBJP2FKBytes = new byte[] { };
            PROState toProfile = PROState.GetProfileByAddress(_toaddress, "good-user", "better-password", @"http://127.0.0.1:18332");


            NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
            RPCClient rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:18332"), Network.Main);
            System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
            byte[] hashValue = new byte[] { };
            hashValue = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(OBJP2FK));

            string signatureAddress;

            signatureAddress = txtFromAddress.Text;
            string signature = "";
            try { signature = rpcClient.SendCommand("signmessage", signatureAddress, BitConverter.ToString(hashValue).Replace("-", String.Empty)).ResultString; }
            catch (Exception ex)
            {
                lblObjectStatus.Text = ex.Message;

                return;
            }


            if (btnEncryptionStatus.Text == "PRIVATE 🤐")
            {
                OBJP2FK = "SIG" + ":" + "88" + ">" + signature + OBJP2FK;
                byte[] combinedBytes = Root.EncryptRootBytes("good-user", "better-password", "http://127.0.0.1:18332", signatureAddress, Encoding.UTF8.GetBytes(OBJP2FK), toProfile.PKX, toProfile.PKY);
                // Split byte array into chunks of maximum length 20
                for (int i = 0; i < combinedBytes.Length; i += 20)
                {
                    byte[] bytechunk = combinedBytes.Skip(i).Take(20).ToArray();
                    string address = "";

                    if (bytechunk.Length < 20)
                    {
                        int diff = 20 - bytechunk.Length;
                        byte[] paddedBytes = bytechunk.Concat(new byte[diff].Select(x => (byte)'#')).ToArray();
                        bytechunk = paddedBytes;
                    }

                    address = Base58.EncodeWithCheckSum(new byte[] { byte.Parse("111") }.Concat(bytechunk).ToArray());
                    encodedList.Add(address);
                }
            }
            else
            {
                OBJP2FK = "SIG" + ":" + "88" + ">" + signature + OBJP2FK;

                for (int i = 0; i < OBJP2FK.Length; i += 20)
                {
                    string chunk = OBJP2FK.Substring(i, Math.Min(20, OBJP2FK.Length - i));
                    if (chunk.Any())
                    {
                        encodedList.Add(Root.GetPublicAddressByKeyword(chunk));
                    }
                }
            }
            string pattern = @"#\w{1,20}";

            Regex regex = new Regex(pattern);

            foreach (Match match in regex.Matches(supMessage.Text))
            {
                string keyword = match.Value.Substring(1);

                encodedList.Add(Root.GetPublicAddressByKeyword(keyword));
            }

            encodedList.Add(txtToAddress.Text);
            encodedList.Add(signatureAddress);
            lblObjectStatus.Text = "cost: " + (0.00000546 * encodedList.Count).ToString("0.00000000") + "  + miner fee";

            if (File.Exists(@"WALKIE_TALKIE_ENABLED") && flowAttachments.Contains(this.btnPlay))
            {

                // Perform the action
                var recipients = new Dictionary<string, decimal>();
                foreach (var encodedAddress in encodedList)
                {
                    try { recipients.Add(encodedAddress, 0.00000546m); } catch { }
                }

                CoinRPC a = new CoinRPC(new Uri("http://127.0.0.1:18332"), new NetworkCredential("good-user", "better-password"));

                try
                {
                    string accountsString = "";
                    try { accountsString = rpcClient.SendCommand("listaccounts").ResultString; } catch { }
                    var accounts = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(accountsString);
                    var keyWithLargestValue = accounts.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                    var results = a.SendMany(keyWithLargestValue, recipients);
                    lblTransactionId.Text = results;
                }
                catch (Exception ex) { lblObjectStatus.Text = ex.Message; }

            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you want to send this?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    // Perform the action
                    var recipients = new Dictionary<string, decimal>();
                    foreach (var encodedAddress in encodedList)
                    {
                        try { recipients.Add(encodedAddress, 0.00000546m); } catch { }
                    }

                    CoinRPC a = new CoinRPC(new Uri("http://127.0.0.1:18332"), new NetworkCredential("good-user", "better-password"));

                    try
                    {
                        string accountsString = "";
                        try { accountsString = rpcClient.SendCommand("listaccounts").ResultString; } catch { }
                        var accounts = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(accountsString);
                        var keyWithLargestValue = accounts.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                        var results = a.SendMany(keyWithLargestValue, recipients);
                        lblTransactionId.Text = results;
                    }
                    catch (Exception ex) { lblObjectStatus.Text = ex.Message; }


                }


            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            GifTool gifToolForm = new GifTool(this); // Pass the reference to the current form as the parent form
            gifToolForm.ShowDialog();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Clean up NAudio resources
            waveIn?.Dispose();
            writer?.Dispose();

            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
            try { File.Delete(wavFileName); } catch { } 
        }
        private void BtnRecord_MouseDown(object sender, MouseEventArgs e)
        {
            btnRecord.BackColor = System.Drawing.Color.Blue;
            btnRecord.ForeColor = System.Drawing.Color.Yellow;
            // Start recording audio if not already recording
            if (!isRecording)
            {
                waveIn.StartRecording();
                isRecording = true;
            }
        }
        private void BtnRecord_MouseUp(object sender, MouseEventArgs e)
        {
            recordTimer.Start(); // Start the delay timer when the button is released

        }
        private void BtnPlay_Click(object sender, MouseEventArgs e)
        {

            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

           
                waveOut = new WaveOut();
                waveOut.PlaybackStopped += waveOut_PlaybackStopped;
                reader = new WaveFileReader(wavFileName);
                waveOut.Init(reader);
                waveOut.Play();
            
        }
        private void BtnPlay_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                flowAttachments.Controls.Remove(this.btnPlay);
                try { File.Delete(wavFileName); } catch { }
            }
        }
        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // Add recorded data to the bufferedWaveProvider
            bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);

            // If writer is not initialized and recording has started, create the WaveFileWriter
            if (writer == null && isRecording)
            {
                writer = new WaveFileWriter(wavFileName, waveIn.WaveFormat);
            }

            // If writer is initialized, write the recorded data to the file
            if (writer != null)
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);

                // Flush the writer to ensure data is written immediately (optional, but recommended)
                writer.Flush();
            }
        }
        private async void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            // Clean up after recording is stopped
            if (writer != null) {
                isRecording = false;
                writer?.Dispose();
                writer = null;
            }
          
            string proccessingFile = wavFileName;
            string processingid = Guid.NewGuid().ToString();
            string ipfsHash = "";

            flowAttachments.Controls.Clear();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnPlay.Font = new System.Drawing.Font("Segoe UI Emoji", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlay.ForeColor = System.Drawing.Color.Black;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnPlay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnPlay.Size = new System.Drawing.Size(50, 46);
            this.btnPlay.Text = "▶️";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.MouseClick += new MouseEventHandler(BtnPlay_Click);
            this.btnPlay.MouseUp += new MouseEventHandler(BtnPlay_MouseUp);

            if (btnEncryptionStatus.Text == "PRIVATE 🤐")
            {
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }

                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }

                byte[] rootbytes = Root.GetRootBytesByFile(new string[] { wavFileName });
                PROState toProfile = PROState.GetProfileByAddress(txtToAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");
                rootbytes = Root.EncryptRootBytes("good-user", "better-password", @"http://127.0.0.1:18332", txtToAddress.Text, rootbytes, toProfile.PKX, toProfile.PKY, true);
                string proccessingDirectory = @"root\" + processingid;
                Directory.CreateDirectory(proccessingDirectory);
                proccessingFile = proccessingDirectory + @"\SEC";
                File.WriteAllBytes(proccessingFile, rootbytes);

            }

            // Add file to IPFS
            Task<string> addTask = Task.Run(() =>
            {
                Process process = new Process();
                process.StartInfo.FileName = @"ipfs\ipfs.exe";
                process.StartInfo.Arguments = "add \"" + proccessingFile + "\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                string hash = output.Split(' ')[1];

                this.Invoke((MethodInvoker)delegate
                {
                    if (btnEncryptionStatus.Text == "PRIVATE 🤐")
                { 
                    this.btnPlay.Tag = "IPFS:" + hash + @"\SEC"; }
                else
                {
                    // Set the PictureBox properties
                    this.btnPlay.Tag = "IPFS:" + hash + @"\" + wavFileName;
                }
            
                flowAttachments.Controls.Add(this.btnPlay);
                });

                return "IPFS:" + hash;
            });
            ipfsHash = await addTask; 

         try { Directory.Delete(@"root\" + processingid, true); } catch { }

            if (File.Exists(@"WALKIE_TALKIE_ENABLED"))
            {
                btnRefresh.PerformClick();
            }



        }
        private void waveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
        }
        private void RecordTimer_Elapsed(object sender, ElapsedEventArgs e)
                    {
            btnRecord.BackColor = System.Drawing.Color.White;
            btnRecord.ForeColor = System.Drawing.Color.Black;
            if (isRecording)
            {
                waveIn.StopRecording();
                isRecording = false;

                // Dispose the MemoryStream to release resources
                waveIn?.Dispose();

                // Save the recorded data to a WAV file
                if (writer != null)
                {
                    writer.Dispose();
                    writer = null;
                }
            }
        }


    }
}
