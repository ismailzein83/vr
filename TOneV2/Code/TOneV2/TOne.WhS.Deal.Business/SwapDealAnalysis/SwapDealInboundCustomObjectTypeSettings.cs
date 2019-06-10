using System;
using System.Text;
using Vanrise.Common;
using TOne.WhS.Deal.Entities;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealInboundCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId => new Guid("E30E4332-DB9F-414D-B58A-8F06E07D0D41");

        public override bool AreEqual(object newValue, object oldValue)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as List<SwapDealAnalysisInbound>;
            if (valueObject == null)
                valueObject = Utilities.ConvertJsonToList<SwapDealAnalysisInbound>(context.FieldValue);
            if (valueObject != null)
            {
                StringBuilder description = new StringBuilder();
                return description.ToString();
            }
            return null;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(SwapDealAnalysisInboundCollection);
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Swap Deal Analysis Inbound";
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            var castedOriginalValue = originalValue as List<SwapDealAnalysisInbound>;
            if (castedOriginalValue != null)
                return castedOriginalValue;
            else
                return Utilities.ConvertJsonToList<SwapDealAnalysisInbound>(originalValue);
        }
    }
}
