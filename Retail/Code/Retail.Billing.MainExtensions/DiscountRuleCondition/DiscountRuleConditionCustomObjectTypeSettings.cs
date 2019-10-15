using System;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.MainExtensions.DiscountRuleCondition
{
    public class DiscountRuleConditionCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId => new Guid("866A226E-E75D-412B-AC0A-2F8AA89AB63E");

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