using RecordAnalysis.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace RecordAnalysis.MainExtensions.C4SwitchInterconnection
{
    public class C4SwitchInterconnectionSettingsCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("42BCEF1E-AF75-4091-BD27-09984ABC8598"); } }

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
            return typeof(Entities.C4SwitchInterconnection);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as Entities.C4SwitchInterconnection;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "SwitchInterconnection";
        }
    }
}
