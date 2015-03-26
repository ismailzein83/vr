using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Entities;

namespace TOne.BI.Data
{
    public interface IGenericEntityDataManager : IDataManager
    {
        IEnumerable<GenericEntityRecord> GetTopEntities(EntityType entityType, MeasureType measureType, DateTime fromDate, DateTime toDate, int topCount);
        IEnumerable<TimeDimensionValueRecord> GetEntityMeasureValues(EntityType entityType, string entityValue, MeasureType measureType, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate);
    }
}
