using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.MainExtensions.DARecordAggregates
{
    public class SumAggregate : DARecordAggregate
    {
        public override DataRecordFieldType FieldType
        {
            get
            {
                return new GenericData.MainExtensions.DataRecordFields.FieldNumberType { DataType = GenericData.MainExtensions.DataRecordFields.FieldNumberDataType.Decimal };
            }
            set
            {
                base.FieldType = value;
            }
        }

        public string SumFieldName { get; set; }
        
        public override DARecordAggregateState CreateState(IDARecordAggregateCreateStateContext context)
        {
            return new SumAggregateState();
        }

        public override void Evaluate(IDARecordAggregateEvaluationContext context)
        {
            if (context.Record == null)
                throw new ArgumentNullException("context.Record");
            (context.State as SumAggregateState).Sum += Vanrise.Common.Utilities.GetPropValueReader(this.SumFieldName).GetPropertyValue(context.Record);
        }

        public override dynamic GetResult(IDARecordAggregateGetResultContext context)
        {
            return (context.State as SumAggregateState).Sum;
        }
    }

    public class SumAggregateState : DARecordAggregateState
    {
        public Decimal Sum { get; set; }
    }
}
