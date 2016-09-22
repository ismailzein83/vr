using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RouteOptionFilterExecutionContext : IRouteOptionFilterExecutionContext
    {
        public IRouteOptionFilterTarget Option { get; set; }

        public decimal? SaleRate { get; set; }

        public HashSet<int> CustomerServices { get; set; }

        public bool FilterOption { get; set; }

        public HashSet<int> SupplierServices{ get; set; }
    }
}
