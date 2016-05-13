using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;


namespace TOne.WhS.DBSync.Business
{
    public class SwitchDBSyncManager
    {
        bool _UseTempTables;
        public SwitchDBSyncManager(bool useTempTables)
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySwitchesToTemp(List<Switch> switches)
        {
            SwitchDBSyncDataManager dataManager = new SwitchDBSyncDataManager(_UseTempTables);
            dataManager.ApplySwitchesToTemp(switches);
        }
    }
}
