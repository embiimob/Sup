using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SUP.P2FK
{
    public class SHA256
    {
        private static readonly Dictionary<string, byte> _hexToByte = new Dictionary<
            string,
            byte
        >();

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

            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var firstHash = sha.ComputeHash(data);
                var secondHash = sha.ComputeHash(firstHash);
                return secondHash;
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
            if (hex.Length % 2 != 0)
            {
                hex = "0" + hex;
            }

            hex = hex.ToLower();

            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++)
            {
                bytes[i] = _hexToByte[hex.Substring(i * 2, 2)];
            }

            return bytes;
        }
    }
}
