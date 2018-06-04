using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.RabbitMessageModels
{
    public class TransactionSeenModel
    {
        private string _CC;

        public string CurrencyCode
        {
            get {
                return _CC;
            }
            set {
                _CC = value.ToUpper();
            }
        }
        public string Address { get; set; }
        public double Amount { get; set; }
        public string TXID { get; set; }
    }
}
