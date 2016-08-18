using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.BEBridge.Business;
using Vanrise.Web.Base;

namespace Vanrise.BEBridge.Web.Controllers
{
      [RoutePrefix(Constants.ROUTE_PREFIX + "BERecieveDefinition")]
    public class BERecieveDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetBERecieveDefinitionsInfo")]
        public object GetBERecieveDefinitionsInfo()
        {
            BEReceiveDefinitionManager manager = new BEReceiveDefinitionManager();
            return manager.GetBEReceiveDefinitionsInfo();
        }
    }
}