using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class StatusChargingSet
    {
        public int StatusChargingSetId { get; set; }

        public string Name { get; set; }

        public StatusChargingSetSettings Settings { get; set; }
    }

    public class StatusChargingSetSettings
    {
        public EntityType EntityType { get; set; }

        /// <summary>
        /// this could be AccountTypeId or ServiceTypeId
        /// </summary>
        public string EntityTypeId { get; set; }

        public List<StatusCharge> StatusCharges { get; set; }

        public int CurrencyId { get; set; }
    }

    public class StatusCharge
    {
        public Guid StatusDefinitionId { get; set; }

        public Decimal InitialPrice { get; set; }

        public RecurringPeriodSettings RecurringPeriodSettings { get; set; }

        public Decimal RecurringPrice { get; set; }
    }
}
