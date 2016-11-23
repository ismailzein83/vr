using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FixedChargingSettings
    {
        public EntityType EntityType { get; set; }

        /// <summary>
        /// this is ServiceTypeId in case EntityType is AccountService
        /// </summary>
        public string EntityTypeId { get; set; }

        public Dictionary<Guid, FixedChargingRecurringCharge> RecurringCharges { get; set; }

        public Dictionary<Guid, FixedChargingSetupCharge> SetupCharges { get; set; }
    }

    public class FixedChargingRecurringCharge
    {
        public Guid ChargingDefinitionItemId { get; set; }

        public RecurringPeriodSettings PeriodSettings { get; set; }

        public Decimal Charge { get; set; }
    }


    public class FixedChargingSetupCharge
    {
        public Guid ChargingDefinitionItemId { get; set; }

        public Decimal Charge { get; set; }
    }
}
