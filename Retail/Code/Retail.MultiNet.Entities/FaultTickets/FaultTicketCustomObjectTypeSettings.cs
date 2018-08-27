using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Retail.MultiNet.Entities;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    class FaultTicketCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {

        public override Guid ConfigId { get { return new Guid("AB921E5F-3CCF-47E2-A606-DBD12E55E026"); } }

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
            return typeof(FaultTicketSettingsDetailsCollection);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as List<FaultTicketDescriptionSettingDetails>;
            if (castedOriginalValue != null)
                return castedOriginalValue;
            else
                return Utilities.ConvertJsonToList<FaultTicketDescriptionSettingDetails>(originalValue);
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Fault Ticket";
        }
    }

}
