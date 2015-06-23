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
        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params MeasureType[] measureTypes)
        {
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();  
            return dataManager.GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypes);
        }

        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(EntityType entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params MeasureType[] measureTypes)
        {
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            return dataManager.GetEntityMeasuresValues(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypes);
        }
        public IEnumerable<GenericEntityRecord> GetTopEntities(EntityType entityType, MeasureType topByMeasureType, DateTime fromDate, DateTime toDate, int topCount, params MeasureType[] moreMeasures)
        {
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            return dataManager.GetTopEntities(entityType, topByMeasureType, fromDate, toDate, topCount, moreMeasures);
        }



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
            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetTopEntities(entityTypeName, topByMeasureTypeName, fromDate, toDate, topCount, measureTypesNames);
        }
    }
}
