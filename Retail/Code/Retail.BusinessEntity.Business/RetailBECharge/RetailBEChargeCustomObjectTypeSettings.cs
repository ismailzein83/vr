using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class RetailBEChargeCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("0C984560-77EE-4BE7-8EFB-A0FFBE54FCBE"); } }

        public override bool AreEqual(object newValue, object oldValue)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            throw new NotImplementedException();
        }

        public override Type GetNonNullableRuntimeType()
        {
            throw new NotImplementedException();
        }

        public override string GetRuntimeTypeDescription()
        {
            throw new NotImplementedException();
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            throw new NotImplementedException();
        }
    }
}
