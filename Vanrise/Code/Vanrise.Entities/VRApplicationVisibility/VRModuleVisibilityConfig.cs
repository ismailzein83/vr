using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRModuleVisibilityConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VRCommon_ModuleVisibility";

        public string Editor { get; set; }
    }
}
