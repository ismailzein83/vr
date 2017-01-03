using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class RecurringChargeDefinition
    {
        public Guid RecurringChargeDefinitionId { get; set; }

        public string Name { get; set; }

        public RecurringChargeDefinitionSettings Settings { get; set; }
    }

    public class RecurringChargeDefinitionSettings
    {
        public RecurringPeriodSettings RecurringPeriod { get; set; }
    }
}
