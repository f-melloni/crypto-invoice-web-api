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
using Microsoft.EntityFrameworkCore;
using WebApi.Models.InvoiceAjaxModel;
using WebApi.Services;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace WebApi.Controllers
{
    public class InvoiceController : Controller
    {
        [Route("api/invoices/{invoice_id}")]
        [HttpGet]
        [Authorize]
        [EnableCors("CorsPolicy")]
        public IActionResult GetInvoice(int invoice_id)
        {
            try
            {
                using (DBEntities dbe = new DBEntities())
                {
                    Invoice invoice = dbe.Invoices.SingleOrDefault(i => i.Id == invoice_id); //get the invoice
                    if (invoice != null)
                        return Ok(invoice);
                    else
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("api/invoices/{invoice_id}")]
        [HttpPut]
        [Authorize]
        public IActionResult EditInvoice(int invoice_id,[FromBody]Invoice invoiceModel)
        {
            try
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == invoiceModel.createdBy.Id) //only user who creted the invoice can edit it
                {
                    using (DBEntities dbe = new DBEntities())
                    {
                        //find invoice
                        Invoice invoice = dbe.Invoices.SingleOrDefault(i => i.Id == invoice_id);
                        invoice.Name = invoiceModel.Name;

                        invoice.Description = invoiceModel.Description;
                        invoice.FiatAmount = invoiceModel.FiatAmount;
                        invoice.FiatCurrencyCode = invoiceModel.FiatCurrencyCode;

                        invoice.NewFixER_BTC = invoiceModel.NewFixER_BTC;
                        invoice.NewFixER_ETH = invoiceModel.NewFixER_ETH;
                        invoice.NewFixER_LTC = invoiceModel.NewFixER_LTC;
                        invoice.NewFixER_XMR = invoiceModel.NewFixER_XMR;
            
                        dbe.Invoices.Update(invoice);
                        dbe.SaveChanges();
                        return Ok();
                    }
                }
                else
                {
                    return Unauthorized();

                }

            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("api/invoice/{id}")]
        [HttpDelete]
        [Authorize]
        [EnableCors("CorsPolicy")]
        public IActionResult deleteInvoice(int id)
        {
            //Delete only invoices belonging to the logged in user
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (DBEntities dbe = new DBEntities())
            {
                var invoiceExists = dbe.Invoices.Any(i => i.Id == id && i.createdBy.Id == userId);
                if (!invoiceExists)
                    return NotFound();
                dbe.Invoices.Remove(dbe.Invoices.Single(i => i.Id == id));
                dbe.SaveChanges();
                return Ok("{}");
            }
        }

        [HttpPost]
        [Authorize]
        [EnableCors("CorsPolicy")]
        [Route("api/invoices")]
        public IActionResult CreateInvoice([FromBody]Invoice invoiceModel, List<IFormFile> file)
        {
            try
            {
                //current user logged in
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Invoice invoice = new Invoice();
                using (DBEntities dbe = new DBEntities())
                {
                    User loggedUser = dbe.Users.SingleOrDefault(u => u.Id == userId);

                    invoice.createdBy = dbe.Users.SingleOrDefault(u => u.Id == userId);
                    invoice.DateCreated = DateTime.UtcNow;

                    invoice.Name = invoiceModel.Name;
                    invoice.Description = invoiceModel.Description;
                    invoice.FiatAmount = invoiceModel.FiatAmount;
                    invoice.FiatCurrencyCode = invoiceModel.FiatCurrencyCode;
                    invoice.state = 1;//not paid
                    invoice.Recipient = invoiceModel.Recipient;
                    //invoice.NewFixER_BTC = invoiceModel.NewFixER_BTC;
                    //invoice.NewFixER_ETH = invoiceModel.NewFixER_ETH;
                    //invoice.NewFixER_LTC = invoiceModel.NewFixER_LTC;
                    //invoice.NewFixER_XMR = invoiceModel.NewFixER_XMR;
         
                    dbe.Invoices.Add(invoice);
                    dbe.SaveChanges();

                    //get the id and call create new address
                    if (invoiceModel.AcceptBTC){
                        RabbitMessages.GetNewAddress("BTC", invoice.Id,loggedUser.BTCXPUB);
                        string apiUrl = "https://min-api.cryptocompare.com/data/generateAvg?fsym=BTC&tsym=USD&e=Poloniex,Kraken,Coinbase,HitBTC";
                        var price = RestClient.GetResponse(apiUrl).ToObject<JObject>().GetValue("RAW").ToObject<JObject>().GetValue("PRICE").ToObject<Double>();
                        invoice.NewFixER_BTC = price;
                        invoice.AcceptBTC = true;
                    }
                    if (invoiceModel.AcceptLTC)
                    {
                        RabbitMessages.GetNewAddress("LTC", invoice.Id, loggedUser.LTCXPUB);
                        string apiUrl = "https://min-api.cryptocompare.com/data/generateAvg?fsym=LTC&tsym=USD&e=Poloniex,Kraken,Coinbase,HitBTC";
                        var price = RestClient.GetResponse(apiUrl).ToObject<JObject>().GetValue("RAW").ToObject<JObject>().GetValue("PRICE").ToObject<Double>();
                        invoice.NewFixER_LTC = price;
                        invoice.AcceptLTC = true;
                    }
                    dbe.SaveChanges();

              
                        return Ok();
                    return Ok(invoice.Id);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Authorize]
        [EnableCors("CorsPolicy")]
        [Route("api/invoices")]
        public IActionResult GetListInvoices()
        {
            try
            {
                //we get the logged user id
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                //find from invoices and filter invoices which belongs to current user
                using (DBEntities dbe = new DBEntities())
                {
                    return Ok(dbe.Invoices.Where(i => i.createdBy.Id == userId).Select(x => new { id = x.Id, name = x.Name }).ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("api/invoices/init")]
        [Authorize]
        [EnableCors("CorsPolicy")]
        public IActionResult initData()
        {
            try
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // List<InvoiceInitModel> listInvoices = new List<InvoiceInitModel>();
                using (DBEntities dbe = new DBEntities())
                {
                    User loggedUser = dbe.Users.SingleOrDefault(u => u.Id == userId);
                    var displayName = loggedUser.UserName;
                    // we do not want to send the User entity (settings, password hash etc.) with invoices
                    List<object> invoices = dbe.Invoices.Where(i => i.createdBy.Id == userId).Select(x => new {
                        id = x.Id, name = x.Name, description = x.Description, btcAddress = x.BTCAddress, ltcAddress = x.LTCAddress,
                        ethvs = x.ETHVS, xmrvs = x.XMRVS, dateCreated = x.DateCreated, dateReceived = x.DateReceived, state = x.state,
                        fiatCurrencyCode = x.FiatCurrencyCode, fiatAmount = x.FiatAmount,
                        newFixER_BTC = x.NewFixER_BTC, newFixER_LTC = x.NewFixER_LTC, newFixER_ETH = x.NewFixER_ETH, newFixER_XMR = x.NewFixER_XMR,
                        createdBy = x.createdBy.Email, transactionCurrencyCode = x.TransactionCurrencyCode, transactionId = x.TransactionId,
                        acceptBTC = x.AcceptBTC, acceptLTC = x.AcceptLTC, acceptETH = x.AcceptETH, acceptXMR = x.AcceptXMR,
                        recipient = x.Recipient
                    }).ToList<object>();
                    /* foreach(Invoice i in invoices)
                        listInvoices.Add(new InvoiceInitModel() {Id = i.Id,Name = i.Name,DateCreated = i.DateCreated,Status = i.state}); */
                    return Ok(new InvoiceInitDataAjaxModel() { UserId = userId, DisplayName = displayName, InvoiceList = invoices });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("api/invoices/testShit")]
        [HttpGet]
        public IActionResult testShit()
        {
            //RabbitMessages.GetNewAddress("BTC", 9999);
            return Ok();
                
        }

    }


}
