using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CDRLog")]
    public class CDRLogController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetCDRLogData")]
        public object GetCDRLogData(Vanrise.Entities.DataRetrievalInput<CDRQuery> input)
        {
            CDRLogManager manager = new CDRLogManager();
            return GetWebResponse(input, manager.GetCDRLogData(input));
        }
        
    }
}