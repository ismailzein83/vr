using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.CodeCharge
{
    public class FixedCodeChargeEvaluator : CodeChargeEvaluator
    {
        public override Guid ConfigID { get { return new Guid("E03E24AE-1985-455B-B843-1ED6A52DA9CE"); } }

        public string CodeCharge { get; set; }

        public override void Evaluate(ICodeChargeEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}