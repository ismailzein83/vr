using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public interface IRPRouteRuleExecutionContext
    {
        RouteRule RouteRule { get; }

        RoutingDatabase RoutingDatabase { get; }

        HashSet<int> SaleZoneServiceIds { get; set; }

        bool TryAddSupplierZoneOption(RouteOptionRuleTarget optionTarget);

        List<SupplierCodeMatchWithRate> GetSupplierCodeMatches(int supplierId);

        List<SupplierCodeMatchWithRate> GetAllSuppliersCodeMatches();

        void CreateSupplierZoneOptionsForRP(RouteRuleTarget target, Action<HashSet<int>, RouteRuleTarget, BaseRouteOptionRuleTarget> filterOption,
            IRouteOptionSettings optionSettings, List<IRouteBackupOptionSettings> backupsSettings, HashSet<int> addedSuppliers);
    }
}