using System;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.CompositeRecordCondition.Runtime
{
    public class FieldComparisonCondition : Entities.CompositeRecordCondition
    {
        public override Guid ConfigId { get { return new Guid("0D3624F1-6F7D-46D4-8A00-0D9F5DD67CAA"); } }

        public override bool Evaluate(ICompositeRecordConditionEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}
