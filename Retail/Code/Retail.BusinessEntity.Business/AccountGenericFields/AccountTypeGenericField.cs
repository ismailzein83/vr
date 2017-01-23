using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountTypeGenericField : AccountGenericField
    {
        Guid _accountBEDefinitionId;
        public AccountTypeGenericField(Guid accountBEDefinitionId)
        {
            _accountBEDefinitionId = accountBEDefinitionId;
        }

        public override string Name
        {
            get
            {
                return "AccountType";
            }
        }

        public override string Title
        {
            get
            {
                return "AccountType";
            }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get
            {
                return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType()
                {
                    BusinessEntityDefinitionId = AccountType.BUSINESSENTITY_DEFINITION_ID,
                    SelectorFilter = new AccountTypeBESelectorFilter() { AccountBEDefinitionId = _accountBEDefinitionId }
                };
            }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            return context.Account.TypeId;
        }
    }
}
