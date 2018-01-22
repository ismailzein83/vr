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
    public class IsAccountProvisioned : AccountGenericField
    {
        public override string Name
        {
            get { return "IsProvisioned"; }
        }

        public override string Title
        {
            get { return "Provisioned"; }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get {
                return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBooleanType();
            }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            var enterpriseAccountMappingInfo = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(context.Account);
            if (enterpriseAccountMappingInfo != null)
            {
                return true;
            }
            var siteAccountMappingInfo = accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(context.Account);
            if (siteAccountMappingInfo != null)
            {
                return true;
            }
            return false;
        }
    }
}
