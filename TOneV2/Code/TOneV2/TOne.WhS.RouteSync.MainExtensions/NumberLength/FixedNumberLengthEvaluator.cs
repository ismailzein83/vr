using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.NumberLength
{
    public class FixedNumberLengthEvaluator : NumberLengthEvaluator
    {
        public override Guid ConfigID { get { return new Guid("E9B480FE-BE8F-4F1F-8D00-A3DF8EE74F7F");  } }

        public int MinCodeLength { get; set; }

        public int MaxCodeLength { get; set; }

        public override void Evaluate(INumberLengthEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}