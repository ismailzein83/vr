using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BI.Business;
using TOne.BI.Entities;

namespace TOne.Web.Controllers
{
    public class BIController : ApiController
    {
        public IEnumerable<ProfitInfo> GetProfit(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            SalesManager manager = new SalesManager();
            return manager.GetProfit(timeDimensionType, fromDate, toDate);
        }

        public IEnumerable<ZoneValue> GetTopZonesByDuration(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, int topCount)
        {
            DestinationManager manager = new DestinationManager();
            return manager.GetTopZonesByDuration(timeDimensionType, fromDate, toDate, topCount);
        }

        public IEnumerable<GenericEntityRecord> GetTopEntities(EntityType entityType, MeasureType measureType, DateTime fromDate, DateTime toDate, int topCount)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetTopEntities(entityType, measureType, fromDate, toDate, topCount);
        }

        public IEnumerable<TimeDimensionValueRecord> GetEntityMeasureValues(EntityType entityType, string entityId, MeasureType measureType, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetEntityMeasureValues(entityType, entityId, measureType, timeDimensionType, fromDate, toDate);
        }

        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(EntityType entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] MeasureType[] measureTypes)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetEntityMeasuresValues(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypes);
        }
    }
}
