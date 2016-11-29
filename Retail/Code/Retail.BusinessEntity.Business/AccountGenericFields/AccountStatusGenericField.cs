using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountStatusGenericField : AccountGenericField
    {
        public override string Name
        {
            get
            {
                return "Status";
            }
        }

        public override string Title
        {
            get
            {
                return "Status";
            }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get
            {
                return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType { BusinessEntityDefinitionId = StatusDefinition.BUSINESSENTITY_DEFINITION_ID };
            }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            return context.Account.StatusId;
        }
    }
}
