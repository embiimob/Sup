
using SUP.P2FK;
using System;
using System.IO;
using System.Windows.Forms;

namespace SUP
{
    public partial class FoundCollectionControl : UserControl
    {
        private bool _testnet;
        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        public FoundCollectionControl(bool testnet = true)
        {
            InitializeComponent();
            _testnet = testnet;
            if (!testnet )
            {
              mainnetURL = @"http://127.0.0.1:8332";
        mainnetLogin = "good-user";
        mainnetPassword = "better-password";
        mainnetVersionByte = "0";
    }
        }
     
        private void foundObjectControl_Click(object sender, EventArgs e)
        {

            Form parentForm = this.FindForm();
            ObjectBrowser childForm = new ObjectBrowser(ObjectAddress.Text, _testnet);
            
            childForm.Owner = parentForm;
          
            childForm.Show();

        }

        private void ObjectAddress_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ObjectAddress.Text);
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
                            string directoryPath = Path.GetDirectoryName(ObjectImage.ImageLocation.Replace(@"/",@"\"));
                            Directory.Delete(directoryPath, true);
                        }
                        catch
                        {
                            // Handle any exceptions thrown during directory deletion
                        }
                    }

                    Root[] root = Root.GetRootsByAddress(ObjectAddress.Text,mainnetLogin,mainnetPassword,mainnetURL,0,-1,mainnetVersionByte);

                    foreach (Root rootItem in root)
                    {

     
                        try
                        {
                            Directory.Delete(@"root\" + rootItem.TransactionId, true);
                        }
                        catch { }

                        foreach (string key in rootItem.Keyword.Keys)
                        {
                            try { File.Delete(@"root\" + key + @"\GetObjectsCollectionsByAddress.json"); }catch { }
                        }

                    }

                    try
                    {

                       
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


    }
}
