using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class SaleZoneGroup
    {
        public abstract List<SaleZone> GetZones(int saleZonePackageId, SaleZoneGroupSettings settings);
    }

    public class SelectiveSaleZones : SaleZoneGroup
    {
        public override List<SaleZone> GetZones(int saleZonePackageId, SaleZoneGroupSettings settings)
        {
            SelectiveSaleZonesSettings selectiveSettings = settings as SelectiveSaleZonesSettings;

            ISaleZoneManager saleZoneManager = BEManagerFactory.GetManager<ISaleZoneManager>();
            return saleZoneManager.GetZones(selectiveSettings.ZoneIds);
        }
    }

    public class AllSaleZones : SaleZoneGroup
    {
        public override List<SaleZone> GetZones(int saleZonePackageId, SaleZoneGroupSettings settings)
        {
            ISaleZoneManager saleZoneManager = BEManagerFactory.GetManager<ISaleZoneManager>();
            return saleZoneManager.GetPackageZones(saleZonePackageId);
        }
    }
}
