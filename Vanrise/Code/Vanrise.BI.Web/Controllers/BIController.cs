using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.BI.Business;
using Vanrise.BI.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BI.Web.Controllers
{

    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BI")]
    public class BIController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetMeasureValues")]
        public IEnumerable<TimeValuesRecord> GetMeasureValues(MeasureValueInput input)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetMeasureValues(input);
        }
      
        [HttpPost]
        [Route("GetEntityMeasuresValues")]
        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(EntityMeasureValueInput input)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetEntityMeasuresValues(input);
        }
       
        [HttpPost]
        [Route("GetTopEntities")]
        public IEnumerable<EntityRecord> GetTopEntities(TopEntityInput input)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetTopEntities(input);
        }
      
        [HttpPost]
        [Route("GetSummaryMeasureValues")]
        public Decimal[] GetSummaryMeasureValues(BaseBIInput input)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return manager.GetSummaryMeasureValues(input);
        }
       
        [HttpPost]
        [Route("ExportMeasureValues")]
        public HttpResponseMessage ExportMeasureValues(MeasureValueInput input)
        {
            GenericEntityManager manager = new GenericEntityManager();
            IEnumerable<TimeValuesRecord> records = manager.GetMeasureValues(input);
            return manager.ExportMeasureValues(records, "Time", input.MeasureTypesNames, input.TimeDimensionType, input.FromDate, input.ToDate);
        }

        [HttpPost]
        [Route("ExportTopEntities")]
        public HttpResponseMessage ExportTopEntities(TopEntityInput input)
        {
            GenericEntityManager manager = new GenericEntityManager();
            IEnumerable<EntityRecord> records = manager.GetTopEntities(input);
            return manager.ExportTopEntities(records, input.EntityTypeName, input.MeasureTypesNames);
        }

        [HttpPost]
        [Route("GetUserMeasuresValidator")]
        public object GetUserMeasuresValidator(Vanrise.Entities.DataRetrievalInput<UserMeasuresValidatorInput> userMeasuresValidatorInput)
        {
            GenericEntityManager manager = new GenericEntityManager();
            return GetWebResponse(userMeasuresValidatorInput, manager.GetUserMeasuresValidator(userMeasuresValidatorInput));
        }
    }
}
