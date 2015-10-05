using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AllExceptSaleZonesBehavior : SaleZoneGroupBehavior
    {
        public override List<long> GetZoneIds(SaleZoneGroupSettings settings)
        {
            SaleZoneManager manager = new SaleZoneManager();
            List<SaleZone> saleZones = manager.GetSaleZones((int)settings.SaleZonePackageId);

            AllExceptSaleZonesSettings allExceptSettings = (AllExceptSaleZonesSettings)settings;

            List<long> retVal = new List<long>();

            foreach(SaleZone zone in saleZones)
            {
                if (!allExceptSettings.ZoneIds.Contains(zone.SaleZoneId))
                {
                    retVal.Add(zone.SaleZoneId);
                }
            }

            return retVal;
        }
    }
}
