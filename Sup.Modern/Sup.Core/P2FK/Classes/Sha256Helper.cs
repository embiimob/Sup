using System.Security.Cryptography;

namespace Sup.Core.P2FK.Classes;

/// <summary>
/// SHA-256 hashing utilities using modern .NET cryptography
/// </summary>
public static class Sha256Helper
{
    public static byte[] Hash(byte[] data)
    {
        return SHA256.HashData(data);
    }

    public static byte[] DoubleHash(byte[] data)
    {
        return SHA256.HashData(SHA256.HashData(data));
    }

    public static byte[] DoubleHashCheckSum(byte[] data)
    {
        var hash = DoubleHash(data);
        var checksum = new byte[4];
        Buffer.BlockCopy(hash, 0, checksum, 0, 4);
        return checksum;
    }

    public static byte[] Hash(string hexData)
    {
        byte[] bytes = Convert.FromHexString(hexData);
        return Hash(bytes);
    }

    public static byte[] DoubleHash(string hexData)
    {
        byte[] bytes = Convert.FromHexString(hexData);
        return DoubleHash(bytes);
    }

    public static string HashToHex(byte[] data)
    {
        return Convert.ToHexString(Hash(data)).ToLowerInvariant();
    }

    public static string DoubleHashToHex(byte[] data)
    {
        return Convert.ToHexString(DoubleHash(data)).ToLowerInvariant();
    }
}
