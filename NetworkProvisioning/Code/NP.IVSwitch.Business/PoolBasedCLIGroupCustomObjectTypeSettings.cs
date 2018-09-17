using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace NP.IVSwitch.Business
{
    public class PoolBasedCLIGroupCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("7CC89311-C9AA-4F9A-9BEB-4549362AEA4F"); } }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as List<PoolBasedCLIDetails>;
            if (valueObject == null)
                valueObject = Utilities.ConvertJsonToList<PoolBasedCLIDetails>(context.FieldValue);

            if (valueObject != null)
            {
                StringBuilder description = new StringBuilder();

                foreach (var value in valueObject)
                {
                    if (description.Length > 0)
                        description.Append(",");
                    description.Append(value.CLIPattern);
                }
                return description.ToString();
            }
            return null;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            return true;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(PoolBasedCLIDetailsCollection);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as List<PoolBasedCLIDetails>;
            if (castedOriginalValue != null)
                return castedOriginalValue;
            else
                return Utilities.ConvertJsonToList<PoolBasedCLIDetails>(originalValue);
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Pool-Based CLI Group";
        }
    }
}
