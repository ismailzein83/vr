using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DAProfCalcOutputSettings")]
    [JSONWithTypeAttribute]
    public class DAProfCalcOutputSettingsController : BaseAPIController
    {
        DAProfCalcOutputSettingsManager _manager = new DAProfCalcOutputSettingsManager();

        [HttpGet]
        [Route("GetOutputFields")]
        public List<DataRecordField> GetOutputFields(Guid dataAnalysisItemDefinitionId)
        {
            return _manager.GetOutputFields(dataAnalysisItemDefinitionId);
        }

    }
}