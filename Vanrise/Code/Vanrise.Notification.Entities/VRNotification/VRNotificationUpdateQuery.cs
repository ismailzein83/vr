using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationUpdateQuery
    {
        public long GreaterThanID { get; set; }
        public int NbOfRows { get; set; }
        public Guid NotificationTypeId { get; set; }
    }
}
