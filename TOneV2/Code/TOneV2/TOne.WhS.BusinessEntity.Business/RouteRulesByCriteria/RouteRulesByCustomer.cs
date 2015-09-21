using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCustomer : RouteRulesByCriteria
    {
        Dictionary<int, List<RouteRule>> _rulesByCustomer = new Dictionary<int, List<RouteRule>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.Criteria.HasCustomerFilter() && !rule.Criteria.HasZoneFilter() && !rule.Criteria.HasCodeFilter())
                {
                    foreach (var customerId in rule.Criteria.CustomerIds)
                    {
                        List<RouteRule> customerRules = GetOrCreateDictionaryItem(customerId, _rulesByCustomer);
                        customerRules.Add(rule);
                    }
                }
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            if (customerId != null)
            {
                List<RouteRule> customerRules;
                if (_rulesByCustomer.TryGetValue(customerId.Value, out customerRules))
                {
                    foreach (var r in customerRules)
                    {
                        if (!r.Criteria.IsAnyExcluded(customerId, code, saleZoneId))
                            return r;
                    }
                }
            }
            return null;
        }
    }
}
