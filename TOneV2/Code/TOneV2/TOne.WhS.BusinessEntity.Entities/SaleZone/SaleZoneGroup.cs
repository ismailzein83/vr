using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SaleZoneGroup
    {
        public abstract List<SaleZone> GetZones(int saleZonePackageId);
    }

    public class SelectiveSaleZones : SaleZoneGroup
    {
        public List<long> ZoneIds { get; set; }

        public override List<SaleZone> GetZones(int saleZonePackageId)
        {
            ISaleZoneManager saleZoneManager = BEManagerFactory.GetManager<ISaleZoneManager>();
            return saleZoneManager.GetZones(this.ZoneIds);
        }
    }

    public class AllSaleZones : SaleZoneGroup
    {
        public override List<SaleZone> GetZones(int saleZonePackageId)
        {
            ISaleZoneManager saleZoneManager = BEManagerFactory.GetManager<ISaleZoneManager>();
            return saleZoneManager.GetPackageZones(saleZonePackageId);
        }
    }
}
