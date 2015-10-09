using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteOptionRuleCriteria
    {
        public int? RoutingProductId { get; set; }

        public int? CodeCriteriaGroupId { get; set; }

        public CodeCriteriaGroupSettings CodeCriteriaGroupSettings { get; set; }

        public List<string> ExcludedCodes { get; set; }

        public int? SaleZoneGroupConfigId { get; set; }

        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }

        public int? CustomersGroupConfigId { get; set; }

        public CustomerGroupSettings CustomerGroupSettings { get; set; }

        public int SuppliersWithZonesGroupConfigId { get; set; }

        public SuppliersWithZonesGroupSettings SuppliersWithZonesGroupSettings { get; set; }
    }
}
