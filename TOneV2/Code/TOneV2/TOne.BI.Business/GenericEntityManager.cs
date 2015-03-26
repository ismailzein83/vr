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
        public IEnumerable<GenericEntityRecord> GetTopEntities(EntityType entityType, MeasureType measureType, DateTime fromDate, DateTime toDate, int topCount)
        {
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            return dataManager.GetTopEntities(entityType, measureType, fromDate, toDate, topCount);
        }

        public IEnumerable<TimeDimensionValueRecord> GetEntityMeasureValues(EntityType entityType, string entityValue, MeasureType measureType, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            IGenericEntityDataManager dataManager = BIDataManagerFactory.GetDataManager<IGenericEntityDataManager>();
            return dataManager.GetEntityMeasureValues(entityType, entityValue, measureType, timeDimensionType, fromDate, toDate);
        }
    }
}
