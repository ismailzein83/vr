using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleZoneGroupContext : ISaleZoneGroupContext
    {
        public SaleZoneFilterSettings FilterSettings { get; set; }

        public IEnumerable<long> GetGroupZoneIds(SaleZoneGroupSettings saleZoneGroup)
        {
            var allGroupZoneIds = saleZoneGroup.GetZoneIds(this);
            if (allGroupZoneIds == null)
                return null;
            HashSet<long> filteredZoneIds = GetFilteredZoneIds(this.FilterSettings);
            if (filteredZoneIds != null)
                return allGroupZoneIds.Where(zoneId => filteredZoneIds.Contains(zoneId));

            return allGroupZoneIds;
        }

        public static HashSet<long> GetFilteredZoneIds(SaleZoneFilterSettings filterSettings)
        {
            if (filterSettings != null && filterSettings.RoutingProductId.HasValue)
            {
                RoutingProductManager routingProductManager = new RoutingProductManager();
                return routingProductManager.GetFilteredZoneIds(filterSettings.RoutingProductId.Value);
            }
            else
                return null;
        }
    }
}
