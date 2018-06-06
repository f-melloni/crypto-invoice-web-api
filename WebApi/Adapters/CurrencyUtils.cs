using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Adapters.Interfaces;

namespace WebApi.Adapters
{
    public static class CurrencyUtils
    {
        private static Dictionary<string, Type> Adapters = new Dictionary<string, Type>()
        {
            { "BTC", typeof(BTCAdapter) },
            { "LTC", typeof(LTCAdapter) }
        };
        public static int GetLowestDenomination(string currencyCode)
        {
            ICurrencyAdapter adapter = (ICurrencyAdapter)Activator.CreateInstance(Adapters[currencyCode]);
            return adapter.LowestDenomination;
        }
    }
}
