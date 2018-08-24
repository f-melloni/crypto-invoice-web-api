using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Services;

namespace WebApi.Database.Entities
{
    public enum InvoiceState
    {
        NOT_PAID = 1,
        TRANSACTION_SEEN = 2,
        TRANSACTION_CONFIRMED = 3
    }

    public class Invoice
    {
        private string _name;
        private string _recipient;
        private string _fiatCurrCode;
        private string _exchangeRateMode;
        private string _description;

        [Key]
        [Required]
        public int Id { get; set; }

        public Guid InvoiceGuid { get; set; }

        [Required]
        public string Name
        {
            get { return _name; }
            set { _name = HtmlUtil.CutHtml(value); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = HtmlUtil.CutHtml(value); }
        }
    
        /// <summary>
        /// EMail of invoice recipient
        /// </summary>
        [Required]
        public string Recipient
        {
            get { return _recipient; }
            set { _recipient = HtmlUtil.CutHtml(value); }
        }

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

        [Required]
        public string FiatCurrencyCode
        {
            get { return _fiatCurrCode; }
            set { _fiatCurrCode = HtmlUtil.CutHtml(value); }
        }
        
        [Required]
        public double FiatAmount { get; set; }

        public string ExchangeRateMode
        {
            get { return _exchangeRateMode; }
            set { _exchangeRateMode = HtmlUtil.CutHtml(value); }
        }
        public DateTime ExchangeRateSetTime { get; set; }

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
