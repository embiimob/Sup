using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using SUP.RPCClient;
using NBitcoin;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using NAudio.Wave;
using System.Drawing;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using AngleSharp.Html.Dom;

namespace SUP
{
    public partial class DiscoBall : Form
    {
        private string _fromaddress;
        private string _toaddress;
        private string _fromimageurl;
        private string _toimageurl;
        private string messagecache;

        // GPT3 AUDIO RECORDING MAGIC
        private WaveInEvent waveIn;
        private BufferedWaveProvider bufferedWaveProvider;
        private WaveFileWriter writer;
        private WaveOut waveOut;
        private WaveFileReader reader;
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        private string wavFileName = @"sup.wav";
        private bool isRecording = false;
        private bool isPrint = false;
        private System.Timers.Timer recordTimer;
        private DateTime startTime;
        private QrEncoder encoder = new QrEncoder();
        private GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(2, QuietZoneModules.Two));
        System.Drawing.Image bmIm;
        private Random random = new Random();


        public DiscoBall(string fromaddress = "", string fromimageurl = "", string toaddress = "", string toimageurl = "", bool isprivate = false, bool testnet = true)
        {
            InitializeComponent();
            CreateEmojiPanel();
            _fromaddress = fromaddress;
            _toaddress = toaddress;
            _fromimageurl = fromimageurl;
            _toimageurl = toimageurl;

            if (isprivate) { btnEncryptionStatus.Text = "PRIVATE 🤐"; btnInquiry.Visible = false; }

            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
            myTooltip.SetToolTip(supMessage, "enter the text of your message here. you can also include searchable #keywords.\nto include #keywords without them showing surround them with <<  >>\nexample << #rad #radical #RAD #RADICAL >>\n\nnote: if you are attaching a gif and you would like to include it in the default gif results\nadd the keyword #gif in your message along with a few #keywords to help other sup users find it.");
            myTooltip.SetToolTip(btnAttach, "click to attach any url entered in the url text box.  if no url is listed you will be prompted to select a file.\nyour file will be uploaded to IPFS and attached to the message.\nnote: if private your file will be encrypted prior to being uploaded to IPFS and attached.");
            myTooltip.SetToolTip(btnEMOJI, "click to select and add an emoji to your message.");
            myTooltip.SetToolTip(btnGIF, "click to select and add a gif to your message.");
            myTooltip.SetToolTip(btnInquiry, "click to create and add a poll to your message.");
            myTooltip.SetToolTip(btnPrint, "click to generate a paper message. right click to print or save it to disk.\npaper messages can be sent via US MAIL if private they can only be read by the recipient.\nnote: paper messages require a mobile app to view ( still in development ).");
            myTooltip.SetToolTip(btnRecord, "click and hold this button to record an audio message.\nrelease the button when finished and it will be attached to your message.\nleft click the attachment to review your recording.\nif you are not happy, right click to remove it and try again.");
            myTooltip.SetToolTip(btnRefresh, "click to etch your message to the active blockchain");
            myTooltip.SetToolTip(btnEncryptionStatus, "this indicator informs you if the current message is\npublic ( viewable by everyone ) or private ( viewable by the recipient only )");
            myTooltip.SetToolTip(btnFromSelector, "click to select from a list of local profiles.");
            myTooltip.SetToolTip(btnToSelector, "click to select a profile you are currently following.");

            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Add a "Save to Disk" menu item
            ToolStripMenuItem hideMenuItem = new ToolStripMenuItem("Exit");
            ToolStripMenuItem saveMenuItem = new ToolStripMenuItem("Save to Disk");
            ToolStripMenuItem printMenuItem = new ToolStripMenuItem("Print");
            saveMenuItem.Click += SaveMenuItem_Click;
            hideMenuItem.Click += HideMenuItem_Click;
            printMenuItem.Click += PrintMenuItem_Click;
            contextMenu.Items.Add(hideMenuItem);
            contextMenu.Items.Add(saveMenuItem);
            contextMenu.Items.Add(printMenuItem);

            // Assign the context menu to the PictureBox
            pictureBox1.ContextMenuStrip = contextMenu;
            if (!testnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";

            }

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
        private string GetRandomDelimiter()
        {
            string[] delimiters = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            return delimiters[random.Next(delimiters.Length)];
        }

        private void PrintImage(System.Drawing.Image img)
        {
            bmIm = img;
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += this.pd_PrintPage;
            pd.OriginAtMargins = false;
            pd.DefaultPageSettings.Landscape = false;
            pd.Print();
        }
        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {

            System.Drawing.Image i = bmIm;

            float newWidth = i.Width * 100 / i.HorizontalResolution;
            float newHeight = i.Height * 100 / i.VerticalResolution;

            float widthFactor = newWidth / e.PageBounds.Width;
            float heightFactor = newHeight / e.PageBounds.Height;

            if (widthFactor > 1 || heightFactor > 1)
            {
                if (widthFactor > heightFactor)
                {
                    newWidth = newWidth / widthFactor;
                    newHeight = newHeight / widthFactor;
                }
                else
                {
                    newWidth = newWidth / heightFactor;
                    newHeight = newHeight / heightFactor;
                }
            }

            // Calculate the x and y coordinates of the top-left corner of the image
            float x = (e.PageBounds.Width - newWidth) / 2;
            float y = (e.PageBounds.Height - newHeight) / 2;

            e.Graphics.DrawImage(i, x, y, (int)newWidth, (int)newHeight);
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


                                            try
                                            {
                                                if (File.Exists("IPFS_PINNING_ENABLED"))
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
                                            catch { }



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

                                    root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);


                                    break;
                            }


                        }
                        catch { }

