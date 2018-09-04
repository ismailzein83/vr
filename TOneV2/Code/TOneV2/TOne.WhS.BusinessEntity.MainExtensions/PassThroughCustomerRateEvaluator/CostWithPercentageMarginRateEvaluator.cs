using System;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.PassThroughCustomerRateEvaluator
{
    public class CostWithPercentageMarginRateEvaluator : TOne.WhS.BusinessEntity.Entities.PassThroughCustomerRateEvaluator
    {
        public override Guid ConfigId { get { return new Guid("3d4b738e-bf30-4f0c-8a92-d22356980503"); } }

        public int Percentage { get; set; }

        public override decimal? EvaluateCustomerRate(IPassThroughEvaluateCustomerRateContext context)
        {
            if (!context.CostRate.HasValue)
                return null;

            return context.CostRate.Value + (context.CostRate.Value * Percentage / 100);
        }
    }
}