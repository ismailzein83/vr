using Retail.BusinessEntity.Entities;
using System;

namespace Retail.Billing.Entities
{
    public class BillingRatePlanService
    {
        public long ID { get; set; }
        public Guid ServiceID { get; set; }
        public long RatePlanId { get; set; }
        public RetailBEChargeEntity ActivationFee { get; set; }
        public RetailBEChargeEntity RecurringFee { get; set; }
        public RetailBEChargeEntity SuspensionCharge { get; set; }
        public RetailBEChargeEntity SuspensionRecurringCharge { get; set; }
        public RetailBEChargeEntity DeactivationCharge { get; set; }
        public RetailBEChargeEntity Deposit { get; set; }
        public RetailBEChargeEntity BankGuarantee { get; set; }
    }
}
