using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountSourceIdGenericField : AccountGenericField
    {
        public override string Name
        {
            get
            {
                return "SourceId";
            }
        }

        public override string Title
        {
            get
            {
                return "Source Id";
            }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get
            {
                return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType();
            }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            return context.Account.SourceId;
        }
    }
}
