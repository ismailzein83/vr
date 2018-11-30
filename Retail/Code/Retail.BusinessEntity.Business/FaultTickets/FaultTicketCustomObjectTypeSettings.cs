using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class FaultTicketCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {

        public override Guid ConfigId { get { return new Guid("AB921E5F-3CCF-47E2-A606-DBD12E55E026"); } }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as List<FaultTicketDescriptionSettingDetails>;
            if (valueObject == null)
                valueObject = Utilities.ConvertJsonToList<FaultTicketDescriptionSettingDetails>(context.FieldValue);
            if (valueObject != null)
            {
                StringBuilder description = new StringBuilder();

                foreach (var value in valueObject)
                {
                    if (description.Length > 0)
                        description.Append(", ");
                    description.Append(value.TicketReasonDescription);
                    description.Append(" ");
                    description.Append(value.Type);
                    if(value.Note!=null)
                    {
                        description.Append(" ");
                        description.Append(value.Note);
                    }
                }
                return description.ToString();
            }
            return null;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            var oldValueObject = oldValue as List<FaultTicketDescriptionSettingDetails>;
            var newValueObject = newValue as List<FaultTicketDescriptionSettingDetails>;

            if (oldValueObject == null)
                oldValueObject = Utilities.ConvertJsonToList<FaultTicketDescriptionSettingDetails>(oldValue);
            if (newValueObject == null)
                newValueObject = Utilities.ConvertJsonToList<FaultTicketDescriptionSettingDetails>(newValue);

            if ((oldValueObject == null && newValueObject == null) || (oldValueObject.Count == 0 && newValueObject.Count == 0))
                return true;
            if (oldValueObject == null || oldValueObject.Count == 0 || newValueObject == null || newValueObject.Count == 0)
                return false;

            foreach (var oldValueItem in oldValueObject)
            {
                var newValueItem = newValueObject.FindRecord(x => x.TicketReasonId == oldValueItem.TicketReasonId && x.TicketReasonDescription == oldValueItem.TicketReasonDescription && x.Type == oldValueItem.Type  && x.Note == oldValueItem.Note);
                if (newValueItem == null)
                    return false;
            }

            foreach (var newValueItem in newValueObject)
            {
                var oldValueItem = oldValueObject.FindRecord(x => x.TicketReasonId == newValueItem.TicketReasonId && x.TicketReasonDescription == newValueItem.TicketReasonDescription && x.Type == newValueItem.Type && x.Note == newValueItem.Note);
                if (oldValueItem == null)
                    return false;
            }

            return true;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(FaultTicketSettingsDetailsCollection);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as List<FaultTicketDescriptionSettingDetails>;
            if (castedOriginalValue != null)
            {
                return castedOriginalValue;
            }
            else if (originalValue is string && originalValue != null)
            {
                return Serializer.Deserialize<FaultTicketSettingsDetailsCollection>(originalValue.ToString());
            }
            else
                return Utilities.ConvertJsonToList<FaultTicketDescriptionSettingDetails>(originalValue);
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Fault Ticket";
        }
    }

}
