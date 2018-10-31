using System;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.DARecordAggregates
{
    public class MaxAggregate : DARecordAggregate
    {
        public override Guid ConfigId { get { return  new Guid("F50143C9-62B4-4A6F-8C94-B9EED7970DCF"); } }

        public string MaxFieldName { get; set; }

        public override DARecordAggregateState CreateState(IDARecordAggregateCreateStateContext context)
        {
            return new MaxAggregateState();
        }
        public override void Evaluate(IDARecordAggregateEvaluationContext context)
        {
            if (context.Record == null)
                //return;
                throw new ArgumentNullException("context.Record");
          //  var fieldValue = context.Record.GetFieldValue(MaxFieldName);
            var fieldValue = Common.Utilities.GetPropValueReader(this.MaxFieldName).GetPropertyValue(context.Record);
            (context.State as MaxAggregateState).Max = Math.Max(fieldValue, (context.State as MaxAggregateState).Max) ;
        }

        public override dynamic GetResult(IDARecordAggregateGetResultContext context)
        {
            return (context.State as MaxAggregateState).Max;
        }

        public override void UpdateExistingFromNew(IDARecordAggregateUpdateExistingFromNewContext context)
        {
            var ExistingMax = (context.ExistingState as MaxAggregateState).Max;
            var CurrentMax = (context.NewState as MaxAggregateState).Max;

            (context.ExistingState as MaxAggregateState).Max = Math.Max(ExistingMax, CurrentMax);
        } 
    }

    public class MaxAggregateState : DARecordAggregateState
    {
        public dynamic Max { get; set; }
    }
}
