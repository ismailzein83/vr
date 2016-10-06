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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Account")]
    public class AccountController : BaseAPIController
    {
        AccountManager _manager = new AccountManager();

        [HttpPost]
        [Route("GetFilteredAccounts")]
        public object GetFilteredAccounts(Vanrise.Entities.DataRetrievalInput<AccountQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredAccounts(input));
        }

        [HttpGet]
        [Route("GetAccount")]
        public Account GetAccount(long accountId)
        {
            return _manager.GetAccount(accountId);
        }

        [HttpGet]
        [Route("GetAccountsInfo")]
        public IEnumerable<AccountInfo> GetAccountsInfo(string nameFilter)
        {
            return _manager.GetAccountsInfo(nameFilter);
        }

        [HttpPost]
        [Route("GetAccountsInfoByIds")]
        public IEnumerable<AccountInfo> GetAccountsInfoByIds(HashSet<long> accountIds)
        {
            return _manager.GetAccountsInfoByIds(accountIds);
        }

        [HttpGet]
        [Route("GetAccountName")]
        public string GetAccountName(long accountId)
        {
            return _manager.GetAccountName(accountId);
        }

        [HttpGet]
        [Route("GetAccountEditorRuntime")]
        public AccountEditorRuntime GetAccountEditorRuntime(Guid accountTypeId, int? parentAccountId = null)
        {
            return _manager.GetAccountEditorRuntime(accountTypeId, parentAccountId);
        }


        [HttpGet]
        [Route("GetAccountDetail")]
        public AccountDetail GetAccountDetail(long accountId)
        {
            return _manager.GetAccountDetail(accountId);
        }


        [HttpPost]
        [Route("AddAccount")]
        public Vanrise.Entities.InsertOperationOutput<AccountDetail> AddAccount(Account account)
        {
            return _manager.AddAccount(account);
        }

        [HttpPost]
        [Route("UpdateAccount")]
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(AccountToEdit account)
        {
            return _manager.UpdateAccount(account);
        }
    }
}