using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data
{
    public interface IVRNotificationDataManager : IDataManager
    {
        void Insert(VRNotification notification);
    }
}
