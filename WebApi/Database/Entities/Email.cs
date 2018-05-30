using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Services;

namespace WebApi.Database.Entities
{
    public class Email
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string Content { get; set; }
        public string AttachmenList { get; set; }
        public DateTime DateInserted { get; set; }
        public MailStatus Status { get; set; }
        public string SmtpError { get; set; }



    }
}
