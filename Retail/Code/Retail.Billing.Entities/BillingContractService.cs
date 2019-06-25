using System;

namespace Retail.Billing.Entities
{
    public class BillingContractService
    {
      public long ID { get; set; }
      public Guid ServiceID { get; set; }
      public long ContractID { get; set; }
      public Guid StatusID { get; set; }
      public DateTime? ActivationDate { get; set; }
      public DateTime? SuspensionDate { get; set; }
      public long EffectiveBillingAccountId { get; set; }
      public long RatePlanId { get; set; }
    }
}
