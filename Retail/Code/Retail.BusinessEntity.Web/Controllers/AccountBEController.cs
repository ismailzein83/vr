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

        [HttpPost]
        [Route("GetFilteredAccounts")]
        public object GetFilteredAccounts(Vanrise.Entities.DataRetrievalInput<AccountQuery> input)
        {
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
            return _manager.GetAccountName(accountBEDefinitionId,accountId);
        }

        [HttpGet]
        [Route("GetAccountDetail")]
        public AccountDetail GetAccountDetail(Guid accountBEDefinitionId, long accountId)
        {
            return _manager.GetAccountDetail(accountBEDefinitionId, accountId);
        }

        [HttpPost]
        [Route("AddAccount")]
        public Vanrise.Entities.InsertOperationOutput<AccountDetail> AddAccount(AccountToInsert accountToInsert)
        {
            return _manager.AddAccount(accountToInsert);
        }

        [HttpPost]
        [Route("UpdateAccount")]
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(AccountToEdit accountToEdit)
        {
            return _manager.UpdateAccount(accountToEdit);
        }

        [HttpGet]
        [Route("GetAccountsInfo")]
        public IEnumerable<AccountInfo> GetAccountsInfo(Guid accountBEDefinitionId, string nameFilter, string serializedFilter)
        {
            AccountFilter accountFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<AccountFilter>(serializedFilter) : null;
            return _manager.GetAccountsInfo(accountBEDefinitionId, nameFilter, accountFilter);
        }

        //[HttpPost]
        //[Route("GetAccountsInfoByIds")]
        //public IEnumerable<AccountInfo> GetAccountsInfoByIds(Guid accountBEDefinitionId, HashSet<long> accountIds)
        //{
        //    return _manager.GetAccountsInfoByIds(accountBEDefinitionId, accountIds);
        //}

        [HttpGet]
        [Route("GetAccountEditorRuntime")]
        public AccountEditorRuntime GetAccountEditorRuntime(Guid accountBEDefinitionId, Guid accountTypeId, int? parentAccountId = null)
        {
            return _manager.GetAccountEditorRuntime(accountBEDefinitionId, accountTypeId, parentAccountId);
        }
    }
}