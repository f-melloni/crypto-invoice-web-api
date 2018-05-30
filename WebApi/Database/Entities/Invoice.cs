using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Services;

namespace WebApi.Database.Entities
{
    public class Invoice
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public Guid InvoiceGuid { get; set; }
        [Required]
        private string _name;
        [Required]
        private string _recipient;
        [Required]
        private string _fiatCurrCode;
        [Required]
        public string Name
        {
            get { return this._name; }
            set { _name = HtmlUtil.CutHtml(value); }
        }

        public string Description { get; set; }
    
        /// <summary>
        /// EMail of invoice recipient
        /// </summary>
        [Required]
        public string Recipient
        {
            get { return this._recipient; }
            set { _recipient = HtmlUtil.CutHtml(value); }
        }

        public bool AcceptBTC { get; set; }
        public bool AcceptLTC { get; set; }
        public bool AcceptETH { get; set; }
        public bool AcceptXMR { get; set; }

        public string BTCAddress { get; set; }
        public string LTCAddress { get; set; }
        public string ETHVS { get; set; }
        public string XMRVS { get; set; }

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
        public User createdBy { get; set; }

        public int state { get; set; }
        [Required]
        public string FiatCurrencyCode
        {
            get { return this._fiatCurrCode; }
            set { _fiatCurrCode = HtmlUtil.CutHtml(value); }
        }

        [Required]
        public double FiatAmount { get; set; }

        // File
        public string File { get; set; }
        public string FileName { get; set; }
        public string FileMime { get; set; }

        //ER = exchange rate
     
        public double? NewFixER_BTC { get; set; }
        public double? NewFixER_LTC { get; set; }
        public double? NewFixER_ETH { get; set; }
        public double? NewFixER_XMR { get; set; }
    }
}
