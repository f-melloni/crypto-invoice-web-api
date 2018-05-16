using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public static class RabbitMessages
    {
        public static void GetNewAddress(string currencyCode, int id)
        {
            string message = String.Format("{\"jsonrpc\": \"2.0\", \"method\": \"GetNewAddress\", \"params\": [], \"id\": {0}}",id);
            RabbitMessenger.Send(message,currencyCode);

        } 
    }
}
