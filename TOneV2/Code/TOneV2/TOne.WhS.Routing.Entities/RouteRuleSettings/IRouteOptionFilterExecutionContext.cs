using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteOptionFilterTarget
    {
        Decimal SupplierRate { get; }
    }

    public interface IRouteOptionFilterExecutionContext
    {
        Decimal? SaleRate { get; }

        IRouteOptionFilterTarget Option { get; }

        HashSet<int> SupplierServices { get; set; }

        HashSet<int> CustomerServices { get; set; }

        bool FilterOption { get; set; }
    }
}
