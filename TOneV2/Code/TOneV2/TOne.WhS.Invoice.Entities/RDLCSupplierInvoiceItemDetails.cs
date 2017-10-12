using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class RDLCSupplierInvoiceItemDetails
    {
        public int SupplierId { get; set; }
        public string SupplierIdDescription { get; set; }
        public long SupplierZoneId { get; set; }
        public string SupplierZoneIdDescription { get; set; }
        public long OriginalSupplierCurrencyId { get; set; }
        public string OriginalSupplierCurrencyIdDescription { get; set; }
        public long SupplierCurrencyId { get; set; }
        public string SupplierCurrencyIdDescription { get; set; }
        public int? SupplierRateTypeId { get; set; }
        public string SupplierRateTypeIdDescription { get; set; }
        public Decimal SupplierRate { get; set; }
        public string SupplierRateDescription { get; set; }

        public int NumberOfCalls { get; set; }
        public decimal CostAmount { get; set; }
        public decimal OriginalCostAmount { get; set; }
        public decimal Duration { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Decimal SaleRateAfterCommission { get; set; }
        public decimal AmountAfterCommission { get; set; }
        public RDLCSupplierInvoiceItemDetails() { }
        public IEnumerable<RDLCSupplierInvoiceItemDetails> GetRDLCSupplierInvoiceItemDetailsSchema()
        {
            return null;
        }
    }
}
