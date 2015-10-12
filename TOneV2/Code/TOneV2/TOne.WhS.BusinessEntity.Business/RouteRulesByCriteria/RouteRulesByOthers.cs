using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByOthers<T> : RouteRulesByCriteria<T> where T : IRouteCriteria
    {
        List<T> _rulesByOthers = new List<T>();

        public override void SetSource(List<T> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.Criteria.RoutingProductId == null && !rule.Criteria.HasCustomerFilter() && !rule.Criteria.HasZoneFilter() && !rule.Criteria.HasCodeFilter())
                {
                    _rulesByOthers.Add(rule);
                }
            }
        }

        public override T GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            foreach (var r in _rulesByOthers)
            {
                if (!RouteRuleManager.IsAnyFilterExcludedInRuleCriteria(r.Criteria, customerId, code, saleZoneId))
                    return r;
            }
            return default(T);
        }

        public override bool IsEmpty()
        {
            return _rulesByOthers.Count == 0;
        }
    }
}
