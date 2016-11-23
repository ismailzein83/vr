using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountServiceCondition
    {
        public abstract bool Evaluate(IAccountServiceConditionEvaluationContext context);
    }

    public interface IAccountServiceConditionEvaluationContext
    {
        Account Account { get; }

        AccountService AccountService { get; }
    }
}
