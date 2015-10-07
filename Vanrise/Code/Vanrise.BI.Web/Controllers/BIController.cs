﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.BI.Business;
using Vanrise.BI.Entities;
using Vanrise.BI.Web.Models;

namespace Vanrise.BI.Web.Controllers
{
    public class BIController : Vanrise.Web.Base.BaseAPIController
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
        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] string[] measureTypesNames)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypesNames);
        }
        [HttpGet]
        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(List<string> entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] string[] measureTypesIDs)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetEntityMeasuresValues(entityType, entityId, timeDimensionType, fromDate, toDate, measureTypesIDs);
        }
        [HttpGet]
        public IEnumerable<EntityRecord> GetTopEntities([FromUri] List<string> entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, [FromUri] string[] measureTypesNames)
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


         [HttpGet]
         public HttpResponseMessage ExportMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, [FromUri] string[] measureTypesNames)
        {
            GenericEntityManager manager = new GenericEntityManager();
            IEnumerable<TimeValuesRecord> records= manager.GetMeasureValues(timeDimensionType, fromDate, toDate, measureTypesNames);
            return manager.ExportMeasureValues(records, "Time", measureTypesNames, timeDimensionType, fromDate, toDate);
        }

        [HttpGet]
         public HttpResponseMessage ExportTopEntities(List<string> entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, [FromUri] string[] measureTypesNames)
        {
            GenericEntityManager manager = new GenericEntityManager();
            IEnumerable<EntityRecord> records=manager.GetTopEntities(entityTypeName, topByMeasureTypeName, fromDate, toDate, topCount, measureTypesNames);
            return manager.ExportTopEntities(records, "EntityName", measureTypesNames);
        }

        [HttpPost]
        public object GetUserMeasuresValidator(Vanrise.Entities.DataRetrievalInput<UserMeasuresValidatorInput> userMeasuresValidatorInput)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return GetWebResponse(userMeasuresValidatorInput,manager.GetUserMeasuresValidator(userMeasuresValidatorInput));
        }
    }
}
