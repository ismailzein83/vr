using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountBalance.Entities;
using Vanrise.Web.Base;

namespace PartnerPortal.CustomerAccess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "LiveBalance")]
    [JSONWithTypeAttribute]
    public class LiveBalanceController:BaseAPIController
    {
        [HttpGet]
        [Route("GetCurrentAccountBalance")]
        public CurrentAccountBalanceTile GetCurrentAccountBalance(Guid accountTypeId, Guid connectionId, Guid? viewId = null)
        {
            LiveBalanceManager manager = new LiveBalanceManager();
            return manager.GetCurrentAccountBalance(connectionId, accountTypeId, viewId);
        }
    }
}