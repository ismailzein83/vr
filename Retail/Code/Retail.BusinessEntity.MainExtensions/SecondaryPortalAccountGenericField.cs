using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions
{
    public class SecondaryPortalAccountGenericField : AccountGenericField
    {
        public override string Name
        {
            get { return "AllPortalEmails"; }
        }

        public override string Title
        {
            get { return "Portal Emails (All)"; }
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

            List<string> allPortalEmails = new List<string>();
            allPortalEmails.Add(portalAccountSettings.Email);
            if (portalAccountSettings.AdditionalUsers != null)
            {
                foreach(var user in portalAccountSettings.AdditionalUsers)
                {
                    allPortalEmails.Add(user.Email);
                }
            }
            return string.Join(",", allPortalEmails);
        }
    }
}
