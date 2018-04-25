using Retail.BusinessEntity.APIEntities;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountBE")]
    public class AccountBEController : BaseAPIController
    {
        AccountBEManager _manager = new AccountBEManager();
        AccountBEDefinitionManager _beManager = new AccountBEDefinitionManager();

        [HttpPost]
        [Route("GetFilteredClientAccounts")]
        public object GetFilteredClientAccounts(Vanrise.Entities.DataRetrievalInput<AccountQuery> input)
        {

            if (!_beManager.DoesUserHaveViewAccess(input.Query.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredClientAccounts(input));
        }

        [HttpPost]
        [Route("GetFilteredAccounts")]
        public object GetFilteredAccounts(Vanrise.Entities.DataRetrievalInput<AccountQuery> input)
        {

            if (!_beManager.DoesUserHaveViewAccess(input.Query.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredAccounts(input));
        }

        [HttpGet]
        [Route("GetAccount")]
        public Account GetAccount(Guid accountBEDefinitionId, long accountId)
        {
            return _manager.GetAccount(accountBEDefinitionId, accountId);
        }

        [HttpGet]
        [Route("GetAccountName")]
        public string GetAccountName(Guid accountBEDefinitionId, long accountId)
        {
            return _manager.GetAccountName(accountBEDefinitionId, accountId);
        }

        [HttpGet]
        [Route("GetAccountDetail")]
        public AccountDetail GetAccountDetail(Guid accountBEDefinitionId, long accountId)
        {
            return _manager.GetAccountDetail(accountBEDefinitionId, accountId);
        }
        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess(Guid accountBEDefinitionId)
        {
            return _beManager.DoesUserHaveAddAccess(accountBEDefinitionId);
        }

        [HttpPost]
        [Route("AddAccount")]
        public object AddAccount(AccountToInsert accountToInsert)
        {
            if (!DoesUserHaveAddAccess(accountToInsert.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.AddAccount(accountToInsert);
        }

        [HttpPost]
        [Route("UpdateAccount")]
        public object UpdateAccount(AccountToEdit accountToEdit)
        {
            if (!_beManager.DoesUserHaveEditAccess(accountToEdit.AccountBEDefinitionId))
                return  GetUnauthorizedResponse();
            return _manager.UpdateAccount(accountToEdit);
        }

        [HttpGet]
        [Route("GetAccountsInfo")]
        public IEnumerable<AccountInfo> GetAccountsInfo(Guid accountBEDefinitionId, string serializedFilter, string nameFilter = null)
        {
            AccountFilter accountFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<AccountFilter>(serializedFilter) : null;
            return _manager.GetAccountsInfo(accountBEDefinitionId, nameFilter, accountFilter);
        }

        [HttpPost]
        [Route("GetAccountsInfoByIds")]
        public IEnumerable<AccountInfo> GetAccountsInfoByIds(AccountInfoFilter filter)
        {
            return _manager.GetAccountsInfoByIds(filter.AccountBEDefinition, filter.AccountIds);
        }

        [HttpGet]
        [Route("GetAccountEditorRuntime")]
        public AccountEditorRuntime GetAccountEditorRuntime(Guid accountBEDefinitionId, Guid accountTypeId, int? parentAccountId = null)
        {
            return _manager.GetAccountEditorRuntime(accountBEDefinitionId, accountTypeId, parentAccountId);
        }

        [HttpGet]
        [Route("GetChildAccountIds")]
        public List<long> GetChildAccountIds(Guid accountBEDefinitionId, long accountId, bool withSubChildren)
        {
            return _manager.GetChildAccountIds(accountBEDefinitionId, accountId, withSubChildren);
        }
        [HttpGet]
        [Route("GetClientChildAccountsInfo")]
        public IEnumerable<ClientAccountInfo> GetClientChildAccountsInfo(Guid accountBEDefinitionId, long accountId, bool withSubChildren)
        {
            return _manager.GetClientChildAccountsInfo(accountBEDefinitionId, accountId, withSubChildren);
        }
        [HttpPost]
        [Route("ExecuteAccountBulkActions")]
        public object ExecuteAccountBulkActions(ExecuteAccountBulkActionProcessInput input)
        {
            return _manager.ExecuteAccountBulkActions(input);
        }
    }

    public class AccountInfoFilter
    {
        public Guid AccountBEDefinition { get; set; }
        public HashSet<long> AccountIds { get; set; }
    }
}