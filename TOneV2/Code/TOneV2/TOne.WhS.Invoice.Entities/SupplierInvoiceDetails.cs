using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

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
        public string Reference { get; set; }
        public List<AttachementFile> AttachementFiles { get; set; }
        public decimal AmountAfterCommission { get; set; }
        public decimal OriginalAmountAfterCommission { get; set; }
        public Decimal TotalAmountAfterCommission { get; set; }
        public Decimal TotalOriginalAmountAfterCommission { get; set; }
        public CommissionType? CommissionType { get; set; }

        public decimal? Commission { get; set; }
        public bool DisplayComission { get; set; }
        public string Offset { get; set; }

        public Dictionary<int, OriginalDataCurrrency> OriginalAmountByCurrency { get; set; }

        public SupplierInvoiceDetails() { }
        public IEnumerable<SupplierInvoiceDetails> GetSupplierInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
    public class AttachementFile
    {
        public long FileId { get; set; }
    }
}
