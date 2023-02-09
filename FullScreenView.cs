using System.Drawing;
using System.Windows.Forms;

namespace SUP
{
    class FullScreenView : Form
    {
        private PictureBox pictureBox1;
       



        public FullScreenView(string imageurl)
        {
            InitializeComponent();
            pictureBox1.ImageLocation = imageurl;
        }

        private void FullScreenView_MouseWheel(object sender, MouseEventArgs e)
        {
           

                // Check if the scroll wheel is being scrolled up or down
                if (e.Delta > 0)
            {
                // Zoom in by increasing the size of the image
                if (pictureBox1.SizeMode == PictureBoxSizeMode.StretchImage) 
                { pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; }
                else { pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; }

            }
            else
                {
                // Zoom out by decreasing the size of the image
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                            
        }



        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 800);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.FullScreenViewClick);
            // 
            // FullScreenView
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 800);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FullScreenView";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FullScreenView_KeyDown);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.FullScreenView_MouseWheel);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }



        private void FullScreenViewClick(object sender, System.EventArgs e)
        {
            Close();
        }

        private void FullScreenView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // Perform action when the Esc key is pressed
                // For example, closing the form
                Close();
            }
           
            if (e.KeyCode == Keys.Space)
            {
                
                if (pictureBox1.SizeMode == PictureBoxSizeMode.StretchImage)
                { pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    return;
                }

                if (pictureBox1.SizeMode == PictureBoxSizeMode.Zoom)
                { pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                    return;
                }

                if (pictureBox1.SizeMode == PictureBoxSizeMode.CenterImage)
                { pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    return;
                }

               

            }

        }

     
    }
}