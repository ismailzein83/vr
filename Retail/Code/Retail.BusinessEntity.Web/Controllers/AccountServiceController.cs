using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountService")]
    [JSONWithTypeAttribute]
    public class AccountServiceController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredAccountServices")]
        public object GetFilteredAccountServices(Vanrise.Entities.DataRetrievalInput<AccountServiceQuery> input)
        {
            AccountServiceManager manager = new AccountServiceManager();
            return GetWebResponse(input, manager.GetFilteredAccountServices(input));
        }

        [HttpGet]
        [Route("GetAccountService")]
        public AccountService GetAccountService(long accountServiceId)
        {
            AccountServiceManager manager = new AccountServiceManager();
            return manager.GetAccountService(accountServiceId);
        }

        [HttpPost]
        [Route("AddAccountService")]
        public InsertOperationOutput<AccountServiceDetail> AddAccountService(AccountService accountService)
        {
            AccountServiceManager manager = new AccountServiceManager();
            return manager.AddAccountService(accountService);
        }
     
        [HttpPost]
        [Route("UpdateAccountService")]
        public UpdateOperationOutput<AccountServiceDetail> UpdateAccountService(AccountService accountService)
        {
            AccountServiceManager manager = new AccountServiceManager();
            return manager.UpdateAccountService(accountService);
        }
        [HttpGet]
        [Route("GetAccountServiceDetail")]
        public AccountServiceDetail GetAccountServiceDetail(long accountServiceId)
        {
            AccountServiceManager manager = new AccountServiceManager();
            return manager.GetAccountServiceDetail(accountServiceId);
        }
    }
}