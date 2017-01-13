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
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountType")]
    [JSONWithTypeAttribute]
    public class VRAccountTypeController : BaseAPIController
    {
        AccountTypeManager _accountTypeManager = new AccountTypeManager();
        [HttpGet]
        [Route("GetAccountSelector")]
        public string GetAccountSelector(Guid accountTypeId)
        {
            return _accountTypeManager.GetAccountSelector(accountTypeId);
        }

        [HttpGet]
        [Route("GetAccountTypeSettings")]
        public AccountTypeSettings GetAccountTypeSettings(Guid accountTypeId)
        {
            return _accountTypeManager.GetAccountTypeSettings(accountTypeId);
        }
    }
}