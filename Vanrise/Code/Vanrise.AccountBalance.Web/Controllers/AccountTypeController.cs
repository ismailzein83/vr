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
        public Dictionary<Guid,IEnumerable<AccountBalanceFieldDefinition>> GetAccountTypeSourcesFields(AccountTypeSourcesQuery query)
        {
            return _accountTypeManager.GetAccountTypeSourcesFields(query.Sources, query.AccountTypeSettings);
        }
        [HttpPost]
        [Route("GetAccountTypeSourceFields")]
        public IEnumerable<AccountBalanceFieldDefinition> GetAccountTypeSourceFields(AccountTypeSourceQuery query)
        {
            return _accountTypeManager.GetAccountTypeSourceFields(query.Source, query.AccountTypeSettings);
        }

        [HttpGet]
        [Route("ConvertToGridColumnAttribute")]
        public IEnumerable<AccountTypeGridFieldColumnAttribute> ConvertToGridColumnAttribute(Guid accountTypeId)
        {
            return _accountTypeManager.ConvertToGridColumnAttribute(accountTypeId);
        }
    }
    public class AccountTypeSourcesQuery
    {
        public List<AccountBalanceFieldSource> Sources { get; set; }
        public AccountTypeSettings AccountTypeSettings { get; set; }
    }
    public class AccountTypeSourceQuery
    {
        public AccountBalanceFieldSource Source { get; set; }
        public AccountTypeSettings AccountTypeSettings { get; set; }
    }
}