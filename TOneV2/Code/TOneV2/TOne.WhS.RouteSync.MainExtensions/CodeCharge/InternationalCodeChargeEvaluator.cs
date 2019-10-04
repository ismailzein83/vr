using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.ChargeCode
{
    public class InternationalCodeChargeEvaluator : CodeChargeEvaluator
    {
        public override Guid ConfigID => throw new NotImplementedException();

        public Guid InternationalCodeChargeDefinitionID { get; set; } 

        public override void Evaluate(ICodeChargeEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}