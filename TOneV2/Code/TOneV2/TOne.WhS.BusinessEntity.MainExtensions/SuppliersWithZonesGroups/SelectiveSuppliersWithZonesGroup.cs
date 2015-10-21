using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups
{
    public class SelectiveSuppliersWithZonesGroup : SuppliersWithZonesGroupSettings
    {
        public List<SupplierWithZones> SuppliersWithZones { get; set; }

        public override IEnumerable<SupplierWithZones> GetSuppliersWithZones(SuppliersWithZonesGroupContext context)
        {
            return this.SuppliersWithZones;
        }
    }
}
