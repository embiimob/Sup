using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUP
{
    public class ScrollableFlowLayoutPanel : FlowLayoutPanel
    {
        private const int WM_MOUSEWHEEL = 0x020A;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEWHEEL)
            {
                int scrollAmount = (int)((short)(m.WParam.ToInt32() >> 16)) / SystemInformation.MouseWheelScrollLines * SystemInformation.VerticalScrollBarThumbHeight;
                int newScrollPos = this.VerticalScroll.Value - scrollAmount;

                newScrollPos = Math.Max(newScrollPos, this.VerticalScroll.Minimum);
                newScrollPos = Math.Min(newScrollPos, this.VerticalScroll.Maximum);

                this.VerticalScroll.Value = newScrollPos;

                m.Result = IntPtr.Zero; // Mark the message as handled
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
