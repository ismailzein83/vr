using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace PartnerPortal.CustomerAccess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountStatement")]
    [JSONWithTypeAttribute]
    public class AccountStatementController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredAccountStatments")]
        public object GetFilteredAccountStatments(Vanrise.Entities.DataRetrievalInput<AccountStatementQuery> input)
        {
            return null;
        }
    }
}