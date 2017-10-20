using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountManager.Business;
using Vanrise.AccountManager.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.AccountManager.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountManager")]
    [JSONWithTypeAttribute]
    public class AccountManagerController : BaseAPIController
    {
        AMManager _manager = new AMManager();
            
        [HttpPost]
        [Route("GetFilteredAccountManagers")]
        public object GetFilteredAccountManagers(Vanrise.Entities.DataRetrievalInput<AccountManagerQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredAccountManagers(input));
        }
    }
}