using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.EmailModels
{
    public class MailMessageJsonModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }

        public MailMessageJsonModel(string from,string to,string body,string subject)
        { 
            From = from;
            To = to;
            Body = body;
            Subject = subject;
        }
    }


}
