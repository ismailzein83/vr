using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Web.Controllers
{
    public class RawCDRLogController : Vanrise.Web.Base.BaseAPIController
    {
         [HttpPost]
        public object GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRInput> input)
        {
            RawCDRLogManager manager = new RawCDRLogManager();
            return GetWebResponse(input, manager.GetRawCDRData(input));
          
        }
        
    }
}