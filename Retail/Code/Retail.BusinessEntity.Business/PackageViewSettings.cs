using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Retail.BusinessEntity.Business
{
    public class PackageViewSettings : Vanrise.Security.Entities.ViewSettings
    {

        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            return new PackageDefinitionManager().DoesUserHaveViewPackageDefinitions(context.UserId);
        }
    }
     
}
