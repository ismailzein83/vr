using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class SettlementGenerationCustomSectionPayload
    {
        public decimal? Commission { get; set; }
        public CommissionType CommissionType { get; set; }
        public List<long> CustomerInvoiceIds { get; set; }
        public List<long> SupplierInvoiceIds { get; set; }
        public SettlementGenerationCustomSectionPayloadSummary Summary { get; set; }
    }

    public class SettlementGenerationCustomSectionPayloadSummary
    {
        public Dictionary<string, decimal> SupplierAmountByCurrency { get; set; }
        public Dictionary<string, decimal> CustomerAmountByCurrency { get; set; }
    }
}