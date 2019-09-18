using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public abstract class RetailBillingChargeType
    {
        public abstract Guid ConfigId { get; }

        public Guid? TargetRecordTypeId { get; set; }

        public abstract Decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context);

        public abstract bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context);
    }

    public interface IRetailBillingChargeTypeCalculateChargeContext
    {
        RetailBillingCharge Charge { get; }

        Dictionary<string, Object> TargetFieldValues { get; }
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
