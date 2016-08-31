using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataAnalysisItemDefinition")]
    [JSONWithTypeAttribute]
    public class DataAnalysisItemDefinitionController : BaseAPIController
    {
        DataAnalysisItemDefinitionManager _manager = new DataAnalysisItemDefinitionManager();

        [HttpPost]
        [Route("GetFilteredDataAnalysisItemDefinitions")]
        public object GetFilteredDataAnalysisItemDefinitions(Vanrise.Entities.DataRetrievalInput<DataAnalysisItemDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredDataAnalysisItemDefinitions(input));
        }

        [HttpGet]
        [Route("GetDataAnalysisItemDefinition")]
        public DataAnalysisItemDefinition GetDataAnalysisItemDefinition(Guid dataAnalysisItemDefinitionId)
        {
            return _manager.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId);
        }

        [HttpPost]
        [Route("AddDataAnalysisItemDefinition")]
        public Vanrise.Entities.InsertOperationOutput<DataAnalysisItemDefinitionDetail> AddDataAnalysisItemDefinition(DataAnalysisItemDefinition dataAnalysisItemDefinitionItem)
        {
            return _manager.AddDataAnalysisItemDefinition(dataAnalysisItemDefinitionItem);
        }

        [HttpPost]
        [Route("UpdateDataAnalysisItemDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<DataAnalysisItemDefinitionDetail> UpdateDataAnalysisItemDefinition(DataAnalysisItemDefinition dataAnalysisItemDefinitionItem)
        {
            return _manager.UpdateDataAnalysisItemDefinition(dataAnalysisItemDefinitionItem);
        }

        //[HttpGet]
        //[Route("GetDataAnalysisItemDefinitionSettingsExtensionConfigs")]
        //public IEnumerable<DataAnalysisItemDefinitionConfig> GetDataAnalysisItemDefinitionSettingsExtensionConfigs()
        //{
        //    return _manager.GetDataAnalysisItemDefinitionSettingsExtensionConfigs();
        //}

        //[HttpGet]
        //[Route("GetDataAnalysisItemDefinitionsInfo")]
        //public IEnumerable<DataAnalysisItemDefinitionInfo> GetDataAnalysisItemDefinitionsInfo(string filter = null)
        //{
        //    DataAnalysisItemDefinitionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<DataAnalysisItemDefinitionFilter>(filter) : null;
        //    return _manager.GetDataAnalysisItemDefinitionsInfo(deserializedFilter);
        //}
    }
}