using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RecurringCharge")]
    [JSONWithTypeAttribute]
    public class RecurringChargeController : BaseAPIController
    {
        RecurringChargeManager _manager = new RecurringChargeManager();

        [HttpGet]
        [Route("GetAccountRecurringChargeEvaluatorExtensionConfigs")]
        public IEnumerable<AccountRecurringChargeEvaluatorConfig> GetAccountRecurringChargeEvaluatorExtensionConfigs()
        {
            return _manager.GetAccountRecurringChargeEvaluatorExtensionConfigs();
        }
    }
}