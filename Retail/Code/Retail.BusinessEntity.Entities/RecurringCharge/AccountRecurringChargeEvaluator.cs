using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountRecurringChargeEvaluator
    {
        public abstract Guid ConfigId { get; }

        public abstract Decimal Evaluate(IAccountRecurringChargeEvaluatorContext context);
    }

    public interface IAccountRecurringChargeEvaluatorContext
    {
        Account Account { get; }

        int CurrencyId { get; }
    }
}