using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public static class WebDAV
    {
        public static Attachment GetFile(string Url,string type)
        {
            var webRequest = WebRequest.Create(Url);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            {
                Uri uri = new Uri(Url);
                Attachment attachment = new Attachment(content,System.IO.Path.GetFileName(uri.LocalPath));
                attachment.ContentType = new ContentType(type);
                return attachment;
            }      
        }
    }
}
