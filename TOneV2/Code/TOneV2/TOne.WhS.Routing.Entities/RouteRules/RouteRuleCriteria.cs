using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleCriteria
    {
        public int? RoutingProductId { get; set; }

        public CodeCriteriaGroupSettings CodeCriteriaGroupSettings { get; set; }

        public List<string> ExcludedCodes { get; set; }

        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }

        public CustomerGroupSettings CustomerGroupSettings { get; set; }

    }
}
