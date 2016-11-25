﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.Business;
namespace Vanrise.Invoice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Invoice")]
    public class InvoiceController:BaseAPIController
    {
        [HttpGet]
        [Route("GetInvoice")]
        public Entities.Invoice GetInvoice(long invoiceId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetInvoice(invoiceId);
        }
        [HttpPost]
        [Route("GenerateInvoice")]
        public Vanrise.Entities.InsertOperationOutput<InvoiceDetail> GenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GenerateInvoice(createInvoiceInput);
        }
        [HttpPost]
        [Route("ReGenerateInvoice")]
        public Vanrise.Entities.UpdateOperationOutput<InvoiceDetail> ReGenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.ReGenerateInvoice(createInvoiceInput);
        }
        [HttpPost]
        [Route("GetFilteredInvoices")]
        public object GetFilteredInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceQuery> input)
        {
            InvoiceManager manager = new InvoiceManager();
            return GetWebResponse(input, manager.GetFilteredInvoices(input));
        }
        [HttpGet]
        [Route("SetInvoicePaid")]
        public bool SetInvoicePaid(long invoiceId, bool isInvoicePaid)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.SetInvoicePaid(invoiceId, isInvoicePaid);
        }
        [HttpGet]
        [Route("SetInvoiceLocked")]
        public bool SetInvoiceLocked(long invoiceId, bool setLocked)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.SetInvoiceLocked(invoiceId, setLocked);
        }
        [HttpGet]
        [Route("GetInvoiceDetail")]
        public Entities.InvoiceDetail GetInvoiceDetail(long invoiceId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetInvoiceDetail(invoiceId);
        }
    }
}