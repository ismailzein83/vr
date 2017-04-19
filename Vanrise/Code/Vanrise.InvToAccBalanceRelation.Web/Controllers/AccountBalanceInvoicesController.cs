using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.InvToAccBalanceRelation.Business;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.Web.Base;

namespace Vanrise.InvToAccBalanceRelation.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountBalanceInvoices")]
    public class AccountBalanceInvoicesController : Vanrise.Web.Base.BaseAPIController
    {
        AccountBalanceInvoicesManager accountBalanceInvoicesManager = new AccountBalanceInvoicesManager();
      
        [HttpPost]
        [Route("GetFilteredAccountInvoices")]
        public object GetFilteredAccountInvoices(Vanrise.Entities.DataRetrievalInput<AccountInvoicesQuery> input)
        {
            return GetWebResponse(input, accountBalanceInvoicesManager.GetFilteredAccountInvoices(input));
        }
    }
}