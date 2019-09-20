using Retail.Billing.Entities;
using System;

namespace Retail.Billing.MainExtensions.RetailBillingChargeType
{
    public class RetailBillingCustomCodeChargeType : RetailBillingChargeTypeExtendedSettings
    {
        public override Guid ConfigId => new Guid("049FB2B2-DB88-4F04-8B8B-69688D4CAB5A");

        public Guid? ChargeSettingsRecordTypeId { get; set; }

        /// <summary>
        /// needs to be of type GenericData GenericEditor
        /// </summary>
        public string ChargeSettingsGenericEditor { get; set; }

        public string PricingLogic { get; set; }

        public override string RuntimeEditor => throw new NotImplementedException();

        public override decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context)
        {
            throw new NotImplementedException();
        }
    }
}
