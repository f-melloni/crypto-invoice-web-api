using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Database;
using WebApi.Database.Entities;
using WebApi.Models.RabbitMessageModels;

namespace WebApi.Services
{
    public static class RabbitMessages
    {
        public static void GetNewAddress(string currencyCode, int invoice_id,string user_xpub)
        {
            string message = $@"{{""jsonrpc"": ""2.0"", ""method"": ""GetNewAddress"", ""params"": {{""InvoiceID"":{invoice_id},""XPUB"":""{user_xpub}"" }} }}";
            RabbitMessenger.Send(message,currencyCode);
        } 

        public static void OnSetAddress(JToken jsonParams)
        {
            SetAddressModel model = jsonParams.ToObject<SetAddressModel>();
            
            using (DBEntities dbe = new DBEntities()) {
                Invoice invoice = dbe.Invoices.Include("PaymentsAvailable").SingleOrDefault(i => i.Id == model.InvoiceId);
                if (invoice != null) {
                    InvoicePayment payment = invoice.PaymentsAvailable.SingleOrDefault(p => p.CurrencyCode == model.CurrencyCode);
                    if (payment != null) {
                        payment.Address = model.Address;
                        dbe.SaveChanges();
                    }
                }
            }
        }

        public static void OnTransactionSeen(JToken jsonParams)
        {
            TransactionSeenModel model = jsonParams.ToObject<TransactionSeenModel>();
            
            //check if theres invoice with the same address and currencycode + amount in invoice is >= amount received
            using (DBEntities dbe = new DBEntities()) {
                InvoicePayment payment = dbe.InvoicePayment.Include("Invoice").SingleOrDefault(p => p.Address == model.Address && p.CurrencyCode == model.CurrencyCode);
                if(payment != null) {
                    double amountRequired = payment.Invoice.FiatAmount / (double)payment.ExchangeRate;
                    if (model.Amount >= amountRequired && payment.Invoice.State == (int)InvoiceState.NOT_PAID) {
                        payment.Invoice.State = (int)InvoiceState.TRANSACTION_SEEN;
                        dbe.SaveChanges();
                    }
                }
            }
        }

        public static void OnTransactionConfirmed(JToken jsonParams)
        {
            TransactionConfirmedModel model = jsonParams.ToObject<TransactionConfirmedModel>();
            using (DBEntities dbe = new DBEntities()) {
                InvoicePayment payment = dbe.InvoicePayment.Include("Invoice").SingleOrDefault(p => p.Address == model.Address && p.CurrencyCode == model.CurrencyCode);
                if (payment != null) {
                    double amountRequired = payment.Invoice.FiatAmount / (double)payment.ExchangeRate;
                    if (model.Amount >= amountRequired) {
                        payment.Invoice.State = (int)InvoiceState.TRANSACTION_CONFIRMED;
                        payment.Invoice.TransactionCurrencyCode = model.CurrencyCode;
                        payment.Invoice.TransactionId = model.TXID;

                        dbe.SaveChanges();
                    }
                }
            }
        }
    }
}
