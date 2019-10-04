using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.NumberLength
{
    public class InternationalNumberLengthEvaluator : NumberLengthEvaluator
    {
        public override Guid ConfigID => throw new NotImplementedException();

        public Guid InternationalNumberLengthDefinitionID { get; set; } 

        public override void Evaluate(INumberLengthEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}