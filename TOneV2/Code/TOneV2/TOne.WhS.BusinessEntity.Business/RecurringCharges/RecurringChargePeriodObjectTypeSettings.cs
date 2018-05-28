using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RecurringChargePeriodObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override bool AreEqual(object newValue, object oldValue)
        {
            var oldValueObject = oldValue as RecurringChargePeriod;
            var newValueObject = newValue as RecurringChargePeriod;

            if (oldValueObject == null && newValueObject == null)
                return true;

            if (oldValueObject == null || newValueObject == null)
                return false;

            return true;
        }

        public override Guid ConfigId
        {
            get { return new Guid("77F773F8-FB42-4A4E-97FB-BCC807BA940F"); }
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as RecurringChargePeriod;
            if (valueObject != null)
            {
               
            }
            return null;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(RecurringChargePeriod);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as RecurringChargePeriod;
        }
    }
}
