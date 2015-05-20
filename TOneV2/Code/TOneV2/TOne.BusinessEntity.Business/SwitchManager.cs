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


        public List<SwitchInfo> GetFilteredSwitches(string switchName,int rowFrom,int rowTo)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            return dataManager.GetSwitches();
        }
    }
}
