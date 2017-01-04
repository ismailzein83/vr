using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountBalance")]
    public class Retail_BEAccountBalanceController:BaseAPIController
    {
        [HttpGet]
        [Route("GetAccountBalanceTypeId")]
        public Guid GetAccountBalanceTypeId(Guid accountBEDefinitionId)
        {
            AccountBalanceManager manager = new AccountBalanceManager();
            return manager.GetAccountBalanceTypeId(accountBEDefinitionId);
        }
    }
}