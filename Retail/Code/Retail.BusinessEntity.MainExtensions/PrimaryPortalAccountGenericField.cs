using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.BusinessEntity.MainExtensions
{
    public class PrimaryPortalAccountGenericField : AccountGenericField
    {
        public override string Name
        {
            get { return "PrimaryPortalEmail"; }
        }

        public override string Title
        {
            get { return "Portal Email (Primary)"; }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get { return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType(); }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            var portalAccountSettings = accountBEManager.GetExtendedSettings<PortalAccountSettings>(context.Account);
            if (portalAccountSettings == null)
                return null;
            return portalAccountSettings.Email;
        }
    }
}
