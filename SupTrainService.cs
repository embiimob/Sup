using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SUP
{
    /// <summary>
    /// Core service for SupTrain operations including job discovery, publishing, and IPFS integration
    /// </summary>
    public class SupTrainService
    {
        private readonly string rpcUrl;
        private readonly string rpcUser;
        private readonly string rpcPassword;
        private readonly bool testnet;

        public SupTrainService(string rpcUrl, string rpcUser, string rpcPassword, bool testnet = true)
        {
            this.rpcUrl = rpcUrl;
            this.rpcUser = rpcUser;
            this.rpcPassword = rpcPassword;
            this.testnet = testnet;
        }

        #region Job Discovery

        /// <summary>
        /// Search for training jobs by keyword using Sup!? message search
        /// </summary>
        public async Task<List<SupTrainJob>> SearchJobsByKeywordAsync(string keyword)
        {
            var jobs = new List<SupTrainJob>();

            try
            {
                // TODO: Integrate with existing Root.GetPublicAddressByKeyword pattern
                // For now, return empty list as placeholder
                // In real implementation:
                // 1. Search blockchain for messages containing keyword
                // 2. Parse messages for SupTrain job genesis announcements
                // 3. Extract CIDs and job metadata
                // 4. Fetch job.json from IPFS to get full details

                Debug.WriteLine($"[SupTrainService] Searching for jobs with keyword: {keyword}");
                
                return jobs;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SupTrainService] Error searching jobs: {ex.Message}");
                return jobs;
            }
        }

        /// <summary>
        /// Parse a Sup!? message to extract SupTrain metadata
        /// </summary>
        public SupTrainJob ParseJobMessage(string message)
        {
            try
            {
                var job = new SupTrainJob();

                // Extract hashtag values
                job.JobId = ExtractHashtagValue(message, "job");
                job.ModelSlug = ExtractHashtagValue(message, "model");
                
                var roundStr = ExtractHashtagValue(message, "round");
                if (int.TryParse(roundStr, out int round))
                {
                    job.Round = round;
                }

                job.BaseCheckpointCID = ExtractHashtagValue(message, "checkpoint");
                job.ManifestCID = ExtractHashtagValue(message, "manifest");
                job.EvalCID = ExtractHashtagValue(message, "eval");
                job.PolicyCID = ExtractHashtagValue(message, "policy");

                // Also check for <<IPFS:CID>> pattern
                var ipfsMatches = Regex.Matches(message, @"<<IPFS:([^>]+)>>");
                if (ipfsMatches.Count > 0)
                {
                    // Use first IPFS CID found if no hashtag CID
                    if (string.IsNullOrEmpty(job.BaseCheckpointCID))
                    {
                        job.BaseCheckpointCID = ipfsMatches[0].Groups[1].Value;
                    }
                }

                return job;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SupTrainService] Error parsing job message: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Find latest checkpoint announcement for a job
        /// </summary>
        public async Task<AggregateCheckpoint> FindLatestCheckpointAsync(string jobId)
        {
            try
            {
                // TODO: Search for messages with #suptrain #aggregate #checkpoint #job:{jobId}
                // Order by timestamp descending, take first
                // Parse and return checkpoint details
                
                Debug.WriteLine($"[SupTrainService] Finding latest checkpoint for job: {jobId}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SupTrainService] Error finding checkpoint: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Publishing

        /// <summary>
        /// Publish a worker update to the Sup!? network
        /// </summary>
        public async Task<bool> PublishUpdateAsync(WorkerUpdate update, string workerAddress)
        {
            try
            {
                // Build message with keywords
                string message = BuildUpdateMessage(update, workerAddress);

                // TODO: Post message using existing RPC pattern
                // Similar to how SupMain posts public messages
                // Use blockchain RPC to create and broadcast transaction
                
                Debug.WriteLine($"[SupTrainService] Publishing update: {message}");
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SupTrainService] Error publishing update: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Publish an aggregate checkpoint announcement
        /// </summary>
        public async Task<bool> PublishAggregateAsync(AggregateCheckpoint checkpoint, string aggregatorAddress)
        {
            try
            {
                string message = BuildAggregateMessage(checkpoint, aggregatorAddress);
                
                // TODO: Post message using blockchain RPC
                Debug.WriteLine($"[SupTrainService] Publishing aggregate: {message}");
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SupTrainService] Error publishing aggregate: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Publish a job genesis announcement
        /// </summary>
        public async Task<bool> PublishJobGenesisAsync(SupTrainJob job, string creatorAddress)
        {
            try
            {
                string message = BuildJobGenesisMessage(job, creatorAddress);
                
                Debug.WriteLine($"[SupTrainService] Publishing job genesis: {message}");
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SupTrainService] Error publishing job genesis: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region IPFS Operations

        /// <summary>
        /// Upload a file to IPFS and return its CID
        /// </summary>
        public async Task<string> IpfsAddFileAsync(string filePath)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = @"ipfs\ipfs.exe",
                    Arguments = $"add \"{filePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(processInfo))
                {
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    await Task.Run(() => process.WaitForExit());

                    if (process.ExitCode == 0)
                    {
                        // Parse CID from output: "added <CID> <filename>"
                        var match = Regex.Match(output, @"added\s+(\S+)");
                        if (match.Success)
                        {
                            string cid = match.Groups[1].Value;
                            Debug.WriteLine($"[SupTrainService] Uploaded {filePath} to IPFS: {cid}");
                            return cid;
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"[SupTrainService] IPFS add error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SupTrainService] Error adding file to IPFS: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Upload a directory to IPFS and return its CID
        /// </summary>
        public async Task<string> IpfsAddDirectoryAsync(string dirPath)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = @"ipfs\ipfs.exe",
                    Arguments = $"add -r \"{dirPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(processInfo))
                {
                    string output = await process.StandardOutput.ReadToEndAsync();
                    await Task.Run(() => process.WaitForExit());

                    if (process.ExitCode == 0)
                    {
                        // Last line contains the directory CID
                        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        if (lines.Length > 0)
                        {
                            var match = Regex.Match(lines[lines.Length - 1], @"added\s+(\S+)");
                            if (match.Success)
                            {
                                string cid = match.Groups[1].Value;
                                Debug.WriteLine($"[SupTrainService] Uploaded directory {dirPath} to IPFS: {cid}");
                                return cid;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SupTrainService] Error adding directory to IPFS: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Download a file from IPFS
        /// </summary>
        public async Task<bool> IpfsGetAsync(string cid, string outputPath, int timeoutMs = 120000)
        {
            return await IpfsHelper.GetAsync(cid, outputPath, timeoutMs);
        }

        /// <summary>
        /// Pin a CID in IPFS
        /// </summary>
        public async Task IpfsPinAsync(string cid)
        {
            await IpfsHelper.PinAsync(cid);
        }

        #endregion

        #region Message Building

        private string BuildUpdateMessage(WorkerUpdate update, string workerAddress)
        {
            return $"{SupTrainKeywords.Base} {SupTrainKeywords.Update} " +
                   $"{SupTrainKeywords.Job(update.JobId)} " +
                   $"{SupTrainKeywords.Round(update.Round)} " +
                   $"{SupTrainKeywords.BaseCheckpoint(update.BaseCheckpointCID)} " +
                   $"{SupTrainKeywords.Delta(update.DeltaCID)} " +
                   $"{SupTrainKeywords.Metrics(update.MetricsCID)} " +
                   $"{SupTrainKeywords.From(workerAddress)}";
        }

        private string BuildAggregateMessage(AggregateCheckpoint checkpoint, string aggregatorAddress)
        {
            return $"{SupTrainKeywords.Base} {SupTrainKeywords.Aggregate} " +
                   $"{SupTrainKeywords.Job(checkpoint.JobId)} " +
                   $"{SupTrainKeywords.Round(checkpoint.Round)} " +
                   $"{SupTrainKeywords.Checkpoint} " +
                   $"#cid:{checkpoint.CheckpointCID} " +
                   $"{SupTrainKeywords.Inputs(checkpoint.InputsCID)} " +
                   $"{SupTrainKeywords.Metrics(checkpoint.MetricsCID)} " +
                   $"{SupTrainKeywords.From(aggregatorAddress)}";
        }

        private string BuildJobGenesisMessage(SupTrainJob job, string creatorAddress)
        {
            return $"{SupTrainKeywords.Base} {SupTrainKeywords.JobGenesis} " +
                   $"{SupTrainKeywords.Model(job.ModelSlug)} " +
                   $"{SupTrainKeywords.Job(job.JobId)} " +
                   $"#cid:{job.BaseCheckpointCID} " +
                   $"{SupTrainKeywords.Manifest(job.ManifestCID)} " +
                   $"{SupTrainKeywords.Checkpoint} " +
                   $"{SupTrainKeywords.BaseCheckpoint(job.BaseCheckpointCID)} " +
                   $"{SupTrainKeywords.From(creatorAddress)}";
        }

        #endregion

        #region Helper Methods

        private string ExtractHashtagValue(string message, string tag)
        {
            // Match patterns like #tag:value
            var pattern = $@"#{tag}:(\S+)";
            var match = Regex.Match(message, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        #endregion
    }
}
