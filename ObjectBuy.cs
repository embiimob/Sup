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
using BitcoinNET.RPCClient;
using NBitcoin.RPC;
using NBitcoin;
using Newtonsoft.Json;
using SUP.P2FK;
using AngleSharp.Common;
using AngleSharp.Text;
using System.Threading.Tasks;
using LevelDB;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.Media;
using AngleSharp.Dom;
using System.Windows.Interop;

namespace SUP
{
    public partial class ObjectBuy : Form
    {
        //GPT3 ROCKS
        private readonly static object SupLocker = new object();
        private List<string> BTCTMemPool = new List<string>();


        private const int MaxRows = 2000;
        private readonly List<(string address, int qty)> _addressQtyList = new List<(string address, int qty)>();
        bool mint = false;
        private readonly string givaddress = "";


        public ObjectBuy(string _address="")
        {
            InitializeComponent();
            givaddress = _address;
        }


        void AddImage(string imagepath, bool isprivate = false, bool addtoTop = false)
        {
            string imagelocation = "";
            if (imagepath != null)
            {
                imagelocation = imagepath;


                if (!imagepath.ToLower().StartsWith("http"))
                {
                    imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (imagepath.ToLower().StartsWith("ipfs:")) { imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\"); if (imagepath.Length == 51) { imagelocation += @"\artifact"; } }
                }
                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                Match imgurnmatch = regexTransactionId.Match(imagelocation);
                string transactionid = imgurnmatch.Value;
                Root root = new Root();
                if (!File.Exists(imagelocation))
                {
                    switch (imagepath.ToUpper().Substring(0, 4))
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
                            try { transid = imagepath.Substring(5, 46); } catch { }

                            if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                            {
                                try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                Directory.CreateDirectory("ipfs/" + transid + "-build");
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + imagepath.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();
                                string fileName;
                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    fileName = imagepath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    Directory.CreateDirectory("ipfs/" + transid);
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", imagelocation);
                                }

                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                {
                                    fileName = imagepath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, imagelocation);
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

                            break;
                        default:
                            if (!imagepath.ToUpper().StartsWith("HTTP") && transactionid != "")
                            {
                                Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");

                            }
                            break;
                    }
                }



            }


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

            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 290));

            if (!isprivate)
               {

                if (addtoTop)
                {
                    flowInMemoryResults.Controls.Add(msg);
                    flowInMemoryResults.Controls.SetChildIndex(msg, 2);
                }
                else
                {
                    flowInMemoryResults.Controls.Add(msg);
                }


            }
            PictureBox pictureBox = new PictureBox();

