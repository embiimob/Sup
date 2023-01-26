namespace SUP.P2FK
{
    public class Hash160
    {
        public static byte[] Hash(byte[] data)
        {
            return RIPEMD160.Hash(SHA256.Hash(data));
        }

        public static byte[] Hash(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return Hash(bytes);
        }
    }
}
