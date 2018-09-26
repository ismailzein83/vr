using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities;
namespace TOne.WhS.Analytics.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CDRLog")]
    public class WhS_CDRController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetCDRLogData")]
        public object GetCDRLogData(Vanrise.Entities.DataRetrievalInput<CDRLogInput> input)
        {
             CDRManager __cdrManager = new CDRManager();
             return GetWebResponse(input, __cdrManager.GetCDRLogData(input), "CDR Logs");
          
        }
    }
}