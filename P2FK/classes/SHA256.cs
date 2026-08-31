using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SUP.P2FK
{
    public class SHA256
    {
        public static byte[] Hash(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input data cannot be null");
            }

            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        public static byte[] DoubleHash(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Input data cannot be null");
            }

            // Create fresh instances for each hash to ensure thread-safety and clarity
            byte[] firstHash;
            using (var sha1 = System.Security.Cryptography.SHA256.Create())
            {
                firstHash = sha1.ComputeHash(data);
            }
            
            using (var sha2 = System.Security.Cryptography.SHA256.Create())
            {
                return sha2.ComputeHash(firstHash);
            }
        }

        public static byte[] DoubleHashCheckSum(byte[] data)
        {
            byte[] checksum = DoubleHash(data);
            Array.Resize(ref checksum, 4);
            return checksum;
        }

        public static byte[] Hash(string hexData)
        {
            byte[] bytes = HexToBytes(hexData);
            return Hash(bytes);
        }

        public static byte[] DoubleHash(string hexData)
        {
            byte[] bytes = HexToBytes(hexData);
            return DoubleHash(bytes);
        }

        public static byte[] DoubleHashCheckSum(string hexData)
        {
            byte[] bytes = HexToBytes(hexData);
            return DoubleHashCheckSum(bytes);
        }

        public static byte[] HexToBytes(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException(nameof(hex), "Input hex cannot be null");
            }

            if (hex.Length % 2 != 0)
            {
                hex = "0" + hex;
            }

            return Convert.FromHexString(hex);
        }
    }
}
