using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByZone : RouteRulesByCriteria
    {
        Dictionary<long, List<RouteRule>> _rulesByZone = new Dictionary<long, List<RouteRule>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.Criteria.HasZoneFilter() && rule.Criteria.RoutingProductId == null && !rule.Criteria.HasCustomerFilter())
                {
                    foreach (var zoneId in rule.Criteria.ZoneIds)
                    {
                        List<RouteRule> zoneRules = GetOrCreateDictionaryItem(zoneId, _rulesByZone);
                        zoneRules.Add(rule);
                    }
                }
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            List<RouteRule> zoneRules;
            if (_rulesByZone.TryGetValue(saleZoneId, out zoneRules))
            {
                foreach (var r in zoneRules)
                {
                    if (!r.Criteria.IsAnyExcluded(customerId, code, saleZoneId))
                        return r;
                }
            }
            return null;
        }
    }
}
