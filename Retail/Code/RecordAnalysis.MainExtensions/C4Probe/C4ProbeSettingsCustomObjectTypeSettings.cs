using RecordAnalysis.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace RecordAnalysis.MainExtensions.C4Probe
{
    public class C4ProbeSettingsCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("EC22C4F8-BF0C-4BD0-BE0A-34BF4210DE3C"); } }

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
            return typeof(Entities.C4Probe);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as Entities.C4Probe;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "C4Probe";
        }
    }
}
