using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPViewSettings : Vanrise.Security.Entities.ViewSettings
    {

        public override bool DoesUserHaveAccess(Security.Entities.IViewUserAccessContext context)
        {
            return BusinessManagerFactory.GetManager<IBPDefinitionManager>().DoesUserHaveViewAccess(context.UserId);
        }
    }
}
