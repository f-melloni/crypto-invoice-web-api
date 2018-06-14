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

        public int LowestDenomination { get { return 8; } }

        public void GetAddress(int invoiceId, string xpub)
        {
            RabbitMessages.GetNewAddress(_cc, invoiceId, xpub);
        }
        public abstract void GetAddress(int invoiceId, User loggedUser);

        public double GetExchangeRate(string fiatCurrencyCode)
        {
            string apiUrl = "";
            double price = 0;
            switch (fiatCurrencyCode)
            {
                case "USD":
                     apiUrl = $"https://min-api.cryptocompare.com/data/generateAvg?fsym={_cc}&tsym=USD&e=Poloniex,Kraken,Coinbase";
                     price = RestClient.GetResponse(apiUrl).ToObject<JObject>().GetValue("RAW").ToObject<JObject>().GetValue("PRICE").ToObject<Double>();
                    break;
                case "EUR":
                    apiUrl = $"https://min-api.cryptocompare.com/data/generateAvg?fsym={_cc}&tsym=EUR&e=Kraken,Coinbase";
                    price = RestClient.GetResponse(apiUrl).ToObject<JObject>().GetValue("RAW").ToObject<JObject>().GetValue("PRICE").ToObject<Double>();
                    break;
                case "CZK":
                    apiUrl = $"https://min-api.cryptocompare.com/data/generateAvg?fsym={_cc}&tsym=USD&e=Poloniex,Kraken,Coinbase";
                    var priceCC_USD = RestClient.GetResponse(apiUrl).ToObject<JObject>().GetValue("RAW").ToObject<JObject>().GetValue("PRICE").ToObject<Double>();

                    var apiUrlUsdCzk = $"http://free.currencyconverterapi.com/api/v5/convert?q=USD_CZK&compact=y";
                    var priceUsdCzk = RestClient.GetResponse(apiUrlUsdCzk).ToObject<JObject>().GetValue("USD_CZK").ToObject<JObject>().GetValue("val").ToObject<Double>();
                    price = priceCC_USD * priceUsdCzk;
                    break;
            }
           
            return price;
        }

        public string GetVarSymbol()
        {
            return "";
        }
    }
}
