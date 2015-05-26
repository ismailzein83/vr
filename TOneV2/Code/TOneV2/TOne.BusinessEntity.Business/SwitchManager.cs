using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class SwitchManager
    {
        public List<SwitchInfo> GetSwitches()
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitches();
        }


        public List<Switch> GetFilteredSwitches(string switchName, int rowFrom, int rowTo)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetFilteredSwitches(switchName, rowFrom, rowTo);
        }

        public Switch GetSwitchDetails(int switchID)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitchDetails(switchID);
        }

        public int UpdateSwitch(Switch switchObject)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.UpdateSwitch(switchObject);
        }


        public int InsertSwitch(Switch switchObject)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.InsertSwitch(switchObject);
        }
    }
}
