using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Notification.Business
{
    public class VRNotificationViewFilterContext : IVRNotificationViewFilterContext
    {
        public Guid VRNotificationTypeId
        {
            get;
            set;
        }
    }
}
