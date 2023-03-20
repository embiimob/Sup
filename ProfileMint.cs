using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LevelDB;
using System.Diagnostics;
using System.Windows.Forms;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using NBitcoin;
using NBitcoin.RPC;
using SUP.P2FK;
using System.Collections.Generic;
using System.Threading;
using System.Web.NBitcoin;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using BitcoinNET.RPCClient;

namespace SUP
{
    public partial class ProfileMint : Form
    {
        private QrEncoder encoder = new QrEncoder();
        private GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(2, QuietZoneModules.Two));
        private bool ismint = false;



        public ProfileMint()
        {
            InitializeComponent();
        }


        private void UpdateRemainingChars()
        {
            if (txtURN.Text != "" && txtObjectAddress.Text != "")
            {
                if ((btnFirstName.BackColor == Color.Blue || btnObjectURN.BackColor == Color.Blue || btnMiddleName.BackColor == Color.Blue || btnSuffix.BackColor == Color.Blue || btnLocation.BackColor == Color.Blue || btnURL.BackColor == Color.Blue || btnObjectImage.BackColor == Color.Blue || btnLastName.BackColor == Color.Blue || btnBio.BackColor == Color.Blue || btnObjectKeywords.BackColor == Color.Blue)) { btnMint.Enabled = true; }
            }


            int maxsize = 888;

            maxsize = maxsize - txtBio.Text.Length - txtIMG.Text.Length - txtFirstName.Text.Length - txtMiddleName.Text.Length - txtURN.Text.Length - txtLastName.Text.Length - txtSuffix.Text.Length;
            maxsize = maxsize - 40; ///estimated json chars required.

            foreach (System.Windows.Forms.Control control in flowURL.Controls)
            {
                maxsize = maxsize - (control.Text.Length + 5);
            }

            maxsize = maxsize - (flowKeywords.Controls.Count * 20) + 5;

            foreach (System.Windows.Forms.Control control in flowLocation.Controls)
            {

                maxsize = maxsize - (control.Text.Length + 5);


            }


            lblRemainingChars.Text = maxsize.ToString();


            if (maxsize < 0)
            {

                btnMint.Enabled = false;
            }


            PRO PROJson = new PRO();
            if (btnFirstName.BackColor == Color.Blue) { PROJson.fnm = txtFirstName.Text; }
            if (btnObjectImage.BackColor == Color.Blue) { PROJson.img = txtIMG.Text; }
            if (btnObjectURN.BackColor == Color.Blue) { PROJson.urn = txtURN.Text; }
            if (btnMiddleName.BackColor == Color.Blue) { PROJson.mnm = txtMiddleName.Text; }
            if (btnBio.BackColor == Color.Blue) { PROJson.bio = txtBio.Text; }
            if (btnLastName.BackColor == Color.Blue) { PROJson.lnm = txtLastName.Text; }
            if (btnSuffix.BackColor == Color.Blue) { PROJson.sfx = txtSuffix.Text; }


            Dictionary<string, string> mintURL = new Dictionary<string, string>();
            foreach (Button attributeControl in flowURL.Controls)
            {
                int colonIndex = attributeControl.Text.IndexOf(':');
                if (colonIndex >= 0 && colonIndex < attributeControl.Text.Length - 1)
                {
                    string key = attributeControl.Text.Substring(0, colonIndex);
                    string value = attributeControl.Text.Substring(colonIndex + 1);
                    mintURL.Add(key, value);
                }
            }
            PROJson.url = mintURL;

            List<int> mintCreators = new List<int>();
            mintCreators.Add(0);
            PROJson.cre = mintCreators;


            if (btnLocation.BackColor == Color.Blue)
            {
                Dictionary<string, string> mintLocation = new Dictionary<string, string>();
                foreach (Button ownerControl in flowLocation.Controls)
                {
                    string[] parts = ownerControl.Text.Split(':');
                    try { mintLocation.Add(parts[0], parts[1]); } catch { }
                }
                PROJson.loc = mintLocation;
            }

            try
            {
                List<string> pubkeys = Root.GetPublicKeysByAddress(txtObjectAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");
                PROJson.pkx = pubkeys[0];
                PROJson.pky = pubkeys[1];
            }
            catch { }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            // Serialize the modified JObject back into a JSON string
            var objectSerialized = JsonConvert.SerializeObject(PROJson, Formatting.None, settings);

            txtOBJJSON.Text = objectSerialized.Replace(",\"url\":{}", "").Replace(",\"loc\":{}", "").Replace(",\"urn\":\"\"", "").Replace(",\"fnm\":\"\"", "").Replace(",\"mnm\":\"\"", "").Replace(",\"lnm\":\"\"", "").Replace(",\"sfx\":\"\"", "").Replace(",\"bio\":\"\"", "").Replace(",\"img\":\"\"", "");


            txtOBJP2FK.Text = "PRO" + ":" + txtOBJJSON.Text.Length + ":" + txtOBJJSON.Text;

            if (btnMint.Enabled)
            {
                NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                RPCClient rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:18332"), Network.Main);
                System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                byte[] hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(txtOBJP2FK.Text));
                string signatureAddress;

                signatureAddress = txtObjectAddress.Text;
                string signature = "";
                try { signature = rpcClient.SendCommand("signmessage", signatureAddress, BitConverter.ToString(hashValue).Replace("-", String.Empty)).ResultString; }
                catch (Exception ex)
                {
                    lblObjectStatus.Text = ex.Message;
                    return;
                }

                txtOBJP2FK.Text = "SIG" + ":" + "88" + ">" + signature + txtOBJP2FK.Text;

                List<string> encodedList = new List<string>();
                for (int i = 0; i < txtOBJP2FK.Text.Length; i += 20)
                {
                    string chunk = txtOBJP2FK.Text.Substring(i, Math.Min(20, txtOBJP2FK.Text.Length - i));
                    if (chunk.Any())
                    {
                        encodedList.Add(Root.GetPublicAddressByKeyword(chunk));
                    }
                }

                //add URN registration
                encodedList.Add(Root.GetPublicAddressByKeyword(txtURN.Text));

                foreach (Button keywordbtn in flowKeywords.Controls)
                {
                    encodedList.Add(Root.GetPublicAddressByKeyword(keywordbtn.Text));
                }

                encodedList.Add(signatureAddress);
                txtAddressListJSON.Text = JsonConvert.SerializeObject(encodedList.Distinct());

                lblCost.Text = "cost: " + (0.00000546 * encodedList.Count).ToString("0.00000000") + "  + miner fee";

                if (ismint)
                {
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
                        lblTransactionID.Text = results;
                    }
                    catch (Exception ex) { lblObjectStatus.Text = ex.Message; }


                    ismint = false;
                }


            }



        }







        Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");

        private void txtIMG_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
        }

        private void txtURN_TextChanged(object sender, EventArgs e)
        {
            

            btnObjectURN.BackColor = Color.Blue;
            btnObjectURN.ForeColor = Color.Yellow;

            if (txtURN.Text != "" && txtObjectAddress.Text != "")
            {

                btnURL.Enabled = true;
                btnBio.Enabled = true;
                btnObjectImage.Enabled = true;
                btnObjectKeywords.Enabled = true;
                btnFirstName.Enabled = true;
                btnMiddleName.Enabled = true;
                btnLastName.Enabled = true;
                btnSuffix.Enabled = true;
                btnLocation.Enabled = true;
                txtBio.Enabled = true;
                txtIMG.Enabled = true;
                txtFirstName.Enabled = true;
                txtMiddleName.Enabled = true;
                txtLastName.Enabled = true;
                txtSuffix.Enabled = true;
              
            }
            else
            {
                if (txtURN.Text == "")
                {
                    btnObjectURN.BackColor = Color.White;
                    btnObjectURN.ForeColor = Color.Black;
                }

                btnURL.Enabled = false;
                btnBio.Enabled = false;
                btnObjectImage.Enabled = false;
                btnObjectKeywords.Enabled = false;
                btnFirstName.Enabled = false;
                btnMiddleName.Enabled = false;
                btnLastName.Enabled = false;
                btnSuffix.Enabled = false;
                btnLocation.Enabled = false;
                txtBio.Enabled = false;
                txtIMG.Enabled = false;
                txtFirstName.Enabled = false;
                txtMiddleName.Enabled = false;
                txtLastName.Enabled = false;
                txtSuffix.Enabled = false;


            }
            UpdateRemainingChars();
        }

        private void txtURI_TextChanged(object sender, EventArgs e)
        {
           

            if (txtMiddleName.Text != "")
            {
                btnMiddleName.BackColor = Color.Blue;
                btnMiddleName.ForeColor = Color.Yellow;
            }
            else
            {
                btnMiddleName.BackColor = Color.White;
                btnMiddleName.ForeColor = Color.Black;
            }
            UpdateRemainingChars();
        }

        private void flowAttribute_ControlAdded(object sender, ControlEventArgs e)
        {
           
            btnURL.BackColor = Color.Blue;
            btnURL.ForeColor = Color.Yellow;
            UpdateRemainingChars();
        }

        private void flowKeyword_ControlAdded(object sender, ControlEventArgs e)
        {
            
            btnObjectKeywords.BackColor = Color.Blue;
            btnObjectKeywords.ForeColor = Color.Yellow;
            UpdateRemainingChars();
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            if (txtFirstName.Text != "")
            {
                btnFirstName.BackColor = Color.Blue;
                btnFirstName.ForeColor = Color.Yellow;
            }
            else
            {
                btnFirstName.BackColor = Color.White;
                btnFirstName.ForeColor = Color.Black;
            }
            UpdateRemainingChars();
        }

        private void ObjectMint_Load(object sender, EventArgs e)
        {
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            
            if (txtBio.Text != "")
            {
                btnBio.BackColor = Color.Blue;
                btnBio.ForeColor = Color.Yellow;

            }
            else
            {
                btnBio.BackColor = Color.White;
                btnBio.ForeColor = Color.Black;
            }

            UpdateRemainingChars();
        }


        private void flowOwners_ControlAdded(object sender, ControlEventArgs e)
        {
           
            btnLocation.BackColor = Color.Blue;
            btnLocation.ForeColor = Color.Yellow;
            UpdateRemainingChars();
        }

        private void txtObjectAddress_TextChanged(object sender, EventArgs e)
        {
            btnObjectAddress.BackColor = Color.Blue;
            btnObjectAddress.ForeColor = Color.Yellow;

            if (txtURN.Text != "" && txtObjectAddress.Text != "")
            {

                btnURL.Enabled = true;
                btnBio.Enabled = true;
                btnObjectImage.Enabled = true;
                btnObjectKeywords.Enabled = true;
                btnFirstName.Enabled = true;
                btnMiddleName.Enabled = true;
                btnLastName.Enabled = true;
                btnSuffix.Enabled = true;
                btnLocation.Enabled = true;
                txtBio.Enabled = true;
                txtIMG.Enabled = true;
                txtFirstName.Enabled = true;
                txtMiddleName.Enabled = true;
                txtLastName.Enabled = true;
                txtSuffix.Enabled = true;

            }
            else
            {
                if (txtObjectAddress.Text == "")
                {
                    btnObjectAddress.BackColor = Color.White;
                    btnObjectAddress.ForeColor = Color.Black;
                }

                btnURL.Enabled = false;
                btnBio.Enabled = false;
                btnObjectImage.Enabled = false;
                btnObjectKeywords.Enabled = false;
                btnFirstName.Enabled = false;
                btnMiddleName.Enabled = false;
                btnLastName.Enabled = false;
                btnSuffix.Enabled = false;
                btnLocation.Enabled = false;
                txtBio.Enabled = false;
                txtIMG.Enabled = false;
                txtFirstName.Enabled = false;
                txtMiddleName.Enabled = false;
                txtLastName.Enabled = false;
                txtSuffix.Enabled = false;


            }
            UpdateRemainingChars();

        }

        private void btnObjectAddress_Click(object sender, EventArgs e)
        {

            if (txtObjectAddress.Text != "")
            {
                lblObjectStatus.Text = "";
                LoadFormByAddress(txtObjectAddress.Text);

            }
            else
            {
                NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                RPCClient rpcClient = new RPCClient(credentials, new Uri("http://127.0.0.1:18332"), Network.Main);
                string newAddress = "";
                try { newAddress = rpcClient.SendCommand("getnewaddress", txtFirstName.Text + "!" + DateTime.UtcNow.ToString("yyyyMMddHHmmss")).ResultString; } catch { }
                txtObjectAddress.Text = newAddress;
                lblObjectStatus.Text = "";
                lblCost.Text = "";
                UpdateRemainingChars();
            }

        }


        private void btnObjectName_Click(object sender, EventArgs e)
        {
            if (btnFirstName.BackColor == Color.Blue) { btnFirstName.BackColor = Color.White; btnFirstName.ForeColor = Color.Black; }
            else
            {
                if (txtFirstName.Text != "") { btnFirstName.BackColor = Color.Blue; btnFirstName.ForeColor = Color.Yellow; } else { btnFirstName.BackColor = Color.White; btnFirstName.ForeColor = Color.Black; }
            }
            UpdateRemainingChars();
        }


        private void btnObjectDescription_Click(object sender, EventArgs e)
        {

            if (btnBio.BackColor == Color.Blue) { btnBio.BackColor = Color.White; btnBio.ForeColor = Color.Black; }
            else
            {
                btnBio.BackColor = Color.Blue; btnBio.ForeColor = Color.Yellow;
            }
            UpdateRemainingChars();
        }

        private async void btnObjectImage_Click(object sender, EventArgs e)
        {
            if (btnObjectImage.BackColor == Color.Blue) { btnObjectImage.BackColor = Color.White; btnObjectImage.ForeColor = Color.Black; }
            else
            {


                string imgurn = "";
                lblIMGBlockDate.Text = "[ unable to verify ]";

                if (txtIMG.Text != "")
                {
                    imgurn = txtIMG.Text;

                    if (!txtIMG.Text.ToLower().StartsWith("http"))
                    {
                        imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + txtIMG.Text.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                        if (txtIMG.Text.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                    }
                }
                else
                {

                    // Open file dialog and get file path and name
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog1.FileName;
                        string fileName = openFileDialog1.SafeFileName;

                        // Add file to IPFS
                        Task<string> addTask = Task.Run(() =>
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = @"ipfs\ipfs.exe";
                            process.StartInfo.Arguments = "add \"" + filePath + "\"";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.Start();
                            string output = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();
                            string hash = output.Split(' ')[1];
                            return "IPFS:" + hash;
                        });
                        string ipfsHash = await addTask;


                        // Update text box with IPFS hash
                        txtIMG.Text = ipfsHash + @"\" + fileName;
                    }


                }


                List<string> allowedExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", "" };
                string extension = Path.GetExtension(imgurn).ToLower();
                if (allowedExtensions.Contains(extension))
                {



                    try
                    {
                        Root root = new Root();
                        Match urimatch = regexTransactionId.Match(txtIMG.Text);
                        string transactionid = urimatch.Value;
                        switch (txtIMG.Text.Substring(0, 4).ToUpper())
                        {
                            case "MZC:":

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                try
                                {
                                    lblIMGBlockDate.Text = "mazacoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    btnObjectImage.BackColor = Color.Blue;
                                    btnObjectImage.ForeColor = Color.Yellow;
                                }
                                catch { }
                                break;
                            case "BTC:":

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                try
                                {
                                    lblIMGBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    btnObjectImage.BackColor = Color.Blue;
                                    btnObjectImage.ForeColor = Color.Yellow;
                                }
                                catch { }
                                break;
                            case "LTC:":

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                try
                                {
                                    lblIMGBlockDate.Text = "litecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    btnObjectImage.BackColor = Color.Blue;
                                    btnObjectImage.ForeColor = Color.Yellow;
                                }
                                catch { }
                                break;
                            case "DOG:":

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                try
                                {
                                    lblIMGBlockDate.Text = "dogecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    btnObjectImage.BackColor = Color.Blue;
                                    btnObjectImage.ForeColor = Color.Yellow;
                                }
                                catch { }

                                break;
                            case "IPFS":
                                if (txtIMG.Text.Length == 51) { imgurn += @"\artifact"; }
                                if (!System.IO.Directory.Exists(@"ipfs/" + txtIMG.Text.Substring(5, 46) + "-build") && !System.IO.File.Exists(@"ipfs/" + txtIMG.Text.Substring(5, 46)))
                                {


                                    Task ipfsTask = Task.Run(() =>
                                    {
                                        Directory.CreateDirectory(@"ipfs/" + txtIMG.Text.Substring(5, 46) + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + txtIMG.Text.Substring(5, 46) + @" -o ipfs\" + txtIMG.Text.Substring(5, 46);
                                        process2.Start();
                                        process2.WaitForExit();

                                        if (System.IO.File.Exists("ipfs/" + txtIMG.Text.Substring(5, 46)))
                                        {
                                            try { System.IO.File.Move("ipfs/" + txtIMG.Text.Substring(5, 46), "ipfs/" + txtIMG.Text.Substring(5, 46) + "_tmp"); }
                                            catch
                                            {

                                                System.IO.File.Delete("ipfs/" + txtIMG.Text.Substring(5, 46) + "_tmp");
                                                System.IO.File.Move("ipfs/" + txtIMG.Text.Substring(5, 46), "ipfs/" + txtIMG.Text.Substring(5, 46) + "_tmp");

                                            }

                                            string fileName = txtIMG.Text.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "")
                                            {
                                                fileName = "artifact";
                                            }
                                            else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                            Directory.CreateDirectory(@"ipfs/" + txtIMG.Text.Substring(5, 46));
                                            try { System.IO.File.Move("ipfs/" + txtIMG.Text.Substring(5, 46) + "_tmp", imgurn); } catch { }
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
                                                        Arguments = "pin add " + txtIMG.Text.Substring(5, 46),
                                                        UseShellExecute = false,
                                                        CreateNoWindow = true
                                                    }
                                                };
                                                process3.Start();
                                            }
                                        }

                                        try { Directory.Delete(@"ipfs/" + txtIMG.Text.Substring(5, 46)); } catch { }
                                        try
                                        {
                                            Directory.Delete(@"ipfs/" + txtIMG.Text.Substring(5, 46) + "-build");
                                        }
                                        catch { }



                                    });
                                    lblIMGBlockDate.Text = "ipfs verified: " + System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    btnObjectImage.BackColor = Color.Blue;
                                    btnObjectImage.ForeColor = Color.Yellow;

                                }
                                else
                                {
                                    lblIMGBlockDate.Text = "ipfs verified: " + System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    btnObjectImage.BackColor = Color.Blue;
                                    btnObjectImage.ForeColor = Color.Yellow;
                                }
                                break;
                            default:

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                try
                                {
                                    lblIMGBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    btnObjectImage.BackColor = Color.Blue;
                                    btnObjectImage.ForeColor = Color.Yellow;
                                }
                                catch { }


                                break;
                        }

                        if (lblIMGBlockDate.Text.Contains("Mon, 01 Jan 0001 12:00:00"))
                        {
                            btnObjectImage.BackColor = Color.Blue;
                            btnObjectImage.ForeColor = Color.Black;
                            lblIMGBlockDate.Text = "[ unable to verify ]";
                        }


                    }
                    catch { }
                    if (File.Exists(imgurn) || imgurn.ToUpper().StartsWith("HTTP"))
                    {

                        pictureBox2.ImageLocation = imgurn;
                    }
                    else
                    {
                        Random rnd = new Random();
                        string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                        if (gifFiles.Length > 0)
                        {
                            int randomIndex = rnd.Next(gifFiles.Length);
                            string randomGifFile = gifFiles[randomIndex];

                            pictureBox2.ImageLocation = randomGifFile;

                        }
                        else
                        {
                            try
                            {
                                pictureBox2.ImageLocation = @"includes\HugPuddle.jpg";
                            }
                            catch { }
                        }


                    }


                }
                else
                {
                    lblIMGBlockDate.Text = "[ unsported image type ]";
                    btnObjectImage.BackColor = Color.White;
                    btnObjectImage.ForeColor = Color.Black;
                }

            }
            UpdateRemainingChars();
        }

        private async void btnObjectURN_Click(object sender, EventArgs e)
        {
            if (btnObjectURN.BackColor == Color.Blue) { btnObjectURN.BackColor = Color.White; btnObjectURN.ForeColor = Color.Black; }
            else
            {
                btnObjectURN.BackColor = Color.Blue; btnObjectURN.ForeColor = Color.Yellow;
            }
            UpdateRemainingChars();

        }

        private void btnObjectURI_Click(object sender, EventArgs e)
        {
            if (btnMiddleName.BackColor == Color.Blue) { btnMiddleName.BackColor = Color.White; btnMiddleName.ForeColor = Color.Black; }
            else
            {
                btnMiddleName.BackColor = Color.Blue; btnMiddleName.ForeColor = Color.Yellow;
            }
            UpdateRemainingChars();
        }
        //GPT3
        private void btnObjectAttributes_Click(object sender, EventArgs e)
        {


            using (var dialog = new Form())
            {
                dialog.Text = String.Empty;
                dialog.AutoSize = true;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ControlBox = false;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new Size(400, 80);

                var tableLayout = new TableLayoutPanel();
                tableLayout.ColumnCount = 2;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

                var keyLabel = new Label();
                keyLabel.Text = "Label";
                keyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var valueLabel = new Label();
                valueLabel.Text = "URL";
                valueLabel.TextAlign = ContentAlignment.MiddleCenter;

                var keyTextBox = new TextBox();
                keyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                keyTextBox.Multiline = true;
                keyTextBox.Size = new Size(170, 70);

                var valueTextBox = new TextBox();
                valueTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                valueTextBox.Size = new Size(170, 70);
                valueTextBox.Multiline = true;
                tableLayout.Controls.Add(keyLabel, 0, 0);
                tableLayout.Controls.Add(valueLabel, 1, 0);
                tableLayout.Controls.Add(keyTextBox, 0, 1);
                tableLayout.Controls.Add(valueTextBox, 1, 1);

                var addButton = new Button();
                addButton.Text = "Add";
                addButton.DialogResult = DialogResult.OK;
                addButton.Anchor = AnchorStyles.Right;

                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Anchor = AnchorStyles.Right;

                dialog.Controls.Add(tableLayout);
                dialog.Controls.Add(addButton);
                dialog.Controls.Add(cancelButton);

                tableLayout.Dock = DockStyle.Top;
                addButton.Dock = DockStyle.Right;
                cancelButton.Dock = DockStyle.Right;

                dialog.AcceptButton = addButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var key = keyTextBox.Text;
                    var value = valueTextBox.Text;

                    var button = new Button();
                    button.Text = $"{key}:{value}";
                    button.AutoSize = true;

                    button.MouseClick += (s, ev) =>
                    {
                        // only proceed if left mouse button is clicked
                        if (ev.Button == MouseButtons.Left)
                        {
                            // disable the button to prevent double-clicks
                            button.Enabled = false;

                            // remove the button from the flow layout panel and dispose of it
                            flowURL.Controls.Remove(button);
                            button.Dispose();
                        }
                    };

                    // add event handler for right click
                    button.MouseDown += (s, ev) =>
                    {
                        // only proceed if right mouse button is clicked
                        if (ev.Button == MouseButtons.Right)
                        {
                            // create the object browser form and show it
                            string labelText = button.Text;
                            ObjectBrowser form = new ObjectBrowser(value);
                            form.Show();
                        }
                    };

                    flowURL.Controls.Add(button);

                }
            }
            UpdateRemainingChars();
        }

        //GPT3
        private void btnObjectKeywords_Click(object sender, EventArgs e)
        {

            using (var dialog = new Form())
            {
                dialog.Text = String.Empty;
                dialog.AutoSize = true;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ControlBox = false;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new Size(400, 80);

                var tableLayout = new TableLayoutPanel();
                tableLayout.ColumnCount = 1;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                var keyLabel = new Label();
                keyLabel.Text = "Keyword";
                keyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var keyTextBox = new TextBox();
                keyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                keyTextBox.Multiline = true;
                keyTextBox.Size = new Size(340, 70);
                tableLayout.Controls.Add(keyLabel, 0, 0);
                tableLayout.Controls.Add(keyTextBox, 0, 1);

                var addButton = new Button();
                addButton.Text = "Add";
                addButton.DialogResult = DialogResult.OK;
                addButton.Anchor = AnchorStyles.Right;

                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Anchor = AnchorStyles.Right;

                dialog.Controls.Add(tableLayout);
                dialog.Controls.Add(addButton);
                dialog.Controls.Add(cancelButton);

                tableLayout.Dock = DockStyle.Top;
                addButton.Dock = DockStyle.Right;
                cancelButton.Dock = DockStyle.Right;

                dialog.AcceptButton = addButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var key = keyTextBox.Text;

                    var button = new Button();
                    button.Text = $"{key}";
                    button.AutoSize = true;

                    // add event handler for left click
                    button.MouseClick += (s, ev) =>
                    {
                        // only proceed if left mouse button is clicked
                        if (ev.Button == MouseButtons.Left)
                        {
                            // disable the button to prevent double-clicks
                            button.Enabled = false;

                            // remove the button from the flow layout panel and dispose of it
                            flowKeywords.Controls.Remove(button);
                            button.Dispose();
                        }
                    };

                    // add event handler for right click
                    button.MouseDown += (s, ev) =>
                    {
                        // only proceed if right mouse button is clicked
                        if (ev.Button == MouseButtons.Right)
                        {
                            // create the object browser form and show it
                            string labelText = button.Text;
                            ObjectBrowser form = new ObjectBrowser("#" + labelText);
                            form.Show();
                        }
                    };

                    flowKeywords.Controls.Add(button);
                }
            }
            UpdateRemainingChars();
        }

        //GPT3

        //GPT3
        private void btnObjectOwners_Click(object sender, EventArgs e)
        {

            using (var dialog = new Form())
            {
                dialog.Text = String.Empty;
                dialog.AutoSize = true;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ControlBox = false;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new Size(400, 80);

                var tableLayout = new TableLayoutPanel();
                tableLayout.ColumnCount = 2;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

                var ownerLabel = new Label();
                ownerLabel.Text = "Key";
                ownerLabel.TextAlign = ContentAlignment.MiddleCenter;

                var qtyLabel = new Label();
                qtyLabel.Text = "Value";
                qtyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var ownerTextBox = new TextBox();
                ownerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                ownerTextBox.Multiline = true;
                ownerTextBox.Size = new Size(170, 70);

                var qtyTextBox = new TextBox();
                qtyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                qtyTextBox.Size = new Size(170, 70);
                qtyTextBox.Multiline = true;
                
                tableLayout.Controls.Add(ownerLabel, 0, 0);
                tableLayout.Controls.Add(qtyLabel, 1, 0);
                tableLayout.Controls.Add(ownerTextBox, 0, 1);
                tableLayout.Controls.Add(qtyTextBox, 1, 1);

                var addButton = new Button();
                addButton.Text = "Add";
                addButton.DialogResult = DialogResult.OK;
                addButton.Anchor = AnchorStyles.Right;

                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Anchor = AnchorStyles.Right;

                dialog.Controls.Add(tableLayout);
                dialog.Controls.Add(addButton);
                dialog.Controls.Add(cancelButton);

                tableLayout.Dock = DockStyle.Top;
                addButton.Dock = DockStyle.Right;
                cancelButton.Dock = DockStyle.Right;

                dialog.AcceptButton = addButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var owner = ownerTextBox.Text;
                    var qty = qtyTextBox.Text;

                   
                        var button = new Button();
                        button.Text = $"{owner}:{qty}";
                        button.AutoSize = true;
                        button.MouseClick += (s, ev) =>
                        {
                            // only proceed if left mouse button is clicked
                            if (ev.Button == MouseButtons.Left)
                            {
                                // disable the button to prevent double-clicks
                                button.Enabled = false;

                                // remove the button from the flow layout panel and dispose of it
                                flowLocation.Controls.Remove(button);
                                button.Dispose();
                            }
                        };                       

                        flowLocation.Controls.Add(button);
                   
                }
            }
            UpdateRemainingChars();
        }


     


        //GPT3
        private void flowAttribute_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (flowURL.Controls.Count < 1)
            {
                btnURL.BackColor = Color.White;
                btnURL.ForeColor = Color.Black;
            }
            UpdateRemainingChars();
        }
        //GPT3
        private void flowKeyword_ControlRemoved(object sender, ControlEventArgs e)
        {

            if (flowKeywords.Controls.Count < 1)
            {
                btnObjectKeywords.BackColor = Color.White;
                btnObjectKeywords.ForeColor = Color.Black;
            }
            UpdateRemainingChars();

        }

        private void txtMaximum_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
            if (txtLastName.Text != "")
            {
                btnLastName.BackColor = Color.Blue;
                btnLastName.ForeColor = Color.Yellow;
                if (!int.TryParse(txtLastName.Text, out int value)) { txtLastName.Text = "0"; }
            }
        }

        private void flowOwners_ControlRemoved(object sender, ControlEventArgs e)
        {

            if (flowLocation.Controls.Count < 1)
            {
                btnLocation.BackColor = Color.White;
                btnLocation.ForeColor = Color.Black;


            }
            UpdateRemainingChars();
        }


        private void btnMint_Click(object sender, EventArgs e)
        {

            ismint = true;
            UpdateRemainingChars();

            OBJState OBJ = OBJState.GetObjectByAddress(txtObjectAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");

            if (OBJ.URN != null)
            {
                lblObjectStatus.Text = "created:[" + OBJ.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  locked:[" + OBJ.LockedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  last seen:[" + OBJ.ChangeDate.ToString("MM/dd/yyyy hh:mm:ss") + "]";
                lblObjectStatus.Text = lblObjectStatus.Text.Replace("Monday, January 1, 0001", " unconfirmed ");

            }
        }









        private void btnScan_Click(object sender, EventArgs e)
        {

        }

        public void LoadFormByAddress(string address)
        {

            PROState foundObject = PROState.GetProfileByAddress(address, "good-user", "better-password", "http://127.0.0.1:18332");
            if (foundObject.URN != null)
            {
                lblObjectStatus.Text = "last seen:[" + foundObject.ChangeDate.ToString("MM/dd/yyyy hh:mm:ss") + "]";


                if (lblObjectStatus.Text.Contains("Monday, January 1, 0001"))
                {
                    lblObjectStatus.Text = lblObjectStatus.Text.Replace("Monday, January 1, 0001", " unconfirmed ");

                }
                else { }

                NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                RPCClient rpcClient = new RPCClient(credentials, new Uri("http://127.0.0.1:18332"), Network.Main);
                string accountName = "";
                try { accountName = rpcClient.SendCommand("getaccount", address).ResultString; } catch { }
                if (accountName != "") { btnMint.Enabled = false; }


                txtFirstName.Text = foundObject.FirstName;
                txtIMG.Text = foundObject.Image;
                txtURN.Text = foundObject.URN;
                txtMiddleName.Text = foundObject.MiddleName;
                txtLastName.Text = foundObject.LastName;
                txtSuffix.Text = foundObject.Suffix;
                txtObjectAddress.Text = address;
                txtBio.Text = foundObject.Bio;

                flowURL.Controls.Clear();

                try
                {
                    // Iterate through all attributes of foundObject and create a button for each
                    foreach (var attrib in foundObject.URL)
                    {
                        // Create a new button with the attribute key and value separated by ':'
                        Button attribButton = new Button();
                        attribButton.AutoSize = true;
                        attribButton.Text = attrib.Key + ":" + attrib.Value;

                        // Add an event handler to the button that removes it from the flowAttribute panel when clicked
                        attribButton.Click += new EventHandler((sender2, e2) =>
                        {
                            flowURL.Controls.Remove(attribButton);
                        });

                        // Add the button to the flowAttribute panel
                        flowURL.Controls.Add(attribButton);
                    }

                }
                catch { }

                // Clear any existing buttons from the flowKeywords panel
                flowKeywords.Controls.Clear();

                List<string> keywords = new List<string>();
                keywords = OBJState.GetKeywordsByAddress(address, "good-user", "better-password", "http://127.0.0.1:18332");

                // Iterate through all attributes of foundObject and create a button for each
                foreach (string attrib in keywords)
                {
                    // Create a new button with the attribute key and value separated by ':'
                    Button attribButton = new Button();
                    attribButton.AutoSize = true;
                    attribButton.Text = attrib;

                    // Add an event handler to the button that removes it from the flowKeywords panel when clicked
                    attribButton.Click += new EventHandler((sender2, e2) =>
                    {
                        flowKeywords.Controls.Remove(attribButton);
                    });

                    // Add an event handler to the button that opens the ObjectBrowser form with the button text
                    attribButton.MouseDown += new MouseEventHandler((sender2, e2) =>
                    {
                        if (e2.Button == MouseButtons.Right)
                        {
                            ObjectBrowser objectBrowserForm = new ObjectBrowser("#" + attribButton.Text);
                            objectBrowserForm.Show();
                        }
                    });

                    // Add the button to the flowKeywords panel
                    flowKeywords.Controls.Add(attribButton);
                }



                flowLocation.Controls.Clear();

                try
                {
                    // Iterate through all attributes of foundObject and create a button for each
                    foreach (KeyValuePair<string, string> attrib in foundObject.Location)
                    {
                        // Split the key and value and create a new button with the attribute key and value separated by ':'
                        string buttonText = attrib.Key + ":" + attrib.Value.ToString();
                        Button attribButton = new Button();
                        attribButton.AutoSize = true;
                        attribButton.Text = buttonText;

                        // Add an event handler to the button that removes it from the flowAttribute panel when clicked
                        attribButton.Click += new EventHandler((sender2, e2) =>
                        {
                            flowLocation.Controls.Remove(attribButton);
                        });

                        // Add an event handler to the button that opens the ObjectBrowser form on right click using the key value
                        attribButton.MouseUp += new MouseEventHandler((sender2, e2) =>
                        {
                            if (e2.Button == MouseButtons.Right)
                            {
                                string[] keyValuePair = attribButton.Text.Split(':');
                                string key = keyValuePair[0].Trim();
                                ObjectBrowser form = new ObjectBrowser(key);
                                form.Show();
                            }
                            else if (e2.Button == MouseButtons.Left)
                            {
                                flowLocation.Controls.Remove(attribButton);
                            }
                        });

                        // Add the button to the flowOwners panel
                        flowLocation.Controls.Add(attribButton);
                    }

                }
                catch { }


            }

        }

        private void btnFirstName_Click(object sender, EventArgs e)
        {
            if (btnFirstName.BackColor == Color.Blue) { btnFirstName.BackColor = Color.White; btnFirstName.ForeColor = Color.Black; }
            else
            {
                btnFirstName.BackColor = Color.Blue; btnFirstName.ForeColor = Color.Yellow;
            }
            UpdateRemainingChars();
        }

        private void txtMiddleName_TextChanged(object sender, EventArgs e)
        {
            if (txtMiddleName.Text != "")
            {
                btnMiddleName.BackColor = Color.Blue;
                btnMiddleName.ForeColor = Color.Yellow;
            }
            else
            {
                btnMiddleName.BackColor = Color.White;
                btnMiddleName.ForeColor = Color.Black;
            }
            UpdateRemainingChars();
        }

        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
            if (txtLastName.Text != "")
            {
                btnLastName.BackColor = Color.Blue;
                btnLastName.ForeColor = Color.Yellow;
            }
            else
            {
                btnLastName.BackColor = Color.White;
                btnLastName.ForeColor = Color.Black;
            }
        }

        private void txtSuffix_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
            if (txtSuffix.Text != "")
            {
                btnSuffix.BackColor = Color.Blue;
                btnSuffix.ForeColor = Color.Yellow;
            }
            else
            {
                btnSuffix.BackColor = Color.White;
                btnSuffix.ForeColor = Color.Black;
            }
        }

        private void btnScan_Click_1(object sender, EventArgs e)
        {
            // Create a new form for the address input dialog
            Form addressForm = new Form();
            addressForm.Text = "Search";
            addressForm.StartPosition = FormStartPosition.CenterParent;
            addressForm.ControlBox = false;
            addressForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addressForm.Width = 410;
            addressForm.Height = 210;
            // Create a label with instructions for the user
            Label instructionLabel = new Label();
            instructionLabel.Text = "Enter profile address";
            instructionLabel.Font = new Font(instructionLabel.Font.FontFamily, 16, System.Drawing.FontStyle.Bold);
            instructionLabel.AutoSize = true;
            instructionLabel.Location = new Point(20, 20);
            addressForm.Controls.Add(instructionLabel);

            // Create a text box for the user to input the address
            TextBox addressTextBox = new TextBox();
            addressTextBox.Location = new Point(20, 70);
            addressTextBox.Width = 300;
            addressTextBox.Height = 50;
            addressTextBox.Multiline = true;
            addressForm.Controls.Add(addressTextBox);

            // Create a button for the user to click to search for the address
            Button searchButton = new Button();
            searchButton.Text = "Search";
            searchButton.Location = new Point(20, 120);
            searchButton.Width = 100;
            searchButton.Click += new EventHandler((searchSender, searchE) =>
            {
                // Perform the search function here
                LoadFormByAddress(addressTextBox.Text);


                addressForm.Close();
                // ...
            }); addressForm.Controls.Add(searchButton);


            // Set the default button to be the search button
            addressForm.AcceptButton = searchButton;

            // Show the dialog and set focus to the address text box
            addressForm.ShowDialog();

            if (txtIMG.Text != "")
            {
                btnObjectImage.PerformClick();
            }
         
        }

        private void btnMiddleName_Click(object sender, EventArgs e)
        {
            if (btnMiddleName.BackColor == Color.Blue) { btnMiddleName.BackColor = Color.White; btnMiddleName.ForeColor = Color.Black; }
            else
            {
                btnMiddleName.BackColor = Color.Blue; btnMiddleName.ForeColor = Color.Yellow;
            }
            UpdateRemainingChars();
        }

        private void btnMint_Click_1(object sender, EventArgs e)
        {

            ismint = true;
            UpdateRemainingChars();

            PROState PRO = PROState.GetProfileByAddress(txtObjectAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");

            if (PRO.URN != null)
            {
                lblObjectStatus.Text = "last seen:[" + PRO.ChangeDate.ToString("MM/dd/yyyy hh:mm:ss") + "]";
                lblObjectStatus.Text = lblObjectStatus.Text.Replace("Monday, January 1, 0001", " unconfirmed ");

            }
        }
    }
}
