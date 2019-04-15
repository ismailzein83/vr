using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public class SettlementGenerationCustomSectionPayload
    {
        public List<InvoiceAvailableForSettlement> AvailableCustomerInvoices { get; set; }
        public List<InvoiceAvailableForSettlement> AvailableSupplierInvoices { get; set; }
        public bool IsCustomerApplicable { get; set; }
        public bool IsSupplierApplicable { get; set; }
        public SettlementGenerationCustomSectionPayloadSummary Summary { get; set; }
    }
    public class InvoiceAvailableForSettlement
    {
        public long InvoiceId { get; set; }
        public int CurrencyId { get; set; }
        public bool IsSelected { get; set; }
    }
    public class SettlementGenerationCustomSectionPayloadSummary
    {
        public string ErrorMessage { get; set; }
        public Dictionary<string, decimal> SupplierAmountByCurrency { get; set; }
        public Dictionary<string, decimal> CustomerAmountByCurrency { get; set; }
    }
}
