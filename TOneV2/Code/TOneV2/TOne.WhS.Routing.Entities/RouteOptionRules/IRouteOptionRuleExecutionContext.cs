using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteOptionRuleExecutionContext
    {
        string CustomerServiceIds { get; set; }
        RouteRule RouteRule { get; set; }
    }
}
