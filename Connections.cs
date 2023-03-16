using LevelDB;
using System;
using System.Diagnostics;
using System.IO;
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
                button1.Text = "disable IPFS pinning";
                btnPinIPFS.Enabled = true;
                btnUnpinIPFS.Enabled = true;

            }
            else
            {
                button1.Text = "enable IPFS pinning";
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "disable IPFS pinning")
            {
                button1.Text = "enable IPFS pinning";

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
                button1.Text = "disable IPFS pinning";

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
            string[] subfolderNames = Directory.GetDirectories("ipfs");

            foreach (string subfolder in subfolderNames)
            {
                string hash = Path.GetFileName(subfolder);

                // Call the Kubo local Pin command using the hash
                Process process2 = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"ipfs\ipfs.exe",
                        Arguments = "pin add " + hash,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process2.Start();
                process2.WaitForExit();

            }
        }

        private void btnUnpinIPFS_Click(object sender, EventArgs e)
        {
            string[] subfolderNames = Directory.GetDirectories("ipfs");

            foreach (string subfolder in subfolderNames)
            {
                string hash = Path.GetFileName(subfolder);

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

        private void btnPurgeIPFS_Click(object sender, EventArgs e)
        {
            string[] subfolderNames = Directory.GetDirectories("ipfs");

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

        private void btnPurge_Click(object sender, EventArgs e)
        {

            string directoryPath = @"root";
            try
            {

                // Get a list of all the subdirectories in the directory
                string[] subdirectories = Directory.GetDirectories(directoryPath);

                // Loop through each subdirectory
                foreach (string subdirectory in subdirectories)
                {
                    // Get the name of the subdirectory
                    string subdirectoryName = Path.GetFileName(subdirectory);

                    // Check if the subdirectory name is "mute" or "block"
                    if (subdirectoryName != "mute" && subdirectoryName != "block")
                    {
                        // Delete the subdirectory and all its contents
                        Directory.Delete(subdirectory, true);
                    }
                }

                // Get a list of all the files in the directory
                string[] files = Directory.GetFiles(directoryPath);

                // Loop through each file
                foreach (string file in files)
                {
                    // Delete the file
                    File.Delete(file);
                }
            }
            catch { }
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
            string[] files = Directory.GetFiles("ipfs");

            foreach (string file in files)
            {
                // Delete the file
                if (!file.Contains("ipfs.exe"))
                {
                    File.Delete(file);
                }
            }

            foreach (string directory in Directory.GetDirectories("ipfs"))
            {
                try { Directory.Delete(directory, false); } catch { }
               
            }



        }

        private void btnPurgeBlock_Click(object sender, EventArgs e)
        {
            string directoryPath = @"root";

            // Get a list of all the subdirectories in the directory
            string[] subdirectories = Directory.GetDirectories(directoryPath);

            // Loop through each subdirectory
            foreach (string subdirectory in subdirectories)
            {
                // Get the name of the subdirectory
                string subdirectoryName = Path.GetFileName(subdirectory);

                // Check if the subdirectory name is "mute" or "block"
                if (subdirectoryName == "oblock" || subdirectoryName == "tblock")
                {
                    // Delete the subdirectory and all its contents
                    Directory.Delete(subdirectory, true);
                }
            }

            
        }

        private void brnPurgeMute_Click(object sender, EventArgs e)
        {
            string directoryPath = @"root";

            // Get a list of all the subdirectories in the directory
            string[] subdirectories = Directory.GetDirectories(directoryPath);

            // Loop through each subdirectory
            foreach (string subdirectory in subdirectories)
            {
                // Get the name of the subdirectory
                string subdirectoryName = Path.GetFileName(subdirectory);

                // Check if the subdirectory name is "mute" or "block"
                if (subdirectoryName == "mute")
                {
                    // Delete the subdirectory and all its contents
                    Directory.Delete(subdirectory, true);
                }
            }
        }

        private void btnPurgeFound_Click(object sender, EventArgs e)
        {
            string directoryPath = @"root";

            // Get a list of all the subdirectories in the directory
            string[] subdirectories = Directory.GetDirectories(directoryPath);

            // Loop through each subdirectory
            foreach (string subdirectory in subdirectories)
            {
                // Get the name of the subdirectory
                string subdirectoryName = Path.GetFileName(subdirectory);

                // Check if the subdirectory name is "mute" or "block"
                if (subdirectoryName == "found")
                {
                    // Delete the subdirectory and all its contents
                    Directory.Delete(subdirectory, true);
                }
            }
        }
    }
}
