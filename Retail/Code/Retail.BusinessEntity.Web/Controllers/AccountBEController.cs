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
        public Account GetAccount(Guid accountDefinitionId, long accountId)
        {
            return _manager.GetAccount(accountDefinitionId, accountId);
        }

        [HttpGet]
        [Route("GetAccountName")]
        public string GetAccountName(Guid accountDefinitionId, long accountId)
        {
            return _manager.GetAccountName(accountDefinitionId,accountId);
        }

        [HttpPost]
        [Route("AddAccount")]
        public Vanrise.Entities.InsertOperationOutput<AccountDetail> AddAccount(Guid accountDefinitionId, Account account)
        {
            return _manager.AddAccount(accountDefinitionId, account);
        }

        [HttpPost]
        [Route("UpdateAccount")]
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(Guid accountDefinitionId, AccountToEdit account)
        {
            return _manager.UpdateAccount(accountDefinitionId, account);
        }

        [HttpGet]
        [Route("GetAccountsInfo")]
        public IEnumerable<AccountInfo> GetAccountsInfo(Guid accountDefinitionId, string nameFilter, string serializedFilter)
        {
            AccountFilter accountFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<AccountFilter>(serializedFilter) : null;
            return _manager.GetAccountsInfo(accountDefinitionId, nameFilter, accountFilter);
        }


        //[HttpPost]
        //[Route("GetAccountsInfoByIds")]
        //public IEnumerable<AccountInfo> GetAccountsInfoByIds(Guid accountDefinitionId, HashSet<long> accountIds)
        //{
        //    return _manager.GetAccountsInfoByIds(accountDefinitionId, accountIds);
        //}
    }
}