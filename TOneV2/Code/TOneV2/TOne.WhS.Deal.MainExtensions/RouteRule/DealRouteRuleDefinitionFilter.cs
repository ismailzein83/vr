using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.MainExtensions.RouteRule.RouteRuleCriteria;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions.RouteRule
{
    public class DealRouteRuleDefinitionFilter : IRouteRuleFilter
    {
        public int DealId { get; set; }

        public bool IsMatched(IRouteRuleFilterContext context)
        {
            context.RouteRule.ThrowIfNull("context.RouteRule");
            context.RouteRule.Criteria.ThrowIfNull("context.RouteRule.Criteria");
            DealRouteRuleCriteria criteria = context.RouteRule.Criteria as DealRouteRuleCriteria;

            if (criteria == null)
                return false;

            if (criteria.DealId != DealId)
                return false;

            return true;
        }
    }
}