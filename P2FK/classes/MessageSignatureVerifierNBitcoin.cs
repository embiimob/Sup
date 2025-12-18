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
        private const string BitcoinSignedMessageHeader = "Bitcoin Signed Message:\n";

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

                // Decode the signature
                byte[] signatureBytes;
                try
                {
                    signatureBytes = Convert.FromBase64String(signature);
                }
                catch
                {
                    return false; // Invalid base64
                }

                if (signatureBytes.Length != 65)
                {
                    return false; // Invalid signature length
                }

                // Create the message hash using Bitcoin's standard format
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                uint256 messageHash = new uint256(HashMessage(messageBytes));

                // Try to recover the public key and verify
                // NBitcoin's CompactSignature expects the full 65-byte signature
                try
                {
                    // Create CompactSignature from the 65-byte signature
                    var compactSig = new CompactSignature(signatureBytes);
                    
                    // Recover the public key using NBitcoin's libsecp256k1 wrapper
                    PubKey recoveredPubKey = compactSig.RecoverPubKey(messageHash);
                    
                    if (recoveredPubKey != null)
                    {
                        // Get the Bitcoin address from the recovered public key
                        BitcoinAddress recoveredAddress = recoveredPubKey.GetAddress(ScriptPubKeyType.Legacy, network);
                        
                        // Compare addresses
                        if (recoveredAddress.ToString() == address)
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    // Signature recovery failed
                    return false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a Bitcoin message hash following the standard Bitcoin message signing format.
        /// </summary>
        private static byte[] HashMessage(byte[] messageBytes)
        {
            // Bitcoin message format: varint(len(header)) + header + varint(len(message)) + message
            byte[] header = Encoding.UTF8.GetBytes(BitcoinSignedMessageHeader);
            
            // Calculate the total size
            int varintSize = messageBytes.Length < 253 ? 1 : 3;
            byte[] fullMessage = new byte[1 + header.Length + varintSize + messageBytes.Length];
            int offset = 0;

            // Add header length (varint)
            fullMessage[offset++] = (byte)header.Length;
            
            // Add header
            Buffer.BlockCopy(header, 0, fullMessage, offset, header.Length);
            offset += header.Length;

            // Add message length (varint)
            if (messageBytes.Length < 253)
            {
                fullMessage[offset++] = (byte)messageBytes.Length;
            }
            else
            {
                fullMessage[offset++] = 0xfd;
                fullMessage[offset++] = (byte)(messageBytes.Length & 0xff);
                fullMessage[offset++] = (byte)((messageBytes.Length >> 8) & 0xff);
            }

            // Add message
            Buffer.BlockCopy(messageBytes, 0, fullMessage, offset, messageBytes.Length);

            // Double SHA256 hash using existing SHA256 class
            return SHA256.DoubleHash(fullMessage);
        }
    }
}
