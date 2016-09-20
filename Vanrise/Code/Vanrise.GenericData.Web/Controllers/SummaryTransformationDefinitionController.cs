using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SummaryTransformationDefinition")]
    public class SummaryTransformationDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetSummaryTransformationDefinition")]
        public SummaryTransformationDefinition GetSummaryTransformationDefinition(int summaryTransformationDefinitionId)
        {
            SummaryTransformationDefinitionManager summaryTransformationDefinitionManager = new SummaryTransformationDefinitionManager();
            return summaryTransformationDefinitionManager.GetSummaryTransformationDefinition(summaryTransformationDefinitionId);
        }

        [HttpPost]
        [Route("GetFilteredSummaryTransformationDefinitions")]
        public object GetFilteredSummaryTransformationDefinitions(Vanrise.Entities.DataRetrievalInput<SummaryTransformationDefinitionQuery> input)
        {
            SummaryTransformationDefinitionManager summaryTransformationDefinitionManager = new SummaryTransformationDefinitionManager();
            return GetWebResponse(input, summaryTransformationDefinitionManager.GetFilteredSummaryTransformationDefinitions(input));
        }

        [Route("UpdateSummaryTransformationDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<SummaryTransformationDefinitionDetail> UpdateSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            SummaryTransformationDefinitionManager summaryTransformationDefinitionManager = new SummaryTransformationDefinitionManager();
            return summaryTransformationDefinitionManager.UpdateSummaryTransformationDefinition(summaryTransformationDefinition);
        }

        [HttpPost]
        [Route("AddSummaryTransformationDefinition")]
        public Vanrise.Entities.InsertOperationOutput<SummaryTransformationDefinitionDetail> AddSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            SummaryTransformationDefinitionManager summaryTransformationDefinitionManager = new SummaryTransformationDefinitionManager();
            return summaryTransformationDefinitionManager.AddSummaryTransformationDefinition(summaryTransformationDefinition);
        }
       
        [HttpGet]
        [Route("GetSummaryTransformationDefinitionInfo")]
        public IEnumerable<SummaryTransformationDefinitionInfo> GetSummaryTransformationDefinitionInfo(string serializedFilter = null)
        {
            SummaryTransformationDefinitionInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<SummaryTransformationDefinitionInfoFilter>(serializedFilter) : null;
            SummaryTransformationDefinitionManager manager = new SummaryTransformationDefinitionManager();
            return manager.GetSummaryTransformationDefinitionInfo(filter);
        }

        [HttpGet]
        [Route("GetSummaryBatchIntervalSourceTemplates")]
        public IEnumerable<SummaryBatchIntervalSettingsConfig> GetSummaryBatchIntervalSourceTemplates()
        {
            SummaryTransformationDefinitionManager manager = new SummaryTransformationDefinitionManager();
            return manager.GetSummaryBatchIntervalSourceTemplates();
        }

    }
}