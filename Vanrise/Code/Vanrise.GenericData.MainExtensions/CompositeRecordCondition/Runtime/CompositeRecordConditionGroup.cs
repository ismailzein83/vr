using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.CompositeRecordCondition.Runtime
{
    public class CompositeRecordConditionGroup : Vanrise.GenericData.Entities.CompositeRecordCondition
    {
        public List<Vanrise.GenericData.Entities.CompositeRecordCondition> RecordConditions { get; set; }

        public CompositeRecordConditionLogicalOperator LogicalOperator { get; set; }

        public override bool Evaluate(ICompositeRecordConditionEvaluateContext context)
        {
            RecordConditions.ThrowIfNull("RecordConditions");
            foreach (Vanrise.GenericData.Entities.CompositeRecordCondition condition in RecordConditions)
            {
                bool result = condition.Evaluate(context);
                switch (LogicalOperator)
                {
                    case CompositeRecordConditionLogicalOperator.And: if (!result) return false; break;
                    case CompositeRecordConditionLogicalOperator.Or: if (result) return true; break;
                    default: throw new NotSupportedException(string.Format("LogicalOperator {0} not supported.", LogicalOperator));
                }
            }

            switch (LogicalOperator)
            {
                case CompositeRecordConditionLogicalOperator.And: return true;
                case CompositeRecordConditionLogicalOperator.Or: return false;
                default: throw new NotSupportedException(string.Format("LogicalOperator {0} not supported.", LogicalOperator));
            }
        }
    }
}