using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.ChargeCode
{
    public class FixedCodeChargeEvaluator : CodeChargeEvaluator
    {
        public override Guid ConfigID => throw new NotImplementedException();

        public string CodeCharge { get; set; }

        public override void Evaluate(ICodeChargeEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}