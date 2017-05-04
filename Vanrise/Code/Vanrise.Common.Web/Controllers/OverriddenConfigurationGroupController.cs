using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "OverriddenConfigurationGroup")]
    public class OverriddenConfigurationGroupController : BaseAPIController
    {
        [HttpGet]
        [Route("GetOverriddenConfigurationGroupInfo")]
        public IEnumerable<OverriddenConfigurationGroupInfo> GetOverriddenConfigurationGroupInfo(string filter = null)
        {
            var manager = new OverriddenConfigurationGroupManager();
            OverriddenConfigurationGroupInfoFilter overriddenConfigurationGroupInfo = (filter != null) ? Vanrise.Common.Serializer.Deserialize<OverriddenConfigurationGroupInfoFilter>(filter) : null;
            return manager.GetOverriddenConfigurationGroupInfo(overriddenConfigurationGroupInfo);
        }

        [HttpPost]
        [Route("AddOverriddenConfigurationGroup")]
        public Vanrise.Entities.InsertOperationOutput<OverriddenConfigGroupDetail> AddOverriddenConfigurationGroup(OverriddenConfigurationGroup overriddenConfigurationGroup)
        {
            var manager = new OverriddenConfigurationGroupManager();
            return manager.AddOverriddenConfigurationGroup(overriddenConfigurationGroup);
        }
    }
}