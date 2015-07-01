using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.BI.Entities;
using TOne.BI.Business;
using System.Net.Http;
using System.Web.Http;
using TOne.BI.Web.Models;

namespace TOne.BI.Web.Controllers
{
    public class BIDataController : ApiController
    {
        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] string[] measureTypesNames)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypesNames);
        }
        [HttpGet]
        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(string entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] string[] measureTypesIDs)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetEntityMeasuresValues(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypesIDs);
        }
        [HttpGet]
        public IEnumerable<EntityRecord> GetTopEntities(string entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, [FromUri] string[] measureTypesNames)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetTopEntities(entityTypeName, topByMeasureTypeName, fromDate, toDate, topCount, measureTypesNames);
        }
        [HttpGet]
        public Decimal[] GetMeasureValues(DateTime fromDate, DateTime toDate, [FromUri] string[] measureTypesNames)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetMeasureValues(fromDate, toDate, measureTypesNames);
        }
        
    }
}