using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Web.Base;

namespace Vanrise.AccountBalance.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Account")]
    public class AccountController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetAccountInfo")]
        public AccountInfo GetAccountInfo(Guid accountTypeId, string accountId)
        {
            return new AccountManager().GetAccountInfo(accountTypeId, accountId);
        }
    }
}