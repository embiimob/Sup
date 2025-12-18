using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net;

namespace SUP.P2FK
{
    /// <summary>
    /// Provides Bitcoin message signature verification.
    /// 
    /// For P2FK verification:
    /// 1. The P2FK message data packet is hashed (SHA-256) to create a hash
    /// 2. The hash (as hex string) is signed using Bitcoin message signing
    /// 3. This class verifies that the signature is valid for that hash
    /// 
    /// Implementation note:
    /// Due to limitations in NBitcoin 9.0.4 (lacks public key recovery methods),
    /// this implementation uses RPC verifymessage when available, which works
    /// in both RPC and API modes since it's a local verification operation.
    /// </summary>
    public static class BitcoinMessageSigner
    {
        /// <summary>
        /// Verifies a Bitcoin message signature using RPC verifymessage command.
        /// 
        /// For P2FK: The 'message' parameter is the SHA-256 hash (hex string) of the 
        /// P2FK data packet. This hash was signed, and we verify the signature is valid.
        /// 
        /// Note: This uses RPC but doesn't require wallet access - verifymessage is a 
        /// utility RPC command that only needs the address, signature, and message.
        /// It works with watch-only addresses and doesn't need the private key.
        /// </summary>
        /// <param name="address">The Bitcoin address that allegedly signed the message</param>
        /// <param name="signature">The base64-encoded signature</param>
        /// <param name="message">The message that was signed (for P2FK: the hex hash of the data packet)</param>
        /// <param name="rpcUrl">The RPC URL to use for verification</param>
        /// <param name="username">RPC username</param>
        /// <param name="password">RPC password</param>
        /// <returns>True if the signature is valid for the given address and message</returns>
        public static bool VerifyMessage(string address, string signature, string message, string rpcUrl, string username, string password)
        {
            try
            {
                // Use RPC verifymessage - this is a utility command that doesn't require wallet access
                // It only needs the address (public), signature, and message to verify
                NetworkCredential credentials = new NetworkCredential(username, password);
                NBitcoin.RPC.RPCClient rpcClient = new NBitcoin.RPC.RPCClient(credentials, new Uri(rpcUrl), NBitcoin.Network.Main);
                
                try
                {
                    string result = rpcClient.SendCommand(
                        "verifymessage",
                        address,
                        signature,
                        message
                    ).ResultString;
                    return Convert.ToBoolean(result);
                }
                catch
                {
                    // If RPC call fails, return false
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
