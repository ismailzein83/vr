using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    public class InterconnectSettlementGenerationCustomSection : GenerationCustomSection
    {

        InterconnectInvoiceSettlementManager _interconnectInvoiceSettlementManager = new InterconnectInvoiceSettlementManager();

        Guid _customerInvoiceTypeId;
        Guid _supplierInvoiceTypeId;
        public InterconnectSettlementGenerationCustomSection(Guid customerInvoiceTypeId, Guid sipplierInvoiceTypeId)
        {
            _customerInvoiceTypeId = customerInvoiceTypeId;
            _supplierInvoiceTypeId = sipplierInvoiceTypeId;
        }
        public override string GenerationCustomSectionDirective
        {
            get
            {
                return "retail-invoicetype-generationcustomsection-settlement";
            }
        }
        public override void EvaluateGenerationCustomPayload(IGetGenerationCustomPayloadContext context)
        {

            List<Vanrise.Invoice.Entities.Invoice> supplierInvoices = new List<Vanrise.Invoice.Entities.Invoice>();

            List<Vanrise.Invoice.Entities.Invoice> customerInvoices = new List<Vanrise.Invoice.Entities.Invoice>();
            List<InvoiceItem> supplierInvoiceItems = new List<InvoiceItem>(), customerInvoiceItems = new List<InvoiceItem>();

            IEnumerable<string> partnerIds = context.InvoiceGenerationInfo.MapRecords(x => x.PartnerId);
            if (context.InvoiceGenerationInfo != null)
            {
                foreach (var generationCustomPayload in context.InvoiceGenerationInfo)
                {
                    generationCustomPayload.CustomPayload = _interconnectInvoiceSettlementManager.EvaluateGenerationCustomPayload(_customerInvoiceTypeId, _supplierInvoiceTypeId, partnerIds, generationCustomPayload.PartnerId, context.InvoiceTypeId, context.MinimumFrom, context.MaximumTo, supplierInvoices, supplierInvoiceItems, customerInvoices, customerInvoiceItems, false);
                }
            }
        }
    }
}
