using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;


namespace Vanrise.Common.Business
{
    public class VRTileCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("46B078B8-3A93-47A1-AC4A-AC1C97E76526"); } }
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
            return typeof(VRDashboardSettings);
        }
        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as VRDashboardSettings;
            if (castedOriginalValue != null)
            {
                return castedOriginalValue;
            }
            else
                return Serializer.Deserialize<VRDashboardSettings>(originalValue.ToString());
        }
        public override string GetRuntimeTypeDescription()
        {
            return "VRTileReport";
        }
    }
}
