using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Business
{
public class AccountCodeCustomObject : FieldCustomObjectTypeSettings
    {

        public override Guid ConfigId { get { return new Guid("A474BA09-750B-4CD7-B842-0BAA550FE108"); } }

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
            return typeof(WhsJazzAccountCodeCarriers);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
                return originalValue as WhsJazzAccountCodeCarriers;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Account Code Custom Object";
        }
    }
}