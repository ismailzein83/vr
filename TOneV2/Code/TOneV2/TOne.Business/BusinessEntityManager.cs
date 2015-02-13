using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data;

namespace TOne.Business
{
    public class BusinessEntityManager
    {
        public List<TOne.Entities.CarrierInfo> GetCarriers(string carrierType)
        {
            IBusinessEntityDataManager datamanager = DataManagerFactory.GetDataManager<IBusinessEntityDataManager>();

            return datamanager.GetCarriers(carrierType);
        }

        public List<TOne.Entities.CodeGroupInfo> GetCodeGroups()
        {
            IBusinessEntityDataManager datamanager = DataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
            return datamanager.GetCodeGroups();
        }

        public List<TOne.Entities.SwitchInfo> GetSwitches()
        {
            IBusinessEntityDataManager datamanager = DataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
            return datamanager.GetSwitches();
        }
    }
}
