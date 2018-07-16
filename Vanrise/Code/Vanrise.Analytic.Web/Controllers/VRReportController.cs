//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http;
//using Vanrise.Analytic.Business;
//using Vanrise.Analytic.Entities;
//using Vanrise.Web.Base;

//namespace Vanrise.Analytic.Web.Controllers
//{
//    [RoutePrefix(Constants.ROUTE_PREFIX + "VRReport")]
//    [JSONWithTypeAttribute]
//    public class VRReportController : BaseAPIController
//    {
//        VRReportManager _manager = new VRReportManager();

//        [HttpPost]
//        [Route("GetFilteredDataAnalysisDefinitions")]
//        public object GetFilteredDataAnalysisDefinitions(Vanrise.Entities.DataRetrievalInput<DataAnalysisDefinitionQuery> input)
//        {
//            return GetWebResponse(input, _manager.GetFilteredDataAnalysisDefinitions(input));
//        }

//        [HttpGet]
//        [Route("GetDataAnalysisDefinition")]
//        public DataAnalysisDefinition GetDataAnalysisDefinition(Guid dataAnalysisDefinitionId)
//        {
//            return _manager.GetDataAnalysisDefinition(dataAnalysisDefinitionId);
//        }

//        [HttpPost]
//        [Route("AddDataAnalysisDefinition")]
//        public Vanrise.Entities.InsertOperationOutput<DataAnalysisDefinitionDetail> AddDataAnalysisDefinition(DataAnalysisDefinition dataAnalysisDefinitionItem)
//        {
//            return _manager.AddDataAnalysisDefinition(dataAnalysisDefinitionItem);
//        }

//        [HttpPost]
//        [Route("UpdateDataAnalysisDefinition")]
//        public Vanrise.Entities.UpdateOperationOutput<DataAnalysisDefinitionDetail> UpdateDataAnalysisDefinition(DataAnalysisDefinition dataAnalysisDefinitionItem)
//        {
//            return _manager.UpdateDataAnalysisDefinition(dataAnalysisDefinitionItem);
//        }

//        [HttpGet]
//        [Route("GetDataAnalysisDefinitionSettingsExtensionConfigs")]
//        public IEnumerable<DataAnalysisDefinitionConfig> GetDataAnalysisDefinitionSettingsExtensionConfigs()
//        {
//            return _manager.GetDataAnalysisDefinitionSettingsExtensionConfigs();
//        }

//        [HttpGet]
//        [Route("GetDataAnalysisDefinitionsInfo")]
//        public IEnumerable<DataAnalysisDefinitionInfo> GetDataAnalysisDefinitionsInfo(string filter = null)
//        {
//            DataAnalysisDefinitionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<DataAnalysisDefinitionFilter>(filter) : null;
//            return _manager.GetDataAnalysisDefinitionsInfo(deserializedFilter);
//        }
//    }
//}