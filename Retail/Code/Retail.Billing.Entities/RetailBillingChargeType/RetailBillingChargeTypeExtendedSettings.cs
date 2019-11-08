using System;
using System.Collections.Generic;

namespace Retail.Billing.Entities
{
    public abstract class RetailBillingChargeTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public Guid? TargetRecordTypeId { get; set; }

        public abstract string RuntimeEditor { get; }

        public abstract Decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context);

        public abstract bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context);
    }

    public interface IRetailBillingChargeTypeCalculateChargeContext
    {
        Guid ComponentTypeId { get; set; }
        RetailBillingCharge Charge { get; }

        Dictionary<string, Object> TargetFieldValues { get; }
    }
    public class RetailBillingChargeTypeCalculateChargeContext: IRetailBillingChargeTypeCalculateChargeContext
    {
        public Guid ComponentTypeId { get; set; }
        public RetailBillingCharge Charge { get; set; }

        public Dictionary<string, Object> TargetFieldValues { get; set; }
    }

    public interface IRetailBillingChargeTypeIsApplicableToTargetContext
    {
        RetailBillingChargeTarget Target { get; }
    }

    public abstract class RetailBillingChargeTarget
    {
        public Guid DataRecordTypeId { get; set; }
    }
}
