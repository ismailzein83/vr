using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.PassThroughCustomerRateEvaluator
{
    public class CostWithPercentageMarginRateEvaluator : TOne.WhS.BusinessEntity.Entities.PassThroughCustomerRateEvaluator
    {
        public int Percentage { get; set; }

        public override decimal? EvaluateCustomerRate(IPassThroughEvaluateCustomerRateContext context)
        {
            if (!context.CostRate.HasValue)
                return null;

            return context.CostRate.Value + (context.CostRate.Value * Percentage / 100);
        }
    }
}
