using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionRuleCriteria
    {
        public int? RoutingProductId { get; set; }

        public CodeCriteriaGroupSettings CodeCriteriaGroupSettings { get; set; }

        public RoutingExcludedDestinations ExcludedDestinations { get; set; } 

        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }

        public CustomerGroupSettings CustomerGroupSettings { get; set; }

        public SuppliersWithZonesGroupSettings SuppliersWithZonesGroupSettings { get; set; }

        public CountryCriteriaGroupSettings CountryCriteriaGroupSettings { get; set; }
    }
}
