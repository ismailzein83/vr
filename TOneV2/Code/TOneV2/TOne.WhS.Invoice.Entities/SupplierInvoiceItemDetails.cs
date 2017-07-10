using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class SupplierInvoiceItemDetails
    {
       public int NumberOfCalls { get; set; }
       public Double SupplierAmount { get; set; }
        public decimal Duration { get; set; }
        public string DimensionName { get; set; }
        public decimal SupplierRate { get; set; }
        public string SupplierCurrency { get; set; }
        public decimal CostAmount { get; set; }
        public decimal OriginalCostAmount { get; set; }
        public int SupplierId { get; set; }
        public long SupplierZoneId { get; set; }
        public long OriginalSupplierCurrencyId { get; set; }
        public long SupplierCurrencyId { get; set; }
        public int? SupplierRateTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public SupplierInvoiceItemDetails() { }
        public IEnumerable<SupplierInvoiceItemDetails> GetSupplierInvoiceItemDetailsRDLCSchema()
        {
            return null;
        }
    }
}
