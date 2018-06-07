using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ZoneCodeGroupManager
    {
        public List<ZoneCodeGroup> GetSaleZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
        {
            IZoneCodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneCodeGroupDataManager>();
            return dataManager.GetSaleZoneCodeGroups(effectiveOn, isFuture);
        }

        public List<ZoneCodeGroup> GetCostZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
        {
            IZoneCodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneCodeGroupDataManager>();
            return dataManager.GetCostZoneCodeGroups(effectiveOn, isFuture);
        }
    }
}