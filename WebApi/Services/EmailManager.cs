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
            string subject = $"CryptoMail - incoming payment from {invoice.Recipient}";
            string body = System.IO.File.ReadAllText("wwwroot/web-api-static/templates/email/paymentConfirmationReciever.html");
            body = body.Replace("{Title}", subject)
                        .Replace("{Sender}", invoice.Recipient)
                        .Replace("{Invoice}", invoice.Name)
                        .Replace("{AmountFiat}", invoice.FiatAmount.ToString())
                        .Replace("{CurrencyFiat}", invoice.FiatCurrencyCode)
                        .Replace("{AmountCrypto}", confirmation.Amount.ToString())
                        .Replace("{CurrencyCrypto}", confirmation.CurrencyCode)
                        .Replace("{TargetAddressCrypto}", confirmation.Address)
                        .Replace("{TransactionTimestamp}", invoice.DateReceived.ToString());

            EmailSender sender = new EmailSender(_configuration);
            Email email = sender.CreateEmailEntity("info@octupus.com", invoice.CreatedBy.Email, body, subject, null);
            sender.AddEmailToQueue(email);
        }
    }
}
