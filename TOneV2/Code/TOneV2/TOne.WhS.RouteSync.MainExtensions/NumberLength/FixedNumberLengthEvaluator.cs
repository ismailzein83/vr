using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.NumberLength 
{
    public class FixedNumberLengthEvaluator : NumberLengthEvaluator
    {
        public override Guid ConfigID => throw new NotImplementedException();

        public int MinCodeLength { get; set; }

        public int MaxCodeLength { get; set; }

        public override void Evaluate(INumberLengthEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}