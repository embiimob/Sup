using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SUP.P2FK
{
    /// <summary>
    /// Provides client-side Bitcoin message signature verification.
    /// This bypasses the need for RPC or API calls to verify message signatures.
    /// Implements the Bitcoin message signing standard (BIP 137).
    /// 
    /// For P2FK verification:
    /// 1. The P2FK message data packet is hashed (SHA-256) to create a hash
    /// 2. The hash (as hex string) is signed using Bitcoin message signing
    /// 3. This class verifies that the signature is valid for that hash
    /// 
    /// The verification process:
    /// - Takes the hash as the "message" parameter
    /// - Formats it with "Bitcoin Signed Message:\n" prefix per BIP 137
    /// - Double-hashes the formatted message (SHA-256)
    /// - Recovers the public key from the ECDSA signature
    /// - Derives the Bitcoin address from the recovered public key
    /// - Compares the derived address with the claimed signer address
    /// </summary>
    public static class BitcoinMessageSigner
    {
        /// <summary>
        /// Verifies a Bitcoin message signature without requiring RPC or API access.
        /// 
        /// For P2FK: The 'message' parameter is the SHA-256 hash (hex string) of the 
        /// P2FK data packet. This hash was signed, and we verify the signature is valid.
        /// </summary>
        /// <param name="address">The Bitcoin address that allegedly signed the message</param>
        /// <param name="signature">The base64-encoded signature</param>
        /// <param name="message">The message that was signed (for P2FK: the hex hash of the data packet)</param>
        /// <param name="network">The Bitcoin network (mainnet=0x00, testnet=0x6F)</param>
        /// <returns>True if the signature is valid for the given address and message</returns>
        public static bool VerifyMessage(string address, string signature, string message, NBitcoin.Network network = null)
        {
            try
            {
                // Default to Bitcoin mainnet if no network specified
                if (network == null)
                {
                    network = NBitcoin.Network.Main;
                }

                // Decode the signature from base64
                byte[] signatureBytes;
                try
                {
                    signatureBytes = Convert.FromBase64String(signature);
                }
                catch
                {
                    return false;
                }

                // The signature should be 65 bytes (recovery flag + r + s)
                if (signatureBytes.Length != 65)
                {
                    return false;
                }

                // Format the message according to Bitcoin message signing standard
                byte[] messageBytes = FormatMessageForSigning(message);

                // Hash the formatted message with double SHA256 using the project's SHA256 class
                byte[] messageHash = SUP.P2FK.SHA256.DoubleHash(messageBytes);

                // Try to recover the public key from the signature and verify
                try
                {
                    // Extract recovery flag and signature components
                    int recoveryFlag = signatureBytes[0] - 27;
                    bool isCompressed = (recoveryFlag & 4) != 0;
                    int recoveryId = recoveryFlag & 3;
                    
                    // Extract r and s components (32 bytes each)
                    byte[] r = new byte[32];
                    byte[] s = new byte[32];
                    Array.Copy(signatureBytes, 1, r, 0, 32);
                    Array.Copy(signatureBytes, 33, s, 0, 32);
                    
                    // Try to recover public key for each possible recovery flag
                    for (int i = 0; i < 4; i++)
                    {
                        try
                        {
                            // Reconstruct the compact signature with current recovery attempt
                            byte[] compactSig = new byte[65];
                            compactSig[0] = (byte)(27 + i + (isCompressed ? 4 : 0));
                            Array.Copy(r, 0, compactSig, 1, 32);
                            Array.Copy(s, 0, compactSig, 33, 32);
                            
                            // Try to recover the public key using NBitcoin's built-in method
                            // PubKey.RecoverFromMessage expects the message hash and compact signature
                            var pubKey = NBitcoin.PubKey.RecoverFromMessage(messageHash, compactSig);
                            
                            if (pubKey != null)
                            {
                                // Get the address from the recovered public key
                                var recoveredAddress = pubKey.GetAddress(NBitcoin.ScriptPubKeyType.Legacy, network);
                                
                                // Compare with the claimed address
                                if (recoveredAddress.ToString() == address)
                                {
                                    return true;
                                }
                                
                                // If mainnet didn't work, try testnet
                                if (network == NBitcoin.Network.Main)
                                {
                                    recoveredAddress = pubKey.GetAddress(NBitcoin.ScriptPubKeyType.Legacy, NBitcoin.Network.TestNet);
                                    if (recoveredAddress.ToString() == address)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Try next recovery flag
                            continue;
                        }
                    }
                    
                    return false;
                }
                catch
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Formats a message according to the Bitcoin message signing standard.
        /// The standard prepends "Bitcoin Signed Message:\n" with length prefixes.
        /// </summary>
        private static byte[] FormatMessageForSigning(string message)
        {
            const string messagePrefix = "Bitcoin Signed Message:\n";
            
            // Convert strings to bytes
            byte[] prefixBytes = Encoding.UTF8.GetBytes(messagePrefix);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            
            // Build the formatted message with compact size prefixes
            var result = new System.Collections.Generic.List<byte>();
            
            // Add prefix length as varint
            result.AddRange(EncodeVarInt(prefixBytes.Length));
            result.AddRange(prefixBytes);
            
            // Add message length as varint
            result.AddRange(EncodeVarInt(messageBytes.Length));
            result.AddRange(messageBytes);
            
            return result.ToArray();
        }

        /// <summary>
        /// Encodes an integer as a Bitcoin variable-length integer (varint).
        /// </summary>
        private static byte[] EncodeVarInt(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be non-negative");
            }

            if (value < 0xFD)
            {
                return new byte[] { (byte)value };
            }
            else if (value <= 0xFFFF)
            {
                return new byte[] 
                { 
                    0xFD, 
                    (byte)(value & 0xFF), 
                    (byte)((value >> 8) & 0xFF) 
                };
            }
            else if (value <= 0xFFFFFFFF)
            {
                return new byte[] 
                { 
                    0xFE,
                    (byte)(value & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 24) & 0xFF)
                };
            }
            else
            {
                return new byte[] 
                { 
                    0xFF,
                    (byte)(value & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 32) & 0xFF),
                    (byte)((value >> 40) & 0xFF),
                    (byte)((value >> 48) & 0xFF),
                    (byte)((value >> 56) & 0xFF)
                };
            }
        }
    }
}
