using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class RouteRulesByOneId<T> : RouteRulesByCriteria
    {
        Dictionary<T, List<RouteRule>> _rulesById = new Dictionary<T, List<RouteRule>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach (var rule in rules)
            {
                IEnumerable<T> ids;
                if (IsRuleMatched(rule, out ids))
                {
                    foreach (var id in ids)
                    {
                        List<RouteRule> zoneRules = GetOrCreateDictionaryItem(id, _rulesById);
                        zoneRules.Add(rule);
                    }
                }
            }
        }

        protected abstract bool IsRuleMatched(RouteRule rule, out IEnumerable<T> ids);

        protected abstract bool IsIdAvailable(int? customerId, int? productId, string code, long saleZoneId, out T id);

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            List<RouteRule> rules;
            T id;
            if (IsIdAvailable(customerId, productId, code, saleZoneId, out id))
            {
                if (_rulesById.TryGetValue(id, out rules))
                {
                    foreach (var r in rules)
                    {
                        if (!RouteRuleManager.IsAnyFilterExcludedInRuleCriteria(r.Criteria, customerId, code, saleZoneId))
                            return r;
                    }
                }
            }
            return null;
        }

        public override bool IsEmpty()
        {
            return _rulesById.Count == 0;
        }
    }
}
