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
    public class AccountMappedToTelesGenericField : AccountGenericField
    {
        string _fieldName;
        string _fieldTitle;
        public AccountMappedToTelesGenericField(string fieldName, string fieldTitle)
        {
            _fieldName = fieldName;
            _fieldTitle = fieldTitle;
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
            var enterpriseAccountMappingInfo = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(context.Account);
            if (enterpriseAccountMappingInfo != null)
                return true;
            var siteAccountMappingInfo = accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(context.Account);
            if (siteAccountMappingInfo != null)
                return true;
            var userAccountMappingInfo = accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(context.Account);
            if (userAccountMappingInfo != null)
                return true;
            return false;
        }
    }
}
