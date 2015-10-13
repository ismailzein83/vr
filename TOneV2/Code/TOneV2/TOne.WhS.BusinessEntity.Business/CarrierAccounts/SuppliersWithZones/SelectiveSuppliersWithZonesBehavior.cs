using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.CarrierAccounts.SuppliersWithZones;

namespace TOne.WhS.BusinessEntity.Business.CarrierAccounts.SuppliersWithZones
{
    public class SelectiveSuppliersWithZonesBehavior : SuppliersWithZonesGroupBehavior
    {
        public override List<SupplierWithZones> GetSuppliersWithZones(SuppliersWithZonesGroupSettings settings)
        {
            SelectiveSuppliersWithZonesSettings selectiveSuppliersWithZonesSettings = settings as SelectiveSuppliersWithZonesSettings;
            if (selectiveSuppliersWithZonesSettings == null)
                throw new Exception(String.Format("{0} is not of type TOne.WhS.BusinessEntity.Entities.CarrierAccounts.SuppliersWithZones.SelectiveSuppliersWithZonesSettings", settings));

            return selectiveSuppliersWithZonesSettings.SuppliersWithZones;
        }
    }
}
