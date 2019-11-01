using Retail.Billing.Entities;
using System;

namespace Retail.Billing.MainExtensions.RetailBillingChargeType
{
    public class RetailBillingAnalyticQueryChargeType : RetailBillingChargeTypeExtendedSettings
    {
        public override Guid ConfigId => new Guid("E611AADC-30EE-488A-894D-526EB922793A");

        public Guid AnalyticTableId { get; set; }

        public string BillingAccountDimensionName { get; set; }

        public string AmountMeasureName { get; set; }

        public string CurrencyMeasureName { get; set; }

        public override string RuntimeEditor => throw new NotImplementedException();

        public override decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context)
        {
            return 1;
        }

        public override bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context)
        {
            throw new NotImplementedException();
        }
    }
}
