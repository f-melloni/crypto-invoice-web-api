using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using WebApi.Database;
using WebApi.Database.Entities;
using WebApi.Models.EmailModels;

namespace WebApi.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private IConfiguration _configuration { get; set; }
        
        public EmailSender(IConfiguration conf)
        {
            _configuration = conf;
        }

        public void AddEmailToQueue(Email emailEntity)
        {
            try {
                //add to DB to send later

                using (DBEntities dbe = new DBEntities()) {
                    dbe.Emails.Add(emailEntity);
                    dbe.SaveChanges();
                }
            }
            catch (Exception ex) {

            }
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }

        public Email CreateEmailEntity(string From, string To, string Body, string Subject, string AttachmentList)
        {
            try {
                //create MailMessage obj
                MailMessageJsonModel mailMessage = new MailMessageJsonModel(From, To, Body, Subject);
                //serialize MailMessage to JSON object
                string mailMessageJson = JsonConvert.SerializeObject(mailMessage);

                //create Email object
                Email email = new Email();
                email.AttachmenList = AttachmentList;
                email.Content = mailMessageJson;
                email.DateInserted = DateTime.UtcNow;
                email.SmtpError = "";
                email.Status = 0;

                return email;
            }
            catch (Exception ex) {
                return null;
            }
        }

        public void SendQueuedEmails()
        {
            //iterate emails in our queue(database) and send them
            using (DBEntities dbe = new DBEntities()) {
                List<Email> emailQueue = dbe.Emails.ToList();

                SmtpClient client = new SmtpClient(_configuration["SmtpServer:Host"], Convert.ToInt32(_configuration["SmtpServer:Port"]));
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_configuration["SmtpServer:Username"], _configuration["SmtpServer:Password"]);

                foreach (Email email in emailQueue) {
                    try {
                        //deserialize json string to onj
                        MailMessageJsonModel mailMessJsonModel = JsonConvert.DeserializeObject<MailMessageJsonModel>(email.Content);
                        MailMessage mailMess = new MailMessage(mailMessJsonModel.From, mailMessJsonModel.To, mailMessJsonModel.Subject, mailMessJsonModel.Body) {
                            IsBodyHtml = true
                        };

                        //check if there's attachment/s
                        if (email.AttachmenList != "" && email.AttachmenList.Split(',').Count() > 0) {
                            foreach (var item in email.AttachmenList.Split(',')) {
                                string[] info = item.Split('|');
                                byte[] fileContent = WebDAVClient.GetFile(info[0]);

                                MemoryStream ms = new MemoryStream(fileContent);

                                Attachment attachment = new Attachment(ms, info[1]) {
                                    ContentType = new ContentType(info[2])
                                };

                                mailMess.Attachments.Add(attachment);
                            }
                        }

                        client.Send(mailMess);

                        //delete this email from our queue
                        dbe.Emails.Remove(email);
                        dbe.SaveChanges(); //removed from queue
                    }
                    catch (Exception ex) {
                        //if sending failed
                        email.SmtpError = ex.Message;
                        email.Status = 1; //0 is in queue,1 is failed
                        dbe.Emails.Update(email);
                        dbe.SaveChanges();
                    }
                }
            }
        }
    }
}
