using LevelDB;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SUP
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ObjectBrowser(null));



            var SUP = new Options { CreateIfMissing = true };
          
            using (var db = new DB(SUP, @"sup"))
            {
                
               string ipfsdaemon = db.Get("ipfs-daemon");

                if (ipfsdaemon != null)
                {

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
                }
                
            }



        }
    }
}
