using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace PartnerPortal.CustomerAccess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DID")]
    [JSONWithTypeAttribute]
    public class CPDIDController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredDIDs")]
        public object GetFilteredDIDs(DataRetrievalInput<DIDAppQuery> input)
        {
            DIDManager manager = new DIDManager();
            return GetWebResponse(input, manager.GetFilteredDIDs(input));
        }
    }
}