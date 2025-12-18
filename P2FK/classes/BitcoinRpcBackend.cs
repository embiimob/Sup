using System;
using System.Collections.Generic;
using System.Net;
using SUP.RPCClient;
using NBitcoin.RPC;

namespace SUP.P2FK
{
    /// <summary>
    /// Bitcoin backend implementation using local RPC (Bitcoin Core).
    /// This is the existing implementation pattern used throughout the application.
    /// </summary>
    public class BitcoinRpcBackend : IBitcoinBackend
    {
        private readonly CoinRPC _coinRpc;
        private readonly RPCClient _nbitcoinRpc;
        private readonly string _url;
        private readonly NetworkCredential _credentials;

        public BitcoinRpcBackend(string url, string username, string password)
        {
            _url = url;
            _credentials = new NetworkCredential(username, password);
            _coinRpc = new CoinRPC(new Uri(url), _credentials);
            _nbitcoinRpc = new RPCClient(_credentials, new Uri(url), NBitcoin.Network.Main);
        }

        public GetRawDataTransactionResponse GetRawTransaction(string txId, int verbose = 0)
        {
            return _coinRpc.GetRawDataTransaction(txId, verbose);
        }

        public List<GetRawDataTransactionResponse> SearchRawTransactions(string address, int verbose = 0, int skip = 0, int returnQty = 100)
        {
            return _coinRpc.SearchRawDataTransaction(address, verbose, skip, returnQty);
        }

        public string SendRawTransaction(string hexString)
        {
            return _nbitcoinRpc.SendRawTransaction(NBitcoin.Transaction.Parse(hexString, NBitcoin.Network.Main)).ToString();
        }

        public decimal GetBalance()
        {
            return _nbitcoinRpc.GetBalance().ToDecimal(NBitcoin.MoneyUnit.BTC);
        }

        public string DumpPrivKey(string address)
        {
            return _coinRpc.DumpPrivKey(address);
        }

        public string SendMany(string fromAddress, IDictionary<string, decimal> toAddresses)
        {
            return _coinRpc.SendMany(fromAddress, toAddresses);
        }

        public object SendCommand(string command, params object[] parameters)
        {
            return _nbitcoinRpc.SendCommand(command, parameters).Result;
        }

        public int GetBlockHeight(string blockHash)
        {
            dynamic blockObject = _nbitcoinRpc.SendCommand("getblock", blockHash).Result;
            if (blockObject != null)
            {
                return (int)blockObject.height;
            }
            return 0;
        }
    }
}
