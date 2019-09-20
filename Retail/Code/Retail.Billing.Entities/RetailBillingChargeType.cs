using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class RetailBillingChargeType : Vanrise.Entities.VRComponentType<RetailBillingChargeTypeSettings>
    {
    }

    public class RetailBillingChargeTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId => new Guid("{91DE3029-2EBE-4B7F-8503-E50E614F5832}");

        public RetailBillingChargeTypeExtendedSettings ExtendedSettings { get; set; }
    }
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
