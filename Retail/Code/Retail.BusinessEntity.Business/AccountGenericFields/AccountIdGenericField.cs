using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountIdGenericField : AccountGenericField
    {
        public override string Name
        {
            get
            {
                return "ID";
            }
        }

        public override string Title
        {
            get
            {
                return "ID";
            }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get
            {
                return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType { DataType = Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberDataType.BigInt };
            }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            return context.Account.AccountId;
        }
    }
}
