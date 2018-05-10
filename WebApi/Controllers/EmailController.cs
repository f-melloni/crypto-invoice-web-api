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
    [Route("api/[controller]/[action]")]
    public class EmailController : Controller
    {
        IConfiguration _configuration { get; }
        [Route("api/[controller]/sendFromQueue")]
        public IActionResult SendMailFromQueue() {
            EmailSender eSend = new EmailSender(_configuration);
            eSend.SendQueuedEmails();
            return Ok();
        }

    }
}
