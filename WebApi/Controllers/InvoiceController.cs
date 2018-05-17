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

namespace WebApi.Controllers
{
    public class InvoiceController : Controller
    {
        [Route("api/invoices/{invoice_id}")]
        [HttpGet]
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
                        invoice.BTCAddress = invoiceModel.BTCAddress;
                        invoice.DateReceived = invoiceModel.DateReceived;
                        invoice.Description = invoiceModel.Description;
                        invoice.ETHVS = invoiceModel.ETHVS;
                        invoice.FiatAmount = invoiceModel.FiatAmount;
                        invoice.FiatCurrencyCode = invoiceModel.FiatCurrencyCode;
                        invoice.FixedRateOnCreation = invoiceModel.FixedRateOnCreation;
                        invoice.LTCAddress = invoiceModel.LTCAddress;
                        invoice.NewFixER_BTC = invoiceModel.NewFixER_BTC;
                        invoice.NewFixER_ETH = invoiceModel.NewFixER_ETH;
                        invoice.NewFixER_LTC = invoiceModel.NewFixER_LTC;
                        invoice.NewFixER_XMR = invoiceModel.NewFixER_XMR;
                        invoice.OldFixER_BTC = invoiceModel.OldFixER_BTC;
                        invoice.OldFixER_ETH = invoiceModel.OldFixER_ETH;
                        invoice.OldFixER_LTC = invoiceModel.OldFixER_LTC;
                        invoice.OldFixER_XMR = invoiceModel.OldFixER_XMR;
                        invoice.XMRVS = invoiceModel.XMRVS;

                        dbe.Invoices.Update(invoice);
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

        [HttpPost]
        [Route("api/invoices")]
        public IActionResult CreateInvoice([FromBody]Invoice invoiceModel)
        {
            try
            {
                //current user logged in
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Invoice invoice = new Invoice();
                using (DBEntities dbe = new DBEntities())
                {
                    invoice.createdBy = dbe.Users.SingleOrDefault(u => u.Id == userId);
                    invoice.DateCreated = DateTime.UtcNow;

                    invoice.Name = invoiceModel.Name;
                    invoice.BTCAddress = invoiceModel.BTCAddress;
                    invoice.DateReceived = invoiceModel.DateReceived;
                    invoice.Description = invoiceModel.Description;
                    invoice.ETHVS = invoiceModel.ETHVS;
                    invoice.FiatAmount = invoiceModel.FiatAmount;
                    invoice.FiatCurrencyCode = invoiceModel.FiatCurrencyCode;
                    invoice.FixedRateOnCreation = invoiceModel.FixedRateOnCreation;
                    invoice.LTCAddress = invoiceModel.LTCAddress;
                    invoice.NewFixER_BTC = invoiceModel.NewFixER_BTC;
                    invoice.NewFixER_ETH = invoiceModel.NewFixER_ETH;
                    invoice.NewFixER_LTC = invoiceModel.NewFixER_LTC;
                    invoice.NewFixER_XMR = invoiceModel.NewFixER_XMR;
                    invoice.OldFixER_BTC = invoiceModel.OldFixER_BTC;
                    invoice.OldFixER_ETH = invoiceModel.OldFixER_ETH;
                    invoice.OldFixER_LTC = invoiceModel.OldFixER_LTC;
                    invoice.OldFixER_XMR = invoiceModel.OldFixER_XMR;
                    invoice.XMRVS = invoiceModel.XMRVS;

                    dbe.Invoices.Add(invoice);
                    dbe.SaveChanges();

                    //get the id and call create new address
                    int id = invoice.Id;
                    RabbitMessages.GetNewAddress("LTC", id);
                    return Ok();
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
                        fixedRateOnCreation = x.FixedRateOnCreation, fiatCurrencyCode = x.FiatCurrencyCode, fiatAmount = x.FiatAmount,
                        oldFixER_BTC = x.OldFixER_BTC, oldFixER_LTC = x.OldFixER_LTC, oldFixER_ETH = x.OldFixER_ETH, oldFixER_XMR = x.OldFixER_XMR,
                        newFixER_BTC = x.NewFixER_BTC, newFixER_LTC = x.NewFixER_LTC, newFixER_ETH = x.NewFixER_ETH, newFixER_XMR = x.NewFixER_XMR,
                        createdBy = x.createdBy.Email
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
            RabbitMessages.GetNewAddress("BTC", 9999);
            return Ok();

        }

    }


}
