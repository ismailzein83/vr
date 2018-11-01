using System;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.DARecordAggregates
{
    public class MinAggregate : DARecordAggregate
    {
        public override Guid ConfigId { get { return new Guid("CE3AC24F-45C2-4965-B2AA-AB09DA9B97F8"); } }
        public string MinFieldName { get; set; }

        public override DARecordAggregateState CreateState(IDARecordAggregateCreateStateContext context)
        {
            return new MinAggregateState();
        }
        public override void Evaluate(IDARecordAggregateEvaluationContext context)
        {
            if (context.Record == null)
                throw new ArgumentNullException("context.Record");

            var fieldValue = context.Record.GetFieldValue(MinFieldName);
            var minAggregateState = context.State as MinAggregateState;

            if (fieldValue == null)
                return;

            if (minAggregateState.Min == null)
            {
                minAggregateState.Min = fieldValue;
                return;
            }

            minAggregateState.Min = minAggregateState.Min > fieldValue ? minAggregateState.Min : fieldValue;
        }

        public override dynamic GetResult(IDARecordAggregateGetResultContext context)
        {
            return (context.State as MinAggregateState).Min;
        }

        public override void UpdateExistingFromNew(IDARecordAggregateUpdateExistingFromNewContext context)
        {
            var existingMinState = context.ExistingState as MinAggregateState;
            var newMinState = context.NewState as MinAggregateState;

            existingMinState.Min = existingMinState.Min > newMinState.Min ? existingMinState.Min : newMinState.Min;
        }
    }

    public class MinAggregateState : DARecordAggregateState
    {
        public dynamic Min { get; set; }
    }
}