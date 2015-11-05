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

        CustomerZoneDetailByZone CustomerZoneDetails { get; }

        List<SupplierCodeMatch> SupplierCodeMatches { get; }

        SupplierCodeMatchBySupplier SupplierCodeMatchesBySupplier { get; }

        SupplierZoneDetailByZone SupplierZoneDetails { get; }

        DateTime? EntitiesEffectiveOn { get; }

        bool EntitiesEffectiveInFuture { get; }
    }
}
