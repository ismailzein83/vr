using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Business
{
public class WhSJazzMarketSettingsCustomObject : FieldCustomObjectTypeSettings
    {

        public override Guid ConfigId { get { return new Guid("11946B35-B549-4988-BFD3-ABB9516F5520"); } }

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
            return typeof(MarketSettings);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
                return originalValue as MarketSettings;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Market Settings Custom Object";
        }
    }
}
