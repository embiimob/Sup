using System;
using System.Text;
using NBitcoin;
using NBitcoin.Crypto;

namespace SUP.P2FK
{
    /// <summary>
    /// Fast Bitcoin message signature verification using NBitcoin's libsecp256k1 wrapper.
    /// This is significantly faster than pure C# elliptic curve operations.
    /// </summary>
    public static class MessageSignatureVerifierNBitcoin
    {
        /// <summary>
        /// Verifies a Bitcoin message signature against an address using NBitcoin.
        /// This uses NBitcoin's optimized crypto which wraps libsecp256k1.
        /// </summary>
        /// <param name="address">The Bitcoin address that supposedly signed the message</param>
        /// <param name="signature">The base64-encoded compact signature</param>
        /// <param name="message">The message that was signed</param>
        /// <param name="isTestnet">Whether to verify against testnet3 addresses (default: true)</param>
        /// <returns>True if the signature is valid for the given address and message</returns>
        public static bool VerifyMessage(string address, string signature, string message, bool isTestnet = true)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(message))
                {
                    return false;
                }

                // Determine the network
                Network network = isTestnet ? Network.TestNet : Network.Main;

                // Parse the Bitcoin address
                BitcoinAddress bitcoinAddress;
                try
                {
                    bitcoinAddress = BitcoinAddress.Create(address, network);
                }
                catch
                {
                    return false; // Invalid address format
                }

                // Use NBitcoin's built-in message verification
                // This internally uses optimized libsecp256k1 for signature recovery
                return bitcoinAddress.VerifyMessage(message, signature);
            }
            catch
            {
                return false;
            }
        }
    }
}
