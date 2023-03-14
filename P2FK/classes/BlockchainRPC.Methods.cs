using System;
using System.Collections.Generic;
using System.Linq;

namespace BitcoinNET.RPCClient
{
    //Courtesy mb300sd Bitcoin.NET
	public partial class CoinRPC
	{
	  	    		
		public string SendMany( string FromAddress, IDictionary<string, decimal> ToBitcoinAddresses)
		{
			return RpcCall<string>
				(new RPCRequest("sendmany", new Object[] { FromAddress, ToBitcoinAddresses, 0 }));
		}     
       
		
	}
}
