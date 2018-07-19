using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class CustomerInvoiceDetails
    {
        public int TotalNumberOfCalls { get; set; }
        public Decimal SaleAmount { get; set; }
        public Decimal OriginalSaleAmount { get; set; }
        public decimal Duration { get; set; }
        public string PartnerType { get; set; }
        public int SaleCurrencyId { get; set; }
        public string SaleCurrency { get; set; }
        public int OriginalSaleCurrencyId { get; set; }
        public string OriginalSaleCurrency { get; set; }
        public Decimal TotalAmount { get; set; }
        public int? TimeZoneId { get; set; }
        public int CountryId { get; set; }
        public int SupplierId { get; set; }
        public int SupplierZoneId { get; set; }
        public decimal AmountAfterCommission { get; set; }
        public decimal OriginalAmountAfterCommission { get; set; }
        public List<AttachementFile> AttachementFiles { get; set; }
        public string CustomerCurrency { get; set; }
        public Decimal TotalAmountAfterCommission { get; set; }
        public Decimal TotalOriginalAmountAfterCommission { get; set; }
        public CommissionType? CommissionType { get; set; }
        public decimal? Commission { get; set; }
        public bool DisplayComission { get; set; }
        public string Offset { get; set; }
        public string Reference { get; set; }
        public CustomerInvoiceDetails() { }
        public Dictionary<int, OriginalDataCurrrency> OriginalAmountByCurrency { get; set; }

        public IEnumerable<CustomerInvoiceDetails> GetCustomerInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}
