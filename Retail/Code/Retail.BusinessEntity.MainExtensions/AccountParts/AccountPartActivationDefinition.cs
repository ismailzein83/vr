using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartActivationDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("a982bc4b-89b9-4a84-abae-78b1d1c37941"); } }

        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartActivation part = context.AccountPartSettings.CastWithValidate<AccountPartActivation>("context.AccountPartSettings");
            return true;
        }
    }
}
