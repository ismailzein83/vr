using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountServiceAccountGenericField : AccountServiceGenericField
    {
        AccountGenericField _accountField;
        public AccountServiceAccountGenericField(AccountGenericField accountField)
        {
            if (accountField == null)
                throw new ArgumentNullException("accountField");
            _accountField = accountField;
            _name = String.Format("AccountField_{0}", _accountField.Name);
            _title = String.Format("Account {0}", _accountField.Title);
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
                return _accountField.FieldType;
            }
        }

        public override dynamic GetValue(IAccountServiceGenericFieldContext context)
        {
            return _accountField.GetValue(new AccountGenericFieldContext(context.Account));
        }
    }
}
