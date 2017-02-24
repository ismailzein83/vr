using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationTypeSettingsInfo
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string SearchDirective { get; set; }
        public string BodyDirective { get; set; }
    }
}
