using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByZoneCustomer : RouteRulesByCriteria
    {
        Dictionary<int, Dictionary<long, List<RouteRule>>> _rulesByZoneCustomer = new Dictionary<int, Dictionary<long, List<RouteRule>>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.Criteria.HasCustomerFilter() && rule.Criteria.HasZoneFilter())
                {
                    foreach (var customerId in rule.Criteria.CustomerIds)
                    {
                        Dictionary<long, List<RouteRule>> customerRulesByZone = GetOrCreateDictionaryItem(customerId, _rulesByZoneCustomer);
                        foreach (var zoneId in rule.Criteria.ZoneIds)
                        {
                            List<RouteRule> zoneRules = GetOrCreateDictionaryItem(zoneId, customerRulesByZone);
                            zoneRules.Add(rule);
                        }
                    }
                }
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            if (customerId != null)
            {
                Dictionary<long, List<RouteRule>> customerRulesByZone;
                if (_rulesByZoneCustomer.TryGetValue(customerId.Value, out customerRulesByZone))
                {
                    List<RouteRule> zoneRules;
                    if (customerRulesByZone.TryGetValue(saleZoneId, out zoneRules))
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