                        if (File.Exists(imgurn) || (imgurn.ToUpper().StartsWith("HTTP") && extension2 != ""))
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
                else
                {
                    System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog1.FileName;
                        ProcessFileAsync(filePath);
                    }



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

            if (txtAttach.Text.Length > 0)
            {
                DialogResult result = MessageBox.Show("You have an unattached URL in the attachment box.\nMake sure to click the 📎 to attach a URL to your message.\nAre you sure you want to send this?", "Confirmation", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                {
                    return;
                }
            }


            string transMessage = supMessage.Text;
            List<string> encodedList = new List<string>();
            foreach (Control attach in flowAttachments.Controls)
            {
                if (attach.Tag != null)
                {
                    transMessage = transMessage + "<<" + attach.Tag.ToString() + ">>";
                }

            }
            int salt;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[4];
                rng.GetBytes(saltBytes);
                salt = -Math.Abs(BitConverter.ToInt32(saltBytes, 0) % 100000);
            }
            transMessage = transMessage + "<<" + salt.ToString() + ">>";
            byte[] messageBytes = Encoding.UTF8.GetBytes(transMessage);
            string OBJP2FK = GetRandomDelimiter() + messageBytes.Length + GetRandomDelimiter() + transMessage + txtINQJson.Text;
            byte[] OBJP2FKBytes = new byte[] { };
            PROState toProfile = PROState.GetProfileByAddress(_toaddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
            string signature = "";
            string signatureAddress = "";
            NetworkCredential credentials = new NetworkCredential(mainnetLogin, mainnetPassword);
            NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);

            if (txtFromAddress.Text != "")
            {

                System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                byte[] hashValue = new byte[] { };
                hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(OBJP2FK));



                signatureAddress = txtFromAddress.Text;

