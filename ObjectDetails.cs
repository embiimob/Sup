using Ganss.Xss;
using LevelDB;
using NBitcoin;
using Newtonsoft.Json;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.NBitcoin;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace SUP
{
    public partial class ObjectDetails : Form
    {
        private readonly string _objectaddress;
        private bool isVerbose = false;
        private int numMessagesDisplayed = 0;
        private int numChangesDisplayed = 0;
        private bool isUserControl = false;
        private bool isInitializing = true;
        public ObjectDetails(string objectaddress, bool isusercontrol = false)
        {
            InitializeComponent();
            _objectaddress = objectaddress;
            isUserControl = isusercontrol;

        }

        private void ObjectDetails_Load(object sender, EventArgs e)
        {
            // Check if the parent form has a button named "btnLive" with blue background color
            // Get a reference to the parent form
            Form parentForm = this.Owner;
            bool isBlue = false;

            // Check if the parent form has a button named "btnLive" with blue background color
            try
            {
                if (parentForm != null)
                {
                    isBlue = parentForm.Controls.OfType<System.Windows.Forms.Button>().Any(b => b.Name == "btnLive" && b.BackColor == System.Drawing.Color.Blue);
                }
            }
            catch
            {
            }

            if (isBlue)
            {
                // If there is a button with blue background color, show a message box
                DialogResult result = System.Windows.Forms.MessageBox.Show("disable Live monitoring to browse sup!? objects.\r\nignoring this warning may cause temporary data corruption that could require a full purge of the cache", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    // If the user clicks OK, close the form
                    this.Close();
                }
            }
            else
            {
                this.Text = String.Empty;

                if (!isUserControl)
                {
                    this.Text = "[ " + _objectaddress + " ]";
                    this.Height = this.Height + 210;
                    this.Width = this.Width + 210;
                    registrationPanel.Visible = true;
                }
                else
                {
                  //
                }

                btnReloadObject.PerformClick();
                lblPleaseStandBy.Visible = false;
            }
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


        private string TruncateAddress(string input)
        {
            if (input.Length <= 13)
            {
                return input;
            }
            else
            {
                return input.Substring(0, 5) + "..." + input.Substring(input.Length - 5);
            }
        }

        private void ShowFullScreenModeClick(object sender, EventArgs e)
        {
            new FullScreenView(pictureBox1.ImageLocation).Show();
        }

        private void LaunchURN(object sender, EventArgs e)
        {
            string src = lblURNFullPath.Text;
            try
            { System.Diagnostics.Process.Start(src); }
            catch { System.Media.SystemSounds.Exclamation.Play(); }
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            var linkData = e.Link.LinkData;
            new ObjectBrowser((string)linkData).Show();

        }

        private void ButtonShowObjectDetailsClick(object sender, EventArgs e)
        {
            CreatorsPanel.SuspendLayout();
            OwnersPanel.SuspendLayout();
            CreatorsPanel.Controls.Clear();
            OwnersPanel.Controls.Clear();
            RoyaltiesPanel.Controls.Clear();
            supPanel.Visible = false;
            btnDisco.Visible = false;


            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");
            Dictionary<string, string> profileAddress = new Dictionary<string, string> { };


            if (objstate.Owners != null)
            {


                OwnersPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                OwnersPanel.AutoScroll = true;

                int row = 0;
                foreach (KeyValuePair<string, long> item in objstate.Owners)
                {

                  
                    TableLayoutPanel rowPanel = new TableLayoutPanel
                    {
                        RowCount = 1,
                        ColumnCount = 2,
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Padding = new System.Windows.Forms.Padding(3)
                    };
                 
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230));
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

                   
                    Label valueLabel = new Label
                    {
                        Text = item.Value.ToString(),
                        AutoSize = true,
                        Dock = DockStyle.Right 
                    };

                 
                    rowPanel.Controls.Add(keyLabel, 0, 0);
                    rowPanel.Controls.Add(valueLabel, 1, 0);

                
                    if (row % 2 == 0)
                    {
                        rowPanel.BackColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        rowPanel.BackColor = System.Drawing.Color.LightGray;
                    }

                
                    OwnersPanel.Controls.Add(rowPanel);
                    row++;



                }

                long totalQty = objstate.Owners.Values.Sum();

                lblTotalOwnedDetail.Text = "total: " + totalQty.ToString("N0");


                ///royalties 

                if (objstate.Royalties != null)
                {


                    RoyaltiesPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                    RoyaltiesPanel.AutoScroll = true;

                    row = 0;
                    foreach (KeyValuePair<string, decimal> item in objstate.Royalties)
                    {


                        TableLayoutPanel rowPanel = new TableLayoutPanel
                        {
                            RowCount = 1,
                            ColumnCount = 2,
                            Dock = DockStyle.Top,
                            AutoSize = true,
                            Padding = new System.Windows.Forms.Padding(3)
                        };

                        rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230));
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


                        Label valueLabel = new Label
                        {
                            Text = item.Value.ToString(),
                            AutoSize = true,
                            Dock = DockStyle.Right
                        };


                        rowPanel.Controls.Add(keyLabel, 0, 0);
                        rowPanel.Controls.Add(valueLabel, 1, 0);


                        if (row % 2 == 0)
                        {
                            rowPanel.BackColor = System.Drawing.Color.White;
                        }
                        else
                        {
                            rowPanel.BackColor = System.Drawing.Color.LightGray;
                        }


                        RoyaltiesPanel.Controls.Add(rowPanel);
                        row++;



                    }

                    decimal totalRoytalties = objstate.Royalties.Values.Sum();

                    lblTotalRoyaltiesDetail.Text = "royalties: " + totalRoytalties.ToString();
                }

                ///


                foreach (KeyValuePair<string, DateTime> item in objstate.Creators)
                {

                    if (item.Value.Year > 1)

                    {

                       
                        TableLayoutPanel rowPanel = new TableLayoutPanel
                        {
                            RowCount = 1,
                            ColumnCount = 2,
                            Dock = DockStyle.Top,
                            AutoSize = true,
                            Padding = new System.Windows.Forms.Padding(3)
                        };


                        rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230));
                        rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 0));



                        LinkLabel keyLabel = new LinkLabel();

                        string searchkey = item.Key;
                        PROState profile = PROState.GetProfileByAddress(searchkey, "good-user", "better-password", "http://127.0.0.1:18332");

                        if (profile.URN != null)
                        {
                            keyLabel.Text = profile.URN;
                        }
                        else
                        {


                            keyLabel.Text = item.Key;
                        }
                        keyLabel.Links[0].LinkData = item.Key;
                        keyLabel.AutoSize = true;
                        keyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        keyLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                        keyLabel.LinkColor = System.Drawing.Color.Black;
                        keyLabel.ActiveLinkColor = System.Drawing.Color.Black;
                        keyLabel.VisitedLinkColor = System.Drawing.Color.Black;
                        keyLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkClicked);
                        keyLabel.Dock = DockStyle.Left; // set dock property for key label 
                        rowPanel.Controls.Add(keyLabel, 0, 0);


                   
                        if (row % 2 == 0)
                        {
                            rowPanel.BackColor = System.Drawing.Color.White;
                        }
                        else
                        {
                            rowPanel.BackColor = System.Drawing.Color.LightGray;
                        }

                    
                        CreatorsPanel.Controls.Add(rowPanel);
                        row++;
                    }
                }

            }
            CreatorsPanel.ResumeLayout();
            OwnersPanel.ResumeLayout();
            supPanel.Visible = false;
            OwnersPanel.Visible = true;

        }

        private void ShowSupPanel(object sender, EventArgs e)
        {
            supPanel.Visible = true;
        }

        private void RefreshSupMessages(object sender, EventArgs e)
        {
           
            if (numMessagesDisplayed == 0)
            {
                supFlow.Controls.Clear();
            }
            supPanel.Visible = true;
            OwnersPanel.Visible = false;
            btnDisco.Visible = true;

            Dictionary<string, string[]> profileAddress = new Dictionary<string, string[]> { };
            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");
            int rownum = 1;

            var SUP = new Options { CreateIfMissing = true };

            using (var db = new DB(SUP, @"root\"+ _objectaddress + @"\sup"))
            {
               
                LevelDB.Iterator it = db.CreateIterator();
                for (
                   it.SeekToLast();
                   it.IsValid()  && rownum <= numMessagesDisplayed + 10; // Only display next 10 messages
                    it.Prev()
                 )
                {
                    // Display only if rownum > numMessagesDisplayed to skip already displayed messages
                    if (rownum > numMessagesDisplayed)
                    {
                        string process = it.ValueAsString();

                        List<string> supMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);

                        try
                        {
                            string message = System.IO.File.ReadAllText(@"root/" + supMessagePacket[1] + @"/MSG").Replace("@" + _objectaddress, "");

                            string fromAddress = supMessagePacket[0];
                            string imagelocation = "";


                            if (!profileAddress.ContainsKey(fromAddress))
                            {

                                PROState profile = PROState.GetProfileByAddress(fromAddress, "good-user", "better-password", "http://127.0.0.1:18332");

                                if (profile.URN != null)
                                {
                                    fromAddress = TruncateAddress(profile.URN);
                                    imagelocation = profile.Image;

                                    string imgurn = "";
                                    string transid = "";

                                    if (profile.Image != null)
                                    {
                                        imgurn = profile.Image;

                                        if (!profile.Image.ToLower().StartsWith("http"))
                                        {
                                            imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + profile.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                                            if (profile.Image.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); if (profile.Image.Length == 51) { imgurn += @"\artifact"; } }
                                        }

                                        Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                                        Match imgurnmatch = regexTransactionId.Match(imgurn);
                                        transid = imgurnmatch.Value;
                                        imagelocation = imgurn;

                                        if (File.Exists(imgurn))
                                        {

                                        }
                                        else
                                        {

                                            if (profile.Image.LastIndexOf('.') > 0 && File.Exists("ipfs/" + transid + "/artifact" + profile.Image.Substring(profile.Image.LastIndexOf('.'))))
                                            {
                                                imagelocation = "ipfs/" + transid + "/artifact" + profile.Image.Substring(profile.Image.LastIndexOf('.'));
                                            }
                                            else
                                            {


                                                switch (profile.Image.ToUpper().Substring(0, 4))
                                                {
                                                    case "BTC:":
                                                        Task.Run(() =>
                                                              {
                                                                  if (!System.IO.Directory.Exists("root/" + transid))
                                                                  {
                                                                      Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                                                  }
                                                              });


                                                        break;
                                                    case "MZC:":

                                                        Task.Run(() =>
                                                        {
                                                            if (!System.IO.Directory.Exists("root/" + transid))
                                                            {
                                                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                                            }
                                                        });

                                                        break;
                                                    case "LTC:":

                                                        Task.Run(() =>
                                                        {
                                                            if (!System.IO.Directory.Exists("root/" + transid))
                                                            {
                                                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                                            }
                                                        });

                                                        break;
                                                    case "DOG:":
                                                        Task.Run(() =>
                                                           {
                                                               if (!System.IO.Directory.Exists("root/" + transid))
                                                               {
                                                                   Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                                               }
                                                           });

                                                        break;
                                                    case "IPFS":
                                                        transid = "empty";
                                                        try { transid = profile.Image.Substring(5, 46); } catch { }

                                                        if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                                        {


                                                            Task.Run(() =>
                                                            {
                                                                try { Directory.Delete("ipfs/" + transid, true); } catch { }
                                                                try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                                                Directory.CreateDirectory("ipfs/" + transid + "-build");
                                                                Process process2 = new Process();
                                                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                                process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                                                                process2.StartInfo.UseShellExecute = false;
                                                                process2.StartInfo.CreateNoWindow = true;
                                                                process2.Start();
                                                                process2.WaitForExit();
                                                                string fileName;
                                                                if (System.IO.File.Exists("ipfs/" + transid))
                                                                {
                                                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                                    Directory.CreateDirectory("ipfs/" + transid);

                                                                    try { System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName); }
                                                                    catch (System.ArgumentException ex)
                                                                    {

                                                                        System.IO.File.Move("ipfs/" + transid + "_tmp", "ipfs/" + transid + "/artifact" + profile.Image.Substring(profile.Image.LastIndexOf('.')));
                                                                        imagelocation = "ipfs/" + transid + "/artifact" + profile.Image.Substring(profile.Image.LastIndexOf('.'));

                                                                    }


                                                                }

                                                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                                {
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                                    try { System.IO.File.Move("ipfs/" + transid + "/" + transid, @"ipfs/" + transid + @"/" + fileName); }
                                                                    catch (System.ArgumentException ex)
                                                                    {

                                                                        System.IO.File.Move("ipfs/" + transid + "/" + transid, "ipfs/" + transid + "/artifact" + profile.Image.Substring(profile.Image.LastIndexOf('.')));
                                                                        imagelocation = "ipfs/" + transid + "/artifact" + profile.Image.Substring(profile.Image.LastIndexOf('.'));

                                                                    }
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


                                                            });

                                                        }

                                                        break;
                                                    case "HTTP":

                                                        break;


                                                    default:
                                                        Task.Run(() =>
                                                           {
                                                               if (!System.IO.Directory.Exists("root/" + transid))
                                                               {
                                                                   Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332");

                                                               }
                                                           });


                                                        break;
                                                }
                                            }

                                        }
                                    }


                                }
                                else
                                { fromAddress = TruncateAddress(fromAddress); }

                                string[] profilePacket = new string[2];

                                profilePacket[0] = fromAddress;
                                profilePacket[1] = imagelocation;
                                profileAddress.Add(supMessagePacket[0], profilePacket);

                            }
                            else
                            {
                                string[] profilePacket = new string[] { };
                                profileAddress.TryGetValue(fromAddress, out profilePacket);
                                fromAddress = profilePacket[0];
                                imagelocation = profilePacket[1];

                            }


                            string tstamp = it.KeyAsString().Split('!')[1];
                            System.Drawing.Color bgcolor = System.Drawing.Color.Black;
                            string unfilteredmessage = message;
                            message = Regex.Replace(message, "<<.*?>>", "");
                            CreateRow(imagelocation, fromAddress, supMessagePacket[0], DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, supMessagePacket[1], bgcolor, supFlow);

                            string pattern = "<<.*?>>";
                            MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                            foreach (Match match in matches)
                            {
                                string content = match.Value.Substring(2, match.Value.Length - 4);
                                if (!int.TryParse(content, out int id))
                                {
                                    AddImage(content);
                                }

                            }

                            TableLayoutPanel msg = new TableLayoutPanel
                            {
                                RowCount = 1,
                                ColumnCount = 1,
                                Dock = DockStyle.Top,
                                BackColor = bgcolor,
                                AutoSize = true,
                                Margin = new System.Windows.Forms.Padding(0, 0, 0, 40),
                                Padding = new System.Windows.Forms.Padding(0)
                            };

                            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supPanel.Width - 40));
                            supFlow.Controls.Add(msg);
                        }
                        catch { }//deleted message
                    }
                    rownum++;
                }
                it.Dispose();
            }

         
            numMessagesDisplayed += 20;

            supFlow.ResumeLayout();

        }

        void AddImage(string imagepath)
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
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 0),
                Padding = new System.Windows.Forms.Padding(0)

            };

            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supPanel.Width - 40));

                supFlow.Controls.Add(msg);
            
            PictureBox pictureBox = new PictureBox();

          

            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Width = supPanel.Width - 40;
            pictureBox.Height = supPanel.Width - 40;
            pictureBox.BackColor = System.Drawing.Color.Black;
            pictureBox.ImageLocation = imagelocation;
            msg.Controls.Add(pictureBox);

        }


        void CreateRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp, string messageText, string transactionid, System.Drawing.Color bgcolor, FlowLayoutPanel layoutPanel)
        {

        
            TableLayoutPanel row = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 4,
                AutoSize = true,
                BackColor = System.Drawing.Color.Black,
                Padding = new System.Windows.Forms.Padding(0),
                Margin = new System.Windows.Forms.Padding(0)
            };
          
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 142));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));
            layoutPanel.Controls.Add(row);

           

            if (imageLocation != null && ( File.Exists(imageLocation) || imageLocation.ToUpper().StartsWith("HTTP")))
            {
                PictureBox picture = new PictureBox
                {
                    Size = new System.Drawing.Size(70, 70),
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
                    Size = new System.Drawing.Size(70, 70),
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
                AutoSize = true
            };
            owner.LinkClicked += (sender, e) => { Owner_LinkClicked(ownerId); };
            owner.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            owner.Margin = new System.Windows.Forms.Padding(0);
            owner.Dock = DockStyle.Bottom;
            row.Controls.Add(owner, 1, 0);


            // Create a LinkLabel with the owner name
            Label tstamp = new Label
            {
                AutoSize = true,
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                Margin = new System.Windows.Forms.Padding(0),
                Dock = DockStyle.Bottom
            };
            row.Controls.Add(tstamp, 2, 0);

            Label deleteme = new Label
            {
                AutoSize = true,
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Text = "🗑",
                Margin = new System.Windows.Forms.Padding(0),
                Dock = DockStyle.Bottom
            };
            deleteme.Click += (sender, e) => { deleteme_LinkClicked(transactionid); };
            row.Controls.Add(deleteme, 3, 0);


            TableLayoutPanel msg = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 1,
                Dock = DockStyle.Top,
                BackColor = bgcolor,
                AutoSize = true,
                Margin = new System.Windows.Forms.Padding(0),
                Padding = new System.Windows.Forms.Padding(10, 20, 10, 20)
            };

            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supPanel.Width - 40));
            layoutPanel.Controls.Add(msg);

            // Create a Label with the message text
            Label message = new Label
            {
                AutoSize = true,
                Text = messageText,
                ForeColor = System.Drawing.Color.White,
                Margin = new System.Windows.Forms.Padding(0),
                TextAlign = System.Drawing.ContentAlignment.TopLeft
            };
            msg.Controls.Add(message);


        }

        void CreateTransRow(string fromName, string fromId, string toName, string toId, string action, string qty, string amount, DateTime timestamp, string status, System.Drawing.Color bgcolor, FlowLayoutPanel layoutPanel)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 3,
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new System.Windows.Forms.Padding(0),
                BackColor = bgcolor,
                Margin = new System.Windows.Forms.Padding(0)
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 107));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 62));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 107));
            layoutPanel.Controls.Add(row);


            LinkLabel fromname = new LinkLabel
            {
                Text = fromName,
                AutoSize = true
            };
            fromname.LinkClicked += (sender, e) => { Owner_LinkClicked(fromId); };
            fromname.Dock = DockStyle.Left;
            fromname.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            fromname.Margin = new System.Windows.Forms.Padding(0);
            row.Controls.Add(fromname, 0, 0);

            Label laction = new Label
            {
                Text = action,
                AutoSize = true,
                Dock = DockStyle.Left,
                TextAlign = System.Drawing.ContentAlignment.TopLeft,
                Margin = new System.Windows.Forms.Padding(0)
            };
            row.Controls.Add(laction, 1, 0);


            LinkLabel toname = new LinkLabel
            {
                Text = toName,
                AutoSize = true
            };
            toname.LinkClicked += (sender, e) => { Owner_LinkClicked(toId); };
            toname.Dock = DockStyle.Right;
            toname.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            toname.Margin = new System.Windows.Forms.Padding(0);
            row.Controls.Add(toname, 2, 0);


            if (qty.Length + amount.Length > 0)
            {


                TableLayoutPanel stats = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 2,
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    BackColor = bgcolor,
                    Padding = new System.Windows.Forms.Padding(0),
                    Margin = new System.Windows.Forms.Padding(0)
                };
                // Add the width of the first column to fixed value and second to fill remaining space
                stats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 138));
                stats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 138));
                layoutPanel.Controls.Add(stats);


                Label lqty = new Label
                {
                    Text = qty,
                    AutoSize = true,
                    TextAlign = System.Drawing.ContentAlignment.TopLeft,
                    Margin = new System.Windows.Forms.Padding(0)
                };
                stats.Controls.Add(lqty, 0, 0);


                Label lamount = new Label
                {
                    Text = amount,
                    AutoSize = true,
                    TextAlign = System.Drawing.ContentAlignment.TopLeft,
                    Margin = new System.Windows.Forms.Padding(0)
                };
                stats.Controls.Add(lamount, 1, 0);
            }


            TableLayoutPanel msg = new TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 1,
                Dock = DockStyle.Bottom,
                AutoSize = true,
                BackColor = bgcolor,
                Padding = new System.Windows.Forms.Padding(0),
                Margin = new System.Windows.Forms.Padding(0)
            };
            // Add the width of the first column to fixed value and second to fill remaining space
            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 276));
            layoutPanel.Controls.Add(msg);

            Label lstatus = new Label
            {
                Text = status,
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.TopLeft,
                Margin = new System.Windows.Forms.Padding(0)
            };
            msg.Controls.Add(lstatus, 0, 0);

            // Create a LinkLabel with the owner name
            Label tstamp = new Label
            {
                AutoSize = true,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                TextAlign = System.Drawing.ContentAlignment.TopLeft,
                Margin = new System.Windows.Forms.Padding(0)
            };
            msg.Controls.Add(tstamp, 0, 1);
        }

        void Owner_LinkClicked(string ownerId)
        {

            new ObjectBrowser(ownerId).Show();
        }

     
        private async void MainRefreshClick(object sender, EventArgs e)
        {
            transFlow.Visible = false;
            KeysFlow.Visible = false;
            txtdesc.Visible = true;
            KeysFlow.Controls.Clear();
            string transactionid;
            string ipfsurn = null;
            string ipfsimg = null;
            string ipfsuri = null;
            bool isWWW = false;
            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");

            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");

            if (objstate.Owners != null)
            {

                string urn = "";
                if (objstate.URN != null)
                {
                    urn = objstate.URN;

                    if (!objstate.URN.ToLower().StartsWith("http"))
                    {
                        urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URN.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                        if (objstate.URN.ToLower().StartsWith("ipfs:")) { urn = urn.Replace(@"\root\", @"\ipfs\"); if (objstate.URN.Length == 51) { urn += @"\artifact"; } }


                    }
                    else
                    {
                        webviewer.Visible = true;
                        await webviewer.EnsureCoreWebView2Async();
                        webviewer.CoreWebView2.Navigate(objstate.URN);
                        lblURNBlockDate.Text = "http get: " + DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                        isWWW = true;
                    }
                }


                string imgurn = "";
                if (objstate.Image != null)
                {
                    imgurn = objstate.Image;

                    if (!objstate.Image.ToLower().StartsWith("http"))
                    {
                        imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                        if (objstate.Image.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); if (objstate.Image.Length == 51) { imgurn += @"\artifact"; } }
                    }
                }


                string uriurn = "";
                if (objstate.URI != null)
                {
                    uriurn = objstate.URI;

                    if (!objstate.URI.ToLower().StartsWith("http"))
                    {
                        uriurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URI.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                        if (objstate.URI.ToLower().StartsWith("ipfs:")) { uriurn = uriurn.Replace(@"\root\", @"\ipfs\"); if (objstate.URI.Length == 51) { uriurn += @"\artifact"; } }

                    }
                }


                DateTime urnblockdate = new DateTime();
                DateTime imgblockdate = new DateTime();
                DateTime uriblockdate = new DateTime();
                lblObjectCreatedDate.Text = objstate.CreatedDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                try
                {

                    Match imgurnmatch = regexTransactionId.Match(imgurn);
                    transactionid = imgurnmatch.Value;
                    Root root = new Root();
                    if (imgurn != "")
                    {
                        switch (objstate.Image.ToUpper().Substring(0, 4))
                        {
                            case "MZC:":
                                 root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                    try
                                    {

                                        lblIMGBlockDate.Text = "mazacoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                               


                                break;
                            case "BTC:":

                                 root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                    try
                                    {

                                        lblIMGBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                                


                                break;
                            case "LTC:":


                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                    try
                                    {

                                        lblIMGBlockDate.Text = "litecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                               


                                break;
                            case "DOG:":
                               
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                    try
                                    {

                                        lblIMGBlockDate.Text = "dogecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                               


                                break;
                            case "IPFS":
                                string transid = "empty";
                                try { transid = objstate.Image.Substring(5, 46); } catch { }

                                if (!File.Exists(imgurn) && !File.Exists("ipfs/" + transid + "/artifact") && !File.Exists("ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'))))
                                {
                                    
                                    if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                    {
                                        try { Directory.Delete("ipfs/" + transid, true); } catch { }
                                        try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                        Directory.CreateDirectory("ipfs/" + transid + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                                        process2.StartInfo.UseShellExecute = false;
                                        process2.StartInfo.CreateNoWindow = true;
                                        process2.Start();
                                        process2.WaitForExit();
                                        string fileName;
                                        if (System.IO.File.Exists("ipfs/" + transid))
                                        {
                                            System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                            System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                            fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                            Directory.CreateDirectory("ipfs/" + transid);
                                           
                                            try { System.IO.File.Move("ipfs/" + transid + "_tmp", imgurn); }
                                            catch (System.ArgumentException ex)
                                            {
                                                
                                                    System.IO.File.Move("ipfs/" + transid + "_tmp", "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + transid + @"\artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));
                                                
                                            }


                                        }

                                        if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                        {
                                            fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }


                                            try { System.IO.File.Move("ipfs/" + transid + "/" + transid, imgurn); }
                                            catch (System.ArgumentException ex)
                                            {
                                                
                                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, "ipfs/" + transid + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')));
                                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + transid + @"\artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.'));
                                               
                                            }



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
                                }
                                else
                                {
                                    if (File.Exists("ipfs/" + transid + "/artifact")) { imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + transid + @"\artifact"; }
                                    else
                                    {
                                        if (File.Exists("ipfs/" + objstate.Image.Substring(5, 46) + "/artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')))) { imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + objstate.Image.Substring(5, 46) + @"\artifact" + objstate.Image.Substring(objstate.Image.LastIndexOf('.')); }

                                    }

                                }
                                break;
                            default:
                                if (!txtIMG.Text.ToUpper().StartsWith("HTTP") && transactionid != "")
                                {
                                   
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");

                                        try
                                        {

                                            lblIMGBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                        }
                                        catch { }
                                   
                                }

                                break;
                        }
                    }

                }
                catch { }



                try
                {
                    transactionid = "";
                    Match urnmatch = regexTransactionId.Match(urn);
                    transactionid = urnmatch.Value;
                    Root root = new Root();

                 
                        switch (objstate.URN.Substring(0, 4))
                        {
                            case "MZC:":

                               
                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                try
                                {

                                        lblURNBlockDate.Text = "mazacoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                               

                                break;
                            case "BTC:":

                              
                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                try
                                {

                                        lblURNBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                              


                                break;
                            case "LTC:":


                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                try
                                {

                                        lblURNBlockDate.Text = "litecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                              

                                break;
                            case "DOG:":

                               
                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                try
                                {

                                        lblURNBlockDate.Text = "dogecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                               


                                break;
                            case "IPFS":
                            if (!File.Exists(urn) && !File.Exists("ipfs/" + objstate.URN.Substring(5, 46) + "/artifact") && !File.Exists("ipfs/" + objstate.URN.Substring(5, 46) + "/artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.'))))
                            {

                                if (!System.IO.Directory.Exists(@"ipfs/" + objstate.URN.Substring(5, 46) + "-build"))
                                {

                                    Task ipfsTask = Task.Run(() =>
                                    {
                                        try { Directory.Delete("ipfs/" + objstate.URN.Substring(5, 46), true); } catch { }
                                        try { Directory.CreateDirectory("ipfs/" + objstate.URN.Substring(5, 46)); } catch { };
                                        Directory.CreateDirectory(@"ipfs/" + objstate.URN.Substring(5, 46) + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + objstate.URN.Substring(5, 46) + @" -o ipfs\" + objstate.URN.Substring(5, 46);
                                        process2.Start();
                                        process2.WaitForExit();

                                        string fileName;
                                        if (System.IO.File.Exists("ipfs/" + objstate.URN.Substring(5, 46)))
                                        {
                                            System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46), "ipfs/" + objstate.URN.Substring(5, 46) + "_tmp");
                                            System.IO.Directory.CreateDirectory("ipfs/" + objstate.URN.Substring(5, 46));
                                            fileName = objstate.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                            Directory.CreateDirectory("ipfs/" + objstate.URN.Substring(5, 46));
         
                                            try { System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46) + "_tmp", urn); }
                                            catch (System.ArgumentException ex)
                                            {
                                                
                                                    System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46) + "_tmp", "ipfs/" + objstate.URN.Substring(5, 46) + "/artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.')));
                                                    urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + objstate.URN.Substring(5, 46) + @"\artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.'));
                                               
                                            }



                                        }

                                        if (System.IO.File.Exists("ipfs/" + objstate.URN.Substring(5, 46) + "/" + objstate.URN.Substring(5, 46)))
                                        {
                                            fileName = objstate.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                            try { System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46) + "/" + objstate.URN.Substring(5, 46), urn); }
                                            catch (System.ArgumentException ex)
                                            {
                                                
                                                    System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46) + "/" + objstate.URN.Substring(5, 46), "ipfs/" + objstate.URN.Substring(5, 46) + "/artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.')));
                                                    urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + objstate.URN.Substring(5, 46) + @"\artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.'));
                                                
                                            }
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
                                                        Arguments = "pin add " + objstate.URN.Substring(5, 46),
                                                        UseShellExecute = false,
                                                        CreateNoWindow = true
                                                    }
                                                };
                                                process3.Start();
                                            }
                                        }


                                        try { Directory.Delete(@"ipfs/" + objstate.URN.Substring(5, 46) + "-build"); } catch { }



                                    });
                                }
                                else
                                {
                                    lblURNBlockDate.Text = "ipfs verified: " + System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                }
                            }
                            else
                            {
                                if (File.Exists("ipfs/" + objstate.URN.Substring(5, 46) + "/artifact")) { urn = "ipfs/" + objstate.URN.Substring(5, 46) + "/artifact"; }
                                else
                                {
                                    if (File.Exists("ipfs/" + objstate.URN.Substring(5, 46) + "/artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.')))) { urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + objstate.URN.Substring(5, 46) + @"\artifact" + objstate.URN.Substring(objstate.URN.LastIndexOf('.')); }

                                }
                            }
                            break;
                            default:
                                if (!txtURN.Text.ToUpper().StartsWith("HTTP"))
                                {
                                    if (transactionid != "")
                                    {
                                       
                                        root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");

                                        try
                                        {

                                                lblURNBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                            }
                                            catch { }
                                       
                                    }


                                }


                                break;
                        }
                    


                }
                catch { urn = imgurn; }


                try
                {
                    transactionid = "";
                    Root root = new Root();
                    Match urimatch = regexTransactionId.Match(uriurn);
                    transactionid = urimatch.Value;

                    if (uriurn != "")
                    {
                        switch (objstate.URI.Substring(0, 4))
                        {
                            case "MZC:":

                               
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                    try
                                    {

                                        lblURIBlockDate.Text = "mazacoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                                

                                break;
                            case "BTC:":

                               
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                    try
                                    {

                                        lblURIBlockDate.Text = "bitcoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                               

                                break;
                            case "LTC:":

                               
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");

                                    try
                                    {

                                        lblURIBlockDate.Text = "litecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                                


                                break;
                            case "DOG:":


                               
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                    try
                                    {

                                        lblURIBlockDate.Text = "dogecoin verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                    }
                                    catch { }
                               

                                break;
                            case "IPFS":


                                if (!System.IO.Directory.Exists(@"ipfs/" + objstate.URI.Substring(5, 46) + "-build") && !System.IO.Directory.Exists(@"ipfs/" + objstate.URI.Substring(5, 46)))
                                {


                                    Task ipfsTask = Task.Run(() =>
                                    {

                                        try { Directory.Delete("ipfs/" + objstate.URI.Substring(5, 46), true); } catch { }
                                        try { Directory.CreateDirectory("ipfs/" + objstate.URI.Substring(5, 46)); } catch { };
                                        Directory.CreateDirectory(@"ipfs/" + objstate.URI.Substring(5, 46) + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + objstate.URI.Substring(5, 46) + @" -o ipfs\" + objstate.URI.Substring(5, 46);
                                        process2.Start();
                                        process2.WaitForExit();

                                        string fileName;
                                        if (System.IO.File.Exists("ipfs/" + objstate.URI.Substring(5, 46)))
                                        {
                                            System.IO.File.Move("ipfs/" + objstate.URI.Substring(5, 46), "ipfs/" + objstate.URI.Substring(5, 46) + "_tmp");
                                            System.IO.Directory.CreateDirectory("ipfs/" + objstate.URI.Substring(5, 46));
                                            fileName = objstate.URI.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                            Directory.CreateDirectory("ipfs/" + objstate.URI.Substring(5, 46));
                                            System.IO.File.Move("ipfs/" + objstate.URI.Substring(5, 46) + "_tmp", uriurn);

                                            try { System.IO.File.Move("ipfs/" + objstate.URI.Substring(5, 46) + "_tmp", uriurn); }
                                            catch (System.ArgumentException ex)
                                            {
                                                
                                                    System.IO.File.Move("ipfs/" + objstate.URI.Substring(5, 46) + "_tmp", "ipfs/" + objstate.URI.Substring(5, 46) + "/artifact" + objstate.URI.Substring(objstate.URI.LastIndexOf('.')));
                                                    uriurn = "ipfs/" + objstate.URI.Substring(5, 46) + "/artifact" + objstate.URI.Substring(objstate.URI.LastIndexOf('.'));
                                                
                                            }


                                        }

                                        if (System.IO.File.Exists("ipfs/" + objstate.URI.Substring(5, 46) + "/" + objstate.URI.Substring(5, 46)))
                                        {
                                            fileName = objstate.URI.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                            try { System.IO.File.Move("ipfs/" + objstate.URI.Substring(5, 46) + "/" + objstate.URI.Substring(5, 46), uriurn); }
                                            catch (System.ArgumentException ex)
                                            {
                                                
                                                    System.IO.File.Move("ipfs/" + objstate.URI.Substring(5, 46) + "/" + objstate.URI.Substring(5, 46), "ipfs/" + objstate.URI.Substring(5, 46) + "/artifact" + objstate.URI.Substring(objstate.URI.LastIndexOf('.')));
                                                    uriurn = "ipfs/" + objstate.URI.Substring(5, 46) + "/artifact" + objstate.URI.Substring(objstate.URI.LastIndexOf('.'));
                                               
                                            }
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
                                                        Arguments = "pin add " + objstate.URI.Substring(5, 46),
                                                        UseShellExecute = false,
                                                        CreateNoWindow = true
                                                    }
                                                };
                                                process3.Start();
                                            }
                                        }


                                        Directory.Delete(@"ipfs/" + objstate.URI.Substring(5, 46) + "-build");



                                    });
                                }
                                else
                                {
                                    lblURIBlockDate.Text = "ipfs verified: " + DateTime.UtcNow.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                }

                                break;
                            default:
                                if (!txtURI.Text.ToUpper().StartsWith("HTTP") && transactionid != "")
                                {
                                    root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                   
                                        try
                                        {

                                            lblURIBlockDate.Text = "bitcoin-t verified: " + root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                                        }
                                        catch { }
                                    
                                }
                                break;
                        }
                    }

                }
                catch { }



                // Get the file extension
                string extension = Path.GetExtension(urn).ToLower();
                Match match = regexTransactionId.Match(urn);
                transactionid = match.Value;
                string filePath = urn;
                filePath = filePath.Replace(@"\root\root\", @"\root\");
                lblURNFullPath.Text = filePath;
                txtURN.Text = objstate.URN;
                txtIMG.Text = objstate.Image;
                txtURI.Text = objstate.URI;
                lblLicense.Text = objstate.License;



                var KEY = new Options { CreateIfMissing = true };

                using (var db = new DB(KEY, @"root\obj"))
                {
                    LevelDB.Iterator it = db.CreateIterator();
                    for (
                       it.Seek(this._objectaddress);
                       it.IsValid() && it.KeyAsString().StartsWith(_objectaddress + "!");  // && rownum <= numMessagesDisplayed + 10; // Only display next 10 messages
                        it.Next()
                     )
                    {
                        string keyaddress = it.KeyAsString().Substring(it.KeyAsString().IndexOf('!') + 1);
                        Base58.DecodeWithCheckSum(keyaddress, out byte[] payloadBytes);
                        keyaddress = IsAsciiText(payloadBytes);

                        if (keyaddress != null)
                        {

                            LinkLabel keyword = new LinkLabel
                            {
                                Text = keyaddress,
                                AutoSize = true
                            };


                            keyword.LinkClicked += (Ksender, b) => { Owner_LinkClicked("#" + keyaddress); };
                            keyword.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            keyword.Margin = new System.Windows.Forms.Padding(0);
                            keyword.Dock = DockStyle.Bottom;
                            KeysFlow.Controls.Add(keyword);

                        }


                    }
                    it.Dispose();
                }


                lblProcessHeight.Text = objstate.ProcessHeight.ToString();
                lblLastChangedDate.Text = objstate.ChangeDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                if (urnblockdate.Year > 1)
                {
                    lblURNBlockDate.Text = urnblockdate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                }
                if (imgblockdate.Year > 1)
                {
                    lblIMGBlockDate.Text = imgblockdate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                }
                if (uriblockdate.Year > 1)
                {
                    lblURIBlockDate.Text = uriblockdate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                }

                txtdesc.Text = objstate.Description;
                txtName.Text = objstate.Name;
                long totalQty = objstate.Owners.Values.Sum();


                if (OwnersPanel.Visible)
                {

                    btnRefreshOwners.PerformClick();

                }
                else
                {
                    btnRefreshSup.PerformClick();

                }

                if (!isUserControl) { registrationPanel.Visible = true; }


                OBJState isOfficial = OBJState.GetObjectByURN(objstate.URN, "good-user", "better-password", "http://127.0.0.1:18332");
                if (isOfficial.URN != null)
                {
                    if (isOfficial.Creators.First().Key != this._objectaddress)
                    {
                        txtOfficialURN.Text = isOfficial.Creators.First().Key;
                        btnLaunchURN.Visible = false;
                        btnOfficial.Visible = true;
                    }
                    else
                    {

                        lblOfficial.Visible = true;
                        System.Windows.Forms.ToolTip myTooltip = new System.Windows.Forms.ToolTip();
                        myTooltip.SetToolTip(lblOfficial, isOfficial.URN);

                    }
                }


                switch (extension.ToLower())
                {
                    case "":

                        if (File.Exists(urn))
                        {
                            pictureBox1.ImageLocation = urn;
                        }
                        else
                        {
                            lblPleaseStandBy.Text = txtURN.Text;
                            lblPleaseStandBy.Visible = true;
                        }
                        break;
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
                        if (File.Exists(imgurn))
                        {

                            pictureBox1.ImageLocation = imgurn;
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


                        if (btnOfficial.Visible == false)
                        {
                            btnLaunchURN.Visible = true;
                            lblWarning.Visible = true;
                        }
                        break;

                    case ".glb":
                        //Show image in main box and show open file button
                        pictureBox1.SuspendLayout();
                        if (File.Exists(imgurn))
                        {

                            pictureBox1.ImageLocation = imgurn;
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
                        if (btnOfficial.Visible == false) { btnLaunchURN.Visible = true; }
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


                        if (btnOfficial.Visible == false) { btnLaunchURN.Visible = true; }
                        break;
                    case ".mp4":
                    case ".avi":
                    case ".mp3":
                    case ".wav":
                    case ".pdf":


                        flowPanel.Visible = false;
                        string viewerPath = Path.GetDirectoryName(urn) + @"\urnviewer.html";
                        flowPanel.Controls.Clear();

                        string htmlstring = "<html><body><embed src=\"" + urn + "\" width=100% height=100%></body></html>";

                        try
                        {
                            System.IO.File.WriteAllText(Path.GetDirectoryName(urn) + @"\urnviewer.html", htmlstring);
                            if (btnOfficial.Visible == false) { btnLaunchURN.Visible = true; }
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(viewerPath);
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            try
                            {
                                await webviewer.EnsureCoreWebView2Async();
                                webviewer.CoreWebView2.Navigate(viewerPath);
                            }
                            catch { }
                        }




                        break;
                    case ".htm":
                    case ".html":
                        if (isWWW == false)
                        {
                            chkRunTrustedObject.Visible = true;
                            flowPanel.Visible = false;
                            flowPanel.Controls.Clear();

                            string htmlembed = "<html><body><embed src=\"" + urn + "\" width=100% height=100%></body></html>";
                            string potentialyUnsafeHtml = "";

                            try
                            {
                                potentialyUnsafeHtml = System.IO.File.ReadAllText(urn);

                            }
                            catch { }


                            if (chkRunTrustedObject.Checked)
                            {
                                try
                                {

                                    try { System.IO.Directory.Delete(Path.GetDirectoryName(urn), true); } catch { }

                                    switch (objstate.URN.Substring(0, 4))
                                    {
                                        case "MZC:":
                                            if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                            {
                                                Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                            }
                                            break;
                                        case "BTC:":
                                            if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                            {
                                                Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                            }
                                            break;
                                        case "LTC:":
                                            if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                            {
                                                Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                            }
                                            break;
                                        case "DOG:":
                                            if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                            {
                                                Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                            }
                                            break;
                                        case "IPFS":
                                            ipfsurn = urn;
                                            if (objstate.URN.Length == 51) { ipfsurn += @"\artifact"; }

                                            if (!System.IO.Directory.Exists(@"ipfs/" + objstate.URN.Substring(5, 46) + "-build") && !System.IO.Directory.Exists(@"ipfs/" + objstate.URN.Substring(5, 46)))
                                            {


                                                Task ipfsTask = Task.Run(() =>
                                                {
                                                    Directory.CreateDirectory(@"ipfs/" + objstate.URN.Substring(5, 46) + "-build");
                                                    Process process2 = new Process();
                                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                    process2.StartInfo.Arguments = "get " + objstate.URN.Substring(5, 46) + @" -o ipfs\" + objstate.URN.Substring(5, 46);
                                                    process2.Start();
                                                    process2.WaitForExit();

                                                    if (System.IO.File.Exists("ipfs/" + objstate.URN.Substring(5, 46)))
                                                    {
                                                        System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46), "ipfs/" + objstate.URN.Substring(5, 46) + "_tmp");

                                                        string fileName = objstate.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                        if (fileName == "")
                                                        {
                                                            fileName = "artifact";
                                                        }
                                                        else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                        Directory.CreateDirectory(@"ipfs/" + objstate.URN.Substring(5, 46));
                                                        try { System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46) + "_tmp", ipfsurn); } catch { }
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
                                                                    Arguments = "pin add " + objstate.URN.Substring(5, 46),
                                                                    UseShellExecute = false,
                                                                    CreateNoWindow = true
                                                                }
                                                            };
                                                            process3.Start();
                                                        }
                                                    }

                                                    Directory.Delete(@"ipfs/" + objstate.URN.Substring(5, 46) + "-build");

                                                });
                                            }

                                            break;
                                        default:
                                            if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                            {
                                                Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                            }
                                            break;
                                    }
                                    try
                                    {
                                        if (Uri.TryCreate(urn, UriKind.Absolute, out Uri uri) && uri.Scheme == Uri.UriSchemeHttp)
                                        {
                                            using (var client = new WebClient())
                                            {
                                                potentialyUnsafeHtml = client.DownloadString(uri);
                                                System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
                                                byte[] hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(potentialyUnsafeHtml));
                                                urn = @"root\" + BitConverter.ToString(hashValue).Replace("-", String.Empty) + @"\index.html";
                                            }

                                        }
                                        else
                                        {
                                            potentialyUnsafeHtml = System.IO.File.ReadAllText(urn);
                                        }
                                    }
                                    catch { }

                                    var matches = regexTransactionId.Matches(potentialyUnsafeHtml);
                                    foreach (Match transactionID in matches)
                                    {

                                        switch (objstate.URN.Substring(0, 4))
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

                                    string _address = _objectaddress;
                                    string _viewer = objstate.Owners.Last().Key;
                                    string _viewername = null; //to be implemented
                                    string _creator = objstate.Creators.Last().Key;
                                    int _owner = objstate.Owners.Count();
                                    string _urn = HttpUtility.UrlEncode(objstate.URN);
                                    string _uri = HttpUtility.UrlEncode(objstate.URI);
                                    string _img = HttpUtility.UrlEncode(objstate.Image);

                                    string querystring = "?address=" + _address + "&viewer=" + _viewer + "&viewername=" + _viewername + "&creator=" + _creator + "&owner=" + _owner + "&urn=" + _urn + "&uri=" + _uri + "&img=" + _img;
                                    htmlembed = "<html><body><embed src=\"" + urn + querystring + "\" width=100% height=100%></body></html>";


                                }
                                catch { }



                            }
                            else
                            {
                                var sanitizer = new HtmlSanitizer();
                                var sanitized = sanitizer.Sanitize(potentialyUnsafeHtml);
                                try { System.IO.File.WriteAllText(urn, sanitized); } catch { }
                            }

                            try
                            {
                                System.IO.File.WriteAllText(Path.GetDirectoryName(urn) + @"\urnviewer.html", htmlembed);
                                if (btnOfficial.Visible == false) { btnLaunchURN.Visible = true; }

                                await webviewer.EnsureCoreWebView2Async();
                                webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(urn) + @"\urnviewer.html");
                            }
                            catch
                            {
                                Thread.Sleep(500);
                                try
                                {
                                    await webviewer.EnsureCoreWebView2Async();
                                    webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(urn) + @"\urnviewer.html");
                                }
                                catch { }
                            }

                        }
                        break;
                    default:

                        pictureBox1.Invoke(new Action(() => pictureBox1.ImageLocation = imgurn));
                        if (btnOfficial.Visible == false)
                        {
                            btnLaunchURN.Visible = true;
                            lblWarning.Visible = true;
                        }
                        break;
                }


                imgPicture.SuspendLayout();
                if (File.Exists(imgurn))
                {

                    imgPicture.ImageLocation = imgurn;
                }
                else
                {
                    Random rnd = new Random();
                    string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                    if (gifFiles.Length > 0)
                    {
                        int randomIndex = rnd.Next(gifFiles.Length);
                        string randomGifFile = gifFiles[randomIndex];

                        imgPicture.ImageLocation = randomGifFile;

                    }
                    else
                    {
                        try
                        {
                            imgPicture.ImageLocation = @"includes\HugPuddle.jpg";
                        }
                        catch { }
                    }


                }

                imgPicture.ResumeLayout();
                if (lblOfficial.Visible) { lblOfficial.Refresh(); }


            }
        }


        private void CopyAddressByCreatedDateClick(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void CopyDescriptionByDescriptionClick(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(txtdesc.Text);
        }


        private void ButtonRefreshTransactionsClick(object sender, EventArgs e)
        {

            transFlow.SuspendLayout();
            txtdesc.Visible = false;
            registrationPanel.Visible = false;
            // Clear controls if no messages have been displayed yet
            if (numChangesDisplayed == 0)
            {
                transFlow.Controls.Clear();
            }

            int rownum = 1;
            bool isverbose;

            // fetch current JSONOBJ from disk if it exists
            try
            {
                string JSONOBJ = System.IO.File.ReadAllText(@"root\" + _objectaddress + @"\OBJ.json");
                OBJState objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);
                if (objectState.Verbose) { isverbose = true; }
                else
                {
                    try { System.IO.Directory.Delete(@"root/" + _objectaddress, true); isVerbose = true; } catch { }
                }
            }
            catch { }

            OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332", "111", true);


            var trans = new Options { CreateIfMissing = true };

            using (var db = new DB(trans, @"root\event"))
            {
                string lastKey = db.Get("lastkey!" + _objectaddress);
                if (lastKey == null) { return; }
                LevelDB.Iterator it = db.CreateIterator();
                for (
                   it.Seek(lastKey);
                  it.IsValid() && it.KeyAsString().StartsWith(_objectaddress) && rownum <= numChangesDisplayed + 20;
                    it.Prev()
                 )


                {
                    if (rownum > numChangesDisplayed)
                    {

                        string process = it.ValueAsString();

                        List<string> transMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);

                        string fromAddress = TruncateAddress(transMessagePacket[0]);
                        string toAddress = TruncateAddress(transMessagePacket[1]);
                        string action = transMessagePacket[2];
                        string qty = transMessagePacket[3];
                        string amount = transMessagePacket[4];
                        string status = transMessagePacket[5];
                        string tstamp = it.KeyAsString().Split('!')[1];

                        System.Drawing.Color bgcolor;
                        if (rownum % 2 == 0)
                        {
                            bgcolor = System.Drawing.Color.White;
                        }
                        else
                        {
                            bgcolor = System.Drawing.Color.LightGray;
                        }




                        CreateTransRow(fromAddress, transMessagePacket[0], toAddress, transMessagePacket[1], action, qty, amount, DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), status, bgcolor, transFlow);

                    }
                    rownum++;
                }
                it.Dispose();
            }

            numChangesDisplayed += 10;
            transFlow.ResumeLayout();
            transFlow.Visible = true;
            KeysFlow.Visible = true;



        }


        public static string IsAsciiText(byte[] data)
        {
            // Check each byte to see if it's in the ASCII range
            for (int i = 0; i < 20; i++)
            {
                if (data[i] < 0x20 || data[i] > 0x7E)
                {
                    return null;
                }
            }

            return Encoding.ASCII.GetString(data).Replace("#", "").Substring(1);
        }

        private void btnOfficial_Click(object sender, EventArgs e)
        {
            new ObjectDetails(txtOfficialURN.Text).Show();
        }

        private void imgPicture_Validated(object sender, EventArgs e)
        {
            lblOfficial.Refresh();
        }

        private void imgPicture_Click(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void txtName_Click(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void lblLaunchURI_Click(object sender, EventArgs e)
        {
            string src = txtURI.Text;
            try
            { System.Diagnostics.Process.Start(src); }
            catch { System.Media.SystemSounds.Exclamation.Play(); }
        }

        private void btnBurn_Click(object sender, EventArgs e)
        {
            new ObjectBurn(_objectaddress).Show();
        }

        private void btnGive_Click(object sender, EventArgs e)
        {
            new ObjectGive(_objectaddress).Show();
        }

        private void btnDisco_Click(object sender, EventArgs e)
        {
            DiscoBall disco = new DiscoBall("", "", _objectaddress, imgPicture.ImageLocation, false);
            disco.StartPosition = FormStartPosition.CenterScreen;
            disco.Show(this);
            disco.Focus();
        }
    }

}

