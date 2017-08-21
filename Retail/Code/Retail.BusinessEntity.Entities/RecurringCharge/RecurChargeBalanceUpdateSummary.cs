using System;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Entities
{
    public class RecurChargeBalanceUpdateSummary
    {
        public DateTime ChargeDay { get; set; }

        public HashSet<AccountPackageRecurChargeKey> AccountPackageRecurChargeKeys { get; set; }
    }
}
