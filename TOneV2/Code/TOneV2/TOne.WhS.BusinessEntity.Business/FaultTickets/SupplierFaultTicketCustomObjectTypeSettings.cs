using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierFaultTicketCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("AA4C29F5-0B6A-4F38-8B2E-29A78D3961FE"); }
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = Utilities.ConvertJsonToList<SupplierFaultTicketDescriptionSetting>(context.FieldValue);
            if (valueObject != null)
            {
                StringBuilder description = new StringBuilder();
                GenericBusinessEntityManager genericBuinessEntityManager = new GenericBusinessEntityManager();
                Guid reasonDefinitionId = new Guid("b8daa0fa-d381-4bb0-b772-2e6b24d199e4");
                Guid internationalReleaseCodeDefinitionId = new Guid("6e7c2b68-3e8e-49a1-8922-796b2ce9cc1c");

                foreach (var value in valueObject)
                {
                    if (description.Length > 0)
                        description.Append(Utilities.NewLine());
                    description.Append(value.CodeNumber);
                    description.AppendFormat(" {0}", genericBuinessEntityManager.GetGenericBusinessEntityName(value.ReasonId, reasonDefinitionId));

                    if (value.InternationalReleaseCodeId.HasValue)
                    {
                        description.AppendFormat(" {0}", genericBuinessEntityManager.GetGenericBusinessEntityName(value.InternationalReleaseCodeId.Value, internationalReleaseCodeDefinitionId));
                    }
                }
                return description.ToString();
            }
            return null;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            var oldValueObject = Utilities.ConvertJsonToList<SupplierFaultTicketDescriptionSetting>(oldValue);
            var newValueObject = Utilities.ConvertJsonToList<SupplierFaultTicketDescriptionSetting>(newValue);

            if ((oldValueObject == null && newValueObject == null) || (oldValueObject.Count == 0 && newValueObject.Count == 0))
                return true;
            if (oldValueObject == null || oldValueObject.Count == 0 || newValueObject == null || newValueObject.Count == 0)
                return false;

            foreach(var oldValueItem in oldValueObject)
            {
                var newValueItem = newValueObject.FindRecord(x => x.CodeNumber == oldValueItem.CodeNumber && x.ReasonId == oldValueItem.ReasonId && oldValueItem.InternationalReleaseCodeId == x.InternationalReleaseCodeId);
                if (newValueItem == null)
                    return false;
            }
            foreach (var newValueItem in newValueObject)
            {
                var oldValueItem = newValueObject.FindRecord(x => x.CodeNumber == newValueItem.CodeNumber && x.ReasonId == newValueItem.ReasonId && newValueItem.InternationalReleaseCodeId == x.InternationalReleaseCodeId);
                if (oldValueItem == null)
                    return false;
            }
            return true;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(List<SupplierFaultTicketDescriptionSetting>);
        }
        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as List<SupplierFaultTicketDescriptionSetting>;
            if (castedOriginalValue != null)
                return castedOriginalValue;
            else
                return Utilities.ConvertJsonToList<SupplierFaultTicketDescriptionSetting>(originalValue);
        }
    }
}
