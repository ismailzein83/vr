using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountRecurringPriceRuleSetSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract List<RecurringPrice> GetApplicablePrices(IAccountRecurringPriceRuleSetContext context);
    }

    public interface IAccountRecurringPriceRuleSetContext
    {
        Account Account { get; }
    }
}
