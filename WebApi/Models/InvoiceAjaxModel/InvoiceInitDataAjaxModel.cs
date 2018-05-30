using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WebApi.Models.InvoiceAjaxModel
{
    public class InvoiceInitDataAjaxModel
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public List<JObject> InvoiceList {get; set; }
        public List<CurrencyConfigurationItem> SupportCurrencies { get; set; } = new List<CurrencyConfigurationItem>();
    }
    public class InvoiceInitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public int Status { get; set; }
    }
}
