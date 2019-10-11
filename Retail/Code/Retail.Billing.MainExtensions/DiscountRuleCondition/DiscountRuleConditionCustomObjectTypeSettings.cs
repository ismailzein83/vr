using System;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.MainExtensions.DiscountRuleCondition
{
    public class DiscountRuleConditionCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public override bool AreEqual(object newValue, object oldValue)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            throw new NotImplementedException();
        }

        public override Type GetNonNullableRuntimeType()
        {
            throw new NotImplementedException();
        }

        public override string GetRuntimeTypeDescription()
        {
            throw new NotImplementedException();
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            throw new NotImplementedException();
        }
    }
}