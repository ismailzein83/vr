using CDRComparison.Business;
using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace CDRComparison.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CDRSource")]
    public class CDRSourceController : BaseAPIController
    {
        CDRSourceManager _manager = new CDRSourceManager();

        [HttpPost]
        [Route("ReadSample")]
        public CDRSample ReadSample(CDRSource cdrSource)
        {
            return _manager.ReadSample(cdrSource);
        }
    }
}