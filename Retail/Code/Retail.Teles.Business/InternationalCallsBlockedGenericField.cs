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
        string _fieldName;
        string _fieldTitle;
        string _actionType;
        public InternationalCallsBlockedGenericField(string fieldName,string fieldTitle, string actionType)
        {
            _fieldName = fieldName;
            _fieldTitle = fieldTitle;
            _actionType = actionType;
        }
        public override string Name
        {
            get { return _fieldName; }
        }

        public override string Title
        {
            get { return _fieldTitle; }
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
                if (changeUsersRGsAccountState.ChangesByActionType != null && changeUsersRGsAccountState.ChangesByActionType.ContainsKey(this._actionType))
                   return true;
            }
            return false;
        }
    }
}
