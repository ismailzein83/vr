using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public abstract class VRAlertRuleTypeSettings
    {
        public virtual string CriteriaEditor { get; set; }

        public virtual string VRActionExtensionType { get; set; }

        public virtual string CheckerBPActivityFQTN { get; set; }
    }
}
