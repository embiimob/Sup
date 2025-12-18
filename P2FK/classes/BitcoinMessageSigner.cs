using System;
using System.Linq;
using System.Text;
using System.Numerics;

namespace SUP.P2FK
{
    /// <summary>
    /// Provides client-side Bitcoin message signature verification.
    /// Implements complete ECDSA signature recovery using the project's Secp256k1 infrastructure.
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
    /// - Recovers the public key from the ECDSA signature using elliptic curve math
    /// - Derives the Bitcoin address from the recovered public key
    /// - Compares the derived address with the claimed signer address
    /// </summary>
    public static class BitcoinMessageSigner
    {
        /// <summary>
        /// Verifies a Bitcoin message signature using client-side ECDSA signature recovery.
        /// 
        /// For P2FK: The 'message' parameter is the SHA-256 hash (hex string) of the 
        /// P2FK data packet. This hash was signed, and we verify the signature is valid.
        /// </summary>
        /// <param name="address">The Bitcoin address that allegedly signed the message</param>
        /// <param name="signature">The base64-encoded signature</param>
        /// <param name="message">The message that was signed (for P2FK: the hex hash of the data packet)</param>
        /// <param name="isTestnet">Whether this is testnet (true) or mainnet (false)</param>
        /// <returns>True if the signature is valid for the given address and message</returns>
        public static bool VerifyMessage(string address, string signature, string message, bool isTestnet = false)
        {
            try
            {
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

                // Extract recovery flag and determine if compressed
                int recoveryFlag = signatureBytes[0] - 27;
                if (recoveryFlag < 0 || recoveryFlag > 7)
                {
                    return false;
                }
                
                bool isCompressed = (recoveryFlag & 4) != 0;
                int recoveryId = recoveryFlag & 3;
                
                // Extract r and s components (32 bytes each)
                byte[] rBytes = new byte[32];
                byte[] sBytes = new byte[32];
                Array.Copy(signatureBytes, 1, rBytes, 0, 32);
                Array.Copy(signatureBytes, 33, sBytes, 0, 32);
                
                BigInteger r = rBytes.ToBigIntegerUnsigned(true);
                BigInteger s = sBytes.ToBigIntegerUnsigned(true);

                // Format and hash the message according to Bitcoin message signing standard
                byte[] messageBytes = FormatMessageForSigning(message);
                byte[] messageHash = SUP.P2FK.SHA256.DoubleHash(messageBytes);
                BigInteger e = messageHash.ToBigIntegerUnsigned(true);

                // Recover the public key from the signature
                ECPoint recoveredPubKey = RecoverPublicKey(r, s, e, recoveryId, isCompressed);
                
                if (recoveredPubKey == null)
                {
                    return false;
                }

                // Get the Bitcoin address from the recovered public key
                string recoveredAddress = recoveredPubKey.GetBitcoinAddress(isCompressed);
                
                // Compare with the claimed address
                return recoveredAddress == address;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Recovers the public key from an ECDSA signature using elliptic curve mathematics.
        /// Implements the public key recovery algorithm for secp256k1.
        /// </summary>
        private static ECPoint RecoverPublicKey(BigInteger r, BigInteger s, BigInteger e, int recoveryId, bool isCompressed)
        {
            try
            {
                // The recovery id indicates which of the possible points to use
                // recoveryId = 0 or 1 for normal case, 2 or 3 if r + n is valid
                BigInteger i = BigInteger.Zero;
                if (recoveryId >= 2)
                {
                    i = BigInteger.One;
                }
                
                // Calculate x coordinate: x = r + i*n
                BigInteger x = r + (i * Secp256k1.N);
                
                // x must be less than the field prime
                if (x >= Secp256k1.P)
                {
                    return null;
                }

                // Recover the point R = (x, y) on the curve
                // y^2 = x^3 + 7 (mod p)
                BigInteger ySquared = (BigInteger.ModPow(x, 3, Secp256k1.P) + 7) % Secp256k1.P;
                BigInteger y = ySquared.ShanksSqrt(Secp256k1.P);
                
                if (y < 0)
                {
                    return null;
                }

                // Select the correct y based on recovery id
                bool isYEven = y.IsEven;
                bool shouldBeEven = (recoveryId & 1) == 0;
                
                if (isYEven != shouldBeEven)
                {
                    y = Secp256k1.P - y;
                }

                // Create point R
                ECPoint R = new ECPoint(x, y);
                
                // Calculate the public key: Q = r^-1 * (s*R - e*G)
                BigInteger rInv = r.ModInverse(Secp256k1.N);
                
                // Calculate s*R
                ECPoint sR = R.Multiply(s);
                
                // Calculate e*G
                ECPoint eG = Secp256k1.G.Multiply(e);
                
                // Calculate s*R - e*G (which is s*R + (-e)*G)
                BigInteger negE = (Secp256k1.N - e) % Secp256k1.N;
                ECPoint negEG = Secp256k1.G.Multiply(negE);
                ECPoint sRminusEG = sR.Add(negEG);
                
                // Calculate Q = r^-1 * (s*R - e*G)
                ECPoint Q = sRminusEG.Multiply(rInv);
                
                return Q;
            }
            catch
            {
                return null;
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
