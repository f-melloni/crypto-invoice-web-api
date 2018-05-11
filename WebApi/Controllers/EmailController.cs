using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Database.Entities;
using WebApi.Models.UserSettingsAjaxModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebApi.Database;
using WebApi.Services;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    public class EmailController : Controller
    {
        IConfiguration _configuration { get; set; }
        public EmailController(IConfiguration co)
        {
            _configuration = co;
        }

        [Route("api/[controller]/sendFromQueue")]
        public IActionResult SendMailFromQueue() {
            EmailSender eSend = new EmailSender(_configuration);
            eSend.SendQueuedEmails();
            return Ok();
        }

        [Route("api/[controller]/TestSendMail")]
        public IActionResult TestSendMail()
        {
            EmailSender eSend = new EmailSender(_configuration);

            //createEmailEntity
            Email email = eSend.CreateEmailEntity("binh.vu4571@gmail.com", "princecz904571@gmail.com", "Ahoj", "Zprava", "");
            eSend.AddEmailToQueue(email);

            eSend.SendQueuedEmails();
            return Ok();
        }
    }
}
