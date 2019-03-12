using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BaseBPTaskTypeSettingsCustomObject : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("9C73540F-97E8-4F7A-BD02-594F6DD29741"); } }

        public override bool AreEqual(object newValue, object oldValue)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            return null;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(BaseBPTaskTypeSettings);
        }

        public override string GetRuntimeTypeDescription()
        {
            throw new NotImplementedException();
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as BaseBPTaskTypeSettings;
            if (castedOriginalValue != null)
            {
                return castedOriginalValue;
            }
            else
                return Serializer.Deserialize<BaseBPTaskTypeSettings>(originalValue.ToString());
        }
    }
}
