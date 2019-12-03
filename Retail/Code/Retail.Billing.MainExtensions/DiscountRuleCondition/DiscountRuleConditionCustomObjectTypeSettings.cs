using System;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.MainExtensions.DiscountRuleCondition
{
    public class DiscountRuleConditionCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("866A226E-E75D-412B-AC0A-2F8AA89AB63E"); } }

        public override string SelectorUIControl { get { return "retail-billing-discountrulecondition-customobject-runtime"; } }

        public override bool AreEqual(object newValue, object oldValue)
        {
            var newDiscountRuleCondition = newValue as Entities.DiscountRuleCondition;
            var oldDiscountRuleCondition = oldValue as Entities.DiscountRuleCondition;

            if (newDiscountRuleCondition == null && oldDiscountRuleCondition == null)
                return true;

            if (newDiscountRuleCondition == null || oldDiscountRuleCondition == null)
                return false;

            return newDiscountRuleCondition.AreEqual(oldDiscountRuleCondition);
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var discountRuleCondition = context.FieldValue as Retail.Billing.Entities.DiscountRuleCondition;
            if (discountRuleCondition == null)
                return null;

            return discountRuleCondition.GetDescription();
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(Retail.Billing.Entities.DiscountRuleCondition);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as Retail.Billing.Entities.DiscountRuleCondition;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Discount Rule Condition Custom Object";
        }
    }
}