using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RouteOptionPercentageExecutionContext : IRouteOptionPercentageExecutionContext
    {
        public IEnumerable<IRouteOptionPercentageTarget> Options { get; set; }
    }
}
