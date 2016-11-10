using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class CustomerInvoiceItemDetails
    {
        public int NumberOfCalls { get; set; }
        public Double SaleAmount { get; set; }
        public decimal Duration { get; set; }
        public string DimensionName { get; set; }
        public long DimensionId { get; set; }
        public string SaleCurrency { get; set; }
        public long SaleCurrencyId { get; set; }
        public Decimal SaleRate { get; set; }
        public int SaleRateTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public CustomerInvoiceItemDetails() { }
        public IEnumerable<CustomerInvoiceItemDetails> GetCustomerInvoiceItemDetailsRDLCSchema()
        {
            return null;
        }
    }
}
