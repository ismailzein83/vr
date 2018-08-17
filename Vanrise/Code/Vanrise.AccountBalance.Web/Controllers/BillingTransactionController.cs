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
        BillingTransactionManager _billingTransactionManager = new BillingTransactionManager();

        [HttpPost]
        [Route("GetFilteredBillingTransactions")]
        public object GetFilteredBillingTransactions(Vanrise.Entities.DataRetrievalInput<BillingTransactionQuery> input)
        {
            if (!DoesUserHaveViewAccess(input.Query.AccountTypeId))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _billingTransactionManager.GetFilteredBillingTransactions(input));
        }

        [HttpGet]
        [Route("DoesUserHaveViewAccess")]
        public bool  DoesUserHaveViewAccess(Guid accountTypeId)
        {
            return _accountTypeManager.DoesUserHaveViewAccess(accountTypeId);              
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
            return _billingTransactionManager.AddBillingTransaction(billingTransaction);
        }

        [HttpGet]
        [Route("GetBillingTransactionById")]
        public BillingTransaction GetBillingTransactionById(long billingTransactionId)
        {
            return _billingTransactionManager.GetBillingTransactionById(billingTransactionId);
        }
    }
}