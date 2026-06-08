using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SUP
{
    /// <summary>
    /// Helper class for IPFS operations with timeout support and error handling.
    /// Provides non-blocking async methods for get and pin operations.
    /// </summary>
    public static class IpfsHelper
    {
        private static readonly string IpfsExecutable = @"ipfs\ipfs.exe";
        
        /// <summary>
        /// Asynchronously retrieves a file from IPFS with timeout support.
        /// </summary>
        /// <param name="hash">IPFS hash to retrieve</param>
        /// <param name="outputPath">Output directory path</param>
        /// <param name="timeoutMs">Timeout in milliseconds (default: 60000ms)</param>
        /// <returns>True if successful, false otherwise</returns>
        public static async Task<bool> GetAsync(string hash, string outputPath, int timeoutMs = 60000)
        {
            if (string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(outputPath))
            {
                LogError("GetAsync", "Invalid hash or output path");
                return false;
            }

            try
            {
                using (var cts = new CancellationTokenSource(timeoutMs))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = IpfsExecutable,
                            Arguments = $"get {hash} -o \"{outputPath}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                    };

                    process.Start();
                    
                    // Start reading output to prevent buffer overflow
                    var outputTask = process.StandardOutput.ReadToEndAsync();
                    var errorTask = process.StandardError.ReadToEndAsync();
                    
                    // Wait for exit with cancellation token
                    var tcs = new TaskCompletionSource<bool>();
                    cts.Token.Register(() =>
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                                LogWarning("GetAsync", $"IPFS get timeout for {hash}, killed process after {timeoutMs}ms");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError("GetAsync", $"Error killing process: {ex.Message}");
                        }
                        tcs.TrySetResult(false);
                    });

                    process.EnableRaisingEvents = true;
                    process.Exited += (sender, args) =>
                    {
                        tcs.TrySetResult(process.ExitCode == 0);
                    };

                    // Wait for process to exit or timeout
                    var result = await tcs.Task.ConfigureAwait(false);
                    
                    // Always read output/error for diagnostics
                    var output = await outputTask.ConfigureAwait(false);
                    var error = await errorTask.ConfigureAwait(false);
                    
                    if (!result)
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            LogError("GetAsync", $"IPFS get failed for {hash}: {error}");
                        }
                        if (!string.IsNullOrEmpty(output))
                        {
                            LogInfo("GetAsync", $"IPFS get output for {hash}: {output}");
                        }
                    }
                    else
                    {
                        LogInfo("GetAsync", $"Successfully retrieved {hash}");
                        if (!string.IsNullOrEmpty(output))
                        {
                            LogInfo("GetAsync", $"IPFS output: {output}");
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                LogError("GetAsync", $"Exception during IPFS get for {hash}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Asynchronously pins a file in IPFS (fire and forget).
        /// </summary>
        /// <param name="hash">IPFS hash to pin</param>
        public static async Task PinAsync(string hash)
        {
            if (string.IsNullOrEmpty(hash))
            {
                return;
            }

            // Check if pinning is enabled
            if (!File.Exists("IPFS_PINNING_ENABLED"))
            {
                return;
            }

            try
            {
                using (var cts = new CancellationTokenSource(30000))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = IpfsExecutable,
                            Arguments = $"pin add {hash}",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardError = true
                        }
                    };

                    process.Start();
                    
                    // Create a task completion source for async waiting
                    var tcs = new TaskCompletionSource<bool>();
                    
                    // Register cancellation
                    cts.Token.Register(() =>
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                                LogWarning("PinAsync", $"IPFS pin timeout for {hash}");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError("PinAsync", $"Error killing pin process: {ex.Message}");
                        }
                        tcs.TrySetResult(false);
                    });

                    // Handle process exit
                    process.EnableRaisingEvents = true;
                    process.Exited += (sender, args) =>
                    {
                        tcs.TrySetResult(process.ExitCode == 0);
                    };

                    // Wait for completion or timeout
                    await tcs.Task;
                    
                    LogInfo("PinAsync", $"Pin completed for {hash}");
                }
            }
            catch (Exception ex)
            {
                LogError("PinAsync", $"Exception during IPFS pin for {hash}: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a locally downloaded IPFS file/folder to the local daemon cache and pins by hash if enabled.
        /// </summary>
        /// <param name="hash">IPFS hash to pin when pinning is enabled</param>
        /// <param name="targetFilePath">Expected local file path downloaded by helper</param>
        public static void AddToLocalCacheAndPinIfEnabled(string hash, string targetFilePath)
        {
            if (string.IsNullOrEmpty(hash))
            {
                LogWarning("AddToLocalCacheAndPinIfEnabled", "Hash is empty");
                return;
            }

            try
            {
                string localPath = null;
                if (!string.IsNullOrEmpty(targetFilePath) && File.Exists(targetFilePath))
                {
                    localPath = targetFilePath;
                }
                else
                {
                    string cidDirectory = Path.Combine("ipfs", hash);
                    if (Directory.Exists(cidDirectory))
                    {
                        localPath = cidDirectory;
                    }
                }

                if (string.IsNullOrEmpty(localPath))
                {
                    LogWarning("AddToLocalCacheAndPinIfEnabled", $"No local file/folder found for {hash}");
                    return;
                }

                bool isDirectory = Directory.Exists(localPath);
                string addArgs = isDirectory
                    ? $"add -r \"{localPath}\""
                    : $"add \"{localPath}\"";

                using (var addProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = IpfsExecutable,
                        Arguments = addArgs,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                })
                {
                    addProcess.Start();

                    var outputTask = addProcess.StandardOutput.ReadToEndAsync();
                    var errorTask = addProcess.StandardError.ReadToEndAsync();

                    if (!addProcess.WaitForExit(120000))
                    {
                        try { addProcess.Kill(); } catch { }
                        LogWarning("AddToLocalCacheAndPinIfEnabled", $"Timeout while adding {localPath} to IPFS cache");
                        return;
                    }

                    string output = outputTask.GetAwaiter().GetResult();
                    string error = errorTask.GetAwaiter().GetResult();

                    if (addProcess.ExitCode != 0)
                    {
                        LogError("AddToLocalCacheAndPinIfEnabled", $"ipfs add failed for {localPath}: {error}");
                        return;
                    }

                    LogInfo("AddToLocalCacheAndPinIfEnabled", $"ipfs add succeeded for {localPath}");
                    if (!string.IsNullOrEmpty(output))
                    {
                        LogInfo("AddToLocalCacheAndPinIfEnabled", $"ipfs add output: {output}");
                    }
                }

                if (File.Exists("IPFS_PINNING_ENABLED"))
                {
                    _ = PinAsync(hash);
                }
            }
            catch (Exception ex)
            {
                LogError("AddToLocalCacheAndPinIfEnabled", $"Exception for {hash}: {ex.Message}");
            }
        }

        /// <summary>
        /// Attempts to download an IPFS file from public HTTP gateways (ipfs.io then p2fk.io).
        /// Downloads the raw content directly to the target file path.
        /// If a gateway succeeds the file is saved locally; the caller should pin if desired.
        /// </summary>
        /// <param name="cid">IPFS content identifier (CID/hash)</param>
        /// <param name="targetFilePath">Full path where the downloaded file should be saved</param>
        /// <param name="timeoutMs">Per-gateway request timeout in milliseconds (default: 30000ms)</param>
        /// <returns>True if any gateway succeeded and a non-empty file was saved, false otherwise</returns>
        public static bool TryGetFromPublicGateways(string cid, string targetFilePath, int timeoutMs = 30000)
        {
            if (string.IsNullOrEmpty(cid) || string.IsNullOrEmpty(targetFilePath))
            {
                LogError("TryGetFromPublicGateways", "Invalid CID or target path");
                return false;
            }

            string[] gateways = new[]
            {
                $"https://ipfs.io/ipfs/{cid}",
                $"https://p2fk.io/ipfs/{cid}"
            };

            foreach (var url in gateways)
            {
                try
                {
                    // Ensure target directory exists
                    string dir = Path.GetDirectoryName(targetFilePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    // Remove any existing (possibly empty/corrupt) file before writing
                    if (File.Exists(targetFilePath))
                        File.Delete(targetFilePath);

                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Timeout = timeoutMs;
                    request.UserAgent = "Mozilla/5.0";
                    request.AllowAutoRedirect = true;

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var responseStream = response.GetResponseStream())
                            using (var fileStream = File.Create(targetFilePath))
                            {
                                responseStream.CopyTo(fileStream);
                            }

                            if (File.Exists(targetFilePath) && new FileInfo(targetFilePath).Length > 0)
                            {
                                LogInfo("TryGetFromPublicGateways", $"Downloaded {cid} from {url}");
                                return true;
                            }

                            // Empty response – clean up and try next gateway
                            try { File.Delete(targetFilePath); } catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogWarning("TryGetFromPublicGateways", $"Gateway {url} failed for {cid}: {ex.Message}");
                    try { if (File.Exists(targetFilePath)) File.Delete(targetFilePath); } catch { }
                }
            }

            LogInfo("TryGetFromPublicGateways", $"All public gateways failed for {cid}");
            return false;
        }

        /// <summary>
        /// Processes the downloaded IPFS file and moves it to the correct location.
        /// </summary>
        /// <param name="hash">IPFS hash</param>
        /// <param name="baseDir">Base directory (e.g., "root" or "ipfs")</param>
        /// <param name="targetFileName">Target file name (e.g., "SEC" or "artifact")</param>
        /// <returns>True if file was processed successfully</returns>
        public static bool ProcessDownloadedFile(string hash, string baseDir, string targetFileName = null)
        {
            try
            {
                var hashPath = Path.Combine(baseDir, hash);
                var targetDir = hashPath;

                LogInfo("ProcessDownloadedFile", $"Processing download for {hash} at {hashPath}");

                // Check if it's a single file or directory
                if (File.Exists(hashPath))
                {
                    // Single file downloaded - need to convert to directory structure
                    LogInfo("ProcessDownloadedFile", $"Found file at {hashPath}, converting to directory");
                    
                    var tempPath = hashPath + "_tmp";
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                    File.Move(hashPath, tempPath);
                    
                    Directory.CreateDirectory(targetDir);
                    
                    var finalFileName = string.IsNullOrEmpty(targetFileName) ? "artifact" : targetFileName;
                    var finalPath = Path.Combine(targetDir, finalFileName);
                    
                    File.Move(tempPath, finalPath);
                    
                    LogInfo("ProcessDownloadedFile", $"Successfully processed single file for {hash} as {finalFileName}");
                    return true;
                }
                else if (Directory.Exists(hashPath))
                {
                    // Directory downloaded - check for files to rename
                    LogInfo("ProcessDownloadedFile", $"Found directory at {hashPath}");
                    
                    // Check if there's a file with the hash name inside that needs renaming
                    var internalFile = Path.Combine(hashPath, hash);
                    if (File.Exists(internalFile) && !string.IsNullOrEmpty(targetFileName))
                    {
                        var finalPath = Path.Combine(hashPath, targetFileName);
                        LogInfo("ProcessDownloadedFile", $"Renaming internal file {hash} to {targetFileName}");
                        
                        try
                        {
                            if (File.Exists(finalPath))
                            {
                                File.Delete(finalPath);
                            }
                            File.Move(internalFile, finalPath);
                            LogInfo("ProcessDownloadedFile", $"Successfully renamed to {targetFileName}");
                        }
                        catch (Exception ex)
                        {
                            LogError("ProcessDownloadedFile", $"Error renaming file: {ex.Message}");
                        }
                    }
                    
                    // Check if directory has any files at all
                    var files = Directory.GetFiles(hashPath, "*", SearchOption.AllDirectories);
                    if (files.Length == 0)
                    {
                        LogWarning("ProcessDownloadedFile", $"Directory {hashPath} is empty");
                        return false;
                    }
                    
                    LogInfo("ProcessDownloadedFile", $"Successfully processed directory for {hash} with {files.Length} file(s)");
                    return true;
                }
                else
                {
                    LogWarning("ProcessDownloadedFile", $"No file or directory found at {hashPath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogError("ProcessDownloadedFile", $"Error processing file for {hash}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cleans up build directory used during IPFS operations.
        /// </summary>
        public static void CleanupBuildDirectory(string hash, string baseDir)
        {
            try
            {
                var buildDir = Path.Combine(baseDir, hash + "-build");
                if (Directory.Exists(buildDir))
                {
                    Directory.Delete(buildDir, true);
                }
            }
            catch { }
        }

        /// <summary>
        /// Normalizes an IPFS URN whose CID/filename separator was corrupted by JSON decoding.
        /// When a URN like <c>IPFS:CID\bfilename</c> is stored in JSON, the backslash-letter
        /// sequence (e.g. <c>\f</c> for a filename starting with 'f') is decoded by the JSON
        /// parser as a control character (form-feed 0x0C) rather than a backslash + the letter.
        /// Both the separator and the first letter of the filename are lost in the process.
        /// This method detects such control characters at position 51 (immediately after the
        /// 5-char "IPFS:" prefix and 46-char CIDv0) and restores them to a forward slash
        /// followed by the original letter, e.g. chr(12) → "/f".
        /// Supports both "IPFS:" and "ipfs:" prefixes and the formats
        /// <c>IPFS:CID/filename</c> and <c>IPFS:CID\filename</c> without modification.
        /// </summary>
        /// <param name="urn">Raw IPFS URN, possibly containing JSON-decoded control characters.</param>
        /// <returns>Normalized URN with correct forward-slash separator and restored letter.</returns>
        public static string NormalizeIpfsControlChars(string urn)
        {
            if (string.IsNullOrEmpty(urn) || urn.Length <= 51) return urn;
            if (!urn.StartsWith("IPFS:", StringComparison.OrdinalIgnoreCase)) return urn;

            // Position 51 is the first character after "IPFS:" (5 chars) + CIDv0 (46 chars).
            // It should be '/', '\', or '.' (extension). A control character here means the
            // JSON encoder wrote a literal backslash before the filename's first letter, which
            // the JSON parser then decoded as that letter's escape sequence (consuming the letter).
            switch (urn[51])
            {
                case '\f': return urn.Substring(0, 51) + "/f" + urn.Substring(52); // \f → form-feed; restore /f
                case '\b': return urn.Substring(0, 51) + "/b" + urn.Substring(52); // \b → backspace; restore /b
                case '\t': return urn.Substring(0, 51) + "/t" + urn.Substring(52); // \t → tab; restore /t
                case '\r': return urn.Substring(0, 51) + "/r" + urn.Substring(52); // \r → carriage-return; restore /r
                case '\n': return urn.Substring(0, 51) + "/n" + urn.Substring(52); // \n → newline; restore /n
                default:   return urn;
            }
        }

        private static void LogInfo(string method, string message)
        {
            Debug.WriteLine($"[IpfsHelper.{method}] INFO: {message}");
        }

        private static void LogWarning(string method, string message)
        {
            Debug.WriteLine($"[IpfsHelper.{method}] WARNING: {message}");
        }

        private static void LogError(string method, string message)
        {
            Debug.WriteLine($"[IpfsHelper.{method}] ERROR: {message}");
        }
    }
}
