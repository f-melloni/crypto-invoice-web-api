using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApi.Database;
using WebApi.Database.Entities;

namespace WebApi.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public IConfiguration Configuration { get; }
        public SmtpClient client { get; set; }
        public EmailSender(IConfiguration configuration)
        {
            Configuration = configuration;
            //create client
            client = new SmtpClient(Configuration["SmtpServer:Host"], Convert.ToInt32(Configuration["SmtpServer:Port"]));
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(Configuration["SmtpServer:Username"], Configuration["SmtpServer:Password"]);
            
        }
        public void AddEmailToQueue(Email emailEntity )
        {
            try
            {
                //add to DB to send later

                using (DBEntities dbe = new DBEntities())
                {
                    dbe.Emails.Add(emailEntity);
                    dbe.SaveChanges();
                }
            }catch(Exception ex)
            {

            }
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }

        public void CreateEmailEntity(string From, string To, string Body, string Subject, string AttachmentList)
        {
            try
            {
                //create MailMessage obj
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(From);
                mailMessage.To.Add(To);
                mailMessage.Body = Body;
                mailMessage.Subject = Subject;

                //serialize MailMessage to JSON object
                string mailMessageJson = JsonConvert.SerializeObject(mailMessage);

                //create Email object
                Email email = new Email();
                email.AttachmenList = AttachmentList;
                email.Content = mailMessageJson;
                email.DateInserted = DateTime.UtcNow;
                email.SmtpError = "";
                email.Status = 0;

                
            }
            catch(Exception ex)
            {

            }
        }

        public void SendEmail(MailMessage mailMessage)
        {
            //iterate emails in our queue(database) and send them
            using (DBEntities dbe = new DBEntities())
            {
                List<Email> emailQueue = dbe.Emails.ToList();
                foreach (Email email in emailQueue)
                {
                    try
                    {
                        //deserialize json string to onj
                        MailMessage mailMess = (MailMessage)JsonConvert.DeserializeObject(email.Content);
                        client.Send(mailMess);

                        //delete this email from our queue
                        dbe.Emails.Remove(email);
                        dbe.SaveChanges(); //removed from queue
                    }
                    catch (Exception ex)
                    {
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
