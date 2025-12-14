using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SUP
{
    /// <summary>
    /// Tracks memory usage and provides diagnostic logging for the message display system.
    /// Helps validate that memory stays bounded as messages are loaded and disposed.
    /// </summary>
    public static class MemoryDiagnostics
    {
        private static long _lastReportedMemoryBytes = 0;
        private static DateTime _lastReportTime = DateTime.MinValue;
        private static readonly TimeSpan ReportInterval = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Gets the current managed memory usage in bytes
        /// </summary>
        public static long GetManagedMemoryBytes()
        {
            return GC.GetTotalMemory(false);
        }

        /// <summary>
        /// Gets the current managed memory usage in megabytes
        /// </summary>
        public static double GetManagedMemoryMB()
        {
            return GetManagedMemoryBytes() / (1024.0 * 1024.0);
        }

        /// <summary>
        /// Gets the current process working set (total memory) in bytes
        /// </summary>
        public static long GetProcessMemoryBytes()
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                return currentProcess.WorkingSet64;
            }
        }

        /// <summary>
        /// Gets the current process working set (total memory) in megabytes
        /// </summary>
        public static double GetProcessMemoryMB()
        {
            return GetProcessMemoryBytes() / (1024.0 * 1024.0);
        }

        /// <summary>
        /// Logs current memory usage with a context message.
        /// Only logs if sufficient time has passed since last report to avoid spam.
        /// </summary>
        /// <param name="context">Context string describing what's happening (e.g., "After loading messages")</param>
        /// <param name="force">If true, always log regardless of time interval</param>
        public static void LogMemoryUsage(string context, bool force = false)
        {
            DateTime now = DateTime.Now;
            
            // Rate limit logging unless forced
            if (!force && (now - _lastReportTime) < ReportInterval)
            {
                return;
            }

            long managedBytes = GetManagedMemoryBytes();
            long processBytes = GetProcessMemoryBytes();
            
            double managedMB = managedBytes / (1024.0 * 1024.0);
            double processMB = processBytes / (1024.0 * 1024.0);
            
            // Calculate delta since last report
            long deltaBytes = managedBytes - _lastReportedMemoryBytes;
            double deltaMB = deltaBytes / (1024.0 * 1024.0);
            string deltaStr = deltaBytes >= 0 ? $"+{deltaMB:F2}" : $"{deltaMB:F2}";

            Debug.WriteLine($"[Memory] {context}");
            Debug.WriteLine($"[Memory]   Managed: {managedMB:F2} MB ({deltaStr} MB)");
            Debug.WriteLine($"[Memory]   Process: {processMB:F2} MB");
            Debug.WriteLine($"[Memory]   GC Gen0: {GC.CollectionCount(0)}, Gen1: {GC.CollectionCount(1)}, Gen2: {GC.CollectionCount(2)}");

            _lastReportedMemoryBytes = managedBytes;
            _lastReportTime = now;
        }

        /// <summary>
        /// Starts a background task that periodically logs memory usage.
        /// Useful for tracking memory over time during long operations.
        /// </summary>
        /// <param name="intervalSeconds">How often to log (default 30 seconds)</param>
        /// <param name="cancellationToken">Token to cancel the monitoring</param>
        public static Task StartPeriodicMonitoring(int intervalSeconds = 30, CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    LogMemoryUsage("Periodic memory check", force: true);
                    
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
                
                Debug.WriteLine("[Memory] Periodic monitoring stopped");
            }, cancellationToken);
        }

        /// <summary>
        /// Forces a garbage collection and logs the results.
        /// Useful for testing memory cleanup effectiveness.
        /// </summary>
        public static void ForceGCAndLog(string context)
        {
            Debug.WriteLine($"[Memory] Forcing GC: {context}");
            
            long beforeBytes = GetManagedMemoryBytes();
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            long afterBytes = GetManagedMemoryBytes();
            long freedBytes = beforeBytes - afterBytes;
            double freedMB = freedBytes / (1024.0 * 1024.0);
            
            Debug.WriteLine($"[Memory] GC freed {freedMB:F2} MB");
            LogMemoryUsage($"After GC: {context}", force: true);
        }

        /// <summary>
        /// Gets a memory usage summary as a formatted string.
        /// Useful for displaying in UI or logs.
        /// </summary>
        public static string GetMemorySummary()
        {
            return $"Managed: {GetManagedMemoryMB():F2} MB, Process: {GetProcessMemoryMB():F2} MB";
        }
    }
}
