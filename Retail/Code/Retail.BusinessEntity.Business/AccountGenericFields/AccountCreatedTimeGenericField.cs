using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountCreatedTimeGenericField : AccountGenericField
    {
        public override string Name
        {
            get
            {
                return "CreatedTime";
            }
        }

        public override string Title
        {
            get
            {
                return "Created Time";
            }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get
            {
                return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType() { DataType = Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeDataType.DateTime };
            }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            return context.Account.CreatedTime;
        }
    }
}
