using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApi.Database.Entities;

namespace WebApi.Services
{
    public interface IEmailSender
    {

        Task SendEmailAsync(string email, string subject, string message);
        void CreateEmailEntity(string From, string To, string Body, string Subject, string AttachmentList);
        void AddEmailToQueue(Email email);
        void SendQueuedEmails();
    }


}
