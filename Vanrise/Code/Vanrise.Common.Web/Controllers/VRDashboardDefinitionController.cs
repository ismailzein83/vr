using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRDashboardDefinition")]
    [JSONWithTypeAttribute]

    public class VRDashboardDefinitionController : BaseAPIController
    {
        VRDashboardDefinitionManager _manager = new VRDashboardDefinitionManager();

        [HttpGet]
        [Route("GetDashboardDefinitionInfo")]
        public IEnumerable<VRDashboardDefinitionInfo> GetDashboardDefinitionInfo(string serializedFilter)
        {
            VRDashboardDefinitionFilter dashboardDefinitionFilter = !string.IsNullOrEmpty(serializedFilter) ? Vanrise.Common.Serializer.Deserialize<VRDashboardDefinitionFilter>(serializedFilter) : null;
            return _manager.GetDashboardDefinitionInfo(dashboardDefinitionFilter);
        }
       
        [HttpGet]
        [Route("GetDashboardDefinitionEntity")]
        public VRDashboardDefinition GetDashboardDefinitionEntity(Guid dashboardDefinitonId)
        {
            return _manager.GetDashboardDefinitionEntity(dashboardDefinitonId);
        }
    }
}