using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class InternationalCallsBlockedGenericField : AccountGenericField
    {
        public override string Name
        {
            get { return "InternationalCallsBlocked"; }
        }

        public override string Title
        {
            get { return "International Calls Blocked"; }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get { return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBooleanType(); }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            var changeUsersRGsAccountState = accountBEManager.GetExtendedSettings<ChangeUsersRGsAccountState>(context.Account);
            if (changeUsersRGsAccountState != null)
            {
                return true;
            }
            return false;
        }
    }
}
