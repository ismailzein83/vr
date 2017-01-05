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
        public decimal SaleAmount { get; set; }
        public decimal OriginalSaleAmount { get; set; }
        public decimal Duration { get; set; }
        public int  CustomerId { get; set; }
        public long SaleZoneId { get; set; }
        public long OriginalSaleCurrencyId { get; set; }
        public long SaleCurrencyId { get; set; }
        public string SaleCurrency { get; set; }
        public Decimal SaleRate { get; set; }
        public int? SaleRateTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public CustomerInvoiceItemDetails() { }
        public IEnumerable<CustomerInvoiceItemDetails> GetCustomerInvoiceItemDetailsRDLCSchema()
        {
            return null;
        }
    }
}
