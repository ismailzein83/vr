using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RawCDR")]
    public class RawCDRController : Vanrise.Web.Base.BaseAPIController
    {
        //[HttpPost]
        //[Route("GetRawCDRData")]
        //public object GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRInput> input)
        //{
        //    //RawCDRManager manager = new RawCDRManager();
        //    return null;//GetWebResponse(input, manager.GetRawCDRData(input));
        //}
        
    }
}