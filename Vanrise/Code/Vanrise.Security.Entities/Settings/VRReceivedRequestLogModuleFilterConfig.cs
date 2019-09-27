using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Security.Entities.Settings
{
    public class VRReceivedRequestLogModuleFilterConfig: ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Security_ReceivedRequestLog_ModuleFilter";
        public string Editor { get; set; }
    }
}
