using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Entities
{
    public enum InvoiceState
    {
        NOT_PAID = 1,
    }

    public class Invoice
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public Guid InvoiceGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// EMail of invoice recipient
        /// </summary>
        [Required]
        public string Recipient { get; set; }

        /// <summary>
        /// Transaction ID of payment
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// In what crypto currency was the payment made
        /// </summary>
        public string TransactionCurrencyCode { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateReceived { get; set; }
        public User CreatedBy { get; set; }

        public int State { get; set; }
        public string FiatCurrencyCode { get; set; }
        public double FiatAmount { get; set; }

        // File
        public string File { get; set; }
        public string FileName { get; set; }
        public string FileMime { get; set; }

        public virtual ICollection<InvoicePayment> PaymentsAvailable { get; set; }

        public Invoice()
        {
            PaymentsAvailable = new List<InvoicePayment>();
        }
    }
}
