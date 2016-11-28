using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Account")]
    [JSONWithTypeAttribute]
    public class AccountController : BaseAPIController
    {
    //    AccountManager _manager = new AccountManager();

        //[HttpPost]
        //[Route("GetFilteredAccounts")]
        //public object GetFilteredAccounts(Vanrise.Entities.DataRetrievalInput<AccountQuery> input)
        //{
        //    return GetWebResponse(input, _manager.GetFilteredAccounts(input));
        //}

        //[HttpGet]
        //[Route("GetAccount")]
        //public Account GetAccount(int accountId)
        //{
        //    return _manager.GetAccount(accountId);
        //}

        //[HttpPost]
        //[Route("AddAccount")]
        //public Vanrise.Entities.InsertOperationOutput<AccountDetail> AddAccount(Account accountItem)
        //{
        //    return _manager.AddAccount(accountItem);
        //}

        //[HttpPost]
        //[Route("UpdateAccount")]
        //public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(Account accountItem)
        //{
        //    return _manager.UpdateAccount(accountItem);
        //}
    }
}