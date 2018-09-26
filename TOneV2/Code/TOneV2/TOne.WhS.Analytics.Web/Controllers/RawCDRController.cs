using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RawCDR")]
    public class RawCDRController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetRawCDRData")]
        public object GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRInput> input)
        {
            RawCDRManager manager = new RawCDRManager();
            return GetWebResponse(input, manager.GetRawCDRData(input), "Raw CDRs");
        }
        
    }
}