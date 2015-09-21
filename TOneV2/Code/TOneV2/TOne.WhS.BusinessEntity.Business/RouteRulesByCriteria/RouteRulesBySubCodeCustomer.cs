using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesBySubCodeCustomer : RouteRulesByCriteria
    {
        Dictionary<int, Dictionary<string, List<RouteRule>>> _rulesByCodeCustomer = new Dictionary<int, Dictionary<string, List<RouteRule>>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.Criteria.HasCustomerFilter() && rule.Criteria.HasCodeFilter())
                {
                    foreach (var customerId in rule.Criteria.CustomerIds)
                    {
                        Dictionary<string, List<RouteRule>> customerRulesByCode = null;
                        
                        foreach (var codeCriteria in rule.Criteria.Codes)
                        {
                            if (codeCriteria.WithSubCodes)
                            {
                                if (customerRulesByCode == null)
                                {
                                    customerRulesByCode = GetOrCreateDictionaryItem(customerId, _rulesByCodeCustomer);
                                }
                                List<RouteRule> codeRules = GetOrCreateDictionaryItem(codeCriteria.Code, customerRulesByCode);
                                codeRules.Add(rule);
                            }
                            
                        }
                    }
                }
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            if (customerId != null && code != null)
            {
                Dictionary<string, List<RouteRule>> customerRulesByCode;
                if (_rulesByCodeCustomer.TryGetValue(customerId.Value, out customerRulesByCode))
                {
                    StringBuilder codeIterator = new StringBuilder(code);
                    while (codeIterator.Length > 1)
                    {
                        string parentCode = codeIterator.ToString();
                        List<RouteRule> codeRules;
                        if (customerRulesByCode.TryGetValue(parentCode, out codeRules))
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
            }
            return null;
        }
    }
}
