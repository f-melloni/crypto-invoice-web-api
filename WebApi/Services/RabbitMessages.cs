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
using WebApi.Adapters;

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
            using (DBEntities dbe = new DBEntities())
            {
                InvoicePayment payment = dbe.InvoicePayment.Include("Invoice").SingleOrDefault(p => p.Address == model.Address && p.CurrencyCode == model.CurrencyCode);
                if (payment != null)
                {
                    double amountRequired = GetAmountRequired(payment.Invoice.FiatAmount, (double)payment.ExchangeRate, model.CurrencyCode);
                    if (payment.Invoice.State == (int)InvoiceState.NOT_PAID)
                    {
                        if (model.Amount >= amountRequired)
                        {
                            payment.Invoice.State = (int)InvoiceState.TRANSACTION_SEEN;
                            payment.Invoice.TransactionId = model.TXID;
                            dbe.SaveChanges();
                        }
                        else
                        {
                            var transactionTime = DateTimeOffset.FromUnixTimeSeconds(model.Timestamp).UtcDateTime;

                            // Special case: if the transaction is less that 3 minutes late, previous exchange rate is still alowed
                            if (payment.Invoice.ExchangeRateSetTime != null && transactionTime.Subtract(payment.Invoice.ExchangeRateSetTime.Value).TotalMinutes < 3)
                            {
                                double previousAmountRequired = GetAmountRequired(payment.Invoice.FiatAmount, (double)payment.PreviousExchangeRate, model.CurrencyCode);
                                if (model.Amount >= previousAmountRequired)
                                {
                                    payment.Invoice.State = (int)InvoiceState.TRANSACTION_SEEN;
                                    payment.Invoice.TransactionId = model.TXID;
                                    dbe.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void OnTransactionConfirmed(JToken jsonParams)
        {
            TransactionConfirmedModel model = jsonParams.ToObject<TransactionConfirmedModel>();
            using (DBEntities dbe = new DBEntities())
            {
                InvoicePayment payment = dbe.InvoicePayment.Include("Invoice").Include("Invoice.CreatedBy").SingleOrDefault(p => p.Address == model.Address && p.CurrencyCode == model.CurrencyCode);
                if (payment != null)
                {
                    double amountRequired = GetAmountRequired(payment.Invoice.FiatAmount, (double)payment.ExchangeRate, model.CurrencyCode);

                    if (model.Amount >= amountRequired)
                    {
                        payment.Invoice.State = (int)InvoiceState.TRANSACTION_CONFIRMED;
                        payment.Invoice.TransactionId = model.TXID;
                        payment.Invoice.DateReceived = DateTime.Now;
                        dbe.SaveChanges();

                        // send mail
                        EmailManager.SendMailToPaymentReciever(payment.Invoice, model);
                        EmailManager.SendMailToPaymentSender(payment.Invoice, model);
                    }
                    else
                    {
                        var transactionTime = DateTimeOffset.FromUnixTimeSeconds(model.Timestamp).UtcDateTime;

                        // Special case: if the transaction is less that 3 minutes late, previous exchange rate is still alowed
                        if (payment.Invoice.ExchangeRateSetTime != null && transactionTime.Subtract(payment.Invoice.ExchangeRateSetTime.Value).TotalMinutes < 3)
                        {
                            double previousAmountRequired = GetAmountRequired(payment.Invoice.FiatAmount, (double)payment.PreviousExchangeRate, model.CurrencyCode);
                            if (model.Amount >= previousAmountRequired)
                            {
                                payment.Invoice.State = (int)InvoiceState.TRANSACTION_SEEN;
                                payment.Invoice.TransactionId = model.TXID;
                                dbe.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        private static double GetAmountRequired(double fiatAmount, double exchangeRate, string currencyCode)
        {
            int lowestDenomination = CurrencyUtils.GetLowestDenomination(currencyCode.ToUpper());
            return Math.Round(fiatAmount / exchangeRate, lowestDenomination);
        }
    }
}
