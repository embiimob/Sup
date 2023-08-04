﻿using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUP
{
    public partial class GifTool : Form
    {
        private Timer delayTimer;
        private DiscoBall parentForm; // Reference to the parent form that opened this form
        private List<PictureBox> pictureBoxes;
        private int numPictureBoxesToAdd = 12;
        private int currentPictureBoxIndex = 0;

        public GifTool(DiscoBall parentForm)
        {
            InitializeComponent();
            InitializeDelayTimer();
            this.parentForm = parentForm;
            pictureBoxes = new List<PictureBox>();

        }

        private void GifTool_Load(object sender, EventArgs e)
        {
            FindGifs(txtSearch.Text);
            AddPictureBoxesToFlowLayout();
            flowLayoutPanel1.MouseWheel += new MouseEventHandler(flowLayoutPanel1_Scroll);
        }

        private void InitializeDelayTimer()
        {
            delayTimer = new Timer();
            delayTimer.Interval = 2000; // 2 seconds delay
            delayTimer.Tick += DelayTimer_Tick;
            delayTimer.Stop();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            // Stop the existing timer if running
            delayTimer.Stop();

            // Restart the timer
            delayTimer.Start();
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            delayTimer.Stop();
            string typedText = txtSearch.Text;
            FindGifs(txtSearch.Text);

            // Reset the currentPictureBoxIndex and add the first batch of PictureBoxes
            currentPictureBoxIndex = 0;
           
        }

        private void FindGifs(string searchstring)
        {
            Task BuildMessage = Task.Run(() =>
            {

                Random rnd = new Random();
                string randomGifFile;
                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                if (gifFiles.Length > 0)
                {
                    int randomIndex = rnd.Next(gifFiles.Length);
                    randomGifFile = gifFiles[randomIndex];
                }
                else
                {
                    randomGifFile = @"includes\HugPuddle.jpg";
                }

                this.Invoke((MethodInvoker)delegate
                {
                    pictureBox1.ImageLocation = randomGifFile;
                    pictureBox1.Visible = true;
                    flowLayoutPanel1.Controls.Clear();
                });

                pictureBoxes = new List<PictureBox>();
                string searchAddress = Root.GetPublicAddressByKeyword(searchstring);

                if (searchstring.Length > 20) { searchAddress = searchstring; }
                else
                {
                    PROState searchprofile = PROState.GetProfileByURN(searchstring, "good-user", "better-password", @"http://127.0.0.1:18332");
                    if (searchprofile.Creators != null)
                    {
                        searchAddress = searchprofile.Creators[0];
                    }
                }

                Root[] GIFS = Root.GetRootsByAddress(searchAddress, "good-user", "better-password", @"http://127.0.0.1:18332");
                   
                    
                    
                    foreach (Root GIF in GIFS)
                {
                    foreach (string message in GIF.Message)
                    {
                        // Find all occurrences of strings surrounded by << >> that end in .gif
                        foreach (Match match in Regex.Matches(message, @"<<([^>]*?(\s*\.gif\s*(?=>>|$)|\.gif\s*>>))"))
                        {

                            string GIFUrl = match.Groups[1].Value;

                            PictureBox pictureBox = CreatePictureBox(GIFUrl);

                            
                                pictureBoxes.Add(pictureBox);
                            
                                
                        }
                    }

                    foreach (string attachment in GIF.File.Keys)
                    {
                        // Check if the attachment ends in .GIF (case-insensitive)
                        if (attachment.EndsWith(".GIF", StringComparison.OrdinalIgnoreCase))
                        {
                            string GIFUrl = GIF.TransactionId + @"\" + attachment;

                            PictureBox pictureBox = CreatePictureBox(GIFUrl);

                                pictureBoxes.Add(pictureBox);
                            
                        }
                    }
                }

                this.Invoke((MethodInvoker)delegate
                {
                  pictureBox1.Visible = false;
                  AddPictureBoxesToFlowLayout();
                });
            });
        }

        private PictureBox CreatePictureBox(string imagepath)
        {
            string imagelocation = "";
            
                imagelocation = imagepath;


                if (!imagepath.ToLower().StartsWith("http"))
                {
                    imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (imagepath.ToLower().StartsWith("ipfs:")) { imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\"); if (imagepath.Length == 51) { imagelocation += @"\artifact"; } }
                }
                Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                Match imgurnmatch = regexTransactionId.Match(imagelocation);
                string transactionid = imgurnmatch.Value;
                Root root = new Root();
                if (!File.Exists(imagelocation))
                {
                    switch (imagepath.ToUpper().Substring(0, 4))
                    {
                        case "MZC:":
                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                            break;
                        case "BTC:":

                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                            break;
                        case "LTC:":

                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");


                            break;
                        case "DOG:":
                            Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                            break;
                        case "IPFS":
                            string transid = "empty";
                            try { transid = imagepath.Substring(5, 46); } catch { }

                            if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                            {
                                try { Directory.CreateDirectory("ipfs/" + transid); } catch { };
                                Directory.CreateDirectory("ipfs/" + transid + "-build");
                                Process process2 = new Process();
                                process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                process2.StartInfo.Arguments = "get " + imagepath.Substring(5, 46) + @" -o ipfs\" + transid;
                                process2.StartInfo.UseShellExecute = false;
                                process2.StartInfo.CreateNoWindow = true;
                                process2.Start();
                                process2.WaitForExit();
                                string fileName;
                                if (System.IO.File.Exists("ipfs/" + transid))
                                {
                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                    fileName = imagepath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                    Directory.CreateDirectory("ipfs/" + transid);
                                    System.IO.File.Move("ipfs/" + transid + "_tmp", imagelocation);
                                }

                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                {
                                    fileName = imagepath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, imagelocation);
                                }


                                Process process3 = new Process
                                {
                                    StartInfo = new ProcessStartInfo
                                    {
                                        FileName = @"ipfs\ipfs.exe",
                                        Arguments = "pin add " + transid,
                                        UseShellExecute = false,
                                        CreateNoWindow = true
                                    }
                                };
                                process3.Start();

                                try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }


                            }

                            break;
                        default:
                            if (!imagepath.ToUpper().StartsWith("HTTP") && transactionid != "")
                            {
                                Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:18332");

                            }
                            break;
                    }
                }
        


            PictureBox pictureBox = new PictureBox
            {
                Width = 100,
                Height = 100,
                ImageLocation = imagelocation,
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };

            // Add a Click event handler to each PictureBox
            pictureBox.Click += (s, e) =>
            {
                parentForm.txtAttach.Text = imagepath;
                // Execute the btnAttach_Click function on the parent form asynchronously
                _ = ExecuteBtnAttachClickAsync();
                this.Close();
            };

            return pictureBox;
        }

        private void flowLayoutPanel1_Scroll(object sender, MouseEventArgs e)
        {
            // Check if the user has scrolled to the bottom
            if (flowLayoutPanel1.VerticalScroll.Value + flowLayoutPanel1.ClientSize.Height >= flowLayoutPanel1.VerticalScroll.Maximum)
            {
                // Add more PictureBoxes if available
                AddPictureBoxesToFlowLayout();
            }
        }

        private void AddPictureBoxesToFlowLayout()
        {
            // Determine the number of PictureBoxes to add in this batch
            int countToAdd = Math.Min(numPictureBoxesToAdd, pictureBoxes.Count - currentPictureBoxIndex);

            // Add PictureBoxes to the FlowLayoutPanel
            for (int i = 0; i < countToAdd; i++)
            {
                flowLayoutPanel1.Controls.Add(pictureBoxes[currentPictureBoxIndex]);
                currentPictureBoxIndex++;
            }
        }

        private async Task ExecuteBtnAttachClickAsync()
        {
            // Call the btnAttach_Click function on the parent form asynchronously
            await Task.Run(() => parentForm.btnAttach_Click(this, EventArgs.Empty));
        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {
            // Check if the user has scrolled to the bottom
            if (flowLayoutPanel1.VerticalScroll.Value + flowLayoutPanel1.ClientSize.Height >= flowLayoutPanel1.VerticalScroll.Maximum)
            {
                // Add more PictureBoxes if available
                AddPictureBoxesToFlowLayout();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}