using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesBySubCode : RouteRulesByCriteria
    {
        Dictionary<string, List<RouteRule>> _rulesByCode = new Dictionary<string, List<RouteRule>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.Criteria.HasCodeFilter() && !rule.Criteria.HasCustomerFilter())
                {
                    foreach (var codeCriteria in rule.Criteria.Codes)
                    {
                        if (codeCriteria.WithSubCodes)
                        {
                            List<RouteRule> codeRules = GetOrCreateDictionaryItem(codeCriteria.Code, _rulesByCode);
                            codeRules.Add(rule);
                        }
                    }
                }
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            if (code != null)
            {
                StringBuilder codeIterator = new StringBuilder(code);
                while (codeIterator.Length > 1)
                {
                    List<RouteRule> codeRules;
                    string parentCode = codeIterator.ToString();
                    if (_rulesByCode.TryGetValue(parentCode, out codeRules))
                    {
                        foreach (var r in codeRules)
                        {
                            if (!r.Criteria.IsAnyExcluded(customerId, code, saleZoneId))
                            {
                                return r;
                            }
                        }
                    }
                    codeIterator.Remove(codeIterator.Length - 1, 1);
                }
            }
            return null;
        }
    }
}
