using System;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.DARecordAggregates
{
    public class MaxAggregate : DARecordAggregate
    {
        public override Guid ConfigId { get { return new Guid("F50143C9-62B4-4A6F-8C94-B9EED7970DCF"); } }

        public string MaxFieldName { get; set; }

        public override DARecordAggregateState CreateState(IDARecordAggregateCreateStateContext context)
        {
            return new MaxAggregateState();
        }
        public override void Evaluate(IDARecordAggregateEvaluationContext context)
        {
            if (context.Record == null)
                throw new ArgumentNullException("context.Record");

            var fieldValue = context.Record.GetFieldValue(MaxFieldName);
            var maxAggregateState = context.State as MaxAggregateState;

            if (fieldValue == null)
                return;

            if (maxAggregateState.Max == null)
            {
                maxAggregateState.Max = fieldValue;
                return;
            }

            maxAggregateState.Max = maxAggregateState.Max > fieldValue ? maxAggregateState.Max : fieldValue;
        }

        public override dynamic GetResult(IDARecordAggregateGetResultContext context)
        {
            return (context.State as MaxAggregateState).Max;
        }

        public override void UpdateExistingFromNew(IDARecordAggregateUpdateExistingFromNewContext context)
        {
            var existingMaxState = context.ExistingState as MaxAggregateState;
            var newMaxState = context.NewState as MaxAggregateState;

            existingMaxState.Max = existingMaxState.Max > newMaxState.Max ? existingMaxState.Max : newMaxState.Max;
        }
    }

    public class MaxAggregateState : DARecordAggregateState
    {
        public dynamic Max { get; set; }
    }
}