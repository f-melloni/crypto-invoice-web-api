using System.Threading.Tasks;
using WebApi.Database.Entities;

namespace WebApi.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Email CreateEmailEntity(string From, string To, string Body, string Subject, string AttachmentList);
        void AddEmailToQueue(Email email);
        void SendQueuedEmails();
    }
}
