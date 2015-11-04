using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierGroupContext : ISupplierGroupContext
    {
        public SupplierFilterSettings FilterSettings { get; set; }

        public IEnumerable<int> GetSupplierIds(SupplierGroupSettings group)
        {
            var allGroupSupplierIds = group.GetSupplierIds(this);
            if (allGroupSupplierIds == null)
                return null;
            HashSet<int> filteredSupplierIds = GetFilteredSupplierIds(this.FilterSettings);
            if (filteredSupplierIds != null)
                return allGroupSupplierIds.Where(supplierId => filteredSupplierIds.Contains(supplierId));

            return allGroupSupplierIds;
        }

        public static HashSet<int> GetFilteredSupplierIds(SupplierFilterSettings filterSettings)
        {
            if (filterSettings != null && filterSettings.RoutingProductId.HasValue)
            {
                RoutingProductManager routingProductManager = new RoutingProductManager();
                return routingProductManager.GetFilteredSupplierIds(filterSettings.RoutingProductId.Value);
            }
            else
                return null;
        }
    }
}
