using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Data;
using Vanrise.BI.Entities;
namespace Vanrise.BI.Business
{
    public class BIConfigurationManager
    {
        public BIConfigurationManager()
        {
        }
        public List<BIConfiguration<BIConfigurationMeasure>> GetMeasures()
        {
            IBIConfigurationDataManager dataManager = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            List<BIConfiguration<BIConfigurationMeasure>> measures = dataManager.GetMeasures();
            foreach (BIConfiguration<BIConfigurationMeasure> measure in measures)
            {
                if (measure.Configuration.Unit == "Currency")
                    measure.Configuration.Unit = System.Configuration.ConfigurationManager.AppSettings["Currency"];
            }
            return measures;
        }
        public List<BIConfiguration<BIConfigurationEntity>> GetEntities()
        {
            IBIConfigurationDataManager dataManager = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            return dataManager.GetEntities();
        }
    }
}
