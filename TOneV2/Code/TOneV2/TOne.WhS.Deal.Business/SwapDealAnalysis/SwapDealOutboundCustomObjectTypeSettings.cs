using System;
using System.Text;
using TOne.WhS.Deal.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealOutboundCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId => new Guid("FFA8B9E5-66E5-4D63-888D-83B45D9137FF");

        public override bool AreEqual(object newValue, object oldValue)
        {
            return true;
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as SwapDealAnalysisOutbound;

            if (valueObject != null)
            {
                StringBuilder description = new StringBuilder();
                return description.ToString();
            }
            return null;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(SwapDealAnalysisOutbound);
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Swap Deal Analysis Outbound";
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as SwapDealAnalysisOutbound;
        }
    }
}
