using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRAlertRuleType
    {
        public Guid VRAlertRuleTypeId { get; set; }

        public string Name { get; set; }

        public VRAlertRuleTypeSettings Settings { get; set; }
    }
}
