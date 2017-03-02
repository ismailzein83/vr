using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRActionSettings
    {
        public Guid DefinitionId { get; set; }

        public VRActionExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class VRActionExtendedSettings
    {

    }
}
