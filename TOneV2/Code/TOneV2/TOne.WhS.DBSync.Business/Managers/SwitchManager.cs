using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data;


namespace TOne.WhS.DBSync.Business
{
    public class SwitchManager
    {

        public void AddSwitchesFromSource(List<Switch> switches)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            dataManager.ApplySwitchesToDB(switches);
        }
    }
}
