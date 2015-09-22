using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class SaleZoneGroupBehavior
    {
        public abstract List<SaleZone> GetZones(int saleZonePackageId, SaleZoneGroupSettings settings);
    }

    public class SelectiveSaleZonesBehavior : SaleZoneGroupBehavior
    {
        public override List<SaleZone> GetZones(int saleZonePackageId, SaleZoneGroupSettings settings)
        {
            SelectiveSaleZonesSettings selectiveSettings = settings as SelectiveSaleZonesSettings;

            ISaleZoneManager saleZoneManager = BEManagerFactory.GetManager<ISaleZoneManager>();
            return saleZoneManager.GetZones(selectiveSettings.ZoneIds);
        }
    }

    public class AllSaleZonesBehavior : SaleZoneGroupBehavior
    {
        public override List<SaleZone> GetZones(int saleZonePackageId, SaleZoneGroupSettings settings)
        {
            ISaleZoneManager saleZoneManager = BEManagerFactory.GetManager<ISaleZoneManager>();
            return saleZoneManager.GetPackageZones(saleZonePackageId);
        }
    }
}
