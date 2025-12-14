using System;
using SUP.P2FK.Tests;

namespace SUP.P2FK.TestRunner
{
    /// <summary>
    /// Simple console program to run SHA256 concurrency tests.
    /// This can be compiled and run independently to verify the SHA256 threading fixes.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running SHA256 Concurrency Tests");
            Console.WriteLine("This tests that SHA256.Hash and SHA256.DoubleHash work correctly");
            Console.WriteLine("when called from multiple threads concurrently.");
            Console.WriteLine();
            
            SHA256ConcurrencyTest.RunAllTests();
            
            Console.WriteLine();
            Console.WriteLine("All tests completed successfully!");
        }
    }
}
