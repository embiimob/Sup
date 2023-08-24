using System;
using System.Windows.Forms;

namespace SUP
{
    public partial class ObjectBrowserControl : UserControl
    {
        private ObjectBrowser control;

        public event EventHandler ProfileURNChanged;

        public ObjectBrowserControl(string searchstring = "", bool iscontrol = true)
        {
            InitializeComponent();

            control = new ObjectBrowser(searchstring, iscontrol);
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
            // Check if the link label text has changed
            if (control.profileURN.Text != "")
            {
                ProfileURNChanged?.Invoke(this, EventArgs.Empty);
            }
                
        }

      
    }
}
