using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities.CarrierAccounts.SuppliersWithZones
{
    public class SelectiveSuppliersWithZonesSettings : SuppliersWithZonesGroupSettings
    {
        public List<SupplierWithZones> SuppliersWithZones { get; set; }

        public override IEnumerable<SupplierWithZones> GetSuppliersWithZones(SuppliersWithZonesGroupContext context)
        {
            throw new NotImplementedException();
        }
    }
}
