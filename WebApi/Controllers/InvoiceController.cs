using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using WebApi.Database;
using WebApi.Database.Entities;
using WebApi.Models;
using WebApi.Models.FileModels;
using WebApi.Models.InvoiceAjaxModel;
using WebApi.Services;

namespace WebApi.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IHostingEnvironment env;
        private readonly IConfiguration configuration;
        private readonly CurrencyConfiguration currencyConfiguration;

        public InvoiceController(IHostingEnvironment _env, IConfiguration _configuration, CurrencyConfiguration _currencyConfiguration)
        {
            env = _env;
            configuration = _configuration;
            currencyConfiguration = _currencyConfiguration;
        }

        [Route("api/invoices/{invoice_id}")]
        [HttpGet]
        [EnableCors("CorsPolicy")]
        public IActionResult GetInvoice(int invoice_id)
        {
            try {
                using (DBEntities dbe = new DBEntities()) {
                    Invoice invoice = dbe.Invoices.SingleOrDefault(i => i.Id == invoice_id); //get the invoice
                    if (invoice != null)
                        return Ok(invoice);
                    else
                        return NotFound();
                }
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }

        [Route("api/invoices/{invoice_guid}")]
        [HttpGet]
        [EnableCors("CorsPolicy")]
        public IActionResult GetInvoice(string invoice_guid)
        {
            try {
                using (DBEntities dbe = new DBEntities()) {
                    Invoice invoice = dbe.Invoices.SingleOrDefault(i => i.InvoiceGuid.ToString() == invoice_guid); //get the invoice
                    if (invoice != null)
                        return Ok(invoice);
                    else
                        return NotFound();
                }
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }

        [Route("api/invoices/{invoice_id}")]
        [HttpPut]
        [Authorize]
        public IActionResult EditInvoice(int invoice_id, [FromBody]InvoiceAjaxModel invoiceModel)
        {
            try {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == invoiceModel.CreatedBy.Id) //only user who creted the invoice can edit it
                {
                    using (DBEntities dbe = new DBEntities()) {
                        //find invoice
                        Invoice invoice = dbe.Invoices.SingleOrDefault(i => i.Id == invoice_id);
                        invoice.Name = invoiceModel.Name;

                        invoice.Description = invoiceModel.Description;
                        invoice.FiatAmount = invoiceModel.FiatAmount;
                        invoice.FiatCurrencyCode = invoiceModel.FiatCurrencyCode;

                        dbe.Invoices.Update(invoice);
                        dbe.SaveChanges();
                        return Ok();
                    }
                }
                else {
                    return Unauthorized();

                }
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }

        [Route("api/invoice/{guid}")]
        [HttpDelete]
        [Authorize]
        [EnableCors("CorsPolicy")]
        public IActionResult DeleteInvoice(string guid)
        {
            //Delete only invoices belonging to the logged in user
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (DBEntities dbe = new DBEntities()) {
                var invoiceExists = dbe.Invoices.Any(i => i.InvoiceGuid.ToString() == guid && i.CreatedBy.Id == userId);
                if (!invoiceExists) {
                    return NotFound();
                }
                dbe.Invoices.Remove(dbe.Invoices.Single(i => i.InvoiceGuid.ToString() == guid));
                dbe.SaveChanges();
                return Ok("{}");
            }
        }

        [HttpPost]
        [Authorize]
        [EnableCors("CorsPolicy")]
        [Route("api/invoices")]
        public IActionResult CreateInvoice([FromBody]InvoiceAjaxModel model)
        {
            try {
                if (ModelState.IsValid) {
                    var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    using (DBEntities dbe = new DBEntities()) 
                    {
                        User loggedUser = dbe.Users.SingleOrDefault(u => u.Id == userId);
                        Invoice invoice = new Invoice() {
                            CreatedBy = dbe.Users.SingleOrDefault(u => u.Id == userId),
                            DateCreated = DateTime.UtcNow,
                            InvoiceGuid = Guid.NewGuid(),
                            State = (int)InvoiceState.NOT_PAID,
                            Name = model.Name,
                            Description = model.Description,
                            Recipient = model.Recipient,
                            FiatCurrencyCode = model.FiatCurrencyCode,
                            FiatAmount = model.FiatAmount,
                            FileName = model.FileName
                        };

                        // Proccess uploaded file
                        if (!string.IsNullOrEmpty(invoice.File)) {
                            string[] fileInfo = invoice.File.Split(';');
                            string mimeType = fileInfo[0].Split(':')[1];
                            string fileContent = fileInfo[1].Split(',')[1];

                            FileData fileData = new FileData() {
                                FileName = invoice.InvoiceGuid.ToString() + Path.GetExtension(invoice.FileName),
                                FileContent = Convert.FromBase64String(fileContent),
                            };

                            WebDAVClient client = new WebDAVClient(env, configuration);
                            invoice.File = client.UploadFile(fileData);
                            invoice.FileMime = mimeType;
                        }

                        foreach (string cc in model.Accept) {
                            string CC = cc.ToUpper();
                            invoice.PaymentsAvailable.Add(new InvoicePayment() {
                                CurrencyCode = CC,
                                VarSymbol = currencyConfiguration.Adapters[CC].GetVarSymbol(),
                                ExchangeRate = currencyConfiguration.Adapters[CC].GetExchangeRate()
                            });
                        }

                        dbe.Invoices.Add(invoice);
                        dbe.SaveChanges();

                        foreach (string cc in model.Accept) {
                            currencyConfiguration.Adapters[cc.ToUpper()].GetAddress(invoice.Id, loggedUser);
                        }

                        // send info e-mail
                        string invoiceUrl = string.Format("{0}/invoice/{1}",
                                env.IsDevelopment() ? configuration["FrontEndHostName:Development"] : configuration["FrontEndHostName:Production"],
                                invoice.InvoiceGuid);

                        string subject = $"New invoice from {loggedUser.UserName}";
                        string attachment = $"{invoice.File}|{invoice.FileName}|{invoice.FileMime}";
                        string body = System.IO.File.ReadAllText("Views/Email/invoice.html");
                        body = body.Replace("{User.Name}", loggedUser.UserName)
                                   .Replace("{Invoice.Name}", invoice.Name)
                                   .Replace("{Invoice.Description}", invoice.Description)
                                   .Replace("{URL}", invoiceUrl);

                        EmailSender sender = new EmailSender(configuration);
                        Email email = sender.CreateEmailEntity("info@octupus.com", invoice.Recipient, body, subject, attachment);

                        sender.AddEmailToQueue(email);

                        //Front end needs this new id to call GetInvoice
                        return Ok(invoice.InvoiceGuid);
                    }
                }
                else {
                    var query = from state in ModelState.Values
                                from error in state.Errors
                                select error.ErrorMessage;
                    var errors = query.ToList();
                    string allErrors = "";
                    foreach (string error in errors) {
                        allErrors += error + "\n";
                    }
                    return BadRequest(allErrors);
                }
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Authorize]
        [EnableCors("CorsPolicy")]
        [Route("api/invoices")]
        public IActionResult GetListInvoices()
        {
            try {
                //we get the logged user id
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                //find from invoices and filter invoices which belongs to current user
                using (DBEntities dbe = new DBEntities()) {
                    return Ok(dbe.Invoices.Where(i => i.CreatedBy.Id == userId).Select(x => new { id = x.Id, name = x.Name }).ToList());
                }
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }

        [Route("api/invoices/init")]
        [Authorize]
        [EnableCors("CorsPolicy")]
        public IActionResult InitData()
        {
            try {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                using (DBEntities dbe = new DBEntities()) {
                    User loggedUser = dbe.Users.SingleOrDefault(u => u.Id == userId);
                    var displayName = loggedUser.UserName;

                    List<Invoice> invoiceList = dbe.Invoices.Where(i => i.CreatedBy.Id == userId).ToList();
                    List<JObject> invoices = new List<JObject>();
                    foreach (Invoice item in invoiceList) {
                        JObject invoice = new JObject() {
                            { "id", item.Id },
                            { "name", item.Name },
                            { "description", item.Description },
                            { "dateCreated", item.DateCreated },
                            { "dateReceived", item.DateReceived },
                            { "state", item.State },
                            { "fiatCurrencyCode", item.FiatCurrencyCode },
                            { "fiatAmount", item.FiatAmount },
                            { "createdBy", item.CreatedBy.Email },
                            { "transactionCurrencyCode", item.TransactionCurrencyCode },
                            { "transactionId", item.TransactionId },
                            { "recipient", item.Recipient },
                            { "invoiceGuid", item.InvoiceGuid },
                            { "fileUrl", item.File },
                            { "fileName", item.FileName }
                        };

                        foreach (CurrencyConfigurationItem currency in currencyConfiguration.Supported) {
                            var payment = item.PaymentsAvailable.Where(p => p.CurrencyCode == currency.CurrencyCode).SingleOrDefault();
                            var CC = currency.CurrencyCode.ToUpper();
                            var cc = currency.CurrencyCode.ToLower();

                            invoice[$"{cc}Address"] = payment != null ? payment.Address : "";
                            invoice[$"{cc}vs"] = payment != null ? payment.VarSymbol : "";
                            invoice[$"newFixER_{CC}"] = payment != null ? payment.ExchangeRate : 0;
                            invoice[$"accept{CC}"] = payment != null ? true : false;
                        }

                        invoices.Add(invoice);
                    }

                    return Ok(new InvoiceInitDataAjaxModel() {
                        UserId = userId,
                        DisplayName = displayName,
                        InvoiceList = invoices,
                        SupportCurrencies = currencyConfiguration.Supported
                    });
                }
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }
    }
}
