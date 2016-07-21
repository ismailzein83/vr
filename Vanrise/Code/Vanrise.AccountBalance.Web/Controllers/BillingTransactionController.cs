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
        [HttpPost]
        [Route("GetFilteredBillingTransactions")]
            public object GetFilteredBillingTransactions(Vanrise.Entities.DataRetrievalInput<BillingTransactionQuery> input)
        {
            BillingTransactionManager manager = new BillingTransactionManager();
            return GetWebResponse(input, manager.GetFilteredBillingTransactions(input));
        }
    }
}