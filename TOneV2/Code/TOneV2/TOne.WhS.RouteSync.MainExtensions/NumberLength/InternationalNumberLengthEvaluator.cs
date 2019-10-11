using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.NumberLength
{
    public class InternationalNumberLengthEvaluator : NumberLengthEvaluator
    {
        public override Guid ConfigID { get { return new Guid("FAE24FC0-F549-41F7-8D3C-A581B63F7AFB"); } }

        public override void Evaluate(INumberLengthEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}