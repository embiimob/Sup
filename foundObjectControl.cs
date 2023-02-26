using System;
using System.Windows.Forms;

namespace SUP
{
    public partial class FoundObjectControl : UserControl
    {
        public FoundObjectControl()
        {
            InitializeComponent();
        }

        private void foundObjectControl_Click(object sender, EventArgs e)
        {

            Form parentForm = this.FindForm();
            ObjectDetails childForm = new ObjectDetails(ObjectAddress.Text);
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

        private void ObjectPrice_TextChanged(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(208, 367);
        }

        private void ObjectOffer_TextChanged(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(208, 367);
        }

        private void ObjectAddress_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ObjectAddress.Text);
        }

        private void btnOfficial_Click(object sender, EventArgs e)
        {
            Form parentForm = this.FindForm();
            ObjectDetails childForm = new ObjectDetails(txtOfficialURN.Text);
            childForm.Owner = parentForm;
            childForm.Show();
        }
    }
}
