using System;
using System.Collections.Generic;

namespace Retail.Interconnect.Entities
{
    public class SettlementInvoiceDetails
    {
        public Decimal CustomerDuration { get; set; }
        public int CustomerTotalNumberOfCalls { get; set; }
        public int CustomerTotalNumberOfSMSs { get; set; }
        public bool IsApplicableToCustomer { get; set; }
        public decimal DueToSystemAmount { get; set; }
        public decimal DueToSystemAmountWithTaxes { get; set; }
        public decimal DueToSystemAmountRecurringCharges { get; set; }
        public decimal DueToSystemTotalTrafficAmount { get; set; }
        public decimal DueToSystemTotalSMSAmount { get; set; }
        public decimal DueToSystemFullAmount { get; set; }
        public decimal DueToSystemDifference { get; set; }
        public bool NoSystemVoice { get; set; }
        public bool NoSystemSMS { get; set; }
        public bool NoSystemRecurringCharges { get; set; }

        public Decimal SupplierDuration { get; set; }
        public int SupplierTotalNumberOfCalls { get; set; }
        public int SupplierTotalNumberOfSMSs { get; set; }
        public bool IsApplicableToSupplier { get; set; }
        public decimal DueToCompanyAmount { get; set; }
        public decimal DueToCompanyAmountWithTaxes { get; set; }
        public decimal DueToCompanyAmountRecurringCharges { get; set; }
        public decimal DueToCompanyTotalTrafficAmount { get; set; }
        public decimal DueToCompanyTotalSMSAmount { get; set; }
        public decimal DueToCompanyFullAmount { get; set; }
        public decimal DueToCompanyDifference { get; set; }
        public bool NoCompanyVoice { get; set; }
        public bool NoCompanySMS { get; set; }
        public bool NoCompanyRecurringCharges { get; set; }

        public bool IsOriginalAmountSetted { get; set; }

        public SettlementInvoiceDetails()
        {
        }

        public IEnumerable<SettlementInvoiceDetails> GetSettlementInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}