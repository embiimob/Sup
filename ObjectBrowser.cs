using LevelDB;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

namespace SUP
{

    public partial class ObjectBrowser : Form
    {
        Button lastClickedButton;
        private readonly string _objectaddress;
        private List<String> SearchHistory = new List<String>();
        private int SearchId = 0;
        public ObjectBrowser(string objectaddress)
        {
            InitializeComponent();
            if (objectaddress != null)
            {
                _objectaddress = objectaddress;
            }
            else
            { _objectaddress = ""; }
        }

        private void GetObjectsbyAddress(string address)
        {

            string profileCheck = address;
            PROState searchprofile = PROState.GetProfileByAddress(address, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (searchprofile.URN != null)
            {
                linkLabel1.Text = searchprofile.URN;
                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(address, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                if (searchprofile.URN != null)
                {
                    linkLabel1.Text = TruncateAddress(searchprofile.Creators.First());
                    linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
                    profileCheck = searchprofile.Creators.First();
                }
                else
                {
                    linkLabel1.Text = "anon";
                    linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;

                }
            }



            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();


            List<OBJState> createdObjects = OBJState.GetObjectsByAddress(profileCheck, txtLogin.Text, txtPassword.Text, txtUrl.Text);


            foreach (OBJState objstate in createdObjects)
            {
                if (objstate.Owners != null)
                {

                    FoundObjectControl foundObject = new FoundObjectControl();

                    switch (objstate.Image.ToUpper().Substring(0, 4))
                    {
                        case "BTC:":
                            string transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                            break;
                        case "MZC:":
                            transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("MZC:", @"root/");
                            break;
                        case "IPFS":
                            transid = objstate.Image.Substring(5, 46);

                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
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
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
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
                                                Arguments = "pin add " + transid,
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                    }
                                }
                            }
                            if (objstate.Image.Length == 51)
                            { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                            else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }

                            break;
                        case "HTTP":
                            foundObject.ObjectImage.ImageLocation = objstate.Image;
                            break;


                        default:
                            transid = objstate.Image.Substring(0, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                            }
                            foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                            break;
                    }
                    foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;
                    foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";

                    foreach (KeyValuePair<string, DateTime> creator in objstate.Creators)
                    {

                        if (creator.Value.Year > 1)
                        {
                            PROState profile = PROState.GetProfileByAddress(creator.Key, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                            if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                            {


                                foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                                foundObject.ObjectCreators.Links.Add(0, creator.Key.Length, creator.Key);
                            }
                            else
                            {


                                if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                                {
                                    foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                    foundObject.ObjectCreators2.Links.Add(0, creator.Key.Length, creator.Key);
                                }

                            }
                        }

                    }

                    flowLayoutPanel1.Controls.Add(foundObject);

                }
            }
            flowLayoutPanel1.ResumeLayout();


        }

        private void GetObjectsByKeyword(string keyword)
        {


            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();

            List<OBJState> createdObjects = OBJState.GetObjectsByKeyword(new List<string> { keyword }, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            foreach (OBJState objstate in createdObjects)
            {
                if (objstate.Owners != null)
                {

                    FoundObjectControl foundObject = new FoundObjectControl();

                    switch (objstate.Image.ToUpper().Substring(0, 4))
                    {
                        case "BTC:":
                            string transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                            break;
                        case "MZC:":
                            transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("MZC:", @"root/");
                            break;
                        case "IPFS":
                            transid = objstate.Image.Substring(5, 46);

                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();

                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    string fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
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
                            if (objstate.Image.Length == 51)
                            { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                            else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }

                            break;
                        case "HTTP":
                            foundObject.ObjectImage.ImageLocation = objstate.Image;
                            break;


                        default:
                            transid = objstate.Image.Substring(0, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                            }
                            foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                            break;
                    }
                    foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;
                    foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator.Key, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                        if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                        {
                            foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                        }
                        else
                        {


                            if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                            {
                                foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                            }

                        }


                    }
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                    foundObject.ObjectAddress.Text = objstate.Creators.First().Key;


                    flowLayoutPanel1.Controls.Add(foundObject);
                }
            }
            flowLayoutPanel1.ResumeLayout();
        }

