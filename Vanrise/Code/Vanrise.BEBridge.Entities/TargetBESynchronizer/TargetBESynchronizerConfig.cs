using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BEBridge.Entities
{
    public class TargetBESynchronizerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_BEBridge_BESynchronizer";
        public string Editor { get; set; }
    }
}
