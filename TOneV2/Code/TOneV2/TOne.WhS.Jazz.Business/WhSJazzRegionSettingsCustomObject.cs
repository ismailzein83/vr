using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Business
{
public class WhSJazzRegionSettingsCustomObject : FieldCustomObjectTypeSettings
    {

        public override Guid ConfigId { get { return new Guid("5C246DF4-7263-4710-9A1E-6EEAD04213CB"); } }

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
            return typeof(RegionSettings);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
                return originalValue as RegionSettings;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Region Settings Custom Object";
        }
    }
}
