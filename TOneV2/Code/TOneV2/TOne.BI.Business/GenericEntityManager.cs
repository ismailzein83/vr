using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Data;
using TOne.BI.Entities;

namespace TOne.BI.Business
{
    public class GenericEntityManager
    {   

        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params string[] measureTypeNames)
        {

            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypeNames);
        }

        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(string entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params string[] measureTypes)
        {
            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetEntityMeasuresValues(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypes);
        }
        public IEnumerable<EntityRecord> GetTopEntities(string entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, params string[] measureTypesNames)
        {
            List<String> queryFilter = new List<String>();
            queryFilter = null;
            //queryFilter.Add("C001");
            //queryFilter.Add("C009");
            //queryFilter.Add("C020");
            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetTopEntities(entityTypeName, topByMeasureTypeName, fromDate, toDate, topCount, queryFilter, measureTypesNames);
        }

        public Decimal[] GetMeasureValues(DateTime fromDate, DateTime toDate, params string[] measureTypeNames)
        {

            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetMeasureValues(fromDate, toDate, measureTypeNames);
        }

        
    }
}
