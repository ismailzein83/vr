using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountRecurringPriceRuleSets
{
    public class ApplyFirstAccountRecurringPriceRuleSetSettings : AccountRecurringPriceRuleSetSettings
    {
        public List<AccountRecurringPriceRule> PricingRules { get; set; }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public override List<RecurringPrice> GetApplicablePrices(IAccountRecurringPriceRuleSetContext context)
        {
            throw new NotImplementedException();
        }
    }
}
