using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public interface IBuildCustomerRoutesContext
    {
        List<SaleCodeMatch> SaleCodeMatches { get; }

        CustomerZoneRatesByZone CustomerZoneRates { get; }

        List<SupplierCodeMatch> SupplierCodeMatches { get; }

        SupplierCodeMatchBySupplier SupplierCodeMatchesBySupplier { get; }

        SupplierZoneRatesByZone SupplierZoneRates { get; }

        DateTime? EntitiesEffectiveOn { get; }

        bool EntitiesEffectiveInFuture { get; }
    }
}
