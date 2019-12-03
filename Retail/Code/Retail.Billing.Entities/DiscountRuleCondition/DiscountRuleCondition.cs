using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.Entities
{
    public abstract class DiscountRuleCondition
    {
        public abstract Guid ConfigID { get; }

        public abstract bool IsMatched(IDiscountRuleConditionIsMatchedContext context);

        public abstract string GetDescription();

        public virtual bool AreEqual(object comparedDiscountRuleCondition) 
        {
            return true;
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