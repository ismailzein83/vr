using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteRuleAreSuppliersIncludedContext
    {
        HashSet<int> SupplierIds { get; }
    }
    public class RouteRuleAreSuppliersIncludedContext : IRouteRuleAreSuppliersIncludedContext
    {
        public HashSet<int> SupplierIds { get; set; }
    }
}
