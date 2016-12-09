using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class RDLCCustomerInvoiceItemDetails
    {
        public int CustomerId { get; set; }
        public string CustomerIdDescription { get; set; }
        public long SaleZoneId { get; set; }
        public string SaleZoneIdDescription { get; set; }
        public long OriginalSaleCurrencyId { get; set; }
        public string OriginalSaleCurrencyIdDescription { get; set; }
        public long SaleCurrencyId { get; set; }
        public string SaleCurrencyIdDescription { get; set; }
        public int SaleRateTypeId { get; set; }
        public string SaleRateTypeIdDescription { get; set; }
        public Decimal SaleRate { get; set; }
        public string SaleRateDescription { get; set; }

        public int NumberOfCalls { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal OriginalSaleAmount { get; set; }
        public decimal Duration { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public RDLCCustomerInvoiceItemDetails() { }
        public IEnumerable<RDLCCustomerInvoiceItemDetails> GetRDLCCustomerInvoiceItemDetailsSchema()
        {
            return null;
        }
    }
}
