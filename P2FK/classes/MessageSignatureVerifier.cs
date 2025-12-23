using System;

namespace SUP.P2FK
{
    /// <summary>
    /// Provides Bitcoin-style message signature verification without requiring RPC calls.
    /// Supports both mainnet and testnet3 address validation.
    /// Uses NBitcoin's optimized libsecp256k1 wrapper for fast verification.
    /// </summary>
    public static class MessageSignatureVerifier
    {
        /// <summary>
        /// Verifies a Bitcoin message signature against an address.
        /// Uses NBitcoin's optimized libsecp256k1 for fast verification.
        /// </summary>
        /// <param name="address">The Bitcoin address that supposedly signed the message</param>
        /// <param name="signature">The base64-encoded compact signature</param>
        /// <param name="message">The message that was signed (can be hex or plain text)</param>
        /// <param name="isTestnet">Whether to verify against testnet3 addresses (default: true)</param>
        /// <returns>True if the signature is valid for the given address and message; False if invalid, null, or empty inputs</returns>
        public static bool VerifyMessage(string address, string signature, string message, bool isTestnet = true)
        {
            // Use NBitcoin's fast verification which wraps libsecp256k1
            return MessageSignatureVerifierNBitcoin.VerifyMessage(address, signature, message, isTestnet);
        }
    }
}
