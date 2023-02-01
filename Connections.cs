using System;
using System.Diagnostics;
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
    }
}
