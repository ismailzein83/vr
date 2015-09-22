using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRuleManager
    {
        public StructuredRouteRules StructureRules(List<IRouteCriteria> rules)
        {
            List<RouteRulesByCriteria> routeRulesByCriteria = new List<RouteRulesByCriteria>();
            routeRulesByCriteria.Add(new RouteRulesByCustomerCode());
            routeRulesByCriteria.Add(new RouteRulesByCustomerSubCode());
            routeRulesByCriteria.Add(new RouteRulesByCode());
            routeRulesByCriteria.Add(new RouteRulesBySubCode());
            routeRulesByCriteria.Add(new RouteRulesByCustomerZone());
            routeRulesByCriteria.Add(new RouteRulesByProductZone());
            routeRulesByCriteria.Add(new RouteRulesByZone());
            routeRulesByCriteria.Add(new RouteRulesByCustomer());
            routeRulesByCriteria.Add(new RouteRulesByProduct());
            routeRulesByCriteria.Add(new RouteRulesByOthers());
            StructuredRouteRules structuredRouteRules = new StructuredRouteRules();
            RouteRulesByCriteria current = null;
            foreach (var r in routeRulesByCriteria)
            {
                r.SetSource(rules);
                if (!r.IsEmpty())
                {
                    if (current != null)
                        current.NextRuleSet = r;
                    else
                        structuredRouteRules.FirstRuleSet = r;
                    current = r;
                }                    
            }
            return structuredRouteRules;
        }

        public IRouteCriteria GetMostMatchedRule(StructuredRouteRules rules, int? customerId, int? productId, string code, long saleZoneId)
        {
            return GetMostMatchedRule(rules.FirstRuleSet, customerId, productId, code, saleZoneId);
        }

        public IRouteCriteria GetMostMatchedRule(RouteRulesByCriteria routeRulesByCriteria, int? customerId, int? productId, string code, long saleZoneId)
        {
            if (routeRulesByCriteria == null)
                return null;
            IRouteCriteria rule = routeRulesByCriteria.GetMostMatchedRule(customerId, productId, code, saleZoneId);
            if (rule != null)
                return rule;
            else
                return GetMostMatchedRule(routeRulesByCriteria.NextRuleSet, customerId, productId, code, saleZoneId);
        }

        public static bool IsAnyFilterExcludedInRuleCriteria(RouteCriteria ruleCriteria, int? customerId, string code, long zoneId)
        {
            if(customerId.HasValue)
            {
                if(ruleCriteria.CustomersGroupConfigId.HasValue && ruleCriteria.CustomerGroupSettings != null && ruleCriteria.CustomerGroupSettings.IsAllExcept)
                {
                    CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                    IEnumerable<int> excludedCustomerIds = carrierAccountManager.GetCustomerIds(ruleCriteria.CustomersGroupConfigId.Value, ruleCriteria.CustomerGroupSettings);
                    if (excludedCustomerIds != null && excludedCustomerIds.Contains(customerId.Value))
                        return true;
                }
            }
            if (code != null && ruleCriteria.ExcludedCodes != null && ruleCriteria.ExcludedCodes.Contains(code))
                return true;
            return false;
            //return  IsItemInList(zoneId, ruleCriteria.ExcludedZoneIds);
        }

        static bool IsItemInList<T>(T item, List<T> list)
        {
            return item != null && list != null && list.Contains(item);
        }
    }
}
