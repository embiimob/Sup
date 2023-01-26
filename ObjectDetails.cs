using LevelDB;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;
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

        private async void ObjectDetails_Load(object sender, EventArgs e)
        {
            this.Text = "Sup!? Object Details [ " + _objectaddress + " ]";


            OBJState objstate = OBJState.GetObjectByAddress(_objectaddress, "good-user", "better-password", "http://127.0.0.1:18332");

            if (objstate.Owners != null)
            {

                string urn = objstate.URN.Replace("BTC:", @"root/");
                string imgurn = objstate.Image.Replace("BTC:", @"root/");
                DateTime urnblockdate = new DateTime();
                DateTime imgblockdate = new DateTime();
                DateTime uriblockdate = new DateTime();
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
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332", "0");
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
                catch { }

                    imgPicture.ImageLocation = lblImageFullPath.Text;

                try
                {
                    if (objstate.URN.StartsWith("BTC:"))
                    {
                        string transid = objstate.URN.Substring(4, 64);

                        if (!System.IO.Directory.Exists("root/" + transid))
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
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332", "0");
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

                        if (!System.IO.Directory.Exists("root/" + transid))
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
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332", "0");
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
                lblurn.Text = "URN " + urnblockdate.ToString() + " " + objstate.URN;
                lblimg.Text = "IMG " + imgblockdate.ToString() + " " + objstate.Image;
                lbluri.Text = "URI " + uriblockdate.ToString() + " " + objstate.URI;
                txtdesc.Text = objstate.Description;
                txtName.Text = objstate.Name;
                long totalQty = objstate.Owners.Values.Sum();
                button7.PerformClick();


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
                    default:
                        // Create a default "not supported" image

                        pictureBox1.ImageLocation = @"root/" + objstate.Image.Replace("BTC:", "");
                        // Add the default image to the flowPanel                        
                        break;
                }

              

            }
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
            { System.Diagnostics.Process.Start(src); }catch{ System.Media.SystemSounds.Exclamation.Play(); }
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Handle the event here

            // Get the link data
            var linkData = e.Link.LinkData;

            // Check if the link data is of the correct type (string in this case)
            if (linkData is string)
            {
                // Display the link data in the message box
                System.Windows.MessageBox.Show((string)linkData);
            }
            else
            {
                // Display an error message
                System.Windows.MessageBox.Show("Invalid link data.");
            }
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

                string urn = objstate.URN.Replace("BTC:", @"root/");
                string imgurn = objstate.Image.Replace("BTC:", @"root/");

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
                        Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332", "0");
                        imgblockdate = root.BlockDate;
                    }
                    else
                    {
                        string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transid + @"/P2FK.json");
                        Root root = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                        imgblockdate = root.BlockDate;
                    }
                }

                imgPicture.ImageLocation = lblImageFullPath.Text;

                try
                {
                    if (objstate.URN.StartsWith("BTC:"))
                    {
                        string transid = objstate.URN.Substring(4, 64);

                        if (!System.IO.Directory.Exists("root/" + transid))
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
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332", "0");
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
                            imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagelocation.Replace("BTC:","").Replace(@"/", @"\");


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
                                Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332", "0");
                              
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

                        CreateRow(imagelocation, fromAddress, supMessagePacket[0], DateTime.ParseExact(tstamp,"yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, supFlow);
                    
                }
                it.Dispose();
            }

        }

        void CreateRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp , string messageText, FlowLayoutPanel layoutPanel)
        {

            // Create a table layout panel for each row
            TableLayoutPanel row = new TableLayoutPanel();
            row.RowCount = 1;
            row.ColumnCount = 2;
            row.Dock = DockStyle.Bottom;
            row.AutoSize = true;
            row.Padding = new System.Windows.Forms.Padding(0);
            // Add the width of the first column to fixed value and second to fill remaining space
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,100));
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

        void Owner_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e, string ownerId)
        {
            // Handle the LinkLabel's Click event and pass the owner id value
            Console.WriteLine("Owner id: " + ownerId);
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
    }
   
}

