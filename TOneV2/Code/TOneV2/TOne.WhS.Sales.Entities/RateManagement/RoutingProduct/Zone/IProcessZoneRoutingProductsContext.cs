﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public interface IProcessSaleZoneRoutingProductsContext
    {
        int OwnerId { get; }

        IEnumerable<SaleZoneRoutingProductToAdd> SaleZoneRoutingProductsToAdd { get; }

        IEnumerable<SaleZoneRoutingProductToClose> SaleZoneRoutingProductsToClose { get; }

        IEnumerable<ExistingSaleZoneRoutingProduct> ExistingSaleZoneRoutingProducts { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }

        IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; }

        IEnumerable<NewSaleZoneRoutingProduct> NewSaleZoneRoutingProducts { set; }

        IEnumerable<ChangedSaleZoneRoutingProduct> ChangedSaleZoneRoutingProducts { set; }
    }
}
