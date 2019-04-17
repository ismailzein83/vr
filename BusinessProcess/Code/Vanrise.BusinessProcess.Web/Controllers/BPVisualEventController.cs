using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPVisualEvent")]

    public class BPVisualEventController: BaseAPIController
    {
        [HttpPost]
        [Route("GetAfterId")]
        public BPVisuaIEventDetailUpdateOutput GetAfterId(BPVisualEventDetailUpdateInput input)
        {
            BPVisualEventManager manager = new BPVisualEventManager();
            return manager.GetAfterId(input);
        }
    }
}