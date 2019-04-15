using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public class SettlementGenerationCustomSectionInvoices
    {
        public string PartnerName { get; set; }
        public List<InterconnectInvoiceDetail> CustomerInvoiceDetails { get; set; }
        public List<InterconnectInvoiceDetail> SupplierInvoiceDetails { get; set; }

    }

    public class InterconnectInvoiceDetail
    {
        public int CurrencyId { get; set; }
        public long InvoiceId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime IssueDate { get; set; }
        public int TotalNumberOfCalls { get; set; }
        public decimal Duration { get; set; }
        public string InterconnectCurrency { get; set; }
        public Decimal TotalAmount { get; set; }
        public bool UseOriginalAmount { get; set; }
        public decimal? OriginalAmount { get; set; }
        public bool IsLocked { get; set; }
        public bool HasRecurringCharge { get; set; }
    }
}
