using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountChargeEvaluator
    {
        public abstract Guid ConfigId { get; }

        public abstract Decimal Evaluate(IAccountChargeEvaluatorContext context);
    }

    public interface IAccountChargeEvaluatorContext
    {
        Account Account { get; }

        int CurrencyId { get; }
    }
}