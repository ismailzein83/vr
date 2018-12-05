using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Retail.RA.Entities
{
    public class SpecialNumberCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("320B61F3-FF99-4E54-B6CE-9EF5DA6128F2"); } }
        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            return null;
        }
        public override bool AreEqual(Object newValue, Object oldValue)
        {

            return true;
        }
        public override Type GetNonNullableRuntimeType()
        {
            return typeof(SpecialNumbersSetting);
        }
        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as SpecialNumbersSetting;
            if (castedOriginalValue != null)
            {
                return castedOriginalValue;
            }
            else
                return Serializer.Deserialize<SpecialNumbersSetting>(originalValue.ToString());
        }
        public override string GetRuntimeTypeDescription()
        {
            return "Special Numbers";
        }
    }
}
