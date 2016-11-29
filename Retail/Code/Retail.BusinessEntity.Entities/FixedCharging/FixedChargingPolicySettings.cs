using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FixedChargingPolicySettings
    {
        public Dictionary<Guid, FixedChargingPolicyRecurringCharge> RecurringCharges { get; set; }

        public Dictionary<Guid, FixedChargingPolicySetupCharge> SetupCharges { get; set; }
    }

    public class FixedChargingPolicyRecurringCharge
    {
        public Guid ChargingDefinitionItemId { get; set; }

        public RecurringPeriodSettings PeriodSettings { get; set; }

        public Decimal Charge { get; set; }
    }


    public class FixedChargingPolicySetupCharge
    {
        public Guid ChargingDefinitionItemId { get; set; }

        public Decimal Charge { get; set; }
    }
}
