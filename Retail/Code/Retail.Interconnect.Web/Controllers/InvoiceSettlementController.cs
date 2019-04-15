using Retail.Interconnect.Business;
using Retail.Interconnect.Entities;
using Retail.Interconnect.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.Voice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceSettlement")]
    public class InvoiceSettlementController : BaseAPIController
    {
        InterconnectInvoiceSettlementManager invoiceSettlementManager = new InterconnectInvoiceSettlementManager();
        [HttpPost]
        [Route("TryLoadInvoicesAndGetAmountByCurrency")]
        public SettlementGenerationCustomSectionPayloadSummary TryLoadInvoicesAndGetAmountByCurrency(InvoiceSettlementInput input)
        {
            return invoiceSettlementManager.TryLoadInvoicesAndGetAmountByCurrency(input.InvoiceTypeId, input.CustomerInvoiceIds, input.SupplierInvoiceIds, input.FromDate, input.ToDate);
        }
        [HttpPost]
        [Route("LoadInvoicesDetails")]
        public SettlementGenerationCustomSectionInvoices LoadInvoicesDetails(InvoiceSettlementInput input)
        {
            return invoiceSettlementManager.LoadInvoicesDetails(input.InvoiceTypeId, input.PartnerId, input.CustomerInvoiceIds, input.SupplierInvoiceIds);
        }
        [HttpGet]
        [Route("EvaluatePartnerCustomPayload")]
        public SettlementGenerationCustomSectionPayload EvaluatePartnerCustomPayload(string partnerId, Guid invoiceTypeId, DateTime fromDate, DateTime toDate)
        {
            return invoiceSettlementManager.EvaluatePartnerCustomPayload(partnerId, invoiceTypeId, fromDate, toDate);

        }
    }

    public class InvoiceSettlementInput
    {
        public string PartnerId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public List<SelectedInvoiceItem> CustomerInvoiceIds { get; set; }
        public List<SelectedInvoiceItem> SupplierInvoiceIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}