using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Database.Entities
{
    public class InvoicePayment
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string CurrencyCode { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(100)]
        public string VarSymbol { get; set; }

        public double? ExchangeRate { get; set; }
        public double? PreviousExchangeRate { get; set; }

        [Required]
        public virtual Invoice Invoice { get; set; }
    }
}
