//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using Vanrise.Common;

//namespace Vanrise.GenericData.Entities
//{
//    public enum CompositeRecordConditionLogicalOperator
//    {
//        [Description("AND")]
//        And = 0,
//        [Description("OR")]
//        Or = 1
//    }
//    public abstract class CompositeRecordCondition
//    {
//        public abstract bool Evaluate(ICompositeRecordConditionEvaluateContext context);
//    }

//    public interface ICompositeRecordConditionEvaluateContext
//    {
//        Dictionary<string, dynamic> Records { get; }
//    }

//    public class FilterGroupRecordCondition : CompositeRecordCondition
//    {
//        public RecordFilterGroup FilterGroup { get; set; }

//        public string RecordName { get; set; }

//        public override bool Evaluate(ICompositeRecordConditionEvaluateContext context)
//        {
//            if (string.IsNullOrEmpty(this.RecordName))
//                throw new NullReferenceException("RecordName");

//            dynamic record = context.Records.GetRecord(RecordName);
//            throw new NotImplementedException();
//        }
//    }

//    public class CompositeRecordConditionGroup : CompositeRecordCondition
//    {
//        public List<CompositeRecordCondition> RecordConditions { get; set; }

//        public CompositeRecordConditionLogicalOperator LogicalOperator { get; set; }

//        public override bool Evaluate(ICompositeRecordConditionEvaluateContext context)
//        {
//            RecordConditions.ThrowIfNull("RecordConditions");
//            foreach (CompositeRecordCondition condition in RecordConditions)
//            {
//                bool result = condition.Evaluate(context);
//                switch (LogicalOperator)
//                {
//                    case CompositeRecordConditionLogicalOperator.And: if (!result) return false; break;
//                    case CompositeRecordConditionLogicalOperator.Or: if (result) return true; break;
//                    default: throw new NotSupportedException(string.Format("LogicalOperator {0} not supported.", LogicalOperator));
//                }
//            }

//            switch (LogicalOperator)
//            {
//                case CompositeRecordConditionLogicalOperator.And: return true;
//                case CompositeRecordConditionLogicalOperator.Or: return false;
//                default: throw new NotSupportedException(string.Format("LogicalOperator {0} not supported.", LogicalOperator));
//            }
//        }
//    }
//}
