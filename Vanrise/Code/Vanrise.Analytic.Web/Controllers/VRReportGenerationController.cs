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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRReportGeneration")]
    [JSONWithTypeAttribute]
    public class VRReportGenerationController : BaseAPIController
    {
        VRReportGenerationManager _manager = new VRReportGenerationManager();

        [HttpPost]
        [Route("GetFilteredVRReportGenerations")]
        public object GetFilteredVRReportGenerations(Vanrise.Entities.DataRetrievalInput<VRReportGenerationQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRReportGenerations(input));
        }

        [HttpGet]
        [Route("GetVRReportGeneration")]
        public VRReportGeneration GetVRReportGeneration(long reportId)
        {
            return _manager.GetVRReportGeneration(reportId);
        }

        [HttpPost]
        [Route("AddVRReportGeneration")]
        public Vanrise.Entities.InsertOperationOutput<VRReportGenerationDetail> AddVRReportGeneration(VRReportGeneration vRReportGenerationItem)
        {
            return _manager.AddVRReportGeneration(vRReportGenerationItem);
        }

        [HttpPost]
        [Route("UpdateVRReportGeneration")]
        public Vanrise.Entities.UpdateOperationOutput<VRReportGenerationDetail> UpdateVRReportGeneration(VRReportGeneration vRReportGenerationItem)
        {
            return _manager.UpdateVRReportGeneration(vRReportGenerationItem);
        }
        [HttpGet]
        [Route("GetReportActionTemplateConfigs")]
        public IEnumerable<VRReportGenerationActionReportActionConfig> GetReportActionTemplateConfigs()
        {
            return _manager.GetReportActionTemplateConfigs();
        }
        [HttpGet]
        [Route("GetVRReportGenerationHistoryDetailbyHistoryId")]
        public VRReportGeneration GetVRReportGenerationHistoryDetailbyHistoryId(int vRReportGenerationHistoryId)
        {
            return _manager.GetVRReportGenerationHistoryDetailbyHistoryId(vRReportGenerationHistoryId);
        }
       
    }
}