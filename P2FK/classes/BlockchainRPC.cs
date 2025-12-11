using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SUP.RPCClient
{
    //Courtesy mb300sd Bitcoin.NET
	public partial class CoinRPC
	{
		protected Uri uri;

		protected NetworkCredential credentials;

		public CoinRPC(Uri _Uri, NetworkCredential _Credentials)
		{
			uri = _Uri;
			credentials = _Credentials;
		}

		protected string HttpCall(string jsonRequest)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

			request.Method = "POST";
			request.ContentType = "application/json-rpc";

			// always send auth to avoid 401 response
			string auth = credentials.UserName + ":" + credentials.Password;
			auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth), Base64FormattingOptions.None);
			request.Headers.Add("Authorization", "Basic " + auth);

			//webRequest.Credentials = Credentials;

			request.ContentLength = jsonRequest.Length;


            try
            {
                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(jsonRequest);
                }
            }
            catch (WebException wex)
            {
             return "{400:\"" + wex.Message +"\"}";
            }
        

            try
            {
			
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (StreamReader sr = new StreamReader(response.GetResponseStream()))
				{
					return sr.ReadToEnd();
				}
			}
			catch (WebException wex)
			{
				using (HttpWebResponse response = (HttpWebResponse)wex.Response)
				using (StreamReader sr = new StreamReader(response.GetResponseStream()))
				{
					if (response.StatusCode != HttpStatusCode.InternalServerError)
					{
						throw;
					}
					return sr.ReadToEnd();
				}
			}
		}

		protected async Task<string> HttpCallAsync(string jsonRequest, CancellationToken cancellationToken = default)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

			request.Method = "POST";
			request.ContentType = "application/json-rpc";

			// always send auth to avoid 401 response
			string auth = credentials.UserName + ":" + credentials.Password;
			auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth), Base64FormattingOptions.None);
			request.Headers.Add("Authorization", "Basic " + auth);

			request.ContentLength = jsonRequest.Length;

            try
            {
                using (Stream requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                using (StreamWriter sw = new StreamWriter(requestStream))
                {
                    await sw.WriteAsync(jsonRequest).ConfigureAwait(false);
                }
            }
            catch (WebException wex)
            {
             return "{400:\"" + wex.Message +"\"}";
            }
        
			cancellationToken.ThrowIfCancellationRequested();

            try
            {
				using (WebResponse webResponse = await request.GetResponseAsync().ConfigureAwait(false))
				using (HttpWebResponse response = (HttpWebResponse)webResponse)
				using (Stream responseStream = response.GetResponseStream())
				using (StreamReader sr = new StreamReader(responseStream))
				{
					return await sr.ReadToEndAsync().ConfigureAwait(false);
				}
			}
			catch (WebException wex)
			{
				if (wex.Response == null) throw;
				
				using (HttpWebResponse response = (HttpWebResponse)wex.Response)
				using (Stream responseStream = response.GetResponseStream())
				using (StreamReader sr = new StreamReader(responseStream))
				{
					if (response.StatusCode != HttpStatusCode.InternalServerError)
					{
						throw;
					}
					return await sr.ReadToEndAsync().ConfigureAwait(false);
				}
			}
		}

		private T RpcCall<T>(RPCRequest rpcRequest)
		{
			string jsonRequest = JsonConvert.SerializeObject(rpcRequest);
            string result = HttpCall(jsonRequest);

			RPCResponse<T> rpcResponse = JsonConvert.DeserializeObject<RPCResponse<T>>(result);

			if (rpcResponse.error != null)
			{
				throw new CoinRPCException(rpcResponse.error);
			}
			return rpcResponse.result;
		}

		private async Task<T> RpcCallAsync<T>(RPCRequest rpcRequest, CancellationToken cancellationToken = default)
		{
			string jsonRequest = JsonConvert.SerializeObject(rpcRequest);
            string result = await HttpCallAsync(jsonRequest, cancellationToken).ConfigureAwait(false);

			RPCResponse<T> rpcResponse = JsonConvert.DeserializeObject<RPCResponse<T>>(result);

			if (rpcResponse.error != null)
			{
				throw new CoinRPCException(rpcResponse.error);
			}
			return rpcResponse.result;
		}
	}
}
