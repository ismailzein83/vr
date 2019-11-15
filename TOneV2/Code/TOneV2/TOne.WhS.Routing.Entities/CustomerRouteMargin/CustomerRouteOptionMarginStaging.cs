using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRouteOptionMarginStaging : BaseCustomerRouteOptionMargin
    {

    }

    public class SaleZoneOptionsMarginStaging
    {
        public Decimal SaleRate { get; set; }

        public int? SaleDealID { get; set; } 

        public List<CodeOptionsMarginStaging> CodeOptionsMarginStagingList { get; set; }
    }

    public class CodeOptionsMarginStaging
    {
        public string Code { get; set; }

        public Dictionary<long, CustomerRouteOptionMarginStaging> CustomerRouteOptionMarginStagingBySupplierZone { get; set; }

        public CustomerRouteOptionMarginStaging OptimalCustomerRouteOptionMarginStaging { get; set; }
    }
}