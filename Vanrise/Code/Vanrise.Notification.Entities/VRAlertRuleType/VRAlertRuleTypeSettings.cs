using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Notification.Entities
{
    public abstract class VRAlertRuleTypeSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual Guid NotificationTypeId { get; set; }

        public virtual string CriteriaEditor { get; set; }

        public virtual string SettingEditor { get; set; }

        public virtual string VRActionExtensionType { get; set; }

        public virtual string CheckerBPActivityFQTN { get; set; }
    }

    public abstract class VRGenericAlertRuleTypeSettings : VRAlertRuleTypeSettings
    {
        public GenericRuleDefinitionCriteria CriteriaDefinition { get; set; }

        public VRObjectVariableCollection Objects { get; set; }
    }
}
