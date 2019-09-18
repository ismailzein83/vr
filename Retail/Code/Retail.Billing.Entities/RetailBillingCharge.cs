using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public abstract class RetailBillingCharge
    {
        public Guid RetailBillingChargeTypeId { get; set; }
    }

    #region Implementations (need to be moved to MainExtensions)

    public class RetailBillingCustomCodeChargeType : RetailBillingChargeType
    {
        public override Guid ConfigId => new Guid("049FB2B2-DB88-4F04-8B8B-69688D4CAB5A");

        public Guid? ChargeSettingsRecordTypeId { get; set; }

        /// <summary>
        /// needs to be of type GenericData GenericEditor
        /// </summary>
        public string ChargeSettingsGenericEditor { get; set; }

        public string PricingLogic { get; set; }

        public override decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class RetailBillingCustomCodeCharge : RetailBillingCharge
    {
        public Dictionary<string, Object> FieldValues { get; set; }
    }

    public class RetailBillingAnalyticQueryChargeType : RetailBillingChargeType
    {
        public override Guid ConfigId => new Guid("E611AADC-30EE-488A-894D-526EB922793A");

        public Guid AnalyticTableId { get; set; }

        public string BillingAccountDimensionName { get; set; }

        public string AmountMeasureName { get; set; }

        public string CurrencyMeasureName { get; set; }

        public override decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class RetailBillingAnalyticQueryCharge : RetailBillingCharge
    {
        /// <summary>
        /// needs to be of type Generic RecordFilterGroup
        /// </summary>
        public string FilterGroup { get; set; }
    }

    #endregion
}
