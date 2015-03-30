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
    }
}
