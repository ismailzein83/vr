using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class RPZoneCodeGroupDataManager : RoutingDataManager, IRPZoneCodeGroupDataManager
    {
        public void ApplyZoneCodeGroupsForDB(object preparedZoneCodeGroups)
        {
            throw new NotImplementedException();
        }

        Dictionary<bool, Dictionary<long, HashSet<string>>> IRPZoneCodeGroupDataManager.GetZoneCodeGroups()
        {
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(ZoneCodeGroup record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }
    }
}