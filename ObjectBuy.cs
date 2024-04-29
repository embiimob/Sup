using System;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using SUP.RPCClient;
using NBitcoin;
using Newtonsoft.Json;
using SUP.P2FK;
using AngleSharp.Common;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Linq.Expressions;

namespace SUP
{
    public partial class ObjectBuy : Form
    {
        //GPT3 ROCKS
        private readonly static object SupLocker = new object();
        private List<string> BTCTMemPool = new List<string>();
        private Dictionary<string, long> pendingBUY = new Dictionary<string, long>();

        bool mint = false;
        long maxHold = 0;
        private readonly string givaddress = "";
        private Random random = new Random();
        private string _activeprofile;
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";

        public ObjectBuy(string _address = "", string activeprofile = "", bool testnet = true)
        {
            InitializeComponent();
            givaddress = _address;
            _activeprofile = activeprofile;
            if (!testnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";
            }
        }

        private string GetRandomDelimiter()
        {
            string[] delimiters = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };

            return delimiters[random.Next(delimiters.Length)];
        }

        void Owner_LinkClicked(string ownerId)
        {

            new ObjectBrowser(ownerId).Show();
        }

        //GPT3 THANKS!
        private void RemoveRowByTransactionId(FlowLayoutPanel flowLayoutPanel, string transactionId)
        {
            // Get all TableLayoutPanel controls with the specified transaction ID tag
            var tableLayoutPanels = flowLayoutPanel.Controls.OfType<TableLayoutPanel>()
                .Where(panel => panel.Tag != null && panel.Tag.ToString() == transactionId)
                .ToList();

            // Remove the TableLayoutPanel controls from the FlowLayoutPanel
            foreach (var panel in tableLayoutPanels)
            {
                flowLayoutPanel.Controls.Remove(panel);
                panel.Dispose(); // Optional: Dispose of the removed control to free up resources
            }

            //remove pending BUY transactions
            Root root = Root.GetRootByTransactionId(transactionId, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
            switch (root.File.Last().Key.ToString().Substring(root.File.Last().Key.ToString().Length - 3))
            {
                case "BUY":


                    List<List<string>> buyinspector = new List<List<string>> { };
                    try
                    {
                        buyinspector = JsonConvert.DeserializeObject<List<List<string>>>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\BUY"));
                    }
                    catch
                    {

                        break;
                    }

                    if (buyinspector == null)
                    {
                        break;
                    }

                    foreach (var buy in buyinspector)
                    {
                        string _from = root.SignedBy;
                        string _to = buy[0];
                        string _message = "BUY 💰 " + buy[1];
                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                        string imglocation = "";

                        if (long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) < 0)
                        {
                            break;
                        }


                        this.Invoke((MethodInvoker)delegate
                        {

                            if (pendingBUY.ContainsKey(buy[0]))
                            {
                                pendingBUY[buy[0]] = pendingBUY[buy[0]] - long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
                            }

                        });


                    }
                    break;

                default:

                    break;
            }

        }



