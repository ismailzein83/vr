using System;
using System.Collections.Generic;
using Vanrise.Data;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface IRPZoneCodeGroupDataManager : IDataManager, IBulkApplyDataManager<ZoneCodeGroup>, IRoutingDataManager
    {
        void ApplyZoneCodeGroupsForDB(object preparedZoneCodeGroups);

        Dictionary<bool, Dictionary<long, HashSet<string>>>  GetZoneCodeGroups();
    }
}