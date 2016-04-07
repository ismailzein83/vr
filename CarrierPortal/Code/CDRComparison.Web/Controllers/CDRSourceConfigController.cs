using CDRComparison.Business;
using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CDRComparison.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CDRSourceConfig")]
    public class CDRSourceConfigController : BaseAPIController
    {
        CDRSourceConfigManager _manager = new CDRSourceConfigManager();

        [HttpGet]
        [Route("GetCDRSourceConfigs")]
        public IEnumerable<CDRSourceConfig> GetCDRSourceConfigs(string filter = null)
        {
            CDRSourceConfigFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<CDRSourceConfigFilter>(filter) : null;
            return _manager.GetCDRSourceConfigs(deserializedFilter);
        }

        [HttpGet]
        [Route("GetCDRSourceConfig")]
        public CDRSourceConfig GetCDRSourceConfig(int cdrSourceConfigId)
        {
            return _manager.GetCDRSourceConfig(cdrSourceConfigId);
        }
    }
}