        void CreateFeedRow(string imageLocation, string SentTo, string SentFrom, DateTime timestamp, string messageText, string transactionid, System.Drawing.Color bgcolor, FlowLayoutPanel layoutPanel, bool addtoTop = false)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 5,
                AutoSize = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Tag = transactionid

            };
            // Add the width of the first column to fixed value and second to fill remaining space
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));

            if (addtoTop)
            {
                layoutPanel.Controls.Add(row);
                layoutPanel.Controls.SetChildIndex(row, 0);
            }
            else
            {
                layoutPanel.Controls.Add(row);
            }



            // Create a PictureBox with the specified image

            if (System.IO.File.Exists(imageLocation) || imageLocation.ToUpper().StartsWith("HTTP"))
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




            PROState profile = PROState.GetProfileByAddress(SentFrom, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            if (profile.URN != null)
            {
                SentFrom = profile.URN;
            }

            profile = PROState.GetProfileByAddress(SentTo, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            if (profile.URN != null)
            {
                SentTo = profile.URN;
            }

            OBJState isobject = OBJState.GetObjectByAddress(txtAddressSearch.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

            if (givaddress == SentFrom) { SentFrom = "primary"; }
            if (givaddress == SentTo) { SentTo = "primary"; }



            // Create a LinkLabel with the owner name
            LinkLabel sentfrom = new LinkLabel
            {
                Text = SentFrom,
                BackColor = Color.Black,
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Bottom

            };
            if (SentFrom != "primary") { sentfrom.LinkClicked += (sender, e) => { Owner_LinkClicked(SentFrom); }; }
            sentfrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            sentfrom.Margin = new System.Windows.Forms.Padding(3);
            row.Controls.Add(sentfrom, 1, 0);

            // Create a LinkLabel with the owner name
            LinkLabel sentto = new LinkLabel
            {
                Text = SentTo,
                BackColor = Color.Black,
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Bottom

            };
            if (SentTo != "primary")
            {
                sentto.LinkClicked += (sender, e) => { Owner_LinkClicked(SentTo); };
            }
            sentto.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            sentto.Margin = new System.Windows.Forms.Padding(3);
            row.Controls.Add(sentto, 2, 0);

            Label message = new Label
            {
                AutoSize = true,
                Text = messageText,
                MinimumSize = new Size(130, 46),
                Font = new System.Drawing.Font("Segoe UI", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Margin = new System.Windows.Forms.Padding(0),
                Padding = new System.Windows.Forms.Padding(0),
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.BottomLeft

            };
            row.Controls.Add(message, 3, 0);






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
            deleteme.Click += (sender, e) => { RemoveRowByTransactionId(flowInMemoryResults, transactionid); };
            row.Controls.Add(deleteme, 4, 0);


        }

        private void ObjectGive_Load(object sender, EventArgs e)
        {
            txtAddressSearch.Text = givaddress;
            txtSignatureAddress.Text = _activeprofile;
            this.Text = "[ " + txtAddressSearch.Text + " ]";
            RefreshPage();
            tmrSearchMemoryPool.Enabled = true;

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


        private void RefreshPage()
        {

            OBJState objstate = OBJState.GetObjectByAddress(txtAddressSearch.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
            Dictionary<string, string> profileAddress = new Dictionary<string, string> { };

            profileAddress.Add(givaddress, "primary");

            txtName.Text = objstate.Name;
            lblLicense.Text = objstate.License;
            lblObjectCreatedDate.Text = objstate.CreatedDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
            lblTotalOwnedDetail.Text = "total: " + objstate.Owners.Values.Sum(tuple => tuple.Item1).ToString("N0");
            lblTotalRoyaltiesDetail.Text = "royalties: " + objstate.Royalties.Values.Sum().ToString();

            if (objstate.Maximum != null && objstate.Maximum > 0)
            {
                lblMAXqty.Text = "MAX: " + objstate.Maximum.ToString("N0");
                maxHold = objstate.Maximum;
            }

            if (objstate.Image == null)
            {
                // Check to see if objstate.URN has an image extension
                string[] validImageExtensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".ico", ".tiff", ".wmf", ".emf" }; // Add more if needed

                bool hasValidImageExtension = validImageExtensions.Any(extension =>
                    objstate.URN.EndsWith(extension, StringComparison.OrdinalIgnoreCase));

                if (hasValidImageExtension)
                {
                    objstate.Image = objstate.URN;
                }
                else { objstate.Image = ""; }
            }

            string imagelocation = "";

            if (objstate.Image != "" && !objstate.Image.ToLower().StartsWith("http"))
            {
                imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");
                if (objstate.Image.ToLower().StartsWith("ipfs:")) { imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\"); if (objstate.Image.Length == 51) { imagelocation += @"\artifact"; } }
            }

            if (imagelocation != "")
            {
                ObjectImage.ImageLocation = imagelocation;
            }
            else
            {
                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");

                Match urnmatch = regexTransactionId.Match(objstate.URN);
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
                    ObjectImage.ImageLocation = filePath;

                }
                else
                {

                    Random rnd = new Random();
                    string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                    if (gifFiles.Length > 0)
                    {
                        int randomIndex = rnd.Next(gifFiles.Length);
                        string randomGifFile = gifFiles[randomIndex];

                        ObjectImage.ImageLocation = randomGifFile;

                    }
                    else
                    {
                        try
                        {
                            ObjectImage.ImageLocation = @"includes\HugPuddle.jpg";
                        }
                        catch { }
                    }
                }
            }





            if (objstate.Listings != null)
            {
                flowListings.Controls.Clear();

                flowListings.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                flowListings.AutoScroll = true;

                int row = 0;
                foreach (KeyValuePair<string, BID> item in objstate.Listings)
                {


                    TableLayoutPanel rowPanel = new TableLayoutPanel
                    {
                        RowCount = 1,
                        ColumnCount = 3,
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Padding = new System.Windows.Forms.Padding(3)
                    };

                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));


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

                    keyLabel.Links[0].LinkData = item.Key + ":" + item.Value.Qty.ToString() + ":" + item.Value.Value.ToString();
                    keyLabel.AutoSize = true;
                    keyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    keyLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                    keyLabel.LinkColor = System.Drawing.Color.Black;
                    keyLabel.ActiveLinkColor = System.Drawing.Color.Black;
                    keyLabel.VisitedLinkColor = System.Drawing.Color.Black;
                    keyLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkClicked);
                    keyLabel.Dock = DockStyle.Left;


                    Label qtyLabel = new Label
                    {
                        Text = item.Value.Qty.ToString("N0"),
                        AutoSize = true,
                        Dock = DockStyle.Right
                    };

                    Label valueLabel = new Label
                    {
                        Text = item.Value.Value.ToString(),
                        AutoSize = true,
                        Dock = DockStyle.Right
                    };


                    rowPanel.Controls.Add(keyLabel, 0, 0);
                    rowPanel.Controls.Add(qtyLabel, 1, 0);
                    rowPanel.Controls.Add(valueLabel, 2, 0);

                    if (row % 2 == 0)
                    {
                        rowPanel.BackColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        rowPanel.BackColor = System.Drawing.Color.LightGray;
                    }


                    flowListings.Controls.Add(rowPanel);
                    row++;



                }

                long totalQty = objstate.Owners.Values.Sum(tuple => tuple.Item1);
            }


            if (objstate.Offers != null)
            {

                flowOffers.Controls.Clear();
                flowOffers.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                flowOffers.AutoScroll = true;

                int row = 0;
                foreach (BID item in objstate.Offers)
                {


                    TableLayoutPanel rowPanel = new TableLayoutPanel
                    {
                        RowCount = 1,
                        ColumnCount = 4,
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Padding = new System.Windows.Forms.Padding(3)
                    };

                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));


                    LinkLabel keyLabel = new LinkLabel();


                    string searchkey = item.Requestor;


                    if (!profileAddress.ContainsKey(searchkey))
                    {

                        PROState profile = PROState.GetProfileByAddress(searchkey, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                        if (profile.URN != null)
                        {
                            keyLabel.Text = profile.URN;

                        }
                        else
                        {
                            keyLabel.Text = searchkey;
                        }
                        profileAddress.Add(searchkey, keyLabel.Text);
                    }
                    else
                    {
                        profileAddress.TryGetValue(searchkey, out string ShortName);
                        keyLabel.Text = ShortName;
                    }

                    keyLabel.Links[0].LinkData = searchkey;
                    keyLabel.AutoSize = true;
                    keyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    keyLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                    keyLabel.LinkColor = System.Drawing.Color.Black;
                    keyLabel.ActiveLinkColor = System.Drawing.Color.Black;
                    keyLabel.VisitedLinkColor = System.Drawing.Color.Black;
                    keyLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkClicked);
                    keyLabel.Dock = DockStyle.Left;



                    LinkLabel ownerLabel = new LinkLabel();

                    searchkey = item.Owner;


                    if (!profileAddress.ContainsKey(searchkey))
                    {

                        PROState profile = PROState.GetProfileByAddress(searchkey, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                        if (profile.URN != null)
                        {
                            ownerLabel.Text = profile.URN;

                        }
                        else
                        {
                            ownerLabel.Text = searchkey;
                        }
                        profileAddress.Add(searchkey, ownerLabel.Text);
                    }
                    else
                    {
                        profileAddress.TryGetValue(searchkey, out string ShortName);
                        ownerLabel.Text = ShortName;
                    }

                    ownerLabel.Links[0].LinkData = searchkey;
                    ownerLabel.AutoSize = true;
                    ownerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    ownerLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                    ownerLabel.LinkColor = System.Drawing.Color.Black;
                    ownerLabel.ActiveLinkColor = System.Drawing.Color.Black;
                    ownerLabel.VisitedLinkColor = System.Drawing.Color.Black;
                    ownerLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkClicked);
                    ownerLabel.Dock = DockStyle.Left;


                    Label qtyLabel = new Label
                    {
                        Text = item.Qty.ToString(),
                        AutoSize = true,
                        Dock = DockStyle.Right
                    };

                    Label valueLabel = new Label
                    {
                        Text = item.Value.ToString(),
                        AutoSize = true,
                        Dock = DockStyle.Right
                    };


                    rowPanel.Controls.Add(keyLabel, 0, 0);
                    rowPanel.Controls.Add(ownerLabel, 1, 0);
                    rowPanel.Controls.Add(qtyLabel, 2, 0);
                    rowPanel.Controls.Add(valueLabel, 3, 0);

                    if (row % 2 == 0)
                    {
                        rowPanel.BackColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        rowPanel.BackColor = System.Drawing.Color.LightGray;
                    }


                    flowOffers.Controls.Add(rowPanel);
                    row++;



                }



            }

        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            string[] linkData = e.Link.LinkData.ToString().Split(':');
            txtCurrentOwnerAddress.Text = linkData[0];
            try
            {
                if (maxHold > 0 && int.Parse(linkData[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) > maxHold)
                {
                    txtBuyQty.Text = maxHold.ToString();
                }
                else
                {
                    txtBuyQty.Text = linkData[1];
                }
                txtBuyEachCost.Text = linkData[2];
            }
            catch { }

        }

        private void tmrSearchMemoryPool_Tick(object sender, EventArgs e)
        {
            lock (SupLocker)
            {
                tmrSearchMemoryPool.Stop();


                try
                {
                    Task SearchMemoryTask = Task.Run(() =>
                    {
                        List<string> differenceQuery = new List<string>();
                        List<string> newtransactions = new List<string>();
                        string flattransactions;
                        OBJState isobject = new OBJState();
                        List<OBJState> foundobjects = new List<OBJState>();
                        NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                        NBitcoin.RPC.RPCClient rpcClient;
                        string myFriendsJson = "";
                        Dictionary<string, string> myFriends = new Dictionary<string, string>();

                        if (System.IO.File.Exists(@"root\MyFriendList.Json"))
                        {
                            myFriendsJson = System.IO.File.ReadAllText(@"root\MyFriendList.Json");
                            myFriends = JsonConvert.DeserializeObject<Dictionary<string, string>>(myFriendsJson);
                        }
                        string filter = txtAddressSearch.Text;
                        isobject = OBJState.GetObjectByAddress(filter, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                        try
                        {
                            rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);
                            flattransactions = rpcClient.SendCommand("getrawmempool").ResultString;
                            flattransactions = flattransactions.Replace("\"", "").Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
                            newtransactions = flattransactions.Split(',').ToList();

                            differenceQuery =
                            (List<string>)newtransactions.Except(BTCTMemPool).ToList(); ;

                            BTCTMemPool = newtransactions;

                            this.Invoke((MethodInvoker)delegate
                            {
                                progressBar1.Maximum = differenceQuery.Count;
                                progressBar1.Value = 0;
                            });

                            foreach (var s in differenceQuery)
                            {
                                try
                                {

                                    Root root = Root.GetRootByTransactionId(s, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);
                                    if (root.Signed == true)
                                    {



                                        if (!System.IO.File.Exists(@"root\" + root.SignedBy + @"\BLOCK"))
                                        {
                                            bool find = false;

                                            if (filter != "")
                                            {

                                                if (filter.StartsWith("#"))
                                                {
                                                    find = root.Keyword.ContainsKey(Root.GetPublicAddressByKeyword(filter.Substring(1)));
                                                }
                                                else
                                                {

                                                    find = root.Output.ContainsKey(filter);

                                                }
                                            }
                                            else { find = true; }

                                            if (find)
                                            {

                                                switch (root.File.Last().Key.ToString().Substring(root.File.Last().Key.ToString().Length - 3))
                                                {
                                                    case "GIV":


                                                        List<List<int>> givinspector = new List<List<int>> { };
                                                        try
                                                        {
                                                            givinspector = JsonConvert.DeserializeObject<List<List<int>>>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\GIV"));
                                                        }
                                                        catch
                                                        {

                                                            break;
                                                        }

                                                        if (givinspector == null)
                                                        {
                                                            break;
                                                        }

                                                        foreach (var give in givinspector)
                                                        {
                                                            string _from = root.SignedBy;
                                                            string _to = "";
                                                            _to = root.Keyword.Reverse().GetItemByIndex(give[0]).Key;
                                                            string _message = "GIV 💕 " + give[1];
                                                            string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                            string imglocation = "";

                                                            if (give[1] < 0)
                                                            {
                                                                break;
                                                            }


                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                try { imglocation = myFriends[_from]; } catch { }

                                                                CreateFeedRow(imglocation, _to, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowInMemoryResults, true);

                                                            });




                                                        }
                                                        break;

                                                    case "BUY":



                                                        List<List<string>> buyinspector = new List<List<string>> { };
                                                        try
                                                        {
                                                            buyinspector = JsonConvert.DeserializeObject<List<List<string>>>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\BUY"));
                                                        }
                                                        catch
                                                        {

                                                            break;
                                                        }

                                                        if (buyinspector == null)
                                                        {
                                                            break;
                                                        }

                                                        foreach (var buy in buyinspector)
                                                        {
                                                            string _from = root.SignedBy;
                                                            string _to = buy[0];
                                                            string _message = "BUY 💰 " + buy[1];
                                                            string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                            string imglocation = "";

                                                            if (long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) < 0)
                                                            {
                                                                break;
                                                            }


                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                try { imglocation = myFriends[_from]; } catch { }

                                                                if (!pendingBUY.ContainsKey(buy[0]))
                                                                {
                                                                    pendingBUY.Add(buy[0], long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")));
                                                                }
                                                                else
                                                                {
                                                                    pendingBUY[buy[0]] = pendingBUY[buy[0]] + long.Parse(buy[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
                                                                }
                                                                CreateFeedRow(imglocation, _to, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowInMemoryResults, true);

                                                            });




                                                        }
                                                        break;

                                                    case "LST":


                                                        List<List<string>> lstinspector = new List<List<string>> { };
                                                        try
                                                        {
                                                            lstinspector = JsonConvert.DeserializeObject<List<List<string>>>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\LST"));
                                                        }
                                                        catch
                                                        {

                                                            break;
                                                        }

                                                        if (lstinspector == null)
                                                        {
                                                            break;
                                                        }

                                                        foreach (var lst in lstinspector)
                                                        {
                                                            string _from = root.SignedBy;
                                                            string _to = "";

                                                            if (givaddress == root.SignedBy) { _to = "primary"; } else { _to = "secondary"; }



                                                            string _message = "LST 📰 " + lst[1] + " at " + lst[2] + " each";
                                                            string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                            string imglocation = "";

                                                            if (long.Parse(lst[1], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) < 0)
                                                            {
                                                                break;
                                                            }


                                                            this.Invoke((MethodInvoker)delegate
                                                            {
                                                                try { imglocation = myFriends[_from]; } catch { }

                                                                CreateFeedRow(imglocation, _to, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowInMemoryResults, true);

                                                            });


                                                        }
                                                        break;
                                                    case "BRN":


                                                        List<List<long>> brninspector = new List<List<long>> { };
                                                        try
                                                        {
                                                            brninspector = JsonConvert.DeserializeObject<List<List<long>>>(System.IO.File.ReadAllText(@"root\" + root.TransactionId + @"\BRN"));
                                                        }
                                                        catch
                                                        {

                                                            break;
                                                        }

                                                        if (brninspector == null)
                                                        {
                                                            break;
                                                        }

                                                        foreach (var give in brninspector)
                                                        {
                                                            string _from = root.SignedBy;
                                                            string _to = "";
                                                            _to = root.Keyword.Reverse().GetItemByIndex((int)give[0]).Key;
                                                            string _message = "BRN 🔥 " + give[1];
                                                            string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                            string imglocation = "";

                                                            if (give[1] > 0 && _to == txtAddressSearch.Text)
                                                            {


                                                                this.Invoke((MethodInvoker)delegate
                                                                {
                                                                    try { imglocation = myFriends[_from]; } catch { }

                                                                    CreateFeedRow(imglocation, _to, _from, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowInMemoryResults, true);

                                                                });

                                                            }


                                                        }
                                                        break;

                                                    default:

                                                        break;

                                                }
                                            }

                                        }
                                        else { try { System.IO.Directory.Delete(@"root\" + s, true); } catch { } }

                                    }
                                    else
                                    {

                                    }

                                }
                                catch (Exception ex)
                                {
                                    string error = ex.Message;
                                }


                                this.Invoke((MethodInvoker)delegate
                                {
                                    progressBar1.Value = progressBar1.Value + 1;
                                });
                            }


                        }
                        catch
                        {

                        }



                        this.Invoke((MethodInvoker)delegate
                        {
                            var tableLayoutPanels = flowInMemoryResults.Controls.OfType<TableLayoutPanel>().ToList();
                            bool refresh = false;
                            foreach (var panel in tableLayoutPanels)
                            {
                                string diskpath = "root\\" + panel.Tag.ToString() + "\\";
                                try { System.IO.File.Delete(diskpath + "ROOT.json"); } catch { }

                                Root root = Root.GetRootByTransactionId(panel.Tag.ToString(), mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                if (root != null && root.BlockDate.Year > 1975)
                                {
                                    RemoveRowByTransactionId(flowInMemoryResults, panel.Tag.ToString());
                                    refresh = true;
                                }
                            }
                            if (refresh) { RefreshPage(); }

                        });

                        this.Invoke((MethodInvoker)delegate
                        {

                            tmrSearchMemoryPool.Start();
                        });

                    });



                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    this.Invoke((MethodInvoker)delegate
                    {
                        tmrSearchMemoryPool.Start();
                    });

                }



            }
        }

        private void lblNonRefundableOffer_DoubleClick(object sender, EventArgs e)
        {
            RefreshPage();
        }

        private void lblCurrentListings_DoubleClick(object sender, EventArgs e)
        {
            RefreshPage();
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {

            if (!int.TryParse(txtBuyQty.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int buyQTY1) || buyQTY1 < 1 || txtBuyQty.Text.IndexOf('.') != -1)
            {
                MessageBox.Show("Buy Qty must be 1 or greater", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBuyQty.Text = "1";
                return;
            }

            if (maxHold > 0 && int.TryParse(txtBuyQty.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int buyQTY2) && buyQTY2 > maxHold)
            {
                MessageBox.Show("Buy Qty exceeds maximum holding amount", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBuyQty.Text = maxHold.ToString();
                return;
            }

            if (txtCurrentOwnerAddress.Text == "")
            {
                MessageBox.Show($"You must enter an Owner Address to Buy from in the Current Owner to Buy from field.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (int.TryParse(txtBuyQty.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int buyQTY3) && buyQTY3 <= maxHold)
            {
                List<OBJState> currentlyOwnedObjects = OBJState.GetObjectsOwnedByAddress(txtSignatureAddress.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                // Find the OBJState object that corresponds to the specified address in Creators
                OBJState objStateForAddress = currentlyOwnedObjects?.FirstOrDefault(obj => obj.Creators.ContainsKey(givaddress));

                // Check if the address is found in Creators and if the buy quantity exceeds the maxHold
                if (objStateForAddress != null && objStateForAddress.Owners.ContainsKey(txtSignatureAddress.Text))
                {
                    long currentHoldings = objStateForAddress.Owners[txtSignatureAddress.Text].Item1;

                    // Calculate the maximum quantity that can be bought
                    long maxBuyQty = Math.Max(0, maxHold - currentHoldings);

                    if (buyQTY3 > maxBuyQty)
                    {
                        MessageBox.Show($"This transaction will likely fail. Buy Qty exceeds MAX holding threshold. Maximum Qty allowed: {maxBuyQty}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBuyQty.Text = maxBuyQty.ToString();
                        return;
                    }
                }
            }


            if (int.TryParse(txtBuyQty.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int buyQTY4))
            {
                List<OBJState> currentlyOwnedObjects = OBJState.GetObjectsOwnedByAddress(txtCurrentOwnerAddress.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                // Find the OBJState object that corresponds to the specified address in Creators
                OBJState objStateForAddress = currentlyOwnedObjects?.FirstOrDefault(obj => obj.Creators.ContainsKey(givaddress));

                // Check if the address is found in Creators and if the buy quantity exceeds the maxHold
                if (objStateForAddress != null && objStateForAddress.Owners.ContainsKey(txtCurrentOwnerAddress.Text))
                {
                    long currentHoldings = objStateForAddress.Owners[txtCurrentOwnerAddress.Text].Item1;

                    // Calculate the maximum quantity that can be bought
                    long maxBuyQty = currentHoldings;

                    if (buyQTY4 > maxBuyQty)
                    {
                        MessageBox.Show($"This transaction will likely fail. Buy Qty exceeds current owner's holdings. Maximum Qty allowed: {maxBuyQty}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBuyQty.Text = maxBuyQty.ToString();
                        return;
                    }



                    try
                    {
                        var listingForCurrentOwner = objStateForAddress.Listings.Values.FirstOrDefault(listing => listing.Owner == txtCurrentOwnerAddress.Text);

                        if (listingForCurrentOwner != null)
                        {
                            if (decimal.TryParse(txtBuyEachCost.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal buyEachCost))
                            {
                                if (buyEachCost < listingForCurrentOwner.Value)
                                {
                                    MessageBox.Show($"This transaction will likely fail. Each unit cost is less than the listed cost. Listed Cost: {listingForCurrentOwner.Value}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtBuyEachCost.Text = listingForCurrentOwner.Value.ToString();
                                    return;
                                }
                            }

                            if (pendingBUY.ContainsKey(txtCurrentOwnerAddress.Text) && buyQTY4 > listingForCurrentOwner.Qty - pendingBUY[txtCurrentOwnerAddress.Text])
                            {
                                DialogResult result = MessageBox.Show($"This transaction will likely fail.\nPending BUY transactions exceed the current owner's listing. Click Ok to ignore this warning.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                if (result == DialogResult.Cancel)
                                {
                                    return; 
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("This listing has been updated..please verify and try again", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        RefreshPage();
                        return;

                    }




                }
                else
                {
                    DialogResult result = MessageBox.Show($"No listings found for the current owner\n a non refundable BUY offer will be generated instead: {txtCurrentOwnerAddress.Text}", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Cancel)
                    {
                        return; 
                    }

                }

            }


            var newdictionary = new List<List<string>>();
            List<string> encodedList = new List<string>();
            newdictionary.Add(new List<string> { txtCurrentOwnerAddress.Text, txtBuyQty.Text });

            // Generate a random negative integer salt between -99999 and -1
            int salt;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[4];
                rng.GetBytes(saltBytes);
                salt = -Math.Abs(BitConverter.ToInt32(saltBytes, 0) % 100000);
            }

            newdictionary.Add(new List<string> { "0", salt.ToString("D5") });

            var json = JsonConvert.SerializeObject(newdictionary);
            txtOBJJSON.Text = json;

            txtOBJP2FK.Text = "BUY" + GetRandomDelimiter() + txtOBJJSON.Text.Length + GetRandomDelimiter() + txtOBJJSON.Text;

            NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
            NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);
            System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
            byte[] hashValue = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(txtOBJP2FK.Text));
            string signatureAddress;

            signatureAddress = txtSignatureAddress.Text;
            string signature = "";
            try { signature = rpcClient.SendCommand("signmessage", signatureAddress, BitConverter.ToString(hashValue).Replace("-", String.Empty)).ResultString; }
            catch (Exception ex)
            {
                lblObjectStatus.Text = ex.Message;
                btnBuy.BackColor = System.Drawing.Color.White;
                btnBuy.ForeColor = System.Drawing.Color.Black;
                mint = false;
                return;
            }

            txtOBJP2FK.Text = "SIG" + GetRandomDelimiter() + "88" + GetRandomDelimiter() + signature + txtOBJP2FK.Text;


            for (int i = 0; i < txtOBJP2FK.Text.Length; i += 20)
            {
                string chunk = txtOBJP2FK.Text.Substring(i, Math.Min(20, txtOBJP2FK.Text.Length - i));
                if (chunk.Any())
                {
                    encodedList.Add(Root.GetPublicAddressByKeyword(chunk, mainnetVersionByte));
                }
            }




            txtAddressListJSON.Text = JsonConvert.SerializeObject(encodedList.Distinct());

            lblBuyCost.Text = "cost: " + (0.00000546 * encodedList.Count + (long.Parse(txtBuyQty.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) * double.Parse(txtBuyEachCost.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")))).ToString("0.00000000") + "  + miner fee";

            if (mint)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to buy a qty of ( " + txtBuyQty.Text + " ) \n" + lblBuyCost.Text, "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    // Perform the action
                    var recipients = new Dictionary<string, decimal>();
                    foreach (var encodedAddress in encodedList)
                    {
                        try { recipients.Add(encodedAddress, 0.00000546m); } catch { }
                    }

                    decimal totalCost = long.Parse(txtBuyQty.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")) * decimal.Parse(txtBuyEachCost.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
                    decimal remainingCost = totalCost;
                    OBJState objstate = OBJState.GetObjectByAddress(txtAddressSearch.Text, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                    foreach (var keyvalue in objstate.Royalties)
                    {
                        if (keyvalue.Key != txtCurrentOwnerAddress.Text && keyvalue.Key != txtSignatureAddress.Text)
                        {
                            try
                            {
                                decimal royaltyCost = totalCost * (keyvalue.Value / 100);
                                if (royaltyCost < 0.00000546m) { royaltyCost = 0.00000546m; }
                                recipients.Add(keyvalue.Key, royaltyCost);

                                remainingCost = remainingCost - (totalCost * (keyvalue.Value / 100));

                            }
                            catch { }

                        }
                    }

                    if (remainingCost < 0.00000546m) { remainingCost = 0.00000546m; }
                    recipients.Add(txtCurrentOwnerAddress.Text, remainingCost);
                    try { recipients.Add(txtAddressSearch.Text, 0.00000546m); } catch { }
                    recipients.Add(txtSignatureAddress.Text, 0.00000546m);


                    CoinRPC a = new CoinRPC(new Uri(mainnetURL), new NetworkCredential("good-user", "better-password"));

                    try
                    {
                        string accountsString = "";
                        try { accountsString = rpcClient.SendCommand("listaccounts").ResultString; } catch { }
                        var accounts = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(accountsString);
                        var keyWithLargestValue = accounts.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                        var results = a.SendMany(keyWithLargestValue, recipients);
                        lblObjectStatus.Text = results;
                    }
                    catch (Exception ex) { lblObjectStatus.Text = ex.Message; }
                    btnBuy.BackColor = System.Drawing.Color.White;
                    btnBuy.ForeColor = System.Drawing.Color.Black;
                    mint = false;

                }
                btnBuy.BackColor = System.Drawing.Color.White;
                btnBuy.ForeColor = System.Drawing.Color.Black;
                mint = false;
            }

            btnBuy.BackColor = System.Drawing.Color.Blue;
            btnBuy.ForeColor = System.Drawing.Color.Yellow;
            mint = true;
        }

        private void giveButton_Click(object sender, EventArgs e)
        {

            if (int.TryParse(txtListQty.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int listQTY) && txtListQty.Text.IndexOf('.') == -1)
            {


                OBJState currentObject = OBJState.GetObjectByAddress(givaddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                // Check if the address is found in Creators and if the give quantity exceeds the maxHold
                if (currentObject.Owners.ContainsKey(txtSignatureAddress.Text))
                {
                    long currentHoldings = 0;
                    try { currentHoldings = currentObject.Owners[txtSignatureAddress.Text].Item1; } catch { }


                    // Calculate the maximum quantity that can be given
                    long maxListQty = currentHoldings;

                    if (listQTY > maxListQty)
                    {
                        MessageBox.Show($"This transaction will likely fail. List Qty exceeds current owner's holdings. Maximum List allowed: {maxListQty}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtListQty.Text = maxListQty.ToString();
                        return;
                    }
                }
                else
                {
                    if (currentObject.Creators.ContainsKey(txtSignatureAddress.Text))
                    {
                        long currentHoldings = 0;
                        try { currentHoldings = currentObject.Owners[givaddress].Item1; } catch { }

                        // Calculate the maximum quantity that can be given
                        long maxListQty = currentHoldings;

                        if (listQTY > maxListQty)
                        {
                            MessageBox.Show($"This transaction will likely fail. List Qty exceeds current owner's holdings. Maximum List allowed: {maxListQty}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtListQty.Text = maxListQty.ToString();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show($"This transaction will likely fail. The current signature does not own any objects to List", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtListQty.Text = "0";

                    }
                }


                var newdictionary = new List<List<string>>();
                List<string> encodedList = new List<string>();
                newdictionary.Add(new List<string> { txtAddressSearch.Text, txtListQty.Text, txtEachValue.Text });

                // Generate a random negative integer salt between -99999 and -1
                int salt;
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    byte[] saltBytes = new byte[4];
                    rng.GetBytes(saltBytes);
                    salt = -Math.Abs(BitConverter.ToInt32(saltBytes, 0) % 100000);
                }

                newdictionary.Add(new List<string> { "0", salt.ToString("D5") });

                var json = JsonConvert.SerializeObject(newdictionary);
                txtOBJJSON.Text = json;

                txtOBJP2FK.Text = "LST" + GetRandomDelimiter() + txtOBJJSON.Text.Length + GetRandomDelimiter() + txtOBJJSON.Text;

                NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(mainnetURL), Network.Main);
                System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                byte[] hashValue = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(txtOBJP2FK.Text));
                string signatureAddress;

                signatureAddress = txtSignatureAddress.Text;
                string signature = "";
                try { signature = rpcClient.SendCommand("signmessage", signatureAddress, BitConverter.ToString(hashValue).Replace("-", String.Empty)).ResultString; }
                catch (Exception ex)
                {
                    lblObjectStatus.Text = ex.Message;
                    btnGive.BackColor = System.Drawing.Color.White;
                    btnGive.ForeColor = System.Drawing.Color.Black;
                    mint = false;
                    return;
                }

                txtOBJP2FK.Text = "SIG" + GetRandomDelimiter() + "88" + GetRandomDelimiter() + signature + txtOBJP2FK.Text;


                for (int i = 0; i < txtOBJP2FK.Text.Length; i += 20)
                {
                    string chunk = txtOBJP2FK.Text.Substring(i, Math.Min(20, txtOBJP2FK.Text.Length - i));
                    if (chunk.Any())
                    {
                        encodedList.Add(Root.GetPublicAddressByKeyword(chunk, mainnetVersionByte));
                    }
                }



                encodedList.Add(txtAddressSearch.Text);
                encodedList.Add(signatureAddress);
                txtAddressListJSON.Text = JsonConvert.SerializeObject(encodedList.Distinct());

                lblCost.Text = "cost: " + (0.00000546 * encodedList.Count).ToString("0.00000000") + "  + miner fee";

                if (mint)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to list this?", "Confirmation", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        // Perform the action
                        var recipients = new Dictionary<string, decimal>();
                        foreach (var encodedAddress in encodedList)
                        {
                            try { recipients.Add(encodedAddress, 0.00000546m); } catch { }
                        }

                        CoinRPC a = new CoinRPC(new Uri(mainnetURL), new NetworkCredential("good-user", "better-password"));

                        try
                        {
                            string accountsString = "";
                            try { accountsString = rpcClient.SendCommand("listaccounts").ResultString; } catch { }
                            var accounts = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(accountsString);
                            var keyWithLargestValue = accounts.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                            var results = a.SendMany(keyWithLargestValue, recipients);
                            lblObjectStatus.Text = results;
                        }
                        catch (Exception ex) { lblObjectStatus.Text = ex.Message; }
                        btnGive.BackColor = System.Drawing.Color.White;
                        btnGive.ForeColor = System.Drawing.Color.Black;
                        mint = false;

                    }
                    btnGive.BackColor = System.Drawing.Color.White;
                    btnGive.ForeColor = System.Drawing.Color.Black;
                    mint = false;
                }

                btnGive.BackColor = System.Drawing.Color.Blue;
                btnGive.ForeColor = System.Drawing.Color.Yellow;
                mint = true;

            }
            else
            {
                MessageBox.Show($"This transaction will likely fail. List Qty is not numeric", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtListQty.Text = "0";
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
                    txtSignatureAddress.Text = selectedProState.Creators[0];
                }

            }
        }
    }
}
