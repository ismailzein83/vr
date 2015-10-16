using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Business.RouteRules
{
    public class RouteRuleExecutionContext : IRouteRuleExecutionContext
    {
        RouteRule _routeRule;
        public RouteRuleExecutionContext(RouteRule routeRule)
        {
            _routeRule = routeRule;
        }

        public List<SupplierCodeMatch> SupplierCodeMatches
        {
            get
            {
                return null;
            }
        }

        public RouteRule RouteRule
        {
            get
            {
                return _routeRule;
            }
        }
    }
}
