using System;
using Retail.RA.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class MNCSettingsCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("E4B2A13C-66DB-462C-973E-F06765056B1D"); } }

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
            return typeof(MNCSettings);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as MNCSettings;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "MNC Settings";
        }
    }
}