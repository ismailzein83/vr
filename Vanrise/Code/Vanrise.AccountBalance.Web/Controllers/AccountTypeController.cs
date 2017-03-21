using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;
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
        [HttpGet]
        [Route("GetAccountTypeSourceSettingsConfigs")]
        public IEnumerable<AccountTypeSourcesConfig> GetAccountTypeSourceSettingsConfigs()
        {
            return _accountTypeManager.GetAccountTypeSourceSettingsConfigs();
        }
        [HttpPost]
        [Route("GetAccountTypeSourcesFields")]
        public Dictionary<Guid,IEnumerable<AccountBalanceFieldDefinition>> GetAccountTypeSourcesFields(AccountTypeSourceQuery query)
        {
            return _accountTypeManager.GetAccountTypeSourcesFields(query.Sources);
        }
        [HttpPost]
        [Route("GetAccountTypeSourceFields")]
        public IEnumerable<AccountBalanceFieldDefinition> GetAccountTypeSourceFields(AccountBalanceFieldSource source)
        {
            return _accountTypeManager.GetAccountTypeSourceFields(source);
        }

        [HttpGet]
        [Route("ConvertToGridColumnAttribute")]
        public IEnumerable<GridColumnAttribute> ConvertToGridColumnAttribute(Guid accountTypeId)
        {
            return _accountTypeManager.ConvertToGridColumnAttribute(accountTypeId);
        }
    }
    public class AccountTypeSourceQuery
    {
        public List<AccountBalanceFieldSource> Sources { get; set; }
    }
}