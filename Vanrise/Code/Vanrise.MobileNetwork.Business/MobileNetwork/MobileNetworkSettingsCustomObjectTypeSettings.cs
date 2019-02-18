using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.MobileNetwork.Entities;

namespace Vanrise.MobileNetwork.Business
{
    public class MobileNetworkSettingsCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("E4B2A13C-66DB-462C-973E-F06765056B1D"); } }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as MobileNetworkSettings;
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
            return typeof(MobileNetworkSettings);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as MobileNetworkSettings;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Mobile Network Settings";
        }
    }
}
