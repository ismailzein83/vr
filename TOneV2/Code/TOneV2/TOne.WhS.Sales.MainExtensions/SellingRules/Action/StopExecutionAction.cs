using System;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    class StopExecutionAction : RateAction
    {
        public override Guid ConfigId { get; }
        public override void Execute(ISellingRuleActionContext context)
        {

        }
    }
}
