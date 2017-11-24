using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleZoneDataManager : IDataManager
    {
        List<SaleZone> GetSaleZones(int sellingNumberPlanId);


        List<SaleZoneInfo> GetSaleZonesInfo(int sellingNumberPlanId, string filter);

        bool AreZonesUpdated(ref object updateHandle);

        IEnumerable<SaleZone> GetAllSaleZones();

        IOrderedEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture);

        bool UpdateSaleZoneName(long zoneId, string zoneName, int sellingNumberPlanId);
    }
}
