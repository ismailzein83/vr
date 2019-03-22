using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.MobileNetwork.Entities;

namespace Vanrise.MobileNetwork.Business
{
    public class MobileCountrySettingsCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("ACEF9EF9-7F07-4967-8BF9-7CBA23307306"); } }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as MobileCountrySettings;
            List<string> codes = new List<string>();
            if (valueObject != null && valueObject.Codes!= null && valueObject.Codes.Count > 0 )
            {
                foreach(var code in valueObject.Codes)
                {
                    if (code != null)
                        codes.Add(code.Code);
                }
                return string.Join(",", codes);
            }
                return null;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            return true;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(MobileCountrySettings);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as MobileCountrySettings;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Mobile Country Settings";
        }
    }
}
