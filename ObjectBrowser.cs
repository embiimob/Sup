using NBitcoin.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _objectaddress = objectaddress;
        }

        private void btnGetObject_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
                lastClickedButton.BackColor = Color.White;

            var button = (Button)sender;
            lastClickedButton = button;
            button.BackColor = Color.Yellow;



            flowLayoutPanel1.Controls.Clear();

            OBJState objstate = OBJState.GetObjectByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtLastSearchJSON.Text = JsonConvert.SerializeObject(new OBJState[] { objstate });
            if (objstate.Owners != null)
            {
                foundObjectControl foundObject = new foundObjectControl();

                if (objstate.Image.StartsWith("BTC:"))
                {
                    string transid = objstate.Image.Substring(4, 64);

                    if (!System.IO.Directory.Exists("root/" + transid))
                    {
                        Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");

                    }

                }
                foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/"); foundObject.ObjectName.Text = objstate.Name;
                foundObject.ObjectDescription.Text = objstate.Description;
         
                foreach (string creator in objstate.Creators.Skip(1))
                {

                    PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                    if (profile.URN != null && foundObject.ObjectCreators.Text == null)
                    {
                        foundObject.ObjectCreators.Text = TruncateAddress(profile.URN);
                    }
                    else
                    {
                        foundObject.ObjectCreators.Text = TruncateAddress(creator);

                        if (profile.URN != null && foundObject.ObjectCreators2.Text == null)
                        {
                            foundObject.ObjectCreators2.Text = TruncateAddress(profile.URN);
                        }
                    }

                }

                foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                foundObject.ObjectAddress.Text = objstate.Creators.First();

                flowLayoutPanel1.Controls.Add(foundObject);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
                lastClickedButton.BackColor = Color.White;

            var button = (Button)sender;
            lastClickedButton = button;
            button.BackColor = Color.Yellow;

            string profileCheck = txtSearchAddress.Text;
            PROState searchprofile = PROState.GetProfileByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (searchprofile.URN != null)
            {
                linkLabel1.Text = searchprofile.URN;
                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

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




            flowLayoutPanel1.Controls.Clear();


            List<OBJState> createdObjects = OBJState.GetObjectsByAddress(profileCheck, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            txtLastSearchJSON.Text = JsonConvert.SerializeObject(createdObjects);
            foreach (OBJState objstate in createdObjects)
            {
                if (objstate.Owners != null)
                {

                    foundObjectControl foundObject = new foundObjectControl();

                    if (objstate.Image.StartsWith("BTC:"))
                    {
                        string transid = objstate.Image.Substring(4, 64);

                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");

                        }

                    }
                    foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                    foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;
                    foundObject.ObjectAddress.Text = objstate.Creators.First();
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                  
                    foreach (string creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

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

                            if (foundObject.ObjectCreators.Text == "") { foundObject.ObjectCreators.Text = TruncateAddress(creator); }
                        }



                    }
                
                    flowLayoutPanel1.Controls.Add(foundObject);

                }
            }


        }

        private void btnGetOwned_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
                lastClickedButton.BackColor = Color.White;

            var button = (Button)sender;
            lastClickedButton = button;
            button.BackColor = Color.Yellow;


            string profileCheck = txtSearchAddress.Text;
            PROState searchprofile = PROState.GetProfileByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (searchprofile.URN != null)
            {
                linkLabel1.Text = searchprofile.URN;
                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

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






            flowLayoutPanel1.Controls.Clear();

            List<OBJState> createdObjects = OBJState.GetObjectsOwnedByAddress(profileCheck, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtLastSearchJSON.Text = JsonConvert.SerializeObject(createdObjects);
            foreach (OBJState objstate in createdObjects)
            {
                if (objstate.Owners != null)
                {
                    foundObjectControl foundObject = new foundObjectControl();

                    if (objstate.Image.StartsWith("BTC:"))
                    {
                        string transid = objstate.Image.Substring(4, 64);

                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");

                        }

                    }
                    foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                    foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;
                    
                    foreach (string creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

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

                            if (foundObject.ObjectCreators.Text == "") { foundObject.ObjectCreators.Text = TruncateAddress(creator); }
                        }




                    }
                  
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                    foundObject.ObjectAddress.Text = objstate.Creators.First();
                    flowLayoutPanel1.Controls.Add(foundObject);
                }
            }
        }

        private void btnGetCreated_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
                lastClickedButton.BackColor = Color.White;

            var button = (Button)sender;
            lastClickedButton = button;
            button.BackColor = Color.Yellow;


            string profileCheck = txtSearchAddress.Text;
            PROState searchprofile = PROState.GetProfileByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            if (searchprofile.URN != null)
            {
                linkLabel1.Text = searchprofile.URN;
                linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            }
            else
            {


                searchprofile = PROState.GetProfileByURN(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

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




            flowLayoutPanel1.Controls.Clear();

            List<OBJState> createdObjects = OBJState.GetObjectsCreatedByAddress(profileCheck, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtLastSearchJSON.Text = JsonConvert.SerializeObject(createdObjects);
            foreach (OBJState objstate in createdObjects)
            {
                if (objstate.Owners != null)
                {
                    foundObjectControl foundObject = new foundObjectControl();

                    if (objstate.Image.StartsWith("BTC:"))
                    {
                        string transid = objstate.Image.Substring(4, 64);

                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");

                        }

                    }
                    foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                    foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;
                 
                    foreach (string creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

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

                            if (foundObject.ObjectCreators.Text == "") { foundObject.ObjectCreators.Text = TruncateAddress(creator); }
                        }



                    }
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                    foundObject.ObjectAddress.Text = objstate.Creators.First();
                    flowLayoutPanel1.Controls.Add(foundObject);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
                lastClickedButton.BackColor = Color.White;

            var button = (Button)sender;
            lastClickedButton = button;
            button.BackColor = Color.Yellow;

            flowLayoutPanel1.Controls.Clear();

            List<OBJState> createdObjects = OBJState.GetObjectsByKeyword(new List<string> { txtSearchAddress.Text }, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtLastSearchJSON.Text = JsonConvert.SerializeObject(createdObjects);
            foreach (OBJState objstate in createdObjects)
            {
                if (objstate.Owners != null)
                {

                    foundObjectControl foundObject = new foundObjectControl();

                    if (objstate.Image.StartsWith("BTC:"))
                    {
                        string transid = objstate.Image.Substring(4, 64);

                        if (!System.IO.Directory.Exists("root/" + transid))
                        {
                            Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");

                        }

                    }
                    foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/"); foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;
                    foreach (string creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

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

                            if (foundObject.ObjectCreators.Text == "") { foundObject.ObjectCreators.Text = TruncateAddress(creator); }
                        }




                    }
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                    foundObject.ObjectAddress.Text = objstate.Creators.First();


                    flowLayoutPanel1.Controls.Add(foundObject);
                }
            }

        }


        string TruncateAddress(string input)
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


        private void button3_Click(object sender, EventArgs e)
        {
            if (lastClickedButton != null)
                lastClickedButton.BackColor = Color.White;

            var button = (Button)sender;
            lastClickedButton = button;
            button.BackColor = Color.Yellow;

            flowLayoutPanel1.Controls.Clear();

            OBJState objstate = OBJState.GetObjectByURN(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtLastSearchJSON.Text = JsonConvert.SerializeObject(new OBJState[] { objstate });

            if (objstate.Owners != null)
            {

                foundObjectControl foundObject = new foundObjectControl();

                if (objstate.Image.StartsWith("BTC:"))
                {
                    string transid = objstate.Image.Substring(4, 64);

                    if (!System.IO.Directory.Exists("root/" + transid))
                    {
                        Root root = Root.GetRootByTransactionId(transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0");

                    }

                }
                foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/"); foundObject.ObjectName.Text = objstate.Name;
                foundObject.ObjectDescription.Text = objstate.Description;
                foreach (string creator in objstate.Creators.Skip(1))
                {

                    PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

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

                        if (foundObject.ObjectCreators.Text == "") { foundObject.ObjectCreators.Text = TruncateAddress(creator); }
                    }




                }
                foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                foundObject.ObjectAddress.Text = objstate.Creators.First();
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
                if (btnObjects.BackColor == Color.Yellow) { btnObjects.PerformClick(); }
                if (btnCreated.BackColor == Color.Yellow) { btnCreated.PerformClick(); }
                if (btnOwned.BackColor == Color.Yellow) { btnOwned.PerformClick(); }
                if (btnKeywords.BackColor == Color.Yellow) { btnKeywords.PerformClick(); }
                if (btnURN.BackColor == Color.Yellow) { btnURN.PerformClick(); }
            }
        }

        private void ObjectBrowser_Load(object sender, EventArgs e)
        {
            if (_objectaddress != null)
            {
                txtSearchAddress.Text = _objectaddress;
                btnObjects.PerformClick();
            }
        }
    }
}
