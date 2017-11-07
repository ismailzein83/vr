using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.AccountManager.Business;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.AccountManager.Entities;

namespace Vanrise.AccountManager.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountManagerDefinition")]
    [JSONWithTypeAttribute]
    public class AccountManagerDefinitionController : BaseAPIController
    {
        AccountManagerDefinitionManager _manager = new AccountManagerDefinitionManager();
        [HttpGet]
        [Route("GetAccountManagerDefinition")]
        public BusinessEntityDefinition GetAccountManagerDefinition(Guid accountManagerDefinitionId)
        {
            return _manager.GetAccountManagerDefinition(accountManagerDefinitionId);
        }
        [HttpGet]
        [Route("GetAssignmentDefinitionConfigs")]
        public IEnumerable<AccountManagerAssignmentConfig> GetAssignmentDefinitionConfigs()
        {
            AccountManagerDefinitionManager manager = new AccountManagerDefinitionManager();
            return manager.GetAssignmentDefinitionConfigs();
        }
        [HttpGet]
        [Route("GetSubViewsDefinitionConfigs")]
        public IEnumerable<AccountManagerSubViewsConfig> GetSubViewsDefinitionConfigs()
        {
            AccountManagerDefinitionManager manager = new AccountManagerDefinitionManager();
            return manager.GetSubViewsDefinitionConfigs();
        }
        [HttpGet]
        [Route("GetAccountManagerSubViewsDefinition")]
        public IEnumerable<AccountManagerSubViewDefinition> GetAccountManagerSubViewsDefinition(Guid accountManagerDefinitionId)
        {
            AccountManagerDefinitionManager manager = new AccountManagerDefinitionManager();
            return manager.GetAccountManagerSubViewsDefinition(accountManagerDefinitionId);
        }
    }
}