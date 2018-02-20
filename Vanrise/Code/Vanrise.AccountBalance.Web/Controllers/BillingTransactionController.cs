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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BillingTransaction")]
    public class BillingTransactionController : BaseAPIController
    {
        AccountTypeManager _accountTypeManager = new AccountTypeManager();

        [HttpPost]
        [Route("GetFilteredBillingTransactions")]
        public object GetFilteredBillingTransactions(Vanrise.Entities.DataRetrievalInput<BillingTransactionQuery> input)
        {
            if (!_accountTypeManager.DoesUserHaveViewAccess(input.Query.AccountTypeId))
                return GetUnauthorizedResponse();
            BillingTransactionManager manager = new BillingTransactionManager();
            return GetWebResponse(input, manager.GetFilteredBillingTransactions(input));
        }

        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess(Guid accountTypeId)
        {
            return _accountTypeManager.DoesUserHaveAddAccess(accountTypeId);
        }
        [HttpPost]
        [Route("AddBillingTransaction")]
        public object AddBillingTransaction(BillingTransaction billingTransaction)
        {
            if (!DoesUserHaveAddAccess(billingTransaction.AccountTypeId))
                return GetUnauthorizedResponse();
            BillingTransactionManager manager = new BillingTransactionManager();
            return manager.AddBillingTransaction(billingTransaction);
        }
    }
}