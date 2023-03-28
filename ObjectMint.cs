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
    public partial class ObjectMint : Form
    {
        private QrEncoder encoder = new QrEncoder();
        private GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(2, QuietZoneModules.Two));
        private bool ismint = false;

        public ObjectMint()
        {
            InitializeComponent();
        }

    
        private void UpdateRemainingChars()
        {
            if (txtURN.Text != "" && txtTitle.Text != "" && txtObjectAddress.Text != "" && flowOwners.Controls.Count > 0)
            {
                if (!btnEdit.Enabled && (btnObjectName.BackColor == Color.Blue || btnObjectURN.BackColor == Color.Blue || btnObjectURI.BackColor == Color.Blue || btnObjectOwners.BackColor == Color.Blue || btnObjectCreators.BackColor == Color.Blue || btnObjectAttributes.BackColor == Color.Blue || btnObjectImage.BackColor == Color.Blue || btnObjectMaximum.BackColor == Color.Blue || btnObjectDescription.BackColor == Color.Blue || btnObjectKeywords.BackColor == Color.Blue)) { btnMint.Enabled = true; btnPrint.Enabled = true; }
            }


            int maxsize = 888;

            maxsize = maxsize - txtDescription.Text.Length - txtIMG.Text.Length - txtTitle.Text.Length - txtURI.Text.Length - txtURN.Text.Length - txtMaximum.Text.Length;
            maxsize = maxsize - 40; ///estimated json chars required.

            foreach (System.Windows.Forms.Control control in flowAttribute.Controls)
            {
                maxsize = maxsize - (control.Text.Length + 5);
            }

            maxsize = maxsize - (flowKeywords.Controls.Count * 20) + 5;

            foreach (System.Windows.Forms.Control control in flowOwners.Controls)
            {

                maxsize = maxsize - (control.Text.Length + 5);


            }
            string License = "";
            foreach (RadioButton control in PanelLicense.Controls)
            {
                if (control.Checked)
                {
                    maxsize = maxsize - (control.Text.Length + 5);
                    License = control.Text;
                }
            }

            maxsize = maxsize - (flowCreators.Controls.Count * 5) + 5;

            lblRemainingChars.Text = maxsize.ToString();


            if (maxsize < 0)
            {
                btnPrint.Enabled = false;
                btnMint.Enabled = false;
            }


            OBJ OBJJson = new OBJ();
            if (btnObjectName.BackColor == Color.Blue) { OBJJson.nme = txtTitle.Text; }
            if (btnObjectImage.BackColor == Color.Blue) { OBJJson.img = txtIMG.Text; }
            if (btnObjectURN.BackColor == Color.Blue) { OBJJson.urn = txtURN.Text; }
            if (btnObjectURI.BackColor == Color.Blue) { OBJJson.uri = txtURI.Text; }
            if (btnObjectDescription.BackColor == Color.Blue) { OBJJson.dsc = txtDescription.Text; }
            if (License != "No License / All Rights Reserved") { OBJJson.lic = License; }
            if (btnObjectMaximum.BackColor == Color.Blue) { if (txtMaximum.Text == "") { } else { try { OBJJson.max = int.Parse(txtMaximum.Text); } catch { OBJJson.max = 0; } } }

            Dictionary<string, string> mintAttributes = new Dictionary<string, string>();
            foreach (Button attributeControl in flowAttribute.Controls)
            {
                string[] parts = attributeControl.Text.Split(':');
                mintAttributes.Add(parts[0], parts[1]);
            }
            OBJJson.atr = mintAttributes;

            if (btnObjectAddress.BackColor == Color.Blue)
            {
                List<string> mintCreators = new List<string>();
                mintCreators.Add(txtObjectAddress.Text);
                foreach (Button creatorControl in flowCreators.Controls) { mintCreators.Add(creatorControl.Text); }
                OBJJson.cre = mintCreators.ToArray();
            }

            if (btnObjectOwners.BackColor == Color.Blue)
            {
                Dictionary<string, long> mintOwners = new Dictionary<string, long>();
                foreach (Button ownerControl in flowOwners.Controls)
                {
                    string[] parts = ownerControl.Text.Split(':');
                    try { mintOwners.Add(parts[0], long.Parse(parts[1])); } catch { }
                }
                OBJJson.own = mintOwners;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            // Serialize the modified JObject back into a JSON string
            var objectSerialized = JsonConvert.SerializeObject(OBJJson, Formatting.None, settings);
            if (btnEdit.Enabled) { txtOBJJSON.Text = objectSerialized; }
            else
            {
                txtOBJJSON.Text = objectSerialized.Replace(",\"max\":0", "").Replace(",\"atr\":{}", "").Replace(",\"own\":{}", "").Replace(",\"uri\":\"\"", "").Replace(",\"dsc\":\"\"", "").Replace(",\"img\":\"\"", "").Replace(",\"lic\":\"\"", "");
            }

            txtOBJP2FK.Text = "OBJ" + ":" + txtOBJJSON.Text.Length + ":" + txtOBJJSON.Text;

            if (btnMint.Enabled)
            {
                NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                RPCClient rpcClient = new RPCClient(credentials, new Uri("http://127.0.0.1:18332"), Network.Main);
                System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                byte[] hashValue = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(txtOBJP2FK.Text));
                string signatureAddress;

                if (flowCreators.Controls.Count > 0)
                { signatureAddress = flowCreators.Controls[0].Text; }
                else { signatureAddress = txtObjectAddress.Text; }
                string signature = "";
                try { signature = rpcClient.SendCommand("signmessage", signatureAddress, BitConverter.ToString(hashValue).Replace("-", String.Empty)).ResultString; }catch(Exception ex) 
                { lblObjectStatus.Text = ex.Message;
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


                //attempt file registration
                string fileSource = txtURN.Text.Trim();
                byte[] fileBytes;

                if (fileSource.ToUpper().StartsWith("IPFS:"))
                {

                    string ipfsHash = fileSource.Substring(5);

                    fileSource = @"ipfs\" + ipfsHash;
                }
                else
                {
                    // extract the remaining string after the colon, or use the full string if there is no colon
                    int colonIndex = fileSource.IndexOf(':');
                    if (colonIndex >= 0)
                    {
                        fileSource = fileSource.Substring(colonIndex + 1);
                    }
                    // concatenate with "root\"
                    fileSource = @"root\" + fileSource;
                }

                // attempt to read the first 20 bytes of the urn file into a base 58 encoded byte array with version and checksum
                try
                {

                    byte[] payload = new byte[21];

                    using (FileStream fileStream = new FileStream(fileSource, FileMode.Open))
                    {
                        fileStream.Read(payload, 1, 20);
                    }

                    payload[0] = Byte.Parse("111");
                    string objectaddress = Base58.EncodeWithCheckSum(payload);

                    encodedList.Add(objectaddress);

                }
                catch { }



                foreach (Button keywordbtn in flowKeywords.Controls)
                {
                    encodedList.Add(Root.GetPublicAddressByKeyword(keywordbtn.Text));
                }

                foreach (Button ownerbtn in flowOwners.Controls)
                {
                    if (ownerbtn.Text.Split(':')[0] != signatureAddress)
                    {
                        encodedList.Add(ownerbtn.Text.Split(':')[0]);
                    }
                }

                foreach (Button creatorbtn in flowCreators.Controls)
                {

                    if (creatorbtn.Text != signatureAddress)
                    {
                        encodedList.Add(creatorbtn.Text);
                    }
                }

                if (!encodedList.Contains(txtObjectAddress.Text)) { encodedList.Add(txtObjectAddress.Text); }
                encodedList.Add(signatureAddress);
                txtAddressListJSON.Text = JsonConvert.SerializeObject(encodedList.Distinct());

                lblCost.Text = "cost: " + (0.00000546 * encodedList.Count).ToString("0.00000000") + "  + miner fee";

                if (ismint)
                {
                    var recipients = new Dictionary<string, decimal>();
                    foreach (var encodedAddress in encodedList)
                    {

                        try { recipients.Add(encodedAddress, 0.00000546m); } catch {}

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





        private void button12_Click_1(object sender, EventArgs e)
        {

            System.Drawing.Bitmap bitmap = new Bitmap(this.Width - 22, this.Height - 44);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(this.PointToScreen(new Point(0, 0)), new Point(0, 0), this.Size);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            PrintImage(bitmap);

        }
        Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
        System.Drawing.Image bmIm;

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

        private void txtIMG_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
        }

        private void txtURN_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();

            if (txtURN.Text != "")
            {
                if (flowOwners.Controls.Count < 1)
                {

                    lblASCIIURN.Text = "Add 1 or more owners each with a qty of 1 or more";
                    lblASCIIURN.Visible = true;

                }

                btnObjectURN.BackColor = Color.Blue;
                btnObjectURN.ForeColor = Color.Yellow;
            }
            else
            {
                btnObjectURN.BackColor = Color.White;
                btnObjectURN.ForeColor = Color.Black;
            }

        }

        private void txtURI_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();

            if (txtURI.Text != "")
            {
                btnObjectURI.BackColor = Color.Blue;
                btnObjectURI.ForeColor = Color.Yellow;
            }
            else
            {
                btnObjectURI.BackColor = Color.White;
                btnObjectURI.ForeColor = Color.Black;
            }
        }

        private void flowAttribute_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateRemainingChars();
            btnObjectAttributes.BackColor = Color.Blue;
            btnObjectAttributes.ForeColor = Color.Yellow;
        }

        private void flowKeyword_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateRemainingChars();
            btnObjectKeywords.BackColor = Color.Blue;
            btnObjectKeywords.ForeColor = Color.Yellow;
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {

            if (txtTitle.Text == "")
            {
                lblASCIIURN.Text = "enter object name to begin";
                lblASCIIURN.Visible = true;
                btnObjectName.BackColor = Color.White;
                btnObjectName.ForeColor = Color.Black;
                btnObjectAttributes.Enabled = false;
                btnObjectDescription.Enabled = false;
                btnObjectImage.Enabled = false;
                btnObjectKeywords.Enabled = false;
                btnObjectName.Enabled = false;
                btnObjectURI.Enabled = false;
                btnObjectURN.Enabled = false;
                btnObjectMaximum.Enabled = false;
                btnObjectCreators.Enabled = false;
                btnObjectOwners.Enabled = false;
                btnObjectAddress.Enabled = false;
                txtDescription.Enabled = false;
                PanelLicense.Enabled = false;
                txtIMG.Enabled = false;
                txtURN.Enabled = false;
                txtURI.Enabled = false;
                txtMaximum.Enabled = false;
                txtObjectAddress.Enabled = false;

            }
            else
            {
                UpdateRemainingChars();
                btnObjectName.BackColor = Color.Blue;
                btnObjectName.ForeColor = Color.Yellow;

                if (txtObjectAddress.Text == "")
                {
                    lblASCIIURN.Text = "push 💎 to obtain a new object address";
                    lblASCIIURN.Visible = true;
                    txtObjectAddress.Enabled = true;
                    btnObjectAddress.Enabled = true;

                }
                else
                {
                    lblASCIIURN.Visible = false;

                    btnObjectAttributes.Enabled = true;
                    btnObjectDescription.Enabled = true;
                    btnObjectImage.Enabled = true;
                    btnObjectKeywords.Enabled = true;
                    btnObjectName.Enabled = true;
                    btnObjectURI.Enabled = true;
                    btnObjectURN.Enabled = true;
                    PanelLicense.Enabled = true;
                    btnObjectCreators.Enabled = true;
                    btnObjectOwners.Enabled = true;
                    btnObjectAddress.Enabled = true;
                    btnObjectMaximum.Enabled = true;
                    txtDescription.Enabled = true;
                    txtIMG.Enabled = true;
                    txtURN.Enabled = true;
                    txtURI.Enabled = true;
                    txtMaximum.Enabled = true;
                    btnObjectMaximum.Enabled = true;
                    txtObjectAddress.Enabled = true;

                }

            }
        }

        private void ObjectMint_Load(object sender, EventArgs e)
        {
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
            if (txtDescription.Text != "")
            {
                btnObjectDescription.BackColor = Color.Blue;
                btnObjectDescription.ForeColor = Color.Yellow;
                if (btnMint.Enabled)
                {
                    try
                    {
                        using (Bitmap qrCode = GenerateQRCode(txtAddressListJSON.Text))
                        {
                            if (qrCode != null)
                            {
                                Directory.CreateDirectory(@"root\" + txtObjectAddress.Text);
                                qrCode.Save(@"root\" + txtObjectAddress.Text + @"\OBJPrint.png", ImageFormat.Png);
                                pictureBox1.ImageLocation = @"root\" + txtObjectAddress.Text + @"\OBJPrint.png";
                            }
                        }
                    }
                    catch { }




                }
            }
            else
            {
                btnObjectDescription.BackColor = Color.White;
                btnObjectDescription.ForeColor = Color.Black;
            }
        }

        private void flowCreators_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateRemainingChars();
            btnObjectCreators.BackColor = Color.Blue;
            btnObjectCreators.ForeColor = Color.Yellow;
        }

        private void flowOwners_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateRemainingChars();
            lblASCIIURN.Visible = false;
            btnObjectOwners.BackColor = Color.Blue;
            btnObjectOwners.ForeColor = Color.Yellow;
        }

        private void txtObjectAddress_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();
            if (txtObjectAddress.Text != "")
            {
                if (txtURN.Text == "")
                {
                    lblASCIIURN.Text = "click the urn and img buttons to upload your file with thumbnail to the interplanetary file system\r\n\r\n\r\n click them again for interplanetary file system verification!";
                    lblASCIIURN.Visible = true;
                }
                else { lblASCIIURN.Visible = false; }
                btnObjectAddress.BackColor = Color.Blue;
                btnObjectAddress.ForeColor = Color.Yellow;
                btnObjectAttributes.Enabled = true;
                btnObjectDescription.Enabled = true;
                btnObjectImage.Enabled = true;
                btnObjectKeywords.Enabled = true;
                btnObjectName.Enabled = true;
                btnObjectURI.Enabled = true;
                btnObjectURN.Enabled = true;

                btnObjectCreators.Enabled = true;
                btnObjectOwners.Enabled = true;
                btnObjectAddress.Enabled = true;
                btnObjectMaximum.Enabled = true;
                txtDescription.Enabled = true;
                PanelLicense.Enabled = true;
                txtIMG.Enabled = true;
                txtURN.Enabled = true;
                txtURI.Enabled = true;
                txtMaximum.Enabled = true;
                btnObjectMaximum.Enabled = true;
                txtObjectAddress.Enabled = true;
                btnObjectName.BackColor = Color.Blue;
                btnObjectName.ForeColor = Color.Yellow;


            }
            else
            {
                lblASCIIURN.Text = "push 👑 to obtain a new object address";
                lblASCIIURN.Visible = true;
                txtTitle.Enabled = true;
                txtObjectAddress.Enabled = true;
                btnObjectAddress.Enabled = true;
                btnObjectName.Enabled = true;
                btnObjectAddress.BackColor = Color.White;
                btnObjectAddress.ForeColor = Color.Black;
                btnObjectAttributes.Enabled = false;
                btnObjectDescription.Enabled = false;
                btnObjectImage.Enabled = false;
                btnObjectKeywords.Enabled = false;
                btnObjectName.Enabled = false;
                btnObjectURI.Enabled = false;
                btnObjectURN.Enabled = false;
                btnObjectCreators.Enabled = false;
                btnObjectOwners.Enabled = false;
                btnObjectMaximum.Enabled = false;
                txtDescription.Enabled = false;
                PanelLicense.Enabled = false;
                txtIMG.Enabled = false;
                txtURN.Enabled = false;
                txtURI.Enabled = false;
                txtMaximum.Enabled = false;
                txtObjectAddress.Enabled = false;


            }
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
                try { newAddress = rpcClient.SendCommand("getnewaddress", txtTitle.Text + "!" + DateTime.UtcNow.ToString("yyyyMMddHHmmss")).ResultString; } catch { }
                txtObjectAddress.Text = newAddress;
                txtTitle.Enabled = false;
                btnObjectName.Enabled = false;
                lblObjectStatus.Text = "";
                lblCost.Text = "";
                btnEdit.Enabled = false;
                UpdateRemainingChars();
            }

        }


        private void btnObjectName_Click(object sender, EventArgs e)
        {
            if (btnObjectName.BackColor == Color.Blue) { btnObjectName.BackColor = Color.White; btnObjectName.ForeColor = Color.Black; }
            else
            {
                if (txtTitle.Text != "") { btnObjectName.BackColor = Color.Blue; btnObjectName.ForeColor = Color.Yellow; } else { btnObjectName.BackColor = Color.White; btnObjectName.ForeColor = Color.Black; }
            }
            UpdateRemainingChars();
        }

        private void btnMaximum_Click(object sender, EventArgs e)
        {

            if (btnObjectMaximum.BackColor == Color.Blue) { btnObjectMaximum.BackColor = Color.White; btnObjectMaximum.ForeColor = Color.Black; }
            else
            {
                if (txtMaximum.Text != "") { btnObjectMaximum.BackColor = Color.Blue; btnObjectMaximum.ForeColor = Color.Yellow; }
                else
                {
                    try
                    {
                        if (long.Parse(txtMaximum.Text.Replace(",", "")) <= 5149219112448) { btnObjectMaximum.BackColor = Color.Blue; btnObjectMaximum.ForeColor = Color.Yellow; } else { btnObjectMaximum.BackColor = Color.White; btnObjectMaximum.ForeColor = Color.Black; }

                    }
                    catch { btnObjectMaximum.BackColor = Color.White; btnObjectMaximum.ForeColor = Color.Black; }
                }
            }

            UpdateRemainingChars();
        }

        private void btnObjectDescription_Click(object sender, EventArgs e)
        {

            if (btnObjectDescription.BackColor == Color.Blue) { btnObjectDescription.BackColor = Color.White; btnObjectDescription.ForeColor = Color.Black; }
            else
            {
                if (txtTitle.Text != "") { btnObjectDescription.BackColor = Color.Blue; btnObjectDescription.ForeColor = Color.Yellow; } else { btnObjectDescription.BackColor = Color.White; btnObjectDescription.ForeColor = Color.Black; }
            }
            UpdateRemainingChars();
        }

        private async void btnObjectImage_Click(object sender, EventArgs e)
        {
            if (btnObjectImage.BackColor == Color.Blue) { btnObjectImage.BackColor = Color.White; btnObjectImage.ForeColor = Color.Black; }
            else
            {


                webviewer.Visible = false;
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
                    pictureBox1.SuspendLayout();
                    if (File.Exists(imgurn) || imgurn.ToUpper().StartsWith("HTTP"))
                    {

                        pictureBox1.ImageLocation = imgurn;
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

                            pictureBox1.ImageLocation = randomGifFile;
                            pictureBox2.ImageLocation = randomGifFile;

                        }
                        else
                        {
                            try
                            {
                                pictureBox1.ImageLocation = @"includes\HugPuddle.jpg";
                                pictureBox2.ImageLocation = @"includes\HugPuddle.jpg";
                            }
                            catch { }
                        }


                    }
                    pictureBox1.ResumeLayout();




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
                string urn = "";
                lblURNBlockDate.Text = "[ unable to verify ]";
                webviewer.Visible = false;
                lblASCIIURN.Visible = false;
                if (txtURN.Text != "")
                {
                    urn = txtURN.Text;

                    if (!txtURN.Text.ToLower().StartsWith("http"))
                    {
                        urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + txtURN.Text.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");
                        if (txtURN.Text.ToLower().StartsWith("ipfs:")) { urn = urn.Replace(@"\root\", @"\ipfs\"); }
                    }
                    else
                    {
                        webviewer.Visible = true;
                        await webviewer.EnsureCoreWebView2Async();
                        webviewer.CoreWebView2.Navigate(txtURN.Text);
                        lblURNBlockDate.Text = "http get: " + DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                        btnObjectURN.BackColor = Color.Blue;
                        btnObjectURN.ForeColor = Color.Yellow;
                        return;
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
                        openFileDialog1.Title = "Select File";

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
                        txtURN.Text = ipfsHash + @"\" + fileName;
                    }


                }


                string extension = "";

                try
                {
                    extension = Path.GetExtension(urn).ToLower();
                    Root root = new Root();
                    Match urimatch = regexTransactionId.Match(txtURN.Text);
                    string transactionid = urimatch.Value;
                    switch (txtURN.Text.Substring(0, 4).ToUpper())
                    {
                        case "MZC:":

                            root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                            try
                            {
                                lblURNBlockDate.Text = "mazacoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                btnObjectURN.BackColor = Color.Blue;
                                btnObjectURN.ForeColor = Color.Yellow;
                            }
                            catch { }
                            break;
                        case "BTC:":

                            root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                            try
                            {
                                lblURNBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                btnObjectURN.BackColor = Color.Blue;
                                btnObjectURN.ForeColor = Color.Yellow;
                            }
                            catch { }
                            break;
                        case "LTC:":

                            root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                            try
                            {
                                lblURNBlockDate.Text = "litecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                btnObjectURN.BackColor = Color.Blue;
                                btnObjectURN.ForeColor = Color.Yellow;
                            }
                            catch { }
                            break;
                        case "DOG:":

                            root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                            try
                            {
                                lblURNBlockDate.Text = "dogecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                btnObjectURN.BackColor = Color.Blue;
                                btnObjectURN.ForeColor = Color.Yellow;
                            }
                            catch { }

                            break;
                        case "IPFS":
                            if (txtURN.Text.Length == 51) { urn += @"\artifact"; }
                            if (!System.IO.Directory.Exists(@"ipfs/" + txtURN.Text.Substring(5, 46) + "-build") && !System.IO.Directory.Exists(@"ipfs/" + txtURN.Text.Substring(5, 46)))
                            {


                                Task ipfsTask = Task.Run(() =>
                                {
                                    Directory.CreateDirectory(@"ipfs/" + txtURN.Text.Substring(5, 46) + "-build");
                                    Process process2 = new Process();
                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                    process2.StartInfo.Arguments = "get " + txtURN.Text.Substring(5, 46) + @" -o ipfs\" + txtURN.Text.Substring(5, 46);
                                    process2.Start();
                                    process2.WaitForExit();

                                    if (System.IO.File.Exists("ipfs/" + txtURN.Text.Substring(5, 46)))
                                    {
                                        try { System.IO.File.Move("ipfs/" + txtURN.Text.Substring(5, 46), "ipfs/" + txtURN.Text.Substring(5, 46) + "_tmp"); }
                                        catch
                                        {
                                            System.IO.File.Delete("ipfs/" + txtURN.Text.Substring(5, 46) + "_tmp");
                                            System.IO.File.Move("ipfs/" + txtURN.Text.Substring(5, 46), "ipfs/" + txtURN.Text.Substring(5, 46) + "_tmp");

                                        }

                                        string fileName = txtURN.Text.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                        if (fileName == "")
                                        {
                                            fileName = "artifact";
                                        }
                                        else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                        Directory.CreateDirectory(@"ipfs/" + txtURN.Text.Substring(5, 46));
                                        try { System.IO.File.Move("ipfs/" + txtURN.Text.Substring(5, 46) + "_tmp", urn); } catch { }
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
                                                    Arguments = "pin add " + txtURN.Text.Substring(5, 46),
                                                    UseShellExecute = false,
                                                    CreateNoWindow = true
                                                }
                                            };
                                            process3.Start();
                                        }
                                    }

                                    try { Directory.Delete(@"ipfs/" + txtURN.Text.Substring(5, 46)); } catch { }
                                    try
                                    {
                                        Directory.Delete(@"ipfs/" + txtURN.Text.Substring(5, 46) + "-build");
                                    }
                                    catch { }



                                });
                            }
                            else
                            {
                                lblURNBlockDate.Text = "ipfs verified: " + System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                btnObjectURN.BackColor = Color.Blue;
                                btnObjectURN.ForeColor = Color.Yellow;
                            }
                            break;
                        default:

                            root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");
                            if (transactionid != "")
                            {
                                if (Directory.Exists(@"root\" + transactionid))
                                {
                                    try
                                    {

                                        lblURNBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                        btnObjectURN.BackColor = Color.Blue;
                                        btnObjectURN.ForeColor = Color.Yellow;
                                    }
                                    catch { }
                                }
                            }
                            else
                            {
                                lblASCIIURN.Text = txtURN.Text;
                                lblASCIIURN.Visible = true;
                                btnObjectURN.BackColor = Color.Blue;
                                btnObjectURN.ForeColor = Color.Yellow;
                            }

                            break;
                    }

                    if (lblURNBlockDate.Text.Contains("Mon, 01 Jan 0001 12:00:00"))
                    {
                        btnObjectURN.BackColor = Color.Blue;
                        btnObjectURN.ForeColor = Color.Black;
                        lblURNBlockDate.Text = "[ unable to verify ]";
                    }


                }
                catch { }

                switch (extension.ToLower())
                {
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

                        break;

                    case ".glb":
                    case ".fbx":
                        //Show image in main box and show open file button
                        pictureBox1.SuspendLayout();
                        if (File.Exists(urn))
                        {

                            pictureBox1.ImageLocation = pictureBox2.ImageLocation;
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


                        break;
                    case ".htm":
                    case ".html":

                        string potentialyUnsafeHtml = "";
                        try { potentialyUnsafeHtml = System.IO.File.ReadAllText(urn); } catch { }

                        var matches = regexTransactionId.Matches(potentialyUnsafeHtml);
                        foreach (Match transactionID in matches)
                        {

                            switch (txtURN.Text.Substring(0, 4))
                            {
                                case "MZC:":
                                    if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                    {
                                        Task.Run(() =>
                                        {
                                            Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                        });
                                    }
                                    break;
                                case "BTC:":
                                    if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                    {
                                        Task.Run(() =>
                                        {
                                            Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                        });
                                    }
                                    break;
                                case "LTC:":
                                    if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                    {
                                        Task.Run(() =>
                                        {
                                            Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                        });
                                    }
                                    break;
                                case "DOG:":
                                    if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                    {
                                        Task.Run(() =>
                                        {
                                            Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                        });
                                    }
                                    break;
                                default:
                                    if (!System.IO.Directory.Exists(@"root/" + transactionID.Value))
                                    {
                                        Task.Run(() =>
                                        {
                                            Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", @"http://127.0.0.1:18332");
                                        });
                                    }
                                    break;
                            }

                        }

                        string _address = txtObjectAddress.Text;
                        string _viewer = null;
                        string _viewername = null; //to be implemented
                        string _creator = null;
                        int _owner = 0;
                        string _urn = HttpUtility.UrlEncode(txtURN.Text);
                        string _uri = HttpUtility.UrlEncode(txtURI.Text);
                        string _img = HttpUtility.UrlEncode(txtIMG.Text);

                        if (flowOwners.Controls.Count > 0)
                        {
                            _viewer = flowOwners.Controls[0].Text.Split(':')[0];
                            _owner = flowOwners.Controls.Count;
                        }

                        if (flowCreators.Controls.Count > 0)
                        {
                            _creator = flowCreators.Controls[0].Text;
                        }

                        string querystring = "?address=" + _address + "&viewer=" + _viewer + "&viewername=" + _viewername + "&creator=" + _creator + "&owner=" + _owner + "&urn=" + _urn + "&uri=" + _uri + "&img=" + _img;
                        string htmlstring = "<html><body><embed src=\"" + urn + querystring + "\" width=100% height=100%></body></html>";
                        string viewerPath = Path.GetDirectoryName(urn) + @"\urnviewer.html";
                        webviewer.Visible = true;

                        try
                        {
                            System.IO.File.WriteAllText(Path.GetDirectoryName(urn) + @"\urnviewer.html", htmlstring);
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(viewerPath);
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(viewerPath);
                        }

                        break;
                    case ".mp4":
                    case ".avi":
                    case ".mp3":
                    case ".wav":
                    case ".pdf":
                        webviewer.Visible = true;
                        string viewerPath2 = Path.GetDirectoryName(urn) + @"\urnviewer.html";
                        string htmlstring2 = "<html><body><embed src=\"" + urn + "\" width=100% height=100%></body></html>";

                        try
                        {
                            System.IO.File.WriteAllText(Path.GetDirectoryName(urn) + @"\urnviewer.html", htmlstring2);
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(viewerPath2);
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(viewerPath2);
                        }


                        break;

                    default:

                        pictureBox1.SuspendLayout();
                        if (File.Exists(urn))
                        {

                            pictureBox1.ImageLocation = pictureBox2.ImageLocation;
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

                        break;
                }
            }
            UpdateRemainingChars();

        }

        private void btnObjectURI_Click(object sender, EventArgs e)
        {
            if (btnObjectURI.BackColor == Color.Blue) { btnObjectURI.BackColor = Color.White; btnObjectURI.ForeColor = Color.Black; }
            else
            {
                if (txtTitle.Text != "") { btnObjectURI.BackColor = Color.Blue; btnObjectURI.ForeColor = Color.Yellow; } else { btnObjectURI.BackColor = Color.White; btnObjectURI.ForeColor = Color.Black; }
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
                keyLabel.Text = "Key";
                keyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var valueLabel = new Label();
                valueLabel.Text = "Value";
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
                    button.Text = $"{key}: {value}";
                    button.AutoSize = true;
                    button.ForeColor = Color.White;
                    button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                    button.Click += (s, ev) => flowAttribute.Controls.Remove(button);
                    flowAttribute.Controls.Add(button);
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
                    button.ForeColor = Color.White;
                    button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
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
        private void btnObjectCreators_Click(object sender, EventArgs e)
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
                keyLabel.Text = "Creator";
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
                    button.ForeColor = Color.White;
                    button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
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
                            flowCreators.Controls.Remove(button);
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
                            ObjectBrowser form = new ObjectBrowser(labelText);
                            form.Show();
                        }
                    };

                    flowCreators.Controls.Add(button);
                }
            }
            UpdateRemainingChars();
        }

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
                ownerLabel.Text = "Owner";
                ownerLabel.TextAlign = ContentAlignment.MiddleCenter;

                var qtyLabel = new Label();
                qtyLabel.Text = "Qty";
                qtyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var ownerTextBox = new TextBox();
                ownerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                ownerTextBox.Multiline = true;
                ownerTextBox.Size = new Size(170, 70);

                var qtyTextBox = new TextBox();
                qtyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                qtyTextBox.Size = new Size(170, 70);
                qtyTextBox.Multiline = true;
                qtyTextBox.KeyPress += new KeyPressEventHandler(qtyTextBox_KeyPress);

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

                    var isNumeric = int.TryParse(qty, out _);

                    if (isNumeric)
                    {
                        var button = new Button();
                        button.ForeColor = Color.White;
                        button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                        button.Text = $"{owner}: {qty}";
                        button.AutoSize = true;
                        button.MouseClick += (s, ev) =>
                        {
                            // only proceed if left mouse button is clicked
                            if (ev.Button == MouseButtons.Left)
                            {
                                // disable the button to prevent double-clicks
                                button.Enabled = false;

                                // remove the button from the flow layout panel and dispose of it
                                flowOwners.Controls.Remove(button);
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
                                ObjectBrowser form = new ObjectBrowser(labelText.Split(':')[0]);
                                form.Show();
                            }
                        };



                        flowOwners.Controls.Add(button);
                    }
                    else
                    {
                        MessageBox.Show("Qty field only accepts numeric input.");
                    }
                }
            }
            UpdateRemainingChars();
        }

        private void qtyTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        //GPT3
        private void flowAttribute_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (flowAttribute.Controls.Count < 1)
            {
                btnObjectAttributes.BackColor = Color.White;
                btnObjectAttributes.ForeColor = Color.Black;
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
            if (txtMaximum.Text != "")
            {
                btnObjectMaximum.BackColor = Color.Blue;
                btnObjectMaximum.ForeColor = Color.Yellow;
                if (!int.TryParse(txtMaximum.Text, out int value)) { txtMaximum.Text = "0"; }
            }
        }

        private void flowOwners_ControlRemoved(object sender, ControlEventArgs e)
        {

            if (flowOwners.Controls.Count < 1)
            {
                btnObjectOwners.BackColor = Color.White;
                btnObjectOwners.ForeColor = Color.Black;
                lblASCIIURN.Visible = true;

            }
            UpdateRemainingChars();
        }

        private void flowCreators_ControlRemoved(object sender, ControlEventArgs e)
        {

            if (flowCreators.Controls.Count < 1)
            {
                btnObjectCreators.BackColor = Color.White;
                btnObjectCreators.ForeColor = Color.Black;
            }
            UpdateRemainingChars();
        }

        private void btnMint_Click(object sender, EventArgs e)
        {

            try
            {

                using (Bitmap qrCode = GenerateQRCode(txtAddressListJSON.Text))
                {
                    if (qrCode != null)
                    {

                        Directory.CreateDirectory(@"root\" + txtObjectAddress.Text);
                        qrCode.Save(@"root\" + txtObjectAddress.Text + @"\OBJPrint.png", ImageFormat.Png);

                    }
                }
            }
            catch { }

            pictureBox1.ImageLocation = @"root\" + txtObjectAddress.Text + @"\OBJPrint.png";
            ismint = true;
            UpdateRemainingChars();

            OBJState OBJ = OBJState.GetObjectByAddress(txtObjectAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");

            if (OBJ.URN != null)
            {
                lblObjectStatus.Text = "created:[" + OBJ.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  locked:[" + OBJ.LockedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  last seen:[" + OBJ.ChangeDate.ToString("MM/dd/yyyy hh:mm:ss") + "]";
                lblObjectStatus.Text = lblObjectStatus.Text.Replace("Monday, January 1, 0001", " unconfirmed ");
               
            }
        }

        private async void btnPrint_Click(object sender, EventArgs e)
        {
            UpdateRemainingChars();

            try
            {
                using (Bitmap qrCode = GenerateQRCode(txtAddressListJSON.Text))
                {
                    if (qrCode != null)
                    {
                        Directory.CreateDirectory(@"root\" + txtObjectAddress.Text);
                        qrCode.Save(@"root\" + txtObjectAddress.Text + @"\OBJPrint.png", ImageFormat.Png);
                        pictureBox1.LoadCompleted += PictureBox_LoadCompleted;
                        pictureBox1.ImageLocation = @"root\" + txtObjectAddress.Text + @"\OBJPrint.png";
                    }
                }
            }
            catch { }
        }

        private void PictureBox_LoadCompleted(object sender, EventArgs e)
        {
            lblRemainingChars.Visible = false;
            pictureBox1.Refresh();
            System.Drawing.Bitmap bitmap = new Bitmap(this.Width - 22, this.Height - 44);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(this.PointToScreen(new Point(0, 0)), new Point(0, 0), this.Size);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            PrintImage(bitmap);
            lblRemainingChars.Visible = true;

            // unsubscribe from LoadCompleted event
            pictureBox1.LoadCompleted -= PictureBox_LoadCompleted;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton != null && radioButton.Checked)
            {
                // Uncheck all other radio buttons in the panel
                foreach (Control control in radioButton.Parent.Controls)
                {
                    RadioButton otherRadioButton = control as RadioButton;

                    if (otherRadioButton != null && otherRadioButton != radioButton)
                    {
                        otherRadioButton.Checked = false;
                    }
                }
            }
            UpdateRemainingChars();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton != null && radioButton.Checked)
            {
                // Uncheck all other radio buttons in the panel
                foreach (Control control in radioButton.Parent.Controls)
                {
                    RadioButton otherRadioButton = control as RadioButton;

                    if (otherRadioButton != null && otherRadioButton != radioButton)
                    {
                        otherRadioButton.Checked = false;
                    }
                }
            }
            UpdateRemainingChars();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton != null && radioButton.Checked)
            {
                // Uncheck all other radio buttons in the panel
                foreach (Control control in radioButton.Parent.Controls)
                {
                    RadioButton otherRadioButton = control as RadioButton;

                    if (otherRadioButton != null && otherRadioButton != radioButton)
                    {
                        otherRadioButton.Checked = false;
                    }
                }
            }
            UpdateRemainingChars();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton != null && radioButton.Checked)
            {
                // Uncheck all other radio buttons in the panel
                foreach (Control control in radioButton.Parent.Controls)
                {
                    RadioButton otherRadioButton = control as RadioButton;

                    if (otherRadioButton != null && otherRadioButton != radioButton)
                    {
                        otherRadioButton.Checked = false;
                    }
                }
            }
            UpdateRemainingChars();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            // Create a new form for the address input dialog
            Form addressForm = new Form();
            addressForm.Text = "Enter an Object Address";
            addressForm.StartPosition = FormStartPosition.CenterParent;
            addressForm.ControlBox = false;
            addressForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addressForm.Width = 410;
            addressForm.Height = 210;
            // Create a label with instructions for the user
            Label instructionLabel = new Label();
            instructionLabel.Text = "Enter an Object Address";
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

            btnObjectImage.PerformClick();
            btnObjectURN.PerformClick();
        }

        public void LoadFormByAddress(string address)
        {

            OBJState foundObject = OBJState.GetObjectByAddress(address, "good-user", "better-password", "http://127.0.0.1:18332");
            if (foundObject.URN != null)
            {
                lblObjectStatus.Text = "created:[" + foundObject.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  locked:[" + foundObject.LockedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  last seen:[" + foundObject.ChangeDate.ToString("MM/dd/yyyy hh:mm:ss") + "]";


                if (lblObjectStatus.Text.Contains("Monday, January 1, 0001"))
                {
                    lblObjectStatus.Text = lblObjectStatus.Text.Replace("Monday, January 1, 0001", " unconfirmed ");
                    NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                    RPCClient rpcClient = new RPCClient(credentials, new Uri("http://127.0.0.1:18332"), Network.Main);
                    string accountName = "";
                    try { accountName = rpcClient.SendCommand("getaccount", address).ResultString; } catch { }
                    if (accountName != "") { btnEdit.Enabled = true; btnMint.Enabled = false; }
                }
                else { btnEdit.Enabled = false; }


                txtTitle.Text = foundObject.Name;
                txtIMG.Text = foundObject.Image;
                txtURN.Text = foundObject.URN;
                txtURI.Text = foundObject.URI;
                txtObjectAddress.Text = address;
                txtDescription.Text = foundObject.Description;
                try { txtMaximum.Text = foundObject.Maximum.ToString(); } catch { txtMaximum.Text = ""; }

                flowAttribute.Controls.Clear();

                try
                { if (foundObject.Attributes != null)
                    {
                        // Iterate through all attributes of foundObject and create a button for each
                        foreach (var attrib in foundObject.Attributes)
                        {
                            // Create a new button with the attribute key and value separated by ':'
                            Button attribButton = new Button();
                            attribButton.ForeColor = Color.White;
                            attribButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));

                            attribButton.AutoSize = true;
                            attribButton.Text = attrib.Key + ":" + attrib.Value;

                            // Add an event handler to the button that removes it from the flowAttribute panel when clicked
                            attribButton.Click += new EventHandler((sender2, e2) =>
                            {
                                flowAttribute.Controls.Remove(attribButton);
                            });

                            // Add the button to the flowAttribute panel
                            flowAttribute.Controls.Add(attribButton);
                        }
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
                    attribButton.ForeColor = Color.White;
                    attribButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));

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


                // Clear any existing buttons from the flowCreators panel
                flowCreators.Controls.Clear();

                // Iterate through all attributes of foundObject and create a button for each
                foreach (KeyValuePair<string, DateTime> attrib in foundObject.Creators)
                {
                    // Create a new button with the attribute key and value separated by ':'
                    Button attribButton = new Button();
                    attribButton.AutoSize = true;
                    attribButton.Text = attrib.Key;
                    attribButton.ForeColor = Color.White;
                    attribButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));

                    // Add an event handler to the button that removes it from the flowCreators panel when clicked
                    attribButton.Click += new EventHandler((sender2, e2) =>
                    {
                        flowCreators.Controls.Remove(attribButton);
                    });

                    // Add an event handler to the button that opens the ObjectBrowser form on right click if the value is not an int
                    attribButton.MouseUp += new MouseEventHandler((sender2, e2) =>
                    {
                        if (e2.Button == MouseButtons.Right && !int.TryParse(attrib.Value.ToString(), out _))
                        {
                            ObjectBrowser form = new ObjectBrowser(attribButton.Text);
                            form.Show();
                        }
                        else if (e2.Button == MouseButtons.Left)
                        {
                            flowCreators.Controls.Remove(attribButton);
                        }
                    });

                    // Add the button to the flowCreators panel
                    flowCreators.Controls.Add(attribButton);
                }

                flowOwners.Controls.Clear();

                try
                {
                    // Iterate through all attributes of foundObject and create a button for each
                    foreach (KeyValuePair<string, long> attrib in foundObject.Owners)
                    {
                        // Split the key and value and create a new button with the attribute key and value separated by ':'
                        string buttonText = attrib.Key + ":" + attrib.Value.ToString();
                        Button attribButton = new Button();
                        attribButton.AutoSize = true;
                        attribButton.Text = buttonText;
                        attribButton.ForeColor = Color.White;
                        attribButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));

                        // Add an event handler to the button that removes it from the flowAttribute panel when clicked
                        attribButton.Click += new EventHandler((sender2, e2) =>
                        {
                            flowOwners.Controls.Remove(attribButton);
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
                                flowOwners.Controls.Remove(attribButton);
                            }
                        });

                        // Add the button to the flowOwners panel
                        flowOwners.Controls.Add(attribButton);
                    }

                }
                catch { }

                if (foundObject.License != null)
                {
                    // If the license field is not null, check the radio button in the panel based on the text passed
                    string licenseText = foundObject.License;

                    // Assuming you have a panel named "paneLicense" and a group of radio buttons inside it
                    RadioButton rb = PanelLicense.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Text == licenseText);

                    if (rb != null)
                    {
                        rb.Checked = true;
                    }
                    else
                    {
                        // If the license text doesn't match any of the radio buttons, do nothing
                    }
                }
                else
                {
                    // If the license field is null, check the radio button in the panel with the text "All Rights Reserved"
                    RadioButton rb = PanelLicense.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Text == "No License / All Rights Reserved");

                    if (rb != null)
                    {
                        rb.Checked = true;
                    }
                    else
                    {
                        // If there is no radio button with the text "All Rights Reserved", do nothing
                    }
                }
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }
    }

}
