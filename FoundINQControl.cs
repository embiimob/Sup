using LevelDB;
using Newtonsoft.Json;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SUP
{
    public partial class FoundINQControl : UserControl
    {
        private string _address;
        public FoundINQControl(string address)
        {
            InitializeComponent();
            _address = address;
        }
     
        private void foundObjectControl_Click(object sender, EventArgs e)
        {

            Form parentForm = this.FindForm();
            ObjectBrowser childForm = new ObjectBrowser(ObjectAddress.Text);
            
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

        private void lblAnswer_Click(object sender, EventArgs e)
        {

        }
    }
}
