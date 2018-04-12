using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Invoice.Entities;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Invoice.Business;
using Vanrise.Common.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SettlementGenerationCustomSection : GenerationCustomSection
    {
        InvoiceSettlementManager _invoiceSettlementManager = new InvoiceSettlementManager();

        Guid _customerInvoiceTypeId;
        Guid _supplierInvoiceTypeId;
        public SettlementGenerationCustomSection(Guid customerInvoiceTypeId, Guid sipplierInvoiceTypeId)
        {
            _customerInvoiceTypeId = customerInvoiceTypeId;
            _supplierInvoiceTypeId = sipplierInvoiceTypeId;
        }
        public override string GenerationCustomSectionDirective
        {
            get
            {
                return "whs-invoicetype-generationcustomsection-settlement";
            }
        }
        public override void EvaluateGenerationCustomPayload(IGetGenerationCustomPayloadContext context)
        {

            List<Vanrise.Invoice.Entities.Invoice> supplierInvoices =  new List<Vanrise.Invoice.Entities.Invoice>();

            List<Vanrise.Invoice.Entities.Invoice> customerInvoices = new List<Vanrise.Invoice.Entities.Invoice>();
            List<InvoiceItem> supplierInvoiceItems = new List<InvoiceItem>(), customerInvoiceItems = new List<InvoiceItem>();

            IEnumerable<string> partnerIds = context.InvoiceGenerationInfo.MapRecords(x => x.PartnerId);
            if (context.InvoiceGenerationInfo != null)
            {
                foreach (var generationCustomPayload in context.InvoiceGenerationInfo)
                {
                    generationCustomPayload.CustomPayload = _invoiceSettlementManager.EvaluateGenerationCustomPayload(_customerInvoiceTypeId, _supplierInvoiceTypeId, partnerIds, generationCustomPayload.PartnerId, context.InvoiceTypeId, context.MinimumFrom, context.MaximumTo, supplierInvoices, supplierInvoiceItems, customerInvoices, customerInvoiceItems, true);
                }
            }
        }
    }
}
