﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCodeCustomer : RouteRulesByCriteria
    {
        Dictionary<int, Dictionary<string, List<RouteRule>>> _rulesByCodeCustomer = new Dictionary<int, Dictionary<string, List<RouteRule>>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach(var rule in rules)
            {
                if (rule.Criteria.HasCustomerFilter() && rule.Criteria.HasCodeFilter())
                {
                    foreach (var customerId in rule.Criteria.CustomerIds)
                    {
                        Dictionary<string, List<RouteRule>> customerRulesByCode = GetOrCreateDictionaryItem(customerId, _rulesByCodeCustomer);
                        foreach(var codeCriteria in rule.Criteria.Codes)
                        {
                            List<RouteRule> codeRules = GetOrCreateDictionaryItem(codeCriteria.Code, customerRulesByCode);
                            codeRules.Add(rule);
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
                    List<RouteRule> codeRules;
                    if (customerRulesByCode.TryGetValue(code, out codeRules))
                    {
                        foreach (var r in codeRules)
                        {
                            if (!r.Criteria.IsAnyExcluded(customerId, code, saleZoneId))
                            {
                                return r;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
