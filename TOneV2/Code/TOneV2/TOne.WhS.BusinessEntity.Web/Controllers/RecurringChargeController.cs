using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RecurringCharge")]
    public class RecurringChargeController : Vanrise.Web.Base.BaseAPIController
    {
        RecurringChargeManager _manager = new RecurringChargeManager();
        WHSFinancialAccountManager _financialAccountManager = new WHSFinancialAccountManager();

        [HttpGet]
        [Route("GetRecurringChargePeriodsConfigs")]
        public IEnumerable<RecurringChargePeriodConfig> GetRecurringChargePeriodsConfigs()
        {
            SupplierRecurringChargeManager supplierManager = new SupplierRecurringChargeManager();
            return _manager.GetRecurringChargePeriodsConfigs();
        }

        [HttpGet]
        [Route("DoesUserHaveViewAccess")]
        public bool DoesUserHaveViewAccess(int financialAccountId)
        {
            return _financialAccountManager.DoesUserHaveViewAccess(financialAccountId);
        }

    }
}