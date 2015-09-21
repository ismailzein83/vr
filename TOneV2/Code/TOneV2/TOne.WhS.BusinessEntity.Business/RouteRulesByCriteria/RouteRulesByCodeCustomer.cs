using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCodeCustomer : RouteRulesByCriteria
    {
        Dictionary<int, Dictionary<string, RouteRule>> _rulesByCodeCustomer = new Dictionary<int, Dictionary<string, RouteRule>>();

        public override void SetSource(List<RouteRule> rules)
        {
            foreach(var rule in rules)
            {
                if (rule.Criteria.CustomerIds != null && rule.Criteria.CustomerIds.Count > 0 && rule.Criteria.Codes != null && rule.Criteria.Codes.Count > 0)
                {
                    foreach (var customerId in rule.Criteria.CustomerIds)
                    {
                        Dictionary<string, RouteRule> customerRulesByCode;
                        if(!_rulesByCodeCustomer.TryGetValue(customerId, out customerRulesByCode))
                        {
                            customerRulesByCode = new Dictionary<string, RouteRule>();
                            _rulesByCodeCustomer.Add(customerId, customerRulesByCode);
                        }
                        foreach(var codeCriteria in rule.Criteria.Codes)
                        {
                            if (!customerRulesByCode.ContainsKey(codeCriteria.Code))
                                customerRulesByCode.Add(codeCriteria.Code, rule);
                        }
                    }
                }
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, int saleZoneId)
        {
            RouteRule rule = null;
            if (customerId != null && code != null)
            {
                Dictionary<string, RouteRule> customerRulesByCode;
                if (_rulesByCodeCustomer.TryGetValue(customerId.Value, out customerRulesByCode))
                {
                    customerRulesByCode.TryGetValue(code, out rule);
                }
            }
            return rule;
        }
    }

    
}
