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

        public List<long> ZoneIds { get; set; }

        public List<long> ExcludedZoneIds { get; set; }

        public int? CustomersGroupConfigId { get; set; }

        public CustomerGroupSettings CustomerGroupSettings { get; set; }

        public List<int> CustomerIds { get; set; }

        public List<int> ExcludedCustomerIds { get; set; }



        public bool HasCustomerFilter()
        {
            return this.CustomersGroupConfigId.HasValue && (this.CustomerGroupSettings == null || !this.CustomerGroupSettings.IsAllExcept); //this.CustomerIds != null && this.CustomerIds.Count > 0;
        }

        public bool HasCodeFilter()
        {
            return this.Codes != null && this.Codes.Count > 0;
        }

        public bool HasZoneFilter()
        {
            return this.ZoneIds != null && this.ZoneIds.Count > 0;
        }

        public bool IsAnyExcluded(int? customerId, string code, long zoneId)
        {            
            return (customerId != null && IsItemInList(customerId.Value, this.ExcludedCustomerIds)) 
                || IsItemInList(code, this.ExcludedCodes) 
                || IsItemInList(zoneId, this.ExcludedZoneIds);
        }

        bool IsItemInList<T>(T item, List<T> list)
        {
            return item != null && list != null && list.Contains(item);
        }
    }

    public class RouteRuleCriteriaCode
    {
        public string Code { get; set; }

        public bool WithSubCodes { get; set; }
    }
}
