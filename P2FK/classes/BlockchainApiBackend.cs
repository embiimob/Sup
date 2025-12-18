using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SUP.RPCClient;

namespace SUP.P2FK
{
    /// <summary>
    /// Bitcoin backend implementation using BlockCypher hosted API.
    /// BlockCypher provides free API access to Bitcoin mainnet and testnet3.
    /// API documentation: https://www.blockcypher.com/dev/bitcoin/
    /// 
    /// Note: BlockCypher free tier limits:
    /// - 200 requests/hour (no API key)
    /// - 3 requests/second
    /// Register for a free API key at https://accounts.blockcypher.com/ for higher limits.
    /// </summary>
    public class BlockchainApiBackend : IBitcoinBackend
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly string _network; // "main" or "test3"

        public BlockchainApiBackend(string baseUrl, string apiKey, string network)
        {
            _baseUrl = baseUrl?.TrimEnd('/') ?? "https://api.blockcypher.com/v1/btc";
            _apiKey = apiKey;
            _network = network ?? "test3";
        }

        private string MakeApiCall(string endpoint, string method = "GET", string body = null)
        {
            try
            {
                string url = $"{_baseUrl}/{_network}/{endpoint}";
                
                // Add API key if provided
                if (!string.IsNullOrEmpty(_apiKey))
                {
                    url += (url.Contains("?") ? "&" : "?") + $"token={_apiKey}";
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = "application/json";
                request.Timeout = 30000; // 30 seconds

                if (!string.IsNullOrEmpty(body) && method != "GET")
                {
                    byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                    request.ContentLength = bodyBytes.Length;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                string errorMessage = "Blockchain API Error: ";
                if (ex.Response != null)
                {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        errorMessage += reader.ReadToEnd();
                    }
                }
                else
                {
                    errorMessage += ex.Message;
                }
                throw new Exception(errorMessage, ex);
            }
        }

        private static long DateTimeToUnixTimeSeconds(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public GetRawDataTransactionResponse GetRawTransaction(string txId, int verbose = 0)
        {
            try
            {
                string endpoint = $"txs/{txId}?includeHex=true";
                string jsonResponse = MakeApiCall(endpoint);
                
                JObject txData = JObject.Parse(jsonResponse);
                
                var response = new GetRawDataTransactionResponse
                {
                    txid = txData["hash"]?.ToString(),
                    hex = txData["hex"]?.ToString(),
                    confirmations = txData["confirmations"]?.Value<int>() ?? 0,
                    blocktime = txData["confirmed"]?.Value<DateTime>() != null ? DateTimeToUnixTimeSeconds(txData["confirmed"].Value<DateTime>()) : 0,
                    blockhash = txData["block_hash"]?.ToString()
                };

                // Parse inputs
                if (txData["inputs"] != null)
                {
                    var inputs = new List<GetRawDataTransactionResponse.Input>();
                    foreach (var input in txData["inputs"])
                    {
                        inputs.Add(new GetRawDataTransactionResponse.Input
                        {
                            txid = input["prev_hash"]?.ToString(),
                            vout = input["output_index"]?.Value<int>() ?? 0,
                            scriptSig = new GetRawDataTransactionResponse.Input.ScriptSig
                            {
                                hex = input["script"]?.ToString()
                            }
                        });
                    }
                    response.vin = inputs.ToArray();
                }

                // Parse outputs
                if (txData["outputs"] != null)
                {
                    var outputs = new List<GetRawDataTransactionResponse.Output>();
                    int n = 0;
                    foreach (var output in txData["outputs"])
                    {
                        var addresses = output["addresses"]?.ToObject<string[]>();
                        outputs.Add(new GetRawDataTransactionResponse.Output
                        {
                            value = (output["value"]?.Value<long>() ?? 0) / 100000000m, // Satoshis to BTC
                            n = n++,
                            scriptPubKey = new GetRawDataTransactionResponse.Output.ScriptPubKey
                            {
                                hex = output["script"]?.ToString(),
                                addresses = addresses
                            }
                        });
                    }
                    response.vout = outputs.ToArray();
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get transaction {txId}: {ex.Message}", ex);
            }
        }

        public List<GetRawDataTransactionResponse> SearchRawTransactions(string address, int verbose = 0, int skip = 0, int returnQty = 100)
        {
            try
            {
                string endpoint = $"addrs/{address}/full?limit={returnQty}";
                if (skip > 0)
                {
                    endpoint += $"&before={skip}";
                }
                
                string jsonResponse = MakeApiCall(endpoint);
                JObject data = JObject.Parse(jsonResponse);
                
                var transactions = new List<GetRawDataTransactionResponse>();
                
                if (data["txs"] != null)
                {
                    foreach (var tx in data["txs"])
                    {
                        string txId = tx["hash"]?.ToString();
                        if (!string.IsNullOrEmpty(txId))
                        {
                            // For each transaction, we need the full details
                            try
                            {
                                var fullTx = GetRawTransaction(txId, verbose);
                                transactions.Add(fullTx);
                            }
                            catch
                            {
                                // Skip transactions that fail to load
                                continue;
                            }
                        }
                    }
                }
                
                return transactions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to search transactions for address {address}: {ex.Message}", ex);
            }
        }

        public string SendRawTransaction(string hexString)
        {
            try
            {
                string endpoint = "txs/push";
                var requestBody = new { tx = hexString };
                string json = JsonConvert.SerializeObject(requestBody);
                
                string jsonResponse = MakeApiCall(endpoint, "POST", json);
                JObject data = JObject.Parse(jsonResponse);
                
                return data["tx"]?["hash"]?.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send raw transaction: {ex.Message}", ex);
            }
        }

        public decimal GetBalance()
        {
            throw new NotSupportedException("GetBalance is not supported in API mode. This requires wallet access which is only available with local RPC.");
        }

        public string DumpPrivKey(string address)
        {
            throw new NotSupportedException("DumpPrivKey is not supported in API mode. Private keys are only accessible via local wallet RPC.");
        }

        public string SendMany(string fromAddress, IDictionary<string, decimal> toAddresses)
        {
            throw new NotSupportedException("SendMany is not supported in API mode. This requires wallet access which is only available with local RPC.");
        }

        public object SendCommand(string command, params object[] parameters)
        {
            throw new NotSupportedException($"Custom command '{command}' is not supported in API mode. API mode provides limited blockchain query functionality.");
        }

        public int GetBlockHeight(string blockHash)
        {
            try
            {
                string endpoint = $"blocks/{blockHash}";
                string jsonResponse = MakeApiCall(endpoint);
                JObject data = JObject.Parse(jsonResponse);
                
                return data["height"]?.Value<int>() ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get block height for {blockHash}: {ex.Message}", ex);
            }
        }
    }
}
