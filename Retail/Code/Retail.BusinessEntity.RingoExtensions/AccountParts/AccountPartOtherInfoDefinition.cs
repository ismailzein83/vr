using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;

namespace Retail.Ringo.MainExtensions.AccountParts
{
    public class AccountPartOtherInfoDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("D64C95FC-5E4B-46B7-95CD-77082F91B07F"); } }


        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartOtherInfo part = context.AccountPartSettings.CastWithValidate<AccountPartOtherInfo>("context.AccountPartSettings");
            return true;
        }
    }
}
