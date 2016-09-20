using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.Sales.Entities
{
    public enum MarginStatus { Valid = 0, Invalid = 1 }
    public abstract class SellingRuleSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract void Execute(ISellingRuleExecutionContext context);
    }
}
