using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteOptionAreSuppliersIncludedContext
    {
        IEnumerable<int> SupplierIds { get; }
    }
    public class RouteOptionAreSuppliersIncludedContext : IRouteOptionAreSuppliersIncludedContext
    {
        public IEnumerable<int> SupplierIds { get; set; }
    }
}
