using RecordAnalysis.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch
{
    public class C4SwitchSettingsCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("58091C7A-B48F-4683-BAA1-CC00D4518364"); } }

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
            return typeof(C4SwitchSettings);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as C4SwitchSettings;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "C4 Switch Settings";
        }
    }
}
