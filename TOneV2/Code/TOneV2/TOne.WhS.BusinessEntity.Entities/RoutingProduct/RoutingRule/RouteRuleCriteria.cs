using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRuleCriteria
    {
        public int? RoutingProductId { get; set; }

        public List<RouteRuleCriteriaCode> Codes { get; set; }

        public List<string> ExcludedCodes { get; set; }

        public int? SaleZoneGroupConfigId { get; set; }

        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }

        public List<long> ZoneIds { get; set; }

        public List<long> ExcludedZoneIds { get; set; }

        public int? CustomersGroupConfigId { get; set; }

        public CustomerGroupSettings CustomerGroupSettings { get; set; }

        public bool HasCustomerFilter()
        {
            return this.CustomersGroupConfigId.HasValue && (this.CustomerGroupSettings == null || !this.CustomerGroupSettings.IsAllExcept); 
        }

        public bool HasCodeFilter()
        {
            return this.Codes != null && this.Codes.Count > 0;
        }

        public bool HasZoneFilter()
        {
            return this.SaleZoneGroupConfigId.HasValue && (this.SaleZoneGroupSettings == null || !this.SaleZoneGroupSettings.IsAllExcept);
        }
    }

    public class RouteRuleCriteriaCode
    {
        public string Code { get; set; }

        public bool WithSubCodes { get; set; }
    }
}
