using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        public int Status { get; set; }
        public string SmtpError { get; set; }



    }
}
