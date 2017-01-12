using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountStatusGenericField : AccountGenericField
    {
        public Guid _statusBEDefinitionId;

        public AccountStatusGenericField(Guid accountBEDefinitionId)
        {
            var accountBEDefinitionSettings = new AccountBEDefinitionManager().GetAccountBEDefinitionSettings(accountBEDefinitionId);
            _statusBEDefinitionId = accountBEDefinitionSettings.StatusBEDefinitionId;
        }

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
                return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType { BusinessEntityDefinitionId = _statusBEDefinitionId };
            }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            return context.Account.StatusId;
        }
    }
}
