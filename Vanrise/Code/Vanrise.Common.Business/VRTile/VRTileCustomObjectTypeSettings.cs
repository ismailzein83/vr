using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;


namespace Vanrise.Entities
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
            return typeof(VRTileReportSettings);
        }
        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as VRTileReportSettings;
            if (castedOriginalValue != null)
            {
                return castedOriginalValue;
            }
            else
                return Serializer.Deserialize<VRTileReportSettings>(originalValue.ToString());
        }
        public override string GetRuntimeTypeDescription()
        {
            return "VRTileReport";
        }
    }
}
