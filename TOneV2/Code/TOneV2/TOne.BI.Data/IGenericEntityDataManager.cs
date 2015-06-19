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
        List<BIConfiguration<BIConfigurationMeasure>> MeasureDefinitions { set; }
        List<BIConfiguration<BIConfigurationEntity>> EntityDefinitions { set; }

        IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params MeasureType[] measureTypes);

        IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(EntityType entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params MeasureType[] measureTypes);

        IEnumerable<GenericEntityRecord> GetTopEntities(EntityType entityType, MeasureType topByMeasureType, DateTime fromDate, DateTime toDate, int topCount, params MeasureType[] moreMeasures);


        IEnumerable<TimeValuesRecord> GetMeasureValues1(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params int[] measureTypes);

        IEnumerable<TimeValuesRecord> GetEntityMeasuresValues1(int entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params int[] measureTypes);

        IEnumerable<EntityRecord> GetTopEntities1(int entityTypeID, int topByMeasureTypeID, DateTime fromDate, DateTime toDate, int topCount, params int[] measureTypesIDs);
    }
}
