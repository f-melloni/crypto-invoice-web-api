using Newtonsoft.Json.Linq;
using System;
using WebApi.Adapters.Interfaces;
using WebApi.Database.Entities;
using WebApi.Services;

namespace WebApi.Adapters
{
    public abstract class BTCBasedAdapter : ICurrencyAdapter
    {
        private string _cc;
        protected string CurrencyCode {
            get {
                return _cc;
            }
            set {
                _cc = value.ToUpper();
            }
        }

        public void GetAddress(int invoiceId, string xpub)
        {
            RabbitMessages.GetNewAddress(_cc, invoiceId, xpub);
        }
        public abstract void GetAddress(int invoiceId, User loggedUser);

        public double GetExchangeRate()
        {
            string apiUrl = $"https://min-api.cryptocompare.com/data/generateAvg?fsym={_cc}&tsym=USD&e=Poloniex,Kraken,Coinbase,HitBTC";
            var price = RestClient.GetResponse(apiUrl).ToObject<JObject>().GetValue("RAW").ToObject<JObject>().GetValue("PRICE").ToObject<Double>();
            return price;
        }

        public string GetVarSymbol()
        {
            return "";
        }
    }
}
