using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Adapters.Interfaces;

namespace WebApi.Models
{
    public class CurrencyConfiguration
    {
        public List<CurrencyConfigurationItem> Supported { get; set; } = new List<CurrencyConfigurationItem>();
        public Dictionary<string, ICurrencyAdapter> Adapters { get; set; } = new Dictionary<string, ICurrencyAdapter>();
    }

    public class CurrencyConfigurationItem
    {
        public string CurrencyCode { get; set; }
        public string BlockExplorerUrl { get; set; }
        public string Color { get; set; }
    }
}
