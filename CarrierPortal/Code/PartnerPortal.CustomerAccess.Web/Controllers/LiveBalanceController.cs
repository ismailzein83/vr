using PartnerPortal.CustomerAccess.Business;
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
        public CurrentAccountBalance GetCurrentAccountBalance(Guid accountTypeId, Guid connectionId)
        {
            LiveBalanceManager manager = new LiveBalanceManager();
            return manager.GetCurrentAccountBalance(connectionId, accountTypeId);
        }
    }
}