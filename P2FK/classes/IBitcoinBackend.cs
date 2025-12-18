using System;
using System.Collections.Generic;
using SUP.RPCClient;

namespace SUP.P2FK
{
    /// <summary>
    /// Interface for Bitcoin blockchain backend operations.
    /// Supports both local RPC and hosted API implementations.
    /// </summary>
    public interface IBitcoinBackend
    {
        /// <summary>
        /// Gets raw transaction data by transaction ID.
        /// </summary>
        /// <param name="txId">Transaction ID</param>
        /// <param name="verbose">Verbosity level (0 = hex, 1 = JSON)</param>
        /// <returns>Transaction response</returns>
        GetRawDataTransactionResponse GetRawTransaction(string txId, int verbose = 0);

        /// <summary>
        /// Searches for raw transactions by address.
        /// </summary>
        /// <param name="address">Bitcoin address</param>
        /// <param name="verbose">Verbosity level</param>
        /// <param name="skip">Number of results to skip</param>
        /// <param name="returnQty">Number of results to return</param>
        /// <returns>List of transaction responses</returns>
        List<GetRawDataTransactionResponse> SearchRawTransactions(string address, int verbose = 0, int skip = 0, int returnQty = 100);

        /// <summary>
        /// Sends a raw transaction to the network.
        /// </summary>
        /// <param name="hexString">Hex-encoded raw transaction</param>
        /// <returns>Transaction ID</returns>
        string SendRawTransaction(string hexString);

        /// <summary>
        /// Gets balance for the wallet or address.
        /// </summary>
        /// <returns>Balance</returns>
        decimal GetBalance();

        /// <summary>
        /// Dumps private key for an address.
        /// </summary>
        /// <param name="address">Bitcoin address</param>
        /// <returns>Private key</returns>
        string DumpPrivKey(string address);

        /// <summary>
        /// Sends to many addresses.
        /// </summary>
        /// <param name="fromAddress">Source address</param>
        /// <param name="toAddresses">Dictionary of destination addresses and amounts</param>
        /// <returns>Transaction ID</returns>
        string SendMany(string fromAddress, IDictionary<string, decimal> toAddresses);

        /// <summary>
        /// Sends a command to the backend.
        /// </summary>
        /// <param name="command">Command name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Response object</returns>
        object SendCommand(string command, params object[] parameters);

        /// <summary>
        /// Gets block height for a block hash.
        /// </summary>
        /// <param name="blockHash">Block hash</param>
        /// <returns>Block height</returns>
        int GetBlockHeight(string blockHash);
    }
}
