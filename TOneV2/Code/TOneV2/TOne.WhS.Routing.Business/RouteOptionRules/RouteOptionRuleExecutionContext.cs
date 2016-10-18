using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RouteOptionRuleExecutionContext : IRouteOptionRuleExecutionContext
    {
        public string CustomerServiceIds { get; set; }

        public RouteRule RouteRule{ get; set; }
    }
}
