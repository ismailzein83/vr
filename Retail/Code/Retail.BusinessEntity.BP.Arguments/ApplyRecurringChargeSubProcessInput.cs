using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;

namespace Retail.BusinessEntity.BP.Arguments
{
    public class ApplyRecurringChargeInput
    {
        public DateTime ChargeDay { get; set; }

        public List<AccountPackageRecurCharge> AccountPackageRecurChargeList { get; set; }
    }
}
