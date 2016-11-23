using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountCondition
    {
        public abstract bool Evaluate(IAccountConditionEvaluationContext context);
    }

    public interface IAccountConditionEvaluationContext
    {
        Account Account { get; }
    }
}
