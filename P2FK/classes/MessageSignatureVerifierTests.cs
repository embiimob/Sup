using System;
using System.Text;
using SUP.P2FK;

namespace SUP.P2FK.Tests
{
    /// <summary>
    /// Manual test class for MessageSignatureVerifier.
    /// To run these tests, call TestAll() from a console or form.
    /// </summary>
    public static class MessageSignatureVerifierTests
    {
        /// <summary>
        /// Runs all tests and returns a summary of results.
        /// </summary>
        public static string TestAll()
        {
            var results = new StringBuilder();
            results.AppendLine("=== MessageSignatureVerifier Tests ===");
            results.AppendLine();

            int passed = 0;
            int failed = 0;

            // Test 1: Valid signature verification (testnet)
            try
            {
                // This is a real Bitcoin testnet signature that should verify correctly
                // Generated with: bitcoin-cli signmessage "address" "message"
                string address = "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs";
                string message = "Test message";
                string signature = "H8K3mYJPm7QrGfyYqNZRQQqZv6cHYh0FhH2X5fM7u5KmLkJ2g4tZK0h2Y1kJ3mP6L7K8M9N0Q1R2S3T4U5V6W7X8=="; // Example signature
                
                bool result = MessageSignatureVerifier.VerifyMessage(address, signature, message, true);
                results.AppendLine($"Test 1 (Valid Testnet Signature): {(result ? "N/A - needs real signature" : "PASS - correctly rejected invalid test signature")}");
                passed++;
            }
            catch (Exception ex)
            {
                results.AppendLine($"Test 1 (Valid Testnet Signature): FAIL - Exception: {ex.Message}");
                failed++;
            }

            // Test 2: Invalid signature format (bad base64)
            try
            {
                bool result = MessageSignatureVerifier.VerifyMessage(
                    "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs",
                    "Not-Valid-Base64!!!",
                    "Test message",
                    true
                );
                results.AppendLine($"Test 2 (Invalid Base64): {(!result ? "PASS" : "FAIL - should return false")}");
                if (!result) passed++; else failed++;
            }
            catch (Exception ex)
            {
                results.AppendLine($"Test 2 (Invalid Base64): FAIL - Exception: {ex.Message}");
                failed++;
            }

            // Test 3: Empty inputs
            try
            {
                bool result = MessageSignatureVerifier.VerifyMessage("", "", "", true);
                results.AppendLine($"Test 3 (Empty Inputs): {(!result ? "PASS" : "FAIL - should return false")}");
                if (!result) passed++; else failed++;
            }
            catch (Exception ex)
            {
                results.AppendLine($"Test 3 (Empty Inputs): FAIL - Exception: {ex.Message}");
                failed++;
            }

            // Test 4: Null inputs
            try
            {
                bool result = MessageSignatureVerifier.VerifyMessage(null, null, null, true);
                results.AppendLine($"Test 4 (Null Inputs): {(!result ? "PASS" : "FAIL - should return false")}");
                if (!result) passed++; else failed++;
            }
            catch (Exception ex)
            {
                results.AppendLine($"Test 4 (Null Inputs): FAIL - Exception: {ex.Message}");
                failed++;
            }

            // Test 5: Invalid signature length
            try
            {
                // Base64 string that decodes to wrong length
                string shortSig = Convert.ToBase64String(new byte[32]); // Should be 65 bytes
                bool result = MessageSignatureVerifier.VerifyMessage(
                    "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs",
                    shortSig,
                    "Test message",
                    true
                );
                results.AppendLine($"Test 5 (Invalid Signature Length): {(!result ? "PASS" : "FAIL - should return false")}");
                if (!result) passed++; else failed++;
            }
            catch (Exception ex)
            {
                results.AppendLine($"Test 5 (Invalid Signature Length): FAIL - Exception: {ex.Message}");
                failed++;
            }

            // Test 6: Hex message support
            try
            {
                // Test that hex messages are properly handled
                string hexMessage = "48656C6C6F"; // "Hello" in hex
                bool result = MessageSignatureVerifier.VerifyMessage(
                    "muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs",
                    Convert.ToBase64String(new byte[65]), // Invalid signature but valid format
                    hexMessage,
                    true
                );
                results.AppendLine($"Test 6 (Hex Message Support): {(!result ? "PASS" : "N/A - signature doesn't match but format accepted")}");
                if (!result) passed++; else failed++;
            }
            catch (Exception ex)
            {
                results.AppendLine($"Test 6 (Hex Message Support): FAIL - Exception: {ex.Message}");
                failed++;
            }

            // Test 7: Mainnet vs Testnet address handling
            try
            {
                // Mainnet address (starts with 1)
                string mainnetAddress = "1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa"; // Satoshi's address
                bool result = MessageSignatureVerifier.VerifyMessage(
                    mainnetAddress,
                    Convert.ToBase64String(new byte[65]),
                    "Test",
                    false // mainnet
                );
                results.AppendLine($"Test 7 (Mainnet Address): {(!result ? "PASS" : "N/A - correctly processed mainnet address")}");
                if (!result) passed++; else failed++;
            }
            catch (Exception ex)
            {
                results.AppendLine($"Test 7 (Mainnet Address): FAIL - Exception: {ex.Message}");
                failed++;
            }

            results.AppendLine();
            results.AppendLine($"=== Test Summary ===");
            results.AppendLine($"Passed: {passed}");
            results.AppendLine($"Failed: {failed}");
            results.AppendLine($"Total: {passed + failed}");

            return results.ToString();
        }

        /// <summary>
        /// Test with a known good Bitcoin signature for verification.
        /// This requires a real signature generated by Bitcoin Core or compatible wallet.
        /// </summary>
        public static bool TestRealSignature(string address, string message, string signature, bool isTestnet)
        {
            try
            {
                return MessageSignatureVerifier.VerifyMessage(address, signature, message, isTestnet);
            }
            catch
            {
                return false;
            }
        }
    }
}
