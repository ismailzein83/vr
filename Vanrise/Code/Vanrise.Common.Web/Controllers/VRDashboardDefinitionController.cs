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
        [Route("GetDashboardInfo")]
        public IEnumerable<VRDashboardDefinitionInfo> GetDashboardInfo(string serializedFilter)
        {
            VRDashboardDefinitionFilter dashboardDefinitionFilter = !string.IsNullOrEmpty(serializedFilter) ? Vanrise.Common.Serializer.Deserialize<VRDashboardDefinitionFilter>(serializedFilter) : null;
            return _manager.GetDashboardInfo(dashboardDefinitionFilter);
        }
       
        [HttpGet]
        [Route("GetDashboardEntity")]
        public VRDashboardDefinition GetDashboardEntity(Guid dashboardId)
        {
            return _manager.GetDashboardEntity(dashboardId);
        }
    }
}