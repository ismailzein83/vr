using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountRecurringPriceRule
    {
        public string Name { get; set; }

        public AccountCondition Condition { get; set; }

        public RecurringPrice RecurringPrice { get; set; }
    }
}
