using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
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
        public List<DAProfCalcOutputField> GetOutputFields(Guid dataAnalysisItemDefinitionId)
        {
            return _manager.GetOutputFields(dataAnalysisItemDefinitionId);
        }

        [HttpGet]
        [Route("GetFilteredOutputFields")]
        public IEnumerable<DAProfCalcOutputField> GetFilteredOutputFields(Guid dataAnalysisItemDefinitionId, string filter = null)
        {
            DAProfCalcOutputFieldFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<DAProfCalcOutputFieldFilter>(filter) : null;
            return _manager.GetFilteredOutputFields(dataAnalysisItemDefinitionId, deserializedFilter);
        }

        [HttpGet]
        [Route("GetInputFields")]
        public List<DataRecordField> GetInputFields(Guid dataAnalysisDefinitionId)
        {
            return _manager.GetInputFields(dataAnalysisDefinitionId);
        }
    }
}