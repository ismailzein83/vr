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



        public IEnumerable<TimeValuesRecord> GetMeasureValues1(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params int[] measureTypes)
        {

            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetMeasureValues1(timeDimensionType, fromDate, toDate, measureTypes);
        }

        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues1(int entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params int[] measureTypes)
        {
            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetEntityMeasuresValues1(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypes);
        }
        public IEnumerable<EntityRecord> GetTopEntities1(int entityTypeID, int topByMeasureTypeID, DateTime fromDate, DateTime toDate, int topCount, params int[] measureTypesIDs)
        {
            IBIConfigurationDataManager configurations = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            dataManager.MeasureDefinitions = configurations.GetMeasures();
            dataManager.EntityDefinitions = configurations.GetEntities();
            return dataManager.GetTopEntities1(entityTypeID, topByMeasureTypeID, fromDate, toDate, topCount, measureTypesIDs);
        }
    }
}
