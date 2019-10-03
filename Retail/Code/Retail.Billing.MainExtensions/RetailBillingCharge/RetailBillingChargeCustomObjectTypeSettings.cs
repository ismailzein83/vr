using System;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.MainExtensions.RetailBillingCharge
{
    public class RetailBillingChargeCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("29cc92fd-9abb-4271-b1de-22181906d7f6"); } }

        public override string SelectorUIControl { get { return "retail-billing-charge-customobject"; } }

        public override bool AreEqual(object newValue, object oldValue)
        {
            return true;
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            return "Retail Billing Charge";
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(Retail.Billing.Entities.RetailBillingCharge);
        }

        public override string GetRuntimeTypeDescription()
        {
            throw new NotImplementedException();
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as Retail.Billing.Entities.RetailBillingCharge;
            return castedOriginalValue;
        }
    }
}