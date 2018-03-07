using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Invoice.Business;
using TOne.WhS.Invoice.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Invoice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceSettlement")]
    public class InvoiceSettlementController : BaseAPIController
    {
        InvoiceSettlementManager invoiceSettlementManager = new InvoiceSettlementManager();
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
        public SettlementGenerationCustomSectionPayload EvaluatePartnerCustomPayload(string partnerId, Guid invoiceTypeId, DateTime fromDate, DateTime toDate, long? currentInvoiceId)
        {
            return invoiceSettlementManager.EvaluatePartnerCustomPayload(currentInvoiceId,partnerId, invoiceTypeId, fromDate, toDate);

        }
    }

    public class InvoiceSettlementInput
    {
        public string PartnerId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public List<long> CustomerInvoiceIds { get; set; }
        public List<long> SupplierInvoiceIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}