using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRApplicationVisibility
    {
        public Guid VRApplicationVisibilityId { get; set; }

        public string Name { get; set; }

        public VRApplicationVisibilitySettings Settings { get; set; }
    }

    public class VRApplicationVisibilitySettings
    {
        public Dictionary<Guid, VRModuleVisibility> ModulesVisibility { get; set; }
    }

    public class VRApplicationVisibilityEditorRuntime
    {
        public VRApplicationVisibility Entity { get; set; }

        public Dictionary<Guid, VRModuleVisibilityEditorRuntime> ModulesVisibilityEditorRuntime { get; set; }

    }
}
