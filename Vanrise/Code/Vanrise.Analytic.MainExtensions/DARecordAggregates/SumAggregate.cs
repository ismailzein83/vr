﻿using System;
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
        public override Guid ConfigId { get { return new Guid("DC962A83-2FDA-456F-9940-15E9BE787D89"); } }
        public override DataRecordFieldType FieldType
        {
            get
            {
                return new GenericData.MainExtensions.DataRecordFields.FieldNumberType
                {
                    DataType = GenericData.MainExtensions.DataRecordFields.FieldNumberDataType.Decimal
                };
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

            dynamic sumField = Vanrise.Common.Utilities.GetPropValueReader(this.SumFieldName).GetPropertyValue(context.Record);
            (context.State as SumAggregateState).Sum += sumField != null ? sumField : 0;
        }

        public override dynamic GetResult(IDARecordAggregateGetResultContext context)
        {
            return (context.State as SumAggregateState).Sum;
        }

        public override void UpdateExistingFromNew(IDARecordAggregateUpdateExistingFromNewContext context)
        {
            (context.ExistingState as SumAggregateState).Sum += (context.NewState as SumAggregateState).Sum;
        }
    }

    public class SumAggregateState : DARecordAggregateState
    {
        public Decimal Sum { get; set; }
    }
}
