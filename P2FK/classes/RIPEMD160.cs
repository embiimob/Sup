using NBitcoin.Crypto;

namespace SUP.P2FK
{
    public class RIPEMD160
    {
        public static byte[] Hash(byte[] data)
        {
            return Hashes.RIPEMD160(data);
        }

        public static byte[] Hash(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return Hash(bytes);
        }
    }
}
