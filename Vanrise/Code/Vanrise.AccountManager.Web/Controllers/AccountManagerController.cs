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
            if (!_manager.DoesUserHaveViewAccess(input.Query.AccountManagerDefinitionId))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredAccountManagers(input), "Account Managers");
        }
        [HttpPost]
        [Route("AddAccountManager")]
        public object AddAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager)
        {
            if (!DoesUserHaveAddAccess(accountManager.AccountManagerDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.AddAccountManager(accountManager);

        }
        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess(Guid accountManagerDefinitionId)
        {
            return _manager.DoesUserHaveAddAccess(accountManagerDefinitionId);
        }
        [HttpGet]
        [Route("HasEditAccountManagerPermission")]
        public bool HasEditAccountManagerPermission(Guid accountManagerDefinitionId)
        {
            return _manager.DoesUserHaveEditAccess(accountManagerDefinitionId);
        }

        [HttpPost]
        [Route("UpdateAccountManager")]
        public object UpdateAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager)
        {
            if (!_manager.DoesUserHaveEditAccess(accountManager.AccountManagerDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.UpdateAccountManager(accountManager);
        }
        [HttpGet]
        [Route("GetAccountManager")]
        public Vanrise.AccountManager.Entities.AccountManager GetAccountManager(long accountManagerId)
        {
            return _manager.GetAccountManager(accountManagerId);
        }
        [HttpGet]
        [Route("GetAccountManagerDefinitionConfigs")]
        public IEnumerable<AccountManagerDefinitionConfig> GetAccountManagerDefinitionConfigs()
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetAccountManagerDefinitionConfigs();
        }
        [HttpGet]
        [Route("GetAccountManagerInfo")]
        public IEnumerable<AccountManagerInfo> GetAccountManagerInfo(string filter = null)
        {
            AccountManagerFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<AccountManagerFilter>(filter) : null;
            return _manager.GetAccountManagerInfo(serializedFilter);
        }

    }
}