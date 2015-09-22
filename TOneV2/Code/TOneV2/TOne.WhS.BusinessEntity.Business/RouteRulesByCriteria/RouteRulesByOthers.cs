using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByOthers : RouteRulesByCriteria
    {
        List<RouteRule> _rulesByOthers = new List<RouteRule>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.Criteria.RoutingProductId == null && !rule.Criteria.HasCustomerFilter() && !rule.Criteria.HasZoneFilter() && !rule.Criteria.HasCodeFilter())
                {
                    _rulesByOthers.Add(rule);
                }
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            foreach (var r in _rulesByOthers)
            {
                if (!r.Criteria.IsAnyExcluded(customerId, code, saleZoneId))
                    return r;
            }
            return null;
        }

        public override bool IsEmpty()
        {
            return _rulesByOthers.Count == 0;
        }
    }
}
