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
        public StructuredRouteRules StructureRules(List<RouteRule> rules)
        {
            StructuredRouteRules structuredRouteRules = new StructuredRouteRules { RouteRulesByCriteria = new List<RouteRulesByCriteria>() };
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesByCustomerCode());
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesByCustomerSubCode());
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesByCode());
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesBySubCode());
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesByCustomerZone());
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesByProductZone());
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesByZone());
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesByCustomer());
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesByProduct());
            structuredRouteRules.RouteRulesByCriteria.Add(new RouteRulesByOthers());
            foreach (var r in structuredRouteRules.RouteRulesByCriteria)
                r.SetSource(rules);
            return structuredRouteRules;
        }
    }
}
