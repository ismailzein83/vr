using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class RouteRulesByOneId<T,Q> : RouteRulesByCriteria<T> where T : IRouteCriteria
    {
        Dictionary<Q, List<T>> _rulesById = new Dictionary<Q, List<T>>();

        public override void SetSource(List<T> rules)
        {
            foreach (var rule in rules)
            {
                IEnumerable<Q> ids;
                if (IsRuleMatched(rule, out ids))
                {
                    foreach (var id in ids)
                    {
                        List<T> zoneRules = _rulesById.GetOrCreateItem(id);
                        zoneRules.Add(rule);
                    }
                }
            }
        }

        protected abstract bool IsRuleMatched(T rule, out IEnumerable<Q> ids);

        protected abstract bool IsIdAvailable(int? customerId, int? productId, string code, long saleZoneId, out Q id);

        public override T GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            List<T> rules;
            Q id;
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
            return default(T);
        }

        public override bool IsEmpty()
        {
            return _rulesById.Count == 0;
        }
    }
}
