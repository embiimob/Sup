using System;
using System.Diagnostics;
using System.IO;
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
                    
                    // Wait for exit with cancellation token
                    var tcs = new TaskCompletionSource<bool>();
                    cts.Token.Register(() =>
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                                LogWarning("GetAsync", $"IPFS get timeout for {hash}, killed process");
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

                    // Start reading output to prevent buffer overflow
                    var outputTask = process.StandardOutput.ReadToEndAsync();
                    var errorTask = process.StandardError.ReadToEndAsync();

                    // Wait for process to exit or timeout
                    var result = await tcs.Task;
                    
                    if (!result)
                    {
                        var error = await errorTask;
                        if (!string.IsNullOrEmpty(error))
                        {
                            LogError("GetAsync", $"IPFS get failed for {hash}: {error}");
                        }
                    }
                    else
                    {
                        LogInfo("GetAsync", $"Successfully retrieved {hash}");
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

                // Check if it's a single file or directory
                if (File.Exists(hashPath))
                {
                    // Single file - move and rename
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
                    
                    LogInfo("ProcessDownloadedFile", $"Processed single file for {hash}");
                    return true;
                }
                else if (Directory.Exists(hashPath))
                {
                    // Directory - check if we need to rename internal file
                    var internalFile = Path.Combine(hashPath, hash);
                    if (File.Exists(internalFile) && !string.IsNullOrEmpty(targetFileName))
                    {
                        var finalPath = Path.Combine(hashPath, targetFileName);
                        try
                        {
                            if (File.Exists(finalPath))
                            {
                                File.Delete(finalPath);
                            }
                            File.Move(internalFile, finalPath);
                        }
                        catch { }
                    }
                    
                    LogInfo("ProcessDownloadedFile", $"Processed directory for {hash}");
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
