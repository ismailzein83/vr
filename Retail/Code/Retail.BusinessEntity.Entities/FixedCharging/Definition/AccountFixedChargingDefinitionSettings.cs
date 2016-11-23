using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountFixedChargingDefinitionSettings
    {
        public Dictionary<Guid, AccountRecurringChargingDefinitionItem> RecurringChargingDefinitionItems { get; set; }

        public Dictionary<Guid, AccountSetupChargingDefinitionItem> SetupChargingDefinitionItems { get; set; }
    }

    public class AccountRecurringChargingDefinitionItem
    {
        public Guid ItemId { get; set; }

        public string Title { get; set; }

        public AccountCondition ChargeCondition { get; set; }
    }

    public class AccountSetupChargingDefinitionItem
    {
        public Guid ItemId { get; set; }

        public string Title { get; set; }

        public AccountCondition ChargeCondition { get; set; }
    }
}
