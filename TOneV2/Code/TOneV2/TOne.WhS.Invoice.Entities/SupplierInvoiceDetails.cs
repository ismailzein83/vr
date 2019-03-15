using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class BaseInvoiceDetails
    {
        public int TotalNumberOfCalls { get; set; }
        public int TotalNumberOfSMS { get; set; }
        public string Offset { get; set; }
        public decimal Duration { get; set; }
        public CommissionType? CommissionType { get; set; }
        public decimal? Commission { get; set; }
        public bool DisplayComission { get; set; }
        public string PartnerType { get; set; }
        public Decimal TotalAmount { get; set; }
        public decimal TotalSMSAmount { get; set; }
        public decimal TotalDealAmount { get; set; }
        public int? TimeZoneId { get; set; }
        public decimal AmountAfterCommission { get; set; }
        public decimal SMSAmountAfterCommission { get; set; }
        public decimal TotalVoiceAmountBeforeTax { get; set; }
        public decimal TotalSMSAmountBeforeTax { get; set; }
        public decimal OriginalAmountAfterCommission { get; set; }
        public decimal SMSOriginalAmountAfterCommission { get; set; }
        public Decimal TotalAmountAfterCommission { get; set; }
        public Decimal TotalSMSAmountAfterCommission { get; set; }

        public Decimal TotalOriginalAmountAfterCommission { get; set; }
        public Decimal TotalSMSOriginalAmountAfterCommission { get; set; }
        public Dictionary<int, OriginalDataCurrrency> OriginalAmountByCurrency { get; set; }
        public string Reference { get; set; }
        public List<AttachementFile> AttachementFiles { get; set; }
        public Decimal TotalReccurringChargesAfterTax { get; set; }
        public Decimal TotalReccurringCharges { get; set; }
        public Decimal TotalInvoiceAmount { get; set; }
        public decimal TotalInvoiceAmountBeforeTax { get; set; }
        public bool NoVoice { get; set; }
        public bool NoSMS { get; set; }
        public bool NoRecurringCharges { get; set; }
        public bool NoDeals { get; set; }

    }
    public class SupplierInvoiceDetails: BaseInvoiceDetails
    {
        public decimal CostAmount { get; set; }
        public Decimal OriginalCostAmount { get; set; }
        public int SupplierCurrencyId { get; set; }
        public string SupplierCurrency { get; set; }
        public int OriginalSupplierCurrencyId { get; set; }
        public string OriginalSupplierCurrency { get; set; }
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
