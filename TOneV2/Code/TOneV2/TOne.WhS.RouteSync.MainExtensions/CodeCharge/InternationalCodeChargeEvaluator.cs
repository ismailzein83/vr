using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.CodeCharge
{
    public class InternationalCodeChargeEvaluator : CodeChargeEvaluator
    {
        public override Guid ConfigID { get { return new Guid("07ECEA2E-EEDD-4C65-B5D4-CC23BF680654"); } }

        public Guid InternationalCodeChargeDefinitionID { get; set; } 

        public override void Evaluate(ICodeChargeEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}