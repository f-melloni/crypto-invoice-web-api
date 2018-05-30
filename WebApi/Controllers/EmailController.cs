using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.Services;

namespace WebApi.Controllers
{
    public class EmailController : Controller
    {
        IConfiguration _configuration { get; set; }
        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("api/[controller]/sendFromQueue")]
        public IActionResult SendMailFromQueue() {
            EmailSender sender = new EmailSender(_configuration);
            sender.SendQueuedEmails();
            return Ok();
        }
    }
}
