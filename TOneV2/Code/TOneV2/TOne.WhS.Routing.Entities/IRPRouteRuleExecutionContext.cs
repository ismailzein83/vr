using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRPRouteRuleExecutionContext
    {
        RouteRule RouteRule { get; }

        bool TryAddSupplierZoneOption(RouteOptionRuleTarget optionTarget);


        List<SupplierCodeMatchWithRate> GetSupplierCodeMatches(int supplierId);

        List<SupplierCodeMatchWithRate> GetAllSuppliersCodeMatches();
    }
}
