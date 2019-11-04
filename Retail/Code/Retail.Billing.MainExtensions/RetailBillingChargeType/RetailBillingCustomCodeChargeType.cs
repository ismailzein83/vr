using Retail.Billing.Entities;
using Retail.Billing.MainExtensions.RetailBillingCharge;
using System;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.MainExtensions.RetailBillingChargeType
{
    public class RetailBillingCustomCodeChargeType : RetailBillingChargeTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("049FB2B2-DB88-4F04-8B8B-69688D4CAB5A"); } }

        public Guid? ChargeSettingsRecordTypeId { get; set; }

        public VRGenericEditorDefinitionSetting ChargeSettingsEditorDefinition { get; set; }

        public string PricingLogic { get; set; }

        public override string RuntimeEditor { get { return "retail-billing-charge-customcode"; } }

        public override decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context)
        {
            RetailBillingCustomCodeCharge retailBillingCustomCodeCharge = context.Charge as RetailBillingCustomCodeCharge;
            return PricingLogic.GetHashCode() + retailBillingCustomCodeCharge.GetHashCode();
        }

        public override bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context)
        {
            throw new NotImplementedException();
        }
    }
}