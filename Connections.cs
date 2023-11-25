using SUP.P2FK;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.Hosting;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUP
{
    public partial class Connections : Form
    {
        public Connections()
        {
            InitializeComponent();
        }

        private void btnMainConnection_Click(object sender, EventArgs e)
        {
            string bitcoinDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\bitcoin";
            string bitcoindPath = AppDomain.CurrentDomain.BaseDirectory + "\\bitcoin-qt.exe";
            System.IO.Directory.CreateDirectory("bitcoin");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = bitcoindPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"-testnet -txindex=1 -addrindex=1 -datadir={bitcoinDirectory} -server -rpcuser=good-user -rpcpassword=better-password -rpcport=18332"

            };

            if (chkBTCT.Checked) { startInfo.Arguments = startInfo.Arguments + " -reindex"; }

            Process.Start(startInfo);
        }

        private void btnBTC_Click(object sender, EventArgs e)
        {
            string bitcoinDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\bitcoin";
            string bitcoindPath = AppDomain.CurrentDomain.BaseDirectory + "\\bitcoin-qt.exe";
            System.IO.Directory.CreateDirectory("bitcoin");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = bitcoindPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"-txindex=1 -addrindex=1 -datadir={bitcoinDirectory} -server -rpcuser=good-user -rpcpassword=better-password -rpcport=8332"
            };
            if (chkBTC.Checked) { startInfo.Arguments = startInfo.Arguments + " -reindex"; }

            Process.Start(startInfo);
        }

        private void btnMZC_Click(object sender, EventArgs e)
        {
            string bitcoinDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\mazacoin";
            string bitcoindPath = AppDomain.CurrentDomain.BaseDirectory + "\\maza-qt.exe";
            System.IO.Directory.CreateDirectory("mazacoin");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = bitcoindPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"-txindex=1 -addrindex=1 -datadir={bitcoinDirectory} -server -rpcuser=good-user -rpcpassword=better-password -rpcport=12832"
            };

            if (chkMZC.Checked) { startInfo.Arguments = startInfo.Arguments + " -reindex"; }

            Process.Start(startInfo);
        }

        private void btnLTC_Click(object sender, EventArgs e)
        {
            string bitcoinDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\litecoin";
            string bitcoindPath = AppDomain.CurrentDomain.BaseDirectory + "\\litecoin-qt.exe";
            System.IO.Directory.CreateDirectory("litecoin");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = bitcoindPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"-txindex=1 -addrindex=1 -datadir={bitcoinDirectory} -server -rpcuser=good-user -rpcpassword=better-password -rpcport=9332"
            };

            if (chkLTC.Checked) { startInfo.Arguments = startInfo.Arguments + " -reindex"; }

            Process.Start(startInfo);
        }

        private void btnDOGE_Click(object sender, EventArgs e)
        {
            string bitcoinDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\dogecoin";
            string bitcoindPath = AppDomain.CurrentDomain.BaseDirectory + "\\dogecoin-qt.exe";
            System.IO.Directory.CreateDirectory("dogecoin");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = bitcoindPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"-txindex=1 -addrindex=1 -datadir={bitcoinDirectory} -server -rpcuser=good-user -rpcpassword=better-password -rpcport=22555"
            };

            if (chkDOG.Checked) { startInfo.Arguments = startInfo.Arguments + " -reindex"; }

            Process.Start(startInfo);
        }

        private void Connections_Load(object sender, EventArgs e)
        {



            if (File.Exists(@"IPFS_PINNING_ENABLED"))
            {
                chkLiveFeedPinning.Checked = true;

            }

            if (File.Exists(@"WALKIE_TALKIE_ENABLED"))
            {
                chkWalkieTalkie.Checked = true;
            }

            if (File.Exists(@"LIVE_FILTER_ENABLED"))
            {
                chkFilterLivePostings.Checked = true;

            }


            string walletUsername = "good-user";
            string walletPassword = "better-password";
            string walletUrl = @"http://127.0.0.1:18332";
            NetworkCredential credentials = new NetworkCredential(walletUsername, walletPassword);
            NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
            Task.Run(() =>
            {
                try
                {
                    string isActive = rpcClient.GetBalance().ToString();
                    if (decimal.TryParse(isActive, out decimal _))
                    {
                        this.Invoke((MethodInvoker)delegate
                    {
                        btnMainConnection.BackColor = Color.Blue;
                        btnMainConnection.ForeColor = Color.Yellow;
                        btnMainConnection.Text = "active";
                    });
                    }
                }
                catch { }
            });


            Task.Run(() =>
            {
                try
                {
                    string walletUrl2 = @"http://127.0.0.1:8332";
                    NBitcoin.RPC.RPCClient rpcClient2 = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl2), NBitcoin.Network.Main);
                    string isActive = rpcClient2.GetBalance().ToString();
                    if (decimal.TryParse(isActive, out decimal _))
                    {
                        this.Invoke((MethodInvoker)delegate
                    {
                        btnBTC.BackColor = Color.Blue;
                        btnBTC.ForeColor = Color.Yellow;
                        btnBTC.Text = "active";
                    });
                    }
                }
                catch { }
            });



            Task.Run(() =>
            {
                try
                {
                    string walletUrl3 = @"http://127.0.0.1:9332";
                    NBitcoin.RPC.RPCClient rpcClient3 = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl3), NBitcoin.Network.Main);
                    string isActive = rpcClient3.GetBalance().ToString();
                    if (decimal.TryParse(isActive, out decimal _))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            btnLTC.BackColor = Color.Blue;
                            btnLTC.ForeColor = Color.Yellow;
                            btnLTC.Text = "active";
                        });

                    }
                }
                catch { }
            });


            Task.Run(() =>
            {
                try
                {
                    string walletUrl4 = @"http://127.0.0.1:12832";
                    NBitcoin.RPC.RPCClient rpcClient4 = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl4), NBitcoin.Network.Main);
                    string isActive = rpcClient4.GetBalance().ToString();
                    if (decimal.TryParse(isActive, out decimal _))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            btnMZC.BackColor = Color.Blue;
                            btnMZC.ForeColor = Color.Yellow;
                            btnMZC.Text = "active";
                        });

                    }
                }
                catch { }
            });

            Task.Run(() =>
            {
                try
                {
                    string walletUrl5 = @"http://127.0.0.1:22555";
                    NBitcoin.RPC.RPCClient rpcClient5 = new NBitcoin.RPC.RPCClient(credentials, new Uri(walletUrl5), NBitcoin.Network.Main);
                    string isActive = rpcClient5.GetBalance().ToString();
                    if (decimal.TryParse(isActive, out decimal _))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            btnDOG.BackColor = Color.Blue;
                            btnDOG.ForeColor = Color.Yellow;
                            btnDOG.Text = "active";
                        });

                    }
                }
                catch { }
            });


            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"ipfs\ipfs.exe",
                    Arguments = "swarm peers",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            if (output.Length > 0)
            {
                button1.Text = "IPFS dameon active";
                btnPinIPFS.Enabled = true;
                btnUnpinIPFS.Enabled = true;
                button1.ForeColor = Color.Yellow;
                button1.BackColor = Color.Blue;

            }
            else
            {
                button1.Text = "enable IPFS dameon";
                button1.ForeColor = Color.Black;
                button1.BackColor = Color.White;
            }

        }

        private void btnIPFS_Click(object sender, EventArgs e)
        {
            if (button1.Text == "IPFS dameon active")
            {
                button1.Text = "enable IPFS dameon";
                button1.BackColor= Color.White;
                button1.ForeColor = Color.Black;

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"ipfs\ipfs.exe",
                        Arguments = "shutdown",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
          
                btnPinIPFS.Enabled = false;
                btnUnpinIPFS.Enabled = false;              


            }
            else
            {
                button1.Text = "IPFS dameon active";
                button1.ForeColor = Color.Yellow;
                button1.BackColor = Color.Blue;

                var init = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"ipfs\ipfs.exe",
                        Arguments = "init",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                init.Start();
                init.WaitForExit();

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"ipfs\ipfs.exe",
                        Arguments = "daemon",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                btnPinIPFS.Enabled = true;
                btnUnpinIPFS.Enabled = true;
                                

            }
        }

        //JAZZED UP BY GPT3
        private async void btnIPFSPin_Click(object sender, EventArgs e)
        {
            try
            {
                // Change the text and background color of the button to indicate "pinning" on the UI thread
                this.Invoke((MethodInvoker)(() =>
                {
                    btnPinIPFS.Text = "pinning";
                    btnPinIPFS.ForeColor = Color.Yellow;
                    btnPinIPFS.BackColor = Color.Blue;
                }));

                string[] subfolderNames = Directory.GetDirectories("ipfs");

                foreach (string subfolder in subfolderNames)
                {
                    string hash = System.IO.Path.GetFileName(subfolder);

                    using (var process2 = new Process())
                    {
                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                        process2.StartInfo.Arguments = "pin add " + hash;
                        process2.StartInfo.UseShellExecute = false;

                        process2.Start();

                        // Wait for the process to complete, but not longer than 5 seconds
                        if (await Task.Run(() => process2.WaitForExit(5000)))
                        {
                            // Process completed within 5 seconds
                            int exitCode = process2.ExitCode;
                            // Handle the exit code or other logic here if needed
                        }
                        else
                        {
                            // Process didn't complete within 5 seconds, you can handle this case
                            // Maybe log or take other actions
                        }
                    }
                }

                // Revert the text and background color of the button to the original state on the UI thread
                this.Invoke((MethodInvoker)(() =>
                {
                    btnPinIPFS.Text = "pin cache";
                    btnPinIPFS.ForeColor = Color.Black;
                    btnPinIPFS.BackColor = Color.White;
                }));
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                // Revert the text and background color of the button in the catch block if necessary
                try
                {
                    this.Invoke((MethodInvoker)(() =>
                 {
                     btnPinIPFS.Text = "pin cache";
                     btnPinIPFS.ForeColor = Color.Black;
                     btnPinIPFS.BackColor = Color.White;
                 }));
                }
                catch { }
            }
        }

        //GPT3 IS COOL
        private async void btnUnpinIPFS_Click(object sender, EventArgs e)
        {
            try
            {
                // Change the text and background color of the button to indicate "pinning" on the UI thread
                this.Invoke((MethodInvoker)(() => {
                    btnUnpinIPFS.Text = "unpinning";
                    btnUnpinIPFS.ForeColor = Color.Yellow;
                    btnUnpinIPFS.BackColor = Color.Blue;
                }));

                string[] subfolderNames = Directory.GetDirectories("ipfs");

                foreach (string subfolder in subfolderNames)
                {
                    string hash = System.IO.Path.GetFileName(subfolder);

                    using (var process2 = new Process())
                    {
                        process2.StartInfo.FileName = @"ipfs\ipfs.exe";
                        process2.StartInfo.Arguments = "pin rm " + hash;
                        process2.StartInfo.UseShellExecute = false;

                        process2.Start();

                        // Wait for the process to complete, but not longer than 5 seconds
                        if (await Task.Run(() => process2.WaitForExit(5000)))
                        {
                            // Process completed within 5 seconds
                            int exitCode = process2.ExitCode;
                            // Handle the exit code or other logic here if needed
                        }
                        else
                        {
                            // Process didn't complete within 5 seconds, you can handle this case
                            // Maybe log or take other actions
                        }
                    }
                }

                // Revert the text and background color of the button to the original state on the UI thread
                this.Invoke((MethodInvoker)(() => {
                    btnUnpinIPFS.Text = "unpin cache";
                    btnUnpinIPFS.ForeColor = Color.Black;
                    btnUnpinIPFS.BackColor = Color.White;
                }));
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                // Revert the text and background color of the button in the catch block if necessary
                try
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        btnUnpinIPFS.Text = "unpin cache";
                        btnUnpinIPFS.ForeColor = Color.Black;
                        btnUnpinIPFS.BackColor = Color.White;
                    }));
                }
                catch { }
            }
        }

        private void btnPurgeIPFS_Click(object sender, EventArgs e)
        {
          try{  string[] subfolderNames = Directory.GetDirectories("ipfs");

            foreach (string subfolder in subfolderNames)
            {
                Directory.Delete(subfolder, true);
            }

            string[] files = Directory.GetFiles("ipfs");

            // Loop through each file
            foreach (string file in files)
            {
                // Delete the file
                if (!file.Contains("ipfs.exe"))
                {
                    File.Delete(file);
                }
            }
            }
            catch { Directory.CreateDirectory("ipfs"); }
        }

        private void btnPurge_Click(object sender, EventArgs e)
        {

            try { Directory.Delete("root", true); } catch { }
            try { Directory.CreateDirectory("root\\sig");} catch { }


        }

        private void btnPurgeIPFSBuilding_Click(object sender, EventArgs e)
        {
            try
            {
                string[] subfolderNames = Directory.GetDirectories("ipfs");

                foreach (string subfolder in subfolderNames)
                {
                    if (subfolder.EndsWith("-build"))
                    {
                        Directory.Delete(subfolder, true);
                    }
                    
                }
               
            }
            catch { }



        }

        private void btnPurgeBlock_Click(object sender, EventArgs e)
        {
            try
            {
                string directoryPath = @"root";



                // Get a list of all the subdirectories in the directory
                string[] subdirectories = Directory.GetDirectories(directoryPath);

                // Loop through each subdirectory
                foreach (string subdirectory in subdirectories)
                {
                    try
                    {
                        // Get the files in the subdirectory
                        string[] files = Directory.GetFiles(subdirectory);

                        // Loop through each file in the subdirectory
                        foreach (string file in files)
                        {
                            // Get the name of the file
                            string fileName = System.IO.Path.GetFileName(file);

                            // Check if the file name is "BLOCK" (case-insensitive check)
                            if (fileName.Equals("BLOCK", StringComparison.OrdinalIgnoreCase))
                            {
                                string[] pathParts = file.Split('\\');



                                Root[] root = Root.GetRootsByAddress(pathParts[pathParts.Length - 2], "good-user", "better-password", @"http://127.0.0.1:18332");

                                foreach (Root rootItem in root)
                                {

                                    try
                                    {
                                        Directory.Delete(@"root\" + rootItem.TransactionId, true);
                                    }
                                    catch { }

                                    foreach (string key in rootItem.Keyword.Keys)
                                    {
                                        try { Directory.Delete(@"root\" + key, true); } catch { }
                                    }

                                }


                                // Delete the Directory
                                try { Directory.Delete(subdirectory, true); } catch { }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }

        }

        private void brnPurgeMute_Click(object sender, EventArgs e)
        {
            string directoryPath = @"root";

            try
            {
                // Get a list of all the subdirectories in the directory
                string[] subdirectories = Directory.GetDirectories(directoryPath);

                // Loop through each subdirectory
                foreach (string subdirectory in subdirectories)
                {
                    // Get the files in the subdirectory
                    string[] files = Directory.GetFiles(subdirectory);

                    // Loop through each file in the subdirectory
                    foreach (string file in files)
                    {
                        // Get the name of the file
                        string fileName = System.IO.Path.GetFileName(file);

                        if (fileName.Equals("MUTE", StringComparison.OrdinalIgnoreCase))
                        {
                            // Delete the file
                            File.Delete(file);
                        }
                    }
                }
            }
            catch { }
        }

      

        private void chkFilterLivePostings_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFilterLivePostings.Checked) {

                using (FileStream fs = File.Create(@"LIVE_FILTER_ENABLED"))
                {

                }

            } else {

                try { File.Delete(@"LIVE_FILTER_ENABLED"); }catch { }
            
            }

        }

        private void chkWalkieTalkie_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWalkieTalkie.Checked) {

                using (FileStream fs = File.Create(@"WALKIE_TALKIE_ENABLED"))
                {

                }

            } else {
                try { File.Delete(@"WALKIE_TALKIE_ENABLED"); } catch { }


            }
        }

        private void chkLiveFeedPinning_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLiveFeedPinning.Checked)
            {

                using (FileStream fs = File.Create(@"IPFS_PINNING_ENABLED"))
                {

                }

            }
            else
            {
                try { File.Delete(@"IPFS_PINNING_ENABLED"); } catch { }


            }
        }
    }
}
