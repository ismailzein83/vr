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

        bool KeepBackupsForRemovedOptions { get; }

        int? NumberOfOptions { get; }

        HashSet<int> SaleZoneServiceList { get; }

        string SaleZoneServiceIds { get; }

        RoutingDatabase RoutingDatabase { get; }

        ReadOnlyCollection<RouteOptionRuleTarget> GetOptions();

        SupplierCodeMatchWithRate GetSupplierCodeMatch(int supplierId);

        List<SupplierCodeMatchWithRate> GetFilteredSuppliersCodeMatches();

        List<SupplierCodeMatchWithRate> GetAllSuppliersCodeMatches();

        RouteOptionRuleTarget BuildRouteOptionRuleTarget(RouteRuleTarget routeRuleTarget, IRouteOptionSettings option, List<IRouteBackupOptionSettings> backups);
    }
}
