using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.MainExtensions.SwapDeal.RouteRuleCriteria;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions.SwapDeal
{
    public class SwapDealRouteRuleDefinitionFilter : IRouteRuleFilter
    {
        public int SwapDealId { get; set; }

        public bool IsMatched(IRouteRuleFilterContext context)
        {
            context.RouteRule.ThrowIfNull("context.RouteRule");
            context.RouteRule.Criteria.ThrowIfNull("context.RouteRule.Criteria");
            SwapDealRouteRuleCriteria criteria = context.RouteRule.Criteria as SwapDealRouteRuleCriteria;

            if (criteria == null)
                return false;

            if (criteria.SwapDealId != SwapDealId)
                return false;

            return true;
        }
    }
}