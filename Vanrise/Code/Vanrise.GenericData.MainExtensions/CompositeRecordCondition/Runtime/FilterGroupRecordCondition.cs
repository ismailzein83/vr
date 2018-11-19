using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.CompositeRecordCondition.Runtime
{
    public class FilterGroupRecordCondition : Entities.CompositeRecordCondition
    {
        public RecordFilterGroup FilterGroup { get; set; }

        public string RecordName { get; set; }

        public override bool Evaluate(ICompositeRecordConditionEvaluateContext context)
        {
            if (string.IsNullOrEmpty(this.RecordName))
                throw new NullReferenceException("RecordName");

            CompositeRecordConditionFields compositeRecordConditionFields = context.CompositeRecordConditionFieldsByRecordName.GetRecord(RecordName);

            return new RecordFilterManager().IsFilterGroupMatch(this.FilterGroup, new DataRecordDictFilterGenericFieldMatchContext(compositeRecordConditionFields.FieldValues, compositeRecordConditionFields.DataRecordFields));
        }
    }
}