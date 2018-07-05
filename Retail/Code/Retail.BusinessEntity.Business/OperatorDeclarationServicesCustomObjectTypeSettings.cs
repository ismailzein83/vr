using Retail.BusinessEntity.Entities;
using System;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class OperatorDeclarationServicesCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("DEDB89C0-370F-4DF8-BE63-EE60C73436F6"); } }

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
            return typeof(OperatorDeclarationServices);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as OperatorDeclarationServices;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Operator Declaration Services";
        }
    }
}