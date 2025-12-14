using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SUP.P2FK;

namespace SUP.P2FK.Tests
{
    /// <summary>
    /// Test class to verify SHA256 hashing works correctly under concurrent access.
    /// This addresses the intermittent CryptographicException that was occurring
    /// when multiple threads tried to use a shared static SHA256 instance.
    /// </summary>
    public class SHA256ConcurrencyTest
    {
        /// <summary>
        /// Test that DoubleHash can be called concurrently from multiple threads
        /// without throwing CryptographicException.
        /// </summary>
        public static bool TestConcurrentDoubleHash()
        {
            Console.WriteLine("Testing concurrent DoubleHash calls...");
            
            const int numThreads = 10;
            const int operationsPerThread = 100;
            var exceptions = new List<Exception>();
            var lockObj = new object();
            
            // Create test data
            var testData = new List<byte[]>();
            var random = new Random(12345); // Fixed seed for reproducibility
            for (int i = 0; i < numThreads; i++)
            {
                var data = new byte[32];
                random.NextBytes(data);
                testData.Add(data);
            }
            
            // Run concurrent hashing operations
            var tasks = new Task[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                var threadIndex = i;
                tasks[i] = Task.Run(() =>
                {
                    try
                    {
                        for (int j = 0; j < operationsPerThread; j++)
                        {
                            var result = SHA256.DoubleHash(testData[threadIndex]);
                            
                            // Verify result is not null and has expected length
                            if (result == null || result.Length != 32)
                            {
                                throw new Exception($"Invalid hash result: {result?.Length ?? -1} bytes");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObj)
                        {
                            exceptions.Add(ex);
                        }
                    }
                });
            }
            
            Task.WaitAll(tasks);
            
            if (exceptions.Count > 0)
            {
                Console.WriteLine($"FAILED: {exceptions.Count} exceptions occurred:");
                foreach (var ex in exceptions)
                {
                    Console.WriteLine($"  - {ex.GetType().Name}: {ex.Message}");
                }
                return false;
            }
            
            Console.WriteLine($"SUCCESS: {numThreads * operationsPerThread} concurrent hash operations completed without errors");
            return true;
        }
        
        /// <summary>
        /// Test that Hash method can be called concurrently.
        /// </summary>
        public static bool TestConcurrentHash()
        {
            Console.WriteLine("Testing concurrent Hash calls...");
            
            const int numThreads = 10;
            const int operationsPerThread = 100;
            var exceptions = new List<Exception>();
            var lockObj = new object();
            
            var testData = new List<byte[]>();
            var random = new Random(54321);
            for (int i = 0; i < numThreads; i++)
            {
                var data = new byte[64];
                random.NextBytes(data);
                testData.Add(data);
            }
            
            var tasks = new Task[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                var threadIndex = i;
                tasks[i] = Task.Run(() =>
                {
                    try
                    {
                        for (int j = 0; j < operationsPerThread; j++)
                        {
                            var result = SHA256.Hash(testData[threadIndex]);
                            
                            if (result == null || result.Length != 32)
                            {
                                throw new Exception($"Invalid hash result: {result?.Length ?? -1} bytes");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObj)
                        {
                            exceptions.Add(ex);
                        }
                    }
                });
            }
            
            Task.WaitAll(tasks);
            
            if (exceptions.Count > 0)
            {
                Console.WriteLine($"FAILED: {exceptions.Count} exceptions occurred:");
                foreach (var ex in exceptions)
                {
                    Console.WriteLine($"  - {ex.GetType().Name}: {ex.Message}");
                }
                return false;
            }
            
            Console.WriteLine($"SUCCESS: {numThreads * operationsPerThread} concurrent hash operations completed without errors");
            return true;
        }
        
        /// <summary>
        /// Test that null inputs are properly handled.
        /// </summary>
        public static bool TestNullInputHandling()
        {
            Console.WriteLine("Testing null input handling...");
            
            try
            {
                SHA256.Hash(null);
                Console.WriteLine("FAILED: Hash(null) did not throw ArgumentNullException");
                return false;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"SUCCESS: Hash(null) correctly threw ArgumentNullException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: Hash(null) threw unexpected exception: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
            
            try
            {
                SHA256.DoubleHash(null);
                Console.WriteLine("FAILED: DoubleHash(null) did not throw ArgumentNullException");
                return false;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"SUCCESS: DoubleHash(null) correctly threw ArgumentNullException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: DoubleHash(null) threw unexpected exception: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Test that hash results are consistent.
        /// </summary>
        public static bool TestHashConsistency()
        {
            Console.WriteLine("Testing hash consistency...");
            
            var testData = Encoding.UTF8.GetBytes("Test data for consistency check");
            
            // Compute hash multiple times
            var hash1 = SHA256.Hash(testData);
            var hash2 = SHA256.Hash(testData);
            var doubleHash1 = SHA256.DoubleHash(testData);
            var doubleHash2 = SHA256.DoubleHash(testData);
            
            // Verify consistency
            if (!hash1.SequenceEqual(hash2))
            {
                Console.WriteLine("FAILED: Hash results are not consistent");
                return false;
            }
            
            if (!doubleHash1.SequenceEqual(doubleHash2))
            {
                Console.WriteLine("FAILED: DoubleHash results are not consistent");
                return false;
            }
            
            Console.WriteLine("SUCCESS: Hash results are consistent across multiple calls");
            return true;
        }
        
        /// <summary>
        /// Run all tests.
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("=== SHA256 Concurrency Tests ===");
            Console.WriteLine();
            
            var results = new List<(string testName, bool passed)>
            {
                ("Null Input Handling", TestNullInputHandling()),
                ("Hash Consistency", TestHashConsistency()),
                ("Concurrent Hash", TestConcurrentHash()),
                ("Concurrent DoubleHash", TestConcurrentDoubleHash())
            };
            
            Console.WriteLine();
            Console.WriteLine("=== Test Summary ===");
            
            int passed = 0;
            int failed = 0;
            
            foreach (var (testName, result) in results)
            {
                if (result)
                {
                    Console.WriteLine($"✓ {testName}");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"✗ {testName}");
                    failed++;
                }
            }
            
            Console.WriteLine();
            Console.WriteLine($"Total: {passed} passed, {failed} failed");
            
            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }
    }
}
