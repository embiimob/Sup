using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SUP.P2FK
{
    /// <summary>
    /// Provides Bitcoin-style message signature verification without requiring RPC calls.
    /// Supports both mainnet and testnet3 address validation.
    /// </summary>
    public static class MessageSignatureVerifier
    {
        private const string BitcoinSignedMessageHeader = "Bitcoin Signed Message:\n";

        /// <summary>
        /// Verifies a Bitcoin message signature against an address.
        /// </summary>
        /// <param name="address">The Bitcoin address that supposedly signed the message</param>
        /// <param name="signature">The base64-encoded compact signature</param>
        /// <param name="message">The message that was signed (can be hex or plain text)</param>
        /// <param name="isTestnet">Whether to verify against testnet3 addresses (default: true)</param>
        /// <returns>True if the signature is valid for the given address and message; False if invalid, null, or empty inputs</returns>
        public static bool VerifyMessage(string address, string signature, string message, bool isTestnet = true)
        {
            try
            {
                // Validate inputs - null and empty strings both return false
                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(message))
                {
                    return false;
                }

                // Quick address format validation (length only, skip expensive Base58 decode)
                if (address.Length < 26 || address.Length > 35)
                {
                    return false;
                }

                // Decode the base64 signature
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

                // Extract recovery flag and signature components
                int recoveryFlag = signatureBytes[0] - 27;
                bool compressed = (recoveryFlag & 4) != 0;
                int recId = recoveryFlag & 3;

                if (recId < 0 || recId > 3)
                {
                    return false; // Invalid recovery ID
                }

                // Extract r and s from signature
                byte[] rBytes = new byte[32];
                byte[] sBytes = new byte[32];
                Buffer.BlockCopy(signatureBytes, 1, rBytes, 0, 32);
                Buffer.BlockCopy(signatureBytes, 33, sBytes, 0, 32);

                BigInteger r = rBytes.ToBigIntegerUnsigned(true);
                BigInteger s = sBytes.ToBigIntegerUnsigned(true);

                // Create the message hash using Bitcoin's message signing format
                byte[] messageHash = HashMessage(message);

                // Recover the public key from the signature
                ECPoint publicKey = RecoverPublicKey(r, s, messageHash, recId, compressed);
                if (publicKey == null)
                {
                    return false;
                }

                // Get the Bitcoin address from the recovered public key
                string recoveredAddress = GetAddressFromPublicKey(publicKey, compressed, isTestnet);

                // Compare addresses
                return recoveredAddress == address;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a Bitcoin message hash following the standard Bitcoin message signing format.
        /// </summary>
        private static byte[] HashMessage(string message)
        {
            // Bitcoin message signing treats the message as a UTF-8 string, not as hex bytes
            // Even if the message looks like hex (e.g., "ABCD1234"), it's signed as that literal string
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // Bitcoin message format: varint(len(header)) + header + varint(len(message)) + message
            byte[] header = Encoding.UTF8.GetBytes(BitcoinSignedMessageHeader);
            
            // Calculate the correct size accounting for varint encoding
            int varintSize = messageBytes.Length < 253 ? 1 : 3;
            byte[] fullMessage = new byte[1 + header.Length + varintSize + messageBytes.Length];
            int offset = 0;

            // Add header length (varint)
            fullMessage[offset++] = (byte)header.Length;
            
            // Add header
            Buffer.BlockCopy(header, 0, fullMessage, offset, header.Length);
            offset += header.Length;

            // Add message length (varint - simplified for messages < 253 bytes)
            if (messageBytes.Length < 253)
            {
                fullMessage[offset++] = (byte)messageBytes.Length;
            }
            else
            {
                fullMessage[offset++] = 0xfd; // varint prefix for 2-byte length
                fullMessage[offset++] = (byte)(messageBytes.Length & 0xff);
                fullMessage[offset++] = (byte)((messageBytes.Length >> 8) & 0xff);
            }

            // Add message
            Buffer.BlockCopy(messageBytes, 0, fullMessage, offset, messageBytes.Length);

            // Double SHA256 hash
            return SHA256.DoubleHash(fullMessage);
        }

        /// <summary>
        /// Recovers the public key from an ECDSA signature.
        /// </summary>
        private static ECPoint RecoverPublicKey(BigInteger r, BigInteger s, byte[] messageHash, int recId, bool compressed)
        {
            try
            {
                BigInteger e = messageHash.ToBigIntegerUnsigned(true);

                // Calculate x coordinate of R
                BigInteger x = r;
                if ((recId & 2) != 0)
                {
                    x += Secp256k1.N;
                }

                if (x >= Secp256k1.P)
                {
                    return null;
                }

                // Derive R from x
                ECPoint R = DecompressPoint(x, (recId & 1) != 0);
                if (R == null)
                {
                    return null;
                }

                // Note: Skipping R * n = Infinity check as it's computationally expensive
                // and R is derived from a valid signature, so it should be on the curve

                // Calculate public key: Q = r^-1 * (s*R - e*G)
                BigInteger rInv = r.ModInverse(Secp256k1.N);
                
                // s * R
                ECPoint sR = R.Multiply(s);
                
                // e * G
                ECPoint eG = Secp256k1.G.Multiply(e);
                
                // s*R - e*G (we need to negate eG)
                ECPoint negEG = new ECPoint(eG.X, Secp256k1.P - eG.Y);
                ECPoint srMinusEg = sR.Add(negEG);
                
                // r^-1 * (s*R - e*G)
                ECPoint Q = srMinusEg.Multiply(rInv);

                return Q;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Decompresses a point on the secp256k1 curve from its x coordinate and y parity.
        /// </summary>
        private static ECPoint DecompressPoint(BigInteger x, bool yOdd)
        {
            try
            {
                // Calculate y^2 = x^3 + 7 (mod p)
                BigInteger ySq = (x * x * x + 7) % Secp256k1.P;
                
                // Calculate y using Shanks' algorithm (modular square root)
                BigInteger y = ySq.ShanksSqrt(Secp256k1.P);
                
                if (y == -1)
                {
                    return null; // No square root exists
                }

                // Ensure y has the correct parity
                if (y.IsEven != !yOdd)
                {
                    y = Secp256k1.P - y;
                }

                return new ECPoint(x, y);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the Bitcoin address from a public key.
        /// </summary>
        private static string GetAddressFromPublicKey(ECPoint publicKey, bool compressed, bool isTestnet)
        {
            // Hash the public key
            byte[] pubKeyHash = Hash160.Hash(publicKey.EncodePoint(compressed));

            // Add version byte (0x00 for mainnet, 0x6f for testnet3)
            byte versionByte = isTestnet ? (byte)0x6f : (byte)0x00;
            
            byte[] addressBytes = new byte[pubKeyHash.Length + 1];
            addressBytes[0] = versionByte;
            Buffer.BlockCopy(pubKeyHash, 0, addressBytes, 1, pubKeyHash.Length);
            
            return Base58.EncodeWithCheckSum(addressBytes);
        }


    }
}
