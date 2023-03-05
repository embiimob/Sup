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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.NBitcoin;
using System.Windows;
using System.Windows.Controls;
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
                isBlue = parentForm.Controls.OfType<System.Windows.Forms.Button>().Any(b => b.Name == "btnLive" && b.BackColor == System.Drawing.Color.Blue);
            }
            catch { }

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
                btnReloadObject.PerformClick();

            }



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
            supPanel.Visible = false;


            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");
            Dictionary<string, string> profileAddress = new Dictionary<string, string> { };


            if (objstate.Owners != null)
            {


                OwnersPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                OwnersPanel.AutoScroll = true;

                int row = 0;
                foreach (KeyValuePair<string, long> item in objstate.Owners)
                {

                    // Create a table layout panel for each row
                    TableLayoutPanel rowPanel = new TableLayoutPanel
                    {
                        RowCount = 1,
                        ColumnCount = 2,
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Padding = new System.Windows.Forms.Padding(3)
                    };
                    // Add the width of the first column to fixed value and second to fill remaining space
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

                    // Create a link label for the key column
                    LinkLabel keyLabel = new LinkLabel();




                    string searchkey = item.Key;


                    if (!profileAddress.ContainsKey(searchkey))
                    {

                        PROState profile = PROState.GetProfileByAddress(searchkey, "good-user", "better-password", "http://127.0.0.1:18332");

                        if (profile.URN != null)
                        {
                            keyLabel.Text = TruncateAddress(profile.URN);

                        }
                        else
                        {
                            keyLabel.Text = TruncateAddress(item.Key);
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
                    keyLabel.Dock = DockStyle.Left; // set dock property for key label 

                    // Create a label for the value column
                    Label valueLabel = new Label
                    {
                        Text = item.Value.ToString(),
                        AutoSize = true,
                        Dock = DockStyle.Right // set dock property for value label 
                    };

                    // Add the link label and value label to the row panel
                    rowPanel.Controls.Add(keyLabel, 0, 0);
                    rowPanel.Controls.Add(valueLabel, 1, 0);

                    // Alternate the background color of the row panel
                    if (row % 2 == 0)
                    {
                        rowPanel.BackColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        rowPanel.BackColor = System.Drawing.Color.LightGray;
                    }

                    // Add the row panel to the main panel
                    OwnersPanel.Controls.Add(rowPanel);
                    row++;



                }

                long totalQty = objstate.Owners.Values.Sum();

                lblTotalOwnedDetail.Text = "total: " + totalQty.ToString("N0");


                foreach (KeyValuePair<string, DateTime> item in objstate.Creators)
                {

                    if (item.Value.Year > 1)

                    {

                        // Create a table layout panel for each row
                        TableLayoutPanel rowPanel = new TableLayoutPanel
                        {
                            RowCount = 1,
                            ColumnCount = 2,
                            Dock = DockStyle.Top,
                            AutoSize = true,
                            Padding = new System.Windows.Forms.Padding(3)
                        };


                        rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
                        rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 0));



                        LinkLabel keyLabel = new LinkLabel();

                        string searchkey = item.Key;
                        PROState profile = PROState.GetProfileByAddress(searchkey, "good-user", "better-password", "http://127.0.0.1:18332");

                        if (profile.URN != null)
                        {
                            keyLabel.Text = TruncateAddress(profile.URN);
                        }
                        else
                        {


                            keyLabel.Text = TruncateAddress(item.Key);
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


                        // Alternate the background color of the row panel
                        if (row % 2 == 0)
                        {
                            rowPanel.BackColor = System.Drawing.Color.White;
                        }
                        else
                        {
                            rowPanel.BackColor = System.Drawing.Color.LightGray;
                        }

                        // Add the row panel to the main panel
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
            // Clear controls if no messages have been displayed yet
            if (numMessagesDisplayed == 0)
            {
                supFlow.Controls.Clear();
            }
            supPanel.Visible = true;
            OwnersPanel.Visible = false;

            Dictionary<string, string[]> profileAddress = new Dictionary<string, string[]> { };
            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");
            int rownum = 1;

            var SUP = new Options { CreateIfMissing = true };

            using (var db = new DB(SUP, @"root\sup"))
            {
                string lastKey = db.Get("lastkey!" + _objectaddress);
                if (lastKey == null) { return; }
                LevelDB.Iterator it = db.CreateIterator();
                for (
                   it.Seek(lastKey);
                   it.IsValid() && it.KeyAsString().StartsWith(_objectaddress) && rownum <= numMessagesDisplayed + 10; // Only display next 10 messages
                    it.Prev()
                 )
                {
                    // Display only if rownum > numMessagesDisplayed to skip already displayed messages
                    if (rownum > numMessagesDisplayed)
                    {
                        string process = it.ValueAsString();

                        List<string> supMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);

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


                                if (imagelocation.StartsWith("BTC:") || imagelocation.StartsWith("MZC:"))
                                {
                                    if (imagelocation.Length > 64)
                                    {
                                        string transid = imagelocation.Substring(4, 64);
                                        imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagelocation.Replace("BTC:", "").Replace("MZC:", "").Replace(@"/", @"\");


                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            if (profile.Image.StartsWith("BTC:"))
                                            {
                                                Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                                            }
                                            else
                                            {
                                                Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (imagelocation.Length > 64)
                                    {
                                        string transid = imagelocation.Substring(0, 64);
                                        imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagelocation.Replace(@" / ", @"\");
                                        if (!System.IO.Directory.Exists("root/" + transid))
                                        {
                                            Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332");

                                        }
                                    }


                                    if (imagelocation.StartsWith("IPFS:"))
                                    {

                                        string transid = imagelocation.Substring(5, 46);
                                        if (!System.IO.Directory.Exists("ipfs/" + transid))
                                        {

                                            string isLoading;
                                            using (var db2 = new DB(SUP, @"ipfs"))
                                            {
                                                isLoading = db2.Get(transid);

                                            }

                                            if (isLoading != "loading")
                                            {
                                                using (var db2 = new DB(SUP, @"ipfs"))
                                                {

                                                    db2.Put(transid, "loading");

                                                }

                                                Task ipfsTask = Task.Run(() =>
                                                {
                                                    Process process2 = new Process();
                                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                    process2.StartInfo.Arguments = "get " + transid + @"-p -o ipfs\" + transid;
                                                    process2.Start();
                                                    process2.WaitForExit();

                                                    if (System.IO.File.Exists("ipfs/" + transid))
                                                    {
                                                        System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                        System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                        string fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                        if (fileName == "")
                                                        {
                                                            fileName = "artifact";

                                                        }
                                                        else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                        System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                                                    }


                                                    //attempt to pin fails silently if daemon is not running
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


                                                    using (var db2 = new DB(SUP, @"ipfs"))
                                                    {
                                                        db2.Delete(transid);

                                                    }
                                                });
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
                        System.Drawing.Color bgcolor;

                        if (rownum % 2 == 0)
                        {
                            bgcolor = System.Drawing.Color.White;
                        }
                        else
                        {
                            bgcolor = System.Drawing.Color.LightGray;
                        }

                        CreateRow(imagelocation, fromAddress, supMessagePacket[0], DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, bgcolor, supFlow);

                    }
                    rownum++;
                }
                it.Dispose();
            }

            // Update number of messages displayed
            numMessagesDisplayed += 20;

            supFlow.ResumeLayout();

        }

        void CreateRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp, string messageText, System.Drawing.Color bgcolor, FlowLayoutPanel layoutPanel)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 3,
                AutoSize = true,
                BackColor = System.Drawing.SystemColors.InactiveCaption,
                Padding = new System.Windows.Forms.Padding(0),
                Margin = new System.Windows.Forms.Padding(0)
            };
            // Add the width of the first column to fixed value and second to fill remaining space
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
            layoutPanel.Controls.Add(row);

            // Create a PictureBox with the specified image

            if (imageLocation != "")
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
                Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                Margin = new System.Windows.Forms.Padding(0),
                Dock = DockStyle.Bottom
            };
            row.Controls.Add(tstamp, 2, 0);



            TableLayoutPanel msg = new TableLayoutPanel
            {
                RowCount = 3,
                ColumnCount = 1,
                Dock = DockStyle.Top,
                BackColor = bgcolor,
                AutoSize = true,
                Margin = new System.Windows.Forms.Padding(0),
                Padding = new System.Windows.Forms.Padding(0)
            };
            // Add the width of the first column to fixed value and second to fill remaining space
            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 282));
            layoutPanel.Controls.Add(msg);

            // Create a Label with the message text
            Label tpadding = new Label
            {
                AutoSize = true,
                Text = " ",

                Margin = new System.Windows.Forms.Padding(0)
            };
            msg.Controls.Add(tpadding, 0, 0);

            // Create a Label with the message text
            Label message = new Label
            {
                AutoSize = true,
                Text = messageText,
                Margin = new System.Windows.Forms.Padding(0),
                TextAlign = System.Drawing.ContentAlignment.TopLeft
            };
            msg.Controls.Add(message, 1, 0);


            // Create a Label with the message text
            Label bpadding = new Label
            {
                AutoSize = true,
                Text = " ",
                Margin = new System.Windows.Forms.Padding(0)
            };
            msg.Controls.Add(bpadding, 2, 0);


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

        private void CopyAddressByNameClick(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void CopyAddressByTotalOwnedClick(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void CopyAddressByImageClick(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
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
                        if (objstate.URN.ToLower().StartsWith("ipfs:")) { urn = urn.Replace(@"\root\", @"\ipfs\"); }

                    }
                }


                string imgurn = "";
                if (objstate.Image != null)
                {
                    imgurn = objstate.Image;

                    if (!objstate.Image.ToLower().StartsWith("http"))
                    {
                        imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                        if (objstate.Image.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                    }
                }


                string uriurn = "";
                if (objstate.URI != null)
                {
                    uriurn = objstate.URI;

                    if (!objstate.URI.ToLower().StartsWith("http"))
                    {
                        uriurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URI.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                        if (objstate.URI.ToLower().StartsWith("ipfs:")) { uriurn = uriurn.Replace(@"\root\", @"\ipfs\"); }

                    }
                }


                DateTime urnblockdate = new DateTime();
                DateTime imgblockdate = new DateTime();
                DateTime uriblockdate = new DateTime();
                lblObjectCreatedDate.Text = objstate.CreatedDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                if (!File.Exists(imgurn))
                {
                    try
                    {

                        Match imgurnmatch = regexTransactionId.Match(imgurn);
                        transactionid = imgurnmatch.Value;

                        switch (objstate.Image.ToUpper().Substring(0, 4))
                        {
                            case "MZC:":
                                Task.Run(() =>
                                {
                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                    if (File.Exists(imgurn)) { imgPicture.Invoke(new Action(() => imgPicture.ImageLocation = imgurn)); }

                                    try
                                    {
                                        lblIMGBlockDate.Invoke(new Action(() => lblIMGBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }


                                });
                                break;
                            case "BTC:":
                                Task.Run(() =>
                                {

                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                    if (File.Exists(imgurn)) { imgPicture.Invoke(new Action(() => imgPicture.ImageLocation = imgurn)); }

                                    try
                                    {
                                        lblIMGBlockDate.Invoke(new Action(() => lblIMGBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }

                                });
                                break;
                            case "LTC:":
                                Task.Run(() =>
                                {

                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                    if (File.Exists(imgurn)) { imgPicture.Invoke(new Action(() => imgPicture.ImageLocation = imgurn)); }

                                    try
                                    {
                                        lblIMGBlockDate.Invoke(new Action(() => lblIMGBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }

                                });
                                break;
                            case "DOG:":
                                Task.Run(() =>
                                {
                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                    if (File.Exists(imgurn)) { imgPicture.Invoke(new Action(() => imgPicture.ImageLocation = imgurn)); }

                                    try
                                    {
                                        lblIMGBlockDate.Invoke(new Action(() => lblIMGBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }


                                });
                                break;
                            case "IPFS":

                                transactionid = objstate.Image.Substring(5, 46);
                                if (objstate.Image.Length == 51) { imgurn += @"\artifact"; }

                                if (!System.IO.Directory.Exists(@"ipfs/" + objstate.Image.Substring(5, 46) + "-build") && !System.IO.Directory.Exists(@"ipfs/" + objstate.Image.Substring(5, 46)))
                                {

                                    Task ipfsTask = Task.Run(() =>
                                    {
                                        Directory.CreateDirectory(@"ipfs/" + objstate.Image.Substring(5, 46) + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + objstate.Image.Substring(5, 46);
                                        process2.Start();
                                        process2.WaitForExit();

                                        if (System.IO.File.Exists("ipfs/" + objstate.Image.Substring(5, 46)))
                                        {
                                            System.IO.File.Move("ipfs/" + objstate.Image.Substring(5, 46), "ipfs/" + objstate.Image.Substring(5, 46) + "_tmp");

                                            string fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                            if (fileName == "")
                                            {
                                                fileName = "artifact";

                                            }
                                            else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                            Directory.CreateDirectory(@"ipfs/" + objstate.Image.Substring(5, 46));
                                            try { System.IO.File.Move("ipfs/" + objstate.Image.Substring(5, 46) + "_tmp", imgurn); } catch { }
                                        }
                                        Process process3 = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = @"ipfs\ipfs.exe",
                                                Arguments = "pin add " + objstate.Image.Substring(5, 46),
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                        Directory.Delete(@"ipfs/" + objstate.Image.Substring(5, 46) + "-build");
                                        if (File.Exists(imgurn)) { imgPicture.Invoke(new Action(() => imgPicture.ImageLocation = imgurn)); }

                                    });

                                }

                                break;
                            default:
                                Task.Run(() =>
                                {
                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                    if (File.Exists(imgurn)) { imgPicture.Invoke(new Action(() => imgPicture.ImageLocation = imgurn)); }


                                    try
                                    {
                                        lblIMGBlockDate.Invoke(new Action(() => lblIMGBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }

                                });
                                break;
                        }


                    }
                    catch { }
                }


                try
                {

                    Match urnmatch = regexTransactionId.Match(urn);
                    transactionid = urnmatch.Value;
                    Root root = new Root();
                    if (!File.Exists(urn))
                    {
                        switch (objstate.URN.Substring(0, 4))
                        {
                            case "MZC:":

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                if (File.Exists(urn)) { btnReloadObject.Invoke(new Action(() => btnReloadObject.PerformClick())); }
                                try { lblURNBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss"); } catch { }


                                break;
                            case "BTC:":

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                if (File.Exists(urn)) { btnReloadObject.Invoke(new Action(() => btnReloadObject.PerformClick())); }

                                try
                                {
                                    lblURNBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                }
                                catch { }


                                break;
                            case "LTC:":

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                if (File.Exists(urn)) { btnReloadObject.Invoke(new Action(() => btnReloadObject.PerformClick())); }
                                try
                                {
                                    lblURNBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                }
                                catch { }


                                break;
                            case "DOG:":

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                if (File.Exists(urn)) { btnReloadObject.Invoke(new Action(() => btnReloadObject.PerformClick())); }

                                try
                                {
                                    lblURNBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                }
                                catch { }

                                break;
                            case "IPFS":

                                if (objstate.URN.Length == 51) { urn += @"\artifact"; }

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
                                            try { System.IO.File.Move("ipfs/" + objstate.URN.Substring(5, 46) + "_tmp", urn); } catch { }
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

                                        if (File.Exists(urn)) { btnReloadObject.Invoke(new Action(() => btnReloadObject.PerformClick())); }


                                    });
                                }

                                break;
                            default:

                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                if (File.Exists(urn)) { btnReloadObject.Invoke(new Action(() => btnReloadObject.PerformClick())); }
                                try
                                {
                                    lblURNBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss");
                                }
                                catch { }


                                break;
                        }

                    }

                }
                catch { urn = imgurn; }


                try
                {

                    Match urimatch = regexTransactionId.Match(uriurn);
                    transactionid = urimatch.Value;

                    switch (objstate.URI.Substring(0, 4))
                    {
                        case "MZC:":
                            Task.Run(() =>
                            {
                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                {
                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                    try
                                    {
                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transactionid + @"/OBJ.json");
                                        Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);

                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }
                                }
                            });
                            break;
                        case "BTC:":
                            Task.Run(() =>
                            {
                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                {
                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");
                                    try
                                    {
                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transactionid + @"/OBJ.json");
                                        Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);

                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }
                                }
                            });
                            break;
                        case "LTC:":
                            Task.Run(() =>
                            {
                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                {
                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");
                                    try
                                    {
                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transactionid + @"/OBJ.json");
                                        Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);

                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }
                                }
                            });
                            break;
                        case "DOG:":
                            Task.Run(() =>
                            {
                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                {
                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");
                                    try
                                    {
                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }
                                }
                                else
                                {

                                    try
                                    {
                                        string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transactionid + @"/OBJ.json");
                                        Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);

                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }

                                }
                            });
                            break;
                        case "IPFS":
                            ipfsuri = uriurn.Replace(@"\root\", @"\ipfs\");
                            transactionid = objstate.URI.Substring(5, 46);
                            if (objstate.URI.Length == 51) { ipfsuri += @"\artifact"; }

                            if (!System.IO.Directory.Exists(@"ipfs/" + objstate.URI.Substring(5, 46) + "-build") && !System.IO.Directory.Exists(@"ipfs/" + objstate.URI.Substring(5, 46)))
                            {
                                Task ipfsTask = Task.Run(() =>
                                {
                                    Directory.CreateDirectory(@"ipfs/" + objstate.URI.Substring(5, 46) + "-build");
                                    Process process2 = new Process();
                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                    process2.StartInfo.Arguments = "get " + objstate.URI.Substring(5, 46) + @" -o ipfs\" + objstate.URI.Substring(5, 46);
                                    process2.Start();
                                    process2.WaitForExit();

                                    if (System.IO.File.Exists("ipfs/" + objstate.URI.Substring(5, 46)))
                                    {
                                        System.IO.File.Move("ipfs/" + objstate.URI.Substring(5, 46), "ipfs/" + objstate.URI.Substring(5, 46) + "_tmp");
                                        string fileName = objstate.URI.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                        if (fileName == "")
                                        {
                                            fileName = "artifact";

                                        }
                                        else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                        Directory.CreateDirectory(@"ipfs/" + objstate.URI.Substring(5, 46));
                                        System.IO.File.Move("ipfs/" + objstate.URI.Substring(5, 46) + "_tmp", @"ipfs/" + objstate.URI.Substring(5, 46) + @"/" + fileName);
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

                                    btnReloadObject.Invoke(new Action(() => btnReloadObject.PerformClick()));
                                    Directory.Delete(@"ipfs/" + objstate.URI.Substring(5, 46) + "-build");
                                });
                            }



                            break;
                        default:
                            Task.Run(() =>
                            {
                                if (!System.IO.Directory.Exists(@"root/" + transactionid))
                                {
                                    Root root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                    try
                                    {
                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }
                                }
                                else
                                {


                                    try
                                    {
                                        string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transactionid + @"/OBJ.json");

                                        Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);

                                        lblURIBlockDate.Invoke(new Action(() => lblURIBlockDate.Text = root.BlockDate.ToString("ddd, dd MMM yyyy hh:mm:ss")));
                                    }
                                    catch { }

                                }
                            });
                            break;
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
                    }
                }


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
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(viewerPath);
                        }




                        break;
                    case ".htm":
                    case ".html":
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

                                System.IO.Directory.Delete(Path.GetDirectoryName(urn), true);

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

                                try { potentialyUnsafeHtml = System.IO.File.ReadAllText(urn); } catch { }

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
                            Thread.Sleep(1000);
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(urn) + @"\urnviewer.html");
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
    }

}

