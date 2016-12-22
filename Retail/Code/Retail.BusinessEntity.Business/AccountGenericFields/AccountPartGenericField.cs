using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountPartGenericField : AccountGenericField
    {
        AccountPartDefinition _partDefinition;
        Entities.GenericFieldDefinition _field;
        AccountManager _accountManager = new AccountManager();

        public AccountPartGenericField(AccountPartDefinition partDefinition, Entities.GenericFieldDefinition fieldDefinition)
        {
            _partDefinition = partDefinition;
            _field = fieldDefinition;
            _name = String.Format("Part_{0}_{1}", _partDefinition.AccountPartDefinitionId.ToString().Replace("-", ""), _field.Name);
            _title = String.Format("{0} ({1})", _field.Title, _partDefinition.Title);
        }

        string _name;
        public override string Name
        {
            get
            {
                return _name;
            }
        }

        string _title;
        public override string Title
        {
            get
            {
                return _title;
            }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get
            {
                return _field.FieldType;
            }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            AccountPart accountPart;
            if (_accountManager.TryGetAccountPart(context.Account, _partDefinition.AccountPartDefinitionId, true, out accountPart))
                return accountPart.Settings.GetFieldValue(new AccountPartGetFieldValueContext(_field.Name, _partDefinition));
            else
                return null;
        }
    }
}
