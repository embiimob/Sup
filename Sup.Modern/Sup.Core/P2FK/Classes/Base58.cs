using System.Numerics;

namespace Sup.Core.P2FK.Classes;

/// <summary>
/// Base58 encoding/decoding implementation
/// Based on CodesInChaos' public domain code
/// </summary>
public static class Base58
{
    private const int CheckSumSizeInBytes = 4;
    private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    public static string EncodeWithCheckSum(byte[] data)
    {
        return Encode(AddCheckSum(data));
    }

    public static byte[] DecodeWithCheckSum(string base58)
    {
        var dataWithCheckSum = Decode(base58);
        return RemoveCheckSum(dataWithCheckSum);
    }

    public static bool TryDecodeWithCheckSum(string base58, out byte[] decoded)
    {
        try
        {
            var dataWithCheckSum = Decode(base58);
            if (!VerifyCheckSum(dataWithCheckSum))
            {
                decoded = Array.Empty<byte>();
                return false;
            }
            decoded = RemoveCheckSum(dataWithCheckSum);
            return true;
        }
        catch
        {
            decoded = Array.Empty<byte>();
            return false;
        }
    }

    public static string Encode(byte[] data)
    {
        BigInteger intData = 0;
        for (int i = 0; i < data.Length; i++)
        {
            intData = intData * 256 + data[i];
        }

        var result = string.Empty;
        while (intData > 0)
        {
            int remainder = (int)(intData % 58);
            intData /= 58;
            result = Digits[remainder] + result;
        }

        // Append '1' for each leading 0 byte
        for (int i = 0; i < data.Length && data[i] == 0; i++)
        {
            result = '1' + result;
        }
        
        return result;
    }

    public static byte[] Decode(string base58)
    {
        BigInteger intData = 0;
        for (int i = 0; i < base58.Length; i++)
        {
            int digit = Digits.IndexOf(base58[i]);
            if (digit < 0)
            {
                throw new FormatException(
                    $"Invalid Base58 character '{base58[i]}' at position {i}");
            }
            intData = intData * 58 + digit;
        }

        int leadingZeroCount = base58.TakeWhile(c => c == '1').Count();
        var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
        var bytesWithoutLeadingZeros = intData
            .ToByteArray()
            .Reverse()
            .SkipWhile(b => b == 0);
        
        return leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
    }

    private static byte[] AddCheckSum(byte[] data)
    {
        byte[] checkSum = GetCheckSum(data);
        var result = new byte[data.Length + checkSum.Length];
        Buffer.BlockCopy(data, 0, result, 0, data.Length);
        Buffer.BlockCopy(checkSum, 0, result, data.Length, checkSum.Length);
        return result;
    }

    private static byte[] RemoveCheckSum(byte[] data)
    {
        byte[] result = new byte[data.Length - CheckSumSizeInBytes];
        Buffer.BlockCopy(data, 0, result, 0, result.Length);
        return result;
    }

    private static bool VerifyCheckSum(byte[] data)
    {
        byte[] dataWithoutChecksum = RemoveCheckSum(data);
        byte[] correctCheckSum = GetCheckSum(dataWithoutChecksum);
        
        for (int i = 0; i < CheckSumSizeInBytes; i++)
        {
            if (data[data.Length - CheckSumSizeInBytes + i] != correctCheckSum[i])
            {
                return false;
            }
        }
        return true;
    }

    private static byte[] GetCheckSum(byte[] data)
    {
        var hash = Sha256Helper.DoubleHash(data);
        var checksum = new byte[CheckSumSizeInBytes];
        Buffer.BlockCopy(hash, 0, checksum, 0, CheckSumSizeInBytes);
        return checksum;
    }
}
