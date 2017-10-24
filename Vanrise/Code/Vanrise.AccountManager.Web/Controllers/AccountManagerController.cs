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
        AccountManagerManager _manager = new AccountManagerManager();
            
        [HttpPost]
        [Route("GetFilteredAccountManagers")]
        public object GetFilteredAccountManagers(Vanrise.Entities.DataRetrievalInput<AccountManagerQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredAccountManagers(input));
        }
        [HttpPost]
        [Route("AddAccountManager")]
        public InsertOperationOutput<AccountManagerDetail> AddAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager)
        {
            return _manager.AddAccountManager(accountManager);

        }
        [HttpPost]
        [Route("UpdateAccountManager")]
        public UpdateOperationOutput<AccountManagerDetail> UpdateAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager)
        {
            return _manager.UpdateAccountManager(accountManager);
        }
        [HttpGet]
        [Route("GetAccountManager")]
        public Vanrise.AccountManager.Entities.AccountManager GetAccountManager(long accountManagerId)
        {
            return _manager.GetAccountManager(accountManagerId);
        }

    }
}