        private void GetObjectsByURN(string urn)
        {

            flowLayoutPanel1.Controls.Clear();

            OBJState objstate = OBJState.GetObjectByURN(urn, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (objstate.Owners != null)
            {

                FoundObjectControl foundObject = new FoundObjectControl();

                switch (objstate.Image.ToUpper().Substring(0, 4))
                {
                    case "BTC:":
                        string transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                        break;
                    case "MZC:":
                        transid = objstate.Image.Substring(4, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                        }
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("MZC:", @"root/");
                        break;
                    case "IPFS":
                        transid = objstate.Image.Substring(5, 46);

                        if (!System.IO.Directory.Exists("ipfs/" + transid))
                        {
                            Process process2 = new Process();
                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                            process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                            process2.StartInfo.UseShellExecute = false;
                            process2.StartInfo.CreateNoWindow = true;
                            process2.Start();
                            process2.WaitForExit();

                            if (System.IO.File.Exists("ipfs/" + transid))
                            {
                                System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                string fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
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
                                            Arguments = "pin add " + transid,
                                            UseShellExecute = false,
                                            CreateNoWindow = true
                                        }
                                    };
                                    process3.Start();
                                }
                            }

                        }
                        if (objstate.Image.Length == 51)
                        { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                        else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }

                        break;
                    case "HTTP":
                        foundObject.ObjectImage.ImageLocation = objstate.Image;
                        break;


                    default:
                        transid = objstate.Image.Substring(0, 64);
                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                        }
                        foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                        break;
                }
                foundObject.ObjectName.Text = objstate.Name;
                foundObject.ObjectDescription.Text = objstate.Description;
                foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                {

                    PROState profile = PROState.GetProfileByAddress(creator.Key, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                    if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                    {
                        foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                    }
                    else
                    {


                        if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                        {
                            foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                        }

                    }


                }
                foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                flowLayoutPanel1.Controls.Add(foundObject);
            }
        }

        private void ButtonGetOwnedClick(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
                lastClickedButton.BackColor = Color.White;

            var button = (Button)sender;
            lastClickedButton = button;
            button.BackColor = Color.Yellow;


            string profileCheck = txtSearchAddress.Text;
            PROState searchprofile = PROState.GetProfileByAddress(txtSearchAddress.Text.Replace("@", ""), txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (searchprofile.URN != null)
            {
                linkLabel1.Text = searchprofile.URN;
                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(txtSearchAddress.Text.Replace("@", ""), txtLogin.Text, txtPassword.Text, txtUrl.Text);

                if (searchprofile.URN != null)
                {
                    linkLabel1.Text = TruncateAddress(searchprofile.Creators.First());
                    linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
                    profileCheck = searchprofile.Creators.First();
                }
                else
                {
                    linkLabel1.Text = "anon";
                    linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;

                }
            }

            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();

            List<OBJState> createdObjects = OBJState.GetObjectsOwnedByAddress(profileCheck, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            foreach (OBJState objstate in createdObjects)
            {
                if (objstate.Owners != null)
                {
                    FoundObjectControl foundObject = new FoundObjectControl();

                    switch (objstate.Image.ToUpper().Substring(0, 4))
                    {
                        case "BTC:":
                            string transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                            break;
                        case "MZC:":
                            transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("MZC:", @"root/");
                            break;
                        case "IPFS":
                            transid = objstate.Image.Substring(5, 46);

                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();

                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    string fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
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
                                                Arguments = "pin add " + transid,
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                    }
                                }

                            }
                            if (objstate.Image.Length == 51)
                            { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                            else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }

                            break;
                        case "HTTP":
                            foundObject.ObjectImage.ImageLocation = objstate.Image;
                            break;


                        default:
                            transid = objstate.Image.Substring(0, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                            }
                            foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                            break;
                    }
                    foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;

                    foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                    {

                        if (creator.Value.Year > 1)
                        {


                            PROState profile = PROState.GetProfileByAddress(creator.Key, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                            if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                            {
                                foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                            }
                            else
                            {


                                if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                                {
                                    foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                }

                            }
                        }
                        else
                        {
                            foundObject.ObjectCreators.Text = TruncateAddress(creator.Key);
                        }


                    }

                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                    foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                    flowLayoutPanel1.Controls.Add(foundObject);
                }
            }
            flowLayoutPanel1.ResumeLayout();
        }

        private void ButtonGetCreatedClick(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
                lastClickedButton.BackColor = Color.White;

            var button = (Button)sender;
            lastClickedButton = button;
            button.BackColor = Color.Yellow;


            string profileCheck = txtSearchAddress.Text;
            PROState searchprofile = PROState.GetProfileByAddress(txtSearchAddress.Text.Replace("@", ""), txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (searchprofile.URN != null)
            {
                linkLabel1.Text = searchprofile.URN;
                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(txtSearchAddress.Text.Replace("@", ""), txtLogin.Text, txtPassword.Text, txtUrl.Text);

                if (searchprofile.URN != null)
                {
                    linkLabel1.Text = TruncateAddress(searchprofile.Creators.First());
                    linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
                    profileCheck = searchprofile.Creators.First();
                }
                else
                {
                    linkLabel1.Text = "anon";
                    linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;

                }
            }

            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();

            List<OBJState> createdObjects = OBJState.GetObjectsCreatedByAddress(profileCheck, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            foreach (OBJState objstate in createdObjects)
            {
                if (objstate.Owners != null)
                {
                    FoundObjectControl foundObject = new FoundObjectControl();

                    switch (objstate.Image.ToUpper().Substring(0, 4))
                    {
                        case "BTC:":
                            string transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                            break;
                        case "MZC:":
                            transid = objstate.Image.Substring(4, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                            }
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("MZC:", @"root/");
                            break;
                        case "IPFS":
                            transid = objstate.Image.Substring(5, 46);

                            if (!System.IO.Directory.Exists("ipfs/" + transid))
                            {
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + objstate.Image.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();

                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    string fileName = objstate.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", @"ipfs/" + transid + @"/" + fileName);
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
                                                Arguments = "pin add " + transid,
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                    }
                                }

                            }
                            if (objstate.Image.Length == 51)
                            { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/") + @"/artifact"; }
                            else { foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); }

                            break;
                        case "HTTP":
                            foundObject.ObjectImage.ImageLocation = objstate.Image;
                            break;


                        default:
                            transid = objstate.Image.Substring(0, 64);
                            if (!System.IO.Directory.Exists("root/" + transid))
                            {
                                Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                            }
                            foundObject.ObjectImage.ImageLocation = @"root/" + objstate.Image;

                            break;
                    }

                    foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;

                    foreach (KeyValuePair<string, DateTime> creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator.Key, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                        if (profile.URN != null && foundObject.ObjectCreators.Text == "")
                        {
                            foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                        }
                        else
                        {


                            if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                            {
                                foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                            }

                        }


                    }
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                    foundObject.ObjectAddress.Text = objstate.Creators.First().Key;
                    flowLayoutPanel1.Controls.Add(foundObject);
                }
            }
            flowLayoutPanel1.ResumeLayout();

        }

        private void MainUserNameClick(object sender, LinkLabelLinkClickedEventArgs e)
        {



            PROState searchprofile = PROState.GetProfileByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (searchprofile.URN != null)
            {
                txtSearchAddress.Text = searchprofile.URN;
                linkLabel1.Text = TruncateAddress(searchprofile.Creators.First());

                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                if (searchprofile.URN != null)
                {
                    txtSearchAddress.Text = searchprofile.Creators.First();
                    linkLabel1.Text = searchprofile.URN;
                    linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;

                }
                else
                {
                    linkLabel1.Text = "anon";
                    linkLabel1.LinkColor = System.Drawing.SystemColors.GradientActiveCaption;

                }
            }




        }

        private async void SearchAddressKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOwned.BackColor = Color.White;
                btnCreated.BackColor = Color.White;

                if (SearchId == SearchHistory.Count)
                {
                    SearchHistory.Add(txtSearchAddress.Text);
                    SearchId++;
                }
                else
                {

                    if (SearchId > SearchHistory.Count - 1) { SearchId = SearchHistory.Count - 1; }
                    SearchHistory[SearchId] = txtSearchAddress.Text;

                }



                if (txtSearchAddress.Text.ToLower().StartsWith("http"))
                {
                    flowLayoutPanel1.Controls.Clear();
                    flowLayoutPanel1.AutoScroll = false;
                    var webBrowser1 = new Microsoft.Web.WebView2.WinForms.WebView2();
                    webBrowser1.Size = flowLayoutPanel1.Size;
                    flowLayoutPanel1.Controls.Add(webBrowser1);

                    await webBrowser1.EnsureCoreWebView2Async();
                    webBrowser1.CoreWebView2.Navigate(txtSearchAddress.Text);
                }
                else
                {
                    flowLayoutPanel1.Controls.Clear();
                    flowLayoutPanel1.AutoScroll = true;


                    if (txtSearchAddress.Text.StartsWith("#"))
                    {
                        GetObjectsByKeyword(txtSearchAddress.Text.Replace("#", ""));

                    }
                    else
                    {

                        if (txtSearchAddress.Text.ToLower().StartsWith(@"ipfs:"))
                        {
                            string ipfsHash = txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Substring(5, 46);

                            if (!System.IO.Directory.Exists("ipfs/" + ipfsHash))
                            {
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + ipfsHash + @" -o ipfs\" + ipfsHash;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();

                                if (System.IO.File.Exists("ipfs/" + ipfsHash))
                                {
                                    System.IO.File.Move("ipfs/" + ipfsHash, "ipfs/" + ipfsHash + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + ipfsHash);
                                    string fileName = txtSearchAddress.Text.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    System.IO.File.Move("ipfs/" + ipfsHash + "_tmp", @"ipfs/" + ipfsHash + @"/" + fileName);

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
                                                Arguments = "pin add " + ipfsHash,
                                                UseShellExecute = false,
                                                CreateNoWindow = true
                                            }
                                        };
                                        process3.Start();
                                    }
                                }

                                Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash);

                            }
                            else
                            {

                                Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ipfs\" + ipfsHash);

                            }


                        }
                        else
                        {
                            if (txtSearchAddress.Text.StartsWith(@"sup:"))
                            {
                                GetObjectsByURN(txtSearchAddress.Text.Replace("sup:", "").Replace(@"\\", "").Replace(@"//", ""));
                            }
                            else
                            {

                                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");

                                if (txtSearchAddress.Text.Count() > 64 && regexTransactionId.IsMatch(txtSearchAddress.Text) && txtSearchAddress.Text.Contains(".htm"))
                                {
                                    if (txtSearchAddress.Text.StartsWith("MZC:"))
                                    {
                                        Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:12832", "50");
                                    }
                                    else
                                    {
                                        if (txtSearchAddress.Text.StartsWith("BTC:"))
                                        {
                                            Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(4, 64), txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");
                                        }
                                        else
                                        {
                                            Root.GetRootByTransactionId(txtSearchAddress.Text.Substring(0, 64), txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:18332");
                                        }

                                    }
                                    Match match = regexTransactionId.Match(txtSearchAddress.Text);
                                    string browserPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + txtSearchAddress.Text.Replace("MZC:","").Replace("BTC:", "");
                                    browserPath = @"file:///" + browserPath.Replace(@"\", @"/");
                                    flowLayoutPanel1.Controls.Clear();
                                    flowLayoutPanel1.AutoScroll = false;
                                    var webBrowser1 = new Microsoft.Web.WebView2.WinForms.WebView2();
                                    webBrowser1.Size = flowLayoutPanel1.Size;
                                    flowLayoutPanel1.Controls.Add(webBrowser1);

                                    await webBrowser1.EnsureCoreWebView2Async();
                                    webBrowser1.CoreWebView2.Navigate(browserPath.Replace(@"/", @"\"));
                                }
                                else
                                {
                                    GetObjectsbyAddress(txtSearchAddress.Text.Replace("@", ""));
                                }

                            }
                        }
                    }
                }

            }


        }

        private void ObjectBrowserLoad(object sender, EventArgs e)
        {
            if (_objectaddress.Length > 0)
            {
                txtSearchAddress.Text = _objectaddress;

                if (txtSearchAddress.Text.StartsWith("#"))
                {
                    GetObjectsByKeyword(txtSearchAddress.Text.Replace("#", ""));

                }
                else
                    GetObjectsbyAddress(txtSearchAddress.Text.Replace("@", ""));
            }
            else
            {
                var SUP = new Options { CreateIfMissing = true };

                using (var db = new DB(SUP, @"ipfs"))
                {

                    string ipfsdaemon = db.Get("ipfs-daemon");

                    if (ipfsdaemon == "true")
                    {

                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = @"ipfs\ipfs.exe",
                                Arguments = "daemon",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                    }

                }
            }


        }

        private void ButtonLoadWorkBench(object sender, EventArgs e)
        {
            new WorkBench().Show();
        }

        private void ButtonLoadConnections(object sender, EventArgs e)
        {
            new Connections().Show();
        }

        string TruncateAddress(string input)
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

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count > 0 && flowLayoutPanel1.Controls[0] is Microsoft.Web.WebView2.WinForms.WebView2)
            {
                flowLayoutPanel1.Controls[0].Size = flowLayoutPanel1.Size;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
                lastClickedButton.BackColor = Color.White;

            var button = (Button)sender;
            lastClickedButton = button;
            button.BackColor = Color.Yellow;
        }

        private void btnHistoryBack_Click(object sender, EventArgs e)
        {

            if (SearchHistory.Count - 1 > 0)
            {
                SearchId--;
                if (SearchId < 0) { SearchId = 0; }

                txtSearchAddress.Text = SearchHistory[SearchId].ToString();
                txtSearchAddress.Focus();
                SendKeys.SendWait("{Enter}");

            }
        }

        private void btnHistoryForward_Click(object sender, EventArgs e)
        {
            if (SearchHistory.Count - 1 > SearchId)
            {
                SearchId++;
                txtSearchAddress.Text = SearchHistory[SearchId].ToString();
                txtSearchAddress.Focus();
                SendKeys.SendWait("{Enter}");
            }
        }
    }
}
