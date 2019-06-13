using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class RetailBEChargeCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("0C984560-77EE-4BE7-8EFB-A0FFBE54FCBE"); } }

        public override bool AreEqual(object newValue, object oldValue)
        {
            return true;
        }
        public override string  SelectorUIControl{ get { return "retail-be-charge-entitydirective"; } }
        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as RetailBEChargeEntity;
            if (valueObject == null || valueObject.Settings==null)
                return null;
            return valueObject.Settings.GetDescription();
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(RetailBEChargeEntity);
        }

        public override string GetRuntimeTypeDescription()
        {
            throw new NotImplementedException();
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as RetailBEChargeEntity;
            return castedOriginalValue;
        }
    }
}
