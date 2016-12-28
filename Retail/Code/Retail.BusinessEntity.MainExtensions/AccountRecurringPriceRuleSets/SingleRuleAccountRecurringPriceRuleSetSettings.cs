using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountRecurringPriceRuleSets
{
    public class SingleRuleAccountRecurringPriceRuleSetSettings : AccountRecurringPriceRuleSetSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public AccountCondition Condition { get; set; }

        public RecurringPrice RecurringPrice { get; set; }

        public override List<RecurringPrice> GetApplicablePrices(IAccountRecurringPriceRuleSetContext context)
        {
            throw new NotImplementedException();
        }
    }
}
