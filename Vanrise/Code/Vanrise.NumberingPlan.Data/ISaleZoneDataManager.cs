using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface ISaleZoneDataManager : IDataManager
    {
        List<SaleZone> GetSaleZones(int sellingNumberPlanId);
        List<SaleZoneInfo> GetSaleZonesInfo(int sellingNumberPlanId, string filter);
        bool AreZonesUpdated(ref object updateHandle);
        IEnumerable<SaleZone> GetAllSaleZones();
        IEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture);
    }
}
