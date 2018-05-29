using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business.RecurringCharges;
using TOne.WhS.BusinessEntity.Entities.RecurringCharges;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RecurringCharge")]
    public class RecurringChargeController : Vanrise.Web.Base.BaseAPIController
    {
        RecurringChargeManager _manager = new RecurringChargeManager();

        [HttpGet]
        [Route("GetRecurringChargePeriodsConfigs")]
        public IEnumerable<RecurringChargePeriodConfig> GetRecurringChargePeriodsConfigs()
        {
            return _manager.GetRecurringChargePeriodsConfigs();
        }
    }
}