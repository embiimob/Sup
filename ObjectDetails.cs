using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using SUP.P2FK;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

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

                if (objstate.Image.StartsWith("BTC:"))
                {
                    string transid = objstate.Image.Substring(4, 64);
                    lblImageFullPath.Text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + imgurn.Replace(@"/", @"\");


                    if (!System.IO.Directory.Exists("root/" + transid))
                    {
                        Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");

                    }
                    
                }
                else
                {
                    string transid = objstate.Image.Substring(0, 64);
                    lblImageFullPath.Text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\"+ imgurn.Replace(@" / ", @"\");
                    if (!System.IO.Directory.Exists("root/" + transid))
                    {
                        Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332", "0");

                    }
                }



                try
                {
                    if (objstate.URN.StartsWith("BTC:"))
                    {
                        string transid = objstate.URN.Substring(4, 64);

                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:8332", "0");

                        }
                       

                    }
                    else
                    {
                        string transid = objstate.URN.Substring(0, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, "good-user", "better-password", "http://127.0.0.1:18332", "0");

                        }


                    }
                }
                catch { urn = objstate.Image.Replace("BTC:", @"root/"); }


                // Get the file extension
                string extension = Path.GetExtension(urn).ToLower();
                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                Match match = regexTransactionId.Match(urn);
                string transactionid = match.Value;
                string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + urn.Replace(@"/", @"\");
                filePath = filePath.Replace(@"\root\root\", @"\root\");
                lblURNFullPath.Text = filePath;

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

                string creators = null;
                foreach (string creator in objstate.Creators.Skip(1))
                {

                    PROState profile = PROState.GetProfileByAddress(creator, "good-user", "better-password", "http://127.0.0.1:18332");

                    if (profile.URN != null)
                    {
                        creators = creators + "  " + profile.URN;
                    }
                    else
                    {
                        creators = creators + "  " + TruncateAddress(creator);
                    }

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
            System.Diagnostics.Process.Start(src);
        }
    }
}

