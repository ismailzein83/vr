using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FixedChargingDefinition
    {
        public Dictionary<Guid, FixedChargingDefinitionRecurringItem> RecurringItems { get; set; }

        public Dictionary<Guid, FixedChargingDefinitionSetupItem> SetupItems { get; set; }
    }

    public class FixedChargingDefinitionRecurringItem
    {
        public Guid ItemId { get; set; }

        public string Title { get; set; }

        public AccountCondition AccountCondition { get; set; }

        public AccountServiceCondition AccountServiceCondition { get; set; }
    }

    public class FixedChargingDefinitionSetupItem
    {
        public Guid ItemId { get; set; }

        public string Title { get; set; }
    }
}
