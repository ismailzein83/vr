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
        public IEnumerable<VRDashboardInfo> GetDashboardInfo(string serializedFilter)
        {
            VRDashboardFilter dashboardDefinitionFilter = !string.IsNullOrEmpty(serializedFilter) ? Vanrise.Common.Serializer.Deserialize<VRDashboardFilter>(serializedFilter) : null;
            return _manager.GetDashboardInfo(dashboardDefinitionFilter);
        }
       
        [HttpGet]
        [Route("GetDashboardEntity")]
        public VRDashboard GetDashboardEntity(Guid dashboardId)
        {
            return _manager.GetDashboardEntity(dashboardId);
        }
    }
}