using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Entities
{
    public class Invoice
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string BTCAddress { get; set; }
        public string LTCAddress { get; set; }
        public string ETHVS { get; set; }
        public string XMRVS { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateReceived { get; set; }
        public User createdBy { get; set; }

        public int state { get; set; }
        public bool FixedRateOnCreation { get; set; }
        public string FiatCurrencyCode { get; set; }
        public string FiatAmount { get; set; }

        //ER = exchange rate
        public double? OldFixER_BTC { get; set; }
        public double? OldFixER_LTC { get; set; }
        public double? OldFixER_ETH { get; set; }
        public double? OldFixER_XMR { get; set; }

        public double? NewFixER_BTC { get; set; }
        public double? NewFixER_LTC { get; set; }
        public double? NewFixER_ETH { get; set; }
        public double? NewFixER_XMR { get; set; }
    }
}
