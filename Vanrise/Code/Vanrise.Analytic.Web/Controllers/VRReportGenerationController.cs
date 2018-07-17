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
//    public class VRReportGenerationController : BaseAPIController
//    {
//        VRReportGenerationManager _manager = new VRReportGenerationManager();

//        [HttpPost]
//        [Route("GetFilteredVRReports")]
//        public object GetFilteredVRReports(Vanrise.Entities.DataRetrievalInput<VRReportQuery> input)
//        {
//            return GetWebResponse(input, _manager.GetFilteredVRReports(input));
//        }

//        [HttpGet]
//        [Route("GetVRReport")]
//        public VRReport GetVRReport(long vRReportId)
//        {
//            return _manager.GetVRReport(vRReportId);
//        }

//        [HttpPost]
//        [Route("AddVRReport")]
//        public Vanrise.Entities.InsertOperationOutput<DataAnalysisDefinitionDetail> AddVRReport(DataAnalysisDefinition dataAnalysisDefinitionItem)
//        {
//            return _manager.AddVRReport(dataAnalysisDefinitionItem);
//        }

//        [HttpPost]
//        [Route("UpdateVRReport")]
//        public Vanrise.Entities.UpdateOperationOutput<DataAnalysisDefinitionDetail> UpdateVRReport(DataAnalysisDefinition dataAnalysisDefinitionItem)
//        {
//            return _manager.UpdateVRReport(dataAnalysisDefinitionItem);
//        }

//        [HttpGet]
//        [Route("GetVRReportsInfo")]
//        public IEnumerable<DataAnalysisDefinitionInfo> GetVRReportsInfo(string filter = null)
//        {
//            DataAnalysisDefinitionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<DataAnalysisDefinitionFilter>(filter) : null;
//            return _manager.GetDataAnalysisDefinitionsInfo(deserializedFilter);
//        }
//    }
//}