using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace P2FK.IO
{
    public class Wrapper
    {
        //default mainnet connection info
        public string ProdCLIPath = @"C:\SUP\SUP.exe"; // Replace with the actual path to SUP.EXE
        public string ProdVersionByte = @"0"; // Replace with the actual version byte
        public string ProdRPCURL = @"http://127.0.0.1:8332";
        public string ProdRPCUser = "good-user";
        public string ProdRPCPassword = "better-password";

        //default testnet connection info
        public string TestCLIPath = @"C:\SUP\SUP.exe"; // Replace with the actual path to SUP.EXE
        public string TestVersionByte = @"111"; // Replace with the actual version byte
        public string TestRPCURL = @"http://127.0.0.1:18332";
        public string TestRPCUser = "good-user";
        public string TestRPCPassword = "better-password";

        //default litecoin mainnet connection info
        public string LTCCLIPath = @"C:\SUP\SUP.exe";
        public string LTCVersionByte = @"48";
        public string LTCRPCURL = @"http://127.0.0.1:9332";
        public string LTCRPCUser = "good-user";
        public string LTCRPCPassword = "better-password";

        //default dogecoin mainnet connection info
        public string DOGCLIPath = @"C:\SUP\SUP.exe";
        public string DOGVersionByte = @"30";
        public string DOGRPCURL = @"http://127.0.0.1:22555";
        public string DOGRPCUser = "good-user";
        public string DOGRPCPassword = "better-password";

        //root folder where synced blockchain data (ROOT.json, OBJ.json, etc.) is stored
        public string RootPath = @"C:\p2fk.io\root";

        //default mazacoin mainnet connection info
        public string MZCCLIPath = @"C:\SUP\SUP.exe";
        public string MZCVersionByte = @"50";
        public string MZCRPCURL = @"http://127.0.0.1:12832";
        public string MZCRPCUser = "good-user";
        public string MZCRPCPassword = "better-password";

        public const int MaxTimeoutSeconds = 420;

        // Global concurrency cap: at most 8 CLI processes running simultaneously.
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(8, 8);
        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(MaxTimeoutSeconds);

        // In-flight request coalescing: concurrent API calls for the same address + command
        // share one SUP.exe process rather than each spawning their own.
        //
        // Why this matters: every CLI invocation reads the current disk cache, increments
        // forward from the stored cursor, and writes the updated cache back.  Multiple
        // concurrent processes for the same address race over the same cache file which --
        // even with the cross-process OS-mutex guards added in SUP.exe -- causes extra
        // work and can still produce cursor regressions.  By coalescing here (in the API)
        // only one process per address+command is ever running at once, so the CLI mutex
        // is almost never contested and cache file writes are sequential by construction.
        //
        // The coalescing key is (executablePath, command verb, address, versionbyte) so
        // concurrent requests with different --skip / --qty values for the same address
        // still share one build.  Per-caller HTTP cancellation only cancels the individual
        // wait; the shared CLI process continues so other callers and the disk cache still
        // benefit even if the originating HTTP request is abandoned.
        private static readonly ConcurrentDictionary<string, Task<string>> _inFlight =
            new(StringComparer.Ordinal);

        // Compiled patterns used by BuildCoalescingKey.
        private static readonly Regex _addrPattern = new(@"--address\s+(\S+)", RegexOptions.Compiled);
        private static readonly Regex _cmdPattern  = new(@"--(get\w+)",         RegexOptions.Compiled);
        private static readonly Regex _vbPattern   = new(@"--versionbyte\s+(\S+)", RegexOptions.Compiled);

        public async Task<string> RunCommandAsync(string executablePath, string arguments, CancellationToken cancellationToken = default)
        {
            string key = BuildCoalescingKey(executablePath, arguments);

            // GetOrAdd ensures that if this key is already in-flight we reuse the existing
            // Task<string> and do not spawn a second CLI process.
            var shared = _inFlight.GetOrAdd(key, _ => LaunchShared(key, executablePath, arguments));

            try
            {
                // WaitAsync lets the individual HTTP caller bail out (disconnected client,
                // request timeout, etc.) without killing the shared CLI process.
                return await shared.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return "[\"error: request cancelled\"]";
            }
        }

        // Derives a stable coalescing key from the CLI verb and address so that callers
        // with different --skip / --qty still coalesce on the same address+command pair.
        // Falls back to the full argument string for commands that have no --address flag.
        private static string BuildCoalescingKey(string executablePath, string arguments)
        {
            var addrMatch = _addrPattern.Match(arguments);
            if (!addrMatch.Success)
                return executablePath + "\0" + arguments;

            string address  = addrMatch.Groups[1].Value;
            string command  = _cmdPattern.Match(arguments) is { Success: true } m ? m.Groups[1].Value : "";
            string vb       = _vbPattern.Match(arguments)  is { Success: true } v ? v.Groups[1].Value : "";
            return $"{executablePath}\0{command}\0{address}\0{vb}";
        }

        // Creates the shared Task and schedules its removal from _inFlight on completion.
        private Task<string> LaunchShared(string key, string executablePath, string arguments)
        {
            var task = ExecuteCliAsync(executablePath, arguments);
            // Remove once done so the next caller after this one gets a fresh run.
            _ = task.ContinueWith(
                _ => _inFlight.TryRemove(new KeyValuePair<string, Task<string>>(key, task)),
                TaskScheduler.Default);
            return task;
        }

        // Executes the CLI process with an internal-only timeout.  Not bound to any
        // individual caller's CancellationToken so HTTP disconnects don't abort the build.
        private async Task<string> ExecuteCliAsync(string executablePath, string arguments)
        {
            using var timeoutCts = new CancellationTokenSource(_timeout);
            bool acquired = false;
            try
            {
                await _semaphore.WaitAsync(timeoutCts.Token);
                acquired = true;
            }
            catch (OperationCanceledException)
            {
                return "[\"error: request timed out\"]";
            }

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using var process = new Process { StartInfo = processStartInfo };
                process.Start();

                var outputTask = process.StandardOutput.ReadToEndAsync(timeoutCts.Token);
                var errorTask  = process.StandardError.ReadToEndAsync(timeoutCts.Token);

                try
                {
                    await Task.WhenAll(outputTask, errorTask);
                    await process.WaitForExitAsync(timeoutCts.Token);
                }
                catch (OperationCanceledException)
                {
                    try { process.Kill(entireProcessTree: true); } catch { /* process may have already exited */ }
                    return "[\"error: request timed out\"]";
                }

                // Task.WhenAll above succeeded without throwing, so both tasks are guaranteed completed successfully
                string output = outputTask.Result;
                string error  = errorTask.Result;

                if (!string.IsNullOrEmpty(error))
                    return $"Error: {error}";

                return output;
            }
            finally
            {
                if (acquired) _semaphore.Release();
            }
        }
    }
}
