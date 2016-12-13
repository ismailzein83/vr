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

        List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; }

        SupplierCodeMatchWithRateBySupplier SupplierCodeMatchesBySupplier { get; }
        
        DateTime? EntitiesEffectiveOn { get; }

        bool EntitiesEffectiveInFuture { get; }

        IEnumerable<RoutingCustomerInfo> ActiveRoutingCustomerInfos { get; }

        Dictionary<int, List<int>> CustomerCountries { get; set; }
    }
}
