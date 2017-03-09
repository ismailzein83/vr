using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRActionDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId { get { return new Guid("D96F17C8-29D7-4C0C-88DC-9D5FBCA2178F"); } }

        public VRActionDefinitionExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class VRActionDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string RuntimeEditor { get; set; }

        public abstract bool IsApplicable(IVRActionDefinitionIsApplicableContext context);
    }

    public interface IVRActionDefinitionIsApplicableContext
    {
        VRActionTargetType Target { get; }
    }

    public class VRActionDefinitionIsApplicableContext : IVRActionDefinitionIsApplicableContext
    {
        public VRActionTargetType Target { get; set; }
    }
}
