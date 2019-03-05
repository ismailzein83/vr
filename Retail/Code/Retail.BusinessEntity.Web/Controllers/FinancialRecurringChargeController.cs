using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "FinancialRecurringCharge")]
    public class FinancialRecurringChargeController : BaseAPIController
    {
        FinancialRecurringChargeManager _manager = new FinancialRecurringChargeManager();

        [HttpGet]
        [Route("GetRecurringChargePeriodsConfigs")]
        public IEnumerable<FinancialRecurringChargePeriodConfig> GetRecurringChargePeriodsConfigs()
        {
            return _manager.GetRecurringChargePeriodsConfigs();
        }
    }
}