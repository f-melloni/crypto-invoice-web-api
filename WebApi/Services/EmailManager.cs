using Microsoft.Extensions.Configuration;
using WebApi.Database.Entities;
using WebApi.Models.RabbitMessageModels;

namespace WebApi.Services
{
    public static class EmailManager
    {
        public static void Setup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static IConfiguration _configuration { get; set; }

        public static void SendMailToPaymentReciever(Invoice invoice, TransactionConfirmedModel confirmation)
        {
            string url = $"https://live.blockcypher.com/{confirmation.CurrencyCode.ToLower()}/tx/{confirmation.TXID}";

            string subject = $"CryptoMail - incoming payment from {invoice.Recipient} confirmed";
            string body = System.IO.File.ReadAllText("wwwroot/web-api-static/templates/email/paymentConfirmationReciever.html");
            body = body.Replace("{Title}", subject)
                        .Replace("{Sender}", invoice.Recipient)
                        .Replace("{Invoice}", invoice.Name)
                        .Replace("{AmountFiat}", invoice.FiatAmount.ToString())
                        .Replace("{CurrencyFiat}", invoice.FiatCurrencyCode)
                        .Replace("{AmountCrypto}", confirmation.Amount.ToString())
                        .Replace("{CurrencyCrypto}", confirmation.CurrencyCode)
                        .Replace("{TargetAddressCrypto}", confirmation.Address)
                        .Replace("{TransactionTimestamp}", invoice.DateReceived.ToString())
                        .Replace("{BlockchainUrl}", $"<a href=\"{url}\">{url}</a>");
            string attachment = !string.IsNullOrEmpty(invoice.File) ? $"{invoice.File}|{invoice.FileName}|{invoice.FileMime}" : "";

            EmailSender sender = new EmailSender(_configuration);
            Email email = sender.CreateEmailEntity("info@octupus.com", invoice.CreatedBy.Email, body, subject, attachment);
            sender.AddEmailToQueue(email);
        }

        public static void SendMailToPaymentSender(Invoice invoice, TransactionConfirmedModel confirmation)
        {
            string url = $"https://live.blockcypher.com/{confirmation.CurrencyCode.ToLower()}/tx/{confirmation.TXID}";

            string subject = $"CryptoMail - outcoming payment to {invoice.CreatedBy.Email} confirmed";
            string body = System.IO.File.ReadAllText("wwwroot/web-api-static/templates/email/paymentConfirmationSender.html");
            body = body.Replace("{Title}", subject)
                        .Replace("{Reciever}", invoice.CreatedBy.Email)
                        .Replace("{Invoice}", invoice.Name)
                        .Replace("{AmountFiat}", invoice.FiatAmount.ToString())
                        .Replace("{CurrencyFiat}", invoice.FiatCurrencyCode)
                        .Replace("{AmountCrypto}", confirmation.Amount.ToString())
                        .Replace("{CurrencyCrypto}", confirmation.CurrencyCode)
                        .Replace("{TargetAddressCrypto}", confirmation.Address)
                        .Replace("{TransactionTimestamp}", invoice.DateReceived.ToString())
                        .Replace("{BlockchainUrl}", $"<a href=\"{url}\">{url}</a>");
            string attachment = !string.IsNullOrEmpty(invoice.File) ? $"{invoice.File}|{invoice.FileName}|{invoice.FileMime}" : "";

            EmailSender sender = new EmailSender(_configuration);
            Email email = sender.CreateEmailEntity("info@octupus.com", invoice.Recipient, body, subject, attachment);
            sender.AddEmailToQueue(email);
        }
    }
}
