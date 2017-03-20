using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Business
{
    public class SchedulerServiceViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public override bool DoesUserHaveAccess(Security.Entities.IViewUserAccessContext context)
        {
            return new SchedulerTaskActionTypeManager().DoesUserHaveViewAccess(context.UserId);
        }
    }
}
