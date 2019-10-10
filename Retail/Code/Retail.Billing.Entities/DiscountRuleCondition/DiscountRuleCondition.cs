using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.Entities
{
    public abstract class DiscountRuleCondition
    {
        public abstract Guid ConfigID { get; }

        public abstract bool IsMatched(IDiscountRuleConditionIsMatchedContext context);
    }

    public class DiscountRuleRecordFilterCondition : DiscountRuleCondition
    {
        public override Guid ConfigID => throw new NotImplementedException();

        public RecordFilterGroup RecordFilterGroup { get; set; }

        public override bool IsMatched(IDiscountRuleConditionIsMatchedContext context)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDiscountRuleConditionIsMatchedContext
    {
        Dictionary<string, dynamic> FieldValues { get; }

        Dictionary<string, DataRecordField> DataRecordFields { get; }
    }

    public class DiscountRuleConditionIsMatchedContext : IDiscountRuleConditionIsMatchedContext
    {
        public Dictionary<string, dynamic> FieldValues { get; set; }

        public Dictionary<string, DataRecordField> DataRecordFields { get; set; }
    }
}