            // Set the PictureBox properties

            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Width = 290;
            pictureBox.Height = 290;
            pictureBox.BackColor = Color.Black;
            pictureBox.ImageLocation = imagelocation;
            pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };
            msg.Controls.Add(pictureBox);


        }



        void deleteme_LinkClicked(string transactionid)
        {

            string unfilteredmessage = "";
            try { unfilteredmessage = System.IO.File.ReadAllText(@"root/" + transactionid + @"/MSG"); } catch { }


            string pattern = "<<.*?>>";
            MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
            foreach (Match match in matches)
            {
                string content = match.Value.Substring(2, match.Value.Length - 4);
                if (!int.TryParse(content, out int id))
                {

                    string imagelocation = "";
                    if (content != null)
                    {
                        imagelocation = content;

                        if (!content.ToLower().StartsWith("http"))
                        {
                            imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");
                            if (content.ToLower().StartsWith("ipfs:"))
                            {
                                imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\");
                                if (content.Length == 51) { imagelocation += @"\artifact"; }
                            }

                            string parentDir = Path.GetDirectoryName(imagelocation);

                            if (Directory.Exists(parentDir))
                            {
                                Directory.Delete(parentDir, true);
                            }

                            parentDir = parentDir.Replace(@"\ipfs\", @"\root\");

                            if (Directory.Exists(parentDir))
                            {
                                Directory.Delete(parentDir, true);
                            }

                            if (Directory.Exists(parentDir + "-build"))
                            {
                                Directory.Delete(parentDir + "-build", true);
                            }

                        }
                    }


                }
            }

            try
            {
                Directory.Delete(@"root\" + transactionid, true);
                Directory.CreateDirectory(@"root\" + transactionid);
            }
            catch { }
            Root P2FKRoot = new Root();
            var rootSerialized = JsonConvert.SerializeObject(P2FKRoot);
            System.IO.File.WriteAllText(@"root\" + transactionid + @"\" + "ROOT.json", rootSerialized);
        }

        void Owner_LinkClicked(string ownerId)
        {

            new ObjectBrowser(ownerId).Show();
        }


        void Attachment_Clicked(string path)
        {
            if (path.ToUpper().StartsWith("IPFS:") || path.ToUpper().StartsWith("BTC:") || path.ToUpper().StartsWith("MZC:") || path.ToUpper().StartsWith("LTC:") || path.ToUpper().StartsWith("DOG:"))
            {
                new ObjectBrowser(path).Show();
            }
            else
            {
                try
                { System.Diagnostics.Process.Start(path); }
                catch { System.Media.SystemSounds.Exclamation.Play(); }
            }
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
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
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

            if (File.Exists(imageLocation) || imageLocation.ToUpper().StartsWith("HTTP"))
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






            

                PROState profile = PROState.GetProfileByAddress(SentFrom, "good-user", "better-password", "http://127.0.0.1:18332");

                if (profile.URN != null)
                {
                SentFrom = profile.URN;
                }

            profile = PROState.GetProfileByAddress(SentTo, "good-user", "better-password", "http://127.0.0.1:18332");

            if (profile.URN != null)
            {
                SentTo = profile.URN;
            }

            if (SentFrom == txtCurrentOwnerAddress.Text) { SentFrom = "primary"; }
            if (SentTo == txtCurrentOwnerAddress.Text) { SentTo = "primary"; }



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
            row.Controls.Add(message,3,0);






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


        private void giveButton_Click(object sender, EventArgs e)
        {
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

            txtOBJP2FK.Text = "LST" + ">" + txtOBJJSON.Text.Length + ">" + txtOBJJSON.Text;

            NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                RPCClient rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:18332"), Network.Main);
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

                txtOBJP2FK.Text = "SIG" + ":" + "88" + ">" + signature + txtOBJP2FK.Text;

                
                for (int i = 0; i < txtOBJP2FK.Text.Length; i += 20)
                {
                    string chunk = txtOBJP2FK.Text.Substring(i, Math.Min(20, txtOBJP2FK.Text.Length - i));
                    if (chunk.Any())
                    {
                        encodedList.Add(Root.GetPublicAddressByKeyword(chunk));
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

                        CoinRPC a = new CoinRPC(new Uri("http://127.0.0.1:18332"), new NetworkCredential("good-user", "better-password"));

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
     

        private void ObjectGive_Load(object sender, EventArgs e)
        {
            txtAddressSearch.Text = givaddress;
            RefreshPage();

        }

        private void RefreshPage()
        {
           
           

            OBJState objstate = OBJState.GetObjectByAddress(txtAddressSearch.Text, "good-user", "better-password", "http://127.0.0.1:18332");
            Dictionary<string, string> profileAddress = new Dictionary<string, string> { };
            profileAddress.Add(txtAddressSearch.Text, "primary");

            txtName.Text  = objstate.Name;
            lblLicense.Text = objstate.License;
            lblObjectCreatedDate.Text = objstate.CreatedDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
            lblTotalOwnedDetail.Text = "total: " + objstate.Owners.Values.Sum().ToString();
            lblTotalRoyaltiesDetail.Text = "royalties: " + objstate.Royalties.Values.Sum().ToString();
            
            string imagelocation = "";
            
            if (!objstate.Image.ToLower().StartsWith("http"))
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

                        PROState profile = PROState.GetProfileByAddress(searchkey, "good-user", "better-password", "http://127.0.0.1:18332");

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

                    keyLabel.Links[0].LinkData = item.Key;
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
                        Text = item.Value.Qty.ToString(),
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

                long totalQty = objstate.Owners.Values.Sum();

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

                        PROState profile = PROState.GetProfileByAddress(searchkey, "good-user", "better-password", "http://127.0.0.1:18332");

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

                        PROState profile = PROState.GetProfileByAddress(searchkey, "good-user", "better-password", "http://127.0.0.1:18332");

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

            var linkData = e.Link.LinkData;
            new ObjectBrowser((string)linkData).Show();

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
                        var SUP = new Options { CreateIfMissing = true };
                        List<string> differenceQuery = new List<string>();
                        List<string> newtransactions = new List<string>();
                        string flattransactions;
                        OBJState isobject = new OBJState();
                        List<OBJState> foundobjects = new List<OBJState>();
                        NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
                        RPCClient rpcClient;
                        string myFriendsJson = "";
                        Dictionary<string, string> myFriends = new Dictionary<string, string>();

                        if (File.Exists(@"root\MyFriendList.Json"))
                        {
                            myFriendsJson = File.ReadAllText(@"root\MyFriendList.Json");
                            myFriends = JsonConvert.DeserializeObject<Dictionary<string, string>>(myFriendsJson);
                        }
                        string filter = txtAddressSearch.Text;

                        
                            try
                            {
                                rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:18332"), Network.Main);
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

                                            Root root = Root.GetRootByTransactionId(s, "good-user", "better-password", @"http://127.0.0.1:18332");
                                            if (root.Signed == true)
                                            {
                                                string isBlocked = "";
                                                var OBJ = new Options { CreateIfMissing = true };
                                                try
                                                {
                                                    using (var db = new DB(OBJ, @"root\oblock"))
                                                    {
                                                        isBlocked = db.Get(root.Signature);
                                                        db.Close();
                                                    }
                                                }
                                                catch
                                                {
                                                    try
                                                    {
                                                        using (var db = new DB(OBJ, @"root\oblock2"))
                                                        {
                                                            isBlocked = db.Get(root.Signature);
                                                            db.Close();
                                                        }
                                                        Directory.Move(@"root\oblock2", @"root\oblock");
                                                    }
                                                    catch
                                                    {
                                                        try { Directory.Delete(@"root\oblock", true); }
                                                        catch { }
                                                    }

                                                }


                                                if (isBlocked != "true")
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


                                            switch (root.File.Last().Key.ToString().Substring(root.File.Last().Key.ToString().Length - 3))
                                            {
                                                case "GIV":


                                                    List<List<int>> givinspector = new List<List<int>> { };
                                                    try
                                                    {
                                                        givinspector = JsonConvert.DeserializeObject<List<List<int>>>(File.ReadAllText(@"root\" + root.TransactionId + @"\GIV"));
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
                                                        string _message = "GIV " + give[1];
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
                                                        buyinspector = JsonConvert.DeserializeObject<List<List<string>>>(File.ReadAllText(@"root\" + root.TransactionId + @"\BUY"));
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
                                                        string _message = "BUY " + buy[1];
                                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                        string imglocation = "";

                                                        if (int.Parse(buy[1]) < 0)
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

                                                case "LST":


                                                    List<List<string>> lstinspector = new List<List<string>> { };
                                                    try
                                                    {
                                                        lstinspector = JsonConvert.DeserializeObject<List<List<string>>>(File.ReadAllText(@"root\" + root.TransactionId + @"\LST"));
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

                                                        if (root.SignedBy == txtCurrentOwnerAddress.Text) { _to = "primary"; } else { _to = "secondary"; }
                                                       


                                                        string _message = "LST " + lst[1] + " at " + lst[2] + " each";
                                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                        string imglocation = "";

                                                        if (int.Parse(lst[1]) < 0)
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

                                                default:
                                                    
                                                   break;

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

                            foreach (var panel in tableLayoutPanels)
                            {
                                string diskpath = "root\\" + panel.Tag.ToString() + "\\";
                                try { System.IO.File.Delete(diskpath + "ROOT.json"); } catch { }
                    
                                Root root = Root.GetRootByTransactionId(panel.Tag.ToString(), "good-user", "better-password", @"http://127.0.0.1:18332");

                                if (root != null && root.BlockDate.Year > 1975) { RemoveRowByTransactionId(flowInMemoryResults, panel.Tag.ToString()); }
                            }
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

        private void label2_DoubleClick(object sender, EventArgs e)
        {
            RefreshPage();
        }

        private void label5_DoubleClick(object sender, EventArgs e)
        {
            RefreshPage();
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            var newdictionary = new List<List<string>>();
            List<string> encodedList = new List<string>();
            newdictionary.Add(new List<string> {txtCurrentOwnerAddress.Text, txtBuyQty.Text});

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

            txtOBJP2FK.Text = "BUY" + ">" + txtOBJJSON.Text.Length + ">" + txtOBJJSON.Text;

            NetworkCredential credentials = new NetworkCredential("good-user", "better-password");
            RPCClient rpcClient = new RPCClient(credentials, new Uri(@"http://127.0.0.1:18332"), Network.Main);
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

            txtOBJP2FK.Text = "SIG" + ":" + "88" + ">" + signature + txtOBJP2FK.Text;


            for (int i = 0; i < txtOBJP2FK.Text.Length; i += 20)
            {
                string chunk = txtOBJP2FK.Text.Substring(i, Math.Min(20, txtOBJP2FK.Text.Length - i));
                if (chunk.Any())
                {
                    encodedList.Add(Root.GetPublicAddressByKeyword(chunk));
                }
            }



           
            txtAddressListJSON.Text = JsonConvert.SerializeObject(encodedList.Distinct());

            lblCost.Text = "cost: " + (0.00000546 * encodedList.Count + (int.Parse(txtBuyQty.Text) * double.Parse(txtBuyEachCost.Text))).ToString("0.00000000") + "  + miner fee";

            if (mint)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to buy this?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    // Perform the action
                    var recipients = new Dictionary<string, decimal>();
                    foreach (var encodedAddress in encodedList)
                    {
                        try { recipients.Add(encodedAddress, 0.00000546m); } catch { }
                    }

                    decimal totalCost = int.Parse(txtBuyQty.Text) * decimal.Parse(txtBuyEachCost.Text);
                    decimal remainingCost = totalCost;
                    OBJState objstate = OBJState.GetObjectByAddress(txtAddressSearch.Text, "good-user", "better-password", "http://127.0.0.1:18332");

                    foreach ( var keyvalue in objstate.Royalties)
                    {
                        if (keyvalue.Key != txtCurrentOwnerAddress.Text && keyvalue.Key != txtSignatureAddress.Text) {
                            try { recipients.Add(keyvalue.Key, totalCost * (keyvalue.Value / 100)); remainingCost = remainingCost - (totalCost * (keyvalue.Value / 100)); } catch { }

                        } }

                    recipients.Add(txtCurrentOwnerAddress.Text, remainingCost);
                    try { recipients.Add(txtAddressSearch.Text, 0.00000546m); } catch { }
                    recipients.Add(txtSignatureAddress.Text, 0.00000546m);            


                    CoinRPC a = new CoinRPC(new Uri("http://127.0.0.1:18332"), new NetworkCredential("good-user", "better-password"));

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
    }
}
