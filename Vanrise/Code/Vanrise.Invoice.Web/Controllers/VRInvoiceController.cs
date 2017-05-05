﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Entities;
using Microsoft.Reporting.WebForms;
using Vanrise.Invoice.MainExtensions;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Business.Context;
using System.IO;
using System.Security;
namespace Vanrise.Invoice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Invoice")]

    public class VRInvoiceController : BaseAPIController
    {
        InvoiceTypeManager _invoiceTypeManager = new InvoiceTypeManager();

        [HttpGet]
        [Route("GetInvoice")]
        public Entities.Invoice GetInvoice(long invoiceId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetInvoice(invoiceId);
        }
        [HttpGet]
        [Route("GetInvoiceEditorRuntime")]
        public Entities.InvoiceEditorRuntime GetInvoiceEditorRuntime(long invoiceId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetInvoiceEditorRuntime(invoiceId);
        }

        [HttpGet]
        [Route("CheckInvoiceFollowBillingPeriod")]
        public bool CheckInvoiceFollowBillingPeriod(Guid invoiceTypeId,string partnerId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.CheckInvoiceFollowBillingPeriod(invoiceTypeId,partnerId);
        }
        [HttpGet]
        [Route("CheckGeneratedInvoicePeriodGaP")]
        public DateTime? CheckGeneratedInvoicePeriodGaP(DateTime fromDate, Guid invoiceTypeId, string partnerId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.CheckGeneratedInvoicePeriodGaP(fromDate, invoiceTypeId, partnerId);
        }
        [HttpPost]
        [Route("GenerateInvoice")]
        public object GenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {
            if (!DoesUserHaveGenerateAccess(createInvoiceInput.InvoiceTypeId))
                return GetUnauthorizedResponse();
            InvoiceManager manager = new InvoiceManager();
            return manager.GenerateInvoice(createInvoiceInput);
        }

        [HttpGet]
        [Route("DoesUserHaveGenerateAccess")]
        public bool DoesUserHaveGenerateAccess(Guid invoiceTypeId)
        {
            return _invoiceTypeManager.DoesUserHaveGenerateAccess(invoiceTypeId);
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
            if (!_invoiceTypeManager.DoesUserHaveViewAccess(input.Query.InvoiceTypeId))
                return GetUnauthorizedResponse();
            InvoiceManager manager = new InvoiceManager();
            return GetWebResponse(input, manager.GetFilteredInvoices(input));
        }
        [HttpPost]
        [Route("GetFilteredClientInvoices")]
        public object GetFilteredClientInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceQuery> input)
        {
            if (!_invoiceTypeManager.DoesUserHaveViewAccess(input.Query.InvoiceTypeId))
                return GetUnauthorizedResponse();
            InvoiceManager manager = new InvoiceManager();
            return GetWebResponse(input, manager.GetFilteredClientInvoices(input));
        }
        [HttpGet]
        [Route("SetInvoicePaid")]
        public bool SetInvoicePaid(long invoiceId, bool isInvoicePaid)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.SetInvoicePaid(invoiceId, isInvoicePaid);
        }
        [HttpGet]
        [Route("UpdateInvoiceNote")]
        public bool UpdateInvoiceNote(long invoiceId, string invoiceNote = null)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.UpdateInvoiceNote(invoiceId, invoiceNote);
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
        [HttpGet]
        [Route("GetBillingInterval")]
        public BillingInterval GetBillingInterval(Guid invoiceTypeId, string partnerId,DateTime issueDate)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetBillingInterval(invoiceTypeId, partnerId, issueDate);
        }
        [HttpGet]
        [Route("DownloadAttachment")]
        public byte[] DownloadAttachment(Guid invoiceTypeId, Guid invoiceAttachmentId, long invoiceId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.DownloadAttachment(invoiceTypeId, invoiceAttachmentId, invoiceId);
        }
        [HttpGet]
        [Route("GetLastInvoice")]
        public Entities.Invoice GetLastInvoice(Guid invoiceTypeId, string partnerId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetLastInvoice(invoiceTypeId, partnerId);
        }
    }
}
