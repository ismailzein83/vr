using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public interface IBuildRoutingProductRoutesContext
    {
        IEnumerable<RoutingProduct> RoutingProducts { get; }

        List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; }

        SupplierCodeMatchesWithRateBySupplier SupplierCodeMatchesBySupplier { get; }

        IEnumerable<SupplierZoneToRPOptionPolicy> SupplierZoneToRPOptionPolicies { get; }

        DateTime? EntitiesEffectiveOn { get; }

        bool EntitiesEffectiveInFuture { get; }

        RoutingDatabase RoutingDatabase { get; }
    }
}
