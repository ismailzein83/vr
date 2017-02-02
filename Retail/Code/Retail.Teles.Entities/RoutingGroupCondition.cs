using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public abstract class RoutingGroupCondition
    {
        public abstract Guid ConfigId { get; }

        public abstract bool Evaluate(IRoutingGroupConditionContext context);
    }

    public interface IRoutingGroupConditionContext
    {
        string RoutingGroupName { get; }
    }
}
