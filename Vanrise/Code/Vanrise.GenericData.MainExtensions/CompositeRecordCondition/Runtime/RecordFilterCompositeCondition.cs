using System;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.CompositeRecordCondition.Runtime
{
    public class RecordFilterCompositeCondition : Entities.CompositeRecordCondition 
    {
        public override Guid ConfigId { get { return new Guid("F16BBC6F-F471-4601-B5C9-7C95B88B3ECB"); } }

        public string RecordName { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public override bool Evaluate(ICompositeRecordConditionEvaluateContext context)
        {
            if (string.IsNullOrEmpty(this.RecordName))
                throw new NullReferenceException("RecordName");

            CompositeRecordConditionFields compositeRecordConditionFields = context.CompositeRecordConditionFieldsByRecordName.GetRecord(RecordName);

            return new RecordFilterManager().IsFilterGroupMatch(this.FilterGroup, new DataRecordDictFilterGenericFieldMatchContext(compositeRecordConditionFields.FieldValues, compositeRecordConditionFields.DataRecordFields));
        }
    }
}