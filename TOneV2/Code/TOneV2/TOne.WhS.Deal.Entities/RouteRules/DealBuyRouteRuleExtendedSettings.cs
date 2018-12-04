using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public abstract class DealBuyRouteRuleExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Evaluate(IDealBuyRouteRuleEvaluateContext context);

        public abstract string GetDescription();
    }

    public interface IDealBuyRouteRuleEvaluateContext
    {
        List<DealBuyRouteRuleEvaluationItem> EvaluationItems { set; }

        List<long> SaleZoneIds { get; }
    }

    public class DealBuyRouteRuleEvaluationItem
    {
        public int CustomerId { get; set; }

        public long SaleZoneId { get; set; }

        public Decimal Percentage { get; set; }
    }
}
