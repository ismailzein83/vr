using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CDR")]
    [JSONWithTypeAttribute]
    public class CDRController : BaseAPIController
    {
        CDRManager cdrManager = new CDRManager();

        [HttpPost]
        [Route("GetCDR")]
        public List<CDR> GetCDR(DataRetrievalInput<CDRQuery> input)
        {
            return cdrManager.GetCDR(input);
        }
       
    }
}
