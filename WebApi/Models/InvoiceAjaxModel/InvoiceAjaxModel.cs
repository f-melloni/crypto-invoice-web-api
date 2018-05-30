using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Database.Entities;

namespace WebApi.Models.InvoiceAjaxModel
{
    public class InvoiceAjaxModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Recipient { get; set; }

        public string FiatCurrencyCode { get; set; }
        public double FiatAmount { get; set; }

        public User CreatedBy { get; set; }

        // File
        public string File { get; set; }
        public string FileName { get; set; }
        public string FileMime { get; set; }

        // Payments enabled
        public ICollection<string> Accept { get; set; }

        public InvoiceAjaxModel()
        {
            Accept = new List<string>();
        }
    }
}
