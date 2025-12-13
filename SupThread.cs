using AngleSharp.Text;
using Newtonsoft.Json;
using NReco.VideoConverter;
using SUP.P2FK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SUP
{
    public partial class SupThread : Form
    {

        private string mainnetURL = @"http://127.0.0.1:18332";
        private string mainnetLogin = "good-user";
        private string mainnetPassword = "better-password";
        private string mainnetVersionByte = "111";
        private bool testnet = true;
        private int numMessagesDisplayed = 0;
        private AudioPlayer audioPlayer = new AudioPlayer();
        private string searchAddress = "";
        private string activeUserAddress = "";
        private string activeUserImage = "";
        public SupThread(string _searchAddress = "", string _activeUserAddress = "", string _activeUserImage = "", bool isTestnet = true)
        {
            InitializeComponent();
            supFlow.MouseWheel += supFlow_MouseWheel;

            searchAddress = _searchAddress;
            activeUserAddress = _activeUserAddress;
            activeUserImage = _activeUserImage;
            if (!isTestnet)
            {
                mainnetURL = @"http://127.0.0.1:8332";
                mainnetVersionByte = "0";
                testnet = false;
            }
            RefreshSupMessages();
        }

        private void supFlow_MouseWheel(object sender, MouseEventArgs e)
        {


            if (supFlow.VerticalScroll.Value != 0 && supFlow.VerticalScroll.Value + supFlow.ClientSize.Height >= supFlow.VerticalScroll.Maximum)
            {

                int startNum = numMessagesDisplayed;
                RefreshSupMessages();

                if (numMessagesDisplayed == startNum + 10)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        supFlow.AutoScrollPosition = new Point(0, 10);
                    });
                }



            }
            else if (supFlow.VerticalScroll.Value == 0)
            {
              

                if (numMessagesDisplayed > 10)
                {
                    numMessagesDisplayed = numMessagesDisplayed - 20; if (numMessagesDisplayed < 0) { numMessagesDisplayed = 0; }
                    else
                    {
                        RefreshSupMessages();
                        this.Invoke((MethodInvoker)delegate
                        {
                            supFlow.AutoScrollPosition = new Point(0, supFlow.VerticalScroll.Maximum - 10);

                        });
                    }
                }


            }

        }

        private void RemoveOverFlowMessages(FlowLayoutPanel flowLayoutPanel)
        {

            // Iterate through the controls in the FlowLayoutPanel
            List<Control> controlsList = flowLayoutPanel.Controls.Cast<Control>().ToList();
            foreach (Control control in controlsList)
            {

                Task memoryPrune = Task.Run(() =>
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        flowLayoutPanel.Controls.Remove(control);
                        control.Dispose();
                    });
                });

            }
        }

        void Owner_LinkClicked(object sender, EventArgs e, string ownerId, string imageLocation, string transactionId = "")
        {

            MouseEventArgs me = e as MouseEventArgs;
         

            if (me != null)
            {
                if (me.Button == MouseButtons.Left)
                {



                    if (activeUserAddress != null)
                    {
                        if (transactionId != "")
                        {
                            ownerId = Root.GetPublicAddressByKeyword(transactionId, mainnetVersionByte);
                        }

                        DiscoBall disco = new DiscoBall(activeUserAddress.ToString(), activeUserImage, ownerId,  "includes\\Hugpuddle.jpg", false, testnet);
                        disco.StartPosition = FormStartPosition.CenterScreen;
                        disco.Show(this);
                        disco.Focus();
                    }

                }
                else if (me.Button == MouseButtons.Right && transactionId != "")
                {


                    SupThread thread = new SupThread(Root.GetPublicAddressByKeyword(transactionId, mainnetVersionByte), activeUserAddress, activeUserImage, testnet);
                    thread.StartPosition = FormStartPosition.CenterScreen;
                    thread.Show(this);
                    thread.Focus();


                }
            }
            else
            {



                if (activeUserAddress != null)
                {
                    DiscoBall disco = new DiscoBall(activeUserAddress.ToString(), activeUserImage, ownerId, imageLocation, false, testnet);
                    disco.StartPosition = FormStartPosition.CenterScreen;
                    disco.Show(this);
                    disco.Focus();
                }
            }







        }
        private void ClearMessages(FlowLayoutPanel flowLayoutPanel)
        {

            this.BeginInvoke((MethodInvoker)delegate
            {
                List<Control> controlsList = flowLayoutPanel.Controls.Cast<Control>().ToList();
                flowLayoutPanel.Controls.Clear();

                foreach (Control control in controlsList)
                {

                    control.Dispose();
                }

            });


        }

        void profileImageClick(string ownerId)
        {


        }

        void Attachment_Clicked(string path)
        {
            if (Regex.IsMatch(path, "^[1-9A-HJ-NP-Za-km-z]+$") || path.ToUpper().StartsWith("@") || path.ToUpper().StartsWith("SUP:") || path.ToUpper().StartsWith("IPFS:") || path.ToUpper().StartsWith("BTC:") || path.ToUpper().StartsWith("MZC:") || path.ToUpper().StartsWith("LTC:") || path.ToUpper().StartsWith("DOG:"))
            {
                new ObjectBrowser(path).Show();
            }
            else
            {
                try
                { System.Diagnostics.Process.Start(path); }
                catch { System.Media.SystemSounds.Exclamation.Play(); }
            }
        }

        void deleteme_LinkClicked(string transactionid)
        {

            string unfilteredmessage = "";
            try
            {
                string P2FKJSONString = System.IO.File.ReadAllText(@"root/" + transactionid + @"/ROOT.json");
                Root DeleteRoot = JsonConvert.DeserializeObject<Root>(P2FKJSONString);
                try { unfilteredmessage = DeleteRoot.Message.FirstOrDefault().ToString(); } catch { }
                foreach (string keyword in DeleteRoot.Keyword.Keys) { try { File.Delete(@"root/" + keyword + @"/ROOTS.json"); } catch { } }
            }
            catch { }


            string pattern = "<<.*?>>";
            MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
            foreach (Match match in matches)
            {
                string content = match.Value.Substring(2, match.Value.Length - 4);
                if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id) && !content.Trim().StartsWith("#"))
                {

                    string imagelocation = "";
                    if (content != null)
                    {
                        imagelocation = content;

                        if (!content.ToLower().StartsWith("http"))
                        {
                            imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");
                            if (content.ToLower().StartsWith("ipfs:"))
                            {
                                imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\");
                                if (content.Length == 51) { imagelocation += @"\artifact"; }
                            }

                            string parentDir = Path.GetDirectoryName(imagelocation);

                            if (Directory.Exists(parentDir))
                            {
                                Directory.Delete(parentDir, true);
                            }

                            parentDir = parentDir.Replace(@"\ipfs\", @"\root\");

                            if (Directory.Exists(parentDir))
                            {
                                Directory.Delete(parentDir, true);
                            }

                            if (Directory.Exists(parentDir + "-build"))
                            {
                                Directory.Delete(parentDir + "-build", true);
                            }

                        }
                    }


                }
            }

            try
            {
                Directory.Delete(@"root\" + transactionid, true);
                Directory.CreateDirectory(@"root\" + transactionid);
            }
            catch { }
            Root P2FKRoot = new Root();
            P2FKRoot.Confirmations = 1;
            var rootSerialized = JsonConvert.SerializeObject(P2FKRoot);
            System.IO.File.WriteAllText(@"root\" + transactionid + @"\" + "ROOT.json", rootSerialized);
        }


        string TruncateAddress(string input)
        {
            if (input.Length <= 13)
            {
                return input;
            }
            else
            {
                return input.Substring(0, 5) + "..." + input.Substring(input.Length - 5);
            }
        }



        private void RefreshSupMessages()
        {

            // sorry cannot run two searches at a time
            if (System.IO.File.Exists("ROOTS-PROCESSING"))
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }


            List<MessageObject> messages = new List<MessageObject>();

            try { messages = OBJState.GetPublicMessagesByAddress(searchAddress.ToString(), mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte, numMessagesDisplayed, 10); }
            catch { }

            if (messages.Count == 0 && numMessagesDisplayed == 0)
            {
                Label noMentionsLabel = new Label();
                noMentionsLabel.Text = "No mentions found";
                noMentionsLabel.AutoSize = true;
                noMentionsLabel.TextAlign = ContentAlignment.MiddleCenter;

                // Set padding to space it nicely from the top
                noMentionsLabel.Padding = new Padding(0, 20, 0, 0);

                // Set a bigger font
                noMentionsLabel.Font = new Font(noMentionsLabel.Font.FontFamily, 16);

                // Center the label within the FlowLayoutPanel
                noMentionsLabel.Anchor = AnchorStyles.None;
                noMentionsLabel.Dock = DockStyle.None;

                // Add the label to the FlowLayoutPanel
                supFlow.Controls.Add(noMentionsLabel);

                // Center the label manually
                noMentionsLabel.Location = new Point(
                    (supFlow.ClientSize.Width - noMentionsLabel.Width) / 2,
                    noMentionsLabel.Padding.Top
                );
                noMentionsLabel.Anchor = AnchorStyles.Top;
            }


            supFlow.SuspendLayout();

            if (messages.Count == 10)
            {
                Task memoryPrune = Task.Run(() =>
                {

                    RemoveOverFlowMessages(supFlow);

                });
            }


            Dictionary<string, string[]> profileAddress = new Dictionary<string, string[]> { };

            foreach (MessageObject messagePacket in messages)
            {
                numMessagesDisplayed++;

                string message = "";
                string tid = messagePacket.TransactionId.ToString();
                string tstamp = messagePacket.BlockDate.ToString("yyyyMMddHHmmss");
                try
                {


                    message = messagePacket.Message.ToString();

                    string relativeFolderPath = @"root\" + tid;
                    string folderPath = Path.Combine(Environment.CurrentDirectory, relativeFolderPath);

                    string[] files = Directory.GetFiles(folderPath);

                    foreach (string file in files)
                    {
                        string extension = Path.GetExtension(file);

                        if (Path.GetFileName(file) == "INQ" || (!string.IsNullOrEmpty(extension) && !file.Contains("ROOT.json")))
                        {
                            message = message + @"<<" + tid + @"/" + Path.GetFileName(file) + ">>";
                        }

                    }

                    string fromAddress = messagePacket.FromAddress;
                    string toAddress = messagePacket.ToAddress;
                    string fromImage = "";
                    string toImage = "";


                    string unfilteredmessage = message;
                    string[] blocks = Regex.Matches(message, "<<[^<>]+>>")
                                             .Cast<Match>()
                                             .Select(m => m.Value.Trim(new char[] { '<', '>' }))
                                             .ToArray();
                    message = Regex.Replace(message, "<<.*?>>", "❤️");


                    if (message != "" || blocks.Length > 1 || (blocks.Length == 1 && !int.TryParse(blocks[0], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out _)))
                    {

                        if (!profileAddress.ContainsKey(fromAddress))
                        {

                            PROState profile = PROState.GetProfileByAddress(fromAddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                            if (profile.URN != null)
                            {
                                fromAddress = TruncateAddress(profile.URN);

                                if (profile.Image != null)
                                {
                                    fromImage = profile.Image;


                                    if (!profile.Image.ToLower().StartsWith("http"))
                                    {
                                        fromImage = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + profile.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                                        if (profile.Image.ToLower().StartsWith("ipfs:")) { fromImage = fromImage.Replace(@"\root\", @"\ipfs\"); if (profile.Image.Length == 51) { fromImage += @"\artifact"; } }
                                    }
                                    Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                                    Match imgurnmatch = regexTransactionId.Match(fromImage);
                                    string transactionid = imgurnmatch.Value;
                                    Root root = new Root();
                                    if (!File.Exists(fromImage))
                                    {
                                        switch (profile.Image.ToUpper().Substring(0, 4))
                                        {
                                            case "MZC:":
                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                                break;
                                            case "BTC:":

                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                                break;
                                            case "LTC:":

                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");


                                                break;
                                            case "DOG:":
                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                                break;
                                            case "IPFS":
                                                string transid = "empty";
                                                try { transid = profile.Image.Substring(5, 46); } catch { }

                                                if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                                {
                                                    try
                                                    {
                                                        Directory.CreateDirectory("ipfs/" + transid);
                                                    }
                                                    catch { };

                                                    Directory.CreateDirectory("ipfs/" + transid + "-build");
                                                    Process process2 = new Process();
                                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                    process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                    process2.StartInfo.UseShellExecute = false;
                                                    process2.StartInfo.CreateNoWindow = true;
                                                    process2.Start();
                                                    if (process2.WaitForExit(5000))
                                                    {
                                                        string fileName;
                                                        if (System.IO.File.Exists("ipfs/" + transid))
                                                        {
                                                            System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                            System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                            fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                            Directory.CreateDirectory("ipfs/" + transid);
                                                            System.IO.File.Move("ipfs/" + transid + "_tmp", fromImage);
                                                        }

                                                        if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                        {
                                                            fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                            System.IO.File.Move("ipfs/" + transid + "/" + transid, fromImage);
                                                        }

                                                        try
                                                        {
                                                            if (File.Exists("IPFS_PINNING_ENABLED"))
                                                            {
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
                                                            }
                                                        }
                                                        catch { }

                                                        try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                                    }
                                                    else
                                                    {
                                                        process2.Kill();

                                                        Task.Run(() =>
                                                        {
                                                            process2 = new Process();
                                                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                            process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                            process2.StartInfo.UseShellExecute = false;
                                                            process2.StartInfo.CreateNoWindow = true;
                                                            process2.Start();
                                                            if (process2.WaitForExit(550000))
                                                            {
                                                                string fileName;
                                                                if (System.IO.File.Exists("ipfs/" + transid))
                                                                {
                                                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                                    Directory.CreateDirectory("ipfs/" + transid);
                                                                    System.IO.File.Move("ipfs/" + transid + "_tmp", fromImage);
                                                                }

                                                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                                {
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, fromImage);
                                                                }

                                                                try
                                                                {
                                                                    if (File.Exists("IPFS_PINNING_ENABLED"))
                                                                    {
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
                                                                    }
                                                                }
                                                                catch { }

                                                                try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }


                                                            }
                                                            else
                                                            {
                                                                process2.Kill();
                                                            }
                                                        });

                                                    }


                                                }

                                                break;
                                            default:
                                                if (!profile.Image.ToUpper().StartsWith("HTTP") && transactionid != "")
                                                {
                                                    root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                                }
                                                break;
                                        }
                                    }



                                }


                            }
                            else
                            { fromAddress = TruncateAddress(fromAddress); }

                            string[] profilePacket = new string[2];

                            profilePacket[0] = fromAddress;
                            profilePacket[1] = fromImage;
                            profileAddress.Add(messagePacket.FromAddress, profilePacket);

                        }
                        else
                        {
                            string[] profilePacket = new string[] { };
                            profileAddress.TryGetValue(fromAddress, out profilePacket);
                            fromAddress = profilePacket[0];
                            fromImage = profilePacket[1];

                        }


                        if (!profileAddress.ContainsKey(toAddress))
                        {

                            PROState profile = PROState.GetProfileByAddress(toAddress, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                            if (profile.URN != null)
                            {
                                toAddress = TruncateAddress(profile.URN);

                                if (profile.Image != null)
                                {
                                    toImage = profile.Image;


                                    if (!profile.Image.ToLower().StartsWith("http"))
                                    {
                                        toImage = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + profile.Image.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                                        if (profile.Image.ToLower().StartsWith("ipfs:")) { toImage = toImage.Replace(@"\root\", @"\ipfs\"); if (profile.Image.Length == 51) { toImage += @"\artifact"; } }
                                    }
                                    Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                                    Match imgurnmatch = regexTransactionId.Match(toImage);
                                    string transactionid = imgurnmatch.Value;
                                    Root root = new Root();
                                    if (!File.Exists(toImage))
                                    {
                                        switch (profile.Image.ToUpper().Substring(0, 4))
                                        {
                                            case "MZC:":
                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:12832", "50");

                                                break;
                                            case "BTC:":

                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:8332", "0");

                                                break;
                                            case "LTC:":

                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:9332", "48");


                                                break;
                                            case "DOG:":
                                                root = Root.GetRootByTransactionId(transactionid, "good-user", "better-password", @"http://127.0.0.1:22555", "30");

                                                break;
                                            case "IPFS":
                                                string transid = "empty";
                                                try { transid = profile.Image.Substring(5, 46); } catch { }

                                                if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                                {
                                                    try
                                                    {
                                                        Directory.CreateDirectory("ipfs/" + transid);
                                                    }
                                                    catch { };

                                                    Directory.CreateDirectory("ipfs/" + transid + "-build");
                                                    Process process2 = new Process();
                                                    process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                    process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                    process2.StartInfo.UseShellExecute = false;
                                                    process2.StartInfo.CreateNoWindow = true;
                                                    process2.Start();
                                                    if (process2.WaitForExit(5000))
                                                    {
                                                        string fileName;
                                                        if (System.IO.File.Exists("ipfs/" + transid))
                                                        {
                                                            System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                            System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                            fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                            Directory.CreateDirectory("ipfs/" + transid);
                                                            System.IO.File.Move("ipfs/" + transid + "_tmp", toImage);
                                                        }

                                                        if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                        {
                                                            fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                            if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                            System.IO.File.Move("ipfs/" + transid + "/" + transid, toImage);
                                                        }

                                                        try
                                                        {
                                                            if (File.Exists("IPFS_PINNING_ENABLED"))
                                                            {
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
                                                            }
                                                        }
                                                        catch { }

                                                        try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                                    }
                                                    else
                                                    {
                                                        process2.Kill();

                                                        Task.Run(() =>
                                                        {
                                                            process2 = new Process();
                                                            process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                                            process2.StartInfo.Arguments = "get " + transid + @" -o ipfs\" + transid;
                                                            process2.StartInfo.UseShellExecute = false;
                                                            process2.StartInfo.CreateNoWindow = true;
                                                            process2.Start();
                                                            if (process2.WaitForExit(550000))
                                                            {
                                                                string fileName;
                                                                if (System.IO.File.Exists("ipfs/" + transid))
                                                                {
                                                                    System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                                    System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                                    Directory.CreateDirectory("ipfs/" + transid);
                                                                    System.IO.File.Move("ipfs/" + transid + "_tmp", toImage);
                                                                }

                                                                if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                                                {
                                                                    fileName = profile.Image.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                                    if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                                    System.IO.File.Move("ipfs/" + transid + "/" + transid, toImage);
                                                                }

                                                                try
                                                                {
                                                                    if (File.Exists("IPFS_PINNING_ENABLED"))
                                                                    {
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
                                                                    }
                                                                }
                                                                catch { }

                                                                try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }


                                                            }
                                                            else
                                                            {
                                                                process2.Kill();
                                                            }
                                                        });

                                                    }


                                                }

                                                break;
                                            default:
                                                if (!profile.Image.ToUpper().StartsWith("HTTP") && transactionid != "")
                                                {
                                                    root = Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                                }
                                                break;
                                        }
                                    }



                                }


                            }
                            else
                            { toAddress = TruncateAddress(toAddress); }

                            string[] profilePacket = new string[2];

                            profilePacket[0] = toAddress;
                            profilePacket[1] = toImage;
                            profileAddress.Add(messagePacket.ToAddress, profilePacket);

                        }
                        else
                        {
                            string[] profilePacket = new string[] { };
                            profileAddress.TryGetValue(toAddress, out profilePacket);
                            toAddress = profilePacket[0];
                            toImage = profilePacket[1];

                        }



                        System.Drawing.Color bgcolor = System.Drawing.Color.White;



                        CreateRow(fromImage, fromAddress, messagePacket.FromAddress, DateTime.ParseExact(tstamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), message, tid, false, supFlow);
                        CreateRow(toImage, toAddress, messagePacket.ToAddress, DateTime.ParseExact("19700101010101", "yyyyMMddHHmmss", CultureInfo.InvariantCulture), "", tid, false, supFlow);

                        bool containsFileWithINQ = files.Any(file =>
                        file.EndsWith("INQ", StringComparison.OrdinalIgnoreCase) &&
                        !file.EndsWith("BLOCK", StringComparison.OrdinalIgnoreCase));

                        if (containsFileWithINQ)
                        {
                            //ADD INQ IF IT EXISTS AND IS NOT BLOCKED

                            string profileowner = "";

                            if (activeUserAddress != null) { profileowner = activeUserAddress.ToString(); }
                            FoundINQControl foundObject = new FoundINQControl(messagePacket.TransactionId, profileowner, testnet);

                            foundObject.Margin = new Padding(20, 7, 8, 7);
                            supFlow.Controls.Add(foundObject);

                        }

                        string pattern = "<<.*?>>";
                        List<string> imgExtensions = new List<string> { ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".mp4", ".mov", ".avi", ".wav", ".mp3" };

                        MatchCollection matches = Regex.Matches(unfilteredmessage, pattern);
                        foreach (Match match in matches)
                        {


                            string content = match.Value.Substring(2, match.Value.Length - 4);

                            if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int cnt) && !content.Trim().StartsWith("#") && !content.EndsWith(@"/INQ"))
                            {



                                string imgurn = content;

                                if (!content.ToLower().StartsWith("http"))
                                {
                                    imgurn = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + content.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace("btc:", "").Replace("mzc:", "").Replace("ltc:", "").Replace("dog:", "").Replace("ipfs:", "").Replace(@"/", @"\");

                                    if (content.ToLower().StartsWith("ipfs:")) { imgurn = imgurn.Replace(@"\root\", @"\ipfs\"); }
                                }

                                string extension = Path.GetExtension(imgurn).ToLower();
                                if (!imgExtensions.Contains(extension) && !imgurn.Contains("youtube.com") && !imgurn.Contains("youtu.be"))
                                {
                                    string title = content;
                                    string description = content;
                                    string imageUrl = @"includes\disco.png";

                                    // Create a new panel to display the metadata
                                    Panel panel = new Panel();
                                    panel.BorderStyle = BorderStyle.FixedSingle;
                                    panel.Size = new Size(supFlow.Width - 50, 100);

                                    // Create a label for the title
                                    Label titleLabel = new Label();
                                    titleLabel.Text = title;
                                    titleLabel.Dock = DockStyle.Top;
                                    titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                                    titleLabel.ForeColor = Color.White;
                                    titleLabel.MinimumSize = new Size(supFlow.Width - 150, 30);
                                    titleLabel.Padding = new Padding(5);
                                    titleLabel.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                    panel.Controls.Add(titleLabel);

                                    // Create a label for the description
                                    Label descriptionLabel = new Label();
                                    descriptionLabel.Text = description;
                                    descriptionLabel.ForeColor = Color.White;
                                    descriptionLabel.Dock = DockStyle.Fill;
                                    descriptionLabel.Padding = new Padding(5, 40, 5, 5);
                                    descriptionLabel.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                    panel.Controls.Add(descriptionLabel);



                                    // Create a new PictureBox control and add it to the panel
                                    PictureBox pictureBox = new PictureBox();
                                    pictureBox.Dock = DockStyle.Left;
                                    pictureBox.Size = new Size(100, 100);
                                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                                    pictureBox.ImageLocation = imageUrl;
                                    pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(content); };
                                    panel.Controls.Add(pictureBox);

                                    this.supFlow.Controls.Add(panel);

                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            string html = "";
                                            WebClient client = new WebClient();
                                            // Create a WebClient object to fetch the webpage
                                            if (!content.ToLower().EndsWith(".zip"))
                                            {
                                                html = client.DownloadString(content.StripLeadingTrailingSpaces());
                                            }
                                            // Create a MemoryStream object from the image data

                                            // Use regular expressions to extract the metadata from the HTML
                                            title = Regex.Match(html, @"<title>\s*(.+?)\s*</title>").Groups[1].Value;
                                            description = Regex.Match(html, @"<meta\s+name\s*=\s*""description""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;
                                            imageUrl = Regex.Match(html, @"<meta\s+property\s*=\s*""og:image""\s+content\s*=\s*""(.+?)""\s*/?>").Groups[1].Value;

                                            byte[] imageData = client.DownloadData(imageUrl);
                                            MemoryStream memoryStream = new MemoryStream(imageData);

                                            this.Invoke((MethodInvoker)delegate
                                            {
                                                titleLabel.Text = title;
                                                descriptionLabel.Text = description;
                                                pictureBox.ImageLocation = null;
                                                pictureBox.Image = System.Drawing.Image.FromStream(memoryStream);
                                            });



                                        }
                                        catch { }
                                    });


                                }
                                else
                                {


                                    if (!int.TryParse(content, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out int id))
                                    {

                                        if (extension == ".mp4" || extension == ".mov" || extension == ".avi" || content.Contains("youtube.com") || content.Contains("youtu.be") || extension == ".wav" || extension == ".mp3")
                                        {

                                            try { AddMedia(content); } catch { }


                                        }
                                        else
                                        {


                                            AddImage(content);

                                        }
                                    }

                                }
                            }
                        }

                        TableLayoutPanel padding = new TableLayoutPanel
                        {
                            RowCount = 1,
                            ColumnCount = 1,
                            Dock = DockStyle.Top,
                            BackColor = Color.Black,
                            ForeColor = Color.White,
                            AutoSize = true,
                            AutoSizeMode = AutoSizeMode.GrowAndShrink,
                            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                            Margin = new System.Windows.Forms.Padding(0, 10, 0, 10),
                            Padding = new System.Windows.Forms.Padding(0),
                        };

                        // Add the TableLayoutPanel to the FlowLayoutPanel
                        supFlow.Controls.Add(padding);

                        // Set the initial column style
                        // Set the initial width of the first column
                        padding.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, supFlow.Width - 50));
                       supFlow.SizeChanged += (sender, e) =>
                        {

                            padding.ColumnStyles[0].Width = supFlow.Width - 100;
                            padding.Width = supFlow.Width - 100;

                        };


                    }
                }
                catch (Exception ex)
                {
                    string help = ex.Message;
                }//deleted file



            }


       
                supFlow.ResumeLayout();
            
        }
        void AddImage(string imagepath, bool isprivate = false, bool addtoTop = false)
        {
            string imagelocation = "";
            if (imagepath != null)
            {
                TableLayoutPanel msg = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 1,
                    Dock = DockStyle.Top,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true,
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                    Margin = new System.Windows.Forms.Padding(0, 0, 0, 0),
                    Padding = new System.Windows.Forms.Padding(0)

                };

                int calcwidth;

                calcwidth = supFlow.Width - 50;
                if (calcwidth > 480) { calcwidth = 480; }
                msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, calcwidth));





                if (addtoTop)
                {
                    supFlow.Controls.Add(msg);
                    supFlow.Controls.SetChildIndex(msg, 2);
                }
                else
                {
                    supFlow.Controls.Add(msg);
                }



                PictureBox pictureBox = new PictureBox();

                // Set the PictureBox properties

                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                pictureBox.Width = calcwidth;
                pictureBox.Height = calcwidth;


                pictureBox.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
                pictureBox.ImageLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\includes\progress.gif";

                msg.Controls.Add(pictureBox);
                //pictures.Add(pictureBox);

                imagelocation = imagepath;

                if (!imagepath.ToLower().StartsWith("http"))
                {
                    imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + imagepath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (imagepath.ToLower().StartsWith("ipfs:")) { imagelocation = imagelocation.Replace(@"\root\", @"\ipfs\"); if (imagepath.Length == 51) { imagelocation += @"\artifact"; } }

                    Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                    Match imgurnmatch = regexTransactionId.Match(imagelocation);
                    string transactionid = imgurnmatch.Value;
                    Root root = new Root();
                    if (!File.Exists(imagelocation))
                    {

                        Task.Run(() =>
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
                                        try
                                        {
                                            Directory.CreateDirectory("ipfs/" + transid);
                                        }
                                        catch { };

                                        Directory.CreateDirectory("ipfs/" + transid + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + imagepath.Substring(5, 46) + @" -o ipfs\" + transid;
                                        process2.StartInfo.UseShellExecute = false;
                                        process2.StartInfo.CreateNoWindow = true;
                                        process2.Start();
                                        if (process2.WaitForExit(550000))
                                        {
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

                                            try
                                            {
                                                if (File.Exists("IPFS_PINNING_ENABLED"))
                                                {
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
                                                }
                                            }
                                            catch { }

                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                        }
                                        else
                                        {
                                            process2.Kill();
                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }

                                            this.Invoke((Action)(() =>
                                            {

                                                Random rnd = new Random();
                                                string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                                                if (gifFiles.Length > 0)
                                                {
                                                    int randomIndex = rnd.Next(gifFiles.Length);
                                                    pictureBox.ImageLocation = gifFiles[randomIndex];

                                                }
                                                else
                                                {
                                                    try
                                                    {

                                                        pictureBox.ImageLocation = @"includes\HugPuddle.jpg";
                                                    }
                                                    catch { }
                                                }
                                            }));
                                        }

                                    }

                                    break;
                                default:
                                    if (!imagepath.ToUpper().StartsWith("HTTP") && transactionid != "")
                                    {
                                        Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                    }
                                    break;

                            }
                            if (File.Exists(imagelocation))
                            {
                                this.Invoke((Action)(() =>
                                {
                                    pictureBox.ImageLocation = imagelocation;
                                    pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };

                                }));
                            }
                            else
                            {
                                this.Invoke((Action)(() =>
                                {

                                    Random rnd = new Random();
                                    string[] gifFiles = Directory.GetFiles("includes", "*.gif");
                                    if (gifFiles.Length > 0)
                                    {
                                        int randomIndex = rnd.Next(gifFiles.Length);
                                        pictureBox.ImageLocation = gifFiles[randomIndex];

                                    }
                                    else
                                    {
                                        try
                                        {

                                            pictureBox.ImageLocation = @"includes\HugPuddle.jpg";
                                        }
                                        catch { }
                                    }
                                }));
                            }


                        });



                    }
                    else
                    {
                        pictureBox.ImageLocation = imagelocation;
                        pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };

                    }
                }
                else
                {

                    Task.Run(() =>
                    {
                        this.Invoke((Action)(() =>
                        {

                            pictureBox.ImageLocation = imagelocation;


                            pictureBox.MouseClick += (sender, e) => { Attachment_Clicked(imagelocation); };
                        }));

                    });

                }


            }


        }

        async void AddMedia(string videopath, bool isprivate = false, bool addtoTop = false, bool autoPlay = false)
        {
            string videolocation = "";
            if (videopath != null)
            {
                videolocation = videopath;

                //build web viewer with default loading page
                Microsoft.Web.WebView2.WinForms.WebView2 webviewer = new Microsoft.Web.WebView2.WinForms.WebView2();
                webviewer.AllowExternalDrop = false;
                webviewer.TabStop = false;
                webviewer.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
                webviewer.CreationProperties = null;
                webviewer.DefaultBackgroundColor = System.Drawing.Color.FromArgb(22, 22, 22);
                webviewer.Name = "webviewer";



                if (videolocation.ToLower().EndsWith(".wav") || videolocation.ToLower().EndsWith(".mp3"))
                {



                    int calcwidth = supFlow.Width - 50;
                    if (calcwidth > 480) { calcwidth = 480; }
                    webviewer.Size = new System.Drawing.Size(calcwidth, 150);

                }
                else
                {

                    int calcwidth = supFlow.Width - 50;
                    if (calcwidth > 960) { calcwidth = 960; }
                    webviewer.Size = new System.Drawing.Size(calcwidth, calcwidth - 100);

                }
                webviewer.ZoomFactor = 1D;




                TableLayoutPanel msg = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 1,
                    Dock = DockStyle.Top,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true,
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                    Margin = new System.Windows.Forms.Padding(0, 0, 0, 0),
                    Padding = new System.Windows.Forms.Padding(0)

                };



                msg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, supFlow.Width - 50));

                if (addtoTop)
                {

                    supFlow.Controls.Add(msg);
                    supFlow.Controls.SetChildIndex(msg, 2);


                }
                else
                {

                    supFlow.Controls.Add(msg);


                }


                msg.Controls.Add(webviewer);

                // Forward mouse wheel events to parent to allow scrolling over video
                webviewer.MouseWheel += (s, e) =>
                {
                    // Call the parent's scroll handler first to handle boundary logic (loading more messages)
                    supFlow_MouseWheel(supFlow, e);
                    
                    // Then manually update scroll position since the event was consumed by WebView2
                    // and won't automatically scroll the parent container
                    int newValue = supFlow.VerticalScroll.Value - e.Delta;
                    if (newValue < 0) newValue = 0;
                    if (newValue > supFlow.VerticalScroll.Maximum - supFlow.VerticalScroll.LargeChange + 1)
                        newValue = supFlow.VerticalScroll.Maximum - supFlow.VerticalScroll.LargeChange + 1;
                    supFlow.AutoScrollPosition = new Point(0, newValue);
                };

                try { await webviewer.EnsureCoreWebView2Async(); } catch { }
                // immediately load Progress content into the WebView2 control
                try { webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\includes\loading.html"); } catch { }


                if (!videopath.ToLower().StartsWith("http"))
                {
                    videolocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\root\" + videopath.Replace("BTC:", "").Replace("MZC:", "").Replace("LTC:", "").Replace("DOG:", "").Replace("IPFS:", "").Replace(@"/", @"\");
                    if (videopath.ToLower().StartsWith("ipfs:")) { videolocation = videolocation.Replace(@"\root\", @"\ipfs\"); if (videopath.Length == 51) { videolocation += @"\artifact"; } }


                    if (!File.Exists(videolocation))
                    {



                        Task.Run(() =>
                        {
                            Regex regexTransactionId = new Regex(@"\b[0-9a-f]{64}\b");
                            Match imgurnmatch = regexTransactionId.Match(videolocation);
                            string transactionid = imgurnmatch.Value;
                            Root root = new Root();

                            switch (videopath.ToUpper().Substring(0, 4))
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
                                    try { transid = videopath.Substring(5, 46); } catch { }

                                    if (!System.IO.Directory.Exists("ipfs/" + transid + "-build"))
                                    {
                                        try
                                        {
                                            Directory.CreateDirectory("ipfs/" + transid);
                                        }
                                        catch { };

                                        Directory.CreateDirectory("ipfs/" + transid + "-build");
                                        Process process2 = new Process();
                                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                                        process2.StartInfo.Arguments = "get " + videopath.Substring(5, 46) + @" -o ipfs\" + transid;
                                        process2.StartInfo.UseShellExecute = false;
                                        process2.StartInfo.CreateNoWindow = true;
                                        process2.Start();
                                        if (process2.WaitForExit(550000))
                                        {
                                            string fileName;
                                            if (System.IO.File.Exists("ipfs/" + transid))
                                            {
                                                System.IO.File.Move("ipfs/" + transid, "ipfs/" + transid + "_tmp");
                                                System.IO.Directory.CreateDirectory("ipfs/" + transid);
                                                fileName = videopath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }
                                                Directory.CreateDirectory("ipfs/" + transid);
                                                System.IO.File.Move("ipfs/" + transid + "_tmp", videolocation);
                                            }

                                            if (System.IO.File.Exists("ipfs/" + transid + "/" + transid))
                                            {
                                                fileName = videopath.Replace(@"//", "").Replace(@"\\", "").Substring(51);
                                                if (fileName == "") { fileName = "artifact"; } else { fileName = fileName.Replace(@"/", "").Replace(@"\", ""); }

                                                System.IO.File.Move("ipfs/" + transid + "/" + transid, videolocation);
                                            }

                                            try
                                            {
                                                if (File.Exists("IPFS_PINNING_ENABLED"))
                                                {
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
                                                }
                                            }
                                            catch { }
                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                        }
                                        else
                                        {
                                            process2.Kill();

                                            try { Directory.Delete("ipfs/" + transid + "-build", true); } catch { }
                                        }



                                    }

                                    break;

                                default:
                                    if (!videopath.ToUpper().StartsWith("HTTP") && transactionid != "")
                                    {
                                        Root.GetRootByTransactionId(transactionid, mainnetLogin, mainnetPassword, mainnetURL, mainnetVersionByte);

                                    }
                                    break;
                            }

                            if (File.Exists(videolocation))
                            {
                                if (videolocation.ToLower().EndsWith(".mov"))
                                {
                                    string inputFilePath = videolocation;
                                    string outputFilePath = System.IO.Path.ChangeExtension(inputFilePath, ".mp4");
                                    if (!File.Exists(outputFilePath))
                                    {
                                        try
                                        {
                                            var ffMpeg = new FFMpegConverter();
                                            ffMpeg.ConvertMedia(inputFilePath, outputFilePath, Format.mp4);
                                            videolocation = outputFilePath;
                                        }
                                        catch { }
                                    }
                                    else { videolocation = outputFilePath; }
                                }

                                this.Invoke((Action)(() =>
                                {

                                    if ((videolocation.ToLower().EndsWith(".wav") || videolocation.ToLower().EndsWith(".mp3")) && autoPlay)
                                    {
                                        audioPlayer.AddToPlaylist(videolocation);

                                    }
                                    string encodedPath = "file:///" + Uri.EscapeUriString(videolocation.Replace('\\', '/'));
                                    string viewerPath = Path.GetDirectoryName(videolocation) + @"\urnviewer.html";
                                    string htmlstring = $"<html><body><embed src=\"{encodedPath}\" width=100% height=100% ></body></html>";

                                    System.IO.File.WriteAllText(viewerPath, htmlstring);

                                    try { webviewer.CoreWebView2.Navigate(viewerPath); } catch { }


                                }));


                            }
                            else
                            {
                                try { webviewer.CoreWebView2.Navigate(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\includes\notfound.html"); } catch { }

                            }



                        });




                    }
                    else
                    {

                        if ((videolocation.ToLower().EndsWith(".wav") || videolocation.ToLower().EndsWith(".mp3")) && autoPlay)
                        {
                            audioPlayer.AddToPlaylist(videolocation);
                        }
                        if (videolocation.ToLower().EndsWith(".mov"))
                        {
                            string inputFilePath = videolocation;
                            string outputFilePath = System.IO.Path.ChangeExtension(inputFilePath, ".mp4");
                            videolocation = outputFilePath;
                        }

                        string encodedPath = "file:///" + Uri.EscapeUriString(videolocation.Replace('\\', '/'));
                        string viewerPath = Path.GetDirectoryName(videolocation) + @"\urnviewer.html";
                        string htmlstring = $"<html><body><embed src=\"{encodedPath}\" width=100% height=100% ></body></html>";


                        System.IO.File.WriteAllText(viewerPath, htmlstring);
                        try { webviewer.CoreWebView2.Navigate(viewerPath); } catch { }
                    }


                }
                else
                {
                    string pattern = @"(?:youtu\.be/|youtube(?:-nocookie)?\.com/(?:[^/\n\s]*[/\n\s]*(?:v/|e(?:mbed)?/|.*[?&]v=))?)?([a-zA-Z0-9_-]{11})";

                    Match match = Regex.Match(videopath, pattern);
                    if (match.Success)
                    {
                        videolocation = @"https://www.youtube.com/embed/" + match.Groups[1].Value;
                    }

                    try { webviewer.CoreWebView2.Navigate(videolocation); } catch { }
                }



            }


        }


        void CreateRow(string imageLocation, string ownerName, string ownerId, DateTime timestamp, string messageText, string transactionid, bool isprivate, FlowLayoutPanel layoutPanel, bool addtoTop = false)
        {


            if (!isprivate)
            {
                // Create a table layout panel for each row
                TableLayoutPanel row = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 5,
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };
                // Add the width of the first column to fixed value and second to fill remaining space
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));


                // Create a PictureBox with the specified image

                if (File.Exists(imageLocation) || imageLocation.ToUpper().StartsWith("HTTP"))
                {
                    PictureBox picture = new PictureBox
                    {
                        Size = new System.Drawing.Size(50, 50),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        ImageLocation = imageLocation,
                        Margin = new System.Windows.Forms.Padding(0)

                    };
                    picture.Click += (sender, e) => { profileImageClick(ownerId); };
                    row.Controls.Add(picture, 0, 0);
                    //pictures.Add(picture);
                }
                else
                {
                    string errorImageUrl = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "includes\\anon.png");


                    PictureBox picture = new PictureBox
                    {
                        Size = new System.Drawing.Size(50, 50),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        ImageLocation = errorImageUrl,
                        Margin = new System.Windows.Forms.Padding(0),
                    };
                    picture.Click += (sender, e) => { profileImageClick(ownerId); };
                    row.Controls.Add(picture, 0, 0);
                    //pictures.Add(picture);
                }


                // Create a LinkLabel with the owner name
                LinkLabel owner = new LinkLabel
                {
                    Text = ownerName,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true

                };
                owner.LinkClicked += (sender, e) => { Owner_LinkClicked(sender, e, ownerId, imageLocation); };
                owner.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                owner.Margin = new System.Windows.Forms.Padding(3);
                owner.Dock = DockStyle.Bottom;
                row.Controls.Add(owner, 1, 0);

                if (timestamp.Year > 1975)
                {
                    // Create a LinkLabel with the owner name
                    Label tstamp = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.Gray,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };
                    row.Controls.Add(tstamp, 2, 0);


                    // Create the label
                    Label loveme = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.Red,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = "🤍",
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };

                    // Add click event handler
                    loveme.Click += (sender, e) => { Owner_LinkClicked(sender, e, ownerId, imageLocation, transactionid); loveme.ForeColor = Color.Blue; };

                    // Add label to the TableLayoutPanel
                    row.Controls.Add(loveme, 3, 0);

                    // Make sure the form's handle is created before starting the task
                    var handle = this.Handle;

                    Task.Run(() =>
                    {
                        // Fetch the roots
                        Root[] roots = Root.GetRootsByAddress(Root.GetPublicAddressByKeyword(transactionid, mainnetVersionByte), mainnetLogin, mainnetPassword, mainnetURL, 0, -1, mainnetVersionByte);

                        if (roots != null && roots.Length > 0)
                        {
                            // Update the label on the UI thread
                            loveme.Invoke((MethodInvoker)delegate
                            {
                                loveme.Text = "🖤 " + roots.Length.ToString();
                                loveme.ForeColor = Color.Red;
                                loveme.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            });
                        }
                    });



                    Label deleteme = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.White,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = "🗑",
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };
                    deleteme.Click += (sender, e) =>
                    {
                        deleteme_LinkClicked(transactionid);
                        deleteme.ForeColor = Color.Black;
                    };
                    row.Controls.Add(deleteme, 4, 0);
                }


                if (addtoTop)
                {
                    layoutPanel.Controls.Add(row);
                    layoutPanel.Controls.SetChildIndex(row, 0);
                }
                else
                {
                    layoutPanel.Controls.Add(row);
                }
            }
            else
            {

                // Create a table layout panel for each row
                TableLayoutPanel row = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 2,
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };
                // Add the width of the first column to fixed value and second to fill remaining space
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));

                // Create a table layout panel for each row
                TableLayoutPanel row2 = new TableLayoutPanel
                {
                    RowCount = 1,
                    ColumnCount = 2,
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };
                row2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
                row2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));


                // Create a PictureBox with the specified image

                if (File.Exists(imageLocation) || imageLocation.ToUpper().StartsWith("HTTP"))
                {
                    PictureBox picture = new PictureBox
                    {
                        Size = new System.Drawing.Size(50, 50),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        ImageLocation = imageLocation,
                        Margin = new System.Windows.Forms.Padding(0)

                    };
                    picture.Click += (sender, e) => { profileImageClick(ownerId); };
                    row.Controls.Add(picture, 0, 0);
                    //pictures.Add(picture);
                }
                else
                {
                    string errorImageUrl = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "includes\\anon.png");


                    PictureBox picture = new PictureBox
                    {
                        Size = new System.Drawing.Size(50, 50),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        ImageLocation = errorImageUrl,
                        Margin = new System.Windows.Forms.Padding(0),
                    };
                    picture.Click += (sender, e) => { profileImageClick(ownerId); };
                    row.Controls.Add(picture, 0, 0);
                    //pictures.Add(picture);
                }


                // Create a LinkLabel with the owner name
                LinkLabel owner = new LinkLabel
                {
                    Text = ownerName,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    AutoSize = true

                };
                owner.LinkClicked += (sender, e) => { Owner_LinkClicked(sender, e, ownerId, imageLocation); };
                owner.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                owner.Margin = new System.Windows.Forms.Padding(3);
                owner.Dock = DockStyle.Bottom;
                row.Controls.Add(owner, 1, 0);

                if (timestamp.Year > 1975)
                {
                    // Create a LinkLabel with the owner name
                    Label tstamp = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.Gray,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = timestamp.ToString("MM/dd/yyyy hh:mm:ss"),
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };
                    row2.Controls.Add(tstamp, 0, 0);




                    Label deleteme = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Black,
                        ForeColor = Color.White,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Text = "🗑",
                        Margin = new System.Windows.Forms.Padding(0),
                        Dock = DockStyle.Bottom
                    };
                    deleteme.Click += (sender, e) =>
                    {
                        deleteme_LinkClicked(transactionid);
                        deleteme.ForeColor = Color.Black;
                    };
                    row2.Controls.Add(deleteme, 1, 0);
                }

                if (addtoTop)
                {

                    layoutPanel.Controls.Add(row);
                    layoutPanel.Controls.SetChildIndex(row, 0);
                    layoutPanel.Controls.Add(row2);
                    layoutPanel.Controls.SetChildIndex(row, 1);
                }
                else
                {
                    layoutPanel.Controls.Add(row);
                    layoutPanel.Controls.Add(row2);

                }

            }

            TableLayoutPanel msg = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 1,
                Dock = DockStyle.Top,
                BackColor = Color.Black,
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new System.Windows.Forms.Padding(0),
                Padding = new System.Windows.Forms.Padding(0, 0, 0, 0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None

            };
            msg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, layoutPanel.Width - 50));


            if (addtoTop)
            {
                layoutPanel.Controls.Add(msg);
                layoutPanel.Controls.SetChildIndex(msg, 1);
            }
            else
            {
                layoutPanel.Controls.Add(msg);
            }

            if (messageText != "")
            {
                Label message = new Label
                {
                    AutoSize = true,
                    Text = messageText,
                    MinimumSize = new Size(200, 46),
                    Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Margin = new System.Windows.Forms.Padding(0),
                    Padding = new System.Windows.Forms.Padding(10, 20, 10, 20),
                    TextAlign = System.Drawing.ContentAlignment.TopLeft
                };

                msg.Controls.Add(message, 1, 0);
            }
            else
            {

                Label message = new Label
                {
                    AutoSize = true,
                    Text = messageText,
                    Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Margin = new System.Windows.Forms.Padding(0),
                    Padding = new System.Windows.Forms.Padding(0, 0, 0, 0),
                    TextAlign = System.Drawing.ContentAlignment.TopLeft
                };

                msg.Controls.Add(message, 1, 0);

            }


        }



    }







}
