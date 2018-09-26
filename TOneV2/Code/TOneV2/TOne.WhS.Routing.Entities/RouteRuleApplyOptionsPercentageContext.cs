using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteRuleApplyOptionsPercentageContext
    {
        List<RouteOption> Options { get; set; }
        List<RouteOptionRuleTarget> FinalRouteOptionRuleTargets { get; }
        RoutingDatabase RoutingDatabase { get; }
    }

    public class RouteRuleApplyOptionsPercentageContext : IRouteRuleApplyOptionsPercentageContext
    {
        public List<RouteOption> Options { get; set; }
        public List<RouteOptionRuleTarget> FinalRouteOptionRuleTargets { get; set; }
        public RoutingDatabase RoutingDatabase { get; set; }
    }
}
