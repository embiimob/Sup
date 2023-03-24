using System;
using System.Windows.Forms;

namespace SUP
{
    public partial class ObjectBrowserControl : UserControl
    {
        public event EventHandler ProfileURNChanged;

        public ObjectBrowserControl(string searchstring = "", bool iscontrol = true)
        {
            InitializeComponent();

            ObjectBrowser control = new ObjectBrowser(searchstring, iscontrol);
            control.TopLevel = false;
            control.Visible = true;
            control.ControlBox = false;
            control.Dock = DockStyle.Fill;
            control.FormBorderStyle = FormBorderStyle.None;
            control.profileURN.TextChanged += ProfileURN_TextChanged;
            panel1.Controls.Add(control);
            
        }

        private void ProfileURN_TextChanged(object sender, EventArgs e)
        {
            ProfileURNChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}