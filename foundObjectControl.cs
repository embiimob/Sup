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
            new ObjectBrowser(ObjectCreators.Text).Show();
        }

        private void ObjectCreators2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ObjectBrowser(ObjectCreators2.Text).Show();
        }
    }
}
