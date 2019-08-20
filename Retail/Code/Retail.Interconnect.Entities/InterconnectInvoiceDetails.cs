using System.Collections.Generic;

namespace Retail.Interconnect.Entities
{
    public class InterconnectInvoiceDetails
    {
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SMSAmount { get; set; }
        public decimal AmountWithTaxes { get; set; }
        public decimal SMSAmountWithTaxes { get; set; }
        public decimal TotalAmountWithTaxes { get; set; }
        public decimal TotalAmountBeforeTaxes { get; set; }
        public decimal Duration { get; set; }
        public int InterconnectCurrencyId { get; set; }
        public string InterconnectCurrency { get; set; }
        public int TotalNumberOfCalls { get; set; }
        public int TotalNumberOfSMS { get; set; }
        public decimal TotalRecurringChargesAfterTaxes { get; set; }
        public decimal TotalRecurringCharges { get; set; }
        public decimal TotalInvoiceAmount { get; set; }
		public bool NoSMS { get; set; }
		public bool NoVoice { get; set; }
		public bool NoRecurringCharges { get; set; }

        public int OriginalCurrencyId { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal OriginalAmountWithTaxes { get; set; }
        public Dictionary<int, OriginalDataCurrrency> OriginalAmountByCurrency { get; set; }
        public List<AttachementFile> AttachementFiles { get; set; }
        public string Reference { get; set; }
        public bool IsOriginalAmountSetted { get; set; }

        public InterconnectInvoiceDetails()
        {
        }

        public IEnumerable<InterconnectInvoiceDetails> GetInterconnectInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}