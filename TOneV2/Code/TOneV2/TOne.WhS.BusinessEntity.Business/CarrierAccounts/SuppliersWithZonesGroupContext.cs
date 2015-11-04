using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SuppliersWithZonesGroupContext : ISuppliersWithZonesGroupContext
    {
        public SupplierFilterSettings FilterSettings { get; set; }

        public IEnumerable<SupplierWithZones> GetSuppliersWithZones(SuppliersWithZonesGroupSettings group)
        {
            var allGroupSupplier = group.GetSuppliersWithZones(this);
            if (allGroupSupplier == null)
                return null;
            HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(this.FilterSettings);
            if (filteredSupplierIds != null)
                return allGroupSupplier.Where(supplier => filteredSupplierIds.Contains(supplier.SupplierId));

            return allGroupSupplier;
        }
    }
}
