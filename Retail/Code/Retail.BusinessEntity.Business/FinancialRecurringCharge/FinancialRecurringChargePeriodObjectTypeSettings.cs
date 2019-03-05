using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class FinancialRecurringChargePeriodObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("77F773F8-FB42-4A4E-97FB-BCC807BA940F"); } }

        public override bool AreEqual(object newValue, object oldValue)
        {
            var oldValueObject = oldValue as FinancialRecurringChargePeriod;
            var newValueObject = newValue as FinancialRecurringChargePeriod;

            if (oldValueObject == null && newValueObject == null)
                return true;

            if (oldValueObject == null || newValueObject == null)
                return false;

            return true;
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as FinancialRecurringChargePeriod;
            if (valueObject != null)
            {

            }
            return null;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(FinancialRecurringChargePeriod);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as FinancialRecurringChargePeriod;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Recurring Charge Period";
        }
    }
}
