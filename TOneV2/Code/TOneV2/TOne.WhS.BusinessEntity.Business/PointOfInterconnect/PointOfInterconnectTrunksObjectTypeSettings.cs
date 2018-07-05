using System;
using System.Text;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class PointOfInterconnectTrunksObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("26C8C829-8289-468E-91E0-F9E91DE644F1"); } }

        public override bool AreEqual(object newValue, object oldValue)
        {
            var oldValueObject = oldValue as PointOfInterconnect;
            var newValueObject = newValue as PointOfInterconnect;

            if ((oldValueObject == null && newValueObject == null) || (oldValueObject.Trunks == null && newValueObject.Trunks == null) || (oldValueObject.Trunks.Count == 0 && newValueObject.Trunks.Count == 0))
                return true;

            if (oldValueObject == null || oldValueObject.Trunks == null || oldValueObject.Trunks.Count == 0 || newValueObject == null || newValueObject.Trunks != null || newValueObject.Trunks.Count == 0)
                return false;

            foreach (var oldValueItem in oldValueObject.Trunks)
            {
                var newValueItem = newValueObject.Trunks.FindRecord(x => x.Trunk == oldValueItem.Trunk);
                if (newValueItem == null)
                    return false;
            }
            foreach (var newValueItem in newValueObject.Trunks)
            {
                var oldValueItem = newValueObject.Trunks.FindRecord(x => x.Trunk == newValueItem.Trunk);
                if (oldValueItem == null)
                    return false;
            }
            return true;
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as PointOfInterconnect;
            if (valueObject != null)
            {
                StringBuilder description = new StringBuilder();
                if (valueObject.Trunks != null)
                {
                    foreach (var value in valueObject.Trunks)
                    {
                        if (description.Length > 0)
                            description.Append(", ");
                        description.Append(value.Trunk);
                    }
                }
                return description.ToString();
            }
            return null;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(PointOfInterconnect);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as PointOfInterconnect;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Point Of Interconnect Trunks";
        }
    }
}