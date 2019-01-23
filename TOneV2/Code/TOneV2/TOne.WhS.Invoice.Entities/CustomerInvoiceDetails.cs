using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class CustomerInvoiceDetails : BaseInvoiceDetails
    {
        public Decimal SaleAmount { get; set; }
        public Decimal OriginalSaleAmount { get; set; }
        public int SaleCurrencyId { get; set; }
        public string SaleCurrency { get; set; }
        public int OriginalSaleCurrencyId { get; set; }
        public string OriginalSaleCurrency { get; set; }
        public int CountryId { get; set; }
        public int SupplierId { get; set; }
        public int SupplierZoneId { get; set; }
        public string CustomerCurrency { get; set; }
        public CustomerInvoiceDetails() { }
        public IEnumerable<CustomerInvoiceDetails> GetCustomerInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}
