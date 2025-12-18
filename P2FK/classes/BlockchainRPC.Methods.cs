using System;
using System.Collections.Generic;
using System.Linq;

namespace SUP.RPCClient
{
    //Courtesy mb300sd Bitcoin.NET
	public partial class CoinRPC
	{
	  	    		
		public string SendMany( string FromAddress, IDictionary<string, decimal> ToBitcoinAddresses)
		{
			return RpcCall<string>
				(new RPCRequest("sendmany", new Object[] { FromAddress, ToBitcoinAddresses, 0 }));
		}

        public string DumpPrivKey(string Address)
        {
            return RpcCall<string>
                (new RPCRequest("dumpprivkey", new Object[] { Address }));
        }

        public List<GetRawDataTransactionResponse> SearchRawDataTransaction(string Address, int Verbose = 0, int skip = 0, int returnQty = 100)
        {
            return RpcCall<List<GetRawDataTransactionResponse>>
                (new RPCRequest("searchrawtransactions", new Object[] { Address, Verbose, skip,returnQty }));
        }


        public GetRawDataTransactionResponse GetRawDataTransaction(string txID, int Verbose = 0)
        {
            return RpcCall<GetRawDataTransactionResponse>
                (new RPCRequest("getrawtransaction", new Object[] { txID, Verbose }));
        }

        /// <summary>
        /// Verifies a Bitcoin message signature locally without RPC calls.
        /// This method works offline and does not require Bitcoin Core to be running.
        /// </summary>
        /// <param name="address">The Bitcoin address that supposedly signed the message</param>
        /// <param name="signature">The base64-encoded compact signature</param>
        /// <param name="message">The message that was signed (can be hex or plain text)</param>
        /// <param name="isTestnet">Whether to verify against testnet3 addresses (default: true)</param>
        /// <returns>True if the signature is valid for the given address and message</returns>
        public bool VerifyMessage(string address, string signature, string message, bool isTestnet = true)
        {
            return SUP.P2FK.MessageSignatureVerifier.VerifyMessage(address, signature, message, isTestnet);
        }




    }
    public class GetRawDataTransactionResponse
    {
        public class Input
        {
            public class ScriptSig
            {
                public string asm;
                public string hex;
            }

            public string txid;
            public int vout;
            public ScriptSig scriptSig;
            public long sequence;
        }

        public class Output
        {
            public class ScriptPubKey
            {
                public string asm;
                public string hex;
                public int reqSigs;
                public string type;
                public string[] addresses;
            }

            public decimal value;
            public int n;
            public ScriptPubKey scriptPubKey;

        }

        public string hex;
        public string txid;
        public int size;
        public int version;
        public int locktime;
        public string data;
        public Input[] vin;
        public Output[] vout;
        public string blockhash;
        public int confirmations;
        public long blocktime;

        public static implicit operator GetRawDataTransactionResponse(String s)
        {
            return new GetRawDataTransactionResponse() { hex = s };
        }
    }
}
