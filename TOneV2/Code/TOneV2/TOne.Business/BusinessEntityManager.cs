using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.Data;

namespace TOne.Business
{
    public class BusinessEntityManager
    {
        public List<TOne.Entities.SwitchInfo> GetSwitches()
        {
            IBusinessEntityDataManager datamanager = DataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
            return datamanager.GetSwitches();
        }
    }
}
