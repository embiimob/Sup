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
        public ObjectBrowser()
        {
            InitializeComponent();
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
            txtLastSearch.Text = JsonConvert.SerializeObject(new OBJState[] { objstate });
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
                string creators = null;
                foreach (string creator in objstate.Creators.Skip(1))
                {

                    PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                    if (profile.URN != null)
                    {
                        creators = creators + "  " + profile.URN;
                    }
                    else
                    {
                        creators = creators + "  " + TruncateAddress(creator);
                    }

                }
                foundObject.ObjectCreators.Text = creators;
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
            
            
            flowLayoutPanel1.Controls.Clear();
           

            List<OBJState> createdObjects = OBJState.GetObjectsByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);

            txtLastSearch.Text = JsonConvert.SerializeObject(createdObjects);
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
                            Root root = Root.GetRootByTransactionId( transid, txtLogin.Text, txtPassword.Text, @"http://127.0.0.1:8332", "0" );

                        }

                    }
                    foundObject.ObjectImage.ImageLocation = objstate.Image.Replace("BTC:", @"root/");
                    foundObject.ObjectName.Text = objstate.Name;
                    foundObject.ObjectDescription.Text = objstate.Description;
                    foundObject.ObjectAddress.Text = objstate.Creators.First();
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                    string creators = null;
                    foreach (string creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                        if (profile.URN != null)
                        {
                            creators = creators + "  " + profile.URN;
                        }
                        else
                        {
                            creators = creators + "  " + TruncateAddress(creator);
                        }

                        

                    }
                    foundObject.ObjectCreators.Text = creators;
                 
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


            flowLayoutPanel1.Controls.Clear();

            List<OBJState> createdObjects = OBJState.GetObjectsOwnedByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtLastSearch.Text = JsonConvert.SerializeObject(createdObjects);
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
                    string creators = null;
                    foreach (string creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                        if (profile.URN != null)
                        {
                            creators = creators + "  " + profile.URN;
                        }
                        else
                        {
                            creators = creators + "  " + TruncateAddress(creator);
                        }



                    }
                    foundObject.ObjectCreators.Text = creators;
                    foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                    foundObject.ObjectAddress.Text = "@" + objstate.Creators.First();
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

            flowLayoutPanel1.Controls.Clear();

            List<OBJState> createdObjects = OBJState.GetObjectsCreatedByAddress(txtSearchAddress.Text, txtLogin.Text, txtPassword.Text, txtUrl.Text);
            txtLastSearch.Text = JsonConvert.SerializeObject(createdObjects);
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
                    string creators = null;
                    foreach (string creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                        if (profile.URN != null)
                        {
                            creators = creators + "  " + profile.URN;
                        }
                        else
                        {
                            creators = creators + "  " + TruncateAddress(creator);
                        }


                    }
                    foundObject.ObjectCreators.Text = creators;
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
            txtLastSearch.Text = JsonConvert.SerializeObject(createdObjects);
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
                    string creators = null;
                    foreach (string creator in objstate.Creators.Skip(1))
                    {

                        PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                        if (profile.URN != null)
                        {
                            creators = creators + "  " + profile.URN;
                        }
                        else
                        {
                            creators = creators + "  " + TruncateAddress(creator);
                        }



                    }
                    foundObject.ObjectCreators.Text = creators;
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
            txtLastSearch.Text = JsonConvert.SerializeObject(new OBJState[] { objstate });

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
                string creators = null;
                foreach (string creator in objstate.Creators.Skip(1))
                {

                    PROState profile = PROState.GetProfileByAddress(creator, txtLogin.Text, txtPassword.Text, txtUrl.Text);

                    if (profile.URN != null)
                    {
                        creators = creators + "  " + profile.URN;
                    }
                    else
                    {
                        creators = creators + "  " + TruncateAddress(creator);
                    }



                }
                foundObject.ObjectCreators.Text = creators;
                foundObject.ObjectQty.Text = objstate.Owners.Values.Sum().ToString() + "x";
                foundObject.ObjectAddress.Text = objstate.Creators.First();
                flowLayoutPanel1.Controls.Add(foundObject);
            }
        }

    

        private void button4_Click(object sender, EventArgs e)
        {
            new WorkBench().Show();
        }
    }
}
