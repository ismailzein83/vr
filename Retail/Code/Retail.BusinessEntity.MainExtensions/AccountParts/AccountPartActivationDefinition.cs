using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartActivationDefinition : AccountPartDefinitionSettings
    {
        public override Object GetDescription()
        {
            return "Activation";
        }
    }
}
