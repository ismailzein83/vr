using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class ManualAddEnabledBillingTransactionTypeFilter : IBillingTransactionTypeFilter
    {

        public bool IsMatched(BillingTransactionTypeSettings context)
        {

            if (context == null || !context.ManualAdditionDisabled)
                return true;
            return false;
        }
    }
}
