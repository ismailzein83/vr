using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BI.Business;
using TOne.BI.Entities;
using TOne.Web.Models;

namespace TOne.Web.Controllers
{
    public class BIController : ApiController
    {
        public IEnumerable<BIMeasureTypeModel> GetMeasureTypeList()
        {
            List<BIMeasureTypeModel> rslt = new List<BIMeasureTypeModel>();
            foreach(int val in Enum.GetValues(typeof(MeasureType)))
            {
                rslt.Add(new BIMeasureTypeModel
                {
                    Value = val.ToString(),
                    Description = ((MeasureType)val).ToString()
                });
            }

            return rslt;
        }

        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] MeasureType[] measureTypes)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypes);
        }

        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(EntityType entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] MeasureType[] measureTypes)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetEntityMeasuresValues(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypes);
        }

        public IEnumerable<GenericEntityRecord> GetTopEntities(EntityType entityType, MeasureType topByMeasureType, DateTime fromDate, DateTime toDate, int topCount, [FromUri] MeasureType[] moreMeasures)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetTopEntities(entityType, topByMeasureType, fromDate, toDate, topCount, moreMeasures);
        }
    }
}
