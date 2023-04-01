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

namespace SUP
{
    public partial class DiscoBall : Form
    {
        private string _fromaddress;
        private string _toaddress;
        private string _fromimageurl;
        private string _toimageurl;
        public DiscoBall(string fromaddress = "", string fromimageurl = "", string toaddress = "", string toimageurl = "", bool isprivate = false)
        {
            InitializeComponent();
            _fromaddress = fromaddress;
            _toaddress = toaddress;
            _fromimageurl = fromimageurl;
            _toimageurl = toimageurl;

            if (isprivate ) { btnEncryptionStatus.Text = "🤐"; }

        }


        private void DiscoBall_Load(object sender, EventArgs e)
        {
            fromImage.ImageLocation = _fromimageurl;
            toImage.ImageLocation = _toimageurl;
            txtFromAddress.Text = _fromaddress;
            txtToAddress.Text = _toaddress;            
        }

        private async void btnAttach_Click(object sender, EventArgs e)
        {
            if (flowAttachments.Controls.Count < 6)
            {
                string imgurn = "";
                List<string> allowedExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", "" };


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
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog1.FileName;
                        string fileName = openFileDialog1.SafeFileName;


                        string extensions = Path.GetExtension(filePath).ToLower();
                        if (allowedExtensions.Contains(extensions))
                        {
                            string proccessingFile = filePath;
                            string processingid = Guid.NewGuid().ToString();

                            if (btnEncryptionStatus.Text == "🤐")
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
                                if (btnEncryptionStatus.Text == "🤐")
                                { pictureBox.Tag = "IPFS:" + hash + @"\SEC"; }
                                else
                                {
                                    // Set the PictureBox properties
                                    pictureBox.Tag = "IPFS:" + hash + @"\" + fileName;
                                }
                                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBox.Width = 50;
                                pictureBox.Height = 50;
                                pictureBox.ImageLocation = filePath;
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


                }


                string extension = Path.GetExtension(imgurn).ToLower();
                if (allowedExtensions.Contains(extension))
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
                                try
                                {

                                }
                                catch { }


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
                        // Add the PictureBox to the FlowLayoutPanel
                        flowAttachments.Controls.Add(pictureBox);

                    }


                }



            }

        }


        private void discoButton_Click(object sender, EventArgs e)
        {

            string transMessage = supMessage.Text;
            List<string> encodedList = new List<string>();
            foreach (PictureBox pb in flowAttachments.Controls)
            {
                transMessage = transMessage + "<<" + pb.Tag.ToString() + ">>";
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
  

            if (btnEncryptionStatus.Text == "🤐")
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
                    supMessage.Text = supMessage.Text + "\r\n" + results;
                }
                catch (Exception ex) { lblObjectStatus.Text = ex.Message; }


            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
