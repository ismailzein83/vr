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
            // var effective = supplierManager.GetEffectiveSupplierRecurringCharges(new DateTime(2018, 4, 18), new DateTime(2021, 1, 1));

            //CustomerRecurringChargeManager customerManager = new CustomerRecurringChargeManager();
            //var effective = customerManager.GetEffectiveCustomerRecurringCharges(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31));
            // var recurringCharges = customerManager.GetEffectiveCustomerRecurringCharges(45, new DateTime(2018, 1, 1), new DateTime(2018, 12, 31));
            var recurringCharges = supplierManager.GetEvaluatedRecurringCharges(45, new DateTime(2018, 1, 1), new DateTime(2018, 12, 31));

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