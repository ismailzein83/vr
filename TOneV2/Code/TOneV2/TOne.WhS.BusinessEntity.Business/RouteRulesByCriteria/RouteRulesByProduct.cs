using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByProduct : RouteRulesByCriteria
    {
        Dictionary<int, List<RouteRule>> _rulesByProduct = new Dictionary<int, List<RouteRule>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.Criteria.RoutingProductId != null && !rule.Criteria.HasZoneFilter())
                {
                    List<RouteRule> productRules = GetOrCreateDictionaryItem(rule.Criteria.RoutingProductId.Value, _rulesByProduct);
                    productRules.Add(rule);
                }
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            if (productId != null)
            {
                List<RouteRule> productRules;
                if (_rulesByProduct.TryGetValue(productId.Value, out productRules))
                {
                    foreach (var r in productRules)
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
