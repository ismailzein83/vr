using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.MainExtensions.DARecordAggregates
{
    public class CountAggregate : DARecordAggregate
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("DAD39EDB-65B1-4C40-935C-7E6339267055"); } }
        public override DataRecordFieldType FieldType
        {
            get
            {
                return new GenericData.MainExtensions.DataRecordFields.FieldNumberType 
                { 
                    ConfigId = 2,
                    DataType = GenericData.MainExtensions.DataRecordFields.FieldNumberDataType.BigInt 
                };
            }
            set
            {
                base.FieldType = value;
            }
        }

        public override DARecordAggregateState CreateState(IDARecordAggregateCreateStateContext context)
        {
            return new CountAggregateState();
        }

        public override void Evaluate(IDARecordAggregateEvaluationContext context)
        {
            (context.State as CountAggregateState).Count++;
        }

        public override dynamic GetResult(IDARecordAggregateGetResultContext context)
        {
            return (context.State as CountAggregateState).Count;
        }

        public override void UpdateExistingFromNew(IDARecordAggregateUpdateExistingFromNewContext context)
        {
            (context.ExistingState as CountAggregateState).Count += (context.NewState as CountAggregateState).Count;
        }
    }

    public class CountAggregateState : DARecordAggregateState
    {
        public long Count { get; set; }
    }
}
