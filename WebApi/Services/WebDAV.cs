using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using WebApi.Models.FileModels;

namespace WebApi.Services
{
    public class WebDAVClient
    {
        private readonly string hostName;
        private readonly int port;
        private readonly string baseDir;
        private readonly string authUserName;
        private readonly string authPassword;
        private readonly bool isAnonymous;

        public WebDAVClient(IHostingEnvironment env, IConfiguration configuration)
        {
            string key = env.IsDevelopment() ? "Development" : "Production";
            
            hostName = configuration[$"WebDav:{key}:Host"];
            port = int.Parse(configuration[$"WebDav:{key}:Port"]);
            baseDir = configuration[$"WebDav:{key}:BaseDir"];
            authUserName = configuration[$"WebDav:{key}:AuthUsername"];
            authPassword = configuration[$"WebDav:{key}:AuthPassword"];
            isAnonymous = string.IsNullOrEmpty(authUserName) && string.IsNullOrEmpty(authPassword);
        }

        private HttpWebRequest CreateWebRequest(FileData file, string method)
        {
            
            Uri webDavUri = new Uri(GetUrl(file));
            
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webDavUri);
            httpWebRequest.Method = method;
            if (!isAnonymous) {
                string authEncoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{authUserName}:{authPassword}"));
                httpWebRequest.Headers.Add("Authorization", $"Basic {authEncoded}");
            }

            return httpWebRequest;
        }

        private string GetUrl(FileData file)
        {
            return string.Format("{0}:{1}/{2}/{3}", hostName, port, baseDir, file.FileName);
        }

        public string UploadFile(FileData file)
        {
            var httpWebRequest = CreateWebRequest(file, WebRequestMethods.Http.Put);

            httpWebRequest.ContentLength = file.FileContent.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(file.FileContent, 0, file.FileContent.Length);
            httpWebRequest.GetResponse();

            return GetUrl(file);
        }

        public void DeleteFile(FileData file)
        {
            HttpWebRequest httpWebRequest = this.CreateWebRequest(file, "DELETE");
            httpWebRequest.GetResponse();
        }

        public static byte[] GetFile(string Url)
        {
            var webRequest = WebRequest.Create(Url);
            
            using (var response = webRequest.GetResponse()) {
                using (var contentStream = response.GetResponseStream()) {
                    using(MemoryStream ms = new MemoryStream()) {
                        contentStream.CopyTo(ms);
                        return ms.ToArray();            
                    }
                }
            }
        }
    }
}
