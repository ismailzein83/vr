using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartResidentialProfileDefinition : AccountPartDefinitionSettings
    {
        public override object GetDescription()
        {
            return "Residential Profile";
        }
    }
}
