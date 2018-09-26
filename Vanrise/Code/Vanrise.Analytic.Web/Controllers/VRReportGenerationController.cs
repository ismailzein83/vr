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
        {   if (!_manager.DoesUserHaveViewAccess())
                return GetUnauthorizedResponse();
        return GetWebResponse(input, _manager.GetFilteredVRReportGenerations(input), "Report Generations");
        }

        [HttpGet]
        [Route("GetVRReportGeneration")]
        public VRReportGeneration GetVRReportGeneration(long reportId)
        {
            return _manager.GetVRReportGeneration(reportId);
        }

        [HttpPost]
        [Route("AddVRReportGeneration")]
        public object AddVRReportGeneration(VRReportGeneration vRReportGenerationItem)
        {
            if (!_manager.DoesUserHaveManageAccess() && vRReportGenerationItem.AccessLevel == AccessLevel.Public)
                return GetUnauthorizedResponse();
            return _manager.AddVRReportGeneration(vRReportGenerationItem);
        }

        [HttpPost]
        [Route("UpdateVRReportGeneration")]
        public object UpdateVRReportGeneration(VRReportGeneration vRReportGenerationItem)
        {
            if (!_manager.DoesUserHaveManageAccess() && vRReportGenerationItem.AccessLevel == AccessLevel.Public)
                return GetUnauthorizedResponse();
            return _manager.UpdateVRReportGeneration(vRReportGenerationItem);
        }
        [HttpGet]
        [Route("GetReportActionTemplateConfigs")]
        public IEnumerable<VRReportGenerationActionReportActionConfig> GetReportActionTemplateConfigs()
        {
            return _manager.GetReportActionTemplateConfigs();
        }
        [HttpGet]
        [Route("GetFilterTemplateConfigs")]
        public IEnumerable<VRReportGenerationFilterConfigConfig> GetFilterTemplateConfigs()
        {
            return _manager.GetFilterTemplateConfigs();
        }
        [HttpGet]
        [Route("GetVRReportGenerationHistoryDetailbyHistoryId")]
        public VRReportGeneration GetVRReportGenerationHistoryDetailbyHistoryId(int vRReportGenerationHistoryId)
        {
            return _manager.GetVRReportGenerationHistoryDetailbyHistoryId(vRReportGenerationHistoryId);
        }
        [HttpGet]
        [Route("DoesUserHaveManageAccess")]
        public bool DoesUserHaveManageAccess()
        {
            return _manager.DoesUserHaveManageAccess();
        }
       
    }
}