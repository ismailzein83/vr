using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class SupplierInvoiceDetails
    {
        public int TotalNumberOfCalls { get; set; }
        public decimal CostAmount { get; set; }
        public Decimal OriginalCostAmount { get; set; }

        public decimal Duration { get; set; }
        public string PartnerType { get; set; }
        public int SupplierCurrencyId { get; set; }
        public string SupplierCurrency { get; set; }
        public int OriginalSupplierCurrencyId { get; set; }
        public string OriginalSupplierCurrency { get; set; }
        public Decimal TotalAmount { get; set; }
        public int? TimeZoneId { get; set; }
        public decimal? OriginalAmount { get; set; }
        public string Reference { get; set; }
        public List<long> AttachementsFileIds { get; set; }
        public SupplierInvoiceDetails() { }
        public IEnumerable<SupplierInvoiceDetails> GetSupplierInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}
