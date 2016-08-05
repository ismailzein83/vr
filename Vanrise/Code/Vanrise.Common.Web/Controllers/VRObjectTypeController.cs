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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRObjectType")]
    [JSONWithTypeAttribute]
    public class VRObjectTypeController : BaseAPIController
    {
        VRObjectTypeManager _manager = new VRObjectTypeManager();

        [HttpGet]
        [Route("GetObjectTypeExtensionConfigs")]
        public IEnumerable<VRObjectTypeConfig> GetStyleFormatingExtensionConfigs()
        {
            System.Threading.Thread.Sleep(2000);
            return _manager.GetObjectTypeExtensionConfigs();
        }
    }
}