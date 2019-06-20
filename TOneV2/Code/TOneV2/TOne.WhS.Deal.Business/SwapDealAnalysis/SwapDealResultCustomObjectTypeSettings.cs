using System;
using TOne.WhS.Deal.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealResultCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId => new Guid("81F94D80-11CD-41C9-9DF0-6373EBFBFC00");

        public override bool AreEqual(object newValue, object oldValue)
        {
            return true;
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as AnalysisResultCustomObject;
           
            return null;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(AnalysisResultCustomObject);
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Swap Deal Analysis Result";
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as AnalysisResultCustomObject;
        }
    }
}
