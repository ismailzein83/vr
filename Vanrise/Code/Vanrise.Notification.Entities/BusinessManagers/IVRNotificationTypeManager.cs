using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationTypeManager : IBusinessManager
    {
        bool DoesUserHaveViewAccess(int userId, List<Guid> VRNotificationTypeIds);

    }
}
