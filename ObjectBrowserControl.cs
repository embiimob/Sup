using System;
using System.Windows.Forms;

namespace SUP
{
    public partial class ObjectBrowserControl : UserControl
    {
        public ObjectBrowser control;

        public event EventHandler ProfileURNChanged;

        public ObjectBrowserControl(string searchstring = "", bool iscontrol = true, bool testnet = true)
        {
            InitializeComponent();

            control = new ObjectBrowser(searchstring, iscontrol, testnet);
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
            // Guard against circular updates: Don't propagate the event if ObjectBrowser is being updated externally
            if (control._isUpdatingFromExternal)
            {
                System.Diagnostics.Debug.WriteLine("[ObjectBrowserControl] Suppressing ProfileURNChanged event - external update in progress");
                return;
            }
            
            // Check if the link label text has changed
            if (control.profileURN.Text != "")
            {
                System.Diagnostics.Debug.WriteLine($"[ObjectBrowserControl] Firing ProfileURNChanged event for: {control.profileURN.Text}");
                ProfileURNChanged?.Invoke(this, EventArgs.Empty);
            }
                
        }

      
    }
}
