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

            return await Task.Run(() =>
            {
                try
                {
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = IpfsExecutable;
                        process.StartInfo.Arguments = $"get {hash} -o \"{outputPath}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;

                        process.Start();

                        // We must read streams to avoid deadlock if they fill up buffer
                        // ReadToEndAsync runs on thread pool, so it won't block the WaitForExit
                        var outputTask = process.StandardOutput.ReadToEndAsync();
                        var errorTask = process.StandardError.ReadToEndAsync();

                        if (process.WaitForExit(timeoutMs))
                        {
                            // Process completed within timeout
                            // Wait for streams to ensure we have the output
                            Task.WaitAll(outputTask, errorTask);

                            var output = outputTask.Result;
                            var error = errorTask.Result;

                            if (process.ExitCode == 0)
                            {
                                LogInfo("GetAsync", $"Successfully retrieved {hash}");
                                if (!string.IsNullOrEmpty(output)) LogInfo("GetAsync", $"IPFS output: {output}");
                                return true;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(error)) LogError("GetAsync", $"IPFS get failed for {hash}: {error}");
                                if (!string.IsNullOrEmpty(output)) LogInfo("GetAsync", $"IPFS get output for {hash}: {output}");
                                return false;
                            }
                        }
                        else
                        {
                            // Timeout
                            try { process.Kill(); } catch (Exception ex) { LogError("GetAsync", $"Error killing process on timeout: {ex.Message}"); }
                            LogError("GetAsync", $"IPFS get timeout for {hash}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError("GetAsync", $"Exception during IPFS get for {hash}: {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Asynchronously pins a file in IPFS (fire and forget).
        /// </summary>
        /// <param name="hash">IPFS hash to pin</param>
        public static async Task PinAsync(string hash)
        {
            if (string.IsNullOrEmpty(hash)) return;

            // Check if pinning is enabled
            if (!File.Exists("IPFS_PINNING_ENABLED")) return;

            await Task.Run(() =>
            {
                try
                {
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = IpfsExecutable;
                        process.StartInfo.Arguments = $"pin add {hash}";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.RedirectStandardError = true;

                        process.Start();

                        if (process.WaitForExit(30000))
                        {
                             LogInfo("PinAsync", $"Pin completed for {hash}");
                        }
                        else
                        {
                            try { process.Kill(); } catch {}
                            LogWarning("PinAsync", $"IPFS pin timeout for {hash}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError("PinAsync", $"Exception during IPFS pin for {hash}: {ex.Message}");
                }
            });
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
