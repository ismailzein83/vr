using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartPersonalInfoDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("3900317c-b982-4d8b-bd0d-01215ac1f3d9"); } }

    }
}
