using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class RouteRulesByTwoIds<T, Q> : RouteRulesByCriteria
    {
        Dictionary<T, Dictionary<Q, List<IRouteCriteria>>> _rulesById1Id2 = new Dictionary<T, Dictionary<Q, List<IRouteCriteria>>>();

        public override void SetSource(List<IRouteCriteria> rules)
        {
            foreach (var rule in rules)
            {
                IEnumerable<T> ids1;
                IEnumerable<Q> ids2;
                if (IsRuleMatched(rule, out ids1, out ids2))
                {
                    foreach (var id1 in ids1)
                    {
                        Dictionary<Q, List<IRouteCriteria>> rulesById2 = GetOrCreateDictionaryItem(id1, _rulesById1Id2);
                        foreach (var id2 in ids2)
                        {
                            List<IRouteCriteria> codeRules = GetOrCreateDictionaryItem(id2, rulesById2);
                            codeRules.Add(rule);
                        }
                    }
                }
            }
        }

        protected abstract bool IsRuleMatched(IRouteCriteria rule, out IEnumerable<T> ids1, out IEnumerable<Q> ids2);
        protected abstract bool AreIdsAvailable(int? customerId, int? productId, string code, long saleZoneId, out T id1, out Q id2);

        public override IRouteCriteria GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            T id1;
            Q id2;
            if (AreIdsAvailable(customerId, productId, code, saleZoneId, out id1, out id2))
            {
                Dictionary<Q, List<IRouteCriteria>> rulesById2;
                if (_rulesById1Id2.TryGetValue(id1, out rulesById2))
                {
                    List<IRouteCriteria> codeRules;
                    if (rulesById2.TryGetValue(id2, out codeRules))
                    {
                        foreach (var r in codeRules)
                        {
                            if (!RouteRuleManager.IsAnyFilterExcludedInRuleCriteria(r.RouteCriteria, customerId, code, saleZoneId))
                            {
                                return r;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public override bool IsEmpty()
        {
            return _rulesById1Id2.Count == 0;
        }
    }
}
