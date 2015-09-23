using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class RouteRulesByTwoIds<T, Q, R> : RouteRulesByCriteria<T> where T : IRouteCriteria
    {
        Dictionary<Q, Dictionary<R, List<T>>> _rulesById1Id2 = new Dictionary<Q, Dictionary<R, List<T>>>();

        public override void SetSource(List<T> rules)
        {
            foreach (var rule in rules)
            {
                IEnumerable<Q> ids1;
                IEnumerable<R> ids2;
                if (IsRuleMatched(rule, out ids1, out ids2))
                {
                    foreach (var id1 in ids1)
                    {
                        Dictionary<R, List<T>> rulesById2 = GetOrCreateDictionaryItem(id1, _rulesById1Id2);
                        foreach (var id2 in ids2)
                        {
                            List<T> codeRules = GetOrCreateDictionaryItem(id2, rulesById2);
                            codeRules.Add(rule);
                        }
                    }
                }
            }
        }

        protected abstract bool IsRuleMatched(T rule, out IEnumerable<Q> ids1, out IEnumerable<R> ids2);
        protected abstract bool AreIdsAvailable(int? customerId, int? productId, string code, long saleZoneId, out Q id1, out R id2);

        public override T GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            Q id1;
            R id2;
            if (AreIdsAvailable(customerId, productId, code, saleZoneId, out id1, out id2))
            {
                Dictionary<R, List<T>> rulesById2;
                if (_rulesById1Id2.TryGetValue(id1, out rulesById2))
                {
                    List<T> codeRules;
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
            return default(T);
        }

        public override bool IsEmpty()
        {
            return _rulesById1Id2.Count == 0;
        }
    }
}
