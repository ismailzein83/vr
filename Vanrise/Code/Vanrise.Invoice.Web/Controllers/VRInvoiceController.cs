using System;
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
    [JSONWithTypeAttribute]
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
        public bool CheckInvoiceFollowBillingPeriod(Guid invoiceTypeId, string partnerId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.CheckInvoiceFollowBillingPeriod(invoiceTypeId, partnerId);
        }
        [HttpGet]
        [Route("CheckGeneratedInvoicePeriodGaP")]
        public DateTime? CheckGeneratedInvoicePeriodGaP(DateTime fromDate, Guid invoiceTypeId, string partnerId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.CheckGeneratedInvoicePeriodGaP(fromDate, invoiceTypeId, partnerId);
        }

        [HttpGet]
        [Route("DoesUserHaveGenerateAccess")]
        public bool DoesUserHaveGenerateAccess(Guid invoiceTypeId)
        {
            return _invoiceTypeManager.DoesUserHaveGenerateAccess(invoiceTypeId);
        }

        [HttpPost]
        [Route("ReGenerateInvoice")]
        public object ReGenerateInvoice(ReGenerateInvoiceInput recreateInvoiceInput)
        {
            if (!_invoiceTypeManager.DosesUserHaveActionAccess(InvoiceActionType.ReCreateInvoice, recreateInvoiceInput.InvoiceTypeId, recreateInvoiceInput.InvoiceActionId))
                return GetUnauthorizedResponse();

            InvoiceManager manager = new InvoiceManager();
            return manager.ReGenerateInvoice(recreateInvoiceInput);
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
        public object SetInvoicePaid(Guid invoiceActionId, long invoiceId, bool isInvoicePaid)
        {
            InvoiceManager manager = new InvoiceManager();

            if (!manager.DosesUserHaveActionAccess(InvoiceActionType.SetInvoicePaid, invoiceId, invoiceActionId))
                return GetUnauthorizedResponse();

            return manager.SetInvoicePaid(invoiceId, isInvoicePaid);
        }
        [HttpGet]
        [Route("UpdateInvoiceNote")]
        public object UpdateInvoiceNote(Guid invoiceActionId, long invoiceId, string invoiceNote = null)
        {
            InvoiceManager manager = new InvoiceManager();

            if (!manager.DosesUserHaveActionAccess(InvoiceActionType.UpdateInvoiceNote, invoiceId, invoiceActionId))
                return GetUnauthorizedResponse();
            return manager.UpdateInvoiceNote(invoiceId, invoiceNote);
        }
        [HttpGet]
        [Route("SetInvoiceLocked")]
        public object SetInvoiceLocked(Guid invoiceActionId, long invoiceId, bool setLocked)
        {
            InvoiceManager manager = new InvoiceManager();
            if (!manager.DosesUserHaveActionAccess(InvoiceActionType.SetInvoiceLocked, invoiceId, invoiceActionId))
                return GetUnauthorizedResponse();
           
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
        [Route("DownloadAttachment")]
        public object DownloadAttachment(Guid invoiceTypeId, Guid invoiceAttachmentId, long invoiceId)
        {
            InvoiceManager manager = new InvoiceManager();
            if (!manager.DoesUserHaveViewAccess(invoiceId))
                return GetUnauthorizedResponse();
            return manager.DownloadAttachment(invoiceTypeId, invoiceAttachmentId, invoiceId);
        }
        [HttpGet]
        [Route("GetLastInvoice")]
        public InvoiceClientDetail GetLastInvoice(Guid invoiceTypeId, string partnerId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetLastInvoice(invoiceTypeId, partnerId);
        }

        [HttpGet]
        [Route("GetPartnerGroupTemplates")]
        public IEnumerable<PartnerGroupConfig> GetPartnerGroupTemplates()
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetPartnerGroupTemplates();
        }

        [HttpPost]
        [Route("GenerateFilteredInvoiceGenerationDrafts")]
        public object GenerateFilteredInvoiceGenerationDrafts(InvoiceGenerationDraftQuery query)
        {
            if (!_invoiceTypeManager.DoesUserHaveViewAccess(query.InvoiceTypeId))
                return GetUnauthorizedResponse();
            InvoiceGenerationDraftManager manager = new InvoiceGenerationDraftManager();
            return manager.GenerateFilteredInvoiceGenerationDrafts(query);
        }

        [HttpPost]
        [Route("ReGenerateInvoiceGenerationDraft")]
        public object ReGenerateInvoiceGenerationDraft(InvoiceGenerationPartnerDraftInput input)
        {
            if (!_invoiceTypeManager.DoesUserHaveViewAccess(input.InvoiceTypeId))
                return GetUnauthorizedResponse();
            InvoiceGenerationDraftManager manager = new InvoiceGenerationDraftManager();
            return manager.ReGenerateInvoiceGenerationDraft(input);
        }


        [HttpPost]
        [Route("GetFilteredInvoiceGenerationDrafts")]
        public object GetFilteredInvoiceGenerationDrafts(Vanrise.Entities.DataRetrievalInput<InvoiceGenerationDraftQuery> input)
        {
            if (!_invoiceTypeManager.DoesUserHaveViewAccess(input.Query.InvoiceTypeId))
                return GetUnauthorizedResponse();
            InvoiceGenerationDraftManager manager = new InvoiceGenerationDraftManager();
            return GetWebResponse(input, manager.GetFilteredInvoiceGenerationDrafts(input));
        }

        [HttpPost]
        [Route("GenerateInvoices")]
        public object GenerateInvoices(GenerateInvoicesInput input)
        {
            if (!_invoiceTypeManager.DoesUserHaveViewAccess(input.InvoiceTypeId))
                return GetUnauthorizedResponse();
            InvoiceManager manager = new InvoiceManager();
            return manager.GenerateInvoices(input.InvoiceTypeId, input.InvoiceGenerationIdentifier, input.IssueDate, input.ChangedItems, input.InvoiceGapAction);
        }
        [HttpGet]
        [Route("DeleteGeneratedInvoice")]
        public object DeleteGeneratedInvoice(Guid invoiceActionId, long invoiceId)
        {
            InvoiceManager manager = new InvoiceManager();
            if (!manager.DosesUserHaveActionAccess(InvoiceActionType.SetInvoiceLocked, invoiceId, invoiceActionId))
                if (!manager.DosesUserHaveActionAccess(InvoiceActionType.SetInvoiceDeleted, invoiceId, invoiceActionId))
                return GetUnauthorizedResponse();
            return manager.DeleteGeneratedInvoice(invoiceId);
        }

        [HttpPost]
        [Route("ExecuteMenualInvoiceActions")]
        public object ExecuteMenualInvoiceActions(ExecuteMenualInvoiceActionsInput input)
        {
            if (!_invoiceTypeManager.DoesUserHaveViewAccess(input.InvoiceTypeId))
                return GetUnauthorizedResponse();
            InvoiceManager manager = new InvoiceManager();
            return manager.ExecuteMenualInvoiceActions(input);
        }


    }

    public class GenerateInvoicesInput
    {
        public DateTime IssueDate { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceGenerationIdentifier { get; set; }
        public List<InvoiceGenerationDraftToEdit> ChangedItems { get; set; }
        public InvoiceGapAction InvoiceGapAction { get; set; }
    }
}