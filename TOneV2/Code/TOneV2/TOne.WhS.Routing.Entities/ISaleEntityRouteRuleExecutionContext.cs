using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public interface ISaleEntityRouteRuleExecutionContext
    {
        RouteRule RouteRule { get; }

        int? NumberOfOptions { get; }

        HashSet<int> SaleZoneServiceList { get; }

        string SaleZoneServiceIds { get; }

        RoutingDatabase RoutingDatabase { get; }

        bool TryAddOption(RouteOptionRuleTarget optionTarget);

        ReadOnlyCollection<RouteOptionRuleTarget> GetOptions();

        SupplierCodeMatchWithRate GetSupplierCodeMatch(int supplierId);

        List<SupplierCodeMatchWithRate> GetAllSuppliersCodeMatches();
    }
}
