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
        public int? RoutingProductId { get; set; }

        public IEnumerable<long> GetGroupZoneIds(SaleZoneGroupSettings saleZoneGroup)
        {
            var allGroupZoneIds = saleZoneGroup.GetZoneIds(this);
            if (allGroupZoneIds == null)
                return null;
            HashSet<long> filteredZoneIds = GetFilteredZoneIds(this);
            if (filteredZoneIds != null)
                return allGroupZoneIds.Where(zoneId => filteredZoneIds.Contains(zoneId));

            return allGroupZoneIds;
        }

        public static HashSet<long> GetFilteredZoneIds(ISaleZoneGroupContext saleZoneGroupContext)
        {
            HashSet<long> filteredZoneIds = null;
            if (saleZoneGroupContext != null)
            {
                if (saleZoneGroupContext.RoutingProductId.HasValue)
                {
                    RoutingProductManager routingProductManager = new RoutingProductManager();
                    var routingProduct = routingProductManager.GetRoutingProduct(saleZoneGroupContext.RoutingProductId.Value);
                    if (routingProduct != null && routingProduct.Settings != null)
                    {
                        if (!routingProduct.Settings.IsAllZones)
                        {
                            if (routingProduct.Settings.Zones == null)
                                filteredZoneIds = new HashSet<long>();//empty list
                            else
                                filteredZoneIds = new HashSet<long>(routingProduct.Settings.Zones.Select(zone => zone.ZoneId));
                        }
                    }
                }
            }
            return filteredZoneIds;
        }


        public string GetGroupDescription(SaleZoneGroupSettings saleZoneGroup)
        {
            throw new NotImplementedException();
        }
    }
}
