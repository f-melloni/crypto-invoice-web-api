using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public static class RabbitMessages
    {
        public static void GetNewAddress(string currencyCode, int invoice_id,string user_xpub)
        {
            string message = $@"{{""jsonrpc"": ""2.0"", ""method"": ""GetNewAddress"", ""params"": {{""InvoiceID"":{invoice_id},""XPUB"":""{user_xpub}"" }} }}";
            RabbitMessenger.Send(message,currencyCode);

        } 
    }
}
