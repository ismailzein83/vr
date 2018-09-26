using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Web.Base;

namespace Vanrise.AccountBalance.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountStatement")]
    [JSONWithTypeAttribute]
    public class AccountStatementController:BaseAPIController
    {
        AccountStatementManager _accountStatementManager = new AccountStatementManager();
        AccountTypeManager _accountTypeManager = new AccountTypeManager();
        [HttpPost]
        [Route("GetFilteredAccountStatments")]
        public object GetFilteredAccountStatments(Vanrise.Entities.DataRetrievalInput<AccountStatementQuery> input)
        {
            if (!_accountTypeManager.DoesUserHaveViewAccess(input.Query.AccountTypeId))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _accountStatementManager.GetFilteredAccountStatments(input), "Account Statements");
        }
    }
}