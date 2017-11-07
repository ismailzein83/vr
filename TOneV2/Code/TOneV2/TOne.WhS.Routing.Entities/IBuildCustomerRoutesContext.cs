﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public interface IBuildCustomerRoutesContext
    {
        List<SaleZoneDefintion> SaleZoneDefintions { get; }

        CustomerZoneDetailByZone CustomerZoneDetails { get; }

        List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; }

        SupplierCodeMatchWithRateBySupplier SupplierCodeMatchesBySupplier { get; }

        DateTime? EntitiesEffectiveOn { get; }

        bool EntitiesEffectiveInFuture { get; }

        IEnumerable<RoutingCustomerInfo> ActiveRoutingCustomerInfos { get; }

        Dictionary<int, HashSet<int>> CustomerCountries { get; set; }

        int? VersionNumber { get; }
    }
}
