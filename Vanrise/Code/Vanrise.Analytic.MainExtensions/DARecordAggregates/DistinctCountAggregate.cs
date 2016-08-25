using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.MainExtensions.DARecordAggregates
{
    public class DistinctCountAggregate : DARecordAggregate
    {
        public override DataRecordFieldType FieldType
        {
            get
            {
                return new GenericData.MainExtensions.DataRecordFields.FieldNumberType { DataType = GenericData.MainExtensions.DataRecordFields.FieldNumberDataType.BigInt };
            }
            set
            {
                base.FieldType = value;
            }
        }

        public string CountFieldName { get; set; }

        public override DARecordAggregateState CreateState(IDARecordAggregateCreateStateContext context)
        {
            return new DistinctCountAggregateState() { DistinctItems = new HashSet<object>() };
        }

        public override void Evaluate(IDARecordAggregateEvaluationContext context)
        {
            if (context.Record == null)
                throw new ArgumentNullException("context.Record");
            var fieldValue = Vanrise.Common.Utilities.GetPropValueReader(this.CountFieldName).GetPropertyValue(context.Record);
            if (fieldValue != null)
                (context.State as DistinctCountAggregateState).DistinctItems.Add(fieldValue);
        }

        public override dynamic GetResult(IDARecordAggregateGetResultContext context)
        {
            return (context.State as DistinctCountAggregateState).DistinctItems.Count;
        }

        public override void UpdateExistingFromNew(IDARecordAggregateUpdateExistingFromNewContext context)
        {
            DistinctCountAggregateState existingDistinctCountAggregateState = context.ExistingState as DistinctCountAggregateState;
            DistinctCountAggregateState newDistinctCountAggregateState = context.NewState as DistinctCountAggregateState;
            foreach(var itm in newDistinctCountAggregateState.DistinctItems)
            {
                existingDistinctCountAggregateState.DistinctItems.Add(itm);
            }
        }
    }

    public class DistinctCountAggregateState : DARecordAggregateState
    {
        public HashSet<Object> DistinctItems { get; set; }
    }
}
