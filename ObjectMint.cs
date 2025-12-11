using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using NBitcoin;
using SUP.P2FK;
using System.Collections.Generic;
using System.Threading;
using System.Web.NBitcoin;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using SUP.RPCClient;
using System.Globalization;
using NReco.VideoConverter;
using System.Windows;


namespace SUP
{
    public partial class ObjectMint : Form
    {
        private QrEncoder encoder = new QrEncoder();
        private GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(2, QuietZoneModules.Two));
        private bool ismint = false;
        private string imagecache;
        private Random random = new Random();
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        private bool _testnet = true;
        public ObjectMint(bool testnet = true)
        {
            InitializeComponent();

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
                _testnet = false;
            }
            System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
            myTooltip.SetToolTip(btnObjectName, "enter the object's name. try to keep it short.\nlong object names will be truncated on the object controls.");

            myTooltip.SetToolTip(btnObjectURN, "click to upload your object to IPFS and generate a sup IPFS URL.\nyou may also add prexisting IPFS urls, blockchain urls or http(s) urls\nyour object will appear in the preview box if it is valid.\nclick the button until it turns blue to include it in the mint.\n\nsupported object urls include:\nIPFS:ipfs_hash/filename.mp4\ntransactionid/filename.wav (refrences current mainchain)\nBTC:transactionid/filename.zip (references bitcoin sidechain)\nMZC:transactionid/index.html (references mazacoin sidechain)\nLTC:transactionid/filename.mp3 (references litecoin sidechain)\nDOG:transactionid/filename.pdf (references dogecoin sidechain)\nclick the button until it turns blue to include it in the mint.");
            myTooltip.SetToolTip(btnObjectAddress, "click 💎 to generate a sup compliant object address.\nthis address will be used to hold your object registration.\nyou may also enter an address that already exists in your local wallet.\nclick 💎 and it will be analysed to be sure it's safe to be used for sup object registration");
            myTooltip.SetToolTip(btnObjectImage, "click to upload an object thumbnail image to IPFS and generate a sup IPFS URL.\nif the URN you are registering is already an image, this field is optional.\nyou may also add prexisting IPFS urls, blockchain urls or http(s) urls\nyour thumbnail image will appear in the top left picture box if it is valid.\nclick the button until it turns blue to include it in the mint.\n\nsupported image urls include:\nIPFS:ipfs_hash/filename.jpg\ntransactionid/filename.gif (refrences current mainchain)\nBTC:transactionid/filename.bmp (references bitcoin sidechain)\nMZC:transactionid/filename.jpg (references mazacoin sidechain)\nLTC:transactionid/filename.gif (references litecoin sidechain)\nDOG:transactionid/filename.bmp (references dogecoin sidechain)\nclick the button until it turns blue to include it in the mint.");
            myTooltip.SetToolTip(btnObjectURI, "enter an optional fully qualified link to the tradditional web.\nnote:this provides a clickable link that could open a website or even launch a local script");
            myTooltip.SetToolTip(btnObjectDescription, "enter an optional description of the object\nclick the button until it turns blue to include it in the mint.");
            myTooltip.SetToolTip(btnObjectOwners, "click the button to enter a key value pair describing the initial object owner and qty of the object.\nfor most scenarios you should copy and use the object address generated above and define the qty ( 1 - 999999999 )\ngranting ownership to the object address itself allows a primary - secondary experience for your object collectors.\ncollecting from the object is primary, collecting from a collector is secondary");
            myTooltip.SetToolTip(btnObjectCreators, "click the button to enter signature addresses with object creator privlidges. if no signature is added the object will be self created.\nthe first entered signature represents a collection. second and additional signatures represent the creators.\nnote: creator signatures must be acknowledged for them to be displayed.\nacknowledgment happens when the object is listed using the creator's signature or if the creator sends a update transaction to the object");
            myTooltip.SetToolTip(btnObjectRoyalties, "click to enter optional key value pairs that represent one or more addresses to recieve royalties of all future sales.\nif royalties are defined any sale without royalties included will be rejected as invalid transactions\nnote:do not include the % symbol in royalty amounts example 3.14");
            myTooltip.SetToolTip(btnMint, "click to mint your object. a transaction id will be displayed if it was succesfull.\nobjects are not discoverable until they have at least one confirmation");
            myTooltip.SetToolTip(btnObjectAttributes, "click to enter optional key value pairs representing object attributes\n example ( hair-color | red ) or ( galactica | true ) etc...");
            myTooltip.SetToolTip(btnObjectKeywords, "click to enter one or more optional keywords.\nobjects with keywords defined are more easily discovered in sup. example art, photography, nude etc... ");
            myTooltip.SetToolTip(btnObjectMaximum, "enter an optional maximum qty that can be collected from any one signature address.\nany transactions attempting to collect more then the defined maximum are rejected as invalid.\nclick the button until it turns blue to include it in the mint.");
            myTooltip.SetToolTip(btnScan, "click to search for a object by address.");
            myTooltip.SetToolTip(btnPrint, "click to generate a paper mint of the object which can be claimed by collectors at no cost to the minter via a mobile sup app still in development.");

        }

        private string GetRandomDelimiter()
        {
            string[] delimiters = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            return delimiters[random.Next(delimiters.Length)];
        }


        private void UpdateRemainingChars()
        {

            if (txtURN.Text != "" && txtTitle.Text != "" && txtObjectAddress.Text != "" && flowOwners.Controls.Count > 0)
            {
                if (btnObjectName.BackColor == Color.Blue || btnObjectURN.BackColor == Color.Blue || btnObjectURI.BackColor == Color.Blue || btnObjectOwners.BackColor == Color.Blue || btnObjectCreators.BackColor == Color.Blue || btnObjectAttributes.BackColor == Color.Blue || btnObjectImage.BackColor == Color.Blue || btnObjectMaximum.BackColor == Color.Blue || btnObjectDescription.BackColor == Color.Blue || btnObjectKeywords.BackColor == Color.Blue || btnObjectRoyalties.BackColor == Color.Blue) { btnMint.Enabled = true; btnPrint.Enabled = true; }
            }


            int maxsize = 8400;

            maxsize = maxsize - txtDescription.Text.Length - txtIMG.Text.Length - txtTitle.Text.Length - txtURI.Text.Length - txtURN.Text.Length - txtMaximum.Text.Length;
            maxsize = maxsize - 40; ///estimated json chars required.

            foreach (System.Windows.Forms.Control control in flowAttribute.Controls)
            {
                maxsize = maxsize - (control.Text.Length + 5);
            }

            maxsize = maxsize - (flowKeywords.Controls.Count * 25);


            foreach (System.Windows.Forms.Control control in flowRoyalties.Controls)
            {

                maxsize = maxsize - (control.Text.Length + 5);


            }

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

            maxsize = maxsize - (flowCreators.Controls.Count * 40) + 5;

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
            if (btnObjectMaximum.BackColor == Color.Blue) { if (txtMaximum.Text == "") { } else { try { OBJJson.max = long.Parse(txtMaximum.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")); } catch { OBJJson.max = 0; } } }

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
                    try { mintOwners.Add(parts[0], long.Parse(parts[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"))); } catch { }
                }
                OBJJson.own = mintOwners;
            }

            if (btnObjectRoyalties.BackColor == Color.Blue)
            {
                Dictionary<string, decimal> mintRoyalties = new Dictionary<string, decimal>();
                foreach (Button royaltyControl in flowRoyalties.Controls)
                {
                    string[] parts = royaltyControl.Text.Split(':');
                    try { mintRoyalties.Add(parts[0], decimal.Parse(parts[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"))); } catch { }
                }
                OBJJson.roy = mintRoyalties;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            // Serialize the modified JObject back into a JSON string
            var objectSerialized = JsonConvert.SerializeObject(OBJJson, Formatting.None, settings);

            txtOBJJSON.Text = objectSerialized.Replace(",\"max\":0", "").Replace(",\"atr\":{}", "").Replace(",\"own\":{}", "").Replace(",\"roy\":{}", "").Replace(",\"uri\":\"\"", "").Replace(",\"dsc\":\"\"", "").Replace(",\"img\":\"\"", "").Replace(",\"lic\":\"\"", "");

            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(txtOBJJSON.Text);

            int lengthInBytes = utf8Bytes.Length;

            string objString = "OBJ" + GetRandomDelimiter() + lengthInBytes + GetRandomDelimiter() + txtOBJJSON.Text;

            // Assign the result to txtOBJP2FK.Text
            txtOBJP2FK.Text = objString;

            NetworkCredential credentials = new NetworkCredential(mainnetLogin, mainnetPassword);
            NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);
            System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
            byte[] hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(txtOBJP2FK.Text));


            string signatureAddress;

            if (flowCreators.Controls.Count > 0)
            { signatureAddress = flowCreators.Controls[0].Text; }
            else { signatureAddress = txtObjectAddress.Text; }

            if (txtURN.Text.Length > 63)
            {
                try
                {
                    Root ROOT = Root.GetRootByTransactionId(txtURN.Text.Substring(0, 64), mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                    if (ROOT.Signed == true && ROOT.SignedBy != signatureAddress) { System.Windows.Forms.MessageBox.Show("sorry, the object urn " + txtURN.Text + " can only be claimed by " + ROOT.SignedBy); return; }
                }
                catch (Exception ex)
                {
                    lblObjectStatus.Text = ex.Message;
                    return;
                }
            }

            string signature = "";
            try { signature = rpcClient.SendCommand("signmessage", signatureAddress, BitConverter.ToString(hashValue).Replace("-", String.Empty)).ResultString; }
            catch (Exception ex)
            {
                lblObjectStatus.Text = ex.Message;
                return;
            }

            txtOBJP2FK.Text = "SIG" + GetRandomDelimiter() + "88" + GetRandomDelimiter() + signature + txtOBJP2FK.Text;

            List<string> encodedList = new List<string>();
            byte[] inputBytes = Encoding.UTF8.GetBytes(txtOBJP2FK.Text); // Convert the string to bytes

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
                    new byte[] { (byte)Int32.Parse(mainnetVersionByte, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) }.Concat(chunkBytes).ToArray());

                if (!encodedList.Contains(chunkBase58))
                {
                    encodedList.Add(chunkBase58);
                }
                else
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show("The following duplicate information was detected: [  " + chunkBase58 + "  ]. Sorry, you must still use Apertus.io for etchings that require repetitive data", "Confirmation", MessageBoxButtons.OK);
                }
            }




            //add URN registration
            encodedList.Add(Root.GetPublicAddressByKeyword(txtURN.Text, mainnetVersionByte));


            if (txtURN.Text != "" && ismint)
            {
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
                    webviewer.Dispose();
                    using (FileStream fileStream = new FileStream(fileSource, FileMode.Open))
                    {
                        using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
                        {
                            byte[] hash = sha256.ComputeHash(fileStream);
                            Array.Copy(hash, payload, 20);
                        }
                    }

                    if (_testnet)
                    {
                        payload[0] = 0x6F; // 0x6F is the hexadecimal representation of 111
                    }
                    else
                    {
                        payload[0] = 0x00; // Hexadecimal representation of 0
                    }

                    string objectaddress = Base58.EncodeWithCheckSum(payload);

                    encodedList.Add(objectaddress);

                }
                catch (Exception ex) { lblObjectStatus.Text = ex.Message; }

            }

            foreach (Button keywordbtn in flowKeywords.Controls)
            {
                encodedList.Add(Root.GetPublicAddressByKeyword(keywordbtn.Text, mainnetVersionByte));
            }

            foreach (Button royaltybtn in flowRoyalties.Controls)
            {
                if (royaltybtn.Text.Split(':')[0] != signatureAddress)
                {
                    encodedList.Add(royaltybtn.Text.Split(':')[0]);
                }
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

            while (encodedList.Contains(txtObjectAddress.Text))
            {
                encodedList.Remove(txtObjectAddress.Text);
            }

            // Assuming signatureAddress is a string
            while (encodedList.Contains(signatureAddress))
            {
                encodedList.Remove(signatureAddress);
            }

            encodedList.Add(txtObjectAddress.Text);
            encodedList.Add(signatureAddress);


            txtAddressListJSON.Text = JsonConvert.SerializeObject(encodedList.Distinct());

            lblCost.Text = "cost: " + (0.00000546 * encodedList.Count).ToString("0.00000000") + "  + miner fee";

            if (ismint)
            {
              

                DialogResult result = System.Windows.Forms.MessageBox.Show("Are you sure you want to mint this object?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {


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
                        lblTransactionID.Text = results;
                    }
                    catch (Exception ex) { lblObjectStatus.Text = ex.Message; }



                }
                ismint = false;
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


            if (txtIMG.Text != "")
            {
                btnObjectImage.BackColor = Color.Blue;
                btnObjectImage.ForeColor = Color.Yellow;
            }
            else
            {
                btnObjectImage.BackColor = Color.White;
                btnObjectImage.ForeColor = Color.Black;
            }
        }

        private void txtURN_TextChanged(object sender, EventArgs e)
        {
            UpdateRemainingChars();

            if (txtURN.Text != "")
            {
                if (flowOwners.Controls.Count < 1)
                {

                    lblASCIIURN.Text = "Click OWN to Add 1 or more owners with qty defined\n\ncopy and paste the 💎 address in most scenarios";
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
                btnObjectRoyalties.Enabled = false;
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
                    btnObjectRoyalties.Enabled = true;
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
                btnObjectRoyalties.Enabled = true;
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
                lblASCIIURN.Text = "push 💎 to obtain a new object address";
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
                btnObjectRoyalties.Enabled = false;
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

                string P2FKASCII = Root.GetKeywordByPublicAddress(txtObjectAddress.Text, "ASCII");
                char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
                // Check for presence of special character followed by a number using regular expression
                string pattern = "[" + Regex.Escape(new string(specialChars)) + "][0-9]";
                if (Regex.IsMatch(P2FKASCII, pattern))
                {
                    System.Windows.Forms.MessageBox.Show("Sup!? Found special characters within this address that could corrupt P2FK transactions.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                LoadFormByAddress(txtObjectAddress.Text);

            }
            else
            {

                NetworkCredential credentials = new NetworkCredential(mainnetLogin, mainnetPassword);
                NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);
                string newAddress = "";
                string P2FKASCII = "";
                char[] specialChars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
                int attempt = 0;
                while (true)
                {
                    try
                    {
                        newAddress = rpcClient.SendCommand("getnewaddress", txtTitle.Text + "!" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "!" + attempt.ToString()).ResultString;
                        P2FKASCII = Root.GetKeywordByPublicAddress(newAddress, "ASCII");
                        string pattern = "[" + Regex.Escape(new string(specialChars)) + "][0-9]";
                        if (!Regex.IsMatch(P2FKASCII, pattern))
                        {

                            break;
                        }
                        attempt++;
                    }
                    catch
                    {

                    }
                }

                txtObjectAddress.Text = newAddress;
                txtTitle.Enabled = false;
                btnObjectName.Enabled = false;
                lblObjectStatus.Text = "";
                lblCost.Text = "";

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
                        if (long.TryParse(txtMaximum.Text.Replace(",", ""), NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out long dummy)) { btnObjectMaximum.BackColor = Color.Blue; btnObjectMaximum.ForeColor = Color.Yellow; } else { btnObjectMaximum.BackColor = Color.White; btnObjectMaximum.ForeColor = Color.Black; }

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

                        lblIMGBlockDate.Text = "[ uploading to IPFS please wait...]";
                        System.Windows.Forms.MessageBox.Show("Uploading a file to IPFS could take a long time. to prevent any issues, Sup!? will lock while it's loading.  just wait for it.");
                        
                        try
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = @"ipfs\ipfs.exe";
                            process.StartInfo.Arguments = "add \"" + filePath + "\"";
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.UseShellExecute = false;
                            process.Start();
                            string output = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();
                            
                            // Parse IPFS output - Expected format: "added <hash> <filename>"
                            // Example: "added QmYwAPJzv5CZsnA625s3Xf2nemtYgPpHdWEz79ojWnPbdG test.jpg"
                            if (string.IsNullOrWhiteSpace(output))
                            {
                                lblIMGBlockDate.Text = "[ upload failed ]";
                                System.Windows.Forms.MessageBox.Show("IPFS upload failed. No output received from IPFS.", "IPFS Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            
                            string[] outputParts = output.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (outputParts.Length < 2)
                            {
                                lblIMGBlockDate.Text = "[ upload failed ]";
                                System.Windows.Forms.MessageBox.Show("IPFS upload failed. Invalid output format: " + output, "IPFS Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            
                            string hash = outputParts[1];
                            txtIMG.Text = "IPFS:" + hash + @"\" + fileName;
                            imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + hash + @"\" + fileName;
                        }
                        catch (Exception ex)
                        {
                            lblIMGBlockDate.Text = "[ upload failed ]";
                            System.Windows.Forms.MessageBox.Show("Error uploading file to IPFS: " + ex.Message, "IPFS Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        // User cancelled the file dialog
                        return;
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
                                if (!System.IO.Directory.Exists(@"ipfs/" + txtIMG.Text.Substring(5, 46) + "-build") && !System.IO.Directory.Exists(@"ipfs/" + txtIMG.Text.Substring(5, 46)))
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

                                    if (File.Exists(imgurn))
                                    {
                                        lblIMGBlockDate.Text = "ipfs verified: " + System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                        btnObjectImage.BackColor = Color.Blue;
                                        btnObjectImage.ForeColor = Color.Yellow;
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
                                                    Arguments = "pin add " + txtIMG.Text.Substring(5, 46),
                                                    UseShellExecute = false,
                                                    CreateNoWindow = true
                                                }
                                            };
                                            process3.Start();
                                        }
                                    }
                                    catch { }


                                    try { Directory.Delete(@"ipfs/" + txtIMG.Text.Substring(5, 46)); } catch { }
                                    try
                                    {
                                        Directory.Delete(@"ipfs/" + txtIMG.Text.Substring(5, 46) + "-build");
                                    }
                                    catch { }



                                }
                                else
                                {
                                    lblIMGBlockDate.Text = "ipfs verified: " + System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    btnObjectImage.BackColor = Color.Blue;
                                    btnObjectImage.ForeColor = Color.Yellow;
                                }
                                break;
                            default:

                                root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                                try
                                {
                                    if (_testnet)
                                    {
                                        lblIMGBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    }
                                    else
                                    {
                                        lblIMGBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    }

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
                    OBJState OBJ = OBJState.GetObjectByURN(txtURN.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                    if (OBJ.Creators != null) {
                        System.Windows.Forms.MessageBox.Show("WARNING: The object URN " + txtURN.Text + " has already been claimed.");
                        btnObjectURN.BackColor = Color.Blue;
                        btnObjectURN.ForeColor = Color.Yellow; }

                    urn = txtURN.Text;
                    if (!txtURN.Text.ToLower().StartsWith("http"))
                    {
                        urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + txtURN.Text.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");
                        if (txtURN.Text.ToLower().StartsWith("ipfs:")) { urn = urn.Replace(@"\root\", @"\ipfs\"); }
                    }
                    else
                    {
                        try
                        {
                            webviewer.Visible = true;
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(txtURN.Text);
                            lblURNBlockDate.Text = "http get: " + DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                            btnObjectURN.BackColor = Color.Blue;
                            btnObjectURN.ForeColor = Color.Yellow;
                        }
                        catch { }
                        return;
                    }
                }
                else
                {
                    // Open file dialog and get file path and name
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Title = "Select File";
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog1.FileName;
                        string fileName = openFileDialog1.SafeFileName;
                        lblURNBlockDate.Text = "[ uploading to IPFS please wait...]";
                        System.Windows.Forms.MessageBox.Show("Uploading a file to IPFS could take a long time. to prevent any issues, Sup!? will lock while it's loading.  just wait for it.");
                        
                        try
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = @"ipfs\ipfs.exe";
                            process.StartInfo.Arguments = "add \"" + filePath + "\"";
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.UseShellExecute = false;
                            process.Start();
                            string output = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();
                            
                            // Parse IPFS output - Expected format: "added <hash> <filename>"
                            // Example: "added QmYwAPJzv5CZsnA625s3Xf2nemtYgPpHdWEz79ojWnPbdG test.jpg"
                            if (string.IsNullOrWhiteSpace(output))
                            {
                                lblURNBlockDate.Text = "[ upload failed ]";
                                System.Windows.Forms.MessageBox.Show("IPFS upload failed. No output received from IPFS.", "IPFS Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            
                            string[] outputParts = output.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (outputParts.Length < 2)
                            {
                                lblURNBlockDate.Text = "[ upload failed ]";
                                System.Windows.Forms.MessageBox.Show("IPFS upload failed. Invalid output format: " + output, "IPFS Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            
                            string hash = outputParts[1];
                            txtURN.Text = "IPFS:" + hash + @"\" + fileName;

                            OBJState OBJ = OBJState.GetObjectByURN(txtURN.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                            if (OBJ.Creators != null) { System.Windows.Forms.MessageBox.Show("Sorry, the object urn " + txtURN.Text + " has already been claimed."); lblURNBlockDate.Text = ""; txtURN.Text = ""; return; }
                            urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + hash + @"\" + fileName;
                            
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
                        }
                        catch (Exception ex)
                        {
                            lblURNBlockDate.Text = "[ upload failed ]";
                            System.Windows.Forms.MessageBox.Show("Error uploading file to IPFS: " + ex.Message, "IPFS Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        // User cancelled the file dialog
                        return;
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
                                    try { Directory.CreateDirectory(@"ipfs/" + txtURN.Text.Substring(5, 46)); } catch { }
                                    try { System.IO.File.Move("ipfs/" + txtURN.Text.Substring(5, 46) + "_tmp", urn); } catch { }
                                }

                                if (File.Exists(urn))
                                {
                                    lblURNBlockDate.Text = "ipfs verified: " + System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                    btnObjectURN.BackColor = Color.Blue;
                                    btnObjectURN.ForeColor = Color.Yellow;
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
                                                Arguments = "pin add " + txtURN.Text.Substring(5, 46),
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                    }
                                }
                                catch { }

                                try { Directory.Delete(@"ipfs/" + txtURN.Text.Substring(5, 46)); } catch { }
                                try
                                {
                                    Directory.Delete(@"ipfs/" + txtURN.Text.Substring(5, 46) + "-build");
                                }
                                catch { }




                            }
                            else
                            {
                                lblURNBlockDate.Text = "ipfs verified: " + System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                btnObjectURN.BackColor = Color.Blue;
                                btnObjectURN.ForeColor = Color.Yellow;
                            }
                            break;
                        default:

                            root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                            if (transactionid != "")
                            {
                                if (Directory.Exists(@"root\" + transactionid))
                                {
                                    try
                                    {
                                        if (_testnet)
                                        {
                                            lblURNBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                        }
                                        else
                                        {
                                            lblURNBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                        }
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

                        break;

                    case ".glb":
                    case ".fbx":
                        //Show image in main box and show open file button
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

                        if (File.Exists(urn))
                        {

                            pictureBox1.ImageLocation = urn;
                            if (txtIMG.Text == "") { pictureBox2.ImageLocation = urn; }
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
                                            Root.GetRootByTransactionId(transactionID.Value, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
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
                    case ".mov":
                    case ".avi":
                    case ".mp3":
                    case ".wav":
                    case ".pdf":


                        if (extension.ToLower() == ".mov")
                        {
                            string inputFilePath = urn;
                            string outputFilePath = System.IO.Path.ChangeExtension(inputFilePath, ".mp4");
                            urn = outputFilePath;
                        }

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

                        if (File.Exists(urn))
                        {

                            pictureBox1.ImageLocation = pictureBox2.ImageLocation;
                        }
                        else
                        {
                            if (extension == "") {

                                Match urnmatch = regexTransactionId.Match(txtURN.Text);
                                string transactionid = urnmatch.Value;

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
                                    string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\keywords\" + hashedString + ".png";
                                    pictureBox1.ImageLocation = filePath;
                                    if (txtIMG.Text == "") { pictureBox2.ImageLocation = filePath; }

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

                        break;
                }
            }
            UpdateRemainingChars();

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
                dialog.ClientSize = new System.Drawing.Size(400, 80);

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
                keyTextBox.Size = new System.Drawing.Size(170, 70);

                var valueTextBox = new TextBox();
                valueTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                valueTextBox.Size = new System.Drawing.Size(170, 70);
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
                dialog.ClientSize = new System.Drawing.Size(400, 80);

                var tableLayout = new TableLayoutPanel();
                tableLayout.ColumnCount = 1;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                var keyLabel = new Label();
                keyLabel.Text = "Keyword";
                keyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var keyTextBox = new TextBox();
                keyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                keyTextBox.Multiline = true;
                keyTextBox.Size = new System.Drawing.Size(340, 70);
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
                dialog.ClientSize = new System.Drawing.Size(400, 80);

                var tableLayout = new TableLayoutPanel();
                tableLayout.ColumnCount = 1;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                var keyLabel = new Label();
                keyLabel.Text = "Creator";
                keyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var keyTextBox = new TextBox();
                keyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                keyTextBox.Multiline = true;
                keyTextBox.Size = new System.Drawing.Size(340, 70);
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
                dialog.ClientSize = new System.Drawing.Size(400, 80);

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
                ownerTextBox.Size = new System.Drawing.Size(170, 70);

                var qtyTextBox = new TextBox();
                qtyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                qtyTextBox.Size = new System.Drawing.Size(170, 70);
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

                    var isNumeric = long.TryParse(qty, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out long OWNQty);

                    if (isNumeric && OWNQty > 0)
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
                        System.Windows.Forms.MessageBox.Show("Qty field only accepts numeric input.");
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

        private void PercentTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.Equals(e.KeyChar, '.'))
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
        private void flowRoyalties_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (flowRoyalties.Controls.Count < 1)
            {
                btnObjectRoyalties.BackColor = Color.White;
                btnObjectRoyalties.ForeColor = Color.Black;
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
                if (!long.TryParse(txtMaximum.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out long value)) { txtMaximum.Text = "0"; }
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
           
            OBJState OBJ = OBJState.GetObjectByAddress(txtObjectAddress.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
            if (OBJ.URN == null && btnObjectURN.BackColor == Color.White) { System.Windows.Forms.MessageBox.Show("WARNING: The object URN is a required field. Verify the URN button is blue before minting."); lblURNBlockDate.Text = ""; return;  }
            if (OBJ.URN == null && flowOwners.Controls.Count == 0) { System.Windows.Forms.MessageBox.Show("WARNING: At least one Owner must be defined. Click the OWN button to add one. For a primary / secondary collection experience, Allocate all units to the object address itself."); return;  }
           
            ismint = true;
            UpdateRemainingChars();

            OBJ = OBJState.GetObjectByAddress(txtObjectAddress.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            if (OBJ.URN != null)
            {
                lblObjectStatus.Text = "created:[" + OBJ.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  locked:[" + OBJ.LockedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  last seen:[" + OBJ.ChangeDate.ToString("MM/dd/yyyy hh:mm:ss") + "]";
                lblObjectStatus.Text = lblObjectStatus.Text.Replace("Monday, January 1, 0001", " unconfirmed ");

            }
            else
            {
                try { Directory.Delete(@"root\" + txtObjectAddress.Text, true); } catch { }
            }
        }

        private async void CreateObjectQRCode()
        {
            UpdateRemainingChars();
            imagecache = pictureBox1.ImageLocation;
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
            instructionLabel.Location = new System.Drawing.Point(20, 20);
            addressForm.Controls.Add(instructionLabel);

            // Create a text box for the user to input the address
            TextBox addressTextBox = new TextBox();
            addressTextBox.Location = new System.Drawing.Point(20, 70);
            addressTextBox.Width = 300;
            addressTextBox.Height = 50;
            addressTextBox.Multiline = true;
            addressForm.Controls.Add(addressTextBox);

            // Create a button for the user to click to search for the address
            Button searchButton = new Button();
            searchButton.Text = "Search";
            searchButton.Location = new System.Drawing.Point(20, 120);
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


        }

        public void LoadFormByAddress(string address)
        {

            OBJState foundObject = OBJState.GetObjectByAddress(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
            if (foundObject.URN != null)
            {
                lblObjectStatus.Text = "created:[" + foundObject.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  locked:[" + foundObject.LockedDate.ToString("MM/dd/yyyy hh:mm:ss") + "]  last seen:[" + foundObject.ChangeDate.ToString("MM/dd/yyyy hh:mm:ss") + "]";

                NetworkCredential credentials = new NetworkCredential(mainnetLogin, mainnetPassword);
                NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);
                string accountName = "";
                try { accountName = rpcClient.SendCommand("getaccount", address).ResultString; } catch { }
                if (accountName != "") { btnMint.Enabled = true; }


                if (lblObjectStatus.Text.Contains("Monday, January 1, 0001"))
                {
                    lblObjectStatus.Text = lblObjectStatus.Text.Replace("Monday, January 1, 0001", " unconfirmed ");
                    btnMint.Enabled = false;
                }

                txtObjectAddress.Text = address;
                txtTitle.Text = foundObject.Name;
                txtIMG.Text = foundObject.Image;
                txtURN.Text = foundObject.URN;
                txtURI.Text = foundObject.URI;
                txtDescription.Text = foundObject.Description;
                try { txtMaximum.Text = foundObject.Maximum.ToString(); } catch { txtMaximum.Text = ""; }

                flowAttribute.Controls.Clear();

                try
                {
                    if (foundObject.Attributes != null)
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
                keywords = OBJState.GetKeywordsByAddress(address, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

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
                        if (e2.Button == MouseButtons.Right && !int.TryParse(attrib.Value.ToString(), NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out _))
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
                    // Iterate through all attributes of foundObject and create a button for each
                    foreach (var kvp in foundObject.Owners)
                    {
                        // Extract key and value from the tuple
                        string key = kvp.Key;
                        long value = kvp.Value.Item1; // Assuming the first item in the tuple represents the quantity

                        // Create a new button with the attribute key and value separated by ':'
                        string buttonText = $"{key}:{value}";
                        Button attribButton = new Button
                        {
                            AutoSize = true,
                            Text = buttonText,
                            ForeColor = Color.White,
                            BackColor = Color.FromArgb(64, 64, 64)
                        };

                        // Add an event handler to the button that removes it from the flowOwners panel when clicked
                        attribButton.Click += (sender2, e2) =>
                        {
                            flowOwners.Controls.Remove(attribButton);
                        };

                        // Add an event handler to the button that opens the ObjectBrowser form on right click using the key value
                        attribButton.MouseUp += (sender2, e2) =>
                        {
                            if (e2.Button == MouseButtons.Right)
                            {
                                ObjectBrowser form = new ObjectBrowser(key);
                                form.Show();
                            }
                            else if (e2.Button == MouseButtons.Left)
                            {
                                flowOwners.Controls.Remove(attribButton);
                            }
                        };

                        // Add the button to the flowOwners panel
                        flowOwners.Controls.Add(attribButton);
                    }


                }
                catch { }

                flowRoyalties.Controls.Clear();

                try
                {
                    // Iterate through all attributes of foundObject and create a button for each
                    foreach (KeyValuePair<string, decimal> royalty in foundObject.Royalties)
                    {
                        // Split the key and value and create a new button with the attribute key and value separated by ':'
                        string buttonText = royalty.Key + ":" + royalty.Value.ToString();
                        Button royalButton = new Button();
                        royalButton.AutoSize = true;
                        royalButton.Text = buttonText;
                        royalButton.ForeColor = Color.White;
                        royalButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));

                        // Add an event handler to the button that removes it from the flowAttribute panel when clicked
                        royalButton.Click += new EventHandler((sender2, e2) =>
                        {
                            flowRoyalties.Controls.Remove(royalButton);
                        });

                        // Add an event handler to the button that opens the ObjectBrowser form on right click using the key value
                        royalButton.MouseUp += new MouseEventHandler((sender2, e2) =>
                        {
                            if (e2.Button == MouseButtons.Right)
                            {
                                string[] keyValuePair = royalButton.Text.Split(':');
                                string key = keyValuePair[0].Trim();
                                ObjectBrowser form = new ObjectBrowser(key);
                                form.Show();
                            }
                            else if (e2.Button == MouseButtons.Left)
                            {
                                flowRoyalties.Controls.Remove(royalButton);
                            }
                        });

                        // Add the button to the flowOwners panel
                        flowRoyalties.Controls.Add(royalButton);
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
            else
            {
                try { Directory.Delete(@"root\" + txtObjectAddress.Text, true); } catch { }
            }


            btnObjectImage.BackColor = Color.White;
            btnObjectImage.PerformClick();
            btnObjectURN.BackColor = Color.White;
            btnObjectURN.PerformClick();


        }

        private void btnObjectRoyalties_Click(object sender, EventArgs e)
        {

            using (var dialog = new Form())
            {
                dialog.Text = String.Empty;
                dialog.AutoSize = true;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ControlBox = false;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new System.Drawing.Size(400, 80);

                var tableLayout = new TableLayoutPanel();
                tableLayout.ColumnCount = 2;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

                var royaltyLabel = new Label();
                royaltyLabel.Text = "Recipient";
                royaltyLabel.TextAlign = ContentAlignment.MiddleCenter;

                var decLabel = new Label();
                decLabel.Text = "Percent";
                decLabel.TextAlign = ContentAlignment.MiddleCenter;

                var ownerTextBox = new TextBox();
                ownerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                ownerTextBox.Multiline = true;
                ownerTextBox.Size = new System.Drawing.Size(170, 70);

                var decTextBox = new TextBox();
                decTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                decTextBox.Size = new System.Drawing.Size(170, 70);
                decTextBox.Multiline = true;
                decTextBox.KeyPress += new KeyPressEventHandler(PercentTextBox_KeyPress);

                tableLayout.Controls.Add(royaltyLabel, 0, 0);
                tableLayout.Controls.Add(decLabel, 1, 0);
                tableLayout.Controls.Add(ownerTextBox, 0, 1);
                tableLayout.Controls.Add(decTextBox, 1, 1);

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
                    var royalty = ownerTextBox.Text;
                    var dec = decTextBox.Text;

                    var isNumeric = decimal.TryParse(dec, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out _);

                    if (isNumeric)
                    {
                        var button = new Button();
                        button.ForeColor = Color.White;
                        button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                        button.Text = $"{royalty}: {dec}";
                        button.AutoSize = true;
                        button.MouseClick += (s, ev) =>
                        {
                            // only proceed if left mouse button is clicked
                            if (ev.Button == MouseButtons.Left)
                            {
                                // disable the button to prevent double-clicks
                                button.Enabled = false;

                                // remove the button from the flow layout panel and dispose of it
                                flowRoyalties.Controls.Remove(button);
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



                        flowRoyalties.Controls.Add(button);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Percent field only accepts numeric input.");
                    }
                }
            }
            UpdateRemainingChars();
        }

        private void flowRoyalties_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateRemainingChars();

            btnObjectRoyalties.BackColor = Color.Blue;
            btnObjectRoyalties.ForeColor = Color.Yellow;
        }

        private void PrintMenuItem_Click(object sender, EventArgs e)
        {
            lblRemainingChars.Visible = false;
            pictureBox1.Refresh();
            System.Drawing.Bitmap bitmap = new Bitmap(this.Width, this.Height - 44);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(this.PointToScreen(new System.Drawing.Point(0, 0)), new System.Drawing.Point(0, 0), this.Size);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            PrintImage(bitmap);
            lblRemainingChars.Visible = true;
        }

        private void HideMenuItem_Click(object sender, EventArgs e)
        {

            pictureBox1.ImageLocation = imagecache;

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
                        System.Windows.Forms.MessageBox.Show("Error saving the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No image location specified.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            CreateObjectQRCode();
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            // Cast the sender to a TextBox to access the specific textbox
            if (sender is TextBox textBox)
            {
                // Set the SelectionStart to 0
                textBox.SelectionStart = 0;
                textBox.SelectionLength = 0;

                // Scroll to the beginning of the text
                textBox.ScrollToCaret();
            }

           
        }

        private void txtURN_Leave(object sender, EventArgs e)
        {
            OBJState OBJ = OBJState.GetObjectByURN(txtURN.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
            if (OBJ.Creators != null) { System.Windows.Forms.MessageBox.Show("WARNING: The object URN " + txtURN.Text + " has already been claimed."); lblURNBlockDate.Text = "";} else
            {
                txtURN.SelectionStart = 0;
                txtURN.SelectionLength = 0;
                txtURN.ScrollToCaret();
            }
    
        }
    }

}
