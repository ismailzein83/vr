using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class BillingInvoiceItem
    {
        public Guid ServiceID { get; set; }
        public String ServiceIDDescription { get; set; }


        public long ContractID { get; set; }
        public String ContractIDDescription { get; set; }

        public long RatePlanId { get; set; }
        public String RatePlanIdDescription { get; set; }

        public decimal? TotalAmount { get; set; }
        public decimal? ActivationFee { get; set; }
        public decimal? RecurringFee { get; set; }
        public decimal? SuspensionCharge { get; set; }
        public decimal? SuspensionRecurringCharge { get; set; }

        public int CurrencyId { get; set; }
        public String CurrencyIdDescription { get; set; }

        public IEnumerable<BillingInvoiceItem> GetInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }

    public struct ContractServiceRatePlanCurrency
    {
        public long ContractID { get; set; }
        public Guid ServiceID { get; set; }
        public long RatePlanId { get; set; }
        public int CurrencyId { get; set; }
        public override int GetHashCode()
        {
            return this.ContractID.GetHashCode() + this.ServiceID.GetHashCode() + this.RatePlanId.GetHashCode() + this.CurrencyId.GetHashCode();
        }
    }

}
