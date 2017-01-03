using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountRecurringChargeRuleSetSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract List<ApplicableRecurringCharge> GetApplicableCharges(IAccountRecurringChargeRuleSetContext context);
    }

    public interface IAccountRecurringChargeRuleSetContext
    {
        Account Account { get; }
    }
}
