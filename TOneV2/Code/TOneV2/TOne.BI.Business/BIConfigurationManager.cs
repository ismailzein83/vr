using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Data;
using TOne.BI.Entities;
namespace TOne.BI.Business
{
    public class BIConfigurationManager
    {
        public BIConfigurationManager()
        {
        }
        public List<BIConfiguration<BIConfigurationMeasure>> GetMeasures()
        {
            IBIConfigurationDataManager dataManager = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            return dataManager.GetMeasures();
        }
        public List<BIConfiguration<BIConfigurationEntity>> GetEntities()
        {
            IBIConfigurationDataManager dataManager = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            return dataManager.GetEntities();
        }
    }
}
