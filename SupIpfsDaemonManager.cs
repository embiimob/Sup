using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SUP
{
    internal static class SupIpfsDaemonManager
    {
        private const int SupApiPort = 5012;
        private const int SupGatewayPort = 8092;
        private const int SupSwarmPort = 4012;
        private static readonly object InitLock = new object();
        private static bool repoConfigured;

        public static string RepoDirectory
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "sup-ipfs-repo");
            }
        }

        private static string IpfsExecutable
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ipfs", "ipfs.exe");
            }
        }

        public static void EnsureEnvironment()
        {
            Environment.SetEnvironmentVariable("IPFS_PATH", RepoDirectory);
        }

        public static void EnsureRepositoryConfigured()
        {
            lock (InitLock)
            {
                EnsureEnvironment();
                Directory.CreateDirectory(RepoDirectory);

                if (!File.Exists(Path.Combine(RepoDirectory, "config")))
                {
                    RunIpfsAndWait($"init --repo-dir \"{RepoDirectory}\"", 30000);
                }

                if (!repoConfigured)
                {
                    RunIpfsAndWait($"config --repo-dir \"{RepoDirectory}\" Addresses.API /ip4/127.0.0.1/tcp/{SupApiPort}", 10000);
                    RunIpfsAndWait($"config --repo-dir \"{RepoDirectory}\" Addresses.Gateway /ip4/127.0.0.1/tcp/{SupGatewayPort}", 10000);
                    RunIpfsAndWait($"config --repo-dir \"{RepoDirectory}\" Swarm.DisableNatPortMap true", 10000);
                    RunIpfsAndWait(
                        $"config --json --repo-dir \"{RepoDirectory}\" Addresses.Swarm \"[\\\"/ip4/0.0.0.0/tcp/{SupSwarmPort}\\\",\\\"/ip6/::/tcp/{SupSwarmPort}\\\"]\"",
                        10000);
                    repoConfigured = true;
                }
            }
        }

        public static Process CreateDaemonProcess()
        {
            EnsureRepositoryConfigured();
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = IpfsExecutable,
                    Arguments = $"daemon --repo-dir \"{RepoDirectory}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
        }

        private static void RunIpfsAndWait(string arguments, int timeoutMs)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = IpfsExecutable,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process.Start();
                process.WaitForExit(timeoutMs);
            }
        }
    }
}
