using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.GenericData;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Notification;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordRuleEvaluatorDefinition")]
    public class DataRecordRuleEvaluatorDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetDataRecordRuleEvaluatorDefinitionsInfo")]
        public IEnumerable<DataRecordRuleEvaluatorDefinitionInfo> GetDataRecordRuleEvaluatorDefinitionsInfo(string filter = null)
        {
            DataRecordRuleEvaluatorDefinitionManager dataRecordRuleEvaluatorDefinitionManager = new DataRecordRuleEvaluatorDefinitionManager();
            DataRecordRuleEvaluatorDefinitionInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<DataRecordRuleEvaluatorDefinitionInfoFilter>(filter) : null;
            return dataRecordRuleEvaluatorDefinitionManager.GetDataRecordRuleEvaluatorDefinitionsInfo(deserializedFilter);
        }
    }
}