using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class SecurityProviderCustomObjectTypeSettings : Vanrise.GenericData.Entities.FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId
        {

            get { return new Guid("17823DCD-AF7B-4DD0-A19F-945DE96B74CE"); }
        }

        public override string GetDescription(Vanrise.GenericData.Entities.IFieldCustomObjectTypeSettingsContext context)
        {
            return null;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            return true;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(SecurityProviderSettings);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as SecurityProviderSettings;
        }
    }
}
