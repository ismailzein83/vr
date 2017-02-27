using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.AccountBalance.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountBalance")]
    public class WHS_AccountBalanceController : BaseAPIController
    {
        AccountBalanceManager _manager = new AccountBalanceManager();
    }
}