﻿using System;
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
