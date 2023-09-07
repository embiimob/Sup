using LevelDB;
using NBitcoin.RPC;
using SUP.P2FK;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
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

            Process.Start(startInfo);
        }

        private void Connections_Load(object sender, EventArgs e)
        {




            if (File.Exists(@"WALKIE_TALKIE_ENABLED"))
            {
                checkBox7.Checked = true;
            }

            if (File.Exists(@"LIVE_FILTER_ENABLED"))
            {
                checkBox6.Checked = true;

            }




            string walletUsername = "good-user";
            string walletPassword = "better-password";
            string walletUrl = @"http://127.0.0.1:18332";
            NetworkCredential credentials = new NetworkCredential(walletUsername, walletPassword);
            RPCClient rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
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
                    walletUrl = @"http://127.0.0.1:8332";
                    rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                    string isActive = rpcClient.GetBalance().ToString();
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
                    walletUrl = @"http://127.0.0.1:9332";
                    rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                    string isActive = rpcClient.GetBalance().ToString();
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
                    walletUrl = @"http://127.0.0.1:12832";
                    rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                    string isActive = rpcClient.GetBalance().ToString();
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
                    walletUrl = @"http://127.0.0.1:22555";
                    rpcClient = new RPCClient(credentials, new Uri(walletUrl), NBitcoin.Network.Main);
                    string isActive = rpcClient.GetBalance().ToString();
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
                button1.Text = "IPFS pinning active";
                btnPinIPFS.Enabled = true;
                btnUnpinIPFS.Enabled = true;
                button1.ForeColor = Color.Yellow;
                button1.BackColor = Color.Blue;

            }
            else
            {
                button1.Text = "enable IPFS pinning";
                button1.ForeColor = Color.Black;
                button1.BackColor = Color.White;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "IPFS pinning active")
            {
                button1.Text = "enable IPFS pinning";
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

                var SUP = new Options { CreateIfMissing = true };

                using (var db = new DB(SUP, @"ipfs"))
                {

                    db.Delete("ipfs-daemon");
                }


            }
            else
            {
                button1.Text = "IPFS pinning active";
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

                var SUP = new Options { CreateIfMissing = true };

                using (var db = new DB(SUP, @"ipfs"))
                {

                    db.Put("ipfs-daemon", "true");

                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string[] subfolderNames = Directory.GetDirectories("ipfs");


                foreach (string subfolder in subfolderNames)
                {

                    try { Directory.Delete(subfolder); } catch { }

                }

                subfolderNames = Directory.GetDirectories("ipfs");

                foreach (string subfolder in subfolderNames)
                {
                    string hash = System.IO.Path.GetFileName(subfolder);

                    try
                    {
                        Directory.Delete(subfolder);
                    }
                    catch
                    {
                        // Handle the exception if necessary
                    }

                    // Call the Kubo local Pin command using the hash
                    Process process2 = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = @"ipfs\ipfs.exe",
                            Arguments = "pin add " + hash,
                            UseShellExecute = false
                        }
                    };

                    Task<int> processTask = Task.Run(() =>
                    {
                        process2.Start();
                        process2.WaitForExit();
                        return process2.ExitCode;
                    });

                    if (processTask.Wait(TimeSpan.FromSeconds(5)))
                    {

                    }
                    else
                    {
                        process2.Kill();
                    }
                }
            }
            catch { Directory.CreateDirectory("ipfs"); }

        }

        private void btnUnpinIPFS_Click(object sender, EventArgs e)
        {
            try{string[] subfolderNames = Directory.GetDirectories("ipfs");

            foreach (string subfolder in subfolderNames)
            {
                string hash = System.IO.Path.GetFileName(subfolder);

                // Call the Kubo local Pin command using the hash
                Process process2 = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"ipfs\ipfs.exe",
                        Arguments = "pin rm " + hash,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process2.Start();
                process2.WaitForExit();

            }
            }
            catch { Directory.CreateDirectory("ipfs"); }
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
        
             try{ Directory.Delete("root", true);} catch{ }

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

            Process.Start(startInfo);
        }

        private void btnDTC_Click(object sender, EventArgs e)
        {
            string bitcoinDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\datacoin";
            string bitcoindPath = AppDomain.CurrentDomain.BaseDirectory + "\\datacoin-qt.exe";
            System.IO.Directory.CreateDirectory("datacoin");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = bitcoindPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"-txindex=1 -addrindex=1 -datadir={bitcoinDirectory} -server -rpcuser=good-user -rpcpassword=better-password -rpcport=11777"
            };

            Process.Start(startInfo);
        }

        private void button2_Click_1(object sender, EventArgs e)
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

        private void btnPurgeFound_Click(object sender, EventArgs e)
        {
            try
            {
                string directoryPath = @"root";

                // Get a list of all the subdirectories in the directory
                string[] subdirectories = Directory.GetDirectories(directoryPath);

                // Loop through each subdirectory
                foreach (string subdirectory in subdirectories)
                {
                    // Get the name of the subdirectory
                    string subdirectoryName = System.IO.Path.GetFileName(subdirectory);


                    if (subdirectoryName == "found")
                    {
                        // Delete the subdirectory and all its contents
                        Directory.Delete(subdirectory, true);
                    }
                }
            }
            catch { }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked) {

                using (FileStream fs = File.Create(@"LIVE_FILTER_ENABLED"))
                {

                }

            } else {

                try { File.Delete(@"LIVE_FILTER_ENABLED"); }catch { }
            
            }

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked) {

                using (FileStream fs = File.Create(@"WALKIE_TALKIE_ENABLED"))
                {

                }

            } else {
                try { File.Delete(@"WALKIE_TALKIE_ENABLED"); } catch { }


            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
