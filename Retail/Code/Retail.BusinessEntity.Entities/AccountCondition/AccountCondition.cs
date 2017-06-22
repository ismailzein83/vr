using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public enum TargetType { Self = 0, Parent = 1 }
    public abstract class AccountCondition
    {
        public abstract Guid ConfigId { get; }
        public TargetType TargetType { get; set; }
        public abstract bool Evaluate(IAccountConditionEvaluationContext context);
    }

    public interface IAccountConditionEvaluationContext
    {
        Account Account { get; }
    }
}
