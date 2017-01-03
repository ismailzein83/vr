using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountRecurringChargeRuleSet
    {
        public Guid AccountRecurringChargeRuleSetId { get; set; }

        public string Name { get; set; }

        public AccountRecurringChargeRuleSetSettings Settings { get; set; }
    }
}
