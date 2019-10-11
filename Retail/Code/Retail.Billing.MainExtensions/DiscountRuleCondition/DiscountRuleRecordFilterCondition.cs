using Retail.Billing.Entities;
using System;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.MainExtensions.DiscountRuleCondition
{
    public class DiscountRuleRecordFilterCondition : Retail.Billing.Entities.DiscountRuleCondition
    {
        public override Guid ConfigID => throw new NotImplementedException();

        public RecordFilterGroup RecordFilterGroup { get; set; }

        public override bool IsMatched(IDiscountRuleConditionIsMatchedContext context)
        {
            throw new NotImplementedException();
        }
    }
}