using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Data;
using Vanrise.BI.Entities;

namespace Vanrise.BI.Business
{
    public class DimensionManager
    {
        public List<DimensionInfo> GetDimensionInfo(string entityName)
        {
            BIConfigurationManager configurationManager = new Business.BIConfigurationManager();
            IDimensionDataManager dataManager = BIDataManagerFactory.GetDataManager<IDimensionDataManager>();
            List<BIConfiguration<BIConfigurationEntity>> entities = configurationManager.GetEntities();
            dataManager.EntityDefinitions = entities;
            return dataManager.GetDimensionInfo(entityName);
        }
    }
}
