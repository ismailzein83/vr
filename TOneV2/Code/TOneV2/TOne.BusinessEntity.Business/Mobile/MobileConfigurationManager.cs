using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class MobileConfigurationManager
    {
        public bool AddMobileConfiguration(MobileConfiguration configuration, out int insertedId)
        {
            IMobileConfigurationDataManager dataManager = BEDataManagerFactory.GetDataManager<IMobileConfigurationDataManager>();
            return dataManager.AddMobileConfiguration(configuration, out insertedId);
        }

        public bool UpdateMobileConfiguration(MobileConfiguration configuration)
        {
            IMobileConfigurationDataManager dataManager = BEDataManagerFactory.GetDataManager<IMobileConfigurationDataManager>();
            return dataManager.UpdateMobileConfiguration(configuration);
        }

        public IEnumerable<MobileConfiguration> GetConfigurations(int? configId)
        {
            IMobileConfigurationDataManager dataManager = BEDataManagerFactory.GetDataManager<IMobileConfigurationDataManager>();
            return dataManager.GetConfigurations(configId);
        }
    }
}
