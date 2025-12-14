# SHA256 Concurrency Tests

This directory contains tests to verify that the SHA256 hashing implementation is thread-safe and works correctly under concurrent access.

## Background

The main SUP application was experiencing intermittent `CryptographicException` errors with message "Unknown error -1073741816" when the Object Details form loaded. This was caused by a static, shared `SHA256Managed` instance being used across multiple threads without synchronization.

## Fix

The `SHA256.cs` class was refactored to:
1. Remove the static `SHA256Managed` instance
2. Create a new `SHA256` instance per call using `SHA256.Create()`
3. Properly dispose of instances using `using` statements
4. Add null argument validation

## Tests

The test suite includes:

1. **Null Input Handling** - Verifies that null inputs throw `ArgumentNullException` with clear messages
2. **Hash Consistency** - Ensures that hashing the same data multiple times produces identical results
3. **Concurrent Hash** - Tests that `SHA256.Hash()` can be called from multiple threads simultaneously
4. **Concurrent DoubleHash** - Tests that `SHA256.DoubleHash()` can be called from multiple threads simultaneously

## Running the Tests

Since this is a Windows Forms application targeting .NET Framework 4.7.2, the tests are designed as simple console tests that can be integrated into the main project or run separately.

To run the tests:
1. Ensure `SHA256ConcurrencyTest.cs` and `TestRunner.cs` are compiled with the project
2. Call `SHA256ConcurrencyTest.RunAllTests()` from your test runner or during application initialization

The tests will:
- Execute 10 threads, each performing 100 hash operations
- Report any exceptions that occur
- Display a summary of passed/failed tests
- Exit with code 1 if any tests fail

## Expected Results

All tests should pass, indicating that:
- The SHA256 implementation is thread-safe
- Multiple threads can hash data concurrently without errors
- No `CryptographicException` is thrown during concurrent access
- Hash results are deterministic and consistent
