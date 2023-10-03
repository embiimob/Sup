using LevelDB;
using Newtonsoft.Json;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SUP
{
    public partial class FoundObjectControl : UserControl

       
    {
        private string _activeprofile;
        public FoundObjectControl(string activeprofile)
        {
            InitializeComponent();
            _activeprofile = activeprofile;
        }

     
        private void foundObjectControl_Click(object sender, EventArgs e)
        {

            Form parentForm = this.FindForm();
            ObjectDetails childForm = new ObjectDetails(ObjectAddress.Text, _activeprofile);
            
            childForm.Owner = parentForm;
          
            childForm.Show();

        }

        private void foundListings_Click(object sender, EventArgs e)
        {

           Form parentForm = this.FindForm();
           ObjectBuy childForm = new ObjectBuy(ObjectAddress.Text, _activeprofile);

            childForm.Owner = parentForm;

            childForm.Show();

        }

        private void ObjectCreators_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form parentForm = this.FindForm();
            ObjectBrowser childForm = new ObjectBrowser(ObjectCreators.Links[0].LinkData.ToString());
            childForm.Owner = parentForm;
            childForm.Show();
        }

        private void ObjectCreators2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form parentForm = this.FindForm();
            ObjectBrowser childForm = new ObjectBrowser(ObjectCreators2.Links[0].LinkData.ToString());
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
            ObjectDetails childForm = new ObjectDetails(txtOfficialURN.Text,_activeprofile);
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


                    var SUP = new Options { CreateIfMissing = true };
                    var keysToDelete = new HashSet<string>(); // Create a new HashSet to store the keys to delete

                    using (var db = new DB(SUP, @"root\found"))
                    {
                        LevelDB.Iterator it = db.CreateIterator();

                        for (
                            it.SeekToLast();
                            it.IsValid();
                            it.Prev()
                        )
                        {
                            string key = it.KeyAsString();
                            if (key.Contains(ObjectAddress.Text))
                            {
                                keysToDelete.Add(key); // Add the key to the HashSet
                            }
                        }

                        it.Dispose();

                        var batch = new WriteBatch(); // Create a new WriteBatch to delete the keys
                        foreach (var key in keysToDelete)
                        {
                            batch.Delete(key); // Add a delete operation for each key in the HashSet
                        }
                        db.Write(batch); // Execute the batch to delete the keys from the database
                    }


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

                    Root[] root = Root.GetRootsByAddress(ObjectAddress.Text, "good-user", "better-password", @"http://127.0.0.1:18332");

                    foreach (Root rootItem in root)
                    {

     
                        try
                        {
                            Directory.Delete(@"root\" + rootItem.TransactionId, true);
                        }
                        catch { }

                        foreach (string key in rootItem.Keyword.Keys)
                        {
                            try { File.Delete(@"root\" + key + @"\GetObjectsByAddress.json"); }catch { }
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

                        //this may no longer be necessary...
                        //if (ObjectId.Text != null) { try { Directory.Delete(@"root\" + ObjectId.Text, true); } catch { } }                        

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


    }
}
