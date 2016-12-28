using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountRecurringPriceRuleSet
    {
        public string Name { get; set; }

        public AccountRecurringPriceRuleSetSettings Settings { get; set; }
    }
}
