using AngleSharp.Dom;
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
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.NBitcoin;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace SUP
{
    public partial class ObjectDetails : Form
    {
        private readonly string _objectaddress;
        private bool isVerbose = false;
        public ObjectDetails(string objectaddress)
        {
            InitializeComponent();
            _objectaddress = objectaddress;
        }

        private void ObjectDetails_Load(object sender, EventArgs e)
        {
            this.Text = "Sup!? Object Details [ " + _objectaddress + " ]";
            btnReloadObject.PerformClick();

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

                lblTotalOwnedMain.Text = "x" + totalQty.ToString("N0");
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

        }

        private void ShowSupPanel(object sender, EventArgs e)
        {
            supPanel.Visible = true;
        }

        private void RefreshSupMessages(object sender, EventArgs e)
        {
            supFlow.SuspendLayout();
            supFlow.Controls.Clear();

            Dictionary<string, string[]> profileAddress = new Dictionary<string, string[]> { };
            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");
            int rownum = 1;

            var SUP = new Options { CreateIfMissing = true };

            using (var db = new DB(SUP, @"root\sup"))
            {
                LevelDB.Iterator it = db.CreateIterator();
                for (
                   it.Seek(this._objectaddress);
                   it.IsValid() && it.KeyAsString().StartsWith(_objectaddress);
                    it.Next()
                 )


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

                    rownum++;

                    CreateRow(imagelocation, fromAddress, supMessagePacket[0], DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, bgcolor, supFlow);

                }
                it.Dispose();
            }

            supFlow.ResumeLayout();
            supPanel.Visible = true;
        }

        void CreateRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp, string messageText, System.Drawing.Color bgcolor, FlowLayoutPanel layoutPanel)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 3,
                AutoSize = true,
                BackColor = bgcolor,
                Padding = new System.Windows.Forms.Padding(0),
                Margin = new System.Windows.Forms.Padding(0)
            };
            // Add the width of the first column to fixed value and second to fill remaining space
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
            layoutPanel.Controls.Add(row);

            // Create a PictureBox with the specified image

            if (imageLocation != "")
            {
                PictureBox picture = new PictureBox
                {
                    Size = new System.Drawing.Size(50, 50),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageLocation = imageLocation,
                    Margin = new System.Windows.Forms.Padding(0),
                    Dock = DockStyle.Bottom
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
            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 252));
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
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
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
                stats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126));
                stats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126));
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
            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 252));
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

            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");

            if (objstate.Owners != null)
            {

                string urn = "";
                if (objstate.URN != null)
                {
                    urn = objstate.URN;

                    if (!objstate.URN.ToLower().StartsWith("http"))
                    {
                        urn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URN.Replace("BTC:", "").Replace("MZC:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    }
                }


                string imgurn = "";
                if (objstate.Image != null)
                {
                    imgurn = objstate.Image;

                    if (!objstate.Image.ToLower().StartsWith("http"))
                    {
                        imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    }
                }


                string uriurn = "";
                if (objstate.URI != null)
                {
                    uriurn = objstate.URI;

                    if (!objstate.URI.ToLower().StartsWith("http"))
                    {
                        uriurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + objstate.URI.Replace("BTC:", "").Replace("MZC:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    }
                }


                DateTime urnblockdate = new DateTime();
                DateTime imgblockdate = new DateTime();
                DateTime uriblockdate = new DateTime();
                lblObjectCreatedDate.Text = objstate.CreatedDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                try
                {
                    if (objstate.Image.StartsWith("BTC:") || objstate.Image.StartsWith("MZC:") || objstate.Image.StartsWith("IPFS:"))
                    {


                        if (objstate.Image.StartsWith("IPFS:"))
                        {
                            imgurn = imgurn.Replace(@"\root\", @"\ipfs\");
                            string transid = objstate.Image.Substring(5, 46);
                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
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
                                        imgurn += @"\artifact";
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

                            }
                            if (objstate.Image.Length == 51) { imgurn += @"\artifact"; }
                        }

                        else
                        {

                            string transid = objstate.Image.Substring(4, 64);

                            if (!System.IO.Directory.Exists("root/" + transid))
                            {

                                if (objstate.Image.StartsWith("MZC:"))
                                {
                                    Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                    imgblockdate = root.BlockDate;
                                }
                                else
                                {
                                    Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                                    imgblockdate = root.BlockDate;
                                }



                            }
                            else
                            {
                                string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                                Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                                imgblockdate = root.BlockDate;
                            }
                        }

                    }
                    else
                    {

                        if (!objstate.Image.ToLower().StartsWith("http"))
                        {
                            string transid = objstate.Image.Substring(0, 64);

                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:18332");
                                imgblockdate = root.BlockDate;
                            }
                            else
                            {
                                string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                                Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                                imgblockdate = root.BlockDate;
                            }
                        }

                    }
                }
                catch { }



                try
                {
                    if (objstate.URN.StartsWith("BTC:") || objstate.URN.StartsWith("MZC:") || objstate.URN.StartsWith("IPFS:"))
                    {

                        if (objstate.URN.StartsWith("IPFS:"))
                        {
                            urn = urn.Replace(@"\root\", @"\ipfs\");
                            string transid = objstate.URN.Substring(5, 46);
                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + objstate.URN.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();

                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    string fileName = objstate.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "")
                                    {
                                        fileName = "artifact";
                                        urn += @"\artifact";
                                    }
                                    else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                                }


                                //attempt to pin fails silently if daemon is not running
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
                                                Arguments = "pin add " + transid,
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                    }
                                }

                            }

                            if (objstate.URN.Length == 51) { urn += @"\artifact"; }
                        }

                        else
                        {

                            string transid = objstate.URN.Substring(4, 64);

                            if (!System.IO.Directory.Exists(@"root/" + transid))
                            {

                                if (objstate.URN.StartsWith("MZC:"))
                                {
                                    Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                    urnblockdate = root.BlockDate;
                                }
                                else
                                {
                                    Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                                    urnblockdate = root.BlockDate;
                                }

                            }
                            else
                            {
                                string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                                Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                                urnblockdate = root.BlockDate;
                            }
                        }


                    }
                    else
                    {
                        if (!objstate.URN.ToLower().StartsWith("http"))
                        {
                            string transid = objstate.URN.Substring(0, 64);
                            if (!System.IO.Directory.Exists(@"root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332");
                                urnblockdate = root.BlockDate;
                            }
                            else
                            {
                                string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                                Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                                urnblockdate = root.BlockDate;
                            }


                        }
                        else
                        {

                            urn = objstate.URN;
                        }


                    }
                }
                catch { urn = imgurn; }


                try
                {
                    if (objstate.URI.StartsWith("BTC:") || objstate.URI.StartsWith("MZC:") || objstate.URI.StartsWith("IPFS:"))
                    {


                        if (objstate.URI.StartsWith("IPFS:"))
                        {
                            uriurn = uriurn.Replace(@"\root\", @"\ipfs\");
                            string transid = objstate.URI.Substring(5, 46);
                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + objstate.URI.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();

                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    string fileName = objstate.URI.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "")
                                    {
                                        fileName = "artifact";
                                        uriurn += @"\artifact";
                                    }
                                    else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                                }

                                //attempt to pin fails silently if daemon is not running
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
                                                Arguments = "pin add " + transid,
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                    }
                                }

                            }

                            if (objstate.URI.Length == 51) { uriurn += @"\artifact"; }
                        }

                        else
                        {

                            string transid = objstate.URI.Substring(4, 64);
                            if (!System.IO.Directory.Exists(@"root/" + transid))
                            {

                                if (objstate.URI.StartsWith("MZC:"))
                                {
                                    Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");
                                    uriblockdate = root.BlockDate;
                                }
                                else
                                {
                                    Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                                    uriblockdate = root.BlockDate;
                                }

                            }
                            else
                            {
                                string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                                Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                                uriblockdate = root.BlockDate;
                            }


                        }



                    }
                    else
                    {
                        string transid = objstate.URI.Substring(0, 64);
                        if (!System.IO.Directory.Exists(@"root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332");
                            uriblockdate = root.BlockDate;
                        }
                        else
                        {
                            string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                            Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                            uriblockdate = root.BlockDate;
                        }


                    }
                }
                catch { }


                // Get the file extension
                string extension = Path.GetExtension(urn).ToLower();
                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                Match match = regexTransactionId.Match(urn);
                string transactionid = match.Value;
                string filePath = urn;
                filePath = filePath.Replace(@"\root\root\", @"\root\");
                lblURNFullPath.Text = filePath;
                txtURN.Text = objstate.URN;
                txtIMG.Text = objstate.Image;
                txtURI.Text = objstate.URI;
                lblLicense.Text = objstate.License;
                lblProcessHeight.Text = objstate.ProcessHeight.ToString();
                lblLastChangedDate.Text = objstate.ChangeDate.ToString("ddd, dd MMM yyyy hh:mm:ss"); ;
                if (urnblockdate.Year > 1)
                {
                    lblURNBlockDate.Text = urnblockdate.ToString("ddd, dd MMM yyyy hh:mm:ss"); ;
                }
                if (imgblockdate.Year > 1)
                {
                    lblIMGBlockDate.Text = imgblockdate.ToString("ddd, dd MMM yyyy hh:mm:ss"); ;
                }
                if (uriblockdate.Year > 1)
                {
                    lblURIBlockDate.Text = uriblockdate.ToString("ddd, dd MMM yyyy hh:mm:ss"); ;
                }

                txtdesc.Text = objstate.Description;
                txtName.Text = objstate.Name;
                long totalQty = objstate.Owners.Values.Sum();


                if (supPanel.Visible)
                {
                    btnRefreshSup.PerformClick();
                }
                else
                {

                    btnRefreshOwners.PerformClick();
                }


                lblTotalOwnedMain.Text = "x" + totalQty.ToString("N0");

                switch (extension.ToLower())
                {
                    case ".glb":
                        //Show image in main box and show open file button
                        pictureBox1.ImageLocation = imgurn;
                        button1.Visible = true;
                        break;
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                        // Create a new PictureBox
                        pictureBox1.ImageLocation = urn;

                        break;
                    case ".mp4":
                    case ".avi":
                    case ".mp3":
                    case ".wav":
                    case ".pdf":


                        flowPanel.Visible = false;
                        string viewerPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + transactionid + @"\urnviewer.html";
                        flowPanel.Controls.Clear();

                        string htmlstring = "<html><body><embed src=\"" + urn + "\" width=100% height=100%></body></html>";

                        try
                        {
                            System.IO.File.WriteAllText(@"root\" + transactionid + @"\urnviewer.html", htmlstring);
                            button1.Visible = true;
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
                        string browserPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + transactionid + @"\urnviewer.html";
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


                            string transid;
                            if (objstate.URN.Contains("BTC:") || objstate.URN.Contains("MZC:") || objstate.URN.Contains("IPFS:"))
                            {


                                if (objstate.URN.StartsWith("IPFS:"))
                                {
                                    transid = objstate.URN.Substring(5, 46);
                                    try { System.IO.Directory.Delete(@"ipfs/" + transid, true); } catch { }


                                    Process process2 = new Process();
                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                    process2.StartInfo.Arguments = "get " + objstate.URN.Substring(5, 46) + @" -o ipfs\" + transid;
                                    process2.StartInfo.UseShellExecute = false;
                                    process2.StartInfo.CreateNoWindow = true;
                                    process2.Start();
                                    process2.WaitForExit();

                                    if (System.IO.File.Exists("ipfs/" + transid))
                                    {
                                        System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                        System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                        string fileName = objstate.URN.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                        if (fileName == "")
                                        {
                                            fileName = "artifact";
                                            urn += @"\artifact";
                                        }
                                        else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                        System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
                                    }

                                    //attempt to pin fails silently if daemon is not running
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
                                                    Arguments = "pin add " + transid,
                                                    UseShellExecute = false,
                                                    CreateNoWindow = true
                                                }
                                            };
                                            process3.Start();
                                        }
                                    }

                                    if (objstate.URN.Length == 51) { urn += @"\artifact"; }

                                }

                                else
                                {


                                    transid = objstate.URN.Substring(4, 64);
                                    try { System.IO.Directory.Delete(@"root/" + transid, true); } catch { }

                                    if (objstate.URN.Contains("MZC:"))
                                    {
                                        P2FK.Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:12832", "50");
                                    }
                                    else
                                    {
                                        P2FK.Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                                    }
                                }

                            }
                            else
                            {
                                transid = objstate.URN.Substring(0, 64);
                                try { System.IO.Directory.Delete(@"root/" + transid, true); } catch { }
                                P2FK.Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332");
                            }

                            try { potentialyUnsafeHtml = System.IO.File.ReadAllText(urn); } catch { }

                            var matches = regexTransactionId.Matches(potentialyUnsafeHtml);
                            foreach (Match transactionID in matches)
                            {


                                if (objstate.URN.Contains("BTC:") || objstate.URN.Contains("MZC:"))
                                {
                                    if (objstate.URN.Contains("MZC:"))
                                    {
                                        P2FK.Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", "http://127.0.0.1:12832", "50");
                                    }
                                    else
                                    {
                                        P2FK.Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                                    }
                                }
                                else
                                {

                                    P2FK.Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", "http://127.0.0.1:18332");
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
                        else
                        {
                            var sanitizer = new HtmlSanitizer();
                            var sanitized = sanitizer.Sanitize(potentialyUnsafeHtml);
                            try { System.IO.File.WriteAllText(urn, sanitized); } catch { }
                        }

                        try
                        {
                            System.IO.File.WriteAllText(@"root\" + transactionid + @"\urnviewer.html", htmlembed);
                            button1.Visible = true;
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(browserPath);
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            await webviewer.EnsureCoreWebView2Async();
                            webviewer.CoreWebView2.Navigate(browserPath);
                        }




                        break;
                    default:


                        pictureBox1.ImageLocation = imgurn;

                        break;
                }
                imgPicture.SuspendLayout();
                imgPicture.ImageLocation = imgurn;
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
            transFlow.Controls.Clear();

            Dictionary<string, string> profileAddress = new Dictionary<string, string> { };
            int rownum = 1;
            bool isverbose;

            // fetch current JSONOBJ from disk if it exists
            try
            {
                string JSONOBJ = System.IO.File.ReadAllText(@"root\" + _objectaddress + @"\P2FK.json");
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
                LevelDB.Iterator it = db.CreateIterator();
                for (
                   it.Seek(this._objectaddress);
                   it.IsValid() && it.KeyAsString().StartsWith(this._objectaddress);
                    it.Next()
                 )


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

                    rownum++;


                    CreateTransRow(fromAddress, transMessagePacket[0], toAddress, transMessagePacket[1], action, qty, amount, DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), status, bgcolor, transFlow);

                }
                it.Dispose();
            }
            if (transFlow.Controls.Count > 0) { transFlow.ScrollControlIntoView(transFlow.Controls[transFlow.Controls.Count - 1]); }

            transFlow.ResumeLayout();
            transFlow.Visible = true;



        }
    }

}

