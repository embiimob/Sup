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
            new ObjectDetails(ObjectAddress.Text).Show();
        }

        private void ObjectCreators_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ObjectBrowser(ObjectCreators.Links[0].LinkData.ToString()).Show();
        }

        private void ObjectCreators2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ObjectBrowser(ObjectCreators2.Links[0].LinkData.ToString()).Show();
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
    }
}
