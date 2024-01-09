using Newtonsoft.Json;
using SUP.P2FK;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SUP
{
    public partial class FoundObjectControl : UserControl


    {
        private string _activeprofile;
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        private bool _testnet = true;
        public FoundObjectControl(string activeprofile, bool testnet = true)
        {
            InitializeComponent();
            _activeprofile = activeprofile;
            if (!testnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetLogin = "good-user";
                mainnetPassword = "better-password";
                mainnetVersionByte = "0";
                _testnet = testnet;
            }
        }


        private void foundObjectControl_Click(object sender, EventArgs e)
        {

            Form parentForm = this.FindForm();
            ObjectDetails childForm = new ObjectDetails(ObjectAddress.Text, _activeprofile ,false, _testnet);

            childForm.Owner = parentForm;

            childForm.Show();

        }

        private void foundListings_Click(object sender, EventArgs e)
        {

            Form parentForm = this.FindForm();
            ObjectBuy childForm = new ObjectBuy(ObjectAddress.Text, _activeprofile, _testnet);

            childForm.Owner = parentForm;

            childForm.Show();

        }

        private void ObjectCreators_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form parentForm = this.FindForm();
            ObjectBrowser childForm = new ObjectBrowser(ObjectCreators.Links[0].LinkData.ToString(),false, _testnet);
            childForm.Owner = parentForm;
            childForm.Show();
        }

        private void ObjectCreators2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form parentForm = this.FindForm();
            ObjectBrowser childForm = new ObjectBrowser(ObjectCreators2.Links[0].LinkData.ToString(),false, _testnet);
            childForm.Owner = parentForm;
            childForm.Show();
        }

        private void ObjectAddress_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ObjectAddress.Text);
        }

        private void btnOfficial_Click(object sender, EventArgs e)
        {
            Form parentForm = this.FindForm();
            ObjectDetails childForm = new ObjectDetails(txtOfficialURN.Text, _activeprofile,false, _testnet);
            childForm.Owner = parentForm;
            childForm.Show();
        }

        private void lblTrash_Click(object sender, EventArgs e)
        {

            // Get the file name from the label
            string fileName = ObjectName.Text;

            // Prompt the user to confirm whether they want to delete the file
            DialogResult result = MessageBox.Show($"Are you sure you want to delete {fileName}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Delete the file
                try
                {

                    if (ObjectImage.ImageLocation != null && !ObjectImage.ImageLocation.StartsWith("includes"))
                    {
                        try
                        {
                            string directoryPath = Path.GetDirectoryName(ObjectImage.ImageLocation.Replace(@"/", @"\"));
                            Directory.Delete(directoryPath, true);
                        }
                        catch
                        {
                            // Handle any exceptions thrown during directory deletion
                        }
                    }

                    Root[] root = Root.GetRootsByAddress(ObjectAddress.Text, mainnetLogin,mainnetPassword,mainnetURL,0,-1,mainnetVersionByte);

                    foreach (Root rootItem in root)
                    {


                        try
                        {
                            Directory.Delete(@"root\" + rootItem.TransactionId, true);
                        }
                        catch { }

                        foreach (string key in rootItem.Keyword.Keys)
                        {
                            try { File.Delete(@"root\" + key + @"\GetObjectsByAddress.json"); } catch { }
                            try { File.Delete(@"root\" + key + @"\GetObjectsCreatedByAddress.json"); } catch { }
                            try { File.Delete(@"root\" + key + @"\GetObjectsOwnedByAddress.json"); } catch { }
                        }

                    }

                    try
                    {

                        string diskpath = "root\\" + ObjectAddress.Text + "\\";

                        // fetch current JSONOBJ from disk if it exists
                        try
                        {
                            string JSONOBJ = System.IO.File.ReadAllText(diskpath + "OBJ.json");
                            OBJState objectState = JsonConvert.DeserializeObject<OBJState>(JSONOBJ);

                            if (objectState.URN != null)
                            {
                                try { Directory.Delete(@"root\" + GetTransactionId(objectState.URN), true); } catch { }
                                try { Directory.Delete(@"ipfs\" + GetTransactionId(objectState.URN), true); } catch { }
                            }
                            if (objectState.Image != null)
                            {
                                try { Directory.Delete(@"root\" + GetTransactionId(objectState.Image), true); } catch { }
                                try { Directory.Delete(@"ipfs\" + GetTransactionId(objectState.Image), true); } catch { }
                            }

                        }
                        catch { }

                        try { Directory.Delete(@"root\" + ObjectAddress.Text, true); } catch { }
                        try { Directory.CreateDirectory(@"root\" + ObjectAddress.Text); } catch { }

                        using (FileStream fs = File.Create(@"root\" + ObjectAddress.Text + @"\BLOCK"))
                        {

                        }

                    }
                    catch { }


                }
                catch (IOException ex)
                {
                    // Handle the exception if the file cannot be deleted
                    MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Remove the user control from its parent flow panel
                this.Parent.Controls.Remove(this);
            }

        }

        public string GetTransactionId(string input)
        {
            int startIndex = input.IndexOf(":") + 1;
            if (startIndex == 0)
            {
                // No colon found, return the original string
                startIndex = 0;
            }

            int endIndex = input.IndexOf("/");
            if (endIndex == -1)
            {
                // No slash found, return the substring starting from the start index
                return input.Substring(startIndex);
            }

            // Return the substring between the colon and the slash
            return input.Substring(startIndex, endIndex - startIndex);
        }

        private void ObjectQty_TextChanged(object sender, EventArgs e)
        {
            AdjustFontSize(ObjectQty, ObjectQty.Text);
        }

        public static void AdjustFontSize(Label label, string text)
        {
            float maxFontSize = 8.25f;
            // Set the original font size and style
            Font originalFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);

            // Get the size of the label with the original font
            Size originalSize = TextRenderer.MeasureText(text+"xx", originalFont);

            // Set the desired maximum size (adjust as needed)
            Size maxSize = new Size(label.Width, label.Height);

            // Calculate the scale factor for both width and height
            float widthScale = maxSize.Width / (float)originalSize.Width;
            float heightScale = maxSize.Height / (float)originalSize.Height;

            // Use the minimum scale factor to ensure both dimensions fit
            float scale = Math.Min(widthScale, heightScale);

            // Calculate the new font size (limiting to maxFontSize)
            float newFontSize = Math.Min(originalFont.Size * scale, maxFontSize);

            // Create a new font with the adjusted size and style
            Font adjustedFont = new Font("Microsoft Sans Serif", newFontSize, FontStyle.Bold);

            // Apply the new font to the label
            label.Font = adjustedFont;
        }
    }
}
