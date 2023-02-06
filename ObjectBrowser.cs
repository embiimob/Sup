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
using System.Windows.Forms;


namespace SUP
{

    public partial class ObjectBrowser : Form
    {
        Button lastClickedButton;
        private string _objectaddress;
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

        private void getObjectsbyAddress(string address)
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



                                var SUP = new Options { CreateIfMissing = true };

                                using (var db = new DB(SUP, @"sup"))
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
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); ;

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
                            }
                            else
                            {


                                if (profile.URN != null && foundObject.ObjectCreators2.Text == "")
                                {
                                    foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                                }
                                                               
                            }
                        }

                    }

                    flowLayoutPanel1.Controls.Add(foundObject);

                }
            }
            flowLayoutPanel1.ResumeLayout();


        }

        private void btnGetOwned_Click(object sender, EventArgs e)
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

                                var SUP = new Options { CreateIfMissing = true };

                                using (var db = new DB(SUP, @"sup"))
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
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); ;

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

        private void btnGetCreated_Click(object sender, EventArgs e)
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

                                var SUP = new Options { CreateIfMissing = true };

                                using (var db = new DB(SUP, @"sup"))
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
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); ;

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

        private void getObjectsByKeyword(string keyword)
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

                                //attempt to pin fails silently if daemon is not running
                                var SUP = new Options { CreateIfMissing = true };

                                using (var db = new DB(SUP, @"sup"))
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
                            foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); ;

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


        private void getObjectsByURN(string urn)
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

                            var SUP = new Options { CreateIfMissing = true };

                            using (var db = new DB(SUP, @"sup"))
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
                        foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("IPFS:", @"ipfs/"); ;

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



        private void button4_Click(object sender, EventArgs e)
        {
            new WorkBench().Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {



            string profileCheck = txtSearchAddress.Text;
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

        private void txtSearchAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtSearchAddress.Text.StartsWith("#"))
                {
                    getObjectsByKeyword(txtSearchAddress.Text.Replace("#", ""));

                }
                else
                {

                    if (txtSearchAddress.Text.ToLower().StartsWith(@"ipfs://"))
                    {
                        string ipfsHash = txtSearchAddress.Text.Substring(7, 46);

                        if (!System.IO.Directory.Exists("ipfs/" + ipfsHash))
                        {
                            Process process2 = new Process();
                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                            process2.StartInfo.Arguments = "get " + ipfsHash + @" -o ipfs\" + ipfsHash;
                            process2.StartInfo.UseShellExecute = false;
                            process2.StartInfo.CreateNoWindow = true;
                            process2.Start();
                            process2.WaitForExit();

                            //attempt to pin fails silently if daemon is not running
                            var SUP = new Options { CreateIfMissing = true };

                            using (var db = new DB(SUP, @"sup"))
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
                        if (txtSearchAddress.Text.StartsWith(@"sup://"))
                        {
                            getObjectsByURN(txtSearchAddress.Text.Replace("sup://", ""));
                        }
                        else { getObjectsbyAddress(txtSearchAddress.Text.Replace("@", "")); }
                    }
                }


            }


        }

        private void ObjectBrowser_Load(object sender, EventArgs e)
        {
            if (_objectaddress.Length > 0)
            {
                txtSearchAddress.Text = _objectaddress;

                if (txtSearchAddress.Text.StartsWith("#"))
                {
                    getObjectsByKeyword(txtSearchAddress.Text.Replace("#", ""));

                }
                else
                    getObjectsbyAddress(txtSearchAddress.Text.Replace("@", ""));
            }
            else
            {
                var SUP = new Options { CreateIfMissing = true };

                using (var db = new DB(SUP, @"sup"))
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


        private void button1_Click_1(object sender, EventArgs e)
        {
            new Connections().Show();
        }
    }
}
