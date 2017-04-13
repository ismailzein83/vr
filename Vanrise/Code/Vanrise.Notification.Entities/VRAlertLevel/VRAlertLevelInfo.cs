using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRAlertLevelInfo
    {
        public Guid VRAlertLevelId { get; set; }
        public string Name { get; set; }

        public VRAlertLevelSettings Settings { get; set; }
    }
}
