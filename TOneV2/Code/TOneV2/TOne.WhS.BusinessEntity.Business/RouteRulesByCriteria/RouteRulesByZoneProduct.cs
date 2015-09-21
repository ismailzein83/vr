using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByZoneProduct : RouteRulesByCriteria
    {
        Dictionary<int, Dictionary<long, List<RouteRule>>> _rulesByZoneProduct = new Dictionary<int, Dictionary<long, List<RouteRule>>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.Criteria.RoutingProductId != null && rule.Criteria.HasZoneFilter())
                {
                    Dictionary<long, List<RouteRule>> productRulesByZone = GetOrCreateDictionaryItem(rule.Criteria.RoutingProductId.Value, _rulesByZoneProduct);
                    foreach (var zoneId in rule.Criteria.ZoneIds)
                    {
                        List<RouteRule> zoneRules = GetOrCreateDictionaryItem(zoneId, productRulesByZone);
                        zoneRules.Add(rule);
                    }
                }
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            if (productId != null)
            {
                Dictionary<long, List<RouteRule>> productRulesByZone;
                if (_rulesByZoneProduct.TryGetValue(productId.Value, out productRulesByZone))
                {
                    List<RouteRule> zoneRules;
                    if (productRulesByZone.TryGetValue(saleZoneId, out zoneRules))
                    {
                        foreach (var r in zoneRules)
                        {
                            if (!r.Criteria.IsAnyExcluded(customerId, code, saleZoneId))
                                return r;
                        }
                    }
                }
            }
            return null;
        }
    }
}
