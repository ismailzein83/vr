using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class SattlementInvoiceDetails
    {
        public Decimal CustomerDuration { get; set; }
        public int CustomerTotalNumberOfCalls { get; set; }
        public string PartnerType { get; set; }
        public Decimal SupplierDuration { get; set; }
        public int SupplierTotalNumberOfCalls { get; set; }
        public bool IsApplicableToSupplier { get; set; }
        public bool IsApplicableToCustomer { get; set; }
        public decimal DueToSystemAmount { get; set; }
        public decimal DueToSystemAmountAfterCommission { get; set; }
        public decimal DueToSystemAmountAfterCommissionWithTaxes { get; set; }
        public decimal DueToSystemAmountRecurringCharges { get; set; }
        public decimal DueToSystemTotalTrafficAmount { get; set; }
        public decimal DueToSystemTotalDealAmount { get; set; }
        public decimal DueToSystemTotalSMSAmount { get; set; }

        public decimal DueToCarrierAmount { get; set; }
        public decimal DueToCarrierAmountAfterCommission { get; set; }
        public decimal DueToCarrierAmountAfterCommissionWithTaxes { get; set; }
        public decimal DueToCarrierAmountRecurringCharges { get; set; }
        public decimal DueToCarrierTotalTrafficAmount { get; set; }
        public decimal DueToCarrierTotalDealAmount { get; set; }
        public decimal DueToCarrierTotalSMSAmount { get; set; }

        public decimal DueToSystemDifference { get; set; }
        public decimal DueToCarrierDifference { get; set; }

        public bool NoCarrierVoice { get; set; }
        public bool NoCarrierSMS { get; set; }
        public bool NoCarrierRecurringCharges { get; set; }
        public bool NoCarrierDeals { get; set; }

        public bool NoSystemVoice { get; set; }
        public bool NoSystemSMS { get; set; }
        public bool NoSystemRecurringCharges { get; set; }
        public bool NoSystemDeals { get; set; }

        public SattlementInvoiceDetails() { }
        public IEnumerable<SattlementInvoiceDetails> GetSattlementInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}
