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


        void CreateFeedRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp, string messageText, string transactionid, System.Drawing.Color bgcolor, FlowLayoutPanel layoutPanel, bool addtoTop = false)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 4,
                AutoSize = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            // Add the width of the first column to fixed value and second to fill remaining space
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
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


            // Create a LinkLabel with the owner name
            LinkLabel owner = new LinkLabel
            {
                Text = ownerName,
                BackColor = Color.Black,
                ForeColor = Color.White,
                AutoSize = true

            };
            owner.LinkClicked += (sender, e) => { Owner_LinkClicked(ownerId); };
            owner.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            owner.Margin = new System.Windows.Forms.Padding(3);
            owner.Dock = DockStyle.Bottom;
            row.Controls.Add(owner, 1, 0);


            if (timestamp.Year > 1975)
            {  // Create a LinkLabel with the owner name
                Label tstamp = new Label
                {
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                    Margin = new System.Windows.Forms.Padding(0),
                    Dock = DockStyle.Bottom
                };
                row.Controls.Add(tstamp, 2, 0);
            }

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
            deleteme.Click += (sender, e) => { deleteme_LinkClicked(transactionid); };
            row.Controls.Add(deleteme, 3, 0);

            if (messageText != "")
            {
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

                msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, flowInMemoryResults.Width - 20));

                if (addtoTop)
                {
                    layoutPanel.Controls.Add(msg);
                    layoutPanel.Controls.SetChildIndex(msg, 1);
                }
                else
                {
                    layoutPanel.Controls.Add(msg);
                }


                Label message = new Label
                {
                    AutoSize = true,
                    Text = messageText,
                    MinimumSize = new Size(280, 46),
                    Font = new System.Drawing.Font("Segoe UI", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Margin = new System.Windows.Forms.Padding(0),
                    Padding = new System.Windows.Forms.Padding(10, 20, 10, 20),
                    TextAlign = System.Drawing.ContentAlignment.TopLeft
                };
                msg.Controls.Add(message);
            }

        }


        private void giveButton_Click(object sender, EventArgs e)
        {
            var dictionary = new Dictionary<string, int>();
            var newdictionary = new List<List<string>>();
            List<string> encodedList = new List<string>();
            int brnOrder = 2;
            foreach (var (address, qty) in _addressQtyList)
            {
                if (!dictionary.ContainsKey(address))
                {
                    dictionary[address] = qty;
                    if (address == txtSignatureAddress.Text)
                    {
                        newdictionary.Clear();
                        newdictionary.Add(new List<string> { "0", qty.ToString() });
                        dictionary.Clear();
                        dictionary.Add(address, qty);
                        break;
                    }
                    if (txtObjectAddress.Text == txtSignatureAddress.Text)
                    {
                        newdictionary.Clear();
                        newdictionary.Add(new List<string> { "1", qty.ToString() });
                        dictionary.Clear();
                        dictionary.Add(address, qty);
                        break;
                    }
                    newdictionary.Add(new List<string> { brnOrder.ToString(), qty.ToString() });
                    brnOrder++;
                }
            }


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

            txtOBJP2FK.Text = "GIV" + ">" + txtOBJJSON.Text.Length + ">" + txtOBJJSON.Text;

            if (btnGive.Enabled)
            {
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

                foreach (string address in dictionary.Keys)
                {
                    encodedList.Add(address);
                }
       
                encodedList.Add(txtObjectAddress.Text);
                encodedList.Add(signatureAddress); 
                txtAddressListJSON.Text = JsonConvert.SerializeObject(encodedList.Distinct());

                lblCost.Text = "cost: " + (0.00000546 * encodedList.Count).ToString("0.00000000") + "  + miner fee";

                if (mint)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to give this?", "Confirmation", MessageBoxButtons.YesNo);
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




        }
     

        private void ObjectGive_Load(object sender, EventArgs e)
        {
            txtObjectAddress.Text = givaddress;
            RefreshPage();

        }

        private void RefreshPage()
        {
           
            flowOffers.Controls.Clear();
            flowListings.Controls.Clear();



            OBJState objstate = OBJState.GetObjectByAddress(txtObjectAddress.Text, "good-user", "better-password", "http://127.0.0.1:18332");
            Dictionary<string, string> profileAddress = new Dictionary<string, string> { };


            if (objstate.Listings != null)
            {


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
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));


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
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));


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
        private void btnSearchMemory_Click(object sender, EventArgs e)
        {


            if (btnSearchMemory.BackColor == Color.White)
            {
                btnSearchMemory.BackColor = Color.Blue;
                btnSearchMemory.ForeColor = Color.Yellow;
                tmrSearchMemoryPool.Enabled = true;

            }
            else
            {
                btnSearchMemory.BackColor = Color.White;
                btnSearchMemory.ForeColor = Color.Black;
                tmrSearchMemoryPool.Enabled = false;

            }

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

                                                    if (find && root.Message.Count() > 0)
                                                    {

                                                        string _from = root.SignedBy;
                                                        string _to = "";
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.First(); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = string.Join(" ", root.Message);
                                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        _message = Regex.Replace(_message, "<<.*?>>", "");


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateFeedRow(imglocation, _to, _to, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", Color.White, flowInMemoryResults, true);
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateFeedRow(imglocation, _from, _from, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowInMemoryResults, true);


                                                        });

                                                        string pattern = "<<.*?>>";
                                                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                                                        foreach (Match match in matches)
                                                        {


                                                            string content = match.Value.Substring(2, match.Value.Length - 4);
                                                            if (!int.TryParse(content, out int r))
                                                            {

                                                                string imgurn = content;

                                                                if (!content.ToLower().StartsWith("http"))
                                                                {
                                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                                                }

                                                                string extension = Path.GetExtension(imgurn).ToLower();
                                                                List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff" };

                                                                if (!imgExtensions.Contains(extension))
                                                                {


                                                                    try
                                                                    {
                                                                        // Create a WebClient object to fetch the webpage
                                                                        WebClient client = new WebClient();
                                                                        string html = client.DownloadString(content.StripLeadingTrailingSpaces());

                                                                        // Use regular expressions to extract the metadata from the HTML
                                                                        string title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                                                        string description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                                                        string imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                                                        if (description != "")
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {
                                                                                // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(flowInMemoryResults.Width - 30, 100);

                                                                                // Create a label for the title
                                                                                Label titleLabel = new Label();
                                                                                titleLabel.Text = title;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                                                                titleLabel.ForeColor = Color.White;
                                                                                titleLabel.MinimumSize = new Size(flowInMemoryResults.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(titleLabel);

                                                                                // Create a label for the description
                                                                                Label descriptionLabel = new Label();
                                                                                descriptionLabel.Text = description;
                                                                                descriptionLabel.ForeColor = Color.White;
                                                                                descriptionLabel.Dock = DockStyle.Fill;
                                                                                descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                                                                descriptionLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                panel.Controls.Add(descriptionLabel);

                                                                                // Add an image to the panel if one is defined
                                                                                if (!String.IsNullOrEmpty(imageUrl))
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        // Create a MemoryStream object from the image data
                                                                                        byte[] imageData = client.DownloadData(imageUrl);
                                                                                        MemoryStream memoryStream = new MemoryStream(imageData);

                                                                                        // Create a new PictureBox control and add it to the panel
                                                                                        PictureBox pictureBox = new PictureBox();
                                                                                        pictureBox.Dock = DockStyle.Left;
                                                                                        pictureBox.Size = new Size(100, 100);
                                                                                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                                        pictureBox.Image = Image.FromStream(memoryStream);
                                                                                        pictureBox.MouseClick += (sender2, e2) => { Attachment_Clicked(content); };
                                                                                        panel.Controls.Add(pictureBox);
                                                                                    }
                                                                                    catch
                                                                                    {
                                                                                    }
                                                                                }


                                                                                this.flowInMemoryResults.Controls.Add(panel);
                                                                                flowInMemoryResults.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                        else
                                                                        {
                                                                            this.Invoke((MethodInvoker)delegate
                                                                            {  // Create a new panel to display the metadata
                                                                                Panel panel = new Panel();
                                                                                panel.BorderStyle = BorderStyle.FixedSingle;
                                                                                panel.Size = new Size(flowInMemoryResults.Width - 20, 30);

                                                                                // Create a label for the title
                                                                                LinkLabel titleLabel = new LinkLabel();
                                                                                titleLabel.Text = content;
                                                                                titleLabel.Links[0].LinkData = imgurn;
                                                                                titleLabel.Dock = DockStyle.Top;
                                                                                titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                                titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                                titleLabel.MinimumSize = new Size(flowInMemoryResults.Width - 120, 30);
                                                                                titleLabel.Padding = new Padding(5);
                                                                                titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                                panel.Controls.Add(titleLabel);


                                                                                this.flowInMemoryResults.Controls.Add(panel);
                                                                                flowInMemoryResults.Controls.SetChildIndex(panel, 0);
                                                                            });

                                                                        }
                                                                    }
                                                                    catch
                                                                    {

                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {  // Create a new panel to display the metadata
                                                                            Panel panel = new Panel();
                                                                            panel.BorderStyle = BorderStyle.FixedSingle;
                                                                            panel.Size = new Size(flowInMemoryResults.Width - 20, 30);

                                                                            // Create a label for the title
                                                                            LinkLabel titleLabel = new LinkLabel();
                                                                            titleLabel.Text = content;
                                                                            titleLabel.Links[0].LinkData = imgurn;
                                                                            titleLabel.Dock = DockStyle.Top;
                                                                            titleLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                                                            titleLabel.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;
                                                                            titleLabel.MinimumSize = new Size(flowInMemoryResults.Width - 120, 30);
                                                                            titleLabel.Padding = new Padding(5);
                                                                            titleLabel.MouseClick += (sender2, e2) => { Attachment_Clicked(imgurn); };
                                                                            panel.Controls.Add(titleLabel);


                                                                            this.flowInMemoryResults.Controls.Add(panel);
                                                                            flowInMemoryResults.Controls.SetChildIndex(panel, 0);
                                                                        });



                                                                    }
                                                                }
                                                                else
                                                                {


                                                                    if (!int.TryParse(content, out int id))
                                                                    {
                                                                        this.Invoke((MethodInvoker)delegate
                                                                        {
                                                                            AddImage(content, false, true);
                                                                        });
                                                                    }

                                                                }
                                                            }
                                                        }

                                                        TableLayoutPanel padding = new TableLayoutPanel
                                                        {
                                                            RowCount = 1,
                                                            ColumnCount = 1,
                                                            Dock = DockStyle.Top,
                                                            BackColor = Color.Black,
                                                            ForeColor = Color.White,
                                                            AutoSize = true,
                                                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                                                            Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                                                            Padding = new System.Windows.Forms.Padding(0)

                                                        };

                                                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, flowInMemoryResults.Width - 20));

                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            flowInMemoryResults.Controls.Add(padding);
                                                        });


                                            }


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
                                                        if (root.Keyword.Count() > 1) { _to = root.Keyword.Keys.First(); } else { _to = root.Keyword.Keys.Last(); }
                                                        string _message = "GIV " + give[1];
                                                        string _blockdate = root.BlockDate.ToString("yyyyMMddHHmmss");
                                                        string imglocation = "";
                                                        string unfilteredmessage = _message;
                                                        
                                                        if (give[1] < 0)
                                                        {
                                                            break;
                                                        }


                                                        this.Invoke((MethodInvoker)delegate
                                                        {
                                                            try { imglocation = myFriends[_to]; } catch { }
                                                            CreateFeedRow(imglocation, _to, _to, DateTime.ParseExact(_blockdate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), " ", "", Color.White, flowInMemoryResults, true);
                                                            try { imglocation = myFriends[_from]; } catch { }
                                                            CreateFeedRow(imglocation, _from, _from, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), _message, root.TransactionId, Color.White, flowInMemoryResults, true);


                                                        });




                                                    }
                                                    break;

                                                case "BUY":



                                                    break;

                                                case "LST":


                                                    break;

                                                default:
                                                    
                                                   break;

                                            }

                                                    //isobject = objstate.getobjectbytransactionid(s, "good-user", "better-password", @"http://127.0.0.1:18332");
                                                    //if (isobject.urn != null && find == true)
                                                    //{
                                                    //    isobject.transactionid = s;
                                                    //    foundobjects.add(isobject);
                                                    //    try { directory.delete(@"root\" + s, true); } catch { }

                                                    //    using (var db = new db(sup, @"root\found"))
                                                    //    {
                                                    //        db.put("found!" + root.blockdate.tostring("yyyymmddhhmmss") + "!" + root.signedby, "1");
                                                    //    }


                                                    //}
                                                    // try { System.IO.Directory.Delete(@"root\" + s, true); } catch { }

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
                                    }

                                
                            }
                            catch
                            {

                            }
                        



                        if (foundobjects.Count > 0)
                        {

                            this.Invoke((MethodInvoker)delegate
                            {
                               // AddToSearchResults(foundobjects);
                            });

                        }


                        this.Invoke((MethodInvoker)delegate
                        {
                            SystemSounds.Beep.Play();
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

        private void txtObjectAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
