using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public abstract class SwapDealBuyRouteRuleExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Evaluate(ISwapDealBuyRouteRuleEvaluateContext context);

        public abstract string GetDescription();
    }

    public interface ISwapDealBuyRouteRuleEvaluateContext
    {
        List<SwapDealBuyRouteRuleEvaluationItem> EvaluationItems { set; }

        List<long> SaleZoneIds { get; }
    }

    public class SwapDealBuyRouteRuleEvaluationItem
    {
        public int CustomerId { get; set; }

        public long SaleZoneId { get; set; }

        public Decimal Percentage { get; set; }
    }
}
