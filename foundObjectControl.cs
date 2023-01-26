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
    public partial class foundObjectControl : UserControl
    {
        public foundObjectControl()
        {
            InitializeComponent();
        }

        private void foundObjectControl_Click(object sender, EventArgs e)
        {

      
            new ObjectDetails(ObjectAddress.Text).Show();
        }

        private void ObjectCreators_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //send to searchtextbox click on the creator button
        }
    }
}
