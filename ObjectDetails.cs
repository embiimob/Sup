using AngleSharp.Dom;
using Ganss.Xss;
using LevelDB;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web.NBitcoin;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using Label = System.Windows.Forms.Label;

namespace SUP
{
    public partial class ObjectDetails : Form
    {
        private string _objectaddress;
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
            if (input.Length <= 10)
            {
                return input;
            }
            else
            {
                return input.Substring(0, 5) + "..." + input.Substring(input.Length - 5);
            }
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            new FullScreenView(pictureBox1.ImageLocation).Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string src = lblURNFullPath.Text;
            try
            { System.Diagnostics.Process.Start(src); }
            catch { System.Media.SystemSounds.Exclamation.Play(); }
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Handle the event here

            // Get the link data
            var linkData = e.Link.LinkData;
            new ObjectBrowser((string)linkData).Show();

        }

        private void btnShowObjectDetails_Click(object sender, EventArgs e)
        {
            CreatorsPanel.Controls.Clear();
            OwnersPanel.Controls.Clear();

            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");

            if (objstate.Owners != null)
            {


                OwnersPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
                OwnersPanel.AutoScroll = true;

                int row = 0;
                DateTime urnblockdate = new DateTime();
                DateTime imgblockdate = new DateTime();
                DateTime uriblockdate = new DateTime();
                foreach (KeyValuePair<string, long> item in objstate.Owners)
                {
                    // Create a table layout panel for each row
                    TableLayoutPanel rowPanel = new TableLayoutPanel();
                    rowPanel.RowCount = 1;
                    rowPanel.ColumnCount = 2;
                    rowPanel.Dock = DockStyle.Top;
                    rowPanel.AutoSize = true;
                    rowPanel.Padding = new System.Windows.Forms.Padding(3);
                    // Add the width of the first column to fixed value and second to fill remaining space
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

                    // Create a link label for the key column
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

                    // Create a label for the value column
                    Label valueLabel = new Label();
                    valueLabel.Text = item.Value.ToString();
                    valueLabel.AutoSize = true;
                    valueLabel.Dock = DockStyle.Right; // set dock property for value label 

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


                foreach (string item in objstate.Creators)
                {
                    // Create a table layout panel for each row
                    TableLayoutPanel rowPanel = new TableLayoutPanel();
                    rowPanel.RowCount = 1;
                    rowPanel.ColumnCount = 2;
                    rowPanel.Dock = DockStyle.Top;
                    rowPanel.AutoSize = true;
                    rowPanel.Padding = new System.Windows.Forms.Padding(3);
                    // Add the width of the first column to fixed value and second to fill remaining space
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 0));


                    // Create a link label for the key column
                    LinkLabel keyLabel = new LinkLabel();

                    string searchkey = item;
                    PROState profile = PROState.GetProfileByAddress(searchkey, "good-user", "better-password", "http://127.0.0.1:18332");

                    if (profile.URN != null)
                    {
                        keyLabel.Text = TruncateAddress(profile.URN);
                    }
                    else
                    {


                        keyLabel.Text = TruncateAddress(item);
                    }
                    keyLabel.Links[0].LinkData = item;
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

            supPanel.Visible = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            supPanel.Visible = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            supFlow.Controls.Clear();

            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");

            var SUP = new Options { CreateIfMissing = true };

            using (var db = new DB(SUP, @"root/sup"))
            {
                LevelDB.Iterator it = db.CreateIterator();
                for (
                   it.Seek(this._objectaddress);
                   it.IsValid() && it.KeyAsString().StartsWith(this._objectaddress);
                    it.Next()
                 )


                {


                    string process = it.ValueAsString();

                    List<string> supMessagePacket = JsonConvert.DeserializeObject<List<string>>(process);

                    string message = System.IO.File.ReadAllText(@"root/" + supMessagePacket[1] + @"/MSG");

                    string fromAddress = supMessagePacket[0];
                    string imagelocation = "";

                    PROState profile = PROState.GetProfileByAddress(fromAddress, "good-user", "better-password", "http://127.0.0.1:18332");

                    if (profile.URN != null)
                    {
                        fromAddress = TruncateAddress(profile.URN);
                        imagelocation = profile.Image;





                        if (imagelocation.StartsWith("BTC:"))
                        {
                            string transid = imagelocation.Substring(4, 64);
                            imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagelocation.Replace("BTC:", "").Replace(@"/", @"\");


                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");


                            }
                            else
                            {
                                string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                                Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);

                            }

                        }
                        else
                        {
                            string transid = imagelocation.Substring(0, 64);
                            imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagelocation.Replace(@" / ", @"\");
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332");

                            }
                            else
                            {
                                string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                                Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);

                            }
                        }


                    }
                    else
                    { fromAddress = TruncateAddress(fromAddress); }


                    string tstamp = it.KeyAsString().Split('!')[1];

                    CreateRow(imagelocation, fromAddress, supMessagePacket[0], DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, supFlow);

                }
                it.Dispose();
            }
            if (supFlow.Controls.Count > 0) { supFlow.ScrollControlIntoView(supFlow.Controls[supFlow.Controls.Count - 1]); }
            supPanel.Visible = true;
        }

        void CreateRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp, string messageText, FlowLayoutPanel layoutPanel)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel();
            row.RowCount = 1;
            row.ColumnCount = 2;
            row.Dock = DockStyle.Bottom;
            row.AutoSize = true;
            row.Padding = new System.Windows.Forms.Padding(0);
            // Add the width of the first column to fixed value and second to fill remaining space
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));

            layoutPanel.Controls.Add(row);

            // Create a PictureBox with the specified image

            if (imageLocation != "")
            {
                PictureBox picture = new PictureBox();
                picture.Size = new System.Drawing.Size(50, 50);
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.ImageLocation = imageLocation;
                picture.Dock = DockStyle.Left;
                row.Controls.Add(picture, 0, 0);
            }

            // Create a LinkLabel with the owner name
            Label tstamp = new Label();
            tstamp.AutoSize = true;
            tstamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tstamp.Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss");
            tstamp.Dock = DockStyle.Right;
            row.Controls.Add(tstamp, 0, 1);


            // Create a LinkLabel with the owner name
            LinkLabel owner = new LinkLabel();
            owner.Text = ownerName;
            owner.AutoSize = true;
            owner.LinkClicked += (sender, e) => { Owner_LinkClicked(sender, e, ownerId); };
            owner.Dock = DockStyle.Left;
            owner.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            row.Controls.Add(owner, 0, 1);


            TableLayoutPanel msg = new TableLayoutPanel();
            msg.RowCount = 1;
            msg.ColumnCount = 1;
            msg.Dock = DockStyle.Bottom;
            msg.AutoSize = true;
            msg.Padding = new System.Windows.Forms.Padding(0);
            // Add the width of the first column to fixed value and second to fill remaining space
            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240));
            layoutPanel.Controls.Add(msg);


            // Create a Label with the message text
            Label message = new Label();
            message.AutoSize = true;
            message.Text = messageText;
            message.Dock = DockStyle.Fill;
            message.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            msg.Controls.Add(message, 0, 1);
        }

        void CreateTransRow(string fromName, string fromId, string toName, string toId, string action, string qty, string amount, DateTime timestamp, string status, FlowLayoutPanel layoutPanel)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel();
            row.RowCount = 1;
            row.ColumnCount = 2;
            row.Dock = DockStyle.Top;
            row.AutoSize = true;
            row.Padding = new System.Windows.Forms.Padding(0);

            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            layoutPanel.Controls.Add(row);

      
            LinkLabel fromname = new LinkLabel();
            fromname.Text = fromName;
            fromname.AutoSize = true;
            fromname.LinkClicked += (sender, e) => { Owner_LinkClicked(sender, e, fromId); };
            fromname.Dock = DockStyle.Left;
            fromname.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            row.Controls.Add(fromname, 0,0);

            LinkLabel toname = new LinkLabel();
            toname.Text = toName;
            toname.AutoSize = true;
            toname.LinkClicked += (sender, e) => { Owner_LinkClicked(sender, e, toId); };
            toname.Dock = DockStyle.Left;
            toname.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            row.Controls.Add(toname, 0, 1);



            TableLayoutPanel stats = new TableLayoutPanel();
            stats.RowCount = 1;
            stats.ColumnCount = 3;
            stats.Dock = DockStyle.Top;
            stats.AutoSize = true;
            stats.Padding = new System.Windows.Forms.Padding(0);
            // Add the width of the first column to fixed value and second to fill remaining space
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            layoutPanel.Controls.Add(stats);


            LinkLabel laction = new LinkLabel();
            laction.Text = action;
            laction.AutoSize = true;
            laction.Dock = DockStyle.Left;
            laction.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            stats.Controls.Add(laction, 0, 0);

     
            LinkLabel lqty = new LinkLabel();
            lqty.Text = qty;
            lqty.AutoSize = true;
            lqty.Dock = DockStyle.Left;
            lqty.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            stats.Controls.Add(lqty, 0, 1);
                    


            LinkLabel lamount = new LinkLabel();
            lamount.Text = amount;
            lamount.AutoSize = true;
            lamount.Dock = DockStyle.Left;
            lamount.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            stats.Controls.Add(lamount, 0, 2);


            

            TableLayoutPanel msg = new TableLayoutPanel();
            msg.RowCount = 1;
            msg.ColumnCount = 1;
            msg.Dock = DockStyle.Bottom;
            msg.AutoSize = true;
            msg.Padding = new System.Windows.Forms.Padding(0);
            // Add the width of the first column to fixed value and second to fill remaining space
            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240));
            layoutPanel.Controls.Add(msg);


            LinkLabel lstatus = new LinkLabel();
            lstatus.Text = status;
            lstatus.AutoSize = true;
            lstatus.Dock = DockStyle.Left;
            lstatus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            msg.Controls.Add(lstatus, 0, 0);


            // Create a LinkLabel with the owner name
            Label tstamp = new Label();
            tstamp.AutoSize = true;
            tstamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.77F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tstamp.Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss");
            tstamp.Dock = DockStyle.Right;
            msg.Controls.Add(tstamp, 1, 0);
        }

        void Owner_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e, string ownerId)
        {

            new ObjectBrowser(ownerId).Show();
        }

        private void txtName_Click(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void lblTotalOwnedMain_Click(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void imgPicture_Click(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private async void lblRefreshFrame_Click(object sender, EventArgs e)
        {
            transFlow.Visible = false;

            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");

            if (objstate.Owners != null)
            {

                string urn = objstate.URN.Replace("BTC:", @"root/");
                string imgurn = objstate.Image.Replace("BTC:", @"root/");
                DateTime urnblockdate = new DateTime();
                DateTime imgblockdate = new DateTime();
                DateTime uriblockdate = new DateTime();
                lblObjectCreatedDate.Text = objstate.CreatedDate.ToString("ddd, dd MMM yyyy hh:mm:ss");

                try
                {
                    if (objstate.Image.StartsWith("BTC:"))
                    {
                        string transid = objstate.Image.Substring(4, 64);
                        lblImageFullPath.Text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + imgurn.Replace(@"/", @"\");


                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                            imgblockdate = root.BlockDate;

                        }
                        else
                        {
                            string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                            Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                            imgblockdate = root.BlockDate;
                        }

                    }
                    else
                    {
                        string transid = objstate.Image.Substring(0, 64);
                        lblImageFullPath.Text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imgurn.Replace(@" / ", @"\");
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332");
                            imgblockdate = root.BlockDate;
                            if (root.BlockDate.Year == 1) { lblImageFullPath.Text = objstate.Image; };
                        }
                        else
                        {
                            string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                            Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                            imgblockdate = root.BlockDate;
                        }
                    }
                }
                catch { }

                imgPicture.ImageLocation = lblImageFullPath.Text;

                try
                {
                    if (objstate.URN.StartsWith("BTC:"))
                    {
                        string transid = objstate.URN.Substring(4, 64);

                        if (!System.IO.Directory.Exists(@"root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
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
                }
                catch { urn = objstate.Image.Replace("BTC:", @"root/"); }


                try
                {
                    if (objstate.URI.StartsWith("BTC:"))
                    {
                        string transid = objstate.URI.Substring(4, 64);

                        if (!System.IO.Directory.Exists(@"root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                            uriblockdate = root.BlockDate;
                        }
                        else
                        {
                            string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                            Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                            uriblockdate = root.BlockDate;
                        }


                    }
                    else
                    {
                        string transid = objstate.URN.Substring(0, 64);
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
                string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + urn.Replace(@"/", @"\");
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

                switch (extension)
                {
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

                        string htmlstring = "<html><body><embed src=\"" + filePath + "\" width=100% height=100%></body></html>";

                        try
                        {
                            System.IO.File.WriteAllText(@"root\" + transactionid + @"\urnviewer.html", htmlstring);
                            button1.Visible = true;
                        }
                        catch { }

                        await webviewer.EnsureCoreWebView2Async();
                        webviewer.CoreWebView2.Navigate(viewerPath);

                        break;
                    case ".htm":
                    case ".html":
                        chkRunTrustedObject.Visible = true;
                        flowPanel.Visible = false;
                        string browserPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + transactionid + @"\urnviewer.html";
                        flowPanel.Controls.Clear();

                        string htmlembed = "<html><body><embed src=\"" + filePath + "\" width=100% height=100%></body></html>";
                        string potentialyUnsafeHtml = System.IO.File.ReadAllText(filePath);

                        if (chkRunTrustedObject.Checked)
                        {
                           


                            string transid;
                            if (objstate.URN.Contains("BTC:"))
                            {
                                 transid = objstate.URN.Substring(4, 64);
                                try { System.IO.Directory.Delete(@"root/" + transid, true); } catch { }
                                P2FK.Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                            }
                            else
                            {
                                 transid = objstate.URN.Substring(0, 64);
                                try { System.IO.Directory.Delete(@"root/" + transid, true); } catch { }
                                P2FK.Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");
                            }

                            potentialyUnsafeHtml = System.IO.File.ReadAllText(filePath);
                                                       
                            var matches = regexTransactionId.Matches(potentialyUnsafeHtml);
                            foreach (Match transactionID in matches)
                            {


                                if (objstate.URN.Contains("BTC:"))
                                {
                                    
                                    P2FK.Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", "http://127.0.0.1:8332","0");
                                }
                                else
                                {

                                    P2FK.Root.GetRootByTransactionId(transactionID.Value, "good-user", "better-password", "http://127.0.0.1:18332");
                                }
                                

                            }

                            string _address = _objectaddress;
                            string _viewer = null;//to be implemented
                            string _viewername = null; //to be implemented
                            string _creator = objstate.Creators.Last();
                            int _owner = 0;//to be implemented
                            string _urn = HttpUtility.UrlEncode(objstate.URN);
                            string _uri = HttpUtility.UrlEncode(objstate.URI);
                            string _img = HttpUtility.UrlEncode(objstate.Image);

                            string querystring = "?address=" + _address + "&viewer=" + _viewer + "&viewername=" + _viewername + "&creator=" + _creator + "&owner=" + _owner + "&urn=" + _urn + "&uri=" + _uri + "&img=" + _img;
                            htmlembed = "<html><body><embed src=\"" + filePath + querystring + "\" width=100% height=100%></body></html>";
                        }
                        else
                        {
                            var sanitizer = new HtmlSanitizer();
                            var sanitized = sanitizer.Sanitize(potentialyUnsafeHtml);
                            System.IO.File.WriteAllText(filePath, sanitized);
                        }

                        try
                        {
                            System.IO.File.WriteAllText(@"root\" + transactionid + @"\urnviewer.html", htmlembed);
                            button1.Visible = true;
                        }
                        catch { }

                        await webviewer.EnsureCoreWebView2Async();
                        webviewer.CoreWebView2.Navigate(browserPath);

                        break;
                    default:
                        // Create a default "not supported" image

                        pictureBox1.ImageLocation = @"root/" + objstate.Image.Replace("BTC:", "");
                        // Add the default image to the flowPanel                        
                        break;
                }



            }
        }

 
        private void lblObjectCreatedDate_Click(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(_objectaddress);
        }

        private void txtdesc_Click(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(txtdesc.Text);
        }

        private void txtURN_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(txtURN.Text);
        }

        private void txtIMG_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(txtIMG.Text);
        }

        private void txtURI_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(txtURI.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            System.Windows.Clipboard.SetText(txtSupMessage.Text);
        }

        private void btnRefreshTransactions_Click(object sender, EventArgs e)
        {
            transFlow.Controls.Clear();

            try { System.IO.Directory.Delete(@"root/" + _objectaddress, true); } catch { }
            try { System.IO.Directory.Delete(@"root/event", true); } catch { }

            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332","111",true);

            var trans = new Options { CreateIfMissing = true };
            
            using (var db = new DB(trans, @"root/event"))
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

                    string fromAddress = transMessagePacket[0];
                    string toAddress = transMessagePacket[1];
                    string action = transMessagePacket[2];
                    string qty = transMessagePacket[3];
                    string amount = transMessagePacket[4];
                    string status = transMessagePacket[5];
           

                    PROState profile = PROState.GetProfileByAddress(fromAddress, "good-user", "better-password", "http://127.0.0.1:18332");

                    if (profile.URN != null)
                    {
                        fromAddress = TruncateAddress(profile.URN);

                        
                    }
                    else
                    { fromAddress = TruncateAddress(fromAddress); }


                   profile = PROState.GetProfileByAddress(toAddress, "good-user", "better-password", "http://127.0.0.1:18332");

                    if (profile.URN != null)
                    {
                        toAddress = TruncateAddress(profile.URN);

                    }
                    else
                    { toAddress = TruncateAddress(toAddress); }



                    string tstamp = it.KeyAsString().Split('!')[1];

                    CreateTransRow(fromAddress, transMessagePacket[0], toAddress, transMessagePacket[1], action, qty,amount, DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),status, transFlow);

                }
                it.Dispose();
            }
            if (transFlow.Controls.Count > 0) { transFlow.ScrollControlIntoView(transFlow.Controls[transFlow.Controls.Count - 1]); }
            
            transFlow.Visible = true;


        }
    }

}