                try { signature = rpcClient.SendCommand("signmessage", signatureAddress, BitConverter.ToString(hashValue).Replace("-", String.Empty)).ResultString; }
                catch (Exception ex)
                {
                    lblObjectStatus.Text = ex.Message;

                    return;
                }
            }

            if (btnEncryptionStatus.Text == "PRIVATE 🤐")
            {
                OBJP2FK = "SIG" + GetRandomDelimiter() + "88" + GetRandomDelimiter() + signature + OBJP2FK;
                byte[] combinedBytes = Root.EncryptRootBytes(mainnetLogin, mainnetPassword, mainnetURL, signatureAddress, Encoding.UTF8.GetBytes(OBJP2FK), toProfile.PKX, toProfile.PKY);
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

                    address = Base58.EncodeWithCheckSum(new byte[] { byte.Parse(mainnetVersionByte) }.Concat(bytechunk).ToArray());
                    encodedList.Add(address);
                }
            }
            else
            {
                if (txtFromAddress.Text != "")
                {
                    OBJP2FK = "SIG" + GetRandomDelimiter() + "88" + GetRandomDelimiter() + signature + OBJP2FK;

                }

                byte[] inputBytes = Encoding.UTF8.GetBytes(OBJP2FK); // Convert the string to bytes

                for (int i = 0; i < inputBytes.Length; i += 20)
                {
                    byte[] chunkBytes = new byte[Math.Min(20, inputBytes.Length - i)];
                    Array.Copy(inputBytes, i, chunkBytes, 0, chunkBytes.Length);

                    // Right-pad the chunkBytes with '#' if it's less than 20 bytes
                    if (chunkBytes.Length < 20)
                    {
                        byte[] paddedChunkBytes = new byte[20];
                        Array.Copy(chunkBytes, paddedChunkBytes, chunkBytes.Length);
                        for (int j = chunkBytes.Length; j < 20; j++)
                        {
                            paddedChunkBytes[j] = (byte)'#';
                        }
                        chunkBytes = paddedChunkBytes;
                    }

                    string chunkBase58 = Base58.EncodeWithCheckSum(
                        new byte[] { (byte)Int32.Parse(mainnetVersionByte) }.Concat(chunkBytes).ToArray());

                    if (!encodedList.Contains(chunkBase58))
                    {
                        encodedList.Add(chunkBase58);
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("The following duplicate information was detected: [  " + chunkBase58 + "  ]. Sorry, you must still use Apertus.io for etchings that require repetitive data", "Confirmation", MessageBoxButtons.OK);
                    }
                }


            }


            string pattern = @"#[^\s]{1,20}";


            Regex regex = new Regex(pattern);

            foreach (Match match in regex.Matches(supMessage.Text))
            {
                string keyword = match.Value.Substring(1);
                string encodedKeyword = Root.GetPublicAddressByKeyword(keyword, mainnetVersionByte);
                string P2FKASCII = Root.GetKeywordByPublicAddress(encodedKeyword, "ASCII");


                Regex regexSpecialChars = new Regex(@"([\\/:*?""<>|])+");

                if (regexSpecialChars.IsMatch(P2FKASCII))
                {
                    MessageBox.Show("Sup!? Found characters within a #keyword " + keyword + " that could corrupt the message. Use at your own risk!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                encodedList.Add(encodedKeyword);



            }

            // Remove spaces from txtToAddress.Text
            string inputText = txtToAddress.Text.Replace(" ", "");

            // Split the input text by comma or semicolon
            char[] delimiters = { ',', ';' };
            string[] addresses = inputText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            // Iterate through the addresses in reverse order and add them to the beginning of encodedList if not already present
            for (int i = addresses.Length - 1; i >= 0; i--)
            {
                string address = addresses[i].Trim(); // Trim any leading/trailing spaces
                if (!encodedList.Contains(address) && address != signatureAddress)
                {
                    encodedList.Add(address); // Add to the beginning of the list
                }
            }

            //this will add the order specific inq address if one exists.
            if (txtINQAddress.Text != "") { encodedList.Add(txtINQAddress.Text); }
            if (txtFromAddress.Text != "") { encodedList.Add(signatureAddress); }
            lblObjectStatus.Text = "cost: " + (0.00000546 * encodedList.Count).ToString("0.00000000") + "  + miner fee";

            if (File.Exists(@"WALKIE_TALKIE_ENABLED") && flowAttachments.Contains(this.btnPlay))
            {

                // Perform the action
                var recipients = new Dictionary<string, decimal>();
                foreach (var encodedAddress in encodedList)
                {
                    try { recipients.Add(encodedAddress, 0.00000546m); } catch { }
                }

                CoinRPC a = new CoinRPC(new Uri(mainnetURL), new NetworkCredential(mainnetLogin, mainnetPassword));

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
                if (isPrint)
                {
                    var recipients = new Dictionary<string, decimal>();
                    foreach (var encodedAddress in encodedList)
                    {
                        try { recipients.Add(encodedAddress, 0.00000546m); } catch { }
                    }
                    string addressList = JsonConvert.SerializeObject(recipients);
                    PrintMessage(addressList);
                    isPrint = false;
                    //pictureBox1.Visible=false;

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

                        CoinRPC a = new CoinRPC(new Uri(mainnetURL), new NetworkCredential(mainnetLogin, mainnetPassword));

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
        }

        private void btnGIF_Click(object sender, EventArgs e)
        {
            GifTool gifToolForm = new GifTool(this); // Pass the reference to the current form as the parent form
            gifToolForm.ShowDialog();
        }

        private void btnEMOJI_Click(object sender, EventArgs e)
        {
            // Show/hide the emoji panel based on its current visibility
            emojiPanel.Visible = !emojiPanel.Visible;
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
                startTime = DateTime.Now;
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
            if (writer != null)
            {
                isRecording = false;
                writer?.Dispose();
                writer = null;
            }

            TimeSpan duration = DateTime.Now - startTime;

            if (duration.TotalSeconds < 2.2)
            {
                // Do nothing, as the time difference is less than 2.2 second
                return;
            }



            string proccessingFile = wavFileName;
            string processingid = Guid.NewGuid().ToString();
            string ipfsHash = "";


            try
            {
                // Attempt to remove the existing btnINQ control
                flowAttachments.Controls.RemoveByKey("btnPlay");
            }
            catch (ArgumentException)
            {
                // Control with the specified name doesn't exist, so no need to handle the exception
            }

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
                PROState toProfile = PROState.GetProfileByAddress(txtToAddress.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                rootbytes = Root.EncryptRootBytes(mainnetLogin, mainnetPassword, mainnetURL, txtToAddress.Text, rootbytes, toProfile.PKX, toProfile.PKY, true);
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
                        this.btnPlay.Tag = "IPFS:" + hash + @"\SEC";
                    }
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

        private void CreateEmojiPanel()
        {
            emojiPanel.BorderStyle = BorderStyle.FixedSingle;

            string[] emojis = {
    "😀", "😃", "😄", "😁", "😆", "😅", "😂", "🤣", "😊", "😍",
    "🥰", "😘", "😚", "😋", "😎", "😻", "🤗", "🤩", "🥳", "😏",
    "😒", "😞", "😔", "😟", "😕", "🙁", "☹️", "😣", "😢", "😭",
    "😤", "😩", "🥺", "😰", "😱", "😨", "😢", "😠", "😡", "🤯",
    "😳", "😫", "😖", "😣", "😮", "😩", "🥱", "😪", "😴", "😷",
    "🤒", "🤕", "🤢", "🤮", "🤧", "😵", "🤨", "🧐", "😐", "😑",
    "😶", "😇", "🙄", "🤔", "🤫", "🤭", "🤥", "😐", "😑", "😬",
    "🙁", "☹️", "😦", "😧", "😮", "😲", "🥴", "🤤", "😴", "😪",
    "😵", "🤐", "🥺", "🥴", "😬", "🤫", "🤭", "🧐", "🤓", "😈",
    "👿", "👹", "👺", "💀", "👻", "👽", "🤖", "💩", "😺", "😸",
    "😹", "😻", "😼", "😽", "🙀", "😿", "😾", "👐", "🙌", "👏",
    "🤝", "👍", "👎", "👊", "✊", "🤛", "🤜", "🤞", "✌️", "🤘",
    "👌", "👈", "👉", "👆", "👇", "☝️", "✋", "🤚", "🖐", "🖖",
    "👋", "🤟", "✍️", "🙏", "💪", "🦾", "🖕", "🦵", "🦿", "🦶",
    "👂", "🦻", "👃", "🧠", "🦷", "🦴", "👀", "👁", "👅", "👄",
    "🔥", "💥", "💣", "🌟", "🎉", "🎊", "🎈", "👑", "🎩", "🧢",
    "💍", "💎", "🐦", "📱", "💻", "🖥", "⌨️", "📲", "📞", "☎️",
    "📧", "📥", "📤", "✉️", "📬", "📮", "📯", "📢", "📣", "📠",
    "🔊", "🔔", "🔕", "📻", "🎙", "🎚", "🎛", "🎤", "🎧", "🎶",
    "🎵", "🥁", "🎷", "🎺", "🎸","🎻", "🎹", "🎤",
    "🎬", "📺", "📽", "🎥", "🍿", "🎞", "🎦", "🏟", "👾", "🕹️",
    "🎮", "🎲", "♟️", "🧩", "🧸", "🎯", "🎳", "🎮", "🎰",
    "🚀", "🛸", "🌍", "🌎", "🌏", "🌐", "🪐", "☄️", "🌠", "🌌",
    "🎇", "🎆", "🌃", "🌆", "🌅", "🌄", "🌇", "🌞", "🌝", "🌛",
    "🌜", "🌚", "🌙", "🌕", "🌖", "🌗", "🌘", "🌑", "🌒", "🌓",
    "🌔", "🌙", "🌚", "🌛", "🌜", "🌡", "🌤", "⛅", "🌥", "🌦",
    "🌧", "⛈️", "🌩", "🌨", "❄️", "☃️", "⛄", "🌬", "💨", "🌪",
    "🌫", "🌦", "🌈", "☔", "🌧", "🌩", "🌨", "❄️", "⛄", "🍃",
    "🍂", "🍁", "🌾", "🌻", "🌼", "🌸", "💮", "🏵", "🌹", "🥀",
    "🌺", "🌷", "🌱", "🌴", "🌲", "🌳", "🌵", "🌿", "☘️", "🍀",
    "🍁", "🍂", "🍃", "🍄", "🌰", "🐚", "🍏", "🍎", "🍐", "🍊",
    "🍋", "🍌", "🍉", "🍇", "🍓", "🍈", "🍒", "🍑", "🥭", "🍍",
    "🥥", "🥝", "🍅", "🍆", "🥑", "🥦", "🥒", "🌶", "🌽", "🥕",
    "🍄", "🥔", "🍠", "🥐", "🍞", "🥖", "🥨", "🧀", "🥚", "🍳",
    "🥓", "🥩", "🍗", "🍖", "🦴", "🌭", "🍔", "🍟", "🍕", "🥪",
    "🥙", "🌮", "🌯", "🥗", "🥘", "🍝", "🍜", "🍲", "🍛", "🍣",
    "🍱", "🥟", "🍤", "🍢", "🍡", "🍦", "🍧", "🍨", "🍩", "🍪",
    "🎂", "🍰", "🧁", "🥧", "🍫", "🍬", "🍭", "🍮", "🍯", "🍼",
    "🥤", "🍺", "🍻", "🥂", "🍷", "🥃", "🍸", "🍹", "🍾", "🥄",
    "🍴", "🧂", "🍽️", "🥢", "🧇", "🥞", "🥡", "🧆", "🥤", "🥢",
    "🍽️", "🥄", "🍴", "🥂", "🍸", "🍷", "🥃", "🍾", "🍻", "🍺",
    "🍯", "🍮", "🍭", "🍬", "🍫", "🎂", "🍰", "🥧", "🍩", "🍨",
    "🍧", "🍦", "🍡", "🍢", "🍤", "🥟", "🍱", "🍣", "🍛", "🍜",
    "🍝", "🥘", "🥗", "🌯", "🌮", "🥪", "🍕", "🍟", "🍔", "🌭",
    "🦴", "🍖", "🍗",
    "👶", "🧒", "👦", "👧", "🧑", "👱‍♂️", "👱‍♀️", "🧔", "👨", "🧔‍♂️",
    "👨‍🦰", "👨‍🦱", "👨‍🦳", "👨‍🦲", "🧑‍🦰", "🧑‍🦱", "🧑‍🦳", "🧑‍🦲", "👩", "👩‍🦰",
    "👩‍🦱", "👩‍🦳", "👩‍🦲", "🧑‍🦰", "🧑‍🦱", "🧑‍🦳", "🧑‍🦲", "🧓", "👴", "👵",
    "🙍‍♂️", "🙍‍♀️", "🙎‍♂️", "🙎‍♀️", "🙅‍♂️", "🙅‍♀️", "🙆‍♂️", "🙆‍♀️", "💁‍♂️", "💁‍♀️",
    "🙋‍♂️", "🙋‍♀️", "🧏‍♂️", "🧏‍♀️", "🙇‍♂️", "🙇‍♀️", "🤦‍♂️", "🤦‍♀️", "🤷‍♂️", "🤷‍♀️",
    "🚶‍♂️", "🚶‍♀️", "🏃‍♂️", "🏃‍♀️", "💃", "🕺", "🕴", "👯‍♂️", "👯‍♀️", "🧖‍♂️",
    "🧖‍♀️", "🧗‍♂️", "🧗‍♀️", "🤺", "🏇", "⛷️", "🏂", "🏌♂️", "🏌♀️", "🏄‍♂️",
    "🏄‍♀️", "🚣‍♂️", "🚣‍♀️", "🧘‍♂️", "🧘‍♀️", "🧖‍♂️", "🧖‍♀️", "🛀", "🛌", "🧑‍🤝‍🧑",
    "👬", "👭", "👫", "👩‍❤️‍👨", "👨‍❤️‍👨", "👩‍❤️‍👩", "💏", "👩‍❤️‍💋‍👨", "👨‍❤️‍💋‍👨", "👩‍","❤️‍🔥", "❤️‍🩹", "💑", "👩‍❤️‍💋‍👩", "👪", "👨‍👩‍👦", "👨‍👩‍👧", "👨‍👩‍👦‍👦", "👨‍👩‍👧‍👧", "👨‍👨‍👦",
    "👨‍👨‍👧", "👨‍👨‍👦‍👦", "👨‍👨‍👧‍👧", "👩‍👩‍👦", "👩‍👩‍👧", "👩‍👩‍👦‍👦", "👩‍👩‍👧‍👧", "👨‍👦", "👨‍👦‍👦", "👨‍👧",
    "👨‍👧‍👦", "👨‍👧‍👧", "👩‍👦", "👩‍👦‍👦", "👩‍👧", "👩‍👧‍👦", "👩‍👧‍👧", "🗣", "👤", "👥", "👣",
    "🧳", "🌂", "☂️", "🧵", "🧶", "🧷", "🧹", "🧺",
    "🧻", "🧼", "🛒", "🚬", "⚰️",
    "🪦", "⚱️", "🗿", "🏺", "🔮", "📿", "🧿", "💎", "💍",
    "🔒", "🔓", "🔏", "🔐", "🔑", "🗝️", "🚪",
    "🔨", "⛏️", "⚒️", "🛠", "🗡", "⚔️", "🔪", "🔫", "🪃", "🏹",
    "🛡",  "🧱", "🧳",  "🛏", "🛋", "🚽",
    "🚿", "🛁",  "🧴", "🧼",
    "🧽", "🧾", "🧻", "📃", "📄", "📑", "🧮", "📊",
    "📈", "📉", "📜", "📋", "📅", "📆", "🗓", "📇", "🗃", "🗳",
    "🗄", "📁", "📂", "🗂", "📰", "📓", "📔", "📒", "📕", "📗",
    "📘", "📙", "📚", "📖", "🔖", "🧷", "🔗", "📎", "🖇", "📐",
    "📏", "🧮", "📌", "📍", "✂️", "🖊", "🖋", "✒️", "🖌", "🖍",
    "📝", "✏️", "🔍", "🔎", "🔏", "🔐", "🔒", "🔓", "🏷", "💼",
    "📁", "📂", "🗂", "📅", "📆", "🗒", "🗓", "🖇", "📈", "📉",
    "📊", "📋", "📌", "📍", "📎", "🖊", "🖋", "✒️", "🖌", "🖍",
    "📝", "✏️", "🔍", "🔎", "🔏", "🔐", "🔒", "🔓", "🏷", "💼",
    "📸", "📷", "📹", "🎥", "📽", "🎞", "📞", "☎️", "📟", "📠",
    "🔋", "🔌", "💻", "🖥", "🖨", "⌨️", "🖱", "🖲", "💽", "💾",
    "💿", "📀", "🧮", "🎙", "🎚", "🎛", "🧭", "🔍", "🔎", "🔏",
    "🔐", "🔒", "🔓", "🏆", "🥇", "🥈", "🥉", "🏅", "🎖", "🏵",
    "🎗", "🎫", "🎟", "🎪", "🤹‍♂️", "🤹‍♀️", "🎭", "🚀", "🛸", "🛰", "🛎", "🧳", "⌛", "⏳", "⌚", "⏰", "⏱️",
    "⏲️", "🕰", "🌡", "🌤", "⛅", "🌥", "🌦", "🌧", "⛈️", "🌩",
    "🌨️", "❄️", "☃️", "⛄", "🌬", "💨", "🌪", "🌫", "🌈", "☂️",
    "☔", "⚡", "❤️", "🧡", "💛", "💚", "💙", "💜", "🖤", "🤍",
    "🤎", "💔", "❣️", "💕", "💞", "💓", "💗", "💖", "💘", "💝",
    "💟", "☮️", "✝️", "☪️", "🕉", "☸️", "✡️", "🔯", "🕎", "☯️",
    "☦️", "🛐", "⛎", "♈", "♉", "♊", "♋", "♌", "♍", "♎",
    "♏", "♐", "♑", "♒", "♓", "🆔", "⚛️", "🉑", "☢️", "☣️",
    "📴", "📳", "🈶", "🈚", "🈸", "🈺", "🈷️", "✴️", "🆚", "🉐",
    "💮", "🉑", "㊙️", "㊗️", "🈴", "🈵", "🈹", "🈲", "🅰️", "🅱",
    "🆎", "🆑", "🅾", "🆘", "❌", "⭕", "🛑", "⛔", "📛", "🚫",
    "💯", "💢", "♨️", "🚷", "🚯", "🚳", "🚱", "🔞", "📵", "🚭",
    "❗", "❕", "❓", "❔", "❣️", "💤", "💬", "🗯", "💭", "🕳",
    "👁", "🗨", "🗩", "🔈", "🔉", "🔊", "📢", "📣", "📯", "🔔",
    "🔕", "🎼", "🎵", "🎶", "🏧", "🚮", "🚰", "♿", "🚹", "🚺",
    "🚻", "🚼", "🚾", "🛂", "🛃", "🛄", "🛅", "🚫", "🚭", "🚯",
    "🚱", "🚷", "📵", "🔞", "🔙", "🔛", "🔝", "🔜", "🔚", "🔈",
    "🔉", "🔊", "🔇", "🔔", "🔕", "📣", "📢", "🕭", "🕯️", "💡",
    "🔦", "🏮", "📻", "📱", "📲", "☎️", "📞", "📟", "📠",
    "🔋", "🔌", "💻", "🖥", "🖨", "⌨️", "🖱", "🖲", "💽", "💾",
    "💿", "📀", "🧮", "🎥", "🎞", "📽", "🎬", "📺", "📷", "📸",
    "📹", "🎤", "🎧", "🎵", "🎶", "🎼", "🎙", "🎚", "🎛", "🎫",
    "🎟", "🎭", "🩰", "🎨", "🖌", "🖍️", "📝", "✏️", "🔍", "🔎",
    "🔏", "🔐", "🔒", "🔓", "🚃", "🚄", "🚅", "🚆", "🚇", "🚈",
    "🚉", "🚊", "🚝", "🚞", "🚋", "🚌", "🚍", "🚎", "🚐", "🚑",
    "🚒", "🚓", "🚔", "🚕", "🚖", "🚗", "🚘", "🚙", "🚚", "🚛",
    "🚜", "🏎", "🏍", "🛵", "🦽", "🦼", "🛴", "🚲", "🛹", "🛼",
    "🚏", "🛣", "🛤", "🛢", "⛽", "🚨", "🚥", "🚦", "🛑", "🚧",
    "⚓", "⛵", "🛶", "🚤", "🛳", "⛴️", "🚢", "✈️", "🛩", "🛫",
    "🛬", "🪂", "💺", "🚁", "🚟",  "🙌", "🤝", "🤞", "✌️", "🤘", "☝️", "✋", "🤚", "🖐", "🖖",
    "🤟", "✍️", "🙏", "💪", "🦾", "🖕", "🦵", "🦿", "🦶", "👂",
    "🦻", "👃", "🧠", "🦷", "🦴", "👀", "👁", "👅", "👄", "🔥",
    "💥", "💣", "🌟", "🎉", "🎊", "🎈", "👑", "🎩", "🧢", "🍾",
    "🎤", "🎬", "📽", "🎥", "🍿", "🎞", "🎦", "🏟", "🕹️", "🎮",
    "🎲", "♟️", "🧩", "🧸", "🎯", "🎳", "🎰", "🚀", "🛸", "🌍",
    "🌎", "🌏", "🌐", "🪐", "☄️", "🌠", "🌌", "🎇", "🎆", "🌃",
    "🌆", "🌅", "🌄", "🌇", "🌞", "🌝", "🌛", "🌜", "🌚", "🌙",
    "🌕", "🌖", "🌗", "🌘", "🌑", "🌒", "🌓", "🌔", "🌙", "🌚",
    "🌛", "🌜", "🌡", "🌤", "⛅", "🌥", "🌦", "🌧", "⛈️", "🌩",
    "🌨", "❄️", "☃️", "⛄", "🌬", "💨", "🌪", "🌫", "🌦", "🌈",
    "☔", "🍃", "🍂", "🍁", "🌾", "🌻", "🌼", "🌸", "💮", "🏵",
    "🌹", "🥀", "🌺", "🌷", "🌱", "🌴", "🌲", "🌳", "🌵", "🌿",
    "☘️", "🍀", "🍁", "🍂", "🍃", "🍄", "🌰", "🐚", "🍏", "🍎",
    "🍐", "🍊", "🍋", "🍌", "🍉", "🍇", "🍓", "🍈", "🍒", "🍑",
    "🥭", "🍍", "🥥", "🥝", "🍅", "🍆", "🥑", "🥦", "🥒", "🌶",
    "🌽", "🥕", "🍄", "🥔", "🍠", "🥐", "🍞", "🥖", "🥨", "🧀",
    "🥚", "🍳", "🥓", "🥩", "🍗", "🍖", "🦴", "🌭", "🍔", "🍟",
    "🍕", "🥪", "🥙", "🌮", "🌯", "🥗", "🥘", "🍝", "🍜", "🍲",
    "🍛", "🍣", "🍱", "🥟", "🍤", "🍢", "🍡", "🍦", "🍧", "🍨",
    "🍩", "🍪", "🎂", "🍰", "🧁", "🥧", "🍫", "🍬", "🍭", "🍮",
    "🍯", "🍼", "🥤", "🍺", "🍻", "🥂", "🍷", "🥃", "🍸", "🍹",
    "🥄", "🍴", "🧂", "🍽️", "🥢", "🧇", "🥞", "🥡", "🧆", "🥤",
    "🥢", "🍽️", "🥄", "🍴", "🥂", "🍸", "🍷", "🥃", "🍾", "🍻",
    "🍺", "🍯", "🍮", "🍭", "🍬", "🍫", "🎂", "🍰", "🥧", "🍩",
    "🍨", "🍧", "🍦", "🍡", "🍢", "🍤", "🥟", "🍱", "🍣", "🍛",
    "🍜", "🍝", "🥘", "🥗", "🌯", "🌮", "🥪", "🍕", "🍟", "🍔",
    "🌭", "🦴", "🍖", "🍗",

 };


            foreach (string emoji in emojis.Distinct())
            {
                Button emojiButton = new Button();
                emojiButton.Text = emoji;
                emojiButton.Font = new Font("Segoe UI Emoji", 16);
                emojiButton.Size = new Size(42, 42);
                emojiButton.Padding = new Padding(2, 0, 0, 1);
                emojiButton.Click += (sender, e) =>
                {
                    // Insert the emoji where the cursor was in the supMessage TextBox
                    supMessage.SelectedText = emoji;
                    emojiPanel.Hide(); // Hide the emoji panel
                };
                emojiPanel.Controls.Add(emojiButton);
            }

        }

        private Bitmap GenerateQRCode(string qrData)
        {
            QrCode qrCode = encoder.Encode(qrData);
            Graphics g = null;
            try
            {
                int height = qrCode.Matrix.Height;
                Bitmap qrCodeImage = new Bitmap((height * 2) + 9, (height * 2) + 9);
                g = Graphics.FromImage(qrCodeImage);
                renderer.Draw(g, qrCode.Matrix);
                return qrCodeImage;
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }
        }

        private async void PrintMessage(string message)
        {


            try
            {
                using (Bitmap qrCode = GenerateQRCode(message))
                {
                    if (qrCode != null)
                    {
                        Directory.CreateDirectory(@"root\" + txtToAddress.Text);
                        qrCode.Save(@"root\" + txtToAddress.Text + @"\MSGPrint.png", ImageFormat.Png);
                        this.Invoke((Action)(() =>
                        {
                            pictureBox1.ImageLocation = @"root\" + txtToAddress.Text + @"\MSGPrint.png";
                        }));
                    }
                }
            }
            catch { }
        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            isPrint = true;


            PROState toProfile = PROState.GetProfileByAddress(txtToAddress.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            var location = toProfile.Location;
            messagecache = supMessage.Text;
            // Create a StringBuilder to build the text
            var stringBuilder = new StringBuilder();

            if (location != null)
            {
                foreach (var kvp in location)
                {
                    stringBuilder.AppendLine($"{kvp.Key} : {kvp.Value}");
                }
            }
            this.Invoke((Action)(() =>
            {
                supMessage.ForeColor = Color.Black;
                supMessage.BackColor = Color.White;
            }));

            this.Invoke((Action)(() =>
            {
                supMessage.Text = stringBuilder.ToString();

            }));

            Application.DoEvents();

            this.Invoke((Action)(() =>
            {
                pictureBox1.Visible = true;
                btnRefresh.PerformClick();
            }));


        }

        private void PrintMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            System.Drawing.Bitmap bitmap = new Bitmap(this.Width - 22, this.Height - 44);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(this.PointToScreen(new Point(0, 0)), new Point(0, 0), this.Size);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            PrintImage(bitmap);
        }

        private void HideMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            pictureBox1.ImageLocation = null;
            supMessage.ForeColor = Color.White;
            supMessage.BackColor = Color.Black;
            supMessage.Text = messagecache;
        }
        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(pictureBox1.ImageLocation))
            {
                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        // Download the image from the specified URL
                        byte[] imageBytes = webClient.DownloadData(pictureBox1.ImageLocation);

                        // Create an Image object from the downloaded bytes
                        using (MemoryStream stream = new MemoryStream(imageBytes))
                        {
                            using (Image image = Image.FromStream(stream))
                            {
                                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                                {
                                    saveFileDialog.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";
                                    saveFileDialog.FilterIndex = 1;
                                    saveFileDialog.RestoreDirectory = true;

                                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                                    {
                                        // Save the Image to the chosen file path
                                        image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No image location specified.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            pictureBox1.ImageLocation = null;
            supMessage.ForeColor = Color.White;
            supMessage.BackColor = Color.Black;
            supMessage.Text = messagecache;
        }

        private void btnInquiry_Click(object sender, EventArgs e)
        {
            INQMint mintForm = new INQMint(this); // Pass the reference to the current form as the parent form
            mintForm.ShowDialog();
        }

        private void txtINQJson_TextChanged(object sender, EventArgs e)
        {
            if (txtINQJson.Text != "")
            {
                try
                {
                    // Attempt to remove the existing btnINQ control
                    flowAttachments.Controls.RemoveByKey("btnINQ");
                }
                catch (ArgumentException)
                {
                    // Control with the specified name doesn't exist, so no need to handle the exception
                }

                // Create and add the new btnINQ control
                Button btnINQ = new System.Windows.Forms.Button();
                btnINQ.Font = new System.Drawing.Font("Segoe UI Emoji", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                btnINQ.ForeColor = System.Drawing.Color.Black;
                btnINQ.Name = "btnINQ";
                btnINQ.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
                btnINQ.RightToLeft = System.Windows.Forms.RightToLeft.No;
                btnINQ.Size = new System.Drawing.Size(50, 46);
                btnINQ.Text = "⁉️";
                btnINQ.UseVisualStyleBackColor = true;
                btnINQ.MouseUp += new MouseEventHandler(BtnINQ_MouseUp);
                flowAttachments.Controls.Add(btnINQ);
            }
        }

        private void BtnINQ_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button button = (Button)sender;
                flowAttachments.Controls.Remove(button);
                txtINQJson.Text = "";
                txtINQAddress.Text = "";
            }
        }


        //GPT3.5 INSPIRED CODE
        string ipfsHash = "";

        // Handle paste event
        private void HandlePaste()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])data.GetData(DataFormats.FileDrop);
                foreach (string filePath in files)
                {
                    ProcessFileAsync(filePath);
                }
            }
            else if (data.GetDataPresent(DataFormats.Bitmap))
            {
                Image image = (Image)data.GetData(DataFormats.Bitmap);
                ProcessImage(image);
            }
        }

        // Process pasted file
        private async Task ProcessFileAsync(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string proccessingFile = filePath;
            string processingid = Guid.NewGuid().ToString();
            List<string> imageExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", "" };

            if (btnEncryptionStatus.Text == "PRIVATE 🤐")
            {
                byte[] rootbytes = Root.GetRootBytesByFile(new string[] { filePath });
                PROState toProfile = PROState.GetProfileByAddress(txtToAddress.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                rootbytes = Root.EncryptRootBytes(mainnetLogin, mainnetPassword, mainnetURL, txtToAddress.Text, rootbytes, toProfile.PKX, toProfile.PKY, true);
                string proccessingDirectory = @"root\" + processingid;
                Directory.CreateDirectory(proccessingDirectory);
                proccessingFile = proccessingDirectory + @"\SEC";
                File.WriteAllBytes(proccessingFile, rootbytes);
            }

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
                else
                {
                    pictureBox.ImageLocation = @"includes\HugPuddle.jpg";
                    pictureBox.MouseClick += PictureBox_MouseClick;
                }

                this.Invoke((MethodInvoker)delegate
                {
                    flowAttachments.Controls.Add(pictureBox);
                });

                return hash;
            });

            ipfsHash = await addTask;

            try
            {
                if (File.Exists("IPFS_PINNING_ENABLED"))
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

            try { Directory.Delete(@"root\" + processingid, true); } catch { }
        }

        // Process pasted image
        private void ProcessImage(Image image)
        {
            // Save the image to a temporary file
            string tempFilePath = Path.GetTempFileName() + ".bmp";
            image.Save(tempFilePath);

            // Process the temporary file
            ProcessFileAsync(tempFilePath);
        }

        // Call this method when paste event occurs
        private void OnPaste(object sender, EventArgs e)
        {
            HandlePaste();
        }

        private void supMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                // Handle paste action
                HandlePaste();
            }
        }

        private void btnFromSelector_Click(object sender, EventArgs e)
        {
            List<PROState> profiles = PROState.GetLocalProfiles(mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, true);

            using (var dialog = new Form())
            {
                dialog.Text = "Select a local profile";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.AutoSize = true;
                dialog.ControlBox = false;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new Size(240, 90);


                var nameComboBox = new ComboBox();
                nameComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                nameComboBox.Font = new Font(nameComboBox.Font.FontFamily, 12f);
                nameComboBox.Width = 200;
                foreach (PROState pro in profiles)
                {
                    if (!nameComboBox.Items.Contains(pro.URN))
                    {
                        nameComboBox.Items.Add(pro.URN);
                    }
                }
                nameComboBox.SelectedIndex = 0;

                var okButton = new Button();
                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;
                okButton.Anchor = AnchorStyles.Right;

                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Anchor = AnchorStyles.Right;

                dialog.Controls.Add(nameComboBox);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);


                nameComboBox.Location = new Point(20, 20);
                okButton.Location = new Point(40, 60);
                cancelButton.Location = new Point(120, 60);

                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PROState selectedProState = PROState.GetProfileByURN(nameComboBox.SelectedItem.ToString(), mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                    txtFromAddress.Text = selectedProState.Creators[0];
                    Dictionary<string, string> friendDict = new Dictionary<string, string>();
                    string filePath = "";

                    if (mainnetVersionByte == "111")
                    { filePath = @"root\MyFriendList.Json"; }
                    else { filePath = @"root\MyProdFriendList.Json"; }
                    try
                    {
                        string json = File.ReadAllText(filePath);
                        friendDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    }
                    catch { }

                    try
                    {
                        if (friendDict.TryGetValue(selectedProState.Creators[0], out string fileLoc))
                        {
                            fromImage.ImageLocation = fileLoc;
                        }
                        else { fromImage.ImageLocation = @"includes\anon.png"; }
                    }
                    catch { fromImage.ImageLocation = @"includes\anon.png"; }
                }

            }


        }

        private void btnToSelector_Click(object sender, EventArgs e)
        {
            string filePath = "";
            Dictionary<string, string> friendDict = new Dictionary<string, string>();
            if (mainnetVersionByte == "111")
            { filePath = @"root\MyFriendList.Json"; }
            else { filePath = @"root\MyProdFriendList.Json"; }



            using (var dialog = new Form())
            {
                dialog.Text = "Select a local profile";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.AutoSize = true;
                dialog.ControlBox = false;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new Size(240, 90);


                var nameComboBox = new ComboBox();
                nameComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                nameComboBox.Font = new Font(nameComboBox.Font.FontFamily, 12f);
                nameComboBox.Width = 200;
                try
                {

                    string json = File.ReadAllText(filePath);

                    // Deserialize the JSON into a Dictionary<string, string> object
                    friendDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                    // Create PictureBox controls for each friend in the dictionary
                    foreach (var friend in friendDict)
                    {
                        PROState Friend = PROState.GetProfileByAddress(friend.Key, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                        if (Friend.URN != null)
                        {
                            nameComboBox.Items.Add(Friend.URN);
                        }

                    }
                }
                catch { }

                nameComboBox.SelectedIndex = 0;

                var okButton = new Button();
                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;
                okButton.Anchor = AnchorStyles.Right;

                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Anchor = AnchorStyles.Right;

                dialog.Controls.Add(nameComboBox);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);


                nameComboBox.Location = new Point(20, 20);
                okButton.Location = new Point(40, 60);
                cancelButton.Location = new Point(120, 60);

                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PROState selectedProState = PROState.GetProfileByURN(nameComboBox.SelectedItem.ToString(), mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                    txtToAddress.Text = selectedProState.Creators[0];
                    try
                    {
                        if (friendDict.TryGetValue(selectedProState.Creators[0], out string fileLoc))
                        {
                            toImage.ImageLocation = fileLoc;
                        }
                        else { toImage.ImageLocation = @"includes\anon.png"; }
                        toImage.ImageLocation = fileLoc;
                    }
                    catch { toImage.ImageLocation = @"includes\anon.png"; }
                }

            }


        }

        private void txtAttach_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                // Handle paste action
                HandlePaste();
            }
        }
    }
}
