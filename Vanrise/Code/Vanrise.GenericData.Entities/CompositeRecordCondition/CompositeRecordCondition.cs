using System;
using System.Collections.Generic;
using System.ComponentModel;
using Vanrise.Common;

namespace Vanrise.GenericData.Entities
{
    public enum CompositeRecordConditionLogicalOperator
    {
        [Description("AND")]
        And = 0,
        [Description("OR")]
        Or = 1
    }
    public abstract class CompositeRecordCondition
    {
        public abstract bool Evaluate(ICompositeRecordConditionEvaluateContext context);
    }

    public interface ICompositeRecordConditionEvaluateContext
    {
        Dictionary<string, CompositeRecordConditionFields> CompositeRecordConditionFieldsByRecordName { get; }
    }

    public class CompositeRecordConditionEvaluateContext : ICompositeRecordConditionEvaluateContext
    {
        public Dictionary<string, CompositeRecordConditionFields> CompositeRecordConditionFieldsByRecordName { get; set; }
    }

    public class CompositeRecordConditionFields
    {
        public Dictionary<string, dynamic> FieldValues { get; set; }

        public Dictionary<string, DataRecordField> DataRecordFields { get; set; }
    }
}
