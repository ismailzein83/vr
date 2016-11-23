using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.Entities
{
    public abstract class DealerCondition
    {
        public abstract bool Evaluate(IDealerConditionEvaluationContext context);
    }

    public interface IDealerConditionEvaluationContext
    {
        Dealer Dealer { get; }
    }
}
