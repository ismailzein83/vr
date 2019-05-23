using System;

namespace TOne.WhS.Sales.Entities
{
    public abstract class RateAction
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(ISellingRuleActionContext context);
    }

    public interface ISellingRuleActionContext
    {
    }
}
