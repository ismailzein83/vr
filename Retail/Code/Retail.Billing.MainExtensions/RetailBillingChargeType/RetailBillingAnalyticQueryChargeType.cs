using Retail.Billing.Entities;
using System;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.MainExtensions.RetailBillingChargeType
{
    public class RetailBillingAnalyticQueryChargeType : RetailBillingChargeTypeExtendedSettings
    {
        public override Guid ConfigId => new Guid("E611AADC-30EE-488A-894D-526EB922793A");

        public Guid AnalyticTableId { get; set; }

        public string BillingAccountDimensionName { get; set; }

        public string AmountMeasureName { get; set; }

        public string CurrencyMeasureName { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public override string RuntimeEditor { get { return "retail-billing-charge-analyticquery"; } }

        public override decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context)
        {
            context.BillingAccountId.ThrowIfNull("context.BillingAccountId");

            if (!context.FromTime.HasValue)
                throw new NullReferenceException("context.FromTime");

            if (!context.ToTime.HasValue)
                throw new NullReferenceException("context.ToTime");

            AnalyticManager anayticManager = new AnalyticManager();

            var filters = new List<DimensionFilter>();
            var dimensionFilter = new DimensionFilter()
            {
                Dimension = BillingAccountDimensionName,
                FilterValues = new List<object>() { context.BillingAccountId }
            };
            filters.Add(dimensionFilter);

            var analyticQuery = new AnalyticQuery()
            {
                TableId = AnalyticTableId,
                MeasureFields = new List<string> { AmountMeasureName, CurrencyMeasureName },
                FromTime=context.FromTime.Value,
                ToTime=context.ToTime.Value,
                FilterGroup = FilterGroup,
                Filters = filters
            };

            var records = anayticManager.GetAllFilteredRecords(analyticQuery);
            if (records != null && records.Count > 0)
            {
                var record = records[0];
                if (record.MeasureValues != null)
                {
                    var amount = record.MeasureValues.GetRecord(AmountMeasureName);
                    if (amount != null && amount.Value != null)
                        return (decimal)amount.Value;
                }
            }
            return 0;
        }
        public override string GetDescription(IRetailBillingChargeTypeGetDescriptionContext context)
        {
            throw new NotImplementedException();
        }
        public override bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context)
        {
            throw new NotImplementedException();
        }
    }
}
